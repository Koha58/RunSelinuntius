using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Playerの動きを管理するクラス
/// </summary>
public class PlayerMove : MonoBehaviour
{
    #region 定数（ゲーム進行・時間制御・移動）

    private const float DefaultNormalTimeScale = 1f;     // 通常時の速度倍率
    private const float ZeroTimeScale = 0f;              // ゲームを完全に停止する時の TimeScale
    private const float MinFixedDeltaTime = 0.002f;      // 最低固定フレームレート
    private const float ZeroSpeedMultiplier = 0f;        // 速度倍率をゼロにする値
    private const float DefaultHorizontalVelocity = 0f;  // 横方向の初期速度（0）

    private const string GameClearSceneName = "GameClearScene"; // ゲームクリアシーンの名前
    private const int TargetFPS = 60;                   // フレームレートの目標値
    private const int VSyncDisabled = 0;                // VSync を無効にする値

    #endregion

    #region SerializeField（インスペクター設定）

    [Header("移動設定")]
    [SerializeField] private Animator animator;        // プレイヤーのアニメーター

    [Header("地面判定設定")]
    [SerializeField] private LayerMask groundLayer;    // 地面のレイヤー
    [SerializeField] private float groundCheckRadius;  // 地面チェックの半径

    [Header("FinalAttackSE設定")]
    [SerializeField] private FinalAttackSEControl seControl; // FinalAttack の SE 制御

    #endregion

    #region JSONから読み込む設定

    private float forwardSpeed;             // 前方向移動速度
    private float moveSpeed;                // 横移動速度
    private float jumpForce;                // ジャンプ力
    private float jumpAnimationDuration;    // ジャンプアニメーションの遅延時間

    private float defaultFixedDeltaTime = 0.0167f;     // デフォルトの FixedDeltaTime
    private float targetSpeedMultiplier;               // Targetの速度を変更する倍率
    private float minSpeedMultiplier;                  // 最終的なプレイヤーの速度倍率
    private float speedMultiplierLerpFactor = 0.1f;    // スムージング係数
    private float slowMotionSpeedMultiplier = 0.1f;    // スローモーション時の倍率

    #endregion

    #region プレイヤー状態

    private float speedMultiplier = 1f;         // 現在の速度倍率
    private float horizontalVelocity;           // 横方向の移動速度
    private Rigidbody playerRigidbody;          // プレイヤーのRigidbody
    private bool isGrounded;                    // 地面にいるかどうか

    private bool isNearTarget = false;          // FinalAttack可能範囲内にいるか
    private bool isFinalAttackSEPlayed = false; // FinalAttackのSEが再生されたか
    private bool isFinalAttackTriggered = false; // FinalAttackが発生したか
    private bool setSlowMotion;                 // スローモーションの設定

    #endregion

    #region ゲームオブジェクト参照

    private GameObject[] targets; // ゲーム内ターゲットオブジェクトの配列

    #endregion

    // 外部から安全に参照できるようにするプロパティ
    public float ForwardSpeed => forwardSpeed;

    /// <summary>
    /// コンポーネントの初期化
    /// </summary>
    private void Awake()
    {
        // Resources/Data/PlayerMoveSettings.json を読み込む（拡張子不要）
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/PlayerMoveSettings");

        if (jsonFile != null)
        {
            // JSONファイルのテキストを取得
            string json = jsonFile.text;

            // JSONを PlayerMoveSettingsExport[] 配列としてデシリアライズ（Newtonsoft.Json使用）
            PlayerMoveSettingsExport[] exports = JsonConvert.DeserializeObject<PlayerMoveSettingsExport[]>(json);

            // 配列が null でなく、1つ以上の要素があれば反映
            if (exports != null && exports.Length > 0)
            {
                // 最初の設定データを使って PlayerMoveSettings に変換
                PlayerMoveSettings settings = exports[0].ToSettings();

                // PlayerMoveSettings の各設定値を、対応する変数に反映

                // 前方移動速度
                forwardSpeed = settings.forwardSpeed;

                // 全体の移動速度
                moveSpeed = settings.moveSpeed;

                // ジャンプ力
                jumpForce = settings.jumpForce;

                // 地面判定の半径
                groundCheckRadius = settings.groundCheckRadius;

                // ジャンプアニメーションの時間
                jumpAnimationDuration = settings.jumpAnimationDuration;

                // 目標速度への倍率調整
                targetSpeedMultiplier = settings.targetSpeedMultiplier;

                // 最低速度倍率
                minSpeedMultiplier = settings.minSpeedMultiplier;

                // 速度倍率の補間係数
                speedMultiplierLerpFactor = settings.speedMultiplierLerpFactor;

                // スローモーション時の速度倍率
                slowMotionSpeedMultiplier = settings.slowMotionSpeedMultiplier;

                // 固定のデルタタイム（デフォルト値）
                defaultFixedDeltaTime = settings.defaultFixedDeltaTime;
            }
            else
            {
                Debug.LogError("JSONのデシリアライズに失敗しました。");
            }
        }
        else
        {
            // JSONファイルが見つからなかった場合のエラーログ
            Debug.LogError("PlayerMoveSettings.json が Resources/Data に見つかりません。");
        }

        // フレームレートとVSyncの設定
        QualitySettings.vSyncCount = VSyncDisabled;
        Application.targetFrameRate = TargetFPS;

        // Rigidbodyコンポーネントを取得
        playerRigidbody = GetComponent<Rigidbody>();

        // FinalAttack可能範囲内にいない
        isNearTarget = false;
        SetSlowMotion(false);

        if (playerRigidbody == null)
        {
            Debug.LogError("Rigidbody が見つかりません！ プレイヤーに Rigidbody コンポーネントをアタッチしてください。");
        }
    }

    /// <summary>
    /// ターゲットを取得
    /// </summary>
    private void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("Target");
    }

    /// <summary>
    /// 移動入力を受け取る
    /// </summary>
    private void OnMove(InputValue value)
    {
        // ジャンプ中は移動を無効化
        if (!isGrounded)
        {
            horizontalVelocity = DefaultHorizontalVelocity;
            return;
        }

        // 左スティックの入力値を取得
        Vector2 axis = value.Get<Vector2>();

        // 横方向の移動速度を保持
        horizontalVelocity = axis.x;
    }

    /// <summary>
    /// ジャンプ入力を受け取る
    /// </summary>
    private void OnJump(InputValue value)
    {
        // ジャンプボタンが押されていて、かつ地面にいる場合のみジャンプを実行
        if (isGrounded && value.isPressed)
        {
            // アニメーションをジャンプ状態にする
            if (animator != null)
            {
                animator.SetBool("Jump", true);
            }

            // Rigidbody にジャンプの力を加える
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // ジャンプ中は地面にいない状態に設定
            isGrounded = false;

            // 一定時間後にジャンプアニメーションを終了する
            StartCoroutine(SetJumpFalseAfterDelay(jumpAnimationDuration));
        }
    }

    /// <summary>
    /// 攻撃入力を受け取る
    /// </summary>
    private void OnFinalAttack(InputValue value)
    {
        // FinalAttack範囲内かつ、ボタンが押された場合のみ攻撃を実行
        if (isNearTarget && value.isPressed)
        {
            // すでにSEが再生されていたら処理をスキップ
            if (isFinalAttackSEPlayed)
            {
                return;
            }

            Debug.Log("Final Attack!");

            // SE を鳴らし、その長さを取得
            float seDuration = seControl != null ? seControl.PlayFinalAttackSE() : 0f;

            // SEが再生されたことを記録
            isFinalAttackSEPlayed = true;

            isFinalAttackTriggered = true; // FinalAttack発生フラグをON

            Time.timeScale = DefaultNormalTimeScale;

            // SE が鳴り終わるまで待ってからシーン遷移
            StartCoroutine(WaitAndLoadScene(seDuration));
        }
    }

    /// <summary>
    /// 毎フレーム呼び出される処理
    /// 横方向と前方向に移動させる
    /// </summary>
    private void Update()
    {
        // 横方向と前方向に移動
        Vector3 movement = new Vector3(horizontalVelocity * moveSpeed * speedMultiplier, 0, forwardSpeed * speedMultiplier) * Time.deltaTime;
        transform.position += movement;
    }

    /// <summary>
    /// 一定間隔で呼び出される物理計算
    /// 地面にいるかどうかを判定
    /// </summary>
    private void FixedUpdate()
    {
        // 地面にいるかどうかを確認
        isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);
    }

    /// <summary>
    /// 一定時間後にジャンプアニメーションを終了し、地面にいる状態に戻す
    /// </summary>
    private IEnumerator SetJumpFalseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ジャンプアニメーションを終了
        if (animator != null)
        {
            animator.SetBool("Jump", false);
        }

        // 地面にいる状態に設定
        isGrounded = true;
    }

    /// <summary>
    /// 速度倍率を設定する
    /// </summary>
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    /// <summary>
    /// 現在の速度を取得
    /// </summary>
    public float GetCurrentSpeed()
    {
        return moveSpeed * speedMultiplier;
    }

    /// <summary>
    /// FinalAttack が可能かを判定するメソッド
    /// </summary>
    internal bool IsFinalAttackPossible()
    {
        return isNearTarget;
    }

    /// <summary>
    /// Target に触れたら(FinalAttack可能なら)ゲームの動きを遅くする
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter: {other.gameObject.name}");

        if (other.CompareTag("Target"))
        {
            isNearTarget = true;
            SetSlowMotion(true);
        }

        if (other.CompareTag("RunAway") && !isFinalAttackTriggered)
        {
            isNearTarget = false;
            SetSlowMotion(false);

            // runAway の SE を鳴らす
            if (seControl != null)
            {
                seControl.PlayrunAwaySE();
            }

            foreach (GameObject target in targets)
            {
                TargetMove targetMove = target.GetComponent<TargetMove>();
                if (targetMove != null)
                {
                    targetMove.SetSpeedMultiplier(targetSpeedMultiplier);
                }
            }
        }
    }

    /// <summary>
    /// シーン全体のスローモーションを設定する
    /// </summary>
    private void SetSlowMotion(bool isSlow)
    {
        setSlowMotion = isSlow;

        if (isSlow)
        {
            // Time.timeScale を更に小さくして時間全体を遅くする
            Time.timeScale = slowMotionSpeedMultiplier; // ここで時間の流れを遅くする
            Time.fixedDeltaTime = Mathf.Max(defaultFixedDeltaTime * Time.timeScale, MinFixedDeltaTime); // 固定フレームレートの調整

            // speedMultiplier をさらに低く設定して、プレイヤーの動きを遅くする
            speedMultiplier = Mathf.Lerp(speedMultiplier, minSpeedMultiplier, speedMultiplierLerpFactor); // より遅くなるように調整
        }
        else
        {
            // 通常速度に戻す
            Time.timeScale = DefaultNormalTimeScale;
            Time.fixedDeltaTime = defaultFixedDeltaTime;
            speedMultiplier = DefaultNormalTimeScale;
        }
    }

    /// <summary>
    /// スローモーションが有効かどうかを確認する
    /// </summary>
    /// <returns>
    /// スローモーションが有効であれば true、それ以外は false を返す
    /// </returns>
    public bool IsSlowMotionEnabled()
    {
        return setSlowMotion;
    }

    /// <summary>
    /// 指定された時間待機した後、指定のシーンに遷移するコルーチン
    /// </summary>
    private IEnumerator WaitAndLoadScene(float waitTime)
    {
        // ゲーム全体を停止（物理計算やUpdate処理も含める）
        Time.timeScale = ZeroTimeScale;

        // プレイヤーとターゲットの動きを完全に止める
        StopPlayerMovement();
        StopTargetMovement();

        // SE の再生時間をリアルタイムで待つ（UnscaledTime を使用）
        yield return new WaitForSecondsRealtime(waitTime);

        // シーン遷移（Time.timeScale を戻さずに遷移する）
        SceneManager.LoadScene(GameClearSceneName);
    }

    /// <summary>
    /// プレイヤーの動きを止める
    /// </summary>
    private void StopPlayerMovement()
    {
        speedMultiplier = ZeroSpeedMultiplier;
        playerRigidbody.velocity = Vector3.zero; // 物理的な動きを止める
    }

    /// <summary>
    /// シーン内のすべての Target の動きを止める
    /// </summary>
    private void StopTargetMovement()
    {
        foreach (GameObject target in targets)
        {
            TargetMove targetMove = target.GetComponent<TargetMove>();
            if (targetMove != null)
            {
                targetMove.SetSpeedMultiplier(ZeroSpeedMultiplier);
            }
        }
    }
}

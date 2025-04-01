using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Playerの動きを管理するクラス
/// </summary>
public class PlayerMove : MonoBehaviour
{
    // 定数の宣言
    private const float DefaultForwardSpeed = 5f;  // 前方向移動速度
    private const float DefaultMoveSpeed = 5f;     // 横移動速度
    private const float DefaultJumpForce = 7f;     // ジャンプ力
    private const float DefaultGroundCheckRadius = 0.1f; // 地面チェックの半径
    private const float DefaultJumpAnimationDuration = 0.8f; // ジャンプアニメーション終了までの遅延時間
    private const float DefaultNormalTimeScale = 1f; // 通常時の速度倍率
    private const float DefaultFixedDeltaTime = 0.0167f; // デフォルトの FixedDeltaTime
    private const float TargetSpeedMultiplier = 5f; // Targetの速度を変更する倍率
    private const float MinFixedDeltaTime = 0.002f; // 最低固定フレームレート
    private const float MinSpeedMultiplier = 0.5f; // 最終的なプレイヤーの速度倍率
    private const float SpeedMultiplierLerpFactor = 0.1f; // スムージング係数
    private const float ZeroSpeedMultiplier = 0f; // 速度倍率をゼロにする値
    private const float DefaultHorizontalVelocity = 0f; // 横方向の初期速度（0）

    private const string GameClearSceneName = "GameClearScene";  // ゲームクリアシーンの名前
    private const float ZeroTimeScale = 0f; // ゲームを完全に停止する時の TimeScale
    private const float SlowMotionSpeedMultiplier = 0.1f; // スローモーション時の倍率

    [Header("移動設定")]
    [SerializeField] private Animator animator;        // プレイヤーのアニメーター
    [SerializeField] private float forwardSpeed = DefaultForwardSpeed;  // 前方向移動速度
    [SerializeField] private float moveSpeed = DefaultMoveSpeed;     // 横移動速度
    [SerializeField] private float jumpForce = DefaultJumpForce;     // ジャンプ力
    [SerializeField] private LayerMask groundLayer;    // 地面のレイヤー

    [Header("地面判定設定")]
    [SerializeField] private float groundCheckRadius = DefaultGroundCheckRadius; // 地面チェックの半径

    [Header("アニメーション設定")]
    [SerializeField] private float jumpAnimationDuration = DefaultJumpAnimationDuration; // ジャンプアニメーション終了までの遅延時間

    [Header("FinalAttackSE設定")]
    [SerializeField] private FinalAttackSEControl seControl; // FinalAttack の SE 制御

    private float speedMultiplier = 1f;               // 速度倍率
    private float horizontalVelocity;                 // 横方向の移動速度
    private Rigidbody playerRigidbody;               // プレイヤーのRigidbody
    private bool isGrounded;                          // 地面にいるかどうか

    private bool isNearTarget = false; // FinalAttack可能範囲内にいるか
    private bool isFinalAttackSEPlayed = false; // FinalAttackのSEが再生されたかどうかを管理

    private const int TargetFPS = 60;  // フレームレートの目標値
    private const int VSyncDisabled = 0; // VSync を無効にする値

    private GameObject[] targets;　// Targetオブジェクトの配列。ゲーム内でターゲットとなるオブジェクトを格納
    private bool setSlowMotion;　  // スローモーションの設定を保持するフラグ。スローモーションが有効かどうかを制御
    private bool isFinalAttackTriggered = false; // FinalAttackが発生したかどうか

    /// <summary>
    /// コンポーネントの初期化
    /// </summary>
    private void Awake()
    {
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
                    targetMove.SetSpeedMultiplier(TargetSpeedMultiplier);
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
            Time.timeScale = SlowMotionSpeedMultiplier; // ここで時間の流れを遅くする
            Time.fixedDeltaTime = Mathf.Max(DefaultFixedDeltaTime * Time.timeScale, MinFixedDeltaTime); // 固定フレームレートの調整

            // speedMultiplier をさらに低く設定して、プレイヤーの動きを遅くする
            speedMultiplier = Mathf.Lerp(speedMultiplier, MinSpeedMultiplier, SpeedMultiplierLerpFactor); // より遅くなるように調整
        }
        else
        {
            // 通常速度に戻す
            Time.timeScale = DefaultNormalTimeScale;
            Time.fixedDeltaTime = DefaultFixedDeltaTime;
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

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤーの動きを制御するクラス
/// </summary>
public class PlayerMove : MonoBehaviour
{
    // 定数の宣言
    private const float DefaultForwardSpeed = 5f;  // 前方向移動速度
    private const float DefaultMoveSpeed = 5f;     // 横移動速度
    private const float DefaultJumpForce = 7f;     // ジャンプ力
    private const float DefaultGroundCheckRadius = 0.1f; // 地面チェックの半径
    private const float DefaultJumpAnimationDuration = 0.8f; // ジャンプアニメーション終了までの遅延時間
    private const float DefaultSlowMotionScale = 0.01f; // スローモーション時の速度倍率
    private const float DefaultNormalTimeScale = 1f; // 通常時の速度倍率
    private const float DefaultFixedDeltaTime = 0.02f; // デフォルトの FixedDeltaTime
    private const float TargetSpeedMultiplier = 5f; // Targetの速度を変更する倍率

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

    [Header("時間操作設定")]
    [SerializeField] private float slowMotionScale = DefaultSlowMotionScale; // スローモーション時の速度倍率
    [SerializeField] private float normalTimeScale = DefaultNormalTimeScale; // 通常時の速度倍率
    [SerializeField] private float defaultFixedDeltaTime = DefaultFixedDeltaTime; // デフォルトの FixedDeltaTime

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
    /// 移動入力を受け取る
    /// </summary>
    /// <param name="value">入力値 (Vector2)</param>
    private void OnMove(InputValue value)
    {
        // ジャンプ中は移動を無効化
        if (!isGrounded)
        {
            horizontalVelocity = 0;
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
    /// <param name="value">入力値 (isPressed)</param>
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
    /// <param name="value">入力値 (isPressed)</param>
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

            // プレイヤーの動きを止める
            StopPlayerMovement();

            // ターゲットの動きを止める
            StopTargetMovement();

            // SE が鳴り終わるまで待ってからシーン遷移
            StartCoroutine(WaitAndLoadScene(seDuration));
        }
    }

    /// <summary>
    /// プレイヤーの動きを止める
    /// </summary>
    private void StopPlayerMovement()
    {
        speedMultiplier = 0f;
        playerRigidbody.velocity = Vector3.zero; // 物理的な動きを止める
    }

    /// <summary>
    /// シーン内のすべての Target の動きを止める
    /// </summary>
    private void StopTargetMovement()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject target in targets)
        {
            TargetMove targetMove = target.GetComponent<TargetMove>();
            if (targetMove != null)
            {
                targetMove.SetSpeedMultiplier(0f); // ターゲットの移動速度を0にする
            }
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
    /// 地面チェック用の可視化処理
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }

    /// <summary>
    /// 一定時間後にジャンプアニメーションを終了し、地面にいる状態に戻す
    /// </summary>
    /// <param name="delay">遅延時間 (秒)</param>
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

        if (other.CompareTag("RunAway"))
        {
            isNearTarget = false;
            SetSlowMotion(false);

            // runAway の SE を鳴らす
            if (seControl != null)
            {
                seControl.PlayrunAwaySE();
            }

            // シーン内のすべての "Target" タグがついたオブジェクトを取得し、それらの速度を変更
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
            foreach (GameObject target in targets)
            {
                TargetMove targetMove = target.GetComponent<TargetMove>();
                if (targetMove != null)
                {
                    targetMove.SetSpeedMultiplier(TargetSpeedMultiplier); // 速度を変更
                }
            }
        }
    }

    /// <summary>
    /// シーン全体のスローモーションを設定する
    /// </summary>
    /// <param name="isSlow">trueなら遅く、falseなら通常速度</param>
    private void SetSlowMotion(bool isSlow)
    {
        if (isSlow)
        {
            Time.timeScale = slowMotionScale;
            Time.fixedDeltaTime = defaultFixedDeltaTime * slowMotionScale;
            speedMultiplier = slowMotionScale;
        }
        else
        {
            Time.timeScale = normalTimeScale;
            Time.fixedDeltaTime = defaultFixedDeltaTime;
            speedMultiplier = normalTimeScale;
        }
    }

    /// <summary>
    /// 指定された時間待機した後、指定のシーンに遷移するコルーチン
    /// </summary>
    /// <param name="waitTime">シーン遷移前に待機する時間（秒）</param>
    private IEnumerator WaitAndLoadScene(float waitTime)
    {
        // 指定された時間だけ待機（FinalAttack の SE の長さを想定）
        yield return new WaitForSeconds(waitTime);

        // SE の再生が完了した後に、GameClearScene へ遷移
        SceneManager.LoadScene("GameClearScene");
    }
}
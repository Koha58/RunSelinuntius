using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの動きを制御するクラス
/// </summary>
public class PlayerMove : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private Animator animator;        // プレイヤーのアニメーター
    [SerializeField] private float forwardSpeed = 5f;  // 前方向移動速度
    [SerializeField] private float moveSpeed = 5f;     // 横移動速度
    [SerializeField] private float jumpForce = 7f;     // ジャンプ力
    [SerializeField] private LayerMask groundLayer;    // 地面のレイヤー

    [Header("地面判定設定")]
    [SerializeField] private float groundCheckRadius = 0.1f; // 地面チェックの半径

    [Header("アニメーション設定")]
    [SerializeField] private float jumpAnimationDuration = 0.8f; // ジャンプアニメーション終了までの遅延時間

    private float horizontalVelocity;                 // 横方向の移動速度
    private Rigidbody playerRigidbody;               // プレイヤーのRigidbody
    private bool isGrounded;                          // 地面にいるかどうか

    /// <summary>
    /// コンポーネントの初期化
    /// </summary>
    private void Awake()
    {
        // Rigidbodyコンポーネントを取得
        playerRigidbody = GetComponent<Rigidbody>();
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
    /// 毎フレーム呼び出される処理
    /// 横方向と前方向に移動させる
    /// </summary>
    private void Update()
    {
        // 横方向と前方向に移動
        Vector3 movement = new Vector3(horizontalVelocity * moveSpeed, 0, forwardSpeed) * Time.deltaTime;
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
}

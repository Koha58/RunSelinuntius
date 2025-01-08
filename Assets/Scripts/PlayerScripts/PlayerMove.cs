using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの動きを制御するクラス
/// </summary>
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Animator _animator;        // プレイヤーのアニメーター
    [SerializeField] private float _forwardSpeed = 3f;  // 前方向移動速度
    [SerializeField] private float _moveSpeed = 5f;     // 横移動速度
    [SerializeField] private float _jumpForce = 5f;     // ジャンプ力
    [SerializeField] private LayerMask _groundLayer;    // 地面のレイヤー

    private float _horizontalVelocity;                 // 横方向の移動速度
    private Rigidbody _rigidbody;                      // プレイヤーのRigidbody
    private bool _isGrounded;                          // 地面にいるかどうか

    /// <summary>
    /// コンポーネントの初期化
    /// </summary>
    private void Awake()
    {
        // Rigidbodyコンポーネントを取得
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
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
        _horizontalVelocity = axis.x;
    }

    /// <summary>
    /// ジャンプ入力を受け取る
    /// </summary>
    /// <param name="value">入力値 (isPressed)</param>
    private void OnJump(InputValue value)
    {
        // ジャンプボタンが押されていて、かつ地面にいる場合のみジャンプを実行
        if (_isGrounded && value.isPressed)
        {
            // アニメーションをジャンプ状態にする
            if (_animator != null)
            {
                _animator.SetBool("Jump", true);
            }

            // Rigidbody にジャンプの力を加える
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

            // ジャンプ中は地面にいない状態に設定
            _isGrounded = false;

            // 一定時間後にジャンプアニメーションを終了する
            StartCoroutine(SetJumpFalseAfterDelay(0.8f));
        }
    }

    /// <summary>
    /// 毎フレーム呼び出される処理
    /// 横方向と前方向に移動させる
    /// </summary>
    private void Update()
    {
        // 横方向と前方向に移動
        Vector3 movement = new Vector3(_horizontalVelocity * _moveSpeed, 0, _forwardSpeed) * Time.deltaTime;
        transform.position += movement;
    }

    /// <summary>
    /// 一定間隔で呼び出される物理計算
    /// 地面にいるかどうかを判定
    /// </summary>
    private void FixedUpdate()
    {
        // 地面にいるかどうかを確認
        _isGrounded = Physics.CheckSphere(transform.position, 0.1f, _groundLayer);
    }

    /// <summary>
    /// 地面チェック用の可視化処理
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }

    /// <summary>
    /// 一定時間後にジャンプアニメーションを終了し、地面にいる状態に戻す
    /// </summary>
    /// <param name="delay">遅延時間 (秒)</param>
    private IEnumerator SetJumpFalseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ジャンプアニメーションを終了
        if (_animator != null)
        {
            _animator.SetBool("Jump", false);
        }

        // 地面にいる状態に設定
        _isGrounded = true;
    }
}

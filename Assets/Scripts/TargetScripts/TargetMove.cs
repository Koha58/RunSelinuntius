using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ターゲットの動きを制御するクラス
/// </summary>
public class TargetMove : MonoBehaviour
{
    [Header("Targetの基本設定")]
    [SerializeField] private Transform player; // プレイヤーのTransform
    [SerializeField] private float baseSpeed = 10.0f; // Targetの基準速度
    [SerializeField] private float targetDistance = 1000.0f; // プレイヤーとの目標距離

    [Header("左右移動設定")]
    [SerializeField] private float lateralMovementAmplitude = 5.0f; // 左右移動の振幅
    [SerializeField] private float lateralMovementFrequency = 0.1f; // 左右移動の頻度
    [SerializeField] private float lateralMoveActiveDuration = 5.0f; // 左右移動を行う時間（秒）
    [SerializeField] private float lateralMoveIdleDuration = 10.0f; // 左右移動を行わない時間（秒）

    private const float TwoPi = Mathf.PI * 2.0f; // 2πの定数（左右移動計算に使用）

    private float lateralMovementTimer = 0.0f; // 左右移動のタイマー
    private bool isLateralMoving = false; // 現在左右移動中かどうか
    private float stateTimer = 0.0f; // 状態変更のタイマー

    private Vector3 initialPosition; // 初期位置を記録

    private float currentSpeedMultiplier = 1.0f; // 現在の速度倍率（通常は1）

    private float originalSpeed; // 初期のSpeedを保存

    [Header("runAway後のPlayerとの距離")]
    [SerializeField] private float distanceThreshold = 500f; // プレイヤーとの距離閾値

    /// <summary>
    /// 初期設定を行う
    /// プレイヤーが設定されていない場合、エラーメッセージを表示する
    /// 初期位置をプレイヤーの前方に設定
    /// </summary>
    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Playerが設定されていません！");
            return;
        }

        // 初期位置をプレイヤーの前方に配置
        initialPosition = player.position + Vector3.forward * targetDistance;
        transform.position = initialPosition;

        originalSpeed = baseSpeed;  // 初期速度を保存
    }

    /// <summary>
    /// 毎フレーム呼び出される処理
    /// Targetの位置を更新する
    /// </summary>
    void Update()
    {
        if (player == null) return;

        // プレイヤーの前方にターゲットを移動させる
        MaintainPositionInFrontOfPlayer();

        // 左右移動を管理
        ManageLateralMovement();

        // プレイヤーとの距離をチェック
        CheckDistanceToPlayer();
    }

    /// <summary>
    /// プレイヤーの前方にターゲットを維持
    /// プレイヤーの移動に合わせてターゲットが前方に進む
    /// </summary>
    private void MaintainPositionInFrontOfPlayer()
    {
        // プレイヤーの移動方向を取得
        Vector3 forwardDirection = player.forward;

        // プレイヤーの移動に合わせてターゲットが前方に進む
        Vector3 targetPosition = player.position + forwardDirection * targetDistance;

        // 現在の位置から目標位置に向かって一定速度で移動
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, baseSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 左右移動の状態を管理
    /// </summary>
    private void ManageLateralMovement()
    {
        if (isLateralMoving)
        {
            lateralMovementTimer += Time.deltaTime * lateralMovementFrequency * TwoPi;
            float lateralOffset = Mathf.Sin(lateralMovementTimer) * lateralMovementAmplitude;
            transform.position = new Vector3(initialPosition.x + lateralOffset, transform.position.y, transform.position.z); // 初期X位置を基準に左右移動

            // 左右移動の継続時間を超えたら終了
            stateTimer += Time.deltaTime;
            if (stateTimer >= lateralMoveActiveDuration)
            {
                stateTimer = 0.0f;
                isLateralMoving = false;
            }
        }
        else
        {
            // 左右移動を行わない時間をカウント
            stateTimer += Time.deltaTime;
            if (stateTimer >= lateralMoveIdleDuration)
            {
                stateTimer = 0.0f;
                isLateralMoving = true; // 左右移動を開始
                lateralMovementTimer = 0.0f; // 左右移動タイマーをリセット
            }
        }
    }

    /// <summary>
    /// プレイヤーとターゲットとの距離をチェック
    /// </summary>
    private void CheckDistanceToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // プレイヤーとの距離がdistanceThreshold以上の場合、元の速度に戻す
        if (distanceToPlayer > distanceThreshold && currentSpeedMultiplier != 1f)
        {
            currentSpeedMultiplier = 1f;
            baseSpeed = originalSpeed;  // 速度を元に戻す
        }
    }

    /// <summary>
    /// FinalAttack範囲に近づいた場合、速度を変更する処理
    /// </summary>
    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = multiplier; // 速度倍率を設定
        baseSpeed = originalSpeed * multiplier; // 新しい速度を適用
    }
}

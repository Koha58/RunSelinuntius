using System;

/// <summary>
/// プレイヤーの移動や挙動に関する設定値を保持するクラス
/// （外部JSONから読み込まれるデータ構造）
/// </summary>
[Serializable]
public class PlayerMoveSettings
{
    /// <summary>
    /// 前方への移動速度
    /// </summary>
    public float forwardSpeed;

    /// <summary>
    /// 横方向の移動速度
    /// </summary>
    public float moveSpeed;

    /// <summary>
    /// ジャンプ時の上方向の力
    /// </summary>
    public float jumpForce;

    /// <summary>
    /// 地面との接地判定に使用する円の半径
    /// </summary>
    public float groundCheckRadius;

    /// <summary>
    /// ジャンプアニメーションが終了するまでの時間
    /// </summary>
    public float jumpAnimationDuration;

    /// <summary>
    /// ターゲットに近づいた時に適用する移動速度の倍率
    /// </summary>
    public float targetSpeedMultiplier;

    /// <summary>
    /// 移動速度の最小倍率（スローモーション時など）
    /// </summary>
    public float minSpeedMultiplier;

    /// <summary>
    /// 移動速度の補間に使うスムージング係数
    /// </summary>
    public float speedMultiplierLerpFactor;

    /// <summary>
    /// スローモーション中に適用する移動速度倍率
    /// </summary>
    public float slowMotionSpeedMultiplier;

    /// <summary>
    /// スローモーションに入る前の Time.fixedDeltaTime（復帰用に保持）
    /// </summary>
    public float defaultFixedDeltaTime;
}

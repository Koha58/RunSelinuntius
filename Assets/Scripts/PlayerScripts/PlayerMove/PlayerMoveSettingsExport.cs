using System;

/// <summary>
/// Excel → JSON 変換で出力されたデータを受け取るための中間クラス。
/// Unity用の実際の設定データ（PlayerMoveSettings）に変換する役割を持つ。
/// </summary>
[Serializable]
public class PlayerMoveSettingsExport
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
    /// ターゲットに近づいたときの速度倍率
    /// </summary>
    public float targetSpeedMultiplier;

    /// <summary>
    /// 最小速度倍率（減速の下限値など）
    /// </summary>
    public float minSpeedMultiplier;

    /// <summary>
    /// 速度補間に使用するスムージング係数
    /// </summary>
    public float speedMultiplierLerpFactor;

    /// <summary>
    /// スローモーション時の速度倍率
    /// </summary>
    public float slowMotionSpeedMultiplier;

    /// <summary>
    /// 通常時の FixedDeltaTime（スローモーション復帰用）
    /// </summary>
    public float defaultFixedDeltaTime;

    /// <summary>
    /// Exportデータから、ゲーム内で使用する PlayerMoveSettings に変換する
    /// </summary>
    /// <returns>PlayerMoveSettings オブジェクト</returns>
    public PlayerMoveSettings ToSettings()
    {
        return new PlayerMoveSettings
        {
            // 前方への移動速度
            forwardSpeed = forwardSpeed,

            // 横方向の移動速度
            moveSpeed = moveSpeed,

            // ジャンプ時の上方向の力
            jumpForce = jumpForce,

            // 地面との接地判定に使用する半径
            groundCheckRadius = groundCheckRadius,

            // ジャンプアニメーションの継続時間
            jumpAnimationDuration = jumpAnimationDuration,

            // ターゲットに近づいたときの速度倍率
            targetSpeedMultiplier = targetSpeedMultiplier,

            // 最小速度倍率（スローモーション下限など）
            minSpeedMultiplier = minSpeedMultiplier,

            // 速度補間に使用するスムージング係数
            speedMultiplierLerpFactor = speedMultiplierLerpFactor,

            // スローモーション中の速度倍率
            slowMotionSpeedMultiplier = slowMotionSpeedMultiplier,

            // 通常時の FixedDeltaTime（スローモーション復帰用）
            defaultFixedDeltaTime = defaultFixedDeltaTime
        };
    }
}

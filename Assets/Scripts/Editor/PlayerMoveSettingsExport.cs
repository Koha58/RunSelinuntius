using System;

/// <summary>
/// プレイヤーの移動設定をJSON化する際のエクスポート用クラス
/// </summary>
[Serializable]
public class PlayerMoveSettingsExport
{
    // 移動前方向速度（文字列形式、小数点以下3桁）
    public string forwardSpeed;

    // 横移動速度（文字列形式、小数点以下3桁）
    public string moveSpeed;

    // ジャンプ力（文字列形式、小数点以下3桁）
    public string jumpForce;

    // 地面判定用の半径（文字列形式、小数点以下3桁）
    public string groundCheckRadius;

    // ジャンプアニメーションの継続時間（文字列形式、小数点以下3桁）
    public string jumpAnimationDuration;

    /// <summary>
    /// PlayerMoveSettingsクラスのデータを受け取り、
    /// 小数点以下3桁で文字列化してフィールドにセットするコンストラクタ
    /// </summary>
    /// <param name="data">PlayerMoveSettingsのインスタンス</param>
    public PlayerMoveSettingsExport(PlayerMoveSettings data)
    {
        // 小数点以下3桁で文字列化して代入
        forwardSpeed = data.forwardSpeed.ToString("0.###");
        moveSpeed = data.moveSpeed.ToString("0.###");
        jumpForce = data.jumpForce.ToString("0.###");
        groundCheckRadius = data.groundCheckRadius.ToString("0.###");
        jumpAnimationDuration = data.jumpAnimationDuration.ToString("0.###");
    }

    public PlayerMoveSettings ToSettings()
    {
        return new PlayerMoveSettings
        {
            forwardSpeed = float.Parse(forwardSpeed),
            moveSpeed = float.Parse(moveSpeed),
            jumpForce = float.Parse(jumpForce),
            groundCheckRadius = float.Parse(groundCheckRadius),
            jumpAnimationDuration = float.Parse(jumpAnimationDuration),
        };
    }

}

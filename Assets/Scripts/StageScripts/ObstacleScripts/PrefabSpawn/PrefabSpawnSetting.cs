using System;

/// <summary>
/// 障害物Prefabの生成に関する設定情報（JSONで読み込まれる）
/// </summary>
[System.Serializable]
public class PrefabSpawnSetting
{
    /// <summary>設定の名前（識別用・デバッグ用途）</summary>
    public string name;

    /// <summary>Resources.Loadで読み込むPrefabのパス</summary>
    public string prefabPath;

    /// <summary>生成位置のY座標（高さ）</summary>
    public float yOffset;

    /// <summary>プレイヤー位置からのZ方向オフセット</summary>
    public float zOffset;

    /// <summary>基本となる生成間隔（秒）</summary>
    public float baseSpawnInterval;

    /// <summary>HP最大時に乗算される生成間隔の倍率（間隔が長くなる）</summary>
    public float maxSpawnMultiplier;

    /// <summary>HP最小時に乗算される生成間隔の倍率（間隔が短くなる）</summary>
    public float minSpawnMultiplier;

    /// <summary>プレイヤーとの基本距離（未使用の場合は将来の拡張用）</summary>
    public float distanceFromPlayer;

    /// <summary>X方向の最小オフセット（プレイヤー初期位置から）</summary>
    public float minXOffset;

    /// <summary>X方向の最大オフセット（minXOffset〜maxXOffsetでランダム）</summary>
    public float maxXOffset;

    /// <summary>Y方向の追加オフセット（使用していない場合は整理検討）</summary>
    public float spawnYOffset;

    /// <summary>生成されたオブジェクトが削除（返却）されるまでの時間（秒）</summary>
    public float destroyDelay;
}

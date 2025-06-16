using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// JSONファイルからPrefabの生成設定を読み込むコンポーネント
/// </summary>
public class SpawnSettingsLoader : MonoBehaviour
{
    /// <summary>
    /// 設定を記述したJSONファイル（TextAssetとしてInspectorから指定）
    /// </summary>
    [SerializeField] private TextAsset jsonFile;

    /// <summary>
    /// 読み込んだPrefab生成設定のリスト
    /// </summary>
    public List<PrefabSpawnSetting> spawnSettings;

    /// <summary>
    /// ゲーム開始前（Awake）にJSONを読み込み、設定リストに格納する
    /// </summary>
    private void Awake()
    {
        // JSONファイルの内容をList<PrefabSpawnSetting>に変換（デシリアライズ）
        spawnSettings = JsonConvert.DeserializeObject<List<PrefabSpawnSetting>>(jsonFile.text);

        // 確認用ログ：最初の要素が存在する場合は名前を表示
        if (spawnSettings != null && spawnSettings.Count > 0)
        {
            Debug.Log($"1番目のPrefab: {spawnSettings[0].name}");
        }
    }
}
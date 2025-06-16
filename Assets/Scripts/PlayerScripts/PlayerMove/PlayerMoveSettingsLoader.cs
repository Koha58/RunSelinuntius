using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Resourcesフォルダ内のJSONファイルからPlayerMoveSettingsを読み込むユーティリティクラス
/// </summary>
public static class PlayerMoveSettingsLoader
{
    // JSONファイルのパス（Resourcesフォルダ内の相対パス、拡張子不要）
    private const string FileName = "Data/PlayerMoveSettings"; // Resources/Data/PlayerMoveSettings.json

    /// <summary>
    /// JSONファイルを読み込み、PlayerMoveSettingsを返す
    /// </summary>
    /// <returns>読み込んだPlayerMoveSettingsインスタンス。読み込み失敗時はnull。</returns>
    public static PlayerMoveSettings Load()
    {
        // ResourcesからTextAssetとしてJSONファイルを読み込む
        TextAsset jsonFile = Resources.Load<TextAsset>(FileName);
        if (jsonFile == null)
        {
            Debug.LogError($"PlayerMoveSettingsLoader: JSONファイルが見つかりません。Resources/{FileName}.json");
            return null;
        }

        // Newtonsoft.Jsonを使いJSON文字列をPlayerMoveSettingsExport配列にデシリアライズする
        PlayerMoveSettingsExport[] exports = JsonConvert.DeserializeObject<PlayerMoveSettingsExport[]>(jsonFile.text);
        if (exports == null || exports.Length == 0)
        {
            Debug.LogError("PlayerMoveSettingsLoader: JSONに有効なデータが含まれていません。");
            return null;
        }

        // 配列の先頭要素をPlayerMoveSettings型に変換して返す
        return exports[0].ToSettings();
    }
}
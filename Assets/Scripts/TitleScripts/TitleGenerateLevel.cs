using System.Collections;
using UnityEngine;

/// <summary>
/// タイトルシーンのランエリアを自動生成するクラス（外部JSONでPrefab管理）
/// </summary>
public class TitleGenerateLevel : MonoBehaviour
{
    // 初期設定・定数的な値

    // 最初にPrefabを生成するX座標の位置
    private const float InitialXPos = 2.0f;

    // 最初にPrefabを生成するZ座標の位置
    private const float InitialZPos = 700f;

    // 各Prefab間のZ方向の間隔
    private const float LevelSpacing = 98.0f;

    // プレイヤー速度に基づく生成間隔の基準値
    private const float GenerationDelay = 30.0f;

    // ステージJSONファイル名（拡張子不要）
    private string stageFileName = "stage01";

    // 現在の生成座標
    private float xPos;
    private float zPos;

    // 生成状態
    private bool creatingLevel;

    // JSONから読み込まれるプレハブ名の配列
    private string[] prefabNames;


    void Start()
    {
        // 初期位置を設定
        xPos = InitialXPos;
        zPos = InitialZPos;

        // JSONファイルからPrefab名リストを読み込む
        LoadPrefabList();
    }

    void Update()
    {
        // レベル生成中でなく、Prefab名リストが読み込まれている場合にレベル生成を開始
        if (!creatingLevel && prefabNames != null && prefabNames.Length > 0)
        {
            creatingLevel = true;
            StartCoroutine(GenerateLvl());
        }
    }

    /// <summary>
    /// JSONファイルからPrefab名のリストを読み込む
    /// </summary>
    void LoadPrefabList()
    {
        // Resourcesフォルダ内の指定パスからJSONテキストを読み込む
        TextAsset json = Resources.Load<TextAsset>($"Stages/{stageFileName}");

        if (json == null)
        {
            // JSONファイルが見つからなかった場合はエラーログを表示して処理を中断
            Debug.LogError($"ステージファイルが見つかりません: {stageFileName}");
            return;
        }

        // JSON形式のテキストをPrefabリストのデータ形式に変換して保持
        PrefabListData data = JsonUtility.FromJson<PrefabListData>(json.text);
        prefabNames = data.prefabs;
    }

    /// <summary>
    /// 新しいレベルを生成する処理（コルーチン）
    /// </summary>
    IEnumerator GenerateLvl()
    {
        // prefabNames配列からランダムにPrefab名を選択
        int index = Random.Range(0, prefabNames.Length);
        string prefabName = prefabNames[index];

        // Resources/PrefabsフォルダからPrefabを読み込む
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");

        if (prefab != null)
        {
            // Prefabを指定位置に生成
            Instantiate(prefab, new Vector3(xPos, 0, zPos), Quaternion.identity);
        }
        else
        {
            // Prefabが見つからない場合は警告ログを表示
            Debug.LogWarning($"Prefabが見つかりません: {prefabName}");
        }

        // Z方向の位置を更新して、次の生成位置をずらす
        zPos += LevelSpacing;

        // 指定した待機時間だけ処理を一時停止してから次の生成へ
        yield return new WaitForSeconds(GenerationDelay);

        // レベル生成フラグを解除し、次回の生成を許可
        creatingLevel = false;
    }

}

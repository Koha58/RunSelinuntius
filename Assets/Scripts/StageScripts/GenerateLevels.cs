using System.Collections;
using UnityEngine;

/// <summary>
/// 外部JSONから読み込んだPrefab名リストを元に、一定間隔でランダムに地面オブジェクトを生成するクラス。
/// Resources/Stages/ 配下のJSONを使い、Resources/Prefabs/ 配下のPrefabをInstantiateする。
/// </summary>
public class GenerateLevels : MonoBehaviour
{
    // 初期設定・定数的な値

    // 最初にPrefabを生成するZ座標の位置
    private const int InitialZPos = 392;

    // 各Prefab間のZ方向の間隔
    private const int LevelSpacing = 98;

    // プレイヤー速度に基づく生成間隔の基準値
    private const float BaseGenerationDelay = 30f;

    // 読み込むJSONファイル名（Resources/Stages/内）
    private string stageFileName = "stage01";

    // プレイヤーの速度を取得するコンポーネント
    private PlayerMove playerMove;

    // --- 状態管理用変数 ---

    // JSONから読み込んだPrefab名の配列
    private string[] prefabNames;

    // 現在のZ生成位置
    private int zPos;

    // 生成中フラグ（連続生成を防止）
    private bool creatingLevel = false;


    void Start()
    {
        // シーン内の PlayerMove コンポーネントを探して取得する
        playerMove = FindObjectOfType<PlayerMove>();

        // PlayerMove コンポーネントが見つからなかった場合はエラーログを出す
        if (playerMove == null)
        {
            Debug.LogError("PlayerMove コンポーネントがシーンに存在しません");
        }

        // 初期Z座標をセット
        zPos = InitialZPos;

        // JSONからPrefabリストを読み込む
        LoadPrefabList();
    }

    void Update()
    {
        // 現在生成中でなく、かつPrefabリストが存在するなら次を生成
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
        // Resources/Stages/ からJSONファイルを読み込む
        TextAsset json = Resources.Load<TextAsset>($"Stages/{stageFileName}");
        if (json == null)
        {
            Debug.LogError($"ステージファイルが見つかりません: {stageFileName}");
            return;
        }

        // JSONをPrefab名リストとして変換する
        PrefabListData data = JsonUtility.FromJson<PrefabListData>(json.text);
        prefabNames = data.prefabs;
    }

    /// <summary>
    /// 一定間隔でPrefabをランダム生成するコルーチン
    /// </summary>
    IEnumerator GenerateLvl()
    {
        // プレイヤー速度に応じて生成間隔を変化
        float speed = playerMove.GetCurrentSpeed();
        float delay = BaseGenerationDelay / speed;

        // ランダムにPrefab名を選ぶ
        int idx = Random.Range(0, prefabNames.Length);
        string prefabName = prefabNames[idx];

        // Resources/Prefabs/ からPrefabをロード
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
        if (prefab != null)
        {
            // 指定Z座標にPrefabを生成
            Instantiate(prefab, new Vector3(0, 0, zPos), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"Prefab '{prefabName}' が見つかりません");
        }

        // 次回生成のZ座標を更新
        zPos += LevelSpacing;

        // 一定時間待ってから次の生成が可能に
        yield return new WaitForSeconds(delay);
        creatingLevel = false;
    }
}

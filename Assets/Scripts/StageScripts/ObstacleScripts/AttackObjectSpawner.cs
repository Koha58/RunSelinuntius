using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// JSONで定義された設定に基づいて攻撃オブジェクト（障害物）を生成するクラス。
/// プレイヤーのHPに応じて生成間隔を変化させ、難易度を調整する。
/// </summary>
public class AttackObjectSpawner : MonoBehaviour
{
    [Header("設定ファイル")]
    [SerializeField] private TextAsset spawnSettingsJson; // 障害物生成設定が記載されたJSONファイル
    [SerializeField] private GameObject player;           // プレイヤーのGameObject（位置参照用）
    [SerializeField] private PlayerStatus playerStatus;   // プレイヤーのHP情報
    [SerializeField] private PlayerMove playerMove;       // プレイヤーの移動速度情報（Z軸速度参照）

    private float timer = 0.0f; // 時間計測用タイマー（生成タイミング制御）

    // prefabPath（Resources内パス）と対応するPrefabをキャッシュ
    private Dictionary<string, GameObject> prefabCache = new();

    // JSONから読み込んだ、各障害物の生成設定情報のリスト
    private List<PrefabSpawnSetting> spawnSettings;

    // ゲーム開始時点のプレイヤーX位置（ランダムX生成の基準）
    private Vector3 playerInitialPosition;

    /// <summary>
    /// 初期化処理。JSONの読み込みとPrefabのキャッシュ、プールの初期化を行う。
    /// </summary>
    void Start()
    {
        // プレイヤーやステータスが設定されていない場合はエラー
        if (player == null || playerStatus == null)
        {
            Debug.LogError("プレイヤーまたはステータスが設定されていません！");
            return;
        }

        // JSONファイルが未指定の場合はエラー
        if (spawnSettingsJson == null)
        {
            Debug.LogError("JSON設定ファイルが指定されていません！");
            return;
        }

        // JSON文字列からPrefabSpawnSettingのリストにデシリアライズ
        spawnSettings = JsonConvert.DeserializeObject<List<PrefabSpawnSetting>>(spawnSettingsJson.text);

        // 設定された各PrefabをResourcesから読み込み、キャッシュ＆プール初期化
        foreach (var setting in spawnSettings)
        {
            GameObject prefab = Resources.Load<GameObject>(setting.prefabPath); // パスからPrefab取得
            if (prefab != null)
            {
                prefabCache[setting.prefabPath] = prefab; // キャッシュに保存
                ObstaclePoolManager.Instance.InitializePool(setting.prefabPath, prefab, 5); // プールに5個登録
            }
            else
            {
                // 指定パスにPrefabが存在しない場合は警告
                Debug.LogWarning($"Prefab 読み込み失敗: {setting.prefabPath}");
            }
        }

        // プレイヤー初期位置を保存（X方向のランダム生成に利用）
        playerInitialPosition = player.transform.position;
    }

    /// <summary>
    /// フレーム毎の更新処理。タイマーによる障害物の定期生成を管理。
    /// </summary>
    void Update()
    {
        // 設定が読み込まれていない場合は処理しない
        if (spawnSettings == null || spawnSettings.Count == 0) return;

        // 経過時間を加算
        timer += Time.deltaTime;

        // プレイヤーの現在のHP割合を計算（0.0〜1.0）
        float hpRatio = playerStatus.CurrentHP / (float)playerStatus.MaxHP;

        // HPが減るほど生成間隔が短くなるように補間（Lerpで直線的に変化）
        float spawnInterval = Mathf.Lerp(
            spawnSettings[0].baseSpawnInterval * spawnSettings[0].minSpawnMultiplier, // 最短間隔
            spawnSettings[0].baseSpawnInterval * spawnSettings[0].maxSpawnMultiplier, // 最長間隔
            1f - hpRatio); // HPが低いほど 1 に近づき、最短間隔に近づく

        // タイマーが生成間隔を超えたら、障害物を生成
        if (timer >= spawnInterval)
        {
            SpawnRandomObject();
            timer = 0.0f;
        }
    }

    /// <summary>
    /// ランダムに選ばれた設定に基づいてPrefabを生成し、位置を調整して配置する。
    /// </summary>
    void SpawnRandomObject()
    {
        // 設定リストからランダムに1つ選択
        var setting = spawnSettings[Random.Range(0, spawnSettings.Count)];

        // 該当Prefabをキャッシュから取得
        GameObject prefab = prefabCache[setting.prefabPath];

        if (prefab == null)
        {
            Debug.LogWarning($"Prefabが見つかりません: {setting.prefabPath}");
            return;
        }

        // 生成位置計算：
        // 自由落下の時間を計算（√(2h/g)）
        float fallTime = Mathf.Sqrt((2f * setting.yOffset) / Physics.gravity.magnitude);

        // 落下中にプレイヤーが進むZ距離を加味した前方位置（zOffsetも加算）
        float forwardZ = setting.zOffset + playerMove.ForwardSpeed * fallTime;

        // 現在のプレイヤー位置にforwardZを加算して生成Z位置を決定
        float spawnZ = player.transform.position.z + forwardZ;

        // プレイヤー初期X位置を基準に、X範囲（min～max）内でランダム生成
        float spawnX = Random.Range(
            playerInitialPosition.x + setting.minXOffset,
            playerInitialPosition.x + setting.maxXOffset
        );

        // 最終的な生成位置を決定（Yは設定された上空の高さ）
        Vector3 spawnPos = new Vector3(spawnX, setting.yOffset, spawnZ);

        // プールから障害物オブジェクトを取得し、位置設定
        GameObject obj = ObstaclePoolManager.Instance.GetObject(setting.prefabPath, prefab);
        obj.transform.position = spawnPos;

        // 一定時間後にプールに返却するコルーチン開始
        StartCoroutine(ReturnToPoolAfterDelay(setting.prefabPath, obj, setting.destroyDelay));
    }

    /// <summary>
    /// 指定秒数後にオブジェクトをプールに返却する（再利用のため）
    /// </summary>
    /// <param name="prefabPath">返却対象のPrefabを識別するパス（プール管理に使用）</param>
    /// <param name="obj">返却する対象のゲームオブジェクト（障害物）</param>
    /// <param name="delay">返却までの待機時間（秒）</param>
    IEnumerator ReturnToPoolAfterDelay(string prefabPath, GameObject obj, float delay)
    {
        // 指定時間待機
        yield return new WaitForSeconds(delay);

        // オブジェクトがすでに破棄されていないかチェック
        if (obj == null)
        {
            Debug.LogWarning($"ReturnToPoolAfterDelay: obj is already destroyed or null. prefabPath={prefabPath}");
            yield break;
        }

        // デバッグ用ログ出力（返却通知）
        Debug.Log($"ReturnToPoolAfterDelay: returning object {obj.name}");

        // プールに返却（非アクティブ化など）
        ObstaclePoolManager.Instance.ReturnObject(prefabPath, obj);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の攻撃用オブジェクト(障害物)生成クラス
/// </summary>
public class AttackObjectSpawner : MonoBehaviour
{
    [Header("Prefab設定")]
    [SerializeField] private GameObject[] prefabsToSpawn; // 生成するPrefabの配列
    [SerializeField] private GameObject player; // プレイヤーオブジェクト

    [Header("生成間隔設定")]
    [SerializeField] private float baseSpawnInterval = 2.0f; // 基本のオブジェクト生成間隔（秒）
    [SerializeField] private float maxSpawnMultiplier = 2.0f; // HP最大時の生成間隔倍率
    [SerializeField] private float minSpawnMultiplier = 0.2f; // HP最小時の生成間隔倍率

    [Header("生成位置設定")]
    [SerializeField] private float distanceFromPlayer = 40.0f; // プレイヤーからのZ軸方向の距離
    [SerializeField] private float minXOffset = -7.0f; // 初期位置からのX座標最小オフセット
    [SerializeField] private float maxXOffset = 8.0f; // 初期位置からのX座標最大オフセット
    [SerializeField] private float spawnYOffset = 0.7f; // 通常Prefab生成時のY軸オフセット
    [SerializeField] private float specialYOffset = 5.0f; // 特定Prefab生成時のY軸オフセット

    [Header("効果音・削除設定")]
    [SerializeField] private CannonSEManager cannonSEManager; // 大砲攻撃の効果音を管理するクラスの参照
    [SerializeField] private float destroyDelay = 10.0f; // オブジェクトを削除するまでの遅延時間（秒）

    [Header("プレイヤー状態")]
    [SerializeField] private PlayerStatus playerStatus; // プレイヤーのHP情報

    private Vector3 playerInitialPosition; // プレイヤーの初期位置
    private float timer = 0.0f; // オブジェクト生成間隔を計測するタイマー

    void Start()
    {
        // プレイヤーの初期位置を保存
        if (player != null)
        {
            playerInitialPosition = player.transform.position;
        }
        else
        {
            Debug.LogError("Player object is not assigned!");
        }
    }

    void Update()
    {
        timer += Time.deltaTime; // 経過時間を加算

        // HPに基づいて動的に生成間隔を計算
        float hpRatio = playerStatus.CurrentHP / (float)playerStatus.MaxHP;

        // HPの割合に応じて生成間隔を補間
        float spawnInterval = Mathf.Lerp(
            baseSpawnInterval * maxSpawnMultiplier,
            baseSpawnInterval * minSpawnMultiplier,
            hpRatio
        );

        if (timer >= spawnInterval)
        {
            SpawnRandomObject(); // ランダムなオブジェクトを生成
            timer = 0.0f; // タイマーをリセット
        }
    }

    /// <summary>
    /// ランダムなPrefabを生成し、プレイヤーの前方に配置する
    /// </summary>
    void SpawnRandomObject()
    {
        if (prefabsToSpawn.Length == 0 || player == null)
        {
            Debug.LogError("Prefabまたはプレイヤーが設定されていません！");
            return;
        }

        // 配列からランダムにPrefabを選択
        int randomIndex = Random.Range(0, prefabsToSpawn.Length);
        GameObject selectedPrefab = prefabsToSpawn[randomIndex];

        // プレイヤー初期位置を基準にランダムなX座標を計算
        float randomX = Random.Range(playerInitialPosition.x + minXOffset, playerInitialPosition.x + maxXOffset);

        // プレイヤーの前方にオブジェクトを配置する位置を計算
        Vector3 spawnPosition = new Vector3(randomX, spawnYOffset, player.transform.position.z + Mathf.Abs(distanceFromPlayer));

        // 特定のPrefabの場合は特別なYオフセットを適用
        if (randomIndex == 2) // インデックス2のPrefabに特殊効果を適用
        {
            spawnPosition.y = player.transform.position.y + specialYOffset;
            cannonSEManager.PlayCannonFireSound(); // 効果音を再生
        }

        // Prefabを生成
        GameObject obj = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        // 指定した時間後にオブジェクトを削除
        StartCoroutine(DestroyObjectAfterDelay(obj, destroyDelay));
    }

    /// <summary>
    /// 指定時間後にオブジェクトを削除するコルーチン
    /// </summary>
    IEnumerator DestroyObjectAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            Destroy(obj);
        }
    }
}

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
    [SerializeField] private float maxSpawnMultiplier = 1.0f; // HP最大時の生成間隔倍率
    [SerializeField] private float minSpawnMultiplier = 0.5f; // HP最小時の生成間隔倍率

    [Header("生成位置設定")]
    [SerializeField] private float distanceFromPlayer = 20.0f; // PrefabのデフォルトZ座標オフセット
    [SerializeField] private float minXOffset = -7.0f; // 初期位置からのX座標最小オフセット
    [SerializeField] private float maxXOffset = 8.0f; // 初期位置からのX座標最大オフセット
    [SerializeField] private float spawnYOffset = 0.7f; // Prefab生成時のデフォルトY軸オフセット

    [Header("Prefabの生成位置設定")]
    [SerializeField] private float[] specialYOffset = new float[] { 0.1f, 0.1f, 5.0f }; // y軸の値を設定{ ArrowAttack, Barrel, IronBall}
    [SerializeField] private float[] specialZOffset = new float[] { 45.0f, 100.0f, 20.0f }; // z軸の値を設定{ ArrowAttack, Barrel, IronBall}

    [Header("効果音・削除設定")]
    [SerializeField] private float destroyDelay = 10.0f; // オブジェクトを削除するまでの遅延時間（秒）

    [Header("プレイヤー状態")]
    [SerializeField] private PlayerStatus playerStatus; // プレイヤーのHP情報

    private Vector3 playerInitialPosition; // プレイヤーの初期位置
    private float timer = 0.0f; // オブジェクト生成間隔を計測するタイマー

    void Start()
    {
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
            SpawnRandomObject();
            timer = 0.0f;
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
        float spawnY = spawnYOffset;
        float spawnZ = player.transform.position.z + distanceFromPlayer;

        // Prefab毎のオフセットを適用
        if (randomIndex < specialYOffset.Length)
        {
            spawnY = specialYOffset[randomIndex];
        }
        if (randomIndex < specialZOffset.Length)
        {
            spawnZ = player.transform.position.z + specialZOffset[randomIndex];
        }

        // 生成位置を決定
        Vector3 spawnPosition = new Vector3(randomX, spawnY, spawnZ);

        // Prefabを生成
        GameObject obj = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        // 指定した時間後にオブジェクトを削除
        StartCoroutine(DestroyObjectAfterDelay(obj, destroyDelay));
    }

    /// <summary>
    /// 指定時間後にオブジェクトを削除するコルーチン。
    /// </summary>
    /// <param name="obj">削除対象のゲームオブジェクト</param>
    /// <param name="delay">削除までの遅延時間（秒）</param>
    private IEnumerator DestroyObjectAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            Destroy(obj);
        }
    }
}

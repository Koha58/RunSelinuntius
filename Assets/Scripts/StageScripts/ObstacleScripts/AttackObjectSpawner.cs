using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の攻撃用オブジェクト(障害物)生成クラス
/// </summary>
public class AttackObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabsToSpawn; // 生成するPrefabの配列
    [SerializeField] private GameObject player; // プレイヤーオブジェクト
    [SerializeField] private float spawnInterval = 5.0f; // オブジェクト生成間隔（秒）
    [SerializeField] private float distanceFromPlayer = 35.0f; // プレイヤーからのZ軸方向の距離
    [SerializeField] private float minX = -4.0f; // 移動可能範囲の最小X座標
    [SerializeField] private float maxX = 4.0f; // 移動可能範囲の最大X座標
    [SerializeField] private CannonSEManager cannonSEManager; // 大砲攻撃の効果音を管理するCannonSEManagerクラスの参照

    [Header("オブジェクト生成設定")]
    [SerializeField] private float specialYOffset = 5.0f; // 特定のPrefab生成時に追加するY軸のオフセット
    [SerializeField] private float destroyDelay = 10.0f; // オブジェクトを削除するまでの遅延時間（秒）
    [SerializeField] private float positionCheckDelay = 0.1f; // オブジェクト生成後の位置確認遅延（秒）

    private float timer = 0.0f; // オブジェクト生成間隔を計測するタイマー

    /// <summary>
    /// 毎フレーム更新処理
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime; // 経過時間を加算

        // 生成間隔を超えた場合、オブジェクトを生成する
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
        // 生成するPrefabまたはプレイヤーが設定されていない場合はエラーを表示して終了
        if (prefabsToSpawn.Length == 0 || player == null)
        {
            Debug.LogError("Prefabまたはプレイヤーが設定されていません！");
            return;
        }

        // 配列からランダムにPrefabを選択
        int randomIndex = Random.Range(0, prefabsToSpawn.Length);
        GameObject selectedPrefab = prefabsToSpawn[randomIndex];

        // プレイヤーの現在位置を取得
        Vector3 playerPosition = player.transform.position;

        // 移動可能範囲内でランダムなX座標を計算
        float randomX = Random.Range(playerPosition.x + minX, playerPosition.x + maxX);

        // プレイヤーの前方にオブジェクトを配置する位置を計算
        Vector3 spawnPosition = new Vector3(randomX, playerPosition.y, playerPosition.z + Mathf.Abs(distanceFromPlayer));

        // 特定のPrefab（配列のインデックスが2）であれば、追加のY軸オフセットを設定
        if (randomIndex == 2)
        {
            spawnPosition.y = playerPosition.y + specialYOffset; // Y軸を変更
            cannonSEManager.PlayCannonFireSound(); // 効果音を再生
            Debug.Log("Changing Y position for third prefab");
        }

        // Prefabを生成し、指定した位置に配置
        GameObject obj = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"Spawn Position Before Animation: {obj.transform.position}");

        // 生成後の位置を一定時間後に確認する
        StartCoroutine(CheckPositionAfterDelay(obj, positionCheckDelay));

        // 指定した時間後にオブジェクトを削除する
        StartCoroutine(DestroyObjectAfterDelay(obj, destroyDelay));
    }

    /// <summary>
    /// 指定時間後にオブジェクトを削除するコルーチン
    /// </summary>
    /// <param name="obj">削除するオブジェクト</param>
    /// <param name="delay">削除までの遅延時間（秒）</param>
    IEnumerator DestroyObjectAfterDelay(GameObject obj, float delay)
    {
        // 指定した時間だけ待機
        yield return new WaitForSeconds(delay);

        // オブジェクトが存在する場合のみ削除
        if (obj != null)
        {
            Destroy(obj);
            Debug.Log("Object destroyed after delay.");
        }
    }

    /// <summary>
    /// 生成されたオブジェクトの位置を一定時間後に確認するコルーチン
    /// </summary>
    /// <param name="obj">確認するオブジェクト</param>
    /// <param name="delay">確認までの遅延時間（秒）</param>
    IEnumerator CheckPositionAfterDelay(GameObject obj, float delay)
    {
        // 指定した時間だけ待機
        yield return new WaitForSeconds(delay);

        // オブジェクトが存在する場合、現在の位置をログに出力
        if (obj != null)
        {
            Debug.Log($"Spawn Position After Animation: {obj.transform.position}");
        }
    }
}

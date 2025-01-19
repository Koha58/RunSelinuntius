using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の攻撃用オブジェクト生成クラス
/// </summary>
public class AttackObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabsToSpawn; // 生成するPrefabの配列

    [SerializeField] private GameObject player; // プレイヤーオブジェクト

    [SerializeField] private float spawnInterval = 5.0f; // 生成間隔

    [SerializeField] private float distanceFromPlayer = 1.0f; // プレイヤーから手前の距離

    [SerializeField] private float minX = -5.0f; // 移動可能範囲の最小X座標

    [SerializeField] private float maxX = 5.0f; // 移動可能範囲の最大X座標

    private float timer = 0.0f; // タイマー

    /// <summary>
    /// 毎フレーム実行されるメソッド。
    /// 一定時間ごとにPrefabを生成。
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime;

        // 一定時間ごとにオブジェクトを生成
        if (timer >= spawnInterval)
        {
            SpawnRandomObject();
            timer = 0.0f; // タイマーをリセット
        }
    }

    /// <summary>
    /// ランダムなPrefabをプレイヤーの前方に生成します。
    /// 配置位置はプレイヤーの移動可能範囲内でランダムに決まる。
    /// </summary>
    void SpawnRandomObject()
    {
        // 配列またはプレイヤーが設定されていない場合、処理を中断
        if (prefabsToSpawn.Length == 0 || player == null)
        {
            Debug.LogError("Prefabまたはプレイヤーが設定されていません！");
            return;
        }

        // ランダムにPrefabを選択
        GameObject selectedPrefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];

        // プレイヤーの位置と範囲を取得
        Vector3 playerPosition = player.transform.position;

        // ランダムなX座標を計算（移動可能範囲内）
        float randomX = Random.Range(minX, maxX);

        // プレイヤーの手前の位置を計算
        Vector3 spawnPosition = new Vector3(randomX, playerPosition.y, playerPosition.z + distanceFromPlayer);

        // オブジェクトを生成
        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }
}

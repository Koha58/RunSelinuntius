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

    [SerializeField] private float distanceFromPlayer = 35.0f; // プレイヤーから手前の距離

    [SerializeField] private float minX = -5.0f; // 移動可能範囲の最小X座標

    [SerializeField] private float maxX = 5.0f; // 移動可能範囲の最大X座標

    [SerializeField] private CannonSEManager cannonSEManager; // CannonSEManagerの参照

    private float timer = 0.0f; // タイマー

    void Update()
    {
        timer += Time.deltaTime; // タイマーを進める

        // 一定時間ごとにオブジェクトを生成
        if (timer >= spawnInterval)
        {
            SpawnRandomObject(); // オブジェクトを生成
            timer = 0.0f; // タイマーをリセット
        }
    }

    /// <summary>
    /// ランダムなPrefabを生成し、プレイヤーの前方に配置する
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
        int randomIndex = Random.Range(0, prefabsToSpawn.Length);
        GameObject selectedPrefab = prefabsToSpawn[randomIndex];

        // プレイヤーの位置と範囲を取得
        Vector3 playerPosition = player.transform.position;

        // ランダムなX座標を計算（移動可能範囲内）
        float randomX = Random.Range(playerPosition.x + minX, playerPosition.x + maxX);

        // プレイヤーの手前の位置を計算
        Vector3 spawnPosition = new Vector3(randomX, playerPosition.y, playerPosition.z + Mathf.Abs(distanceFromPlayer));

        // 3番目のPrefab（インデックス2）であれば、y座標を変更する
        if (randomIndex == 2)
        {
            spawnPosition.y = playerPosition.y + 5.0f; // 例として+5.0fだけ変更
            // 効果音を再生
            cannonSEManager.PlayCannonFireSound();
            Debug.Log("Changing Y position for third prefab");
        }

        // オブジェクトを生成
        GameObject obj = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        // 生成直後の位置を確認
        Debug.Log($"Spawn Position Before Animation: {obj.transform.position}");

        // 0.1秒後の位置確認
        StartCoroutine(CheckPositionAfterDelay(obj, 0.1f));

        // 10秒後にオブジェクトを削除
        StartCoroutine(DestroyObjectAfterDelay(obj, 10.0f)); // 2秒後に削除
    }

    /// <summary>
    /// オブジェクトを指定した時間後に削除するコルーチン
    /// </summary>
    IEnumerator DestroyObjectAfterDelay(GameObject obj, float delay)
    {
        // 指定した時間だけ待機
        yield return new WaitForSeconds(delay);

        // オブジェクトが存在していれば削除
        if (obj != null)
        {
            Destroy(obj);
            Debug.Log("Object destroyed after delay.");
        }
    }

    /// <summary>
    /// 生成したオブジェクトの位置がアニメーションで変更される可能性があるため、一定時間後に位置を確認する
    /// </summary>
    IEnumerator CheckPositionAfterDelay(GameObject obj, float delay)
    {
        // 指定した時間だけ待機
        yield return new WaitForSeconds(delay);

        // 位置を確認してログに出力
        if (obj != null)
        {
            Debug.Log($"Spawn Position After Animation: {obj.transform.position}");
        }
    }
}

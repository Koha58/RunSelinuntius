using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 障害物のオブジェクトプールを管理するクラス（Singleton）
/// 生成コスト削減・再利用によってパフォーマンスを向上させる
/// </summary>
public class ObstaclePoolManager : MonoBehaviour
{
    /// <summary>
    /// Singletonインスタンス（他クラスからアクセス可能）
    /// </summary>
    public static ObstaclePoolManager Instance;

    /// <summary>
    /// プール管理用ディクショナリ（キー:Prefabパス、値:オブジェクトのキュー）
    /// </summary>
    private Dictionary<string, Queue<GameObject>> pool = new();

    /// <summary>
    /// インスタンスの初期化（Singletonのセット）
    /// </summary>
    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 指定のPrefabパスに対応するオブジェクトプールを初期化する
    /// </summary>
    /// <param name="prefabPath">Resources.Loadなどに使うPrefabのパス</param>
    /// <param name="prefab">生成元のPrefab</param>
    /// <param name="initialSize">初期生成しておく数</param>
    public void InitializePool(string prefabPath, GameObject prefab, int initialSize)
    {
        // まだプールが存在しない場合のみ初期化
        if (!pool.ContainsKey(prefabPath))
        {
            pool[prefabPath] = new Queue<GameObject>();

            // 指定数だけオブジェクトを生成・非アクティブ化してプールに格納
            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                pool[prefabPath].Enqueue(obj);
            }
        }
    }

    /// <summary>
    /// プールからオブジェクトを1つ取得する（足りない場合は新規生成）
    /// </summary>
    /// <param name="prefabPath">オブジェクトの種類を示すキー</param>
    /// <param name="prefab">必要に応じて使うPrefab</param>
    /// <returns>使用可能なGameObject</returns>
    public GameObject GetObject(string prefabPath, GameObject prefab)
    {
        // プールがない、または空なら新規生成
        if (!pool.ContainsKey(prefabPath) || pool[prefabPath].Count == 0)
        {
            GameObject newObj = Instantiate(prefab);
            return newObj;
        }

        // プールから取得し、アクティブ化して返す
        GameObject obj = pool[prefabPath].Dequeue();
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 使用済みオブジェクトをプールに返却する（非アクティブ化）
    /// </summary>
    /// <param name="prefabPath">オブジェクトの種類を示すキー</param>
    /// <param name="obj">返却するGameObject</param>
    public void ReturnObject(string prefabPath, GameObject obj)
    {
        // オブジェクトがすでに破棄されていないか確認
        if (obj == null)
        {
            Debug.LogWarning($"ReturnObject called but obj is already destroyed or null. prefabPath={prefabPath}");
            return;
        }

        // デバッグログ：返却処理を確認用に出力
        Debug.Log($"ReturnObject: returning {obj.name} for prefabPath={prefabPath}");

        // 非アクティブ化してプールに戻す
        obj.SetActive(false);
        pool[prefabPath].Enqueue(obj);
    }
}

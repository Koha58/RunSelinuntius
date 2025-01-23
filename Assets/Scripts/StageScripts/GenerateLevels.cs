using System.Collections;
using UnityEngine;

/// <summary>
/// ランエリアを自動生成するクラス
/// </summary>
public class GenerateLevels : MonoBehaviour
{
    [Header("レベル設定")]
    [SerializeField] private GameObject[] level;              // レベルオブジェクトを格納する配列
    [SerializeField] private int initialZPos = 392;           // 初期のz座標
    [SerializeField] private int levelSpacing = 98;           // レベル間の距離
    [SerializeField] private float baseGenerationDelay = 30f; // 基本の生成間隔（速度に応じて変化）

    [Header("速度設定")]
    [SerializeField] private PlayerMove playerMove;           // プレイヤーの移動管理クラス

    private int zPos;                                         // 現在のz座標
    private bool creatingLevel = false;                       // レベルを生成中かどうかのフラグ

    void Start()
    {
        // 初期のz座標を設定
        zPos = initialZPos;
    }

    void Update()
    {
        // レベルが生成中でない場合、レベル生成を始める
        if (!creatingLevel)
        {
            creatingLevel = true; // 生成中フラグを設定
            StartCoroutine(GenerateLvl()); // 非同期メソッドを実行
        }
    }

    /// <summary>
    /// 新しいレベルを生成するコルーチン
    /// </summary>
    IEnumerator GenerateLvl()
    {
        // プレイヤーの速度に応じて生成間隔を変更
        float currentSpeed = playerMove.GetCurrentSpeed(); // プレイヤーの速度を取得
        float adjustedGenerationDelay = baseGenerationDelay / currentSpeed; // 速度に応じて生成間隔を調整

        // レベルオブジェクトの配列からランダムな項目を選択
        int lvlNum = Random.Range(0, level.Length);

        // 新しいレベルを生成
        Instantiate(level[lvlNum], new Vector3(0, 0, zPos), Quaternion.identity);

        // レベルを生成する位置を更新
        zPos += levelSpacing;

        // 次の生成までの一時停止（速度に応じて間隔が短縮）
        yield return new WaitForSeconds(adjustedGenerationDelay);

        // レベル生成が終了したのでフラグをリセット
        creatingLevel = false;
    }
}

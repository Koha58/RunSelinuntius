using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトルシーンのランエリアを自動生成するクラス
/// </summary>
public class TitleGenerateLevel : MonoBehaviour
{
    [Header("レベル設定")]
    [SerializeField] private GameObject[] level;              // レベルオブジェクトを格納する配列
    [SerializeField] private float initialZPos = 700f;           // 初期のz座標
    [SerializeField] private float initialXPos = 2.0f;            // 初期のx座標
    [SerializeField] private float levelSpacing = 98.0f;           // レベル間の距離
    [SerializeField] private float GenerationDelay = 30.0f; // 生成間隔

    private float xPos;                                         // 現在のx座標
    private float zPos;                                         // 現在のz座標
    private bool creatingLevel = false;                       // レベルを生成中かどうかのフラグ

    void Start()
    {
        // 初期のx座標を設定
        xPos = initialXPos;
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
        // レベルオブジェクトの配列からランダムな項目を選択
        int lvlNum = Random.Range(0, level.Length);

        // 新しいレベルを生成
        Instantiate(level[lvlNum], new Vector3(xPos, 0, zPos), Quaternion.identity);

        // レベルを生成するz位置を更新
        zPos += levelSpacing;

        // 次の生成までの一時停止
        yield return new WaitForSeconds(GenerationDelay);

        // レベル生成が終了したのでフラグをリセット
        creatingLevel = false;
    }
}

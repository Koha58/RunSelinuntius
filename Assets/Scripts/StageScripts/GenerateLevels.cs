using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ランエリアを自動生成するクラス
/// </summary>
public class GenerateLevels : MonoBehaviour
{
    // レベルオブジェクトを格納する配列
    [SerializeField] private GameObject[] level;
    // 生成されるレベルの新しいz座標
    [SerializeField] private int zPos = 98;
    // レベルを生成中かどうかを表すフラグ
    [SerializeField] private bool creatingLevel = false;
    // ランダムなレベルを選択するための数字
    [SerializeField] private int lvlNum;

    void Start()
    {
        // 初期化処理を実装する場所
    }

    void Update()
    {
        // レベルが生成中でない場合、レベル生成を始める
        if (!creatingLevel)
        {
            creatingLevel = true; // 生成中フラグを終了済みに設定
            StartCoroutine(GenerateLvl()); // 非同期メソッドを実行
        }
    }

    /// <summary>
    /// 新しいレベルを生成するコルチンメソッド
    /// </summary>
    IEnumerator GenerateLvl()
    {
        // レベルオブジェクトの配列からランダムな項目を選択
        lvlNum = Random.Range(0, 4); // レベルオブジェクトのインデックス(0から4の間)

        // 新しいレベルを生成
        Instantiate(level[lvlNum], new Vector3(0, 0, zPos), Quaternion.identity);

        // レベルを生成する位置を更新
        zPos += 98;

        // 次の生成までの一時停止
        yield return new WaitForSeconds(20);

        // レベル生成が終了したのでフラグをリセット
        creatingLevel = false;
    }
}

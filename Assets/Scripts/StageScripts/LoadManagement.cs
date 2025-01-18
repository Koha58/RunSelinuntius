using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラ外のランエリアを削除するクラス
/// </summary>
public class LoadManagement : MonoBehaviour
{
    /// <summary>
    /// トリガーエリアに別のオブジェクトが侵入した際に呼び出される処理
    /// </summary>
    /// <param name="collision">侵入したオブジェクトのコライダー</param>
    private void OnTriggerEnter(Collider collision)
    {
        // トリガーに侵入したオブジェクトが "Level" タグを持っている場合
        if (collision.gameObject.CompareTag("Level"))
        {
            // そのオブジェクトを破壊する
            Destroy(collision.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
//　タイトルシーンのプレイヤーの動きを制御するクラス
/// </summary>
public class TitlePlayerMove : MonoBehaviour
{
    // 移動速度
    [SerializeField] private float speed = 5.0f;

    /// <summary>
    /// 毎フレーム呼び出される処理
    /// 前方向（Z軸方向）に移動させる
    /// </summary>
    private void Update()
    {
        // オブジェクトの前方向（ローカルZ軸）に移動させる
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}

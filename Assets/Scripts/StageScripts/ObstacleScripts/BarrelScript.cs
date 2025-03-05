using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 樽オブジェクトを転がすクラス
/// </summary>
public class BarrelScript : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f; // 移動速度

    void Start()
    {

    }

    // FixedUpdate() メソッドは物理演算が行われるタイミングで実行される
    // 物理計算のタイミングで移動処理を行うことで、スムーズな動作が可能になる
    void FixedUpdate()
    {
        // 世界座標のZ方向に移動させる
        Vector3 moveDirection = Vector3.forward * speed * Time.fixedDeltaTime;

        // transformを直接移動
        transform.position -= moveDirection;
    }

}
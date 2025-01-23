using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Playerの進んだ距離を表示させるクラス
/// </summary>
public class PlayerDistanceTracker : MonoBehaviour
{
    [SerializeField] private Text distanceText; // 距離を表示するUIテキスト
    private Vector3 initialPosition; // プレイヤーの初期位置

    void Start()
    {
        // プレイヤーの初期位置を保存
        initialPosition = transform.position;
    }

    void Update()
    {
        // Z軸方向の進行距離を計算
        float distanceTraveled = transform.position.z - initialPosition.z;

        // 距離をテキストに出力
        distanceText.text = $"{Mathf.FloorToInt(distanceTraveled)} M";
    }
}

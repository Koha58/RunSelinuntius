using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TargetとPlayerの間の距離を表示させるクラス
/// </summary>
public class TargetDistanceTracker : MonoBehaviour
{
    [SerializeField] private Transform target; // ターゲットのTransform
    [SerializeField] private Transform player; // プレイヤーのTransform
    [SerializeField] private Text distanceText; // 距離を表示するUIテキスト

    void Update()
    {
        if (target == null || player == null || distanceText == null) return;

        // ターゲットとプレイヤーの間の距離を計算
        float distanceBetween = Vector3.Distance(target.position, player.position);

        // 距離をテキストに出力
        distanceText.text = $"{Mathf.FloorToInt(distanceBetween)} M";
    }
}
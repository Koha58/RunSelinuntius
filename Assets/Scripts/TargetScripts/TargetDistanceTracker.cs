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
    [SerializeField] private float updateInterval = 0.1f; // テキスト更新の間隔 (秒)

    private float timeSinceLastUpdate = 0f; // 最後に更新した時間

    void Update()
    {
        if (target == null || player == null || distanceText == null) return;

        // 時間が経過した場合にテキストを更新
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            // ターゲットとプレイヤーの間の距離を計算
            float distanceBetween = Vector3.Distance(target.position, player.position);

            // 距離を四捨五入して整数にして表示
            distanceText.text = $"{Mathf.RoundToInt(distanceBetween)} M";

            // 最後に更新した時間をリセット
            timeSinceLastUpdate = 0f;
        }
    }
}
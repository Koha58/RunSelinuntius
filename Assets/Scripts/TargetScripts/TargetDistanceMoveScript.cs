using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Target(Melos)の頭上に表示するUIを管理するクラス
/// </summary>
public class TargetDistanceMoveScript : MonoBehaviour
{
    [SerializeField] private Transform target; // ターゲットのTransform
    [SerializeField] private Transform player; // プレイヤーのTransform
    [SerializeField] private Text distanceText; // 距離を表示するUIテキスト
    [SerializeField] private Text arrowText; // ▼ を表示するためのUIテキスト

    [SerializeField] private float maxOffsetY = 30f; // 遠いときの最大オフセット
    [SerializeField] private float minOffsetY = 1f;  // 近いときの最小オフセット
    [SerializeField] private float maxDistance = 500f; // 最大距離（これ以上遠いと maxOffsetY で固定）
    [SerializeField] private float arrowOffsetY = -30f; // 矢印の距離テキストからのオフセット

    void Update()
    {
        if (target == null || player == null || distanceText == null || arrowText == null) return;

        // ターゲットとプレイヤーの間の距離を計算
        float distanceBetween = Vector3.Distance(target.position, player.position);

        // 距離に応じてオフセットを変化させる（遠いほど大きく、近いほど小さく）
        float dynamicOffsetY = Mathf.Lerp(minOffsetY, maxOffsetY, Mathf.Clamp01(distanceBetween / maxDistance));

        // 位置を調整
        Vector3 offset = new Vector3(0, dynamicOffsetY, 0);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + offset);

        // テキストの更新
        distanceText.text = $"{Mathf.FloorToInt(distanceBetween)}M";
        distanceText.transform.position = screenPos;

        // 矢印の表示位置
        arrowText.text = "▼";
        arrowText.transform.position = screenPos + new Vector3(0, arrowOffsetY, 0);
    }
}
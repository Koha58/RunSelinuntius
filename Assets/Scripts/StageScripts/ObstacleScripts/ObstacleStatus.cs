using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の攻撃用オブジェクト(障害物)のステータス管理クラス
/// </summary>
public class ObstacleStatus : MonoBehaviour
{
    [Header("ダメージ設定")]
    [SerializeField] private int damage; // 障害物が与えるダメージ量

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーとトリガー内で接触した場合
        if (other.CompareTag("Player"))
        {
            // プレイヤーのHPコンポーネントを取得してダメージを与える
            PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                playerStatus.TakeDamage(damage);
            }
        }
    }
}

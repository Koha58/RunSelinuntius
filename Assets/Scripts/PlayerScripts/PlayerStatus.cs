using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Playerのステータス管理用クラス
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    [Header("HP設定")]
    [SerializeField] private int maxHP = 100; // 最大HP
    private int currentHP;                   // 現在のHP

    public int MaxHP => maxHP; // 公開用プロパティ
    public int CurrentHP => currentHP; // 現在のHPを公開

    public delegate void OnHealthChanged(int current, int max);
    public event OnHealthChanged HealthChanged; // HP変化イベント

    private void Awake()
    {
        // 初期化
        currentHP = maxHP;
    }

    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        // HP変化イベントを通知
        HealthChanged?.Invoke(currentHP, maxHP);

        // HPが0になったら死亡処理を呼び出し
        if (currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    private void Die()
    {
        Debug.Log("Player has died!");
        // ここで死亡時の処理を実装 (アニメーション、リスポーンなど)
    }
}

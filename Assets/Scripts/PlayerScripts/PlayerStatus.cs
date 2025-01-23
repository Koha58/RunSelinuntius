using System.Collections;
using UnityEngine;

/// <summary>
/// Playerのステータス管理用クラス
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    [Header("HP設定")]
    [SerializeField] private int maxHP = 100; // 最大HP
    private int currentHP;                   // 現在のHP
    [SerializeField] private float recoveryRate = 5f; // HP回復速度 (秒ごとの回復量)

    public int MaxHP => maxHP; // 公開用プロパティ
    public int CurrentHP => currentHP; // 現在のHPを公開

    public delegate void OnHealthChanged(int current, int max);
    public event OnHealthChanged HealthChanged; // HP変化イベント

    private void Awake()
    {
        // 初期化 (HPを最大値の半分から開始)
        currentHP = maxHP / 2;

        // 初期値をイベントで通知
        HealthChanged?.Invoke(currentHP, maxHP);
    }

    private void Start()
    {
        // HP回復処理を開始
        StartCoroutine(RecoverHP());
    }

    /// <summary>
    /// HPを徐々に回復するコルーチン
    /// </summary>
    private IEnumerator RecoverHP()
    {
        while (currentHP < maxHP)
        {
            currentHP += Mathf.FloorToInt(recoveryRate * Time.deltaTime);  // 回復量を計算して加算
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);  // HPを最大値に制限

            // HP変化イベントを通知
            HealthChanged?.Invoke(currentHP, maxHP);

            yield return null;  // 毎フレーム処理を継続
        }
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

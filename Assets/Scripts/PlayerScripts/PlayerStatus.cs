using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤーのステータスを管理するクラス
/// HPの管理や回復処理を行う
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    [Header("HP設定")]
    [SerializeField] private int maxHP = 100; // プレイヤーの最大HP
    private int currentHP;                   // 現在のHP
    [SerializeField] private float recoveryRate = 0.5f; // 毎秒のHP回復速度
    private float recoveryBuffer = 0f;       // 小数点以下の回復量を保持するバッファ

    // HPの最大値と現在値を公開するプロパティ
    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    // HPの変化を通知するイベント
    public delegate void OnHealthChanged(int current, int max);
    public event OnHealthChanged HealthChanged;

    /// <summary>
    /// 初期化処理
    /// HPを最大値の半分に設定し、初期値を通知
    /// </summary>
    private void Awake()
    {
        // HPを最大値の半分に設定
        currentHP = maxHP / 2;

        // HPの初期状態を通知
        HealthChanged?.Invoke(currentHP, maxHP);
    }

    /// <summary>
    /// HP回復処理を開始
    /// </summary>
    private void Start()
    {
        StartCoroutine(RecoverHP());
    }

    /// <summary>
    /// HPを徐々に回復させるコルーチン
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator RecoverHP()
    {
        while (true)
        {
            // HPが最大値未満の場合に回復処理を実行
            if (currentHP < maxHP)
            {
                // 回復バッファに回復量を加算
                recoveryBuffer += recoveryRate * Time.deltaTime;

                // バッファが1以上の場合、整数部分をHPに加算
                if (recoveryBuffer >= 1f)
                {
                    int increase = Mathf.FloorToInt(recoveryBuffer);
                    recoveryBuffer -= increase; // 小数部分を残す
                    currentHP += increase;
                    currentHP = Mathf.Clamp(currentHP, 0, maxHP); // HPを最大値で制限

                    // HP変化を通知
                    HealthChanged?.Invoke(currentHP, maxHP);
                }
            }

            // 次のフレームまで待機
            yield return null;
        }
    }

    /// <summary>
    /// ダメージを受けたときの処理
    /// </summary>
    /// <param name="damage">受けるダメージ量</param>
    public void TakeDamage(int damage)
    {
        // HPを減少させ、0未満にならないよう制限
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        // HP変化を通知
        HealthChanged?.Invoke(currentHP, maxHP);

        // HPが0以下の場合、死亡処理を呼び出す
        if (currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// プレイヤーが死亡したときの処理
    /// </summary>
    private void Die()
    {
        Debug.Log("Player has died!");
        SceneManager.LoadScene("GameOverScene");
    }
}

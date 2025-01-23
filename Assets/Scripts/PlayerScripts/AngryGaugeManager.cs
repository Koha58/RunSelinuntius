using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 憤度(PlayerのHP＆速度)管理用クラス
/// </summary>
public class AngryGaugeManager : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;  // プレイヤーのHP
    [SerializeField] private PlayerMove playerMove;      // プレイヤーの動き
    [SerializeField] private Image barImage;             // HP＆憤度を表示するImage (1つのImageを共有)

    [Header("ゲージ設定")]
    [SerializeField] private float maxGauge = 100f;      // ゲージの最大値
    [SerializeField] private float gaugeIncreaseRate = 1f; // ゲージの上昇速度 (毎秒の上昇量)
    [SerializeField] private float minSpeedMultiplier = 0.5f; // 速度の最小倍率
    [SerializeField] private float maxSpeedMultiplier = 3f;   // 速度の最大倍率

    private float currentGauge;  // 現在のゲージ値
    private bool isPlayerAlive = true; // プレイヤーが生存しているかどうか

    private void Start()
    {
        if (playerStatus != null)
        {
            // HPイベントに登録
            playerStatus.HealthChanged += OnHealthChanged;

            // 初期表示を更新
            UpdateBarUI(playerStatus.CurrentHP, playerStatus.MaxHP);
        }

        // ゲージの上昇コルーチンを開始
        StartCoroutine(IncreaseGaugeOverTime());
    }

    private void Update()
    {
        if (!isPlayerAlive) return;

        // ゲージ値に応じてプレイヤーの速度を調整
        if (playerMove != null)
        {
            float speedMultiplier = Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, currentGauge / maxGauge);
            playerMove.SetSpeedMultiplier(speedMultiplier);
        }
    }

    /// <summary>
    /// HPが変化したときの処理
    /// </summary>
    private void OnHealthChanged(int currentHP, int maxHP)
    {
        UpdateBarUI(currentHP, maxHP);

        // プレイヤーが死亡した場合、ゲージ処理を停止
        if (currentHP <= 0)
        {
            isPlayerAlive = false;
            StopAllCoroutines();
        }
    }

    /// <summary>
    /// HPと憤度ゲージを同一バーとして更新
    /// </summary>
    private void UpdateBarUI(int currentHP, int maxHP)
    {
        if (barImage != null)
        {
            // HPに基づいたゲージのfillAmountを設定
            float fillAmount = (float)currentHP / maxHP;  // ゲージの割合を直接計算
            barImage.fillAmount = fillAmount;
        }
    }

    /// <summary>
    /// ゲージを時間経過で増加させる
    /// </summary>
    private IEnumerator IncreaseGaugeOverTime()
    {
        while (true)
        {
            // ゲージを時間経過で増加
            if (currentGauge < maxGauge) // 最大値を超えないように確認
            {
                currentGauge += gaugeIncreaseRate * Time.deltaTime;
            }

            // ゲージの増加状況を確認
            Debug.Log("Current Gauge: " + currentGauge);

            // ゲージを上限にクランプ
            currentGauge = Mathf.Clamp(currentGauge, 0, maxGauge);

            // UIの更新
            UpdateBarUI(playerStatus.CurrentHP, playerStatus.MaxHP);

            yield return null; // 1フレーム待機
        }
    }
}

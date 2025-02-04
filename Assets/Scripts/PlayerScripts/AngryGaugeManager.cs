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
    [SerializeField] private float minSpeedMultiplier = 0.5f; // 速度の最小倍率
    [SerializeField] private float maxSpeedMultiplier = 5f;   // 速度の最大倍率

    private float currentGauge;  // 現在のゲージ値 (HPに基づく)

    private void Start()
    {
        if (playerStatus != null)
        {
            // HPイベントに登録
            playerStatus.HealthChanged += OnHealthChanged;

            // 初期表示を更新
            UpdateBarUI(playerStatus.CurrentHP, playerStatus.MaxHP);
        }
    }

    private void Update()
    {
        // ゲージ値に応じてプレイヤーの速度を調整
        if (playerMove != null)
        {
            float speedMultiplier = Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, currentGauge / playerStatus.MaxHP);
            playerMove.SetSpeedMultiplier(speedMultiplier);
        }
    }

    /// <summary>
    /// HPが変化した際に呼ばれるイベントハンドラ。
    /// ゲージの値を更新し、UIを変更する。
    /// </summary>
    /// <param name="currentHP">プレイヤーの現在のHP</param>
    /// <param name="maxHP">プレイヤーの最大HP</param>
    private void OnHealthChanged(int currentHP, int maxHP)
    {
        UpdateBarUI(currentHP, maxHP);
    }

    /// <summary>
    /// HPの変化をUIに反映し、ゲージ値を更新する。
    /// </summary>
    /// <param name="currentHP">プレイヤーの現在のHP</param>
    /// <param name="maxHP">プレイヤーの最大HP</param>
    private void UpdateBarUI(int currentHP, int maxHP)
    {
        if (barImage != null)
        {
            // HPに基づいたゲージの割合を計算
            float fillAmount = (float)currentHP / maxHP;
            barImage.fillAmount = fillAmount;

            // ゲージの内部値を更新
            currentGauge = currentHP;
        }
    }
}

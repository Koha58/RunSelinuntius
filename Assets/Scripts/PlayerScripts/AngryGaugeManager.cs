using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 憤度(PlayerのHP＆速度)管理用クラス
/// </summary>
public class AngryGaugeManager : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus; // プレイヤーのHP
    [SerializeField] private Image healthBarImage;      // HPを表示するImage (塗りつぶしImage)

    private void Start()
    {
        if (playerStatus != null)
        {
            // イベントに登録
            playerStatus.HealthChanged += UpdateHealthUI;

            // 初期表示を更新
            UpdateHealthUI(playerStatus.MaxHP, playerStatus.MaxHP);
        }
    }

    private void UpdateHealthUI(int currentHP, int maxHP)
    {
        if (healthBarImage != null)
        {
            // HPの割合をImageのfillAmountに反映
            healthBarImage.fillAmount = (float)currentHP / maxHP;
        }
    }
}

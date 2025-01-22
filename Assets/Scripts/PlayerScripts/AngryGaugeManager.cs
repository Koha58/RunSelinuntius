using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���x(Player��HP�����x)�Ǘ��p�N���X
/// </summary>
public class AngryGaugeManager : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus; // �v���C���[��HP
    [SerializeField] private Image healthBarImage;      // HP��\������Image (�h��Ԃ�Image)

    private void Start()
    {
        if (playerStatus != null)
        {
            // �C�x���g�ɓo�^
            playerStatus.HealthChanged += UpdateHealthUI;

            // �����\�����X�V
            UpdateHealthUI(playerStatus.MaxHP, playerStatus.MaxHP);
        }
    }

    private void UpdateHealthUI(int currentHP, int maxHP)
    {
        if (healthBarImage != null)
        {
            // HP�̊�����Image��fillAmount�ɔ��f
            healthBarImage.fillAmount = (float)currentHP / maxHP;
        }
    }
}

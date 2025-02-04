using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���x(Player��HP�����x)�Ǘ��p�N���X
/// </summary>
public class AngryGaugeManager : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;  // �v���C���[��HP
    [SerializeField] private PlayerMove playerMove;      // �v���C���[�̓���
    [SerializeField] private Image barImage;             // HP�����x��\������Image (1��Image�����L)

    [Header("�Q�[�W�ݒ�")]
    [SerializeField] private float minSpeedMultiplier = 0.5f; // ���x�̍ŏ��{��
    [SerializeField] private float maxSpeedMultiplier = 5f;   // ���x�̍ő�{��

    private float currentGauge;  // ���݂̃Q�[�W�l (HP�Ɋ�Â�)

    private void Start()
    {
        if (playerStatus != null)
        {
            // HP�C�x���g�ɓo�^
            playerStatus.HealthChanged += OnHealthChanged;

            // �����\�����X�V
            UpdateBarUI(playerStatus.CurrentHP, playerStatus.MaxHP);
        }
    }

    private void Update()
    {
        // �Q�[�W�l�ɉ����ăv���C���[�̑��x�𒲐�
        if (playerMove != null)
        {
            float speedMultiplier = Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, currentGauge / playerStatus.MaxHP);
            playerMove.SetSpeedMultiplier(speedMultiplier);
        }
    }

    /// <summary>
    /// HP���ω������ۂɌĂ΂��C�x���g�n���h���B
    /// �Q�[�W�̒l���X�V���AUI��ύX����B
    /// </summary>
    /// <param name="currentHP">�v���C���[�̌��݂�HP</param>
    /// <param name="maxHP">�v���C���[�̍ő�HP</param>
    private void OnHealthChanged(int currentHP, int maxHP)
    {
        UpdateBarUI(currentHP, maxHP);
    }

    /// <summary>
    /// HP�̕ω���UI�ɔ��f���A�Q�[�W�l���X�V����B
    /// </summary>
    /// <param name="currentHP">�v���C���[�̌��݂�HP</param>
    /// <param name="maxHP">�v���C���[�̍ő�HP</param>
    private void UpdateBarUI(int currentHP, int maxHP)
    {
        if (barImage != null)
        {
            // HP�Ɋ�Â����Q�[�W�̊������v�Z
            float fillAmount = (float)currentHP / maxHP;
            barImage.fillAmount = fillAmount;

            // �Q�[�W�̓����l���X�V
            currentGauge = currentHP;
        }
    }
}

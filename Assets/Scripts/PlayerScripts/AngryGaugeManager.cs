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
    [SerializeField] private float maxGauge = 100f;      // �Q�[�W�̍ő�l
    [SerializeField] private float gaugeIncreaseRate = 1f; // �Q�[�W�̏㏸���x (���b�̏㏸��)
    [SerializeField] private float minSpeedMultiplier = 0.5f; // ���x�̍ŏ��{��
    [SerializeField] private float maxSpeedMultiplier = 3f;   // ���x�̍ő�{��

    private float currentGauge;  // ���݂̃Q�[�W�l
    private bool isPlayerAlive = true; // �v���C���[���������Ă��邩�ǂ���

    private void Start()
    {
        if (playerStatus != null)
        {
            // HP�C�x���g�ɓo�^
            playerStatus.HealthChanged += OnHealthChanged;

            // �����\�����X�V
            UpdateBarUI(playerStatus.CurrentHP, playerStatus.MaxHP);
        }

        // �Q�[�W�̏㏸�R���[�`�����J�n
        StartCoroutine(IncreaseGaugeOverTime());
    }

    private void Update()
    {
        if (!isPlayerAlive) return;

        // �Q�[�W�l�ɉ����ăv���C���[�̑��x�𒲐�
        if (playerMove != null)
        {
            float speedMultiplier = Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, currentGauge / maxGauge);
            playerMove.SetSpeedMultiplier(speedMultiplier);
        }
    }

    /// <summary>
    /// HP���ω������Ƃ��̏���
    /// </summary>
    private void OnHealthChanged(int currentHP, int maxHP)
    {
        UpdateBarUI(currentHP, maxHP);

        // �v���C���[�����S�����ꍇ�A�Q�[�W�������~
        if (currentHP <= 0)
        {
            isPlayerAlive = false;
            StopAllCoroutines();
        }
    }

    /// <summary>
    /// HP�ƕ��x�Q�[�W�𓯈�o�[�Ƃ��čX�V
    /// </summary>
    private void UpdateBarUI(int currentHP, int maxHP)
    {
        if (barImage != null)
        {
            // HP�Ɋ�Â����Q�[�W��fillAmount��ݒ�
            float fillAmount = (float)currentHP / maxHP;  // �Q�[�W�̊����𒼐ڌv�Z
            barImage.fillAmount = fillAmount;
        }
    }

    /// <summary>
    /// �Q�[�W�����Ԍo�߂ő���������
    /// </summary>
    private IEnumerator IncreaseGaugeOverTime()
    {
        while (true)
        {
            // �Q�[�W�����Ԍo�߂ő���
            if (currentGauge < maxGauge) // �ő�l�𒴂��Ȃ��悤�Ɋm�F
            {
                currentGauge += gaugeIncreaseRate * Time.deltaTime;
            }

            // �Q�[�W�̑����󋵂��m�F
            Debug.Log("Current Gauge: " + currentGauge);

            // �Q�[�W������ɃN�����v
            currentGauge = Mathf.Clamp(currentGauge, 0, maxGauge);

            // UI�̍X�V
            UpdateBarUI(playerStatus.CurrentHP, playerStatus.MaxHP);

            yield return null; // 1�t���[���ҋ@
        }
    }
}

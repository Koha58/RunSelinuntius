using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �v���C���[�̃X�e�[�^�X���Ǘ�����N���X
/// HP�̊Ǘ���񕜏������s��
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    [Header("HP�ݒ�")]
    [SerializeField] private int maxHP = 100; // �v���C���[�̍ő�HP
    private int currentHP;                   // ���݂�HP
    [SerializeField] private float recoveryRate = 0.5f; // ���b��HP�񕜑��x
    private float recoveryBuffer = 0f;       // �����_�ȉ��̉񕜗ʂ�ێ�����o�b�t�@

    // HP�̍ő�l�ƌ��ݒl�����J����v���p�e�B
    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    // HP�̕ω���ʒm����C�x���g
    public delegate void OnHealthChanged(int current, int max);
    public event OnHealthChanged HealthChanged;

    /// <summary>
    /// ����������
    /// HP���ő�l�̔����ɐݒ肵�A�����l��ʒm
    /// </summary>
    private void Awake()
    {
        // HP���ő�l�̔����ɐݒ�
        currentHP = maxHP / 2;

        // HP�̏�����Ԃ�ʒm
        HealthChanged?.Invoke(currentHP, maxHP);
    }

    /// <summary>
    /// HP�񕜏������J�n
    /// </summary>
    private void Start()
    {
        StartCoroutine(RecoverHP());
    }

    /// <summary>
    /// HP�����X�ɉ񕜂�����R���[�`��
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator RecoverHP()
    {
        while (true)
        {
            // HP���ő�l�����̏ꍇ�ɉ񕜏��������s
            if (currentHP < maxHP)
            {
                // �񕜃o�b�t�@�ɉ񕜗ʂ����Z
                recoveryBuffer += recoveryRate * Time.deltaTime;

                // �o�b�t�@��1�ȏ�̏ꍇ�A����������HP�ɉ��Z
                if (recoveryBuffer >= 1f)
                {
                    int increase = Mathf.FloorToInt(recoveryBuffer);
                    recoveryBuffer -= increase; // �����������c��
                    currentHP += increase;
                    currentHP = Mathf.Clamp(currentHP, 0, maxHP); // HP���ő�l�Ő���

                    // HP�ω���ʒm
                    HealthChanged?.Invoke(currentHP, maxHP);
                }
            }

            // ���̃t���[���܂őҋ@
            yield return null;
        }
    }

    /// <summary>
    /// �_���[�W���󂯂��Ƃ��̏���
    /// </summary>
    /// <param name="damage">�󂯂�_���[�W��</param>
    public void TakeDamage(int damage)
    {
        // HP�����������A0�����ɂȂ�Ȃ��悤����
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        // HP�ω���ʒm
        HealthChanged?.Invoke(currentHP, maxHP);

        // HP��0�ȉ��̏ꍇ�A���S�������Ăяo��
        if (currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// �v���C���[�����S�����Ƃ��̏���
    /// </summary>
    private void Die()
    {
        Debug.Log("Player has died!");
        SceneManager.LoadScene("GameOverScene");
    }
}

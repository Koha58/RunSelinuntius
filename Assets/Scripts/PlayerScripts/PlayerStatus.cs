using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player�̃X�e�[�^�X�Ǘ��p�N���X
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    [Header("HP�ݒ�")]
    [SerializeField] private int maxHP = 100; // �ő�HP
    private int currentHP;                   // ���݂�HP

    public int MaxHP => maxHP; // ���J�p�v���p�e�B
    public int CurrentHP => currentHP; // ���݂�HP�����J

    public delegate void OnHealthChanged(int current, int max);
    public event OnHealthChanged HealthChanged; // HP�ω��C�x���g

    private void Awake()
    {
        // ������
        currentHP = maxHP;
    }

    /// <summary>
    /// �_���[�W���󂯂鏈��
    /// </summary>
    /// <param name="damage">�_���[�W��</param>
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        // HP�ω��C�x���g��ʒm
        HealthChanged?.Invoke(currentHP, maxHP);

        // HP��0�ɂȂ����玀�S�������Ăяo��
        if (currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// ���S����
    /// </summary>
    private void Die()
    {
        Debug.Log("Player has died!");
        // �����Ŏ��S���̏��������� (�A�j���[�V�����A���X�|�[���Ȃ�)
    }
}

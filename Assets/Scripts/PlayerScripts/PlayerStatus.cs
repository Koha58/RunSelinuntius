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

    [Header("�T�E���h�ݒ�")]
    [SerializeField] private AudioClip damageSound; // �_���[�W��
    private AudioSource audioSource;               // AudioSource�R���|�[�l���g

    [Header("�v���C���[�_�Őݒ�")]
    [SerializeField] private Renderer playerRenderer; // �v���C���[��Renderer
    [SerializeField] private float flashDuration = 0.1f; // �_�ł̊Ԋu
    [SerializeField] private int flashCount = 5;         // �_�ł̉�

    private bool isFlashing = false;  // �_�Œ����ǂ����������t���O

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

        // AudioSource�R���|�[�l���g���擾
        audioSource = GetComponent<AudioSource>();
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

        // �_�Œ��̏ꍇ�A�_���[�W�������X�L�b�v
        if (isFlashing)
        {
            return;
        }

        // HP�����������A0�����ɂȂ�Ȃ��悤����
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        // �T�E���h�Đ�
        if (audioSource != null && damageSound != null)
        {
            Debug.Log("Playing damage sound");  // �T�E���h���Đ������^�C�~���O���m�F���邽�߂̃��O
            audioSource.PlayOneShot(damageSound);
        }
        else
        {
            Debug.LogWarning("AudioSource or damageSound is null!");
        }

        // �v���C���[�̓_�ŏ���
        StartCoroutine(FlashPlayer());

        // HP�ω���ʒm
        HealthChanged?.Invoke(currentHP, maxHP);

        // HP��0�ȉ��̏ꍇ�A���S�������Ăяo��
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashPlayer()
    {
        isFlashing = true; // �_�Œ��t���O�𗧂Ă�

        for (int i = 0; i < flashCount; i++)
        {
            if (playerRenderer != null)
            {
                playerRenderer.enabled = false; // �v���C���[���\��
                yield return new WaitForSeconds(flashDuration);
                playerRenderer.enabled = true;  // �v���C���[��\��
                yield return new WaitForSeconds(flashDuration);
            }
        }

        isFlashing = false; // �_�ŏI����A�t���O��߂�
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

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// �v���C���[�̓����𐧌䂷��N���X
/// </summary>
public class PlayerMove : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    [SerializeField] private Animator animator;        // �v���C���[�̃A�j���[�^�[
    [SerializeField] private float forwardSpeed = 5f;  // �O�����ړ����x
    [SerializeField] private float moveSpeed = 5f;     // ���ړ����x
    [SerializeField] private float jumpForce = 7f;     // �W�����v��
    [SerializeField] private LayerMask groundLayer;    // �n�ʂ̃��C���[

    [Header("�n�ʔ���ݒ�")]
    [SerializeField] private float groundCheckRadius = 0.1f; // �n�ʃ`�F�b�N�̔��a

    [Header("�A�j���[�V�����ݒ�")]
    [SerializeField] private float jumpAnimationDuration = 0.8f; // �W�����v�A�j���[�V�����I���܂ł̒x������

    [Header("���ԑ���ݒ�")]
    [SerializeField] private float slowMotionScale = 0.5f; // �X���[���[�V�������̑��x�{��
    [SerializeField] private float normalTimeScale = 1f; // �ʏ펞�̑��x�{��
    [SerializeField] private float defaultFixedDeltaTime = 0.02f; // �f�t�H���g�� FixedDeltaTime

    private float speedMultiplier = 1f;               // ���x�{��
    private float horizontalVelocity;                 // �������̈ړ����x
    private Rigidbody playerRigidbody;               // �v���C���[��Rigidbody
    private bool isGrounded;                          // �n�ʂɂ��邩�ǂ���

    private bool isNearTarget = false; // FinalAttack�\�͈͓��ɂ��邩

    /// <summary>
    /// �R���|�[�l���g�̏�����
    /// </summary>
    private void Awake()
    {
        // Rigidbody�R���|�[�l���g���擾
        playerRigidbody = GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            Debug.LogError("Rigidbody ��������܂���I �v���C���[�� Rigidbody �R���|�[�l���g���A�^�b�`���Ă��������B");
        }
    }

    /// <summary>
    /// �ړ����͂��󂯎��
    /// </summary>
    /// <param name="value">���͒l (Vector2)</param>
    private void OnMove(InputValue value)
    {
        // �W�����v���͈ړ��𖳌���
        if (!isGrounded)
        {
            horizontalVelocity = 0;
            return;
        }
        // ���X�e�B�b�N�̓��͒l���擾
        Vector2 axis = value.Get<Vector2>();

        // �������̈ړ����x��ێ�
        horizontalVelocity = axis.x;
    }

    /// <summary>
    /// �W�����v���͂��󂯎��
    /// </summary>
    /// <param name="value">���͒l (isPressed)</param>
    private void OnJump(InputValue value)
    {
        // �W�����v�{�^����������Ă��āA���n�ʂɂ���ꍇ�̂݃W�����v�����s
        if (isGrounded && value.isPressed)
        {
            // �A�j���[�V�������W�����v��Ԃɂ���
            if (animator != null)
            {
                animator.SetBool("Jump", true);
            }

            // Rigidbody �ɃW�����v�̗͂�������
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // �W�����v���͒n�ʂɂ��Ȃ���Ԃɐݒ�
            isGrounded = false;

            // ��莞�Ԍ�ɃW�����v�A�j���[�V�������I������
            StartCoroutine(SetJumpFalseAfterDelay(jumpAnimationDuration));
        }
    }

    /// <summary>
    /// �U�����͂��󂯎��
    /// </summary>
    /// <param name="value">���͒l (isPressed)</param>
    private void OnFinalAttack(InputValue value)
    {
        // FinalAttack�͈͓����A�{�^���������ꂽ�ꍇ�̂ݍU�������s
        if (isNearTarget && value.isPressed)
        {
            // �U�����������s
            Debug.Log("Final Attack!");
            SceneManager.LoadScene("GameClearScene");
        }
    }

    /// <summary>
    /// ���t���[���Ăяo����鏈��
    /// �������ƑO�����Ɉړ�������
    /// </summary>
    private void Update()
    {
        // �������ƑO�����Ɉړ�
        Vector3 movement = new Vector3(horizontalVelocity * moveSpeed * speedMultiplier, 0, forwardSpeed * speedMultiplier) * Time.deltaTime;
        transform.position += movement;
    }

    /// <summary>
    /// ���Ԋu�ŌĂяo����镨���v�Z
    /// �n�ʂɂ��邩�ǂ����𔻒�
    /// </summary>
    private void FixedUpdate()
    {
        // �n�ʂɂ��邩�ǂ������m�F
        isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);
    }

    /// <summary>
    /// �n�ʃ`�F�b�N�p�̉�������
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }

    /// <summary>
    /// ��莞�Ԍ�ɃW�����v�A�j���[�V�������I�����A�n�ʂɂ����Ԃɖ߂�
    /// </summary>
    /// <param name="delay">�x������ (�b)</param>
    private IEnumerator SetJumpFalseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // �W�����v�A�j���[�V�������I��
        if (animator != null)
        {
            animator.SetBool("Jump", false);
        }

        // �n�ʂɂ����Ԃɐݒ�
        isGrounded = true;
    }

    //// <summary>
    /// ���x�{����ݒ肷��
    /// </summary>
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    /// <summary>
    /// ���݂̑��x���擾
    /// </summary>
    public float GetCurrentSpeed()
    {
        return moveSpeed * speedMultiplier;
    }

    /// <summary>
    /// FinalAttack ���\���𔻒肷�郁�\�b�h
    /// </summary>
    internal bool IsFinalAttackPossible()
    {
        return isNearTarget;
    }

    /// <summary>
    /// Target �ɐG�ꂽ��(FinalAttack�\�Ȃ�)�Q�[���̓�����x������
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            isNearTarget = true;
            SetSlowMotion(true);
        }
    }

    /// <summary>
    /// Target ���痣�ꂽ�猳�̑��x�ɖ߂�
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            isNearTarget = false;
            SetSlowMotion(false);
        }
    }

    /// <summary>
    /// �V�[���S�̂̃X���[���[�V������ݒ肷��
    /// </summary>
    /// <param name="isSlow">true�Ȃ�x���Afalse�Ȃ�ʏ푬�x</param>
    private void SetSlowMotion(bool isSlow)
    {
        if (isSlow)
        {
            Time.timeScale = slowMotionScale;
            Time.fixedDeltaTime = defaultFixedDeltaTime * slowMotionScale;
            speedMultiplier = slowMotionScale;
        }
        else
        {
            Time.timeScale = normalTimeScale;
            Time.fixedDeltaTime = defaultFixedDeltaTime;
            speedMultiplier = normalTimeScale;
        }
    }
}
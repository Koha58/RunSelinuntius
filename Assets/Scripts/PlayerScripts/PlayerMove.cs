using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// �v���C���[�̓����𐧌䂷��N���X
/// </summary>
public class PlayerMove : MonoBehaviour
{
    // �萔�̐錾
    private const float DefaultForwardSpeed = 5f;  // �O�����ړ����x
    private const float DefaultMoveSpeed = 5f;     // ���ړ����x
    private const float DefaultJumpForce = 7f;     // �W�����v��
    private const float DefaultGroundCheckRadius = 0.1f; // �n�ʃ`�F�b�N�̔��a
    private const float DefaultJumpAnimationDuration = 0.8f; // �W�����v�A�j���[�V�����I���܂ł̒x������
    private const float DefaultSlowMotionScale = 0.01f; // �X���[���[�V�������̑��x�{��
    private const float DefaultNormalTimeScale = 1f; // �ʏ펞�̑��x�{��
    private const float DefaultFixedDeltaTime = 0.02f; // �f�t�H���g�� FixedDeltaTime
    private const float TargetSpeedMultiplier = 5f; // Target�̑��x��ύX����{��

    [Header("�ړ��ݒ�")]
    [SerializeField] private Animator animator;        // �v���C���[�̃A�j���[�^�[
    [SerializeField] private float forwardSpeed = DefaultForwardSpeed;  // �O�����ړ����x
    [SerializeField] private float moveSpeed = DefaultMoveSpeed;     // ���ړ����x
    [SerializeField] private float jumpForce = DefaultJumpForce;     // �W�����v��
    [SerializeField] private LayerMask groundLayer;    // �n�ʂ̃��C���[

    [Header("�n�ʔ���ݒ�")]
    [SerializeField] private float groundCheckRadius = DefaultGroundCheckRadius; // �n�ʃ`�F�b�N�̔��a

    [Header("�A�j���[�V�����ݒ�")]
    [SerializeField] private float jumpAnimationDuration = DefaultJumpAnimationDuration; // �W�����v�A�j���[�V�����I���܂ł̒x������

    [Header("���ԑ���ݒ�")]
    [SerializeField] private float slowMotionScale = DefaultSlowMotionScale; // �X���[���[�V�������̑��x�{��
    [SerializeField] private float normalTimeScale = DefaultNormalTimeScale; // �ʏ펞�̑��x�{��
    [SerializeField] private float defaultFixedDeltaTime = DefaultFixedDeltaTime; // �f�t�H���g�� FixedDeltaTime

    [Header("FinalAttackSE�ݒ�")]
    [SerializeField] private FinalAttackSEControl seControl; // FinalAttack �� SE ����

    private float speedMultiplier = 1f;               // ���x�{��
    private float horizontalVelocity;                 // �������̈ړ����x
    private Rigidbody playerRigidbody;               // �v���C���[��Rigidbody
    private bool isGrounded;                          // �n�ʂɂ��邩�ǂ���

    private bool isNearTarget = false; // FinalAttack�\�͈͓��ɂ��邩
    private bool isFinalAttackSEPlayed = false; // FinalAttack��SE���Đ����ꂽ���ǂ������Ǘ�

    private const int TargetFPS = 60;  // �t���[�����[�g�̖ڕW�l
    private const int VSyncDisabled = 0; // VSync �𖳌��ɂ���l

    /// <summary>
    /// �R���|�[�l���g�̏�����
    /// </summary>
    private void Awake()
    {
        // �t���[�����[�g��VSync�̐ݒ�
        QualitySettings.vSyncCount = VSyncDisabled;
        Application.targetFrameRate = TargetFPS;

        // Rigidbody�R���|�[�l���g���擾
        playerRigidbody = GetComponent<Rigidbody>();

        // FinalAttack�\�͈͓��ɂ��Ȃ�
        isNearTarget = false;
        SetSlowMotion(false);

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
            // ���ł�SE���Đ�����Ă����珈�����X�L�b�v
            if (isFinalAttackSEPlayed)
            {
                return;
            }

            Debug.Log("Final Attack!");

            // SE ��炵�A���̒������擾
            float seDuration = seControl != null ? seControl.PlayFinalAttackSE() : 0f;

            // SE���Đ����ꂽ���Ƃ��L�^
            isFinalAttackSEPlayed = true;

            // �v���C���[�̓������~�߂�
            StopPlayerMovement();

            // �^�[�Q�b�g�̓������~�߂�
            StopTargetMovement();

            // SE ����I���܂ő҂��Ă���V�[���J��
            StartCoroutine(WaitAndLoadScene(seDuration));
        }
    }

    /// <summary>
    /// �v���C���[�̓������~�߂�
    /// </summary>
    private void StopPlayerMovement()
    {
        speedMultiplier = 0f;
        playerRigidbody.velocity = Vector3.zero; // �����I�ȓ������~�߂�
    }

    /// <summary>
    /// �V�[�����̂��ׂĂ� Target �̓������~�߂�
    /// </summary>
    private void StopTargetMovement()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject target in targets)
        {
            TargetMove targetMove = target.GetComponent<TargetMove>();
            if (targetMove != null)
            {
                targetMove.SetSpeedMultiplier(0f); // �^�[�Q�b�g�̈ړ����x��0�ɂ���
            }
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

    /// <summary>
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
        Debug.Log($"OnTriggerEnter: {other.gameObject.name}");

        if (other.CompareTag("Target"))
        {
            isNearTarget = true;
            SetSlowMotion(true);
        }

        if (other.CompareTag("RunAway"))
        {
            isNearTarget = false;
            SetSlowMotion(false);

            // runAway �� SE ��炷
            if (seControl != null)
            {
                seControl.PlayrunAwaySE();
            }

            // �V�[�����̂��ׂĂ� "Target" �^�O�������I�u�W�F�N�g���擾���A�����̑��x��ύX
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
            foreach (GameObject target in targets)
            {
                TargetMove targetMove = target.GetComponent<TargetMove>();
                if (targetMove != null)
                {
                    targetMove.SetSpeedMultiplier(TargetSpeedMultiplier); // ���x��ύX
                }
            }
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

    /// <summary>
    /// �w�肳�ꂽ���ԑҋ@������A�w��̃V�[���ɑJ�ڂ���R���[�`��
    /// </summary>
    /// <param name="waitTime">�V�[���J�ڑO�ɑҋ@���鎞�ԁi�b�j</param>
    private IEnumerator WaitAndLoadScene(float waitTime)
    {
        // �w�肳�ꂽ���Ԃ����ҋ@�iFinalAttack �� SE �̒�����z��j
        yield return new WaitForSeconds(waitTime);

        // SE �̍Đ�������������ɁAGameClearScene �֑J��
        SceneManager.LoadScene("GameClearScene");
    }
}
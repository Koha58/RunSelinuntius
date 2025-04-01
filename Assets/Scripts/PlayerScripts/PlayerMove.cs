using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Player�̓������Ǘ�����N���X
/// </summary>
public class PlayerMove : MonoBehaviour
{
    // �萔�̐錾
    private const float DefaultForwardSpeed = 5f;  // �O�����ړ����x
    private const float DefaultMoveSpeed = 5f;     // ���ړ����x
    private const float DefaultJumpForce = 7f;     // �W�����v��
    private const float DefaultGroundCheckRadius = 0.1f; // �n�ʃ`�F�b�N�̔��a
    private const float DefaultJumpAnimationDuration = 0.8f; // �W�����v�A�j���[�V�����I���܂ł̒x������
    private const float DefaultNormalTimeScale = 1f; // �ʏ펞�̑��x�{��
    private const float DefaultFixedDeltaTime = 0.0167f; // �f�t�H���g�� FixedDeltaTime
    private const float TargetSpeedMultiplier = 5f; // Target�̑��x��ύX����{��
    private const float MinFixedDeltaTime = 0.002f; // �Œ�Œ�t���[�����[�g
    private const float MinSpeedMultiplier = 0.5f; // �ŏI�I�ȃv���C���[�̑��x�{��
    private const float SpeedMultiplierLerpFactor = 0.1f; // �X���[�W���O�W��
    private const float ZeroSpeedMultiplier = 0f; // ���x�{�����[���ɂ���l
    private const float DefaultHorizontalVelocity = 0f; // �������̏������x�i0�j

    private const string GameClearSceneName = "GameClearScene";  // �Q�[���N���A�V�[���̖��O
    private const float ZeroTimeScale = 0f; // �Q�[�������S�ɒ�~���鎞�� TimeScale
    private const float SlowMotionSpeedMultiplier = 0.1f; // �X���[���[�V�������̔{��

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

    private GameObject[] targets;�@// Target�I�u�W�F�N�g�̔z��B�Q�[�����Ń^�[�Q�b�g�ƂȂ�I�u�W�F�N�g���i�[
    private bool setSlowMotion;�@  // �X���[���[�V�����̐ݒ��ێ�����t���O�B�X���[���[�V�������L�����ǂ����𐧌�
    private bool isFinalAttackTriggered = false; // FinalAttack�������������ǂ���

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
    /// �^�[�Q�b�g���擾
    /// </summary>
    private void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("Target");
    }

    /// <summary>
    /// �ړ����͂��󂯎��
    /// </summary>
    private void OnMove(InputValue value)
    {
        // �W�����v���͈ړ��𖳌���
        if (!isGrounded)
        {
            horizontalVelocity = DefaultHorizontalVelocity;
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

            isFinalAttackTriggered = true; // FinalAttack�����t���O��ON

            Time.timeScale = DefaultNormalTimeScale;

            // SE ����I���܂ő҂��Ă���V�[���J��
            StartCoroutine(WaitAndLoadScene(seDuration));
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
    /// ��莞�Ԍ�ɃW�����v�A�j���[�V�������I�����A�n�ʂɂ����Ԃɖ߂�
    /// </summary>
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

        if (other.CompareTag("RunAway") && !isFinalAttackTriggered)
        {
            isNearTarget = false;
            SetSlowMotion(false);

            // runAway �� SE ��炷
            if (seControl != null)
            {
                seControl.PlayrunAwaySE();
            }

            foreach (GameObject target in targets)
            {
                TargetMove targetMove = target.GetComponent<TargetMove>();
                if (targetMove != null)
                {
                    targetMove.SetSpeedMultiplier(TargetSpeedMultiplier);
                }
            }
        }
    }

    /// <summary>
    /// �V�[���S�̂̃X���[���[�V������ݒ肷��
    /// </summary>
    private void SetSlowMotion(bool isSlow)
    {
        setSlowMotion = isSlow;

        if (isSlow)
        {
            // Time.timeScale ���X�ɏ��������Ď��ԑS�̂�x������
            Time.timeScale = SlowMotionSpeedMultiplier; // �����Ŏ��Ԃ̗����x������
            Time.fixedDeltaTime = Mathf.Max(DefaultFixedDeltaTime * Time.timeScale, MinFixedDeltaTime); // �Œ�t���[�����[�g�̒���

            // speedMultiplier ������ɒႭ�ݒ肵�āA�v���C���[�̓�����x������
            speedMultiplier = Mathf.Lerp(speedMultiplier, MinSpeedMultiplier, SpeedMultiplierLerpFactor); // ���x���Ȃ�悤�ɒ���
        }
        else
        {
            // �ʏ푬�x�ɖ߂�
            Time.timeScale = DefaultNormalTimeScale;
            Time.fixedDeltaTime = DefaultFixedDeltaTime;
            speedMultiplier = DefaultNormalTimeScale;
        }
    }

    /// <summary>
    /// �X���[���[�V�������L�����ǂ������m�F����
    /// </summary>
    /// <returns>
    /// �X���[���[�V�������L���ł���� true�A����ȊO�� false ��Ԃ�
    /// </returns>
    public bool IsSlowMotionEnabled()
    {
        return setSlowMotion;
    }

    /// <summary>
    /// �w�肳�ꂽ���ԑҋ@������A�w��̃V�[���ɑJ�ڂ���R���[�`��
    /// </summary>
    private IEnumerator WaitAndLoadScene(float waitTime)
    {
        // �Q�[���S�̂��~�i�����v�Z��Update�������܂߂�j
        Time.timeScale = ZeroTimeScale;

        // �v���C���[�ƃ^�[�Q�b�g�̓��������S�Ɏ~�߂�
        StopPlayerMovement();
        StopTargetMovement();

        // SE �̍Đ����Ԃ����A���^�C���ő҂iUnscaledTime ���g�p�j
        yield return new WaitForSecondsRealtime(waitTime);

        // �V�[���J�ځiTime.timeScale ��߂����ɑJ�ڂ���j
        SceneManager.LoadScene(GameClearSceneName);
    }

    /// <summary>
    /// �v���C���[�̓������~�߂�
    /// </summary>
    private void StopPlayerMovement()
    {
        speedMultiplier = ZeroSpeedMultiplier;
        playerRigidbody.velocity = Vector3.zero; // �����I�ȓ������~�߂�
    }

    /// <summary>
    /// �V�[�����̂��ׂĂ� Target �̓������~�߂�
    /// </summary>
    private void StopTargetMovement()
    {
        foreach (GameObject target in targets)
        {
            TargetMove targetMove = target.GetComponent<TargetMove>();
            if (targetMove != null)
            {
                targetMove.SetSpeedMultiplier(ZeroSpeedMultiplier);
            }
        }
    }
}

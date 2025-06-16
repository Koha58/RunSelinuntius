using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Player�̓������Ǘ�����N���X
/// </summary>
public class PlayerMove : MonoBehaviour
{
    #region �萔�i�Q�[���i�s�E���Ԑ���E�ړ��j

    private const float DefaultNormalTimeScale = 1f;     // �ʏ펞�̑��x�{��
    private const float ZeroTimeScale = 0f;              // �Q�[�������S�ɒ�~���鎞�� TimeScale
    private const float MinFixedDeltaTime = 0.002f;      // �Œ�Œ�t���[�����[�g
    private const float ZeroSpeedMultiplier = 0f;        // ���x�{�����[���ɂ���l
    private const float DefaultHorizontalVelocity = 0f;  // �������̏������x�i0�j

    private const string GameClearSceneName = "GameClearScene"; // �Q�[���N���A�V�[���̖��O
    private const int TargetFPS = 60;                   // �t���[�����[�g�̖ڕW�l
    private const int VSyncDisabled = 0;                // VSync �𖳌��ɂ���l

    #endregion

    #region SerializeField�i�C���X�y�N�^�[�ݒ�j

    [Header("�ړ��ݒ�")]
    [SerializeField] private Animator animator;        // �v���C���[�̃A�j���[�^�[

    [Header("�n�ʔ���ݒ�")]
    [SerializeField] private LayerMask groundLayer;    // �n�ʂ̃��C���[
    [SerializeField] private float groundCheckRadius;  // �n�ʃ`�F�b�N�̔��a

    [Header("FinalAttackSE�ݒ�")]
    [SerializeField] private FinalAttackSEControl seControl; // FinalAttack �� SE ����

    #endregion

    #region JSON����ǂݍ��ސݒ�

    private float forwardSpeed;             // �O�����ړ����x
    private float moveSpeed;                // ���ړ����x
    private float jumpForce;                // �W�����v��
    private float jumpAnimationDuration;    // �W�����v�A�j���[�V�����̒x������

    private float defaultFixedDeltaTime = 0.0167f;     // �f�t�H���g�� FixedDeltaTime
    private float targetSpeedMultiplier;               // Target�̑��x��ύX����{��
    private float minSpeedMultiplier;                  // �ŏI�I�ȃv���C���[�̑��x�{��
    private float speedMultiplierLerpFactor = 0.1f;    // �X���[�W���O�W��
    private float slowMotionSpeedMultiplier = 0.1f;    // �X���[���[�V�������̔{��

    #endregion

    #region �v���C���[���

    private float speedMultiplier = 1f;         // ���݂̑��x�{��
    private float horizontalVelocity;           // �������̈ړ����x
    private Rigidbody playerRigidbody;          // �v���C���[��Rigidbody
    private bool isGrounded;                    // �n�ʂɂ��邩�ǂ���

    private bool isNearTarget = false;          // FinalAttack�\�͈͓��ɂ��邩
    private bool isFinalAttackSEPlayed = false; // FinalAttack��SE���Đ����ꂽ��
    private bool isFinalAttackTriggered = false; // FinalAttack������������
    private bool setSlowMotion;                 // �X���[���[�V�����̐ݒ�

    #endregion

    #region �Q�[���I�u�W�F�N�g�Q��

    private GameObject[] targets; // �Q�[�����^�[�Q�b�g�I�u�W�F�N�g�̔z��

    #endregion

    // �O��������S�ɎQ�Ƃł���悤�ɂ���v���p�e�B
    public float ForwardSpeed => forwardSpeed;

    /// <summary>
    /// �R���|�[�l���g�̏�����
    /// </summary>
    private void Awake()
    {
        // Resources/Data/PlayerMoveSettings.json ��ǂݍ��ށi�g���q�s�v�j
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/PlayerMoveSettings");

        if (jsonFile != null)
        {
            // JSON�t�@�C���̃e�L�X�g���擾
            string json = jsonFile.text;

            // JSON�� PlayerMoveSettingsExport[] �z��Ƃ��ăf�V���A���C�Y�iNewtonsoft.Json�g�p�j
            PlayerMoveSettingsExport[] exports = JsonConvert.DeserializeObject<PlayerMoveSettingsExport[]>(json);

            // �z�� null �łȂ��A1�ȏ�̗v�f������Δ��f
            if (exports != null && exports.Length > 0)
            {
                // �ŏ��̐ݒ�f�[�^���g���� PlayerMoveSettings �ɕϊ�
                PlayerMoveSettings settings = exports[0].ToSettings();

                // PlayerMoveSettings �̊e�ݒ�l���A�Ή�����ϐ��ɔ��f

                // �O���ړ����x
                forwardSpeed = settings.forwardSpeed;

                // �S�̂̈ړ����x
                moveSpeed = settings.moveSpeed;

                // �W�����v��
                jumpForce = settings.jumpForce;

                // �n�ʔ���̔��a
                groundCheckRadius = settings.groundCheckRadius;

                // �W�����v�A�j���[�V�����̎���
                jumpAnimationDuration = settings.jumpAnimationDuration;

                // �ڕW���x�ւ̔{������
                targetSpeedMultiplier = settings.targetSpeedMultiplier;

                // �Œᑬ�x�{��
                minSpeedMultiplier = settings.minSpeedMultiplier;

                // ���x�{���̕�ԌW��
                speedMultiplierLerpFactor = settings.speedMultiplierLerpFactor;

                // �X���[���[�V�������̑��x�{��
                slowMotionSpeedMultiplier = settings.slowMotionSpeedMultiplier;

                // �Œ�̃f���^�^�C���i�f�t�H���g�l�j
                defaultFixedDeltaTime = settings.defaultFixedDeltaTime;
            }
            else
            {
                Debug.LogError("JSON�̃f�V���A���C�Y�Ɏ��s���܂����B");
            }
        }
        else
        {
            // JSON�t�@�C����������Ȃ������ꍇ�̃G���[���O
            Debug.LogError("PlayerMoveSettings.json �� Resources/Data �Ɍ�����܂���B");
        }

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
                    targetMove.SetSpeedMultiplier(targetSpeedMultiplier);
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
            Time.timeScale = slowMotionSpeedMultiplier; // �����Ŏ��Ԃ̗����x������
            Time.fixedDeltaTime = Mathf.Max(defaultFixedDeltaTime * Time.timeScale, MinFixedDeltaTime); // �Œ�t���[�����[�g�̒���

            // speedMultiplier ������ɒႭ�ݒ肵�āA�v���C���[�̓�����x������
            speedMultiplier = Mathf.Lerp(speedMultiplier, minSpeedMultiplier, speedMultiplierLerpFactor); // ���x���Ȃ�悤�ɒ���
        }
        else
        {
            // �ʏ푬�x�ɖ߂�
            Time.timeScale = DefaultNormalTimeScale;
            Time.fixedDeltaTime = defaultFixedDeltaTime;
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

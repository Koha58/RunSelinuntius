using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �^�[�Q�b�g�̓����𐧌䂷��N���X
/// </summary>
public class TargetMove : MonoBehaviour
{
    [Header("Target�̊�{�ݒ�")]
    [SerializeField] private Transform player; // �v���C���[��Transform
    [SerializeField] private float baseSpeed = 10.0f; // Target�̊���x
    [SerializeField] private float targetDistance = 1000.0f; // �v���C���[�Ƃ̖ڕW����

    [Header("���E�ړ��ݒ�")]
    [SerializeField] private float lateralMovementAmplitude = 5.0f; // ���E�ړ��̐U��
    [SerializeField] private float lateralMovementFrequency = 0.1f; // ���E�ړ��̕p�x
    [SerializeField] private float lateralMoveActiveDuration = 5.0f; // ���E�ړ����s�����ԁi�b�j
    [SerializeField] private float lateralMoveIdleDuration = 10.0f; // ���E�ړ����s��Ȃ����ԁi�b�j

    private const float TwoPi = Mathf.PI * 2.0f; // 2�΂̒萔�i���E�ړ��v�Z�Ɏg�p�j

    private float lateralMovementTimer = 0.0f; // ���E�ړ��̃^�C�}�[
    private bool isLateralMoving = false; // ���ݍ��E�ړ������ǂ���
    private float stateTimer = 0.0f; // ��ԕύX�̃^�C�}�[

    private Vector3 initialPosition; // �����ʒu���L�^

    private float currentSpeedMultiplier = 1.0f; // ���݂̑��x�{���i�ʏ��1�j

    private float originalSpeed; // ������Speed��ۑ�

    [Header("runAway���Player�Ƃ̋���")]
    [SerializeField] private float distanceThreshold = 500f; // �v���C���[�Ƃ̋���臒l

    /// <summary>
    /// �����ݒ���s��
    /// �v���C���[���ݒ肳��Ă��Ȃ��ꍇ�A�G���[���b�Z�[�W��\������
    /// �����ʒu���v���C���[�̑O���ɐݒ�
    /// </summary>
    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player���ݒ肳��Ă��܂���I");
            return;
        }

        // �����ʒu���v���C���[�̑O���ɔz�u
        initialPosition = player.position + Vector3.forward * targetDistance;
        transform.position = initialPosition;

        originalSpeed = baseSpeed;  // �������x��ۑ�
    }

    /// <summary>
    /// ���t���[���Ăяo����鏈��
    /// Target�̈ʒu���X�V����
    /// </summary>
    void Update()
    {
        if (player == null) return;

        // �v���C���[�̑O���Ƀ^�[�Q�b�g���ړ�������
        MaintainPositionInFrontOfPlayer();

        // ���E�ړ����Ǘ�
        ManageLateralMovement();

        // �v���C���[�Ƃ̋������`�F�b�N
        CheckDistanceToPlayer();
    }

    /// <summary>
    /// �v���C���[�̑O���Ƀ^�[�Q�b�g���ێ�
    /// �v���C���[�̈ړ��ɍ��킹�ă^�[�Q�b�g���O���ɐi��
    /// </summary>
    private void MaintainPositionInFrontOfPlayer()
    {
        // �v���C���[�̈ړ��������擾
        Vector3 forwardDirection = player.forward;

        // �v���C���[�̈ړ��ɍ��킹�ă^�[�Q�b�g���O���ɐi��
        Vector3 targetPosition = player.position + forwardDirection * targetDistance;

        // ���݂̈ʒu����ڕW�ʒu�Ɍ������Ĉ�葬�x�ňړ�
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, baseSpeed * Time.deltaTime);
    }

    /// <summary>
    /// ���E�ړ��̏�Ԃ��Ǘ�
    /// </summary>
    private void ManageLateralMovement()
    {
        if (isLateralMoving)
        {
            lateralMovementTimer += Time.deltaTime * lateralMovementFrequency * TwoPi;
            float lateralOffset = Mathf.Sin(lateralMovementTimer) * lateralMovementAmplitude;
            transform.position = new Vector3(initialPosition.x + lateralOffset, transform.position.y, transform.position.z); // ����X�ʒu����ɍ��E�ړ�

            // ���E�ړ��̌p�����Ԃ𒴂�����I��
            stateTimer += Time.deltaTime;
            if (stateTimer >= lateralMoveActiveDuration)
            {
                stateTimer = 0.0f;
                isLateralMoving = false;
            }
        }
        else
        {
            // ���E�ړ����s��Ȃ����Ԃ��J�E���g
            stateTimer += Time.deltaTime;
            if (stateTimer >= lateralMoveIdleDuration)
            {
                stateTimer = 0.0f;
                isLateralMoving = true; // ���E�ړ����J�n
                lateralMovementTimer = 0.0f; // ���E�ړ��^�C�}�[�����Z�b�g
            }
        }
    }

    /// <summary>
    /// �v���C���[�ƃ^�[�Q�b�g�Ƃ̋������`�F�b�N
    /// </summary>
    private void CheckDistanceToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // �v���C���[�Ƃ̋�����distanceThreshold�ȏ�̏ꍇ�A���̑��x�ɖ߂�
        if (distanceToPlayer > distanceThreshold && currentSpeedMultiplier != 1f)
        {
            currentSpeedMultiplier = 1f;
            baseSpeed = originalSpeed;  // ���x�����ɖ߂�
        }
    }

    /// <summary>
    /// FinalAttack�͈͂ɋ߂Â����ꍇ�A���x��ύX���鏈��
    /// </summary>
    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = multiplier; // ���x�{����ݒ�
        baseSpeed = originalSpeed * multiplier; // �V�������x��K�p
    }
}

using System;

/// <summary>
/// Excel �� JSON �ϊ��ŏo�͂��ꂽ�f�[�^���󂯎�邽�߂̒��ԃN���X�B
/// Unity�p�̎��ۂ̐ݒ�f�[�^�iPlayerMoveSettings�j�ɕϊ�������������B
/// </summary>
[Serializable]
public class PlayerMoveSettingsExport
{
    /// <summary>
    /// �O���ւ̈ړ����x
    /// </summary>
    public float forwardSpeed;

    /// <summary>
    /// �������̈ړ����x
    /// </summary>
    public float moveSpeed;

    /// <summary>
    /// �W�����v���̏�����̗�
    /// </summary>
    public float jumpForce;

    /// <summary>
    /// �n�ʂƂ̐ڒn����Ɏg�p����~�̔��a
    /// </summary>
    public float groundCheckRadius;

    /// <summary>
    /// �W�����v�A�j���[�V�������I������܂ł̎���
    /// </summary>
    public float jumpAnimationDuration;

    /// <summary>
    /// �^�[�Q�b�g�ɋ߂Â����Ƃ��̑��x�{��
    /// </summary>
    public float targetSpeedMultiplier;

    /// <summary>
    /// �ŏ����x�{���i�����̉����l�Ȃǁj
    /// </summary>
    public float minSpeedMultiplier;

    /// <summary>
    /// ���x��ԂɎg�p����X���[�W���O�W��
    /// </summary>
    public float speedMultiplierLerpFactor;

    /// <summary>
    /// �X���[���[�V�������̑��x�{��
    /// </summary>
    public float slowMotionSpeedMultiplier;

    /// <summary>
    /// �ʏ펞�� FixedDeltaTime�i�X���[���[�V�������A�p�j
    /// </summary>
    public float defaultFixedDeltaTime;

    /// <summary>
    /// Export�f�[�^����A�Q�[�����Ŏg�p���� PlayerMoveSettings �ɕϊ�����
    /// </summary>
    /// <returns>PlayerMoveSettings �I�u�W�F�N�g</returns>
    public PlayerMoveSettings ToSettings()
    {
        return new PlayerMoveSettings
        {
            // �O���ւ̈ړ����x
            forwardSpeed = forwardSpeed,

            // �������̈ړ����x
            moveSpeed = moveSpeed,

            // �W�����v���̏�����̗�
            jumpForce = jumpForce,

            // �n�ʂƂ̐ڒn����Ɏg�p���锼�a
            groundCheckRadius = groundCheckRadius,

            // �W�����v�A�j���[�V�����̌p������
            jumpAnimationDuration = jumpAnimationDuration,

            // �^�[�Q�b�g�ɋ߂Â����Ƃ��̑��x�{��
            targetSpeedMultiplier = targetSpeedMultiplier,

            // �ŏ����x�{���i�X���[���[�V���������Ȃǁj
            minSpeedMultiplier = minSpeedMultiplier,

            // ���x��ԂɎg�p����X���[�W���O�W��
            speedMultiplierLerpFactor = speedMultiplierLerpFactor,

            // �X���[���[�V�������̑��x�{��
            slowMotionSpeedMultiplier = slowMotionSpeedMultiplier,

            // �ʏ펞�� FixedDeltaTime�i�X���[���[�V�������A�p�j
            defaultFixedDeltaTime = defaultFixedDeltaTime
        };
    }
}

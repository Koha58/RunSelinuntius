using System;

/// <summary>
/// �v���C���[�̈ړ��⋓���Ɋւ���ݒ�l��ێ�����N���X
/// �i�O��JSON����ǂݍ��܂��f�[�^�\���j
/// </summary>
[Serializable]
public class PlayerMoveSettings
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
    /// �^�[�Q�b�g�ɋ߂Â������ɓK�p����ړ����x�̔{��
    /// </summary>
    public float targetSpeedMultiplier;

    /// <summary>
    /// �ړ����x�̍ŏ��{���i�X���[���[�V�������Ȃǁj
    /// </summary>
    public float minSpeedMultiplier;

    /// <summary>
    /// �ړ����x�̕�ԂɎg���X���[�W���O�W��
    /// </summary>
    public float speedMultiplierLerpFactor;

    /// <summary>
    /// �X���[���[�V�������ɓK�p����ړ����x�{��
    /// </summary>
    public float slowMotionSpeedMultiplier;

    /// <summary>
    /// �X���[���[�V�����ɓ���O�� Time.fixedDeltaTime�i���A�p�ɕێ��j
    /// </summary>
    public float defaultFixedDeltaTime;
}

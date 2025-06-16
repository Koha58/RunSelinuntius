using System;

/// <summary>
/// �v���C���[�̈ړ��ݒ��JSON������ۂ̃G�N�X�|�[�g�p�N���X
/// </summary>
[Serializable]
public class PlayerMoveSettingsExport
{
    // �ړ��O�������x�i������`���A�����_�ȉ�3���j
    public string forwardSpeed;

    // ���ړ����x�i������`���A�����_�ȉ�3���j
    public string moveSpeed;

    // �W�����v�́i������`���A�����_�ȉ�3���j
    public string jumpForce;

    // �n�ʔ���p�̔��a�i������`���A�����_�ȉ�3���j
    public string groundCheckRadius;

    // �W�����v�A�j���[�V�����̌p�����ԁi������`���A�����_�ȉ�3���j
    public string jumpAnimationDuration;

    /// <summary>
    /// PlayerMoveSettings�N���X�̃f�[�^���󂯎��A
    /// �����_�ȉ�3���ŕ����񉻂��ăt�B�[���h�ɃZ�b�g����R���X�g���N�^
    /// </summary>
    /// <param name="data">PlayerMoveSettings�̃C���X�^���X</param>
    public PlayerMoveSettingsExport(PlayerMoveSettings data)
    {
        // �����_�ȉ�3���ŕ����񉻂��đ��
        forwardSpeed = data.forwardSpeed.ToString("0.###");
        moveSpeed = data.moveSpeed.ToString("0.###");
        jumpForce = data.jumpForce.ToString("0.###");
        groundCheckRadius = data.groundCheckRadius.ToString("0.###");
        jumpAnimationDuration = data.jumpAnimationDuration.ToString("0.###");
    }

    public PlayerMoveSettings ToSettings()
    {
        return new PlayerMoveSettings
        {
            forwardSpeed = float.Parse(forwardSpeed),
            moveSpeed = float.Parse(moveSpeed),
            jumpForce = float.Parse(jumpForce),
            groundCheckRadius = float.Parse(groundCheckRadius),
            jumpAnimationDuration = float.Parse(jumpAnimationDuration),
        };
    }

}

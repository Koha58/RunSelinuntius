using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �|�U���̌��ʉ� (SE) ���Ǘ�����N���X
/// </summary>
public class ArrowSEManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // �|�U�����ɍĐ�������ʉ�

    /// <summary>
    /// �A�j���[�V�����C�x���g����Ăяo����郁�\�b�h
    /// �|�U���̌��ʉ����Đ�����
    /// </summary>
    public void ArrowSE()
    {
        // AudioSource ���ݒ肳��Ă���ꍇ�ɂ̂݌��ʉ����Đ�
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            // AudioSource ���ݒ肳��Ă��Ȃ��ꍇ�̃f�o�b�O���b�Z�[�W
            Debug.LogWarning("AudioSource ���ݒ肳��Ă��܂���B���ʉ����Đ��ł��܂���B");
        }
    }
}

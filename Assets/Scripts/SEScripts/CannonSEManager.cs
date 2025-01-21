using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��C�U���̌��ʉ� (SE) ���Ǘ�����N���X
/// </summary>
public class CannonSEManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // ���ʉ����Đ����� AudioSource �R���|�[�l���g

    [SerializeField] private AudioClip cannonFireSound;    // ��C���ˎ��̌��ʉ��N���b�v
    [SerializeField] private AudioClip cannonImpactSound; // ��C���e���̌��ʉ��N���b�v

    [SerializeField] private LayerMask groundLayer; // �n�ʂ̃��C���[

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(cannonFireSound);
    }
    private void OnCollisionEnter(Collision collision)
    {
        // �Փ˂����I�u�W�F�N�g�̃��C���[���n�ʂ̃��C���[�ƈ�v����ꍇ
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            PlayCannonImpactSound();
        }
    }

    /// <summary>
    /// ��C���ˎ��̌��ʉ����Đ����郁�\�b�h
    /// </summary>
    public void PlayCannonFireSound()
    {
        if (audioSource != null && cannonFireSound != null)
        {
            audioSource.PlayOneShot(cannonFireSound);
        }
        else
        {
            Debug.LogWarning("AudioSource �܂��� cannonFireSound ���ݒ肳��Ă��܂���B");
        }
    }

    /// <summary>
    /// ��C���e���̌��ʉ����Đ����郁�\�b�h
    /// </summary>
    public void PlayCannonImpactSound()
    {
        if (audioSource != null && cannonImpactSound != null)
        {
            audioSource.PlayOneShot(cannonImpactSound);
        }
        else
        {
            Debug.LogWarning("AudioSource �܂��� cannonImpactSound ���ݒ肳��Ă��܂���B");
        }
    }
}

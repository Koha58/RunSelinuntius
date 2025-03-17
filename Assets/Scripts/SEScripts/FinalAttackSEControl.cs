using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FinalAttack�֘A��SE�Ǘ��N���X
/// </summary>
public class FinalAttackSEControl : MonoBehaviour
{
    [SerializeField] private AudioSource SESource; // FinalAttack�pSE �� AudioSource
    [SerializeField] private AudioClip finalAttackSE; // FinalAttack ���� SE
    [SerializeField] private AudioClip runAwaySE; // Target�ɓ�����ꂽ ���� SE

    /// <summary>
    /// FinalAttack �� SE ���Đ����A�Đ����Ԃ�Ԃ�
    /// </summary>
    public float PlayFinalAttackSE()
    {
        if (SESource != null && finalAttackSE != null)
        {
            SESource.PlayOneShot(finalAttackSE);
            return finalAttackSE.length; // SE �̒�����Ԃ�
        }
        return 0f; // SE ���ݒ肳��Ă��Ȃ��ꍇ�� 0 ��Ԃ�
    }

    /// <summary>
    /// PlayrunAway �� SE ���Đ�
    /// </summary>
    public void PlayrunAwaySE()
    {
        if (SESource != null && runAwaySE != null)
        {
            SESource.PlayOneShot(runAwaySE);
        }
    }
}

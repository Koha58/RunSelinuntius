using UnityEngine;

/// <summary>
/// FinalAttack �̉\�͈͓��ɂ���Ƃ����� BGM ��ύX����N���X
/// </summary>
public class FinalAttackBGMChanger : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource; // BGM �� AudioSource
    [SerializeField] private AudioClip finalAttackBGM; // FinalAttack ���� BGM
    [SerializeField] private AudioClip normalBGM; // �ʏ펞�� BGM
    [SerializeField] private PlayerMove playerMove; // PlayerMove �̎Q��

    private bool isInFinalAttackRange = false; // FinalAttack �͈͓����ǂ���

    private void Awake()
    {
        // �����I�u�W�F�N�g�ɃA�^�b�`���ꂽ PlayerMove ���擾
        playerMove = GetComponent<PlayerMove>();
        isInFinalAttackRange = false;
    }

    private void Update()
    {
        if (playerMove == null || bgmSource == null) return;

        // �v���C���[�� FinalAttack �\�����擾
        bool isNearTarget = playerMove.IsFinalAttackPossible();

        // ��Ԃ��ς�����ꍇ�̂� BGM ��؂�ւ���
        if (isNearTarget && !isInFinalAttackRange)
        {
            ChangeBGM(finalAttackBGM);
            isInFinalAttackRange = true;
        }
        else if (!isNearTarget && isInFinalAttackRange)
        {
            ChangeBGM(normalBGM);
            isInFinalAttackRange = false;
        }
    }

    /// <summary>
    /// BGM ��ύX����
    /// </summary>
    private void ChangeBGM(AudioClip newClip)
    {
        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();
    }
}

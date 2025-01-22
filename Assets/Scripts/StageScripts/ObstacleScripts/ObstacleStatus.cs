using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�̍U���p�I�u�W�F�N�g(��Q��)�̃X�e�[�^�X�Ǘ��N���X
/// </summary>
public class ObstacleStatus : MonoBehaviour
{
    [Header("�_���[�W�ݒ�")]
    [SerializeField] private int damage; // ��Q�����^����_���[�W��

    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[�ƃg���K�[���ŐڐG�����ꍇ
        if (other.CompareTag("Player"))
        {
            // �v���C���[��HP�R���|�[�l���g���擾���ă_���[�W��^����
            PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                playerStatus.TakeDamage(damage);
            }
        }
    }
}

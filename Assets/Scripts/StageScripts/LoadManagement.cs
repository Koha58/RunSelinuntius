using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �J�����O�̃����G���A���폜����N���X
/// </summary>
public class LoadManagement : MonoBehaviour
{
    /// <summary>
    /// �g���K�[�G���A�ɕʂ̃I�u�W�F�N�g���N�������ۂɌĂяo����鏈��
    /// </summary>
    /// <param name="collision">�N�������I�u�W�F�N�g�̃R���C�_�[</param>
    private void OnTriggerEnter(Collider collision)
    {
        // �g���K�[�ɐN�������I�u�W�F�N�g�� "Level" �^�O�������Ă���ꍇ
        if (collision.gameObject.CompareTag("Level"))
        {
            // ���̃I�u�W�F�N�g��j�󂷂�
            Destroy(collision.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
//�@�^�C�g���V�[���̃v���C���[�̓����𐧌䂷��N���X
/// </summary>
public class TitlePlayerMove : MonoBehaviour
{
    // �ړ����x
    [SerializeField] private float speed = 5.0f;

    /// <summary>
    /// ���t���[���Ăяo����鏈��
    /// �O�����iZ�������j�Ɉړ�������
    /// </summary>
    private void Update()
    {
        // �I�u�W�F�N�g�̑O�����i���[�J��Z���j�Ɉړ�������
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}

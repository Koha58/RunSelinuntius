using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �M�I�u�W�F�N�g��]�����N���X
/// </summary>
public class BarrelScript : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f; // �ړ����x

    void Start()
    {

    }

    // FixedUpdate() ���\�b�h�͕������Z���s����^�C�~���O�Ŏ��s�����
    // �����v�Z�̃^�C�~���O�ňړ��������s�����ƂŁA�X���[�Y�ȓ��삪�\�ɂȂ�
    void FixedUpdate()
    {
        // ���E���W��Z�����Ɉړ�������
        Vector3 moveDirection = Vector3.forward * speed * Time.fixedDeltaTime;

        // transform�𒼐ڈړ�
        transform.position -= moveDirection;
    }

}
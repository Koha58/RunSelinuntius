using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Player�̐i�񂾋�����\��������N���X
/// </summary>
public class PlayerDistanceTracker : MonoBehaviour
{
    [SerializeField] private Text distanceText; // ������\������UI�e�L�X�g
    private Vector3 initialPosition; // �v���C���[�̏����ʒu

    void Start()
    {
        // �v���C���[�̏����ʒu��ۑ�
        initialPosition = transform.position;
    }

    void Update()
    {
        // Z�������̐i�s�������v�Z
        float distanceTraveled = transform.position.z - initialPosition.z;

        // �������e�L�X�g�ɏo��
        distanceText.text = $"{Mathf.FloorToInt(distanceTraveled)} M";
    }
}

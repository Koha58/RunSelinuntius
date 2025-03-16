using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Target��Player�̊Ԃ̋�����\��������N���X
/// </summary>
public class TargetDistanceTracker : MonoBehaviour
{
    [SerializeField] private Transform target; // �^�[�Q�b�g��Transform
    [SerializeField] private Transform player; // �v���C���[��Transform
    [SerializeField] private Text distanceText; // ������\������UI�e�L�X�g

    void Update()
    {
        if (target == null || player == null || distanceText == null) return;

        // �^�[�Q�b�g�ƃv���C���[�̊Ԃ̋������v�Z
        float distanceBetween = Vector3.Distance(target.position, player.position);

        // �������e�L�X�g�ɏo��
        distanceText.text = $"{Mathf.FloorToInt(distanceBetween)} M";
    }
}
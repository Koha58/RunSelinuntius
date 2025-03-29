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
    [SerializeField] private float updateInterval = 0.1f; // �e�L�X�g�X�V�̊Ԋu (�b)

    private float timeSinceLastUpdate = 0f; // �Ō�ɍX�V��������

    void Update()
    {
        if (target == null || player == null || distanceText == null) return;

        // ���Ԃ��o�߂����ꍇ�Ƀe�L�X�g���X�V
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            // �^�[�Q�b�g�ƃv���C���[�̊Ԃ̋������v�Z
            float distanceBetween = Vector3.Distance(target.position, player.position);

            // �������l�̌ܓ����Đ����ɂ��ĕ\��
            distanceText.text = $"{Mathf.RoundToInt(distanceBetween)} M";

            // �Ō�ɍX�V�������Ԃ����Z�b�g
            timeSinceLastUpdate = 0f;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Target(Melos)�̓���ɕ\������UI���Ǘ�����N���X
/// </summary>
public class TargetDistanceMoveScript : MonoBehaviour
{
    [SerializeField] private Transform target; // �^�[�Q�b�g��Transform
    [SerializeField] private Transform player; // �v���C���[��Transform
    [SerializeField] private Text distanceText; // ������\������UI�e�L�X�g
    [SerializeField] private Text arrowText; // �� ��\�����邽�߂�UI�e�L�X�g

    [SerializeField] private float maxOffsetY = 30f; // �����Ƃ��̍ő�I�t�Z�b�g
    [SerializeField] private float minOffsetY = 1f;  // �߂��Ƃ��̍ŏ��I�t�Z�b�g
    [SerializeField] private float maxDistance = 500f; // �ő勗���i����ȏ㉓���� maxOffsetY �ŌŒ�j
    [SerializeField] private float arrowOffsetY = -30f; // ���̋����e�L�X�g����̃I�t�Z�b�g

    // �e�L�X�g�̍X�V�Ԋu�����߂āA�X�V�̕p�x�����炷�i������h�~�j
    [SerializeField] private float updateInterval = 0.1f; // �X�V�Ԋu�i�b�j
    private float timeSinceLastUpdate = 0f;

    void Update()
    {
        if (target == null || player == null || distanceText == null || arrowText == null) return;

        timeSinceLastUpdate += Time.deltaTime;

        // �w�肵�����ԊԊu���o�߂����ꍇ�̂�UI���X�V
        if (timeSinceLastUpdate >= updateInterval)
        {
            // �^�[�Q�b�g�ƃv���C���[�̊Ԃ̋������v�Z
            float distanceBetween = Vector3.Distance(target.position, player.position);

            // �����ɉ����ăI�t�Z�b�g��ω�������i�����قǑ傫���A�߂��قǏ������j
            float dynamicOffsetY = Mathf.Lerp(minOffsetY, maxOffsetY, Mathf.Clamp01(distanceBetween / maxDistance));

            // �ʒu�𒲐�
            Vector3 offset = new Vector3(0, dynamicOffsetY, 0);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + offset);

            // �����\���̈��艻�̂��߁A���l��؂�̂Ăď����_�ȉ��̉e�������炷
            distanceText.text = $"{Mathf.FloorToInt(distanceBetween)}M";
            distanceText.transform.position = screenPos;

            // ���̕\���ʒu�i�����e�L�X�g�̉��ɕ\���j
            arrowText.text = "��";
            arrowText.transform.position = screenPos + new Vector3(0, arrowOffsetY, 0);

            // �Ō�ɍX�V�������Ԃ����Z�b�g
            timeSinceLastUpdate = 0f;
        }
    }
}
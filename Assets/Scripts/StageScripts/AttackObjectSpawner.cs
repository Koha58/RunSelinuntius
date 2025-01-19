using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�̍U���p�I�u�W�F�N�g�����N���X
/// </summary>
public class AttackObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabsToSpawn; // ��������Prefab�̔z��

    [SerializeField] private GameObject player; // �v���C���[�I�u�W�F�N�g

    [SerializeField] private float spawnInterval = 5.0f; // �����Ԋu

    [SerializeField] private float distanceFromPlayer = 1.0f; // �v���C���[�����O�̋���

    [SerializeField] private float minX = -5.0f; // �ړ��\�͈͂̍ŏ�X���W

    [SerializeField] private float maxX = 5.0f; // �ړ��\�͈͂̍ő�X���W

    private float timer = 0.0f; // �^�C�}�[

    /// <summary>
    /// ���t���[�����s����郁�\�b�h�B
    /// ��莞�Ԃ��Ƃ�Prefab�𐶐��B
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime;

        // ��莞�Ԃ��ƂɃI�u�W�F�N�g�𐶐�
        if (timer >= spawnInterval)
        {
            SpawnRandomObject();
            timer = 0.0f; // �^�C�}�[�����Z�b�g
        }
    }

    /// <summary>
    /// �����_����Prefab���v���C���[�̑O���ɐ������܂��B
    /// �z�u�ʒu�̓v���C���[�̈ړ��\�͈͓��Ń����_���Ɍ��܂�B
    /// </summary>
    void SpawnRandomObject()
    {
        // �z��܂��̓v���C���[���ݒ肳��Ă��Ȃ��ꍇ�A�����𒆒f
        if (prefabsToSpawn.Length == 0 || player == null)
        {
            Debug.LogError("Prefab�܂��̓v���C���[���ݒ肳��Ă��܂���I");
            return;
        }

        // �����_����Prefab��I��
        GameObject selectedPrefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];

        // �v���C���[�̈ʒu�Ɣ͈͂��擾
        Vector3 playerPosition = player.transform.position;

        // �����_����X���W���v�Z�i�ړ��\�͈͓��j
        float randomX = Random.Range(minX, maxX);

        // �v���C���[�̎�O�̈ʒu���v�Z
        Vector3 spawnPosition = new Vector3(randomX, playerPosition.y, playerPosition.z + distanceFromPlayer);

        // �I�u�W�F�N�g�𐶐�
        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }
}

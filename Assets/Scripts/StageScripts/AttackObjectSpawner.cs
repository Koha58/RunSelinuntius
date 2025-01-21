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

    [SerializeField] private float distanceFromPlayer = 35.0f; // �v���C���[�����O�̋���

    [SerializeField] private float minX = -5.0f; // �ړ��\�͈͂̍ŏ�X���W

    [SerializeField] private float maxX = 5.0f; // �ړ��\�͈͂̍ő�X���W

    [SerializeField] private CannonSEManager cannonSEManager; // CannonSEManager�̎Q��

    private float timer = 0.0f; // �^�C�}�[

    void Update()
    {
        timer += Time.deltaTime; // �^�C�}�[��i�߂�

        // ��莞�Ԃ��ƂɃI�u�W�F�N�g�𐶐�
        if (timer >= spawnInterval)
        {
            SpawnRandomObject(); // �I�u�W�F�N�g�𐶐�
            timer = 0.0f; // �^�C�}�[�����Z�b�g
        }
    }

    /// <summary>
    /// �����_����Prefab�𐶐����A�v���C���[�̑O���ɔz�u����
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
        int randomIndex = Random.Range(0, prefabsToSpawn.Length);
        GameObject selectedPrefab = prefabsToSpawn[randomIndex];

        // �v���C���[�̈ʒu�Ɣ͈͂��擾
        Vector3 playerPosition = player.transform.position;

        // �����_����X���W���v�Z�i�ړ��\�͈͓��j
        float randomX = Random.Range(playerPosition.x + minX, playerPosition.x + maxX);

        // �v���C���[�̎�O�̈ʒu���v�Z
        Vector3 spawnPosition = new Vector3(randomX, playerPosition.y, playerPosition.z + Mathf.Abs(distanceFromPlayer));

        // 3�Ԗڂ�Prefab�i�C���f�b�N�X2�j�ł���΁Ay���W��ύX����
        if (randomIndex == 2)
        {
            spawnPosition.y = playerPosition.y + 5.0f; // ��Ƃ���+5.0f�����ύX
            // ���ʉ����Đ�
            cannonSEManager.PlayCannonFireSound();
            Debug.Log("Changing Y position for third prefab");
        }

        // �I�u�W�F�N�g�𐶐�
        GameObject obj = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        // ��������̈ʒu���m�F
        Debug.Log($"Spawn Position Before Animation: {obj.transform.position}");

        // 0.1�b��̈ʒu�m�F
        StartCoroutine(CheckPositionAfterDelay(obj, 0.1f));

        // 10�b��ɃI�u�W�F�N�g���폜
        StartCoroutine(DestroyObjectAfterDelay(obj, 10.0f)); // 2�b��ɍ폜
    }

    /// <summary>
    /// �I�u�W�F�N�g���w�肵�����Ԍ�ɍ폜����R���[�`��
    /// </summary>
    IEnumerator DestroyObjectAfterDelay(GameObject obj, float delay)
    {
        // �w�肵�����Ԃ����ҋ@
        yield return new WaitForSeconds(delay);

        // �I�u�W�F�N�g�����݂��Ă���΍폜
        if (obj != null)
        {
            Destroy(obj);
            Debug.Log("Object destroyed after delay.");
        }
    }

    /// <summary>
    /// ���������I�u�W�F�N�g�̈ʒu���A�j���[�V�����ŕύX�����\�������邽�߁A��莞�Ԍ�Ɉʒu���m�F����
    /// </summary>
    IEnumerator CheckPositionAfterDelay(GameObject obj, float delay)
    {
        // �w�肵�����Ԃ����ҋ@
        yield return new WaitForSeconds(delay);

        // �ʒu���m�F���ă��O�ɏo��
        if (obj != null)
        {
            Debug.Log($"Spawn Position After Animation: {obj.transform.position}");
        }
    }
}

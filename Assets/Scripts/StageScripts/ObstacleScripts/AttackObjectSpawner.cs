using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�̍U���p�I�u�W�F�N�g(��Q��)�����N���X
/// </summary>
public class AttackObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabsToSpawn; // ��������Prefab�̔z��
    [SerializeField] private GameObject player; // �v���C���[�I�u�W�F�N�g
    [SerializeField] private float spawnInterval = 5.0f; // �I�u�W�F�N�g�����Ԋu�i�b�j
    [SerializeField] private float distanceFromPlayer = 35.0f; // �v���C���[�����Z�������̋���
    [SerializeField] private float minX = -4.0f; // �ړ��\�͈͂̍ŏ�X���W
    [SerializeField] private float maxX = 4.0f; // �ړ��\�͈͂̍ő�X���W
    [SerializeField] private CannonSEManager cannonSEManager; // ��C�U���̌��ʉ����Ǘ�����CannonSEManager�N���X�̎Q��

    [Header("�I�u�W�F�N�g�����ݒ�")]
    [SerializeField] private float specialYOffset = 5.0f; // �����Prefab�������ɒǉ�����Y���̃I�t�Z�b�g
    [SerializeField] private float destroyDelay = 10.0f; // �I�u�W�F�N�g���폜����܂ł̒x�����ԁi�b�j
    [SerializeField] private float positionCheckDelay = 0.1f; // �I�u�W�F�N�g������̈ʒu�m�F�x���i�b�j

    private float timer = 0.0f; // �I�u�W�F�N�g�����Ԋu���v������^�C�}�[

    /// <summary>
    /// ���t���[���X�V����
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime; // �o�ߎ��Ԃ����Z

        // �����Ԋu�𒴂����ꍇ�A�I�u�W�F�N�g�𐶐�����
        if (timer >= spawnInterval)
        {
            SpawnRandomObject(); // �����_���ȃI�u�W�F�N�g�𐶐�
            timer = 0.0f; // �^�C�}�[�����Z�b�g
        }
    }

    /// <summary>
    /// �����_����Prefab�𐶐����A�v���C���[�̑O���ɔz�u����
    /// </summary>
    void SpawnRandomObject()
    {
        // ��������Prefab�܂��̓v���C���[���ݒ肳��Ă��Ȃ��ꍇ�̓G���[��\�����ďI��
        if (prefabsToSpawn.Length == 0 || player == null)
        {
            Debug.LogError("Prefab�܂��̓v���C���[���ݒ肳��Ă��܂���I");
            return;
        }

        // �z�񂩂烉���_����Prefab��I��
        int randomIndex = Random.Range(0, prefabsToSpawn.Length);
        GameObject selectedPrefab = prefabsToSpawn[randomIndex];

        // �v���C���[�̌��݈ʒu���擾
        Vector3 playerPosition = player.transform.position;

        // �ړ��\�͈͓��Ń����_����X���W���v�Z
        float randomX = Random.Range(playerPosition.x + minX, playerPosition.x + maxX);

        // �v���C���[�̑O���ɃI�u�W�F�N�g��z�u����ʒu���v�Z
        Vector3 spawnPosition = new Vector3(randomX, playerPosition.y, playerPosition.z + Mathf.Abs(distanceFromPlayer));

        // �����Prefab�i�z��̃C���f�b�N�X��2�j�ł���΁A�ǉ���Y���I�t�Z�b�g��ݒ�
        if (randomIndex == 2)
        {
            spawnPosition.y = playerPosition.y + specialYOffset; // Y����ύX
            cannonSEManager.PlayCannonFireSound(); // ���ʉ����Đ�
            Debug.Log("Changing Y position for third prefab");
        }

        // Prefab�𐶐����A�w�肵���ʒu�ɔz�u
        GameObject obj = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"Spawn Position Before Animation: {obj.transform.position}");

        // ������̈ʒu����莞�Ԍ�Ɋm�F����
        StartCoroutine(CheckPositionAfterDelay(obj, positionCheckDelay));

        // �w�肵�����Ԍ�ɃI�u�W�F�N�g���폜����
        StartCoroutine(DestroyObjectAfterDelay(obj, destroyDelay));
    }

    /// <summary>
    /// �w�莞�Ԍ�ɃI�u�W�F�N�g���폜����R���[�`��
    /// </summary>
    /// <param name="obj">�폜����I�u�W�F�N�g</param>
    /// <param name="delay">�폜�܂ł̒x�����ԁi�b�j</param>
    IEnumerator DestroyObjectAfterDelay(GameObject obj, float delay)
    {
        // �w�肵�����Ԃ����ҋ@
        yield return new WaitForSeconds(delay);

        // �I�u�W�F�N�g�����݂���ꍇ�̂ݍ폜
        if (obj != null)
        {
            Destroy(obj);
            Debug.Log("Object destroyed after delay.");
        }
    }

    /// <summary>
    /// �������ꂽ�I�u�W�F�N�g�̈ʒu����莞�Ԍ�Ɋm�F����R���[�`��
    /// </summary>
    /// <param name="obj">�m�F����I�u�W�F�N�g</param>
    /// <param name="delay">�m�F�܂ł̒x�����ԁi�b�j</param>
    IEnumerator CheckPositionAfterDelay(GameObject obj, float delay)
    {
        // �w�肵�����Ԃ����ҋ@
        yield return new WaitForSeconds(delay);

        // �I�u�W�F�N�g�����݂���ꍇ�A���݂̈ʒu�����O�ɏo��
        if (obj != null)
        {
            Debug.Log($"Spawn Position After Animation: {obj.transform.position}");
        }
    }
}

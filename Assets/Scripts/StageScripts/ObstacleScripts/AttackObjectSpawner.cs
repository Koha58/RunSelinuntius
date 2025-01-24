using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�̍U���p�I�u�W�F�N�g(��Q��)�����N���X
/// </summary>
public class AttackObjectSpawner : MonoBehaviour
{
    [Header("Prefab�ݒ�")]
    [SerializeField] private GameObject[] prefabsToSpawn; // ��������Prefab�̔z��
    [SerializeField] private GameObject player; // �v���C���[�I�u�W�F�N�g

    [Header("�����Ԋu�ݒ�")]
    [SerializeField] private float baseSpawnInterval = 2.0f; // ��{�̃I�u�W�F�N�g�����Ԋu�i�b�j
    [SerializeField] private float maxSpawnMultiplier = 2.0f; // HP�ő厞�̐����Ԋu�{��
    [SerializeField] private float minSpawnMultiplier = 0.2f; // HP�ŏ����̐����Ԋu�{��

    [Header("�����ʒu�ݒ�")]
    [SerializeField] private float distanceFromPlayer = 40.0f; // �v���C���[�����Z�������̋���
    [SerializeField] private float minXOffset = -7.0f; // �����ʒu�����X���W�ŏ��I�t�Z�b�g
    [SerializeField] private float maxXOffset = 8.0f; // �����ʒu�����X���W�ő�I�t�Z�b�g
    [SerializeField] private float spawnYOffset = 0.7f; // �ʏ�Prefab��������Y���I�t�Z�b�g
    [SerializeField] private float specialYOffset = 5.0f; // ����Prefab��������Y���I�t�Z�b�g

    [Header("���ʉ��E�폜�ݒ�")]
    [SerializeField] private CannonSEManager cannonSEManager; // ��C�U���̌��ʉ����Ǘ�����N���X�̎Q��
    [SerializeField] private float destroyDelay = 10.0f; // �I�u�W�F�N�g���폜����܂ł̒x�����ԁi�b�j

    [Header("�v���C���[���")]
    [SerializeField] private PlayerStatus playerStatus; // �v���C���[��HP���

    private Vector3 playerInitialPosition; // �v���C���[�̏����ʒu
    private float timer = 0.0f; // �I�u�W�F�N�g�����Ԋu���v������^�C�}�[

    void Start()
    {
        // �v���C���[�̏����ʒu��ۑ�
        if (player != null)
        {
            playerInitialPosition = player.transform.position;
        }
        else
        {
            Debug.LogError("Player object is not assigned!");
        }
    }

    void Update()
    {
        timer += Time.deltaTime; // �o�ߎ��Ԃ����Z

        // HP�Ɋ�Â��ē��I�ɐ����Ԋu���v�Z
        float hpRatio = playerStatus.CurrentHP / (float)playerStatus.MaxHP;

        // HP�̊����ɉ����Đ����Ԋu����
        float spawnInterval = Mathf.Lerp(
            baseSpawnInterval * maxSpawnMultiplier,
            baseSpawnInterval * minSpawnMultiplier,
            hpRatio
        );

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
        if (prefabsToSpawn.Length == 0 || player == null)
        {
            Debug.LogError("Prefab�܂��̓v���C���[���ݒ肳��Ă��܂���I");
            return;
        }

        // �z�񂩂烉���_����Prefab��I��
        int randomIndex = Random.Range(0, prefabsToSpawn.Length);
        GameObject selectedPrefab = prefabsToSpawn[randomIndex];

        // �v���C���[�����ʒu����Ƀ����_����X���W���v�Z
        float randomX = Random.Range(playerInitialPosition.x + minXOffset, playerInitialPosition.x + maxXOffset);

        // �v���C���[�̑O���ɃI�u�W�F�N�g��z�u����ʒu���v�Z
        Vector3 spawnPosition = new Vector3(randomX, spawnYOffset, player.transform.position.z + Mathf.Abs(distanceFromPlayer));

        // �����Prefab�̏ꍇ�͓��ʂ�Y�I�t�Z�b�g��K�p
        if (randomIndex == 2) // �C���f�b�N�X2��Prefab�ɓ�����ʂ�K�p
        {
            spawnPosition.y = player.transform.position.y + specialYOffset;
            cannonSEManager.PlayCannonFireSound(); // ���ʉ����Đ�
        }

        // Prefab�𐶐�
        GameObject obj = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        // �w�肵�����Ԍ�ɃI�u�W�F�N�g���폜
        StartCoroutine(DestroyObjectAfterDelay(obj, destroyDelay));
    }

    /// <summary>
    /// �w�莞�Ԍ�ɃI�u�W�F�N�g���폜����R���[�`��
    /// </summary>
    IEnumerator DestroyObjectAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            Destroy(obj);
        }
    }
}

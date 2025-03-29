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
    [SerializeField] private float maxSpawnMultiplier = 1.0f; // HP�ő厞�̐����Ԋu�{��
    [SerializeField] private float minSpawnMultiplier = 0.5f; // HP�ŏ����̐����Ԋu�{��

    [Header("�����ʒu�ݒ�")]
    [SerializeField] private float distanceFromPlayer = 20.0f; // Prefab�̃f�t�H���gZ���W�I�t�Z�b�g
    [SerializeField] private float minXOffset = -7.0f; // �����ʒu�����X���W�ŏ��I�t�Z�b�g
    [SerializeField] private float maxXOffset = 8.0f; // �����ʒu�����X���W�ő�I�t�Z�b�g
    [SerializeField] private float spawnYOffset = 0.7f; // Prefab�������̃f�t�H���gY���I�t�Z�b�g

    [Header("Prefab�̐����ʒu�ݒ�")]
    [SerializeField] private float[] specialYOffset = new float[] { 0.1f, 0.1f, 5.0f }; // y���̒l��ݒ�{ ArrowAttack, Barrel, IronBall}
    [SerializeField] private float[] specialZOffset = new float[] { 45.0f, 100.0f, 20.0f }; // z���̒l��ݒ�{ ArrowAttack, Barrel, IronBall}

    [Header("���ʉ��E�폜�ݒ�")]
    [SerializeField] private float destroyDelay = 10.0f; // �I�u�W�F�N�g���폜����܂ł̒x�����ԁi�b�j

    [Header("�v���C���[���")]
    [SerializeField] private PlayerStatus playerStatus; // �v���C���[��HP���

    private Vector3 playerInitialPosition; // �v���C���[�̏����ʒu
    private float timer = 0.0f; // �I�u�W�F�N�g�����Ԋu���v������^�C�}�[

    void Start()
    {
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
            SpawnRandomObject();
            timer = 0.0f;
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
        float spawnY = spawnYOffset;
        float spawnZ = player.transform.position.z + distanceFromPlayer;

        // Prefab���̃I�t�Z�b�g��K�p
        if (randomIndex < specialYOffset.Length)
        {
            spawnY = specialYOffset[randomIndex];
        }
        if (randomIndex < specialZOffset.Length)
        {
            spawnZ = player.transform.position.z + specialZOffset[randomIndex];
        }

        // �����ʒu������
        Vector3 spawnPosition = new Vector3(randomX, spawnY, spawnZ);

        // Prefab�𐶐�
        GameObject obj = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        // �w�肵�����Ԍ�ɃI�u�W�F�N�g���폜
        StartCoroutine(DestroyObjectAfterDelay(obj, destroyDelay));
    }

    /// <summary>
    /// �w�莞�Ԍ�ɃI�u�W�F�N�g���폜����R���[�`���B
    /// </summary>
    /// <param name="obj">�폜�Ώۂ̃Q�[���I�u�W�F�N�g</param>
    /// <param name="delay">�폜�܂ł̒x�����ԁi�b�j</param>
    private IEnumerator DestroyObjectAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            Destroy(obj);
        }
    }
}

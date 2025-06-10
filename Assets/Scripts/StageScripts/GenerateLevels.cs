using System.Collections;
using UnityEngine;

/// <summary>
/// �O��JSON����ǂݍ���Prefab�����X�g�����ɁA���Ԋu�Ń����_���ɒn�ʃI�u�W�F�N�g�𐶐�����N���X�B
/// Resources/Stages/ �z����JSON���g���AResources/Prefabs/ �z����Prefab��Instantiate����B
/// </summary>
public class GenerateLevels : MonoBehaviour
{
    // �����ݒ�E�萔�I�Ȓl

    // �ŏ���Prefab�𐶐�����Z���W�̈ʒu
    private const int InitialZPos = 392;

    // �ePrefab�Ԃ�Z�����̊Ԋu
    private const int LevelSpacing = 98;

    // �v���C���[���x�Ɋ�Â������Ԋu�̊�l
    private const float BaseGenerationDelay = 30f;

    // �ǂݍ���JSON�t�@�C�����iResources/Stages/���j
    private string stageFileName = "stage01";

    // �v���C���[�̑��x���擾����R���|�[�l���g
    private PlayerMove playerMove;

    // --- ��ԊǗ��p�ϐ� ---

    // JSON����ǂݍ���Prefab���̔z��
    private string[] prefabNames;

    // ���݂�Z�����ʒu
    private int zPos;

    // �������t���O�i�A��������h�~�j
    private bool creatingLevel = false;


    void Start()
    {
        // �V�[������ PlayerMove �R���|�[�l���g��T���Ď擾����
        playerMove = FindObjectOfType<PlayerMove>();

        // PlayerMove �R���|�[�l���g��������Ȃ������ꍇ�̓G���[���O���o��
        if (playerMove == null)
        {
            Debug.LogError("PlayerMove �R���|�[�l���g���V�[���ɑ��݂��܂���");
        }

        // ����Z���W���Z�b�g
        zPos = InitialZPos;

        // JSON����Prefab���X�g��ǂݍ���
        LoadPrefabList();
    }

    void Update()
    {
        // ���ݐ������łȂ��A����Prefab���X�g�����݂���Ȃ玟�𐶐�
        if (!creatingLevel && prefabNames != null && prefabNames.Length > 0)
        {
            creatingLevel = true;
            StartCoroutine(GenerateLvl());
        }
    }

    /// <summary>
    /// JSON�t�@�C������Prefab���̃��X�g��ǂݍ���
    /// </summary>
    void LoadPrefabList()
    {
        // Resources/Stages/ ����JSON�t�@�C����ǂݍ���
        TextAsset json = Resources.Load<TextAsset>($"Stages/{stageFileName}");
        if (json == null)
        {
            Debug.LogError($"�X�e�[�W�t�@�C����������܂���: {stageFileName}");
            return;
        }

        // JSON��Prefab�����X�g�Ƃ��ĕϊ�����
        PrefabListData data = JsonUtility.FromJson<PrefabListData>(json.text);
        prefabNames = data.prefabs;
    }

    /// <summary>
    /// ���Ԋu��Prefab�������_����������R���[�`��
    /// </summary>
    IEnumerator GenerateLvl()
    {
        // �v���C���[���x�ɉ����Đ����Ԋu��ω�
        float speed = playerMove.GetCurrentSpeed();
        float delay = BaseGenerationDelay / speed;

        // �����_����Prefab����I��
        int idx = Random.Range(0, prefabNames.Length);
        string prefabName = prefabNames[idx];

        // Resources/Prefabs/ ����Prefab�����[�h
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
        if (prefab != null)
        {
            // �w��Z���W��Prefab�𐶐�
            Instantiate(prefab, new Vector3(0, 0, zPos), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"Prefab '{prefabName}' ��������܂���");
        }

        // ���񐶐���Z���W���X�V
        zPos += LevelSpacing;

        // ��莞�ԑ҂��Ă��玟�̐������\��
        yield return new WaitForSeconds(delay);
        creatingLevel = false;
    }
}

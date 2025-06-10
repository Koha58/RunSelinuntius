using System.Collections;
using UnityEngine;

/// <summary>
/// �^�C�g���V�[���̃����G���A��������������N���X�i�O��JSON��Prefab�Ǘ��j
/// </summary>
public class TitleGenerateLevel : MonoBehaviour
{
    // �����ݒ�E�萔�I�Ȓl

    // �ŏ���Prefab�𐶐�����X���W�̈ʒu
    private const float InitialXPos = 2.0f;

    // �ŏ���Prefab�𐶐�����Z���W�̈ʒu
    private const float InitialZPos = 700f;

    // �ePrefab�Ԃ�Z�����̊Ԋu
    private const float LevelSpacing = 98.0f;

    // �v���C���[���x�Ɋ�Â������Ԋu�̊�l
    private const float GenerationDelay = 30.0f;

    // �X�e�[�WJSON�t�@�C�����i�g���q�s�v�j
    private string stageFileName = "stage01";

    // ���݂̐������W
    private float xPos;
    private float zPos;

    // �������
    private bool creatingLevel;

    // JSON����ǂݍ��܂��v���n�u���̔z��
    private string[] prefabNames;


    void Start()
    {
        // �����ʒu��ݒ�
        xPos = InitialXPos;
        zPos = InitialZPos;

        // JSON�t�@�C������Prefab�����X�g��ǂݍ���
        LoadPrefabList();
    }

    void Update()
    {
        // ���x���������łȂ��APrefab�����X�g���ǂݍ��܂�Ă���ꍇ�Ƀ��x���������J�n
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
        // Resources�t�H���_���̎w��p�X����JSON�e�L�X�g��ǂݍ���
        TextAsset json = Resources.Load<TextAsset>($"Stages/{stageFileName}");

        if (json == null)
        {
            // JSON�t�@�C����������Ȃ������ꍇ�̓G���[���O��\�����ď����𒆒f
            Debug.LogError($"�X�e�[�W�t�@�C����������܂���: {stageFileName}");
            return;
        }

        // JSON�`���̃e�L�X�g��Prefab���X�g�̃f�[�^�`���ɕϊ����ĕێ�
        PrefabListData data = JsonUtility.FromJson<PrefabListData>(json.text);
        prefabNames = data.prefabs;
    }

    /// <summary>
    /// �V�������x���𐶐����鏈���i�R���[�`���j
    /// </summary>
    IEnumerator GenerateLvl()
    {
        // prefabNames�z�񂩂烉���_����Prefab����I��
        int index = Random.Range(0, prefabNames.Length);
        string prefabName = prefabNames[index];

        // Resources/Prefabs�t�H���_����Prefab��ǂݍ���
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");

        if (prefab != null)
        {
            // Prefab���w��ʒu�ɐ���
            Instantiate(prefab, new Vector3(xPos, 0, zPos), Quaternion.identity);
        }
        else
        {
            // Prefab��������Ȃ��ꍇ�͌x�����O��\��
            Debug.LogWarning($"Prefab��������܂���: {prefabName}");
        }

        // Z�����̈ʒu���X�V���āA���̐����ʒu�����炷
        zPos += LevelSpacing;

        // �w�肵���ҋ@���Ԃ����������ꎞ��~���Ă��玟�̐�����
        yield return new WaitForSeconds(GenerationDelay);

        // ���x�������t���O���������A����̐���������
        creatingLevel = false;
    }

}

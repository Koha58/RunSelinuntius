using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// �X�e�[�WJSON����Prefab���X�g��ǂݍ��݁A�ePrefab�̃T���l�C�����~�j�}�b�v���ɕ\������G�f�B�^�g���E�B���h�E�B
/// </summary>
public class StagePreviewWindow : EditorWindow
{
    // �ǂݍ��ރX�e�[�W�t�@�C�����iResources/Stages/ ���̃t�@�C�����j
    private string stageFileName = "stage01";

    // JSON�Ŏ擾����Prefab���̃��X�g
    private string[] prefabNames;

    // Prefab�����Ƃ̃T���l�C���摜���i�[���鎫��
    private Dictionary<string, Texture2D> thumbnails = new();

    // �X�N���[���r���[�̃X�N���[���ʒu��ێ�
    private Vector2 scroll;

    // ===== �萔 =====

    private const string WindowTitle = "Stage Preview";              // �E�B���h�E�^�C�g��
    private const string StageFolderPath = "Stages/";               // �X�e�[�WJSON�t�@�C���̃p�X�iResources���j
    private const string PrefabFolderPath = "Prefabs/";             // �v���n�u�i�[��t�H���_�iResources���j

    private const float ScrollViewHeight = 400f;                    // �X�N���[���r���[�̍���
    private const float ThumbnailSize = 80f;                        // �T���l�C���̕\���T�C�Y�i�c���j
    private const float LabelWidth = 60f;                           // Z�l���x���̉���
    private const int ZOffsetStep = 98;                             // Z���W�̊Ԋu�iPrefab1���̋����j

    /// <summary>
    /// ���j���[����G�f�B�^�E�B���h�E���J��
    /// </summary>
    [MenuItem("Window/Stage Preview")]
    public static void ShowWindow()
    {
        // �w�肵���^�C�g����StagePreviewWindow��\��
        GetWindow<StagePreviewWindow>(WindowTitle);
    }

    /// <summary>
    /// �E�B���h�E���L���ɂȂ����Ƃ��ɌĂ΂�鏈��
    /// </summary>
    private void OnEnable()
    {
        // �X�e�[�WJSON��ǂݍ���Prefab�����X�g���Z�b�g����
        LoadStage();

        // �G�f�B�^�X�V�C�x���g�ɃT���l�C���擾�֐���o�^����
        EditorApplication.update += TryUpdateThumbnails;
    }

    /// <summary>
    /// �E�B���h�E������ꂽ�Ƃ��ɌĂ΂�鏈��
    /// </summary>
    private void OnDisable()
    {
        // �E�B���h�E������ꂽ��X�V�C�x���g����T���l�C���擾�֐����������Ė��ʂȏ�����h��
        EditorApplication.update -= TryUpdateThumbnails;
    }

    /// <summary>
    /// �X�e�[�W��JSON�t�@�C����ǂݍ����Prefab�����X�g������������
    /// </summary>
    private void LoadStage()
    {
        // Resources/Stages/ �t�H���_����w�肳�ꂽJSON�t�@�C����ǂݍ���
        TextAsset json = Resources.Load<TextAsset>($"{StageFolderPath}{stageFileName}");
        if (json == null)
        {
            // JSON�t�@�C����������Ȃ���΃G���[�����O�ɏo�͂��ď������f
            Debug.LogError($"�X�e�[�W�t�@�C����������܂���: {stageFileName}");
            return;
        }

        // JSON�e�L�X�g��PrefabListData�N���X�Ƀf�V���A���C�Y���APrefab���z����擾
        PrefabListData data = JsonUtility.FromJson<PrefabListData>(json.text);
        prefabNames = data.prefabs;

        // �����̃T���l�C�������N���A���APrefab�����Ƃ�null�ŏ��������Ă���
        thumbnails.Clear();
        foreach (string prefabName in prefabNames)
        {
            thumbnails[prefabName] = null;
        }
    }

    /// <summary>
    /// AssetPreview���g����Prefab�̃T���l�C����񓯊��Ɏ擾���A�擾�ł�����ĕ`�悷��
    /// </summary>
    private void TryUpdateThumbnails()
    {
        bool updated = false;  // �T���l�C����1�ł��V�K�擾�ł������̃t���O

        // prefabNames�z������[�v���A�܂��擾�ł��Ă��Ȃ�Prefab�̃T���l�C����T��
        foreach (string prefabName in prefabNames)
        {
            // ���łɃT���l�C��������΃X�L�b�v
            if (thumbnails[prefabName] != null) continue;

            // Resources����Prefab�����[�h
            GameObject prefab = Resources.Load<GameObject>($"{PrefabFolderPath}{prefabName}");
            if (prefab == null) continue; // Prefab���Ȃ���΃X�L�b�v

            // AssetPreview.GetAssetPreview�ŃT���l�C�����擾�i�񓯊��ōŏ���null�̂��Ƃ�����j
            Texture2D thumb = AssetPreview.GetAssetPreview(prefab);
            if (thumb != null)
            {
                // �擾�ł����玫���ɕۑ����A�X�V�t���O��ON�ɂ���
                thumbnails[prefabName] = thumb;
                updated = true;
            }
        }

        // �V���ɃT���l�C�����擾�ł��Ă���Ή�ʍĕ`����s��
        if (updated) Repaint();

        // �S�Ă�Prefab�̃T���l�C�����擾�ς݂Ȃ�X�V�C�x���g����������ď������~����
        if (!System.Array.Exists(prefabNames, name => thumbnails[name] == null))
        {
            EditorApplication.update -= TryUpdateThumbnails;
        }
    }

    /// <summary>
    /// �G�f�B�^�E�B���h�E��GUI��`�悷�郁�\�b�h
    /// </summary>
    private void OnGUI()
    {
        // Prefab�����X�g���܂��ǂݍ��܂�Ă��Ȃ���Ή����\�����Ȃ�
        if (prefabNames == null) return;

        EditorGUILayout.Space();

        // ���o���\��
        EditorGUILayout.LabelField("Stage Preview (Minimap Style)", EditorStyles.boldLabel);

        // �X�N���[���r���[�J�n�A�Œ荂���ŃX�N���[���\��
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(ScrollViewHeight));

        // prefabNames�z��̗v�f�����Ԃɕ`��
        for (int i = 0; i < prefabNames.Length; i++)
        {
            string prefabName = prefabNames[i];

            // �T���l�C���摜�����邩��������擾�B�Ȃ��ꍇ��null�B
            Texture2D thumb = thumbnails.ContainsKey(prefabName) ? thumbnails[prefabName] : null;

            // �����{�b�N�X���C�A�E�g�J�n
            EditorGUILayout.BeginHorizontal("box");

            // Z�ʒu�\���ii�Ԗڂ�Prefab�̋[���IZ���W�j
            GUILayout.Label($"Z: {i * ZOffsetStep}", GUILayout.Width(LabelWidth));

            // �T���l�C���摜��\���A�܂��ǂݍ��ݒ��Ȃ�v���[�X�z���_�[�\��
            if (thumb != null)
            {
                GUILayout.Label(thumb, GUILayout.Width(ThumbnailSize), GUILayout.Height(ThumbnailSize));
            }
            else
            {
                GUILayout.Label("(Loading...)", GUILayout.Width(ThumbnailSize), GUILayout.Height(ThumbnailSize));
            }

            // Prefab����\��
            GUILayout.Label(prefabName);

            // �������C�A�E�g�I��
            EditorGUILayout.EndHorizontal();
        }

        // �X�N���[���r���[�I��
        EditorGUILayout.EndScrollView();
    }
}

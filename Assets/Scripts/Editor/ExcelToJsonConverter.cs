using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Excel�f�[�^��JSON�ɕϊ�����Unity�G�f�B�^�g���c�[��
/// </summary>
public class ExcelToJsonConverter : EditorWindow
{
    // Excel�t�@�C���̔z�u�t�H���_�i�v���W�F�N�g�O���j
    private string excelPath = "../ExcelData/";

    // JSON�̏o�͐�t�H���_�iResources�ȉ��ɒu���ƃ��[�h���₷���j
    private string outputPath = "Assets/Resources/Data/";

    /// <summary>
    /// Unity�̃��j���[�ɕ\��
    /// </summary>
    [MenuItem("Tools/Excel �� JSON �ϊ�")]
    public static void ShowWindow()
    {
        ExcelToJsonConverter window = GetWindow<ExcelToJsonConverter>("Excel To JSON");
        window.Show();
    }

    /// <summary>
    /// ���[�U�[�C���^�[�t�F�[�X�`��
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("Excel �� JSON �ϊ��c�[��", EditorStyles.boldLabel);

        // Excel�t�@�C���̃t�H���_�p�X�����
        excelPath = EditorGUILayout.TextField("Excel�t�H���_", excelPath);

        // �o�͂���JSON�t�@�C���̃t�H���_�p�X�����
        outputPath = EditorGUILayout.TextField("�o�͐�t�H���_", outputPath);

        // �ϊ��{�^��
        if (GUILayout.Button("�ϊ����s"))
        {
            ConvertAllExcelFiles();
        }
    }

    /// <summary>
    /// �w�肳�ꂽExcel�t�H���_���̑SExcel�t�@�C�����������AJSON�ɕϊ�
    /// </summary>
    private void ConvertAllExcelFiles()
    {
        // Excel�t�H���_�����݂��邩�m�F
        if (!Directory.Exists(excelPath))
        {
            Debug.LogError("Excel�t�H���_�����݂��܂���: " + excelPath);
            return;
        }

        // �o�͐�t�H���_���Ȃ���΍쐬
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        // �t�H���_���� .xlsx �t�@�C�������ׂĎ擾
        string[] excelFiles = Directory.GetFiles(excelPath, "*.xlsx");

        // �e�t�@�C�����ʂɕϊ�
        foreach (string filePath in excelFiles)
        {
            ConvertExcelToJson(filePath);
        }

        // Unity�Ƀt�@�C���X�V��ʒm���čăC���|�[�g
        AssetDatabase.Refresh();

        Debug.Log("�ϊ������I");
    }

    /// <summary>
    /// Excel�t�@�C��1��ǂݍ��݁AJSON�`���ɕϊ��E�ۑ�����
    /// </summary>
    private void ConvertExcelToJson(string path)
    {
        // �t�@�C�����o�C�i���ŊJ��
        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            // ���{���Shift-JIS�n�̃G���R�[�f�B���O�ɑΉ�
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Excel�̃��[�_�[���쐬
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // �V�[�g�S�̂�DataSet�Ƃ��ēǂݍ��ށi�����V�[�g�Ή��j
                var result = reader.AsDataSet();

                // �ŏ��̃V�[�g���擾
                var table = result.Tables[0];

                // 1�s�ڂ̓w�b�_�[�Ȃ̂ŁA�Œ�2�s�K�v�i�f�[�^���Ȃ��ꍇ�̓X�L�b�v�j
                if (table.Rows.Count < 2) return;

                // �G�N�X�|�[�g�p���X�g���쐬�iPlayerMoveSettingsExport�^�j

                // 2�s�ڈȍ~�̊e�s���f�[�^�Ƃ��ď���
                List<PlayerMoveSettingsExport> exportList = new List<PlayerMoveSettingsExport>();

                for (int row = 1; row < table.Rows.Count; row++)
                {
                    PlayerMoveSettings data = new PlayerMoveSettings()
                    {
                        forwardSpeed = Convert.ToSingle(table.Rows[row][0]),
                        moveSpeed = Convert.ToSingle(table.Rows[row][1]),
                        jumpForce = Convert.ToSingle(table.Rows[row][2]),
                        groundCheckRadius = Convert.ToSingle(table.Rows[row][3]),
                        jumpAnimationDuration = Convert.ToSingle(table.Rows[row][4]),
                    };

                    exportList.Add(new PlayerMoveSettingsExport(data));
                }


                // JSON������ɕϊ��i�z��`���E���`�t���j
                string json = JsonHelper.ToJson(exportList.ToArray(), true);

                // �t�@�C������Excel�t�@�C��������擾
                string fileName = Path.GetFileNameWithoutExtension(path);

                // JSON�o�̓p�X���\�z
                string jsonPath = Path.Combine(outputPath, fileName + ".json");

                // JSON�t�@�C���Ƃ��ĕۑ�
                File.WriteAllText(jsonPath, json);

                Debug.Log($"�ϊ�����: {fileName}.json");
            }
        }
    }
}

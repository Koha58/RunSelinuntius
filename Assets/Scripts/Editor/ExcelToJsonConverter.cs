using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

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
        // Excel�t�@�C�����J��
        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            // �����R�[�h�v���o�C�_�̓o�^�iShift-JIS�ȂǂɑΉ����邽�߁j
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Excel�t�@�C����ǂݍ��ރ��[�_�[���쐬
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // DataSet�Ƃ��đS�V�[�g���擾
                var result = reader.AsDataSet();

                // �ŏ��̃V�[�g���擾
                var table = result.Tables[0];

                // �s��2�s�����̏ꍇ�i�w�b�_�[�̂� or ��j�͏������Ȃ�
                if (table.Rows.Count < 2) return;

                // 1�s�ڂ���w�b�_�[�i�񖼁j���擾
                var columnNames = new List<string>();
                for (int col = 0; col < table.Columns.Count; col++)
                {
                    columnNames.Add(table.Rows[0][col]?.ToString() ?? $"Column{col}");
                }

                // �f�[�^�s�������̃��X�g�Ƃ��č\�z
                var exportList = new List<Dictionary<string, object>>();
                for (int row = 1; row < table.Rows.Count; row++)
                {
                    var rowData = new Dictionary<string, object>();
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        rowData[columnNames[col]] = table.Rows[row][col];
                    }
                    exportList.Add(rowData);
                }

                // �����̃��X�g��JSON������ɕϊ��i���`�t���j
                string json = JsonConvert.SerializeObject(exportList, Formatting.Indented);

                // JSON�t�@�C���Ƃ��ĕۑ�
                string fileName = Path.GetFileNameWithoutExtension(path);
                string jsonPath = Path.Combine(outputPath, fileName + ".json");
                File.WriteAllText(jsonPath, json);

                Debug.Log($"�ϊ�����: {fileName}.json");
            }
        }
    }

}

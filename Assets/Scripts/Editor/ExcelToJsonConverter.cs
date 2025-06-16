using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// ExcelデータをJSONに変換するUnityエディタ拡張ツール
/// </summary>
public class ExcelToJsonConverter : EditorWindow
{
    // Excelファイルの配置フォルダ（プロジェクト外も可）
    private string excelPath = "../ExcelData/";

    // JSONの出力先フォルダ（Resources以下に置くとロードしやすい）
    private string outputPath = "Assets/Resources/Data/";

    /// <summary>
    /// Unityのメニューに表示
    /// </summary>
    [MenuItem("Tools/Excel → JSON 変換")]
    public static void ShowWindow()
    {
        ExcelToJsonConverter window = GetWindow<ExcelToJsonConverter>("Excel To JSON");
        window.Show();
    }

    /// <summary>
    /// ユーザーインターフェース描画
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("Excel → JSON 変換ツール", EditorStyles.boldLabel);

        // Excelファイルのフォルダパスを入力
        excelPath = EditorGUILayout.TextField("Excelフォルダ", excelPath);

        // 出力するJSONファイルのフォルダパスを入力
        outputPath = EditorGUILayout.TextField("出力先フォルダ", outputPath);

        // 変換ボタン
        if (GUILayout.Button("変換実行"))
        {
            ConvertAllExcelFiles();
        }
    }

    /// <summary>
    /// 指定されたExcelフォルダ内の全Excelファイルを処理し、JSONに変換
    /// </summary>
    private void ConvertAllExcelFiles()
    {
        // Excelフォルダが存在するか確認
        if (!Directory.Exists(excelPath))
        {
            Debug.LogError("Excelフォルダが存在しません: " + excelPath);
            return;
        }

        // 出力先フォルダがなければ作成
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        // フォルダ内の .xlsx ファイルをすべて取得
        string[] excelFiles = Directory.GetFiles(excelPath, "*.xlsx");

        // 各ファイルを個別に変換
        foreach (string filePath in excelFiles)
        {
            ConvertExcelToJson(filePath);
        }

        // Unityにファイル更新を通知して再インポート
        AssetDatabase.Refresh();

        Debug.Log("変換完了！");
    }

    /// <summary>
    /// Excelファイル1つを読み込み、JSON形式に変換・保存する
    /// </summary>
    private void ConvertExcelToJson(string path)
    {
        // Excelファイルを開く
        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            // 文字コードプロバイダの登録（Shift-JISなどに対応するため）
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Excelファイルを読み込むリーダーを作成
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // DataSetとして全シートを取得
                var result = reader.AsDataSet();

                // 最初のシートを取得
                var table = result.Tables[0];

                // 行が2行未満の場合（ヘッダーのみ or 空）は処理しない
                if (table.Rows.Count < 2) return;

                // 1行目からヘッダー（列名）を取得
                var columnNames = new List<string>();
                for (int col = 0; col < table.Columns.Count; col++)
                {
                    columnNames.Add(table.Rows[0][col]?.ToString() ?? $"Column{col}");
                }

                // データ行を辞書のリストとして構築
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

                // 辞書のリストをJSON文字列に変換（整形付き）
                string json = JsonConvert.SerializeObject(exportList, Formatting.Indented);

                // JSONファイルとして保存
                string fileName = Path.GetFileNameWithoutExtension(path);
                string jsonPath = Path.Combine(outputPath, fileName + ".json");
                File.WriteAllText(jsonPath, json);

                Debug.Log($"変換成功: {fileName}.json");
            }
        }
    }

}

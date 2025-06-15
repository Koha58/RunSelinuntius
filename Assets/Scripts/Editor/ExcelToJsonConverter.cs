using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using UnityEditor;
using UnityEngine;

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
        // ファイルをバイナリで開く
        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            // 日本語やShift-JIS系のエンコーディングに対応
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Excelのリーダーを作成
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // シート全体をDataSetとして読み込む（複数シート対応）
                var result = reader.AsDataSet();

                // 最初のシートを取得
                var table = result.Tables[0];

                // 1行目はヘッダーなので、最低2行必要（データがない場合はスキップ）
                if (table.Rows.Count < 2) return;

                // エクスポート用リストを作成（PlayerMoveSettingsExport型）

                // 2行目以降の各行をデータとして処理
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


                // JSON文字列に変換（配列形式・整形付き）
                string json = JsonHelper.ToJson(exportList.ToArray(), true);

                // ファイル名をExcelファイル名から取得
                string fileName = Path.GetFileNameWithoutExtension(path);

                // JSON出力パスを構築
                string jsonPath = Path.Combine(outputPath, fileName + ".json");

                // JSONファイルとして保存
                File.WriteAllText(jsonPath, json);

                Debug.Log($"変換成功: {fileName}.json");
            }
        }
    }
}

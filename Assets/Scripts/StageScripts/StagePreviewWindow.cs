using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ステージJSONからPrefabリストを読み込み、各Prefabのサムネイルをミニマップ風に表示するエディタ拡張ウィンドウ。
/// </summary>
public class StagePreviewWindow : EditorWindow
{
    // 読み込むステージファイル名（Resources/Stages/ 内のファイル名）
    private string stageFileName = "stage01";

    // JSONで取得したPrefab名のリスト
    private string[] prefabNames;

    // Prefab名ごとのサムネイル画像を格納する辞書
    private Dictionary<string, Texture2D> thumbnails = new();

    // スクロールビューのスクロール位置を保持
    private Vector2 scroll;

    // ===== 定数 =====

    private const string WindowTitle = "Stage Preview";              // ウィンドウタイトル
    private const string StageFolderPath = "Stages/";               // ステージJSONファイルのパス（Resources内）
    private const string PrefabFolderPath = "Prefabs/";             // プレハブ格納先フォルダ（Resources内）

    private const float ScrollViewHeight = 400f;                    // スクロールビューの高さ
    private const float ThumbnailSize = 80f;                        // サムネイルの表示サイズ（縦横）
    private const float LabelWidth = 60f;                           // Z値ラベルの横幅
    private const int ZOffsetStep = 98;                             // Z座標の間隔（Prefab1つ分の距離）

    /// <summary>
    /// メニューからエディタウィンドウを開く
    /// </summary>
    [MenuItem("Window/Stage Preview")]
    public static void ShowWindow()
    {
        // 指定したタイトルでStagePreviewWindowを表示
        GetWindow<StagePreviewWindow>(WindowTitle);
    }

    /// <summary>
    /// ウィンドウが有効になったときに呼ばれる処理
    /// </summary>
    private void OnEnable()
    {
        // ステージJSONを読み込みPrefab名リストをセットする
        LoadStage();

        // エディタ更新イベントにサムネイル取得関数を登録する
        EditorApplication.update += TryUpdateThumbnails;
    }

    /// <summary>
    /// ウィンドウが閉じられたときに呼ばれる処理
    /// </summary>
    private void OnDisable()
    {
        // ウィンドウが閉じられたら更新イベントからサムネイル取得関数を解除して無駄な処理を防ぐ
        EditorApplication.update -= TryUpdateThumbnails;
    }

    /// <summary>
    /// ステージのJSONファイルを読み込んでPrefab名リストを初期化する
    /// </summary>
    private void LoadStage()
    {
        // Resources/Stages/ フォルダから指定されたJSONファイルを読み込む
        TextAsset json = Resources.Load<TextAsset>($"{StageFolderPath}{stageFileName}");
        if (json == null)
        {
            // JSONファイルが見つからなければエラーをログに出力して処理中断
            Debug.LogError($"ステージファイルが見つかりません: {stageFileName}");
            return;
        }

        // JSONテキストをPrefabListDataクラスにデシリアライズし、Prefab名配列を取得
        PrefabListData data = JsonUtility.FromJson<PrefabListData>(json.text);
        prefabNames = data.prefabs;

        // 既存のサムネイル情報をクリアし、Prefab名ごとにnullで初期化しておく
        thumbnails.Clear();
        foreach (string prefabName in prefabNames)
        {
            thumbnails[prefabName] = null;
        }
    }

    /// <summary>
    /// AssetPreviewを使ってPrefabのサムネイルを非同期に取得し、取得できたら再描画する
    /// </summary>
    private void TryUpdateThumbnails()
    {
        bool updated = false;  // サムネイルが1つでも新規取得できたかのフラグ

        // prefabNames配列をループし、まだ取得できていないPrefabのサムネイルを探す
        foreach (string prefabName in prefabNames)
        {
            // すでにサムネイルがあればスキップ
            if (thumbnails[prefabName] != null) continue;

            // ResourcesからPrefabをロード
            GameObject prefab = Resources.Load<GameObject>($"{PrefabFolderPath}{prefabName}");
            if (prefab == null) continue; // Prefabがなければスキップ

            // AssetPreview.GetAssetPreviewでサムネイルを取得（非同期で最初はnullのこともある）
            Texture2D thumb = AssetPreview.GetAssetPreview(prefab);
            if (thumb != null)
            {
                // 取得できたら辞書に保存し、更新フラグをONにする
                thumbnails[prefabName] = thumb;
                updated = true;
            }
        }

        // 新たにサムネイルを取得できていれば画面再描画を行う
        if (updated) Repaint();

        // 全てのPrefabのサムネイルが取得済みなら更新イベントから解除して処理を停止する
        if (!System.Array.Exists(prefabNames, name => thumbnails[name] == null))
        {
            EditorApplication.update -= TryUpdateThumbnails;
        }
    }

    /// <summary>
    /// エディタウィンドウのGUIを描画するメソッド
    /// </summary>
    private void OnGUI()
    {
        // Prefab名リストがまだ読み込まれていなければ何も表示しない
        if (prefabNames == null) return;

        EditorGUILayout.Space();

        // 見出し表示
        EditorGUILayout.LabelField("Stage Preview (Minimap Style)", EditorStyles.boldLabel);

        // スクロールビュー開始、固定高さでスクロール可能に
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(ScrollViewHeight));

        // prefabNames配列の要素を順番に描画
        for (int i = 0; i < prefabNames.Length; i++)
        {
            string prefabName = prefabNames[i];

            // サムネイル画像があるか辞書から取得。ない場合はnull。
            Texture2D thumb = thumbnails.ContainsKey(prefabName) ? thumbnails[prefabName] : null;

            // 水平ボックスレイアウト開始
            EditorGUILayout.BeginHorizontal("box");

            // Z位置表示（i番目のPrefabの擬似的Z座標）
            GUILayout.Label($"Z: {i * ZOffsetStep}", GUILayout.Width(LabelWidth));

            // サムネイル画像を表示、まだ読み込み中ならプレースホルダー表示
            if (thumb != null)
            {
                GUILayout.Label(thumb, GUILayout.Width(ThumbnailSize), GUILayout.Height(ThumbnailSize));
            }
            else
            {
                GUILayout.Label("(Loading...)", GUILayout.Width(ThumbnailSize), GUILayout.Height(ThumbnailSize));
            }

            // Prefab名を表示
            GUILayout.Label(prefabName);

            // 水平レイアウト終了
            EditorGUILayout.EndHorizontal();
        }

        // スクロールビュー終了
        EditorGUILayout.EndScrollView();
    }
}

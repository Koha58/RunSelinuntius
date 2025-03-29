using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームオーバーのテキストを管理するクラス
/// </summary>
public class GameOverManager : MonoBehaviour
{
    [SerializeField] private Text dialogueText; // テキストを表示するUIコンポーネント
    [SerializeField] private float typingSpeed = 0.1f; // 文字を表示する間隔（秒）
    [SerializeField] private Color normalTextColor = Color.white; // 通常のテキスト色（白）

    // UIオブジェクトの参照
    [SerializeField] private GameObject backGroundUI; // 背景UI
    [SerializeField] private Image yesUI;  // リトライ->「はい」ボタン UI
    [SerializeField] private Image noUI; // リトライ->「いいえ」ボタン UI
    [SerializeField] private Image retryUI;  // コントローラー用リトライボタン UI
    [SerializeField] private Image titleUI; // コントローラー用タイトルへ戻るボタン UI


    // セリフ群
    private string[] dialogueLines =
    {
        "セリヌンティウスは力尽きた・・・。",
        "リトライしますか？\n"
    };

    private int currentLineIndex = 0; // 現在表示しているセリフのインデックス
    private bool isTyping = false; // 現在文字を表示しているかどうかのフラグ
    private bool isNext = false; // テキストごとの表示の終了フラグ

    /// <summary>
    /// 最初のセリフを表示開始する。
    /// </summary>
    void Start()
    {
        // 背景UIを表示させる
        backGroundUI.GetComponent<Image>().enabled = true;
        // 「はい」UIを表示させない
        yesUI.enabled = false;
        // 「いいえ」UIを表示させない
        noUI.enabled = false;
        // コントローラー用リトライボタン UIを表示させない
        retryUI.enabled = false;
        // コントローラー用タイトルへ戻るボタン UIを表示させない
        titleUI.enabled = false;

        dialogueText.color = normalTextColor; // 白文字で表示
        StartCoroutine(TypeText(dialogueLines[currentLineIndex])); // 最初のセリフを表示開始

        // テキスト表示が終了していない
        isNext = false;
    }

    private void Update()
    {
        // 現在の入力デバイスを判定（true: コントローラー, false: キーボード）
        bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();

        // 選択肢の表示中（「リトライしますか？」の部分が表示された場合）
        if (dialogueText.text == "リトライしますか？\n")
        {
            // 「はい」「いいえ」のUIを有効化（どのデバイスでも共通）
            yesUI.enabled = true;
            noUI.enabled = true;

            // コントローラー使用時のみ、専用のUIを有効化
            retryUI.enabled = isUsingGamepad;
            titleUI.enabled = isUsingGamepad;
        }
    }

    /// <summary>
    /// ユーザーの入力に合わせ次のセリフを表示する。
    /// </summary>
    private void OnClick(InputValue value)
    {
        // 現在の入力デバイスを判定（true: コントローラー, false: キーボード）
        bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();

        // 左クリック/Aボタンが押されたかつ、文字入力中でない場合に次のセリフを表示
        if (value.isPressed && !isTyping)
        {
            NextLine();
        }

        // 選択肢の表示中（「リトライしますか？」の部分が表示された場合）
        if (dialogueText.text == "リトライしますか？\n" && isUsingGamepad)
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    /// <summary>
    /// 次のセリフを表示する。
    /// </summary>
    private void NextLine()
    {
        currentLineIndex++; // 次のセリフに進む
        if (currentLineIndex < dialogueLines.Length) // テキストが残っている場合
        {
            StartCoroutine(TypeText(dialogueLines[currentLineIndex])); // 次のテキストを表示
        }
    }

    /// <summary>
    /// 指定された文字列を1文字ずつ表示する。
    /// </summary>
    /// <param name="line">表示するテキスト</param>
    private IEnumerator TypeText(string line)
    {
        isTyping = true; // 文字表示中
        isNext = false; // 次のセリフ開始時に false にする
        dialogueText.text = ""; // 新しいテキストを表示するために一度空にする

        foreach (char letter in line) // 文字列の1文字ずつ処理
        {
            dialogueText.text += letter; // 1文字ずつ表示
            yield return new WaitForSeconds(typingSpeed); // 表示間隔を設定
        }

        isTyping = false; // 文字の表示が完了したので、入力中フラグを解除

        // リトライ表示中はisNextをfalseにする
        if (line != "リトライしますか？\n")
        {
            isNext = true; // 通常のセリフなら次に進める
        }
    }

    /// <summary>
    /// 現在のセリフの表示が完了し、次のセリフへ進むことが可能かどうかを判定する
    /// </summary>
    /// <returns>次のセリフへ進める場合は true、進めない場合は false。</returns>
    internal bool IsNextPossible()
    {
        return isNext;
    }

    /// <summary>
    /// ゲームシーンに遷移する。
    /// </summary>
    public void Retry()
    {
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// タイトル画面へ戻る。
    /// </summary>
    public void BackTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    /// <summary>
    /// ユーザーが「戻る」操作を行った際に、タイトル画面へ遷移する。
    /// Bボタンの入力
    /// </summary>
    /// <param name="value">戻る操作の入力値。</param>
    private void OnBack(InputValue value)
    {
        SceneManager.LoadScene("TitleScene");
    }

}

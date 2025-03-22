using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ストーリー導入部分のテキストを管理するクラス
/// </summary>
public class IntroductionManager : MonoBehaviour
{
    [SerializeField] private Text dialogueText; // テキストを表示するUIコンポーネント
    [SerializeField] private float typingSpeed = 0.1f; // 文字を表示する間隔（秒）
    [SerializeField] private Color normalTextColor = Color.white; // 通常のテキスト色（白）
    [SerializeField] private Color finalTextColor = Color.red; // 赤文字のテキスト色（赤）

    // UIオブジェクトの参照（切り替えるためのUI）
    [SerializeField] private GameObject scene1; // scene1
    [SerializeField] private GameObject scene2; // scene2
    [SerializeField] private GameObject scene3; // scene3
    [SerializeField] private GameObject scene4; // scene4

    [SerializeField] private AudioSource bgmSource; // BGM用のAudioSource

    // 通常フェーズのセリフ群
    private string[] dialogueLines =
    {
        "メロスは激怒した。",
        "必ずかの邪知暴虐な王を除かねばならぬと決意した。",
        "この正義感ばかり強い男は無策にも短剣を持ち王城に入り",
        "そして捕縛された。",
        "王の前に引き出されたメロスに王は何をするつもりであったか問うた。",
        "幾分かの問答の後、メロスは王にある賭けを提案した。",
        "深夜",
        "竹馬の友、セリヌンティウスは王城に召された。",
        "王の面前で佳き友と佳き友は二年ぶりに相逢うた。",
        "メロスは、友に一切の事情を語った。",
        "「友よ、私はお前を人質にした。」",
        "「この三日の間、私は妹の結婚式に行く。」",
        "「三日目の日没までに戻ってこなければお前を絞め殺してよいというものだ。」",
        "「無論、私は必ず日没までに帰ってくる。」",
        "「私を信じて待っていてもらえぬか。」",
        "セリヌンティウスは無言で頷き、メロスをひしと抱きしめた。"
    };

    // 赤文字フェーズのセリフ群
    private string[] finalDialogueLines =
    {
        "「ありえぬ。」",
        "「それが二年ぶりの友に対する仕打ちか。」",
        "「なぜ私がお前の身代わりに死なねばならぬ。」",
        "「私からすればお前も王も変わらぬぞ。」",
        "「私はお前を信じない。」",
        "「お前を妹に会わせてなるものか。」"
    };

    // 最後の白文字フェーズ（状況が進行するセリフ群）
    private string[] extraDialogueLines =
    {
        "メロスは息をのんだ。",
        "あの無二の友が、かの邪知暴虐な王より余程恐ろしく感じた。",
        "メロスは逃げ出した。",
        "セリヌンティウスもメロスを追いかけ王城を出て行った。",
        "王は自分が蚊帳の外に置かれている状況にしばらく呆然としていたが、",
        "ふと我に返り、「人質が逃げた」と叫び兵を招集した。",
        "「必ず人質を捕らえよ。失敗すれば命はないと思え。」"
    };

    private int currentLineIndex = 0; // 現在表示しているセリフのインデックス
    private bool isTyping = false; // 現在文字を表示しているかどうかのフラグ
    private bool isFinalPhase = false; // 赤文字フェーズかどうかのフラグ
    private bool isExtraPhase = false; // 白文字フェーズに戻る状態かどうかのフラグ
    private bool isNext = false; // テキストごとの表示の終了フラグ

    /// <summary>
    /// 最初のセリフを表示開始する。
    /// </summary>
    void Start()
    {
        // scene1のみ表示させる
        scene1.GetComponent<Image>().enabled = true;
        scene2.GetComponent<Image>().enabled = false;
        scene3.GetComponent<Image>().enabled = false;
        scene4.GetComponent<Image>().enabled = false;

        dialogueText.color = normalTextColor; // 最初は白文字
        StartCoroutine(TypeText(dialogueLines[currentLineIndex])); // 最初のセリフを表示開始

        // テキスト表示が終了していない
        isNext = false;
    }

    /// <summary>
    /// ユーザーの入力に合わせ次のセリフを表示する。
    /// </summary>
    private void OnClick(InputValue value)
    {
        // 左クリック/Aボタンが押されたかつ、文字入力中でない場合に次のセリフを表示
        if (value.isPressed && !isTyping)
        {
            NextLine();
        }
    }

    /// <summary>
    /// 次のセリフを表示する。
    /// 現在のフェーズ（通常フェーズ、赤文字フェーズ、白文字フェーズ）によって適切なセリフを選択する。
    /// </summary>
    private void NextLine()
    {
        if (!isFinalPhase && !isExtraPhase) // 通常の白文字フェーズ
        {
            currentLineIndex++; // 次のセリフに進む
            if (currentLineIndex < dialogueLines.Length) // 通常フェーズのテキストが残っている場合
            {
                StartCoroutine(TypeText(dialogueLines[currentLineIndex])); // 次のテキストを表示
            }
            else // 通常フェーズが終了したら
            {
                StartCoroutine(DeleteTextAndStartFinal()); // テキストを削除し、赤文字フェーズを開始
            }
        }
        else if (isFinalPhase) // 赤文字フェーズ
        {
            currentLineIndex++; // 次のセリフに進む
            if (currentLineIndex < finalDialogueLines.Length) // 赤文字フェーズのテキストが残っている場合
            {
                StartCoroutine(TypeText(finalDialogueLines[currentLineIndex])); // 次の赤文字テキストを表示
            }
            else // 赤文字フェーズが終了したら
            {
                StartCoroutine(DeleteTextAndStartExtra()); // テキストを削除し、白文字フェーズに戻す
            }
        }
        else if (isExtraPhase) // 白文字フェーズ（最終的なセリフ）
        {
            currentLineIndex++; // 次のセリフに進む
            if (currentLineIndex < extraDialogueLines.Length) // 最後のセリフが残っている場合
            {
                StartCoroutine(TypeText(extraDialogueLines[currentLineIndex])); // 最後のセリフを表示
            }
            else // 最後のセリフが終了したら、シーンを切り替える
            {
                SceneManager.LoadScene("GameScene");
            }
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
        isNext = true; // 一文が終わったことを示す

        // セリフに応じてUIを切り替える
        if (line == "竹馬の友、セリヌンティウスは王城に召された。") // このセリフ終了後にUI1を表示
        {
            ShowUI(scene2);
        }
        else if (line == "「ありえぬ。」") // このセリフ終了後にUI2を表示
        {
            ShowUI(scene3);
            StopBGM(); // BGMを停止
        }
        else if (line == "セリヌンティウスもメロスを追いかけ王城を出て行った。") // このセリフ終了後にUI3を表示
        {
            ShowUI(scene4);
            ResumeBGM(); // BGMを再開
        }
    }

    /// <summary>
    /// 画面のテキストを削除し、赤文字フェーズを開始する。
    /// </summary>
    private IEnumerator DeleteTextAndStartFinal()
    {
        isTyping = true; // テキスト削除中は文字入力中としてフラグを立てる
        yield return new WaitForSeconds(0.5f); // 少し待機してから削除開始

        // 1文字ずつテキストを削除
        while (dialogueText.text.Length > 0)
        {
            dialogueText.text = dialogueText.text.Substring(0, dialogueText.text.Length - 1); // 1文字ずつ削除
            yield return new WaitForSeconds(typingSpeed); // 削除間隔
        }

        // 赤文字フェーズを開始
        isFinalPhase = true;
        currentLineIndex = 0; // 赤文字フェーズの最初のセリフに戻す
        dialogueText.color = finalTextColor; // テキスト色を赤に変更
        StartCoroutine(TypeText(finalDialogueLines[currentLineIndex])); // 赤文字のセリフを表示開始
    }

    /// <summary>
    /// 画面のテキストを削除し、白文字フェーズを開始する。
    /// </summary>
    private IEnumerator DeleteTextAndStartExtra()
    {
        isTyping = true; // テキスト削除中は文字入力中としてフラグを立てる
        yield return new WaitForSeconds(0.5f); // 少し待機してから削除開始

        dialogueText.text = ""; // 一気にテキストを削除

        // 白文字フェーズに戻る
        isFinalPhase = false;
        isExtraPhase = true;
        currentLineIndex = 0; // 白文字フェーズの最初のセリフに戻す
        dialogueText.color = normalTextColor; // テキスト色を元に戻す（白）
        StartCoroutine(TypeText(extraDialogueLines[currentLineIndex])); // 最後の白文字のセリフを表示開始
    }

    /// <summary>
    /// 特定のUIコンポーネントを有効にし、他のUIコンポーネントを無効にする
    /// </summary>
    private void ShowUI(GameObject uiToEnable)
    {
        // すべてのUIコンポーネントを無効にする
        scene1.GetComponent<Image>().enabled = false;
        scene2.GetComponent<Image>().enabled = false;
        scene3.GetComponent<Image>().enabled = false;
        scene4.GetComponent<Image>().enabled = false;

        // 指定されたUIコンポーネントだけを有効にする
        uiToEnable.GetComponent<Image>().enabled = true;
    }

    /// <summary>
    /// BGMを停止する
    /// </summary>
    private void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
    }

    /// <summary>
    /// BGMを再開する
    /// </summary>
    private void ResumeBGM()
    {
        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
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
}
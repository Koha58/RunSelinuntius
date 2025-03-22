using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// ストーリークリア部分のテキストを管理するクラス
/// </summary>
public class ClearManager : MonoBehaviour
{
    private const float DefaultFadeDuration = 1.5f; // フェードイン/アウトのデフォルト時間（秒）
    private const float MaxBGMVolume = 0.5f; // BGMの最大音量
    private const float MinBGMVolume = 0f; // BGMの最小音量

    [SerializeField] private Text dialogueText; // テキストを表示するUIコンポーネント
    [SerializeField] private float typingSpeed = 0.1f; // 文字を表示する間隔（秒）
    [SerializeField] private Color normalTextColor = Color.white; // 通常のテキスト色（白）

    // UIオブジェクトの参照（切り替えるためのUI）
    [SerializeField] private GameObject scene1; // scene1
    [SerializeField] private GameObject scene2; // scene2

    [SerializeField] private AudioSource bgmSource; // BGM用のAudioSource
    [SerializeField] private AudioClip scene1BGM; // scene1 時の BGM
    [SerializeField] private AudioClip scene2BGM; // scene2 時の BGM

    // セリフ群
    private string[] dialogueLines =
    {
        "セリヌンティウスはメロス目掛けて思い切り蹴り上げた。",
        "そこでようやくメロスは理解した。",
        "自分が犯してしまった事の重大さを。",
        "倒れたメロスは口を開く。",
        "「済まなかった。」",
        "「私はお前に何てことをしてしまったのか。」",
        "セリヌンティウスはメロスを見ることもなく言葉を返す。",
        "「本当に私に申し訳なく思っているのなら王城に戻れ。」",
        "「私を追ってきた兵たちがそこまで来ている。」",
        "「お前は自分が起こした行いの全ての責任を取るのだ。」",
        "メロスは小さく頷くと、体を起こし兵たちの方へ歩いて行った。",
        "完"
    };

    private int currentLineIndex = 0; // 現在表示しているセリフのインデックス
    private bool isTyping = false; // 現在文字を表示しているかどうかのフラグ
    private bool isNext = false; // テキストごとの表示の終了フラグ

    /// <summary>
    /// 最初のセリフを表示開始する。
    /// </summary>
    void Start()
    {
        // scene1のみ表示させる
        scene1.GetComponent<Image>().enabled = true;
        scene2.GetComponent<Image>().enabled = false;

        dialogueText.color = normalTextColor; // 白文字で表示
        StartCoroutine(TypeText(dialogueLines[currentLineIndex])); // 最初のセリフを表示開始

        // テキスト表示が終了していない
        isNext = false;

        // 最初のBGMを再生
        bgmSource.clip = scene1BGM; // BGMをセット
        bgmSource.Play(); // 再生
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
    /// </summary>
    private void NextLine()
    {
        currentLineIndex++; // 次のセリフに進む
        if (currentLineIndex < dialogueLines.Length) // テキストが残っている場合
        {
            StartCoroutine(TypeText(dialogueLines[currentLineIndex])); // 次のテキストを表示
        }
        else // 最後のセリフが終了したら、シーンを切り替える
        {
            SceneManager.LoadScene("TitleScene");
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
        if (line == "自分が犯してしまった事の重大さを。") // このセリフ終了後にUI1を表示
        {
            ShowUI(scene2);
            ChangeBGM(scene2BGM);
        }
    }


    /// <summary>
    /// 特定のUIコンポーネントを有効にし、他のUIコンポーネントを無効にする
    /// </summary>
    private void ShowUI(GameObject uiToEnable)
    {
        // すべてのUIコンポーネントを無効にする
        scene1.GetComponent<Image>().enabled = false;
        scene2.GetComponent<Image>().enabled = false;

        // 指定されたUIコンポーネントだけを有効にする
        uiToEnable.GetComponent<Image>().enabled = true;
    }

    /// <summary>
    /// BGM をフェードアウトしてから変更し、フェードインする
    /// </summary>
    /// <param name="newClip">変更するBGM</param>
    /// <param name="fadeDuration">フェード時間（秒）</param>
    private void ChangeBGM(AudioClip newClip, float fadeDuration = DefaultFadeDuration)
    {
        StartCoroutine(FadeOutAndChangeBGM(newClip, fadeDuration));
    }

    /// <summary>
    /// BGM をフェードアウトしてから、フェードインしながら変更するコルーチン
    /// </summary>
    /// <param name="newClip">変更するBGM</param>
    /// <param name="fadeDuration">フェード時間（秒）</param>
    private IEnumerator FadeOutAndChangeBGM(AudioClip newClip, float fadeDuration)
    {
        // 現在のBGMをフェードアウト
        yield return StartCoroutine(FadeOutBGM(fadeDuration));

        // 新しいBGMをセットしてフェードイン
        yield return StartCoroutine(FadeInBGM(newClip, fadeDuration));
    }

    /// <summary>
    /// BGM をフェードアウトする
    /// </summary>
    /// <param name="fadeDuration">フェードアウトにかける時間（秒）</param>
    private IEnumerator FadeOutBGM(float fadeDuration)
    {
        float startVolume = bgmSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, MinBGMVolume, elapsedTime / fadeDuration); // 定数を使用
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bgmSource.volume = MinBGMVolume;
        bgmSource.Stop(); // 音を完全に停止
    }

    /// <summary>
    /// BGM をフェードインしながら再生する
    /// </summary>
    /// <param name="newClip">変更するBGM</param>
    /// <param name="fadeDuration">フェードインにかける時間（秒）</param>
    private IEnumerator FadeInBGM(AudioClip newClip, float fadeDuration)
    {
        bgmSource.clip = newClip;
        bgmSource.Play();

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            bgmSource.volume = Mathf.Lerp(MinBGMVolume, MaxBGMVolume, elapsedTime / fadeDuration); // 定数を使用
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bgmSource.volume = MaxBGMVolume; // 最大音量
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// ストーリーシーンのデバイスごとのUI切り替えクラス
/// </summary>
public class StoryUIDeviceCheck : MonoBehaviour
{
    [SerializeField] private Image keyboardNextUI;  // キーボード用次の文へ移る UI
    [SerializeField] private Image controllerNextUI;  // コントローラー用次の文へ移る UI
    [SerializeField] private Image controllerSkipUI; // コントローラー用スキップ UI
    [SerializeField] private Image mouseUI; // マウス用 UI
    [SerializeField] private Color blinkColor = Color.gray; // 点滅時の色
    [SerializeField] private float blinkInterval = 1.0f; // 点滅間隔（秒）
    [SerializeField] private IntroductionManager introductionManager; // IntroductionManager の参照
    [SerializeField] private ClearManager clearManager;// ClearManager の参照
    [SerializeField] private GameOverManager gameOverManager;// GameOverManager の参照

    private bool isBlinking = false;   // 現在点滅中かどうか

    private bool isNextSentence = false;   // 次のテキストに移行可能か

    private void Awake()
    {
        // IntroductionSceneの場合
        if(introductionManager != null)
        {
            // 同じオブジェクトにアタッチされた IntroductionManager を取得
            introductionManager = GetComponent<IntroductionManager>();
        }
        // GameClearSceneの場合
        else if (clearManager != null)
        {
            // 同じオブジェクトにアタッチされた ClearManager を取得
            clearManager = GetComponent<ClearManager>();
        }
        // GameOverSceneの場合
        else if (gameOverManager != null)
        {
            // 同じオブジェクトにアタッチされた ClearManager を取得
            gameOverManager = GetComponent<GameOverManager>();
        }

    }

    void Update()
    {
        if (InputDeviceManager.Instance == null) return;

        // 現在の入力デバイスを取得（true: コントローラー, false: キーボード）
        bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();

        // 次のテキストに移行可能かを取得
        SceneCheck();  // isNextSentence を更新

        // UI の表示 / 非表示
        mouseUI.enabled = isNextSentence && !isUsingGamepad;
        keyboardNextUI.enabled = isNextSentence && !isUsingGamepad;
        controllerNextUI.enabled = isNextSentence && isUsingGamepad;

        // IntroductionScene または GameClearScene の場合
        if (introductionManager != null || clearManager != null)
        {
            controllerSkipUI.enabled = isUsingGamepad;
        }
        
        // 点滅処理を開始・停止
        if (isNextSentence && !isBlinking)
        {
            StartCoroutine(BlinkUI());
        }
        else if (!isNextSentence && isBlinking)
        {
            StopCoroutine(BlinkUI());
            ResetUIColor();
        }
    }

    /// <summary>
    /// UI を点滅させるコルーチン
    /// </summary>
    private IEnumerator BlinkUI()
    {
        isBlinking = true;
        while (true)
        {
            // 現在の UI を取得
            bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();
            Image activeUI = isUsingGamepad ? controllerNextUI : keyboardNextUI;
            Image uiImage = activeUI.GetComponent<Image>();

            if (uiImage != null)
            {
                // 色を切り替える
                uiImage.color = (uiImage.color == Color.white) ? blinkColor : Color.white;
            }

            yield return new WaitForSeconds(blinkInterval);
        }
    }

    /// <summary>
    /// UI の色を元に戻す
    /// </summary>
    private void ResetUIColor()
    {
        keyboardNextUI.GetComponent<Image>().color = Color.white;
        controllerNextUI.GetComponent<Image>().color = Color.white;
        isBlinking = false;
    }

    /// <summary>
    /// スキップボタン用
    /// </summary>
    public void Skip()
    {
        // スキップ先を選択（introductionManager または clearManager のどちらが存在するかで分岐）
        if (introductionManager != null)
        {
            SceneManager.LoadScene("GameScene");
        }
        else if(clearManager != null)
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

    /// <summary>
    /// ユーザーがスキップ操作を行った際に、対応するシーンへ遷移する。
    /// </summary>
    /// <param name="value">スキップ操作の入力値。</param>
    private void OnSkip(InputValue value)
    {
        // スキップ先を選択（introductionManager または clearManager のどちらが存在するかで分岐）
        if (introductionManager != null)
        {
            SceneManager.LoadScene("GameScene");
        }
        else if (clearManager != null)
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

    /// <summary>
    /// 次のテキストに移行可能かを取得
    /// </summary>
    private void SceneCheck()
    {
        // 次のテキストに移行可能かを取得（introductionManager , clearManager , gameOverManager のいずれかが存在するかで分岐）
        if (introductionManager != null)
        {
            isNextSentence = introductionManager.IsNextPossible();
        }
        else if (clearManager != null)
        {
            isNextSentence = clearManager.IsNextPossible();
        }
        else if (gameOverManager != null)
        {
            isNextSentence = gameOverManager.IsNextPossible();
        }
        else
        {
            isNextSentence = false;  // いずれも存在しない場合は false
        }
    }
}

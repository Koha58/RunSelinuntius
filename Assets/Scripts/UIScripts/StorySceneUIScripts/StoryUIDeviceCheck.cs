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
    [SerializeField] private IntroductionManager introductionManager;

    private bool isBlinking = false;   // 現在点滅中かどうか

    private void Awake()
    {
        // 同じオブジェクトにアタッチされた PlayerMove を取得
        introductionManager = GetComponent<IntroductionManager>();
    }

    void Update()
    {
        if (InputDeviceManager.Instance == null) return;

        // 現在の入力デバイスを取得（true: コントローラー, false: キーボード）
        bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();

        // プレイヤーが FinalAttack 可能かを取得
        bool isNextSentence = introductionManager.IsNextPossible();

        // UI の表示 / 非表示
        mouseUI.enabled = isNextSentence && !isUsingGamepad;
        keyboardNextUI.enabled = isNextSentence && !isUsingGamepad;
        controllerNextUI.enabled = isNextSentence && isUsingGamepad;
        controllerSkipUI.enabled = isUsingGamepad;

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
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// ユーザーがスキップ操作を行った際に、ゲームシーンへ遷移する。
    /// </summary>
    /// <param name="value">スキップ操作の入力値。</param>
    private void OnSkip(InputValue value)
    {
        SceneManager.LoadScene("GameScene");
    }
}

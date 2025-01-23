using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// StartのUIを点滅させるクラス
/// </summary>
public class UIFader : MonoBehaviour
{
    // フェード対象のImageコンポーネント（マウス/キーボード用）
    [SerializeField] private GameObject keyboardMouseUI;

    // フェード対象のImageコンポーネント（コントローラー用）
    [SerializeField] private GameObject controllerUI;

    // フェードイン・アウトの周期（秒単位）
    [SerializeField] private float fadeDuration = 1f;

    private bool isFadingIn = true; // 現在のフェード方向
    private float currentAlpha = 1f; // 現在のアルファ値
    private CanvasGroup activeCanvasGroup; // 現在のアクティブなCanvasGroup

    private void Start()
    {
        // 初期設定：キーボード/マウス用UIをアクティブに
        UpdateDeviceUI();

        // デバイス変更イベントを監視
        InputSystem.onDeviceChange += OnDeviceChange;

        // フェード処理開始
        StartCoroutine(FadeLoop());
    }

    private void OnDestroy()
    {
        // デバイス変更イベントを解除
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    /// <summary>
    /// 入力デバイスが変更されたときに呼び出される
    /// </summary>
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
        {
            UpdateDeviceUI();
        }
    }

    /// <summary>
    /// 現在の入力デバイスに応じて表示するUIを切り替える
    /// </summary>
    private void UpdateDeviceUI()
    {
        // 入力デバイスの優先順位を確認
        if (Gamepad.current != null) // コントローラーが接続されている場合
        {
            SetActiveUI(controllerUI);
        }
        else if (Mouse.current != null || Keyboard.current != null) // マウスまたはキーボードが接続されている場合
        {
            SetActiveUI(keyboardMouseUI);
        }
    }

    /// <summary>
    /// 指定されたUIをアクティブにする
    /// </summary>
    /// <param name="uiObject">アクティブにするUIのGameObject</param>
    private void SetActiveUI(GameObject uiObject)
    {
        // すべてのUIを非アクティブ化
        keyboardMouseUI.SetActive(false);
        controllerUI.SetActive(false);

        // アクティブにするUIを表示
        uiObject.SetActive(true);

        // CanvasGroupを取得（フェード処理用）
        activeCanvasGroup = uiObject.GetComponent<CanvasGroup>();
        if (activeCanvasGroup == null)
        {
            activeCanvasGroup = uiObject.AddComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// フェード処理を繰り返すコルーチン。
    /// </summary>
    private System.Collections.IEnumerator FadeLoop()
    {
        while (true)
        {
            if (activeCanvasGroup != null)
            {
                float elapsedTime = 0f;
                float startAlpha = isFadingIn ? 0f : 1f;
                float endAlpha = isFadingIn ? 1f : 0f;

                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
                    activeCanvasGroup.alpha = currentAlpha;
                    yield return null;
                }

                isFadingIn = !isFadingIn;
            }

            yield return null;
        }
    }
}

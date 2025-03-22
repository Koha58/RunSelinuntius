using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// StartのUIを点滅させるクラス
/// 入力デバイス（キーボード/マウス or コントローラー）に応じて適切なUIを表示し、
/// フェードイン・フェードアウトを繰り返す。
/// </summary>
public class UIFader : MonoBehaviour
{
    // フェード対象のUI（マウス/キーボード用）
    [SerializeField] private GameObject keyboardMouseUI;

    // フェード対象のUI（コントローラー用）
    [SerializeField] private GameObject controllerUI;

    // フェードイン・アウトの周期（秒単位）
    [SerializeField] private float fadeDuration = DefaultFadeDuration;

    private bool isFadingIn = true; // 現在のフェード方向（true: フェードイン, false: フェードアウト）
    private float currentAlpha = MaxAlpha; // 現在のアルファ値
    private CanvasGroup activeCanvasGroup; // 現在のアクティブなCanvasGroup

    // 定数（マジックナンバーの削除）
    private static readonly float DefaultFadeDuration = 1f; // デフォルトのフェード時間（秒）
    private static readonly float MaxAlpha = 1f; // 最大アルファ値（完全に表示）
    private static readonly float MinAlpha = 0f; // 最小アルファ値（完全に非表示）

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
        // デバイス変更イベントを解除（メモリリーク防止）
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    /// <summary>
    /// 入力デバイスが変更されたときに呼び出される。
    /// 追加または削除が検知された場合、UIを更新する。
    /// </summary>
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
        {
            UpdateDeviceUI();
        }
    }

    /// <summary>
    /// 現在の入力デバイスに応じて表示するUIを切り替える。
    /// </summary>
    private void UpdateDeviceUI()
    {
        // 入力デバイスの優先順位を確認し、適切なUIを表示
        if (Gamepad.current != null) // コントローラーが接続されている場合
        {
            SetActiveUI(controllerUI);
        }
        else // マウスまたはキーボードが接続されている場合
        {
            SetActiveUI(keyboardMouseUI);
        }
    }

    /// <summary>
    /// 指定されたUIをアクティブにし、それ以外を非アクティブにする。
    /// </summary>
    /// <param name="uiObject">アクティブにするUIのGameObject</param>
    private void SetActiveUI(GameObject uiObject)
    {
        // すべてのUIを非アクティブ化
        keyboardMouseUI.SetActive(false);
        controllerUI.SetActive(false);

        // 指定されたUIをアクティブ化
        uiObject.SetActive(true);

        // CanvasGroupを取得（フェード処理用）
        activeCanvasGroup = uiObject.GetComponent<CanvasGroup>();
        if (activeCanvasGroup == null)
        {
            activeCanvasGroup = uiObject.AddComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// フェードイン・フェードアウトを繰り返すコルーチン。
    /// </summary>
    private System.Collections.IEnumerator FadeLoop()
    {
        while (true)
        {
            if (activeCanvasGroup != null)
            {
                float elapsedTime = 0f;
                float startAlpha = isFadingIn ? MinAlpha : MaxAlpha; // 開始時のアルファ値
                float endAlpha = isFadingIn ? MaxAlpha : MinAlpha; // 終了時のアルファ値

                // 指定された時間でフェード処理を行う
                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
                    activeCanvasGroup.alpha = currentAlpha;
                    yield return null;
                }

                // フェードイン/フェードアウトの切り替え
                isFadingIn = !isFadingIn;
            }

            yield return null;
        }
    }
}
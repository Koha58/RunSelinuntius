using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// 操作説明用UIを管理するクラス
/// </summary>
public class ExplanationManager : MonoBehaviour
{
    [SerializeField] private SettingManager settingManager; // SettingManagerへの参照
    [SerializeField] private GameObject[] keyboardUIs; // キーボード用UI
    [SerializeField] private GameObject[] gamepadUIs;  // コントローラー用UI
    [SerializeField] private Image leftCursor; // 左カーソル
    [SerializeField] private Image rightCursor; // 右カーソル
    [SerializeField] private Image buttonAUI; // Aボタン
    [SerializeField] private Image buttonBUI; // Bボタン
    [SerializeField] private Image closeButtonUI; // クローズボタン
    [SerializeField] private Image closeYUI; // コントローラーのYボタン

    private GameObject[] activeUIs; // 現在表示されているUI
    private int currentIndex = 0; // 現在選択されているUIのインデックス
    private bool isPaused = true; // ゲームがポーズ中かどうか

    [SerializeField] private PlayerInput playerInput; // プレイヤー操作用のPlayerInput
    [SerializeField] private PlayerInput uiInput; // UI用のPlayerInput

    private const int FIRST_UI_INDEX = 0; // 最初のUIのインデックス
    private const int LAST_UI_INDEX_OFFSET = 1; // 最後のUIのインデックス計算のためのオフセット

    private float lastInputTime = 0f; // 最後に入力を受け付けた時間
    private float inputDelay = 0.5f; // 連続入力を防ぐための遅延時間
    private const float KEYBOARD_INPUT_DELAY = 10.0f;// キーボード用：連続入力を防ぐための遅延時間
    private const float GAMEPAD_INPUT_DELAY = 0.5f;// ゲームパッド用：連続入力を防ぐための遅延時間


    // 他のスクリプトからカーソルの表示状態を取得するためのプロパティ
    public bool IsLeftCursorVisible => leftCursor.enabled;
    public bool IsRightCursorVisible => rightCursor.enabled;

    // Startメソッドはシーン開始時に呼ばれる
    private void Start()
    {
        // ゲームパッドが接続されているか確認
        bool isUsingGamepad = IsUsingGamepad();
        Debug.Log("Using Gamepad: " + isUsingGamepad);

        // ゲームパッドが接続されていればゲームパッド用UIを使用、それ以外はキーボード用UIを使用
        activeUIs = isUsingGamepad ? gamepadUIs : keyboardUIs;

        // すべてのUIを非表示にする
        DisableAllUI();

        // 初期状態で非表示のボタンを設定
        closeButtonUI.enabled = false;
        closeYUI.enabled = false;

        // ゲームパッドが使用されている場合のみボタンUIを表示
        buttonAUI.gameObject.SetActive(isUsingGamepad);
        buttonBUI.gameObject.SetActive(isUsingGamepad);

        // ゲームをポーズ状態にする
        Time.timeScale = 0;

        // UIの初期状態を更新
        StartCoroutine(DelayedUpdateUI());
    }

    // ゲームパッドが接続されているか確認するメソッド
    private bool IsUsingGamepad()
    {
        var gamepads = Gamepad.all; // 接続されているゲームパッドのリストを取得
        return gamepads.Count > 0; // 1台でも接続されていればゲームパッド使用中とみなす
    }

    // 1フレーム遅れてUIを更新するコルーチン
    private IEnumerator DelayedUpdateUI()
    {
        yield return null; // 1フレーム待つ
        UpdateUI(); // UIを更新
    }

    // UIの表示状態を更新するメソッド
    private void UpdateUI()
    {
        bool isUsingGamepad = IsUsingGamepad(); // 現在ゲームパッドを使用中か確認

        if (currentIndex == -1)
        {
            // ポーズ解除時の処理
            Time.timeScale = 1; // ゲームを再開
            isPaused = false; // ポーズを解除
            closeButtonUI.enabled = false; // クローズボタン非表示
            closeYUI.enabled = false; // コントローラーのYボタン非表示
            buttonAUI.enabled = false; // Aボタン非表示

            // UI用の入力を無効化し、プレイヤー操作用の入力を有効化
            uiInput.enabled = false; // UI用入力無効
            playerInput.enabled = true; // プレイヤー操作用入力有効
        }
        else
        {
            // ポーズ中の処理
            Time.timeScale = 0; // ゲームを停止
            isPaused = true; // ポーズ状態
            uiInput.enabled = true; // UI用入力有効
            playerInput.enabled = false; // プレイヤー操作用入力無効
        }

        // 現在のインデックスに対応するUIだけを表示
        for (int i = 0; i < activeUIs.Length; i++)
        {
            activeUIs[i].SetActive(i == currentIndex);
        }

        // カーソルの表示/非表示
        leftCursor.enabled = currentIndex > FIRST_UI_INDEX; // 左カーソルは最初のUIより後に表示
        rightCursor.enabled = currentIndex < activeUIs.Length - LAST_UI_INDEX_OFFSET && currentIndex != -1; // 右カーソルは最後のUIより前に表示

        // ゲームパッド使用時にのみAボタン、Bボタンを表示
        buttonAUI.gameObject.SetActive(isUsingGamepad && currentIndex < activeUIs.Length - LAST_UI_INDEX_OFFSET);
        buttonBUI.gameObject.SetActive(isUsingGamepad && currentIndex > FIRST_UI_INDEX);

        // 最後のUIであればクローズボタンを表示
        bool isLastUI = currentIndex == activeUIs.Length - LAST_UI_INDEX_OFFSET;
        closeButtonUI.enabled = isLastUI;
        closeYUI.enabled = isLastUI && isUsingGamepad;

        Debug.Log($"Current Index: {currentIndex}, IsUsingGamepad: {isUsingGamepad}, Close UI Active: {closeButtonUI.enabled}");
    }

    // すべてのUIを非表示にするメソッド
    private void DisableAllUI()
    {
        foreach (var ui in keyboardUIs) ui.SetActive(false);
        foreach (var ui in gamepadUIs) ui.SetActive(false);
    }

    // クリック入力の処理（次のUIに進む）
    private void OnNext(InputValue value)
    {
        bool isUsingGamepad = IsUsingGamepad(); // 現在ゲームパッドを使用中か確認

        if (!isUsingGamepad)
        {
            inputDelay = KEYBOARD_INPUT_DELAY;
        }
        else
        {
            inputDelay = GAMEPAD_INPUT_DELAY;
        }

        // 連続入力を防ぐため、一定時間（INPUT_DELAY）経過していない場合は処理しない
        if (Time.unscaledTime - lastInputTime < inputDelay) return;

        bool isPressed = value.isPressed; // 入力が押されたかどうかを取得
        if (isPressed && isPaused && !settingManager.IsSettingActive) // ボタンが押され、かつUIが表示中の場合のみ処理
        {
            Next(); // 次のUIへ進む
            lastInputTime = Time.unscaledTime; // 最後の入力時間を更新（次の入力までの待機時間を管理）
        }
    }

    // 戻る入力の処理（前のUIに戻る）
    private void OnReturn(InputValue value)
    {
        bool isUsingGamepad = IsUsingGamepad(); // 現在ゲームパッドを使用中か確認

        if (!isUsingGamepad)
        {
            inputDelay = KEYBOARD_INPUT_DELAY;
        }
        else
        {
            inputDelay = GAMEPAD_INPUT_DELAY;
        }

        // 連続入力を防ぐため、一定時間（INPUT_DELAY）経過していない場合は処理しない
        if (Time.unscaledTime - lastInputTime < inputDelay) return;

        bool isPressed = value.isPressed; // 入力が押されたかどうかを取得
        if (isPressed && isPaused) // ボタンが押され、かつUIが表示中の場合のみ処理
        {
            Back(); // 前のUIに戻る
            lastInputTime = Time.unscaledTime; // 最後の入力時間を更新（次の入力までの待機時間を管理）
        }
    }

    // クローズボタンの入力処理
    private void OnClose(InputValue value)
    {
        bool isPressed = value.isPressed;

        // クローズボタンが押され、クローズボタンUIが表示されており、現在のUIが最後であれば
        if (isPressed && closeButtonUI.enabled && isPaused && currentIndex == activeUIs.Length - LAST_UI_INDEX_OFFSET)
        {
            Close(); // UIを閉じる
        }
    }

    // 次のUIに進む処理
    public void Next()
    {
        if (!isPaused) return; // ゲームがポーズ中でなければ何もしない

        if (currentIndex < activeUIs.Length - LAST_UI_INDEX_OFFSET)
        {
            currentIndex++; // 次のUIへ進む
        }
        else
        {
            // 現在が最後のUIの場合、次のUIには進まない
            return;
        }
        UpdateUI(); // UIを更新
    }

    // 前のUIに戻る処理
    public void Back()
    {
        if (!isPaused) return; // ゲームがポーズ中でなければ何もしない

        if (currentIndex > FIRST_UI_INDEX)
        {
            currentIndex--; // 前のUIに戻る
        }
        UpdateUI(); // UIを更新
    }

    // UIを閉じる処理
    public void Close()
    {
        currentIndex = -1; // UIをすべて非表示にする
        UpdateUI(); // UIを更新
    }

}

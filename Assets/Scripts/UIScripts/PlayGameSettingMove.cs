using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// ポーズ時の処理を管理するクラス
/// </summary>
public class PlayGameSettingMove : MonoBehaviour
{
    [SerializeField] private SettingManager settingManager; // SettingManagerへの参照
    [SerializeField] private ExplanationManager explanationManager; // ExplanationManagerへの参照
    [SerializeField] private PlayerMove playerMove; // SettingManagerへの参照
    [SerializeField] private Image keyboardPauseUI; // キーボード用ポーズUI
    [SerializeField] private Image controllerPauseUI; // コントローラー用ポーズUI

    [SerializeField] private PlayerInput playerInput; // プレイヤー操作用のPlayerInput
    [SerializeField] private PlayerInput uiInput; // UI用のPlayerInput

    // Update is called once per frame
    void Update()
    {
        // 現在の入力デバイスを取得（true: コントローラー, false: マウス）
        bool isUsingGamepad = Gamepad.current != null;  // コントローラーの判定

        // コントローラーが使用されている場合、コントローラーUIを表示し、キーボードUIを非表示
        if (isUsingGamepad)
        {
            keyboardPauseUI.enabled = false; // キーボードUIを非表示
            controllerPauseUI.enabled = true; // コントローラーUIを表示
        }
        // キーボードが使用されている場合、キーボードUIを表示し、コントローラーUIを非表示
        else
        {
            keyboardPauseUI.enabled = true; // キーボードUIを表示
            controllerPauseUI.enabled = false; // コントローラーUIを非表示
        }

        // 設定メニューが表示されていない、かつ説明のカーソルが非表示の場合のみゲームを再開
        if (!settingManager.IsSettingActive && !playerMove.IsSlowMotionEnabled() && (!explanationManager.IsLeftCursorVisible && !explanationManager.IsRightCursorVisible))
        {
            Time.timeScale = 1; // ゲームを再生（ポーズ解除）

            // UI用の入力を無効化し、プレイヤー操作用の入力を有効化
            uiInput.enabled = false; // UI用入力無効化
            playerInput.enabled = true; // プレイヤー操作用入力有効化
        }
    }

    /// <summary>
    /// ポーズボタンが押されたときに呼び出されるメソッド
    /// </summary>
    /// <param name="value">入力値</param>
    private void OnPause(InputValue value)
    {
        // 設定メニューを表示
        settingManager.ToggleSettingMenu(true);

        // ポーズ中の処理
        Time.timeScale = 0; // ゲームを停止（ポーズ）

        // UI用の入力を有効化し、プレイヤー操作用の入力を無効化
        uiInput.enabled = true; // UI用入力有効化
        playerInput.enabled = false; // プレイヤー操作用入力無効化
    }
}

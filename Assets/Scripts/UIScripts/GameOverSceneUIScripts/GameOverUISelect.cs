using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// GameOverのUIをデバイスに応じて切り替えるクラス
/// </summary>
public class GameOverUISelect : MonoBehaviour
{
    // マウス/キーボード用のUI
    [SerializeField] private GameObject keyboardMouseUI;

    // コントローラー用のUI
    [SerializeField] private GameObject controllerUI;

    private void Start()
    {
        // 初期設定：入力デバイスに応じてUIを切り替える
        UpdateDeviceUI();

        // デバイス変更イベントを監視
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDestroy()
    {
        // デバイス変更イベントを解除
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    /// <summary>
    /// 入力デバイスが変更された際に呼び出されるコールバック。
    /// UIを適切に更新する。
    /// </summary>
    /// <param name="device">変更が発生した入力デバイス</param>
    /// <param name="change">デバイスの変更内容（追加・削除など）</param>
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
        // 入力デバイスの優先順位を確認してUIを切り替え
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

        // 指定されたUIをアクティブ化
        if (uiObject != null)
        {
            uiObject.SetActive(true);
        }
    }
}

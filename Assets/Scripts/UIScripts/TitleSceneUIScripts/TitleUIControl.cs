using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル画面の遷移を管理するクラス
/// </summary>
public class TitleUIControl : MonoBehaviour
{
    void Update()
    {
        // マウスまたはキーボード入力を検知
        if (Mouse.current?.leftButton.wasPressedThisFrame == true || Keyboard.current?.enterKey.wasPressedThisFrame == true)
        {
            LoadGameScene();
        }

        // コントローラー入力を検知
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) // "A"ボタン
        {
            LoadGameScene();
        }
    }

    /// <summary>
    /// ゲームシーンをロードする
    /// </summary>
    private void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

}

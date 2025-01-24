using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームクリア画面の遷移を管理するクラス
/// </summary>
public class GameClearControl : MonoBehaviour
{
    [SerializeField] private float idleTimeToReturn = 5f; // 入力がないときにタイトル画面に戻るまでの時間（秒）
    private float idleTimer = 0f; // 入力がない時間をカウントするタイマー

    void Update()
    {
        idleTimer += Time.deltaTime; // 入力がない間、タイマーを増加

        if (idleTimer >= idleTimeToReturn) // 10秒経過したらタイトル画面に戻る
        {
            LoadTitleScene();
        }
    }

    /// <summary>
    /// 入力があるかをチェックする
    /// </summary>
    /// <returns>入力があればtrue、なければfalse</returns>
    private bool CheckForInput()
    {
        // マウスまたはキーボードの入力をチェック
        if (Mouse.current?.leftButton.wasPressedThisFrame == true || Keyboard.current?.enterKey.wasPressedThisFrame == true)
        {
            return true;
        }

        // コントローラーの入力をチェック
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) // "A"ボタン
        {
            return true;
        }

        return false; // 入力がなければfalseを返す
    }

    /// <summary>
    /// タイトル画面をロードする
    /// </summary>
    private void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}

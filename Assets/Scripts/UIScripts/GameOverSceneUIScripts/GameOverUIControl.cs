using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームオーバー画面の遷移を管理するクラス
/// </summary>
public class GameOverUIControl : MonoBehaviour
{
    [SerializeField] private float idleTimeToReturn = 10f; // 入力がないときにタイトル画面に戻るまでの時間（秒）
    private float idleTimer = 0f; // 入力がない時間をカウントするタイマー

    void Update()
    {
        // 入力があるかチェック
        if (CheckForInput())
        {
            idleTimer = 0f; // 入力があればタイマーをリセット
            LoadGameScene();
        }
        else
        {
            idleTimer += Time.deltaTime; // 入力がない間、タイマーを増加

            if (idleTimer >= idleTimeToReturn) // 10秒経過したらタイトル画面に戻る
            {
                LoadTitleScene();
            }
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
    /// ゲームシーンをロードする
    /// </summary>
    private void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// タイトル画面をロードする
    /// </summary>
    private void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}

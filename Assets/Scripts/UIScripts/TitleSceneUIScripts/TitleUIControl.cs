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

    }

    /// <summary>
    /// 左クリック/Aボタンでシーン遷移
    /// </summary>
    private void OnClick(InputValue value)
    {
        // 左クリック/Aボタンが押されたら
        if (value.isPressed)
        {
            SceneManager.LoadScene("IntroductionScene");
        }
    }

}

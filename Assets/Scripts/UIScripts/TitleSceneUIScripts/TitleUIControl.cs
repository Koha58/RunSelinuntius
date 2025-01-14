using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル画面のUIを管理するクラス
/// </summary>
public class TitleUIControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 左クリックを検知
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // シーンを切り替える
            SceneManager.LoadScene("GameScene");
        }
    }

}

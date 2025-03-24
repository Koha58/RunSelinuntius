using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ExplanationManager : MonoBehaviour
{
    [SerializeField] private GameObject[] keyboardUIs; // キーボード用UI
    [SerializeField] private GameObject[] gamepadUIs;  // コントローラー用UI
    [SerializeField] private Image leftCursor;
    [SerializeField] private Image rightCursor;
    [SerializeField] private Image buttonAui;
    [SerializeField] private Image buttonBui;

    private GameObject[] activeUIs;
    private int currentIndex = 0;
    private bool isPaused = true; // 初期状態はポーズ

    void Start()
    {
        bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();
        activeUIs = isUsingGamepad ? gamepadUIs : keyboardUIs;

        DisableAllUI(); // すべてのUIを非表示にする
        UpdateUI(); // 適切なUIを表示する

        buttonAui.enabled = isUsingGamepad;
        buttonBui.enabled = isUsingGamepad;

        Time.timeScale = 0; // ポーズ開始
    }

    private void DisableAllUI()
    {
        foreach (var ui in keyboardUIs) ui.SetActive(false);
        foreach (var ui in gamepadUIs) ui.SetActive(false);
    }

    private void UpdateUI()
    {
        if (currentIndex == -1)
        {
            Time.timeScale = 1; // ゲーム再開
            isPaused = false;
        }
        else
        {
            Time.timeScale = 0; // ゲーム停止
            isPaused = true;
        }

        for (int i = 0; i < activeUIs.Length; i++)
        {
            activeUIs[i].SetActive(i == currentIndex);
        }

        // カーソルとボタンの有効化/無効化
        leftCursor.enabled = currentIndex > 0;
        buttonBui.enabled = currentIndex > 0;
        rightCursor.enabled = currentIndex < activeUIs.Length - 1;
        buttonAui.enabled = currentIndex < activeUIs.Length - 1;
    }

    private void OnClick(InputValue value)
    {
        if (!value.isPressed || !isPaused) return;

        currentIndex++;
        if (currentIndex >= activeUIs.Length)
        {
            currentIndex = -1; // UI 非表示＆ポーズ解除
        }
        UpdateUI();
    }

    private void OnBack(InputValue value)
    {
        if (!value.isPressed || !isPaused) return;

        if (currentIndex > 0)
        {
            currentIndex--;
        }
        UpdateUI();
    }

    public void Next()
    {
        if (!isPaused) return;

        currentIndex++;
        if (currentIndex >= activeUIs.Length)
        {
            currentIndex = -1; // すべて非表示
        }
        UpdateUI();
    }

    public void Back()
    {
        if (!isPaused) return;

        if (currentIndex > 0)
        {
            currentIndex--;
        }
        UpdateUI();
    }
}

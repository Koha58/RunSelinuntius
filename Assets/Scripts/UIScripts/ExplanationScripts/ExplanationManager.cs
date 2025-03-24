using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ExplanationManager : MonoBehaviour
{
    [SerializeField] private GameObject[] keyboardUIs; // �L�[�{�[�h�pUI
    [SerializeField] private GameObject[] gamepadUIs;  // �R���g���[���[�pUI
    [SerializeField] private Image leftCursor;
    [SerializeField] private Image rightCursor;
    [SerializeField] private Image buttonAui;
    [SerializeField] private Image buttonBui;

    private GameObject[] activeUIs;
    private int currentIndex = 0;
    private bool isPaused = true; // ������Ԃ̓|�[�Y

    void Start()
    {
        bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();
        activeUIs = isUsingGamepad ? gamepadUIs : keyboardUIs;

        DisableAllUI(); // ���ׂĂ�UI���\���ɂ���
        UpdateUI(); // �K�؂�UI��\������

        buttonAui.enabled = isUsingGamepad;
        buttonBui.enabled = isUsingGamepad;

        Time.timeScale = 0; // �|�[�Y�J�n
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
            Time.timeScale = 1; // �Q�[���ĊJ
            isPaused = false;
        }
        else
        {
            Time.timeScale = 0; // �Q�[����~
            isPaused = true;
        }

        for (int i = 0; i < activeUIs.Length; i++)
        {
            activeUIs[i].SetActive(i == currentIndex);
        }

        // �J�[�\���ƃ{�^���̗L����/������
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
            currentIndex = -1; // UI ��\�����|�[�Y����
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
            currentIndex = -1; // ���ׂĔ�\��
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

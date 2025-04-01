using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// ��������pUI���Ǘ�����N���X
/// </summary>
public class ExplanationManager : MonoBehaviour
{
    [SerializeField] private SettingManager settingManager; // SettingManager�ւ̎Q��
    [SerializeField] private GameObject[] keyboardUIs; // �L�[�{�[�h�pUI
    [SerializeField] private GameObject[] gamepadUIs;  // �R���g���[���[�pUI
    [SerializeField] private Image leftCursor; // ���J�[�\��
    [SerializeField] private Image rightCursor; // �E�J�[�\��
    [SerializeField] private Image buttonAUI; // A�{�^��
    [SerializeField] private Image buttonBUI; // B�{�^��
    [SerializeField] private Image closeButtonUI; // �N���[�Y�{�^��
    [SerializeField] private Image closeYUI; // �R���g���[���[��Y�{�^��

    private GameObject[] activeUIs; // ���ݕ\������Ă���UI
    private int currentIndex = 0; // ���ݑI������Ă���UI�̃C���f�b�N�X
    private bool isPaused = true; // �Q�[�����|�[�Y�����ǂ���

    [SerializeField] private PlayerInput playerInput; // �v���C���[����p��PlayerInput
    [SerializeField] private PlayerInput uiInput; // UI�p��PlayerInput

    private const int FIRST_UI_INDEX = 0; // �ŏ���UI�̃C���f�b�N�X
    private const int LAST_UI_INDEX_OFFSET = 1; // �Ō��UI�̃C���f�b�N�X�v�Z�̂��߂̃I�t�Z�b�g

    private float lastInputTime = 0f; // �Ō�ɓ��͂��󂯕t��������
    private float inputDelay = 0.5f; // �A�����͂�h�����߂̒x������
    private const float KEYBOARD_INPUT_DELAY = 10.0f;// �L�[�{�[�h�p�F�A�����͂�h�����߂̒x������
    private const float GAMEPAD_INPUT_DELAY = 0.5f;// �Q�[���p�b�h�p�F�A�����͂�h�����߂̒x������


    // ���̃X�N���v�g����J�[�\���̕\����Ԃ��擾���邽�߂̃v���p�e�B
    public bool IsLeftCursorVisible => leftCursor.enabled;
    public bool IsRightCursorVisible => rightCursor.enabled;

    // Start���\�b�h�̓V�[���J�n���ɌĂ΂��
    private void Start()
    {
        // �Q�[���p�b�h���ڑ�����Ă��邩�m�F
        bool isUsingGamepad = IsUsingGamepad();
        Debug.Log("Using Gamepad: " + isUsingGamepad);

        // �Q�[���p�b�h���ڑ�����Ă���΃Q�[���p�b�h�pUI���g�p�A����ȊO�̓L�[�{�[�h�pUI���g�p
        activeUIs = isUsingGamepad ? gamepadUIs : keyboardUIs;

        // ���ׂĂ�UI���\���ɂ���
        DisableAllUI();

        // ������ԂŔ�\���̃{�^����ݒ�
        closeButtonUI.enabled = false;
        closeYUI.enabled = false;

        // �Q�[���p�b�h���g�p����Ă���ꍇ�̂݃{�^��UI��\��
        buttonAUI.gameObject.SetActive(isUsingGamepad);
        buttonBUI.gameObject.SetActive(isUsingGamepad);

        // �Q�[�����|�[�Y��Ԃɂ���
        Time.timeScale = 0;

        // UI�̏�����Ԃ��X�V
        StartCoroutine(DelayedUpdateUI());
    }

    // �Q�[���p�b�h���ڑ�����Ă��邩�m�F���郁�\�b�h
    private bool IsUsingGamepad()
    {
        var gamepads = Gamepad.all; // �ڑ�����Ă���Q�[���p�b�h�̃��X�g���擾
        return gamepads.Count > 0; // 1��ł��ڑ�����Ă���΃Q�[���p�b�h�g�p���Ƃ݂Ȃ�
    }

    // 1�t���[���x���UI���X�V����R���[�`��
    private IEnumerator DelayedUpdateUI()
    {
        yield return null; // 1�t���[���҂�
        UpdateUI(); // UI���X�V
    }

    // UI�̕\����Ԃ��X�V���郁�\�b�h
    private void UpdateUI()
    {
        bool isUsingGamepad = IsUsingGamepad(); // ���݃Q�[���p�b�h���g�p�����m�F

        if (currentIndex == -1)
        {
            // �|�[�Y�������̏���
            Time.timeScale = 1; // �Q�[�����ĊJ
            isPaused = false; // �|�[�Y������
            closeButtonUI.enabled = false; // �N���[�Y�{�^����\��
            closeYUI.enabled = false; // �R���g���[���[��Y�{�^����\��
            buttonAUI.enabled = false; // A�{�^����\��

            // UI�p�̓��͂𖳌������A�v���C���[����p�̓��͂�L����
            uiInput.enabled = false; // UI�p���͖���
            playerInput.enabled = true; // �v���C���[����p���͗L��
        }
        else
        {
            // �|�[�Y���̏���
            Time.timeScale = 0; // �Q�[�����~
            isPaused = true; // �|�[�Y���
            uiInput.enabled = true; // UI�p���͗L��
            playerInput.enabled = false; // �v���C���[����p���͖���
        }

        // ���݂̃C���f�b�N�X�ɑΉ�����UI������\��
        for (int i = 0; i < activeUIs.Length; i++)
        {
            activeUIs[i].SetActive(i == currentIndex);
        }

        // �J�[�\���̕\��/��\��
        leftCursor.enabled = currentIndex > FIRST_UI_INDEX; // ���J�[�\���͍ŏ���UI����ɕ\��
        rightCursor.enabled = currentIndex < activeUIs.Length - LAST_UI_INDEX_OFFSET && currentIndex != -1; // �E�J�[�\���͍Ō��UI���O�ɕ\��

        // �Q�[���p�b�h�g�p���ɂ̂�A�{�^���AB�{�^����\��
        buttonAUI.gameObject.SetActive(isUsingGamepad && currentIndex < activeUIs.Length - LAST_UI_INDEX_OFFSET);
        buttonBUI.gameObject.SetActive(isUsingGamepad && currentIndex > FIRST_UI_INDEX);

        // �Ō��UI�ł���΃N���[�Y�{�^����\��
        bool isLastUI = currentIndex == activeUIs.Length - LAST_UI_INDEX_OFFSET;
        closeButtonUI.enabled = isLastUI;
        closeYUI.enabled = isLastUI && isUsingGamepad;

        Debug.Log($"Current Index: {currentIndex}, IsUsingGamepad: {isUsingGamepad}, Close UI Active: {closeButtonUI.enabled}");
    }

    // ���ׂĂ�UI���\���ɂ��郁�\�b�h
    private void DisableAllUI()
    {
        foreach (var ui in keyboardUIs) ui.SetActive(false);
        foreach (var ui in gamepadUIs) ui.SetActive(false);
    }

    // �N���b�N���͂̏����i����UI�ɐi�ށj
    private void OnNext(InputValue value)
    {
        bool isUsingGamepad = IsUsingGamepad(); // ���݃Q�[���p�b�h���g�p�����m�F

        if (!isUsingGamepad)
        {
            inputDelay = KEYBOARD_INPUT_DELAY;
        }
        else
        {
            inputDelay = GAMEPAD_INPUT_DELAY;
        }

        // �A�����͂�h�����߁A��莞�ԁiINPUT_DELAY�j�o�߂��Ă��Ȃ��ꍇ�͏������Ȃ�
        if (Time.unscaledTime - lastInputTime < inputDelay) return;

        bool isPressed = value.isPressed; // ���͂������ꂽ���ǂ������擾
        if (isPressed && isPaused && !settingManager.IsSettingActive) // �{�^����������A����UI���\�����̏ꍇ�̂ݏ���
        {
            Next(); // ����UI�֐i��
            lastInputTime = Time.unscaledTime; // �Ō�̓��͎��Ԃ��X�V�i���̓��͂܂ł̑ҋ@���Ԃ��Ǘ��j
        }
    }

    // �߂���͂̏����i�O��UI�ɖ߂�j
    private void OnReturn(InputValue value)
    {
        bool isUsingGamepad = IsUsingGamepad(); // ���݃Q�[���p�b�h���g�p�����m�F

        if (!isUsingGamepad)
        {
            inputDelay = KEYBOARD_INPUT_DELAY;
        }
        else
        {
            inputDelay = GAMEPAD_INPUT_DELAY;
        }

        // �A�����͂�h�����߁A��莞�ԁiINPUT_DELAY�j�o�߂��Ă��Ȃ��ꍇ�͏������Ȃ�
        if (Time.unscaledTime - lastInputTime < inputDelay) return;

        bool isPressed = value.isPressed; // ���͂������ꂽ���ǂ������擾
        if (isPressed && isPaused) // �{�^����������A����UI���\�����̏ꍇ�̂ݏ���
        {
            Back(); // �O��UI�ɖ߂�
            lastInputTime = Time.unscaledTime; // �Ō�̓��͎��Ԃ��X�V�i���̓��͂܂ł̑ҋ@���Ԃ��Ǘ��j
        }
    }

    // �N���[�Y�{�^���̓��͏���
    private void OnClose(InputValue value)
    {
        bool isPressed = value.isPressed;

        // �N���[�Y�{�^����������A�N���[�Y�{�^��UI���\������Ă���A���݂�UI���Ō�ł����
        if (isPressed && closeButtonUI.enabled && isPaused && currentIndex == activeUIs.Length - LAST_UI_INDEX_OFFSET)
        {
            Close(); // UI�����
        }
    }

    // ����UI�ɐi�ޏ���
    public void Next()
    {
        if (!isPaused) return; // �Q�[�����|�[�Y���łȂ���Ή������Ȃ�

        if (currentIndex < activeUIs.Length - LAST_UI_INDEX_OFFSET)
        {
            currentIndex++; // ����UI�֐i��
        }
        else
        {
            // ���݂��Ō��UI�̏ꍇ�A����UI�ɂ͐i�܂Ȃ�
            return;
        }
        UpdateUI(); // UI���X�V
    }

    // �O��UI�ɖ߂鏈��
    public void Back()
    {
        if (!isPaused) return; // �Q�[�����|�[�Y���łȂ���Ή������Ȃ�

        if (currentIndex > FIRST_UI_INDEX)
        {
            currentIndex--; // �O��UI�ɖ߂�
        }
        UpdateUI(); // UI���X�V
    }

    // UI����鏈��
    public void Close()
    {
        currentIndex = -1; // UI�����ׂĔ�\���ɂ���
        UpdateUI(); // UI���X�V
    }

}

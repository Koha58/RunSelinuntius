using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// �|�[�Y���̏������Ǘ�����N���X
/// </summary>
public class PlayGameSettingMove : MonoBehaviour
{
    [SerializeField] private SettingManager settingManager; // SettingManager�ւ̎Q��
    [SerializeField] private ExplanationManager explanationManager; // ExplanationManager�ւ̎Q��
    [SerializeField] private PlayerMove playerMove; // SettingManager�ւ̎Q��
    [SerializeField] private Image keyboardPauseUI; // �L�[�{�[�h�p�|�[�YUI
    [SerializeField] private Image controllerPauseUI; // �R���g���[���[�p�|�[�YUI

    [SerializeField] private PlayerInput playerInput; // �v���C���[����p��PlayerInput
    [SerializeField] private PlayerInput uiInput; // UI�p��PlayerInput

    // Update is called once per frame
    void Update()
    {
        // ���݂̓��̓f�o�C�X���擾�itrue: �R���g���[���[, false: �}�E�X�j
        bool isUsingGamepad = Gamepad.current != null;  // �R���g���[���[�̔���

        // �R���g���[���[���g�p����Ă���ꍇ�A�R���g���[���[UI��\�����A�L�[�{�[�hUI���\��
        if (isUsingGamepad)
        {
            keyboardPauseUI.enabled = false; // �L�[�{�[�hUI���\��
            controllerPauseUI.enabled = true; // �R���g���[���[UI��\��
        }
        // �L�[�{�[�h���g�p����Ă���ꍇ�A�L�[�{�[�hUI��\�����A�R���g���[���[UI���\��
        else
        {
            keyboardPauseUI.enabled = true; // �L�[�{�[�hUI��\��
            controllerPauseUI.enabled = false; // �R���g���[���[UI���\��
        }

        // �ݒ胁�j���[���\������Ă��Ȃ��A�������̃J�[�\������\���̏ꍇ�̂݃Q�[�����ĊJ
        if (!settingManager.IsSettingActive && !playerMove.IsSlowMotionEnabled() && (!explanationManager.IsLeftCursorVisible && !explanationManager.IsRightCursorVisible))
        {
            Time.timeScale = 1; // �Q�[�����Đ��i�|�[�Y�����j

            // UI�p�̓��͂𖳌������A�v���C���[����p�̓��͂�L����
            uiInput.enabled = false; // UI�p���͖�����
            playerInput.enabled = true; // �v���C���[����p���͗L����
        }
    }

    /// <summary>
    /// �|�[�Y�{�^���������ꂽ�Ƃ��ɌĂяo����郁�\�b�h
    /// </summary>
    /// <param name="value">���͒l</param>
    private void OnPause(InputValue value)
    {
        // �ݒ胁�j���[��\��
        settingManager.ToggleSettingMenu(true);

        // �|�[�Y���̏���
        Time.timeScale = 0; // �Q�[�����~�i�|�[�Y�j

        // UI�p�̓��͂�L�������A�v���C���[����p�̓��͂𖳌���
        uiInput.enabled = true; // UI�p���͗L����
        playerInput.enabled = false; // �v���C���[����p���͖�����
    }
}

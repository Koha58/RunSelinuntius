using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// GameOver��UI���f�o�C�X�ɉ����Đ؂�ւ���N���X
/// </summary>
public class GameOverUISelect : MonoBehaviour
{
    // �}�E�X/�L�[�{�[�h�p��UI
    [SerializeField] private GameObject keyboardMouseUI;

    // �R���g���[���[�p��UI
    [SerializeField] private GameObject controllerUI;

    private void Start()
    {
        // �����ݒ�F���̓f�o�C�X�ɉ�����UI��؂�ւ���
        UpdateDeviceUI();

        // �f�o�C�X�ύX�C�x���g���Ď�
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDestroy()
    {
        // �f�o�C�X�ύX�C�x���g������
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    /// <summary>
    /// ���̓f�o�C�X���ύX���ꂽ�ۂɌĂяo�����R�[���o�b�N�B
    /// UI��K�؂ɍX�V����B
    /// </summary>
    /// <param name="device">�ύX�������������̓f�o�C�X</param>
    /// <param name="change">�f�o�C�X�̕ύX���e�i�ǉ��E�폜�Ȃǁj</param>
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
        {
            UpdateDeviceUI();
        }
    }

    /// <summary>
    /// ���݂̓��̓f�o�C�X�ɉ����ĕ\������UI��؂�ւ���
    /// </summary>
    private void UpdateDeviceUI()
    {
        // ���̓f�o�C�X�̗D�揇�ʂ��m�F����UI��؂�ւ�
        if (Gamepad.current != null) // �R���g���[���[���ڑ�����Ă���ꍇ
        {
            SetActiveUI(controllerUI);
        }
        else if (Mouse.current != null || Keyboard.current != null) // �}�E�X�܂��̓L�[�{�[�h���ڑ�����Ă���ꍇ
        {
            SetActiveUI(keyboardMouseUI);
        }
    }

    /// <summary>
    /// �w�肳�ꂽUI���A�N�e�B�u�ɂ���
    /// </summary>
    /// <param name="uiObject">�A�N�e�B�u�ɂ���UI��GameObject</param>
    private void SetActiveUI(GameObject uiObject)
    {
        // ���ׂĂ�UI���A�N�e�B�u��
        keyboardMouseUI.SetActive(false);
        controllerUI.SetActive(false);

        // �w�肳�ꂽUI���A�N�e�B�u��
        if (uiObject != null)
        {
            uiObject.SetActive(true);
        }
    }
}

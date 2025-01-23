using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// �^�C�g����ʂ̑J�ڂ��Ǘ�����N���X
/// </summary>
public class TitleUIControl : MonoBehaviour
{
    void Update()
    {
        // �}�E�X�܂��̓L�[�{�[�h���͂����m
        if (Mouse.current?.leftButton.wasPressedThisFrame == true || Keyboard.current?.enterKey.wasPressedThisFrame == true)
        {
            LoadGameScene();
        }

        // �R���g���[���[���͂����m
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) // "A"�{�^��
        {
            LoadGameScene();
        }
    }

    /// <summary>
    /// �Q�[���V�[�������[�h����
    /// </summary>
    private void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

}

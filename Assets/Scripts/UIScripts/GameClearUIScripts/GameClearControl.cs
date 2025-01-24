using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// �Q�[���N���A��ʂ̑J�ڂ��Ǘ�����N���X
/// </summary>
public class GameClearControl : MonoBehaviour
{
    [SerializeField] private float idleTimeToReturn = 5f; // ���͂��Ȃ��Ƃ��Ƀ^�C�g����ʂɖ߂�܂ł̎��ԁi�b�j
    private float idleTimer = 0f; // ���͂��Ȃ����Ԃ��J�E���g����^�C�}�[

    void Update()
    {
        idleTimer += Time.deltaTime; // ���͂��Ȃ��ԁA�^�C�}�[�𑝉�

        if (idleTimer >= idleTimeToReturn) // 10�b�o�߂�����^�C�g����ʂɖ߂�
        {
            LoadTitleScene();
        }
    }

    /// <summary>
    /// ���͂����邩���`�F�b�N����
    /// </summary>
    /// <returns>���͂������true�A�Ȃ����false</returns>
    private bool CheckForInput()
    {
        // �}�E�X�܂��̓L�[�{�[�h�̓��͂��`�F�b�N
        if (Mouse.current?.leftButton.wasPressedThisFrame == true || Keyboard.current?.enterKey.wasPressedThisFrame == true)
        {
            return true;
        }

        // �R���g���[���[�̓��͂��`�F�b�N
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) // "A"�{�^��
        {
            return true;
        }

        return false; // ���͂��Ȃ����false��Ԃ�
    }

    /// <summary>
    /// �^�C�g����ʂ����[�h����
    /// </summary>
    private void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}

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

    }

    /// <summary>
    /// ���N���b�N/A�{�^���ŃV�[���J��
    /// </summary>
    private void OnClick(InputValue value)
    {
        // ���N���b�N/A�{�^���������ꂽ��
        if (value.isPressed)
        {
            SceneManager.LoadScene("IntroductionScene");
        }
    }

}

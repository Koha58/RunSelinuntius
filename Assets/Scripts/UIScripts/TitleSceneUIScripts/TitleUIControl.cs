using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// �^�C�g����ʂ�UI���Ǘ�����N���X
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
        // ���N���b�N�����m
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // �V�[����؂�ւ���
            SceneManager.LoadScene("GameScene");
        }
    }

}

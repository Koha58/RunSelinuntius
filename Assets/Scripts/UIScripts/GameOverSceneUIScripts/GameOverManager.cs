using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// �Q�[���I�[�o�[�̃e�L�X�g���Ǘ�����N���X
/// </summary>
public class GameOverManager : MonoBehaviour
{
    [SerializeField] private Text dialogueText; // �e�L�X�g��\������UI�R���|�[�l���g
    [SerializeField] private float typingSpeed = 0.1f; // ������\������Ԋu�i�b�j
    [SerializeField] private Color normalTextColor = Color.white; // �ʏ�̃e�L�X�g�F�i���j

    // UI�I�u�W�F�N�g�̎Q��
    [SerializeField] private GameObject backGroundUI; // �w�iUI
    [SerializeField] private Image yesUI;  // ���g���C->�u�͂��v�{�^�� UI
    [SerializeField] private Image noUI; // ���g���C->�u�������v�{�^�� UI
    [SerializeField] private Image retryUI;  // �R���g���[���[�p���g���C�{�^�� UI
    [SerializeField] private Image titleUI; // �R���g���[���[�p�^�C�g���֖߂�{�^�� UI


    // �Z���t�Q
    private string[] dialogueLines =
    {
        "�Z���k���e�B�E�X�͗͐s�����E�E�E�B",
        "���g���C���܂����H\n"
    };

    private int currentLineIndex = 0; // ���ݕ\�����Ă���Z���t�̃C���f�b�N�X
    private bool isTyping = false; // ���ݕ�����\�����Ă��邩�ǂ����̃t���O
    private bool isNext = false; // �e�L�X�g���Ƃ̕\���̏I���t���O

    /// <summary>
    /// �ŏ��̃Z���t��\���J�n����B
    /// </summary>
    void Start()
    {
        // �w�iUI��\��������
        backGroundUI.GetComponent<Image>().enabled = true;
        // �u�͂��vUI��\�������Ȃ�
        yesUI.enabled = false;
        // �u�������vUI��\�������Ȃ�
        noUI.enabled = false;
        // �R���g���[���[�p���g���C�{�^�� UI��\�������Ȃ�
        retryUI.enabled = false;
        // �R���g���[���[�p�^�C�g���֖߂�{�^�� UI��\�������Ȃ�
        titleUI.enabled = false;

        dialogueText.color = normalTextColor; // �������ŕ\��
        StartCoroutine(TypeText(dialogueLines[currentLineIndex])); // �ŏ��̃Z���t��\���J�n

        // �e�L�X�g�\�����I�����Ă��Ȃ�
        isNext = false;
    }

    private void Update()
    {
        // ���݂̓��̓f�o�C�X�𔻒�itrue: �R���g���[���[, false: �L�[�{�[�h�j
        bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();

        // �I�����̕\�����i�u���g���C���܂����H�v�̕������\�����ꂽ�ꍇ�j
        if (dialogueText.text == "���g���C���܂����H\n")
        {
            // �u�͂��v�u�������v��UI��L�����i�ǂ̃f�o�C�X�ł����ʁj
            yesUI.enabled = true;
            noUI.enabled = true;

            // �R���g���[���[�g�p���̂݁A��p��UI��L����
            retryUI.enabled = isUsingGamepad;
            titleUI.enabled = isUsingGamepad;
        }
    }

    /// <summary>
    /// ���[�U�[�̓��͂ɍ��킹���̃Z���t��\������B
    /// </summary>
    private void OnClick(InputValue value)
    {
        // ���݂̓��̓f�o�C�X�𔻒�itrue: �R���g���[���[, false: �L�[�{�[�h�j
        bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();

        // ���N���b�N/A�{�^���������ꂽ���A�������͒��łȂ��ꍇ�Ɏ��̃Z���t��\��
        if (value.isPressed && !isTyping)
        {
            NextLine();
        }

        // �I�����̕\�����i�u���g���C���܂����H�v�̕������\�����ꂽ�ꍇ�j
        if (dialogueText.text == "���g���C���܂����H\n" && isUsingGamepad)
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    /// <summary>
    /// ���̃Z���t��\������B
    /// </summary>
    private void NextLine()
    {
        currentLineIndex++; // ���̃Z���t�ɐi��
        if (currentLineIndex < dialogueLines.Length) // �e�L�X�g���c���Ă���ꍇ
        {
            StartCoroutine(TypeText(dialogueLines[currentLineIndex])); // ���̃e�L�X�g��\��
        }
    }

    /// <summary>
    /// �w�肳�ꂽ�������1�������\������B
    /// </summary>
    /// <param name="line">�\������e�L�X�g</param>
    private IEnumerator TypeText(string line)
    {
        isTyping = true; // �����\����
        isNext = false; // ���̃Z���t�J�n���� false �ɂ���
        dialogueText.text = ""; // �V�����e�L�X�g��\�����邽�߂Ɉ�x��ɂ���

        foreach (char letter in line) // �������1����������
        {
            dialogueText.text += letter; // 1�������\��
            yield return new WaitForSeconds(typingSpeed); // �\���Ԋu��ݒ�
        }

        isTyping = false; // �����̕\�������������̂ŁA���͒��t���O������

        // ���g���C�\������isNext��false�ɂ���
        if (line != "���g���C���܂����H\n")
        {
            isNext = true; // �ʏ�̃Z���t�Ȃ玟�ɐi�߂�
        }
    }

    /// <summary>
    /// ���݂̃Z���t�̕\�����������A���̃Z���t�֐i�ނ��Ƃ��\���ǂ����𔻒肷��
    /// </summary>
    /// <returns>���̃Z���t�֐i�߂�ꍇ�� true�A�i�߂Ȃ��ꍇ�� false�B</returns>
    internal bool IsNextPossible()
    {
        return isNext;
    }

    /// <summary>
    /// �Q�[���V�[���ɑJ�ڂ���B
    /// </summary>
    public void Retry()
    {
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// �^�C�g����ʂ֖߂�B
    /// </summary>
    public void BackTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    /// <summary>
    /// ���[�U�[���u�߂�v������s�����ۂɁA�^�C�g����ʂ֑J�ڂ���B
    /// B�{�^���̓���
    /// </summary>
    /// <param name="value">�߂鑀��̓��͒l�B</param>
    private void OnBack(InputValue value)
    {
        SceneManager.LoadScene("TitleScene");
    }

}

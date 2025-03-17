using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �X�g�[���[���������̃e�L�X�g���Ǘ�����N���X
/// </summary>
public class IntroductionManager : MonoBehaviour
{
    [SerializeField] private Text dialogueText; // �e�L�X�g��\������UI�R���|�[�l���g
    [SerializeField] private float typingSpeed = 0.1f; // ������\������Ԋu�i�b�j
    [SerializeField] private Color normalTextColor = Color.white; // �ʏ�̃e�L�X�g�F�i���j
    [SerializeField] private Color finalTextColor = Color.red; // �ԕ����̃e�L�X�g�F�i�ԁj

    // UI�I�u�W�F�N�g�̎Q�Ɓi�؂�ւ��邽�߂�UI�j
    [SerializeField] private GameObject scene1; // scene1
    [SerializeField] private GameObject scene2; // scene2
    [SerializeField] private GameObject scene3; // scene3
    [SerializeField] private GameObject scene4; // scene4

    // �ʏ�t�F�[�Y�̃Z���t�Q
    private string[] dialogueLines =
    {
        "�����X�͌��{�����B",
        "�K�����̎גm�\�s�ȉ��������˂΂Ȃ�ʂƌ��ӂ����B",
        "���̐��`���΂��苭���j�͖���ɂ��Z������������ɓ���",
        "�����ĕߔ����ꂽ�B",
        "���̑O�Ɉ����o���ꂽ�����X�ɉ��͉����������ł��������₤���B",
        "�������̖ⓚ�̌�A�����X�͉��ɂ���q�����Ă����B",
        "�[��",
        "�|�n�̗F�A�Z���k���e�B�E�X�͉���ɏ����ꂽ�B",
        "���̖ʑO�ŉ����F�Ɖ����F�͓�N�Ԃ�ɑ��������B",
        "�����X�́A�F�Ɉ�؂̎����������B",
        "�u�F��A���͂��O��l���ɂ����B�v",
        "�u���̎O���̊ԁA���͖��̌������ɍs���B�v",
        "�u�O���ڂ̓��v�܂łɖ߂��Ă��Ȃ���΂��O���i�ߎE���Ă悢�Ƃ������̂��B�v",
        "�u���_�A���͕K�����v�܂łɋA���Ă���B�v",
        "�u����M���đ҂��Ă��Ă��炦�ʂ��B�v",
        "�Z���k���e�B�E�X�͖����������A�����X���Ђ��ƕ������߂��B"
    };

    // �ԕ����t�F�[�Y�̃Z���t�Q
    private string[] finalDialogueLines =
    {
        "�u���肦�ʁB�v",
        "�u���ꂪ��N�Ԃ�̗F�ɑ΂���d�ł����B�v",
        "�u�Ȃ��������O�̐g����Ɏ��Ȃ˂΂Ȃ�ʁB�v",
        "�u�����炷��΂��O�������ς��ʂ��B�v",
        "�u���͂��O��M���Ȃ��B�v",
        "�u���O�𖅂ɉ�킹�ĂȂ���̂��B�v"
    };

    // �Ō�̔������t�F�[�Y�i�󋵂��i�s����Z���t�Q�j
    private string[] extraDialogueLines =
    {
        "�����X�͑����̂񂾁B",
        "���̖���̗F���A���̎גm�\�s�ȉ����]�����낵���������B",
        "�����X�͓����o�����B",
        "�Z���k���e�B�E�X�������X��ǂ�����������o�čs�����B",
        "���͎������ᒠ�̊O�ɒu����Ă���󋵂ɂ��΂炭��R�Ƃ��Ă������A",
        "�ӂƉ�ɕԂ�A�u�l�����������v�Ƌ��ѕ������W�����B",
        "�u�K���l����߂炦��B���s����Ζ��͂Ȃ��Ǝv���B�v"
    };

    private int currentLineIndex = 0; // ���ݕ\�����Ă���Z���t�̃C���f�b�N�X
    private bool isTyping = false; // ���ݕ�����\�����Ă��邩�ǂ����̃t���O
    private bool isFinalPhase = false; // �ԕ����t�F�[�Y���ǂ����̃t���O
    private bool isExtraPhase = false; // �������t�F�[�Y�ɖ߂��Ԃ��ǂ����̃t���O

    /// <summary>
    /// �ŏ��̃Z���t��\���J�n����B
    /// </summary>
    void Start()
    {
        //scene1�̂ݕ\��������
        scene1.GetComponent<Image>().enabled = true;
        scene2.GetComponent<Image>().enabled = false;
        scene3.GetComponent<Image>().enabled = false;
        scene4.GetComponent<Image>().enabled = false;

        dialogueText.color = normalTextColor; // �ŏ��͔�����
        StartCoroutine(TypeText(dialogueLines[currentLineIndex])); // �ŏ��̃Z���t��\���J�n
    }

    /// <summary>
    /// ���[�U�[���N���b�N�����ۂɎ��̃Z���t��\������B
    /// </summary>
    void Update()
    {
        // �}�E�X�{�^���������ꂽ���A�������͒��łȂ��ꍇ�Ɏ��̃Z���t��\��
        if (Input.GetMouseButtonDown(0) && !isTyping)
        {
            NextLine();
        }
    }

    /// <summary>
    /// ���̃Z���t��\������B
    /// ���݂̃t�F�[�Y�i�ʏ�t�F�[�Y�A�ԕ����t�F�[�Y�A�������t�F�[�Y�j�ɂ���ēK�؂ȃZ���t��I������B
    /// </summary>
    private void NextLine()
    {
        if (!isFinalPhase && !isExtraPhase) // �ʏ�̔������t�F�[�Y
        {
            currentLineIndex++; // ���̃Z���t�ɐi��
            if (currentLineIndex < dialogueLines.Length) // �ʏ�t�F�[�Y�̃e�L�X�g���c���Ă���ꍇ
            {
                StartCoroutine(TypeText(dialogueLines[currentLineIndex])); // ���̃e�L�X�g��\��
            }
            else // �ʏ�t�F�[�Y���I��������
            {
                StartCoroutine(DeleteTextAndStartFinal()); // �e�L�X�g���폜���A�ԕ����t�F�[�Y���J�n
            }
        }
        else if (isFinalPhase) // �ԕ����t�F�[�Y
        {
            currentLineIndex++; // ���̃Z���t�ɐi��
            if (currentLineIndex < finalDialogueLines.Length) // �ԕ����t�F�[�Y�̃e�L�X�g���c���Ă���ꍇ
            {
                StartCoroutine(TypeText(finalDialogueLines[currentLineIndex])); // ���̐ԕ����e�L�X�g��\��
            }
            else // �ԕ����t�F�[�Y���I��������
            {
                StartCoroutine(DeleteTextAndStartExtra()); // �e�L�X�g���폜���A�������t�F�[�Y�ɖ߂�
            }
        }
        else if (isExtraPhase) // �������t�F�[�Y�i�ŏI�I�ȃZ���t�j
        {
            currentLineIndex++; // ���̃Z���t�ɐi��
            if (currentLineIndex < extraDialogueLines.Length) // �Ō�̃Z���t���c���Ă���ꍇ
            {
                StartCoroutine(TypeText(extraDialogueLines[currentLineIndex])); // �Ō�̃Z���t��\��
            }
            else // �Ō�̃Z���t���I��������
            {
                dialogueText.text = ""; // �S�ďI��������e�L�X�g���N���A
            }
        }
    }

    /// <summary>
    /// �w�肳�ꂽ�������1�������\������B
    /// </summary>
    /// <param name="line">�\������e�L�X�g</param>
    private IEnumerator TypeText(string line)
    {
        isTyping = true; // �����\����
        dialogueText.text = ""; // �V�����e�L�X�g��\�����邽�߂Ɉ�x��ɂ���

        foreach (char letter in line) // �������1����������
        {
            dialogueText.text += letter; // 1�������\��
            yield return new WaitForSeconds(typingSpeed); // �\���Ԋu��ݒ�
        }

        isTyping = false; // �����̕\�������������̂ŁA���͒��t���O������

        // �Z���t�ɉ�����UI��؂�ւ���
        if (line == "�|�n�̗F�A�Z���k���e�B�E�X�͉���ɏ����ꂽ�B") // ���̃Z���t�I�����UI1��\��
        {
            ShowUI(scene2);
        }
        else if (line == "�u���肦�ʁB�v") // ���̃Z���t�I�����UI2��\��
        {
            ShowUI(scene3);
        }
        else if (line == "�Z���k���e�B�E�X�������X��ǂ�����������o�čs�����B") // ���̃Z���t�I�����UI3��\��
        {
            ShowUI(scene4);
        }
    }

    /// <summary>
    /// ��ʂ̃e�L�X�g���폜���A�ԕ����t�F�[�Y���J�n����B
    /// </summary>
    private IEnumerator DeleteTextAndStartFinal()
    {
        isTyping = true; // �e�L�X�g�폜���͕������͒��Ƃ��ăt���O�𗧂Ă�
        yield return new WaitForSeconds(0.5f); // �����ҋ@���Ă���폜�J�n

        // 1�������e�L�X�g���폜
        while (dialogueText.text.Length > 0)
        {
            dialogueText.text = dialogueText.text.Substring(0, dialogueText.text.Length - 1); // 1�������폜
            yield return new WaitForSeconds(typingSpeed); // �폜�Ԋu
        }

        // �ԕ����t�F�[�Y���J�n
        isFinalPhase = true;
        currentLineIndex = 0; // �ԕ����t�F�[�Y�̍ŏ��̃Z���t�ɖ߂�
        dialogueText.color = finalTextColor; // �e�L�X�g�F��ԂɕύX
        StartCoroutine(TypeText(finalDialogueLines[currentLineIndex])); // �ԕ����̃Z���t��\���J�n
    }

    /// <summary>
    /// ��ʂ̃e�L�X�g���폜���A�������t�F�[�Y���J�n����B
    /// </summary>
    private IEnumerator DeleteTextAndStartExtra()
    {
        isTyping = true; // �e�L�X�g�폜���͕������͒��Ƃ��ăt���O�𗧂Ă�
        yield return new WaitForSeconds(0.5f); // �����ҋ@���Ă���폜�J�n

        dialogueText.text = ""; // ��C�Ƀe�L�X�g���폜

        // �������t�F�[�Y�ɖ߂�
        isFinalPhase = false;
        isExtraPhase = true;
        currentLineIndex = 0; // �������t�F�[�Y�̍ŏ��̃Z���t�ɖ߂�
        dialogueText.color = normalTextColor; // �e�L�X�g�F�����ɖ߂��i���j
        StartCoroutine(TypeText(extraDialogueLines[currentLineIndex])); // �Ō�̔������̃Z���t��\���J�n
    }

    /// <summary>
    /// �����UI�R���|�[�l���g��L���ɂ��A����UI�R���|�[�l���g�𖳌��ɂ���
    /// </summary>
    private void ShowUI(GameObject uiToEnable)
    {
        // ���ׂĂ�UI�R���|�[�l���g�𖳌��ɂ���
        scene1.GetComponent<Image>().enabled = false;
        scene2.GetComponent<Image>().enabled = false;
        scene3.GetComponent<Image>().enabled = false;
        scene4.GetComponent<Image>().enabled = false;

        // �w�肳�ꂽUI�R���|�[�l���g������L���ɂ���
        uiToEnable.GetComponent<Image>().enabled = true;
    }
}
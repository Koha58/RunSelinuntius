using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// �X�g�[���[�N���A�����̃e�L�X�g���Ǘ�����N���X
/// </summary>
public class ClearManager : MonoBehaviour
{
    private const float DefaultFadeDuration = 1.5f; // �t�F�[�h�C��/�A�E�g�̃f�t�H���g���ԁi�b�j
    private const float MaxBGMVolume = 0.5f; // BGM�̍ő剹��
    private const float MinBGMVolume = 0f; // BGM�̍ŏ�����

    [SerializeField] private Text dialogueText; // �e�L�X�g��\������UI�R���|�[�l���g
    [SerializeField] private float typingSpeed = 0.1f; // ������\������Ԋu�i�b�j
    [SerializeField] private Color normalTextColor = Color.white; // �ʏ�̃e�L�X�g�F�i���j

    // UI�I�u�W�F�N�g�̎Q�Ɓi�؂�ւ��邽�߂�UI�j
    [SerializeField] private GameObject scene1; // scene1
    [SerializeField] private GameObject scene2; // scene2

    [SerializeField] private AudioSource bgmSource; // BGM�p��AudioSource
    [SerializeField] private AudioClip scene1BGM; // scene1 ���� BGM
    [SerializeField] private AudioClip scene2BGM; // scene2 ���� BGM

    // �Z���t�Q
    private string[] dialogueLines =
    {
        "�Z���k���e�B�E�X�̓����X�ڊ|���Ďv���؂�R��グ���B",
        "�����ł悤�₭�����X�͗��������B",
        "�������Ƃ��Ă��܂������̏d�傳���B",
        "�|�ꂽ�����X�͌����J���B",
        "�u�ς܂Ȃ������B�v",
        "�u���͂��O�ɉ��Ă��Ƃ����Ă��܂����̂��B�v",
        "�Z���k���e�B�E�X�̓����X�����邱�Ƃ��Ȃ����t��Ԃ��B",
        "�u�{���Ɏ��ɐ\����Ȃ��v���Ă���̂Ȃ牤��ɖ߂�B�v",
        "�u����ǂ��Ă����������������܂ŗ��Ă���B�v",
        "�u���O�͎������N�������s���̑S�Ă̐ӔC�����̂��B�v",
        "�����X�͏����������ƁA�̂��N�����������̕��֕����čs�����B",
        "��"
    };

    private int currentLineIndex = 0; // ���ݕ\�����Ă���Z���t�̃C���f�b�N�X
    private bool isTyping = false; // ���ݕ�����\�����Ă��邩�ǂ����̃t���O
    private bool isNext = false; // �e�L�X�g���Ƃ̕\���̏I���t���O

    /// <summary>
    /// �ŏ��̃Z���t��\���J�n����B
    /// </summary>
    void Start()
    {
        // scene1�̂ݕ\��������
        scene1.GetComponent<Image>().enabled = true;
        scene2.GetComponent<Image>().enabled = false;

        dialogueText.color = normalTextColor; // �������ŕ\��
        StartCoroutine(TypeText(dialogueLines[currentLineIndex])); // �ŏ��̃Z���t��\���J�n

        // �e�L�X�g�\�����I�����Ă��Ȃ�
        isNext = false;

        // �ŏ���BGM���Đ�
        bgmSource.clip = scene1BGM; // BGM���Z�b�g
        bgmSource.Play(); // �Đ�
    }

    /// <summary>
    /// ���[�U�[�̓��͂ɍ��킹���̃Z���t��\������B
    /// </summary>
    private void OnClick(InputValue value)
    {
        // ���N���b�N/A�{�^���������ꂽ���A�������͒��łȂ��ꍇ�Ɏ��̃Z���t��\��
        if (value.isPressed && !isTyping)
        {
            NextLine();
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
        else // �Ō�̃Z���t���I��������A�V�[����؂�ւ���
        {
            SceneManager.LoadScene("TitleScene");
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
        isNext = true; // �ꕶ���I��������Ƃ�����

        // �Z���t�ɉ�����UI��؂�ւ���
        if (line == "�������Ƃ��Ă��܂������̏d�傳���B") // ���̃Z���t�I�����UI1��\��
        {
            ShowUI(scene2);
            ChangeBGM(scene2BGM);
        }
    }


    /// <summary>
    /// �����UI�R���|�[�l���g��L���ɂ��A����UI�R���|�[�l���g�𖳌��ɂ���
    /// </summary>
    private void ShowUI(GameObject uiToEnable)
    {
        // ���ׂĂ�UI�R���|�[�l���g�𖳌��ɂ���
        scene1.GetComponent<Image>().enabled = false;
        scene2.GetComponent<Image>().enabled = false;

        // �w�肳�ꂽUI�R���|�[�l���g������L���ɂ���
        uiToEnable.GetComponent<Image>().enabled = true;
    }

    /// <summary>
    /// BGM ���t�F�[�h�A�E�g���Ă���ύX���A�t�F�[�h�C������
    /// </summary>
    /// <param name="newClip">�ύX����BGM</param>
    /// <param name="fadeDuration">�t�F�[�h���ԁi�b�j</param>
    private void ChangeBGM(AudioClip newClip, float fadeDuration = DefaultFadeDuration)
    {
        StartCoroutine(FadeOutAndChangeBGM(newClip, fadeDuration));
    }

    /// <summary>
    /// BGM ���t�F�[�h�A�E�g���Ă���A�t�F�[�h�C�����Ȃ���ύX����R���[�`��
    /// </summary>
    /// <param name="newClip">�ύX����BGM</param>
    /// <param name="fadeDuration">�t�F�[�h���ԁi�b�j</param>
    private IEnumerator FadeOutAndChangeBGM(AudioClip newClip, float fadeDuration)
    {
        // ���݂�BGM���t�F�[�h�A�E�g
        yield return StartCoroutine(FadeOutBGM(fadeDuration));

        // �V����BGM���Z�b�g���ăt�F�[�h�C��
        yield return StartCoroutine(FadeInBGM(newClip, fadeDuration));
    }

    /// <summary>
    /// BGM ���t�F�[�h�A�E�g����
    /// </summary>
    /// <param name="fadeDuration">�t�F�[�h�A�E�g�ɂ����鎞�ԁi�b�j</param>
    private IEnumerator FadeOutBGM(float fadeDuration)
    {
        float startVolume = bgmSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, MinBGMVolume, elapsedTime / fadeDuration); // �萔���g�p
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bgmSource.volume = MinBGMVolume;
        bgmSource.Stop(); // �������S�ɒ�~
    }

    /// <summary>
    /// BGM ���t�F�[�h�C�����Ȃ���Đ�����
    /// </summary>
    /// <param name="newClip">�ύX����BGM</param>
    /// <param name="fadeDuration">�t�F�[�h�C���ɂ����鎞�ԁi�b�j</param>
    private IEnumerator FadeInBGM(AudioClip newClip, float fadeDuration)
    {
        bgmSource.clip = newClip;
        bgmSource.Play();

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            bgmSource.volume = Mathf.Lerp(MinBGMVolume, MaxBGMVolume, elapsedTime / fadeDuration); // �萔���g�p
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bgmSource.volume = MaxBGMVolume; // �ő剹��
    }

    /// <summary>
    /// ���݂̃Z���t�̕\�����������A���̃Z���t�֐i�ނ��Ƃ��\���ǂ����𔻒肷��
    /// </summary>
    /// <returns>���̃Z���t�֐i�߂�ꍇ�� true�A�i�߂Ȃ��ꍇ�� false�B</returns>
    internal bool IsNextPossible()
    {
        return isNext;
    }
}

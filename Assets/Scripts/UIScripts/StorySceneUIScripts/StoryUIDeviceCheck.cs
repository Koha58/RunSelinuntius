using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// �X�g�[���[�V�[���̃f�o�C�X���Ƃ�UI�؂�ւ��N���X
/// </summary>
public class StoryUIDeviceCheck : MonoBehaviour
{
    [SerializeField] private Image keyboardNextUI;  // �L�[�{�[�h�p���̕��ֈڂ� UI
    [SerializeField] private Image controllerNextUI;  // �R���g���[���[�p���̕��ֈڂ� UI
    [SerializeField] private Image controllerSkipUI; // �R���g���[���[�p�X�L�b�v UI
    [SerializeField] private Image mouseUI; // �}�E�X�p UI
    [SerializeField] private Color blinkColor = Color.gray; // �_�Ŏ��̐F
    [SerializeField] private float blinkInterval = 1.0f; // �_�ŊԊu�i�b�j
    [SerializeField] private IntroductionManager introductionManager; // IntroductionManager �̎Q��
    [SerializeField] private ClearManager clearManager;// ClearManager �̎Q��
    [SerializeField] private GameOverManager gameOverManager;// GameOverManager �̎Q��

    private bool isBlinking = false;   // ���ݓ_�Œ����ǂ���

    private bool isNextSentence = false;   // ���̃e�L�X�g�Ɉڍs�\��

    private void Awake()
    {
        // IntroductionScene�̏ꍇ
        if(introductionManager != null)
        {
            // �����I�u�W�F�N�g�ɃA�^�b�`���ꂽ IntroductionManager ���擾
            introductionManager = GetComponent<IntroductionManager>();
        }
        // GameClearScene�̏ꍇ
        else if (clearManager != null)
        {
            // �����I�u�W�F�N�g�ɃA�^�b�`���ꂽ ClearManager ���擾
            clearManager = GetComponent<ClearManager>();
        }
        // GameOverScene�̏ꍇ
        else if (gameOverManager != null)
        {
            // �����I�u�W�F�N�g�ɃA�^�b�`���ꂽ ClearManager ���擾
            gameOverManager = GetComponent<GameOverManager>();
        }

    }

    void Update()
    {
        if (InputDeviceManager.Instance == null) return;

        // ���݂̓��̓f�o�C�X���擾�itrue: �R���g���[���[, false: �L�[�{�[�h�j
        bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();

        // ���̃e�L�X�g�Ɉڍs�\�����擾
        SceneCheck();  // isNextSentence ���X�V

        // UI �̕\�� / ��\��
        mouseUI.enabled = isNextSentence && !isUsingGamepad;
        keyboardNextUI.enabled = isNextSentence && !isUsingGamepad;
        controllerNextUI.enabled = isNextSentence && isUsingGamepad;

        // IntroductionScene �܂��� GameClearScene �̏ꍇ
        if (introductionManager != null || clearManager != null)
        {
            controllerSkipUI.enabled = isUsingGamepad;
        }
        
        // �_�ŏ������J�n�E��~
        if (isNextSentence && !isBlinking)
        {
            StartCoroutine(BlinkUI());
        }
        else if (!isNextSentence && isBlinking)
        {
            StopCoroutine(BlinkUI());
            ResetUIColor();
        }
    }

    /// <summary>
    /// UI ��_�ł�����R���[�`��
    /// </summary>
    private IEnumerator BlinkUI()
    {
        isBlinking = true;
        while (true)
        {
            // ���݂� UI ���擾
            bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();
            Image activeUI = isUsingGamepad ? controllerNextUI : keyboardNextUI;
            Image uiImage = activeUI.GetComponent<Image>();

            if (uiImage != null)
            {
                // �F��؂�ւ���
                uiImage.color = (uiImage.color == Color.white) ? blinkColor : Color.white;
            }

            yield return new WaitForSeconds(blinkInterval);
        }
    }

    /// <summary>
    /// UI �̐F�����ɖ߂�
    /// </summary>
    private void ResetUIColor()
    {
        keyboardNextUI.GetComponent<Image>().color = Color.white;
        controllerNextUI.GetComponent<Image>().color = Color.white;
        isBlinking = false;
    }

    /// <summary>
    /// �X�L�b�v�{�^���p
    /// </summary>
    public void Skip()
    {
        // �X�L�b�v���I���iintroductionManager �܂��� clearManager �̂ǂ��炪���݂��邩�ŕ���j
        if (introductionManager != null)
        {
            SceneManager.LoadScene("GameScene");
        }
        else if(clearManager != null)
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

    /// <summary>
    /// ���[�U�[���X�L�b�v������s�����ۂɁA�Ή�����V�[���֑J�ڂ���B
    /// </summary>
    /// <param name="value">�X�L�b�v����̓��͒l�B</param>
    private void OnSkip(InputValue value)
    {
        // �X�L�b�v���I���iintroductionManager �܂��� clearManager �̂ǂ��炪���݂��邩�ŕ���j
        if (introductionManager != null)
        {
            SceneManager.LoadScene("GameScene");
        }
        else if (clearManager != null)
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

    /// <summary>
    /// ���̃e�L�X�g�Ɉڍs�\�����擾
    /// </summary>
    private void SceneCheck()
    {
        // ���̃e�L�X�g�Ɉڍs�\�����擾�iintroductionManager , clearManager , gameOverManager �̂����ꂩ�����݂��邩�ŕ���j
        if (introductionManager != null)
        {
            isNextSentence = introductionManager.IsNextPossible();
        }
        else if (clearManager != null)
        {
            isNextSentence = clearManager.IsNextPossible();
        }
        else if (gameOverManager != null)
        {
            isNextSentence = gameOverManager.IsNextPossible();
        }
        else
        {
            isNextSentence = false;  // ����������݂��Ȃ��ꍇ�� false
        }
    }
}

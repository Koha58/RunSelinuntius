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
    [SerializeField] private IntroductionManager introductionManager;

    private bool isBlinking = false;   // ���ݓ_�Œ����ǂ���

    private void Awake()
    {
        // �����I�u�W�F�N�g�ɃA�^�b�`���ꂽ PlayerMove ���擾
        introductionManager = GetComponent<IntroductionManager>();
    }

    void Update()
    {
        if (InputDeviceManager.Instance == null) return;

        // ���݂̓��̓f�o�C�X���擾�itrue: �R���g���[���[, false: �L�[�{�[�h�j
        bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();

        // �v���C���[�� FinalAttack �\�����擾
        bool isNextSentence = introductionManager.IsNextPossible();

        // UI �̕\�� / ��\��
        mouseUI.enabled = isNextSentence && !isUsingGamepad;
        keyboardNextUI.enabled = isNextSentence && !isUsingGamepad;
        controllerNextUI.enabled = isNextSentence && isUsingGamepad;
        controllerSkipUI.enabled = isUsingGamepad;

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
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// ���[�U�[���X�L�b�v������s�����ۂɁA�Q�[���V�[���֑J�ڂ���B
    /// </summary>
    /// <param name="value">�X�L�b�v����̓��͒l�B</param>
    private void OnSkip(InputValue value)
    {
        SceneManager.LoadScene("GameScene");
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Start��UI��_�ł�����N���X
/// ���̓f�o�C�X�i�L�[�{�[�h/�}�E�X or �R���g���[���[�j�ɉ����ēK�؂�UI��\�����A
/// �t�F�[�h�C���E�t�F�[�h�A�E�g���J��Ԃ��B
/// </summary>
public class UIFader : MonoBehaviour
{
    // �t�F�[�h�Ώۂ�UI�i�}�E�X/�L�[�{�[�h�p�j
    [SerializeField] private GameObject keyboardMouseUI;

    // �t�F�[�h�Ώۂ�UI�i�R���g���[���[�p�j
    [SerializeField] private GameObject controllerUI;

    // �t�F�[�h�C���E�A�E�g�̎����i�b�P�ʁj
    [SerializeField] private float fadeDuration = DefaultFadeDuration;

    private bool isFadingIn = true; // ���݂̃t�F�[�h�����itrue: �t�F�[�h�C��, false: �t�F�[�h�A�E�g�j
    private float currentAlpha = MaxAlpha; // ���݂̃A���t�@�l
    private CanvasGroup activeCanvasGroup; // ���݂̃A�N�e�B�u��CanvasGroup

    // �萔�i�}�W�b�N�i���o�[�̍폜�j
    private static readonly float DefaultFadeDuration = 1f; // �f�t�H���g�̃t�F�[�h���ԁi�b�j
    private static readonly float MaxAlpha = 1f; // �ő�A���t�@�l�i���S�ɕ\���j
    private static readonly float MinAlpha = 0f; // �ŏ��A���t�@�l�i���S�ɔ�\���j

    private void Start()
    {
        // �����ݒ�F�L�[�{�[�h/�}�E�X�pUI���A�N�e�B�u��
        UpdateDeviceUI();

        // �f�o�C�X�ύX�C�x���g���Ď�
        InputSystem.onDeviceChange += OnDeviceChange;

        // �t�F�[�h�����J�n
        StartCoroutine(FadeLoop());
    }

    private void OnDestroy()
    {
        // �f�o�C�X�ύX�C�x���g�������i���������[�N�h�~�j
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    /// <summary>
    /// ���̓f�o�C�X���ύX���ꂽ�Ƃ��ɌĂяo�����B
    /// �ǉ��܂��͍폜�����m���ꂽ�ꍇ�AUI���X�V����B
    /// </summary>
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
        {
            UpdateDeviceUI();
        }
    }

    /// <summary>
    /// ���݂̓��̓f�o�C�X�ɉ����ĕ\������UI��؂�ւ���B
    /// </summary>
    private void UpdateDeviceUI()
    {
        // ���̓f�o�C�X�̗D�揇�ʂ��m�F���A�K�؂�UI��\��
        if (Gamepad.current != null) // �R���g���[���[���ڑ�����Ă���ꍇ
        {
            SetActiveUI(controllerUI);
        }
        else // �}�E�X�܂��̓L�[�{�[�h���ڑ�����Ă���ꍇ
        {
            SetActiveUI(keyboardMouseUI);
        }
    }

    /// <summary>
    /// �w�肳�ꂽUI���A�N�e�B�u�ɂ��A����ȊO���A�N�e�B�u�ɂ���B
    /// </summary>
    /// <param name="uiObject">�A�N�e�B�u�ɂ���UI��GameObject</param>
    private void SetActiveUI(GameObject uiObject)
    {
        // ���ׂĂ�UI���A�N�e�B�u��
        keyboardMouseUI.SetActive(false);
        controllerUI.SetActive(false);

        // �w�肳�ꂽUI���A�N�e�B�u��
        uiObject.SetActive(true);

        // CanvasGroup���擾�i�t�F�[�h�����p�j
        activeCanvasGroup = uiObject.GetComponent<CanvasGroup>();
        if (activeCanvasGroup == null)
        {
            activeCanvasGroup = uiObject.AddComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// �t�F�[�h�C���E�t�F�[�h�A�E�g���J��Ԃ��R���[�`���B
    /// </summary>
    private System.Collections.IEnumerator FadeLoop()
    {
        while (true)
        {
            if (activeCanvasGroup != null)
            {
                float elapsedTime = 0f;
                float startAlpha = isFadingIn ? MinAlpha : MaxAlpha; // �J�n���̃A���t�@�l
                float endAlpha = isFadingIn ? MaxAlpha : MinAlpha; // �I�����̃A���t�@�l

                // �w�肳�ꂽ���ԂŃt�F�[�h�������s��
                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
                    activeCanvasGroup.alpha = currentAlpha;
                    yield return null;
                }

                // �t�F�[�h�C��/�t�F�[�h�A�E�g�̐؂�ւ�
                isFadingIn = !isFadingIn;
            }

            yield return null;
        }
    }
}
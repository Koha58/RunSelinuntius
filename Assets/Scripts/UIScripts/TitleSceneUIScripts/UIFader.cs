using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Start��UI��_�ł�����N���X
/// </summary>
public class UIFader : MonoBehaviour
{
    // �t�F�[�h�Ώۂ�Image�R���|�[�l���g�i�}�E�X/�L�[�{�[�h�p�j
    [SerializeField] private GameObject keyboardMouseUI;

    // �t�F�[�h�Ώۂ�Image�R���|�[�l���g�i�R���g���[���[�p�j
    [SerializeField] private GameObject controllerUI;

    // �t�F�[�h�C���E�A�E�g�̎����i�b�P�ʁj
    [SerializeField] private float fadeDuration = 1f;

    private bool isFadingIn = true; // ���݂̃t�F�[�h����
    private float currentAlpha = 1f; // ���݂̃A���t�@�l
    private CanvasGroup activeCanvasGroup; // ���݂̃A�N�e�B�u��CanvasGroup

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
        // �f�o�C�X�ύX�C�x���g������
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    /// <summary>
    /// ���̓f�o�C�X���ύX���ꂽ�Ƃ��ɌĂяo�����
    /// </summary>
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
        {
            UpdateDeviceUI();
        }
    }

    /// <summary>
    /// ���݂̓��̓f�o�C�X�ɉ����ĕ\������UI��؂�ւ���
    /// </summary>
    private void UpdateDeviceUI()
    {
        // ���̓f�o�C�X�̗D�揇�ʂ��m�F
        if (Gamepad.current != null) // �R���g���[���[���ڑ�����Ă���ꍇ
        {
            SetActiveUI(controllerUI);
        }
        else if (Mouse.current != null || Keyboard.current != null) // �}�E�X�܂��̓L�[�{�[�h���ڑ�����Ă���ꍇ
        {
            SetActiveUI(keyboardMouseUI);
        }
    }

    /// <summary>
    /// �w�肳�ꂽUI���A�N�e�B�u�ɂ���
    /// </summary>
    /// <param name="uiObject">�A�N�e�B�u�ɂ���UI��GameObject</param>
    private void SetActiveUI(GameObject uiObject)
    {
        // ���ׂĂ�UI���A�N�e�B�u��
        keyboardMouseUI.SetActive(false);
        controllerUI.SetActive(false);

        // �A�N�e�B�u�ɂ���UI��\��
        uiObject.SetActive(true);

        // CanvasGroup���擾�i�t�F�[�h�����p�j
        activeCanvasGroup = uiObject.GetComponent<CanvasGroup>();
        if (activeCanvasGroup == null)
        {
            activeCanvasGroup = uiObject.AddComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// �t�F�[�h�������J��Ԃ��R���[�`���B
    /// </summary>
    private System.Collections.IEnumerator FadeLoop()
    {
        while (true)
        {
            if (activeCanvasGroup != null)
            {
                float elapsedTime = 0f;
                float startAlpha = isFadingIn ? 0f : 1f;
                float endAlpha = isFadingIn ? 1f : 0f;

                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
                    activeCanvasGroup.alpha = currentAlpha;
                    yield return null;
                }

                isFadingIn = !isFadingIn;
            }

            yield return null;
        }
    }
}

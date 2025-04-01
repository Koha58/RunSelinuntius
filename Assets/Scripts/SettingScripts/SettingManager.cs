using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// �ݒ胁�j���[�̊Ǘ��N���X
/// </summary>
public class SettingManager : MonoBehaviour
{
    [SerializeField] private GameObject SettingMenu; // �ݒ胁�j���[�I�u�W�F�N�g
    [SerializeField] private Image buttonBui;       // B�{�^����UI
    [SerializeField] private Image settingCursorUI; // �J�[�\���摜
    [SerializeField] private Slider BGMSlider;      // BGM�X���C�_�[
    [SerializeField] private Slider SESlider;       // SE�X���C�_�[
    [SerializeField] private AudioMixer audioMixer; // �I�[�f�B�I�~�L�T�[

    private List<Slider> sliders;
    private int currentIndex = 0;
    private bool stickNeutral = true;
    private bool canMove = true;

    // �萔�l
    private const float InputCooldown = 0.1f; // �A�����͂̃N�[���_�E������
    private const float StickThreshold = 0.5f; // �X�e�B�b�N�̓���臒l�i�ړ������𔻒肷�邽�߂̂������l�j
    private const float MinStickNeutralThreshold = 0.1f; // �X�e�B�b�N���j���[�g�����ƌ��Ȃ����臒l
    private const float VolumeAdjustStep = 0.1f; // ���ʕύX�̊�{�X�e�b�v
    private const float VolumeAdjustMultiplierMax = 5f; // �X�e�B�b�N�̓��͋��x�ɉ������ő剹�ʒ����{��
    private const float CursorOffsetX = -400f; // �J�[�\���ʒu��X���I�t�Z�b�g

    // �ݒ胁�j���[�̏�Ԃ��Ǘ�����v���p�e�B
    public bool IsSettingActive { get; private set; } // �ݒ胁�j���[�̏��

    private void Start()
    {
        // ������ԂŐݒ胁�j���[���\���ɂ��AUI�֘A�̕\���𐧌䂷��
        ToggleSettingMenu(false);
        buttonBui.enabled = false;
        settingCursorUI.enabled = false;

        // �X���C�_�[�����X�g�����A�������������s��
        sliders = new List<Slider> { BGMSlider, SESlider };

        // �I�[�f�B�I�~�L�T�[���特�ʂ��擾���ăX���C�_�[�ɔ��f
        InitializeSliders();
        // �����J�[�\���ʒu�̐ݒ�
        UpdateCursorPosition();
    }

    /// <summary>
    /// �I�[�f�B�I�~�L�T�[���猻�݂̉��ʂ��擾���A�X���C�_�[�ɓK�p����
    /// </summary>
    private void InitializeSliders()
    {
        // BGM��SE�̃X���C�_�[�ɂ��ꂼ�ꉹ�ʂ�ݒ�
        InitializeSlider(BGMSlider, AudioParameterName.BGM);
        InitializeSlider(SESlider, AudioParameterName.SE);
    }

    /// <summary>
    /// �w�肳�ꂽ�X���C�_�[�̉��ʂ��I�[�f�B�I�~�L�T�[����擾���ēK�p����
    /// </summary>
    /// <param name="slider">���ʂ�ݒ肷��X���C�_�[</param>
    /// <param name="parameterName">�I�[�f�B�I�~�L�T�[�Őݒ肳��Ă���p�����[�^���i"BGM"�܂���"SE"�j</param>
    private void InitializeSlider(Slider slider, string parameterName)
    {
        // �I�[�f�B�I�~�L�T�[���特�ʂ̌��ݒl���擾
        if (audioMixer.GetFloat(parameterName, out float volume))
        {
            // �擾�������ʂ��X���C�_�[�ɔ��f
            slider.value = volume;
        }
    }

    /// <summary>
    /// �ݒ胁�j���[���J����
    /// </summary>
    /// <param name="isActive">�ݒ胁�j���[��\�����邩�ǂ����̃t���O</param>
    public void ToggleSettingMenu(bool isActive)
    {
        // ���j���[�̕\���E��\���𐧌�
        SettingMenu.SetActive(isActive);
        IsSettingActive = isActive;

        // �Q�[���p�b�h���g�p����Ă���ꍇ�̂݁AUI��B�{�^���ƃJ�[�\����L���ɂ���
        bool isUsingGamepad = Gamepad.current != null;
        buttonBui.enabled = isUsingGamepad && isActive;
        settingCursorUI.enabled = isUsingGamepad && isActive;

        // ���j���[���\������Ă���ꍇ�A�J�[�\���ʒu���X�V
        if (isActive)
        {
            UpdateCursorPosition();
        }
    }

    private void Update()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();

        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            Debug.Log("Gamepad B (Back) button detected!");
        }
        // �ݒ胁�j���[���\������Ă���ꍇ�̂ݏ������s��
        if (!IsSettingActive) return;

        // ���݂̃V�[����GameScene�̏ꍇ�AcanMove��true�ɐݒ�
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            canMove = true;
        }

        // �Q�[���p�b�h���g�p����Ă���ꍇ�A���͂�����
        if (Gamepad.current != null)
        {
            HandleGamepadInput();
        }
    }

    /// <summary>
    /// �Q�[���p�b�h�̓��͂���������
    /// </summary>
    private void HandleGamepadInput()
    {
        // ���X�e�B�b�N�̓��͂��擾
        Vector2 input = Gamepad.current.leftStick.ReadValue();

        // ���͉\�ȏ�ԂŁA�X�e�B�b�N�̏㉺���͂ɉ����ăX���C�_�[�̑I����ύX
        if (canMove && stickNeutral)
        {
            if (input.y > StickThreshold) // ��ɓ��͂��ꂽ�ꍇ�A�O�̃X���C�_�[�Ɉړ�
            {
                ChangeSliderSelection(-1);
            }
            else if (input.y < -StickThreshold) // ���ɓ��͂��ꂽ�ꍇ�A���̃X���C�_�[�Ɉړ�
            {
                ChangeSliderSelection(1);
            }
        }

        // �X�e�B�b�N��y�����͂��j���[�g�����̈�i�������l�����j�ɖ߂����ꍇ
        if (Mathf.Abs(input.y) < MinStickNeutralThreshold)
        {
            stickNeutral = true;
        }

        // ���E���͂ŉ��ʒ���
        if (input.x > StickThreshold)
        {
            AdjustVolume(VolumeAdjustStep); // ���ʂ𑝉�
        }
        else if (input.x < -StickThreshold)
        {
            AdjustVolume(-VolumeAdjustStep); // ���ʂ�����
        }
    }

    /// <summary>
    /// �X���C�_�[�̑I����ύX����
    /// </summary>
    /// <param name="direction">�I������i-1: ��A1: ���j</param>
    private void ChangeSliderSelection(int direction)
    {
        // ���ݑI�𒆂̃X���C�_�[�C���f�b�N�X��ύX
        currentIndex = (currentIndex + direction + sliders.Count) % sliders.Count;
        // �J�[�\���̈ʒu���X�V
        UpdateCursorPosition();
        // �ړ��������I���܂Ŏ��̈ړ����󂯕t���Ȃ�
        StartCoroutine(ResetMove());
        stickNeutral = false;
    }

    /// <summary>
    /// �J�[�\���̈ʒu�����ݑI�𒆂̃X���C�_�[�̈ʒu�ɍ��킹��
    /// </summary>
    private void UpdateCursorPosition()
    {
        // �J�[�\���ʒu�̃I�t�Z�b�g��K�p���A���ݑI������Ă���X���C�_�[�ɃJ�[�\�����ړ�
        Vector3 offset = new Vector3(CursorOffsetX, 0f, 0f);
        settingCursorUI.transform.position = sliders[currentIndex].transform.position + offset;
    }

    /// <summary>
    /// ���ʂ𒲐�����
    /// </summary>
    /// <param name="adjustment">���ʒ����̕ύX�ʁi��: �����A��: �����j</param>
    private void AdjustVolume(float adjustment)
    {
        // ���ݑI������Ă���X���C�_�[���擾
        Slider selectedSlider = sliders[currentIndex];

        // �X�e�B�b�N��x���̓��͋��x���擾
        float inputStrength = Mathf.Abs(Gamepad.current.leftStick.x.ReadValue());

        // ���͋��x�ɉ����ĉ��ʒ������x��ω�������
        float speedMultiplier = Mathf.Lerp(1f, VolumeAdjustMultiplierMax, inputStrength);

        // ���ʂ𒲐����A�X���C�_�[�͈͓̔��Ɏ��߂�
        selectedSlider.value = Mathf.Clamp(selectedSlider.value + adjustment * speedMultiplier, selectedSlider.minValue, selectedSlider.maxValue);

        // ���݂̃X���C�_�[�ɑΉ������I�[�f�B�I�~�L�T�[�̃p�����[�^��ݒ�
        audioMixer.SetFloat(currentIndex == 0 ? AudioParameterName.BGM : AudioParameterName.SE, selectedSlider.value);
    }

    /// <summary>
    /// ���͎�t���N�[���_�E������
    /// </summary>
    private IEnumerator ResetMove()
    {
        // ���͎�t�𖳌���
        canMove = false;
        // ��莞�ԑҋ@
        yield return new WaitForSeconds(InputCooldown);
        // ���͎�t���ēx�L����
        canMove = true;
    }

    /// <summary>
    /// �ݒ胁�j���[�����
    /// </summary>
    public void Close()
    {
        ToggleSettingMenu(false);
    }

    /// <summary>
    /// �o�b�N�{�^���������ꂽ�Ƃ��̏���
    /// </summary>
    /// <param name="value">�o�b�N�{�^���̓��͒l</param>
    public void OnBack(InputValue value)
    {
        Debug.Log("OnBack Called! Scene: " + SceneManager.GetActiveScene().name);

        if (value == null)
        {
            Debug.LogError("OnBack: InputValue is NULL!");
        }
        else
        {
            Debug.Log("OnBack: Value is " + value.isPressed);
        }
    }

    /// <summary>
    /// �ݒ胁�j���[�����
    /// </summary>
    public void Quit()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // �����p�����[�^�����`����N���X
    private static class AudioParameterName
    {
        public const string BGM = "BGM"; // BGM�̃p�����[�^��
        public const string SE = "SE";   // SE�̃p�����[�^��
    }
}

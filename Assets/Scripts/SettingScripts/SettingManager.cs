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
    // �ݒ胁�j���[�֘A�̃I�u�W�F�N�g
    [SerializeField] private GameObject SettingMenu;  // �ݒ胁�j���[�S�̂̃I�u�W�F�N�g
    [SerializeField] private Image buttonBui;         // B�{�^����UI�i�߂�{�^���\���p�j
    [SerializeField] private Image buttonAui;         // A�{�^����UI�i�^�C�g���֖߂�{�^���\���p�j
    [SerializeField] private Image settingCursorUI;   // �ݒ胁�j���[�̃J�[�\���摜
    [SerializeField] private Slider BGMSlider;        // BGM���ʒ����p�X���C�_�[
    [SerializeField] private Slider SESlider;         // SE���ʒ����p�X���C�_�[
    [SerializeField] private AudioMixer audioMixer;   // �I�[�f�B�I�~�L�T�[�i���ʐ���j

    // �X���C�_�[�̊Ǘ�
    private List<Slider> sliders; // �ݒ胁�j���[���̃X���C�_�[�iBGM�ESE�j���Ǘ����郊�X�g
    private int currentIndex = 0; // ���ݑI�𒆂̍��ځi0: BGM, 1: SE, 2: A�{�^���j
    private int totalOptions;     // �I���\�ȍ��ڐ��i�X���C�_�[�̐� + A�{�^��UI�j

    // �X�e�B�b�N���͂̊Ǘ�
    private bool stickNeutral = true; // �X�e�B�b�N�̓��͂��j���[�g�����ȏ�Ԃ��i�������ςȂ��h�~�j
    private bool canMove = true;      // �I���ړ����\���i���̓N�[���_�E���Ǘ��j

    // �J�[�\���ʒu�Ǘ�
    private Vector3 lastCursorPosition; // �Ō�ɐݒ肳�ꂽ�J�[�\���̈ʒu

    // �萔�i���l�ݒ�j
    private const float InputCooldown = 0.1f;           // �A�����͂̃N�[���_�E������
    private const float StickThreshold = 0.5f;          // �X�e�B�b�N�̓���臒l
    private const float MinStickNeutralThreshold = 0.1f;// �X�e�B�b�N���j���[�g�����ƌ��Ȃ����臒l
    private const float VolumeAdjustStep = 0.1f;        // ���ʕύX�̊�{�X�e�b�v
    private const float VolumeAdjustMultiplierMax = 5f; // �X�e�B�b�N���͋��x�ɂ�鉹�ʒ����{��
    private const float CursorOffsetX = -400f;         // �J�[�\���ʒu��X���I�t�Z�b�g
    private const float DefaultVolume = 0f;            // ���ʂ̃f�t�H���g�l

    // �ݒ胁�j���[�̏�Ԃ��Ǘ�����v���p�e�B
    public bool IsSettingActive { get; private set; }  // �ݒ胁�j���[���J���Ă��邩


    private void Start()
    {
        // ������ԂŐݒ胁�j���[���\���ɂ��AUI�֘A�̕\���𐧌䂷��
        ToggleSettingMenu(false);
        buttonBui.enabled = false;
        settingCursorUI.enabled = false;

        // �X���C�_�[�����X�g�����A�������������s��
        sliders = new List<Slider> { BGMSlider, SESlider };

        // A�{�^��UI������ꍇ�A�I������ǉ�
        totalOptions = sliders.Count;
        if (buttonAui != null)
        {
            buttonAui.enabled = false;
            totalOptions++; // A�{�^��UI��I��Ώۂɒǉ�
        }

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
    /// �w�肳�ꂽ�X���C�_�[�̉��ʂ� PlayerPrefs ���烍�[�h���A�I�[�f�B�I�~�L�T�[�ɓK�p����B
    /// </summary>
    /// <param name="slider">�Ώۂ̃X���C�_�[</param>
    /// <param name="parameterName">�I�[�f�B�I�~�L�T�[�ł̃p�����[�^���i"BGM" �܂��� "SE"�j</param>
    private void InitializeSlider(Slider slider, string parameterName)
    {
        // PlayerPrefs ����ۑ����ꂽ���ʂ��擾�i���ݒ�̏ꍇ�� DefaultVolume ���g�p�j
        float defaultValue = PlayerPrefs.GetFloat(parameterName, DefaultVolume);

        // PlayerPrefs ����ۑ�����Ă��鉹�ʐݒ���擾
        if (parameterName == AudioParameterName.BGM)
        {
            defaultValue = PlayerPrefs.GetFloat(AudioParameterName.BGM, DefaultVolume);
        }
        else if (parameterName == AudioParameterName.SE)
        {
            defaultValue = PlayerPrefs.GetFloat(AudioParameterName.SE, DefaultVolume);
        }

        // �X���C�_�[�̒l��ݒ肵�A�I�[�f�B�I�~�L�T�[�ɂ����f
        slider.value = defaultValue;
        audioMixer.SetFloat(parameterName, defaultValue);
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
        // �ݒ胁�j���[���\������Ă���ꍇ�̂ݏ������s��
        if (!IsSettingActive) return;

        // ���݂̃V�[����GameScene�̏ꍇ�AcanMove��true�ɐݒ�
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            canMove = true;

            // �Q�[���p�b�h���ڑ�����Ă���AB(East)�{�^���������ꂽ��
            if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
            {
                // �ݒ胁�j���[�����
                Close();
            }
            // �Q�[���p�b�h���ڑ�����Ă���AA�{�^��UI���\������A����A(South)�{�^���������ꂽ��
            else if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame && buttonAui.enabled == true)
            {
                // �^�C�g���V�[���ɖ߂�
                Quit();
            }
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
        // �I�����̐��ɉ����ăC���f�b�N�X���z������
        currentIndex = (currentIndex + direction + totalOptions) % totalOptions;
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
        Vector3 offset = new Vector3(CursorOffsetX, 0f, 0f);

        if (currentIndex < sliders.Count)
        {
            // �X���C�_�[��I�����Ă���ꍇ
            // �J�[�\���̈ʒu�����݂̃X���C�_�[�̈ʒu + �I�t�Z�b�g�ɐݒ�
            settingCursorUI.transform.position = sliders[currentIndex].transform.position + offset;

            // �L�����Ă����J�[�\����X���W���X�V
            lastCursorPosition = settingCursorUI.transform.position;

            // A�{�^��UI�����݂���ꍇ�͔�\���ɂ���i�X���C�_�[�I�����͕s�v�j
            if (buttonAui != null)
            {
                buttonAui.enabled = false;
            }
        }
        else if (buttonAui != null)
        {
            // A�{�^��UI��I�����Ă���ꍇ
            // A�{�^��UI��L�����i�\���j
            buttonAui.enabled = true;

            // �L�����Ă���X���W���g�p���AA�{�^����Y���W�ɃJ�[�\�����ړ�
            settingCursorUI.transform.position = new Vector3(
                lastCursorPosition.x,  // X���W�͕ێ��i�X���C�_�[�Ƒ�����j
                buttonAui.transform.position.y, // Y���W�̂�A�{�^���̈ʒu�ɍ��킹��
                lastCursorPosition.z   // Z���W�͕ێ�
            );
        }
    }

    /// <summary>
    /// ���ݑI������Ă���X���C�_�[�̉��ʂ𒲐����A�ݒ��ۑ�����B
    /// </summary>
    /// <param name="adjustment">���ʂ̑����l�i��: ����, ��: �����j</param>
    private void AdjustVolume(float adjustment)
    {
        // A�{�^�� UI ���I������Ă���ꍇ�͉��ʒ������s��Ȃ�
        if (currentIndex >= sliders.Count) return;

        // ���ݑI������Ă���X���C�_�[���擾
        Slider selectedSlider = sliders[currentIndex];

        // �X�e�B�b�N�̓��͋��x���擾���A���ʕύX���x�𒲐�
        float inputStrength = Mathf.Abs(Gamepad.current.leftStick.x.ReadValue());
        float speedMultiplier = Mathf.Lerp(1f, VolumeAdjustMultiplierMax, inputStrength);

        // �X���C�_�[�̒l��ύX�i�͈͂𒴂��Ȃ��悤�� Clamp�j
        selectedSlider.value = Mathf.Clamp(selectedSlider.value + adjustment * speedMultiplier, selectedSlider.minValue, selectedSlider.maxValue);

        // �I�[�f�B�I�~�L�T�[�ɓK�p����p�����[�^���𔻒�
        string parameterName = currentIndex == 0 ? AudioParameterName.BGM : AudioParameterName.SE;

        // �I�[�f�B�I�~�L�T�[�ɐV�������ʂ�K�p
        audioMixer.SetFloat(parameterName, selectedSlider.value);

        // �ݒ�� PlayerPrefs �ɕۑ�
        PlayerPrefs.SetFloat(parameterName, selectedSlider.value);
        PlayerPrefs.Save();
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
    /// B�{�^���������ꂽ�Ƃ��̏���
    /// </summary>
    /// <param name="value">B�{�^���̓��͒l</param>
    private void OnBack(InputValue value)
    {
        Close();
    }

    /// <summary>
    /// �^�C�g���V�[���ɖ߂�
    /// </summary>
    public void Quit()
    {
        SceneManager.LoadScene("TitleScene");
    }

    /// <summary>
    /// �����p�����[�^�����`����N���X
    /// </summary>
    private static class AudioParameterName
    {
        public const string BGM = "BGM"; // BGM�̃p�����[�^��
        public const string SE = "SE";   // SE�̃p�����[�^��
    }
}

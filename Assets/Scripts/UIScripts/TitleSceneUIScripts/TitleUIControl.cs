using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor; // �G�f�B�^�p�̖��O���
#endif

/// <summary>
/// �^�C�g����ʂ̑J�ڂ��Ǘ�����N���X
/// </summary>
public class TitleUIControl : MonoBehaviour
{
    #region �� �����o�ϐ� ��

    [SerializeField] private Image cursorUI;    // �J�[�\���摜
    [SerializeField] private Image startUI;     // "Start" �{�^��
    [SerializeField] private Image settingUI;   // "Settings" �{�^��
    [SerializeField] private Image quitUI;      // "Quit" �{�^��
    [SerializeField] private SettingManager settingManager; // SettingManager�ւ̎Q��

    private List<Image> menuOptions;  // �I�����̃��X�g
    private int currentIndex = 0;     // ���݂̑I���ʒu
    private bool canMove = true;      // ���͎�t�ۃt���O
    private bool stickNeutral = true; // �X�e�B�b�N���j���[�g������Ԃ��ǂ���

    #endregion

    #region �� �I�����̃C���f�b�N�X��` ��

    private const int StartIndex = 0;     // "Start" �̃C���f�b�N�X
    private const int SettingsIndex = 1;  // "Settings" �̃C���f�b�N�X
    private const int QuitIndex = 2;      // "Quit" �̃C���f�b�N�X

    #endregion

    #region �� �萔��` ��

    private const float InputCooldown = 0.2f; // ���͎�t�̃N�[���_�E�����ԁi�A�����͖h�~�p�j
    private const float StickThreshold = 0.5f; // �X�e�B�b�N���͂̔���臒l
    private static readonly Color SelectedColor = Color.white; // �I�𒆂̍��ڂ̐F�i���j
    private static readonly Color UnselectedColor = Color.gray; // ��I���̍��ڂ̐F�i�D�F�j

    #endregion

    /// <summary>
    /// ����������
    /// ���j���[���X�g���쐬���A�J�[�\���������ʒu�֐ݒ�
    /// </summary>
    void Start()
    {
        // ���j���[�̃��X�g���쐬
        menuOptions = new List<Image> { startUI, settingUI, quitUI };

        // �����I���ʒu��ݒ�
        UpdateCursorPosition();
    }

    /// <summary>
    /// ����{�^���iA�{�^���j�ŃV�[���J�� or �ݒ�/�I������
    /// </summary>
    private void OnClick(InputValue value)
    {
        // ���݂̓��̓f�o�C�X���擾�itrue: �R���g���[���[, false: �}�E�X�j
        bool isUsingGamepad = Gamepad.current != null;  // �R���g���[���[�̔���

        if (isUsingGamepad && value.isPressed)
        {
            switch (currentIndex)
            {
                case StartIndex:  // "Start" ���I������Ă���ꍇ
                    SceneManager.LoadScene("IntroductionScene");
                    break;
                case SettingsIndex: // "Settings" ���I������Ă���ꍇ
                    settingManager.ToggleSettingMenu(true); // �ݒ胁�j���[��\��
                    break;
                case QuitIndex: // "Quit" ���I������Ă���ꍇ
                    QuitGame();
                    break;
            }
        }
    }

    /// <summary>
    /// �}�E�X���I�����ɏd�Ȃ����Ƃ��ɑI�����̐F��ύX���A�J�[�\�������킹��
    /// </summary>
    public void OnPointerEnter(int index)
    {
        // ���݂̑I�����̐F���X�V
        currentIndex = index;
        UpdateCursorPosition();
    }

    /// <summary>
    /// �}�E�X�̈ʒu�Ɋ�Â��ăJ�[�\���̈ʒu���X�V
    /// </summary>
    void Update()
    {
        // �ݒ胁�j���[���\������Ă���ꍇ�A���͂𖳌���
        if (settingManager.IsSettingActive)
        {
            canMove = false;
            return;
        }

        // �ݒ胁�j���[���\������Ă��Ȃ��ꍇ�A���͂��󂯕t����
        canMove = true;

        // ���݂̓��̓f�o�C�X���擾�itrue: �R���g���[���[, false: �}�E�X�j
        bool isUsingGamepad = Gamepad.current != null;  // �R���g���[���[�̔���

        if (isUsingGamepad)
        {
            // �R���g���[���[�̏ꍇ�̑���
            HandleGamepadInput();
        }
        else
        {
            // �}�E�X�̏ꍇ�̑���
            HandleMouseInput();
        }
    }

    /// <summary>
    /// �R���g���[���[�̓��͂őI������ύX
    /// </summary>
    private void HandleGamepadInput()
    {
        Vector2 input = Gamepad.current.leftStick.ReadValue();

        if (canMove && stickNeutral)
        {
            if (input.y > StickThreshold) // ��Ɉړ�
            {
                currentIndex = (currentIndex - 1 + menuOptions.Count) % menuOptions.Count;
                UpdateCursorPosition();
                StartCoroutine(ResetMove());
                stickNeutral = false; // �X�e�B�b�N�������ꂽ�̂Ńj���[�g�����łȂ�
            }
            else if (input.y < -StickThreshold) // ���Ɉړ�
            {
                currentIndex = (currentIndex + 1) % menuOptions.Count;
                UpdateCursorPosition();
                StartCoroutine(ResetMove());
                stickNeutral = false; // �X�e�B�b�N�������ꂽ�̂Ńj���[�g�����łȂ�
            }
        }

        // �X�e�B�b�N���������l�����i�قڒ����j�ɂȂ�����j���[�g�����ɖ߂�
        if (Mathf.Abs(input.y) < 0.1f)
        {
            stickNeutral = true;
        }
    }

    /// <summary>
    /// �}�E�X�őI������ύX
    /// </summary>
    private void HandleMouseInput()
    {
        // �}�E�X�̈ʒu�őI������ύX
        Vector3 mousePosition = Mouse.current.position.ReadValue();

        // �}�E�X���e�I�����̋߂��ɂ���ꍇ�A���̑I������I����Ԃɂ���
        for (int i = 0; i < menuOptions.Count; i++)
        {
            // �I�����̈ʒu�ƃT�C�Y����Ƀ}�E�X�ʒu���߂�������
            if (RectTransformUtility.RectangleContainsScreenPoint(menuOptions[i].rectTransform, mousePosition))
            {
                currentIndex = i;
                UpdateCursorPosition();
                break;
            }
        }
    }

    /// <summary>
    /// "Start" �N���b�N�ŃV�[���J��
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("IntroductionScene");
    }

    /// <summary>
    /// "Settings"�N���b�N�ŃV�[���J��
    /// </summary>
    public void SettingMenu()
    {
        settingManager.ToggleSettingMenu(true); // �ݒ胁�j���[��\��
    }

    /// <summary>
    /// "Quit"�N���b�N�ŃQ�[���I������
    /// </summary>
    public void Quit()
    {
        QuitGame();
    }

    /// <summary>
    /// "Quit" �I�����̏����i�J�����ł̓G�f�B�^�̎��s���~�A�r���h�łł̓Q�[���I���j
    /// </summary>
    private void QuitGame()
    {
#if UNITY_EDITOR
        // �G�f�B�^���s���̏ꍇ�͒�~
        EditorApplication.isPlaying = false;
#else
        // �r���h�łł̓A�v���P�[�V�������I��
        Application.Quit();
#endif
    }

    /// <summary>
    /// �J�[�\���̈ʒu�����݂̑I�𒆂�UI�ɍ��킹��
    /// </summary>
    private void UpdateCursorPosition()
    {
        // �J�[�\���̈ʒu���A���ݑI�𒆂�UI�̈ʒu�Ɉړ�
        cursorUI.transform.position = menuOptions[currentIndex].transform.position;

        // �I�𒆂̃��j���[�̐F���X�V
        SetMenuColor();
    }

    // �ݒ胁�j���[����鏈��
    public void CloseSettingMenu()
    {
        settingManager.ToggleSettingMenu(false);
    }

    /// <summary>
    /// �I�𒆂�UI�͔��A��I����UI�͊D�F�ɂ���
    /// </summary>
    private void SetMenuColor()
    {
        for (int i = 0; i < menuOptions.Count; i++)
        {
            // �C���X�y�N�^�Őݒ肳�ꂽ�F�𖳎����A�R�[�h�Ŏw�肵���F�������K�p
            if (i == currentIndex)
            {
                menuOptions[i].color = SelectedColor;  // �I�𒆂̐F�i���j
            }
            else
            {
                menuOptions[i].color = UnselectedColor;  // ��I���̐F�i�D�F�j
            }
        }
    }

    /// <summary>
    /// ��莞�Ԍo�ߌ�ɓ��͂��Ď�t����i�A�����͖h�~�j
    /// </summary>
    private IEnumerator ResetMove()
    {
        canMove = false; // �ꎞ�I�ɓ��͂𖳌���
        yield return new WaitForSeconds(InputCooldown); // �N�[���_�E�����ԑҋ@
        canMove = true; // ���͂��Ď�t�\�ɂ���
    }
}

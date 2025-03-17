using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ���̓f�o�C�X�Ǘ��N���X
/// </summary>
public class InputDeviceManager : MonoBehaviour
{
    private static InputDeviceManager instance;
    private bool isUsingGamepad = false;

    public static InputDeviceManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �V�[�����܂����ł��c��

            // �t���[�����[�g��60�ɌŒ�
            Application.targetFrameRate = 60;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // �Q�[���p�b�h���ڑ�����Ă��邩����
        isUsingGamepad = Gamepad.current != null;
    }

    /// <summary>
    /// ���݃Q�[���p�b�h���g�p����Ă��邩���擾
    /// </summary>
    public bool IsUsingGamepad()
    {
        return isUsingGamepad;
    }
}
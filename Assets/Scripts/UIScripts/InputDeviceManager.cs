using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 入力デバイス管理クラス
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
            DontDestroyOnLoad(gameObject); // シーンをまたいでも残る

            // フレームレートを60に固定
            Application.targetFrameRate = 60;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // ゲームパッドが接続されているか判定
        isUsingGamepad = Gamepad.current != null;
    }

    /// <summary>
    /// 現在ゲームパッドが使用されているかを取得
    /// </summary>
    public bool IsUsingGamepad()
    {
        return isUsingGamepad;
    }
}
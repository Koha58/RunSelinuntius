using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 設定メニューの管理クラス
/// </summary>
public class SettingManager : MonoBehaviour
{
    [SerializeField] private GameObject SettingMenu; // 設定メニューオブジェクト
    [SerializeField] private Image buttonBui;       // BボタンのUI
    [SerializeField] private Image settingCursorUI; // カーソル画像
    [SerializeField] private Slider BGMSlider;      // BGMスライダー
    [SerializeField] private Slider SESlider;       // SEスライダー
    [SerializeField] private AudioMixer audioMixer; // オーディオミキサー

    private List<Slider> sliders;
    private int currentIndex = 0;
    private bool stickNeutral = true;
    private bool canMove = true;

    // 定数値
    private const float InputCooldown = 0.1f; // 連続入力のクールダウン時間
    private const float StickThreshold = 0.5f; // スティックの入力閾値（移動方向を判定するためのしきい値）
    private const float MinStickNeutralThreshold = 0.1f; // スティックがニュートラルと見なされる閾値
    private const float VolumeAdjustStep = 0.1f; // 音量変更の基本ステップ
    private const float VolumeAdjustMultiplierMax = 5f; // スティックの入力強度に応じた最大音量調整倍率
    private const float CursorOffsetX = -400f; // カーソル位置のX軸オフセット

    // 設定メニューの状態を管理するプロパティ
    public bool IsSettingActive { get; private set; } // 設定メニューの状態

    private void Start()
    {
        // 初期状態で設定メニューを非表示にし、UI関連の表示を制御する
        ToggleSettingMenu(false);
        buttonBui.enabled = false;
        settingCursorUI.enabled = false;

        // スライダーをリスト化し、初期化処理を行う
        sliders = new List<Slider> { BGMSlider, SESlider };

        // オーディオミキサーから音量を取得してスライダーに反映
        InitializeSliders();
        // 初期カーソル位置の設定
        UpdateCursorPosition();
    }

    /// <summary>
    /// オーディオミキサーから現在の音量を取得し、スライダーに適用する
    /// </summary>
    private void InitializeSliders()
    {
        // BGMとSEのスライダーにそれぞれ音量を設定
        InitializeSlider(BGMSlider, AudioParameterName.BGM);
        InitializeSlider(SESlider, AudioParameterName.SE);
    }

    /// <summary>
    /// 指定されたスライダーの音量をオーディオミキサーから取得して適用する
    /// </summary>
    /// <param name="slider">音量を設定するスライダー</param>
    /// <param name="parameterName">オーディオミキサーで設定されているパラメータ名（"BGM"または"SE"）</param>
    private void InitializeSlider(Slider slider, string parameterName)
    {
        // オーディオミキサーから音量の現在値を取得
        if (audioMixer.GetFloat(parameterName, out float volume))
        {
            // 取得した音量をスライダーに反映
            slider.value = volume;
        }
    }

    /// <summary>
    /// 設定メニューを開閉する
    /// </summary>
    /// <param name="isActive">設定メニューを表示するかどうかのフラグ</param>
    public void ToggleSettingMenu(bool isActive)
    {
        // メニューの表示・非表示を制御
        SettingMenu.SetActive(isActive);
        IsSettingActive = isActive;

        // ゲームパッドが使用されている場合のみ、UIのBボタンとカーソルを有効にする
        bool isUsingGamepad = Gamepad.current != null;
        buttonBui.enabled = isUsingGamepad && isActive;
        settingCursorUI.enabled = isUsingGamepad && isActive;

        // メニューが表示されている場合、カーソル位置を更新
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
        // 設定メニューが表示されている場合のみ処理を行う
        if (!IsSettingActive) return;

        // 現在のシーンがGameSceneの場合、canMoveをtrueに設定
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            canMove = true;
        }

        // ゲームパッドが使用されている場合、入力を処理
        if (Gamepad.current != null)
        {
            HandleGamepadInput();
        }
    }

    /// <summary>
    /// ゲームパッドの入力を処理する
    /// </summary>
    private void HandleGamepadInput()
    {
        // 左スティックの入力を取得
        Vector2 input = Gamepad.current.leftStick.ReadValue();

        // 入力可能な状態で、スティックの上下入力に応じてスライダーの選択を変更
        if (canMove && stickNeutral)
        {
            if (input.y > StickThreshold) // 上に入力された場合、前のスライダーに移動
            {
                ChangeSliderSelection(-1);
            }
            else if (input.y < -StickThreshold) // 下に入力された場合、次のスライダーに移動
            {
                ChangeSliderSelection(1);
            }
        }

        // スティックのy軸入力がニュートラル領域（しきい値未満）に戻った場合
        if (Mathf.Abs(input.y) < MinStickNeutralThreshold)
        {
            stickNeutral = true;
        }

        // 左右入力で音量調整
        if (input.x > StickThreshold)
        {
            AdjustVolume(VolumeAdjustStep); // 音量を増加
        }
        else if (input.x < -StickThreshold)
        {
            AdjustVolume(-VolumeAdjustStep); // 音量を減少
        }
    }

    /// <summary>
    /// スライダーの選択を変更する
    /// </summary>
    /// <param name="direction">選択方向（-1: 上、1: 下）</param>
    private void ChangeSliderSelection(int direction)
    {
        // 現在選択中のスライダーインデックスを変更
        currentIndex = (currentIndex + direction + sliders.Count) % sliders.Count;
        // カーソルの位置を更新
        UpdateCursorPosition();
        // 移動処理が終わるまで次の移動を受け付けない
        StartCoroutine(ResetMove());
        stickNeutral = false;
    }

    /// <summary>
    /// カーソルの位置を現在選択中のスライダーの位置に合わせる
    /// </summary>
    private void UpdateCursorPosition()
    {
        // カーソル位置のオフセットを適用し、現在選択されているスライダーにカーソルを移動
        Vector3 offset = new Vector3(CursorOffsetX, 0f, 0f);
        settingCursorUI.transform.position = sliders[currentIndex].transform.position + offset;
    }

    /// <summary>
    /// 音量を調整する
    /// </summary>
    /// <param name="adjustment">音量調整の変更量（正: 増加、負: 減少）</param>
    private void AdjustVolume(float adjustment)
    {
        // 現在選択されているスライダーを取得
        Slider selectedSlider = sliders[currentIndex];

        // スティックのx軸の入力強度を取得
        float inputStrength = Mathf.Abs(Gamepad.current.leftStick.x.ReadValue());

        // 入力強度に応じて音量調整速度を変化させる
        float speedMultiplier = Mathf.Lerp(1f, VolumeAdjustMultiplierMax, inputStrength);

        // 音量を調整し、スライダーの範囲内に収める
        selectedSlider.value = Mathf.Clamp(selectedSlider.value + adjustment * speedMultiplier, selectedSlider.minValue, selectedSlider.maxValue);

        // 現在のスライダーに対応したオーディオミキサーのパラメータを設定
        audioMixer.SetFloat(currentIndex == 0 ? AudioParameterName.BGM : AudioParameterName.SE, selectedSlider.value);
    }

    /// <summary>
    /// 入力受付をクールダウンする
    /// </summary>
    private IEnumerator ResetMove()
    {
        // 入力受付を無効化
        canMove = false;
        // 一定時間待機
        yield return new WaitForSeconds(InputCooldown);
        // 入力受付を再度有効化
        canMove = true;
    }

    /// <summary>
    /// 設定メニューを閉じる
    /// </summary>
    public void Close()
    {
        ToggleSettingMenu(false);
    }

    /// <summary>
    /// バックボタンが押されたときの処理
    /// </summary>
    /// <param name="value">バックボタンの入力値</param>
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
    /// 設定メニューを閉じる
    /// </summary>
    public void Quit()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // 音声パラメータ名を定義するクラス
    private static class AudioParameterName
    {
        public const string BGM = "BGM"; // BGMのパラメータ名
        public const string SE = "SE";   // SEのパラメータ名
    }
}

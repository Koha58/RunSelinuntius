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
    // 設定メニュー関連のオブジェクト
    [SerializeField] private GameObject SettingMenu;  // 設定メニュー全体のオブジェクト
    [SerializeField] private Image buttonBui;         // BボタンのUI（戻るボタン表示用）
    [SerializeField] private Image buttonAui;         // AボタンのUI（タイトルへ戻るボタン表示用）
    [SerializeField] private Image settingCursorUI;   // 設定メニューのカーソル画像
    [SerializeField] private Slider BGMSlider;        // BGM音量調整用スライダー
    [SerializeField] private Slider SESlider;         // SE音量調整用スライダー
    [SerializeField] private AudioMixer audioMixer;   // オーディオミキサー（音量制御）

    // スライダーの管理
    private List<Slider> sliders; // 設定メニュー内のスライダー（BGM・SE）を管理するリスト
    private int currentIndex = 0; // 現在選択中の項目（0: BGM, 1: SE, 2: Aボタン）
    private int totalOptions;     // 選択可能な項目数（スライダーの数 + AボタンUI）

    // スティック入力の管理
    private bool stickNeutral = true; // スティックの入力がニュートラルな状態か（押しっぱなし防止）
    private bool canMove = true;      // 選択移動が可能か（入力クールダウン管理）

    // カーソル位置管理
    private Vector3 lastCursorPosition; // 最後に設定されたカーソルの位置

    // 定数（数値設定）
    private const float InputCooldown = 0.1f;           // 連続入力のクールダウン時間
    private const float StickThreshold = 0.5f;          // スティックの入力閾値
    private const float MinStickNeutralThreshold = 0.1f;// スティックがニュートラルと見なされる閾値
    private const float VolumeAdjustStep = 0.1f;        // 音量変更の基本ステップ
    private const float VolumeAdjustMultiplierMax = 5f; // スティック入力強度による音量調整倍率
    private const float CursorOffsetX = -400f;         // カーソル位置のX軸オフセット
    private const float DefaultVolume = 0f;            // 音量のデフォルト値

    // 設定メニューの状態を管理するプロパティ
    public bool IsSettingActive { get; private set; }  // 設定メニューが開いているか


    private void Start()
    {
        // 初期状態で設定メニューを非表示にし、UI関連の表示を制御する
        ToggleSettingMenu(false);
        buttonBui.enabled = false;
        settingCursorUI.enabled = false;

        // スライダーをリスト化し、初期化処理を行う
        sliders = new List<Slider> { BGMSlider, SESlider };

        // AボタンUIがある場合、選択肢を追加
        totalOptions = sliders.Count;
        if (buttonAui != null)
        {
            buttonAui.enabled = false;
            totalOptions++; // AボタンUIを選択対象に追加
        }

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
    /// 指定されたスライダーの音量を PlayerPrefs からロードし、オーディオミキサーに適用する。
    /// </summary>
    /// <param name="slider">対象のスライダー</param>
    /// <param name="parameterName">オーディオミキサーでのパラメータ名（"BGM" または "SE"）</param>
    private void InitializeSlider(Slider slider, string parameterName)
    {
        // PlayerPrefs から保存された音量を取得（未設定の場合は DefaultVolume を使用）
        float defaultValue = PlayerPrefs.GetFloat(parameterName, DefaultVolume);

        // PlayerPrefs から保存されている音量設定を取得
        if (parameterName == AudioParameterName.BGM)
        {
            defaultValue = PlayerPrefs.GetFloat(AudioParameterName.BGM, DefaultVolume);
        }
        else if (parameterName == AudioParameterName.SE)
        {
            defaultValue = PlayerPrefs.GetFloat(AudioParameterName.SE, DefaultVolume);
        }

        // スライダーの値を設定し、オーディオミキサーにも反映
        slider.value = defaultValue;
        audioMixer.SetFloat(parameterName, defaultValue);
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
        // 設定メニューが表示されている場合のみ処理を行う
        if (!IsSettingActive) return;

        // 現在のシーンがGameSceneの場合、canMoveをtrueに設定
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            canMove = true;

            // ゲームパッドが接続されており、B(East)ボタンが押されたら
            if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
            {
                // 設定メニューを閉じる
                Close();
            }
            // ゲームパッドが接続されており、AボタンUIが表示され、かつA(South)ボタンが押されたら
            else if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame && buttonAui.enabled == true)
            {
                // タイトルシーンに戻る
                Quit();
            }
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
        // 選択肢の数に応じてインデックスを循環させる
        currentIndex = (currentIndex + direction + totalOptions) % totalOptions;
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
        Vector3 offset = new Vector3(CursorOffsetX, 0f, 0f);

        if (currentIndex < sliders.Count)
        {
            // スライダーを選択している場合
            // カーソルの位置を現在のスライダーの位置 + オフセットに設定
            settingCursorUI.transform.position = sliders[currentIndex].transform.position + offset;

            // 記憶していたカーソルのX座標を更新
            lastCursorPosition = settingCursorUI.transform.position;

            // AボタンUIが存在する場合は非表示にする（スライダー選択時は不要）
            if (buttonAui != null)
            {
                buttonAui.enabled = false;
            }
        }
        else if (buttonAui != null)
        {
            // AボタンUIを選択している場合
            // AボタンUIを有効化（表示）
            buttonAui.enabled = true;

            // 記憶していたX座標を使用し、AボタンのY座標にカーソルを移動
            settingCursorUI.transform.position = new Vector3(
                lastCursorPosition.x,  // X座標は保持（スライダーと揃える）
                buttonAui.transform.position.y, // Y座標のみAボタンの位置に合わせる
                lastCursorPosition.z   // Z座標は保持
            );
        }
    }

    /// <summary>
    /// 現在選択されているスライダーの音量を調整し、設定を保存する。
    /// </summary>
    /// <param name="adjustment">音量の増減値（正: 増加, 負: 減少）</param>
    private void AdjustVolume(float adjustment)
    {
        // Aボタン UI が選択されている場合は音量調整を行わない
        if (currentIndex >= sliders.Count) return;

        // 現在選択されているスライダーを取得
        Slider selectedSlider = sliders[currentIndex];

        // スティックの入力強度を取得し、音量変更速度を調整
        float inputStrength = Mathf.Abs(Gamepad.current.leftStick.x.ReadValue());
        float speedMultiplier = Mathf.Lerp(1f, VolumeAdjustMultiplierMax, inputStrength);

        // スライダーの値を変更（範囲を超えないように Clamp）
        selectedSlider.value = Mathf.Clamp(selectedSlider.value + adjustment * speedMultiplier, selectedSlider.minValue, selectedSlider.maxValue);

        // オーディオミキサーに適用するパラメータ名を判定
        string parameterName = currentIndex == 0 ? AudioParameterName.BGM : AudioParameterName.SE;

        // オーディオミキサーに新しい音量を適用
        audioMixer.SetFloat(parameterName, selectedSlider.value);

        // 設定を PlayerPrefs に保存
        PlayerPrefs.SetFloat(parameterName, selectedSlider.value);
        PlayerPrefs.Save();
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
    /// Bボタンが押されたときの処理
    /// </summary>
    /// <param name="value">Bボタンの入力値</param>
    private void OnBack(InputValue value)
    {
        Close();
    }

    /// <summary>
    /// タイトルシーンに戻る
    /// </summary>
    public void Quit()
    {
        SceneManager.LoadScene("TitleScene");
    }

    /// <summary>
    /// 音声パラメータ名を定義するクラス
    /// </summary>
    private static class AudioParameterName
    {
        public const string BGM = "BGM"; // BGMのパラメータ名
        public const string SE = "SE";   // SEのパラメータ名
    }
}

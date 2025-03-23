using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor; // エディタ用の名前空間
#endif

/// <summary>
/// タイトル画面の遷移を管理するクラス
/// </summary>
public class TitleUIControl : MonoBehaviour
{
    #region ▼ メンバ変数 ▼

    [SerializeField] private Image cursorUI;    // カーソル画像
    [SerializeField] private Image startUI;     // "Start" ボタン
    [SerializeField] private Image settingUI;   // "Settings" ボタン
    [SerializeField] private Image quitUI;      // "Quit" ボタン
    [SerializeField] private SettingManager settingManager; // SettingManagerへの参照

    private List<Image> menuOptions;  // 選択肢のリスト
    private int currentIndex = 0;     // 現在の選択位置
    private bool canMove = true;      // 入力受付可否フラグ
    private bool stickNeutral = true; // スティックがニュートラル状態かどうか

    #endregion

    #region ▼ 選択肢のインデックス定義 ▼

    private const int StartIndex = 0;     // "Start" のインデックス
    private const int SettingsIndex = 1;  // "Settings" のインデックス
    private const int QuitIndex = 2;      // "Quit" のインデックス

    #endregion

    #region ▼ 定数定義 ▼

    private const float InputCooldown = 0.2f; // 入力受付のクールダウン時間（連続入力防止用）
    private const float StickThreshold = 0.5f; // スティック入力の判定閾値
    private static readonly Color SelectedColor = Color.white; // 選択中の項目の色（白）
    private static readonly Color UnselectedColor = Color.gray; // 非選択の項目の色（灰色）

    #endregion

    /// <summary>
    /// 初期化処理
    /// メニューリストを作成し、カーソルを初期位置へ設定
    /// </summary>
    void Start()
    {
        // メニューのリストを作成
        menuOptions = new List<Image> { startUI, settingUI, quitUI };

        // 初期選択位置を設定
        UpdateCursorPosition();
    }

    /// <summary>
    /// 決定ボタン（Aボタン）でシーン遷移 or 設定/終了処理
    /// </summary>
    private void OnClick(InputValue value)
    {
        // 現在の入力デバイスを取得（true: コントローラー, false: マウス）
        bool isUsingGamepad = Gamepad.current != null;  // コントローラーの判定

        if (isUsingGamepad && value.isPressed)
        {
            switch (currentIndex)
            {
                case StartIndex:  // "Start" が選択されている場合
                    SceneManager.LoadScene("IntroductionScene");
                    break;
                case SettingsIndex: // "Settings" が選択されている場合
                    settingManager.ToggleSettingMenu(true); // 設定メニューを表示
                    break;
                case QuitIndex: // "Quit" が選択されている場合
                    QuitGame();
                    break;
            }
        }
    }

    /// <summary>
    /// マウスが選択肢に重なったときに選択肢の色を変更し、カーソルを合わせる
    /// </summary>
    public void OnPointerEnter(int index)
    {
        // 現在の選択肢の色を更新
        currentIndex = index;
        UpdateCursorPosition();
    }

    /// <summary>
    /// マウスの位置に基づいてカーソルの位置を更新
    /// </summary>
    void Update()
    {
        // 設定メニューが表示されている場合、入力を無効化
        if (settingManager.IsSettingActive)
        {
            canMove = false;
            return;
        }

        // 設定メニューが表示されていない場合、入力を受け付ける
        canMove = true;

        // 現在の入力デバイスを取得（true: コントローラー, false: マウス）
        bool isUsingGamepad = Gamepad.current != null;  // コントローラーの判定

        if (isUsingGamepad)
        {
            // コントローラーの場合の操作
            HandleGamepadInput();
        }
        else
        {
            // マウスの場合の操作
            HandleMouseInput();
        }
    }

    /// <summary>
    /// コントローラーの入力で選択肢を変更
    /// </summary>
    private void HandleGamepadInput()
    {
        Vector2 input = Gamepad.current.leftStick.ReadValue();

        if (canMove && stickNeutral)
        {
            if (input.y > StickThreshold) // 上に移動
            {
                currentIndex = (currentIndex - 1 + menuOptions.Count) % menuOptions.Count;
                UpdateCursorPosition();
                StartCoroutine(ResetMove());
                stickNeutral = false; // スティックが押されたのでニュートラルでない
            }
            else if (input.y < -StickThreshold) // 下に移動
            {
                currentIndex = (currentIndex + 1) % menuOptions.Count;
                UpdateCursorPosition();
                StartCoroutine(ResetMove());
                stickNeutral = false; // スティックが押されたのでニュートラルでない
            }
        }

        // スティックがしきい値未満（ほぼ中央）になったらニュートラルに戻す
        if (Mathf.Abs(input.y) < 0.1f)
        {
            stickNeutral = true;
        }
    }

    /// <summary>
    /// マウスで選択肢を変更
    /// </summary>
    private void HandleMouseInput()
    {
        // マウスの位置で選択肢を変更
        Vector3 mousePosition = Mouse.current.position.ReadValue();

        // マウスが各選択肢の近くにある場合、その選択肢を選択状態にする
        for (int i = 0; i < menuOptions.Count; i++)
        {
            // 選択肢の位置とサイズを基にマウス位置が近いか判定
            if (RectTransformUtility.RectangleContainsScreenPoint(menuOptions[i].rectTransform, mousePosition))
            {
                currentIndex = i;
                UpdateCursorPosition();
                break;
            }
        }
    }

    /// <summary>
    /// "Start" クリックでシーン遷移
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("IntroductionScene");
    }

    /// <summary>
    /// "Settings"クリックでシーン遷移
    /// </summary>
    public void SettingMenu()
    {
        settingManager.ToggleSettingMenu(true); // 設定メニューを表示
    }

    /// <summary>
    /// "Quit"クリックでゲーム終了処理
    /// </summary>
    public void Quit()
    {
        QuitGame();
    }

    /// <summary>
    /// "Quit" 選択時の処理（開発環境ではエディタの実行を停止、ビルド版ではゲーム終了）
    /// </summary>
    private void QuitGame()
    {
#if UNITY_EDITOR
        // エディタ実行中の場合は停止
        EditorApplication.isPlaying = false;
#else
        // ビルド版ではアプリケーションを終了
        Application.Quit();
#endif
    }

    /// <summary>
    /// カーソルの位置を現在の選択中のUIに合わせる
    /// </summary>
    private void UpdateCursorPosition()
    {
        // カーソルの位置を、現在選択中のUIの位置に移動
        cursorUI.transform.position = menuOptions[currentIndex].transform.position;

        // 選択中のメニューの色を更新
        SetMenuColor();
    }

    // 設定メニューを閉じる処理
    public void CloseSettingMenu()
    {
        settingManager.ToggleSettingMenu(false);
    }

    /// <summary>
    /// 選択中のUIは白、非選択のUIは灰色にする
    /// </summary>
    private void SetMenuColor()
    {
        for (int i = 0; i < menuOptions.Count; i++)
        {
            // インスペクタで設定された色を無視し、コードで指定した色を強制適用
            if (i == currentIndex)
            {
                menuOptions[i].color = SelectedColor;  // 選択中の色（白）
            }
            else
            {
                menuOptions[i].color = UnselectedColor;  // 非選択の色（灰色）
            }
        }
    }

    /// <summary>
    /// 一定時間経過後に入力を再受付する（連続入力防止）
    /// </summary>
    private IEnumerator ResetMove()
    {
        canMove = false; // 一時的に入力を無効化
        yield return new WaitForSeconds(InputCooldown); // クールダウン時間待機
        canMove = true; // 入力を再受付可能にする
    }
}

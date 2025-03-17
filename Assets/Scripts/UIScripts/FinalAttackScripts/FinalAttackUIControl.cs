using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// FinalAttackUI(クリアするために必要な攻撃用UI)管理クラス
/// </summary>
public class FinalAttackUIControl : MonoBehaviour
{
    [SerializeField] private Image pressUI;  // 共通 UI
    [SerializeField] private Image keyboardUI;  // キーボード用 UI
    [SerializeField] private Image controllerUI; // コントローラー用 UI
    [SerializeField] private Color blinkColor = Color.gray; // 点滅時の色
    [SerializeField] private float blinkInterval = 0.5f; // 点滅間隔（秒）
    [SerializeField] private PlayerMove playerMove; // PlayerMove の参照

    private bool isBlinking = false;   // 現在点滅中かどうか

    private void Awake()
    {
        // 同じオブジェクトにアタッチされた PlayerMove を取得
        playerMove = GetComponent<PlayerMove>();
        pressUI.enabled = false;
    }

    void Update()
    {
        if (playerMove == null || InputDeviceManager.Instance == null) return;

        // 現在の入力デバイスを取得（true: コントローラー, false: キーボード）
        bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();

        // プレイヤーが FinalAttack 可能かを取得
        bool isNearTarget = playerMove.IsFinalAttackPossible();

        // UI の表示 / 非表示
        keyboardUI.enabled = isNearTarget && !isUsingGamepad;
        controllerUI.enabled = isNearTarget && isUsingGamepad;

        // 点滅処理を開始・停止
        if (isNearTarget && !isBlinking)
        {
            StartCoroutine(BlinkUI());
        }
        else if (!isNearTarget && isBlinking)
        {
            StopCoroutine(BlinkUI());
            ResetUIColor();
        }
    }

    /// <summary>
    /// UI を点滅させるコルーチン
    /// </summary>
    private IEnumerator BlinkUI()
    {
        isBlinking = true;
        pressUI.enabled = true;
        while (true)
        {
            // 現在の UI を取得
            bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();
            Image activeUI = isUsingGamepad ? controllerUI : keyboardUI;
            Image uiImage = activeUI.GetComponent<Image>();

            if (uiImage != null)
            {
                // 色を切り替える
                uiImage.color = (uiImage.color == Color.white) ? blinkColor : Color.white;
            }

            yield return new WaitForSeconds(blinkInterval);
        }
    }

    /// <summary>
    /// UI の色を元に戻す
    /// </summary>
    private void ResetUIColor()
    {
        keyboardUI.GetComponent<Image>().color = Color.white;
        controllerUI.GetComponent<Image>().color = Color.white;
        pressUI.enabled = false;
        isBlinking = false;
    }
}
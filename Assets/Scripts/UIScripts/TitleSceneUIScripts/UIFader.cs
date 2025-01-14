using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// StartのUIを点滅させるクラス
/// </summary>
public class UIFader : MonoBehaviour
{
    // フェード対象のImageコンポーネント
    [SerializeField] private Image startImage;

    // フェードイン・アウトの周期（秒単位）
    [SerializeField] private float fadeDuration = 1f;

    // スクリプト開始時にフェードを自動的に開始するかどうかのフラグ
    [SerializeField] private bool startFading = true;

    // 現在のフェード方向を管理するフラグ（true: フェードイン, false: フェードアウト）
    private bool isFadingIn = true;

    // 現在のアルファ値（透明度）
    private float currentAlpha = 1f;

    private void Start()
    {
        // startImageが設定されていない場合、アタッチされているImageコンポーネントを取得
        if (startImage == null)
        {
            startImage = GetComponent<Image>();
        }

        // startFadingがtrueの場合、フェード処理を開始
        if (startFading)
        {
            StartCoroutine(FadeLoop());
        }
    }

    /// <summary>
    /// フェード処理を繰り返すコルーチン。
    /// フェードインとフェードアウトを交互に実行。
    /// </summary>
    private System.Collections.IEnumerator FadeLoop()
    {
        while (true)
        {
            // フェードの開始と終了のアルファ値を設定
            float elapsedTime = 0f;
            float startAlpha = isFadingIn ? 0f : 1f;
            float endAlpha = isFadingIn ? 1f : 0f;

            // 指定されたfadeDurationの間でアルファ値を補間
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime; // 経過時間を加算
                currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration); // 線形補間
                UpdateAlpha(currentAlpha); // アルファ値を更新
                yield return null; // 次のフレームまで待機
            }

            // フェードの方向を反転
            isFadingIn = !isFadingIn;
        }
    }

    /// <summary>
    /// 画像のアルファ値を更新する。
    /// </summary>
    /// <param name="alpha">更新するアルファ値（0～1）</param>
    private void UpdateAlpha(float alpha)
    {
        if (startImage != null)
        {
            Color color = startImage.color; // 現在の色を取得
            color.a = alpha; // アルファ値を設定
            startImage.color = color; // 更新された色を反映
        }
    }

    /// <summary>
    /// フェード処理を開始する。
    /// </summary>
    public void StartFading()
    {
        startFading = true;
        StartCoroutine(FadeLoop());
    }

    /// <summary>
    /// フェード処理を停止する。
    /// </summary>
    public void StopFading()
    {
        startFading = false;
        StopAllCoroutines(); // 全てのコルーチンを停止
    }
}

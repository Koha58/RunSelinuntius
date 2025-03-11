using System.Collections;
using UnityEngine;

/// <summary>
/// 時間経過で空の色を変化させるクラス
/// </summary>
public class SkyboxChanger : MonoBehaviour
{
    // Skybox 切り替え間隔（秒）
    private const float DEFAULT_CHANGE_INTERVAL = 20f;

    // Skybox フェード時間（秒）
    private const float DEFAULT_TRANSITION_DURATION = 5f;

    // Skybox のインデックス増加値
    private const int INDEX_INCREMENT = 1;

    // 1フレームごとに待機する時間（秒）
    private const float FRAME_WAIT_TIME = 0f;

    // 切り替える Skybox のマテリアル一覧（Inspector で設定）
    [SerializeField] private Material[] skyboxMaterials;

    // Skybox を変更する間隔（秒）
    [SerializeField] private float changeInterval = DEFAULT_CHANGE_INTERVAL;

    // Skybox のフェードにかかる時間（秒）
    [SerializeField] private float transitionDuration = DEFAULT_TRANSITION_DURATION;

    // 現在の Skybox インデックス（最初のマテリアル）
    private int currentIndex = 0;

    void Start()
    {
        // Skybox の変更処理をコルーチンで実行開始
        StartCoroutine(ChangeSkyboxRoutine());
    }

    IEnumerator ChangeSkyboxRoutine()
    {
        while (true)
        {
            // 次の Skybox のインデックスを決定（ループするように）
            int nextIndex = (currentIndex + INDEX_INCREMENT) % skyboxMaterials.Length;

            // 現在の Skybox マテリアルを取得
            Material currentSkybox = skyboxMaterials[currentIndex];

            // 次に適用する Skybox マテリアルを取得
            Material nextSkybox = skyboxMaterials[nextIndex];

            // 一度、現在の Skybox を設定（Lerp のベースとして使用）
            RenderSettings.skybox = currentSkybox;

            // ライティング情報を更新（変更を反映させる）
            DynamicGI.UpdateEnvironment();

            // 徐々に Skybox をフェードさせる
            float elapsedTime = 0f; // 経過時間を初期化

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime; // 経過時間を更新
                float t = Mathf.Clamp01(elapsedTime / transitionDuration); // 0 から 1 へスムーズに変化する値

                // Lerp で Skybox のマテリアルを滑らかに補間
                RenderSettings.skybox.Lerp(currentSkybox, nextSkybox, t);

                // ライティング情報を更新
                DynamicGI.UpdateEnvironment();

                // 次のフレームまで待機
                yield return new WaitForSeconds(FRAME_WAIT_TIME);
            }

            // 最終的な Skybox に確定（誤差をなくすため）
            RenderSettings.skybox = nextSkybox;

            // ライティング情報を更新
            DynamicGI.UpdateEnvironment();

            // 次の Skybox に切り替え
            currentIndex = nextIndex;

            // 次の変更まで待機
            yield return new WaitForSeconds(changeInterval);
        }
    }
}
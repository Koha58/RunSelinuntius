using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// スライダーを制御し、オーディオミキサーの音量を調整するクラス
/// </summary>
public class SliderControl : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; // オーディオミキサー
    [SerializeField] private Slider BGMSlider;      // BGMの音量調整スライダー
    [SerializeField] private Slider SESlider;       // SEの音量調整スライダー
    [SerializeField] private AudioSource sampleSE;  // サンプルSEのAudioSource

    private void Start()
    {
        // スライダーの初期値をオーディオミキサーから取得
        InitializeSlider(BGMSlider, "BGM");
        InitializeSlider(SESlider, "SE");

        // スライダーの値変更時に音量を適用
        BGMSlider.onValueChanged.AddListener(SetBGM);
        SESlider.onValueChanged.AddListener(SetSE);
    }

    /// <summary>
    /// 指定したスライダーにオーディオミキサーの値を適用
    /// </summary>
    private void InitializeSlider(Slider slider, string parameterName)
    {
        if (audioMixer.GetFloat(parameterName, out float volume))
        {
            slider.value = volume;
        }
    }

    /// <summary>
    /// BGMの音量を変更
    /// </summary>
    public void SetBGM(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }

    /// <summary>
    /// SEの音量を変更し、サンプルSEを再生
    /// </summary>
    public void SetSE(float volume)
    {
        audioMixer.SetFloat("SE", volume);
        PlaySampleSE();
    }

    /// <summary>
    /// サンプルSEを再生
    /// </summary>
    private void PlaySampleSE()
    {
        if (sampleSE != null)
        {
            sampleSE.Play();
        }
    }
}
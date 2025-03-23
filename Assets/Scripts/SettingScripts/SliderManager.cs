using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SliderManager : MonoBehaviour
{
    private static SliderManager instance;

    public static SliderManager Instance => instance;

    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SESlider;
    [SerializeField] private AudioMixer audioMixer; // オーディオミキサー

    private void Start()
    {
        // 必要な初期設定
    }

    public void SetBGM(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }

    public void SetSE(float volume)
    {
        audioMixer.SetFloat("SE", volume);
    }
}

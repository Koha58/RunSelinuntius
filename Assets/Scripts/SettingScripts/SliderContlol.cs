using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// �X���C�_�[�𐧌䂵�A�I�[�f�B�I�~�L�T�[�̉��ʂ𒲐�����N���X
/// </summary>
public class SliderControl : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; // �I�[�f�B�I�~�L�T�[
    [SerializeField] private Slider BGMSlider;      // BGM�̉��ʒ����X���C�_�[
    [SerializeField] private Slider SESlider;       // SE�̉��ʒ����X���C�_�[
    [SerializeField] private AudioSource sampleSE;  // �T���v��SE��AudioSource

    private void Start()
    {
        // �X���C�_�[�̏����l���I�[�f�B�I�~�L�T�[����擾
        InitializeSlider(BGMSlider, "BGM");
        InitializeSlider(SESlider, "SE");

        // �X���C�_�[�̒l�ύX���ɉ��ʂ�K�p
        BGMSlider.onValueChanged.AddListener(SetBGM);
        SESlider.onValueChanged.AddListener(SetSE);
    }

    /// <summary>
    /// �w�肵���X���C�_�[�ɃI�[�f�B�I�~�L�T�[�̒l��K�p
    /// </summary>
    private void InitializeSlider(Slider slider, string parameterName)
    {
        if (audioMixer.GetFloat(parameterName, out float volume))
        {
            slider.value = volume;
        }
    }

    /// <summary>
    /// BGM�̉��ʂ�ύX
    /// </summary>
    public void SetBGM(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }

    /// <summary>
    /// SE�̉��ʂ�ύX���A�T���v��SE���Đ�
    /// </summary>
    public void SetSE(float volume)
    {
        audioMixer.SetFloat("SE", volume);
        PlaySampleSE();
    }

    /// <summary>
    /// �T���v��SE���Đ�
    /// </summary>
    private void PlaySampleSE()
    {
        if (sampleSE != null)
        {
            sampleSE.Play();
        }
    }
}
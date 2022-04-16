using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioSource sfxSource;
    public AudioClip popClip;
    public AudioClip punchClip;
    public AudioClip kickClip;
    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", .5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", .5f);
    }
    public void SetMusicLevel()
    {
        float sliderValue = musicSlider.value;
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVol", sliderValue);
    }
    public void SetSFXLevel()
    {
        float sliderValue = sfxSlider.value;
        mixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVol", sliderValue);
    }
    public void PlayPopSound()
    {
        sfxSource.PlayOneShot(popClip, PlayerPrefs.GetFloat("SFXVol"));
    }
    public void PunchSound()
    {
        sfxSource.PlayOneShot(punchClip, PlayerPrefs.GetFloat("SFXVol"));
    }
    public void KickSound()
    {
        sfxSource.PlayOneShot(kickClip, PlayerPrefs.GetFloat("SFXVol"));
    }
}
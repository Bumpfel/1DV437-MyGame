using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour {

    public AudioMixer m_Mixer;

    public void SetMasterLevel(float sliderValue) {
        m_Mixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void SetSFXLevel(float sliderValue) {
        m_Mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void SetMusicLevel(float sliderValue) {
        m_Mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
    }

}

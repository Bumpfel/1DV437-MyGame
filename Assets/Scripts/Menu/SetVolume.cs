using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour {

    public AudioMixer m_Mixer;

    void Start() {
        if(name == "GameController") {
            AudioMixerGroup[] mixerGroups = m_Mixer.FindMatchingGroups("");
            foreach(AudioMixerGroup mixerGroup in mixerGroups) {
                string exposedParamName = mixerGroup.name + "Volume";
                float savedValue = PlayerPrefs.GetFloat(exposedParamName, 999);
                if(savedValue != 999)
                    SetMixerGroupVolume(exposedParamName, savedValue);
            }
        }
        else {
            float savedValue = PlayerPrefs.GetFloat(name, -1);
            if(savedValue > -1) {
                SetMixerGroupVolume(name, savedValue);
                gameObject.GetComponent<Slider>().value = savedValue;
            }
        }
 
    }

    // void OnDisable() {
    //     PlayerPrefs.Save();
    // }

    // helped method for UI sliders
    public void AdjustVolume(float sliderValue) {
        SetMixerGroupVolume(name, sliderValue);
        PlayerPrefs.SetFloat(name, sliderValue);
        PlayerPrefs.Save();
    }

    private void SetMixerGroupVolume(string exposedParam, float value) {
        m_Mixer.SetFloat(exposedParam, Mathf.Log10(value) * 20);
    }
}

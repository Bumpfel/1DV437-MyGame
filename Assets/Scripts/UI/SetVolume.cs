using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour {

    public AudioMixer m_Mixer;

    void Start() {
        if(name == "GameController") { // load saved settings and set volume for all groups on startup
            AudioMixerGroup[] mixerGroups = m_Mixer.FindMatchingGroups("");
            foreach(AudioMixerGroup mixerGroup in mixerGroups) {
                string exposedParamName = mixerGroup.name + "Volume";
                float savedValue = PlayerPrefs.GetFloat(exposedParamName, 999);
                if(savedValue != 999)
                    SetMixerGroupVolume(exposedParamName, savedValue);
            }
        }
        else { // attached to a volume group (channel) in the options menu. load saved settings and set slider value
            float savedValue = PlayerPrefs.GetFloat(name, -1);
            if(savedValue > -1) {
                SetMixerGroupVolume(name, savedValue);
                gameObject.GetComponent<Slider>().value = savedValue;
            }
        }
 
    }

    // helper method for UI sliders
    public void AdjustVolume(float sliderValue) {
        SetMixerGroupVolume(name, sliderValue);
        PlayerPrefs.SetFloat(name, sliderValue);
        PlayerPrefs.Save();
    }

    private void SetMixerGroupVolume(string exposedParam, float value) {
        m_Mixer.SetFloat(exposedParam, Mathf.Log10(value) * 20);
    }
}

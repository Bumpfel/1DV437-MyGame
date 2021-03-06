using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SetGraphics : MonoBehaviour {

    private GameController m_GameController;

    void Start() {
        m_GameController = FindObjectOfType<GameController>();
        int simpleImpacts = PlayerPrefs.GetInt(Settings.BulletImpactEffects.ToString());
        Toggle toggle = gameObject.GetComponent<Toggle>();
        if(simpleImpacts == 1)
            toggle.isOn = true;
        else
            toggle.isOn = false;
    }

    public void SetSimpleImpactEffects(bool enabled) {
        PlayerPrefs.SetInt(Settings.BulletImpactEffects.ToString(), enabled ? 1 : 0);
        PlayerPrefs.Save();
        m_GameController.SetImpactEffects(enabled ? 1 : 0);
    }
}
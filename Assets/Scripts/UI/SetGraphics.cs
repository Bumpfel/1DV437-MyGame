using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SetGraphics : MonoBehaviour {

    private GameController m_GameController;

    void Start() {
        m_GameController = FindObjectOfType<GameController>();
        int impactOn = PlayerPrefs.GetInt(Strings.Settings.BulletImpactEffects.ToString());
        Toggle toggle = gameObject.GetComponent<Toggle>();
        if(impactOn == 1)
            toggle.isOn = true;
        else
            toggle.isOn = false;
    }

    public void SetImpactEffects(bool enabled) {
        PlayerPrefs.SetInt(Strings.Settings.BulletImpactEffects.ToString(), enabled ? 1 : 0);
        PlayerPrefs.Save();
        m_GameController.SetImpactEffects(enabled);
    }
}
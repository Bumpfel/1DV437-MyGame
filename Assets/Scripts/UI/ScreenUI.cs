using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScreenUI : MonoBehaviour {

    private TextMeshProUGUI m_BigText;
    private TextMeshProUGUI m_Messages;

    private float m_BigTextDuration = 1;
    private float m_BigTextFadeDuration = 2;
    private float m_MessageDuration = 2;
    private float m_MessageFadeDuration = 1;
    private bool m_IsFading = false;

    void Start() {
        m_BigText = transform.Find("BigText").GetComponentInChildren<TextMeshProUGUI>(true);
        ShowLevelText();

        m_Messages = transform.Find("Message").GetComponentInChildren<TextMeshProUGUI>(true);
        m_Messages.gameObject.SetActive(false);
    }

    public void ShowLevelText() {
        m_BigText.gameObject.SetActive(true);
        m_BigText.SetText(SceneManager.GetActiveScene().name);
        StartCoroutine(Fade(m_BigText, m_BigTextDuration, m_BigTextFadeDuration,  true));
    }

    public void ShowMessage(string msg) {
        if(!m_IsFading) {
            m_Messages.gameObject.SetActive(true);
            m_Messages.SetText(msg);
            StartCoroutine(Fade(m_Messages, m_MessageDuration, m_MessageFadeDuration,  false));
        }
    }

    private IEnumerator Fade(TextMeshProUGUI text, float showTextDuration, float fadeDuration, bool ignoreTimeScale) {
        m_IsFading = true;
        yield return new WaitForSeconds(showTextDuration);
        text.CrossFadeAlpha(0, fadeDuration, ignoreTimeScale);
        yield return new WaitForSeconds(fadeDuration);
        text.gameObject.SetActive(false);
        m_IsFading = false;
    }

}
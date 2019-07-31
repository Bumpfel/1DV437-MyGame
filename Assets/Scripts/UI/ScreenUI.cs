using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum Field { BigText, Message };

public class ScreenUI : MonoBehaviour {

    private TextMeshProUGUI m_BigText;
    private TextMeshProUGUI m_Messages;
      private float m_BigTextDuration = 1;
    private float m_BigTextFadeDuration = 2;
    private float m_MessageDuration = 2;
    private float m_MessageFadeDuration = 1;
    private bool m_IsFading = false;
    private Coroutine m_ActiveRoutine;

    void Start() {
        m_BigText = transform.Find("BigText").GetComponent<TextMeshProUGUI>();
       ShowLevelText();

        m_Messages = transform.Find("Message").GetComponent<TextMeshProUGUI>();
        m_Messages.gameObject.SetActive(false);
    }

    // public void SetBigText(bool fadeOut) {
    //     m_BigText.gameObject.SetActive(true);
    //     m_BigText.SetText(SceneManager.GetActiveScene().name);
    //     if(fadeOut) {
    //         m_Background.SetActive(false);
    //         m_ActiveRoutine = StartCoroutine(Fade(m_BigText, m_BigTextDuration, m_BigTextFadeDuration,  true));
    //     }
    //     else
    //         m_Background.SetActive(true);
    // }

    public void ShowLevelText() {
        SetScreenMessage(Field.BigText, SceneManager.GetActiveScene().name.ToString(), true);
    }

    public void SetScreenMessage(Field field, string text, bool fadeOut) {
        Settings settings;
        if(field == Field.BigText)
            settings =  new Settings(m_BigTextDuration, m_BigTextFadeDuration, m_BigText);
        else
            settings =  new Settings(m_BigTextDuration, m_BigTextFadeDuration, m_Messages);

        settings.textField.gameObject.SetActive(true);
        settings.textField.SetText(text);
        if(fadeOut) {
            m_ActiveRoutine = StartCoroutine(Fade(settings.textField, settings.duration, settings.fadeDuration,  false));
        }
    }

    private IEnumerator Fade(TextMeshProUGUI text, float showTextDuration, float fadeDuration, bool ignoreTimeScale) {
        yield return new WaitForSeconds(showTextDuration);
        text.CrossFadeAlpha(0, fadeDuration, ignoreTimeScale);
        yield return new WaitForSeconds(fadeDuration);
        text.gameObject.SetActive(false);
        m_ActiveRoutine = null;
    }

    private struct Settings {
        public float duration;
        public float fadeDuration;
        public TextMeshProUGUI textField;

        public Settings(float _duration, float _fadeDuration, TextMeshProUGUI _textField) {
            duration = _duration;
            fadeDuration = _fadeDuration;
            textField = _textField;
        }
    }

}
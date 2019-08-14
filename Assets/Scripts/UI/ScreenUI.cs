using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum Field { BigText, Message };

public class ScreenUI : MonoBehaviour {

    private static TextMeshProUGUI m_BigText;
    private static TextMeshProUGUI m_MessageTemplate;
    private static Coroutine m_ActiveRoutine;
    private static float BigTextDuration = 1;
    private static float BigTextFadeDuration = 2;
    private static float MessageDuration = 2;
    private static float MessageFadeDuration = 1;
    private static MonoBehaviour monobehaviour = null;

    void Start() {
        ShowLevelText();
    }

    void OnEnable() {
        m_BigText = transform.Find("BigText").GetComponent<TextMeshProUGUI>();
        m_BigText.gameObject.SetActive(false);
        m_BigText.SetText("");

        m_MessageTemplate = transform.Find("Messages").Find("Message").GetComponent<TextMeshProUGUI>();
        m_MessageTemplate.gameObject.SetActive(false);
        m_MessageTemplate.SetText("");
    }

    public void ShowLevelText() {
        DisplayMessage(SceneManager.GetActiveScene().name.ToString(), Field.BigText);
    }

    public static void DisplayMessage(string text, Field field = Field.Message) {
        Settings settings;
        if(field == Field.BigText) {
            settings = new Settings(BigTextDuration, BigTextFadeDuration, m_BigText);
            monobehaviour = m_BigText;
        }
        else {
            TextMeshProUGUI newMessage = Instantiate(m_MessageTemplate);
            newMessage.transform.parent = m_MessageTemplate.transform.parent;
            newMessage.transform.SetAsFirstSibling();
            settings = new Settings(MessageDuration, MessageFadeDuration, newMessage);

            monobehaviour = newMessage;
        }

        settings.textField.gameObject.SetActive(true);
        settings.textField.SetText(text);

        if(m_ActiveRoutine != null && field == Field.BigText)
            monobehaviour.StopCoroutine(m_ActiveRoutine);
        monobehaviour.StartCoroutine(Fade(field, settings, false));
    }

    private static IEnumerator Fade(Field field, Settings settings, bool ignoreTimeScale) {
        settings.textField.CrossFadeAlpha(1, 0, true);
        yield return new WaitForSeconds(settings.showDuration);
        settings.textField.CrossFadeAlpha(0, settings.fadeDuration, ignoreTimeScale);
        yield return new WaitForSeconds(settings.fadeDuration);
        if(field == Field.BigText) {
            settings.textField.gameObject.SetActive(false);
            settings.textField.SetText("");
        }
        else {
            Destroy(settings.textField.gameObject);
        }
        m_ActiveRoutine = null;
    }

    private struct Settings {
        public float showDuration;
        public float fadeDuration;
        public TextMeshProUGUI textField;

        public Settings(float _duration, float _fadeDuration, TextMeshProUGUI _textField) {
            showDuration = _duration;
            fadeDuration = _fadeDuration;
            textField = _textField;
        }
    }

}
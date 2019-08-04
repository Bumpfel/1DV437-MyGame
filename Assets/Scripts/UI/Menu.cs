using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour {

    private GameController m_GameController;
    private GameObject m_MainMenu;
    private List<GameObject> m_Submenus = new List<GameObject>();
    private GameObject m_GameOverMenu;
    private GameObject m_Credits;

    void Start() {
        // print("Menu Start()");
        m_GameController = GetComponentInParent<GameController>();
        if(!m_GameController.LevelIsLoaded()) {
            Initialize();
        }
    }

    public void Initialize() { // Used by GameController to load the script since it won't load automatically as the menu is hidden when loading a level
        // print("Menu Initialize()");
        m_GameController = GetComponentInParent<GameController>();

        // Makes sure all menus are hidden and stores references to them in order to easily manipulate their visibility
        GameObject child;
        for(int i = 0; i < transform.childCount; i ++) {
            child = transform.GetChild(i).gameObject;
            
            if(child.tag != "Background" && m_GameController.LevelIsLoaded()) {
                child.SetActive(false);
            }
            if(child.tag == "PauseMenu") {
                if(child.name == "MainMenu") {
                    m_MainMenu = child;
                }
                else
                    m_Submenus.Add(transform.GetChild(i).gameObject);
            }
            else if(child.tag == "GameOverMenu")
                m_GameOverMenu = child;
            else if(child.tag == "Credits")
                m_Credits = child;

        }

        // Hides menu and switches out play button for a resume button if game is started
        if(m_GameController.LevelIsLoaded()) {
            gameObject.SetActive(false);
            m_MainMenu.transform.Find("PlayButton").gameObject.SetActive(false);
            m_MainMenu.transform.Find("ResumeButton").gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Toggles menu if main menu is active; sets main menu as active and all submenus as inactive otherwise. Returns true if menu is toggled
    /// </summary>
    public bool ToggleMenu() {
        if((!gameObject.activeSelf || m_MainMenu.activeSelf) && m_GameController.LevelIsLoaded()) {
            // activate menu and main menu
            gameObject.SetActive(!gameObject.activeSelf);
            m_MainMenu.SetActive(!m_MainMenu.activeSelf);
            return true;
        }
        else {
            //deactivate sub menu, activate main menu
            foreach(GameObject subMenu in m_Submenus) {
                subMenu.SetActive(false);
            }
            m_MainMenu.SetActive(true);
            return false;
        }
    }

    public void ShowGameOverMenu() {
        gameObject.SetActive(true);
        m_GameOverMenu.gameObject.SetActive(true);
        Cursor.visible = true;

        GameObject restartButton = m_GameOverMenu.transform.Find("RespawnButton").gameObject;

        StartCoroutine(FadeInButton(restartButton, 1));
    }

    public void ShowCredits() {
        gameObject.SetActive(true);
        m_Credits.SetActive(true);
        Cursor.visible = true;

        GameObject mainMenuButton = m_Credits.transform.Find("MainMenuButton").gameObject;
        mainMenuButton.GetComponent<Button>().onClick.AddListener(() => m_GameController.LoadMainMenu());
        StartCoroutine(FadeInButton(mainMenuButton, 2));
    }


    private IEnumerator FadeInButton(GameObject buttonObject, float fadeDuration) {
        Button button = buttonObject.GetComponent<Button>();
        Image image = buttonObject.GetComponent<Image>();
        image.enabled = false;

        button.interactable = false;
        TextMeshProUGUI buttonText = buttonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        buttonText.canvasRenderer.SetAlpha(0);

        buttonText.CrossFadeAlpha(1, fadeDuration, false);
        yield return new WaitForSeconds(fadeDuration);

        image.enabled = true;
        button.interactable = true;
    }

    public void PlayButton() {
        m_GameController.StartGame();
    }

    public void ResumeButton() {
        m_GameController.TogglePauseMenu();
    }

    public void RespawnButton() {
        m_GameController.RestartLevel();
        // m_GameController.Respawn();
    }

    public void RestartButton() {
        m_GameController.RestartLevel();
    }

    public void QuitButton() {
        m_GameController.QuitGame();
    }

}
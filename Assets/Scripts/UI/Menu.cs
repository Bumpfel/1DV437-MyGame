using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour {

    private GameController m_GameController;
    private GameObject m_MainMenu;
    private List<GameObject> m_Submenus = new List<GameObject>();
    private GameObject m_LevelEndedMenu;
    private GameObject m_GameOverMenu;
    private GameObject m_Credits;

    void Start() {
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
            
            if(child.tag != "Background") {
                child.SetActive(false);
            }
            if(child.tag == "PauseMenu") {
                if(child.name == "MainMenu") {
                    m_MainMenu = child;
                }
                else
                    m_Submenus.Add(transform.GetChild(i).gameObject);
            }
            else if(child.tag == "LevelEndedMenu")
                m_LevelEndedMenu = child;
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
            m_MainMenu.transform.Find("RestartButton").gameObject.SetActive(true);
            m_MainMenu.transform.Find("QuitGameButton").gameObject.SetActive(false);
            m_MainMenu.transform.Find("QuitToMainMenuButton").gameObject.SetActive(true);
        }
        else { // on start menu
            gameObject.SetActive(true);
            m_MainMenu.SetActive(true);
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

    public void ShowGameOverMenu(PlayerStats playerStats) {
        DoCommonGameOverTasks(m_GameOverMenu, playerStats, false);

        GameObject restartButton = m_GameOverMenu.transform.Find("RespawnButton").gameObject;
        StartCoroutine(FadeInButton(restartButton, 1));
    }
    public void ShowLevelEndedMenu(PlayerStats playerStats, bool wasHighScore) {
        DoCommonGameOverTasks(m_LevelEndedMenu, playerStats, wasHighScore);
    }
    public void ShowCredits(PlayerStats playerStats, bool wasHighScore) {
        DoCommonGameOverTasks(m_Credits, playerStats, wasHighScore);
        
        GameObject mainMenuButton = m_Credits.transform.Find("MainMenuButton").gameObject;
        mainMenuButton.GetComponent<Button>().onClick.AddListener(() => m_GameController.LoadMainMenu());
        StartCoroutine(FadeInButton(mainMenuButton, 2));
    }

    private void DoCommonGameOverTasks(GameObject obj, PlayerStats playerStats, bool wasHighScore) {
        gameObject.SetActive(true);
        obj.SetActive(true);
        Cursor.visible = true;
        TextMeshProUGUI text = obj.transform.Find("Stats").GetComponent<TextMeshProUGUI>();

        StringBuilder msg = new StringBuilder(playerStats.ToString());
        if(playerStats.FinishedLevel) {
            if(wasHighScore) {
                msg.Insert(0, "You set a new high score!\n");
                if(SaveSystem.OldHighScore != null)
                    msg.Append("\n\nThe old high score was " + SaveSystem.OldHighScore.GetFormattedTimeTaken() + " minutes");
            }
        }
        text.SetText(msg.ToString());
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

    public void StartLevel(int index) {
        m_GameController.LoadLevel(index);
    }

    public void ResumeButton() {
        m_GameController.TogglePauseMenu();
    }

    public void RespawnButton() {
        m_GameController.RestartLevel();
    }

    public void RestartButton() {
        m_GameController.RestartLevel();
    }

    public void QuitGameButton() {
        m_GameController.QuitGame();
    }

    public void QuitToMainMenuButton() {
        m_GameController.LoadMainMenu();
    }

    public void NextLevelButton() {
        m_GameController.LoadNextLevel();
    }

}
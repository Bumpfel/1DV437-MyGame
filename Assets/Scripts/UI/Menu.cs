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

    public void Initialize() {
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
            else if(child.tag == "GameOverMenu")
                m_GameOverMenu = child;
            else if(child.tag == "Credits")
                m_Credits = child;

        }
        gameObject.SetActive(false);

        // Hides play button and shows resume button if game is started
        if(SceneManager.GetActiveScene().buildIndex > 0) {
            m_MainMenu.transform.Find("PlayButton").gameObject.SetActive(false);
            m_MainMenu.transform.Find("ResumeButton").gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Toggles menu if main menu is active; sets main menu as active and all submenus as inactive otherwise. Returns true if menu is toggled
    /// </summary>
    public bool ToggleMenu() {
        if(!gameObject.activeSelf || m_MainMenu.activeSelf) {
            gameObject.SetActive(!gameObject.activeSelf);
            m_MainMenu.SetActive(!m_MainMenu.activeSelf);
            return true;
        }
        else {
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

        StartCoroutine(FadeInButton(restartButton));
        // StartCoroutine(FadeInButton(restartButton.GetComponent<Button>()));
    }

    public void ShowCredits() {
        gameObject.SetActive(true);
        m_Credits.gameObject.SetActive(true);
        Cursor.visible = true;

        GameObject mainMenuButton = m_Credits.transform.Find("MainMenuButton").gameObject;
        StartCoroutine(FadeInButton(mainMenuButton));
    }



    private IEnumerator FadeInButton(GameObject button) {
        button.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        button.SetActive(true);


    //     button.interactable = false;
    //     TextMeshProUGUI buttonText = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    //     buttonText.alpha = 0;
    //     print("fading button...");
    //     while(buttonText.alpha < 1) {
    //         buttonText.CrossFadeAlpha(1, 2f, false);
    //         yield return new WaitForEndOfFrame();
    //     }
    //     button.interactable = true;
    //     print("done!");

    //     // button.colors = Color.Lerp(Color.);
    //     yield break;

    }

    public void PlayButton() {
        m_GameController.StartGame();
    }

    public void ResumeButton() {
        m_GameController.TogglePause();
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
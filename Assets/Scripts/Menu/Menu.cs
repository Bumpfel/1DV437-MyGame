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

    void Start() {
        // Hides play button and shows resume button if game is started
        if(SceneManager.GetActiveScene().buildIndex > 0) {
            foreach(Button button in GetComponentsInChildren<Button>(true)) {
                if(button.name == "PlayButton") {
                    button.gameObject.SetActive(false);
                }
                else if(button.name == "ResumeButton") {
                    button.gameObject.SetActive(true);
                }
            }
        }
        m_GameController = GetComponentInParent<GameController>();

        // Stores main menu and sub menus to easily manipulate their visibility
        for(int i = 0; i < transform.childCount; i ++) {
            GameObject thisChild = transform.GetChild(i).gameObject;

            if(thisChild.name == "MainMenu") {
                m_MainMenu = thisChild;
                thisChild.SetActive(true);
            }
            else if(thisChild.name != "Background") {
                m_Submenus.Add(transform.GetChild(i).gameObject);
                thisChild.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Toggles menu if on root menu is displayed or navigates backwards otherwise. Returns true if menu is toggled
    /// </summary>
    public bool ToggleMenu() {
        if(m_MainMenu && !m_MainMenu.activeSelf) {
            foreach(GameObject subMenu in m_Submenus) {
                subMenu.SetActive(false);
            }
            m_MainMenu.SetActive(true);
            return false;
        }
        else {
            gameObject.SetActive(!gameObject.activeSelf);
            return true;
        }
    }

    public void PlayGame() {
        SceneManager.LoadScene(1);
    }

    public void QuitGame() {
        print("Quit game");
        Application.Quit();
    }

}
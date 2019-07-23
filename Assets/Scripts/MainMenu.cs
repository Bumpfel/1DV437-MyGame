using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour {

    public GameController m_GameController;

    void Start() {
        // Changes play button to a resume button if game is started
        if(SceneManager.GetActiveScene().buildIndex > 0) {
            foreach(Button button in GetComponentsInChildren<Button>()) {
                if(button.name == "PlayButton") {
                    button.GetComponentInChildren<TextMeshProUGUI>().SetText("Resume Game");
                    Rect rect = button.GetComponent<Rect>();
                    rect.Set(rect.x, rect.y, 350, rect.height);
                }
            }
        }
    }

    public void PlayGame() {
        if(SceneManager.GetActiveScene().buildIndex > 0) {
            m_GameController.TogglePause();
        }
        else
            SceneManager.LoadScene(1);
    }

    public void QuitGame() {
        print("Quit game");
        Application.Quit();
    }

}
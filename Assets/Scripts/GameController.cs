using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : ObservableComponent {

    private GameObject m_Player;

    [HideInInspector]
    public PlayerStats m_PlayerStats;
    private Menu m_Menu;
    private bool m_Paused = false;

    void Start() {
        m_PlayerStats = new PlayerStats("Player", SceneManager.GetActiveScene().buildIndex);
        NotifySubscribers(); // TODO prova med Awake här istället för att se om det garanterar att denna körs före subscribern (Health tror jag det var) ?
        m_Menu = GetComponentInChildren<Menu>(true);
        m_Menu.gameObject.SetActive(false);

        for(int i = 0; i < transform.childCount; i ++) {
            if(transform.GetChild(i).tag == "Player")
                m_Player = transform.GetChild(i).gameObject;
        }
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    void EndLevel() {
        m_PlayerStats.SetLevelEnded();
        SaveSystem.SavePlayerData(m_PlayerStats);
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    private void debugSavePlayerData() { // TODO temp
        if(m_Paused) {
            print("this game stats: " + m_PlayerStats);
            List<PlayerStats> stats = SaveSystem.LoadPlayerData();
            SaveSystem.SavePlayerData(m_PlayerStats);
            // print(stats.ToString());
        }
    }

    public void TogglePause() {
        if(m_Menu.ToggleMenu()) {
            m_Paused = !m_Paused;
            m_Player.GetComponent<PlayerMovement>().m_GamePaused = m_Paused;
            m_Player.GetComponent<PlayerAttack>().m_GamePaused = m_Paused;
            Time.timeScale = m_Paused ? 0 : 1;
        }
        // m_Menu.SetActive(m_Paused);
        
        // debugSavePlayerData();
    }

}
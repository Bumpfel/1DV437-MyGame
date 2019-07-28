using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public GameObject PlayerModel;
    public CameraController m_CameraController;

    [HideInInspector]
    public PlayerStats m_PlayerStats;

    private GameObject m_Player;
    private Transform m_PlayerSpawn;
    private Menu m_Menu;
    private bool m_Paused = false;
    private int m_CurrentSceneIndex;

    private bool m_GameOver = false;

    void Start() {
        Initialize();
    }

    private void Initialize() {
        m_PlayerStats = new PlayerStats("Player", SceneManager.GetActiveScene().buildIndex);
        
        m_Menu = GetComponentInChildren<Menu>(true);
        m_Menu.Initialize();

        m_PlayerSpawn = GetComponentInChildren<Transform>().Find("PlayerSpawn");
        m_Player = Instantiate(PlayerModel, m_PlayerSpawn.position, m_PlayerSpawn.rotation, transform);
        m_CameraController.SetPlayer(m_Player.transform);
        Time.timeScale = 1;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    public void StartGame() {
        SceneManager.LoadScene(1);
    }

    public void RestartLevel() { //Respawn() {
        Destroy(m_Player);
        Initialize();
        m_GameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // public void RestartLevel() {
    //     Respawn();
    // }

    public void QuitGame() {
        Application.Quit();
    }

    public void SetGameOver() {
        m_GameOver = true;
        m_Menu.ShowGameOverMenu();
    }

    public void EndLevel() {
        m_PlayerStats.SetLevelEnded();
        SaveSystem.SaveHighScoreData(m_PlayerStats);
        int nrOfScenes = SceneManager.sceneCount;
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        if(currentScene < nrOfScenes) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else {
            // TODO present game over UI
            print("Game Over!");
        }
    }
    
    private void debugSavePlayerData() { // TODO debug
        if(m_Paused) {
            print("------------------------------");
            // List<PlayerStats> stats = SaveSystem.LoadHighScoreData();
            m_PlayerStats.SetLevelEnded();
            // print("this game stats: " + m_PlayerStats);
            SaveSystem.SaveHighScoreData(m_PlayerStats);
            // print(stats.ToString());
        }
    }

    public void TogglePause() {
        if(!m_GameOver && m_Menu.ToggleMenu()) {
            // Cursor.visible = !Cursor.visible;
            m_Paused = !m_Paused;
            m_Player.GetComponent<PlayerMovement>().m_GamePaused = m_Paused;
            m_Player.GetComponent<PlayerAttack>().m_GamePaused = m_Paused;
            Time.timeScale = m_Paused ? 0 : 1;
        }

        if(!m_Paused) {
            // print("hiding cursor");
            Cursor.visible = false; // TODO not working if you press esc to hide menu
        }
        // debugSavePlayerData();
    }

}
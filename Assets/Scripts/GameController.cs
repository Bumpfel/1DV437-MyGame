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
    private ScreenUI m_ScreenUI;

    // TODO add all controls here
    public readonly string m_ActionKey = "Action_Player1";

    private bool m_Paused = false;
    private int m_CurrentSceneIndex;
    private bool m_GameOver = false;

    // private enum Message { DOOR_LOCKED  };

    void Start() {
        // print("GameController Start()");
        m_Menu = GetComponentInChildren<Menu>(true);
        if(LevelIsLoaded()) {
            Initialize();
        }
    }

    private void Initialize() {
        // print("GameController Initialize()");
        m_PlayerStats = new PlayerStats("Player", SceneManager.GetActiveScene().buildIndex);
        
        // m_Menu = GetComponentInChildren<Menu>(true);
        m_Menu.Initialize(); //sad

        m_ScreenUI = GetComponentInChildren<ScreenUI>(true);
        m_ScreenUI.gameObject.SetActive(true);

        m_PlayerSpawn = GetComponentInChildren<Transform>().Find("PlayerSpawn");
        m_Player = Instantiate(PlayerModel, m_PlayerSpawn.position, m_PlayerSpawn.rotation, transform);
        m_CameraController.SetPlayer(m_Player.transform);
        Time.timeScale = 1;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
        if(Input.GetKeyDown(KeyCode.F9)) { // debug
            EndLevel();
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

    public void LoadMainMenu() {
        SceneManager.LoadScene(0);
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

    public void DisplayMessage(string msg) {
        m_ScreenUI.SetScreenMessage(Field.Message, msg, true);
    }


    public void EndLevel() {
        m_GameOver = true;
        m_PlayerStats.SetLevelEnded();
        SaveSystem.SaveHighScoreData(m_PlayerStats);

        int nrOfScenes = SceneManager.sceneCountInBuildSettings - 1; // exclude menu
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if(currentSceneIndex < nrOfScenes) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            //TODO meny med knapp innan nästa bana laddas
        }
        else {
            m_Player.GetComponent<PlayerMovement>().m_GamePaused = true;
            m_Player.GetComponent<PlayerAttack>().m_GamePaused = true;
            m_Player.layer = 0; // makes enemies ignore the player
            m_Menu.ShowCredits();
            
            // TODO play cheerful music
            // UI animation ? balloons and fireworks
        }
    }

    public bool LevelIsLoaded() {
        return SceneManager.GetActiveScene().buildIndex > 0;
    }

    // public bool IsGameOver() {
    //     return m_GameOver;
    // }
    
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
            if(LevelIsLoaded()) {
                // Cursor.visible = !Cursor.visible;
                m_Paused = !m_Paused;
                m_Player.GetComponent<PlayerMovement>().m_GamePaused = m_Paused;
                m_Player.GetComponent<PlayerAttack>().m_GamePaused = m_Paused;
                Time.timeScale = m_Paused ? 0 : 1;
            }
        }

        if(!m_Paused) {
            // print("hiding cursor");
            Cursor.visible = false; // TODO not working if you press esc to hide menu
        }
        // debugSavePlayerData();
    }

}
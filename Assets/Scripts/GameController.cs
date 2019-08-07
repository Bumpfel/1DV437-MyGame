using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public GameObject PlayerModel;
    public CameraController m_CameraController;

    public AudioClip m_LevelEndedAudio;

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
    
    private Light m_DayLight;

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
        m_Menu.Initialize();

        m_ScreenUI = GetComponentInChildren<ScreenUI>(true);
        m_ScreenUI.gameObject.SetActive(true);

        m_PlayerSpawn = GetComponentInChildren<Transform>().Find("PlayerSpawn");
        m_Player = Instantiate(PlayerModel, m_PlayerSpawn.position, m_PlayerSpawn.rotation, transform);
        m_CameraController.SetPlayer(m_Player.transform);

        Light[] lights = FindObjectsOfType<Light>();
        foreach(Light light in lights) {
            if(light.tag == "DayLight") {
                m_DayLight = light;
            }
        }
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            TogglePauseMenu();
        }
        if(Input.GetKeyDown(KeyCode.F9)) { // debug
            AudioSource.PlayClipAtPoint(m_LevelEndedAudio, Camera.main.transform.position, 1);
            // EndLevel();
        }
        // m_DayLight.intensity = Mathf.Min(Time.timeSinceLevelLoad / 240, 1);
    }

    public void StartGame() {
        SceneManager.LoadScene(2);
    }

    public void RestartLevel() { //Respawn() {
        Destroy(m_Player);
        Initialize();
        m_GameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        m_Paused = false;
        Time.timeScale = 1;
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

            // AudioSource audioSource = GetComponent<AudioSource>();
            // audioSource.Play();
            AudioSource.PlayClipAtPoint(m_LevelEndedAudio, Camera.main.transform.position, 1);
            //TODO meny med knapp innan nästa bana laddas
        }
        else {
            m_Player.GetComponent<PlayerMovement>().m_GamePaused = true;
            m_Player.GetComponent<PlayerAttack>().m_GamePaused = true;
            m_Player.layer = 0; // makes enemies ignore the player
            m_Menu.ShowCredits();
            AudioSource.PlayClipAtPoint(m_LevelEndedAudio, Camera.main.transform.position, 1);

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

    public void TogglePauseMenu() {
        if(!m_GameOver && m_Menu.ToggleMenu()) {
            if(LevelIsLoaded()) {
                // Cursor.visible = !Cursor.visible;
                m_Paused = !m_Paused;
                TogglePlayerControl(m_Paused);
                Time.timeScale = m_Paused ? 0 : 1;
            }
        }

        if(!m_Paused) {
            // print("hiding cursor");
            Cursor.visible = false; // TODO not working if you press esc to hide menu
        }
        // debugSavePlayerData();
    }

    public void TogglePlayerControl(bool disabled) {
        m_Player.GetComponent<PlayerMovement>().m_GamePaused = disabled;
        m_Player.GetComponent<PlayerAttack>().m_GamePaused = disabled;
    }

    public float GetSavedVolume(ExposedMixerGroup mixerGroup) {
        return PlayerPrefs.GetFloat(mixerGroup.ToString());
    }

}
public enum ExposedMixerGroup { MasterVolume, SFXVolume, MusicVolume };
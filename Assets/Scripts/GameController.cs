using System;
using System.Collections.Generic;
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

    private bool m_Paused = false;
    private int m_CurrentSceneIndex;
    private bool m_GameOver = false;


    void Start() {
        m_Menu = GetComponentInChildren<Menu>(true);
        if(LevelIsLoaded()) {
            m_PlayerStats = new PlayerStats("Player", SceneManager.GetActiveScene().buildIndex);
            
            m_Menu.Initialize();

            GetComponentInChildren<ScreenUI>(true).gameObject.SetActive(true);
            ShowLevelText();

            m_PlayerSpawn = GetComponentInChildren<Transform>().Find("PlayerSpawn");
            m_Player = Instantiate(PlayerModel, m_PlayerSpawn.position, m_PlayerSpawn.rotation, transform);
            m_CameraController.SetPlayer(m_Player.transform);

            SetImpactEffects(GetSavedImpactEffects());
        }
    }

    private void ShowLevelText() {
        string levelText = SceneManager.GetActiveScene().name.ToString() + "\n" + Strings.GetLevelName(SceneManager.GetActiveScene().buildIndex);
        ScreenUI.DisplayMessage(levelText, Field.BigText);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            TogglePauseMenu();
        }
        if(Input.GetKeyDown(KeyCode.F9)) {
            EndLevel();
        }
    }

    public void StartNewGame() {
        LoadLevel(1);
    }

    public void LoadNextLevel() {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void LoadLevel(int index) {
        if(index < 0 || index > SceneManager.sceneCountInBuildSettings)
            throw new ArgumentException("Error loading level - level does not exist");
        
        SceneManager.LoadScene(index);
        Time.timeScale = 1;
        Cursor.visible = false;
    }


    public void RestartLevel() {
        m_GameOver = false;
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
        m_Paused = false;
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene(0);
    }

    public void QuitGame() {
            Application.Quit();
    }

    public void SetGameOver() {
        m_GameOver = true;
        DisablePlayerControlsActive();
        m_PlayerStats.SetLevelEnded(false);
        m_Menu.ShowGameOverMenu(m_PlayerStats);
    }

    public void EndLevel() {
        m_Player.GetComponent<CapsuleCollider>().enabled = false;
        // m_Player.layer = 0; // makes enemies ignore the player
        m_GameOver = true;
        DisablePlayerControlsActive();
        m_PlayerStats.SetLevelEnded(true);

        bool isHighScore = SaveSystem.SaveIfNewHighScore(m_PlayerStats);

        AudioSource audio = GetComponent<AudioSource>();
        audio.volume /= 2;

        int nrOfScenes = SceneManager.sceneCountInBuildSettings - 1; // excludes menu
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if(currentSceneIndex < nrOfScenes) {
            m_Menu.ShowLevelEndedMenu(m_PlayerStats, isHighScore);
            AudioSource.PlayClipAtPoint(m_LevelEndedAudio, Camera.main.transform.position, 1);
        }
        else {
            m_Menu.ShowCredits(m_PlayerStats, isHighScore);

            // TODO play cheerful audio clip
            AudioSource.PlayClipAtPoint(m_LevelEndedAudio, Camera.main.transform.position, 1);
        }
    }

    private void DisablePlayerControlsActive() {
        m_Player.GetComponent<PlayerMovement>().enabled = false;
        m_Player.GetComponent<Attack>().enabled = false;
        Camera.main.GetComponent<CameraController>().enabled = false;
    }

    public bool LevelIsLoaded() {
        return SceneManager.GetActiveScene().buildIndex > 0 || SceneManager.GetActiveScene().name == "Test";
    }
    
    public void TogglePauseMenu() {
        if(!m_GameOver && m_Menu.ToggleMenu()) {
            if(LevelIsLoaded()) {
                m_Paused = !m_Paused;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = m_Paused;
                Time.timeScale = m_Paused ? 0 : 1;
            }
        }
        else
            Cursor.lockState = CursorLockMode.None;

    }

    public float GetSavedVolume(ExposedMixerGroup mixerGroup) {
        return PlayerPrefs.GetFloat(mixerGroup.ToString());
    }

    public void SetImpactEffects(int impactEffect) {
        foreach(Attack combatant in FindObjectsOfType<Attack>()) {
            combatant.SetSimpleImpactEffects(impactEffect == 0 ? false : true);
        }
    }

    public int GetSavedImpactEffects() {
        return PlayerPrefs.GetInt(Settings.BulletImpactEffects.ToString());
    }

}
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerStats {

    public int LevelIndex { get; private set; }
    public float TimeTaken { get; private set; }
    public int Kills { get; private set; }
    public int TotalEnemies { get; set; }
    public int PlayerDeaths { get; private set; }
    public string PlayerName { get; private set; }
    private float m_StartTime;

    public bool FinishedLevel { get; private set; }

    public PlayerStats(string playerName, int levelIndex) {
        PlayerName = playerName;
        m_StartTime = Time.time;
        LevelIndex = levelIndex;
    }

    public void SetLevelEnded(bool finished) {
        FinishedLevel = finished;
        TimeTaken = Time.time - m_StartTime;
        TotalEnemies = GetTotalEnemiesInActiveScene();
    }

    public void AddKill() {
        Kills ++;
    }

    public void AddPlayerDeath() {
        PlayerDeaths ++;
    }

    public override string ToString() {
        // return "Your time was " + GetFormattedTimeTaken() + " minutes. You got " + Kills + " of " + TotalEnemies + " possible kills and had " + PlayerDeaths + " deaths";
        return (FinishedLevel ? "Your time was " : "You died after ") + GetFormattedTimeTaken() + " minutes. You got " + Kills + " of " + TotalEnemies + " possible kills";// and had " + PlayerDeaths + " deaths";
    }

    public string GetFormattedTimeTaken() {
        float seconds = Mathf.Floor(TimeTaken % 60);
        return (int) (TimeTaken / 60) + ":" + (seconds < 10f ? "0" : "") + seconds;
    }

    private int GetTotalEnemiesInActiveScene() {        
        foreach(GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
            if(obj.name == "Enemies") {
                return obj.transform.childCount;
            }
        }
        return 0;
    }


}
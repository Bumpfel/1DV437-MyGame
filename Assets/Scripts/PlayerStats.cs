using System;
using UnityEngine;

[Serializable]
public class PlayerStats {

    // (TODO find out how to exlude fields when saving. don't need startTime for instance)
    private int m_LevelIndex;
    private float m_StartTime;
    private float m_TimeTaken;
    private int m_Kills;
    private string m_PlayerName;
    private int m_PlayerDeaths;

    public string PlayerName { get => m_PlayerName; set => m_PlayerName = value; }
    public int PlayerDeaths { get => m_PlayerDeaths; set => m_PlayerDeaths = value; }
    public int Kills { get => m_Kills; set => m_Kills = value; }
    public float TimeTaken { get => m_TimeTaken; set => m_TimeTaken = value; }
    public int LevelIndex { get => m_LevelIndex; set => m_LevelIndex = value; }

    public PlayerStats(string playerName, int levelIndex) {
        PlayerName = playerName;
        m_StartTime = Time.time;
        LevelIndex = levelIndex;
    }

    public void SetLevelEnded() {
        TimeTaken = Time.time - m_StartTime;
    }

    public void AddKill() {
        Kills ++;
        // Debug.Log("added kill: " + m_Kills);
    }

    public int GetKills() {
        return Kills;
    }

    public void AddPlayerDeath() {
        PlayerDeaths ++;
    }

    public int GetPlayerDeaths() {
        return PlayerDeaths;
    }


    public override string ToString() {
        return PlayerName + " has " + Kills + " kills and " + PlayerDeaths + " deaths. " + (TimeTaken > 0 ? "Level " + + LevelIndex + "  was (completed) in " + TimeTaken + " seconds" : "");
    }



}
using System;
using UnityEngine;

[Serializable]
public class PlayerStats {

    // (TODO find out how to exlude fields when saving. don't need startTime for instance)
    public int m_LevelIndex;
    private float m_StartTime;
    private float m_TimeTaken;
    private int m_Kills;
    private string m_PlayerName;
    private int m_PlayerDeaths;

    public PlayerStats(string playerName, int levelIndex) {
        m_PlayerName = playerName;
        m_StartTime = Time.time;
        m_LevelIndex = levelIndex;
    }

    public void SetLevelEnded() {
        m_TimeTaken = Time.time - m_StartTime;
    }

    public void AddKill() {
        m_Kills ++;
        // Debug.Log("added kill: " + m_Kills);
    }

    public int GetKills() {
        return m_Kills;
    }

    public void AddPlayerDeath() {
        m_PlayerDeaths ++;
    }

    public int GetPlayerDeaths() {
        return m_PlayerDeaths;
    }


    public override string ToString() {
        return m_PlayerName + " has " + m_Kills + " kills and " + m_PlayerDeaths + " deaths. " + (m_TimeTaken > 0 ? "Level " + + m_LevelIndex + "  was (completed) in " + m_TimeTaken + " seconds" : "");
    }



}
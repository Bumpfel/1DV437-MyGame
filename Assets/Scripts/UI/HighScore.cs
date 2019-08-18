using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HighScore : MonoBehaviour {

    private List<PlayerStats> m_HighScoreStats;
    private List<GameObject> m_Rows = new List<GameObject>();

    void OnEnable() {
        PopulateHighScoreList();
    }

    void OnDisable() {
        if(m_HighScoreStats != null)
            m_HighScoreStats.Clear();
        m_Rows.ForEach(obj => Destroy(obj));
        m_Rows.Clear();
    }

    public void PopulateHighScoreList() {
        m_HighScoreStats = SaveSystem.GetHighScoreData();
        if(m_HighScoreStats == null) {
            SetStatsListOn(false);
            return;
        }
        SetStatsListOn(true);
        
        m_HighScoreStats.Sort((ps1, ps2) => ps1.LevelIndex.CompareTo(ps2.LevelIndex));

        Transform dataRow = Array.Find<Transform>(GetComponentsInChildren<Transform>(true), obj => obj.name == "Data");

        foreach(PlayerStats playerStats in m_HighScoreStats) {
            List<object> scoreRow = new List<object>();
            // stat.Add(playerStats.PlayerName);
            scoreRow.Add("Level " + playerStats.LevelIndex + " - " + Strings.GetLevelName(playerStats.LevelIndex));
            scoreRow.Add(playerStats.GetFormattedTimeTaken());
            scoreRow.Add(playerStats.Kills + "/" + playerStats.TotalEnemies);
            // scoreRow.Add(playerStats.PlayerDeaths);
           
            Transform row = Instantiate(dataRow, dataRow.parent);
            row.gameObject.SetActive(true);
            m_Rows.Add(row.gameObject);
            
            TextMeshProUGUI[] dataCells = row.GetComponentsInChildren<TextMeshProUGUI>();
            for(int i = 0; i < dataCells.Length; i ++) {
                row.gameObject.name = "Row " + playerStats.LevelIndex;
                dataCells[i].SetText("" + scoreRow[i]);
            }
        }
        dataRow.gameObject.SetActive(false);
    }

    private void SetStatsListOn(bool enabled) {
        transform.Find("Headers").gameObject.SetActive(enabled);
        transform.Find("NoData").gameObject.SetActive(!enabled);
    }
}

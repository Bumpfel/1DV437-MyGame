using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScore : MonoBehaviour {

    private List<PlayerStats> m_StatsList;
    private List<GameObject> m_Rows = new List<GameObject>();

    void Start() {
    }

    void OnEnable() {
        GetHighScoreList();
    }

    void OnDisable() {
        m_StatsList.Clear();
        m_Rows.ForEach(obj => Destroy(obj));
        m_Rows.Clear();
    }

    public void GetHighScoreList() {
        m_StatsList = SaveSystem.LoadHighScoreData();
        if(m_StatsList == null)
            return;
        
        m_StatsList.Sort((ps1, ps2) => ps1.LevelIndex.CompareTo(ps2.LevelIndex));

        Transform dataRow = Array.Find<Transform>(GetComponentsInChildren<Transform>(true), obj => obj.name == "Data");

        foreach(PlayerStats playerStats in m_StatsList) {
            List<object> stat = new List<object>();
            stat.Add(playerStats.LevelIndex);
            // stat.Add(playerStats.PlayerName);
            stat.Add(playerStats.Kills);
            stat.Add(playerStats.PlayerDeaths);

            float seconds = Mathf.Round(playerStats.TimeTaken % 60);
            string formattedTime = (int) (playerStats.TimeTaken / 60) + ":" + (seconds < 10f ? "0" : "") + seconds;
            stat.Add(formattedTime);
            
            Transform row = Instantiate(dataRow, dataRow.parent);
            row.gameObject.SetActive(true);
            m_Rows.Add(row.gameObject);
            
            TextMeshProUGUI[] dataCells = row.GetComponentsInChildren<TextMeshProUGUI>();
            for(int i = 0; i < dataCells.Length; i ++) {
                row.gameObject.name = "Row " + playerStats.LevelIndex;
                dataCells[i].SetText("" + stat[i]);
            }
        }
        dataRow.gameObject.SetActive(false);
    }
}

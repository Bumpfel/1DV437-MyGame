using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScore : MonoBehaviour {

    void Start() {
    }

    void OnEnable() {
        RefreshScore();
    }

    void OnDisable() {
    }

    public void RefreshScore() {
        List<PlayerStats> statsList = SaveSystem.LoadPlayerData();
        string stats = "";
        foreach(PlayerStats playerStats in statsList) {
            stats += playerStats.ToString() + "\n";
        }

        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        // GetComponentInChildren<Scroll
        
        
        // foreach(TextMeshProUGUI text in texts) {
        //     if(text.name == "HS (TMP)") {
        //         text.SetText(stats);
        //     }
        // }
    }
}

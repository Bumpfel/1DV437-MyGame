using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButtons : MonoBehaviour {

    private void OnEnable() {
        SetLevelButtonStates();
    }

    private void SetLevelButtonStates() {
        var highScoreData = SaveSystem.GetHighScoreData();
        if(highScoreData != null) {
            for(int i = 1; i < transform.childCount; i ++) {
                if(highScoreData.Exists(stats => stats.LevelIndex == i)) {
                    Button button = transform.GetChild(i).GetComponent<Button>();
                    button.interactable = true;
                    button.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                }
            }
        }
    }
    private void SetDebugState(int buttonIndex) {
        transform.GetChild(buttonIndex).GetComponent<Button>().interactable = true;
        transform.GetChild(buttonIndex).GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }
}
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem {
    private static string savePath = Application.persistentDataPath + "/HighScoreData.sav";
    private static BinaryFormatter formatter = new BinaryFormatter();
    
    public static void SaveHighScoreData(PlayerStats playerStats) {
        List<PlayerStats> existingHighScore = LoadHighScoreData();
        if(existingHighScore != null) {
            foreach(PlayerStats score in existingHighScore) {
                // Debug.Log(score.TimeTaken);
            }
        }

        PlayerStats currentBest = null;
        if(existingHighScore != null && existingHighScore.Count > 0) {
            currentBest = existingHighScore.Find(stat => stat.LevelIndex == playerStats.LevelIndex);
            // Debug.Log("Current best " + currentBest.TimeTaken);
        }

        if(currentBest == null || playerStats.TimeTaken < currentBest.TimeTaken) {
            List<PlayerStats> newHighScore;
            if(existingHighScore == null)
                newHighScore = new List<PlayerStats>();
            else
                newHighScore = new List<PlayerStats>(existingHighScore);
            newHighScore.Remove(currentBest);
            newHighScore.Add(playerStats);

            SavePlayerData(newHighScore);
            // Debug.LogWarning("new record found. saving");
        }
        else {
            // Debug.Log("old record is better");
        }
    }

    private static void SavePlayerData(List<PlayerStats> playerStats) {
        FileStream stream;
        // if(!File.Exists(savePath)) {
            stream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
        // }
        // else {
        //     stream = new FileStream(savePath, FileMode.Append);
        // }
        formatter.Serialize(stream, playerStats);
        stream.Close();
    }

    public static List<PlayerStats> LoadHighScoreData() {
        if(File.Exists(savePath)) {
            FileStream stream = new FileStream(savePath, FileMode.Open, FileAccess.Read);

            List<PlayerStats> playerStatsList = (List<PlayerStats>) formatter.Deserialize(stream);
            stream.Close();
            return playerStatsList;
        }
        else {
            Debug.LogError("Save file not found");
            return null;
        }
    }
}
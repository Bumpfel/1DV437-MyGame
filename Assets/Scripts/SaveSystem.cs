using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// ref https://www.youtube.com/watch?v=XOjd_qU2Ido
public class SaveSystem {
    private static readonly string HighScoreSavePath = Application.persistentDataPath + "/HighScoreData.sav";
    private static readonly string PlayerTempDataSavePath = Application.persistentDataPath + "/TempPlayerData.sav";
    private static BinaryFormatter formatter = new BinaryFormatter();
    public static PlayerStats OldHighScore { get; private set; }
    
    ///<summary>
    // Returns true if arg was inserted as a new higscore, false otherwise
    ///</summary>
    public static bool SaveIfNewHighScore(PlayerStats playerStats) {
        List<PlayerStats> existingHighScore = SaveSystem.GetHighScoreData();

        PlayerStats currentBest = null;
        if(existingHighScore != null && existingHighScore.Count > 0) {
            currentBest = existingHighScore.Find(stat => stat.LevelIndex == playerStats.LevelIndex);
        }

        if(currentBest == null || playerStats.TimeTaken < currentBest.TimeTaken) {
            List<PlayerStats> newHighScore;
            if(existingHighScore == null)
                newHighScore = new List<PlayerStats>();
            else
                newHighScore = new List<PlayerStats>(existingHighScore);
            OldHighScore = currentBest;
            newHighScore.Remove(currentBest);
            newHighScore.Add(playerStats);

            SaveData(HighScoreSavePath, newHighScore);
            return true;
        }
        return false;
    }

    public static List<PlayerStats> GetHighScoreData() {
        return (List<PlayerStats>) LoadData(HighScoreSavePath);
    }

    public static void SaveTempPlayerData(PlayerStats playerStats) {
        SaveData(PlayerTempDataSavePath, playerStats);
    }

    public static PlayerStats GetTempPlayerData() {
        return (PlayerStats) LoadData(PlayerTempDataSavePath);
    }



    private static void SaveData(string savePath, object data) {
        FileStream stream;
        stream = new FileStream(savePath, FileMode.Create, FileAccess.Write);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    private static object LoadData(string filepath) {
        if(File.Exists(filepath)) {
            FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);

            object data = formatter.Deserialize(stream);
            stream.Close();
            return data;
        }
        return null;
    }

}
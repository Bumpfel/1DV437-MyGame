using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem {
    private static string saveFilePath = "/playerData";
    private static BinaryFormatter formatter = new BinaryFormatter();
    public static void SavePlayerData(object data) {

        string path = Application.persistentDataPath + saveFilePath;
        FileStream stream;
        if(!File.Exists(path)) {
            stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        }
        else {
            stream = new FileStream(path, FileMode.Append);
        }
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static List<PlayerStats> LoadPlayerData() {
        string path = Application.persistentDataPath + saveFilePath;
        
        if(File.Exists(path)) {
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            List<PlayerStats> playerStatsList = new List<PlayerStats>();
            while(stream.Position < stream.Length) {
                playerStatsList.Add((PlayerStats) formatter.Deserialize(stream));
            }
            stream.Close();
            return playerStatsList;
        }
        else {
            Debug.LogError("Save file not found");
            return null;
        }
    }
}
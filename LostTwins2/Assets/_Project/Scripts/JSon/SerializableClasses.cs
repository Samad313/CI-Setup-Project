using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZonesData
{
    public bool ftueCompleted = default;
    public List<Zone> zones_List = default;
}

[System.Serializable]
public class Zone
{
    public int totalLevels = default;
    public List<Level> levels = default;
    public bool isZoneCompleted = default;

    public Zone(int totalLevels)
    {
        //this.levelsUnlocked = levelsUnlocked;
        this.totalLevels = totalLevels;
    }
}

[System.Serializable]
public class Level
{
    public int levelNumber = default;
    public int crystalsCollected = default;
    public int zoneNumber = default;
    public bool isLevelCompleted = default;
    public bool isUnlocked = default;

    //public Level(int crystalsCollected, bool isLevelCompleted, int zoneNumber)
    //{
    //    this.crystalsCollected = crystalsCollected;
    //    this.isLevelCompleted = isLevelCompleted;
    //    this.zoneNumber = zoneNumber;
    //}
}

[System.Serializable]
public class ZonesDataContainer
{
    public List<string> levels = default;
    //public int totalLevels = 0;
    public int zoneUnlockedCondition = 0; // condition => Crystals collected in pervious zone;

    public void Initialize(string filePath, int fileNumber)
    {
        string concatPath = "";
        concatPath = filePath + fileNumber.ToString();
        string[,] rawData = CSVFileReader.GetFileData(concatPath);
        SaveData(rawData);
    }

    public void SaveData(string[,] rawData)
    {
        levels = new List<string>();
        for (int i = 0; i < 1; i++)
        {
            for (int j = 1; j < rawData.GetLength(1) - 1; j++)
            {
                levels.Add(rawData[i, j]);
                //Debug.Log("<color=yellow> Levels In CSV: </color>" + rawData[i, j]);
            }
        }
    }
}

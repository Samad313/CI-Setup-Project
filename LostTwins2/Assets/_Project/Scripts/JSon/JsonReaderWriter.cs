using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public static class JsonReaderWriter
{

    public static ZonesData zonesData = default;

    public static int zonesCount = default; 

    public static void Initialize()
    {
        zonesData = new ZonesData();

        if(PlayerPrefs.HasKey(Constants.Prefs.GameProgressionStates))
        {
            Debug.Log("<color=green> Data exists in player prefs </color>");
            string jsonToLoad = PlayerPrefs.GetString(Constants.Prefs.GameProgressionStates);
            zonesData = JsonConvert.DeserializeObject<ZonesData>(jsonToLoad);
        }

        else
        {
            SetZonesData();
            SaveJsonToString();
        }
    }

    public static void SaveJsonToString()
    {
        var saveToJson = JsonConvert.SerializeObject(zonesData);
        PlayerPrefs.SetString(Constants.Prefs.GameProgressionStates, saveToJson);
        PlayerPrefs.Save();
        Debug.Log("<color=green> Info is saved </color>");
    }

    public static void SetZonesData()
    {
        //zonesData.totalCrystalsCollected = 0;
        //int numberOfZones = GameDataManager.instance.TotalZones;
        zonesData.zones_List = new List<Zone>();

        for(int i = 0; i < zonesCount; i++)
        {
            int levelsInZone = PlayerStateManager.instance.GetLevelsInAZone(i);//GameDataManager.instance.GetLevelsInZone(i);
            Zone newZone = new Zone(levelsInZone);
            InitializeLevels(newZone,levelsInZone,i);
            zonesData.zones_List.Add(newZone);
        }
    }

    public static void InitializeLevels(Zone zone, int levelsInZone, int zoneNumber)
    {
        zone.levels = new List<Level>();

        for(int i = 0; i < levelsInZone; i++)
        {
            Level level = new Level();
            level.levelNumber = i + 1;
            level.zoneNumber = zoneNumber;
            zone.levels.Add(level);

            Debug.LogFormat("<color=green> LevelNumber: {0} , Crystals Collected: {1}", level.levelNumber, level.crystalsCollected);
        }
        Debug.Log("Levels In Zone: " + zone.levels.Count);


    }

    public static void UpdateLevelDetails(Level level)
    {
        Zone zone = zonesData.zones_List[level.zoneNumber];
        Level levelEntry = zone.levels.FirstOrDefault(t => t.levelNumber == level.levelNumber);

        if (levelEntry != null)
        {
            levelEntry.isLevelCompleted = level.isLevelCompleted;
            Debug.LogFormat("<color=cyan> levelCompleted: {0} , CrystalsCollected: {1}", level.isLevelCompleted, level.crystalsCollected);

            if(level.crystalsCollected > levelEntry.crystalsCollected)
            {
                levelEntry.crystalsCollected = level.crystalsCollected;
            }

            SaveJsonToString();
            //evelEntry.isLevelCompleted = level.isLevelCompleted;
        }
    }

    public static void UnlockAllLevels()
    {
        for(int i = 0; i < zonesData.zones_List.Count; i++)
        {
            for(int j = 0; j < zonesData.zones_List[i].levels.Count; j++)
            {
                zonesData.zones_List[i].levels[j].isLevelCompleted = true;
                zonesData.zones_List[i].levels[j].crystalsCollected = 3;
            }
        }

        SaveJsonToString();
    }
    //public static void UpdateCrystalsCollect(Level level)
    //{
    //    //Level level = GameDataManager.instance.ActiveLevel;
    //    Zone zone = zonesData.zones_List[level.zoneNumber];
    //    Level levelEntry = zone.levels.FirstOrDefault(t => t.levelNumber == level.levelNumber);

    //    if (levelEntry != null && levelEntry.crystalsCollected < level.crystalsCollected)
    //    {
    //        levelEntry.crystalsCollected = level.crystalsCollected;
    //        SaveJsonToString();
    //    }
    //}

    //public static void MarkLevelUnlocked(Level level)
    //{
        
    //    zonesData.zones_List[level.zoneNumber].levels[GameDataManager.instance.NewUnlockedLevel].isUnlocked = true;
    //    SaveJsonToString();
    //}

    public static List<Level> GetLevelsInZone(int zoneNumber)
    {
        return zonesData.zones_List[zoneNumber].levels;
    }

    public static int GetUnlockedLevelsCount(int zoneNumber)
    {
        int count = 0;

        for(int i = 0; i < zonesData.zones_List[zoneNumber].levels.Count;i++)
        {
            if(zonesData.zones_List[zoneNumber].levels[i].isUnlocked)
            {
                count++;
            }
        }

        return count;
    }

    public static void UpdateFtueCompleteStatus(bool value)
    {
        zonesData.ftueCompleted = value;
        SaveJsonToString();
    }

    public static bool IsFtueComplete()
    {
        return zonesData.ftueCompleted;
    }

    public static ZonesData GetZonesData()
    {
        return zonesData;
    }

    public static bool ZoneCompletionStatus(int zoneNumber)
    {
        return zonesData.zones_List[zoneNumber].isZoneCompleted;
    }

    public static Zone GetActiveZone(int index)
    {
        return zonesData.zones_List[index];
    }

    public static List<Zone> GetAllZones()
    {
        return zonesData.zones_List;
    }

    public static Level GetActiveLevel(int levelNumber , int zoneNumber)
    {
        Zone zone = zonesData.zones_List[zoneNumber];
        Level levelEntry = zone.levels.FirstOrDefault(t => t.levelNumber == levelNumber);

        if(levelEntry != null)
        {
            return levelEntry;
        }

        return null;
        
    }
}

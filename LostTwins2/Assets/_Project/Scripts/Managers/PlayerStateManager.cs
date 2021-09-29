using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager instance = default;

    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private DebugPanelManager debugPanelManager;
    [SerializeField] private GameDataManager gameDataManager;
    [SerializeField] private float crystalsCollectedPercentage; // defines the percentage of crystals collection at which next zone should be unlocked;
    [SerializeField] private bool autoMoveToNextLevel = default;

    private int totalsCrystalsCollected = default;
    private LevelManager activeLevelManagerInstance;
    private bool unlockAllCheatUsed = default;

    public int TotalCrystalsCollected { get { return totalsCrystalsCollected; } }
    public bool IsFTUEComplete { get { return JsonReaderWriter.IsFtueComplete(); } }
    public LevelManager ActiveLevelManagerInstance { get { return activeLevelManagerInstance; } set { activeLevelManagerInstance = value; } }
    public DebugPanelManager InstanceOfDebugPanelManager {  get { return debugPanelManager; } }
    public bool UnlockAllCheatUsed {  get { return unlockAllCheatUsed; } }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            Initialize();
        }

        if(!IsFTUEComplete && SceneLoader.Instance)
        {
            SceneLoader.Instance.LoadSpecificScene(Constants.Scenes.FTUE_Scene);
            debugPanelManager.Initialize();
            debugPanelManager.ShowOnlyGamePlayDebugItems();
            //if (DebugPanelManager.instance)
            //{
            //    DebugPanelManager.instance.ShowOnlyGamePlayDebugItems();
            //}
        }
    }

    private void Initialize()
    {
        mainMenu.Reset();
        mainMenu.Initialize(this,debugPanelManager);
        JsonReaderWriter.zonesCount = mainMenu.GetNumberOfZones();
        JsonReaderWriter.Initialize();
        CalculateTotalCrystalsCollected();
        mainMenu.SetupScene();
    }

    public void CalculateTotalCrystalsCollected()
    {
        List<Zone> zonesList = JsonReaderWriter.GetAllZones();

        int crystalsCollectedInAZone = 0;

        for (int i = 0; i < zonesList.Count; i++)
        {
            crystalsCollectedInAZone += zonesList[i].levels.Sum(level => level.crystalsCollected);
        }

        totalsCrystalsCollected = crystalsCollectedInAZone;
    }

    public void MarkFTUEAsComplete()
    {
        JsonReaderWriter.UpdateFtueCompleteStatus(true);
        activeLevelManagerInstance = mainMenu.GetNextLevel(0, 0);
        Debug.Log("<color=cyan> FTUE is Marked As Completed </color>");
    }

    public void SaveGameProgress(int crystalsCollected)
    {
        if(!IsFTUEComplete)
        {
            return;
        }

        Level level = JsonReaderWriter.GetActiveLevel(activeLevelManagerInstance.LevelNumber, activeLevelManagerInstance.ZoneNumber);

        if(level != null)
        {
            if(!level.isLevelCompleted)
            {
                level.crystalsCollected = crystalsCollected;
                level.isLevelCompleted = true;
            }

            else if(crystalsCollected > level.crystalsCollected)
            {
                level.crystalsCollected = crystalsCollected;
            }

            CalculateTotalCrystalsCollected();
            activeLevelManagerInstance = mainMenu.GetNextLevel(activeLevelManagerInstance.LevelNumber, activeLevelManagerInstance.ZoneNumber);

            if (activeLevelManagerInstance == null)
            {
                GameController.instance.LoadMenu();
                SceneLoader.Instance.LoadSpecificScene(Constants.Scenes.MainScene);
            }

            JsonReaderWriter.UpdateLevelDetails(level);
            LoadNextScene();
        }

    }

    private void LoadNextScene()
    {
        if(!autoMoveToNextLevel || activeLevelManagerInstance == null)
        {
            SceneLoader.Instance.LoadSpecificScene(Constants.Scenes.MainScene);
        }

        else
        {
            SceneLoader.Instance.LoadSpecificScene(activeLevelManagerInstance.SceneName);
            DebugPanelManager.instance.SetActiveLevelDebugText(activeLevelManagerInstance.ZoneNumber + 1, activeLevelManagerInstance.LevelNumber);
        }
    }

    public string[] GetScenesFromCSV(int fileNumber)
    {
        return gameDataManager.CSVInitializer(fileNumber);
    }

    public int GetZoneUnlockCondition(int zoneNumber, ZoneManager[] zoneManagers)
    {
        if( (zoneNumber - 1) < 0)
        {
            return 0;
        }

        int condition = 0;

        for(int i = 1; i <= zoneNumber; i++)
        {
            condition += Mathf.CeilToInt((zoneManagers[zoneNumber - 1].LevelScenes.Length * 3) * crystalsCollectedPercentage);
        }
        
        return condition;
    }

    public List<Level> GetLevelsInZoneFromJson(int zoneNumber)
    {
        return JsonReaderWriter.GetLevelsInZone(zoneNumber);
    }

    public int GetLevelsInAZone(int zoneNumber)
    {
        return mainMenu.GetLevelsInAZone(zoneNumber);
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        Initialize();

        if(SceneLoader.Instance)
        {
            SceneLoader.Instance.LoadSpecificScene(Constants.Scenes.FTUE_Scene);
        }
    }

    public void UnlockAllLevels()
    {
        if(!unlockAllCheatUsed)
        {
            unlockAllCheatUsed = true;
            JsonReaderWriter.UnlockAllLevels();
            CalculateTotalCrystalsCollected();
            mainMenu.UpdateValues();
            debugPanelManager.UpdateCollectablesCount(totalsCrystalsCollected);
        }


    }

    [ContextMenu("ClearPlayerPrefs")]
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineHelperFunctions;
using UnityEngine.UI;

public class ZoneManager : MonoBehaviour
{

    private MainMenu mainMenu;
    private PathManager pathManager;
    //private LevelManager[] levelManagers;
    private List<LevelManager> levelManagersList = default;
    private List<Level> savedLevels = default;
    private LevelManager previousSelected = default;
    private string[] levelScenes = default;
    private int zoneUnlockedCondition = 0; // condition => Crystals collected in pervious zone;
    private int levelsCompleted = default, unlockedLevels = default;


    public string[] LevelScenes { get { return levelScenes; } }

    //[SerializeField] private string[] scenes;
    [SerializeField] private int zoneNumber = 0;
    [SerializeField] private Transform selectedLevelMarker;
    [SerializeField] private float markerYOffset = default;
    [SerializeField] private StarsUI starsUI;
    [SerializeField] private GameObject levelPrefab;


    public MainMenu MainMenu { get { return mainMenu; } }

    public void Init(MainMenu instance)
    {
        mainMenu = instance;
        pathManager = this.gameObject.GetComponentInChildren<PathManager>();
        //levelManagers = pathManager.transform.GetComponentsInChildren<LevelManager>();
        levelScenes = mainMenu.GetScenesFromCSV(zoneNumber + 1);
        pathManager.InIt(this);
        zoneUnlockedCondition = mainMenu.GetZoneUnlockCondition(zoneNumber);
        Debug.Log("<color=green> Zone Unlock Condition: </color>" + zoneUnlockedCondition);
        //PopulateLevels();
    }

    public void PopulateLevels()
    {
        Vector3[] points = pathManager.Points;
        GameObject newLevel = default;
        savedLevels = mainMenu.GetLevelsFromJson(zoneNumber);
        levelManagersList = new List<LevelManager>();

        for (int i = 0; i < levelScenes.Length; i++)
        {
            newLevel = Instantiate(levelPrefab, pathManager.transform);
            LevelManager levelManager = newLevel.GetComponent<LevelManager>();
            newLevel.transform.localPosition = points[i + 1];
            newLevel.name = LevelScenes[i];
            levelManager.LevelNumber = savedLevels[i].levelNumber;
            levelManager.ZoneNumber = zoneNumber;
            levelManager.ZoneManager = this;
            levelManager.SceneName = levelScenes[i];
            UpdateCrystalsCollectedCount(levelManager, savedLevels[i].crystalsCollected);
            levelManagersList.Add(levelManager);
        }
    }

    public LevelManager GetNextLevel(int levelNumber)
    {
        if(levelNumber >= levelManagersList.Count)
        {
            return null;
        }

        return levelManagersList[levelNumber];
    }

    public void UpdateCrystalsCollectedCount(LevelManager levelManager, int crystalsCount)
    {
        levelManager.CrystalsCollected = crystalsCount;
    }

    public void UpdateZoneLevelsState()
    {
        savedLevels = mainMenu.GetLevelsFromJson(zoneNumber);
        unlockedLevels = 0;
        levelsCompleted = 0;

        if (zoneUnlockedCondition >= mainMenu.TotalCrystalCollected && zoneNumber > 0)
        {
            Debug.LogFormat("<color=green> ZoneUnlockCondition: {0}, ZoneNumber: {1}</color>", zoneUnlockedCondition, zoneNumber);
            selectedLevelMarker.gameObject.SetActive(false);
            return;
        }

        selectedLevelMarker.gameObject.SetActive(true);

        for (int i = 0; i < levelManagersList.Count; i++)
        {
            UpdateCrystalsCollectedCount(levelManagersList[i], savedLevels[i].crystalsCollected);

            if (savedLevels[i].isLevelCompleted || i == 0)
            {
                levelManagersList[i].IsLevelPlayable = true;
                levelManagersList[i].DisableLockIcon();
                levelsCompleted++;
            }

            if ((i - 1) >= 0 && savedLevels[i - 1].isLevelCompleted)
            {
                levelManagersList[i].IsLevelPlayable = true;
                levelManagersList[i].DisableLockIcon();
                unlockedLevels++;
            }
        }

        unlockedLevels = Mathf.Clamp(unlockedLevels, 0, levelScenes.Length);
        //Debug.Log("<color=green> Unlocked Levels Count: </color>" + unlockedLevels);
        pathManager.UpdateLevelProgression(unlockedLevels);

        UpdateLevelSelectionMarkerPosition(levelManagersList[unlockedLevels].transform.position, levelManagersList[unlockedLevels]);
        UpdateCrystalsFillCount(levelManagersList[unlockedLevels].CrystalsCollected);

        //if(unlockedLevels > 0)
        //{
        //    UpdateLevelSelectionMarkerPosition(levelManagersList[unlockedLevels].transform.localPosition, levelManagersList[unlockedLevels]);
        //    UpdateCrystalsFillCount(levelManagersList[unlockedLevels].CrystalsCollected);
        //}

        //else
        //{
        //    UpdateLevelSelectionMarkerPosition(levelManagersList[unlockedLevels].transform.localPosition, levelManagersList[unlockedLevels]);
        //    UpdateCrystalsFillCount(levelManagersList[unlockedLevels].CrystalsCollected);
        //}

    }

    //private void InitializeZoneLevels()
    //{
    //    if(zoneUnlockedCondition <= mainMenu.TotalCrystalCollected)
    //    {
    //        return;
    //    }

    //    levelsCompleted = 0;
    //    Debug.Log("<color=yellow> ZoneManager Level initializer is called </color>");
    //    savedLevels = GameDataManager.instance.GetLevels(zoneNumber);
    //    //levelManagers = pathManager.transform.GetComponentsInChildren<LevelManager>();
    //    bool zoneIsFinished = GameDataManager.instance.GetZoneCompletionStatus(zoneNumber);

    //    Debug.Log("Levels Count: " + savedLevels.Count);

    //    for (int i = 0; i < savedLevels.Count; i++)
    //    {
    //        levelManagers[i].LevelNumber = savedLevels[i].levelNumber;
    //        levelManagers[i].ZoneNumber = savedLevels[i].zoneNumber;
    //        levelManagers[i].CrystalsCollected = savedLevels[i].crystalsCollected;
    //        levelManagers[i].ZoneManager = this;

    //        if (GameDataManager.instance)
    //        {
    //            levelManagers[i].SceneName = GameDataManager.instance.GetLevelSceneName(i, zoneNumber);
    //            Debug.Log("<color= cyan> Level Names: </color>" + levelManagers[i].SceneName);
    //        }

    //        //levelManagers[i].SceneName = GameDataManager.instance != null ? GameDataManager.instance.GetLevelSceneName(i, zoneNumber) : "FC13";
    //        //if (scenes.Length > 0)
    //        //{
    //        //    levelManagers[i].SceneName = scenes[i];
    //        //}

    //        UpdatePlayableLevels(i);
    //    }

    //    if (!zoneIsFinished)
    //    {
    //        pathManager.UpdateLevelProgression(levelsCompleted + 1);
    //    }

    //    if (levelsCompleted >= 10)
    //    {
    //        selectedLevelMarker.gameObject.SetActive(false);
    //    }
    //}

    //private void UpdatePlayableLevels(int index)
    //{
    //    if(savedLevels[index].isLevelCompleted || index == 0)
    //    {
    //        levelManagers[index].IsLevelPlayable = true;
    //    }

    //    if ((index - 1) >= 0 && savedLevels[index - 1].isLevelCompleted)
    //    {
    //        levelManagers[index].IsLevelPlayable = true;
    //        UpdateLevelSelectionMarkerPosition(levelManagers[index].transform.localPosition, levelManagers[index]);
    //        UpdateCrystalsFillCount(levelManagers[index].CrystalsCollected);
    //        levelsCompleted++;
    //    }
    //    else if(index== 0)
    //    {
    //        UpdateLevelSelectionMarkerPosition(levelManagers[index].transform.localPosition, levelManagers[index]);
    //        UpdateCrystalsFillCount(levelManagers[index].CrystalsCollected);
    //    }

    //}

    public void UpdateLevelSelectionMarkerPosition(Vector3 newPosition, LevelManager selected)
    {
        if(previousSelected == null)
        {
            previousSelected = selected;
            previousSelected.TimeSelected++;
        }
        else
        {
            previousSelected.TimeSelected = 0;
            previousSelected = selected;
        }

        //selectedLevelMarker.localPosition = new Vector3(newPosition.x, newPosition.y, selectedLevelMarker.localPosition.z);
        selectedLevelMarker.position = new Vector3(newPosition.x, newPosition.y, selectedLevelMarker.position.z);
        selectedLevelMarker.localPosition = new Vector3(selectedLevelMarker.localPosition.x, selectedLevelMarker.localPosition.y, 0f);
    }

    public void UpdateCrystalsFillCount(int value)
    {
        starsUI.DisableStars(true);
        starsUI.ShowFilledCrystalsOnMainMenu(value);
    }

    public int GetLevelsCount()
    {
        return levelScenes.Length;
    }


    public void Reset()
    {
        levelManagersList.ForEach(obj => obj.Reset());
        pathManager.Reset();

        if (zoneNumber > 0)
        {
            selectedLevelMarker.gameObject.SetActive(false);
        }
    }
}

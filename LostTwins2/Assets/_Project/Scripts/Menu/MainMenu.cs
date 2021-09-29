using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class MainMenu : BaseMenu
{
    private ZoneManager[] zoneManagers;
    private SwipeScroller swipeScroller;
    private DebugPanelManager debugPanelManager;

    [SerializeField] private AudioListener uiCameraAudioListener;

    private PlayerStateManager playerStateManager;


    public int TotalCrystalCollected { get { return playerStateManager.TotalCrystalsCollected; } }

    public PlayerStateManager PlayerStateManager { get { return playerStateManager; } }

    public override void Init()
    {
        base.AdjustElementsAffectedByNotch();
        //If some UI element's final position on screen is needed, get it here before it gets pushed away off screen
        base.Init();
    }

    public void Initialize(PlayerStateManager instance, DebugPanelManager debugInstance)
    {
        playerStateManager = instance;
        debugPanelManager = debugInstance;
        debugPanelManager.Initialize();
        debugPanelManager.ShowAllMainMenuDebugItems();
        zoneManagers = transform.GetComponentsInChildren<ZoneManager>();
        swipeScroller = transform.GetComponentInChildren<SwipeScroller>();
        InitializeChildren();
    }

    public override void Enable(string arguments)
    {
        base.Enable(arguments);
        uiCameraAudioListener.enabled = true;
        //UpdateValues();

        if(debugPanelManager != null)
        {
            debugPanelManager.Initialize();
            debugPanelManager.UpdateCollectablesCount(playerStateManager.TotalCrystalsCollected);
            debugPanelManager.ShowAllMainMenuDebugItems();
        }

        //if (DebugPanelManager.instance && PlayerStateManager.instance)
        //{
        //    DebugPanelManager.instance.UpdateCollectablesCount(PlayerStateManager.instance.TotalCrystalsCollected);
        //    Debug.Log("<color=green> CalcularedTotalCrystals: </color>" + PlayerStateManager.instance.TotalCrystalsCollected);
        //    DebugPanelManager.instance.ShowAllMainMenuDebugItems();
        //}

        //StartCoroutine(_Initializer());
    }

    // Update is called once per frame
    void Update()
    {
        base.MyUpdate();
    }

    public int GetNumberOfZones()
    {
        int zonesCount = zoneManagers.Length > 0 ? zoneManagers.Length : 0;
        return zonesCount;
    }

    public string[] GetScenesFromCSV(int fileNumber)
    {
        return playerStateManager.GetScenesFromCSV(fileNumber);
    }

    //private IEnumerator _Initializer()
    //{
    //    while (GameDataManager.InitializeLevels == null)
    //    {
    //        yield return null;
    //    }

    //    GameDataManager.InitializeLevels.Invoke();
    //}

    private void InitializeChildren()
    {
        //zoneManagers = transform.GetComponentsInChildren<ZoneManager>();
        //swipeScroller = transform.GetComponentInChildren<SwipeScroller>();

        if(zoneManagers != null)
        {
            for(int i =0; i < zoneManagers.Length; i++)
            {
                zoneManagers[i].Init(this);
            }
        }
    }

    public int GetZoneUnlockCondition(int zoneNumber)
    {
        return playerStateManager.GetZoneUnlockCondition(zoneNumber, zoneManagers);
    }

    public void SetupScene()
    {
        if(DebugPanelManager.instance)
        {
            DebugPanelManager.instance.UpdateCollectablesCount(playerStateManager.TotalCrystalsCollected);
        }
        // Calls CHILDREN zoneManagers function to instantiate levels at specific points;

        if (zoneManagers != null)
        {
            for (int i = 0; i < zoneManagers.Length; i++)
            {
                zoneManagers[i].PopulateLevels();
                zoneManagers[i].UpdateZoneLevelsState();
            }
        }
    }

    public LevelManager GetNextLevel(int levelNumber, int zoneNumber)
    {
        return zoneManagers[zoneNumber].GetNextLevel(levelNumber);
    }

    public List<Level> GetLevelsFromJson(int zoneNumber)
    {
        return playerStateManager.GetLevelsInZoneFromJson(zoneNumber);
    }

    public void BackButtonCallback()
    {
        bool someActivityGoingOn = false;
        if (someActivityGoingOn == true)
            return;

#if UNITY_ANDROID
        if (MenuController.instance.IsOnTop(MenuType.MainMenuScreen))
            MenuController.instance.ShowMenu(MenuType.QuitPopup, "");
#endif
    }

    public override void HandleBackButton()
    {
        BackButtonCallback();
    }

    //public override void Enable(string arguments)
    //{
    //    base.Enable(arguments);

    //    UpdateValues();
            
    //}

    public override void UpdateValues()
    {
        base.UpdateValues();

        if (zoneManagers != null)
        {
            for (int i = 0; i < zoneManagers.Length; i++)
            {
                zoneManagers[i].UpdateZoneLevelsState();
            }
        }
    }

    public override void Disable()
    {
        base.Disable();
        uiCameraAudioListener.enabled = false;
    }

    public void TapToPlayCallback()
    {
        //GameController.instance.LoadLevel(false);
        SceneLoader.Instance.LoadNextSceneInBuild();
        menuController.ClearMenusList();

        menuController.ShowMenu(MenuType.GameplayHUD, "");
    }

    public void SettingsCallback()
    {
        AudioManager.instance.PlayMusic(AudioManager.Track.Gameplay);
        menuController.ShowMenu(MenuType.SettingsScreen, "argument1,argument2,argument3");
    }

    public int GetLevelsInAZone(int zoneNumber)
    {
        return zoneManagers[zoneNumber].GetLevelsCount();
    }

    public void Reset()
    {
        if(zoneManagers != null)
        {
            for (int i = 0; i < zoneManagers.Length; i++)
            {
                zoneManagers[i].Reset();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private string sceneName;
    private int timesSelected = 0;

    private int levelNumber = 0;
    private int zoneNumber = 0;
    private int crystalsCollected = 0;
    private bool isLevelPlayable = false;
    private ZoneManager zoneManager;

    [SerializeField] private GameObject lockIcon;

    public string SceneName { get { return sceneName; } set { sceneName = value; } }
    public int LevelNumber { get { return levelNumber; } set { levelNumber = value; } }
    public int CrystalsCollected { get { return crystalsCollected; } set { crystalsCollected = value; } }
    public int ZoneNumber { get { return zoneNumber; } set { zoneNumber = value; } }
    public bool IsLevelPlayable { get { return isLevelPlayable; } set { isLevelPlayable = value; } }
    public ZoneManager ZoneManager { get { return zoneManager; } set { zoneManager = value; } }
    public int TimeSelected { get { return timesSelected; } set { timesSelected = value; } }

    public void OnClickCallBack()
    {

        if (IsLevelPlayable)
        {
            switch (timesSelected)
            {
                case 0:
                    Debug.Log("<color=green> Level Position: </color>" + this.transform.position);
                    zoneManager.UpdateLevelSelectionMarkerPosition(this.transform.position,this);
                    zoneManager.UpdateCrystalsFillCount(crystalsCollected);
                    timesSelected++;
                    break;
                case 1:
                    Debug.Log("CallBack Called");
                    PlayerStateManager.instance.ActiveLevelManagerInstance = this;
                    SceneLoader.Instance.LoadSpecificScene(SceneName);
                    DebugPanelManager.instance.ShowOnlyGamePlayDebugItems();
                    DebugPanelManager.instance.SetActiveLevelDebugText(ZoneNumber + 1, levelNumber);
                    timesSelected = 0;
                    break;
            }
        }
    }

    // Disable tapblocker , lock icon for levels which are unlocked;
    public void DisableLockIcon()
    {
        lockIcon.SetActive(false);
    }

    public void EnableLockIcon()
    {
        lockIcon.SetActive(true);
    }

    public void Reset()
    {
        timesSelected = 0;
        LevelNumber = 0;
        zoneNumber = 0;
        isLevelPlayable = false;
        crystalsCollected = 0;
        EnableLockIcon();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DebugPanelManager : MonoBehaviour
{
    public static DebugPanelManager instance;

    public bool onlyShowFps = false;

    //[SerializeField] private TextMeshProUGUI crystalCountTMPro;
    //[SerializeField] private TextMeshProUGUI fpsTMPro;
    //[SerializeField] private TextMeshProUGUI activeLevelTMPro;
    //[SerializeField] private GameObject resetButton;
    //[SerializeField] private GameObject nextLevelButton;
    //[SerializeField] private RawImage panelBg;
    private TextMeshProUGUI crystalCountTMPro;
    private TextMeshProUGUI fpsTMPro;
    private TextMeshProUGUI activeLevelTMPro;
    private GameObject resetButton;
    private GameObject nextLevelButton;
    private GameObject unlockAllButton;
    private RawImage panelBg;

    private int crystalsCount = 0;

    #region Fps Variables
    private float updateInterval = 0.5f;
    private float fpsUpdateInterval = 0.5f;
    private float accum = 0.0f;
    private int frames = 0;
    private float timeLeft;
    private float fps;
    #endregion

    public void Initialize()
    {
        resetButton = this.transform.Find("ResetButton").gameObject;
        nextLevelButton = this.transform.Find("NextLevel").gameObject;
        crystalCountTMPro = this.transform.Find("CrystalsCollected").GetComponent<TextMeshProUGUI>();
        activeLevelTMPro = this.transform.Find("ActiveLevel").GetComponent<TextMeshProUGUI>();
        fpsTMPro = this.transform.Find("Fps").GetComponent<TextMeshProUGUI>();
        unlockAllButton = this.transform.Find("UnlockAll").gameObject;
        activeLevelTMPro.gameObject.SetActive(false);

        instance = this;
        timeLeft = updateInterval;

        if (onlyShowFps)
        {
            ShowOnlyFps();
        }
    }

    private void Update()
    {
        //currentFps.text = "FPS: " + (1.0f / Time.smoothDeltaTime).ToString("00");

        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;
        if (timeLeft <= 0.0f)
        {
            fps = (accum / frames);
            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;

            fpsTMPro.text = "FPS: " + fps.ToString("00");
        }
    }

    public void ShowOnlyFps()
    {
        panelBg.enabled = false;
        crystalCountTMPro.enabled = false;
        resetButton.SetActive(false);
    }

    public void ResetButtonCallBack()
    {
        if(PlayerStateManager.instance)
        {
            PlayerStateManager.instance.ResetAllProgress();
        }
    }

    public void UnlockAllCallBack()
    {
        if(PlayerStateManager.instance)
        {
            PlayerStateManager.instance.UnlockAllLevels();
        }
    }

    public void ShowOnlyGamePlayDebugItems()
    {
        if (resetButton != null && resetButton.activeInHierarchy)
        {
            resetButton.SetActive(false);
        }

        if (crystalCountTMPro != null && crystalCountTMPro.gameObject.activeInHierarchy)
        {
            crystalCountTMPro.gameObject.SetActive(false);
        }

        if (activeLevelTMPro != null)
            activeLevelTMPro.gameObject.SetActive(true);

        if (unlockAllButton != null && unlockAllButton.activeInHierarchy)
            crystalCountTMPro.gameObject.SetActive(false);

#if UNITY_IPHONE
        if (nextLevelButton != null && !nextLevelButton.activeInHierarchy)
        {
            nextLevelButton.SetActive(true);
        }
#endif
    }

    public void ShowAllMainMenuDebugItems()
    {
        resetButton.SetActive(true);
        crystalCountTMPro.gameObject.SetActive(true);
        nextLevelButton.SetActive(false);
        activeLevelTMPro.gameObject.SetActive(false);
        unlockAllButton.SetActive(true);
    }

    public void UpdateCollectablesCount(int crystals)
    {
        //crystalsCount += crystals;
        crystalCountTMPro.text = "Crystals: " + crystals.ToString();
    }

    public void SetActiveLevelDebugText(int currentZone, int currentLevel)
    {
        activeLevelTMPro.text = "Level: " + currentZone.ToString() + " - " + currentLevel.ToString();
    }

    private void Reset()
    {
        crystalCountTMPro.text = " ";
    }

    // for iOS Devices;
    public void NextCheatButtonCallBack()
    {
        if(GameController.instance)
        {
            GameController.instance.SkipToNextLevel();
        }
    }

    public string GetCoinsCount()
    {
        return crystalCountTMPro.ToString();
    }
}

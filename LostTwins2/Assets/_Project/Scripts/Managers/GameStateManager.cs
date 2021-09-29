using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    private const string soundsEnabledPP = "soundsEnabled";
    private const string musicEnabledPP = "musicEnabled";
    private const string adsEnabledPP = "adsEnabled";

    private bool soundsEnabled;
    private bool musicEnabled;
    private bool adsEnabled = true;

    [SerializeField] private ZoneManager[] zoneManagers;
    [SerializeField] private SwipeScroller swipeScroller;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        FetchAllValuesFromPlayerPrefs();
    }

    private void FetchAllValuesFromPlayerPrefs()
    {
        FetchSettingsValues();

        PlayerPrefs.Save();
    }

    private void FetchSettingsValues()
    {
        if (PlayerPrefs.HasKey(soundsEnabledPP)) { soundsEnabled = (PlayerPrefs.GetInt(soundsEnabledPP)==1); }
        else { SetSoundsEnabled(true); }

        if (PlayerPrefs.HasKey(musicEnabledPP)) { musicEnabled = (PlayerPrefs.GetInt(musicEnabledPP) == 1); }
        else { SetMusicEnabled(true); }

        if(PlayerPrefs.HasKey(adsEnabledPP)) { adsEnabled = (PlayerPrefs.GetInt(adsEnabledPP) == 1); }
        else { SetAdsEnabled(true); }
    }

    public bool GetSoundsEnabled()
    {
        return soundsEnabled;
    }

    public void SetSoundsEnabled(bool value)
    {
        soundsEnabled = value;
        PlayerPrefs.SetInt(soundsEnabledPP, soundsEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool GetMusicEnabled()
    {
        return musicEnabled;
    }

    public void SetMusicEnabled(bool value)
    {
        musicEnabled = value;
        PlayerPrefs.SetInt(musicEnabledPP, musicEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool GetAdsEnabled()
    {
        return adsEnabled;
    }

    public void SetAdsEnabled(bool value)
    {
        adsEnabled = value;
        PlayerPrefs.SetInt(adsEnabledPP, adsEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public ZoneManager GetZoneManager()
    {
        return zoneManagers[0];
    }

}

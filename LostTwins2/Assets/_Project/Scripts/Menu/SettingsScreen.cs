using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SettingsScreen : BaseMenu
{
    public override void Init()
    {
        base.AdjustElementsAffectedByNotch();
        base.Init();

#if UNITY_ANDROID
        SetButtonsAccordingToAndroid();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        base.MyUpdate();
    }

    public void BackButtonCallback()
    {
        bool someActivityGoingOn = false;
        if (someActivityGoingOn == true)
            return;

        AudioManager.instance.PlayMusic(AudioManager.Track.MainMenu);
        menuController.BackButtonCallback("");
    }

    public override void HandleBackButton()
    {
        BackButtonCallback();
    }

    public override void Enable(string arguments)
    {
        base.Enable(arguments);

        SetButtons();
    }

    private void SetButtonsAccordingToAndroid()
    {
        //Extra settings for Android if any
    }

    private void SetButtons()
    {
        if(GameStateManager.instance.GetSoundsEnabled())
        {
            //Enable Sounds button
        }
        else
        {
            //Disable Sounds button
        }
    }

    public void SoundsButtonCallback()
    {
        bool currentState = GameStateManager.instance.GetSoundsEnabled();
        GameStateManager.instance.SetSoundsEnabled(!currentState);
        GameStateManager.instance.SetMusicEnabled(!currentState);
        AudioManager.instance.SetMusicActive(!currentState);
        AudioManager.instance.SetSoundEffectsActive(!currentState);
        SetButtons(); 
    }

    public void PrivacyPolicyButtonCallback()
    {
        Application.OpenURL("https://www.bbtv.com/privacy-policy");
    }
}

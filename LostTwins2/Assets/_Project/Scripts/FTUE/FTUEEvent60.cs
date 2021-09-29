using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent60 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "Starts revolving around the portal, Highlight switch character button";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void StartEvent()
    {
        base.StartEvent();
    }

    private IEnumerator SequenceEvent()
    {
        //FTUEHelper.instance.RevolveBirdAroundPortal();
        FTUEHelper.instance.StopShockwave();
        yield return new WaitForSeconds(1.5f);
        GameplayManager.instance.CharacterSwitchingDisabled = false;
        if (GameplayScreen.instance)
        {
            GameplayScreen.instance.SetCharSwitchButtonVisibility(true);
        }
        while (GameplayManager.instance.CurrentCharacterIndex!=0)
        {
            yield return new WaitForEndOfFrame();
        }
        if (GameplayScreen.instance)
        {
            GameplayScreen.instance.SetCharSwitchButtonVisibility(false);
        }
        GameplayManager.instance.CurrentPlayer.SetBenSleeping();
        GameplayManager.instance.CharacterSwitchingDisabled = true;
        FTUEHelper.instance.GetWisp2Transform().GetComponent<Wisp>().SetPosition(FTUEHelper.instance.LeftWispStartingPosition);
        FTUEHelper.instance.GetWisp2Transform().GetComponent<Wisp>().StartMovement();
        Debug.Log("Event 60 completed");
        isCompleted = true;
    }
}

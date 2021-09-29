using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent80 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "Ben stands up, curious dialogue box, controls given";
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
        while (GameplayManager.instance.CurrentPlayer.IsIdlePlaying() == false)
        {
            yield return new WaitForEndOfFrame();
        }
        //yield return new WaitForSeconds(0.5f);
        GameplayManager.instance.CurrentPlayer.ShockToIdleBoy();

        yield return new WaitForSeconds(1.0f);
        FTUEHelper.instance.StartShockwave();
        //!! dialogue box
        GameplayManager.instance.SetControlsDisabled(false);
        if (GameplayScreen.instance)
        {
            GameplayScreen.instance.SetLeftRightButtonsVisibility(true);
        }
        Debug.Log("Event 80 completed");
        isCompleted = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent110 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "Controls taken away, glass shatters";
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
        while (FTUEHelper.instance.GetBoyTransform().position.x < FTUEHelper.instance.BoyBeforePortalPosition.x - 0.1f)
        {
            yield return new WaitForEndOfFrame();
        }

        //FTUEHelper.instance.StopShockwave();
        GameplayManager.instance.SetControlsDisabled(true);
        if (GameplayScreen.instance)
        {
            GameplayScreen.instance.SetAllButtonsVisibility(false);
        }
        yield return new WaitForSeconds(0.5f);

        //FOR TRAILER
        //yield return new WaitForSeconds(4.0f);

        FTUEHelper.instance.GetWisp1Transform().GetComponent<Wisp>().MoveTransform(GameplayManager.instance.FinalPortal.GetCenterForOrbPosition(), 10.0f, false);
        FTUEHelper.instance.GetWisp1Transform().GetComponent<Wisp>().DisableAfterSomeTime();

        yield return new WaitForSeconds(0.5f);

        FTUEHelper.instance.GetWisp2Transform().GetComponent<Wisp>().MoveTransform(GameplayManager.instance.FinalPortal.GetCenterForOrbPosition(), 10.0f, false);
        FTUEHelper.instance.GetWisp2Transform().GetComponent<Wisp>().DisableAfterSomeTime();


        //FXManager.instance.ShakeCamera(3.5f, 1.5f);
        //FXManager.instance.Shockwave();
        FTUEHelper.instance.ShatterGlass();
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Event 110 completed");
        isCompleted = true;
    }
}

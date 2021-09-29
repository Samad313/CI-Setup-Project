using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent120 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "Level end portal animation";
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
        FTUEHelper.instance.EnablePortal();
        //FTUEHelper.instance.SetBirdTopOfGate();
        //FTUEHelper.instance.GetBirdTransform().GetComponent<Phoenix>().SetFlyingAnimationForFTUE(1.0f);
        //FTUEHelper.instance.EnableTitleTextCamera();
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Event 120 completed");
        isCompleted = true;

        if (PlayerStateManager.instance)
        {
            PlayerStateManager.instance.MarkFTUEAsComplete();
        }
    }
}

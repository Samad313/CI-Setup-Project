using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent40 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "Bird now flies to museum entrance. Wait till girl reaches museum entrance";
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
        FTUEHelper.instance.GetWisp1Transform().GetComponent<Wisp>().MoveTransform(FTUEHelper.instance.RightWispRestPosition(2), 6.0f, false);

        while (FTUEHelper.instance.GetGirlTransform().position.x > FTUEHelper.instance.RightWispRestPosition(2).x + 4.0f)
        {
            yield return new WaitForEndOfFrame();
        }

        FTUEHelper.instance.GetWisp1Transform().GetComponent<Wisp>().MoveTransform(FTUEHelper.instance.RightWispRestPosition(3), 6.0f, false);
        //FTUEHelper.instance.GetWisp1Transform().GetComponent<Wisp>().MoveTransform(GameplayManager.instance.FinalPortal.GetGateTop().position, 35.0f, false);

        //FTUEHelper.instance.GetWisp1Transform().GetComponent<Wisp>().DisableAfterSomeTime();

        //while (FTUEHelper.instance.GetGirlTransform().position.x > FTUEHelper.instance.RightWispRestPosition(3).x + 0.3f)
        while (FTUEHelper.instance.GetGirlTransform().position.x > FTUEHelper.instance.GirlBeforePortalPosition.x + 0.1f)  
        {
            yield return new WaitForEndOfFrame();
        }

        DialoguesController.instance.ShowDialogue(DialogueType.Question, CharName.Girl);
        

        Debug.Log("Event 40 completed");
        isCompleted = true;
    }
}

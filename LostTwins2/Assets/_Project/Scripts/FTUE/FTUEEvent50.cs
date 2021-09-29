using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent50 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "Bird moves towards portal top while converting into phoenix. Controls taken, girls automatically moves near portal & stops";
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
        //FTUEHelper.instance.GetBirdTransform().GetComponent<Phoenix>().SetRevolvingAnimation();

        //FTUEHelper.instance.MoveTransform(FTUEHelper.instance.GetBirdTransform(), GameplayManager.instance.FinalPortal.GetGateTop().position, 3.0f, false);

        //FTUEHelper.instance.ConvertIntoPhoenix(3.0f);

        GameplayManager.instance.SetControlsDisabled(true);
        GameplayManager.instance.CurrentPlayer.SeeingPhoenix();
        if (GameplayScreen.instance)
        {
            GameplayScreen.instance.SetLeftRightButtonsVisibility(false);
        }

        yield return new WaitForSeconds(1.5f);
        Debug.Log("Event 50 completed");
        isCompleted = true;
    }
}

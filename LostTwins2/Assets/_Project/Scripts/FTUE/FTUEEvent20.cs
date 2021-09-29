using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent20 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "Bird goes towards corridor door, Abi stands up & exclamation mark appears, controls given";
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
        
        while(GameplayManager.instance.CurrentPlayer.IsIdlePlaying()==false)
        {
            yield return new WaitForEndOfFrame();
        }

        //yield return new WaitForSeconds(1.0f);
        //yield return new WaitForSeconds(2.0f);
        GameplayManager.instance.CurrentPlayerTransform.position += new Vector3(0.15f, 0, 0);
        GameplayManager.instance.CurrentPlayer.ShockToIdleGirl();
        GameplayManager.instance.CurrentPlayer.SetFacingDirection(-1.0f);

        yield return new WaitForSeconds(0.5f);

        GameplayManager.instance.SetControlsDisabled(false);
        GameplayManager.instance.CurrentPlayer.SetRigidbody(true);
        GameplayManager.instance.CurrentPlayer.DisableRunDelay();
        if (GameplayScreen.instance)
        {
            GameplayScreen.instance.SetLeftRightButtonsVisibility(true);
        }
        

        Debug.Log("Event 20 completed");
        isCompleted = true;
    }
}

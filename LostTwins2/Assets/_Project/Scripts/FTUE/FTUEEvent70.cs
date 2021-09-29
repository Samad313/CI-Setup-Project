using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent70 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "Ben sleeping, shockwave triggers falls him down & cupboard falls";
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
        yield return new WaitForSeconds(0.6f);
        GameplayManager.instance.CurrentPlayer.SetBenShock();
        yield return new WaitForSeconds(0.8f);
        FTUEHelper.instance.GetWisp2Transform().GetComponent<Wisp>().MoveTransform(FTUEHelper.instance.LeftWispRestPosition(1), 6.0f, false);
        yield return new WaitForSeconds(0.1f);
        DialoguesController.instance.ShowDialogue(DialogueType.Question, CharName.Boy, 1.0f, new Vector3(-0.65f, 2.1f, 0));
        FXManager.instance.ShakeCamera(3.5f, 1.5f);
        FXManager.instance.Shockwave();
        yield return new WaitForSeconds(0.5f);
        FTUEHelper.instance.MakeCupboardFall();
        FTUEHelper.instance.GetBirdTransform().GetComponent<Phoenix>().SetSkeletonScale(1.0f);
        Debug.Log("Event 70 completed");
        isCompleted = true;
    }
}

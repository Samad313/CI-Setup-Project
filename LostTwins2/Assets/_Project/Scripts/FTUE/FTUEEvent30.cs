using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent30 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "Highlight to go left & wait till girl reaches the bedroom door";
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
        while(FTUEHelper.instance.GetGirlTransform().position.x>FTUEHelper.instance.RightWispRestPosition(1).x + 4.0f)
        {
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Event 30 completed");
        isCompleted = true;
    }
}

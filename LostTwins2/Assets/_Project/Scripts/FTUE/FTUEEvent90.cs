using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent90 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "Highlight to go right & wait till boy reaches the cupboard";
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
        while (FTUEHelper.instance.GetBoyTransform().position.x < FTUEHelper.instance.BeforeCupboardPosition.x - 1.0f)
        {
            yield return new WaitForEndOfFrame();
        }

        
        Debug.Log("Event 90 completed");
        isCompleted = true;
    }
}

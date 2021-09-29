using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent140 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "End FTUE";
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
        yield return new WaitForSeconds(9.0f);
        //FTUEHelper.instance.SaveTitleTextPosition();
        FTUEHelper.instance.LoadFirstLevel();
        
        Debug.Log("Event 140 completed");
        isCompleted = true;
    }
}

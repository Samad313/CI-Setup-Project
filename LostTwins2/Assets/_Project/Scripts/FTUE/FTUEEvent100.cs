using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent100 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "Jump button now also appears, wait till boy reaches portal";
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
        #if UNITY_IOS || UNITY_ANDROID
        if (GameplayScreen.instance)
        {
            GameplayScreen.instance.SetJumpButtonVisibility(true);
        }
        #endif

        while (FTUEHelper.instance.GetBoyTransform().position.x < FTUEHelper.instance.LeftWispRestPosition(1).x - 4.0f)
        {
            yield return new WaitForEndOfFrame();
        }

        FTUEHelper.instance.GetWisp2Transform().GetComponent<Wisp>().MoveTransform(FTUEHelper.instance.LeftWispRestPosition(2), 6.0f, false);

        while (FTUEHelper.instance.GetBoyTransform().position.x < FTUEHelper.instance.LeftWispRestPosition(2).x - 4.0f)
        {
            yield return new WaitForEndOfFrame();
        }

        FTUEHelper.instance.GetWisp2Transform().GetComponent<Wisp>().MoveTransform(FTUEHelper.instance.LeftWispRestPosition(3), 6.0f, false);

        FTUEHelper.instance.StopShockwave();

        Debug.Log("Event 100 completed");

        isCompleted = true;
    }
}

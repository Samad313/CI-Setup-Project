using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent130 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "Bird flying out & title appears";
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
        

        //while (FTUEHelper.instance.GetCameraController().transform.position.x < FTUEHelper.instance.GetTitleTextGroup().position.x - 5.0f)
        //{
        //    yield return new WaitForEndOfFrame();
        //}

        yield return new WaitForSeconds(8.0f);

        //FOR TRAILER
        //yield return new WaitForSeconds(5.0f);

        FTUEHelper.instance.PlayLostTwinsLogoVideo();
        //FTUEHelper.instance.AnimatedTitleText();
        //FTUEHelper.instance.GetCameraController().DecreaseBirdFollowSpeed();

        //while (FTUEHelper.instance.GetCameraController().GetDontFollowAnything()==false)
        //{
        //    yield return new WaitForEndOfFrame();
        //}

        Debug.Log("Event 130 completed");
        isCompleted = true;
    }
}

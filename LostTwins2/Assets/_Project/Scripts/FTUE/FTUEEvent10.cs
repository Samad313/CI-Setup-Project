using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEEvent10 : FTUEEvent
{
    // Start is called before the first frame update
    void Start()
    {
        isCompleted = false;
        description = "Girl playing with the bird in cage, then shockwave trigger every 8 seconds";
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
        if (GameplayScreen.instance)
            GameplayScreen.instance.SetAllButtonsVisibility(false);

        GameplayManager.instance.SetCharacter(CharName.Boy);
        GameplayManager.instance.CurrentPlayer.SetBenSleeping();

        GameplayManager.instance.SetCharacter(CharName.Girl);
        FTUEHelper.instance.GetGirlTransform().position = FTUEHelper.instance.GirlPlayingPosition;
        FTUEHelper.instance.GetBoyTransform().position = FTUEHelper.instance.BoySleepingPosition;
        FTUEHelper.instance.GetBirdTransform().GetComponent<Phoenix>().Skip();
        
        FTUEHelper.instance.GetWisp1Transform().gameObject.SetActive(false);
        GameplayManager.instance.CurrentPlayer.SetRigidbody(false);
        GameplayManager.instance.CurrentPlayer.SetShadow(false);
        GameplayManager.instance.CurrentPlayer.SetSittingWithBirdIdle();
        FTUEHelper.instance.GetCameraController().SetPositionNow();
        FTUEHelper.instance.GetCameraController().SetCameraStartingOffset(FTUEHelper.instance.CameraStartingPosition - FTUEHelper.instance.GetCameraController().transform.position);
        FTUEHelper.instance.GetCameraController().SetCameraStartingRotationOffset(FTUEHelper.instance.CameraStartingRotation);
        FTUEHelper.instance.GetCameraController().SetCameraStartingFOV(FTUEHelper.instance.CameraStartingFOV);
        FTUEHelper.instance.GetCameraController().SetPositionNow();
        FXManager.instance.SetCameraFade(0.0f);
        GameplayManager.instance.SetControlsDisabled(true);
        GameplayManager.instance.CharacterSwitchingDisabled = true;

        if (GameplayScreen.instance)
        {
            //FTUEHelper.instance.GetWispTransform().GetComponent<Wisp>().SetPosition(FTUEHelper.instance.BirdStartingPosition);
            FTUEHelper.instance.GetWisp1Transform().GetComponent<Wisp>().SetPosition(GameplayScreen.instance.GetMenuCrystalWorldPosition);
        }
        else
        {
            FTUEHelper.instance.GetWisp1Transform().GetComponent<Wisp>().SetPosition(FTUEHelper.instance.RightWispStartingPosition);
        }

        //FTUEHelper.instance.HoldObjectPosition(FTUEHelper.instance.GetGirlTransform());

        yield return new WaitForSeconds(0.5f);

        //FXManager.instance.FadeCamera(false, 0.5f);
        yield return new WaitUntil( ()=> GameplayManager.instance.IsGamePlayState);
       
        //yield return new WaitUntil(()=>Input.GetMouseButtonDown(0));
        FTUEHelper.instance.GetWisp1Transform().gameObject.SetActive(true);

        if (GameplayScreen.instance)
            GameplayScreen.instance.HandleTitleScreenInit();

        FTUEHelper.instance.GetWisp1Transform().GetComponent<Wisp>().StartMovement();

        yield return new WaitForSeconds(0.5f);
        GameplayManager.instance.CurrentPlayer.SetSittingWithBirdShock();
        yield return new WaitForSeconds(1.5f);
        FTUEHelper.instance.GetWisp1Transform().GetComponent<Wisp>().MoveTransform(FTUEHelper.instance.RightWispRestPosition(1), 6.0f, false);

        FTUEHelper.instance.GetCameraController().SetCameraMovingDown();
        yield return new WaitForSeconds(0.5f);

        DialoguesController.instance.ShowDialogue(DialogueType.Exclamation, CharName.Girl, 1.0f);
        
        yield return new WaitForSeconds(1.5f);
        
        //FTUEHelper.instance.StartShockwave();
        yield return new WaitForSeconds(0.5f);
        FTUEHelper.instance.MoveTransform(GameplayManager.instance.CurrentPlayerTransform, new Vector3(-0.5f, 0.5f, 0), 8.0f, true);
        yield return new WaitForSeconds(0.1f);
        FTUEHelper.instance.MoveTransform(GameplayManager.instance.CurrentPlayerTransform, new Vector3(-0.8f, -0.77825f, 0), 8.0f, true);
        GameplayManager.instance.CurrentPlayer.SetShadow(true);
        yield return new WaitForSeconds(0.4f);
        
        Debug.Log("Event 10 completed");
        isCompleted = true;
    }
}

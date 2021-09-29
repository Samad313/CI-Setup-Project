using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterClimbable : Climbable
{

    void Awake()
    {
        base.AwakeInit();

    }

    // Start is called before the first frame update
    void Start()
    {
        base.StartInit();
    }

    void Update()
    {
        base.MyUpdate();

        if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
            return;

    }

    public override void Mounted(Transform playerTransform, float inputClimbingTopOffset, Transform charPositionObject, int charIndex)
    {
        base.Mounted(playerTransform, inputClimbingTopOffset, charPositionObject, charIndex);

    }

    public override void UnMount(Transform playerTransform, int charIndex)
    {
        base.UnMount(playerTransform, charIndex);
    }

    public override void Climb(Transform playerTransform, Transform charPositionObject, int charIndex)
    {
        base.Climb(playerTransform, charPositionObject, charIndex);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==10)
        {
            if (other.GetComponent<PlayerController>().GetInWater() == true)
            {
                if(GetJumpInput(other.gameObject.GetComponent<PlayerController>()) && GetSideInput(other.gameObject.GetComponent<PlayerController>()))
                {
                    other.gameObject.GetComponent<PlayerController>().MountMe(this, climbDirection);
                    other.gameObject.GetComponent<PlayerController>().SwimmingToClimbing();
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            if (other.GetComponent<PlayerController>().GetInWater() == true)
            {
                if (GetJumpInput(other.gameObject.GetComponent<PlayerController>()) && GetSideInput(other.gameObject.GetComponent<PlayerController>()))
                {
                    other.gameObject.GetComponent<PlayerController>().MountMe(this, climbDirection);
                    other.gameObject.GetComponent<PlayerController>().SwimmingToClimbing();
                }
            }
        }
    }

    private bool GetJumpInput(PlayerController playerController)
    {
        if (playerController.GetIsActive() == false)
            return false;

        return InputManager.jump || InputManager.up;
    }

    private bool GetSideInput(PlayerController playerController)
    {
        if (playerController.GetIsActive() == false)
            return false;

        if(climbDirection==ClimbDirection.left)
        {
            return InputManager.left;
        }
        else if (climbDirection == ClimbDirection.right)
        {
            return InputManager.right;
        }

        return false;
    }

}

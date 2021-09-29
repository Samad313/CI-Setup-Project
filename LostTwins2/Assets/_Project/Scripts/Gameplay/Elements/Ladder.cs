using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Ladder : MonoBehaviour
{
    private Animator myAnimator;

    private int lastIsMoving = 0;
    private int isMoving = 0;
    private float lastX = 0;

    private float movementThreshold = 0.5f;

    private float stopppingDelay = -1.0f;

    private Bone leftWheel;
    private Bone rightWheel;
    private Bone[] gears;

    private float wheelRotation;

    private Transform parentTransform;

    private Transform myPushyParent;

    // Start is called before the first frame update
    void Start()
    {
        parentTransform = transform.parent;
        lastX = parentTransform.position.x;

        myAnimator = GetComponent<Animator>();
        SkeletonMecanim mySkeletonMecanim = GetComponent<SkeletonMecanim>();
        leftWheel = mySkeletonMecanim.skeleton.FindBone("wheel2");
        rightWheel = mySkeletonMecanim.skeleton.FindBone("wheel3");
        gears = new Bone[3];
        gears[0] = mySkeletonMecanim.skeleton.FindBone("gear-02");
        gears[1] = mySkeletonMecanim.skeleton.FindBone("gear-4");
        gears[2] = mySkeletonMecanim.skeleton.FindBone("gear-5");

        myPushyParent = transform.parent.Find("Collider").GetComponent<Climbable>().GetMyPushyParent();

        mySkeletonMecanim.UpdateLocal += HandleUpdateLocal;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
        {
            lastX = parentTransform.position.x;
            lastIsMoving = isMoving;
            return;
        }
            

        float movementThresholdDT = movementThreshold * Time.deltaTime;
        //Debug.Log(transform.position.x + " : " + movementThresholdDT);
        if (parentTransform.position.x > lastX + movementThresholdDT)
        {
            stopppingDelay = 0.3f;
            isMoving = 1;
        }
        else if (parentTransform.position.x < lastX - movementThresholdDT)
        {
            stopppingDelay = 0.3f;
            isMoving = -1;
        }
        else
        {
            isMoving = 0;
        }

        if(isMoving!=lastIsMoving)
        {
            if (isMoving != 0)
            {
                
            }

            if (isMoving==1)
            {
                if(myAnimator.GetBool("DragRight")==false)
                {
                    FXManager.instance.LadderMoved(myPushyParent.position, false);
                    myAnimator.SetBool("DragRight", true);
                } 
            }
            else if (isMoving == -1)
            {
                if (myAnimator.GetBool("DragLeft") == false)
                {
                    FXManager.instance.LadderMoved(myPushyParent.position, true);
                    myAnimator.SetBool("DragLeft", true);
                }
            }
        }

        if(isMoving==0&&stopppingDelay<0)
        {
            myAnimator.SetBool("DragRight", false);
            myAnimator.SetBool("DragLeft", false);
        }

        float angleChange = -90.0f * (parentTransform.position.x - lastX);
        wheelRotation += angleChange;

        
        lastX = parentTransform.position.x;
        lastIsMoving = isMoving;

        stopppingDelay -= Time.deltaTime;
    }

    void HandleUpdateLocal(ISkeletonAnimation value)
    {
        leftWheel.Rotation = wheelRotation;
        rightWheel.Rotation = wheelRotation;
        for (int i = 0; i < gears.Length; i++)
        {
            gears[i].Rotation = -wheelRotation * 0.5f;
        }
    }

    public void Entered(Transform climber)
    {
        if(climber.position.x< parentTransform.position.x)
        {
            myAnimator.SetTrigger("SwayRight");
        }
        else
        {
            myAnimator.SetTrigger("SwayLeft");
        }
    }

    public void Exitted(Transform climber)
    {
        if (climber.position.x < parentTransform.position.x)
        {
            myAnimator.SetTrigger("SwayRight");
        }
        else
        {
            myAnimator.SetTrigger("SwayLeft");
        }
    }
}

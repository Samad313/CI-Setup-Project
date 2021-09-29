using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperAirplane : MonoBehaviour
{

    #region Private Variables
    private const string flyAnimationParameter = "FlyLoop";
    private const string stopEndingAnimationParameter = "StopEnding";
    private const string loopEndingAnimationParameter = "LoopEnding";
    private const string idleAnimationParameter = "Idle";

    private Animator animator;
    private bool isAnimationRunning = false;
    AnimatorStateInfo animState;
    private float normalisedTimeInCurrentLoop = 0f;
    #endregion

    #region Exposed Variables
    [SerializeField]
    [Range(0f, 1f)]
    private float loopAnimationThreshold = 0.8f;
    [SerializeField]
    private Fan fan;
    #endregion


    #region Getters
    public Animator Animator
    {
        get => animator;
    }

    #endregion

    private void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!isAnimationRunning)
            return;

        animState = animator.GetCurrentAnimatorStateInfo(0);

        if(!animState.IsName(idleAnimationParameter))
        {
            normalisedTimeInCurrentLoop = animState.normalizedTime - Mathf.Floor(animState.normalizedTime);
        }


        if (normalisedTimeInCurrentLoop > loopAnimationThreshold)
        {
            isAnimationRunning = false;
            normalisedTimeInCurrentLoop = 0f;

            if (fan.IsFanRunning)
            {
                PlayLoopEndAnimation();
                Invoke("EnableAnimationRunningBool", 0.2f);
            }
            else
            {
                PlayStopEndAnimation();
            }
        }
      

    }

    private void EnableAnimationRunningBool()
    {
        isAnimationRunning = true;
    }
    public void SetTrigger(bool triggered)
    {
        if(triggered)
        {
            animator.ResetTrigger(stopEndingAnimationParameter);
            FlyPlane();
        }
        else
        {
            animator.ResetTrigger(loopEndingAnimationParameter);
        }
    }

    private void PlayLoopEndAnimation()
    {
        animator.SetTrigger(loopEndingAnimationParameter);
        
    }

    private void PlayStopEndAnimation()
    {
        animator.SetTrigger(stopEndingAnimationParameter);
    }

    private void FlyPlane()
    {
        animator.SetTrigger(flyAnimationParameter);
        normalisedTimeInCurrentLoop = 0f;
        isAnimationRunning = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public enum PhoenixState {WaitingForCameraToComeDown, FlyingToGate, Revolving, SittingOnGate, Disappear, FlyingAwayFromGate, Vanished, EndRevolveFTUE, WaitBeforeFlying }

public class Phoenix : MonoBehaviour
{
    [SerializeField] private Animator myAnimator = default;

    private Vector3 startingPosition;

    private Vector3 finalPosition;

    private Transform gateTop;

    [SerializeField]
    private Vector3 offsetFromFinalPosition = default;

    [SerializeField]
    private float flyingSpeed = 4.0f;

    private PhoenixState state;
    private float waitTime = 0.0f;

    [SerializeField]
    private float finalScaleValue = 2.0f;

    [SerializeField]
    private Transform tailParticlesPrefab = default;

    private Transform tailParticles;

    private Bone tailBone;

    private float secondaryIdleWaitTime = 5.0f;

    void Awake()
    {
        SkeletonMecanim mySkeletonMecanim = myAnimator.gameObject.GetComponent<SkeletonMecanim>();
        tailBone = mySkeletonMecanim.skeleton.FindBone("tail unity");
        tailParticles = Instantiate(tailParticlesPrefab);
        //tailParticles.gameObject.SetActive(false);

        mySkeletonMecanim.UpdateLocal += HandleUpdateLocal;
    }

    public Vector3 Init(Vector3 cameraPosition)
    {
        Vector3 cameraOffset = new Vector3(25, 0, 0);
        gateTop = GameplayManager.instance.FinalPortal.GetGateTop();
        if(gateTop.position.x> cameraPosition.x)
        {
            offsetFromFinalPosition.x *= -1.0f;
            cameraOffset *= -1.0f;
            SetSkeletonScale(1.0f);
            //iconTransform.localScale = new Vector3(-iconTransform.localScale.x, iconTransform.localScale.y, iconTransform.localScale.z);
        }
        else
        {
            SetSkeletonScale(-1.0f);
        }

        if(GameplayManager.instance.FirstLevel)
        {
            offsetFromFinalPosition.x /= 2.0f;
        }
        transform.position = cameraPosition + offsetFromFinalPosition + new Vector3(0, 0, -6.5f);
        if(GameplayManager.instance.IsFTUE)
        {
            myAnimator.SetTrigger("sittingidle");
            state = PhoenixState.SittingOnGate;
            cameraOffset = Vector3.zero;
        }
        else if (GameplayManager.instance.PhoenixAlreadyOnGate)
        {
            myAnimator.SetTrigger("sittingidle");
            state = PhoenixState.SittingOnGate;
            transform.position = gateTop.position;
            myAnimator.transform.localScale = Vector3.one * finalScaleValue;
            offsetFromFinalPosition = Vector3.zero;
            cameraOffset = Vector3.zero;
        }
        else
        {
            myAnimator.SetTrigger("flying");
            state = PhoenixState.WaitingForCameraToComeDown;
        }

        return cameraOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
            return;

        if (state == PhoenixState.WaitingForCameraToComeDown)
        {
            if(GameplayManager.instance.IsCameraMovingDown==false)
            {
                state = PhoenixState.FlyingToGate;
            }
        }
        else if (state==PhoenixState.FlyingToGate)
        {
            transform.position = gateTop.position + offsetFromFinalPosition + new Vector3(0, 0, -6.5f);
            offsetFromFinalPosition = Vector3.MoveTowards(offsetFromFinalPosition, Vector3.zero, Time.deltaTime * flyingSpeed);

            if (Mathf.Abs(offsetFromFinalPosition.x) < 6.0f)
            {
                myAnimator.SetTrigger("revolving");
                state = PhoenixState.Revolving;
            }
        }
        else if (state == PhoenixState.Revolving)
        {
            transform.position = Vector3.MoveTowards(transform.position, gateTop.position, Time.deltaTime * 7.0f);
            myAnimator.transform.localScale = Vector3.MoveTowards(myAnimator.transform.localScale, Vector3.one * finalScaleValue, Time.deltaTime * 2.0f);
            
            if ((transform.position - gateTop.position).sqrMagnitude < 0.05f)
            {
                transform.position = gateTop.position;
                myAnimator.transform.localScale = Vector3.one * finalScaleValue;
                GameplayManager.instance.SetAperture(1.0f, 1.0f, null);
                offsetFromFinalPosition = Vector3.zero;
                state = PhoenixState.SittingOnGate;
                GameplayManager.instance.PhoenixAlreadyOnGate = true;
            }
        }
        else if (state == PhoenixState.SittingOnGate)
        {
            transform.position = gateTop.position;
            secondaryIdleWaitTime -= Time.deltaTime;
            if(secondaryIdleWaitTime < 0)
            {
                myAnimator.SetTrigger("idle"+Random.Range(2, 5));
                secondaryIdleWaitTime = Random.Range(5.0f, 10.0f);
            }
        }
        else if (state == PhoenixState.Disappear)
        {
            if(waitTime<0)
            {
            //    myAnimator.transform.localScale = Vector3.Lerp(myAnimator.transform.localScale, Vector3.zero, Time.deltaTime * 105.0f);
                //FXManager.instance.BirdDisappear(transform.position);
                state = PhoenixState.Vanished;
            }
            waitTime -= Time.deltaTime;
        }
        else if (state == PhoenixState.EndRevolveFTUE)
        {
            if (waitTime < 0)
            {
                myAnimator.GetComponent<Renderer>().enabled = false;
            }
            waitTime -= Time.deltaTime;
        }
        else if (state == PhoenixState.FlyingAwayFromGate)
        {
            transform.position = Vector3.MoveTowards(transform.position, finalPosition, Time.deltaTime * 5.0f);
        }
    }

    void HandleUpdateLocal(ISkeletonAnimation value)
    {
        //tailParticles.position = tailBone.GetWorldPosition(myAnimator.transform);
    }

    public void FlyAway()
    {
        finalPosition = transform.position + new Vector3(70, 0, 0);
        transform.position = transform.position + new Vector3(-10.0f, 0, 0);
        SetFlyingAnimationForFTUE(1.0f);
        state = PhoenixState.FlyingAwayFromGate;
        myAnimator.SetTrigger("flying");
    }

    public void SetFlyingAnimationForFTUE(float scaleX)
    {
        myAnimator.GetComponent<SkeletonMecanim>().skeleton.ScaleX = scaleX;
        myAnimator.SetTrigger("flying");
    }

    public void SetSkeletonScale(float scaleX)
    {
        myAnimator.GetComponent<SkeletonMecanim>().skeleton.ScaleX = scaleX;
        GetComponent<ZoomInOutIcon>().SetDirection(-scaleX);
        Vector3 centerPosition = transform.Find("center").localPosition;
        transform.Find("center").localPosition = new Vector3(Mathf.Abs(centerPosition.x) * -scaleX, centerPosition.y, centerPosition.z);
    }

    public float GetScaleX()
    {
        return myAnimator.GetComponent<SkeletonMecanim>().skeleton.ScaleX;
    }

    public void SetRevolvingAnimation()
    {
        //gateTop = GameplayManager.instance.GetFinalGate().GetGateTop();
        //transform.position = gateTop.position + offsetFromFinalPosition + new Vector3(0, 0, -6.5f);
        //state = PhoenixState.FlyingToGate;
        myAnimator.SetTrigger("revolving");
    }

    public void RevolveAroundPortal()
    {
        if(GameplayManager.instance.IsFTUE)
        {
            myAnimator.SetTrigger("revolvearoundportalftue");
            state = PhoenixState.EndRevolveFTUE;
        }
        else
        {
            myAnimator.SetTrigger("revolvearoundportal");
            state = PhoenixState.Disappear;
            
        }

        waitTime = 4.0f;
    }

    public Transform GetVisual()
    {
        return myAnimator.transform;
    }

    public void Skip()
    {
        transform.position = gateTop.position;
        myAnimator.transform.localScale = Vector3.one * finalScaleValue;
        offsetFromFinalPosition = Vector3.zero;
        state = PhoenixState.SittingOnGate;
        myAnimator.SetTrigger("sittingidle");
        GameplayManager.instance.PhoenixAlreadyOnGate = true;
    }

    public void SetVisual(bool value)
    {
        myAnimator.GetComponent<Renderer>().enabled = value;
    }
}

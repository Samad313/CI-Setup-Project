using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClimbDirection {none, left, right };
public enum ClimbStates {UnMounted, GoingTowardsPosition, OnClimbable, ClimbingUp1, ClimbingUp2}

public class Climbable : MonoBehaviour
{

    [SerializeField]
    protected Texture2D icon;

    [SerializeField]
    protected Vector3 iconScale;

    protected Transform iconTransform;

    [SerializeField]
    protected Transform myPushyParent;

    protected Vector3 offsetFromPushyParent;

    [SerializeField]
    protected Transform myRoot;

    protected float climbDelay = -1.0f;

    [SerializeField]
    protected float climbingSpeed = 2.0f;

    [SerializeField]
    protected float climbingLerpSpeed = 5.0f;

    [SerializeField]
    protected float climbingDropSpeed = 1.0f;

    [SerializeField]
    protected ClimbDirection climbDirection;

    protected Vector3 climbTopPositon;

    protected int numPlayers = 2;

    
    protected float[] climbingTopOffset;

    protected ClimbStates[] climbingStates;

    protected float[] climbingDelay;

    protected Vector3[] finalClimbStartPosition;

    protected Vector3[] currentClimbingPosition;
    protected Vector3[] wantedClimbingPosition;

    protected PlayerController[] players;

    [SerializeField]
    protected bool fromWater = false;

    public virtual void AwakeInit()
    {
        
        if (climbDirection == ClimbDirection.left)
        {
            climbTopPositon = new Vector3(-0.6f, 0.02f, 0);
        }

        else if (climbDirection == ClimbDirection.right)
        {
            climbTopPositon = new Vector3(0.6f, 0.02f, 0);
        }

        
        climbingTopOffset = new float[numPlayers];
        climbingStates = new ClimbStates[numPlayers];
        climbingDelay = new float[numPlayers];
        finalClimbStartPosition = new Vector3[numPlayers];
        currentClimbingPosition = new Vector3[numPlayers];
        wantedClimbingPosition = new Vector3[numPlayers];
    }

    public virtual void StartInit()
    {
        if(GameplayManager.instance)
        {
            players = new PlayerController[2];
            players = GameplayManager.instance.GetAllPlayers();
        }
        
    }

    // Update is called once per frame
    public virtual void MyUpdate()
    {
        if(GameplayManager.instance)
        {
            if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
                return;
        }

        climbDelay -= Time.deltaTime;
    }

    public virtual void Mounted(Transform playerTransform, float inputClimbingTopOffset, Transform charPositionObject, int charIndex)
    {
        charPositionObject.parent = myRoot;

        currentClimbingPosition[charIndex] = playerTransform.position - myRoot.position;
        charPositionObject.position = playerTransform.position;
        climbingTopOffset[charIndex] = inputClimbingTopOffset;
        climbingDelay[charIndex] = 0.5f;
        climbingStates[charIndex] = ClimbStates.GoingTowardsPosition;
    }

    protected void EnteredTrigger(Collider other)
    {
        if(climbDelay<0)
        {
            climbDelay = 0.1f;
            if(other.GetComponent<PlayerController>().GetIsClimbing()==false)
            {
                other.gameObject.GetComponent<PlayerController>().MountMe(this, climbDirection);
                    
            }
        }
    }

    public virtual void MoveUp(Transform charPositionObject, int charIndex)
    {
        
    }

    public virtual void MoveDown(Transform charPositionObject, int charIndex)
    {

    }

    //void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.layer == 10)
    //    {
    //        if (climbDelay < 0 && other.GetComponent<PlayerController>().GetIsClimbing())
    //        {
    //            Debug.Log("UN------MOUNT");
    //            other.gameObject.GetComponent<PlayerController>().UnMount();
    //            climbDelay = 0.4f;
    //        }
    //    }
    //}

    public Transform GetMyPushyParent()
    {
        return myPushyParent;
    }

    public virtual void Climb(Transform playerTransform, Transform charPositionObject, int charIndex)
    {
        if(climbingStates[charIndex] == ClimbStates.GoingTowardsPosition)
        {
            currentClimbingPosition[charIndex] = Vector3.Lerp(currentClimbingPosition[charIndex], wantedClimbingPosition[charIndex], Time.deltaTime * climbingLerpSpeed);
            charPositionObject.position = myRoot.position + currentClimbingPosition[charIndex];

            climbingDelay[charIndex] -= Time.deltaTime;
            if (climbingDelay[charIndex] < 0)
            {
                if(fromWater)
                {
                    playerTransform.GetComponent<PlayerController>().SetClimbingState(ClimbStates.ClimbingUp1);
                    climbingStates[charIndex] = ClimbStates.ClimbingUp1;
                }
                else
                {
                    playerTransform.GetComponent<PlayerController>().SetClimbingState(ClimbStates.OnClimbable);
                    climbingStates[charIndex] = ClimbStates.OnClimbable;
                }
            }
        }
        else if(climbingStates[charIndex] == ClimbStates.OnClimbable)
        {
            wantedClimbingPosition[charIndex].y = Mathf.Clamp(wantedClimbingPosition[charIndex].y, -10000.0f, -climbingTopOffset[charIndex]);
            currentClimbingPosition[charIndex] = Vector3.Lerp(currentClimbingPosition[charIndex], wantedClimbingPosition[charIndex], Time.deltaTime * climbingLerpSpeed);
            charPositionObject.position = myRoot.position + currentClimbingPosition[charIndex];
        }
        else if(climbingStates[charIndex] == ClimbStates.ClimbingUp1)
        {
            float topPositionMultiplier = 10.0f;//30
            float xPosition = climbTopPositon.x * 0.5f;
            float part1Speed = 1.5f;
           
            if (playerTransform.GetComponent<PlayerController>().GetCharName() == CharName.Girl)
            {
                topPositionMultiplier = 20.0f;//20
                xPosition = -climbTopPositon.x * 0.3f;
                part1Speed = 1.5f;
            }

            currentClimbingPosition[charIndex] = Vector3.Lerp(finalClimbStartPosition[charIndex], new Vector3(xPosition, -climbTopPositon.y * topPositionMultiplier, 0), climbingDelay[charIndex]);
            charPositionObject.position = myRoot.position + currentClimbingPosition[charIndex];
            
            climbingDelay[charIndex] += Time.deltaTime * part1Speed;
            if (climbingDelay[charIndex] > 1.0f)
            {
                finalClimbStartPosition[charIndex] = currentClimbingPosition[charIndex];
                climbingDelay[charIndex] = 0.0f;
                climbingStates[charIndex] = ClimbStates.ClimbingUp2;
                playerTransform.GetComponent<PlayerController>().SetClimbingState(ClimbStates.ClimbingUp2);
            }
        }
        else if (climbingStates[charIndex] == ClimbStates.ClimbingUp2)
        {
            float part2Speed = 3.5f;
            if (playerTransform.GetComponent<PlayerController>().GetCharName() == CharName.Girl)
            {
                part2Speed = 2.0f;
            }

            currentClimbingPosition[charIndex] = Vector3.Lerp(finalClimbStartPosition[charIndex], climbTopPositon, climbingDelay[charIndex]);
            charPositionObject.position = myRoot.position + currentClimbingPosition[charIndex];
            climbingDelay[charIndex] += Time.deltaTime * part2Speed;
            if (climbingDelay[charIndex] > 1.0f)
            {
                playerTransform.GetComponent<PlayerController>().UnMount();
                climbingStates[charIndex] = 0;
            }
        }
    }

    public void SetClimbingState(ClimbStates state, int charIndex)
    {
        climbingStates[charIndex] = state;
    }

    public void ReachedTop(int charIndex)
    {
        finalClimbStartPosition[charIndex] = currentClimbingPosition[charIndex];
        climbingDelay[charIndex] = 0.0f;
        climbingStates[charIndex] = ClimbStates.ClimbingUp1;
    }

    public bool IsReachedTop(int charIndex)
    {
        if (currentClimbingPosition[charIndex].y >= -climbingTopOffset[charIndex] - 0.1f && climbDirection != ClimbDirection.none)
            return true;

        return false;
    }

    public bool IsAlmostReachedTop(int charIndex)
    {
        if (currentClimbingPosition[charIndex].y >= -climbingTopOffset[charIndex] - 0.3f && climbDirection != ClimbDirection.none)
            return true;

        return false;
    }

    public virtual void UnMount(Transform playerTransform, int charIndex)
    {
        climbDelay = 0.5f;
        climbingStates[charIndex] = ClimbStates.UnMounted;
    }

    public virtual void CharJumpedOff(bool isRight, int charIndex)
    {

    }

    protected void SpawnIcon()
    {
        if(GameData.instance)
        {
            iconTransform = Instantiate(GameData.instance.IconTransform, null);
            iconTransform.position = transform.position;
            iconTransform.localScale = iconScale;
            iconTransform.GetComponent<Renderer>().material.mainTexture = icon;
            iconTransform.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0);
            iconTransform.parent = transform;
        }
        
    }

    public Transform GetIconTransform()
    {
        return iconTransform;
    }
}

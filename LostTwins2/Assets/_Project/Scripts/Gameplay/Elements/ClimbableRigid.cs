using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableRigid : Climbable
{
    [SerializeField]
    private Ladder myLadder = default;

    // Start is called before the first frame update
    void Awake()
    {
        base.AwakeInit();
        SpawnIcon();
        if (myPushyParent)
            offsetFromPushyParent = myRoot.position - myPushyParent.position;
    }

    // Update is called once per frame
    void Update()
    {
        base.MyUpdate();

        if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
            return;

        if (myPushyParent)
        {
            myRoot.position = myPushyParent.position + offsetFromPushyParent;
        }

    }

    public override void Mounted(Transform playerTransform, float inputClimbingTopOffset, Transform charPositionObject, int charIndex)
    {
        base.Mounted(playerTransform, inputClimbingTopOffset, charPositionObject, charIndex);

        float wantedY = playerTransform.position.y + 0.25f - myRoot.position.y;
        wantedY = Mathf.Clamp(wantedY, -10000.0f, -climbingTopOffset[charIndex]);
        wantedClimbingPosition[charIndex] = new Vector3(0, wantedY, 0);

        if (myLadder)
            myLadder.Entered(playerTransform);
    }

    public override void UnMount(Transform playerTransform, int charIndex)
    {
        base.UnMount(playerTransform, charIndex);
        if (myLadder)
        {
            myLadder.Exitted(playerTransform);
        }
    }

    public override void Climb(Transform playerTransform, Transform charPositionObject, int charIndex)
    {
        base.Climb(playerTransform, charPositionObject, charIndex);
        
    }

    public override void MoveUp(Transform charPositionObject, int charIndex)
    {
        base.MoveUp(charPositionObject, charIndex);
        wantedClimbingPosition[charIndex].y += Time.deltaTime * climbingSpeed;
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            base.EnteredTrigger(other);
        }
    }
}

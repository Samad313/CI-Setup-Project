using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineHelperFunctions;
using UnityEditor;

public class Rope : Climbable
{
    [SerializeField]
    private Material iconMaterial = default;

    private LineRenderer iconLineRenderer;

    [SerializeField]
    private bool isClimbable = true;

    [SerializeField]
    private LineRenderer myLineRenderer = default;

    [SerializeField]
    private GameObject myOriginalVisual = default;

    [SerializeField]
    private Transform physicsRopeGroup = default;

    [SerializeField]
    private int resolution = default;

    private Transform[] myJoints;
    private Transform ropeEnd;

    private int numJoints = 7;
    private Vector3[] positions;
    private float offset;

    Vector3[] finalPositions;

    [SerializeField]
    private float[] currentT;
    private float ropeLength = 1;

    private Transform[] handRotationGroup;

    private bool isActive = true;

    private Vector3[] savedVelocities;

    private float idleForceDelay = 0.0f;
    private float idleForceDirection = 1.0f;

    private int idleForceCurrentIndex = 1;

    [SerializeField]
    private bool clampRopeTop = false;

    private Transform myPiece;

    [SerializeField] private int numSegments = 6;
    [SerializeField] private GameObject rb;
    [SerializeField] private float idleForce = 1.0f;

    [SerializeField] private Piece connectingPiece = default;
    [SerializeField] private int connectingDirection = 3;
    [SerializeField] private float minTWhenDisconnected = 0.1f;

    void Awake()
    {
        base.AwakeInit();
        numJoints = physicsRopeGroup.childCount - 1;
        myJoints = new Transform[numJoints];
        myJoints[0] = physicsRopeGroup.Find("Top");
        for (int i = 1; i < numJoints; i++)
        {
            myJoints[i] = physicsRopeGroup.Find("RB" + i);
        }
        ropeEnd = myJoints[numJoints - 1].Find("RopeEnd");
        positions = new Vector3[numJoints];
        offset = 1.0f / (float)(resolution);
        finalPositions = new Vector3[resolution + 1];

        ropeLength = (numJoints-1.0f)*0.5f + 0.397f;
        currentT = new float[numPlayers];
        handRotationGroup = new Transform[2];
        myOriginalVisual.SetActive(false);

        savedVelocities = new Vector3[numJoints];

        if (myPushyParent)
            offsetFromPushyParent = myRoot.position - myPushyParent.position;

        if(iconTransform)
        {
            iconTransform.parent = null;
            iconTransform.localScale = new Vector3(iconScale.x, myJoints.Length / 2.0f, iconScale.z);
            iconTransform.parent = transform;
        }

        SpawnIconLR();
    }

    // Start is called before the first frame update
    void Start()
    {
        base.StartInit();

        if(GameplayManager.instance)
            myPiece = GameplayManager.instance.GetPieceHoverTransformFromPosition(new Vector2(myRoot.position.x, myRoot.position.y));

        DrawRope();
    }

    void Update()
    {
        if (!isClimbable)
        {
            DrawRope();
            return;
        }
        
        base.MyUpdate();

        if(GameplayManager.instance)
        {
            if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
                return;
        }
        
        if (!isActive)
            return;

        if (myPushyParent)
        {
            myRoot.position = myPushyParent.position + offsetFromPushyParent;
        }

        if (idleForceDelay < 0)
        {
            myJoints[idleForceCurrentIndex].GetComponent<Rigidbody>().AddForce(new Vector3(idleForceDirection * idleForce, 0, 0));
            idleForceCurrentIndex++;
            if (idleForceCurrentIndex > numJoints - 1)
                idleForceCurrentIndex = 1;

            idleForceDelay = 0.05f;
            idleForceDirection = -1.0f;
            if (Random.value > 0.5f)
                idleForceDirection = 1.0f;
        }
        
        idleForceDelay -= Time.deltaTime;

        DrawRope();
    }

    public override void Mounted(Transform playerTransform, float inputClimbingTopOffset, Transform charPositionObject, int charIndex)
    {
        base.Mounted(playerTransform, inputClimbingTopOffset, charPositionObject, charIndex);

        float wantedY = playerTransform.position.y + 0.25f - myRoot.position.y;
        wantedY = Mathf.Clamp(wantedY, -10000.0f, -climbingTopOffset[charIndex]);
        wantedY += players[charIndex].GetRopeClimbingHandOffset().y;
        
        
       //wantedClimbingPosition[charIndex] = new Vector3(0, wantedY, 0);
        currentT[charIndex] = -wantedY / ropeLength;
        wantedClimbingPosition[charIndex] = SplineHelper.Interp(positions, currentT[charIndex]);
        wantedClimbingPosition[charIndex] -= players[charIndex].GetRopeClimbingHandOffset();
        wantedClimbingPosition[charIndex].z = 0;

        int boneIndex = Mathf.CeilToInt(currentT[charIndex] * numJoints);
        boneIndex = Mathf.Clamp(boneIndex, 0, numJoints-1);

        float forceDirection = 1.0f;
        if (playerTransform.position.x > myJoints[boneIndex].position.x)
            forceDirection = -1.0f;

        float forceMagnitude = players[charIndex].CurrentHorizontalSpeed * 100.0f;
        forceMagnitude = Mathf.Clamp(forceMagnitude, 2.0f, 10.0f);

        myJoints[boneIndex].GetComponent<Rigidbody>().velocity = new Vector3(forceDirection * forceMagnitude, 0, 0);
        handRotationGroup[charIndex] = playerTransform.GetComponent<PlayerController>().GetHandRotationGroup();
    }

    public override void UnMount(Transform playerTransform, int charIndex)
    {
        base.UnMount(playerTransform, charIndex);
        handRotationGroup[charIndex].localEulerAngles = Vector3.zero;
    }

    public override void Climb(Transform playerTransform, Transform charPositionObject, int charIndex)
    {
        if (climbingStates[charIndex] == ClimbStates.OnClimbable || climbingStates[charIndex] == ClimbStates.GoingTowardsPosition)
        {
            wantedClimbingPosition[charIndex] = SplineHelper.Interp(positions, currentT[charIndex]);
            wantedClimbingPosition[charIndex] -= players[charIndex].GetRopeClimbingHandOffset();
            wantedClimbingPosition[charIndex].z = 0;
            wantedClimbingPosition[charIndex].y = Mathf.Clamp(wantedClimbingPosition[charIndex].y, -10000.0f, -climbingTopOffset[charIndex]);

            if (climbingStates[charIndex] == ClimbStates.GoingTowardsPosition)
                currentClimbingPosition[charIndex] = Vector3.Lerp(currentClimbingPosition[charIndex], wantedClimbingPosition[charIndex], Time.deltaTime * climbingLerpSpeed);
            else if (climbingStates[charIndex] == ClimbStates.OnClimbable)
                currentClimbingPosition[charIndex] = wantedClimbingPosition[charIndex];

            charPositionObject.position = myRoot.position + currentClimbingPosition[charIndex];

            Vector3 rotationVector = SplineHelper.GetVectorAtPoint(positions, currentT[charIndex] - 0.25f/ropeLength);

            rotationVector.z = 0;

            float angleX = Vector3.SignedAngle(new Vector3(0, 1, 0), rotationVector, new Vector3(0,0,-1));

            handRotationGroup[charIndex].localEulerAngles = new Vector3(angleX, 0, 0);
        }
        else if (climbingStates[charIndex] == ClimbStates.ClimbingUp1)
        {
            handRotationGroup[charIndex].localEulerAngles = Vector3.zero;
        }

        base.Climb(playerTransform, charPositionObject, charIndex);
    }

    public override void MoveUp(Transform charPositionObject, int charIndex)
    {
        base.MoveUp(charPositionObject, charIndex);
        currentT[charIndex] -= Time.deltaTime * climbingSpeed / ropeLength;

        float currentMinT = 0.0f;
        if(connectingPiece)
        {
            bool isConnected = GameplayManager.instance.IsConnected(connectingPiece, connectingDirection, transform.position);
            if(!isConnected)
                currentMinT = minTWhenDisconnected;
        }

        currentT[charIndex] = Mathf.Clamp(currentT[charIndex], currentMinT, 10.0f);

        int boneIndex = Mathf.CeilToInt(currentT[charIndex] * numJoints);
        boneIndex = Mathf.Clamp(boneIndex, 0, numJoints - 1);

        float forceDirection = 1.0f;
        if (Random.value > 0.5f)
            forceDirection = -1.0f;

        myJoints[boneIndex].GetComponent<Rigidbody>().velocity = new Vector3(forceDirection * 1.0f, 0, 0);
    }

    public override void MoveDown(Transform charPositionObject, int charIndex)
    {
        base.MoveDown(charPositionObject, charIndex);
        currentT[charIndex] += Time.deltaTime * climbingSpeed / ropeLength;

        int boneIndex = Mathf.CeilToInt(currentT[charIndex] * numJoints);
        boneIndex = Mathf.Clamp(boneIndex, 0, numJoints - 1);

        float forceDirection = 1.0f;
        if (Random.value > 0.5f)
            forceDirection = -1.0f;

        myJoints[boneIndex].GetComponent<Rigidbody>().velocity = new Vector3(forceDirection * 1.0f, 0, 0);
        if(currentT[charIndex]>1.0f)
        {
            charPositionObject.GetComponent<PlayerController>().UnMount();
        }
    }

    public void EnteredRopeColliders(Collider other)
    {
        if(isClimbable)
            base.EnteredTrigger(other);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if(Input.GetKeyDown(KeyCode.A))
        //{
        //    //GetComponent<Rigidbody>(). = new Vector3(0, 0, -1000000000);
        //    GetComponent<Rigidbody>().velocity = new Vector3(-10, 0, 0);
        //}
        //else if (Input.GetKeyDown(KeyCode.D))
        //{
        //    //GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 1000000000);
        //    GetComponent<Rigidbody>().velocity = new Vector3(10, 0, 0);
        //}
    }

    private void DrawRope()
    {
        float topClampY = 10000.0f;
        if(myPiece)
        {
            topClampY = myPiece.parent.GetComponent<Piece>().PieceTopY;
            topClampY = topClampY - myRoot.position.y;
        }
        
        int startingIndex = 0;

        if(clampRopeTop)
        {
            while (myJoints[startingIndex].localPosition.y > topClampY)
            {
                startingIndex++;
                if(startingIndex> myJoints.Length-1)
                {
                    startingIndex = 0;
                    break;
                }
            }
        }
        
        int numVisualJoint = numJoints;
        if(startingIndex>0)
        {
            numVisualJoint = numJoints - startingIndex;
        }

        myLineRenderer.positionCount = resolution+1;
        positions = new Vector3[numVisualJoint];
        positions[0] = new Vector3(myJoints[0].localPosition.x, myJoints[0].localPosition.y, 0);
        if (startingIndex > 0)
        {
            positions[0] = new Vector3(myJoints[startingIndex].localPosition.x, myJoints[startingIndex].localPosition.y, 0);
            positions[0] = new Vector3(positions[0].x, Mathf.Clamp(positions[0].y, -1000, topClampY));
        }

        Vector3 midPoint;
        for (int i = 1; i < numVisualJoint - 1; i++)
        {
            midPoint = (myJoints[i+startingIndex].localPosition + myJoints[i+1+startingIndex].localPosition) / 2.0f;
            positions[i] = new Vector3(midPoint.x, midPoint.y, 0);
        }

        positions[numVisualJoint - 1] = new Vector3(ropeEnd.position.x, ropeEnd.position.y, 0);
        positions[numVisualJoint - 1] = positions[numVisualJoint - 1] - myRoot.position;

        positions = SplineHelper.AddSplineEndPoints(positions);

        for (int i = 0; i < resolution+1; i++)
        {
            finalPositions[i] = SplineHelper.Interp(positions, i * offset);
        }

        myLineRenderer.SetPositions(finalPositions);


        iconLineRenderer.positionCount = resolution + 1;
        iconLineRenderer.SetPositions(finalPositions);
    }

    public void SetActive(bool value)
    {
        isActive = value;
        if (value == true)
        {
            for (int i = 0; i < myJoints.Length; i++)
            {
                myJoints[i].GetComponent<Rigidbody>().isKinematic = false;
                myJoints[i].GetComponent<Rigidbody>().velocity = savedVelocities[i];
            }
        }
        else
        {
            for (int i = 0; i < myJoints.Length; i++)
            {
                myJoints[i].GetComponent<Rigidbody>().isKinematic = true;
                savedVelocities[i] = myJoints[i].GetComponent<Rigidbody>().velocity;
                myJoints[i].GetComponent<Rigidbody>().velocity = Vector3.zero;

            }
        }
    }

    public override void CharJumpedOff(bool isRight, int charIndex)
    {
        int boneIndex = Mathf.CeilToInt(currentT[charIndex] * numJoints);
        boneIndex = Mathf.Clamp(boneIndex, 0, numJoints - 1);

        float forceDirection = 1.0f;
        if (isRight)
            forceDirection = -1.0f;

        myJoints[boneIndex].GetComponent<Rigidbody>().velocity = new Vector3(forceDirection * 5.0f, 0, 0);
    }

    public Vector3 GetCenterPosition()
    {
        return myLineRenderer.transform.position;
        //return myJoints[myJoints.Length/2].position;
    }

    private void SpawnIconLR()
    {
        iconTransform = Instantiate(myLineRenderer.transform, null);
        iconTransform.parent = myLineRenderer.transform;
        iconTransform.localPosition = Vector3.zero;
        iconTransform.localRotation = Quaternion.identity;
        iconTransform.localScale = Vector3.one;

        iconLineRenderer = iconTransform.GetComponent<LineRenderer>();
        iconLineRenderer.widthMultiplier = 3.0f;

        if (iconMaterial)
        {
            iconLineRenderer.sharedMaterial = iconMaterial;
            iconLineRenderer.sharedMaterial.color = new Color(1, 1, 1, 0);
        }
    }

    public void ReplaceRope(Transform previousRope)
    {
        myRoot.position = previousRope.position;
        List<Transform> ropeColliders = new List<Transform>();
        foreach(Transform child in previousRope)
        {
            if (child.name.Contains("RopeCollider"))
                ropeColliders.Add(child);
        }

        myRoot.Find("RopeCollider").localPosition = ropeColliders[0].localPosition;
        myRoot.Find("RopeCollider").localEulerAngles = ropeColliders[0].localEulerAngles;
        myRoot.Find("RopeCollider").localScale = ropeColliders[0].localScale;

        for (int i = 1; i < ropeColliders.Count; i++)
        {
            Transform ropeCollider = Instantiate(myRoot.Find("RopeCollider"), myRoot);
            ropeCollider.localPosition = ropeColliders[i].localPosition;
            ropeCollider.localEulerAngles = ropeColliders[i].localEulerAngles;
            ropeCollider.localScale = ropeColliders[i].localScale;
        }

        climbDirection = previousRope.Find("Collider").GetComponent<Rope>().climbDirection;
        clampRopeTop = previousRope.Find("Collider").GetComponent<Rope>().clampRopeTop;
        numSegments = previousRope.Find("PhysicsRope").childCount - 2;
        resolution = numSegments * 2;
        myRoot.name = previousRope.name;
        myRoot.parent = previousRope.parent;
        GenerateRope();

#if UNITY_EDITOR
        for (int i = 0; i < ropeColliders.Count; i++)
        {
            EditorUtility.SetDirty(ropeColliders[i].gameObject);
        }
        EditorUtility.SetDirty(previousRope.gameObject);
        EditorUtility.SetDirty(gameObject);
#endif
        DestroyImmediate(previousRope.gameObject);
    }

    public void GenerateRope()
    {
        if(numSegments<2)
        {
            return;
        }

        int maxIndex = physicsRopeGroup.childCount - 1;
        for (int i = 2; i < maxIndex; i++)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(physicsRopeGroup.Find("RB" + i).gameObject);
#endif
            DestroyImmediate(physicsRopeGroup.Find("RB"+i).gameObject);
        }

        for (int i = 0; i < numSegments-1; i++)
        {
            GameObject newRB = null;
#if UNITY_EDITOR
            newRB = PrefabUtility.InstantiatePrefab(rb) as GameObject;
#endif
            Transform tempTransform = newRB.transform;
            tempTransform.parent = physicsRopeGroup;
            tempTransform.localPosition = new Vector3(0, -0.85f - 0.5f*i, 0);
            tempTransform.name = "RB"+(i+2);
            tempTransform.GetChild(0).name = "CL" + (i + 2);
            tempTransform.GetComponent<HingeJoint>().connectedBody = physicsRopeGroup.Find("RB"+(i+1)).GetComponent<Rigidbody>();
#if UNITY_EDITOR
            EditorUtility.SetDirty(tempTransform.gameObject);
#endif
        }

        Transform ropeEnd = (new GameObject("RopeEnd")).transform;
        ropeEnd.parent = physicsRopeGroup.Find("RB"+numSegments);
        ropeEnd.localPosition = new Vector3(0, -0.6f, 0);
        ropeEnd.localScale = Vector3.one;
#if UNITY_EDITOR
        EditorUtility.SetDirty(ropeEnd.gameObject);
#endif

        for (int i = 0; i < numSegments; i++)
        {
            Rigidbody tempRB = physicsRopeGroup.Find("RB"+(i+1)).GetComponent<Rigidbody>();
            tempRB.constraints = RigidbodyConstraints.FreezePositionZ;
            //if(numSegments<=10)
            //{
            //    tempRB.constraints = RigidbodyConstraints.None;
            //}
            //else
            //{
            //    tempRB.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            //}
        }

        transform.parent.Find("VisualGroup/Main").localPosition = new Vector3(0, -numSegments/4.0f, 0);
        transform.parent.Find("VisualGroup/Main").localScale = new Vector3(0.15f, numSegments / 2.0f, 1);
        transform.localPosition = new Vector3(0, -numSegments / 4.0f, 0);
        transform.localScale = new Vector3(0.4f, numSegments / 2.0f, 0);

        if(climbDirection==ClimbDirection.right)
        {
            Vector3 tempVector = transform.parent.Find("RopeCollider").localPosition;
            tempVector.x = Mathf.Abs(tempVector.x);
            transform.parent.Find("RopeCollider").localPosition = tempVector;

            tempVector = transform.parent.Find("VisualGroup/Head").localPosition;
            tempVector.x = Mathf.Abs(tempVector.x);
            transform.parent.Find("VisualGroup/Head").localPosition = tempVector;

            tempVector = transform.parent.Find("VisualGroup/Head").localScale;
            tempVector.x = -Mathf.Abs(tempVector.x);
            transform.parent.Find("VisualGroup/Head").localScale = tempVector;
        }
        else
        {
            Vector3 tempVector = transform.parent.Find("RopeCollider").localPosition;
            tempVector.x = -Mathf.Abs(tempVector.x);
            transform.parent.Find("RopeCollider").localPosition = tempVector;

            tempVector = transform.parent.Find("VisualGroup/Head").localPosition;
            tempVector.x = -Mathf.Abs(tempVector.x);
            transform.parent.Find("VisualGroup/Head").localPosition = tempVector;

            tempVector = transform.parent.Find("VisualGroup/Head").localScale;
            tempVector.x = Mathf.Abs(tempVector.x);
            transform.parent.Find("VisualGroup/Head").localScale = tempVector;
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(transform.parent.Find("VisualGroup/Main").gameObject);
        EditorUtility.SetDirty(transform.parent.Find("VisualGroup/Head").gameObject);
        EditorUtility.SetDirty(transform.parent.Find("RopeCollider").gameObject);
        EditorUtility.SetDirty(gameObject);
#endif
    }
}

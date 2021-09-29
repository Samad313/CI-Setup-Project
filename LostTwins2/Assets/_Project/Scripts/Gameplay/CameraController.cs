using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float zoomedInFOV = 26.5f;

    [SerializeField]
    private float zoomedOutFOV = 2.0f;

    [SerializeField]
    private float zoomedOutZ = -1050.0f;

    private Vector3 zoomedInPosition = default;

    [SerializeField]
    private Camera myCamera = default;

    [SerializeField]
    private float playerFollowSpeed = 5.0f;

    [SerializeField]
    private Vector3 offsetFromPlayer = new Vector3(0, 3.0f, -23.5f);

    private Vector4 clamps = default;
    private Vector4 defaultClampsForLevel1 = default;

    private float aspectRatio = 1.778f;
    private float clampXIncrementValue = 0;

    [SerializeField]
    private bool clampInCurrentPiece = true;

    private Quaternion wantedRotation = default;
    private Quaternion currentRotation = Quaternion.identity;

    private Quaternion zoomedOutRotation = default;

    private bool followingObject = false;
    private Transform transformToFollow = default;

    private float zoomedOutHorizontalDistance = default;
    private float zoomedInHorizontalDistance = default;

    [SerializeField]
    private Vector3[] offsetFromFinalGate = default;

    [SerializeField]
    private float[] waitTimesToSetAllOffets = default;

    private float birdFollowSpeed = 0.0f;

    private bool isFTUE = false;

    [SerializeField]
    private Vector3 ftueBounds = default;

    private Vector3 currentPiecePosition = default;

    private bool dontFollowAnything = false;

    [SerializeField]
    private Vector3 startingOffset = default;

    private Vector3 startingRotationOffset = default;

    private float extraFOV = default;

    private float comingDownLerp = default;

    [SerializeField]
    private GameObject titleTextCamera = default;

    private List<Transform> transformsToFollowAtStart = new List<Transform>();

    [SerializeField]
    private GameObject firstLevelTopSkyGroup = default;

    [SerializeField]
    private CameraPointsController cameraPointsController = default;

    [SerializeField]
    private Transform debugTransformToFollow;

    [SerializeField]
    private float cameraLerpSpeedWhenNearConnectedGates = 0.5f;

    private float currentCameraLerpSpeedWhenNearConnectedGates = 1.0f;

    private bool characterSwitching = false;

    [SerializeField]
    private float clampY = 10.5f;

    [SerializeField]
    private Vector3 cameraRotationOffset = default;

    #region Getters
    public Camera MainCamera
    {
        get { return myCamera; }
    }


    #endregion

    [SerializeField] private Vector3[] customDebugStartingPositions;
    [SerializeField] private float customDebugFOV = 26.5f;

    public void Init()
    {
        isFTUE = GameplayManager.instance.IsFTUE;

        aspectRatio = 16.0f / 9.0f;
        float clampXIncrementLerpValue = Mathf.InverseLerp(1.34f, 2.16f, aspectRatio);
        clampXIncrementValue = Mathf.Lerp(5.3f, 1.0f, clampXIncrementLerpValue);
        defaultClampsForLevel1 = new Vector4(12.0f - clampXIncrementValue, 12.0f + clampXIncrementValue, -clampY, clampY);

        aspectRatio = (Screen.width * 1.0f) / (Screen.height * 1.0f);
        clampXIncrementLerpValue = Mathf.InverseLerp(1.34f, 2.16f, aspectRatio);
        clampXIncrementValue = Mathf.Lerp(5.3f, 1.0f, clampXIncrementLerpValue);
        clamps = new Vector4(12.0f - clampXIncrementValue, 12.0f + clampXIncrementValue, -clampY, clampY);

        
        int numPieces = 3;
        if(isFTUE==false)
        {
            numPieces = GameplayManager.instance.GetNumPieces();
        }

        if (aspectRatio < 1.9f)
        {
            zoomedOutFOV = 1.9f;
            if (numPieces > 3)
                zoomedOutFOV = 2.4f;

            if (aspectRatio < 1.5f)
            {
                zoomedOutFOV = 2.1f;
                if(numPieces>3)
                    zoomedOutFOV = 3.2f;
            }
        }
        
        zoomedOutRotation = Quaternion.identity;

        cameraPointsController.Init();

        if (isFTUE==false)
        {
            zoomedOutHorizontalDistance = (zoomedOutZ + offsetFromPlayer.z) * -1.0f * Mathf.Tan(zoomedOutFOV * Mathf.Deg2Rad);
            zoomedInHorizontalDistance = offsetFromPlayer.z * -1.0f * Mathf.Tan((zoomedInFOV + cameraPointsController.GetAdditionalFOV()) * Mathf.Deg2Rad);

            if(GameplayManager.instance.ZoomedInStart)
            {

            }
            else
            {
                startingOffset = Vector3.zero;
                currentRotation = Quaternion.identity;
            }
        }

        if (customDebugStartingPositions.Length > 0)
        {
            StartCoroutine("MoveCameraThroughCustomDebugStartingPositions");
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(GameplayManager.instance)
        {
            if (GameplayManager.instance.LevelCompleted)
            {
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            clampInCurrentPiece = !clampInCurrentPiece;
        }

        if (customDebugStartingPositions.Length > 0)
        {
            return;
        }

        if (GameplayManager.instance==null || GameplayManager.instance.ZoomStatus==ZoomStatuses.ZoomedIn)
        {
            FollowCamera(false);
        }
    }

    public void DoTasksBeforeZoomOut()
    {
        zoomedInPosition = transform.position;
        cameraPointsController.CalculateValues(GameplayManager.instance.CurrentPlayerTransform.position);
        zoomedOutHorizontalDistance = (zoomedOutZ + offsetFromPlayer.z + cameraPointsController.GetAdditionalOffset().z) * -1.0f * Mathf.Tan(zoomedOutFOV * Mathf.Deg2Rad);
        zoomedInHorizontalDistance = (offsetFromPlayer.z + cameraPointsController.GetAdditionalOffset().z) * -1.0f * Mathf.Tan((zoomedInFOV + cameraPointsController.GetAdditionalFOV()) * Mathf.Deg2Rad);
    }

    public void DoTasksBeforeZoomIn()
    {
        myCamera.fieldOfView = zoomedOutFOV;
    }

    public void DoTasksWhileZoominInOut(float t)
    {
        myCamera.fieldOfView = Mathf.Lerp(zoomedInFOV + cameraPointsController.GetAdditionalFOV(), zoomedOutFOV, t);

        float currentHorizontalDistance = Mathf.Lerp(zoomedInHorizontalDistance, zoomedOutHorizontalDistance, t);
        float positionZ = -currentHorizontalDistance / Mathf.Tan(myCamera.fieldOfView * Mathf.Deg2Rad);
        float positionX = Mathf.Lerp(zoomedInPosition.x, 0, t);
        float positionY = Mathf.Lerp(zoomedInPosition.y, 0, t);
        transform.position = new Vector3(positionX, positionY, positionZ);
        transform.rotation = Quaternion.Lerp(currentRotation, zoomedOutRotation, Easing.Ease(Equation.QuadEaseOut, t, 0, 1, 1));
    }

    public void SetPositionNow()
    {
        FollowCamera(true);
    }

    private void FollowCamera(bool setNow)
    {
        if (dontFollowAnything)
            return;

        SetCurrentPiecePosition();
        if(followingObject)
        {
            FollowObjectSetPosition(setNow);
        }
        else
        {
            FollowPlayerSetPosition(setNow);
        }

        FollowSetRotation(setNow);
    }

    private void SetCurrentPiecePosition()
    {
        currentPiecePosition = ftueBounds;

        if (isFTUE == false && GameplayManager.instance)
        {
            Vector3 originalCharacterPosition = GameplayManager.instance.CurrentPlayerTransform.position;
            if(followingObject)
            {
                originalCharacterPosition = transformToFollow.position;
            }
            currentPiecePosition = GameplayManager.instance.GetPieceHoverTransformFromPosition(new Vector2(originalCharacterPosition.x, originalCharacterPosition.y)).position;
        }
    }

    private void FollowObjectSetPosition(bool setNow)
    {
        SetClamps(new Vector2(transformToFollow.position.x, transformToFollow.position.y));
        if (setNow)
        {
            transform.position = ClampPosition(transformToFollow.position + offsetFromPlayer) + startingOffset;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, ClampPosition(transformToFollow.position + offsetFromPlayer) + startingOffset, Time.deltaTime * 5.0f);
        }
    }

    private void FollowPlayerSetPosition(bool setNow)
    {
        Vector3 originalCharacterPosition = Vector3.zero;
        if(debugTransformToFollow)
        {
            originalCharacterPosition = debugTransformToFollow.position;
        }
        else
        {
            originalCharacterPosition = GameplayManager.instance.CurrentPlayerTransform.position;
        }

        Vector3 additionalOffset = Vector3.zero;
        float additionalFOV = 0.0f;

        if(cameraPointsController)
        {
            cameraPointsController.CalculateValues(originalCharacterPosition);
            additionalOffset = cameraPointsController.GetAdditionalOffset();
            additionalFOV = cameraPointsController.GetAdditionalFOV();
        }

        Vector3 characterPosition = originalCharacterPosition + additionalOffset;

        myCamera.fieldOfView = zoomedInFOV + additionalFOV + extraFOV;

        characterPosition.z = additionalOffset.z;
        SetClamps(new Vector2(originalCharacterPosition.x, originalCharacterPosition.y));
        if(setNow)
        {
            transform.position = ClampPosition(characterPosition + offsetFromPlayer)+ startingOffset;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, ClampPosition(characterPosition + offsetFromPlayer)+ startingOffset, Time.deltaTime * playerFollowSpeed * currentCameraLerpSpeedWhenNearConnectedGates);
        }
    }

    public void SetPositionValues()
    {
        Vector3 originalCharacterPosition = GameplayManager.instance.CurrentPlayerTransform.position;

        cameraPointsController.CalculateValues(originalCharacterPosition);
        Vector3 additionalOffset = cameraPointsController.GetAdditionalOffset();
        Vector3 characterPosition = originalCharacterPosition + additionalOffset;

        characterPosition.z = additionalOffset.z;
        SetClamps(new Vector2(originalCharacterPosition.x, originalCharacterPosition.y));
        zoomedInPosition = ClampPosition(characterPosition + offsetFromPlayer);
        Vector3 piecePosition = GameplayManager.instance.GetPieceHoverTransformFromPosition(new Vector2(originalCharacterPosition.x, originalCharacterPosition.y)).position;
        float lerpValue = Mathf.InverseLerp(piecePosition.y - offsetFromPlayer.y, piecePosition.y + 4.5f, zoomedInPosition.y);
        wantedRotation = Quaternion.Euler(Mathf.Lerp(0, 5, lerpValue), 0, 0);
        currentRotation = wantedRotation;
    }

    private void FollowSetRotation(bool setNow)
    {
        float lerpValue = Mathf.InverseLerp(currentPiecePosition.y - offsetFromPlayer.y, currentPiecePosition.y + 4.5f, transform.position.y-startingOffset.y);
        wantedRotation = Quaternion.Euler(Mathf.Lerp(0, 5, lerpValue), 0, 0);

        wantedRotation = Quaternion.Lerp(Quaternion.identity, wantedRotation, comingDownLerp);
        if(setNow)
        {
            currentRotation = wantedRotation;
        }
        else
        {
            currentRotation = Quaternion.Lerp(currentRotation, wantedRotation, Time.deltaTime * 4.0f);
        }
        transform.rotation = currentRotation * Quaternion.Euler(startingRotationOffset) * Quaternion.Euler(cameraRotationOffset);
    }

    private void SetClamps(Vector2 playerPosition)
    {
        Vector3 piecePosition = ftueBounds;

        Piece currentPlayerPiece = null;
        if (isFTUE==false&&GameplayManager.instance)
        {
            currentPlayerPiece = GameplayManager.instance.GetPieceFromPosition(playerPosition);
            piecePosition = currentPlayerPiece.transform.Find("HoverGroup").position;
        }
        

        clamps.x = piecePosition.x - clampXIncrementValue;
        clamps.y = piecePosition.x + clampXIncrementValue;
        clamps.z = piecePosition.y - offsetFromPlayer.y;
        clamps.w = piecePosition.y + 4.5f;

        float lerpTo = 1.0f;
        

        if (isFTUE||debugTransformToFollow)
        {
            clamps.x = -piecePosition.x - clampXIncrementValue;
            clamps.y = piecePosition.x + clampXIncrementValue;
        }

        if(isFTUE==false && GameplayManager.instance)
        {
            if(currentPlayerPiece.GetAreSidesMatched(0)==true)
            {
                //clamps.x = -12.0f - clampXIncrementValue;
                float t = Mathf.InverseLerp(piecePosition.x - 6.5f, piecePosition.x - 12.0f, playerPosition.x);
                //t = Easing.Ease(Equation.CubicEaseIn, t, 0, 1, 1);
                clamps.x = Mathf.Lerp(piecePosition.x - clampXIncrementValue, -1.0f - clampXIncrementValue, t);
                //currentCameraLerpSpeedWhenNearConnectedGates = 0.4f;// Mathf.Lerp(1.0f, 0.4f, t);
                if (t > 0)
                    lerpTo = 0.3f;
            }
            if (currentPlayerPiece.GetAreSidesMatched(1) == true)
            {
                //clamps.y = 12.0f + clampXIncrementValue;
                float t = Mathf.InverseLerp(piecePosition.x + 6.5f, piecePosition.x + 12.0f, playerPosition.x);
                //t = Easing.Ease(Equation.CubicEaseIn, t, 0, 1, 1);
                clamps.y = Mathf.Lerp(piecePosition.x + clampXIncrementValue, 1.0f + clampXIncrementValue, t);
                //currentCameraLerpSpeedWhenNearConnectedGates = 0.4f;// Mathf.Lerp(1.0f, 0.4f, t);
                if (t > 0)
                    lerpTo = 0.3f;
            }
            if (currentPlayerPiece.GetAreSidesMatched(2) == true)
            {
                clamps.z = -clampY;
            }
            if (currentPlayerPiece.GetAreSidesMatched(3) == true)
            {
                clamps.w = clampY;
            }
        }

        if (lerpTo == 1.0f)
            currentCameraLerpSpeedWhenNearConnectedGates = Mathf.Lerp(currentCameraLerpSpeedWhenNearConnectedGates, lerpTo, Time.deltaTime * 0.5f);
        else
            currentCameraLerpSpeedWhenNearConnectedGates = lerpTo;

        if(characterSwitching)
        {
            currentCameraLerpSpeedWhenNearConnectedGates = 1.0f;
        }  
    }

    public void CharacterSwitched()
    {
        characterSwitching = true;
        //SetPositionValues();
        float distance = (transform.position - zoomedInPosition).magnitude;
        
        StartCoroutine("SetCharacterSwitchingFalseAfterDelay", distance);
    }

    private IEnumerator SetCharacterSwitchingFalseAfterDelay(float distance)
    {
        yield return new WaitForSeconds(distance / 40.0f);
        characterSwitching = false;
    }

    private Vector3 ClampPosition(Vector3 inputPosition)
    {
        float clampedX = Mathf.Clamp(inputPosition.x, clamps.x, clamps.y);
        float clampedY = Mathf.Clamp(inputPosition.y, clamps.z, clamps.w);
        if(GameplayManager.instance)
        {
            if (GameplayManager.instance.FirstLevel && IsCameraMovingDown())
            {
                clampedX = Mathf.Clamp(inputPosition.x, defaultClampsForLevel1.x, defaultClampsForLevel1.y);
                clampedY = Mathf.Clamp(inputPosition.y, defaultClampsForLevel1.z, defaultClampsForLevel1.w);
            }
        }
        
        return new Vector3(clampedX, clampedY, inputPosition.z);
    }

    public void StartFollowing(Transform givenTransformToFollow)
    {
        transformToFollow = givenTransformToFollow;
        followingObject = true;
    }

    public void StopFollowing()
    {
        followingObject = false;
    }

    public bool GetDontFollowAnything()
    {
        return dontFollowAnything;
    }

    public void LevelFinishedSequence()
    {
        StartCoroutine("LevelFinishedSequenceCoroutine");
    }

    private IEnumerator LevelFinishedSequenceCoroutine()
    {
        float currentWaitTimesToSetAllOffets;

        

        for (int i = 0; i < offsetFromFinalGate.Length; i++)
        {
            if(i<offsetFromFinalGate.Length-1)
            {
                currentWaitTimesToSetAllOffets = waitTimesToSetAllOffets[i+1];
            }
            else
            {
                currentWaitTimesToSetAllOffets = 0;
            }
            
            while (currentWaitTimesToSetAllOffets > 0)
            {
                currentWaitTimesToSetAllOffets -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (i == 0)
            {
                //StartCoroutine("SetRotationToZero");
            }

            Vector3 finalPosition = GameplayManager.instance.FinalPortal.transform.position + offsetFromFinalGate[i];

            //if (GameplayManager.instance.IsFTUE)
            //{
            //    finalPosition = GameplayManager.instance.FinalPortal.transform.position + new Vector3(0, 2.5f, offsetFromFinalGate.z);
            //}
            Vector3 startPosition = transform.position;
            Quaternion startingRotation = transform.localRotation;
            float t = 0;
            while (t<1.0f)
            {
                transform.position = Vector3.Lerp(startPosition, finalPosition, t);
                transform.localRotation = Quaternion.Lerp(startingRotation, Quaternion.identity, t);
                t += Time.deltaTime * 0.5f;
                yield return new WaitForEndOfFrame();
            }

            transform.position = finalPosition;
            

            
        }

        
    }

    private IEnumerator SetRotationToZero()
    {
        float t = 0;
        Quaternion startingRotation = transform.localRotation;
        while (t<1.0f)
        {
            transform.localRotation = Quaternion.Lerp(startingRotation, Quaternion.identity, t);
            t += Time.deltaTime * 0.5f;
            yield return new WaitForEndOfFrame();
        }
        transform.localRotation = Quaternion.identity;
    }

    public void FollowBird()
    {
        StartCoroutine("FollowBirdCoroutine");
    }

    private IEnumerator FollowBirdCoroutine()
    {
        Transform phoenix = GameplayManager.instance.Phoenix.transform;
        birdFollowSpeed = 0;
        StartCoroutine("IncreaseBirdFollowSpeedCoroutine");

        float t = 15.0f;
        while (t > 0.0f)
        {
            transform.position = Vector3.Lerp(transform.position, phoenix.position + new Vector3(0, 1.2f, offsetFromFinalGate[0].z), Time.deltaTime * birdFollowSpeed);
            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        phoenix.parent = null;

        //if (GameController.instance && isFTUE == false)
        //{
        //    GameController.instance.LoadNextLevelAsync(phoenix.gameObject);
        //}
    }

    private IEnumerator IncreaseBirdFollowSpeedCoroutine()
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            birdFollowSpeed = Mathf.Lerp(0, 4.0f, t);
            t += Time.deltaTime*0.5f;
            yield return new WaitForEndOfFrame();
        }
    }

    public void DecreaseBirdFollowSpeed()
    {
        StopCoroutine("IncreaseBirdFollowSpeedCoroutine");
        StartCoroutine("DecreaseBirdFollowSpeedCoroutine");
    }

    private IEnumerator DecreaseBirdFollowSpeedCoroutine()
    {
        float t = 0.0f;

        t = 0.0f;
        while (t < 1.0f)
        {
            birdFollowSpeed = Mathf.Lerp(4.0f, 0.0f, t);
            t += Time.deltaTime*0.7f;
            yield return new WaitForEndOfFrame();
        }

        Transform phoenix = GameplayManager.instance.Phoenix.transform;
        transform.position = new Vector3(36.225f, phoenix.position.y+1.2f, phoenix.position.z + offsetFromFinalGate[0].z);

        dontFollowAnything = true;
        birdFollowSpeed = 0.0f;
    }

    private IEnumerator MoveCameraDownAfterFTUE()
    {
        float firstLevelSpeed = 1.0f;
        if (GameplayManager.instance.FirstLevel)
        {
            yield return new WaitForSeconds(2.0f);
            firstLevelSpeed = 0.5f;
        }
        else
        {
            yield return new WaitForSeconds(2.0f);
        }
        
        float speed = 0;

        float endingDogDensity = 0.006f;
        float currentDensity = RenderSettings.fogDensity;
        
        GameplayManager.instance.SetAperture(2.0f, 2.5f, null);
        while (startingOffset.sqrMagnitude>0.1f)
        {
            startingOffset = Vector3.MoveTowards(startingOffset, Vector3.zero, Time.deltaTime * 14.0f* speed* firstLevelSpeed);
            startingRotationOffset = Vector3.MoveTowards(startingRotationOffset, Vector3.zero, Time.deltaTime * 2.0f * speed * firstLevelSpeed);
            extraFOV = Mathf.MoveTowards(extraFOV, 0, Time.deltaTime * 10.0f * speed * firstLevelSpeed);
            comingDownLerp = Mathf.MoveTowards(comingDownLerp, 1.0f, Time.deltaTime * 1.0f* speed* firstLevelSpeed);
            defaultClampsForLevel1 = Vector4.MoveTowards(defaultClampsForLevel1, clamps, Time.deltaTime * 10.0f * speed * firstLevelSpeed);

            if(isFTUE)
            {
                currentDensity = Mathf.MoveTowards(currentDensity, endingDogDensity, Time.deltaTime * 0.1f * speed * firstLevelSpeed);
                RenderSettings.fogDensity = currentDensity;
            }
            

            //ppVolume.weight = Mathf.MoveTowards(ppVolume.weight, 1, Time.deltaTime * 0.25f* speed);
            if (speed < 2.0f)
                speed += Time.deltaTime*3.0f* firstLevelSpeed;
            yield return new WaitForEndOfFrame();
        }

        startingOffset = Vector3.zero;
        startingRotationOffset = Vector3.zero;
        extraFOV = 0.0f;
        comingDownLerp = 1;
        defaultClampsForLevel1 = clamps;
        //ppVolume.weight = 1;
        if (titleTextCamera)
        {
            Destroy(titleTextCamera);
        }

        if (firstLevelTopSkyGroup)
        {
            Destroy(firstLevelTopSkyGroup);
        }

        GameplayManager.instance.IsCameraMovingDown = false;
        if(isFTUE==false)
        {
            StartCoroutine("StartingSequence");
        }
        
    }

    public bool IsCameraMovingDown()
    {
        if(startingOffset.sqrMagnitude<0.5f)
        {
            return false;
        }

        return true;
    }

    private IEnumerator StartingSequence()
    {
        if(customDebugStartingPositions.Length>0)
        {
            yield break;
        }

        if (transformsToFollowAtStart.Count<=0)
        {
            StopFollowing();
            while (GameplayManager.instance.PhoenixAlreadyOnGate == false)
            {
                yield return new WaitForEndOfFrame();
            }

            if (GameplayManager.instance.FirstLevel == false)
            {
                yield return new WaitForSeconds(2.5f);
            }

            GameplayManager.instance.AddPhoenixToElementsList();
            GameplayManager.instance.StartGameplay();
            GameplayManager.instance.FirstTimeZoomingOut();
            yield break;
        }

        float extraDelay = 2.0f;
        while(GameplayManager.instance.PhoenixAlreadyOnGate==false)
        {
            extraDelay = 2.0f;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(2.0f+ extraDelay);
        if(transformsToFollowAtStart.Count>1)
        {
            StartFollowing(transformsToFollowAtStart[1]);
            yield return new WaitForSeconds(2.0f);
        }
        
        StopFollowing();
        GameplayManager.instance.AddPhoenixToElementsList();

        if ((GameplayManager.instance.GetAllPlayerTransforms()[0].position - transformsToFollowAtStart[0].position).magnitude >= 6.0f)
        {
            yield return new WaitForSeconds(2.0f);
        }

        for (int i = 0; i < transformsToFollowAtStart.Count; i++)
        {
            Destroy(transformsToFollowAtStart[i].gameObject);
        }

        transformsToFollowAtStart.Clear();

        GameplayManager.instance.StartGameplay();
        GameplayManager.instance.FirstTimeZoomingOut();
    }

    public void SetTransformsToFollowAtStart()
    {
        //transformsToFollowAtStart = new List<Transform>();
        List<Vector3> positionsToFollowAtStart = new List<Vector3>();

        positionsToFollowAtStart.Add(GameplayManager.instance.FinalPortal.transform.position);
        if((GameplayManager.instance.GetAllPlayerTransforms()[1].position - positionsToFollowAtStart[0]).magnitude<6.0f)
        {
            positionsToFollowAtStart[0] = (positionsToFollowAtStart[0] + GameplayManager.instance.GetAllPlayerTransforms()[1].position) / 2.0f;
            if ((GameplayManager.instance.GetAllPlayerTransforms()[0].position - positionsToFollowAtStart[0]).magnitude < 6.0f)
            {
                positionsToFollowAtStart.Clear();
                return;
            }
        }
        else
        {
            positionsToFollowAtStart.Add(GameplayManager.instance.GetAllPlayerTransforms()[1].position);
        }

        for (int i = positionsToFollowAtStart.Count-1; i >= 1; i--)
        {
            if((positionsToFollowAtStart[i]- GameplayManager.instance.GetAllPlayerTransforms()[0].position).magnitude<6.0f)
            {
                positionsToFollowAtStart.RemoveAt(i);
            }
        }

        for (int i = 0; i < positionsToFollowAtStart.Count; i++)
        {
            transformsToFollowAtStart.Add((new GameObject()).transform);
            transformsToFollowAtStart[i].position = positionsToFollowAtStart[i];
        }

        if (transformsToFollowAtStart.Count > 0)
        {
            StartFollowing(transformsToFollowAtStart[0]);
        }
    }

    public void Skip()
    {
        StopCoroutine("MoveCameraDownAfterFTUE");
        startingOffset = Vector3.zero;
        comingDownLerp = 1;
        defaultClampsForLevel1 = clamps;
        StartCoroutine("DestroyFirstLevelObjects");
        

        StopCoroutine("StartingSequence");
        
        StopFollowing();
    }

    private IEnumerator DestroyFirstLevelObjects()
    {
        yield return new WaitForSeconds(0.5f);

        float t = 0.0f;

        while(t<1.0f)
        {
            if (firstLevelTopSkyGroup)
            {
                firstLevelTopSkyGroup.transform.position = Vector3.MoveTowards(firstLevelTopSkyGroup.transform.position, new Vector3(25, 100, 0), Time.deltaTime * 25.0f);
            }
            else
            {
                break;
            }
            t += Time.deltaTime*0.5f;
            yield return new WaitForEndOfFrame();
        }

        if (titleTextCamera)
        {
            Destroy(titleTextCamera);
        }

        if (firstLevelTopSkyGroup)
        {
            Destroy(firstLevelTopSkyGroup);
        }
    }

    public void MoveCameraSideways()
    {
        StartCoroutine("MoveCameraSidewaysCoroutine");
    }

    private IEnumerator MoveCameraSidewaysCoroutine()
    {
        float waitTime = 3.0f;
        Vector3 finalPosition = transform.position + new Vector3(30, 0, 0);
        while (waitTime>0.0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finalPosition, Time.deltaTime * 3.5f);
            waitTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if(GameController.instance)
        {
            //GameController.instance.LoadLevel(true);
            SceneLoader.Instance.LoadNextSceneInBuild();
        }
    }

    public void MakeCameraPointsChildrenOfPieces()
    {
        cameraPointsController.MakeChildrenOfPieces();
    }

    public void MakeChildrenOfCameraPointsGroup()
    {
        cameraPointsController.MakeChildrenOfCameraPointsGroup();
    }

    public void SetCameraStartingOffset(Vector3 startingOffset)
    {
        this.startingOffset = startingOffset;
    }

    public void SetCameraStartingRotationOffset(Vector3 startingRotationOffset)
    {
        this.startingRotationOffset = startingRotationOffset;
    }

    public void SetCameraStartingFOV(float extraFOV)
    {
        this.extraFOV = extraFOV;
    }

    public void SetCameraMovingDown()
    {
        if (startingOffset.magnitude > 0)
        {
            //ppVolume.weight = 0;
            if (firstLevelTopSkyGroup)
            {
                firstLevelTopSkyGroup.SetActive(true);
            }


            StartCoroutine("MoveCameraDownAfterFTUE");
        }
        else
        {
            if (firstLevelTopSkyGroup)
            {
                Destroy(firstLevelTopSkyGroup);
            }
            comingDownLerp = 1.0f;
            if(GameplayManager.instance.PhoenixAlreadyOnGate)
            {
                StartCoroutine("StartingSequence");
            }
        }
    }

    private IEnumerator MoveCameraThroughCustomDebugStartingPositions()
    {
        float waitTime = 8.0f;

        while(waitTime>0)
        {
            transform.position = customDebugStartingPositions[0];
            waitTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        for (int i = 1; i < customDebugStartingPositions.Length; i++)
        {
            while ((transform.position - customDebugStartingPositions[i]).magnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, customDebugStartingPositions[i], Time.deltaTime * 3.5f);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Water : MonoBehaviour
{

    [System.Serializable]
    public class VisualWaterMeta
    {
        [SerializeField]
        private Transform visualWater;
        [SerializeField]
        private Transform topSurface;
        [SerializeField]
        private Transform bottomSurface;
        [SerializeField]
        private Transform frontSurface;
        //[SerializeField]
        //private Transform light;
        [SerializeField]
        private float scaleSpeed = 0.1f;
        [SerializeField]
        private float endScale = -1f;
        [SerializeField]
        private bool isDisableTopSurface = false;
        [SerializeField]
        private bool isSetTiling = false;
        [SerializeField]
        private bool isMoveTopBoundWithBottomSurface = false;

        public Transform TopSurface
        {
            get { return topSurface; }
        }
        public Transform BottomSurface
        {
            get { return bottomSurface; }
        }
        public Transform FrontSurface
        {
            get { return frontSurface; }
        }
        public Transform VisualWater
        {
            get { return visualWater; }
        }
        //public Transform Light
        //{
        //    get { return light; }
        //}
        public float ScaleSpeed
        {
            get { return scaleSpeed; }
        }
        public float EndScale
        {
            get { return endScale; }
        }
        public bool IsDisableTopSurface
        {
            get { return isDisableTopSurface; }
        }
        public bool IsSetTiling
        {
            get { return isSetTiling; }
        }
        public bool IsMoveTopBoundWithBottomSurface
        {
            get { return isMoveTopBoundWithBottomSurface; }
        }


    }

    #region Exposed Variables
    [SerializeField]
    private Vector3 defaultPosition = default;

    [SerializeField]
    private Vector3 pressedPosition = default;

    [SerializeField]
    private bool canChangePosition = true;

    [SerializeField]
    private bool canChangeScale = false;

    [SerializeField]
    private bool isPieceDependent = false;
    [SerializeField]
    private Transform topBound;
   

    [SerializeField]
    private List<VisualWaterMeta> visualWaterMeta = new List<VisualWaterMeta>();

    //public bool isShow = false;
    [SerializeField]
    private float speed = 3.0f;
    [SerializeField]
    private float fillWaterQuarters = 1f;
    [SerializeField]
    private Piece currentWaterPiece = default;
    [SerializeField]
    private Piece connectedWaterPiece = default;
    [SerializeField]
    private int currentPieceConnectedDirection = 2; //0 left 1 right 2 bottom 3 top
    [SerializeField]
    private int connectedPieceConnectedDirection = 3; //0 left 1 right 2 bottom 3 top

    #endregion

    #region Private Variables

    private Transform myPiece;
    private Vector3 wantedPosition;

    private List<Transform> objectsInsideMe;
    private List<Transform> objectsOutisdeMe;

    //private Transform[] objectThatCanGetInsideMe;
    private Transform leftBound;
    private Transform rightBound;
    private Transform bottomBound;
    
    [SerializeField]
    private bool isStartScale = false;
    private float currentFill = 0f;
    private float remainingQuarters = 0f;
    private int quarterIndex = 1;
    public float endFillScale = 0f;
    private int waterButtonIndex = -1;

    #endregion


    #region Getters
    public bool IsRequiredPieceConnected
    {
        get
        {
            return currentWaterPiece.GetAreSidesMatched(currentPieceConnectedDirection) && connectedWaterPiece.GetAreSidesMatched(connectedPieceConnectedDirection);
        }
    }


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        myPiece = GameplayManager.instance.GetPieceHoverTransformFromPosition(new Vector2(transform.position.x, transform.position.y));
        wantedPosition = defaultPosition;

        Transform[] waterElements = GameplayManager.instance.GetAllWaterObjects();

        Transform[] playerTransforms = GameplayManager.instance.GetAllPlayerTransforms();

        objectsInsideMe = new List<Transform>();
        objectsOutisdeMe = new List<Transform>();

        for (int i = 0; i < playerTransforms.Length; i++)
        {
            objectsOutisdeMe.Add( playerTransforms[i] );
        }

        for (int i = 0; i < waterElements.Length; i++)
        {
            objectsOutisdeMe.Add( waterElements[i] );
        }

        leftBound = transform.Find("Bounds/Left");
        rightBound = transform.Find("Bounds/Right");
        bottomBound = transform.Find("Bounds/Bottom");
        topBound = transform.Find("Bounds/Top");

        if(canChangeScale)
        {
            StartCoroutine("ScaleWater");
        }

        SetObjectTiling();


    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
            return;

        if(canChangePosition)
            transform.position = Vector3.MoveTowards(transform.position, wantedPosition+myPiece.position, Time.deltaTime * speed);

        List<Transform> newObjectGoingInside = new List<Transform>();

        for (int i = 0; i < objectsOutisdeMe.Count; i++)
        {
            if(IsInsideMe(objectsOutisdeMe[i]))
            {
                if(objectsOutisdeMe[i].GetComponent<PlayerController>())
                {
                    if (objectsOutisdeMe[i].GetComponent<PlayerController>().GetDelayBeforeEnteringWater()<0)
                    {
                        newObjectGoingInside.Add(objectsOutisdeMe[i]);
                        SetWaterStateOfObject(objectsOutisdeMe[i], true);
                    }
                }
                else
                {
                    newObjectGoingInside.Add(objectsOutisdeMe[i]);
                    SetWaterStateOfObject(objectsOutisdeMe[i], true);
                }
            }
        }

        for (int i = 0; i < newObjectGoingInside.Count; i++)
        {
            objectsOutisdeMe.Remove(newObjectGoingInside[i]);
        }

        List<Transform> newObjectGoingOutside = new List<Transform>();

        for (int i = 0; i < objectsInsideMe.Count; i++)
        {
            if (!IsInsideMe(objectsInsideMe[i]))
            {
                newObjectGoingOutside.Add(objectsInsideMe[i]);
                SetWaterStateOfObject(objectsInsideMe[i], false);
            }
        }

        for (int i = 0; i < newObjectGoingOutside.Count; i++)
        {
            objectsInsideMe.Remove(newObjectGoingOutside[i]);
            objectsOutisdeMe.Add(newObjectGoingOutside[i]);
        }

        for (int i = 0; i < newObjectGoingInside.Count; i++)
        {
            objectsInsideMe.Add(newObjectGoingInside[i]);
        }

    

    }

    private IEnumerator ScaleWater()
    {
        //if (canChangeScale && isStartScale)
        //{
           

        //}


        yield return new WaitUntil(() => isStartScale);

        if (isPieceDependent)
        {
            if (!IsRequiredPieceConnected)
                yield return new WaitUntil(() => IsRequiredPieceConnected);
        }

        float timeElapsed = 0f;
        
        
        for (int i = 0; i < visualWaterMeta.Count; i++)
        {
            timeElapsed = 0f;
            Vector3 currentWaterScale = visualWaterMeta[i].VisualWater.localScale;
            float currentYScale = visualWaterMeta[i].VisualWater.localScale.y;
            Vector3 currentPos = visualWaterMeta[i].VisualWater.localPosition;
            Vector3 topPointStartPos = topBound.transform.position;
            Vector3 topPointEndPos = default;

            if (fillWaterQuarters <= 1f)
            {
                endFillScale = visualWaterMeta[i].EndScale;
            }

           

            if (visualWaterMeta[i].TopSurface != null)
                visualWaterMeta[i].TopSurface.gameObject.SetActive(true);
          
            while (timeElapsed <= visualWaterMeta[i].ScaleSpeed )
            {
                yield return new WaitUntil(() => isStartScale);
             
                currentWaterScale.y = Mathf.Lerp(currentYScale, endFillScale, timeElapsed / visualWaterMeta[i].ScaleSpeed);

                float scaleDifference = visualWaterMeta[i].VisualWater.localScale.y - currentWaterScale.y;
                //float scaleDifference = visualWaterMeta[i].VisualWater.localScale.y - currentWaterScale.y;
                float offsetPos = scaleDifference / 1.5f;

                if (visualWaterMeta[i].IsMoveTopBoundWithBottomSurface)
                    topPointEndPos = visualWaterMeta[i].BottomSurface.position;
                else
                    topPointEndPos = visualWaterMeta[i].TopSurface.position;

                topBound.transform.position = Vector3.Lerp(topPointStartPos, topPointEndPos, timeElapsed / visualWaterMeta[i].ScaleSpeed);
                currentPos.y = visualWaterMeta[i].VisualWater.localPosition.y - offsetPos;
                visualWaterMeta[i].VisualWater.localScale = currentWaterScale;
                visualWaterMeta[i].VisualWater.localPosition = currentPos;
                //visualWaterMeta[i].Light.position = currentPos;

                timeElapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            if (visualWaterMeta[i].IsDisableTopSurface)
            {
                visualWaterMeta[i].VisualWater.gameObject.SetActive(false);
            }
            topBound.transform.position = new Vector3(topBound.transform.position.x, topBound.transform.position.y + 0.95f, topBound.transform.position.z);

        }

    }

    public void SetTrigger(bool value)
    {
        if(value)
        {
            wantedPosition = pressedPosition;
        }
        else
        {
            wantedPosition = defaultPosition;
        }

    }

    public void SetTrigger(bool value, int buttonIndex)
    {
        if (value)
        {
            if (canChangeScale)
                isStartScale = true;
        }
        else
        {
            if (canChangeScale)
                isStartScale = false;
        }
        if(buttonIndex != waterButtonIndex)
        {
            HandleWaterFillQuarters();
            waterButtonIndex = buttonIndex;
        }
    }

    private void HandleWaterFillQuarters()
    {
        if(fillWaterQuarters > 1)
        {
            if(isPieceDependent)
            {
                if (!IsRequiredPieceConnected)
                    return;
            }

            for (int i = 0; i < visualWaterMeta.Count; i++)
            {
                float quarterToFill = visualWaterMeta[i].EndScale/ fillWaterQuarters;
                endFillScale = quarterToFill * quarterIndex;
                endFillScale = Mathf.Clamp(endFillScale, endFillScale, visualWaterMeta[i].EndScale);

            }

            quarterIndex++;
            StopCoroutine("ScaleWater");
            StartCoroutine("ScaleWater");
        }
    }

    public bool IsInsideMe(Transform inputTransform)
    {
        if (inputTransform.GetComponent<PlayerController>())
        {
            inputTransform.GetComponent<PlayerController>().ClampWaterY();
        }


        if (inputTransform.Find("center"))
            inputTransform = inputTransform.Find("center");

        if(inputTransform.position.x > leftBound.position.x && inputTransform.position.x < rightBound.position.x
            && inputTransform.position.y > bottomBound.position.y && inputTransform.position.y < topBound.position.y)
        {
            return true;
        }

        return false;
    }

    private void SetWaterStateOfObject(Transform objectForChangedState, bool value)
    {
        int nearestSide = 3;
        if(objectForChangedState.GetComponent<PlayerController>())
        {
            objectForChangedState.GetComponent<PlayerController>().SetWaterState(value, this);
            nearestSide = WaterSplashEffect(objectForChangedState.Find("center"), value);
            objectForChangedState.GetComponent<PlayerController>().SetWaterNearestSide(nearestSide);
        }
        else if (objectForChangedState.GetComponent<PushableBox>())
        {
            nearestSide = WaterSplashEffect(objectForChangedState, value);
            objectForChangedState.GetComponent<PushableBox>().SetWaterState(value, this, nearestSide);
        }
    }

    public Transform GetTopBound()
    {
        return topBound;
    }

    public Transform GetBottomBound()
    {
        return bottomBound;
    }

    private int WaterSplashEffect(Transform inputTransform, bool value)
    {
        float smallestDistane = 100000.0f;
        int nearestSide = 0; //0 left 1 right 2 bottom 3 top

        smallestDistane = Mathf.Abs(inputTransform.position.x - leftBound.position.x);
        nearestSide = 0;

        if (Mathf.Abs(inputTransform.position.x - rightBound.position.x) < smallestDistane)
        {
            smallestDistane = Mathf.Abs(inputTransform.position.x - rightBound.position.x);
            nearestSide = 1;
        }

        if (Mathf.Abs(inputTransform.position.y - bottomBound.position.y) < smallestDistane)
        {
            smallestDistane = Mathf.Abs(inputTransform.position.y - bottomBound.position.y);
            nearestSide = 2;
        }

        if (Mathf.Abs(inputTransform.position.y - topBound.position.y) < smallestDistane)
        {
            //smallestDistane = Mathf.Abs(inputTransform.position.y - topBound.position.y);
            nearestSide = 3;
        }

        Vector3 spawnPosition = new Vector3(leftBound.position.x, inputTransform.position.y, 0);
        float rotationZ = 90.0f;


        if (nearestSide == 1)
        {
            spawnPosition = new Vector3(rightBound.position.x, inputTransform.position.y, 0);
            rotationZ = -90.0f;
        }
        else if (nearestSide == 2)
        {
            spawnPosition = new Vector3(inputTransform.position.x, bottomBound.position.y, 0);
            rotationZ = 180.0f;
        }
        else if (nearestSide == 3)
        {
            spawnPosition = new Vector3(inputTransform.position.x, topBound.position.y, 0);
            rotationZ = 1.0f;
        }


        if (inputTransform.parent.GetComponent<PlayerController>())
        {
            if (value)
                FXManager.instance.CharacterDroppedInWater(spawnPosition, rotationZ, inputTransform.parent.GetComponent<PlayerController>().GetCurrentWaterVelocity().magnitude, nearestSide);
            else
                FXManager.instance.CharacterComingOutOfWater(spawnPosition, rotationZ, inputTransform.parent.GetComponent<PlayerController>().GetCurrentWaterVelocity().magnitude);
        }
        else
        {
            if (value)
                FXManager.instance.CharacterDroppedInWater(spawnPosition, rotationZ, 20.0f, nearestSide);
            else
                FXManager.instance.CharacterComingOutOfWater(spawnPosition, rotationZ, 20.0f);
        }

        return nearestSide;
    }

    private void SetObjectTiling()
    {
        Vector2 topSurfaceDefaultTiling = default;
        Vector2 bottomSurfaceDefaultTiling = default;
        Vector2 frontSurfaceDefaultTiling = default;
        Vector2 visualScale = default;

        Vector2 topSurfaceTargetScale = default;
        Vector2 topSurfaceTargetOffset = default;

        Vector2 bottomSurfaceTargetScale = default;
        Vector2 bottomSurfaceTargetOffset = default;

        Vector2 frontSurfaceTargetScale = default;
        Vector2 frontSurfaceTargetOffset = default;

        for (int i = 0; i < visualWaterMeta.Count; i++)
        {
            if (visualWaterMeta[i].IsSetTiling)
            {
                topSurfaceDefaultTiling = GetObjectTiling(visualWaterMeta[i].TopSurface);
                bottomSurfaceDefaultTiling = GetObjectTiling(visualWaterMeta[i].BottomSurface);
                frontSurfaceDefaultTiling = GetObjectTiling(visualWaterMeta[i].FrontSurface);
                visualScale = visualWaterMeta[i].VisualWater.transform.localScale;

                topSurfaceTargetScale.x = visualScale.x * topSurfaceDefaultTiling.x;
                topSurfaceTargetScale.y = visualScale.y * topSurfaceDefaultTiling.y;
                topSurfaceTargetOffset.x = visualScale.x / 2f;
                topSurfaceTargetOffset.y = visualScale.y / 2f;

                bottomSurfaceTargetScale.x = visualScale.x * bottomSurfaceDefaultTiling.x;
                bottomSurfaceTargetScale.y = visualScale.y * bottomSurfaceDefaultTiling.y;
                bottomSurfaceTargetOffset.x = visualScale.x / 2f;
                bottomSurfaceTargetOffset.y = visualScale.y / 2f;

                frontSurfaceTargetScale.x = visualScale.x * frontSurfaceDefaultTiling.x;
                frontSurfaceTargetScale.y = visualScale.y * frontSurfaceDefaultTiling.y;
                frontSurfaceTargetOffset.x = visualScale.x / 2f;
                frontSurfaceTargetOffset.y = visualScale.y / 2f;

                SetTiling(visualWaterMeta[i].TopSurface, topSurfaceTargetScale, topSurfaceTargetOffset);
                SetTiling(visualWaterMeta[i].BottomSurface, bottomSurfaceTargetScale, bottomSurfaceTargetOffset);
                SetTiling(visualWaterMeta[i].FrontSurface, frontSurfaceTargetScale, frontSurfaceTargetOffset);

            }


        }
    }

    private Vector2 GetObjectTiling(Transform objectTiling)
    {
        return objectTiling.GetComponent<Renderer>().material.GetTextureScale("_WaterNormal");
    }
    private Vector2 GetObjectOffset(Transform objectScale)
    {
        return objectScale.GetComponent<Renderer>().material.GetTextureOffset("_WaterNormal");
    }

    private void SetTiling(Transform target, Vector2 targetTiling, Vector2 offset)
    {
        target.GetComponent<Renderer>().material.SetTextureScale("_WaterNormal", targetTiling);
        target.GetComponent<Renderer>().material.SetTextureOffset("_WaterNormal", offset);
        target.GetComponent<Renderer>().material.SetTextureScale("_Foam", targetTiling);
        target.GetComponent<Renderer>().material.SetTextureOffset("_Foam", offset);
    }


}

//#if UNITY_EDITOR
//[CustomEditor(typeof(Water), true)]
//public class WaterOnInspector : Editor
//{
//    Water water;

//    void OnEnable()
//    {
//        water = serializedObject.targetObject as Water;
//    }

//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        if(water.isShow)
//        {

//        }

//    }
//}
//#endif

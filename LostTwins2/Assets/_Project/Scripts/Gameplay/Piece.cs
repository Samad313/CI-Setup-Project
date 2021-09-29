using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum MoveDirection{none, up, down, left, right}

public class Piece : MonoBehaviour
{
    #region Variables

    #region Exposed in Inspector
    [SerializeField] private bool[] leftValues = default;
    [SerializeField] private bool[] rightValues = default;
    [SerializeField] private bool[] topValues = default;
    [SerializeField] private bool[] bottomValues = default;

    [SerializeField] private int myIndex = -1;
    [SerializeField] private int namingIndex = -1;

    [SerializeField] private Transform[] leftDoorways = default;
    [SerializeField] private Transform[] rightDoorways = default;
    [SerializeField] private Transform[] bottomDoorways = default;
    [SerializeField] private Transform[] topDoorways = default;

    [SerializeField] private Renderer[] objectsToFadeInZoomOut;

    public int NamingIndex { get { return namingIndex; } }
    public int MyIndex { get { return myIndex; } }
    #endregion

    #region Normal Variables

    private PiecesManager piecesManager;
    private Transform borderGroup;

    private int indexX = -1;
    private int indexY = -1;
    private bool[] areSidesMatched; //0 left 1 right 2 bottom 3 top
    private bool[] lastAreSidesMatched;
    private bool isConnected = false;

    private Vector2 wantedPiecePosition;
    private float lerpSpeed = 8.0f;
    private bool held = false;

    private Transform hoverGroup;
    private float hoverT = 0.0f;
    private Vector3 hoveGroupWantedTranslation;
    private Quaternion hoveGroupWantedRotation;
    private Vector3 hoverCurrentOffset;

    public bool IsConnected { get { return isConnected; } }
    public Transform HoverGroup { get { return hoverGroup; } }
    public int IndexX { get { return indexX; } }
    public int IndexY { get { return indexY; } }
    public float PieceTopY { get { return hoverGroup.position.y + piecesManager.GetPieceHeight() / 2.0f; } }

    #endregion

    #endregion

    #region Functions

    #region Inits & Update
    void Awake()
    {
        areSidesMatched = new bool[4];
        lastAreSidesMatched = new bool[4];
        hoverGroup = transform.Find("HoverGroup");
        transform.Find("VisualGroup").parent = hoverGroup;
        transform.Find("Overlay").parent = hoverGroup;
        transform.Find("OverlayBlack").parent = hoverGroup;
        borderGroup = transform.Find("borders");
        borderGroup.parent = hoverGroup;
        transform.Find("RaycastCollider").parent = hoverGroup;
        transform.Find("CollidersGroup").parent = hoverGroup;
        transform.Find("SidesGroup").parent = hoverGroup;

        SetHoverCurrentOffset(1.0f);
    }

    public void SetInitialPieceHighlight()
    {
        if (isConnected)
        {
            transform.Find("HoverGroup/Overlay").GetComponent<Renderer>().material.SetFloat("_wave_Horizontal_scroll", 1);
        }
        else
        {
            transform.Find("HoverGroup/Overlay").GetComponent<Renderer>().material.SetFloat("_wave_Horizontal_scroll", -1);
        }

        transform.Find("HoverGroup/Overlay").GetComponent<Renderer>().material.SetFloat("_rotate", 0);
    }

    public void SetPiecesManager(PiecesManager piecesManager)
    {
        this.piecesManager = piecesManager;
    }

    // Start is called before the first frame update
    void Start()
    {
        wantedPiecePosition = transform.position;
        hoverT = myIndex;
    }

    // Update is called once per frame
    void Update()
    {
        float finalLerpSpeed = lerpSpeed;
        if (held)
            finalLerpSpeed = lerpSpeed * 2.0f;

        if(isConnected)
            transform.localPosition = Vector3.Lerp(transform.localPosition, wantedPiecePosition, Time.deltaTime * finalLerpSpeed * 3.0f);
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, wantedPiecePosition, Time.deltaTime * finalLerpSpeed);

        SetHoverGroupPosition();
    }

    #endregion

    #region Hover Group Positioning
    private void SetHoverCurrentOffset(float t)
    {
        //t = 1.0f;
        hoverCurrentOffset = Vector3.Lerp(new Vector3(transform.localPosition.x * 0.2f, transform.localPosition.y * 0.6f, 0), new Vector3(transform.localPosition.x * 0.1f, transform.localPosition.y * 0.3f, 0), t);
    }

    public void ResetMastiOnHoverGroup()
    {
        if (isConnected)
        {
            hoveGroupWantedTranslation = Vector3.zero;
        }
        else
        {
            SetHoverCurrentOffset(0);
            hoveGroupWantedTranslation = hoverCurrentOffset;
            SetHoverCurrentOffset(1);
        }

        hoverGroup.localPosition = hoveGroupWantedTranslation;
        hoveGroupWantedRotation = Quaternion.identity;
        hoverGroup.localRotation = Quaternion.identity;
    }

    private void SetHoverGroupPosition()
    {
        float zoomInOutT = GameplayManager.instance.ZoomInOutLerpValue;
        zoomInOutT = Mathf.InverseLerp(0.3f, 1.0f, zoomInOutT);
        SetHoverCurrentOffset(zoomInOutT);

        if (isConnected)
        {
            hoveGroupWantedTranslation = Vector3.zero;
            hoveGroupWantedRotation = Quaternion.identity;
            //hoverGroup.localScale = Vector3.one;

        }
        else
        {
            float normalizedSine = Mathf.Sin(hoverT) / 2.0f + 0.5f;
            //hoverGroup.localScale = Vector3.one * (1.0f + normalizedSine * 0.2f);
            hoveGroupWantedRotation = Quaternion.Euler(0, 0, Mathf.Sin(hoverT + 0.3f) * 0.6f);
            hoveGroupWantedTranslation = new Vector3(0, Mathf.Sin(hoverT + 1.0f) * 0.6f, 0);
            hoveGroupWantedTranslation = Vector3.Lerp(Vector3.zero, hoveGroupWantedTranslation, zoomInOutT);
            hoveGroupWantedTranslation += hoverCurrentOffset;
            hoverT += Time.deltaTime * 2.0f;
        }

        //hoveGroupWantedTranslation = Vector3.Lerp(Vector3.zero, hoveGroupWantedTranslation, zoomInOutT);
        hoveGroupWantedRotation = Quaternion.Lerp(Quaternion.identity, hoveGroupWantedRotation, zoomInOutT);

        float hoverJoiningSpeed = 5.0f;
        if (isConnected)
            hoverJoiningSpeed = 15.0f;
        if (zoomInOutT < 0.9f)
            hoverJoiningSpeed = 25.0f;

        //hoverGroup.localRotation = hoveGroupWantedRotation;
        //hoverGroup.localPosition = hoveGroupWantedTranslation;
        hoverGroup.localRotation = Quaternion.Lerp(hoverGroup.localRotation, hoveGroupWantedRotation, Time.deltaTime * hoverJoiningSpeed);
        hoverGroup.localPosition = Vector3.Lerp(hoverGroup.localPosition, hoveGroupWantedTranslation, Time.deltaTime * hoverJoiningSpeed);
    }

    public void ResetHoverGroup()
    {
        //return;
        SetHoverGroupPosition();
        //hoveGroupWantedTranslation = Vector3.zero;
        //hoveGroupWantedRotation = Quaternion.identity;
        hoverGroup.localRotation = hoveGroupWantedRotation;
        hoverGroup.localPosition = hoveGroupWantedTranslation;
    }

    #endregion

    #region Setting/Getting Positions
    public void SetWantedPosition(Vector2 positionToSet, bool isHeld)
    {
        wantedPiecePosition = positionToSet;
        held = isHeld;
    }

    public void SetPositionNow(Vector2 positionToSet)
    {
        transform.localPosition = positionToSet;
        wantedPiecePosition = positionToSet;
    }

    public MoveDirection CanMove()
    {
        return piecesManager.CanMove(myIndex);
    }

    public bool CanMoveRight()
    {
        return piecesManager.CanPieceMoveRight(myIndex);
    }

    public bool CanMoveUp()
    {
        return piecesManager.CanPieceMoveUp(myIndex);
    }

    public void MoveToNewPosition(MoveDirection moveDirection)
    {
        if (moveDirection == MoveDirection.down)
        {
            indexY -= 1;
        }
        else if (moveDirection == MoveDirection.up)
        {
            indexY += 1;
        }
        else if (moveDirection == MoveDirection.left)
        {
            indexX -= 1;
        }
        else if (moveDirection == MoveDirection.right)
        {
            indexX += 1;
        }
        piecesManager.ChangePiecePosition(myIndex, moveDirection);
    }

    public Vector2 GetAndSetWantedPosition()
    {
        transform.localPosition = wantedPiecePosition;
        return transform.localPosition;
    }

    public void SetToWantedPosition()
    {
        transform.localPosition = wantedPiecePosition;
    }

    public Vector2 GetPosition()
    {
        return transform.localPosition;
    }

    #endregion

    #region Getting/Setting Values/States
    public void SetIndicesXY(int x, int y)
    {
        indexX = x;
        indexY = y;
    }

    public bool[] GetLeftValues()
    {
        return leftValues;
    }

    public bool[] GetRightValues()
    {
        return rightValues;
    }

    public bool[] GetTopValues()
    {
        return topValues;
    }

    public bool[] GetBottomValues()
    {
        return bottomValues;
    }

    public void SetLeftValues(bool[] inputValues)
    {
        leftValues = new bool[inputValues.Length];
        for (int i = 0; i < inputValues.Length; i++)
        {
            leftValues[i] = inputValues[i];
        }
    }

    public void SetRightValues(bool[] inputValues)
    {
        rightValues = new bool[inputValues.Length];
        for (int i = 0; i < inputValues.Length; i++)
        {
            rightValues[i] = inputValues[i];
        }
    }

    public void SetTopValues(bool[] inputValues)
    {
        topValues = new bool[inputValues.Length];
        for (int i = 0; i < inputValues.Length; i++)
        {
            topValues[i] = inputValues[i];
        }
    }

    public void SetBottomValues(bool[] inputValues)
    {
        bottomValues = new bool[inputValues.Length];
        for (int i = 0; i < inputValues.Length; i++)
        {
            bottomValues[i] = inputValues[i];
        }
    }

    public void SetAreSidesMatched(bool value, int index)
    {
        if (index < 0 || index > 3)
            return;

        areSidesMatched[index] = value;
    }

    public bool GetAreSidesMatched(int index)
    {
        if (index < 0 || index > 3)
            return false;

        return areSidesMatched[index];
    }

    public void SetConnectState()
    {
        isConnected = false;

        for (int i = 0; i < areSidesMatched.Length; i++)
        {
            if (areSidesMatched[i] == true)
            {
                isConnected = true;
                break;
            }
        }
    }

    #endregion

    #region Visuals On/Off

    private void SetVisualParts(bool value, Transform[] doorways)
    {
        for (int i = 0; i < doorways.Length; i++)
        {

            if (doorways[i].Find("connected"))
            {
                doorways[i].Find("connected").gameObject.SetActive(value);
            }

            if (doorways[i].Find("disconnected"))
            {
                doorways[i].Find("disconnected").gameObject.SetActive(!value);
            }
        }
    }




    public void SetAllAsDisconnected()
    {
        borderGroup.Find("border_left").gameObject.SetActive(true);
        borderGroup.Find("border_right").gameObject.SetActive(true);
        borderGroup.Find("border_bottom").gameObject.SetActive(true);
        borderGroup.Find("border_top").gameObject.SetActive(true);

        Material boundaryMaterial = GameData.instance.SidesBlueBoundary;

        borderGroup.Find("border_left").GetComponent<Renderer>().sharedMaterial = boundaryMaterial;
        borderGroup.Find("border_right").GetComponent<Renderer>().sharedMaterial = boundaryMaterial;
        borderGroup.Find("border_bottom").GetComponent<Renderer>().sharedMaterial = boundaryMaterial;
        borderGroup.Find("border_top").GetComponent<Renderer>().sharedMaterial = boundaryMaterial;

        SetVisualParts(false, leftDoorways);
        SetVisualParts(false, rightDoorways);
        SetVisualParts(false, bottomDoorways);
        SetVisualParts(false, topDoorways);
    }

    private void DoorwaysConnected(Transform[] doorways, MoveDirection moveDirection)
    {
        FXManager.instance.ConnectDisconectLineTravel(transform.Find("HoverGroup/Overlay").GetComponent<Renderer>().material, moveDirection, true);

        if(doorways.Length==0)
        {
            DoorwaysConnectDisconnectFX(moveDirection, true);
        }
        else
        {
            DoorwaysConnectDisconnectFX(doorways, true);
        }
    }

    private void DoorwaysConnectDisconnectFX(MoveDirection moveDirection, bool isConnected)
    {
        Vector3 offset = Vector3.zero;
        float pieceWidthBy2 = piecesManager.GetPieceWidth() / 2.0f;
        float pieceHeightBy2 = piecesManager.GetPieceHeight() / 2.0f;

        if (moveDirection==MoveDirection.left)
        {
            offset = new Vector3(-pieceWidthBy2, 0, 0);
        }
        else if (moveDirection == MoveDirection.right)
        {
            offset = new Vector3(pieceWidthBy2, 0, 0);
        }
        else if (moveDirection == MoveDirection.down)
        {
            offset = new Vector3(0, -pieceHeightBy2, 0);
        }
        else if (moveDirection == MoveDirection.up)
        {
            offset = new Vector3(0, pieceHeightBy2, 0);
        }

        if (isConnected)
        {
            FXManager.instance.DoorwaysConnected(transform, offset);
        }
        else
        {
            FXManager.instance.DoorwaysDisconnected(transform, offset);
        }
    }

    private void DoorwaysConnectDisconnectFX(Transform[] doorways, bool isConnected)
    {
        for (int i = 0; i < doorways.Length; i++)
        {
            Transform fxSpawnTransform = doorways[i];
            if (doorways[i].Find("patakha_center"))
            {
                fxSpawnTransform = doorways[i].Find("patakha_center");
            }
            else if (doorways[i].Find("connected"))
            {
                fxSpawnTransform = doorways[i].Find("connected");
            }

            if(isConnected)
            {
                FXManager.instance.DoorwaysConnected(fxSpawnTransform, Vector3.zero);
            }
            else
            {
                FXManager.instance.DoorwaysDisconnected(fxSpawnTransform, Vector3.zero);
            }
        }
    }

    private void DoorwaysDisconnected(Transform[] doorways, MoveDirection moveDirection)
    {
        FXManager.instance.ConnectDisconectLineTravel(transform.Find("HoverGroup/Overlay").GetComponent<Renderer>().material, moveDirection, false);

        if(doorways.Length==0)
        {
            DoorwaysConnectDisconnectFX(moveDirection, false);
        }
        else
        {
            DoorwaysConnectDisconnectFX(doorways, false);
        }
    }

    private bool LastStateOfSideMatched(MoveDirection moveDirection)
    {
        //for (int i = 0; i < doorways.Length; i++)
        //{
        //    if (doorways[i].Find("connected"))
        //    {
        //        return doorways[i].Find("connected").gameObject.activeSelf;
        //    }
        //}

        int index = -1;

        if(moveDirection==MoveDirection.left)
        {
            index = 0;
        }
        else if (moveDirection == MoveDirection.right)
        {
            index = 1;
        }
        else if (moveDirection == MoveDirection.down)
        {
            index = 2;
        }
        else if (moveDirection == MoveDirection.up)
        {
            index = 3;
        }

        return lastAreSidesMatched[index];
    }

    public void SetSidesVisuals()
    {
        bool leftSideWasMatched = LastStateOfSideMatched(MoveDirection.left);
        bool rightSideWasMatched = LastStateOfSideMatched(MoveDirection.right);
        bool bottomSideWasMatched = LastStateOfSideMatched(MoveDirection.down);
        bool topSideWasMatched = LastStateOfSideMatched(MoveDirection.up);

        bool wasAlreadyConnectedToSome = leftSideWasMatched || rightSideWasMatched || bottomSideWasMatched || topSideWasMatched;

        bool isConnectedWithSides = false;

        SetAllAsDisconnected();

        if (areSidesMatched[0] == true)//left
        {
            isConnectedWithSides = true;
            borderGroup.Find("border_left").gameObject.SetActive(false);

            SetVisualParts(true, leftDoorways);

            if (leftSideWasMatched == false && wasAlreadyConnectedToSome==false)
            {
                DoorwaysConnected(leftDoorways, MoveDirection.left);
            }
        }

        if (areSidesMatched[1] == true)//right
        {
            isConnectedWithSides = true;
            borderGroup.Find("border_right").gameObject.SetActive(false);

            SetVisualParts(true, rightDoorways);

            if (rightSideWasMatched == false && wasAlreadyConnectedToSome == false)
            {
                DoorwaysConnected(rightDoorways, MoveDirection.right);
            }
        }

        if (areSidesMatched[2] == true)//bottom
        {
            isConnectedWithSides = true;
            borderGroup.Find("border_bottom").gameObject.SetActive(false);

            SetVisualParts(true, bottomDoorways);

            if (bottomSideWasMatched == false && wasAlreadyConnectedToSome == false)
            {
                DoorwaysConnected(bottomDoorways, MoveDirection.down);
            }
        }

        if (areSidesMatched[3] == true)//top
        {
            isConnectedWithSides = true;
            borderGroup.Find("border_top").gameObject.SetActive(false);

            SetVisualParts(true, topDoorways);

            if (topSideWasMatched == false && wasAlreadyConnectedToSome == false)
            {
                DoorwaysConnected(topDoorways, MoveDirection.up);
            }
        }

        if (isConnectedWithSides)
        {
            Material boundaryMaterial = GameData.instance.SidesYellowBoundary;
            borderGroup.Find("border_left").GetComponent<Renderer>().sharedMaterial = boundaryMaterial;
            borderGroup.Find("border_right").GetComponent<Renderer>().sharedMaterial = boundaryMaterial;
            borderGroup.Find("border_bottom").GetComponent<Renderer>().sharedMaterial = boundaryMaterial;
            borderGroup.Find("border_top").GetComponent<Renderer>().sharedMaterial = boundaryMaterial;
        }

        for (int i = 0; i < 4; i++)
        {
            lastAreSidesMatched[i] = areSidesMatched[i];
        }

        //if(leftOpacity[0].material.GetFloat("_Opacity")>0.9f && leftSideWasMatched)
        //{
        //    FXManager.instance.DoorwaysDisconnected(leftOpacity[0].transform);
        //}

        //if (rightOpacity[0].material.GetFloat("_Opacity") > 0.9f && rightSideWasMatched)
        //{
        //    FXManager.instance.DoorwaysDisconnected(rightOpacity[0].transform);
        //}
    }

    public void SetBorders(bool value)
    {
        borderGroup.gameObject.SetActive(value);
    }

    public void DisconnectFX(MoveDirection moveDirection)
    {
        if(LastStateOfSideMatched(moveDirection))
        {
            if (moveDirection == MoveDirection.left)
            {
                DoorwaysDisconnected(leftDoorways, MoveDirection.left);
            }
            else if (moveDirection == MoveDirection.right)
            {
                DoorwaysDisconnected(rightDoorways, MoveDirection.right);
            }
            else if (moveDirection == MoveDirection.down)
            {
                DoorwaysDisconnected(bottomDoorways, MoveDirection.down);
            }
            else if (moveDirection == MoveDirection.up)
            {
                DoorwaysDisconnected(topDoorways, MoveDirection.up);
            }
        }
    }

    public void DisconnectShashka()
    {
        for (int i = 0; i < 4; i++)
        {
            lastAreSidesMatched[i] = areSidesMatched[i];
        }
        SetAllAsDisconnected();
        isConnected = false;

        Piece connectedPiece = null;
        if (areSidesMatched[0] == true)//left
        {
            DisconnectFX(MoveDirection.left);

            connectedPiece = piecesManager.GetPieceFromIndicesXY(indexX - 1, indexY);
            if(connectedPiece.areSidesMatched[0]==false&& connectedPiece.areSidesMatched[2] == false&& connectedPiece.areSidesMatched[3] == false)
            {
                connectedPiece.DisconnectFX(MoveDirection.right);
                connectedPiece.SetAllAsDisconnected();
                connectedPiece.SetIsConnectedFalse();
                connectedPiece.SetLastAreSidesMatchedFalse();
            }
        }

        if (areSidesMatched[1] == true)//right
        {
            DisconnectFX(MoveDirection.right);

            connectedPiece = piecesManager.GetPieceFromIndicesXY(indexX + 1, indexY);
            if (connectedPiece.areSidesMatched[1] == false && connectedPiece.areSidesMatched[2] == false && connectedPiece.areSidesMatched[3] == false)
            {
                connectedPiece.DisconnectFX(MoveDirection.left);
                connectedPiece.SetAllAsDisconnected();
                connectedPiece.SetIsConnectedFalse();
                connectedPiece.SetLastAreSidesMatchedFalse();
            }
        }

        if (areSidesMatched[2] == true)//bottom
        {
            DisconnectFX(MoveDirection.down);

            connectedPiece = piecesManager.GetPieceFromIndicesXY(indexX, indexY-1);
            if (connectedPiece.areSidesMatched[0] == false && connectedPiece.areSidesMatched[1] == false && connectedPiece.areSidesMatched[2] == false)
            {
                connectedPiece.DisconnectFX(MoveDirection.up);
                connectedPiece.SetAllAsDisconnected();
                connectedPiece.SetIsConnectedFalse();
                connectedPiece.SetLastAreSidesMatchedFalse();
            }
        }

        if (areSidesMatched[3] == true)//top
        {
            DisconnectFX(MoveDirection.up);

            connectedPiece = piecesManager.GetPieceFromIndicesXY(indexX, indexY + 1);
            if (connectedPiece.areSidesMatched[0] == false && connectedPiece.areSidesMatched[1] == false && connectedPiece.areSidesMatched[3] == false)
            {
                connectedPiece.DisconnectFX(MoveDirection.down);
                connectedPiece.SetAllAsDisconnected();
                connectedPiece.SetIsConnectedFalse();
                connectedPiece.SetLastAreSidesMatchedFalse();
            }
        }

        SetLastAreSidesMatchedFalse();
    }

    public void SetLastAreSidesMatchedFalse()
    {
        for (int i = 0; i < 4; i++)
        {
            lastAreSidesMatched[i] = false;
        }
    }

    public void ZoomingInOut(float t)
    {
        for (int i = 0; i < objectsToFadeInZoomOut.Length; i++)
        {
            objectsToFadeInZoomOut[i].material.SetFloat("_opacity", 1.0f-t);
        }
    }

    public void SetIsConnectedFalse()
    {
        isConnected = false;
    }
    #endregion

    #endregion

}

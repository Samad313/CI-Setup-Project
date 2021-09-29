using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesManager : MonoBehaviour
{
    [SerializeField]
    private Piece[] pieces = default;

    private int[,] fillValues;

    private Vector2[,] positions;

    private float pieceWidth = 24.0f;
    private float pieceHeight = 15.0f;

    private int numX = 2;
    private int numY = 2;

    [SerializeField]
    private float overlayFinalAlpha = 0.16f;

    [SerializeField]
    private float overlayDullAlpha = 1.0f;

    [SerializeField]
    private float offsetBeforeSettingAlpha = 0.3f;

    // Start is called before the first frame update
    public void Init()
    {
        if (pieces.Length > 3)
        {
            numX = 3;
            if (pieces.Length > 5)
                numY = 3;
        }
        
        fillValues = new int[numX, numY];
        for (int i = 0; i < numX; i++)
        {
            for (int j = 0; j < numY; j++)
            {
                fillValues[i, j] = -1;
            }
        }

        positions = new Vector2[numX, numY];
        float startingX = (-pieceWidth / 2.0f) * (numX - 1.0f);
        float startingY = (-pieceHeight / 2.0f) * (numY - 1.0f);
        for (int i = 0; i < numX; i++)
        {
            for (int j = 0; j < numY; j++)
            {
                positions[i,j] = new Vector2(startingX + i * pieceWidth, startingY + j * pieceHeight);
            }
        }

        int indexX = -1;
        int indexY = -1;

        for (int i = 0; i < pieces.Length; i++)
        {
            indexX = GetIndexX(pieces[i].MyIndex);
            indexY = GetIndexY(pieces[i].MyIndex);
            fillValues[indexX, indexY] = pieces[i].MyIndex;
            pieces[i].SetPositionNow(positions[indexX, indexY]);
            pieces[i].SetIndicesXY(indexX, indexY);
            pieces[i].SetPiecesManager(this);
        }

        SetSides();
        SetInitialPiecesHighlight();
    }

    public int GetNumPieces()
    {
        return pieces.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Piece GetMoveablePiece(MoveDirection moveDirection)
    {
        if (moveDirection == MoveDirection.none)
            return null;

        MoveDirection currentPiecemoveDirection;
        for (int i = 0; i < pieces.Length; i++)
        {
            currentPiecemoveDirection = pieces[i].CanMove();

            if (currentPiecemoveDirection == moveDirection)
            {
                return pieces[i];
            }
        }
        return null;
    }

    public bool CanPieceMoveRight(int index)
    {
        int x = -1;
        int y = -1;
        for (int i = 0; i < numX; i++)
        {
            for (int j = 0; j < numY; j++)
            {
                if (fillValues[i, j] == index)
                {
                    x = i;
                    y = j;
                    break;
                }
            }
        }

        if (x > 0)
        {
            if (fillValues[x - 1, y] == -1)
                return false; //Move Left
        }

        if (x < numX - 1)
        {
            if (fillValues[x + 1, y] == -1)
                return true; //Move Right
        }

        return false;

    }

    public bool CanPieceMoveUp(int index)
    {
        int x = -1;
        int y = -1;
        for (int i = 0; i < numX; i++)
        {
            for (int j = 0; j < numY; j++)
            {
                if (fillValues[i, j] == index)
                {
                    x = i;
                    y = j;
                    break;
                }
            }
        }

        if (y > 0)
        {
            if (fillValues[x, y - 1] == -1)
                return false;
        }

        if (y < numY - 1)
        {
            if (fillValues[x, y + 1] == -1)
                return true;
        }

        return false;

    }



    public MoveDirection CanMove(int index)
    {
        int x = -1;
        int y = -1;
        for (int i = 0; i < numX; i++)
        {
            for (int j = 0; j < numY; j++)
            {
                if(fillValues[i,j]==index)
                {
                    x = i;
                    y = j;
                    break;
                }
            }
        }

        MoveDirection moveDirection = MoveDirection.none;

        if(x > 0)
        {
            if (fillValues[x - 1, y] == -1)
                moveDirection = MoveDirection.left;
        }

        if(x < numX - 1)
        {
            if (fillValues[x + 1, y] == -1)
                moveDirection = MoveDirection.right;
        }

        if (y > 0)
        {
            if (fillValues[x, y - 1] == -1)
                moveDirection = MoveDirection.down;
        }

        if (y < numY - 1)
        {
            if (fillValues[x, y + 1] == -1)
                moveDirection = MoveDirection.up;
        }

        return moveDirection;
    }

    public float GetPieceWidth()
    {
        return pieceWidth;
    }

    public float GetPieceHeight()
    {
        return pieceHeight;
    }

    public void ChangePiecePosition(int index, MoveDirection moveDirection)
    {
        int x = -1;
        int y = -1;
        for (int i = 0; i < numX; i++)
        {
            for (int j = 0; j < numY; j++)
            {
                if (fillValues[i, j] == index)
                {
                    x = i;
                    y = j;
                    break;
                }
            }
        }

        if (moveDirection == MoveDirection.right)
            fillValues[x + 1, y] = index;
        else if (moveDirection == MoveDirection.left)
            fillValues[x - 1, y] = index;
        else if (moveDirection == MoveDirection.up)
            fillValues[x, y+1] = index;
        else if (moveDirection == MoveDirection.down)
            fillValues[x, y-1] = index;

        fillValues[x, y] = -1;

        SetSides();
    }

    private int GetIndexX(int pieceIndex)
    {
        return (pieceIndex%numX);
    }

    private int GetIndexY(int pieceIndex)
    {
        return pieceIndex/numX;
    }

    //public Transform GetPieceTransformFromPosition(Vector2 inputPosition)
    //{
    //    return GetPieceFromPosition(inputPosition).transform;
    //}

    public Transform GetPieceHoverTransformFromPosition(Vector2 inputPosition)
    {
        return GetPieceFromPosition(inputPosition).HoverGroup;
    }

    public Piece GetPieceFromPosition(Vector2 inputPosition)
    {
        Vector2 piecePosition;
        for (int i = 0; i < pieces.Length; i++)
        {
            piecePosition = pieces[i].transform.Find("HoverGroup").position;
            if (inputPosition.x > piecePosition.x - pieceWidth / 2.0f && inputPosition.x < piecePosition.x + pieceWidth / 2.0f
                && inputPosition.y > piecePosition.y - pieceHeight / 2.0f && inputPosition.y < piecePosition.y + pieceHeight / 2.0f)
            {
                return pieces[i];
            }
        }

        return null;
    }

    public void SetPiecesToWantedPosition()
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].SetToWantedPosition();
        }
    }

    public void SetSides()
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            SetPieceSides(pieces[i]);
        }
    }

    private void SetPieceSides(Piece inputPiece)
    {
        Transform sidesGroup = inputPiece.transform.Find("HoverGroup/SidesGroup");
        sidesGroup.Find("Left").gameObject.SetActive(true);
        sidesGroup.Find("Right").gameObject.SetActive(true);
        sidesGroup.Find("Top").gameObject.SetActive(true);
        sidesGroup.Find("Bottom").gameObject.SetActive(true);

        for (int i = 0; i < 4; i++)
        {
            inputPiece.SetAreSidesMatched(false, i);
        }

        int x = inputPiece.IndexX;
        int y = inputPiece.IndexY;

        if (x > 0)
        {
            if (fillValues[x - 1, y] != -1)
            {
                if (AreMatchingSides(inputPiece.GetLeftValues(), GetPieceFromIndicesXY(x - 1, y).GetRightValues()))
                {
                    inputPiece.SetAreSidesMatched(true, 0);
                    sidesGroup.Find("Left").gameObject.SetActive(false);
                }  
            }
        }

        if (x < numX - 1)
        {
            if (fillValues[x + 1, y] != -1)
            {
                if (AreMatchingSides(inputPiece.GetRightValues(), GetPieceFromIndicesXY(x + 1, y).GetLeftValues()))
                {
                    inputPiece.SetAreSidesMatched(true, 1);
                    sidesGroup.Find("Right").gameObject.SetActive(false);
                }   
            }
        }

        if (y > 0)
        {
            if (fillValues[x, y - 1] != -1)
            {
                if (AreMatchingSides(inputPiece.GetBottomValues(), GetPieceFromIndicesXY(x, y-1).GetTopValues()))
                {
                    inputPiece.SetAreSidesMatched(true, 2);
                    sidesGroup.Find("Bottom").gameObject.SetActive(false);
                } 
            }
        }

        if (y < numY - 1)
        {
            if (fillValues[x, y + 1] != -1)
            {
                if (AreMatchingSides(inputPiece.GetTopValues(), GetPieceFromIndicesXY(x, y + 1).GetBottomValues()))
                {
                    inputPiece.SetAreSidesMatched(true, 3);
                    sidesGroup.Find("Top").gameObject.SetActive(false);
                }
            }
        }

        inputPiece.SetConnectState();
        inputPiece.SetSidesVisuals();
    }

    private bool AreMatchingSides(bool[] side1, bool[] side2)
    {
        bool fullSide = true;

        if (side1.Length != side2.Length)
            return false;

        for (int i = 0; i < side1.Length; i++)
        {
            if (side1[i] == false)
                fullSide = false;

            if (side1[i] != side2[i])
                return false;
        }

        return !fullSide;
    }

    public Piece GetPieceFromIndicesXY(int x, int y)
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i].IndexX == x && pieces[i].IndexY == y)
                return pieces[i];
        }

        return null;
    }

    public void ZoomingInOut(float t)
    {
        float adjustedT = Mathf.InverseLerp(offsetBeforeSettingAlpha, 1.0f, t);
        adjustedT = Easing.Ease(Equation.SineEaseIn, adjustedT, 0, 1, 1);
        float alphaValue;
        
        for (int i = 0; i < pieces.Length; i++)
        {
            if(pieces[i].IsConnected)
            {
                alphaValue = Mathf.Lerp(0, overlayFinalAlpha, adjustedT);
            }
            else
            {
                alphaValue = Mathf.Lerp(0, overlayDullAlpha, adjustedT);
            }

            pieces[i].transform.Find("HoverGroup/Overlay").GetComponent<Renderer>().material.SetColor("_Tint", new Color(alphaValue, alphaValue, alphaValue, 0));

            pieces[i].ZoomingInOut(t);
        }
    }

    public void SetOverlaysActiveState(bool value)
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].transform.Find("HoverGroup/Overlay").gameObject.SetActive(value);
            pieces[i].transform.Find("HoverGroup/OverlayBlack").gameObject.SetActive(value);
        }
    }

    public void ResetHoverGroup()
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].ResetHoverGroup();
        }
    }

    //public void SetPiecesConnectStates()
    //{
    //    for (int i = 0; i < pieces.Length; i++)
    //    {
    //        pieces[i].SetConnectState();
    //    }
    //}

    public void SetBorders(bool value)
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].SetBorders(value);
        }
    }

    private void SetInitialPiecesHighlight()
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].SetInitialPieceHighlight();
        }
    }

    public bool IsConnected(Piece connectingPiece, int connectingDirection, Vector3 ropePosition)
    {
        Piece myPiece = GetPieceFromPosition(ropePosition);
        if(myPiece.GetAreSidesMatched(connectingDirection))
        {
            if(connectingPiece==GetPieceFromDirection(myPiece, connectingDirection))
            {
                return true;
            }
        }
        return false;
    }

    private Piece GetPieceFromDirection(Piece inputPiece, int direction)
    {
        int x = inputPiece.IndexX;
        int y = inputPiece.IndexY;
        Debug.Log(y);
        if(direction==0 && x>0)
        {
            return GetPieceFromIndicesXY(x-1, y);
        }
        else if (direction == 1 && x < numX - 1)
        {
            return GetPieceFromIndicesXY(x + 1, y);
        }
        else if (direction == 2 && y > 0)
        {
            return GetPieceFromIndicesXY(x, y-1);
        }
        else if (direction == 3 && y < numY - 1)
        {
            return GetPieceFromIndicesXY(x, y + 1);
        }

        return null;
    }
}

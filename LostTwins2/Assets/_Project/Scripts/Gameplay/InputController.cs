using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private bool firstTimeMouseDown = true;
    private bool isNotHolding = true;
    private float screenHeight = 1080.0f;
    private Vector2 mouseStartingPosition;
    private Vector2 startingPiecePosition;
    private Vector2 endingPiecePosition;

    private MoveDirection currentMoveDirection;

    [SerializeField]
    private float moveSpeedFactor = 2.0f;

    private float xSign = 1.0f;
    private float ySign = -1.0f;

    private float maxPixelsX = 160.0f;
    private float maxPixelsY = 100.0f;

    private Piece currentPieceToMove;

    private bool currentPieceMovedEnough = false;

    private PiecesManager piecesManager;
    private bool IsKeyBoardKey = false;

    // Start is called before the first frame update
    public void Init(PiecesManager piecesManager)
    {
        this.piecesManager = piecesManager;
        screenHeight = Screen.height;
        currentMoveDirection = MoveDirection.none;
        maxPixelsX = 160.0f * screenHeight / moveSpeedFactor;
        maxPixelsY = 100.0f * screenHeight / moveSpeedFactor;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.instance.LevelCompleted)
            return;

        if (GameplayManager.instance.GameStarted && GameplayManager.instance.ZoomStatus==ZoomStatuses.ZoomedOut)
        {
            if (InputManager.rightArrowPressed || InputManager.leftArrowPressed || InputManager.upArrowPressed || InputManager.downArrowPressed)
            {
                if (isNotHolding)
                {
                    isNotHolding = false;

                    MoveDirection moveDirection = GetMoveDirectionFromKeys();
                    currentPieceToMove = piecesManager.GetMoveablePiece(moveDirection);

                    if (currentPieceToMove != null)
                    {
                        currentMoveDirection = currentPieceToMove.CanMove();
                        startingPiecePosition = currentPieceToMove.GetAndSetWantedPosition();
                        endingPiecePosition = GetEndingPiecePosition(startingPiecePosition, currentMoveDirection);
                        IsKeyBoardKey = true;
                        currentPieceToMove.DisconnectShashka();
                    }

                }

                //firstTimeMouseDown = true;

            }

            if (Input.GetMouseButton(0))
            {
                if(firstTimeMouseDown)
                {
                    firstTimeMouseDown = false;
                    mouseStartingPosition = Input.mousePosition;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 5000, 1<<9))
                    {
                        AudioManager.instance.PlaySoundEffect("ShuffleStart");
                        currentPieceToMove = hit.transform.parent.parent.GetComponent<Piece>();
                        currentMoveDirection = currentPieceToMove.CanMove();
                        startingPiecePosition = currentPieceToMove.GetAndSetWantedPosition();
                        endingPiecePosition = GetEndingPiecePosition(startingPiecePosition, currentMoveDirection);
                    }

                }
                else if(currentPieceToMove)
                {
                    float xLerpValue = Mathf.InverseLerp(0, maxPixelsX * xSign, (Input.mousePosition.x - mouseStartingPosition.x));
                    float yLerpValue = Mathf.InverseLerp(0, maxPixelsY * ySign, (Input.mousePosition.y - mouseStartingPosition.y));
                    
                    float xPos = Mathf.Lerp(startingPiecePosition.x, endingPiecePosition.x, xLerpValue);
                    float yPos = Mathf.Lerp(startingPiecePosition.y, endingPiecePosition.y, yLerpValue);
                    currentPieceToMove.SetWantedPosition(new Vector2(xPos, yPos), true);

                    if (currentPieceMovedEnough == false && currentPieceToMove.IsConnected)
                    {
                        if(xLerpValue>0.05f || yLerpValue>0.08f)
                        {
                            currentPieceMovedEnough = true;
                            currentPieceToMove.DisconnectShashka();
                        }
                    }
                }
            }
            else
            {
                firstTimeMouseDown = true;

                if (currentPieceToMove)
                {
                    Vector2 finalPositon = startingPiecePosition;
                    bool gotoEndPosition = false;
                    Vector2 currentPosition = currentPieceToMove.transform.localPosition;
                    if (MyMath.InverseLerp(startingPiecePosition, endingPiecePosition, currentPosition) > 0.5f || IsKeyBoardKey)
                    {
                        finalPositon = endingPiecePosition;
                        gotoEndPosition = true;
                      
                    }
                    
                    if(gotoEndPosition)
                    {
                        //if(currentPieceMovedEnough==false)
                            currentPieceToMove.MoveToNewPosition(currentMoveDirection);
                       
                    }
                    else
                    {
                        piecesManager.SetSides();
                    }
                    //else
                    //{
                    //    if (currentPieceMovedEnough == true)
                    //        currentPieceToMove.MoveToNewPosition(ReversedMoveDirection(currentMoveDirection));
                    //}
                    AudioManager.instance.PlaySoundEffect("ShuffleEnd");
                    currentPieceToMove.SetWantedPosition(finalPositon, false);
                    currentPieceToMove = null;
                    currentPieceMovedEnough = false;
                }

                IsKeyBoardKey = false;


            }

            if(!Input.anyKey && !isNotHolding)
            {
                isNotHolding = true;
            }

        }
    }

    private MoveDirection GetMoveDirectionFromKeys()
    {
        if (InputManager.rightArrowPressed)
            return MoveDirection.right;
        else if (InputManager.leftArrowPressed)
            return MoveDirection.left;
        else if (InputManager.upArrowPressed)
            return MoveDirection.up;
        else if (InputManager.downArrowPressed)
            return MoveDirection.down;

        return MoveDirection.none;
    }

    private Vector2 GetEndingPiecePosition(Vector2 startingPosition, MoveDirection moveDirection)
    {
        float pieceWidth = piecesManager.GetPieceWidth();
        float pieceHeight = piecesManager.GetPieceHeight();

        xSign = 1.0f;
        ySign = 1.0f;

        if (moveDirection == MoveDirection.right)
        {
            ySign = 0;
            return startingPiecePosition + new Vector2(pieceWidth, 0);
        }
        else if (moveDirection == MoveDirection.left)
        {
            ySign = 0;
            xSign = -1.0f;
            return startingPiecePosition + new Vector2(-pieceWidth, 0);
        }
        else if (moveDirection == MoveDirection.up)
        {
            xSign = 0;
            return startingPiecePosition + new Vector2(0, pieceHeight);
        }
        else if (moveDirection == MoveDirection.down)
        {
            xSign = 0;
            ySign = -1.0f;
            return startingPiecePosition + new Vector2(0, -pieceHeight);
        }
        else if (moveDirection == MoveDirection.none)
        {
            xSign = 0;
            ySign = 0;
            return startingPiecePosition;
        }

        return startingPiecePosition;
    }

    //private MoveDirection ReversedMoveDirection(MoveDirection inputMoveDirection)
    //{
    //    if (inputMoveDirection == MoveDirection.left)
    //        return MoveDirection.right;

    //    if (inputMoveDirection == MoveDirection.right)
    //        return MoveDirection.left;

    //    if (inputMoveDirection == MoveDirection.down)
    //        return MoveDirection.up;

    //    if (inputMoveDirection == MoveDirection.up)
    //        return MoveDirection.down;

    //    return MoveDirection.none;
    //}
}

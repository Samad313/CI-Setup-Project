using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectToTarget : MonoBehaviour
{
    [SerializeField]
    private Vector3 defaultPosition = default;

    [SerializeField]
    private Vector3 pressedPosition = default;

    [SerializeField]
    private int numButtonsRequired = 1;

    [SerializeField]
    private float speed = 3.0f;

    private Transform myPiece;

    private Vector3 wantedPosition;

    private List<Transform> myButtons;

    private float wobbleY = 0;
    private float wobbleSinLerp = 0;

    [SerializeField]
    private float wobbleIntensity = 0.08f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        myPiece = GameplayManager.instance.GetPieceHoverTransformFromPosition(new Vector2(transform.position.x, transform.position.y));
        wantedPosition = defaultPosition;

        myButtons = new List<Transform>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
      
        if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
            return;

        transform.position = Vector3.MoveTowards(transform.position, wantedPosition + myPiece.position + new Vector3(0, wobbleY, 0), Time.deltaTime * speed);

        wobbleY = Mathf.Sin(wobbleSinLerp) * wobbleIntensity;
        wobbleSinLerp += Time.deltaTime * 1.5f;
    }

    public void SetTrigger(bool value, Transform inputButton)
    {
        if (value)
        {
            if (myButtons.Contains(inputButton) == false)
            {
                myButtons.Add(inputButton);
            }

            SetWantedPosition();
        }
        else
        {
            if (myButtons.Contains(inputButton))
            {
                myButtons.Remove(inputButton);
            }

            SetWantedPosition();
        }
       
    }

    private void SetWantedPosition()
    {
        wantedPosition = defaultPosition + (pressedPosition - defaultPosition) * (myButtons.Count * 1.0f) / (numButtonsRequired*1.0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollSnapper : MonoBehaviour, IDragHandler, IEndDragHandler
{

    public ScrollRect myScrollRect;
    public RectTransform scrollviewContent;
    public float snapSpeed = 30.0f;
    public float velocityCutoff = 30.0f;
    public int numElements = 5;

    private bool isDragging = false;
    private bool startLerping = false;
    private float wantedPosition = 1.0f;
    private float currentPosition = 1.0f;
    private float increment = 0;
    public int elementIndexToSnapTo = 0;

	// Use this for initialization
	void Start ()
    {
        Activated();
	}

    public void Activated()
    {
        increment = 1.0f / (numElements - 1.0f);
        /*elementIndexToSnapTo = numElements - 1;
        SetWantedPosition();*/
    }

    public void SetNumberOfElements(int numberOfElements , int snapTo)
    {
        myScrollRect.horizontalNormalizedPosition = 0f;
        numElements = numberOfElements;
        increment = 1.0f / (numberOfElements - 1.0f);
        SnapToIndex(snapTo);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!isDragging)
        {
            if(Mathf.Abs(myScrollRect.velocity.x)<velocityCutoff && startLerping==false)
            {
                startLerping = true;    
                CalculateElementIndexToSnap();
                SetWantedPosition();
                currentPosition = myScrollRect.horizontalNormalizedPosition;
            }
            if(startLerping)
            {
                currentPosition = Mathf.Lerp(currentPosition, wantedPosition, Time.deltaTime * snapSpeed);
                myScrollRect.horizontalNormalizedPosition = currentPosition;
            } 
        }
        else
        {
            CalculateElementIndexToSnap();
            SetWantedPosition();
            currentPosition = myScrollRect.horizontalNormalizedPosition;
        }
	}

    //Do this while the user is dragging this UI Element.
    public void OnDrag(PointerEventData data)
    {
        isDragging = true;
        startLerping = false;
    }

    //Do this when the user stops dragging this UI Element.
    public void OnEndDrag(PointerEventData data)
    {
        isDragging = false;
        startLerping = false;
    }

    private void SetWantedPosition()
    {
        wantedPosition = (float)elementIndexToSnapTo * increment;
    }

    private void CalculateElementIndexToSnap()
    {
        elementIndexToSnapTo = Mathf.RoundToInt(myScrollRect.horizontalNormalizedPosition * (numElements-1.0f));
        elementIndexToSnapTo = Mathf.Clamp(elementIndexToSnapTo, 0, numElements-1);
    }


    public int GetElementIndex()
    {
        return numElements - elementIndexToSnapTo;
    }

    public bool IsLerping()
    {
        return startLerping;
    }

    public void SnapToIndex(int index)
    {
		//elementIndexToSnapTo = numElements - index;
		increment = 1.0f / (numElements - 1.0f);
		elementIndexToSnapTo = index;
		elementIndexToSnapTo = Mathf.Clamp(elementIndexToSnapTo, 0, numElements - 1);
        SetWantedPosition();
        currentPosition = wantedPosition;
        myScrollRect.horizontalNormalizedPosition = currentPosition;
    }

    public int GetScrollIndex()
	{
		return elementIndexToSnapTo;
	}

    public void SetScrollIndex(int index)
	{
		elementIndexToSnapTo = index;
	}
}

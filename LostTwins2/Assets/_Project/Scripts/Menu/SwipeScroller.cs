
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeScroller : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public enum Type
    {
        Swipe = 0,
        Button = 1
    }

    //public
    [SerializeField] private Type scrollerType = Type.Swipe;
    [SerializeField] private RectTransform scroller;
    [SerializeField] private float distanceToTravel;
    [SerializeField] private float maxSwipeAcceleration;
    [SerializeField] private float minSwipeDistance;
    [SerializeField] private float maxSwipeDistance;
    [SerializeField] private float snapSpeed;
    [SerializeField] private GameObject rightButton, leftButton;
    [SerializeField] private RawImage[] elements;
    [SerializeField] private Texture[] mapTextures; // 0 => Default , 1 => Selected;

    //private
    private float dragStartXVal;
    private Vector2 dragStartPosition;
    private Vector2 dragCurrentPosition;
    private Vector2 dragEndPosition;
    private float elapasedTime = 0;
    private float scrollerCurrentPos = 0;
    private float scrollerTargetPos = 0;
    private bool isDragging;
    private bool startLerping = false;
    private int previousElement = 0;
    private bool moving = false;
    [HideInInspector] public int selectedIndex = 0;


    public void Start()
    {
        leftButton.SetActive(false);
        elements[0].texture = mapTextures[1];

//#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
     scrollerType = Type.Swipe;
//#endif

//#if UNITY_STANDALONE_OSX
        //scrollerType = Type.Button;

            selectedIndex = 0;
            scrollerTargetPos = distanceToTravel * selectedIndex;
            scroller.anchoredPosition = new Vector2(scrollerTargetPos, 0);
            scrollerCurrentPos = scroller.anchoredPosition.x;
//#endif

    }

    //private void OnEnable()
    //{
    //    GameDataManager.Resetter += Reset;
    //}

    //private void OnDisable()
    //{
    //    GameDataManager.Resetter -= Reset;
    //}

    public void Reset()
    {
        leftButton.SetActive(false);
        elements[0].texture = mapTextures[1];

        scrollerType = Type.Swipe;

        selectedIndex = 0;
        scrollerTargetPos = distanceToTravel * selectedIndex;
        scroller.anchoredPosition = new Vector2(scrollerTargetPos, 0);
        scrollerCurrentPos = scroller.anchoredPosition.x;
    }

    //Perform Movement After Swipe
    private void Update()
    {

        if (!isDragging)
        {
            if (startLerping)
            {
                scrollerCurrentPos = Mathf.Lerp(scrollerCurrentPos, scrollerTargetPos, Time.deltaTime * snapSpeed);
                scroller.anchoredPosition = new Vector2(scrollerCurrentPos, 0);

                //if(Mathf.Abs(scrollerCurrentPos - scrollerTargetPos) <= Mathf.Abs(distanceToTravel/2))
                //{
                //    moving = false;
                //}

                if (Mathf.Approximately(scrollerCurrentPos, scrollerTargetPos))
                {
                    scroller.anchoredPosition = new Vector2(scrollerTargetPos, 0);
                    startLerping = false;
                }
            }
        }

        HandleKeyboardKeyPress();
        ManageLeftRightButtonActiveStatus();
    }

    private void ManageLeftRightButtonActiveStatus()
    {
        //Debug.Log("<color=green> Selected Index Value: </color>" + selectedIndex);
        if (selectedIndex == 0 && leftButton.activeInHierarchy)
        {
            leftButton.SetActive(false);
        }

        if ((selectedIndex == elements.Length - 1) && rightButton.activeInHierarchy)
        {
            rightButton.SetActive(false);
        }
    }

    private void HandleKeyboardKeyPress()
    {
        if (Input.GetKeyDown("right") && !moving)
        {
            OnRightButtonCallBack();
        }

        if (Input.GetKeyDown("left") && !moving)
        {
            OnLeftButtonCallBack();
        }

        if (Input.GetKeyUp("right") || Input.GetKeyUp("left"))
        {
            moving = false;

        }
    }

    private void SwitchSelectedIsland()
    {
        elements[selectedIndex].texture = mapTextures[1];
        elements[previousElement].texture = mapTextures[0];
    }

    //Do this while the user starts dragging this UI Element.
    public void OnBeginDrag(PointerEventData data)
    {
        if (scrollerType == Type.Swipe)
        {
            startLerping = false;
            isDragging = true;
            dragStartXVal = scroller.anchoredPosition.x;
            dragStartPosition = data.position;
        }
    }

    //Do this while the user is dragging this UI Element.
    public void OnDrag(PointerEventData data)
    {
        if (scrollerType == Type.Swipe)
        {
            elapasedTime += Time.deltaTime;
            dragCurrentPosition = data.position;
            CalculateDrag();
        }
    }

    //Do this when the user stops dragging this UI Element.
    public void OnEndDrag(PointerEventData data)
    {
        if (scrollerType == Type.Swipe)
        {
            scrollerCurrentPos = scroller.anchoredPosition.x;
            dragEndPosition = data.position;
            CalculateSwipe();
            isDragging = false;
            startLerping = true;
        }
    }
    //Buttons
    public void OnLeftButtonCallBack()
    {
        if(selectedIndex != 0)
        {
            if (!rightButton.activeInHierarchy)
            {
                rightButton.SetActive(true);
            }
            previousElement = selectedIndex;
            selectedIndex--;
            selectedIndex = Mathf.Clamp(selectedIndex, 0, elements.Length - 1);
            scrollerTargetPos = distanceToTravel * selectedIndex;
            isDragging = false;
            startLerping = true;
            moving = true;
            SwitchSelectedIsland();
        }
    }

    public void OnRightButtonCallBack()
    {
        if(selectedIndex < elements.Length - 1)
        {
            if(!leftButton.activeInHierarchy)
            {
                leftButton.SetActive(true);
            }

            Debug.Log("Move To Right");
            previousElement = selectedIndex;
            selectedIndex++;
            selectedIndex = Mathf.Clamp(selectedIndex, 0, elements.Length - 1);
            scrollerTargetPos = distanceToTravel * selectedIndex;
            isDragging = false;
            startLerping = true;
            moving = true;
            SwitchSelectedIsland();
        }
    }

    //On Dragging
    private void CalculateDrag()
    {
        float d = Vector2.Distance(dragStartPosition, dragCurrentPosition);
        if (dragStartPosition.x < dragCurrentPosition.x)
        {
            scroller.anchoredPosition = new Vector2(dragStartXVal + d, 0);
        }
        else if (dragStartPosition.x > dragCurrentPosition.x)
        {
            scroller.anchoredPosition = new Vector2(dragStartXVal - d, 0);
        }
        scroller.anchoredPosition = new Vector2(Mathf.Clamp(scroller.anchoredPosition.x, distanceToTravel * (elements.Length - 1), 0), 0);
    }

    //On Swipe
    private void CalculateSwipe()
    {
        previousElement = selectedIndex;

        float a = CalculateAcceleration(dragStartPosition, dragEndPosition, elapasedTime);
        float d = CalculateDistance(dragStartPosition, dragEndPosition);

        if ((dragStartPosition.x > dragEndPosition.x) && (selectedIndex < elements.Length - 1))
        {
            if (a > maxSwipeAcceleration && d > minSwipeDistance || d > maxSwipeDistance)
            {
                if (!leftButton.activeInHierarchy)
                {
                    leftButton.SetActive(true);
                }
                selectedIndex++;
                SwitchSelectedIsland();

            }
        }
        else if ((dragStartPosition.x < dragEndPosition.x) && (selectedIndex != 0))
        {
            if (a > maxSwipeAcceleration && d > minSwipeDistance || d > maxSwipeDistance)
            {
                if (!rightButton.activeInHierarchy)
                {
                    rightButton.SetActive(true);
                }

                selectedIndex--;
                SwitchSelectedIsland();
            }
        }
        selectedIndex = Mathf.Clamp(selectedIndex, 0, elements.Length - 1);
        scrollerTargetPos = distanceToTravel * selectedIndex;
    }

    //Calculate Swipe Acceleration
    private float CalculateAcceleration(Vector2 p1, Vector2 p2, float t)
    {
        float a, v;
        v = Vector2.Distance(p1, p2) / t;
        a = v / t;
        return a;
    }
    //Calculate Distance In Inches
    private float CalculateDistance(Vector2 p1, Vector2 p2)
    {
        float d = Vector2.Distance(p1, p2) / Screen.dpi;
        return d;
    }
}

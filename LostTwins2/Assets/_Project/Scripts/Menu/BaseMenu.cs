using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuType
{
	MainMenuScreen = 0,
    GameplayHUD = 1,
    PauseScreen = 2,
	SettingsScreen = 3,
	ShopScreen = 4,
	Leaderboard = 5,
    QuitPopup = 6
};

public abstract class BaseMenu : MonoBehaviour
{

    [System.Serializable]
    public class MoveableItemsData
    {
       [SerializeField]
       private Transform animatedElement = null;
       [SerializeField]
       private Vector3 offsetVector;
       [SerializeField]
       private float waitTime = 0f;
       [HideInInspector] public Vector3 animatedElementStartingPosition;
       [HideInInspector] public Vector3 animatedElementFinalPosition;

        public Transform AnimatedElement
        {
            get { return animatedElement; }
        }

        public Vector3 OffsetVector
        {
            get { return offsetVector; }
        }

        public float WaitTime
        {
            get { return waitTime; }
        }
    }

    [System.Serializable]
    public class ScaleableItemsData
    {
        [SerializeField]
        private Transform scalableElement = null;
        [SerializeField]
        private float waitTime = 0f;
        [HideInInspector] public Vector3 scalebleElementStartingScale;
        [HideInInspector] public Vector3 scalableElementFinalScale;

        public Transform ScalableElement
        {
            get { return scalableElement; }
        }
        public float WaitTime
        {
            get { return waitTime; }
        }
    }

    [SerializeField]
    private MenuType type = MenuType.MainMenuScreen;

	[SerializeField]
	private Transform[] animatedElements = null;

	private Vector3[] animatedElementsStartingPosition;
	private Vector3[] animatedElementsFinalPosition;


	private int numAnimatedElements;

    [SerializeField]
    private Transform[] animatedScaledElements = null;

    private Vector3[] animatedScaledElementsStartingScale;
    private Vector3[] animatedScaledElementsFinalScale;
    private int numAnimatedScaledElements;

    [SerializeField]
    private List<Transform> elementsAffectedByNotch = null;

    [SerializeField]
    private List<MoveableItemsData> moveableItemsData = new List<MoveableItemsData>();
    [SerializeField]
    private List<ScaleableItemsData> scaleableItemsData = new List<ScaleableItemsData>();
    
    private float moveableElementsMaxTimeForActualT = 0f;
    private float scalableElementsMaxTimeForActualT = 0f;


    #region Animation Coroutines
    private Coroutine elementsInCoroutine;
    private Coroutine elementsOutCoroutine;
    private Coroutine scaleItemsCoroutine;
    #endregion

    public MenuType Type
    {
        get
        {
            return type;
        }
    }

    [SerializeField]
    private bool isPopup = false;

    public bool IsPopup
    {
        get
        {
            return isPopup;
        }
    }

    protected MenuController menuController = null;

    public virtual void Init()
    {
        menuController = GameObject.Find("Canvas").GetComponent<MenuController>();
        HandleAnimatedElements();
        HandleScalableElements();


		numAnimatedElements = animatedElements.Length;
		if (numAnimatedElements > 0)
		{
			animatedElementsStartingPosition = new Vector3[numAnimatedElements];
			animatedElementsFinalPosition = new Vector3[numAnimatedElements];
		}
		for (int i = 0; i < numAnimatedElements; i++)
		{
			animatedElementsFinalPosition[i] = animatedElements[i].localPosition;
            animatedElementsStartingPosition[i] = animatedElementsFinalPosition[i] + animatedElementsFinalPosition[i].normalized * 500.0f;
			animatedElements[i].localPosition = animatedElementsStartingPosition[i];
		}

        numAnimatedScaledElements = animatedScaledElements.Length;
        if (numAnimatedScaledElements > 0)
        {
            animatedScaledElementsStartingScale = new Vector3[numAnimatedScaledElements];
            animatedScaledElementsFinalScale = new Vector3[numAnimatedScaledElements];
        }
        for (int i = 0; i < numAnimatedScaledElements; i++)
        {
            animatedScaledElementsFinalScale[i] = animatedScaledElements[i].localScale;
            animatedScaledElementsStartingScale[i] = Vector3.one * 0.0001f;
            animatedScaledElements[i].localScale = animatedScaledElementsStartingScale[i];
        }

    }

    private void HandleAnimatedElements()
    {
        if(moveableItemsData.Count > 0)
        {
            for (int i = 0; i < moveableItemsData.Count; i++)
            {
                moveableItemsData[i].animatedElementFinalPosition = moveableItemsData[i].AnimatedElement.position;
                moveableItemsData[i].animatedElementStartingPosition = moveableItemsData[i].animatedElementFinalPosition + moveableItemsData[i].OffsetVector;
                moveableItemsData[i].AnimatedElement.position = moveableItemsData[i].animatedElementStartingPosition;
                moveableElementsMaxTimeForActualT = moveableItemsData[i].WaitTime;
               
            }
        }

    }

    private void HandleScalableElements()
    {
        if(scaleableItemsData.Count > 0)
        {
            for (int i = 0; i < scaleableItemsData.Count; i++)
            {
                scaleableItemsData[i].scalableElementFinalScale = scaleableItemsData[i].ScalableElement.localScale;
                scaleableItemsData[i].scalebleElementStartingScale = Vector3.one * 0.0001f;
                scaleableItemsData[i].ScalableElement.localScale = scaleableItemsData[i].scalebleElementStartingScale;
            }
        }
    }


    public virtual void MyUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }

    public virtual void Disable()
    {
        //MoveItemsOut();
        gameObject.SetActive(false);
	}

    public virtual void Enable(string arguments)
    {
		gameObject.SetActive(true);
        ScaleElements();
        //StartCoroutine("AnimateElements");
    }

    public virtual void UpdateValues()
    {
        
    }

    public virtual void HandleBackButton()
    {

    }

    public void ScaleElements(float animationSpeed = 1f)
    {
        if (scaleItemsCoroutine != null)
            StopCoroutine(scaleItemsCoroutine);
        scaleItemsCoroutine = StartCoroutine(ScaleIcons(animationSpeed));
    }

    public void AnimateElementsIn(float animationSpeed = 1f, bool isAnimate = false)
    {
        if (isAnimate)
        {
            if(elementsInCoroutine != null)
                StopCoroutine(elementsInCoroutine);
            elementsInCoroutine = StartCoroutine(AnimateIconsIn(animationSpeed));
        }
        else
            TurnOnElementsImmediately();
    }

    public void AnimateElementsOut(float animationSpeed = 1f, bool isAnimate = false )
    {
        if (isAnimate)
        {
            if(elementsOutCoroutine != null)
                StopCoroutine(elementsOutCoroutine);
            elementsOutCoroutine = StartCoroutine(AnimateIconsOut(animationSpeed));
        }
        else
            TurnOffElementsImmediately();
    }

    private void TurnOnElementsImmediately()
    {
        for (int i = 0; i < numAnimatedElements; i++)
        {
            animatedElements[i].localPosition = animatedElementsFinalPosition[i];
            animatedElements[i].gameObject.SetActive(true);
        }

    }

    private void TurnOffElementsImmediately()
    {
        for (int i = 0; i < numAnimatedElements; i++)
        {
            animatedElements[i].gameObject.SetActive(false);
        }

    }

    public void AdjustElementsAffectedByNotch()
    {
        if (MenuController.notch)
        {
            for (int i = 0; i < elementsAffectedByNotch.Count; i++)
            {
                elementsAffectedByNotch[i].localPosition = elementsAffectedByNotch[i].localPosition + new Vector3(0, -25, 0);
            }
        }
    }

    private IEnumerator AnimateElements()
    {
        for (int i = 0; i < numAnimatedElements; i++)
        {
            animatedElements[i].localPosition = animatedElementsStartingPosition[i];
        }

        for (int i = 0; i < numAnimatedScaledElements; i++)
        {
            animatedScaledElements[i].localScale = animatedScaledElementsStartingScale[i];
        }

        bool waitNeededDueToSomeReasonBeforeAnimatingElements = false;
        float delayNeededDueToSomeReasonBeforeAnimatingElements = 0.01f;
        while (waitNeededDueToSomeReasonBeforeAnimatingElements || delayNeededDueToSomeReasonBeforeAnimatingElements > 0.0f)
        {
            delayNeededDueToSomeReasonBeforeAnimatingElements -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        float t = 0.0f;
        float actualT = 0.0f;
        while (t < 1.0f)
        {
            actualT = MyMath.EaseOutElastic(t);
            for (int i = 0; i < numAnimatedElements; i++)
            {
                animatedElements[i].localPosition = Vector3.LerpUnclamped(animatedElementsStartingPosition[i], animatedElementsFinalPosition[i], actualT);
            }

            for (int i = 0; i < numAnimatedScaledElements; i++)
            {
                animatedScaledElements[i].localScale = Vector3.LerpUnclamped(animatedScaledElementsStartingScale[i], animatedScaledElementsFinalScale[i], actualT);
            }


            t += Time.deltaTime * menuController.GetMenuAnimationSpeed();
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < numAnimatedElements; i++)
        {
            animatedElements[i].localPosition = animatedElementsFinalPosition[i];
        }

        for(int i = 0; i < numAnimatedScaledElements; i++)
        {
            animatedScaledElements[i].localScale = animatedScaledElementsFinalScale[i];
        }
    }

    private IEnumerator ScaleIcons(float animationSpeed = 1f)
    {
        for (int i = 0; i < scaleableItemsData.Count; i++)
        {
            scaleableItemsData[i].ScalableElement.localScale = scaleableItemsData[i].scalebleElementStartingScale;
        }

        bool waitNeededDueToSomeReasonBeforeAnimatingElements = false;
        float delayNeededDueToSomeReasonBeforeAnimatingElements = 0.01f;
        while (waitNeededDueToSomeReasonBeforeAnimatingElements || delayNeededDueToSomeReasonBeforeAnimatingElements > 0.0f)
        {
            delayNeededDueToSomeReasonBeforeAnimatingElements -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        float t = 0.0f;
        float actualT = 0.0f;
        scalableElementsMaxTimeForActualT += 1f;

        while (t <= moveableElementsMaxTimeForActualT)
        {

            for (int i = 0; i < scaleableItemsData.Count; i++)
            {
                actualT = MyMath.EaseOutElastic(Mathf.Clamp(t - scaleableItemsData[i].WaitTime, 0f, 1f));
                scaleableItemsData[i].ScalableElement.localScale = Vector3.LerpUnclamped(scaleableItemsData[i].scalebleElementStartingScale, scaleableItemsData[i].scalableElementFinalScale, actualT);
            }
           
            t += Time.deltaTime * animationSpeed;
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < scaleableItemsData.Count; i++)
        {
            scaleableItemsData[i].ScalableElement.localScale = scaleableItemsData[i].scalableElementFinalScale;
        }
    }


    private IEnumerator AnimateIconsIn(float animationSpeed = 1f)
    {
        for (int i = 0; i < moveableItemsData.Count; i++)
        {
            moveableItemsData[i].AnimatedElement.position = moveableItemsData[i].animatedElementStartingPosition;
            moveableItemsData[i].AnimatedElement.gameObject.SetActive(false);
        }

        bool waitNeededDueToSomeReasonBeforeAnimatingElements = false;
        float delayNeededDueToSomeReasonBeforeAnimatingElements = 0.01f;
        while (waitNeededDueToSomeReasonBeforeAnimatingElements || delayNeededDueToSomeReasonBeforeAnimatingElements > 0.0f)
        {
            delayNeededDueToSomeReasonBeforeAnimatingElements -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        float t = 0.0f;
        float actualT = 0.0f;
        moveableElementsMaxTimeForActualT += 1f;
       
        while (t <= moveableElementsMaxTimeForActualT)
        {
        
            for (int i = 0; i < moveableItemsData.Count; i++)
            {
                actualT = MyMath.EaseOutElastic(Mathf.Clamp(t - moveableItemsData[i].WaitTime, 0f, 1f));
                moveableItemsData[i].AnimatedElement.gameObject.SetActive(true);
                moveableItemsData[i].AnimatedElement.position = Vector3.LerpUnclamped(moveableItemsData[i].animatedElementStartingPosition, moveableItemsData[i].animatedElementFinalPosition, actualT);
            }
          
            t += Time.deltaTime * animationSpeed;
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < moveableItemsData.Count; i++)
        {
            moveableItemsData[i].AnimatedElement.position = moveableItemsData[i].animatedElementFinalPosition;
        }
    }

    private IEnumerator AnimateIconsOut(float animationSpeed = 1f)
    {
      
        bool waitNeededDueToSomeReasonBeforeAnimatingElements = false;
        float delayNeededDueToSomeReasonBeforeAnimatingElements = 0.01f;
        while (waitNeededDueToSomeReasonBeforeAnimatingElements || delayNeededDueToSomeReasonBeforeAnimatingElements > 0.0f)
        {
            delayNeededDueToSomeReasonBeforeAnimatingElements -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        float t = 0.0f;
        float actualT = 0.0f;
        //moveableElementsMaxTimeForActualT += 1f;

        while (t <= moveableElementsMaxTimeForActualT)
        {

            for (int i = 0; i < moveableItemsData.Count; i++)
            {
                actualT = MyMath.EaseOutElastic(Mathf.Clamp(t - moveableItemsData[i].WaitTime, 0f, 1f));
                moveableItemsData[i].AnimatedElement.position = Vector3.LerpUnclamped(moveableItemsData[i].animatedElementFinalPosition, moveableItemsData[i].animatedElementStartingPosition, actualT);
            }
            
            t += Time.deltaTime * animationSpeed;
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < moveableItemsData.Count; i++)
        {
            moveableItemsData[i].AnimatedElement.position = moveableItemsData[i].animatedElementStartingPosition;
            moveableItemsData[i].AnimatedElement.gameObject.SetActive(false);
        }
    }



}


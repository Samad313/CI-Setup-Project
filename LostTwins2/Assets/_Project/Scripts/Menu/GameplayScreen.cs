using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayScreen : BaseMenu
{
    public static GameplayScreen instance;

    [SerializeField]
    private bool isDebug = false;

    [SerializeField]
    private Transform leftButton = default;

    [SerializeField]
    private Transform rightButton = default;

    [SerializeField]
    private Transform jumpButton = default;

    [SerializeField]
    private Transform charSwitchButton = default;

    [SerializeField]
    private RawImage boyIcon;
    [SerializeField]
    private RawImage girlIcon;


    [SerializeField]
    private Transform zoomButton = default;

    [SerializeField]
    private RawImage zoomInImage;
    [SerializeField]
    private RawImage zoomOutImage;

    [SerializeField]
    private Transform refreshButton = default;

    [SerializeField]
    private Transform backToMainMenuButton = default;

    [SerializeField]
    private Transform nextLevelButton = default;
    
    [SerializeField]
    private Button playButton;

    [SerializeField]
    private StarsUI starsUI;

    [SerializeField]
    private TitleScreenUI titleScreenUI;

    [SerializeField]
    private Transform dotMenuIconTransform;

    [SerializeField]
    private RawImage outerWave;
    [SerializeField]
    private RawImage innerWave;

    [SerializeField]
    private Transform menuTransform;
    [SerializeField]
    private Vector3 menuFinalPositionOffset = Vector3.zero;
    private Vector3 menuStartPosition = Vector3.zero;
    private bool isOpenMenu = false;


    private Quaternion dotStartRot = Quaternion.Euler(0f, 0f, 0f);
    private Quaternion dotEndRot = Quaternion.Euler(0f, 0f, 90f);
    bool isDotImageRotateToDefaultValue = false;

    [SerializeField]
    private float menuEndtime = 1.5f;
    public bool testBool = false;

    [SerializeField]
    private GameObject joystickGroup;

    public Vector3 GetMenuCrystalWorldPosition
    {
        get { return titleScreenUI.GetCrystalPosition; }
    }

    public StarsUI starsUIObject
    {
        get { return starsUI; }
    }

    private void OnEnable()
    {
        InputManager.playButtonCallBack += OnClickPlayButton;
    }

    private void OnDisable()
    {
        InputManager.playButtonCallBack -= OnClickPlayButton;
    }

    void Awake()
    {
        instance = this;
        playButton.onClick.AddListener( ()=> OnClickPlayButton() );
        menuStartPosition = menuTransform.position;

#if UNITY_IOS || UNITY_ANDROID

#else
        SetJumpButtonVisibility(false);
        SetJoystickGroupVisibility(false);
#endif
    }

    private void Start()
    {
        HandleDebugButtons();
    }

    public void OnClickPlayButton()
    {
        GameplayManager.instance.PlayGameState();
    }

    private void HandleDebugButtons()
    {
        if (isDebug)
        {
            refreshButton.gameObject.SetActive(true);
            backToMainMenuButton.gameObject.SetActive(true);
            nextLevelButton.gameObject.SetActive(true);
            InputManager.EnableJoystickForDebug();
        }
        else
        {
            refreshButton.gameObject.SetActive(false);
            backToMainMenuButton.gameObject.SetActive(false);
            nextLevelButton.gameObject.SetActive(false);

        }
    }

    public void GameplayScreenInit(bool IsFTUE)
    {
        starsUIObject.StarsUIInit();

        if(IsFTUE)
        {
            titleScreenUI.EnableTitleScreen();
        }
        else
        {
            titleScreenUI.DisableTitleScreen();
        }

    }

   
    public override void Init()
    {
        base.AdjustElementsAffectedByNotch();
        //If some UI element's final position on screen is needed, get it here before it gets pushed away off screen
        base.Init();
    }

    // Update is called once per frame
    void Update()
    {
        base.MyUpdate();
    }

    public void BackButtonCallback()
    {
        bool someActivityGoingOn = false;
        if (someActivityGoingOn == true)
            return;

#if UNITY_ANDROID
        if (MenuController.instance.IsOnTop(MenuType.MainMenuScreen))
            MenuController.instance.ShowMenu(MenuType.QuitPopup, "");
#endif
    }

    public override void HandleBackButton()
    {
        BackButtonCallback();
    }

    public override void Enable(string arguments)
    {
        base.Enable(arguments);

        UpdateValues();
    }

    public override void UpdateValues()
    {
        base.UpdateValues();
    }

    public override void Disable()
    {
        base.Disable();
    }

    public void CharacterButtonCallback()
    {
        InputManager.SetCharacterSwitchButtonDown();
    }

    public void ChangeCharacterIcon(bool isBoy)
    {
        if (isBoy)
        {
            StartCoroutine(FadeIcon(boyIcon, 0f));
            StartCoroutine(FadeIcon(girlIcon, 1f));
        }
        else
        {
            StartCoroutine(FadeIcon(boyIcon, 1f));
            StartCoroutine(FadeIcon(girlIcon, 0f));
        }
    }

    private IEnumerator FadeIcon(RawImage image, float endAlpha)
    {
        Color imageColor = image.color;
        while (!Mathf.Approximately(imageColor.a, endAlpha))
        {
            imageColor.a = Mathf.MoveTowards(imageColor.a, endAlpha, Time.deltaTime * 1.5f);
            image.color = imageColor;
            yield return new WaitForEndOfFrame();
        }
    }

    public void ZoomButtonDownCallback()
    {
        InputManager.SetCameraZoomButtonDown();
    }

    public void JumpButtonCallback()
    {
        InputManager.SetJumpButton();
    }

    public void LeftRightButtonCallback(int isRight)
    {
        if(isRight==0)
            InputManager.SetLeftButton();
        else
            InputManager.SetRightButton();
    }

    public void RefreshButtonCallback()
    {
        HandleMenu();
        //GameController.instance.ReloadLevel();
        if(SceneLoader.Instance)
        {
            SceneLoader.Instance.ReloadScene();
        }
        MyAnalytics.CustomEvent(Constants.Analytics.LevelRestart, new Dictionary<string, object>());
        //starsUI.StarsUIInit();
    }

    public void BackToMainMenuCallback()
    {
        //if(SceneLoader.Instance)
        //{
        //    SceneLoader.Instance.UnloadScene(PlayerStateManager.instance.ActiveLevelManagerInstance.SceneName);
        //}
        GameController.instance.LoadMenu();
        SceneLoader.Instance.LoadSpecificScene(Constants.Scenes.MainScene);
    }

    public void NextLevelCallback()
    {
        DisableTitleScreen();
        HandleDebugButtons();
        HandleMenu();
        //GameController.instance.LoadLevel(true);
        SceneLoader.Instance.LoadNextSceneInBuild();

        MyAnalytics.CustomEvent(Constants.Analytics.LevelSkip, new Dictionary<string, object>());

        //if(GameplayManager.instance)
        //{
        //    FXManager.instance.FadeCamera(false, 0.3f, MenuController.instance.UICamera.GetComponent<UICamera>().BlackBG);

        //}
        //starsUI.StarsUIInit();
    }

    public void PreviousLevelCallback()
    {
        DisableTitleScreen();
        HandleDebugButtons();
        HandleMenu();
        GameController.instance.LoadPreviousLevel();
        //MyAnalytics.CustomEvent(Constants.Analytics.LevelSkip, new Dictionary<string, object>());
    }

    public void JumpButtonUpCallback()
    {
        InputManager.SetJumpButtonUp();
        StartCoroutine(AnimateImageAlpha(jumpButton.GetChild(0).GetComponent<RawImage>(), 0.24f));
    }

    public void JumpButtonDownCallback()
    {
        InputManager.SetJumpButtonDown();
    }

    public void MenuButtonCallBack()
    {
        HandleMenu();
    }

    public void LeftRightButtonUpCallback(int isRight)
    {
        if (isRight == 0)
            InputManager.SetLeftButtonUp();
        else
            InputManager.SetRightButtonUp();
    }

    public void SetLeftRightButtonsVisibility(bool value)
    {
        leftButton.gameObject.SetActive(value);
        rightButton.gameObject.SetActive(value);
    }

    public void SetJumpButtonVisibility(bool value)
    {
        jumpButton.gameObject.SetActive(value);
    }

    public void SetCharSwitchButtonVisibility(bool value)
    {
        charSwitchButton.gameObject.SetActive(value);
    }

    public void SetJoystickGroupVisibility(bool value)
    {
        joystickGroup.SetActive(value);
    }

    public void SetZoomButtonVisibility(bool value)
    {
        zoomButton.gameObject.SetActive(value);
    }

    public void SetAllButtonsVisibility(bool value)
    {
        leftButton.gameObject.SetActive(value);
        rightButton.gameObject.SetActive(value);
#if UNITY_IOS || UNITY_ANDROID
        jumpButton.gameObject.SetActive(value);
#endif
        charSwitchButton.gameObject.SetActive(value);
        zoomButton.gameObject.SetActive(value);
    }

    public void HandleStarsUI()
    {
        if(starsUI != null && instance)
            starsUI.EnableStarFillImage();
    }

    public void HandleTitleScreenInit()
    {
        if(titleScreenUI != null)
        {
            titleScreenUI.OnButtonTap();
        }
    }

    public void DisableTitleScreen()
    {
        if (titleScreenUI != null)
        {
            titleScreenUI.DisableTitleScreen();
        }
    }

    private void HandleMenu()
    {

        isDotImageRotateToDefaultValue = !isDotImageRotateToDefaultValue;

        if (isDotImageRotateToDefaultValue)
            StartCoroutine(RotateImage(dotMenuIconTransform.GetChild(0).GetComponent<RawImage>(), dotEndRot));
        else
            StartCoroutine(RotateImage(dotMenuIconTransform.GetChild(0).GetComponent<RawImage>(), dotStartRot));

        AnimateWaves();
        StartCoroutine(AnimateMenu());
    }

    private IEnumerator RotateImage(RawImage image, Quaternion endRot)
    {
        float timeElapsed = 0f;
        float endTime = 0.2f;
        Quaternion startRot = image.gameObject.transform.rotation;
        Quaternion currentRot;

        while (timeElapsed <= endTime)
        {
            currentRot = Quaternion.Lerp(startRot, endRot, timeElapsed / endTime);
            image.transform.rotation = currentRot;
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        image.transform.rotation = endRot;
    }

    public void ChangeZoomInOutIcon(float t)
    {
        if (!zoomInImage || !zoomOutImage)
            return;

        Color zoomInColor = zoomInImage.color;
        Color zoomOutColor = zoomOutImage.color;

        float alphaT = 0f;
        float minRange = 0.1f;
        float maxRange = 0.2f;
        float endAlpha = 0.2f;


        if(t >= 0 && GameplayManager.instance.IsZoomOut)
        {
            if(t < 0.1f)
            {
                alphaT = Mathf.InverseLerp(0f, minRange, t);
                zoomOutColor.a = Mathf.Lerp(endAlpha, 1f, alphaT * 10f);
            }

            else if (t >= 0.1f && t < 0.2f)
            {
                alphaT = Mathf.InverseLerp(0.1f, maxRange, t );
                zoomOutColor.a = Mathf.Lerp(1f, endAlpha, alphaT * 10f);
            }

            else if (t >= 0.2f)
            {
                alphaT = Mathf.InverseLerp(0.2f, 1, t );
                zoomOutColor.a = Mathf.Lerp(endAlpha, 0f, alphaT * 10f);
                
                zoomInColor.a = Mathf.Lerp(0f, endAlpha, alphaT * 10f);
            }
            zoomOutImage.color = zoomOutColor;
            zoomInImage.color = zoomInColor;
        }


        if (t <= 1f && GameplayManager.instance.IsZoomIn)
        {

            if (t > 0.9f)
            {
                alphaT = Mathf.InverseLerp(1, 0.9f, t);
                zoomInColor.a = Mathf.Lerp(endAlpha, 1f, alphaT * 10f);

            }
            else if (t >= 0.8f)
            {
                alphaT = Mathf.InverseLerp(0.9f, 0.8f, t );
                zoomInColor.a = Mathf.Lerp(1f, endAlpha, alphaT * 10f);
            }
            else
            {
                alphaT = Mathf.InverseLerp(0.8f, 0f, t );
                zoomInColor.a = Mathf.Lerp(endAlpha, 0f, alphaT * 10f);

                zoomOutColor.a = Mathf.Lerp(0f, endAlpha, alphaT * 10f);

            }

            zoomInImage.color = zoomInColor;
            zoomOutImage.color = zoomOutColor;

        }

    }

    private IEnumerator AnimateImageAlpha(RawImage image, float endAlpha)
    {
        float timeElapsed = 0f;
        float endTime = 0.2f;
        Color imageColor = image.color;

        while (timeElapsed < endTime)
        {
            float alphaValue = Mathf.Lerp(imageColor.a, 1f, timeElapsed / endTime);
            imageColor.a = alphaValue;
            image.color = imageColor;
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        timeElapsed = 0f;

        while (timeElapsed < endTime)
        {
            float alphaValue = Mathf.Lerp(imageColor.a, endAlpha, timeElapsed / endTime);
            imageColor.a = alphaValue;
            image.color = imageColor;
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }

    private void AnimateWaves()
    {
        StartCoroutine(WaveMovement(outerWave, 0.51f, 2f, 0.05f));
        StartCoroutine(WaveMovement(innerWave, 0.51f, 2.5f, 0.35f));
        StartCoroutine(LerpWaveAlpha(outerWave, 1f, 0.05f));
        StartCoroutine(LerpWaveAlpha(innerWave, 0.8f, 0.35f, 0.4f));
    }


    private IEnumerator WaveMovement(RawImage image, float startScaleFactor, float endTime, float waitTime = 0f)
    {
        yield return new WaitForSeconds(waitTime);

        float timeElapsed = 0f;
        Vector3 startScale = Vector3.one * startScaleFactor;
        Vector3 currentScale = startScale;
        image.transform.localScale = currentScale;
        
        while (timeElapsed < endTime)
        {

            currentScale = Vector3.Lerp(startScale, new Vector3(1.2f, 1.2f, 1.2f), timeElapsed / endTime);
            image.transform.localScale = currentScale;
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
       
    }

    private IEnumerator LerpWaveAlpha(RawImage image, float endTime, float waitTime = 0f, float startAlphaValue = 1f)
    {
        yield return new WaitForSeconds(waitTime);
        float timeElapsed = 0f;
        Color imageColor = image.color;
        imageColor.a = startAlphaValue;
        image.color = imageColor;
        
        while (timeElapsed < endTime)
        {
            imageColor.a = Mathf.Lerp(startAlphaValue, 0f, timeElapsed / endTime);
            image.color = imageColor;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        imageColor.a = 0f;
        image.color = imageColor;

    }


    private IEnumerator AnimateMenu()
    {
        float timeElapsed = 0f;
        //float endTime = 1.5f;
        isOpenMenu = !isOpenMenu;
       
        Vector3 menuStartPos = menuTransform.position;
        Vector3 menuFinalPos = isOpenMenu ? menuTransform.position + menuFinalPositionOffset : menuStartPosition;
        Vector3 currentPos = menuStartPos;

        while (timeElapsed < menuEndtime)
        {
            currentPos = Vector3.Lerp(menuStartPos, menuFinalPos, timeElapsed / menuEndtime);
            menuTransform.position = currentPos;
            timeElapsed += Time.deltaTime;
            if (testBool)
                yield return new WaitForEndOfFrame();
            else
                yield return null;

        }

        menuTransform.position = menuFinalPos;

    }


}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;

public enum ZoomStatuses { ZoomedOut, ZoomingIn, ZoomedIn, ZoomingOut, StartingSequence };

public enum GameStates { None, Play};

public class GameplayManager : MonoBehaviour
{
    #region Variables

    public static GameplayManager instance;

    private GameStates gameState = GameStates.None;

    #region Manager Classes References
    [SerializeField] private CameraController cameraController = default;
    [SerializeField] private ElementsManager elementsManager = default;
    [SerializeField] private PiecesManager piecesManager = default;
    [SerializeField] private InputController inputController = default;
    [SerializeField] private MoveOutClouds moveOutClouds = default;
    #endregion

    #region Bools
    private bool gameStarted = false;
    private bool levelCompleted = false;

    private bool isCutsceneRunning = false;
    private bool isCameraMovingDown = false;

    private bool characterSwitchingDisabled = false;
    private bool cameraZoomDisabled = false;

    private bool hasWaterInLevel = false;

    [SerializeField] private bool firstLevel = false;
    [SerializeField] private bool isFTUE = false;
    [SerializeField] private bool zoomedInStart = false;
    [SerializeField] private bool phoenixAlreadyOnGate = false;

    public bool FirstLevel { get { return firstLevel; } }
    public bool IsCameraMovingDown { get { return isCameraMovingDown; } set { isCameraMovingDown = value; } }
    public bool GameStarted { get { return gameStarted; } }
    public bool LevelCompleted { get { return levelCompleted; } }
    public bool IsFTUE { get { return isFTUE; } }
    public bool CharacterSwitchingDisabled { get { return characterSwitchingDisabled; } set { characterSwitchingDisabled = value; } }
    public bool ZoomedInStart { get { return zoomedInStart; } }
    public bool PhoenixAlreadyOnGate { get { return phoenixAlreadyOnGate; } set { phoenixAlreadyOnGate = value; } }
    public bool HasWaterInLevel { get { return hasWaterInLevel; } }

    public bool IsGamePlayState { get { return gameState == GameStates.Play; } }

    #endregion

    #region Exposed Object References
    [SerializeField] private Transform[] playerTransforms = default;
    [SerializeField] private Phoenix phoenix = default;
    [SerializeField] private FinalGate finalGate = default;
    [SerializeField] private GameObject ftueEnvironment = default;
    [SerializeField] private PostProcessVolume ppVolume = default;
    [SerializeField] private PostProcessVolume nextLevelPP = default;
    [SerializeField] private Transform portalEndTimelinePrefab = default;
    [SerializeField] private TitleText titleText = default;

    public FinalGate FinalPortal { get { return finalGate; } }
    public Phoenix Phoenix { get { return phoenix; } }
    public Transform CurrentPlayerTransform { get { return playerTransforms[currentCharacterIndex]; } }
    public TitleText TitleText { get { return titleText; } }
    public Transform[] PlayerTransforms { get => playerTransforms; }
    #endregion

    #region Objects
    private PlayerController[] players;
    private Transform[] playerCenters;
    private Transform[] playerPreviousParents;

    private Transform portalEndTimeline = default;
    #endregion

    #region Normal Variables
    private int currentCharacterIndex = 0;
    private int previousPlayerCurrentPiece = -1;
    private int starCurrentAmount = 0;
    private Vector3 currentCrystalPosition;
    private bool isZoomIn = false;
    private bool isZoomOut = false;

    public bool IsZoomIn
    {
        get { return isZoomIn; }
    }

    public bool IsZoomOut
    {
        get { return isZoomOut; }
    }


    private ZoomStatuses zoomStatus = default;
    private float zoomInOutLerpValue = 0.0f;
    private float levelTime = 0.0f;
    private float currentCrystalCollectTime = 0.0f;

    [SerializeField] private int levelNumber = 1;
    [SerializeField] private Equation animCurveType = Equation.CubicEaseInOut;
    [SerializeField] private float zoomInSpeed = 1.0f;
    [SerializeField] private float zoomOutSpeed = 1.0f;

    public ZoomStatuses ZoomStatus { get { return zoomStatus; } }
    public GameStates CurrentGameState { get { return gameState; } }
    public int CurrentCharacterIndex { get { return currentCharacterIndex; } }
    public float ZoomInOutLerpValue { get { return zoomInOutLerpValue; } }
    public PlayerController CurrentPlayer { get { return players[currentCharacterIndex]; } }
    public int GetStartCurrentAmount { get { return starCurrentAmount; } }
    #endregion


    #region Coroutines
    private IEnumerator setApertureCoroutine;
    #endregion

    #endregion

    #region Functions

    #region Inits & Update

    private void OnEnable()
    {
        if (!GameplayScreen.instance)
        {
            InputManager.playButtonCallBack += PlayGameState;
        }
    }

    void Awake()
    {
        instance = this;

        if (QualitySettings.vSyncCount > 0)
            Application.targetFrameRate = 60;
        else
            Application.targetFrameRate = -1;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();

        if (IsFTUE)
        {
            MyAnalytics.CustomEvent(Constants.Analytics.LevelStart, new Dictionary<string, object>());
        }
        else
        {
            MyAnalytics.CustomEvent(Constants.Analytics.TutorialStart, new Dictionary<string, object>());
        }

        if (GameController.instance)
        {
            if (GameController.instance.GetLevelRestarted() && isFTUE == false)
            {
                phoenixAlreadyOnGate = true;
                zoomedInStart = false;
            }

            if (MenuController.instance.ShouldEnableDebugMenu)
            {
                SetDebugPPS(DebugMenu.instance.MyCurrentProfile);
            }
        }

        InitPlayers();

        if (isFTUE == false)
        {
            elementsManager.Init();
            piecesManager.Init();
        }

        cameraController.Init();
        inputController.Init(piecesManager);

        if (isFTUE == false)
        {
            elementsManager.MakeChildrenOfPieces();
            MakePlayersChildrenOfPieces();
            cameraController.MakeCameraPointsChildrenOfPieces();
        }

        isCameraMovingDown = cameraController.IsCameraMovingDown();

        if (isFTUE == false)
        {
            if (zoomedInStart)
            {
                DoTasksWhileZoominInOut(0.0f);
                piecesManager.ResetHoverGroup();

                cameraController.SetPositionValues();

                DoTasksWhileZoominInOut(0.0f);
                DoTasksAfterZoomIn();
                cameraController.SetTransformsToFollowAtStart();

                if (phoenixAlreadyOnGate)
                {
                    if (firstLevel)
                        SetAperture(13.0f);
                    else
                        SetAperture(1.0f);
                }
                else
                {
                    SetAperture(13.0f);
                }

                cameraController.SetPositionNow();
            }
            else
            {
                phoenix.Init(cameraController.transform.position);
                elementsManager.AddPhoenixToElementsList();
                phoenix.Skip();
                FirstTimeZoomingOut();

                zoomInOutLerpValue = 0.5f;
                DoTasksWhileZoominInOut(zoomInOutLerpValue);
            }
        }
        else
        {
            SetAperture(2.0f);
            zoomStatus = ZoomStatuses.ZoomedIn;
            cameraController.SetPositionNow();
        }

        if (firstLevel)
        {
            phoenix.Init(cameraController.transform.position);
        }
        else if (zoomedInStart)
        {
            Vector3 cameraStartingOffset = phoenix.Init(cameraController.transform.position);
            if (isFTUE == false)
            {
                cameraController.SetCameraStartingOffset(cameraStartingOffset);
            }

            cameraController.SetPositionNow();
        }

        cameraController.SetCameraMovingDown();

        if (zoomedInStart == false)
        {
            phoenixAlreadyOnGate = true;
        }
        else
        {
            if (!firstLevel)
            {
                if (MenuController.instance)
                {
                    FXManager.instance.SetCameraFade(1, MenuController.instance.UICamera.GetComponent<UICamera>().BlackBG);
                    FXManager.instance.FadeCamera(false, 0.3f, MenuController.instance.UICamera.GetComponent<UICamera>().BlackBG);
                }
                else
                {
                    FXManager.instance.SetCameraFade(1);
                    FXManager.instance.FadeCamera(false, 0.3f);
                }
            }
            else
            {
                if (MenuController.instance)
                    FXManager.instance.SetCameraFade(0, MenuController.instance.UICamera.GetComponent<UICamera>().BlackBG);
                else
                    FXManager.instance.SetCameraFade(0);
            }
        }

        if (titleText)
        {
            titleText.Init(cameraController.transform);
        }

        if (isFTUE || phoenix == null || zoomedInStart == false)
        {
            StartGameplay();
            if (GameplayScreen.instance && isFTUE)
            {
                if (MenuController.instance)
                    MenuController.instance.DisableGameplayHUD(false);
            }
        }

        if (GameplayScreen.instance && !isFTUE)
        {
            if (MenuController.instance)
                MenuController.instance.DisableGameplayHUD(true);

            GameplayScreen.instance.ChangeCharacterIcon(true);

        }
    }

    public void Init()
    {
        if(MenuController.instance)
        {
            MenuController.instance.ShowMenu(MenuType.GameplayHUD, "");
        }

        if(GameplayScreen.instance)
        {
            GameplayScreen.instance.GameplayScreenInit(isFTUE);
        }
    }

    private void InitPlayers()
    {
        playerPreviousParents = new Transform[playerTransforms.Length];
        playerCenters = new Transform[playerTransforms.Length];
        players = new PlayerController[playerTransforms.Length];
        for (int i = 0; i < playerTransforms.Length; i++)
        {
            playerPreviousParents[i] = null;
            players[i] = playerTransforms[i].GetComponent<PlayerController>();
            players[i].SetIsActive(true);
            players[i].SetControlsDisabled(i != currentCharacterIndex);
            //if (isFTUE)
            //{
            //    if (i != currentCharacterIndex)
            //        players[i].DisablePlayer(true);
            //}
            //else
            //{
            //    players[i].DisablePlayer(true);
            //}

            playerCenters[i] = playerTransforms[i].Find("center");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (levelCompleted == false)
        {
            if (isFTUE)
            {
                if (IsGamePlayState)
                {
                    levelTime += Time.deltaTime;
                    currentCrystalCollectTime += Time.deltaTime;
                }
            }
            else if (gameStarted)
            {
                levelTime += Time.deltaTime;
                currentCrystalCollectTime += Time.deltaTime;
            }
        }

        if (InputManager.skipButtonDown)
        {
            Skip();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }

        if (levelCompleted || isCutsceneRunning)
            return;

        if (zoomStatus == ZoomStatuses.ZoomedIn && gameStarted)
        {
            if (InputManager.characterSwitchDown)
            {
                SwitchCharacter();
                Debug.Log("Character switched");
            }
        }

        SetCameraZoom();
        SwitchPieceAmbientSound();
        SetJoystickAccordingToPlayerState();
    }
    #endregion

    #region Camera Zooming In & Out
    private void SetCameraZoom()
    {
        if (zoomStatus == ZoomStatuses.ZoomedIn)
        {
            if (InputManager.cameraZoomDown && isCutsceneRunning == false && isFTUE == false && cameraZoomDisabled == false && gameStarted)
            {
                DoTasksBeforeZoomOut();

            }
        }
        else if (zoomStatus == ZoomStatuses.ZoomingOut)
        {
            DoTasksWhileZoominInOut(zoomInOutLerpValue);
            zoomInOutLerpValue += Time.deltaTime * zoomOutSpeed;
            if (zoomInOutLerpValue > 1.0f)
            {
                DoTasksWhileZoominInOut(1.0f);
                DoTasksAfterZoomOut();
            }
        }
        else if (zoomStatus == ZoomStatuses.ZoomedOut)
        {
            if (InputManager.cameraZoomDown && cameraZoomDisabled == false && gameStarted)
            {
                DoTasksBeforeZoomIn();
            }
        }
        else if (zoomStatus == ZoomStatuses.ZoomingIn)
        {
            DoTasksWhileZoominInOut(zoomInOutLerpValue);
            zoomInOutLerpValue -= Time.deltaTime * zoomInSpeed;
            if (zoomInOutLerpValue < 0.0f)
            {
                DoTasksWhileZoominInOut(0.0f);
                DoTasksAfterZoomIn();
            }
        }
        //else if (zoomStatus == ZoomStatus.StartingSequence)
        //{

        //}
    }

    public void SetJoystickAccordingToPlayerState()
    {
        if (CurrentPlayer.IsInWater())
        {
            if (JoystickController.instance)
                JoystickController.instance.Joystick.SetAxisTo360Movement();
            // Debug.Log("360 Movement");
        }
        else
        {
            if (JoystickController.instance)
                JoystickController.instance.Joystick.SetAxisToHorizontalMovement();
        }



    }

    public void FirstTimeZoomingOut()
    {
        Invoke("GamePlayHUD", 0.4f);
        DoTasksBeforeZoomOut();
    }

    private void GamePlayHUD()
    {
        if (GameplayScreen.instance)
        {
            GameplayScreen.instance.SetAllButtonsVisibility(true);
            if (MenuController.instance)
                MenuController.instance.ShowGameplayHUD();
        }
    }

    public void DoTasksBeforeZoomOut()
    {
        Debug.Log("DoTasksBeforeZoomOut");
        InputManager.SetJoystickDisable();
        zoomStatus = ZoomStatuses.ZoomingOut;

        isZoomIn = false;
        isZoomOut = true;

        cameraController.DoTasksBeforeZoomOut();
        DisableAllPlayers();
        elementsManager.SetActiveStatusOfAllMovingObjects(false);
        elementsManager.MakeChildrenOfPieces();
        cameraController.MakeCameraPointsChildrenOfPieces();

        MakePlayersChildrenOfPieces();

        zoomInOutLerpValue = 0;
        piecesManager.SetBorders(true);
        piecesManager.SetOverlaysActiveState(true);

        DoTasksWhileZoominInOut(0.0f);
    }

    private void DoTasksAfterZoomOut()
    {
        Debug.Log("DoTasksAfterZoomOut");
        zoomStatus = ZoomStatuses.ZoomedOut;
        zoomInOutLerpValue = 1;
        //myCamera.orthographic = true;
        //myCamera.orthographicSize = zoomedOutHorizontalDistance / 2.0f;
    }

    private void DoTasksBeforeZoomIn()
    {
        Debug.Log("DoTasksBeforeZoomIn");
        InputManager.SetJoystickEnable();
        isZoomIn = true;
        isZoomOut = false;
        zoomStatus = ZoomStatuses.ZoomingIn;

        cameraController.DoTasksBeforeZoomIn();

        Transform characterCurrentHoverGroup = CurrentPlayerTransform.parent;
        Vector3 savedPosition = characterCurrentHoverGroup.localPosition;
        Quaternion savedRotation = characterCurrentHoverGroup.localRotation;
        characterCurrentHoverGroup.parent.GetComponent<Piece>().ResetMastiOnHoverGroup();

        cameraController.SetPositionValues();

        piecesManager.SetPiecesToWantedPosition();
        zoomInOutLerpValue = 1;

        characterCurrentHoverGroup.localPosition = savedPosition;
        characterCurrentHoverGroup.localRotation = savedRotation;

        //for (int i = 0; i < players.Length; i++)
        //{
        //    players[i].CalculateZoomedInXrayValue();
        //}
    }

    private void DoTasksWhileZoominInOut(float t)
    {
        t = Easing.Ease(animCurveType, t, 0, 1, 1);

        cameraController.DoTasksWhileZoominInOut(t);
        piecesManager.ZoomingInOut(t);
        elementsManager.ZoomingInOut(t, cameraController.transform.position);
        FXManager.instance.SetMaterialsWhileZoomingInOut(t);
        SetPlayerZoomOutIcons(t);
        SetAperture(Mathf.Lerp(1.0f, 13.0f, t));


        moveOutClouds.SetClouds(t);
        if (GameplayScreen.instance != null)
            GameplayScreen.instance.ChangeZoomInOutIcon(t);


    }

    private void DoTasksAfterZoomIn()
    {
        zoomStatus = ZoomStatuses.ZoomedIn;
        piecesManager.ResetHoverGroup();
        EnableAllPlayers();
        elementsManager.SetActiveStatusOfAllMovingObjects(true);
        elementsManager.MakeChildrenOfElementsGroup();
        cameraController.MakeChildrenOfCameraPointsGroup();
        MakePlayersChildrenOfWorld();
        zoomInOutLerpValue = 0;

        piecesManager.SetBorders(false);
        piecesManager.SetOverlaysActiveState(false);

    }

    public void SetPlayerZoomOutIcons(float t)
    {
        FXManager.instance.SetCharacterRenderQueueAbove3000(t > 0.3f);

        float adjustedT = Mathf.InverseLerp(0.3f, 1.0f, t);
        for (int i = 0; i < playerTransforms.Length; i++)
        {
            players[i].GetIconTransform().position = ZoomInOutIconSetter.GetIconPositionFromCamera(cameraController.transform.position, playerTransforms[i].Find("center").position, 6.0f);
            //players[i].SetXRayAlpha(Easing.Ease(Equation.CubicEaseInOut, adjustedT, players[i].LastZoomedInXrayValue, 0.7f- players[i].LastZoomedInXrayValue, 1.0f));
            players[i].ZoomingOutXRayValue(t > 0.3f);
            players[i].GetIconTransform().GetComponent<Renderer>().material.color = new Color(1, 1, 1, adjustedT);
        }
    }

    #endregion

    #region Switching Sounds
    private void SwitchPieceAmbientSound()
    {
        if (isFTUE)
            return;

        int playerCurrentPiece = -1;
        if (zoomStatus == ZoomStatuses.ZoomedIn)
        {
            Vector3 playerPosition = CurrentPlayerTransform.position;
            Piece currentPiece = piecesManager.GetPieceFromPosition(new Vector2(playerPosition.x, playerPosition.y));
            playerCurrentPiece = currentPiece.NamingIndex;
        }

        if (playerCurrentPiece != previousPlayerCurrentPiece)
        {
            AudioManager.instance.PlayAmbientSound(playerCurrentPiece, previousPlayerCurrentPiece);
        }
        previousPlayerCurrentPiece = playerCurrentPiece;
    }
    #endregion

    #region Start, Restart, Finish Level Sequence

    public void StartGameplay()
    {
        Debug.Log("Gameplay Started");
        gameStarted = true;
    }

    private void RestartLevel()
    {
        if(SceneLoader.Instance)
        {
            SceneLoader.Instance.ReloadScene();
        }
        //if (GameController.instance)
        //{
        //    GameController.instance.ReloadLevel();
        //}
        //else
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}
    }

    public void StartCutscene(Transform transformToFollow)
    {
        isCutsceneRunning = true;
        cameraController.StartFollowing(transformToFollow);
    }

    public void StopCutscene()
    {
        isCutsceneRunning = false;
        cameraController.StopFollowing();
    }

    private void Skip()
    {
        Debug.Log("Skip: ");
        if (gameStarted == false)
        {
            elementsManager.AddPhoenixToElementsList();
            phoenix.Skip();
            FXManager.instance.Skip();
            FirstTimeZoomingOut();
            cameraController.Skip();

            isCameraMovingDown = false;

            if (setApertureCoroutine != null)
            {
                StopCoroutine(setApertureCoroutine);
            }

            StartGameplay();
        }
    }

    public void FinishLevel(Vector3 gatePosition)
    {
        levelCompleted = true;
        portalEndTimeline = Instantiate(portalEndTimelinePrefab, null);
        portalEndTimeline.position = finalGate.GetMainVisual().position;
        if (IsFTUE)
        {
            Dictionary<string, object> eventData = new Dictionary<string, object>();
            eventData.Add(Constants.Analytics.LevelNumber, levelNumber);
            eventData.Add(Constants.Analytics.TimeToCompleteLevel, levelTime);
            eventData.Add(Constants.Analytics.CrystalsCollectedInThisLevel, starCurrentAmount);

            MyAnalytics.CustomEvent(Constants.Analytics.LevelComplete, eventData);
        }
        else
        {
            Dictionary<string, object> eventData = new Dictionary<string, object>();
            eventData.Add(Constants.Analytics.TimeToCompleteLevel, levelTime);
            MyAnalytics.CustomEvent(Constants.Analytics.TutorialCompletion, new Dictionary<string, object>());
        }
        StartCoroutine("LevelFinishedSequence", gatePosition);

        if(PlayerStateManager.instance)
        {
            PlayerStateManager.instance.SaveGameProgress(starCurrentAmount);
            //GameDataManager.instance.SaveLevelData(starCurrentAmount);
        }
    }

    //For Cheat exit to next level;
    public void CheatFinishLevel()
    {
        if (GameController.instance)
        {
            levelCompleted = true;

            if(PlayerStateManager.instance)
            {
                if (starCurrentAmount < 3)
                {
                    PlayerStateManager.instance.SaveGameProgress(3);
                }
                else
                {
                    PlayerStateManager.instance.SaveGameProgress(starCurrentAmount);
                }
            }

            //if(GameDataManager.instance)
            //{
            //    if (starCurrentAmount < 3)
            //    {
            //        GameDataManager.instance.SaveLevelData(3);
            //        SceneLoader.Instance.LoadSpecificScene(GameDataManager.instance.NextSceneToLoad);

            //        return;
            //    }

            //    GameDataManager.instance.SaveLevelData(starCurrentAmount);
            //    SceneLoader.Instance.LoadSpecificScene(GameDataManager.instance.NextSceneToLoad);
            //}
        }
    }

    private IEnumerator LevelFinishedSequence()
    {
        if (!IsFTUE)
        {
            if (MenuController.instance)
            {
                MenuController.instance.DisableGameplayHUD(true);
                GameplayScreen.instance.SetAllButtonsVisibility(false);
            }

        }

        //FOR TRAILER ONLY
        //if(IsFTUE)
        //yield return new WaitForSeconds(5.0f);

        phoenix.RevolveAroundPortal();
        yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(0.42f);
        portalEndTimeline.GetComponent<EndPortalTimelineChanger>().Play();
        cameraController.LevelFinishedSequence();
        for (int i = 0; i < playerTransforms.Length; i++)
        {
            //players[i].SetIsActive(true);
            //players[i].DisablePlayer(false);
            players[i].SetIdle();
        }

        FXManager.instance.FadeAllEnvironment();

        //if (isFTUE == false)
        //{

        //}
        yield return new WaitForSeconds(2.7f);
        phoenix.gameObject.SetActive(false);
        for (int i = 0; i < playerTransforms.Length; i++)
        {
            FXManager.instance.CharacterFadeIntoPortal(playerTransforms[i].position);
        }
        StartCoroutine("ScaleDownCharacterShadows");
        StartCoroutine("ScaleDownCharacters");
        yield return new WaitForSeconds(0.3f);
        FXManager.instance.GlowGate(2.0f);
        AudioManager.instance.PlaySoundEffect("PortalLoop");
        AudioManager.instance.PlaySoundEffect("PortalCharge");
        yield return new WaitForSeconds(0.75f);

        for (int i = 0; i < playerTransforms.Length; i++)
        {
            players[i].SetIsActive(false);
        }

        AudioManager.instance.PlaySoundEffect("PortalAction");

        yield return new WaitForSeconds(1.55f);
        AudioManager.instance.StopSoundEffect("PortalLoop");

        yield return new WaitForSeconds(1.5f);
        FXManager.instance.GlowGate(0.0f);

        yield return new WaitForSeconds(1.5f);

        if (isFTUE)
        {
            //phoenix.gameObject.SetActive(true);
            //phoenix.FlyAway();
            StartCoroutine("SetPPVolumeWeightToZero");
            yield return new WaitForSeconds(3.0f);
            //cameraController.FollowBird();
            FTUEHelper.instance.EnableBGSky();
            //SetAperture(2.0f, 1.0f, nextLevelPP);
        }
        else
        {
            yield return new WaitForSeconds(3.0f);
            if (GameController.instance)
            {
                //GameController.instance.LoadLevel(true);
                SceneLoader.Instance.LoadNextSceneInBuild();
            }
            //cameraController.MoveCameraSideways();
        }

        //RestartLevel();
        //GameController.instance.LoadLevel(true);

    }

    public void DisableEnvironment()
    {
        for (int i = 0; i < playerTransforms.Length; i++)
        {
            players[i].SetIsActive(false);
        }

        if (piecesManager)
        {
            piecesManager.gameObject.SetActive(false);
        }
        else
        {
            ftueEnvironment.SetActive(false);
        }

        if (elementsManager)
        {
            elementsManager.gameObject.SetActive(false);
        }
    }

    private IEnumerator ScaleDownCharacters()
    {
        float delayTime = 2.5f;

        while (delayTime > 0)
        {
            for (int i = 0; i < playerTransforms.Length; i++)
            {
                playerTransforms[i].localScale = Vector3.Lerp(playerTransforms[i].localScale, Vector3.zero, Time.deltaTime * 8.5f);
            }

            delayTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < playerTransforms.Length; i++)
        {
            playerTransforms[i].localScale = Vector3.zero;
        }
    }

    private IEnumerator ScaleDownCharacterShadows()
    {
        float t = 0;
        float startingFOV = 50.0f;

        while (t < 1.0f)
        {
            for (int i = 0; i < playerTransforms.Length; i++)
            {
                playerTransforms[i].Find("BlobShadow").GetComponent<Projector>().fieldOfView = Mathf.Lerp(startingFOV, 0, t);
            }

            t += Time.deltaTime * 3.0f;
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < playerTransforms.Length; i++)
        {
            playerTransforms[i].Find("BlobShadow").gameObject.SetActive(false);
        }
    }

    #endregion

    #region Functions on Players

    public void SwitchCharacter()
    {
        if (characterSwitchingDisabled)
            return;

        players[currentCharacterIndex].SetControlsDisabled(true);
        if (GameplayScreen.instance != null)
        {
            GameplayScreen.instance.ChangeCharacterIcon(false);
        }

        currentCharacterIndex++;
        if (currentCharacterIndex > playerTransforms.Length - 1)
        {
            currentCharacterIndex = 0;
            if (GameplayScreen.instance != null)
                GameplayScreen.instance.ChangeCharacterIcon(true);

        }
        players[currentCharacterIndex].SetControlsDisabled(false);

        if (IsFTUE == false)
        {
            cameraController.CharacterSwitched();
        }
    }

    public void SetCharacter(CharName charName)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetCharName() == charName)
            {
                players[currentCharacterIndex].SetIsActive(false);
                currentCharacterIndex = i;
                players[currentCharacterIndex].SetIsActive(true);
            }
        }

    }

    public void DisableAllPlayers()
    {
        for (int i = 0; i < playerTransforms.Length; i++)
        {
            players[i].DisablePlayer(true);
        }
    }

    public void EnableAllPlayers()
    {
        for (int i = 0; i < playerTransforms.Length; i++)
        {
            //if (i == currentCharacterIndex)
            //{
                players[i].EnablePlayer();
            //}

        }
    }

    public void MakePlayersChildrenOfPieces()
    {
        for (int i = 0; i < playerTransforms.Length; i++)
        {
            playerPreviousParents[i] = playerTransforms[i].parent;
            playerTransforms[i].parent = piecesManager.GetPieceHoverTransformFromPosition(new Vector2(playerTransforms[i].position.x, playerTransforms[i].position.y));
        }
    }

    public void MakePlayersChildrenOfWorld()
    {
        for (int i = 0; i < playerTransforms.Length; i++)
        {
            playerTransforms[i].parent = playerPreviousParents[i];
        }
    }

    public void SetControlsDisabled(bool value)
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetControlsDisabled(value);
        }
    }

    #endregion

    #region Changing PostProcess attributes
    private IEnumerator SetPPVolumeWeightToZero()
    {
        float speed = 0.0f;
        nextLevelPP.gameObject.SetActive(true);
        if (ppVolume && ppVolume.gameObject.activeSelf)
        {
            if (ppVolume.profile)
            {
                DepthOfField dof;
                nextLevelPP.profile.TryGetSettings(out dof);
                if (dof)
                {
                    dof.aperture.value = 13.0f;
                }

                while (ppVolume.weight > 0.05f)
                {
                    ppVolume.weight = Mathf.MoveTowards(ppVolume.weight, 0, Time.deltaTime * 0.25f * speed);
                    nextLevelPP.weight = 1.0f - ppVolume.weight;
                    if (speed < 2.0f)
                        speed += Time.deltaTime * 1.5f;
                    yield return new WaitForEndOfFrame();
                }

                ppVolume.weight = 0;
                nextLevelPP.weight = 1.0f;
                ppVolume.gameObject.SetActive(true);
            }
        }
    }

    public void SetAperture(float value)
    {
        if (ppVolume)
        {
            if (ppVolume.profile)
            {
                DepthOfField dof;
                ppVolume.profile.TryGetSettings(out dof);
                if (dof)
                {
                    dof.aperture.value = value;
                }
            }
        }
    }

    public void SetAperture(float value, float speed, PostProcessVolume postProcessVolume)
    {
        if (setApertureCoroutine != null)
        {
            StopCoroutine(setApertureCoroutine);
        }
        setApertureCoroutine = SetApertureCoroutine(value, speed, postProcessVolume);
        StartCoroutine(setApertureCoroutine);
    }

    private IEnumerator SetApertureCoroutine(float value, float speed, PostProcessVolume postProcessVolume)
    {
        if (postProcessVolume == null)
        {
            postProcessVolume = ppVolume;
        }

        if (postProcessVolume.profile)
        {
            DepthOfField dof;

            postProcessVolume.profile.TryGetSettings(out dof);
            if (dof)
            {
                while (Mathf.Abs(dof.aperture.value - value) > 0.01f)
                {
                    dof.aperture.value = Mathf.MoveTowards(dof.aperture.value, value, Time.deltaTime * speed);
                    yield return new WaitForEndOfFrame();
                }
                dof.aperture.value = value;
            }
        }
    }

    public void SetDebugPPS(PostProcessProfile postProcessProfile)
    {
        StopCoroutine("SetPPVolumeWeightToZero");
        if (setApertureCoroutine != null)
        {
            StopCoroutine(setApertureCoroutine);
        }

        if (postProcessProfile == null)
        {
            ppVolume.gameObject.SetActive(false);
        }
        else
        {
            ppVolume.gameObject.SetActive(true);
            ppVolume.profile = postProcessProfile;
        }
    }

    #endregion

    #region Getters
    public Transform[] GetAllPlayerTransforms()
    {
        return playerTransforms;
    }

    public PlayerController[] GetAllPlayers()
    {
        return players;
    }

    public Transform[] GetAllPlayerCenters()
    {
        return playerCenters;
    }

    public int GetNumPieces()
    {
        return piecesManager.GetNumPieces();
    }

    public Transform GetPieceHoverTransformFromPosition(Vector2 inputPosition)
    {
        return piecesManager.GetPieceHoverTransformFromPosition(inputPosition);
    }

    public Piece GetPieceFromPosition(Vector2 inputPosition)
    {
        return piecesManager.GetPieceFromPosition(inputPosition);
    }

    public bool IsConnected(Piece connectingPiece, int connectingDirection, Vector3 ropePosition)
    {
        return piecesManager.IsConnected(connectingPiece, connectingDirection, ropePosition);
    }

    public void AddPhoenixToElementsList()
    {
        elementsManager.AddPhoenixToElementsList();
    }

    public void ElementDestroyed(Transform elementToDestroy)
    {
        elementsManager.ElementDestroyed(elementToDestroy);
    }

    public void ContainsWaterInThisLevel()
    {
        hasWaterInLevel = true;
    }

    public Transform[] GetAllWaterObjects()
    {
        return elementsManager.GetAllWaterObjects();
    }

    public float GetPieceWidth()
    {
        return piecesManager.GetPieceWidth();
    }

    public float GetPieceHeight()
    {
        return piecesManager.GetPieceHeight();
    }

    #endregion

    #region GameStates
    public void PlayGameState()
    {
        gameState = GameStates.Play;
    }

    public void PauseGameState()
    {
        gameState = GameStates.None;
    }

    #endregion


    #region Star Collection
    public void HandleStarCollect(int crystalIndex)
    {
        Dictionary<string, object> eventData = new Dictionary<string, object>();
        eventData.Add(Constants.Analytics.TimeToCollectCrystal, currentCrystalCollectTime);
        eventData.Add(Constants.Analytics.CrystalNumber, crystalIndex);

        MyAnalytics.CustomEvent(Constants.Analytics.CrystalCollected, eventData);

        currentCrystalCollectTime = 0;

        starCurrentAmount++;
        if(GameplayScreen.instance)
            GameplayScreen.instance.HandleStarsUI();
    }

    public Vector3 GetWorldPositionWRTViewPortOfUICamera(Vector3 givenPosition)
    {
        //currentCrystalPosition = givenPosition;
        Vector3 worldToViewPortPos = cameraController.MainCamera.WorldToViewportPoint(givenPosition);
        Vector3 viewToWorldPos = MenuController.instance.UICamera.ViewportToWorldPoint(worldToViewPortPos);
        return viewToWorldPos;
    }

    public Vector3 GetWorldPositionWRTViewPortOfMainCamera(Vector3 givenPosition)
    {
        currentCrystalPosition = givenPosition;
        Vector3 worldToViewPortPos = MenuController.instance.UICamera.WorldToViewportPoint(givenPosition);
        worldToViewPortPos.z = -cameraController.MainCamera.transform.position.z;
        Vector3 viewToWorldPos = cameraController.MainCamera.ViewportToWorldPoint(worldToViewPortPos);
        return viewToWorldPos;
    }

    public Vector3 GetWorldPositionOfUIWRTMainCamera(Vector3 givenPosition)
    {
        
        Vector3 worldToViewPortPos = MenuController.instance.UICamera.WorldToViewportPoint(givenPosition);
        worldToViewPortPos.z = -cameraController.MainCamera.transform.position.z;
        Vector3 viewToWorldPos = cameraController.MainCamera.ViewportToWorldPoint(worldToViewPortPos);
        return viewToWorldPos;
    }

    public Vector3 GetCrystalPosition()
    {
        return GetWorldPositionWRTViewPortOfUICamera(currentCrystalPosition);
    }


    public Vector3 GetCrystalUIWorldPosition()
    {
        return GetWorldPositionWRTViewPortOfMainCamera(currentCrystalPosition);
    }

    #endregion

    private void OnDisable()
    {
        if (!GameplayScreen.instance)
        {
            InputManager.playButtonCallBack -= PlayGameState;
        }
    }


    #endregion
}

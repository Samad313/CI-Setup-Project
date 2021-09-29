using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class FXManager : MonoBehaviour
{
    #region Variables
    public static FXManager instance;

    //Gameplay
    [SerializeField] private Transform collectingCrystals = default;
    [SerializeField] private Transform collectingCrystalsForMenu = default;
    [SerializeField] private Transform collectingCrystalsForFTUE = default;
    [SerializeField] private Transform doorwaysConnect = default;
    [SerializeField] private Transform kidLanding = default;
    [SerializeField] private Transform ladderMove = default;
    [SerializeField] private Transform runningDust = default;
    [SerializeField] private Transform characterDrop = default;
    [SerializeField] private Transform waterEntry = default;
    [SerializeField] private Transform comingOutOfWater = default;
    [SerializeField] private Transform waterBubbles = default;

    //Level End Sequence
    [SerializeField] private Transform characterFadeIntoPortal = default;
    [SerializeField] private Transform characterFadeIntoPortalOrb = default;
    [SerializeField] private Transform birdAppear = default;
    [SerializeField] private Material gateMaterialToChange = default;

    //Screen Black
    [SerializeField] private GameObject blackFG = default;
    [SerializeField] private Renderer cameraFade = default;

    //FTUE
    [SerializeField] private CameraShake cameraShake = default;
    [SerializeField] private Transform shockwave = default;

    //Wisp
    [SerializeField] private Transform wisp;
    private Transform wispObjectForUI;

    //Piece connect/disconect
    [SerializeField] private float connectEffectSpeed = 1.8f;
    [SerializeField] private float disconnectEffectSpeed = 10.0f;
    [SerializeField] private float delayBeforeConnectingEffect = 0.0f;

    [SerializeField] private Color sidesBlueBoundaryColor = default;
    [SerializeField] private Color sidesYellowBoundaryColor = default;
    [SerializeField] private Color cameraFrontConnectColor = default;

    [SerializeField] private Renderer[] objectsToFadeOutInZoomIn;

    private float cameraFadeExtraSpeed = 1.0f;

    private Transform boyRunningDust;
    private Transform girlRunningDust;

    public Transform WispObjectForUI
    {
        get { return wispObjectForUI; }
    }

    #endregion

    #region Functions

    #region Inits & Update
    void Awake()
    {
        instance = this;
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += LogPlayModeState;
#endif
    }

#if UNITY_EDITOR
    private void LogPlayModeState(PlayModeStateChange state)
    {
        if(state==PlayModeStateChange.ExitingPlayMode)
        {
            if(GameplayManager.instance)
            {
                GameplayManager.instance.GetAllPlayers()[0].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
                GameplayManager.instance.GetAllPlayers()[1].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
                GameplayManager.instance.Phoenix.GetVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
            }
            GameData.instance.SidesBlueBoundary.SetColor("_Tint", Color.black);
            GameData.instance.SidesYellowBoundary.SetColor("_Tint", Color.black);
            GameData.instance.CameraFrontConnect.SetColor("_Tint", Color.black);
            GameData.instance.Aurora.SetColor("_TintColor", new Color32(255, 255, 255, 76));
            GameData.instance.SkyGradient.SetColor("_TintColor", new Color32(255, 255, 255, 76));
            GameData.instance.OverlayBlack.SetColor("_tint", new Color(1.0f, 1.0f, 1.0f, 0));
            gateMaterialToChange.renderQueue = 2000;
            gateMaterialToChange.SetFloat("_emissiveBoost", 0.04f);
        }
    }
#endif
    // Start is called before the first frame update
    void Start()
    {
        if (GameplayManager.instance == null)
            return;


        GameData.instance.SidesYellowBoundary.SetFloat("_boost", 0);
        GameData.instance.SidesYellowBoundary.SetFloat("_pulse_min", 0);

        if(GameplayManager.instance)
        {
            boyRunningDust = Instantiate(runningDust, GameplayManager.instance.GetAllPlayerTransforms()[0]);
            girlRunningDust = Instantiate(runningDust, GameplayManager.instance.GetAllPlayerTransforms()[1]);
            boyRunningDust.gameObject.SetActive(false);
            girlRunningDust.gameObject.SetActive(false);
        }

        if (GameplayManager.instance.ZoomedInStart == false)
        {
            blackFG.SetActive(false);
            blackFG.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
        }

        if(GameplayManager.instance.IsFTUE)
        {
            GameplayManager.instance.GetAllPlayers()[0].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
            GameplayManager.instance.GetAllPlayers()[1].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
        }
        else
        {
            GameplayManager.instance.GetAllPlayers()[0].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
            GameplayManager.instance.GetAllPlayers()[1].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
        }
        
        gateMaterialToChange.renderQueue = 2000;


        gateMaterialToChange.SetFloat("_emissiveBoost", 0.04f);
        if(GameplayManager.instance.PhoenixAlreadyOnGate || GameplayManager.instance.FirstLevel)
        {
            GameplayManager.instance.Phoenix.GetVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
        }
        else
        {
            GameplayManager.instance.Phoenix.GetVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 3021;
        }
        //}

        //if(GameplayManager.instance.GetIsFTUE())
        //  SetCameraFade(1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.L))
        //    ShakeCamera();
    }

    #endregion

    #region Gameplay  FX
    public void CrystalCollected(Vector3 inputPosition)
    {
        Transform tempTransform = Instantiate(collectingCrystals, null);
        tempTransform.position = inputPosition;
    }
    public void CrystalCollectedForMenu(Vector3 inputPosition, float darkenValue = 1.2f)
    {
        Transform tempTransform = Instantiate(collectingCrystalsForMenu, null);
        tempTransform.gameObject.GetComponent<AnimateSpritesheet>().AssignDarkenValue(darkenValue);
        tempTransform.gameObject.GetComponent<MeshRenderer>().material.renderQueue = 3100;
        tempTransform.position = inputPosition;
    }
    public void CrystalCollectedForFTUE(Vector3 inputPosition, float darkenValue = 1.2f)
    {
        Transform tempTransform = Instantiate(collectingCrystalsForFTUE, null);
        tempTransform.gameObject.GetComponent<AnimateSpritesheet>().AssignDarkenValue(darkenValue);
        tempTransform.position = inputPosition;
    }

    public void KidLanded(Vector3 inputPosition)
    {
        Transform tempTransform = Instantiate(kidLanding, null);
        tempTransform.position = inputPosition;
    }

    public void LadderMoved(Vector3 inputPosition, bool isLeft)
    {
        Transform tempTransform = Instantiate(ladderMove, null);

        if (!isLeft)
        {
            tempTransform.position = inputPosition + new Vector3(-1, 0, 0);
            tempTransform.localScale = new Vector3(-1.0f, 1, 1);
        }
        else
        {
            tempTransform.position = inputPosition + new Vector3(1, 0, 0);
        }
    }

    public void SetRunningDust(bool shouldEnable, CharName charName)
    {
        //if(charName==CharName.Boy)
        //{
        //    boyRunningDust.gameObject.SetActive(shouldEnable);
        //}
        //else
        //{
        //    girlRunningDust.gameObject.SetActive(shouldEnable);
        //}
    }

    public void CharacterDroppedInWater(Vector3 inputPosition, float rotationZ, float charSpeed, int nearestSide)
    {
        float lerpValue = Mathf.InverseLerp(10.0f, 25.0f, charSpeed);
        Vector3 spawnScale = Vector3.one * Mathf.Lerp(0.4f, 1.0f, lerpValue);

        Transform tempTransform = Instantiate(characterDrop, null);
        tempTransform.position = inputPosition;
        tempTransform.localEulerAngles = new Vector3(0, 0, rotationZ);
        tempTransform.localScale = spawnScale;

        if (nearestSide == 3)
        {
            Transform tempTransform2 = Instantiate(waterEntry, null);
            tempTransform2.position = inputPosition;
            tempTransform2.localEulerAngles = new Vector3(0, 0, rotationZ);

            tempTransform2.GetComponent<MoveTowards>().SetValues(inputPosition + new Vector3(0, -0.1f * charSpeed, 0), charSpeed * 0.2f, false);
            //tempTransform2.GetComponent<FollowTransform>().SetTransform(transformToFollow);
            //tempTransform2.GetComponent<FollowTransform>().SetOffset(tempTransform2.position - transformToFollow.position);
        }

    }

    public void CharacterComingOutOfWater(Vector3 inputPosition, float rotationZ, float charSpeed)
    {
        float lerpValue = Mathf.InverseLerp(6.0f, 15.0f, charSpeed);
        Vector3 spawnScale = Vector3.one * Mathf.Lerp(0.6f, 1.0f, lerpValue);
        Transform tempTransform = Instantiate(comingOutOfWater, null);
        tempTransform.position = inputPosition;
        tempTransform.localEulerAngles = new Vector3(0, 0, rotationZ);
        tempTransform.localScale = spawnScale;
    }

    public Transform GetWaterBubbles()
    {
        return waterBubbles;
    }

    #endregion

    #region Pieces Connect/Disconnect FX

    public void SetMaterialsWhileZoomingInOut(float t)
    {
        GameData.instance.SidesBlueBoundary.SetColor("_Tint", Color.Lerp(Color.black, sidesBlueBoundaryColor, t));
        GameData.instance.SidesYellowBoundary.SetColor("_Tint", Color.Lerp(Color.black, sidesYellowBoundaryColor, t));
        GameData.instance.CameraFrontConnect.SetColor("_Tint", Color.Lerp(Color.black, cameraFrontConnectColor, t));
        GameData.instance.Aurora.SetColor("_TintColor", Color.Lerp(new Color(1, 1, 1, 0.3f), new Color(1, 1, 1, 0.0f), t));
        GameData.instance.SkyGradient.SetColor("_TintColor", Color.Lerp(new Color(1, 1, 1, 0.3f), new Color(1, 1, 1, 0.0f), t));
        GameData.instance.OverlayBlack.SetColor("_tint", new Color(1.0f - t, 1.0f - t, 1.0f - t, 0));
        for (int i = 0; i < objectsToFadeOutInZoomIn.Length; i++)
        {
            objectsToFadeOutInZoomIn[i].material.SetFloat("_opacity", t);
        }
    }

    public void DoorwaysConnected(Transform transformToFollow, Vector3 offsetFromTransform)
    {
        Transform tempTransform = Instantiate(doorwaysConnect, null);
        offsetFromTransform += new Vector3(0, 0, -6.0f);
        tempTransform.position = transformToFollow.position + offsetFromTransform;
        tempTransform.GetComponent<FollowTransform>().SetTransform(transformToFollow);
        tempTransform.GetComponent<FollowTransform>().SetOffset(offsetFromTransform);
    }

    public void DoorwaysDisconnected(Transform transformToFollow, Vector3 offsetFromTransform)
    {
        //Transform tempTransform = Instantiate(doorwaysDisconnect, null);
        //offsetFromTransform += new Vector3(0, 0, -6.0f);
        //tempTransform.position = transformToFollow.position + offsetFromTransform;
        //tempTransform.GetComponent<FollowTransform>().SetTransform(transformToFollow);
        //tempTransform.GetComponent<FollowTransform>().SetOffset(offsetFromTransform);
    }

    public void ConnectDisconectLineTravel(Material inputMaterial, MoveDirection moveDirection, bool connected)
    {
        float rotateValue = 0.0f;
        
        if (moveDirection == MoveDirection.left)
            rotateValue = Mathf.PI;
        else if (moveDirection == MoveDirection.down)
            rotateValue = -Mathf.PI/2.0f;
        else if (moveDirection == MoveDirection.up)
            rotateValue = Mathf.PI/2.0f;

        if (!connected)
        {
            rotateValue = Mathf.PI;

            if (moveDirection == MoveDirection.left)
                rotateValue = 0;
            else if (moveDirection == MoveDirection.down)
                rotateValue = Mathf.PI / 2.0f;
            else if (moveDirection == MoveDirection.up)
                rotateValue = -Mathf.PI / 2.0f;
        }

        StartCoroutine(ConnectDisconectLineTravelCoroutine(inputMaterial, connected, moveDirection, rotateValue));
    }

    private IEnumerator ConnectDisconectLineTravelCoroutine(Material inputMaterial, bool connected, MoveDirection moveDirection, float rotateValue)
    {

        if (connected)
            yield return new WaitForSeconds(delayBeforeConnectingEffect);

        inputMaterial.SetFloat("_rotate", rotateValue);
        float startValue = -1.0f;
        float endValue = 1.0f;

        if(moveDirection==MoveDirection.left || moveDirection == MoveDirection.down)
        {
            startValue = 1.0f;
            endValue = -1.0f;
        }

        float t = 0.0f;
        float effectSpeed = disconnectEffectSpeed;
        if(connected)
            effectSpeed = connectEffectSpeed;

        if(connected)
            StartCoroutine("PulseYellowBorder");
        
        while (t<1.0f)
        {
            float alphaValue = Mathf.Lerp(0.16f, 1.0f, t);
            float valueToSet = Mathf.Lerp(startValue, endValue, t);
            if(connected)
                alphaValue = Mathf.Lerp(1.0f, 0.16f, t);

            inputMaterial.SetColor("_Tint", new Color(alphaValue, alphaValue, alphaValue, 0));
            inputMaterial.SetFloat("_wave_Horizontal_scroll", valueToSet);
            t += Time.deltaTime * effectSpeed;
            yield return new WaitForEndOfFrame();
        }

        inputMaterial.SetFloat("_wave_Horizontal_scroll", endValue);
    }

    private IEnumerator PulseYellowBorder()
    {
        GameData.instance.SidesYellowBoundary.SetFloat("_boost", 0);
        GameData.instance.SidesYellowBoundary.SetFloat("_pulse_min", 0);

        yield return new WaitForSeconds(0.4f);

        float t = 0.0f;
        while (t < 1.0f)
        {
            GameData.instance.SidesYellowBoundary.SetFloat("_boost", t);
            GameData.instance.SidesYellowBoundary.SetFloat("_pulse_min", t*3.0f);
            t += Time.deltaTime * 8.0f;
            yield return new WaitForEndOfFrame();
        }

        t = 1.0f;

        while (t > 0.0f)
        {
            GameData.instance.SidesYellowBoundary.SetFloat("_boost", t);
            GameData.instance.SidesYellowBoundary.SetFloat("_pulse_min", t * 3.0f);
            t -= Time.deltaTime * 2.0f;
            yield return new WaitForEndOfFrame();
        }

        GameData.instance.SidesYellowBoundary.SetFloat("_boost", 0);
        GameData.instance.SidesYellowBoundary.SetFloat("_pulse_min", 0);
    }

    #endregion

    #region Starting Sequence
    public void FadeInEnvironment()
    {
        StartCoroutine("FadeInEnvironmentCoroutine");
    }

    private IEnumerator FadeInEnvironmentCoroutine()
    {
        if (GameplayManager.instance.IsFTUE == false)
        {
            blackFG.SetActive(true);
            //SetRenderQueuesOfVisibleObjects();
            blackFG.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);

            yield return new WaitForSeconds(1.5f);

            float t = 1.0f;
            while (t > 0.0f)
            {
                blackFG.GetComponent<Renderer>().material.color = new Color(0, 0, 0, t);
                //GameplayManager.instance.SetAperture(Mathf.Lerp(2.0f, 13.0f, t));
                t -= Time.deltaTime * 0.3f;
                yield return new WaitForEndOfFrame();
            }
        }

        yield return new WaitForEndOfFrame();

        blackFG.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0.0f);
        blackFG.SetActive(false);

        GameplayManager.instance.Phoenix.GetVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
        //SetRenderQueuesToNormalOfVisibleObjects();
    }

    private void SetRenderQueuesToNormalOfVisibleObjects()
    {
        GameplayManager.instance.GetAllPlayers()[0].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
        GameplayManager.instance.GetAllPlayers()[1].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
        GameplayManager.instance.Phoenix.GetVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
        gateMaterialToChange.renderQueue = 2000;
        //for (int i = 0; i < gateObjectsForMaterialChange.Length; i++)
        //{
        //    gateObjectsForMaterialChange[i].GetComponent<Renderer>().material.renderQueue = 3010;
        //}
    }

    public void Skip()
    {
        GameplayManager.instance.Phoenix.GetVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
        cameraFadeExtraSpeed = 5.0f;
    }

    #endregion

    #region Ending Sequence

    public void FadeAllEnvironment()
    {
        StartCoroutine("FadeAllEnvironmentCoroutine");
    }

    private IEnumerator FadeAllEnvironmentCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        if(GameplayManager.instance.IsFTUE)
        {
            SetFogDensitytoZero();
        }
        
        blackFG.SetActive(true);
        SetRenderQueuesOfVisibleObjects();
        GameplayManager.instance.SetAperture(3.0f);
        float t = 0;
        while(t<1.0f)
        {
            blackFG.GetComponent<Renderer>().material.color = new Color(0, 0, 0, t);
            GameplayManager.instance.SetAperture(Mathf.Lerp(3.0f, 13.0f, t));
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        blackFG.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1.0f);

        GameplayManager.instance.DisableEnvironment();

        //PiecesManager.instance.gameObject.SetActive(false);
    }

    private void SetRenderQueuesOfVisibleObjects()
    {
        GameplayManager.instance.GetAllPlayers()[0].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 3023;
        GameplayManager.instance.GetAllPlayers()[1].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 3022;
        GameplayManager.instance.Phoenix.GetVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 3021;
        gateMaterialToChange.renderQueue = 3010;
        //for (int i = 0; i < gateObjectsForMaterialChange.Length; i++)
        //{
        //    gateObjectsForMaterialChange[i].GetComponent<Renderer>().material.renderQueue = 3010;
        //}
    }

    public void SetCharacterRenderQueueAbove3000(bool value)
    {
        if(value)
        {
            GameplayManager.instance.GetAllPlayers()[0].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 3023;
            GameplayManager.instance.GetAllPlayers()[1].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 3022;
        }
        else
        {
            GameplayManager.instance.GetAllPlayers()[0].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
            GameplayManager.instance.GetAllPlayers()[1].GetMyVisual().GetComponent<Renderer>().sharedMaterial.renderQueue = 2455;
        }
    }

    //private void FadeAllParticles()
    //{
    //    if (PiecesManager.instance == null)
    //        return;

    //    ParticleSystem[] particleSystem = PiecesManager.instance.gameObject.GetComponentsInChildren<ParticleSystem>();
    //    for (int i = 0; i < particleSystem.Length; i++)
    //    {
    //        var particleEmission = particleSystem[i].emission;
    //        particleEmission.rateOverTime = 0;
    //    }

    //    particleSystem = ElementsManager.instance.gameObject.GetComponentsInChildren<ParticleSystem>();
    //    for (int i = 0; i < particleSystem.Length; i++)
    //    {
    //        var particleEmission = particleSystem[i].emission;
    //        particleEmission.rateOverTime = 0;
    //    }
    //}

    public void CharacterFadeIntoPortal(Vector3 inputPosition)
    {
        Instantiate(characterFadeIntoPortal, inputPosition, Quaternion.identity, null);
        Instantiate(characterFadeIntoPortalOrb, inputPosition + new Vector3(0, 1.0f, 0), Quaternion.identity, null);
    }

    public void BirdDisappear(Vector3 inputPosition)
    {
        Instantiate(birdAppear, inputPosition, Quaternion.identity, null);
    }

    public void GlowGate(float glowValue)
    {
        StopCoroutine("GlowGateSequence");
        StartCoroutine("GlowGateSequence", glowValue);
    }

    public void SpawnWisp(Vector3 crystalCurrentPosition)
    {
        if (GameController.instance == null)
            return;

        Vector3 spawnPosition = crystalCurrentPosition;
        //spawnPosition.y -= 0.3f;
        //spawnPosition.x += 0.6f;
        wispObjectForUI = Instantiate(wisp, spawnPosition, Quaternion.identity);
        //Transform wispObject = Instantiate(wisp, null);
        //Vector3 spawnPosition = GameplayManager.instance.GetWorldPositionWRTViewPortOfUICamera(crystalCurrentPosition);
        //wispObjectForUI.transform.position = spawnPosition;
        wispObjectForUI.gameObject.GetComponent<HudWisp>().SetPosition(spawnPosition, GameplayScreen.instance.starsUIObject.GetCurrentStarWorldPosition());
        wispObjectForUI.gameObject.GetComponent<HudWisp>().StartMovement();
    }

    private IEnumerator GlowGateSequence(float glowValue)
    {
        float currentGlowValue = 0.0f;
        float speed = 2.0f;
        if (glowValue < 0.5f)
        {
            speed = 15.0f;
        }

        if (gateMaterialToChange)
        {
            currentGlowValue = gateMaterialToChange.GetFloat("_emissiveBoost");
        }
        else
        {
            yield break;
        }

        float currentSpeed = 0.0f;
        while (Mathf.Abs(currentGlowValue - glowValue) > 0.05f)
        {
            currentGlowValue = Mathf.Lerp(currentGlowValue, glowValue, Time.deltaTime * currentSpeed);

            gateMaterialToChange.SetFloat("_emissiveBoost", currentGlowValue);

            currentSpeed = Mathf.Lerp(currentSpeed, speed, Time.deltaTime * 2.0f);
            yield return new WaitForEndOfFrame();
        }
    }

    #endregion

    #region FTUE

    public void ShakeCamera()
    {
        cameraShake.ShakeCamera();
    }

    public void ShakeCamera(float amount, float duration)
    {
        cameraShake.ShakeCamera(amount, duration);
    }

    public void Shockwave()
    {
        Instantiate(shockwave);
    }

    public void SetCameraFade(float value, Renderer blackFG = null)
    {
        Renderer objectToFade = cameraFade;
        if(blackFG!=null)
        {
            objectToFade = blackFG;
        }

        if (value < 0.01f)
            objectToFade.gameObject.SetActive(false);
        else
            objectToFade.gameObject.SetActive(true);
        objectToFade.material.color = new Color(0, 0, 0, value);
    }

    public void FadeCamera(bool toBlack, float speed, Renderer blackFG = null)
    {
        StartCoroutine(FadeCameraCoroutine(toBlack, speed, blackFG));
    }

    private IEnumerator FadeCameraCoroutine(bool toBlack, float speed, Renderer blackFG)
    {
        cameraFadeExtraSpeed = 1.0f;
        float startingAlpha = 1.0f;
        float finalAlpha = 0.0f;

        if (toBlack)
        {
            startingAlpha = 0.0f;
            finalAlpha = 1.0f;
        }

        float t = 0.0f;
        while (t < 1.0f)
        {
            SetCameraFade(Mathf.Lerp(startingAlpha, finalAlpha, Easing.Ease(Equation.ExpoEaseIn, t, 0.0f, 1.0f, 1.0f)), blackFG);
            t += Time.deltaTime * speed * cameraFadeExtraSpeed;
            yield return new WaitForEndOfFrame();
        }

        SetCameraFade(finalAlpha);
    }

    public void SetBlackFG(bool shouldEnable)
    {
        blackFG.SetActive(shouldEnable);
    }

    public void SetFogDensitytoZero()
    {
        StartCoroutine("SetFogDensitytoZeroCoroutine");
    }

    private IEnumerator SetFogDensitytoZeroCoroutine()
    {
        float startingDensity = RenderSettings.fogDensity;
        float t = 0.0f;

        while(t<1.0f)
        {
            RenderSettings.fogDensity = Mathf.Lerp(startingDensity, 0, Easing.Ease(Equation.QuadEaseIn, t, 0, 1, 1));
            t += Time.deltaTime * 0.5f;
            yield return new WaitForEndOfFrame();
        }

        RenderSettings.fogDensity = 0.0f;
    }

    #endregion

    #endregion

    #region Piano Music Symbols
    public void SpawnMusicSymbol(int musicSymbolID ,Transform spawnPosition, Vector3 targetPosition, System.Action<int> Completed = null)
    {
        musicSymbolID -= 1;
        GameObject symbol = new GameObject();
        symbol.transform.position = spawnPosition.position;
        symbol.AddComponent<MeshFilter>();
        symbol.AddComponent<MeshRenderer>();
        symbol.GetComponent<MeshFilter>().mesh = GameData.instance.MusicNotes[musicSymbolID].sharedMesh;
        symbol.GetComponent<MeshRenderer>().material = GameData.instance.MusicSymbol;
        symbol.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
        symbol.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        StartCoroutine(MoveSymbol(symbol.transform, targetPosition, Completed));
        StartCoroutine(FadeOutMaterialAlpha(symbol.transform, 0.5f, 0f, 1f ,true));
    }



    private IEnumerator MoveSymbol(Transform symbol, Vector3 targetPosition, System.Action<int> Completed = null)
    {
        float timeElapsed = 0f;
        float endTime = 1f;
        //targetPosition.x -= 0.4f;
        Vector3 startPosition = symbol.position;
        Vector3 currentPosition = startPosition;

        while (timeElapsed < endTime)
        {
            currentPosition = Vector3.Lerp(startPosition, targetPosition, timeElapsed / endTime);
            symbol.transform.position = currentPosition;
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if(Completed != null)
            Completed?.Invoke(1);
    }

    public void AssignColor(Transform element, float endTime, Color fromColor, Color toColor)
    {
        StartCoroutine(ColorMaterialAnimation(element, endTime, fromColor, toColor));
    }

    private IEnumerator ColorMaterialAnimation(Transform element, float endTime, Color fromColor, Color toColor)
    {
        float timeElapsed = 0f;
        Material mat = element.GetComponent<MeshRenderer>().material;
        //mat.SetColor("_Color", fromColor);
        Color currentColor = mat.color;
        while (timeElapsed < endTime)
        {
            currentColor = Color.Lerp(fromColor, toColor, timeElapsed / endTime);
            mat.SetColor("_Color", currentColor);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }


        yield return null;
    }

    private IEnumerator FadeOutMaterialAlpha(Transform element, float endTime, float toAlphaValue, float startingValue ,bool wantToDestroyAtEnd)
    {
        yield return new WaitForSeconds(0.5f);
        float currentValue = 0f;
        float startShadeValue = startingValue;
        float timeElapsed = 0f;
        Material mat = element.GetComponent<MeshRenderer>().material;
       
        Color materialColor = mat.color;
        materialColor.a = startShadeValue;
        mat.SetColor("_Color", materialColor);

        while (timeElapsed < endTime)
        {
            currentValue = Mathf.Lerp(startShadeValue, toAlphaValue, timeElapsed / endTime);
            materialColor.a = currentValue;
            mat.SetColor("_Color", materialColor);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        materialColor.a = 0f;
        mat.SetColor("_Color", materialColor);
        if (wantToDestroyAtEnd)
        {
            yield return new WaitForSeconds(1f);
            Destroy(element.gameObject);
        }
    }


    #endregion

}

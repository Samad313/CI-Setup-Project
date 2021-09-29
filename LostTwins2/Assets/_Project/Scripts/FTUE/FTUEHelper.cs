using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FTUEHelper : MonoBehaviour
{
    public static FTUEHelper instance;

    [SerializeField]
    private Transform girlTransform = default;

    [SerializeField]
    private Transform boyTransform = default;

    [SerializeField]
    private Transform birdTransform = default;

    [SerializeField]
    private Transform cameraStarting = default;

    public Vector3 CameraStartingPosition
    {
        get { return cameraStarting.position; }
    }

    public Vector3 CameraStartingRotation
    {
        get { return cameraStarting.localEulerAngles; }
    }

    [SerializeField]
    private float cameraStartingFOV = 8.5f;

    public float CameraStartingFOV
    {
        get { return cameraStartingFOV; }
    }

    [SerializeField]
    private Transform girlPlaying = default;

    public Vector3 GirlPlayingPosition
    {
        get { return girlPlaying.position; }
    }

    [SerializeField]
    private Transform girlBeforePortal = default;

    public Vector3 GirlBeforePortalPosition
    {
        get { return girlBeforePortal.position; }
    }

    [SerializeField]
    private Transform rightWispStarting = default;

    public Vector3 RightWispStartingPosition
    {
        get { return rightWispStarting.position; }
    }

    [SerializeField]
    private Transform leftWispStarting = default;

    public Vector3 LeftWispStartingPosition
    {
        get { return leftWispStarting.position; }
    }

    [SerializeField]
    private Transform[] rightWispRests = default;

    [SerializeField]
    private Transform[] leftWispRests = default;

    [SerializeField]
    private Transform birdRevolving = default;

    public Vector3 BirdRevolvingPosition
    {
        get { return birdRevolving.position; }
    }

    [SerializeField]
    private Transform boySleeping = default;

    public Vector3 BoySleepingPosition
    {
        get { return boySleeping.position; }
    }

    [SerializeField]
    private Transform beforeCupboard = default;

    public Vector3 BeforeCupboardPosition
    {
        get { return beforeCupboard.position; }
    }

    [SerializeField]
    private Transform boyBeforePortal = default;

    public Vector3 BoyBeforePortalPosition
    {
        get { return boyBeforePortal.position; }
    }

    [SerializeField]
    private Vector3 birdInitialScale = default;

    public Vector3 BirdInitialScale
    {
        get { return birdInitialScale; }
    }

    [SerializeField]
    private Vector3 birdFinalScale = default;

    public Vector3 BirdFinalScale
    {
        get { return birdFinalScale; }
    }

    [SerializeField]
    private Rigidbody cupboard = default;

    private bool iBirdRevolving = false;

    [SerializeField]
    private GameObject[] shatteredGlasses = default;

    [SerializeField]
    private GameObject finalPortal = default;

    [SerializeField]
    private Image titleText = default;

    [SerializeField]
    private GameObject bgSkyGroup = default;

    [SerializeField]
    private GameObject titleTextCamera = default;

    [SerializeField]
    private Transform wisp1Transform = default;

    [SerializeField]
    private Transform wisp2Transform = default;

    [SerializeField]
    private CameraController cameraController = default;

    [SerializeField]
    private VideoPlayer lostTwinsLogoVideo = default;

    [SerializeField]
    private Transform lostTwinsLogoVideoPlane = default;

    [SerializeField]
    private bool showStatue = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //StartShockwave();
        SetStatue(showStatue);
    }

    public Vector3 RightWispRestPosition(int index)
    {
        return rightWispRests[index-1].position;
    }

    public Vector3 LeftWispRestPosition(int index)
    {
        return leftWispRests[index - 1].position;
    }

    public Transform GetGirlTransform()
    {
        return girlTransform;
    }

    public Transform GetBoyTransform()
    {
        return boyTransform;
    }

    public Transform GetBirdTransform()
    {
        return birdTransform;
    }

    public Transform GetWisp1Transform()
    {
        return wisp1Transform;
    }

    public Transform GetWisp2Transform()
    {
        return wisp2Transform;
    }

    public void StopShockwave()
    {
        StopCoroutine("Shockwave");
    }

    public void StartShockwave()
    {
        StopCoroutine("Shockwave");
        StartCoroutine("Shockwave");
    }

    private IEnumerator Shockwave()
    {
        while(true)
        {
            FXManager.instance.ShakeCamera();
            FXManager.instance.Shockwave();
            yield return new WaitForSeconds(5.0f);
        }
        
    }

    public void MoveTransform(Transform transformToMove,  Vector3 inputVector, float speed, bool isOffset)
    {
        if(isOffset)
            StartCoroutine(MoveTransformCoroutine(transformToMove, transformToMove.position + inputVector, speed));
        else
            StartCoroutine(MoveTransformCoroutine(transformToMove, inputVector, speed));


    }

    private IEnumerator MoveTransformCoroutine(Transform transformToMove, Vector3 finalPosition, float speed)
    {
        float t = 0;
        Vector3 startingPosition = transformToMove.position;

        while(t<1.0f)
        {
            transformToMove.position = Vector3.MoveTowards(startingPosition, finalPosition, t);
            t += Time.deltaTime * speed;
            yield return new WaitForEndOfFrame();
        }

        transformToMove.position = finalPosition;
    }

    public void ConvertIntoPhoenix(float speed)
    {
        StartCoroutine("ConvertIntoPhoenixCoroutine", speed);
    }

    private IEnumerator ConvertIntoPhoenixCoroutine(float speed)
    {
        while ((birdTransform.localScale - birdFinalScale).sqrMagnitude > 0.001f)
        {
            birdTransform.localScale = Vector3.MoveTowards(birdTransform.localScale, birdFinalScale, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }

        birdTransform.localScale = birdFinalScale;
    }

    public void RevolveBirdAroundPortal()
    {
        StartCoroutine("RevolveBirdAroundPortalCorotine");
    }

    private IEnumerator RevolveBirdAroundPortalCorotine()
    {
        iBirdRevolving = true;
        while (iBirdRevolving)
        {
            birdTransform.Rotate(0,0, Time.deltaTime * 50.0f);
            yield return new WaitForEndOfFrame();
        }
    }

    public void MakeCupboardFall()
    {
        StartCoroutine("CupboardFall");
    }

    private IEnumerator CupboardFall()
    {
        cupboard.AddTorque(new Vector3(-10000,0,0));
        yield return new WaitForSeconds(0.2f);
        cupboard.AddForce(new Vector3(50, 0, -350));
        yield return new WaitForSeconds(2.5f);
        cupboard.isKinematic = true;
    }

    public void ShatterGlass()
    {
        for (int i = 0; i < shatteredGlasses.Length; i++)
        {
            shatteredGlasses[i].SetActive(false);
        }
    }

    public void EnablePortal()
    {
        finalPortal.SetActive(true);
        finalPortal.GetComponent<FinalGate>().SetReachedDistance(10.0f);
    }

    public void SetBirdTopOfGate()
    {
        StopCoroutine("RevolveBirdAroundPortalCorotine");

        StartCoroutine("SetBirdTopOfGateCoroutine");
    }

    private IEnumerator SetBirdTopOfGateCoroutine()
    {
        Vector3 birdFinalPosition = birdTransform.position + new Vector3(0, 1.0f, 0);

        while ((birdTransform.position - birdFinalPosition).sqrMagnitude > 0.001f)
        {
            birdTransform.position = Vector3.MoveTowards(birdTransform.position, birdFinalPosition, Time.deltaTime * 4.0f);
            birdTransform.localRotation = Quaternion.RotateTowards(birdTransform.localRotation, Quaternion.identity, Time.deltaTime * 90.0f);
            yield return new WaitForEndOfFrame();
        }

        birdTransform.position = birdFinalPosition;
        birdTransform.localRotation = Quaternion.identity;
    }

    public void AnimatedTitleText()
    {
        StartCoroutine("AnimateTitleTextCoroutine");
    }

    private IEnumerator AnimateTitleTextCoroutine()
    {
        float t = 0;
        while (t < 1.0f)
        {
            titleText.fillAmount = t;
            titleText.material.color = new Color(1,1,1,t*2.0f);
            t += Time.deltaTime * 0.6f;
            yield return new WaitForEndOfFrame();
        }

        titleText.fillAmount = 1.0f;

        //yield return new WaitForSeconds(3.0f);

        //t = 1;
        //while (t > 0.0f)
        //{
        //    titleText.color = new Color(1, 1, 1, t);
        //    t -= Time.deltaTime * 0.4f;
        //    yield return new WaitForEndOfFrame();
        //}

        //titleText.color = new Color(1, 1, 1, 0);

        //if (GameController.instance)
        //{
        //    GameController.instance.LoadNextLevelAsync(null);
        //}
    }

    public void SaveTitleTextPosition()
    {
        titleText.transform.parent.GetComponent<TitleText>().SetOffset(cameraController.transform.position);
    }

    public Transform GetTitleTextGroup()
    {
        return titleText.transform.parent;
    }

    public void LoadFirstLevel()
    {

        if (GameController.instance)
        {
            //GameController.instance.LoadLevel(true);
            SceneLoader.Instance.LoadNextSceneInBuild();
        }
    }

    public void EnableBGSky()
    {
        bgSkyGroup.SetActive(true);
        FXManager.instance.FadeCamera(false, 0.25f);
        FXManager.instance.SetBlackFG(false);
        FadeInTitleTextImage();
    }

    private void FadeInTitleTextImage()
    {
        StartCoroutine("FadeInTitleTextImageCoroutine");
    }

    private IEnumerator FadeInTitleTextImageCoroutine()
    {
        Image titleTextImage = GameplayManager.instance.TitleText.transform.Find("Image").GetComponent<Image>();
        float t = 0.0f;
        while (t < 1.0f)
        {
            titleTextImage.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), Easing.Ease(Equation.ExpoEaseIn, t, 0.0f, 1.0f, 1.0f));
            lostTwinsLogoVideo.targetMaterialRenderer.material.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), Easing.Ease(Equation.ExpoEaseIn, t, 0.0f, 1.0f, 1.0f));
            t += Time.deltaTime * 0.25f;
            yield return new WaitForEndOfFrame();
        }

        titleTextImage.color = new Color(1, 1, 1, 1);
        lostTwinsLogoVideo.targetMaterialRenderer.material.color = new Color(1, 1, 1, 0);
    }

    private void EnableTitleTextCamera()
    {
        titleTextCamera.SetActive(true);
    }

    public void SetStuffForEndingAnimation()
    {
        if (GameplayScreen.instance)
            GameplayScreen.instance.SetAllButtonsVisibility(false);

        //GameplayManager.instance.SetCharacter(CharName.Boy);
        //GameplayManager.instance.GetCurrentPlayer().SetBenSleeping();

        GameplayManager.instance.SetCharacter(CharName.Girl);
        GetGirlTransform().position = new Vector3(14, 0, 0);
        GetBoyTransform().position = new Vector3(-14, 0, 0);
        GetBirdTransform().position = RightWispRestPosition(3);
        GetBirdTransform().localScale = BirdInitialScale;
        cameraController.SetPositionNow();
        GetWisp1Transform().GetComponent<Wisp>().SetPosition(RightWispRestPosition(3));
        GetWisp2Transform().GetComponent<Wisp>().SetPosition(LeftWispRestPosition(3));
        GetWisp1Transform().GetComponent<Wisp>().StartMovement();
        GetWisp2Transform().GetComponent<Wisp>().StartMovement();
        RenderSettings.fogDensity = 0.006f;

        GetBirdTransform().GetComponent<Phoenix>().SetFlyingAnimationForFTUE(1.0f);
        //GetBirdTransform().GetComponent<Phoenix>().SetRevolvingAnimation();

        MoveTransform(GetBirdTransform(), GameplayManager.instance.FinalPortal.GetGateTop().position, 3.0f, false);

        ConvertIntoPhoenix(3.0f);
        //FXManager.instance.SetCameraFade(1.0f);
    }

    public void HoldObjectPosition(Transform inputTransform)
    {
        holdingPosition = true;
        StartCoroutine("HoldObjectPositionCoroutine", inputTransform);
    }

    private void SetStatue(bool value)
    {
        GameplayManager.instance.FinalPortal.SetStatue(value);
        GetBirdTransform().GetComponent<Phoenix>().SetVisual(!value);
    }

    private bool holdingPosition = false;

    private IEnumerator HoldObjectPositionCoroutine(Transform inputTransform)
    {
        Vector3 startingPosition = inputTransform.position;

        while(holdingPosition)
        {
            inputTransform.position = startingPosition;
            yield return new WaitForEndOfFrame();
        }
    }

    public CameraController GetCameraController()
    {
        return cameraController;
    }

    public void PlayLostTwinsLogoVideo()
    {
        lostTwinsLogoVideo.Prepare();
        StartCoroutine("WaitBeforeVideoIsPrepared");
    }

    private IEnumerator WaitBeforeVideoIsPrepared()
    {
        yield return new WaitUntil(() => lostTwinsLogoVideo.isPrepared);

        lostTwinsLogoVideo.Play();
        EnableTitleTextCamera();
        
    }
}

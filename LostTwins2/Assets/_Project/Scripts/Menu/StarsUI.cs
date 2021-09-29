using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarsUI : MonoBehaviour
{
    [System.Serializable]
    public class StarsElements
    {
        [SerializeField]
        private RawImage emptyStarImage;
        [SerializeField]
        private RawImage fillStarImage;
        [SerializeField]
        private RawImage glowImage;

        public RawImage EmptyStarImage
        {
            get { return emptyStarImage; }
        }
        public RawImage FillStarImage
        {
            get { return fillStarImage; }
        }
        public RawImage GlowImage
        {
            get { return glowImage; }
        }
    }

    [SerializeField]
    private List<StarsElements> starsElements = new List<StarsElements>();
    private int currentStarIndex = 0;

    private void Start()
    {
        if (GameplayScreen.instance)
            DisableStars();
        else
            DisableStars(true);
        //GameController.instance.gameControllerInit += StarsUIInit;

        //StarsUIInit();
    }

    public void StarsUIInit()
    {
        Invoke("InvokeStarsUIInit", 0.1f);
    }

    private void InvokeStarsUIInit()
    {
        if (GameplayManager.instance)
        {
            if (!GameplayManager.instance.IsFTUE)
            {
                DefaultStarsUIState();
                currentStarIndex = 0;
            }
            else
            {
                DisableStars();
            }
        }
    }

    private void DefaultStarsUIState()
    {

        for (int i = 0; i < starsElements.Count; i++)
        {
            starsElements[i].EmptyStarImage.gameObject.SetActive(true);
            starsElements[i].FillStarImage.gameObject.SetActive(false);
            starsElements[i].GlowImage.gameObject.SetActive(false);

        }
    }

    public void EnableStarFillImage()
    {
        if(currentStarIndex < starsElements.Count)
        {
            StartCoroutine(TurnOnStarFillImage());
        }
    }

    private IEnumerator TurnOnStarFillImage()
    {
        //yield return new WaitForSeconds(0.7f);

        yield return new WaitUntil( ()=> FXManager.instance.WispObjectForUI.GetComponent<HudWisp>().IsWispReachedToUICrystal );
        FXManager.instance.CrystalCollectedForMenu(starsElements[currentStarIndex].FillStarImage.transform.position, 1.2f);
        yield return new WaitForSeconds(0.3f);

        starsElements[currentStarIndex].FillStarImage.gameObject.SetActive(true);
        starsElements[currentStarIndex].GlowImage.gameObject.SetActive(true);
        StartCoroutine(LerpWaveAlpha(starsElements[currentStarIndex].FillStarImage, 0.5f));

        currentStarIndex++;
    }

    private IEnumerator LerpWaveAlpha(RawImage image, float endTime)
    {
        float timeElapsed = 0f;
        Color imageColor = image.color;
        imageColor.a = 0.1f;
        image.color = imageColor;

        while (timeElapsed < endTime)
        {
            imageColor.a = Mathf.Lerp(0.1f, 1f, timeElapsed / endTime);
            image.color = imageColor;
            timeElapsed += Time.deltaTime;
            //Debug.Log(timeElapsed);
            yield return new WaitForEndOfFrame();
        }

    }

    //public Vector3 GetCurrentStarWorldPosition()
    //{
    //    Vector3 worldPos = starsElements[currentStarIndex].FillStarImage.transform.position;
    //    return worldPos;
    //}

    public Vector3 GetCurrentStarWorldPosition()
    {
        Vector3 worldPos = starsElements[currentStarIndex].FillStarImage.transform.position;
        Vector3 result = GameplayManager.instance.GetWorldPositionWRTViewPortOfMainCamera(worldPos);
        return result;
    }

    public void DisableStars(bool disableInUI = false) // boolean to differentiate between Main Menu UI and Gameplay disable stars, in Main Menu empty crystals should always be visible;
    {

        for (int i = 0; i < starsElements.Count; i++)
        {
            if (!disableInUI)
                starsElements[i].EmptyStarImage.gameObject.SetActive(false);

            starsElements[i].FillStarImage.gameObject.SetActive(false);
            starsElements[i].GlowImage.gameObject.SetActive(false);
        }
    }

    //For Display Crystals on UI when each level is being selected;
    public void ShowFilledCrystalsOnMainMenu(int count)
    {
        for(int i = 0; i < count; i++)
        {
            starsElements[i].FillStarImage.gameObject.SetActive(true);
            starsElements[i].GlowImage.gameObject.SetActive(true);
        }
    }

}

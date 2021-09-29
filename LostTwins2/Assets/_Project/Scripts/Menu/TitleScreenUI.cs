using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField]
    private RawImage crystalImage;

    [SerializeField]
    private RawImage logo;

    [SerializeField]
    private RawImage outerWave1;
    [SerializeField]
    private RawImage innerWave1;

    [SerializeField]
    private RawImage outerWave2;
    [SerializeField]
    private RawImage innerWave2;

    [SerializeField]
    private GameObject crystalParent;

    private float t = 0;
    private Vector3 crystalScale = Vector3.one;
    //private Vector3 crystalScale = new Vector3(0.6f, 0.6f, 0.6f);

    private float multiplierValue = 2f;
    private bool isAlternate = false;
    private bool onlyOnce = false;
    public float normalisedSineDivisorValue = 2f;
    
    public Vector3 GetCrystalPosition
    {
        get { return GameplayManager.instance.GetWorldPositionOfUIWRTMainCamera(crystalImage.transform.position); }
    }


    private void AnimateWaves()
    {
        if(onlyOnce)
        {
            onlyOnce = false;
            isAlternate = !isAlternate;

            RawImage outerW = isAlternate ? outerWave1 : outerWave2;
            RawImage InnerW = isAlternate ? innerWave1 : innerWave2;

            StartCoroutine(WaveMovement(outerW, 0.1f, 2f));
            StartCoroutine(WaveMovement(InnerW, 0.1f, 2.5f, 0.2f));
            StartCoroutine(LerpWaveAlpha(outerW, 2f));
            StartCoroutine(LerpWaveAlpha(InnerW, 2.5f, 0.2f));
        }    
    }

  
    // Update is called once per frame
    void Update()
    {
        if (!crystalImage.gameObject.activeInHierarchy)
            return;

        float sinValue = Mathf.Sin(t);
        float normalizedSine = sinValue / normalisedSineDivisorValue;

        crystalParent.gameObject.transform.localScale = crystalScale * (1.0f + normalizedSine);
        crystalImage.gameObject.transform.localPosition = new Vector3(0, Mathf.Sin(t + 1.0f) * 0.3f, 0);
        t += Time.deltaTime * multiplierValue;

        if (sinValue >= 0.95f)
        {
            multiplierValue = 3f;
            onlyOnce = true;
        }
        else if (sinValue <= -0.95f)
        {
            multiplierValue = 2f;
            AnimateWaves();

        }

    }

    public void OnButtonTap()
    {
        ResetToDefault();
        AnimateTitleScreen();
        FXManager.instance.CrystalCollectedForFTUE(crystalImage.transform.position, 2.2f);

    }

    public void EnableTitleScreen()
    {
        FadeInElements();
    }

    public void DisableTitleScreen()
    {
        ResetImageProperties(logo, 1f, false);
        ResetImageProperties(crystalImage, 1f, false);
        ResetImageProperties(outerWave1, 1f, false);
    }

    private void AnimateTitleScreen()
    {
        StartCoroutine(LerpDissolveAmount(logo, 1f, 1f));
        StartCoroutine(LerpDissolveAmount(outerWave1, 0.8f, 1f ,0.2f));
        StartCoroutine(LerpDissolveAmount(crystalImage, 0.5f, 1f));
    }

    private void ResetToDefault()
    {
        ResetImageProperties(crystalImage, 0f, true);
        ResetImageProperties(logo, 0f, true);
        ResetImageProperties(outerWave1, 0f, true);
    }

    private void ResetImageProperties(RawImage image, float value, bool activeStatus)
    {
        image.material.SetFloat("_DissolutionLevel", value);
        image.gameObject.SetActive(activeStatus);
    }

    private void FadeInElements()
    {
        SetImageDefaultColorAlpha(outerWave1, true);
        SetImageDefaultColorAlpha(outerWave2, true);
        SetImageDefaultColorAlpha(innerWave1, true);
        SetImageDefaultColorAlpha(innerWave2, true);

        ResetImageProperties(logo, 0f, true);
        ResetImageProperties(crystalImage, 0f, true);
        ResetImageProperties(outerWave1, 0f, true);
      
    }

    public void SetImageDefaultColorAlpha(RawImage image, bool isDefaultColor)
    {
        if (isDefaultColor)
        {
            Color imageColor = image.color;
            imageColor.a = 0f;
            image.color = imageColor;
        }
        else
        {
            Color imageColor = image.color;
            imageColor.a = 1f;
            image.color = imageColor;
        }

    }


    private IEnumerator LerpDissolveAmount(RawImage image, float endTime, float endValue, float waitTime = 0f)
    {
        Material currentMaterial = image.material;
        image.gameObject.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        float timeElapsed = 0f;
        float currentValue = 0f;
        float startValue = currentMaterial.GetFloat("_DissolutionLevel");
        while (timeElapsed < endTime)
        {
            timeElapsed += Time.deltaTime;
            currentValue = Mathf.Lerp(startValue, endValue, timeElapsed / endTime);
            currentMaterial.SetFloat("_DissolutionLevel", currentValue);
            yield return new WaitForEndOfFrame();
        }
        image.gameObject.SetActive(false);
    }

    private IEnumerator LerpImageAlphaValue(RawImage image, float endTime, float waitTime = 0f)
    {
        float timeElapsed = 0f;
        Color color = image.color;
        color.a = 0f;
        image.color = color;

        yield return new WaitForSeconds(waitTime);

        while (timeElapsed < endTime)
        {
            timeElapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timeElapsed / endTime);
            image.color = color;
            yield return new WaitForEndOfFrame();
        }

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

            currentScale = Vector3.Lerp(startScale, new Vector3(1.5f, 1.5f, 1.5f), timeElapsed / endTime);
            image.transform.localScale = currentScale;
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }

    private IEnumerator LerpWaveAlpha(RawImage image, float endTime, float waitTime = 0f)
    {
        yield return new WaitForSeconds(waitTime);
        float timeElapsed = 0f;
        Color imageColor = image.color;
        imageColor.a = 1f;
        image.color = imageColor;

        while (timeElapsed < endTime)
        {
            imageColor.a = Mathf.Lerp(1f, 0f, timeElapsed);
            image.color = imageColor;
            timeElapsed += Time.deltaTime / endTime;
            yield return new WaitForEndOfFrame();
        }

    }



}

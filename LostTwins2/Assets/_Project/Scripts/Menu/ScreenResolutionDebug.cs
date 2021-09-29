using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenResolutionDebug : MonoBehaviour
{
    #region Attributes

    #region PlayerPrefsKey

    private const string Resolution_Pref_Key = "Resolution";

    #endregion

    #region Resolution

    [SerializeField]
    private Text resolutionText;

    private Resolution[] resolutions;
    private int[] heightResolution = new int[14] { 480, 524 , 600, 720, 768, 800, 900, 960, 1050, 1120, 1440, 1680, 1920, 2240};
    private int currentResolutionIndex = 0;

    float aspectRatio = 0f;

    private float currentHeight = 0;
    private float currentWidth = 0;
    private float defaultHeight = 0;
    private float defaultWidth = 0;
    private float currentPercentage = 100;

    #endregion


    #endregion

    // Start is called before the first frame update
    void Start()
    {
      
        //CalculateAspectRatio();
        //currentResolutionIndex = PlayerPrefs.GetInt(Resolution_Pref_Key , 0);
        //int width = CalculateWidth(heightResolution[currentResolutionIndex]);
        //SetResolutionText(width, heightResolution[currentResolutionIndex]);

      
        //Resolution according to percentage
        GetCurrentWidthHeight();
        HandleWidthHeight();
        ApplyResolution();

    }

    private void HandleWidthHeight()
    {
        CalculateAndSetWidthHeightAccordingToPercentage();
    }

    private void GetCurrentWidthHeight()
    {
        currentHeight = defaultHeight = Screen.height;
        currentWidth = defaultWidth = Screen.width;
    }

    private void CalculateAndSetWidthHeightAccordingToPercentage()
    {
        if(currentPercentage >= 100)
        {
            currentHeight = defaultHeight;
            currentWidth = defaultWidth;
            return;
        }

        currentHeight = defaultHeight * (currentPercentage / 100f);
        currentWidth = defaultWidth * (currentPercentage / 100f);

        //Debug.Log("Current percenatge = " + currentPercentage / 100);

        //Debug.Log("Default Res Height = " + defaultHeight);
        //Debug.Log("Default Res Width = " + defaultWidth);

        //Debug.Log("Height = " + currentHeight);
        //Debug.Log("Width = " + currentWidth);
    }

    private void IncreasePercentage()
    {
        if(currentPercentage < 100)
        {
            currentPercentage += 1;
        }
    }

    private void DecreasePercentage()
    {
        if(currentPercentage > 10)
        {
            currentPercentage -= 1;
        }
    }



    private void CalculateAspectRatio()
    {
        aspectRatio = (float)Screen.width / (float)Screen.height;
        Debug.Log("Screen Width = " + Screen.width);
        Debug.Log("Screen Height = " + Screen.height);
        Debug.Log("Aspect ratio = " + aspectRatio);
    }

    #region Resolution Cycling

    private void SetResolutionText(Resolution resolution)
    {
        resolutionText.text = resolution.width + "x" + resolution.height;
    }
    private void SetResolutionText()
    {
        resolutionText.text = currentPercentage.ToString() + "%";
    }

    private void SetResolutionText(int width ,int height)
    {
        resolutionText.text = width.ToString() + " x " + height.ToString();
    }

    public void SetNextResolution()
    {
        //currentResolutionIndex = GetNextWrappedIndex(heightResolution, currentResolutionIndex);
        //int width = CalculateWidth(heightResolution[currentResolutionIndex]);
        //SetResolutionText(width ,heightResolution[currentResolutionIndex]);

        IncreasePercentage();
        HandleWidthHeight();
        //ApplyResolution();
        SetResolutionText();

    }

    public void SetPreviousResolution()
    {
        //currentResolutionIndex = GetPreviousWrappedIndex(heightResolution, currentResolutionIndex);
        //int width = CalculateWidth(heightResolution[currentResolutionIndex]);
        //SetResolutionText(width ,heightResolution[currentResolutionIndex]);

        DecreasePercentage();
        HandleWidthHeight();
        SetResolutionText();
        //ApplyResolution();

    }

    #endregion


    #region Apply resolution

    private void SetAndApplyResolution(int newResolutionIndex)
    {
        currentResolutionIndex = newResolutionIndex;
        ApplyCurrentResolution();
    }

    private void ApplyCurrentResolution()
    {
        //Debug.Log("Resolution = "+ resolutions[currentResolutionIndex].ToString());

        ApplyResolution(heightResolution);
        //ApplyResolution(resolutions[currentResolutionIndex]);
    }

    private void ApplyResolution(Resolution resolution)
    {
        SetResolutionText(resolution);
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt(Resolution_Pref_Key, currentResolutionIndex);
    }

    private void ApplyResolution(int[] heights)
    {
        int width = CalculateWidth(heightResolution[currentResolutionIndex]);
        int height = heightResolution[currentResolutionIndex];
        SetResolutionText(width ,heightResolution[currentResolutionIndex]);

        Debug.Log("Width = " + width);
        Debug.Log("Height = " + height);

        Screen.SetResolution(width, height, Screen.fullScreen);
        PlayerPrefs.SetInt(Resolution_Pref_Key, currentResolutionIndex);
    }

    private void ApplyResolution()
    {
        Screen.SetResolution((int)currentWidth, (int)currentHeight, Screen.fullScreen);
        Debug.Log("Screen Width = " + Screen.width);
        Debug.Log("Screen Height = " + Screen.height);
        SetResolutionText();
    }

    private int CalculateWidth(int height)
    {
        float width = aspectRatio * height;
        return (int)width;
    }

    #endregion

    #region Index Wrap Helpers

    private int GetNextWrappedIndex<T>(IList<T> collection, int currentIndex)
    {
        if (collection.Count < 0) return 0;
        return (currentIndex + 1) % collection.Count;
    }

    private int GetPreviousWrappedIndex<T>(IList<T> collection, int currentIndex)
    {
        if (collection.Count < 0) return 0;
        if ((currentIndex - 1) < 0) return collection.Count - 1;
        return (currentIndex - 1) % collection.Count;
    }

    #endregion



    public void ApplyChanges()
    {
        //SetAndApplyResolution(currentResolutionIndex);
        ApplyResolution();
        Debug.Log("Resoultion Changes Applied");
    }
}

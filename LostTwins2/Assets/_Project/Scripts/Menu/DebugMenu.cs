using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class DebugMenu : MonoBehaviour
{
    public static DebugMenu instance;

    [SerializeField]
    private TextMeshProUGUI textureQualityText;

    [SerializeField]
    private TextMeshProUGUI antialiasingText;

    [SerializeField]
    private Toggle bloomToggle;

    [SerializeField]
    private Toggle colorGradingToggle;

    [SerializeField]
    private Toggle depthOfFieldToggle;

    private string textureQuality;

    private string antialiasing;

    private int bloom;
    private int colorGrading;
    private int depthOfField;

    [SerializeField]
    private PostProcessProfile[] profiles;

    private PostProcessProfile myCurrentProfile;

    public PostProcessProfile MyCurrentProfile { get { return myCurrentProfile; } }

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("textureQuality"))
        {
            textureQuality = PlayerPrefs.GetString("textureQuality");
        }
        else
        {
            textureQuality = "Full Res";
            PlayerPrefs.SetString("textureQuality", textureQuality);
        }

        if (PlayerPrefs.HasKey("antialiasing"))
        {
            antialiasing = PlayerPrefs.GetString("antialiasing");
        }
        else
        {
            antialiasing = "4x";
            PlayerPrefs.SetString("antialiasing", antialiasing);
        }

        if (PlayerPrefs.HasKey("bloom"))
        {
            bloom = PlayerPrefs.GetInt("bloom");
        }
        else
        {
            bloom = 1;
            PlayerPrefs.SetInt("bloom", bloom);
        }

        if (PlayerPrefs.HasKey("colorGrading"))
        {
            colorGrading = PlayerPrefs.GetInt("colorGrading");
        }
        else
        {
            colorGrading = 1;
            PlayerPrefs.SetInt("colorGrading", colorGrading);
        }

        if (PlayerPrefs.HasKey("depthOfField"))
        {
            depthOfField = PlayerPrefs.GetInt("depthOfField");
        }
        else
        {
            depthOfField = 1;
            PlayerPrefs.SetInt("depthOfField", depthOfField);
        }

        SetTextureSizes();
        SetAntialiasing();
        SetToggleStates();

        SetMyCurrentProfile();

        gameObject.SetActive(false);
    }

    public void SwitchTextureSizes()
    {
        if(textureQuality=="Full Res")
        {
            textureQuality = "Half Res";
        }
        else
        {
            textureQuality = "Full Res";
        }

        PlayerPrefs.SetString("textureQuality", textureQuality);

        SetTextureSizes();
    }

    private void SetTextureSizes()
    {
        if(textureQuality=="Full Res")
        {
            QualitySettings.masterTextureLimit = 0;
        }
        else
        {
            QualitySettings.masterTextureLimit = 1;
        }

        textureQualityText.text = textureQuality;
    }

    public void SwitchAntialiasing()
    {
        if (antialiasing == "4x")
        {
            antialiasing = "2x";
        }
        else if (antialiasing == "2x")
        {
            antialiasing = "None";
        }
        else
        {
            antialiasing = "4x";
        }

        PlayerPrefs.SetString("antialiasing", antialiasing);

        SetAntialiasing();
    }

    public void SetAntialiasing()
    {
        if (antialiasing == "4x")
        {
            QualitySettings.antiAliasing = 4;
        }
        else if (antialiasing == "2x")
        {
            QualitySettings.antiAliasing = 2;
        }
        else
        {
            QualitySettings.antiAliasing = 0;
        }

        antialiasingText.text = antialiasing;
    }

    public void BloomValueChanged(bool value)
    {
        bloom = value ? 1 : 0;
        PlayerPrefs.SetInt("bloom", bloom);
        SetMyCurrentProfile();
        if (GameplayManager.instance)
        {
            GameplayManager.instance.SetDebugPPS(myCurrentProfile);
        }
    }

    public void ColorGradingValueChanged(bool value)
    {
        colorGrading = value ? 1 : 0;
        PlayerPrefs.SetInt("colorGrading", colorGrading);
        SetMyCurrentProfile();
        if (GameplayManager.instance)
        {
            GameplayManager.instance.SetDebugPPS(myCurrentProfile);
        }
    }

    public void DOFValueChanged(bool value)
    {
        depthOfField = value ? 1 : 0;
        PlayerPrefs.SetInt("depthOfField", depthOfField);
        SetMyCurrentProfile();
        if (GameplayManager.instance)
        {
            GameplayManager.instance.SetDebugPPS(myCurrentProfile);
        }
    }

    private void SetToggleStates()
    {
        bloomToggle.isOn = (bloom == 1);
        colorGradingToggle.isOn = (colorGrading == 1);
        depthOfFieldToggle.isOn = (depthOfField == 1);
    }

    private void SetMyCurrentProfile()
    {
        if(bloom==0 && colorGrading==0 && depthOfField==0)
        {
            myCurrentProfile = null;
        }
        else if (bloom == 0 && colorGrading == 0 && depthOfField == 1)
        {
            myCurrentProfile = profiles[0];
        }
        else if (bloom == 0 && colorGrading == 1 && depthOfField == 0)
        {
            myCurrentProfile = profiles[1];
        }
        else if (bloom == 0 && colorGrading == 1 && depthOfField == 1)
        {
            myCurrentProfile = profiles[2];
        }
        else if (bloom == 1 && colorGrading == 0 && depthOfField == 0)
        {
            myCurrentProfile = profiles[3];
        }
        else if (bloom == 1 && colorGrading == 0 && depthOfField == 1)
        {
            myCurrentProfile = profiles[4];
        }
        else if (bloom == 1 && colorGrading == 1 && depthOfField == 0)
        {
            myCurrentProfile = profiles[5];
        }
        else if (bloom == 1 && colorGrading == 1 && depthOfField == 1)
        {
            myCurrentProfile = profiles[6];
        }
    }
}

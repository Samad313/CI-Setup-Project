using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;
using TMPro;

public class FramesPerSecond : MonoBehaviour
{
	private float updateInterval = 0.5f;
	private float accum = 0.0f;
	private int frames = 0;
	private float timeLeft;
	
	private float FPS = 0.0f;

    private Component fpsTextRef;
    PropertyInfo textProperty;

	// Use this for initialization
	void Start ()
	{
        //Application.targetFrameRate = 60;
		timeLeft = updateInterval;

        if(GetComponent<Text>())
        {
            fpsTextRef = GetComponent<Text>();
            textProperty = typeof(Text).GetProperty("text");
            enabled = true;
        }
        else if(GetComponent<TextMesh>())
        {
            fpsTextRef = GetComponent<TextMesh>();
            textProperty = typeof(TextMesh).GetProperty("text");
            enabled = true;
        }
        else if (GetComponent<TextMeshProUGUI>())
        {
            fpsTextRef = GetComponent<TextMeshProUGUI>();
            textProperty = typeof(TextMeshProUGUI).GetProperty("text");
            enabled = true;
        }
#if !UNITY_5_3_OR_NEWER
        else if(GetComponent<GUIText>())
        {
            fpsTextRef = GetComponent<GUIText>();
            textProperty = typeof(GUIText).GetProperty("text");
            enabled = true;
        }
#endif
        else
        {
            this.enabled = false;
            Destroy(this);
        }
	}
	
	// Update is called once per frame
	void Update ()
	{
		timeLeft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		frames++;

		if( timeLeft <= 0.0f )
		{
			FPS = (accum/frames);
			timeLeft = updateInterval;
			accum = 0.0f;
			frames = 0;

            textProperty.SetValue(fpsTextRef, ""+FPS.ToString("00"), null);
		}
	}
}
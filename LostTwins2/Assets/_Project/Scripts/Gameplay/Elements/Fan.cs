using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{

    #region Private Variables
    private float fanThrottle;
    private const float perSecondDegrees = 6f; //1 rpm = 6 degrees/sec
    private float fanTopSpeed = 1f;
    private int fanCurrentBlurStateIndex = -1;
    #endregion

    #region Exposed Variables
    [SerializeField]
    private Transform fanMesh;
    [SerializeField]
    private MeshRenderer blurRenderer;
    [SerializeField]
    private Texture2D[] fanBlurTextures; // An array of increasingly blurred fan textures.
    [SerializeField]
    private BatteryContainer batteryContainer;
    [SerializeField]
    private GameObject[] myTriggers;

    [SerializeField]
    private float maxRPM = 100f;
    [SerializeField]
    private bool isStop = false;
    [SerializeField]
    private bool isFanOn = false;

    [SerializeField] [Range(0f, 1f)] private float fanBlurStart = 0.25f;   // The point at which the blurred textures start.
    [SerializeField] [Range(0f, 1f)] private float fanBlurEnd = 0.7f;      // The point at which the blurred textures stop changing.

    #endregion


    #region Getters
    public float MaxRPM
    {
        get => maxRPM;
    }

    public bool IsFanRunning
    {
        get => !isStop;
    }

    #endregion

    // Update is called once per frame
    void Update()
    {

        if (!isFanOn)
        {
            return;
        }

        if (isStop)
        {
            fanThrottle -= Time.deltaTime * 0.5f;
        }
        else
        {
            fanThrottle += Time.deltaTime;
        }
  
        fanThrottle = Mathf.Clamp(fanThrottle, 0f, fanTopSpeed);

        //rotate the fanMesh at a rate proportional to the fanThrottle.
        fanMesh.Rotate(0f, ((maxRPM * perSecondDegrees) * Time.deltaTime * fanThrottle) * -1f, 0f); //multiply by -1 is to rotate the fan in clockwise direction.

        int newBlurState = 0;

        //Start apply
        if(fanThrottle > fanBlurStart)
        {
            var blurProportion = Mathf.InverseLerp(fanBlurStart, fanBlurEnd, fanThrottle);
            newBlurState = Mathf.FloorToInt(blurProportion * (fanBlurTextures.Length - 1));
        }
        else
        {
            fanMesh.GetComponent<MeshRenderer>().enabled = true;
            blurRenderer.enabled = false;
        }

        if(newBlurState != fanCurrentBlurStateIndex)
        {
            fanCurrentBlurStateIndex = newBlurState;

            if(fanCurrentBlurStateIndex >= 0)
            {
                //fanMesh.GetComponent<MeshRenderer>().enabled = false;
                blurRenderer.enabled = true;
            }
            blurRenderer.material.mainTexture = fanBlurTextures[fanCurrentBlurStateIndex];
        }


    }

    //When player collides with the button, this method will trigger.
    public void SetTrigger(bool isTrigger)
    {
        if(!batteryContainer.AllBatteriesFixed())
        {
            return;
        }

        for (int i = 0; i < myTriggers.Length; i++)
        {
            if (myTriggers[i].GetComponent<PaperAirplane>())
                myTriggers[i].GetComponent<PaperAirplane>().SetTrigger(isTrigger);
        }


        if (isTrigger)
        {
            StopCoroutine("WaitBeforeExitting");
            isFanOn = true;
            isStop = false;
        }
        else
        {
            isStop = true;
            StartCoroutine("WaitBeforeExitting");
        }

    }

    private IEnumerator WaitBeforeExitting()
    {
        yield return new WaitUntil( ()=> fanThrottle <= 0f );
        fanThrottle = 0f;
        isFanOn = false;
    }
}

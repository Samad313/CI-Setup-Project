using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureButton : MonoBehaviour
{
    [SerializeField]
    private bool isTriggerButton = true;
    [SerializeField]
    private bool isCutScene = false;
    [SerializeField]
    private int triggerObjectIndexForCutScene = 0;

    private bool triggered = false;

    [SerializeField]
    private GameObject[] myTriggers = default;

    private bool notPressedOnce = true;

    [SerializeField]
    private GameObject[] pressedObjectsOff = default;

    [SerializeField]
    private Renderer tintChangeRenderers = default;

    [SerializeField]
    private Color offColor = default;

    [SerializeField]
    private Color onColor = default;

    [SerializeField]
    private Light[] myLights = default;

    private bool pressedByOrb = false;

    private float lerpT = 0.0f;

    [SerializeField]
    private Color pressedOnColor = default;

    [SerializeField]
    private Transform buttonToScaleDownOnPress = default;

    [SerializeField]
    private int buttonIndex = -1;


    private float delayTimeBeforeChangingState = -1.0f;

    // Start is called before the first frame update
    void Start()
    {
        //pressedOnColor = new Color32(191, 138, 24, 255);
        for (int i = 0; i < pressedObjectsOff.Length; i++)
        {
            pressedObjectsOff[i].SetActive(false);
        }
        tintChangeRenderers.material.SetColor("_Second_tint", offColor);

        for (int i = 0; i < myLights.Length; i++)
        {
            myLights[i].color = offColor;
        }
        SetVisuals();
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 wantedScale = Vector3.one;
        //if (triggered)
        //{
        //    wantedScale = new Vector3(1, 0.4f, 1);
        //}
        delayTimeBeforeChangingState -= Time.deltaTime;
        //transform.Find("Visual").localScale = Vector3.Lerp(transform.Find("Visual").localScale, wantedScale, Time.deltaTime * 2.0f);
    }

    void OnTriggerStay(Collider other)
    {
        if (pressedByOrb)
            return;

        if (other.gameObject.layer == 10 || other.gameObject.layer == 17)
        {
            delayTimeBeforeChangingState = 0.05f;
            if (triggered==false)
            {
                Debug.Log("STAY");
                for (int i = 0; i < pressedObjectsOff.Length; i++)
                {
                    pressedObjectsOff[i].SetActive(true);
                    StopCoroutine("SetVisualsCoroutine");
                    StartCoroutine("SetVisualsCoroutine", 1.0f);
                }

                AudioManager.instance.PlaySoundEffect("PressureButtonPress");
                //tintChangeRenderers.material.SetColor("_Second_tint", onColor);
                //for (int i = 0; i < myLights.Length; i++)
                //{
                //    myLights[i].color = onColor;
                //}
                triggered = true;
                for (int i = 0; i < myTriggers.Length; i++)
                {
                    if (myTriggers[i])
                    {
                        if(myTriggers[i].GetComponent<Gate>())
                            myTriggers[i].GetComponent<Gate>().SetTrigger(triggered);
                        else if (myTriggers[i].GetComponent<Water>())
                        {
                            myTriggers[i].GetComponent<Water>().SetTrigger(triggered);
                            myTriggers[i].GetComponent<Water>().SetTrigger(triggered, buttonIndex);
                        }
                        else if (myTriggers[i].GetComponent<MoveObjectToTarget>())
                            myTriggers[i].GetComponent<MoveObjectToTarget>().SetTrigger(triggered, transform);
                        else if (myTriggers[i].GetComponent<Fan>())
                            myTriggers[i].GetComponent<Fan>().SetTrigger(triggered);
                        else if (myTriggers[i].GetComponent<PianoController>())
                            myTriggers[i].GetComponent<PianoController>().SetTrigger(triggered, transform);
                    }
                        
                }

                if(notPressedOnce && isCutScene)
                {
                    notPressedOnce = false;
                    Cutscene.instance.StartCutscene(myTriggers[triggerObjectIndexForCutScene].transform, 2.0f);
                }

                if(other.gameObject.layer == 17)
                {
                    if(other.gameObject.GetComponent<PushableBox>().IsOrb())
                    {
                        other.gameObject.GetComponent<PushableBox>().FixToButton(transform);
                        pressedByOrb = true;
                    }
                    
                }
            }
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (pressedByOrb)
            return;

        if (other.gameObject.layer == 10 || other.gameObject.layer == 17)
        {
            if(isTriggerButton)
            {
                StopCoroutine("WaitBeforeExitting");
                StartCoroutine("WaitBeforeExitting"); 
            }
        }
    }

    private IEnumerator WaitBeforeExitting()
    {
        yield return new WaitForSeconds(0.1f);

        if (delayTimeBeforeChangingState > 0.0f)
            yield break;

        Debug.Log("EXIT");
        for (int i = 0; i < pressedObjectsOff.Length; i++)
        {
            //pressedObjectsOff[i].SetActive(false);
            StopCoroutine("SetVisualsCoroutine");
            StartCoroutine("SetVisualsCoroutine", 0.0f);
        }

        AudioManager.instance.PlaySoundEffect("PressureButtonRelease");
        //tintChangeRenderers.material.SetColor("_Second_tint", offColor);
        //for (int i = 0; i < myLights.Length; i++)
        //{
        //    myLights[i].color = offColor;
        //}
        triggered = false;
        for (int i = 0; i < myTriggers.Length; i++)
        {
            if (myTriggers[i])
            {
                if (myTriggers[i].GetComponent<Gate>())
                    myTriggers[i].GetComponent<Gate>().SetTrigger(triggered);
                else if (myTriggers[i].GetComponent<Water>())
                {
                    myTriggers[i].GetComponent<Water>().SetTrigger(triggered);
                    myTriggers[i].GetComponent<Water>().SetTrigger(triggered, buttonIndex);
                }
                else if (myTriggers[i].GetComponent<MoveObjectToTarget>())
                    myTriggers[i].GetComponent<MoveObjectToTarget>().SetTrigger(triggered, transform);
                else if (myTriggers[i].GetComponent<Fan>())
                    myTriggers[i].GetComponent<Fan>().SetTrigger(triggered);
                else if (myTriggers[i].GetComponent<PianoController>())
                    myTriggers[i].GetComponent<PianoController>().SetTrigger(triggered, transform);
            }
        }
    }

    private IEnumerator SetVisualsCoroutine(float targetLerpT)
    {
        StopCoroutine("ScaleButtonOnPress");
        StartCoroutine("ScaleButtonOnPress", targetLerpT);
        while(Mathf.Abs(targetLerpT-lerpT)>0.001f)
        {
            SetVisuals();
            lerpT = Mathf.MoveTowards(lerpT, targetLerpT, Time.deltaTime * 1.0f);
            yield return new WaitForEndOfFrame();
        }

        lerpT = targetLerpT;
        SetVisuals();
        if (targetLerpT < 0.1f)
        {
            for (int i = 0; i < pressedObjectsOff.Length; i++)
            {
                if (pressedObjectsOff[i].GetComponent<ParticleSystem>()==null)
                    pressedObjectsOff[i].SetActive(false);
            }
        }
    }

    private IEnumerator ScaleButtonOnPress(float targetLerpT)
    {
        float buttonT = lerpT;
        while (Mathf.Abs(targetLerpT - buttonT) > 0.001f)
        {
            buttonToScaleDownOnPress.localScale = Vector3.Lerp(Vector3.one, new Vector3(1, 1, 0.4f), buttonT);
            buttonT = Mathf.MoveTowards(buttonT, targetLerpT, Time.deltaTime * 3.0f);
            yield return new WaitForEndOfFrame();
        }

        buttonT = targetLerpT;
        buttonToScaleDownOnPress.localScale = Vector3.Lerp(Vector3.one, new Vector3(1, 1, 0.4f), buttonT);
    }

    private void SetVisuals()
    {
        for (int i = 0; i < pressedObjectsOff.Length; i++)
        {
            if(pressedObjectsOff[i].GetComponent<ParticleSystem>())
            {
                var particleEmission = pressedObjectsOff[i].GetComponent<ParticleSystem>().emission;
                particleEmission.rateOverTime = Mathf.Lerp(0, 3.0f, lerpT);
            }
            else
            {
                pressedObjectsOff[i].GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", Color.Lerp(Color.black, pressedOnColor, lerpT));
            }
        }

        tintChangeRenderers.material.SetColor("_Second_tint", Color.Lerp(offColor, onColor, lerpT));

        for (int i = 0; i < myLights.Length; i++)
        {
            myLights[i].color = Color.Lerp(offColor, onColor, lerpT);
        }
    }
}

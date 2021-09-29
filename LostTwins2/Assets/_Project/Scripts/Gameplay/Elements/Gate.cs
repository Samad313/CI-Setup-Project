using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnOffState
{
    bool OnState { get; }
}

public class Gate : MonoBehaviour, IOnOffState
{
    public bool OnState { get { return isOpening; } }

    private bool isOpening = false;

    [SerializeField]
    private float openingSpeed = 4.0f;

    [SerializeField]
    private float closingSpeed = 4.0f;

    private Vector3 startingScale;

    [SerializeField]
    private Animator myAnimator = default;

    [SerializeField]
    private Transform colliderGroup = default;

    [SerializeField]
    private GameObject[] objectsToTurnOff = default;

    [SerializeField]
    private Material materialToFade = default;

    [SerializeField]
    private Renderer centerForceField = default;

    [SerializeField]
    private Color centerForceFieldColor = default;

    [SerializeField]
    private Color startingTint = default;

    [SerializeField]
    private bool isShowCutScene = false;

    private bool stillLerpingColor = false;

    private float normalizedTime = 0.0f;
    private bool isCutSceneDependent = false;

    private void OnEnable()
    {
        if(isShowCutScene)
            Cutscene.instance.cutSceneCalled += EnableCutScene;
    }

    private void OnDisable()
    {
        if(isShowCutScene)
            Cutscene.instance.cutSceneCalled -= EnableCutScene;

    }

    // Start is called before the first frame update
    void Start()
    {
        startingScale = colliderGroup.localScale;
        for (int i = 0; i < objectsToTurnOff.Length; i++)
        {
            objectsToTurnOff[i].SetActive(true);
        }

        materialToFade.SetColor("_Second_tint", startingTint);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 wantedScale = startingScale;
        if(isOpening)
        {
            wantedScale = new Vector3(startingScale.x, startingScale.y * 0.0f, startingScale.z);
            colliderGroup.localScale = Vector3.Lerp(colliderGroup.localScale, wantedScale, Time.deltaTime * openingSpeed);
            if (colliderGroup.localScale.y < 0.1f)
            {
                colliderGroup.gameObject.SetActive(false);
            }

            if (stillLerpingColor)
            {
                Color currentColor = materialToFade.GetColor("_Second_tint");
                currentColor = Color.Lerp(currentColor, Color.black, Time.deltaTime);
                materialToFade.SetColor("_Second_tint", currentColor);

                currentColor = centerForceField.material.GetColor("_Tint");
                currentColor = Color.Lerp(currentColor, Color.black, Time.deltaTime);
                centerForceField.material.SetColor("_Tint", currentColor);

                if (currentColor == Color.black)
                {
                    stillLerpingColor = false;
                }
            }

            myAnimator.SetFloat("Direction", normalizedTime);
            normalizedTime += Time.deltaTime * 0.65f;
            if (normalizedTime > 1.0f)
                normalizedTime = 1.0f;
        }
        else
        {
            colliderGroup.gameObject.SetActive(true);
            colliderGroup.localScale = Vector3.Lerp(colliderGroup.localScale, wantedScale, Time.deltaTime * closingSpeed);

            if (stillLerpingColor)
            {
                Color currentColor = materialToFade.GetColor("_Second_tint");
                currentColor = Color.Lerp(currentColor, startingTint, Time.deltaTime);
                materialToFade.SetColor("_Second_tint", currentColor);

                currentColor = centerForceField.material.GetColor("_Tint");
                currentColor = Color.Lerp(currentColor, centerForceFieldColor, Time.deltaTime);
                centerForceField.material.SetColor("_Tint", currentColor);

                if (currentColor == startingTint)
                {
                    stillLerpingColor = false;
                }
            }

            myAnimator.SetFloat("Direction", normalizedTime);
            normalizedTime -= Time.deltaTime;
            if (normalizedTime < 0.0f)
                normalizedTime = 0.0f;
        }
    }

    public void SetTrigger(bool value)
    {
        StartCoroutine("SetDelayedTrigger", value);
        
    }

    private void EnableCutScene()
    {
        isCutSceneDependent = true;
    }
   

    private IEnumerator SetDelayedTrigger(bool value)
    {
        if(isCutSceneDependent)
        {
            yield return new WaitUntil( ()=> !Cutscene.instance.WaitForCutScene );
            isCutSceneDependent = false;
        }
        else
            yield return new WaitForSeconds(0.4f);
        if (value != isOpening)
        {
            if (value == true)
            {
                //myAnimator.SetTrigger("Drop");
                //myAnimator.SetFloat("Direction", 1.0f);
                //myAnimator.Play("Drop", 0, 0.05f);
                for (int i = 0; i < objectsToTurnOff.Length; i++)
                {
                    objectsToTurnOff[i].SetActive(false);
                }
            }
            else
            {
                //myAnimator.SetTrigger("Rise");
                //myAnimator.SetFloat("Direction", -1.0f);
                //myAnimator.Play("Drop", 0, 0.95f);
                //myAnimator.Play("Rise", 0, 0.05f);

                for (int i = 0; i < objectsToTurnOff.Length; i++)
                {
                    objectsToTurnOff[i].SetActive(true);
                }
            }
        }
        isOpening = value;
        stillLerpingColor = true;
        GetComponent<ZoomInOutIcon>().SetIconAgain();
    }
}

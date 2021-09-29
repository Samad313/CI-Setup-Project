using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisp : MonoBehaviour
{

    [SerializeField]
    private float rotationSpeed = 5.0f;

    [SerializeField] private float timeOffset = 0;

    private float offsetX = 0.0f;
    private float offsetY = 0.0f;

    private Vector3 centerPosition;
    private Vector3 finalPosition;
    private float speed;

    private bool movementStarted = false;

    private bool infinityMovement = true;

    [SerializeField] private Renderer frostTrail;
    [SerializeField] private Renderer mainTrail;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(movementStarted == false)
        {
            return;
        }
        //if(infinityMovement)
        //{
            offsetX = Mathf.Sin((Time.time + timeOffset) * rotationSpeed);
            offsetY = Mathf.Sin((Time.time + timeOffset) * rotationSpeed * 2.0f) * 0.5f;
        //}
        //else
        //{
        //    offsetX = Mathf.Lerp(offsetX, 0, Time.deltaTime * 2.0f);
        //    offsetY = Mathf.Lerp(offsetY, 0, Time.deltaTime * 2.0f);
        //}
        
        centerPosition = Vector3.MoveTowards(centerPosition, finalPosition, Time.deltaTime * speed);
        transform.position = centerPosition + new Vector3(offsetX, offsetY, -1);
    }

    public void MoveTransform(Vector3 inputVector, float speed, bool isOffset)
    {
        this.speed = speed;

        if (isOffset)
            finalPosition = centerPosition + inputVector;
        else
            finalPosition =  inputVector;
    }

    public void SetPosition(Vector3 inputPosition)
    {
        transform.position = inputPosition;
        centerPosition = inputPosition;
    }

    public void DisableAfterSomeTime()
    {
        infinityMovement = false;
        StartCoroutine("DisableAfterSomeTimeCoroutine");
    }

    private IEnumerator DisableAfterSomeTimeCoroutine()
    {
        yield return new WaitForSeconds(0.4f);
        StartCoroutine("FadeRenderers");
        //yield return new WaitForSeconds(0.3f);
        //Destroy(gameObject);
    }

    private IEnumerator FadeRenderers()
    {
        float t = 0.0f;
        Color frostStartColor = frostTrail.material.GetColor("_TintColor");
        Color mainTrailColor = mainTrail.material.color;
        while (t<1.0f)
        {
            frostTrail.material.SetColor("_TintColor", Color.Lerp(frostStartColor, Color.black, Easing.Ease(Equation.QuadEaseOut, t, 0, 1, 1)));
            mainTrail.material.color = Color.Lerp(mainTrailColor, new Color(mainTrailColor.r, mainTrailColor.g, mainTrailColor.b, 0), Easing.Ease(Equation.QuadEaseOut, t, 0, 1, 1));
            t += Time.deltaTime * 4.0f;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);

    }

    public void StartMovement()
    {
        movementStarted = true;
    }
}

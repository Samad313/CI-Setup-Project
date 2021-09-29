using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudWisp : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 4.0f;

    [SerializeField]
    private Transform trail;
    private float offsetX = 0.0f;
    private float offsetY = 0.0f;

    private Vector3 centerPosition;
    private Vector3 finalPosition;
    private Vector3 targetPosition;

    private bool movementStarted = false;
    private bool isMove = false;
    private float timeToCheckStartingPosition = 0f;
    private float speed = 0f;
    float remainingDistance = 10f;
    

    public bool IsWispReachedToUICrystal
    {
        get
        {
            return remainingDistance <= 2f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (movementStarted == false)
        {
            return;
        }

        if(isMove)
        {
            timeToCheckStartingPosition += Time.deltaTime;

            if(timeToCheckStartingPosition < 2f)
            {
                Vector3 target = GameplayManager.instance.GetCrystalUIWorldPosition();
                finalPosition = target;
            }
        }

        offsetX = Mathf.Sin((Time.time * rotationSpeed) * 3.5f) * 0.2f;
        offsetY = Mathf.Sin((Time.time * rotationSpeed) * 3.5f) * 0.2f;

        centerPosition = Vector3.MoveTowards(centerPosition, finalPosition, Time.deltaTime * speed);
        transform.position = centerPosition + new Vector3(offsetX, offsetY, 0);

        remainingDistance = Mathf.Sqrt((finalPosition - centerPosition).sqrMagnitude);
        if (remainingDistance <= 2f)
        {
            Invoke("DisableWisp", 0.1f);
        }

    }

    public void MoveTransform(Vector3 inputVector, float speed, bool isOffset)
    {
        this.speed = speed;

        if (isOffset)
            finalPosition = centerPosition + inputVector;
        else
            finalPosition = inputVector;
    }

    public void SetPosition(Vector3 inputPosition, Vector3 targetPos)
    {
        trail.gameObject.SetActive(false);
        targetPosition = targetPos;
        transform.position = inputPosition;
        centerPosition = inputPosition;
    }

    public void DisableWisp()
    {
        Destroy(gameObject);
    }

    public void StartMovement()
    {
        isMove = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        yield return new WaitForSeconds(0.2f);
        trail.GetComponent<TrailRenderer>().emitting = true;
        trail.gameObject.SetActive(true);

        isMove = true;
        MoveTransform(targetPosition, 18f, false);
        timeToCheckStartingPosition = 0f;
        movementStarted = true;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);

        //float timeElapsed = 0f;
        //Vector3 currentPos;
        //float endTIme = 1f;

        //while (timeElapsed < endTIme)
        //{
        //    timeElapsed += Time.deltaTime;

        //    currentPos = Vector3.Lerp(centerPosition, finalPosition, timeElapsed / endTIme);
        //    offsetX = Mathf.Sin((timeElapsed * rotationSpeed) * 3.5f) * 0.1f;
        //    offsetY = Mathf.Sin((timeElapsed * rotationSpeed) * 3.5f) * 0.1f;
        //    transform.position = currentPos + new Vector3(offsetX, offsetY, 0);
        //    yield return new WaitForEndOfFrame();

        //}

        //DisableWisp();
        yield return new WaitForSeconds(5f);
        if (gameObject.activeInHierarchy)
            DisableWisp();
    }
}

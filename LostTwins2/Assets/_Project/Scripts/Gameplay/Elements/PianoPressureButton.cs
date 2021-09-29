using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoPressureButton : MonoBehaviour
{
    #region Exposed Variables
    [SerializeField]
    private Transform keyVisual;

    #endregion

    #region Private Variables
    private float pressedTimeDuration = 0.5f;
    private float unPressedTimeDuration = 0.5f;
    private Quaternion currentKeyRotation = default;

    private Vector3 keyPressedRotation = new Vector3(0f, 5.5f, 0f);

    private Vector3 startingDistanceX = default;
    private Vector3 endingDistanceX = default;

    private float keyLength = 5f; //3.74

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (!checkDistance)
    //        return;


    //    if (isKeyPressed)
    //    {
    //        float remainingDistance = Mathf.Sqrt((endingDistanceX - startingDistanceX).sqrMagnitude);

    //        if(remainingDistance >= 0.7f)
    //        {
                
    //            Debug.Log("Distance reaached");
    //            StopCoroutine("UnPressedStateOfKey");
    //            StartCoroutine("UnPressedStateOfKey");
    //            isKeyPressed = false;
    //            checkDistance = false;
    //        }
    //    }

    //}

    public float GetAngleInRadians(float angleInDegrees)
    {
        return angleInDegrees * Mathf.Deg2Rad;
    }

    public void FindHeight()
    {
        float angleOfDeviation = keyPressedRotation.y;
        float height = Mathf.Tan(GetAngleInRadians(angleOfDeviation)) * keyLength; //trignometric formula tan = P / B
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 10 ) //Layer 10 = player
        {
            startingDistanceX = collision.gameObject.transform.position;
            StopCoroutine("PressedStateOfKey");
            StartCoroutine("PressedStateOfKey");
            FindHeight();
        }
    }

    //public void OnCollisionStay(Collision collision)
    //{
    //    if (collision.gameObject.layer == 10) //Layer 10 = player
    //    {
    //        endingDistanceX = collision.gameObject.transform.position;
    //        //transform.rotation = currentKeyRotation;
    //    }
    //}

    //public void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.layer == 10) //Layer 10 = player
    //    {
    //        Debug.Log("Exiting");

    //        //isKeyPressed = false;
    //        StopCoroutine("UnPressedStateOfKey");
    //        StartCoroutine("UnPressedStateOfKey");
    //    }
    //}

    //public void OnTriggerEnter(Collider other)
    //{
    //    if(other.gameObject.layer == 10) //Layer 10 = player
    //    {
    //        StopCoroutine("PressedStateOfKey");
    //        StartCoroutine("PressedStateOfKey");
    //    }
    //}

    //public void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.layer == 10) //Layer 10 = player
    //    {
    //        transform.rotation = currentKeyRotation;
    //    }
    //}

  

    private IEnumerator PressedStateOfKey()
    {
        Debug.Log("Key pressed");
        float timeElapsed = 0f;
        Quaternion startRotation = keyVisual.localRotation;

        while (timeElapsed < pressedTimeDuration)
        {
            currentKeyRotation = Quaternion.Lerp(startRotation, Quaternion.Euler(keyPressedRotation), timeElapsed / pressedTimeDuration);
            keyVisual.localRotation = currentKeyRotation;
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator UnPressedStateOfKey()
    {
        Debug.Log("Key unpressed");

        float timeElapsed = 0f;
        Quaternion startRotation = keyVisual.localRotation;
        currentKeyRotation = startRotation;

        while (timeElapsed < unPressedTimeDuration)
        {
            currentKeyRotation = Quaternion.Lerp(startRotation, Quaternion.identity, timeElapsed / pressedTimeDuration);
            keyVisual.localRotation = currentKeyRotation;
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }

}

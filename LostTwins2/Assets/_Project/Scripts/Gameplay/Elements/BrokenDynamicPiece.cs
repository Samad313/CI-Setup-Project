using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenDynamicPiece : MonoBehaviour
{
    private Vector3 savedVelocity;

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //void FixedUpdate()
    //{
    //    if (GameplayManager.instance.GetCameraZoomedStatus() != ZoomStatus.ZoomedIn)
    //        return;

    //    waitTime -= Time.deltaTime;
    //    if(waitTime<0)
    //    {
    //        waitTime = 1.0f;
    //        GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere*50000.0f);
    //        Debug.Log("ffffffffff");
    //    }
    //}

    public void SetActive(bool value)
    {
        if (value == true)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().velocity = savedVelocity;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;
            savedVelocity = GetComponent<Rigidbody>().velocity;
            GetComponent<Rigidbody>().velocity = Vector3.zero;

        }
    }
}

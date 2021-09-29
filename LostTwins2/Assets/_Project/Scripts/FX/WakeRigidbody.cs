using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeRigidbody : MonoBehaviour
{
    private Rigidbody myRigidbody;
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (myRigidbody.IsSleeping())
        //{
            myRigidbody.WakeUp();
        //}
    }
}

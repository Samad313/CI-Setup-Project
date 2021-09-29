using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour
{

    private Vector3 finalPosition;

    private bool shouldLerp = false;

    private float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldLerp)
        {
            transform.position = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * speed);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, finalPosition, Time.deltaTime * speed);
        }
    }

    public void SetValues(Vector3 inputFinalPosition, float inputSpeed, bool inputShouldLerp)
    {
        finalPosition = inputFinalPosition;
        shouldLerp = inputShouldLerp;
        speed = inputSpeed;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectOnSpline : MonoBehaviour
{
    private Transform myTransform;
    private Vector3[] splinePoints;
    float speed;

    public void Init(Transform myTransform, Vector3[] splinePoints, float speed)
    {
        this.myTransform = myTransform;
        this.speed = speed;

        this.splinePoints = new Vector3[splinePoints.Length];
        for (int i = 0; i < splinePoints.Length; i++)
        {
            this.splinePoints[i] = splinePoints[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

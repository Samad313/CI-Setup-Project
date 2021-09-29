using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovementHelper 
{

    public static void MoveOnSpline(this Transform myTransform, Vector3[] splinePoints, float speed)
    {
        GameObject go = new GameObject("MoverOnSpline");
        go.AddComponent<MoveObjectOnSpline>();
        go.GetComponent<MoveObjectOnSpline>().Init(myTransform, splinePoints, speed);
    }
}

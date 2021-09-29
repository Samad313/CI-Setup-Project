using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationFollow : MonoBehaviour
{
    private float rot1;

    [SerializeField]
    private Transform transformToFollow = default;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float rot2 = Vector3.SignedAngle(new Vector3(1, 0, 0), transformToFollow.position - transform.position, new Vector3(0, 0, 1));

        rot1 = GetModuloRotation(transform.localEulerAngles.z, rot2);
        Debug.Log(transform.localEulerAngles.z + " : " + rot1 + " : " + rot2);
        if (rot1 < rot2)
            rot1 += 2.0f + Time.deltaTime;
        else
            rot1 -= 2.0f + Time.deltaTime;

        transform.localEulerAngles = new Vector3(0, 0, rot1);
    }

    private float GetModuloRotation(float inputAngle1, float inputAngle2)
    {
        inputAngle1 = inputAngle1 % 360.0f;
        if (Mathf.Abs(inputAngle1 - inputAngle2) >= 180.0f)
        {
            inputAngle1 = inputAngle1 - 360.0f;
        }

        return inputAngle1;
    }
}

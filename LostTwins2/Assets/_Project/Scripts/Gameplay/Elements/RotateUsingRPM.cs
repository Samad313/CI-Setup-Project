using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUsingRPM : MonoBehaviour
{
    #region Private Variables
    private float fanThrottle;
    private const float perSecondDegrees = 6f; //1 rpm = 6 degrees/sec
    private float fanTopSpeed = 1f;

    #endregion

    #region Exposed Variables
   
    [SerializeField]
    private float maxRPM = 100f;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fanThrottle += Time.deltaTime;
        fanThrottle = Mathf.Clamp(fanThrottle, 0f, fanTopSpeed);

        //rotate the fanMesh at a rate proportional to the fanThrottle.
        transform.Rotate(0f, ((maxRPM * perSecondDegrees) * Time.deltaTime * fanThrottle), 0f);
    }
}

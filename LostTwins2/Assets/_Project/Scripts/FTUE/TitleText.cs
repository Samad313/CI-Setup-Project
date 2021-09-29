using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleText : MonoBehaviour
{
    public static bool shouldSetPosition;
    public static Vector3 offset;

    // Start is called before the first frame update
    public void Init(Transform cameraTransform)
    {
        if(GameplayManager.instance.ZoomedInStart)
        {
            if (shouldSetPosition)
            {
                transform.position = cameraTransform.position - offset;

                Debug.Log("CamPos :" + cameraTransform.position.ToString("G9") + ", TTPos :" + transform.position.ToString("G9") + ", offset : " + offset.ToString("G9"));
            }
            else if(GameplayManager.instance.FirstLevel)
            {
                transform.position = cameraTransform.position - new Vector3(-0.275f, 0, -6.55f);
            }
        }
        
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOffset(Vector3 cameraPosition)
    {
        offset = cameraPosition - transform.position;
        Debug.Log("CamPos :" + cameraPosition.ToString("G9") + ", TTPos :" + transform.position.ToString("G9") + ", offset : " + offset.ToString("G9"));
        shouldSetPosition = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlatformCollider : MonoBehaviour
{
    private oldPressurePlatform myParent;

    // Start is called before the first frame update
    void Start()
    {
        myParent = transform.parent.parent.GetComponent<oldPressurePlatform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==10 || other.gameObject.layer == 17)
        {
            myParent.AddWeightObject(other.transform, other.gameObject.layer == 17);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10 || other.gameObject.layer == 17)
        {
            myParent.RemoveWeightObject(other.transform);
        }
    }
}

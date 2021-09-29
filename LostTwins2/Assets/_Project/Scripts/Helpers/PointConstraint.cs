using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointConstraint : MonoBehaviour
{
    [SerializeField]
    private Transform myParent;

    private Vector3 offset;
    
    // Start is called before the first frame update
    void Awake()
    {
        offset = transform.position - myParent.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = myParent.position + offset;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField]
    private Transform myTransformToFollow;

    [SerializeField]
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (myTransformToFollow)
    //    {
    //        transform.position = myTransformToFollow.position + offset;
    //    }
    //}

    void LateUpdate()
    {
        if (myTransformToFollow)
        {
            transform.position = myTransformToFollow.position + offset;
        }
    }

    public void SetTransform(Transform inputTransform)
    {
        myTransformToFollow = inputTransform;
    }

    public void SetOffset(Vector3 inputOffset)
    {
        offset = inputOffset;
    }
}

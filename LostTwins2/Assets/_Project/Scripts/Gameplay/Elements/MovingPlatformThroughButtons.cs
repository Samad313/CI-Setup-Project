using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformThroughButtons : MoveObjectToTarget
{
    [SerializeField]
    private Transform top;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer==10)
        {
            collision.gameObject.GetComponent<PlayerController>().TouchedByPlatform(top);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
            collision.gameObject.GetComponent<PlayerController>().TouchedByPlatform(top);
        }
    }
}

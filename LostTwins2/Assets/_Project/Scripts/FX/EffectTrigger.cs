using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform effectParticles = default;

    [SerializeField]
    private Vector3 offset = default;

    [SerializeField]
    private float delay = 30.0f;

    [SerializeField]
    private float destroyTime = 2.0f;


    private float delayTime = -10.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
            return;

        delayTime -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==10)
        {
            if(delayTime<0)
            {
                Transform tempTransform = Instantiate(effectParticles, transform);
                tempTransform.localPosition = offset;
                delayTime = delay;

                if(destroyTime>0)
                {
                    Destroy(tempTransform.gameObject, destroyTime);
                }
            }
        }
    }
}

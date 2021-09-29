using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeTriggerCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            transform.parent.parent.parent.Find("Collider").GetComponent<Rope>().EnteredRopeColliders(other);
        }
    }
}

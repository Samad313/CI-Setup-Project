using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomTrigger : MonoBehaviour
{
    [SerializeField] private Mushroom myMushroom;

    private void OnCollisionEnter(Collision collision)
    {
        //myMushroom.CollisionOccured(collision);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerComingSoon : MonoBehaviour
{
    
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 10) //Player
        {
            FadeInOutMeshColor.instance.ChangeInstantColorToWhite();

        }
    }



}

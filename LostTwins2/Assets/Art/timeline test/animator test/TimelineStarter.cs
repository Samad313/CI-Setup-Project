using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineStarter : MonoBehaviour
{

    public GameObject Triggerme;

     void OnTriggerEnter(Collider other)
    {
        
         PlayableDirector pd = Triggerme.GetComponent<PlayableDirector>();
        if (pd != null)
        {
           pd.Play();
        }
    }
}
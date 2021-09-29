using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEventDetector : MonoBehaviour
{
    private EventSystem myEventSystem;
    private GameObject lastActiveGameobject;

	// Use this for initialization
	void Start ()
    {
        myEventSystem = GetComponent<EventSystem>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(myEventSystem.currentSelectedGameObject)
        {
//            Debug.Log("BUTTON CLICKED NAME : " + myEventSystem.currentSelectedGameObject.name);
            if(myEventSystem.currentSelectedGameObject.GetComponent<Button>())
            {
                PlayButtonSound();
                myEventSystem.SetSelectedGameObject(null);
            }
        }
	}

    private void PlayButtonSound()
    {
        if(AudioManager.instance)
            AudioManager.instance.PlaySoundEffect("ButtonClick");
    }
}

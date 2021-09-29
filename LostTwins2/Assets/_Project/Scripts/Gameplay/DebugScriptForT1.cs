using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScriptForT1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(MenuController.instance)
            MenuController.instance.UICamera.GetComponent<UICamera>().BlackBG.gameObject.SetActive(false);
    }

   
}

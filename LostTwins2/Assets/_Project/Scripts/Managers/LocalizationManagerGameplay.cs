using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalizationManagerGameplay : MonoBehaviour
{
    [SerializeField]
    public Transform canvas;
    [SerializeField]
    public bool enableLocalization = true;
    // Use this for initialization
	void Start () 
    {
        if(!Localization.GetInitCalled())
            Localization.Init();
        
        if(enableLocalization)
        {
            //Main Menu
            canvas.Find("HUD/CopChaseStarted").GetComponent<TextMeshProUGUI>().text = Localization.GetText("HUD_EvadeCopsText");
        }


    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorBox : MonoBehaviour
{
    [SerializeField]
    private int onState = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WasClicked(Material onMaterial, Material offMaterial, Material specialMaterial, bool shouldCycleSpecial)
    {
        onState = GetRealOnState();
        onState++;
        int maxStates = 2;
        if (shouldCycleSpecial)
            maxStates = 3;

        if (onState >= maxStates)
            onState = 0;

        if(onState==0)
        {
            GetComponent<Renderer>().sharedMaterial = offMaterial;
        }
        else if (onState == 1)
        {
            GetComponent<Renderer>().sharedMaterial = onMaterial;
        }
        else if (onState == 2)
        {
            GetComponent<Renderer>().sharedMaterial = specialMaterial;
        }
    }

    public int GetOnState()
    {
        return onState;
    }

    public bool IsOnMaterial()
    {
        return (GetComponent<Renderer>().sharedMaterial.name.Contains("On"));
    }

    public int GetRealOnState()
    {
        if ((GetComponent<Renderer>().sharedMaterial.name.Contains("Off")))
            return 0;
        else if ((GetComponent<Renderer>().sharedMaterial.name.Contains("On")))
            return 1;

        return 2;
    }
}

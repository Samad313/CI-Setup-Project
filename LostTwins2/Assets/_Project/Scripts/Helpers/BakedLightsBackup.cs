using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakedLightsBackup : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> lightsBackup = new List<GameObject>();

    public BakedLightsBackup()
    {
        lightsBackup.Clear();
    }

    public void SaveLight(GameObject light)
    {
        lightsBackup.Add(light);
    }

    public void EnableLights()
    {
        for (int i = 0; i < lightsBackup.Count; i++)
        {
            lightsBackup[i].SetActive(true);
        }
    }


}

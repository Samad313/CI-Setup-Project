using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy Instance = null;

    // Start is called before the first frame update
    void Awake()
    {
        ManageSingletonInstance();
    }

    private void ManageSingletonInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);

        }

        DontDestroyOnLoad(gameObject);
    }
}

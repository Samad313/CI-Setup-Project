using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootProjector : MonoBehaviour
{
    [SerializeField]
    private float initialDelay = 2.0f;

    [SerializeField]
    private float fadeSpeed = 2.0f;

    private Material myMaterial;

    private float t = 0;

    private int currentState = 0;

    // Start is called before the first frame update
    void Start()
    {
        myMaterial = new Material(GetComponent<Projector>().material);
        GetComponent<Projector>().material = myMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == 0)
        {
            myMaterial.SetFloat("_intensity", t);
            t += Time.deltaTime * fadeSpeed * 3.0f;
            if (t > 1)
            {
                currentState = 1;
                t = 0.0f;
            }
        }
        else if (currentState == 1)
        {
            t += Time.deltaTime;
            if(t>initialDelay)
            {
                currentState = 2;
                t = 1.0f;
            }
        }
        else if (currentState == 2)
        {
            myMaterial.SetFloat("_intensity", t);
            t -= Time.deltaTime * fadeSpeed;
            if(t<0)
            {
                Destroy(gameObject);
            }
        }
    }
}

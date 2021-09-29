using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowTrack : MonoBehaviour
{
    [SerializeField]
    private float initialDelay = 2.0f;

    [SerializeField]
    private float fadeSpeed = 0.2f;

    [SerializeField]
    private float growSpeed = 3.0f;

    private float t = 0;

    private int currentState = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == 0)
        {
            transform.localScale = Vector3.one * t;
            t += Time.deltaTime * growSpeed;
            if (t > 1)
            {
                transform.localScale = Vector3.one;
                currentState = 1;
                t = 0.0f;
            }
        }
        else if (currentState == 1)
        {
            t += Time.deltaTime;
            if (t > initialDelay)
            {
                currentState = 2;
                t = 1.0f;
            }
        }
        else if (currentState == 2)
        {
            transform.localScale = Vector3.one * t;
            t -= Time.deltaTime * fadeSpeed;
            if (t < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

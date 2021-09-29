using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] private Transform myVisual = default;

    [SerializeField] private Light myLight = default;

    private float t = 0;

    private float lightIntensity = 1.0f;
    private float lightRange = 1.0f;

    private float speed = 1.0f;

    private bool isCollected = false;

    private Vector3 startPosition = default;

    private Transform myHolder = default;

    private float speedFactor = 1.0f;

    private bool inHolder = false;

    private bool isTriggered = true;

    private int myIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
        lightIntensity = myLight.intensity;
        lightRange = myLight.range;
        t = Random.value * 2.0f;
        speed = 3.0f + Random.value;
        isTriggered = true;

        if(name.Contains("1"))
        {
            myIndex = 1;
        }
        else if (name.Contains("2"))
        {
            myIndex = 2;
        }
        else if (name.Contains("3"))
        {
            myIndex = 3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inHolder)
            return;
        
        if(isCollected)
        {
            transform.position = Vector3.Lerp(startPosition, myHolder.position, t);
            transform.localScale = Vector3.Lerp(Vector3.one, myHolder.localScale, t);
            t += Time.deltaTime * speedFactor;
            if(t>=1.0f)
            {
                transform.parent = myHolder;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
                inHolder = true;
            }
            return;
        }

        float normalizedSine = Mathf.Sin(t) / 2.0f + 0.5f;
        myVisual.localScale = Vector3.one * (1.0f + normalizedSine * 0.07f);
        myVisual.localEulerAngles = new Vector3(0, 0, Mathf.Sin(t+0.3f) * 2.0f);
        myVisual.localPosition = new Vector3(0, Mathf.Sin(t+1.0f) * 0.1f, 0);

        myLight.intensity = lightIntensity + normalizedSine * 1.0f;
        myLight.range = lightRange + normalizedSine * 0.2f;

        t += Time.deltaTime * 3.5f;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==10)
        {
            if(isTriggered)
            {
                isTriggered = false;
                FXManager.instance.CrystalCollected(transform.position);
                FXManager.instance.SpawnWisp(transform.position);
                AudioManager.instance.PlaySoundEffect("StarCollected");
                GameplayManager.instance.ElementDestroyed(transform);
                GameplayManager.instance.HandleStarCollect(myIndex);
                Destroy(gameObject);
                //GetComponent<BoxCollider>().enabled = false;
                //isCollected = true;
                //startPosition = transform.position;
                //myHolder = GameplayManager.instance.GetFinalGate().GetHolder();
                //speedFactor = 1.0f;// 5.0f / (transform.position - myHolder.position).magnitude;
                //t = 0;
                //Cutscene.instance.StartCutscene(transform, 1.0f / speedFactor + 2.0f);
            }
        }
    }
}

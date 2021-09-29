using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField]
    private Transform myCaster;

    [SerializeField]
    private float maxAlpha = 0.4f;

    [SerializeField]
    private float heightForZeroAlpha = 5.0f;

    private Material myMat;

    // Start is called before the first frame update
    void Start()
    {
        myMat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if(myCaster)
        {
            float alphaLerpValue = Mathf.InverseLerp(0.0f, heightForZeroAlpha, myCaster.position.y);
            float alphaValue = Mathf.Lerp(maxAlpha, 0.0f, alphaLerpValue);
            myMat.color = new Color(1, 1, 1, alphaValue);

            transform.position = new Vector3(myCaster.position.x, 0, 0);
        }
        else
        {
            transform.position = new Vector3(1000, 1000, 1000);
        }
    }

    public void SetCaster(Transform caster)
    {
        myCaster = caster;
    }
}

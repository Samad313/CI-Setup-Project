using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSpritesheet : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.0f;

    [SerializeField]
    private float startingT = 0;

    [SerializeField]
    private bool isLooping = false;

    private float lerpValue = 0;
    private Material myMaterial;

    private void Awake()
    {
        lerpValue = startingT;
        myMaterial = GetComponent<Renderer>().material;
        myMaterial.SetFloat("_Animate", lerpValue);
    }

    // Start is called before the first frame update
    void Start()
    {
        

    }

    public void AssignDarkenValue(float value)
    {
        myMaterial.SetFloat("_Darken", value);
    }


    // Update is called once per frame
    void Update()
    {
        myMaterial.SetFloat("_Animate", lerpValue);
        lerpValue += Time.deltaTime * speed;
        if(lerpValue>=1.0f)
        {
            if(isLooping)
            {
                lerpValue -= 1.0f;
            }
            else
            {
                if (transform.parent == null)
                    Destroy(gameObject);
                else
                    Destroy(transform.parent.gameObject);
            }
        }
    }
}

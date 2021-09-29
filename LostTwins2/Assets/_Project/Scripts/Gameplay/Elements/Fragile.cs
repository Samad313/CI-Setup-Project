using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragile : MonoBehaviour
{

    private bool destructionStarted = false;

    [SerializeField]
    private Transform shatteredDome = default;

    [SerializeField]
    private Renderer visualRenderer = default;

    [SerializeField]
    private Material brokenMaterial = default;

    [SerializeField]
    private CameraPoint myCameraPoint = default;

    // Start is called before the first frame update
    void Start()
    {
        brokenMaterial.SetFloat("_opacity", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer==17 && destructionStarted==false)
        {
            if(collision.gameObject.GetComponent<PushableBox>().GetCanShatter())
            {
                AudioManager.instance.PlaySoundEffect("BreakingDome");
                destructionStarted = true;
                float hitDirection = 1.0f;
                if (transform.position.x > collision.transform.position.x)
                    hitDirection = -1.0f;
                collision.gameObject.GetComponent<PushableBox>().FragileHit(hitDirection);
                visualRenderer.enabled = false;
                shatteredDome.gameObject.SetActive(true);
                shatteredDome.parent = null;
                shatteredDome.GetComponent<Animator>().SetTrigger("break");
                //GetComponent<MeshCollider>().enabled = false;
                StartCoroutine("DestroyAfterSomeTime");

                if(myCameraPoint)
                {
                    myCameraPoint.DisableInfluence();
                }
            }
        }
    }

    private IEnumerator DestroyAfterSomeTime()
    {
        yield return new WaitForSeconds(1.0f);

        GetComponent<MeshCollider>().enabled = false;

        float t = 1.0f;
        while (t > 0)
        {
            brokenMaterial.SetFloat("_opacity", t);
            t -= Time.deltaTime * 1.0f;
            yield return new WaitForEndOfFrame();
        }

        Destroy(shatteredDome.gameObject, 1.0f);
        GameplayManager.instance.ElementDestroyed(transform);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField]
    private Renderer shockwaveRenderer = default;

    [SerializeField]
    private float waveSpeed = 2.0f;

    private float currentWaveX = 0.5f;
    private Vector3 startPosition;
    private Vector3 endPosition = new Vector3(-70f, -1f, 7.78f);

    // Start is called before the first frame update
    void Start()
    {
        //currentWaveX = 0f;
        //startPosition = transform.position;
        //endPosition = 
        currentWaveX = 0.5f;
        //shockwaveRenderer.material.SetFloat("_offset", currentWaveY);
        //StartCoroutine("Move");
        //StartCoroutine("DestroyObject");
    }

    // Update is called once per frame
    void Update()
    {
        shockwaveRenderer.material.SetTextureOffset("_wave_gradient", new Vector2(currentWaveX, 0));
        currentWaveX = Mathf.MoveTowards(currentWaveX, 1.5f, Time.deltaTime * waveSpeed);
        if (currentWaveX >= 1.495f)
        {
            Destroy(gameObject);
        }

        //shockwaveRenderer.material.SetFloat("_OffsetX", currentWaveX);

        //shockwaveRenderer.material.SetFloat("_OffsetZ", currentWaveX);
        //shockwaveRenderer.material.SetTextureOffset("_wave_gradient", new Vector2(currentWaveX, 0));


        //currentWaveY = Mathf.Clamp(currentWaveY, 0f, Mathf.Sin((Time.time * 0.001f)));

        //currentWaveX = Mathf.MoveTowards(currentWaveX, 1.5f, Time.deltaTime * waveSpeed);
        //if (currentWaveX <= -19f)
        //{
        //    //Destroy(gameObject);

        //}
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private IEnumerator Move()
    {
        float timeElapsed = 0f;
        float endTime = 3.5f;
        Vector3 currentPosition;
        float offsetY = 0f;

        while (timeElapsed < endTime)
        {
            timeElapsed += Time.deltaTime;
            currentPosition = Vector3.Lerp(startPosition, endPosition, timeElapsed / endTime);
            offsetY = Mathf.Sin((timeElapsed * 0.5f) * 0.01f);
            transform.position = currentPosition;
            //transform.position = currentPosition + new Vector3(currentPosition.x, offsetY, currentPosition.z);

            yield return new WaitForEndOfFrame();
        }
    }
}

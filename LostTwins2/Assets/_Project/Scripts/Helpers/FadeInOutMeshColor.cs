using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOutMeshColor : MonoBehaviour
{
    public static FadeInOutMeshColor instance;

    [SerializeField]
    private MeshRenderer[] renderers;

    [SerializeField]
    private string materialShadingProperty = "";

    
    private string defaultShadingProperty = "_Color";


    private void Awake()
    {
        if (!instance)
            instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeInstantColorToWhite()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            StartCoroutine(AnimateAlpha(renderers[i].material));
        }
    }

    private IEnumerator AnimateAlpha(Material mat)
    {
        float currentValue = 0f;
        float shadingValue = 0f;
        float timeElapsed = 0f;
        float endTime = 1f;

        Color materialColor = mat.color;

        while (timeElapsed < endTime)
        {
            currentValue = Mathf.Lerp(shadingValue, 1f, timeElapsed / endTime);
            materialColor.a = currentValue;
            mat.SetColor(defaultShadingProperty, materialColor);    
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.5f);
        timeElapsed = 0f;
        shadingValue = 1f;
        while (timeElapsed < endTime)
        {
            currentValue = Mathf.Lerp(shadingValue, 0f, timeElapsed / endTime);
            materialColor.a = currentValue;
            mat.SetColor(defaultShadingProperty, materialColor);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }


}

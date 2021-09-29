using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCurveVisualizer : MonoBehaviour
{

    [SerializeField]
    private Equation animCurveType = default;

    [SerializeField]
    private float increment = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject lineRendererGO = new GameObject("LineRendererGO");
        LineRenderer lineRenderer = lineRendererGO.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.02f;

        float t = 0.0f;
        Vector3[] pointsArray = new Vector3[(int)((1.0f / increment) + 1.0f)];
        float tY = 0.0f;
        for (int i = 0; i < pointsArray.Length; i++)
        {
            t = i * increment;

            tY = Easing.Ease(animCurveType, t, 0.0f, 1.0f, 1.0f);

            pointsArray[i] = new Vector3(t, tY, 0);
        }

        lineRenderer.positionCount = pointsArray.Length;
        lineRenderer.SetPositions(pointsArray);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

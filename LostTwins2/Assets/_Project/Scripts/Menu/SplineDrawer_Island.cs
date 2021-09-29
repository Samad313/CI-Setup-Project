using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineHelperFunctions;

public class SplineDrawer_Island : MonoBehaviour
{
    [SerializeField] private int totalLevels = 0;
    [SerializeField] private int baseNumPoints = 50;
    [SerializeField] private LineRenderer lineRenderer;

    private Vector3[] splinePoints;
    private Vector3[] parametrizedPoints;
    private Vector3[] levelPoints;
    private float horizontalTiling = 0.0f;
    private int updatedNumPoints = 0;
    private float pathLength = 0.0f;

    private bool pathCreated = false;
    private Vector3 point1, point2;
    //private Vector3[] pathPoints;

    private float cp_IncrementVal;
    private int lastIndexOfParametPoint, tillIndex;
    private int levelsCompleted = 0;

    //private int currentPosition_LineRenderer = 1;

    private void Start()
    {
        splinePoints = GetSplinePoints();//SplineHelper.GetSplinePoints(transform);
        updatedNumPoints = Mathf.CeilToInt(baseNumPoints * SplineHelper.GetPathLength(splinePoints));
        parametrizedPoints = SplineHelper.ParameterizeCPs(splinePoints, updatedNumPoints);
        float TotalPathLength = SplineHelper.GetPathLength(splinePoints);
        float numFloatPoints = updatedNumPoints;
        cp_IncrementVal = TotalPathLength / numFloatPoints;
        //lineRenderer.positionCount = parametrizedPoints.Length;
        GetCPs();
        //SetSpline();
        //horizontalTiling = CalculateTextureTiling();
        //UpdatePath();

        //Debug.Log("Tiling Value: " + horizontalTiling);
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Space) && !pathCreated)
        {
            levelsCompleted++;
            GetPointsForPath();
            //horizontalTiling = CalculateTextureTiling();
            Debug.Log("Horizontal Tiling Value: " + horizontalTiling);
            UpdatePath();
            pathCreated = true;
        }
    }

    private float CalculateTextureTiling()
    {
        float distanceBetweenPoints = Vector3.Distance(point1, point2);
        float points = baseNumPoints * distanceBetweenPoints;

        //float numFloatPoints = points;
        Debug.Log("Distance between points: " + points);

        cp_IncrementVal = distanceBetweenPoints / points;
        return cp_IncrementVal;

    }

    private void GetPointsForPath()
    {
        Debug.Log("Levels completed: " + levelsCompleted);
        point1 = levelPoints[levelsCompleted - 1];
        point2 = levelPoints[levelsCompleted];

        for (int i = 0; i < parametrizedPoints.Length; i++)
        {
            if(point2 == parametrizedPoints[i])
            {
                tillIndex = i;
            }
        }

        Debug.LogFormat("Point1: {0} , p2: {1}", point1, point2);
    }


    //private void SetSpline()
    //{
    //    Transform[] levelChildren = this.transform.GetComponentsInChildren<Transform>();
    //    splinePoints = GetSplinePoints();
    //    pathLength = SplineHelper.GetPathLength(splinePoints);
    //    Debug.Log(pathLength);
    //    updatedNumPoints = Mathf.CeilToInt(baseNumPoints * (int)pathLength);
    //    parametrizedPoints = SplineHelper.ParameterizeCPs(splinePoints, updatedNumPoints);
    //}


    //private float CalculateTextureTiling()
    //{
    //    if(pathLength > 2)
    //    {
    //        Debug.LogFormat("<color=green> PathLength: {0} , Parametrized Length: {1}", pathLength, parametrizedPoints.Length);
    //        float value = (parametrizedPoints.Length * (pathLength)) / splinePoints.Length;
    //        Debug.Log("Path value");
    //        return value;
    //    }

    //    return 3;

    //}

    private void UpdatePath()
    {
        //Debug.Log("<color=green> Parametrized Length: </color>" + parametrizedPoints.Length);
        StartCoroutine(_PlacePathPoints());

        //lineRenderer.SetPositions(parametrizedPoints);

    }


    private IEnumerator _PlacePathPoints()
    {
        int toBegin = 1;

        if (lastIndexOfParametPoint > 1)
        {
            toBegin = lastIndexOfParametPoint;
        }

        Vector3 previousPoint = parametrizedPoints[toBegin];


        for (int i = toBegin; i <= tillIndex; i++)
        {

            //float distance = Vector3.Distance(previousPoint, parametrizedPoints[i]);

            //if (distance == 0 || distance >= horizontalTiling)
            //{
            lineRenderer.positionCount = i;
            lineRenderer.SetPosition(i - 1, parametrizedPoints[i]);
            lineRenderer.material.mainTextureScale = new Vector2(i, lineRenderer.material.mainTextureScale.y);
            //currentPosition_LineRenderer++;

            //}

            //previousPoint = parametrizedPoints[i];

            yield return new WaitForSeconds(0.5f);
        }
        lastIndexOfParametPoint = tillIndex;
        pathCreated = false;
        yield break;
    }

    //private IEnumerator _PlacePathPoints()
    //{


    //    for (int i = 0; i < parametrizedPoints.Length; i++)
    //    {
    //        lineRenderer.positionCount = i+1;
    //        lineRenderer.material.mainTextureScale = new Vector2(i+1, lineRenderer.material.mainTextureScale.y);
    //        lineRenderer.SetPosition(i, parametrizedPoints[i]);

    //        yield return new WaitForSeconds(0.5f);

    //    }

    //    yield break;
    //}

    public Vector3[] GetSplinePoints()
    {
        //int NumPoints = parentTransform.childCount;
        Vector3[] vectorsArray = new Vector3[totalLevels + 2];

        int i = 0;
        for (i = 1; i < totalLevels + 1; i++)
            vectorsArray[i] = this.transform.Find("p" + (i)).localPosition;

        vectorsArray[0] = vectorsArray[1] + (vectorsArray[1] - vectorsArray[2]);
        vectorsArray[vectorsArray.Length - 1] = vectorsArray[vectorsArray.Length - 2] + (vectorsArray[vectorsArray.Length - 2] - vectorsArray[vectorsArray.Length - 3]);

        return vectorsArray;
    }


    public void GetCPs()
    {
        levelPoints = new Vector3[splinePoints.Length - 2];
        int index = 0;
        for(int i = 1; i < splinePoints.Length - 1; i++)
        {
            for(int j = 0; j < parametrizedPoints.Length; j++)
            {
                if(Vector3.Distance(splinePoints[i], parametrizedPoints[j]) <= 0.2)
                {
                    levelPoints[index] = parametrizedPoints[j];
                    Debug.Log("<color=cyan> Level Point: </color>" + parametrizedPoints[j]);
                    index++;
                    break;
                }
            }
        }

        Debug.Log("<color=green> Level Points Length: </color>" + levelPoints.Length);
    }    
}

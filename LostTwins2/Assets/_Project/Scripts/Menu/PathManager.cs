using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineHelperFunctions;

public class PathManager : MonoBehaviour
{
    [SerializeField] private int totalLevels = 0;
    [SerializeField] private int baseNumPoints = 50;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float pathTilingDelay = 0.5f;

    private Vector3[] splinePoints;
    private Vector3[] parametrizedPoints;
    private Vector3[] levelPoints;

    private Vector3 point1, point2;
    private int updatedNumPoints = 0;
    private int lastIndexOfParametPoint = default, tillIndex = default;
    private int tiling = 1;
    private bool firstTime = true;
    private ZoneManager zoneManager;


    public Vector3[] Points { get { return splinePoints;} }

    public void InIt(ZoneManager instance)
    {
        zoneManager = instance;
        firstTime = true;
        tiling = 1;
        splinePoints = GetSplinePoints();
        updatedNumPoints = Mathf.FloorToInt(baseNumPoints * (splinePoints.Length));//SplineHelper.GetPathLength(splinePoints));
        Debug.Log("<color=green> Num Points: </color>" + updatedNumPoints);
        parametrizedPoints = SplineHelper.ParameterizeCPs(splinePoints, updatedNumPoints);
        GetCPs();
    }

    public void UpdateLevelProgression(int unlockedLevel)
    {

        if(firstTime || zoneManager.MainMenu.PlayerStateManager.UnlockAllCheatUsed)
        {
            if (unlockedLevel > 0) 
            {
                SetPathRangeValue(unlockedLevel);
                PlaceTilesTillLastUnlockedLevel(); // On first launch if certain levels are already completed then just put the path tiles on it;
            }

            firstTime = false;
            return;
        }

        if (unlockedLevel > 0)
        {
            Debug.Log("Create A Path");
            SetPathRangeValue(unlockedLevel);
            UpdatePath(pathTilingDelay);
        }
    }

    private void SetPathRangeValue(int levelUnlocked)
    {
        Debug.Log("unlocked level: " + levelPoints[levelUnlocked]);
        point2 = levelPoints[levelUnlocked];

        for (int i = 0; i < parametrizedPoints.Length; i++)
        {
            if (point2 == parametrizedPoints[i])
            {
                tillIndex = i;
            }
        }

        Debug.LogFormat("Point1: {0} , p2: {1}", point1, point2);
    }

    private Vector3[] GetSplinePoints()
    {
        Vector3[] vectorsArray = new Vector3[zoneManager.LevelScenes.Length + 2];

        int i = 0;

        for (i = 1; i < zoneManager.LevelScenes.Length + 1; i++)
        {
            vectorsArray[i] = this.transform.Find("p" + (i)).localPosition;
        }

        vectorsArray[0] = vectorsArray[1] + (vectorsArray[1] - vectorsArray[2]);
        vectorsArray[vectorsArray.Length - 1] = vectorsArray[vectorsArray.Length - 2] + (vectorsArray[vectorsArray.Length - 2] - vectorsArray[vectorsArray.Length - 3]);

        return vectorsArray;
    }

    public void GetCPs()
    {
        levelPoints = new Vector3[splinePoints.Length - 2];
        int index = 0;
        for (int i = 1; i < splinePoints.Length - 1; i++)
        {
            for (int j = 0; j < parametrizedPoints.Length; j++)
            {
                if (Vector3.Distance(splinePoints[i], parametrizedPoints[j]) <= Mathf.Floor(SplineHelper.CP_Increment))
                {
                    levelPoints[index] = parametrizedPoints[j];
                    index++;
                    break;
                }
            }
        }
    }

    private void UpdatePath(float tilingDelay)
    {
        StartCoroutine(_PlacePathPoints(tilingDelay));
    }

    private void PlaceTilesTillLastUnlockedLevel()
    {
        Vector3 previousPoint = parametrizedPoints[0];


        for (int i = 1; i <= tillIndex; i++)
        {
                lineRenderer.positionCount = i;
                lineRenderer.SetPosition(i - 1, parametrizedPoints[i]);

                float distance = Vector3.Distance(previousPoint, parametrizedPoints[i]);

                if (distance >= Mathf.Floor(SplineHelper.CP_Increment))
                {
                    lineRenderer.material.mainTextureScale = new Vector2(tiling, lineRenderer.material.mainTextureScale.y);
                    tiling++;
                    previousPoint = parametrizedPoints[i - 1];
                }
        }

        lastIndexOfParametPoint = tillIndex;
    }

    private IEnumerator _PlacePathPoints(float tilingDelay)
    {
        Vector3 previousPoint;

        if(lastIndexOfParametPoint > 0)
        {
            previousPoint = parametrizedPoints[lastIndexOfParametPoint];
            lastIndexOfParametPoint++;
        }

        else
        {
            lastIndexOfParametPoint = 1;
            previousPoint = parametrizedPoints[0];
        }

        for (int i = lastIndexOfParametPoint; i <= tillIndex; i++)
        {
            lineRenderer.positionCount = i;
            lineRenderer.SetPosition(i - 1, parametrizedPoints[i]);

            float distance = Vector3.Distance(previousPoint, parametrizedPoints[i]);

            if (distance >= Mathf.Floor(SplineHelper.CP_Increment))
            {
                lineRenderer.material.mainTextureScale = new Vector2(tiling, lineRenderer.material.mainTextureScale.y);
                tiling++;
                previousPoint = parametrizedPoints[i];
            }

            yield return new WaitForSeconds(tilingDelay);
        }

        lastIndexOfParametPoint = tillIndex;
        yield break;
    }

    public void Reset()
    {
        lineRenderer.positionCount = 0;
        lineRenderer.material.mainTextureScale = new Vector2(1, lineRenderer.material.mainTextureScale.y);
        lastIndexOfParametPoint = default;
        tiling = 1;
    }
}

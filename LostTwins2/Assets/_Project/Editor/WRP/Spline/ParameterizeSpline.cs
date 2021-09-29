using UnityEditor;
using UnityEngine;
using SplineHelperFunctions;

public class ParameterizeSpline : ScriptableWizard
{
    public int numPoints = 50;

    void OnWizardUpdate()
    {
        helpString = "Select number of points";
        isValid = (numPoints>=2);
    }

    void OnWizardCreate()
    {
        if (Selection.activeGameObject == null)
            return;

        Transform originalParent = Selection.activeTransform;
        Vector3[] splinePoints;
        splinePoints = null;
        splinePoints = SplineHelper.GetSplinePoints (originalParent);
        splinePoints = SplineHelper.ParameterizeCPs (splinePoints, numPoints);
        GameObject parentGO = new GameObject(originalParent.name+"_P");
        parentGO.transform.parent = originalParent.parent;
        parentGO.AddComponent<SplineDrawer>();

        int i = 0;
        GameObject tempGO;
        for(i = 1; i < splinePoints.Length-1; i++)
        {
            tempGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tempGO.name = "p"+i;
            DestroyImmediate(tempGO.GetComponent<Collider>());
            tempGO.transform.parent = parentGO.transform;
            tempGO.transform.position = splinePoints[i];
            tempGO.GetComponent<Renderer>().enabled = false;
        }
    }

	[MenuItem ("We.R.Play/Spline/Create Parameterized Spline")]
	static void MakeParameterizedSpline()
	{
        ScriptableWizard.DisplayWizard("Create Parameterized Spline", typeof(ParameterizeSpline), "Create");
	}
}
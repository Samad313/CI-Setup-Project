using UnityEditor;
using UnityEngine;
using SplineHelperFunctions;

public class CreateSpline : ScriptableWizard
{
	public int numPoints = 5;
	
	void OnWizardUpdate()
	{
		helpString = "Select number of points";
		isValid = (numPoints>=2);
	}
	
	void OnWizardCreate()
	{
		GameObject parentGO = new GameObject("Spline");
		parentGO.AddComponent<SplineDrawer>();
		
		int i = 0;
		GameObject tempGO;
		for(i = 0; i < numPoints; i++)
		{
			tempGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
			tempGO.name = "p"+(i+1);
			DestroyImmediate(tempGO.GetComponent<Collider>());
			tempGO.transform.parent = parentGO.transform;
            if(i==0)
			    tempGO.transform.localPosition = new Vector3(0, 0, 0);
            else
                tempGO.transform.localPosition = new Vector3(Random.Range(0, 50), Random.Range(0, 50), i * 100);
		}
	}
	
	[MenuItem ("We.R.Play/Spline/Create Spline")]
	static void MakeSpline()
	{
		ScriptableWizard.DisplayWizard("Create Spline", typeof(CreateSpline), "Create");
	}
}
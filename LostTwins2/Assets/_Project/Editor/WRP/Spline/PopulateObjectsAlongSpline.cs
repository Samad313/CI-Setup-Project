using UnityEditor;
using UnityEngine;
using SplineHelperFunctions;

public class PopulateObjectsAlongSpline : ScriptableWizard
{
	public GameObject theSpline;
	public GameObject objectToPopulate;
	public int numObjects = 10;
	public bool alignObject = false;
	
	void OnWizardUpdate()
	{
		helpString = "Drag spline and object";
        isValid = (theSpline != null && objectToPopulate != null && numObjects>1);
	}
	
	void OnWizardCreate()
	{
		Vector3[] vectorsArray = SplineHelper.GetSplinePoints(theSpline.transform);
		float offset = 1.0f/(float)(numObjects-1.0f);

		GameObject parentGO = new GameObject("PopulatedObjects");

		float i = 0;
		for(i = 0; i <= 1; i+=offset)
		{
			GameObject tempGO = (GameObject)Instantiate(objectToPopulate, SplineHelper.Interp(vectorsArray, i), Quaternion.identity);
			tempGO.transform.parent = parentGO.transform;
			tempGO.name = objectToPopulate.name;

			if(alignObject)
			{
				Vector3 relativePos = SplineHelper.Interp(vectorsArray, i-0.001f) - SplineHelper.Interp(vectorsArray, i+0.001f);
				Quaternion rotation = Quaternion.LookRotation(relativePos);
				tempGO.transform.rotation = rotation;
			}
		}
	}

	[MenuItem ("We.R.Play/Spline/Populate Objects Along Spline")]
	static void PopulateObjects()
	{
		ScriptableWizard.DisplayWizard("Populate Objects", typeof(PopulateObjectsAlongSpline), "Populate");
	}
}

using UnityEngine;
using System.Collections;
using SplineHelperFunctions;

public class SplineDrawer : MonoBehaviour
{
	public int resolution = 100;


    private void Start()
    {
		Debug.Log("<color=green> Spline Points Value: </color>" + SplineHelperFunctions.SplineHelper.GetSplinePoints(this.transform).Length);


	}

	void OnDrawGizmos()
	{

		//Gizmos.color = Color.blue;
		//Gizmos.DrawLine(transform.position, target.position);

		if(transform.Find("p1"))
		{
			Vector3[] vectorsArray = SplineHelper.GetSplinePoints(transform);
			DrawPath(vectorsArray);
		}
	}



	private void DrawPath(Vector3[] vectorsArray)
	{
		float offset = 1.0f/(float)(resolution);

		for(int i=0;i<resolution;i++)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(SplineHelper.Interp(vectorsArray, i*offset), SplineHelper.Interp(vectorsArray, i*offset+offset));
			//Debug.Log(i);
		}
	}


}

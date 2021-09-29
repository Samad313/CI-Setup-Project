using UnityEngine;
using System.Collections;

namespace SplineHelperFunctions
{
	public static class SplineHelper
	{

		public static float CP_Increment = default;

		public static Vector3 Interp(Vector3[] pts, float t)
		{
			t = Mathf.Clamp(t, 0.0f, 2.0f);
			//t = ActualPercentage(t);
			int numSections = pts.Length - 3;
			int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
			float u = t * (float)numSections - (float)currPt;
			Vector3 a = pts[currPt];
			Vector3 b = pts[currPt + 1];
			Vector3 c = pts[currPt + 2];
			Vector3 d = pts[currPt + 3];
			
			return .5f * (
				(-a + 3f * b - 3f * c + d) * (u * u * u)
				+ (2f * a - 5f * b + 4f * c - d) * (u * u)
				+ (-a + c) * u
				+ 2f * b
				);
		}

		public static Vector3[] GetSplinePoints(Transform parentTransform)
		{
			int NumPoints = parentTransform.childCount;
			Vector3[] vectorsArray = new Vector3[NumPoints+2];
			
			int i = 0;
			for(i=1;i<NumPoints+1;i++)
				vectorsArray[i] = parentTransform.Find("p"+(i)).localPosition;
			
			vectorsArray[0] = vectorsArray[1] + (vectorsArray[1] - vectorsArray[2]);
			vectorsArray[vectorsArray.Length-1] = vectorsArray[vectorsArray.Length-2] + (vectorsArray[vectorsArray.Length-2] - vectorsArray[vectorsArray.Length-3]);

			return vectorsArray;
		}

		public static Vector3[] AddSplineEndPoints(Vector3[] inputVectors)
		{
			int NumPoints = inputVectors.Length;
			Vector3[] vectorsArray = new Vector3[NumPoints+2];
			
			int i = 0;
			for(i=1;i<NumPoints+1;i++)
				vectorsArray[i] = inputVectors[i-1];
			
			vectorsArray[0] = vectorsArray[1] + (vectorsArray[1] - vectorsArray[2]);
			vectorsArray[vectorsArray.Length-1] = vectorsArray[vectorsArray.Length-2] + (vectorsArray[vectorsArray.Length-2] - vectorsArray[vectorsArray.Length-3]);
			
			return vectorsArray;
		}

		public static Vector3[] PathControlPointGenerator(Vector3[] path)
		{
			Vector3[] suppliedPath;
			Vector3[] vector3s;
			
			//create and store path points:
			suppliedPath = path;
			
			//populate calculate path;
			int offset = 2;
			vector3s = new Vector3[suppliedPath.Length+offset];
			System.Array.Copy(suppliedPath,0,vector3s,1,suppliedPath.Length);
			
			//populate start and end control points:
			//vector3s[0] = vector3s[1] - vector3s[2];
			vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
			vector3s[vector3s.Length-1] = vector3s[vector3s.Length-2] + (vector3s[vector3s.Length-2] - vector3s[vector3s.Length-3]);
			
			//is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
			if(vector3s[1] == vector3s[vector3s.Length-2]){
				Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
				System.Array.Copy(vector3s,tmpLoopSpline,vector3s.Length);
				tmpLoopSpline[0]=tmpLoopSpline[tmpLoopSpline.Length-3];
				tmpLoopSpline[tmpLoopSpline.Length-1]=tmpLoopSpline[2];
				vector3s=new Vector3[tmpLoopSpline.Length];
				System.Array.Copy(tmpLoopSpline,vector3s,tmpLoopSpline.Length);
			}
			
			return(vector3s);
		}

		public static Vector3[] ReverseSplinePoints(Vector3[] pts)
		{
			Vector3[] reversedSplinePoints = new Vector3[pts.Length];
			for (int i = 0; i < pts.Length; i++)
			{
				reversedSplinePoints[pts.Length - i - 1] = pts[i];
			}

			return reversedSplinePoints;
		}

		public static Quaternion GetRotationAtPoint(Vector3[] pts, float t)
		{
			Vector3 firstPoint = Interp(pts, t-0.001f);
			Vector3 secondPoint = Interp(pts, t+0.001f);
			Vector3 rotationVector = secondPoint - firstPoint;

			return Quaternion.LookRotation(rotationVector);
		}

		public static Vector3 GetVectorAtPoint(Vector3[] pts, float t)
        {
			Vector3 firstPoint = Interp(pts, t - 0.001f);
			Vector3 secondPoint = Interp(pts, t + 0.001f);

			return firstPoint - secondPoint;
		}

		public static float GetPathLength(Vector3[] pts)
		{
			float totalDistance = 0;
			float i = 0.0f;
			int j = 0;
			while(i<1.0f)
			{
				totalDistance+=(Interp(pts, i) - Interp(pts, i+0.0001f) ).magnitude;
				j++;
				i = j * 0.0001f;
			}

			return totalDistance;
		}

        public static Vector3[] ParameterizeCPs(Vector3[] pts, int numPoints)
		{
			float i = 0.0f;
			float Current_TD = 0.0f;
			float TotalPathLength = SplineHelper.GetPathLength(pts);
			Debug.Log("Total Path Length: " + TotalPathLength);
            float numPointsFloat = numPoints;

            //float CP_Increment = TotalPathLength / numPointsFloat;
            CP_Increment = TotalPathLength / numPointsFloat;
			Debug.Log("CP Increment Value: " + (TotalPathLength / numPointsFloat));
			Vector3 PreviousPoint = pts[1];
			Vector3 CurrentPoint = PreviousPoint;
            Vector3[] FinalPoints = new Vector3[numPoints+1];
			int Index = 0;
			FinalPoints[Index] = pts[1];
			Index++;
			for(i=0;i<=1.0f;i+=0.0001f)
			{
				CurrentPoint = SplineHelper.Interp(pts,i);
				Current_TD+=Vector3.Distance(CurrentPoint,PreviousPoint);

				if(Current_TD >= CP_Increment)
				{
					//Debug.Log("<color=green> Final Points: </color>" + CurrentPoint);

                    FinalPoints[Index] = CurrentPoint;
					Current_TD = 0;
					Index++;
				}
				PreviousPoint = CurrentPoint;
			}
            FinalPoints[numPoints] = pts[pts.Length-2];
			FinalPoints = SplineHelper.AddSplineEndPoints(FinalPoints);


            return FinalPoints;
		}
	}
}

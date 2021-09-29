using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPointsController : MonoBehaviour
{
    private CameraPoint[] myCameraPoints;

    private Vector3 combinedAdditionalOffset;
    private float combinedAdditionalFOV;

    public void Init()
    {
        myCameraPoints = new CameraPoint[transform.childCount];
        int i = 0;

        foreach (Transform child in transform)
        {
            myCameraPoints[i] = child.GetComponent<CameraPoint>();
            i++;
        }
    }

    public void CalculateValues(Vector3 cameraTargetPosition)
    {
        combinedAdditionalOffset = Vector3.zero;
        combinedAdditionalFOV = 0;

        for (int i = 0; i < myCameraPoints.Length; i++)
        {
            float strength = Mathf.InverseLerp(myCameraPoints[i].GetInfluenceRadius(), 0, (myCameraPoints[i].transform.position - cameraTargetPosition).magnitude);
            if(strength>0)
            {
                combinedAdditionalOffset += myCameraPoints[i].GetAdditionalOffset() * strength;
                combinedAdditionalFOV += myCameraPoints[i].GetAdditionalFOV() * strength;
            }
        }
    }

    public Vector3 GetAdditionalOffset()
    {
        return combinedAdditionalOffset;
    }

    public float GetAdditionalFOV()
    {
        return combinedAdditionalFOV;
    }

    public void MakeChildrenOfPieces()
    {
        for (int i = 0; i < myCameraPoints.Length; i++)
        {
            myCameraPoints[i].transform.parent = GameplayManager.instance.GetPieceHoverTransformFromPosition(new Vector2(myCameraPoints[i].transform.position.x, myCameraPoints[i].transform.position.y));
        }
    }

    public void MakeChildrenOfCameraPointsGroup()
    {
        for (int i = 0; i < myCameraPoints.Length; i++)
        {
            myCameraPoints[i].transform.parent = transform;
        }
    }
}

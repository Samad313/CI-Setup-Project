using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOutClouds : MonoBehaviour
{
    private Transform[] allClouds;

    private Vector3[] startingPositions;
    private Vector3[] endingPositions;

    private bool initCalled = false;

    [SerializeField]
    private float outDistance = 80.0f;


    [SerializeField]
    private Transform starParticles = default;

    // Start is called before the first frame update
    void Start()
    {
        if (initCalled == false)
            Init();
    }

    private void Init()
    {
        allClouds = new Transform[transform.childCount];
        startingPositions = new Vector3[transform.childCount];
        endingPositions = new Vector3[transform.childCount];
        Vector2 tempVector;
        for (int i = 0; i < allClouds.Length; i++)
        {
            allClouds[i] = transform.GetChild(i);
            startingPositions[i] = allClouds[i].position;
            tempVector = new Vector2(startingPositions[i].x, startingPositions[i].y);
            tempVector = tempVector.normalized * outDistance;
            endingPositions[i] = new Vector3(tempVector.x, tempVector.y, startingPositions[i].z);
        }

        initCalled = true;
    }

    public void SetClouds(float lerpValue)
    {
        if (initCalled == false)
            Init();

        for (int i = 0; i < allClouds.Length; i++)
        {
            allClouds[i].position = Vector3.Lerp(endingPositions[i], startingPositions[i], lerpValue);
        }

        if(starParticles)
            starParticles.localPosition = Vector3.Lerp(new Vector3(0, 0, 45), new Vector3(0, 0, 35), lerpValue);
    }
}

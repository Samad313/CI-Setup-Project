using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepMaker : MonoBehaviour
{
    [SerializeField]
    private Transform projectorsParent = default;

    [SerializeField]
    private Transform leftFoot = default;

    [SerializeField]
    private Transform rightFoot = default;

    [SerializeField]
    private float yOffsetFromGround = 1.0f;

    private Transform transformForGround = default;
    private float wantedYForGround = default;
    private float currentY = 0.0f;
    [SerializeField]
    private float ySpeed = 3.5f;

    [SerializeField]
    private Transform snowTrack = default;

    [SerializeField]
    private float snowTrackYOffset = -0.02f;

    private Vector3 currentGroundNormal = new Vector3(0, 1, 0);

    // Start is called before the first frame update
    void Start()
    {
        transformForGround = rightFoot;
        SetGroundPosition();
        currentY = wantedYForGround;
    }

    // Update is called once per frame
    void Update()
    {
        SetGroundPosition();
        transform.Translate(0.7f * Time.deltaTime, 0, 0, Space.World);
        currentY = Mathf.Lerp(currentY, wantedYForGround, Time.deltaTime * ySpeed);
        transform.position = new Vector3(transform.position.x, currentY, transform.position.z);
    }

    public void MakeFootStep(string direction)
    {
        //Transform tempTransform = Instantiate(footstepsProjector, projectorsParent);
        //if(direction=="Right")
        //    tempTransform.localPosition = new Vector3(rightFoot.position.x, 1, rightFoot.position.z);
        //else
        //    tempTransform.localPosition = new Vector3(leftFoot.position.x, 1, leftFoot.position.z);

        
        Transform tempTransform = Instantiate(snowTrack, projectorsParent);
        tempTransform.localPosition = new Vector3(transformForGround.position.x, wantedYForGround + snowTrackYOffset, transformForGround.position.z);
        tempTransform.rotation = Quaternion.FromToRotation(Vector3.up, currentGroundNormal);
    }

    private void NextFoot(string direction)
    {
        if (direction == "Right")
            transformForGround = rightFoot;
        else
            transformForGround = leftFoot;
    }

    private void SetGroundPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(transformForGround.position + new Vector3(0, 2, 0), new Vector3(0, -1, 0), out hit, 100, 1 << 15))
        {
            wantedYForGround = hit.point.y + yOffsetFromGround;
            currentGroundNormal = hit.normal;
        }
    }
}

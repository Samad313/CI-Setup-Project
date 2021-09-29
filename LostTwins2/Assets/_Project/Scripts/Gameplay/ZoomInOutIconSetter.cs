using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInOutIconSetter : MonoBehaviour
{
    public static Vector3 GetIconPositionFromCamera(Vector3 cameraPosition, Vector3 playerPosition, float zOffset)
    {
        float angle = Vector3.Angle(cameraPosition - playerPosition, Vector3.right);
        float xOffset = zOffset / Mathf.Tan(Mathf.Deg2Rad * angle);
        angle = Vector3.Angle(cameraPosition - playerPosition, Vector3.up);
        float yOffset = zOffset / Mathf.Tan(Mathf.Deg2Rad * angle);

        return new Vector3(playerPosition.x + xOffset, playerPosition.y + yOffset, -zOffset);
    }
}

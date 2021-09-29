using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPoint : MonoBehaviour
{
    [SerializeField]
    private Vector3 additionalOffset;

    [SerializeField]
    private float additionalFOV;

    [SerializeField]
    private float influenceDistance = 10.0f;

    [SerializeField]
    private float influenceDisableStartingTime = 2.0f;

    public Vector3 GetAdditionalOffset()
    {
        return additionalOffset;
    }

    public float GetAdditionalFOV()
    {
        return additionalFOV;
    }

    public float GetInfluenceRadius()
    {
        return influenceDistance;
    }

    public void DisableInfluence()
    {
        StartCoroutine("DisableInfluenceCoroutine");
    }

    private IEnumerator DisableInfluenceCoroutine()
    {
        yield return new WaitForSeconds(influenceDisableStartingTime);
        Vector3 startingAdditionalOffset = additionalOffset;
        float startingAdditionalFOV = additionalFOV;
        float t = 0;
        while(t<1.0f)
        {
            additionalOffset = Vector3.Lerp(startingAdditionalOffset, Vector3.zero, t);
            additionalFOV = Mathf.Lerp(startingAdditionalFOV, 0, t);
            t += Time.deltaTime * 0.3f;
            yield return new WaitForEndOfFrame();
        }

        additionalOffset = Vector3.zero;
        additionalFOV = 0;
    }
}

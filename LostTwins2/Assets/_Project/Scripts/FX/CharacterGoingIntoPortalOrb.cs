using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGoingIntoPortalOrb : MonoBehaviour
{
    private Vector3 finalPosition;

    private Transform parentGroup;

    private float rotationDirection = 1.0f;

    [SerializeField]
    private float rotationSpeed = 180.0f;

    [SerializeField]
    private float goingInwardSpeed = 1.5f;

    private bool movementDisabled = false;

    private int moveState = 0;

    private float currentRotationZ = 0;
    private float initialDelay = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        finalPosition = GameplayManager.instance.FinalPortal.GetCenterForOrbPosition();
        parentGroup = (new GameObject("CharacterGoingIntoPortalOrbGroup")).transform;
        parentGroup.position = (finalPosition + transform.position)/2.0f + new Vector3(0, -0.5f, 0);
        transform.parent = parentGroup;
        if (transform.position.x < finalPosition.x)
            rotationDirection = -1.0f;

        rotationSpeed += Random.Range(-0.3f, 0.3f);
        goingInwardSpeed += Random.Range(-0.3f, 0.3f);
        initialDelay = Random.Range(0, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        if (movementDisabled)
            return;

        if (moveState == 0)
        {
            initialDelay -= Time.deltaTime;
            if(initialDelay<0)
            {
                moveState = 10;
            }
        }
        else if (moveState==10)
        {
            parentGroup.localEulerAngles = new Vector3(0, 0, currentRotationZ* rotationDirection);
            currentRotationZ += Time.deltaTime * rotationSpeed;
            if(currentRotationZ>180.0f)
            {
                currentRotationZ = 0;
                transform.parent = null;
                parentGroup.position = finalPosition;
                parentGroup.localEulerAngles = Vector3.zero;
                transform.parent = parentGroup;
                moveState = 20;
            }
        }
        else if (moveState == 20)
        {
            parentGroup.localEulerAngles = new Vector3(0, 0, currentRotationZ* rotationDirection*-1.0f*1.5f);
            currentRotationZ += Time.deltaTime * rotationSpeed * 3.0f;
            if (currentRotationZ > 180.0f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, Time.deltaTime * goingInwardSpeed);
            }
        }

        if ((transform.localPosition).sqrMagnitude<0.01f)
        {
            movementDisabled = true;
            StartCoroutine("DisableParticles");
        }
    }

    private IEnumerator DisableParticles()
    {
        //foreach (Transform item in transform)
        //{
        //    var particleEmission = item.GetComponent<ParticleSystem>().emission;
        //    particleEmission.rateOverTime = 0;
        //}

        yield return new WaitForSeconds(0.5f);
        Destroy(parentGroup.gameObject);
    }
}

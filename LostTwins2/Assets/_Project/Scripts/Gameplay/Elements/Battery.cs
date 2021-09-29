using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    private bool isBatteryActivated = false;

    public bool IsBatteryActivated
    {
        get => isBatteryActivated;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 18) //Pressure Button Layer
        {
            //Disable the pushable battery controls
            //Get the empty transform where the battery fits.
            Transform fixTransform = other.gameObject.GetComponentInParent<BatteryContainer>().GetBatteryFixTransform(other.transform, this.transform);
            
            if(fixTransform != null)
            {
                StartCoroutine(FixBatteryAfterSomeTime(fixTransform));
                StartCoroutine(ActivateBattery(fixTransform));
            }

        }
    }

    private IEnumerator FixBatteryAfterSomeTime(Transform fixTransform)
    {
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<PushableBox>().FixToButton(fixTransform);
    }

    private IEnumerator ActivateBattery(Transform triggerObject)
    {
        float xDistance = (transform.position - triggerObject.position).sqrMagnitude; //Check if battery reaches to the the trigger object
        while (xDistance > 0.04f)
        {
            xDistance = (transform.position - triggerObject.position).sqrMagnitude;
            yield return new WaitForEndOfFrame();
        }
        isBatteryActivated = true;
    }

}

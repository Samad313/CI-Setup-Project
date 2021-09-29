///Daniel Moore (Firedan1176) - Firedan1176.webs.com/
///26 Dec 2015
///
///Shakes camera parent object

using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	[SerializeField]
	private float shakeAmount = default;//The amount to shake this frame.

	[SerializeField]
	private float shakeDuration = default;//The duration this frame.

	private float timeLeft;

	//Readonly values...
	private float shakePercentage;//A percentage (0-1) representing the amount of shake to be applied when setting rotation.
	private float startAmount;//The initial shake amount (to determine percentage), set when ShakeCamera is called.
	private float startDuration;//The initial shake duration, set when ShakeCamera is called.

	private bool isRunning = false; //Is the coroutine running right now?

	[SerializeField]
	private bool smooth = default;//Smooth rotation?

	[SerializeField]
	private float smoothAmount = 5f;//Amount to smooth

	void Start()
	{

	}

	public void ShakeCamera()
	{
		startAmount = shakeAmount;//Set default (start) values
		startDuration = shakeDuration;//Set default (start) values
		timeLeft = shakeDuration;

		if (!isRunning) StartCoroutine(Shake());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
	}

	public void ShakeCamera(float amount, float duration)
	{
		startAmount = amount;
		startDuration = duration;
		timeLeft = duration;

		if (!isRunning) StartCoroutine(Shake());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
	}

	IEnumerator Shake()
	{
		isRunning = true;

		while (timeLeft > 0.01f)
		{
			shakePercentage = timeLeft / startDuration;

			Vector3 rotationAmount = Random.insideUnitSphere * startAmount * shakePercentage;
			//rotationAmount.z = 0;

			//timeLeft = Mathf.Lerp(timeLeft, 0, Time.deltaTime);
			timeLeft -= Time.deltaTime;

			if (smooth)
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationAmount), Time.deltaTime * smoothAmount);
			else
				transform.localRotation = Quaternion.Euler(rotationAmount);//Set the local rotation the be the rotation amount.

			yield return null;
		}

		transform.localRotation = Quaternion.identity;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
		isRunning = false;
	}

}
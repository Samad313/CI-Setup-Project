using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [SerializeField] private float minForce = 10.0f;
    [SerializeField] private float force = 16.0f;
    [SerializeField] private Animator myAnimator;
    [SerializeField] private Transform[] bounds;
    [SerializeField] private Transform centerPoint;

    private float maxDistanceSquaredFromCenter;

    [SerializeField] private Transform top;

    private PlayerController myPlayer;

    private float delayForJump = -1.0f;

    // Start is called before the first frame update
    void Start()
    {
        float centerX = Mathf.Abs(bounds[0].localPosition.x + bounds[1].localPosition.x) / 2.0f;
        float centerY = Mathf.Abs(bounds[0].localPosition.y + bounds[1].localPosition.y) / 2.0f;
        centerPoint.localPosition = new Vector3(centerX, centerY, 0);

        maxDistanceSquaredFromCenter = 0;
        maxDistanceSquaredFromCenter = Mathf.Abs(bounds[0].localPosition.x - bounds[1].localPosition.x)/2.0f;

        maxDistanceSquaredFromCenter *= 1.1f;
        maxDistanceSquaredFromCenter *= maxDistanceSquaredFromCenter;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsPlayerOnTop(GameplayManager.instance.CurrentPlayerTransform))
        {
            CollisionOccured(GameplayManager.instance.CurrentPlayerTransform);
        }
        delayForJump -= Time.deltaTime;
    }

    public void CollisionOccured(Transform playerTransform)
    {
        if (delayForJump > 0)
            return;

        delayForJump = 0.5f;
        myPlayer = playerTransform.GetComponent<PlayerController>();
        myPlayer.EnteredMushroom(top);
        myAnimator.SetTrigger("JumpedOn");

        StartCoroutine("WaitForAnimation");
    }

    private bool IsPlayerOnTop(Transform playerTransform)
    {
        Vector2 playerPos = playerTransform.position;
        if((playerPos- new Vector2(centerPoint.position.x, centerPoint.position.y)).sqrMagnitude > maxDistanceSquaredFromCenter)
        {
            return false;
        }

        if(playerPos.x > bounds[0].position.x && playerPos.x < bounds[1].position.x
            && playerPos.y > bounds[2].position.y && playerPos.y < bounds[3].position.y)
        {
            return true;
        }

        return false;
        //float middleX = (colliderPoints[0].position.x + colliderPoints[2].position.x)/2.0f;
        //if (playerPos.x < middleX)
        //{
        //    playerPos.x = middleX + middleX - playerPos.x;
        //}

        //float tY = slopeOfPoints * (playerPos.x - colliderPoints[2].position.x) + colliderPoints[2].position.y;

        //if (playerPos.y < tY)
        //    return true;

        //return false;
    }

    private IEnumerator WaitForAnimation()
    {
        while(myAnimator.GetCurrentAnimatorStateInfo(0).IsName("JumpedOnEnd")==false)
        {
            yield return new WaitForEndOfFrame();
        }

        myPlayer.MushroomJump(force, minForce);
    }
}

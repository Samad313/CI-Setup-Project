using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalGate : MonoBehaviour
{

    private Transform[] playerCenters;
    [SerializeField] private Transform visualGroup = default;

    [SerializeField] private Transform mainVisual = default;

    [SerializeField] private float reachedDistance = 2.0f;

    [SerializeField]
    private GameObject[] fxObjects;

    [SerializeField]
    private GameObject statue;

    private bool girlReached = false;
    private bool boyReached = false;

    private float reachedDistanceSquared;

    // Start is called before the first frame update
    void Start()
    {
        playerCenters = GameplayManager.instance.GetAllPlayerCenters();

        reachedDistanceSquared = reachedDistance * reachedDistance;

        Vector3 gateCenterPosition = new Vector3(visualGroup.position.x, visualGroup.position.y, 0);
        boyReached = (gateCenterPosition - playerCenters[0].position).sqrMagnitude < reachedDistanceSquared;
        girlReached = (gateCenterPosition - playerCenters[1].position).sqrMagnitude < reachedDistanceSquared;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.instance.LevelCompleted || GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
            return;

        bool previousBoyReached = boyReached;
        bool previousGirlReached = girlReached;

        Vector3 gateCenterPosition = new Vector3(visualGroup.position.x, visualGroup.position.y, 0);

        boyReached = (gateCenterPosition - playerCenters[0].position).sqrMagnitude < reachedDistanceSquared;
        girlReached = (gateCenterPosition - playerCenters[1].position).sqrMagnitude < reachedDistanceSquared;

        if(previousBoyReached != boyReached)
        {
            if(boyReached)
            {
                AudioManager.instance.PlaySoundEffect("PortalAction");
            }
        }

        if (previousGirlReached != girlReached)
        {
            if(girlReached)
            {
                AudioManager.instance.PlaySoundEffect("PortalAction");
            }
        }

        if (boyReached && girlReached)
        {
            GameplayManager.instance.FinishLevel(transform.Find("VisualGroup").position);
            if (mainVisual)
            {
                mainVisual.parent = null;
            }
        }
    }

    public Vector3 GetCenterForOrbPosition()
    {
        return transform.Find("centerForOrb").position;
    }

    public void SetReachedDistance(float value)
    {
        reachedDistance = value;
        reachedDistanceSquared = reachedDistance * reachedDistance;
    }

    public Transform GetGateTop()
    {
        return transform.Find("Top");
    }

    public Transform GetMainVisual()
    {
        return mainVisual;
    }

    public void SetStatue(bool value)
    {
        for (int i = 0; i < fxObjects.Length; i++)
        {
            fxObjects[i].SetActive(!value);
        }
        statue.SetActive(value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlatformWeight
{
    public Transform myTransform;
    public bool shouldIncreaseWeight;
    public Transform myParent;

    public PressurePlatformWeight()
    {
        myTransform = null;
        shouldIncreaseWeight = false;
        myParent = null;
    }

    public PressurePlatformWeight(Transform inputTransform, bool weightIncreasing, Transform parentTransform)
    {
        myTransform = inputTransform;
        shouldIncreaseWeight = weightIncreasing;
        myParent = parentTransform;
    }
}

public class oldPressurePlatform : MonoBehaviour
{
    [SerializeField]
    private oldPressurePlatform myPartner = default;

    private float currentUpValue = 1.0f;
    private float wantedUpValue = 1.0f;

    [SerializeField]
    private float defaultUpValue = 1.0f;

    [SerializeField]
    private float maxScale = 4.0f;

    [SerializeField]
    private int maxNumWeights = 2;

    [SerializeField]
    private int numWeightsOnMe = 0;
    private List<PressurePlatformWeight> myWeights;

    private float incrementValue;

    private Transform baseGroup;
    private Transform topGroup;

    [SerializeField]
    private float lerpSpeed = 3.0f;

    private void Awake()
    {
        myWeights = new List<PressurePlatformWeight>();
        baseGroup = transform.Find("BaseGroup");
        topGroup = transform.Find("TopGroup");
    }

    // Start is called before the first frame update
    void Start()
    {
        
        incrementValue = 1.0f / maxNumWeights;
        
        SetWantedUpValue();
        currentUpValue = wantedUpValue;
        SetScaleAndPositions();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
            return;

        SetWantedUpValue();
        currentUpValue = Mathf.Lerp(currentUpValue, wantedUpValue, Time.deltaTime * lerpSpeed);

        SetScaleAndPositions();
    }

    private void SetWantedUpValue()
    {
        int weightsDifference = myPartner.GetNumWeightsOnMe() - numWeightsOnMe;
        wantedUpValue = defaultUpValue + incrementValue * weightsDifference;
    }

    private void SetScaleAndPositions()
    {
        float baseGroupScaleY = Mathf.Lerp(1.0f, maxScale, currentUpValue);
        baseGroup.localScale = new Vector3(1, baseGroupScaleY, 1);
        topGroup.localPosition = new Vector3(0, baseGroupScaleY + 1.0f, 0);
    }

    public int GetNumWeightsOnMe()
    {
        return numWeightsOnMe;
    }

    public void AddWeightObject(Transform weightTransform, bool shouldIncreaseWait)
    {
        bool newObject = true;

        for (int i = 0; i < myWeights.Count; i++)
        {
            if(weightTransform == myWeights[i].myTransform)
            {
                newObject = false;
                break;
            }
        }

        if(newObject)
        {
            myWeights.Add(new PressurePlatformWeight(weightTransform, shouldIncreaseWait, weightTransform.parent));
            weightTransform.parent = topGroup;
            if (shouldIncreaseWait)
                numWeightsOnMe++;
        }
    }

    public void RemoveWeightObject(Transform weightTransform)
    {
        for (int i = 0; i < myWeights.Count; i++)
        {
            if (weightTransform == myWeights[i].myTransform)
            {
                if (myWeights[i].shouldIncreaseWeight)
                    numWeightsOnMe--;
                weightTransform.parent = myWeights[i].myParent;
                myWeights.RemoveAt(i);
                break;
            }
        }
    }
}

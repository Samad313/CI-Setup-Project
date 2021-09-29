using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryContainer : MonoBehaviour
{
    [System.Serializable]
    public class BatteriesContainer
    {
        [SerializeField]
        private Transform fixPositionTransform;

        [SerializeField]
        private Transform triggerTransform;

        private bool isPositionFilled = false;


        public BatteriesContainer(Transform fixPositionTransform, Transform triggerTransform)
        {
            this.fixPositionTransform = fixPositionTransform;
            this.triggerTransform = triggerTransform;
        }

        public Transform FixPositionTransform
        {
            get => fixPositionTransform;
        }

        public Transform TriggerTransform
        {
            get => triggerTransform;
        }

        public bool IsPositionFilled
        {
            get => isPositionFilled;
        }

        public void FillBatteryPosition()
        {
            isPositionFilled = true;
        }
    }

    [SerializeField]
    private int maxTriggerCollidersCount = 0;
  
    private List<BatteriesContainer> batteriesContainer = new List<BatteriesContainer>();


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < maxTriggerCollidersCount; i++)
        {
            Transform triggerObject = transform.Find("TriggerCollider" + (i + 1));
            Transform fixTransform = transform.Find("FixTransform" + (i + 1));
            batteriesContainer.Add(new BatteriesContainer( fixTransform, triggerObject));
        }
    }


    public bool AllBatteriesFixed()
    {
        for (int i = 0; i < batteriesContainer.Count; i++)
        {
            if(!batteriesContainer[i].IsPositionFilled)
            {
                return false;
            }

        }
        return true;
    }

    public Transform GetBatteryFixTransform(Transform triggerObject, Transform battery)
    {
        if (battery.position.x < triggerObject.position.x)
        {
            for (int i = batteriesContainer.Count - 1; i >= 0; i--)
            {
                if (!batteriesContainer[i].IsPositionFilled)
                {
                    batteriesContainer[i].FillBatteryPosition();
                    return batteriesContainer[i].FixPositionTransform;
                }
            }
        }
        else
        {
            for (int i = 0; i < batteriesContainer.Count; i++)
            {
                if (!batteriesContainer[i].IsPositionFilled)
                {
                    batteriesContainer[i].FillBatteryPosition();
                    return batteriesContainer[i].FixPositionTransform;
                }
            }
        }
        return null;
    }
    
}

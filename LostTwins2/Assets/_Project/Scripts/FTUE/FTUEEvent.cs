using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class FTUEEvent : MonoBehaviour
{
    protected string description;
    protected bool isCompleted;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool GetIsCompleted()
    {
        return isCompleted;
    }

    public string GetDescription()
    {
        return description;
    }

    public virtual void StartEvent()
    {
        StartCoroutine("SequenceEvent");
    }
}

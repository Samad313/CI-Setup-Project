using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class FTUEScheduler : MonoBehaviour
{
    [SerializeField]
    private FTUEEvent[] ftueEvents = default;

    [SerializeField]
    private bool startFromEndingAnimation = default;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("SequenceFTUEEvents");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SequenceFTUEEvents()
    {
        int i = 0;
        if(startFromEndingAnimation)
        {
            i = 9;
            FTUEHelper.instance.SetStuffForEndingAnimation();
        }
        for (; i < ftueEvents.Length; i++)
        {
            ftueEvents[i].StartEvent();
            while(ftueEvents[i].GetIsCompleted()==false)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public FTUEEvent GetFTUEEventAtIndex(int index)
    {
        return ftueEvents[index];
    }

    public int GetNumFTUEEvents()
    {
        return ftueEvents.Length;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FTUEScheduler), true)]
public class FTUESchedulerOnInspector : Editor
{
    FTUEScheduler ftueScheduler;

    void OnEnable()
    {
        ftueScheduler = serializedObject.targetObject as FTUEScheduler;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Story so far...");
        EditorGUILayout.Space();
        for (int i = 0; i < ftueScheduler.GetNumFTUEEvents(); i++)
        {
            EditorGUILayout.LabelField((i + 1) + ". " + ftueScheduler.GetFTUEEventAtIndex(i).GetDescription());
        }
    }
}
#endif

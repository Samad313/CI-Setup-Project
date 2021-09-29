using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    public static Cutscene instance;
    public delegate void CutSceneCalled();
    public event CutSceneCalled cutSceneCalled;
    private bool waitForCutScene = false;

    public bool WaitForCutScene
    {
        get { return waitForCutScene; }
    }


    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCutscene(Transform transformToFollow, float time = 10000.0f)
    {
        cutSceneCalled();
        GameplayManager.instance.StartCutscene(transformToFollow);
        StartCoroutine("CutSceneHandler", time);
        StartCoroutine("CutsceneSequence", time);
    }

    private IEnumerator CutSceneHandler(float time)
    {
        waitForCutScene = true;
        float waitTime = time * 0.5f;
        yield return new WaitForSeconds(waitTime);
        waitForCutScene = false;

    }

    private IEnumerator CutsceneSequence(float time)
    {
        yield return new WaitForSeconds(time);
        EndCutscene();
    }

    public void EndCutscene()
    {
        StopCoroutine("CutsceneSequence");
        GameplayManager.instance.StopCutscene();
    }
}

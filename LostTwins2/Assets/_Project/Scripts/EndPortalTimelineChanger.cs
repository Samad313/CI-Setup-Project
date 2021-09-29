using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class EndPortalTimelineChanger : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectsForMaterialChange = default;

    // Start is called before the first frame update
    void Start()
    {
        PlayableDirector playableDirector = GetComponent<PlayableDirector>();
        TimelineAsset timelineAsset = (TimelineAsset)playableDirector.playableAsset;

        foreach (PlayableBinding output in timelineAsset.outputs)
        {
            if (output.streamName == "GateGroupAnimationTrack")
            {
                GameplayManager.instance.FinalPortal.GetMainVisual().gameObject.AddComponent<Animator>();
                playableDirector.SetGenericBinding(output.sourceObject, GameplayManager.instance.FinalPortal.GetMainVisual().GetComponent<Animator>());
            }
            else if (output.streamName == "PhoenixActivationTrack")
            {
                playableDirector.SetGenericBinding(output.sourceObject, GameplayManager.instance.Phoenix.GetVisual().gameObject);
                Debug.Log(output.outputTargetType);
            }
        }

        for (int i = 0; i < objectsForMaterialChange.Length; i++)
        {
            objectsForMaterialChange[i].GetComponent<Renderer>().material.renderQueue = 3015;
        }

        //GetComponent<PlayableDirector>().time = 0.0f;
        //GetComponent<PlayableDirector>().Play();
        //StartCoroutine("PauseAfterSomeTime");
        //GetComponent<PlayableDirector>().Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        GetComponent<PlayableDirector>().timeUpdateMode = DirectorUpdateMode.GameTime;
        GetComponent<PlayableDirector>().Play();
        GetComponent<PlayableDirector>().time = 1.5f;
    }

    //private IEnumerator PauseAfterSomeTime()
    //{
    //    yield return new WaitForEndOfFrame();

    //    GetComponent<PlayableDirector>().Pause();
    //}
}

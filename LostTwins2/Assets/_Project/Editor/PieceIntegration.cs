using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;

public class PieceIntegration : ScriptableObject
{
    public static List<GameObject> lights = new List<GameObject>();

    [MenuItem("We.R.Play/Piece Integration/All Integration Steps For Piece", false, 40)]
    static void AllIntegrationStepsForPiece()
    {
        Step1SetParentAsNull();
        Step2RemoveColliders();
        Step3RemoveExtraScripts();
        Step4DisableLights();
        Step5SetMyLightmapGroup();
        Step6MarkStaticExceptBatching();
    }

    [MenuItem ("We.R.Play/Piece Integration/1. Make objects parent of the world", false, 40)]
    static void Step1SetParentAsNull()
    {
        Camera[] cameraObjects = Selection.activeGameObject.GetComponentsInChildren<Camera>();
        for (int i = 0; i < cameraObjects.Length; i++)
        {
            cameraObjects[i].gameObject.transform.parent = null;
#if UNITY_EDITOR
            EditorUtility.SetDirty(cameraObjects[i].gameObject);
#endif
        }

        PostProcessVolume[] ppsObjects = Selection.activeGameObject.GetComponentsInChildren<PostProcessVolume>();
        for (int i = 0; i < ppsObjects.Length; i++)
        {
            ppsObjects[i].gameObject.transform.parent = null;
#if UNITY_EDITOR
            EditorUtility.SetDirty(ppsObjects[i].gameObject);
#endif
        }
    }

    [MenuItem("We.R.Play/Piece Integration/2. Remove Colliders", false, 40)]
    static void Step2RemoveColliders()
    {
        Collider[] colliderObjects = Selection.activeGameObject.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliderObjects.Length; i++)
        {
            GameObject tempGO = colliderObjects[i].gameObject;
            DestroyImmediate(colliderObjects[i]);

#if UNITY_EDITOR
            EditorUtility.SetDirty(tempGO);
#endif
        }
    }

    [MenuItem("We.R.Play/Piece Integration/3. Remove Extra Scripts", false, 40)]
    static void Step3RemoveExtraScripts()
    {
        MoveOutClouds[] moveOutCloudsObjects = Selection.activeGameObject.GetComponentsInChildren<MoveOutClouds>();
        for (int i = 0; i < moveOutCloudsObjects.Length; i++)
        {
            GameObject tempGO = moveOutCloudsObjects[i].gameObject;
            DestroyImmediate(moveOutCloudsObjects[i]);
#if UNITY_EDITOR
            EditorUtility.SetDirty(tempGO);
#endif
        }
    }

    [MenuItem("We.R.Play/Piece Integration/4. Disable Lights", false, 40)]
    static void Step4DisableLights()
    {
        if (Selection.activeGameObject.GetComponent<BakedLightsBackup>())
        {
            DestroyImmediate(Selection.activeGameObject.GetComponent<BakedLightsBackup>());
        }

        Selection.activeGameObject.AddComponent<BakedLightsBackup>();

        Transform[] allObjects = Selection.activeGameObject.GetComponentsInChildren<Transform>();

        for (int i = 0; i < allObjects.Length; i++)
        {
            for (int j = 0; j < allObjects[i].childCount; j++)
            {

                if (allObjects[i].GetChild(j).gameObject.name.Contains("Lamp_ Light") || allObjects[i].GetChild(j).gameObject.name.Contains("Point Light"))
                {
                    if (allObjects[i].GetChild(j).gameObject.GetComponent<Light>().lightmapBakeType == LightmapBakeType.Baked)
                    {
                        Selection.activeGameObject.GetComponent<BakedLightsBackup>().SaveLight(allObjects[i].GetChild(j).gameObject);
                        allObjects[i].GetChild(j).gameObject.SetActive(false);
#if UNITY_EDITOR
                        EditorUtility.SetDirty(allObjects[i].GetChild(j).gameObject);
#endif
                    }

                }
            }
        }

        foreach (Transform child in Selection.activeGameObject.transform)
        {

            if(child.name.Contains("--ambience--"))
            {
                Selection.activeGameObject.GetComponent<BakedLightsBackup>().SaveLight(child.gameObject);
                child.gameObject.SetActive(false);
#if UNITY_EDITOR
                EditorUtility.SetDirty(child.gameObject);
#endif
            }

            if (child.name.Contains("--lighting--"))
            {
                Selection.activeGameObject.GetComponent<BakedLightsBackup>().SaveLight(child.gameObject);
                child.gameObject.SetActive(false);
#if UNITY_EDITOR
                EditorUtility.SetDirty(child.gameObject);
#endif
            }
  
        }

        Light[] lightObjects = Selection.activeGameObject.GetComponentsInChildren<Light>();
        for (int i = 0; i < lightObjects.Length; i++)
        {
            SerializedObject serialObj = new SerializedObject(lightObjects[i]);
            SerializedProperty lightmapProp = serialObj.FindProperty("m_Lightmapping");
            if(lightmapProp.intValue==2)            //Baked
            {
                lightObjects[i].gameObject.SetActive(false);
#if UNITY_EDITOR
                EditorUtility.SetDirty(lightObjects[i].gameObject);
#endif
            }
            else if (lightmapProp.intValue == 4)    //Realtime
            {
                if(lightObjects[i].transform.parent.name.Contains("lamp") || lightObjects[i].transform.parent.name.Contains("Lamp"))
                {
                    if(lightObjects[i].gameObject.activeSelf)
                    {
                        DisableRealtimeBackLight(lightObjects[i]);
                    }
                }
            }
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(Selection.activeGameObject);
#endif

    }

    private static void DisableRealtimeBackLight(Light light)
    {
        Light[] lights = light.transform.parent.GetComponentsInChildren<Light>();
        List<Light> realtimeLight = new List<Light>();
        for (int i = 0; i < lights.Length; i++)
        {
            SerializedObject serialObj = new SerializedObject(lights[i]);
            SerializedProperty lightmapProp = serialObj.FindProperty("m_Lightmapping");
            if (lightmapProp.intValue == 4)
            {
                realtimeLight.Add(lights[i]);
            }
        }

        for (int i = 0; i < realtimeLight.Count; i++)
        {
            if(realtimeLight[i].transform.position.z>0)
            {
                realtimeLight[i].gameObject.SetActive(false);
#if UNITY_EDITOR
                EditorUtility.SetDirty(realtimeLight[i].gameObject);
#endif
            }
        }

        if(realtimeLight.Count==2)
        {
            if(realtimeLight[0].transform.position.z> realtimeLight[1].transform.position.z)
            {
                realtimeLight[0].gameObject.SetActive(false);
#if UNITY_EDITOR
                EditorUtility.SetDirty(realtimeLight[0].gameObject);
#endif
            }
            else
            {
                realtimeLight[1].gameObject.SetActive(false);
#if UNITY_EDITOR
                EditorUtility.SetDirty(realtimeLight[1].gameObject);
#endif
            }
        }
    }

    [MenuItem("We.R.Play/Piece Integration/5. Set MyLightmapGroup script Inspector", false, 40)]
    static void Step5SetMyLightmapGroup()
    {
        if(Selection.activeGameObject.GetComponent<MyLightmapGroup>())
        {
            DestroyImmediate(Selection.activeGameObject.GetComponent<MyLightmapGroup>());
        }

        Selection.activeGameObject.AddComponent<MyLightmapGroup>();
        Selection.activeGameObject.GetComponent<MyLightmapGroup>().SaveLightmapSettings();
#if UNITY_EDITOR
        EditorUtility.SetDirty(Selection.activeGameObject);
#endif
    }

    [MenuItem("We.R.Play/Piece Integration/6. Mark all static except batching", false, 40)]
    static void Step6MarkStaticExceptBatching()
    {
        Transform[] allTransforms = Selection.activeGameObject.GetComponentsInChildren<Transform>(true);
        
        GameObjectUtility.SetStaticEditorFlags(Selection.activeGameObject, StaticEditorFlags.ContributeGI | StaticEditorFlags.ContributeGI | StaticEditorFlags.NavigationStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OffMeshLinkGeneration | StaticEditorFlags.ReflectionProbeStatic);
#if UNITY_EDITOR
        EditorUtility.SetDirty(Selection.activeGameObject);
#endif

        for (int i = 0; i < allTransforms.Length; i++)
        {
            GameObjectUtility.SetStaticEditorFlags(allTransforms[i].gameObject, StaticEditorFlags.ContributeGI | StaticEditorFlags.ContributeGI | StaticEditorFlags.NavigationStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OffMeshLinkGeneration | StaticEditorFlags.ReflectionProbeStatic);
#if UNITY_EDITOR
            EditorUtility.SetDirty(allTransforms[i].gameObject);
#endif
        }
    }

    [MenuItem("We.R.Play/Piece Integration/7. Enable Lights", false, 40)]
    static void EnableLights()
    {
        if(Selection.activeGameObject.GetComponent<BakedLightsBackup>())
        {
            Selection.activeGameObject.GetComponent<BakedLightsBackup>().EnableLights();
        }


#if UNITY_EDITOR
        EditorUtility.SetDirty(Selection.activeGameObject);
#endif
    }


}
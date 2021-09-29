using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using UnityEditor.SceneManagement;

[System.Serializable]
public class MyLightmapInfo
{
    public Renderer myRenderer;
    public Vector4 offsetScale;
    public int lightmapIndex;

    public MyLightmapInfo()
    {
        myRenderer = null;
        offsetScale = Vector4.zero;
        lightmapIndex = -1;
    }
}
public class MyLightmapGroup : MonoBehaviour
{
    public MyLightmapInfo[] lightmapInfos;
    public Texture2D[] lightmaps;
    //public LightingDataAsset lightingDataAsset;

    void Awake()
    {
        AddToMainScene();
    }

    public void SaveLightmapSettings()
    {
        //lightingDataAsset = Lightmapping.lightingDataAsset;
        LightmapData[] lightmaparray = LightmapSettings.lightmaps;
        lightmaps = new Texture2D[lightmaparray.Length];
        for (int i = 0; i < lightmaparray.Length; i++)
        {
            lightmaps[i] = lightmaparray[i].lightmapColor;
        }

        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        int numRenderers = renderers.Length;

        lightmapInfos = new MyLightmapInfo[numRenderers];
        for (int i = 0; i < numRenderers; i++)
        {
            lightmapInfos[i] = new MyLightmapInfo();
            lightmapInfos[i].myRenderer = renderers[i];
            lightmapInfos[i].offsetScale = renderers[i].lightmapScaleOffset;
            lightmapInfos[i].lightmapIndex = renderers[i].lightmapIndex;
            //EditorUtility.SetDirty(renderers[i].gameObject);
        }

        //EditorUtility.SetDirty(gameObject);
        //EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }

    public void PrintLightmapValues()
    {
        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            Debug.Log(renderers[i].lightmapScaleOffset.ToString("F8"));
            Debug.Log(renderers[i].lightmapIndex);
        }
    }

    public void AddToMainScene()
    {
        //if(Lightmapping.lightingDataAsset==null)
        //    Lightmapping.lightingDataAsset = lightingDataAsset;

        LightmapData[] lightmapArray = LightmapSettings.lightmaps;
        LightmapData[] newLightmapArray = new LightmapData[lightmapArray.Length + lightmaps.Length];

        for (int i = 0; i < lightmapArray.Length; i++)
        {
            newLightmapArray[i] = lightmapArray[i];
        }

        for (int i = 0; i < lightmaps.Length; i++)
        {
            newLightmapArray[i + lightmapArray.Length] = new LightmapData();
            newLightmapArray[i + lightmapArray.Length].lightmapColor = lightmaps[i];
        }
        //Undo.RecordObject((UnityEngine.Object) LightmapSettings.lightmaps, "ChangeLightmaps");
        LightmapSettings.lightmaps = newLightmapArray;

        Debug.Log(gameObject.name);
        for (int i = 0; i < lightmapInfos.Length; i++)
        {
            if (lightmapInfos[i].lightmapIndex < 0 || lightmapInfos[i].lightmapIndex > 65500)
                continue;

            //var so = new SerializedObject(lightmapInfos[i].myRenderer);
            //so.FindProperty("m_ReceiveShadows").boolValue = false;
            //Undo.RecordObject(lightmapInfos[i].myRenderer, "SetLightmapValues1");
            //Undo.RegisterCompleteObjectUndo(lightmapInfos[i].myRenderer, "SetLightmapValues2");
            lightmapInfos[i].myRenderer.lightmapIndex = lightmapInfos[i].lightmapIndex + lightmapArray.Length;
            lightmapInfos[i].myRenderer.lightmapScaleOffset = lightmapInfos[i].offsetScale;
            //so.FindProperty("m_LightmapIndex").intValue = lightmapInfos[i].lightmapIndex + lightmapArray.Length;
            //so.FindProperty("m_LightmapTilingOffset").vector4Value = lightmapInfos[i].offsetScale;
            
            //lightmapInfos[i].myRenderer.receiveShadows = false;
            //serializedObject.ApplyModifiedProperties();
            //lightmapInfos[i].myRenderer.transform.position = Vector3.one;
            //so.ApplyModifiedProperties();
            //EditorUtility.SetDirty(lightmapInfos[i].myRenderer);
            //EditorUtility.SetDirty(lightmapInfos[i].myRenderer.gameObject);
        }

        //EditorUtility.SetDirty(gameObject);
        //EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }
}

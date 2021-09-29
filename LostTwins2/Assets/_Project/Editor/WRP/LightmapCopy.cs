using UnityEditor;
using UnityEngine;

public class LightmapCopy : ScriptableObject
{
    [MenuItem ("We.R.Play/Lightmap Save", false, 40)]
    static void SaveLightmapSettings()
    {
        MyLightmapGroup myLightmapGroup = Selection.activeTransform.GetComponent<MyLightmapGroup>();
        myLightmapGroup.SaveLightmapSettings();
    }

    [MenuItem("We.R.Play/Lightmap AddToScene", false, 40)]
    static void AddLightmapSettingsToScene()
    {
        MyLightmapGroup myLightmapGroup = Selection.activeTransform.GetComponent<MyLightmapGroup>();
        myLightmapGroup.AddToMainScene();
    }
}
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MyLightmapGroup))]
public class LightmapLoader : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MyLightmapGroup lightmapData = (MyLightmapGroup)target;

        if (GUILayout.Button("AddToScene"))
        {
            lightmapData.AddToMainScene();
        }
    }
}
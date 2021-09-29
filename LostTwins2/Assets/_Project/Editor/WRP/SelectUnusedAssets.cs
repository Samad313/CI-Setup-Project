using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SelectUnusedAssets : ScriptableObject
{
    [MenuItem ("We.R.Play/Select Unused Assets")]
    static void SelectUnusedAssetsInProject()
    {
        int i = 0;
        string[] paths = new string[Selection.objects.Length];
        Object[] scenes = Selection.objects;
        for (i = 0; i < paths.Length; i++)
        {
            paths[i] = AssetDatabase.GetAssetPath(scenes[i].GetInstanceID());
        }

        string[] dependenciesPaths = AssetDatabase.GetDependencies(paths, true);
        string[] allAssetPaths = Directory.GetFiles(Application.dataPath, searchPattern:"*", searchOption: SearchOption.AllDirectories);
        List<Object> independantObjects = new List<Object>();
        for (i = 0; i < allAssetPaths.Length; i++)
        {
            allAssetPaths[i] = allAssetPaths[i].Replace(Application.dataPath, "Assets");
            if(IsNormalAsset(allAssetPaths[i]))
            {
                if(!DoesExistsInThis(allAssetPaths[i], dependenciesPaths))
                    independantObjects.Add((Object)AssetDatabase.LoadMainAssetAtPath(allAssetPaths[i]));
            }
        }

        Selection.objects = independantObjects.ToArray();
    }

    static bool IsNormalAsset(string assetPath)
    {
        if(assetPath.EndsWith(".meta")||assetPath.EndsWith(".cs")||assetPath.EndsWith(".js"))
            return false;

        if((Object)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object))==null)
            return false;

        return true;
    }

    static bool DoesExistsInThis(string text, string[] textArray)
    {
        for (int i = 0; i < textArray.Length; i++)
        {
            if(text==textArray[i])
                return true;
        }

        return false;
    }
}

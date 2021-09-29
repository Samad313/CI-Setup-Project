using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InvertPositionValues : ScriptableObject
{
    static float invertMultiplier = -1f;

    [MenuItem("We.R.Play/Invert Positions/1. Invert X position", false, 40)]
    static void InvertX()
    {
        if(Selection.activeGameObject != null)
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Vector3 objectlocalPosition = Selection.gameObjects[i].transform.localPosition;
                objectlocalPosition.x *= invertMultiplier;
                Selection.gameObjects[i].transform.localPosition = objectlocalPosition;
#if UNITY_EDITOR
                EditorUtility.SetDirty(Selection.gameObjects[i].gameObject);
#endif

            }
        }
    }

    [MenuItem("We.R.Play/Invert Positions/1. Invert Y position", false, 40)]
    static void InvertY()
    {
        if (Selection.activeGameObject != null)
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Vector3 objectlocalPosition = Selection.gameObjects[i].transform.localPosition;
                objectlocalPosition.y *= invertMultiplier;
                Selection.gameObjects[i].transform.localPosition = objectlocalPosition;
#if UNITY_EDITOR
                EditorUtility.SetDirty(Selection.gameObjects[i].gameObject);
#endif
            }
        }
    }


    [MenuItem("We.R.Play/Invert Positions/1. Invert Z position", false, 40)]
    static void InvertZ()
    {
        if (Selection.activeGameObject != null)
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Vector3 objectlocalPosition = Selection.gameObjects[i].transform.localPosition;
                objectlocalPosition.z *= invertMultiplier;
                Selection.gameObjects[i].transform.localPosition = objectlocalPosition;
#if UNITY_EDITOR
                EditorUtility.SetDirty(Selection.gameObjects[i].gameObject);
#endif
            }
        }
    }

}

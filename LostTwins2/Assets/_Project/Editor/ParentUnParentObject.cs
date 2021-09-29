using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ParentUnParentObject : ScriptableObject
{
    // shift u shortcut key
    [MenuItem("We.R.Play/Unparent #p")]
    static void UnParent()
    {
       
        if (Selection.activeGameObject != null && Selection.activeGameObject.transform.parent != null)
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Selection.gameObjects[i].transform.parent = null;
            }

            //Selection.activeGameObject.transform.parent = null;
        }
    }

    // shift p shortcut key
    [MenuItem("We.R.Play/Parent _p")]
    static void Parent()
    {

        if (Selection.activeGameObject != null)
        {
            GameObject parentSelected = Selection.activeGameObject;
            
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                if(!Selection.gameObjects[i].name.Equals(parentSelected.name))
                {
                    Selection.gameObjects[i].transform.parent = parentSelected.transform;
                }

            }

        }
    }
}

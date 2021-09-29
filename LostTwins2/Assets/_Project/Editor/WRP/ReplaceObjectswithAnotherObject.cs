﻿using UnityEngine;
using UnityEditor;

public class ReplaceObjectswithAnotherObject : ScriptableWizard
{
    public GameObject objectToReplaceWith;

    void OnWizardUpdate()
    {
        helpString = "Drag object to replace with";
        isValid = (objectToReplaceWith != null);// && Selection.transforms.Length != 0);

    }

    void OnWizardCreate()
    {
        //bool isPrefab = ( PrefabUtility.GetPrefabType(objectToReplaceWith)==PrefabType.Prefab || PrefabUtility.GetPrefabType(objectToReplaceWith)==PrefabType.ModelPrefab );
        bool isPrefab = !(PrefabUtility.GetPrefabAssetType(objectToReplaceWith) == PrefabAssetType.NotAPrefab);
        int i = 0;
        Transform[] allObjects = Selection.transforms;

        Transform tempTransform;
        string objectName = objectToReplaceWith.name;

        for(i = 0; i < allObjects.Length; i++)
        {
            if(isPrefab)
                tempTransform = ((GameObject)PrefabUtility.InstantiatePrefab(objectToReplaceWith)).transform;
            else
                tempTransform = Instantiate(objectToReplaceWith.transform);
            Undo.RegisterCreatedObjectUndo (tempTransform.gameObject, "Replace objects");
            tempTransform.name = objectName;
            tempTransform.parent = allObjects[i].parent;
            tempTransform.localPosition = allObjects[i].localPosition;
            tempTransform.localEulerAngles = allObjects[i].localEulerAngles;
            tempTransform.localScale = allObjects[i].localScale;

            Undo.DestroyObjectImmediate(allObjects[i].gameObject);
        }
    }

    [MenuItem ("We.R.Play/Replace Objects with Another Object")]
    static void ReplaceObjects()
    {
        ScriptableWizard.DisplayWizard("Replace Objects", typeof(ReplaceObjectswithAnotherObject), "Replace");
    }
}

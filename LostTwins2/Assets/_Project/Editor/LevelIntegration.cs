using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;

public class LevelIntegration : ScriptableObject
{
    [MenuItem("We.R.Play/Level Integration/All Integration Steps For Level", false, 40)]
    static void AllIntegrationStepsForLevel()
    {
        Step1RemovePreviousVisuals();
    }

    [MenuItem ("We.R.Play/Level Integration/1. Remove Previous Visuals", false, 40)]
    static void Step1RemovePreviousVisuals()
    {
        for (int i = 0; i < 3; i++)
        {
            Transform visualGroup = GameObject.Find("PiecesGroup/PieceGroup"+(i+1)+"/VisualGroup").transform;
            int childCount = visualGroup.childCount;
            for (int j = 0; j < childCount; j++)
            {
#if UNITY_EDITOR
                EditorUtility.SetDirty(visualGroup.GetChild(0).gameObject);
#endif
                DestroyImmediate(visualGroup.GetChild(0).gameObject);
            }
        }
    }

    [MenuItem("We.R.Play/Level Integration/2. Instantiate Visual Prefabs", false, 40)]
    static void Step2SpawnVisualPrefabs()
    {
        GameObject[] visualPrefabs = Selection.gameObjects;
        for (int i = 0; i < 3; i++)
        {
            Transform visualGroup = GameObject.Find("PiecesGroup/PieceGroup" + (i + 1) + "/VisualGroup").transform;

            Transform visualPiece = Instantiate(((GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Project/Prefabs/Pieces/fileName.prefab", typeof(GameObject))).transform);
            visualPiece.parent = visualGroup;
            visualPiece.localPosition = Vector3.zero;
            visualPiece.localRotation = Quaternion.identity;

        }
    }

    [MenuItem("We.R.Play/Level Integration/Paste Transform Horizontal &h")]
    static void PasteTransformHorizontal()
    {
        Undo.RecordObject(Selection.activeTransform, "Paste Transform Horizontal");

        string[] splittedString = ClipboardHelper.clipBoard.Split('\n');
        if (splittedString.Length < 4)
            return;

        GameObject[] selectedObjects = Selection.gameObjects;

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            Vector3 tempVector3 = StringToVector3(splittedString[1]);
            selectedObjects[i].transform.localPosition = new Vector3(tempVector3.x, selectedObjects[i].transform.localPosition.y, selectedObjects[i].transform.localPosition.z);
            tempVector3 = StringToVector3(splittedString[3]);
            selectedObjects[i].transform.localScale = new Vector3(tempVector3.x, selectedObjects[i].transform.localScale.y, selectedObjects[i].transform.localScale.z);
        }
    }

    [MenuItem("We.R.Play/Level Integration/Paste Transform Vertical &j")]
    static void PasteTransformVertical()
    {
        Undo.RecordObject(Selection.activeTransform, "Paste Transform Vertical");

        string[] splittedString = ClipboardHelper.clipBoard.Split('\n');
        if (splittedString.Length < 4)
            return;

        GameObject[] selectedObjects = Selection.gameObjects;

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            Vector3 tempVector3 = StringToVector3(splittedString[1]);
            selectedObjects[i].transform.localPosition = new Vector3(selectedObjects[i].transform.localPosition.x, tempVector3.y, selectedObjects[i].transform.localPosition.z);
            tempVector3 = StringToVector3(splittedString[3]);
            selectedObjects[i].transform.localScale = new Vector3(selectedObjects[i].transform.localScale.x, tempVector3.y, selectedObjects[i].transform.localScale.z);
        }
    }

    private static Vector3 StringToVector3(string inputString)
    {
        inputString = inputString.Replace("(", "");
        inputString = inputString.Replace(")", "");
        inputString = inputString.Replace(" ", "");
        inputString = inputString.Replace("f", "");
        string[] splittedString = inputString.Split(',');
        if (splittedString.Length < 3)
            return new Vector3(0, 0, 0);
        float x = float.Parse(splittedString[0]);
        float y = float.Parse(splittedString[1]);
        float z = float.Parse(splittedString[2]);
        return new Vector3(x, y, z);
    }
}
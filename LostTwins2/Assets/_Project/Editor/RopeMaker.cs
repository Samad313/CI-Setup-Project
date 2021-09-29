using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Rope))]
public class RopeMaker : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Rope rope = (Rope)target;

        if (GUILayout.Button("Generate Rope"))
        {
            rope.GenerateRope();
        }
    }
}

public class RopeReplacer : ScriptableObject
{
    [MenuItem("We.R.Play/Lost Twins 2/Replace all ropes", false, 40)]
    static void ReplaceAllRopes()
    {
        List<Transform> previousRopes = new List<Transform>();

        foreach (Transform child in GameObject.Find("ElementsGroup").transform)
        {
            if (child.name.Contains("Rope"))
                previousRopes.Add(child);
        }

        for (int i = 0; i < previousRopes.Count; i++)
        {
            GameObject newRope  = PrefabUtility.InstantiatePrefab( (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Project/Prefabs/Elements/Rope/Rope.prefab", typeof(GameObject)) ) as GameObject;
            newRope.transform.Find("Collider").GetComponent<Rope>().ReplaceRope(previousRopes[i]);
        }

    }
}
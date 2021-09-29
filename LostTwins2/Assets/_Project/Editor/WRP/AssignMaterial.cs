using UnityEngine;
using UnityEditor;

public class AssignMaterial : ScriptableWizard
{
    public Material theMaterial;

    void OnWizardUpdate()
    {
        helpString = "Select Game Obects";
        isValid = (theMaterial != null);
    }

    void OnWizardCreate()
    {
        GameObject[] gos = Selection.gameObjects;
        foreach(GameObject go in gos)
        {
            if(go.GetComponent<Renderer>())
            {
                Undo.RecordObject(go.GetComponent<Renderer>(), "Assign Material");
                go.GetComponent<Renderer>().material = theMaterial;
            }
        }
    }

    [MenuItem("We.R.Play/Assign Material", false, 1)]
    static void assignMaterial()
    {
        ScriptableWizard.DisplayWizard("Assign Material", typeof(AssignMaterial), "Assign");
    }
}

using UnityEngine;
using UnityEditor;

public class DuplicateWithOffset : ScriptableWizard
{
    public int numberOfDuplicates = 10;

    public bool translateRelative = true;
    public Vector3 translateOffset = Vector3.zero;
    public bool rotationRelative = true;
    public Vector3 rotationOffset = Vector3.zero;
    public bool scaleRelative = true;
    public Vector3 scaleOffset = Vector3.one;


    void OnWizardUpdate()
    {
        helpString = "Select a GameObject";
        isValid = true;
    }

    void OnWizardCreate()
    {
        if(Selection.gameObjects==null)
            return;

        if(Selection.gameObjects.Length<1)
            return;
        
        Transform originalTransform = Selection.activeGameObject.transform;
        Vector3 currentTranslation = originalTransform.localPosition;
        Vector3 currentRotation = originalTransform.localEulerAngles;
        Vector3 currentScale = originalTransform.localScale;

        if(translateRelative==false)
            currentTranslation = translateOffset;
        if(rotationRelative==false)
            currentRotation = rotationOffset;
        if(scaleRelative==false)
            currentScale = scaleOffset;

        Transform tempTransform;
        for (int i = 0; i < numberOfDuplicates; i++)
        {
            AddOffsets(ref currentTranslation, ref currentRotation, ref currentScale);
            tempTransform = Instantiate(originalTransform);
            tempTransform.parent = originalTransform.parent;
            tempTransform.localPosition = currentTranslation;
            tempTransform.localEulerAngles = currentRotation;
            tempTransform.localScale = currentScale;
            Undo.RegisterCreatedObjectUndo(tempTransform.gameObject, "Duplicate with Offset");
        }
    }

    [MenuItem("We.R.Play/Duplicate with Offset", false, 1)]
    static void DuplicateObjects()
    {
        ScriptableWizard.DisplayWizard("Duplicate with Offset", typeof(DuplicateWithOffset), "Duplicate");
    }

    private void AddOffsets(ref Vector3 currentTranslation, ref Vector3 currentRotation, ref Vector3 currentScale)
    {
        if(translateRelative)
            currentTranslation = currentTranslation + translateOffset;
        if(rotationRelative)
            currentRotation = currentRotation + rotationOffset;
        if(scaleRelative)
            currentScale = new Vector3( currentScale.x * scaleOffset.x, currentScale.y * scaleOffset.y, currentScale.z * scaleOffset.z);
    }
}

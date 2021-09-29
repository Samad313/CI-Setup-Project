using UnityEngine;
using UnityEditor;

public class CopyRagdoll : ScriptableWizard
{
    public Transform from;
	public Transform to;

	void OnWizardUpdate()
    {
        helpString = "Select Transforms";
        isValid = (from != null && to != null);
    }

    void OnWizardCreate()
    {
        Rigidbody[] allRigidbodies = from.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < allRigidbodies.Length; i++)
        {
            Transform transformWithSameNameInTo = FindDeepChild(to, allRigidbodies[i].gameObject.name);
            foreach (var component in allRigidbodies[i].transform.GetComponents<Component>())
            {
                var componentType = component.GetType();
                if (componentType == typeof(Rigidbody) ||
                    componentType == typeof(CharacterJoint) ||
                    componentType == typeof(BoxCollider) ||
                    componentType == typeof(CapsuleCollider) ||
                    componentType == typeof(SphereCollider)
                    )
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(component);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(transformWithSameNameInTo.gameObject);
                    if(componentType == typeof(CharacterJoint))
                    {
                        CharacterJoint characterJoint = transformWithSameNameInTo.GetComponent<CharacterJoint>();
                        string connecterName = characterJoint.connectedBody.gameObject.name;
                        Transform tempTansform = FindDeepChild(to, connecterName);
                        characterJoint.connectedBody = tempTansform.GetComponent<Rigidbody>();
                    }
                }
            }
        }
    }

    Transform FindDeepChild(Transform parentTransform, string inputName)
    {
        Transform[] children = parentTransform.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.name == inputName)
            {
                return child;
            }
        }

        return null;
    }

    [MenuItem("We.R.Play/Copy Ragdoll", false, 1)]
    static void CopyRagdollFromOneToAnother()
    {
        ScriptableWizard.DisplayWizard("Copy Ragdoll", typeof(CopyRagdoll), "Copy");
    }
}

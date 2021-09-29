using UnityEngine;
using UnityEditor;

public class ClipboardHelper
{
    public static string clipBoard
    {
        get 
        {
            return GUIUtility.systemCopyBuffer;
        }
        set
        {
            GUIUtility.systemCopyBuffer = value;
        }
    }
}

public class TransformCopier : ScriptableObject
{
    [MenuItem ("We.R.Play/Transform Copier/Copy Transform &c")]
    static void CopyTransform()
    {
		string finalString = Selection.activeTransform.name+"\n"+Vector3ToString(Selection.activeTransform.localPosition)+"\n"
			+Vector3ToString(Selection.activeTransform.localEulerAngles)+"\n"+Vector3ToString(Selection.activeTransform.localScale);
		ClipboardHelper.clipBoard = finalString;
    }

	[MenuItem ("We.R.Play/Transform Copier/Paste Transform &v")]
    static void PasteTransform()
    {
        Undo.RecordObject (Selection.activeTransform, "Paste Transform");

		string[] splittedString = ClipboardHelper.clipBoard.Split('\n');
		if(splittedString.Length<4)
			return;
		Selection.activeTransform.localPosition = StringToVector3(splittedString[1]);
		Selection.activeTransform.localRotation = Quaternion.Euler(StringToVector3(splittedString[2]));
		Selection.activeTransform.localScale = StringToVector3(splittedString[3]);
    }

	[MenuItem ("We.R.Play/Transform Copier/Copy Position &w")]
	static void CopyPosition()
	{
		string finalString = Vector3ToString(Selection.activeTransform.localPosition);
		ClipboardHelper.clipBoard = finalString;
	}

	[MenuItem ("We.R.Play/Transform Copier/Paste Position #&w")]
	static void PastePosition()
	{
		Undo.RecordObject (Selection.activeTransform, "Paste Position");
		Selection.activeTransform.localPosition = StringToVector3(ClipboardHelper.clipBoard);
	}

	[MenuItem ("We.R.Play/Transform Copier/Copy Rotation &e")]
	static void CopyRotation()
	{
		string finalString = Vector3ToString(Selection.activeTransform.localEulerAngles);
		ClipboardHelper.clipBoard = finalString;
	}
	
	[MenuItem ("We.R.Play/Transform Copier/Paste Rotation #&e")]
	static void PasteRotation()
	{
		Undo.RecordObject (Selection.activeTransform, "Paste Rotation");
		Selection.activeTransform.localRotation = Quaternion.Euler(StringToVector3(ClipboardHelper.clipBoard));
	}

	[MenuItem ("We.R.Play/Transform Copier/Copy Scale &r")]
	static void CopyScale()
	{
		string finalString = Vector3ToString(Selection.activeTransform.localScale);
		ClipboardHelper.clipBoard = finalString;
	}
	
	[MenuItem ("We.R.Play/Transform Copier/Paste Scale #&r")]
	static void PasteScale()
	{
		Undo.RecordObject (Selection.activeTransform, "Paste Scale");
		Selection.activeTransform.localScale = StringToVector3(ClipboardHelper.clipBoard);
	}
    


	private static Vector3 StringToVector3(string inputString)
	{
		inputString = inputString.Replace("(","");
		inputString = inputString.Replace(")","");
		inputString = inputString.Replace(" ","");
		inputString = inputString.Replace("f","");
		string[] splittedString = inputString.Split(',');
		if(splittedString.Length<3)
			return new Vector3(0, 0, 0);
		float x = float.Parse(splittedString[0]);
		float y = float.Parse(splittedString[1]);
		float z = float.Parse(splittedString[2]);
		return new Vector3(x, y, z);
	}

	private static string Vector3ToString(Vector3 inputVector)
	{
		string x = ""+inputVector.x;
		if(x.Contains("."))
			x = x + "f";
		string y = ""+inputVector.y;
		if(y.Contains("."))
			y = y + "f";
		string z = ""+inputVector.z;
		if(z.Contains("."))
			z = z + "f";

		return "("+x+", "+y+", "+z+")";
	}
}


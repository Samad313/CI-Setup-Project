using UnityEngine;
using UnityEditor;
using System.Globalization;

public class BulkRename : ScriptableWizard
{
    public string newName = "";
    public int startingNumber = 1;
    public string textToReplace = "";
    public string replacedText = "";
    public int appendType = 0;
    public bool makeCamelCase = false;

    void OnWizardUpdate()
    {
        helpString = "Select GameObects. Use # where you want the number to be";
        isValid = (Selection.gameObjects!=null && Selection.gameObjects.Length>=1);
    }

    protected override bool DrawWizardGUI()
    {
        this.position = new Rect(this.position.x, this.position.y, Mathf.Clamp(this.position.width, 400, 2000), Mathf.Clamp(this.position.height, 250, 2000));
        EditorGUILayout.BeginVertical();
        newName = EditorGUILayout.TextField("New Name", newName);
        int.TryParse(EditorGUILayout.TextField("Starting Number", ""+startingNumber), out startingNumber);
        string[] radioButtonsText = new string[3]{"No Append", "Append Before Name", "Append After Name"};
        appendType = GUILayout.SelectionGrid(appendType ,radioButtonsText, 3, EditorStyles.radioButton);

        if(appendType!=0)
        {
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Select additional changes to original text");
            makeCamelCase = EditorGUILayout.Toggle("Make CamelCase", makeCamelCase);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Replace", GUILayout.Width(this.position.width / 6.0f));
            textToReplace = EditorGUILayout.TextField(textToReplace, GUILayout.Width(this.position.width / 3.0f));
            EditorGUILayout.LabelField("with", GUILayout.Width(this.position.width / 12.0f));
            replacedText = EditorGUILayout.TextField(replacedText, GUILayout.Width(this.position.width / 3.0f));
            EditorGUILayout.EndHorizontal();


        }

        EditorGUILayout.EndVertical();
        return true;
    }

    void OnWizardCreate()
    {
        GameObject[] gos = Selection.gameObjects;
        string suffix = "";
        string postfix = "";
        bool shouldAddNumbering = newName.Contains("#");
        string[] brokenStringArray = newName.Split('#');
        suffix = brokenStringArray[0];
        if(brokenStringArray.Length>1)
            postfix= brokenStringArray[1];
        string currentNumber = "";
        bool shouldReplaceText = (textToReplace!="" && appendType!=0);

        gos = SortBasedOnSiblingIndex(gos);

        for (int i = 0; i < gos.Length; i++)
        {
            Undo.RecordObject(gos[i], "Bulk Rename");
            if(shouldReplaceText)
                gos[i].name = gos[i].name.Replace(textToReplace, replacedText);

            if(makeCamelCase)
                gos[i].name = GetCamelCase(gos[i].name);

            if(shouldAddNumbering)
                currentNumber = ""+(i+startingNumber);

            if(newName!="")
            {
                if(appendType==1)
                    gos[i].name = suffix+currentNumber+postfix+gos[i].name;
                else if(appendType==2)
                    gos[i].name = gos[i].name+suffix+currentNumber+postfix;
                else
                    gos[i].name = suffix+currentNumber+postfix;
            }
        }
    }

    [MenuItem("We.R.Play/Bulk Rename", false, 1)]
    static void RenameObjects()
    {
        ScriptableWizard.DisplayWizard("Bulk Rename", typeof(BulkRename), "Rename");
    }

    private string GetCamelCase(string originalName)
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        originalName = textInfo.ToLower(originalName);
        originalName = originalName.Replace("_", " ");
        originalName = originalName.Replace("-", " ");
        originalName = textInfo.ToTitleCase(originalName);
        originalName = originalName.Replace(" ", "");
        return originalName;
    }

    private GameObject[] SortBasedOnSiblingIndex(GameObject[] originalObjects)
    {
        GameObject tempGameObject = null;

        bool swapped = false;
        for (int i = 0; i < originalObjects.Length-1; i++)
        {
            swapped = false;
            for (int j = 0; j < originalObjects.Length-i-1; j++)
            {
                if (originalObjects[j].transform.GetSiblingIndex() > originalObjects[j+1].transform.GetSiblingIndex())
                {
                    tempGameObject = originalObjects[j];
                    originalObjects[j] = originalObjects[j+1];
                    originalObjects[j+1] = tempGameObject;
                    swapped = true;
                }
            }
            if (swapped == false)
                break;
        }

        return originalObjects;
    }
}

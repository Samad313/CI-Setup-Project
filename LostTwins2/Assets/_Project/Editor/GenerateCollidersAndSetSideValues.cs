using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class GenerateCollidersAndSetSideValues : ScriptableWizard
{

    void OnWizardUpdate()
    {
        helpString = "Drag object to replace with";
        isValid = (Selection.transforms.Length == 1 && Selection.activeTransform.name== "LevelEditorGroup");
    }

    void OnWizardCreate()
    {
        Transform levelEditorGroup = Selection.activeTransform;
        for (int i = 0; i < 3; i++)
        {
            GenerateForOnePiece(levelEditorGroup.Find("Piece"+(i+1)), GameObject.Find("PiecesGroup/PieceGroup"+(i+1)).transform);
        }

        
    }
    void GenerateForOnePiece(Transform editorPieceGroup, Transform pieceGroup)
    {
        Transform collTransform = ((GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Project/Prefabs/Collider.prefab", typeof(GameObject))).transform;

        bool[,] onValues = new bool[24, 15];
        int indexX = -1;
        int indexY = -1;
        //int onState = -1;
        foreach(Transform box in editorPieceGroup)
        {
            indexX = Mathf.RoundToInt(box.localPosition.x + 11.5f);
            indexY = Mathf.RoundToInt(-box.localPosition.y + 7.0f);
            onValues[indexX, indexY] = box.GetComponent<LevelEditorBox>().IsOnMaterial();
            //onValues[indexX, indexY] = (onState == 1);
        }

        Transform tempTransform = null;
        //if (pieceGroup.Find("CollidersGroup/PieceColliders"))
        //    DestroyImmediate(pieceGroup.Find("CollidersGroup/PieceColliders").gameObject);

        Transform collidersGroup = (new GameObject("PieceColliders")).transform;
        Undo.RegisterCreatedObjectUndo(collidersGroup.gameObject, "Generate Colliders Group");
        collidersGroup.parent = pieceGroup.Find("CollidersGroup");
        collidersGroup.localPosition = Vector3.zero;
        collidersGroup.localRotation = Quaternion.identity;
        collidersGroup.localScale = Vector3.one;

        for (int x = 0; x < 24; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                if(onValues[x,y])
                {
                    tempTransform = (Transform)PrefabUtility.InstantiatePrefab(collTransform);
                    Undo.RegisterCreatedObjectUndo(tempTransform.gameObject, "Generate Colliders");
                    tempTransform.parent = collidersGroup;
                    tempTransform.localPosition = new Vector3(x-11.5f, -y+7.0f, 0);
                    tempTransform.GetComponent<Renderer>().enabled = false;
                }
            }
        }

        Undo.RecordObject (pieceGroup, "Set Side Values");
        bool[] sideValues = new bool[15];
        for (int i = 0; i < 15; i++)
        {
            sideValues[i] = onValues[0, i];
        }
        pieceGroup.GetComponent<Piece>().SetLeftValues(sideValues);
        for (int i = 0; i < 15; i++)
        {
            sideValues[i] = onValues[23, i];
        }
        pieceGroup.GetComponent<Piece>().SetRightValues(sideValues);

        sideValues = new bool[24];
        for (int i = 0; i < 24; i++)
        {
            sideValues[i] = onValues[i, 0];
        }
        pieceGroup.GetComponent<Piece>().SetTopValues(sideValues);

        for (int i = 0; i < 24; i++)
        {
            sideValues[i] = onValues[i, 14];
        }
        pieceGroup.GetComponent<Piece>().SetBottomValues(sideValues);

        EditorUtility.SetDirty(pieceGroup);
        EditorSceneManager.MarkSceneDirty(pieceGroup.gameObject.scene);
    }

    [MenuItem ("We.R.Play/Lost Twins 2/Generate Colliders & Set Side Values")]
    static void GenerateCollidersAndSetValues()
    {
        ScriptableWizard.DisplayWizard("Generate Collider And Set Values", typeof(GenerateCollidersAndSetSideValues), "Generate");
    }
}

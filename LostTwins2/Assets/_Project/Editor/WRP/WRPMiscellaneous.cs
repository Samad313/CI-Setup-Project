using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditorInternal;

public class WRPMiscellaneous : ScriptableObject
{
    [MenuItem ("We.R.Play/Take Screenshot/Normal &g", false, 40)]
    static void TakeScreenshot()
    {
        CaptureScreenshot(1);
    }

    [MenuItem ("We.R.Play/Take Screenshot/Hi-Res &#g", false, 40)]
    static void TakeScreenshotHiRes()
    {
        CaptureScreenshot(3);
    }

    [MenuItem ("We.R.Play/Update UI", false, 40)]
    static void UpdateUI()
    {
        //LayoutBuilder.ForceRebuildLayoutImmediate();
        Canvas.ForceUpdateCanvases();
        SceneView.RepaintAll();
    }

    static void CaptureScreenshot(int resolution)
    {
        string screenshotFilename;
        int screenshotCount = 1;

        int i = 0;
        while(true)
        {
            screenshotFilename = "Screenshots/Screenshot_" + screenshotCount + ".png";
            if(System.IO.File.Exists(screenshotFilename))
                screenshotCount++;
            else
                break;
            i++;
            if(i>=9999)
            {
                Debug.LogWarning("Empty your folder. Screenshots limit reached");
                return;
            }
        }

        if(!Directory.Exists(Application.dataPath.Replace("Assets", "Screenshots")))
            Directory.CreateDirectory(Application.dataPath.Replace("Assets", "Screenshots"));

        #if UNITY_2017_2_OR_NEWER
        ScreenCapture.CaptureScreenshot(screenshotFilename, resolution);
        #else
        Application.CaptureScreenshot(screenshotFilename, resolution);
        #endif

        Debug.Log("Screenshot saved as : "+Application.dataPath.Replace("Assets", "")+screenshotFilename);

        EditorWindow previousWindow = EditorWindow.focusedWindow;

        System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
        System.Type type = assembly.GetType( "UnityEditor.GameView" );
        EditorWindow gameview = EditorWindow.GetWindow(type);
        gameview.Repaint();

        previousWindow.Focus();
    }

    [MenuItem("We.R.Play/OpenPlayerSettings &p")]
    static void OpenPlayerSettings()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
    }

    [MenuItem ("We.R.Play/Renderers/Hide")]
    static void DeactivateRenderers ()
    {
        GameObject[] gos = Selection.gameObjects;
        foreach (GameObject go in gos) 
        {
            if(go.GetComponent<Renderer>())
            {
                Undo.RecordObject (go.GetComponent<Renderer>(), "Hide Renderers");
                go.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    [MenuItem ("We.R.Play/Renderers/Show")]
    static void ActivateRenderers ()
    {
        GameObject[] gos = Selection.gameObjects;

        foreach (GameObject go in gos)
        {
            if(go.GetComponent<Renderer>())
            {
                Undo.RecordObject (go.GetComponent<Renderer>(), "Show Renderers");
                go.GetComponent<Renderer>().enabled = true;
            }
        }
    }

    [MenuItem ("We.R.Play/Renderers/Hide (Hierarchy)")]
    static void DeactivateRenderersInHierarchy ()
    {
        GameObject[] gos = Selection.gameObjects;
        List<Renderer> allRenderers = GetAllRenderers(gos);
        for (int i = 0; i < allRenderers.Count; i++)
        {
            Undo.RecordObject (allRenderers[i], "Hide Renderers");
            allRenderers[i].enabled = false;
        }
    }

    [MenuItem ("We.R.Play/Renderers/Show (Hierarchy)")]
    static void ActivateRenderersInHierarchy ()
    {
        GameObject[] gos = Selection.gameObjects;
        List<Renderer> allRenderers = GetAllRenderers(gos);
        for (int i = 0; i < allRenderers.Count; i++)
        {
            Undo.RecordObject (allRenderers[i], "Show Renderers");
            allRenderers[i].enabled = true;
        }
    }

    private static List<Renderer> GetAllRenderers(GameObject[] allGameObjects)
    {
        Renderer[] tempRenderers = null;
        List<Renderer> allRenderersList = new List<Renderer>();
        for (int i = 0; i < allGameObjects.Length; i++)
        {
            tempRenderers = null;
            tempRenderers = allGameObjects[i].GetComponentsInChildren<Renderer>();
            for (int j = 0; j < tempRenderers.Length; j++)
            {
                if(!allRenderersList.Contains(tempRenderers[j]))
                    allRenderersList.Add(tempRenderers[j]);
            }
        }

        return allRenderersList;
    }

    [MenuItem ("We.R.Play/Make Default Folders")]
    static void MakeDefaultFolders ()
    {
        string[] folderNames = new string[]{"Audio", "Extras", "Editor", "Meshes", "Plugins", "Prefabs", "Scenes", "Scripts", "Shaders", "Textures"};

        for (int i = 0; i < folderNames.Length; i++)
        {
            MakeFolder(folderNames[i]);
        }
    }

    static void MakeFolder(string folderName)
    {
        if(!AssetDatabase.IsValidFolder("Assets/" + folderName))
            AssetDatabase.CreateFolder("Assets", folderName);
    }

    [MenuItem("We.R.Play/Select Rigidbodies")]
    static void SelectRigidbodies()
    {
        GameObject[] gos = Selection.gameObjects;
        List<GameObject> allGameObjectsWithComponent = new List<GameObject>();
        for (int i = 0; i < gos.Length; i++)
        {
            Rigidbody[] rigidbodies = gos[i].GetComponentsInChildren<Rigidbody>();
            for (int j = 0; j < rigidbodies.Length; j++)
            {
                allGameObjectsWithComponent.Add(rigidbodies[j].gameObject);
            }
        }

        Selection.objects = allGameObjectsWithComponent.ToArray();
    }
}
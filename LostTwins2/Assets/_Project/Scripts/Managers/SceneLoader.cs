using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance = default;

    [SerializeField] private int firstLevelIndex = default;

    private int buildScenesCount = default;

    private void Awake()
    {
        Instance = this;
        buildScenesCount = SceneManager.sceneCountInBuildSettings;
        Debug.Log("<color=green> Debug:: Total Scenes In a Build: </color>" + buildScenesCount);
    }

    public void LoadNextSceneInBuild()
    {
        if (GameController.instance)
        {
            GameController.instance.LevelRestarted = false;
        }

        int currentLoadedScene = SceneManager.GetActiveScene().buildIndex;
        //Debug.Log("<color=green> Current Loaded Scene: </color>" + currentLoadedScene);
        if((currentLoadedScene + 1) >= buildScenesCount)
        {
            SceneManager.LoadScene(firstLevelIndex);
            return;
        }

        SceneManager.LoadScene(currentLoadedScene + 1);
    }

    public void ReloadScene()
    {
        if(GameController.instance)
        {
            GameController.instance.LevelRestarted = true;
        }

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void LoadSpecificScene(string sceneName)
    {
        Debug.Log("<color=green> Scene Name: </color>" + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    //public void UnloadScene(string oldSceneName)
    //{
    //    if (MenuController.instance)
    //    {
    //        FXManager.instance.SetCameraFade(1, MenuController.instance.UICamera.GetComponent<UICamera>().BlackBG);
    //    }

    //    Debug.Log("<color=green> Old Scene Name: </color>" + oldSceneName);

    //    StartCoroutine(_UnloadsceneAsync(oldSceneName));
    //}

    //IEnumerator _UnloadsceneAsync(string oldSceneName)
    //{
    //    AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(oldSceneName);

    //    while (asyncUnload != null && !asyncUnload.isDone)
    //    {
    //        yield return null;
    //    }

    //    if (asyncUnload.isDone)
    //    {
    //        Debug.Log(oldSceneName + " was unloaded asyncly!");

    //        if (GameController.instance && MenuController.instance)
    //        {
    //            GameController.instance.LoadMenu();
    //            FXManager.instance.FadeCamera(false, 0.3f, MenuController.instance.UICamera.GetComponent<UICamera>().BlackBG);
    //        }
    //    }
    //}
}

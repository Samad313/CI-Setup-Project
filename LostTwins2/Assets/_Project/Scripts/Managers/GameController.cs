using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    //Delegate which will call on every loadLevel.
    //public delegate void GameControllerInit();
    //public event GameControllerInit gameControllerInit;

    public static GameController instance;

    private int currentLevel = 1;
    private bool unlockNextLevelCheatIsUsed = default;

    [SerializeField]
    private int numLevels = 6;

    private bool levelRestarted = false;

    public bool LevelRestarted { get { return levelRestarted; } set { levelRestarted = value; } }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //if (!PlayerPrefs.HasKey(Constants.Prefs.FirstSession))
        //{
        //    MyAnalytics.CustomEvent(Constants.Analytics.FirstSession, new Dictionary<string, object>());
        //    PlayerPrefs.SetInt(Constants.Prefs.FirstSession, 1);
        //}
    }

    private void Start()
    {
        //if (PlayerStateManager.instance)
        //{
        //    if (!PlayerStateManager.instance.IsFTUEComplete && SceneLoader.Instance)
        //    {
        //        SceneLoader.Instance.LoadSpecificScene(Constants.Scenes.FTUE_Scene);

        //        if(DebugPanelManager.instance)
        //        {
        //            DebugPanelManager.instance.ShowOnlyGamePlayDebugItems();
        //        }
        //    }
        //}
        //LoadLevel(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    LoadLevel(true);
        //}
        if (GameplayManager.instance)
        {
            if (Input.GetKeyDown(KeyCode.N) && !unlockNextLevelCheatIsUsed)
            {
                Debug.Log("<color=green> Cheat Button Is Pressed </color>");
                GameplayManager.instance.CheatFinishLevel();
                unlockNextLevelCheatIsUsed = true;
            }

            if (Input.GetKeyUp(KeyCode.N))
            {
                Debug.Log("Key is up");
                unlockNextLevelCheatIsUsed = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadPreviousLevel();
        }
    }

    public void LoadFirstLevel()
    {
        levelRestarted = false;
        SceneManager.LoadScene(1);
    }

    public void LoadLevel(bool isNext)
    {
        levelRestarted = false;
        if (isNext)
            currentLevel++;

        if (currentLevel > numLevels)
        {
            currentLevel = 1;
        }

        SceneManager.LoadScene(currentLevel);
    }

    public void LoadPreviousLevel()
    {
        levelRestarted = false;

        currentLevel--;
        if (currentLevel < 1)
        {
            currentLevel = numLevels;
        }

        SceneManager.LoadScene(currentLevel);
    }

    public void LoadSpecificScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadLevel()
    {
        levelRestarted = true;
        SceneManager.LoadScene(currentLevel);
    }

    public void LoadMenu()
    {
        levelRestarted = false;
        //SceneManager.LoadScene(1);
        
        MenuController.instance.ClearMenusList();
        MenuController.instance.ShowMenu(MenuType.MainMenuScreen, "");

    }

    public void LoadNextLevelAsync(GameObject objectToRetain)
    {
        StartCoroutine("LoadYourAsyncScene", objectToRetain);
    }

    IEnumerator LoadYourAsyncScene(GameObject objectToRetain)
    {
        // Set the current Scene to be able to unload it later
        Scene currentScene = SceneManager.GetActiveScene();
        currentLevel++;

        if (currentLevel <= numLevels + 1)
        {
            // The Application loads the Scene in the background at the same time as the current Scene.
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentLevel, LoadSceneMode.Additive);

            // Wait until the last operation fully loads to return anything
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            //Move the GameObject(you attach this in the Inspector) to the newly loaded Scene
            //if (objectToRetain != null)
            //{
            //    SceneManager.MoveGameObjectToScene(objectToRetain, SceneManager.GetSceneByBuildIndex(currentLevel));
            //}
            
            // Unload the previous Scene
            SceneManager.UnloadSceneAsync(currentScene);

            //objectToRetain.GetComponent<TitleText>().SetPositionInNewScene(Camera.main.transform.position);
        }
        else
        {
            currentLevel = 2;
            LoadMenu();
        }
    }

    public bool GetLevelRestarted()
    {
        return levelRestarted;
    }

    public void SkipToNextLevel()
    {
        if (GameplayManager.instance)
        {
            GameplayManager.instance.CheatFinishLevel();
        }
    }
}

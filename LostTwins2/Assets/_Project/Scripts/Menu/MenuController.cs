using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;

    [SerializeField]
    List<BaseMenu> menusList = new List<BaseMenu>();

    List<BaseMenu> currentMenus = new List<BaseMenu>();

    [SerializeField]
    private float menuAnimationSpeed = 1.8f;

    public static float aspectRatio = 1.778f;
    public static float dpi = 105;
    public static bool notch = false;

    private bool isGameplayMenuActive = true;

    [SerializeField]
    private GameObject fps = default;

    [HideInInspector] public Camera UICamera;

    [SerializeField]
    private GameObject debugMenu;

    [SerializeField]
    private bool shouldEnableDebugMenu = false;

    public bool ShouldEnableDebugMenu { get { return shouldEnableDebugMenu; } }


    private void Awake()
    {
        for (int i = 0; i < menusList.Count; i++)
        {
            menusList[i].Init();
            menusList[i].Disable();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        aspectRatio = (Screen.height * 1.0f) / (Screen.width * 1.0f);
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        dpi = Screen.dpi;
#endif
        if (aspectRatio > 2.0f)
            notch = true;

        //for (int i = 0; i < menusList.Count; i++)
        //{
        //    menusList[i].Init();
        //    menusList[i].Disable();
        //}

        ShowMenu(MenuType.MainMenuScreen, "");
        UICamera = gameObject.GetComponent<Canvas>().worldCamera;

        if(shouldEnableDebugMenu)
        {
            debugMenu.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            isGameplayMenuActive = !isGameplayMenuActive;
            menusList[2].gameObject.SetActive(isGameplayMenuActive);
            fps.SetActive(isGameplayMenuActive);
        }
       
        if (Input.GetKeyDown(KeyCode.P) || Input.touchCount==3)
        {
            if( Input.GetTouch(0).phase == TouchPhase.Began)
                debugMenu.SetActive(!debugMenu.activeSelf);
        }
    }

    public float GetMenuAnimationSpeed()
    {
        return menuAnimationSpeed;
    }

    public void BackButtonCallback(string arguments)
    {
        EnablePreviousMenu();
        DisableMenu(currentMenus[currentMenus.Count - 1]);
        currentMenus.RemoveAt(currentMenus.Count - 1);
    }

    public bool IsOnTop(MenuType type)
    {
        if (currentMenus[currentMenus.Count - 1].Type == type)
            return true;

        return false;
    }

    public void ShowMenu(MenuType type, string arguments)
    {
        

        for (int i = 0; i < menusList.Count; i++)
        {
            if (menusList[i].Type == type)
            {
                if (menusList[i].IsPopup == false)
                {
                    DisablePreviousMenu();
                }
                if (type == MenuType.GameplayHUD)
                {
                    if(isGameplayMenuActive)
                        EnableMenu(menusList[i], arguments);
                }
                else
                {
                    EnableMenu(menusList[i], arguments);
                }
                
                currentMenus.Add(menusList[i]);
                break;
            }
        }
    }

    private void DisablePreviousMenu()
    {
        if (currentMenus.Count > 0)
        {
            DisableMenu(currentMenus[currentMenus.Count - 1]);
            currentMenus.RemoveAt(currentMenus.Count - 1);
        }
    }

    private void EnablePreviousMenu()
    {
        if (currentMenus.Count > 1)
        {
            EnableMenu(currentMenus[currentMenus.Count - 2], "");
        }
    }

    private void EnableMenu(BaseMenu inputMenu, string arguments)
    {
        if (!inputMenu.gameObject.activeSelf)
        {
            inputMenu.Enable(arguments);
            //AudioManager.instance.PlaySoundEffect("MenuSlide");
        }

        inputMenu.UpdateValues();
    }

    private void DisableMenu(BaseMenu inputMenu)
    {
        if (inputMenu.gameObject.activeSelf)
            inputMenu.Disable();
    }

    public void ClearMenusList()
    {
        for (int i = 0; i < currentMenus.Count; i++)
        {
            currentMenus[i].Disable();
        }
        currentMenus.Clear();
    }

    public void ShowGameplayHUD()
    {
        if(GetMenuReference(MenuType.GameplayHUD).GetComponent<GameplayScreen>().gameObject.activeInHierarchy)
        {
            GetMenuReference(MenuType.GameplayHUD).GetComponent<GameplayScreen>().AnimateElementsIn(0.6f, true);
        }
    }

    public void DisableGameplayHUD(bool isAnimate)
    {
        GetMenuReference(MenuType.GameplayHUD).GetComponent<GameplayScreen>().AnimateElementsOut(1f, isAnimate);
    }

    public BaseMenu GetMenuReference(MenuType givenMenuType)
    {
        foreach (BaseMenu baseMenu in menusList)
        {
            if (givenMenuType == baseMenu.Type)
            {
                return baseMenu;
            }
        }

        return null;
    }
}

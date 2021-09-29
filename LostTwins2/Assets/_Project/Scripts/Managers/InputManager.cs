using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//using UnityEngine.tvOS;

public class InputManager : MonoBehaviour//,  PlayerControls.IGamePlayActions
{
    public delegate void PlayButtonCallBack();
    public static event PlayButtonCallBack playButtonCallBack;

    public static bool left = false;
    public static bool right = false;
    public static bool jump = false;
    public static bool down = false;
    public static bool up = false;

    public static bool leftUp = false;
    public static bool rightUp = false;
    public static bool jumpUp = false;

    public static bool jumpDown = false;

    private static bool leftButton = false;
    private static bool rightButton = false;
    private static bool jumpButton = false;
    private static bool downButton = false;

    private static bool leftButtonUp = false;
    private static bool rightButtonUp = false;
    private static bool jumpButtonUp = false;

    private static bool jumpButtonDown = false;

    private static float leftFloat = 0;
    private static float rightFloat = 0;
    private static float jumpFloat = 0;
    private static float downFloat = 0;
    private static float upFloat = 0;

    private static bool cameraZoomButtonDown = false;
    public static bool cameraZoomDown = false;

    private static bool characterSwitchButtonDown = false;
    public static bool characterSwitchDown = false;

    private float clicked = 0;
    private float clicktime = 0;
    private float clickdelay = 0.5f;

    public static bool doubleClicked = false;
    public static bool doubleClickedTVOSButton = false;
    public static bool skipButtonDown = false;

    public static bool rightArrowPressed = false;
    public static bool leftArrowPressed = false;
    public static bool upArrowPressed = false;
    public static bool downArrowPressed = false;



    #region JoyStick
   

    #endregion

    #region MFI Controls
    public PlayerControls controls;
    private Vector2 move;

    #endregion

    #region Apple TV Controls
    private float horizontalAxisValue = 0f;
    private float verticalAxisValue = 0f;
    #endregion

    #region Mobile Controls
    private float joystickHorizontalAxis = 0.0f;
    private float joystickVerticalAxis = 0.0f;
    #endregion

    //private void OnEnable()
    //{
    //    controls.GamePlay.Enable();
    //}

    //private void OnDisable()
    //{
    //    controls.GamePlay.Disable();
    //}

    private void Awake()
    {
        //controls = new PlayerControls();
        //controls.GamePlay.SetCallbacks(this);
        //controls.GamePlay.Play.performed += ctx => ButtonPressedFromController(true);
      
    }

    // Start is called before the first frame update
    void Start()
    {
        AppleTVInitialization();
        StartCoroutine("ButtonPressedFromMouse");
    }

    private void AppleTVInitialization()
    {
#if !UNITY_EDITOR && UNITY_TVOS
            UnityEngine.tvOS.Remote.reportAbsoluteDpadValues = true;
            UnityEngine.tvOS.Remote.reportAbsoluteDpadValues = true;
            UnityEngine.tvOS.Remote.allowExitToHome = false;
#endif
    }

    private Vector2 GetJoystickDirectionValue()
    {
        if(JoystickController.instance)
            return JoystickController.instance.Joystick.Direction;
        return Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
            doubleClicked = CheckForDoubleClick();

        if(CameraZoomAndCharacterSwitchKeyForTVOS())
        {
            doubleClickedTVOSButton = CheckForDoubleClick();
        }

        skipButtonDown = doubleClicked || Input.GetKeyDown(KeyCode.Escape) || doubleClickedTVOSButton;
        horizontalAxisValue = Input.GetAxis("Horizontal");
        verticalAxisValue = Input.GetAxis("Vertical");
        if (leftButton || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || horizontalAxisValue < -0.9f || GetJoystickDirectionValue().x < 0f)
            leftFloat = 1.0f;

        if (rightButton || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || horizontalAxisValue > 0.9f || GetJoystickDirectionValue().x > 0f)
            rightFloat = 1.0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || verticalAxisValue > 0.9f || GetJoystickDirectionValue().y > 0f)
            upFloat = 1.0f;

        if (jumpButton || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || verticalAxisValue > 0.9f)
            jumpFloat = 1.0f;

        if (downButton || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || verticalAxisValue < -0.9f || GetJoystickDirectionValue().y < 0f)
            downFloat = 1.0f;

        left = leftFloat > 0.05f;
        right = rightFloat > 0.05f;
        jump = jumpFloat > 0.05f;
        down = downFloat > 0.05f;
        up = upFloat > 0.05f;

        leftUp = leftButtonUp || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow);
        rightUp = rightButtonUp || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow);
        jumpUp = jumpButtonUp || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow);
        jumpDown = jumpButtonDown || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || IsAppleTVJumpButton();

        cameraZoomDown = cameraZoomButtonDown || Input.GetKeyDown(KeyCode.Space) || doubleClickedTVOSButton;

        characterSwitchDown = characterSwitchButtonDown || Input.GetKeyDown(KeyCode.C) || CameraZoomAndCharacterSwitchKeyForTVOS();

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || horizontalAxisValue > 0.5f)
        {
            verticalAxisValue = 0f;
            rightArrowPressed = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || horizontalAxisValue < -0.5f)
        {
            verticalAxisValue = 0f;
            leftArrowPressed = true;
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || verticalAxisValue > 0.5f)
        {
            horizontalAxisValue = 0f;
            upArrowPressed = true;
        }

        else if( Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || verticalAxisValue < -0.5f)
        {
            horizontalAxisValue = 0f;
            downArrowPressed = true;
        }

        if(!Input.anyKey)
        {
            verticalAxisValue = 0f;
            horizontalAxisValue = 0f;
            rightArrowPressed = false;
            leftArrowPressed = false;
            upArrowPressed = false;
            downArrowPressed = false;
        }

        SetDirectionAccordingToJoystickInput();

    }

    private bool IsAppleTVJumpButton()
    {
        if (Input.GetKey(KeyCode.JoystickButton14))
        {
            return true;

        }

        return false;
    }

    private void SetDirectionAccordingToJoystickInput()
    {
        if(JoystickController.instance)
        {
            float xValue = JoystickController.instance.Joystick.Direction.x;
            float yValue = JoystickController.instance.Joystick.Direction.y;

            if(yValue >= 0.3f && yValue < 0.7f)
            {
                if(xValue > 0f)
                {
                    //right is true
                    rightFloat = 1f;
                }
                else
                {
                    leftFloat = 1f;
                    //left is true
                }
                upFloat = 1f;
                //up is true
            }
            if (yValue <= 1f && yValue >= 0.7f)
            {
                if (xValue > 0f)
                {
                    //right is false
                    rightFloat = 0f;
                }
                else
                {
                    //left is false
                    leftFloat = 0f;
                }
                upFloat = 1f;
                //up is true
            }
            if (yValue <= -0.3f && yValue > -0.7f)
            {
                if (xValue > 0f)
                {
                    //right is true
                    rightFloat = 1f;
                }
                else
                {
                    //left is true
                    leftFloat = 1f;
                }
                downFloat = 1f;
                //down is true
            }
            if (yValue >= -1f && yValue < -0.7f)
            {
                if (xValue > 0f)
                {
                    //right is false
                    rightFloat = 0f;
                }
                else
                {
                    //left is false
                    leftFloat = 0f;
                }
                downFloat = 1f;
                //down is true
            }




        }

    }

    public void ButtonPressedFromController(bool isMFIController = false)
    {
        if(isMFIController)
        {
            playButtonCallBack();
            return;
        }
        if (Input.GetKey(KeyCode.JoystickButton14))
        {
            //Joystick1Button14 is for Apple TV remote
            playButtonCallBack();
            return;
        }
    }

    private IEnumerator ButtonPressedFromMouse()
    {
        yield return new WaitUntil( ()=> Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.JoystickButton14));
        if(playButtonCallBack!=null)
        {
            playButtonCallBack();
        }
    }

    void LateUpdate()
    {
        leftFloat = Mathf.MoveTowards(leftFloat, 0.0f, Time.deltaTime * 25.0f);
        rightFloat = Mathf.MoveTowards(rightFloat, 0.0f, Time.deltaTime * 25.0f);
        jumpFloat = Mathf.MoveTowards(jumpFloat, 0.0f, Time.deltaTime * 25.0f);
        downFloat = Mathf.MoveTowards(downFloat, 0.0f, Time.deltaTime * 25.0f);
        upFloat = Mathf.MoveTowards(upFloat, 0.0f, Time.deltaTime * 25.0f);


        leftUp = false;
        rightUp = false;
        jumpUp = false;

        jumpDown = false;

        leftButton = false;
        rightButton = false;
        jumpButton = false;
        downButton = false;
     

        leftButtonUp = false;
        rightButtonUp = false;
        jumpButtonUp = false;

        jumpButtonDown = false;

        cameraZoomButtonDown = false;
        cameraZoomDown = false;

        characterSwitchButtonDown = false;
        characterSwitchDown = false;

        doubleClicked = false;
        doubleClickedTVOSButton = false;
        skipButtonDown = false;

    }

    //Repeat buttons

    public static void SetLeftButton()
    {
        leftButton = true;
    }

    public static void SetRightButton()
    {
        rightButton = true;
    }

    public static void SetJumpButton()
    {
        jumpButton = true;
    }

    public static void SetDownButton()
    {
        downButton = true;
    }

    //Up buttons

    public static void SetLeftButtonUp()
    {
        leftButtonUp = true;
    }

    public static void SetRightButtonUp()
    {
        rightButtonUp = true;
    }

    public static void SetJumpButtonUp()
    {
        jumpButtonUp = true;
    }

    //down buttons

    public static void SetJumpButtonDown()
    {
        jumpButtonDown = true;
    }

    public static void SetCameraZoomButtonDown()
    {
        cameraZoomButtonDown = true;
    }

    public static void SetCharacterSwitchButtonDown()
    {
        characterSwitchButtonDown = true;
    }

    public static void SetJoystickDisable()
    {
#if UNITY_IOS || UNITY_ANDROID
        if (JoystickController.instance)
            JoystickController.instance.Joystick.DisableJoystickInput();
#endif
    }
    public static void SetJoystickEnable()
    {
    #if UNITY_IOS || UNITY_ANDROID
        if (JoystickController.instance)
            JoystickController.instance.Joystick.EnableJoystickInput();
    #endif
    }

    public static void EnableJoystickForDebug()
    {
        if (JoystickController.instance)
            JoystickController.instance.Joystick.EnableJoystickInput();
    }

    private bool CameraZoomAndCharacterSwitchKeyForTVOS()
    {
#if UNITY_EDITOR
        return Input.GetKeyUp(KeyCode.Y);
#elif !UNITY_EDITOR
        return Input.GetKeyUp(KeyCode.JoystickButton15);
#endif
    }

    private bool CheckForDoubleClick()
    {

        if (Input.GetMouseButtonDown(0) == false && CameraZoomAndCharacterSwitchKeyForTVOS() == false)
        {
            return false;
        }
       
        if (Time.time - clicktime > 1)
        {
            clicked = 0;
        }
        clicked++;
        if (clicked == 1)
        {
            clicktime = Time.time;
        }

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            clicked = 0;
            clicktime = 0;
            return true;
        }
        else if (clicked > 2)
        {
            clicked = 0;
        }
            
        return false;

    }
    
    //public void OnPlay(InputAction.CallbackContext context)
    //{
    //    Debug.Log("On play");
    //}

    //public void OnMove(InputAction.CallbackContext context)
    //{
    //    Debug.Log("Moving right left");

    //    //float value;
    //    //value = context.action.ReadValue<float>();
    //    //Debug.Log("Value = " + value);
    //}

    //public void OnJump(InputAction.CallbackContext context)
    //{
    //    Debug.Log("Jumping");
    //    SetJumpButtonDown();
    //}

    //public void OnCharacterSwitch(InputAction.CallbackContext context)
    //{
    //    SetCharacterSwitchButtonDown();
    //    Debug.Log("Character switch");

    //}

    //public void OnMoveUpDown(InputAction.CallbackContext context)
    //{
    //    Debug.Log("Move up and down");

    //}

    //public void OnZoom(InputAction.CallbackContext context)
    //{
    //    Debug.Log("Zoom in out");
    //    SetCameraZoomButtonDown();

    //}
}

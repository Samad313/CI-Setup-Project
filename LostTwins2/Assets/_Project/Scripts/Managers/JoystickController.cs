using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickController : MonoBehaviour
{
    public static JoystickController instance;

    [SerializeField]
    private VariableJoystick variableJoystick;


    public VariableJoystick Joystick
    {
        get { return variableJoystick; }
    }

    private void Awake()
    {
//#if UNITY_IOS || UNITY_ANDROID
//if (!instance)
//            instance = this;
//#endif
        if (!instance)
            instance = this;

    }

    public void SnapX(bool value)
    {
        variableJoystick.SnapX = value;
    }

    public void SnapY(bool value)
    {
        variableJoystick.SnapY = value;
    }
}

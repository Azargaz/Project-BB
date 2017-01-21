using UnityEngine;
using System.Collections;

public class InputControl : MonoBehaviour
{
    //*********************//
    // Public member data  //
    //*********************//

    public Animator prompt;

    //*********************//
    // Private member data //
    //*********************//

    public enum InputState
    {
        MouseKeyboard,
        Controler
    };
    private static InputState curState = InputState.MouseKeyboard;

    //*************************//
    // Unity member methods    //
    //*************************//

    void OnGUI()
    {
        switch (curState)
        {
            case InputState.MouseKeyboard:
                if (isControlerInput())
                {
                    curState = InputState.Controler;
                    prompt.SetTrigger("usingGamepad");
                    Debug.Log("JoyStick being used");
                }
                break;
            case InputState.Controler:
                if (isMouseKeyboard())
                {
                    curState = InputState.MouseKeyboard;
                    Debug.Log("Mouse & Keyboard being used");
                }
                break;
        }
    }

    //***************************//
    // Public member methods     //
    //***************************//

    static InputState GetInputState()
    {
        return curState;
    }

    public static bool UsingGamepad()
    {
        if (GetInputState() == InputState.MouseKeyboard)
            return false;
        else
            return true;
    }

    //****************************//
    // Private member methods     //
    //****************************//

    private bool isMouseKeyboard()
    {
        // mouse & keyboard buttons
        if (Event.current.isKey ||
            Event.current.isMouse)
        {
            return true;
        }
        // mouse movement
        if (Input.GetAxis("Mouse X") != 0.0f ||
            Input.GetAxis("Mouse Y") != 0.0f)
        {
            return true;
        }
        return false;
    }

    private bool isControlerInput()
    {
        // joystick buttons
        if (Input.GetKey(KeyCode.Joystick1Button0) ||
           Input.GetKey(KeyCode.Joystick1Button1) ||
           Input.GetKey(KeyCode.Joystick1Button2) ||
           Input.GetKey(KeyCode.Joystick1Button3) ||
           Input.GetKey(KeyCode.Joystick1Button4) ||
           Input.GetKey(KeyCode.Joystick1Button5) ||
           Input.GetKey(KeyCode.Joystick1Button6) ||
           Input.GetKey(KeyCode.Joystick1Button7) ||
           Input.GetKey(KeyCode.Joystick1Button8) ||
           Input.GetKey(KeyCode.Joystick1Button9) ||
           Input.GetKey(KeyCode.Joystick1Button10) ||
           Input.GetKey(KeyCode.Joystick1Button11) ||
           Input.GetKey(KeyCode.Joystick1Button12) ||
           Input.GetKey(KeyCode.Joystick1Button13) ||
           Input.GetKey(KeyCode.Joystick1Button14) ||
           Input.GetKey(KeyCode.Joystick1Button15) ||
           Input.GetKey(KeyCode.Joystick1Button16) ||
           Input.GetKey(KeyCode.Joystick1Button17) ||
           Input.GetKey(KeyCode.Joystick1Button18) ||
           Input.GetKey(KeyCode.Joystick1Button19))
        {
            return true;
        }

        // joystick axis
        if (Input.GetAxis("XC Left Stick X") != 0.0f ||
           Input.GetAxis("XC Left Stick Y") != 0.0f ||
           Input.GetAxis("XC Triggers") != 0.0f ||
           Input.GetAxis("XC Right Stick X") != 0.0f ||
           Input.GetAxis("XC Right Stick Y") != 0.0f)
        {
            return true;
        }

        return false;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;

[System.Serializable]
public class MyButtonEvent : UnityEvent<InputDevice, float, Vector2>
{
}

[System.Serializable]
public class ButtonMapping
{
    // For the full list: https://docs.unity3d.com/Manual/xr_input.html
    static readonly Dictionary<string, InputFeatureUsage<bool>> availableButtons = new Dictionary<string, InputFeatureUsage<bool>>
    {
        {"indexTriggerButton", CommonUsages.triggerButton},     // Oculus: Index trigger - press
        {"joystickButton", CommonUsages.primary2DAxisClick},    // Oculus: Joystick - press
        {"menuButton", CommonUsages.menuButton },               // Oculus: Start - press (only left controller)
        {"gripTriggerButton", CommonUsages.gripButton },        // Oculus: grip trigger - press
        {"secondaryButton", CommonUsages.secondaryButton },     // Oculus: Y/B - Press
        {"primaryButton", CommonUsages.primaryButton },         // Oculus: X/A - Press
    };
    static readonly Dictionary<string, InputFeatureUsage<Vector2>> available2DSAxes = new Dictionary<string, InputFeatureUsage<Vector2>>
    {
        {"joystickValue", CommonUsages.primary2DAxis},  // Oculus: Joystick - push direction
    };
    static readonly Dictionary<string, InputFeatureUsage<float>> available1DAxes = new Dictionary<string, InputFeatureUsage<float>>
    {
        {"indexTriggerValue", CommonUsages.trigger},    // Oculus Index Trigger
        {"gripTriggerValue", CommonUsages.grip},        // Oculus Grip Trigger
    };

    public enum ButtonOption {
        indexTriggerButton,
        indexTriggerValue,
        menuButton,
        gripTriggerButton,
        gripTriggerValue,
        secondaryButton,
        primaryButton,
        joystickButton,
        joystickValue,
    };

    [Tooltip("The deadzone limit - how far should the input be pressed/pushed?")]
    public float deadZoneLimit = 0.1f;

    private InputFeatureUsage<bool> m_inputButtonUsage;
    private InputFeatureUsage<Vector2> m_inputJoystickUsage;
    private bool m_isButton;
    private string m_featureLabel;

    [SerializeField] [Tooltip("Select the button")]
    private ButtonOption m_button;
    
    // NOT SERIALIZED
    private float m_timePressed, m_timeHeld;
    // NOT SERIALIZED
    private bool registered = false;

    [Tooltip("When the input is first pressed, pushed, etc.")]
    // InputDevice = left or right controller,
    // floats = intermediant values
    //  for buttons: 1st = press state, 2nd = null
    //  for joystick: 1st = magnitude, 2nd = angle
    // vector2 = value (usually only for joystick)
    public MyButtonEvent InputActivated;
    [Tooltip("While the input is active, being pushed, etc.")]
    public MyButtonEvent InputActive;
    [Tooltip("When the input is let go, no longer pushed, etc.")]
    public MyButtonEvent InputDeactivated;
    
    // Constructor
    public void Init() {
        // get label selected by the user
        m_featureLabel = Enum.GetName(typeof(ButtonOption), m_button);
        // find dictionary entry, depending on the nature of the button
        switch(m_button) {
            case(ButtonOption.joystickValue):
                available2DSAxes.TryGetValue(m_featureLabel, out m_inputJoystickUsage);
                m_isButton = false;
                break;
            default:
                availableButtons.TryGetValue(m_featureLabel, out m_inputButtonUsage);
                m_isButton = true;
                break;
        }
        return;
    }

    public void CheckStatus(InputDevice device, bool debugMode) {
        if (m_isButton) CheckPressed(device, debugMode);
        else CheckJoystick(device, debugMode);
    }
    
    // Called when value needs to be updated
    public void CheckPressed(InputDevice device, bool debugMode) {
        // Useless value, just for this scope
        bool discardedValue;
        // We actually toggle between 0, 1, and -1
        // 0 = no action this update, 1 = pressed down this frame, -1 = released this frame

        if (!device.TryGetFeatureValue(m_inputButtonUsage, out discardedValue)) {
             if (debugMode) DebugLogger.current.AddLine("Error with getting thumb position: " + m_featureLabel);
            // End early
            return;
        }

        if (discardedValue) {
            if (!registered) {
                m_timePressed = Time.time;
                registered = discardedValue;
                InputActivated?.Invoke(device, 1f, Vector2.zero);
                if (debugMode) DebugLogger.current.AddLine(m_featureLabel + " Down");
            } else {
                m_timeHeld = Time.time - m_timePressed;
                InputActive?.Invoke(device, 0f, Vector2.zero);
            }
        } 
        else if (discardedValue != registered) {
            m_timePressed = 0f;
            registered = discardedValue;
            InputDeactivated?.Invoke(device, -1f, Vector2.zero);
            if (debugMode) DebugLogger.current.AddLine(Enum.GetName(typeof(ButtonOption), m_button) + " Up");
        }
    }

    public void CheckJoystick(InputDevice device, bool debugMode) {
        Vector2 thumbPosition = Vector2.zero;

        if (!device.TryGetFeatureValue(m_inputJoystickUsage, out thumbPosition)) {
            if (debugMode) DebugLogger.current.AddLine("Error with getting thumb position: " + m_featureLabel);
            // End early
            return;
        }

        thumbPosition = Vector2.ClampMagnitude(thumbPosition,1f);
        if (debugMode) DebugLogger.current.AddLine(m_featureLabel + " Down");
        //angle = CommonFunctions.GetAngleFromVector2(thumbPosition);

        if (thumbPosition.magnitude >= deadZoneLimit) {
            if (!registered) {
                m_timePressed = Time.time;
                registered = true;
                InputActivated?.Invoke(device, 0f, thumbPosition);
                if (debugMode) DebugLogger.current.AddLine(m_featureLabel + " Down");
            } else {
                m_timeHeld = Time.time - m_timePressed;
                InputActive?.Invoke(device, 0f, thumbPosition);
            }
        } 
        else if (registered) {
            m_timePressed = 0f;
            registered = false;
            InputDeactivated?.Invoke(device, 0f, thumbPosition);
            if (debugMode) DebugLogger.current.AddLine(Enum.GetName(typeof(ButtonOption), m_button) + " Up");
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class XRController : MonoBehaviour
{

    // List of Inputs: https://docs.unity3d.com/2019.1/Documentation/Manual/xr_input.html
    public static XRController current;

    // An XR experience only has a left or right hand. Never more...
    [SerializeField] [Tooltip("Reference to Left and Right Hand")]
    private XRHand m_leftHand = null, m_rightHand = null;

    [SerializeField] [Tooltip("Should this start on awake?")]
    private bool m_runOnStart = true;
    // NOT SERIALIZED
    [Tooltip("tells the script if it should run its update or not - only runs when Init() has been called")]
    private bool m_initialized = false;
    
    [SerializeField] [Tooltip("Bool for debugger")]
    private bool m_debugMode = false;

    private void Awake() {
        current = this;
        // get label selected by the user
        //string featureLabel = Enum.GetName(typeof(ButtonOption), button);
 
        // find dictionary entry
        //availableButtons.TryGetValue(featureLabel, out inputFeature);
           
        // init list
        //inputDevices = new List<InputDevice>();
    }

    private void Start() {
        if (m_runOnStart) Init();
    }

    public void Init()
    {
        // Depending on m_debugMode, we either turn on or off our debugger
        DebugLogger.current.SetStatus(m_debugMode);
        // Start cycle for checking for controllers
        StartCoroutine(CheckForControllers());
        // Ensure that initialization has been recorded
        m_initialized = true;

        // End
        return;
    }

    private IEnumerator CheckForControllers() {
        // Will be thrown out later
        List<InputDevice> toDiscard;
        while(true) {
            yield return null;
            // Get Left Hand Data, if a reference to it was found
            if (m_leftHand != null) {
                toDiscard = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, toDiscard);
                switch(toDiscard.Count) {
                    case(1):
                        if (m_leftHand.XRdevice == null || m_leftHand.XRdevice != toDiscard[0]) {
                            m_leftHand.Init(toDiscard[0], InputDeviceCharacteristics.Left, m_debugMode);
                            if (m_debugMode) DebugLogger.current.AddLine(string.Format("Device name '{0}' with role '{1}'", toDiscard[0].name, toDiscard[0].characteristics.ToString()));
                        }
                        break;
                    case(0):
                        if (m_debugMode) DebugLogger.current.AddLine("No left hand found");
                        break;
                    default:
                        if (m_debugMode) DebugLogger.current.AddLine("Found more than one left hand!");
                        break;
                }
            }

            // Get Right Hand Data
            if (m_rightHand != null) {
                toDiscard = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, toDiscard);
                switch(toDiscard.Count) {
                    case(1):
                        if (m_rightHand.XRdevice == null || m_rightHand.XRdevice != toDiscard[0]) {
                            m_rightHand.Init(toDiscard[0], InputDeviceCharacteristics.Right, m_debugMode);
                            if (m_debugMode) DebugLogger.current.AddLine(string.Format("Device name '{0}' with role '{1}'", toDiscard[0].name, toDiscard[0].characteristics.ToString()));
                        }
                        break;
                    case(0):
                        if (m_debugMode) DebugLogger.current.AddLine("No right hand found");
                        break;
                    default:
                        if (m_debugMode) DebugLogger.current.AddLine("Found more than one right hand!");
                        break;
                }
            }
        }
    }

    private void Update() {
        // This won't run if we haven't initialized yet
        if (!m_initialized) return;
        if (m_leftHand != null) m_leftHand.CheckInputs();
        if (m_rightHand != null) m_rightHand.CheckInputs();
        UpdateEvents();
    }

    private void UpdateEvents() {
        if (m_leftHand != null) m_leftHand.UpdateInputs();
        if (m_rightHand != null) m_rightHand.UpdateInputs();
    }

    // Events for grip
    public event Action<InputDevice> onLeftGripDown, onLeftGripUp, onRightGripDown, onRightGripUp, onGripDown, onGripUp;
    public void LeftGripDown(InputDevice device) {
        onLeftGripDown?.Invoke(device);
    }
    public void LeftGripUp(InputDevice device) {
        onLeftGripUp?.Invoke(device);
    }
    public void RightGripDown(InputDevice device) {
        onRightGripDown?.Invoke(device);
    }
    public void RightGripUp(InputDevice device) {
        onRightGripUp?.Invoke(device);
    }
    public void GripDown(InputDevice device) {
        if (m_debugMode) DebugLogger.current.AddLine("Grip Down Registered");
        onGripDown?.Invoke(device);
    }
    public void GripUp(InputDevice device) {
        if (m_debugMode) DebugLogger.current.AddLine("Grip Up Registered");
        onGripUp?.Invoke(device);
    }

    public event Action<InputDevice> onLeftTriggerDown, onLeftTriggerUp, onRightTriggerDown, onRightTriggerUp, onTriggerDown, onTriggerUp;
    public void LeftTriggerDown(InputDevice device) {
        onLeftTriggerDown?.Invoke(device);
    }
    public void LeftTriggerUp(InputDevice device) {
        onLeftTriggerUp?.Invoke(device);
    }
    public void RightTriggerDown(InputDevice device) {
        onRightTriggerDown?.Invoke(device);
    }
    public void RightTriggerUp(InputDevice device) {
        onRightTriggerUp?.Invoke(device);
    }
    public void TriggerDown(InputDevice device) {
        if (m_debugMode) DebugLogger.current.AddLine("Trigger Down Registered");
        onTriggerDown?.Invoke(device);
    }
    public void TriggerUp(InputDevice device) {
        if (m_debugMode) DebugLogger.current.AddLine("Trigger Up Registered");
        onTriggerUp?.Invoke(device);
    }

    public event Action<InputDevice> onLeftOneDown, onLeftOneUp, onRightOneDown, onRightOneUp, onOneDown, onOneUp;
    public void LeftOneDown(InputDevice device) {
        onLeftOneDown?.Invoke(device);
    }
    public void LeftOneUp(InputDevice device) {
        onLeftOneUp?.Invoke(device);
    }
    public void RightOneDown(InputDevice device) {
        onRightOneDown?.Invoke(device);
    }
    public void RightOneUp(InputDevice device) {
        onRightOneUp?.Invoke(device);
    }
    public void OneDown(InputDevice device) {
        if (m_debugMode) DebugLogger.current.AddLine("One Down Registered");
        onOneDown?.Invoke(device);
    }
    public void OneUp(InputDevice device) {
        if (m_debugMode) DebugLogger.current.AddLine("One Up Registered");
        onOneUp?.Invoke(device);
    }

    public event Action<InputDevice> onLeftTwoDown, onLeftTwoUp, onRightTwoDown, onRightTwoUp, onTwoDown, onTwoUp;
    public void LeftTwoDown(InputDevice device) {
        onLeftTwoDown?.Invoke(device);
    }
    public void LeftTwoUp(InputDevice device) {
        onLeftTwoUp?.Invoke(device);
    }
    public void RightTwoDown(InputDevice device) {
        onRightTwoDown?.Invoke(device);
    }
    public void RightTwoUp(InputDevice device) {
        onRightTwoUp?.Invoke(device);
    }
    public void TwoDown(InputDevice device) {
        if (m_debugMode) DebugLogger.current.AddLine("Two Down Registered");
        onTwoDown?.Invoke(device);
    }
    public void TwoUp(InputDevice device) {
        if (m_debugMode) DebugLogger.current.AddLine("Two Up Registered");
        onTwoUp?.Invoke(device);
    }

    public event Action<InputDevice> onLeftThumbPress, onLeftThumbRelease, onRightThumbPress, onRightThumbRelease, onThumbPress, onThumbRelease;
    public void LeftThumbPress(InputDevice device) {
        onLeftThumbPress?.Invoke(device);
    }
    public void LeftThumbRelease(InputDevice device) {
        onLeftThumbRelease?.Invoke(device);
    }
    public void RightThumbPress(InputDevice device) {
        onRightThumbPress?.Invoke(device);
    }
    public void RightThumbRelease(InputDevice device) {
        onRightThumbRelease?.Invoke(device);
    }
    public void ThumbPress(InputDevice device) {
        if (m_debugMode) DebugLogger.current.AddLine("Thumb Press Registered");
        onThumbPress?.Invoke(device);
    }
    public void ThumbRelease(InputDevice device) {
        if (m_debugMode) DebugLogger.current.AddLine("Thumb Release Registered");
        onThumbRelease?.Invoke(device);
    }

    public event Action onStartDown, onStartUp;
    public void StartDown() {
        if (m_debugMode) DebugLogger.current.AddLine("Start Down Registered");
        onStartDown?.Invoke();
    }
    public void StartUp() {
        if (m_debugMode) DebugLogger.current.AddLine("Start Up Registered");
        onStartUp?.Invoke();
    }

    public event Action<InputDevice, Vector2, float, float> onLeftThumbDirection, onRightThumbDirection, onThumbDirection;
    public void LeftThumbDirection(InputDevice device, Vector2 pos, float distance, float angle) {
        onLeftThumbDirection?.Invoke(device, pos, distance, angle);
    }
    public void RightThumbDirection(InputDevice device, Vector2 pos, float distance, float angle) {
        onRightThumbDirection?.Invoke(device, pos, distance, angle);
    }
    public void ThumbDirection(InputDevice device, Vector2 pos, float distance, float angle) {
        onThumbDirection?.Invoke(device, pos, distance, angle);
    }

    /*
    public event Action<Collider, GameObject> onCollisionEnter, onCollisionExit, onTriggerEnter, onTriggerExit;
    public void CollisionEnter(Collider collidedWith, GameObject obj) {
        onCollisionEnter?.Invoke(collidedWith, obj);
    }
    public void CollisionExit(Collider collidedWith, GameObject obj) {
        onCollisionExit?.Invoke(collidedWith, obj);
    }
    public void TriggerEnter(Collider trigger, GameObject obj) {
        onTriggerEnter?.Invoke(trigger, obj);
    }
    public void TriggerExit(Collider trigger, GameObject obj) {
        onTriggerExit?.Invoke(trigger, obj);
    }
    */
}

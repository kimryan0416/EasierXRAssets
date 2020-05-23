using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRHand : MonoBehaviour
{   

    static readonly Dictionary<string, InputDeviceCharacteristics> availableCharacteristics = new Dictionary<string, InputDeviceCharacteristics>
    {
        {"leftController", InputDeviceCharacteristics.Left},
        {"rightController", InputDeviceCharacteristics.Right},
    };
    private enum ControllerCharacteristic {
        leftController,
        rightController
    };
    [SerializeField]
    private ControllerCharacteristic m_controllerCharacteristicChoice;

    // NOT SERIALIZED [Tooltip("What Input Device matches with the inputCharacteristics defined - retrieved from Init")]
    private InputDevice m_XRdevice;
    public InputDevice XRdevice {
        get {   return m_XRdevice;  }
        set {}
    }
    // NOT SERIALIZED [Tooltip("Storing characteristics")]
    private InputDeviceCharacteristics m_inputCharacteristics;
    public InputDeviceCharacteristics inputCharacteristics {
        get {   return m_inputCharacteristics;  }
        set {}
    }

    // NOT SERIALIZED [Tooltip("Check to make sure that we're actually instantiating")]
    private bool m_initialized = false;
    public bool initialized {
        get {   return m_initialized;   }
        set {}
    }

    [SerializeField] [Tooltip("All button mappings")]
    private List<ButtonMapping> m_inputs = new List<ButtonMapping>();

    [SerializeField] [Tooltip("Debug Mode toggle")]
    private bool m_debugMode = false;

    private void Start() {
        // Ensure that our debug logger actually exists and if not do not debug
        if (DebugLogger.current == null) m_debugMode = false;

        // Update our m_inputCharacteristics
        string characteristicLabel = Enum.GetName(typeof(ControllerCharacteristic), m_controllerCharacteristicChoice);
        if (!availableCharacteristics.TryGetValue(characteristicLabel, out m_inputCharacteristics)) {
            if (m_debugMode) DebugLogger.current.AddLine("Error finding devices w/ characteristics " + characteristicLabel);
            return;
        } 

        // Update our inputs
        ResetInputs();

        // Start our coroutine
        StartCoroutine(CustomUpdate());

        // End
        return;
    }

    private IEnumerator CustomUpdate() {
        var devices = new List<InputDevice>();
        while(true) {
            yield return null;
            InputDevices.GetDevicesWithCharacteristics(m_inputCharacteristics, devices);
            if (devices.Count == 1) {
                CheckInputs(devices[0]);
            } else if (m_debugMode) {
                DebugLogger.current.AddLine("Found "+devices.Count.ToString()+" devices w/ characteristic " + m_inputCharacteristics.ToString());
            }
        }
    }

    public void ResetInputs() {
        if (m_inputs.Count == 0) return;
        // Reset input mappings
        foreach(ButtonMapping bm in m_inputs) {
            bm.Init();
        }
        // End
        return;
    }

    public void CheckInputs(InputDevice d) {
        // Return early if our XR device or inputs are null
        if (d == null || m_inputs.Count == 0) return;
        // Check buttons
        foreach(ButtonMapping bm in m_inputs) {
            bm.CheckStatus(d, m_debugMode);
        }
        // End
        return;
    }
}

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

    [SerializeField] [Tooltip("Reference to hand grab volume")]
    private XRGrabVolume m_grabVol = null;
    public XRGrabVolume grabVol {
        get {   return m_grabVol;   }
        set {}
    }
    private enum GrabType {None,Grip}
    [SerializeField] [Tooltip("Method to detect grab metaphors")]
    private GrabType m_grabType;
    // NOT SERIALIZED
    private XRGrabbable m_grabbedObject = null;

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
    private class FramePosMap {
        public int thisFrame;
        public Vector3 thisPos;
        public FramePosMap(int frame, Vector3 pos) {
            thisFrame = frame;
            thisPos = pos;
        }
    }
    // NOT SERIALIZED [Tooltip("saving all the positions")]
    private List<Vector3> m_handPositions = new List<Vector3>();
    // NOT SERIALIZED [Tooltip("velocity of the hand")]
    private Vector3 m_velocity;
    // NOT SERIALIZED [Tooltip("angular velocity of the hand)]
    private Quaternion m_prevRotation;
    // NOT SERIALIZED
    private Vector3 m_angularVelocity;
    [SerializeField] [Tooltip("Should objects grabbed by this hand snap to position?")]
    private bool m_shouldSnap = false;
    public bool shouldSnap {
        get {   return m_shouldSnap;    }
        set {   m_shouldSnap = value;   }
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

        // Update the initial, first-ever hand position
        m_handPositions.Add(this.transform.position);
        m_angularVelocity = Vector3.zero;

        // If our grab volume is not null, we set it
        if (m_grabVol != null) m_grabVol.Init();

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
                UpdateVelocity();
                UpdateAngularVelocity();
            } else if (m_debugMode) {
                DebugLogger.current.AddLine("Found "+devices.Count.ToString()+" devices w/ characteristic " + m_inputCharacteristics.ToString());
            }
        }
    }

    private void UpdateVelocity() {
        m_handPositions.Add(this.transform.position);
        if (m_handPositions.Count > 15) m_handPositions.RemoveAt(0);
        if (m_handPositions.Count < 2) {
            m_velocity = Vector3.zero;
            return;
        }
        m_velocity = m_handPositions[m_handPositions.Count-1] - m_handPositions[0];
        if (m_debugMode) DebugLogger.current.AddLine("Velocity: " + m_velocity.ToString());
        return;
    }
    private void UpdateAngularVelocity() {
        Quaternion currentRotation = this.transform.rotation;
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(m_prevRotation);
        m_prevRotation = currentRotation;
        deltaRotation.ToAngleAxis(out var angle, out var axis);
        angle *= Mathf.Deg2Rad;
        m_angularVelocity = (1.0f / Time.deltaTime) * angle * axis;
        if (m_debugMode) DebugLogger.current.AddLine("Ang. Vel: " + m_angularVelocity.ToString());
        return;
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

    public void GripDown() {
        // Storing a possible reference
        Transform closest = null;
            
        // switch based on grab type
        switch(m_grabType) {
            case(GrabType.Grip):
                // If grip, we grab from our grip detector
                closest = m_grabVol.closestInRange;
                break;
            /*
            case(grabType.Distance):
                // We cannot use the teleport ponter for grabbing -_-
                closest = (m_pointer.GetPointerType() != "Teleport" && m_pointer.raycastTarget != null && m_pointer.raycastTarget.GetComponent<CustomGrabbable>()) ? m_pointer.raycastTarget.transform : null;
                break;
            case(grabType.Both):
                // If grip, we grab from our grip detector
                closest = m_gripDetector.closestInRange;
                // closest is updated based on if it's already not null or not - if it is still null, we check the pointer
                closest = (closest != null) ? closest : (m_pointer.GetPointerType() != "Teleport" && m_pointer.raycastTarget != null && m_pointer.raycastTarget.GetComponent<CustomGrabbable>()) ? m_pointer.raycastTarget.transform : null;
                break;
            */
        }
        // If closest is not == null, we grab it!
        if (closest != null && m_grabbedObject == null) {
            closest.GetComponent<XRGrabbable>().GrabBegin(this);
            m_grabbedObject = closest.GetComponent<XRGrabbable>();
        }
        return;
    }

    public void GripUp() {
        if (m_grabbedObject == null) return;
        m_grabbedObject.GrabEnd(this,m_velocity,m_angularVelocity);
        m_grabbedObject = null;
        /*
        OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(m_OVRController), orientation = OVRInput.GetLocalControllerRotation(m_OVRController) };
        OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
        Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_OVRController);
        Vector3 angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_OVRController);
        angularVelocity *= -1;
        m_grabbedObject.GrabEnd(this, linearVelocity, angularVelocity);
        m_grabbedObject.GrabEnd(this, 0f, 0f);
        m_grabbedObject = null;
        */
        return;
    }
}

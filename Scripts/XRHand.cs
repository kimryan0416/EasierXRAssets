﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRHand : MonoBehaviour
{   
    // NOT SERIALIZED [Tooltip("What Input Device matches with the inputCharacteristics defined - retrieved from Init")]
    private InputDevice m_XRdevice;
    public InputDevice XRdevice {
        get {   return m_XRdevice;  }
        set {   return; }
    }
    // NOT SERIALIZED [Tooltip("Storing characteristics")]
    private InputDeviceCharacteristics m_inputCharacteristics;
    public InputDeviceCharacteristics inputCharacteristics {
        get {   return m_inputCharacteristics;  }
        set {   return; }
    }

    // NOT SERIALIZED [Tooltip("Check to make sure that we're actually instantiating")]
    private bool m_initialized = false;
    public bool initialized {
        get {   return m_initialized;   }
        set {   return; }
    }

    // NOT SERIALIZED [Tooltip("All button mappings")]
    private Dictionary<string, ButtonMapping> m_inputs = new Dictionary<string, ButtonMapping>();
    private Axis2DMapping m_thumbstick;

    private enum DebugOption {  Off, On, From_Controller }
    [SerializeField] [Tooltip("Debug Mode")]
    private DebugOption m_debugMode = DebugOption.Off;

    /*
    [SerializeField]
    [Tooltip("Reference to object that acts as object detection for the grabbing function")]
    private XRGrabVolume m_gripDetector;
    [SerializeField]
    [Tooltip("Reference to object that acts as object detection for the tooltip function")]
    private XRGrabVolume m_tooltipDetector;
    */
    /*
    [SerializeField]
    [Tooltip("Reference to a Custom Pointer")]
    private CustomPointer m_pointer;
    public CustomPointer pointer{
        get {   return m_pointer;     }
        set {   m_pointer = value;    }
    }
    */
    /*
    [SerializeField]
    [Tooltip("Reference to where objects to attach to when being grabbed")]
    private Transform m_grabDestination;
    public Transform grabDestination {
        get {   return m_grabDestination;   }
    }

    private enum grabType {
        Grip,
        //Distance,
        //Both
    }
    [SerializeField]
    [Tooltip("The grab type")]
    private grabType m_grabType = grabType.Grip;

    // NOT SERIALIZED
    [Tooltip("If we're holding something... we store a reference to it")]
    private XRGrabbable m_grabbedObject = null;
    public XRGrabbable grabbedObject {
        get {   return m_grabbedObject;     }
        set {   m_grabbedObject = value;    }
    }

    /*
    [SerializeField]
    [Tooltip("Reference to controller appearance")]
    private OVRControllerHelper m_OVRControllerHelper;
    */

    /*
    [SerializeField]
    [Tooltip("Should grabbed objects snap to the hand?")]
    private bool m_shouldSnap = true;
    public bool shouldSnap {
        get {   return m_shouldSnap;    }
    }
    */

    public void Init(InputDevice ds, InputDeviceCharacteristics characteristics, bool debug) {
        m_XRdevice = ds;
        m_inputCharacteristics = characteristics;
        if (m_debugMode == DebugOption.From_Controller) m_debugMode = (debug) ? DebugOption.On : DebugOption.Off;
        ResetInputs();
        /*
        // Each hand comes with two detectors: a grip detector, and a tooltip detector
        // Their inclusion in the hand is totally optional. But a hand wouldn't really work if there wasn't at least one grab detection at some point...
        // A hand can still work without any detection for grip or tooltip... but that would seem odd, in a sense.
        if (m_gripDetector != null) {
            if (!m_gripDetector.shouldStartOnRun) m_gripDetector.Init(true);
            if (m_XRdevice.InputDeviceCharacteristics)
            switch(m_inputCharacteristics) {
                case(InputDeviceCharacteristics.Left):
                    CustomEvents.current.onLeftGripDown += GripDown;
                    CustomEvents.current.onLeftGripUp +=  GripUp;
                    break;
                case(InputDeviceCharacteristics.Right):
                    CustomEvents.current.onRightGripDown += GripDown;
                    CustomEvents.current.onRightGripUp += GripUp;
                    break;
            }
        }
        /*
        if (m_tooltipDetector) {
            if(!m_tooltipDetector.shouldStartOnRun) m_tooltipDetector.Init(true);
        }

        // If a custom pointer is attached to this hand, we initialize it
        if (m_pointer != null) {
            m_pointer.Init(true, this);
            switch(m_OVRController) {
                case(OVRInput.Controller.LTouch):
                    // CustomEvents.current.onLeftTriggerDown += TriggerDown;
                    // CustomEvents.current.onLeftTriggerUp += TriggerUp;
                    CustomEvents.current.onLeftTriggerDown += m_pointer.LineOn;
                    CustomEvents.current.onLeftTriggerUp += m_pointer.LineOff;
                    break;
                case(OVRInput.Controller.RTouch):
                    // CustomEvents.current.onRightTriggerDown += TriggerDown;
                    // CustomEvents.current.onRightTriggerUp += TriggerUp;
                    CustomEvents.current.onRightTriggerDown += m_pointer.LineOn;
                    CustomEvents.current.onRightTriggerUp += m_pointer.LineOff;
                    break;
            }
        }
        */
        return;
    }

    public void ResetInputs() {
        if (m_XRdevice == null) return;
        m_inputs = new Dictionary<string, ButtonMapping>();
        m_thumbstick = null;
        bool debug = (m_debugMode == DebugOption.On) ? true : false;

        m_thumbstick = new Axis2DMapping(m_XRdevice, CommonUsages.primary2DAxis,"Left Thumb Joystick", debug);
        m_inputs.Add("index", new ButtonMapping(m_XRdevice,CommonUsages.triggerButton, "Left Trigger", debug));
        m_inputs.Add("grip",  new ButtonMapping(m_XRdevice,CommonUsages.gripButton, "Left Grip", debug));
        m_inputs.Add("one",   new ButtonMapping(m_XRdevice,CommonUsages.primaryButton, "Left One", debug));
        m_inputs.Add("two",   new ButtonMapping(m_XRdevice,CommonUsages.secondaryButton, "Left Two", debug));
        m_inputs.Add("thumbClick",new ButtonMapping(m_XRdevice,CommonUsages.primary2DAxisClick, "Left Thumb Click", debug));
        if (m_inputCharacteristics == InputDeviceCharacteristics.Left) m_inputs.Add("start", new ButtonMapping(m_XRdevice,CommonUsages.menuButton, "Start Button", debug));
        return;
    }

    public void CheckInputs() {
        if (m_XRdevice == null) return;
        m_thumbstick.CheckPressed();
        foreach(ButtonMapping bm in m_inputs.Values) {
            bm.CheckPressed();
        }
    }

    public void UpdateInputs() {
        if (m_XRdevice == null) return;
        // Thumbstick
        XRController.current.ThumbDirection(m_XRdevice, m_thumbstick.thumbPosition, m_thumbstick.distance, m_thumbstick.angle);
        // Index
        if (m_inputs["index"].value == 1f) XRController.current.TriggerDown(m_XRdevice);
        else if (m_inputs["index"].value == -1f) XRController.current.TriggerUp(m_XRdevice);
        // Grip
        if (m_inputs["grip"].value == 1f) XRController.current.GripDown(m_XRdevice);
        else if (m_inputs["grip"].value == -1f) XRController.current.GripUp(m_XRdevice);
        // One
        if (m_inputs["one"].value == 1f) XRController.current.OneDown(m_XRdevice);
        else if (m_inputs["one"].value == -1f) XRController.current.OneUp(m_XRdevice);
        // Two
        if (m_inputs["two"].value == 1f) XRController.current.TwoDown(m_XRdevice);
        else if (m_inputs["two"].value == -1f) XRController.current.TwoUp(m_XRdevice);
        // ThumbPress
        if (m_inputs["thumbClick"].value == 1f) XRController.current.ThumbPress(m_XRdevice);
        else if (m_inputs["thumbClick"].value == -1f) XRController.current.ThumbRelease(m_XRdevice);
        // return if Right Hand, otherwise continue with start button
        if (m_inputCharacteristics == InputDeviceCharacteristics.Right) return;
        if (m_inputs["start"].value == 1f) XRController.current.StartDown();
        else if (m_inputs["start"].value == -1f) XRController.current.StartUp();
    }

    /*
    private void GripDown(InputDevice d) {
        if (d == m_XRdevice) {
             // Storing a possible reference
            Transform closest = null;
            
            // switch based on grab type
            switch(m_grabType) {
                case(grabType.Grip):
                    // If grip, we grab from our grip detector
                    closest = m_gripDetector.closestInRange;
                    break;

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

            }
            // If closest is not == null, we grab it!
            if (closest != null && m_grabbedObject == null) GrabBegin(closest.GetComponent<XRGrabbable>());
        }
    }

    private void GripUp(InputDevice d) {
        if (d == m_XRdevice && m_grabbedObject != null) GrabEnd();
    }

    private void TriggerDown(InputDevice d) {
        if (d == m_XRdevice) m_pointer.LineSet(true);
    }
    private void TriggerUp(InputDevice d) {
        if (d == m_XRdevice) m_pointer.LineSet(false);
    }

    private void GrabBegin(XRGrabbable c) {
        c.GrabBegin(this);
        m_grabbedObject = c;
    }

    private void GrabEnd() {        

        OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(m_OVRController), orientation = OVRInput.GetLocalControllerRotation(m_OVRController) };

		OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
		Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_OVRController);
		Vector3 angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_OVRController);
        angularVelocity *= -1;
        m_grabbedObject.GrabEnd(this, linearVelocity, angularVelocity);

        m_grabbedObject.GrabEnd(this, 0f, 0f);
        m_grabbedObject = null;
    }
    */
}

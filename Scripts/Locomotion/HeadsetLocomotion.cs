using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HeadsetLocomotion : MonoBehaviour
{       
    /*
    [SerializeField] [Tooltip("Active status")]
    private bool m_activeStatus = false;
    public bool activeStatus {
        get {   return m_activeStatus;  }
        set {   MovementToggle(value);       }
    }

    [SerializeField] [Tooltip("Refernce to rig")]
    private Transform m_rig;
    
    private enum WhichHand {Left,Right}
    [SerializeField] [Tooltip("Hand that acts as toggle for button")]
    private WhichHand m_toggleHand = WhichHand.Left;
    // NOT SERIALIZED [Tooltip("Reference to XRHand that corresponds with toggle hand")]
    private XRHand m_hand = null;

    // NOT SERIALIZED [Tooltip("Reference to main camera")]
    private Camera m_mainCamera;

    [SerializeField] [Tooltip("What distance from the center is considered 1 unit of distance")]
    private float m_normalizationFactor = 0.1f;

    // NOT SERIALIZED [Tooltip("Coroutine")]
    private Coroutine m_customUpdate = null;
    
    // NOT SERIALIZED [Tooltip("Saved reference for local position of camera")]
    private Vector3 m_referenceLocalPosition = Vector3.zero;
    // NOT SERIALIZED [Tooltip("Rotational offset of the cameraand the parent rig")]
    private float m_rotationOffset = 0f;

    private void Awake() {
        m_mainCamera = Camera.main;
    }
    private void Start() {
        m_hand = (m_toggleHand == WhichHand.Left) ? XRController.current.leftHand : XRController.current.rightHand;
        XRController.current.onTwoDown += MovementToggle;
        XRController.current.onOneDown += RecalibratePosition;
        MovementToggle(m_activeStatus);
    }

    private IEnumerator CustomUpdate() {
        while(true) {
            yield return null;
            // Update the rotational offset between the camera and the parent transform
            //m_rotationOffset = Vector3.Angle(m_mainCamera.transform.forward, m_rig.transform.forward);
            //m_rotationOffset *= (m_mainCamera.transform.forward.x >= m_rig.transform.forward.x) ? 1f : -1f;
            Vector3 positionOffset = m_mainCamera.transform.position - 
        }
    }

    public void MovementToggle(InputDevice device) {
        if (device != m_hand.XRdevice) return;
        MovementToggle();
    }
    public void MovementToggle(bool en) {
        m_activeStatus = en;
        if (m_activeStatus) {
            RecalibratePosition();
            if (m_customUpdate == null) {
                m_customUpdate = StartCoroutine(CustomUpdate());
            }
        } else {
            if (m_customUpdate != null) {
                StopCoroutine(m_customUpdate);
                m_customUpdate = null;
            }
        }
    }
    public void MovementToggle() {
        m_activeStatus = !m_activeStatus;
        if (m_activeStatus) {
            RecalibratePosition();
            if (m_customUpdate == null) {
                m_customUpdate = StartCoroutine(CustomUpdate());
            }
        } else {
            if (m_customUpdate != null) {
                StopCoroutine(m_customUpdate);
                m_customUpdate = null;
            }
        }
    }

    public void RecalibratePosition(InputDevice device) {
        if (device != m_hand.XRdevice) return;
        RecalibratePosition();
        
    }
    public void RecalibratePosition() {
        m_referenceLocalPosition = m_mainCamera.transform.localPosition;
        return;
    }
    */
}

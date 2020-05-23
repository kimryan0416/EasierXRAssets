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

    [SerializeField] [Tooltip("Hand Reference - used by other scripts")]
    private XRHand m_leftHand, m_rightHand;
    public XRHand leftHand {
        get {   return m_leftHand;  }
        set {}
    }
    public XRHand rightHand {
        get {   return m_rightHand; }
        set {}
    }

    private void Awake() {
        // Set the instance so that any script can call this from script
        current = this;
        // End
        return;
    }

    private void Start()
    {
        // End
        return;
    }
}

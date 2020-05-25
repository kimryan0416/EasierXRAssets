using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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

    [SerializeField] [Tooltip("Hover Cursor Prefab Reference")]
    private XRHoverCursor m_hoverCursorPrefab;
    // NOT SERIALIZED
    private Dictionary<int, XRHoverCursor> m_hoverCursors = new Dictionary<int, XRHoverCursor>();

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

    public void HandleHovers(List<Transform> targets) {
        List<int> idsToCheck = m_hoverCursors.Keys.ToList(); 
        // Foreach object inside what was found in range, we need to check if 1) any new hovers were detected, and 2) if any match existing copies
        foreach(Transform target in targets) {
            // This function returns TRUE if there already is a hover cursor attached to the target... if FALSE, then it had to instantiate a new hover cursor
            if (InstantiateHover(target)) {
                idsToCheck.Remove(target.gameObject.GetInstanceID());
            }
        }
        // For any id's that we did not find, we relieve them from their duties.
        while(idsToCheck.Count > 0) {
            m_hoverCursors[idsToCheck[0]].Relieve();
            m_hoverCursors.Remove(idsToCheck[0]);
            idsToCheck.RemoveAt(0);
        }
    }
    public void HandleHovers(List<Transform> targets, Color m_hoverColor) {
        List<int> idsToCheck = m_hoverCursors.Keys.ToList(); 
        // Foreach object inside what was found in range, we need to check if 1) any new hovers were detected, and 2) if any match existing copies
        foreach(Transform target in targets) {
            // This function returns TRUE if there already is a hover cursor attached to the target... if FALSE, then it had to instantiate a new hover cursor
            if (InstantiateHover(target, m_hoverColor)) {
                idsToCheck.Remove(target.gameObject.GetInstanceID());
            }
        }
        // For any id's that we did not find, we relieve them from their duties.
        while(idsToCheck.Count > 0) {
            m_hoverCursors[idsToCheck[0]].Relieve();
            m_hoverCursors.Remove(idsToCheck[0]);
            idsToCheck.RemoveAt(0);
        }
    }

    public bool InstantiateHover(Transform target) {
        if (m_hoverCursors.ContainsKey(target.gameObject.GetInstanceID())) return true;
        XRHoverCursor m_newHover = Instantiate(m_hoverCursorPrefab, Vector3.zero, Quaternion.identity) as XRHoverCursor;
        m_newHover.Init(target, Color.yellow);
        m_hoverCursors.Add(target.gameObject.GetInstanceID(), m_newHover);
        return false;
    }
    public bool InstantiateHover(Transform target, Color m_hoverColor) {
        if (m_hoverCursors.ContainsKey(target.gameObject.GetInstanceID())) return true;
        XRHoverCursor m_newHover = Instantiate(m_hoverCursorPrefab, Vector3.zero, Quaternion.identity) as XRHoverCursor;
        m_newHover.Init(target, m_hoverColor);
        m_hoverCursors.Add(target.gameObject.GetInstanceID(), m_newHover);
        return false;
    }

/*
    public void RemoveHover(Transform target) {
        if (m_hoverCursors.ContainsKey(target.gameObject.GetInstanceID())) {
            XRHoverCursor thisHover = m_hoverCursors[target.gameObject.GetInstanceID()];
            m_hoverCursors.Remove(target.gameObject.GetInstanceID());
            thisHover.Relieve();
        }
        return;
    }
*/


}

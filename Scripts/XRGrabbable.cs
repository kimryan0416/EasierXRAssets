﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class XRGrabbable : MonoBehaviour
{   
    // NOT SERIALIZED
    private Dictionary<XRHand, Transform> m_grabbers = new Dictionary<XRHand, Transform>();
    // NOT SERIALIZED 
    private List<XRHand> m_semanticGrabbers = new List<XRHand>();
    // NOT SERIALIZED
    private XRHand m_mainGrabber;
    public XRHand mainGrabber {
        get {return m_mainGrabber;}
        set {}
    }
    // NOT SERIALIZED
    private Dictionary<XRHand, Transform> heldBy = new Dictionary<XRHand, Transform>();

    [SerializeField] [Tooltip("Transform references that the script uses as indicators of where the object should snap to. Also controls how many grabbers are allowed to grab it")]
    private List<Transform> snapTransforms = new List<Transform>();
    // NOT SERIALIZED
    private Dictionary<Transform, SnapTransformData> snapTransformInfo = new Dictionary<Transform, SnapTransformData>();

    // NOT SERIALIZED
    private Rigidbody m_RigidBody;
    // NOT SERIALIZED
    private bool m_grabbedKinematic;
    // NOT SERIALIZED
    private Transform originalParent;
    // NOT SERIALIZED
    private int m_originalLayer;
    // NOT SERIALIZED
    private Dictionary<int, int> m_childrenOriginalLayers = new Dictionary<int, int>();
    // NOT SERIALIZED
    private int numGrabbersAllowed;

    [SerializeField] [Tooltip("Deubg mode toggle")]
    private bool m_debugMode = false;

    public class SnapTransformData {
        public bool taken;
        public Vector3 posOffset;
        public Quaternion rotOffset;
        public SnapTransformData(Transform snap, Transform parent) {
            taken = false;
            posOffset = snap.position - parent.position;
            rotOffset = Quaternion.Inverse(snap.rotation * Quaternion.Inverse(parent.rotation));
        }
    }

    // Think of it like this:
    // Each object that has this script can be grabbed by XRHand
    // There is one caveat: how do we handle two (or more) hands grabbing something simultaneously? 
    // The logic runs like this:
    // - There is an infinite number of grabbers allowed to grab this object. However, there can only be one "main" grabber
    // - This "main" grabber is where the object focuses its orientation and position on
    // - Any additional hands grabbing the object only grab it semantically... we'll explain this more in detail
    // - When a hand attempts to grab the object, it checks if the main grabber null or not.
    //      1. If null, it'll set this grabber as the main grabber.
    //          This also entails setting all the rigidbody values to fit that scenario (aka mkaing the rb kinematic, not affected by gravity, etc)
    //      2. If not null, we label it as a semantic grabber.
    // - We also need to take into account snap transforms
    //      - If we have any snap transforms set, that means that the object must snap to the main grabber's position and whatnot.
    //      - If no snap transforms are set, we don't need to care about snap transforming to fit the hand's orientation and position
    // - When a hand attempts to let go of the object:
    //      1. It removes the hand from the total list of grabbers (m_grabbers);
    //      2. Checks if the hand is the same as the main grabber
    //          - If so, have to check if there are any semantic grabbers that can replace the main grabber
    //              - If there are, we replace the main grabber with the oldest semantic grabber and remove that grabber from the semantic list
    //              - If not, then we reset the object with its original rigidbody values alongside the position and angular velocity from the hand
    //      3. If not, we simply remove it from our list of semantic grabbers

    private void Start() {
        if (DebugLogger.current == null) m_debugMode = false;
        // We get the initial values for this object
        m_RigidBody = this.GetComponent<Rigidbody>();
        m_grabbedKinematic = m_RigidBody.isKinematic;
        m_originalLayer = this.gameObject.layer;
        originalParent = transform.parent;
        // We grab any children this object has and also save their layers
        Transform[] children = this.GetComponentsInChildren<Transform>();
        foreach (Transform c in children) {
            m_childrenOriginalLayers.Add(c.gameObject.GetInstanceID(), c.gameObject.layer);
        }
        // For each snap transform, we add it to our SnapTransformData list
        foreach(Transform t in snapTransforms) {    
            snapTransformInfo.Add(t, new SnapTransformData(t,this.transform));  
        }

        //numGrabbersAllowed = (snapTransforms.Count > 0) ? snapTransforms.Count : 1;
        StartCoroutine(SetParent());
    }

    private IEnumerator SetParent() {
        while(true) {
            if (m_mainGrabber == null) {  transform.SetParent(originalParent);    }
            else {
                transform.SetParent(m_mainGrabber.transform);
                if (m_mainGrabber.shouldSnap) {   SnapToTransform();  }
            }
            yield return null;
        }
    }

    private void FixedUpdate() {
        if (m_debugMode) {
            string mainName = (m_mainGrabber != null) ? m_mainGrabber.gameObject.name : "NONE";
            DebugLogger.current.AddLine("Main Grabber: " + mainName);
            DebugLogger.current.AddLine("SemanticGrabbers: " + m_semanticGrabbers.Count);
        }
    }

    // Called by XRHand to initiate grabbing
    public void GrabBegin(XRHand g) {
        // Kind of par on the course for all grabbers, we find the closest snap transform
        Transform snapTo =  (snapTransforms.Count > 0) ?  FindClosestSnapTransform(g) : null;
        m_grabbers.Add(g,snapTo);
        if (m_debugMode) DebugLogger.current.AddLine("Grabbed By: " + g.gameObject.name);

        // check if we have a main grabber or not
        if (m_mainGrabber == null) {
            m_RigidBody.isKinematic = true;
            m_RigidBody.velocity = Vector3.zero;
            m_RigidBody.angularVelocity = Vector3.zero;
            //this.gameObject.layer = LayerMask.NameToLayer("AvoidHover");
            //Transform[] children = this.GetComponentsInChildren<Transform>();
            //foreach (Transform c in children) {
            //    c.gameObject.layer = LayerMask.NameToLayer("AvoidHover");
            //}
            m_mainGrabber = g;
        } else {
            m_semanticGrabbers.Add(g);
        }
        return;
    }

    public void GrabEnd(XRHand hand, Vector3 linVel, Vector3 angVel) {
        // Remove hand from grabbers list
        m_grabbers.Remove(hand);
        // If the hand letting go is the same as the main grabber, we need to check if we can assign a new main grabber
        if (GameObject.ReferenceEquals(hand.gameObject, m_mainGrabber.gameObject)) {
            // If we have any semantic grabbers, we'll assign the oldest one as the new main
            if (m_semanticGrabbers.Count > 0) {
                // Find the new grabber
                XRHand newGrabber = m_semanticGrabbers[0];
                // Remove it from our semantics list
                m_semanticGrabbers.Remove(newGrabber);
                // Set it
                m_mainGrabber = newGrabber;
            } else {
                // Otherwise, set main grabber to null and do everything else.
                m_mainGrabber = null;
                // Set its linear and angular velocities to the object's rigidbody
                m_RigidBody.isKinematic = m_grabbedKinematic;
                m_RigidBody.velocity = linVel * 2f;
                m_RigidBody.angularVelocity = angVel;
                //this.gameObject.layer = m_originalLayer;
                //Transform[] children = this.GetComponentsInChildren<Transform>();
                //foreach (Transform c in children) {
                //    c.gameObject.layer = m_childrenOriginalLayers[c.gameObject.GetInstanceID()];
                //}
            }
        } else {
            m_semanticGrabbers.Remove(hand);
        }
        return;
    }

    public List<XRHand> GetGrabbers() {
        List<XRHand> toReturn = new List<XRHand>();
        foreach(XRHand hand in m_grabbers.Keys) { toReturn.Add(hand);   }
        return toReturn;
    }

    private Transform FindClosestSnapTransform(XRHand cg) {
        Transform closest = null;
        float distance = 0f;
        foreach(Transform t in snapTransforms) {
            if (snapTransformInfo[t].taken) {   continue;   }
            if (closest == null) {
                closest = t;
                distance = Vector3.Distance(cg.grabVol.transform.position, t.position);
            }
            float tempDist = Vector3.Distance(cg.grabVol.transform.position, t.position);
            if (tempDist < distance) {
                closest = t;
                distance = tempDist;
            }
        }
        return closest;
    }
    private void SnapToTransform() {
        
        if (m_mainGrabber == null || !m_mainGrabber.shouldSnap) return;
        
        Transform snapReference = m_grabbers[m_mainGrabber];
        if (snapReference == null) {
            snapReference = this.transform;
            transform.rotation = m_mainGrabber.grabVol.transform.rotation * Quaternion.Euler(45,0,0);
        } else {
            transform.rotation = (m_mainGrabber.grabVol.transform.rotation * Quaternion.Inverse(snapReference.localRotation)) * Quaternion.Euler(45,0,0);
        }
        transform.position = m_mainGrabber.grabVol.transform.position + (transform.position - snapReference.position);
    }
}

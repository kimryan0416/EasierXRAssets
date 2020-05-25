using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Linq;

public class XRGrabVolume : MonoBehaviour
{

    [SerializeField] [Tooltip("Active state")]
    private bool m_isActive = false;
    public bool isActive {
        get {   return m_isActive;  }
        set {   m_isActive = value; }
    }
    
    [SerializeField] [Tooltip("The transform where the collision should occur")]
    private Transform m_collisionOrigin;

    [SerializeField] [Tooltip("The world-space radius where detection should occur")]
    private float m_collisionRadius = 0.1f;
    public float collisionRadius {
        get {   return m_collisionRadius;   }
        set {   m_collisionRadius = value;  }
    }

    [SerializeField] [Tooltip("List of layers that the grab volume should avoid or only observe")]
    private List<string> m_layers = new List<string>();
    private List<int> m_layerInts = new List<int>();
    private enum LayerOption {Disable_Layers, Ignore_Layers, Look_For_Layers}
    private bool layerState = false;
    [SerializeField] [Tooltip("Should the grab volume avoid or only look for these tags?")]
    private LayerOption m_layerOption = LayerOption.Disable_Layers;

    /*
    [SerializeField] [Tooltip("List of scripts that the grab volume should avoid or only observe")]
    private List<string> m_components = new List<string>();
    private enum ComponentOption {Disable_Components, Ignore_Components, Look_For_Components}
    private bool componentState = false;
    [SerializeField] [Tooltip("Should the grab volume avoid or only look for these components?")]
    private ComponentOption m_componentOpton = ComponentOption.Disable_Components;
    */

    // NOT SERIALIZED [Tooltip("Reference to all objects that are in range")]
    private List<Transform> m_inRange = new List<Transform>();
    public List<Transform> inRange {
        get {   return m_inRange;   }
        set {}
    }
    // Get the closest in range
    public Transform closestInRange {
        get {   return (m_inRange.Count > 0) ? m_inRange[0] : null; }
        set {}
    }

    [SerializeField] [Tooltip("should hover cursors be used to indicate grabbable objects?")]
    private bool m_useHoverCursor = false;

    [SerializeField] [Tooltip("Color of hover cursor, if hover cursor is added - otherwise, just changes itself's color")]
    private Color m_hoverColor = Color.yellow;
    // NOT SERIALIZED [Tooltip("Original color material of original material - used only if the hover prefab is not set")]
    private Color m_originalColor;

    /*
    [SerializeField]
    [Tooltip("Boolean to check if the custom update should activate upon starting the game")]
    private bool m_shouldStartOnRun = true;
    public bool shouldStartOnRun {
        get {   return m_shouldStartOnRun;  }
    }
    [SerializeField]
    [Tooltip("Boolean to check if itself and its children can collide with other objects")]
    private bool m_canCollide = false;
    public bool canCollide {
        get {   return m_canCollide;      }
        set {   ToggleCollision(value);   }
    }
    
    */

    [SerializeField] [Tooltip("Debug Mode")]
    private bool m_debugMode = false;

    public void Init(bool shouldStart = true) {
        if (DebugLogger.current == null) m_debugMode = false;
        if (m_collisionOrigin == null) m_collisionOrigin = this.transform;
        m_originalColor = m_collisionOrigin.GetComponent<Renderer>().material.GetColor("_Color");
        // Set up our layers
        foreach(string m_layer in m_layers) {
            m_layerInts.Add(LayerMask.NameToLayer(m_layer));
        }
        layerState = (m_layerOption == LayerOption.Ignore_Layers) ? true : false;

        m_isActive = shouldStart;
    }

    private void Update() {
        // End early if we are deactivated
        if (!m_isActive) return;
        // Get all gameobjects in range
        if (m_layerOption == LayerOption.Disable_Layers) {
            m_inRange = CommonFunctions.GetInRange<Transform, XRGrabbable>(m_collisionOrigin, m_collisionRadius);
        }
        else {
            m_inRange = CommonFunctions.GetInRange<Transform, XRGrabbable>(m_collisionOrigin, m_collisionRadius, m_layerInts, layerState);
        }
        if (m_debugMode) DebugLogger.current.AddLine(m_inRange.Count.ToString());

        // Hover cursors
        if (!m_useHoverCursor) {
            // If our hover cursor prefab is null, we just change the material color of our grab volume
            m_collisionOrigin.GetComponent<Renderer>().material.color = (m_inRange.Count > 0) ? m_hoverColor : m_originalColor;
        } else {
            XRController.current.HandleHovers(m_inRange, m_hoverColor);
        }
    }

    /*

    public IEnumerator CustomUpdate() {
        // update boolean to tell system that this coroutine is running
        m_updateRunning = true;
        // Execute while loop
        while(true) {
            // get all gameobjects in range
            m_inRange = CommonFunctions.GetInRange<Transform, XRHand, HoverCursor>(m_collisionOrigin, m_collisionRadius, LayerMask.NameToLayer("AvoidHover"));
            if (m_hoverCursorPrefab == null) {
                m_collisionOrigin.GetComponent<Renderer>().material.color = (m_inRange.Count > 0) ? m_hoverColor : m_originalColor;
            } else {
                // Get a list of all instance ID's to check for 
                List<int> idsToCheck = m_hovers.Keys.ToList(); 
                // Foreach object inside what was found in range, we need to check if 1) any new hovers were detected, and 2) if any match existing copies
                foreach(Transform g in m_inRange) {
                    if (!m_hovers.ContainsKey(g.gameObject.GetInstanceID())) {
                        // a new gameobject has entered the scene... add it
                        HoverCursor newHover = Instantiate(m_hoverCursorPrefab, Vector3.zero, Quaternion.identity) as HoverCursor;
                        newHover.Init(g, m_hoverColor);
                        m_hovers.Add(g.gameObject.GetInstanceID(), newHover);
                    } else {
                        idsToCheck.Remove(g.gameObject.GetInstanceID());
                    }
                }
                // For any id's that we did not find, we relieve them from their duties.
                while(idsToCheck.Count > 0) {
                    m_hovers[idsToCheck[0]].Relieve();
                    m_hovers.Remove(idsToCheck[0]);
                    idsToCheck.RemoveAt(0);
                }
            }
            yield return null;
        }
    }

    public void ToggleCollision(bool toggle = false) {
        m_canCollide = toggle;
        Collider[] cs = this.GetComponentsInChildren<Collider>() as Collider[];
        foreach(Collider c in cs) {
            c.enabled = m_canCollide;
        }
    }

    */
}

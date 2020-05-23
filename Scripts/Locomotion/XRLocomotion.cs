using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRLocomotion : MonoBehaviour
{

    public static XRLocomotion current;

    [SerializeField] [Tooltip("Reference to XRRig's transform")]
    private Transform m_rig = null;
    // NOT SERIALIZED [Tooltip("Reference to XRRig's External Collider)]
    //private ExternalCollider m_rigEC = null;
    /*
    [SerializeField] [Tooltip("Which hand controls the locomotion and rotation, depending on m_locomotionHandChoice and m_rotationHandChoice respectively")]
    private XRHand m_locomotionHand = null, m_rotationHand = null;
    */

    // NOT SERIALIZED
    private enum MovementOption {Continuous,Instant,Teleport,SmoothDamp}
    [SerializeField] [Tooltip("How will our player move?")]
    private MovementOption m_movementOption = MovementOption.Instant;
    [SerializeField] [Tooltip("Will thumbstick movement be relative to your head's orientation while walking? If not, it'll only be relative to your head's orientation at rest")]
    private bool m_resetOrientationWhileMoving = false;
    [SerializeField] [Tooltip("Speed of player in M/S for thumbstick")]
    private float m_movementSpeed = 1.4f;
    // NOT SERIALIZED [Tooltip("Storing a movement translation")]
    private Vector3 m_movementTranslation = Vector3.zero;

    // NOT SERIALIZED
    private enum RotationOption {Continuous,Snap}
    [SerializeField] [Tooltip("Rotation type - thumbstick or no?")]
    private RotationOption m_rotationOption = RotationOption.Continuous;
    [SerializeField] [Tooltip("Rotation amount (degrees per second) - only applies if rotating with joystick")]
    private float m_rotationRate = 10f;
    // NOT SERIALIZED [Tooltip("Checks if we already rotated or not - only used with snap rotation")]
    private bool m_alreadyRotated = false;
    // NOT SERIALIZED [Tooltip("Track rotation offset")]
    private float m_rotationOffset = 0f;
    public float rotationOffset {
        get {   return m_rotationOffset;    }
        set {}
    }
    // NOT SERIALIZED [Tooltip("Reference to initial rotation of rig")]
    private Vector3 m_rigInitialForward;

    [SerializeField] [Tooltip("Locomotion toggles")]
    private bool m_movementAllowed = true, m_rotationAllowed = true;
    public bool movementAllowed {
        get {   return m_movementAllowed;   }
        set {   m_movementAllowed = value;  }
    }
    public bool rotationAllowed {
        get {   return m_rotationAllowed;   }
        set {   m_rotationAllowed = value;  }
    }

    // NOT SERIALIZED
    private Camera mainCamera;
    // NOT SERIALIZED
    private Vector3 m_camForward = Vector3.zero, m_camRight = Vector3.zero;

    [SerializeField] [Tooltip("Should we debug? Overwritten by Custom Controller if not running on start")]
    private bool m_debugMode = false;

    private void Awake() {
        current = this;
        mainCamera = Camera.main;
        //if (m_rig.gameObject.GetComponent<Rigidbody>() == null) m_rig.gameObject.GetComponent<Rigidbody>();
        //if (m_rig.gameObject.GetComponent<ExternalCollider>() == null) { m_rig.gameObject.AddComponent<ExternalCollider>(); }
        //m_rigEC = m_rig.gameObject.GetComponent<ExternalCollider>();
    }

    private void Start() {
        // Set the rig's initial foward position
        m_rigInitialForward = m_rig.transform.forward;    
        // Adjust movement option    
        /*
        switch(m_movementOption) {
            case(MovementOption.Thumbstick):
                XRController.current.onThumbDirection += Movement;
                break;
            case(MovementOption.Teleportation):
                // Add teleport stuff
                break;
            case(MovementOption.Both):
                XRController.current.onThumbDirection += Movement;
                // Add teleport stuff
                break;
        }
        // Adjust rotation option
        switch(m_rotationOption) {
            case(RotationOption.Thumbstick):
                XRController.current.onThumbDirection += Rotation;
                break;
        }
        */
        // End
        return;
    }

    /*
    private void FixedUpdate() {
         if (m_movementTranslation.magnitude>0) {
            //m_rig.Translate(translation);
            m_rig.gameObject.GetComponent<Rigidbody>().MovePosition(m_rig.transform.position + m_movementTranslation);
            //Vector3 smoothedDelta = Vector3.MoveTowards(m_rig.position, m_rig.position + translation, m_movementSpeed * Time.deltaTime);
            //m_rig.gameObject.GetComponent<Rigidbody>().MovePosition(smoothedDelta);
        }
    }
    */

    // Particularly when moving with the joystick
    public void ContinuousMovement(InputDevice device, float buttonVal, Vector2 pos) {
        // Check - return early if the XR Devices don't correspond with each other
        if (!m_movementAllowed) return;
        //if (m_rigEC.colliding) return;
        // So joystick movement will work like this:
        // Joysticks will control motion such that they are relative to the player's INITIAL camera forward position at rest
        // So for example, if you start moving and you were at rest, as you're moving, even though you're rotating your head, the joystick will still move you relative to your initial head orientation
        // Only when you are at rest (aka distance < 0.01f) when the orientation reorients itself
        //float rigRotationOffset = 0f;
        float angleOffset = m_rotationOffset;
        float distance = pos.magnitude;
        if (m_resetOrientationWhileMoving || distance < 0.1f) {
            m_camForward = mainCamera.transform.forward;
            m_camRight = mainCamera.transform.right;
        }
        m_camForward.y = 0f;
        m_camRight.y = 0f;
        m_movementTranslation = Quaternion.AngleAxis(-1f * angleOffset,Vector3.up) * (m_camForward * pos.y + m_camRight * pos.x) * m_movementSpeed * Time.deltaTime;
        //m_movementTranslation = (m_camForward * thumbPos.y + m_camRight * thumbPos.x) * m_movementSpeed * Time.deltaTime;
        if (m_movementTranslation.magnitude>0) {
            m_rig.Translate(m_movementTranslation);
            //m_rig.gameObject.GetComponent<Rigidbody>().MoveTowards(translation);
            //Vector3 smoothedDelta = Vector3.MoveTowards(m_rig.position, m_rig.position + translation, m_movementSpeed * Time.deltaTime);
            //m_rig.gameObject.GetComponent<Rigidbody>().MovePosition(smoothedDelta);
        }
        return;
    }

    public void MovementInstant(Vector3 pos) {
        m_rig.position = pos;
        /*
        float angleOffset = m_rotationOffset;
        m_camForward = mainCamera.transform.forward;
        m_camRight = mainCamera.transform.right;
        m_camForward.y = 0f;
        m_camRight.y = 0f;
        m_movementTranslation = Quaternion.AngleAxis(-1f * angleOffset,Vector3.up) * (m_camForward * amount.y + m_camRight * amount.x) * m_movementSpeed * Time.deltaTime;
        if (m_movementTranslation.magnitude>0) {
            m_rig.Translate(m_movementTranslation);
        }
        */
    }

    public void Rotation(InputDevice device, float buttonVal, Vector2 pos) {
        // Check - return early if the XR Devices don't correspond with each other
        if (!m_rotationAllowed) return;

        // Rotation is dependent on how much "Left" or "Right" the joystick of choice is.
        // For example, if the joystick's X value is 0.1, then the rotation is small... but if the joystick's Y value is 1, then it rotates at the full expected rate.
        // However, this is also dependent on the type of rotation we're going for (Snap or Continuous)
        // If we're set to Snap rotation, then we rotate only once and reset when the thumbstick's X < 0.5f
        // If we're set to Continuous, this function runs all the time

        if (m_rotationOption == RotationOption.Snap && m_alreadyRotated) {
            if (Mathf.Abs(pos.x) < 0.5f) m_alreadyRotated = false;
            return;
        }
        // Prevent dead-zoneness
        if (m_rotationOption == RotationOption.Snap && Mathf.Abs(pos.x) < 0.5f) return;

        float angleToRotate = (m_rotationOption == RotationOption.Snap) ? m_rotationRate * Mathf.Sign(pos.x) : m_rotationRate * pos.x * Time.deltaTime;
        m_rig.transform.Rotate(0f,angleToRotate,0f);

        m_rotationOffset = Vector3.Angle(m_rig.transform.forward, m_rigInitialForward);
        m_rotationOffset *= (m_rig.transform.forward.x >= m_rigInitialForward.x) ? 1f : -1f;
        m_alreadyRotated = true;
        return;
    }

    public void RotationDirectly(Vector2 pos) {
        if (!m_rotationAllowed) return;
        if (m_rotationOption == RotationOption.Snap && m_alreadyRotated) {
            if (Mathf.Abs(pos.x) < 0.5f) m_alreadyRotated = false;
            return;
        }
        // Prevent dead-zoneness
        if (m_rotationOption == RotationOption.Snap && Mathf.Abs(pos.x) < 0.5f) return;
        
        float angleToRotate = (m_rotationOption == RotationOption.Snap) ? m_rotationRate * Mathf.Sign(pos.x) : m_rotationRate * pos.x * Time.deltaTime;
        m_rig.transform.Rotate(0f,angleToRotate,0f);

        m_rotationOffset = Vector3.Angle(m_rig.transform.forward, m_rigInitialForward);
        m_rotationOffset *= (m_rig.transform.forward.x >= m_rigInitialForward.x) ? 1f : -1f;
        m_alreadyRotated = true;
    }
    public void RotationDirectly(Vector2 pos, float angleAmount) {
        if (!m_rotationAllowed) return;
        if (m_rotationOption == RotationOption.Snap && m_alreadyRotated) {
            if (Mathf.Abs(pos.x) < 0.5f) m_alreadyRotated = false;
            return;
        }
        // Prevent dead-zoneness
        if (m_rotationOption == RotationOption.Snap && Mathf.Abs(pos.x) < 0.5f) return;
        
        float angleToRotate = (m_rotationOption == RotationOption.Snap) ? angleAmount * Mathf.Sign(pos.x) : angleAmount * pos.x * Time.deltaTime;
        m_rig.transform.Rotate(0f,angleToRotate,0f);

        m_rotationOffset = Vector3.Angle(m_rig.transform.forward, m_rigInitialForward);
        m_rotationOffset *= (m_rig.transform.forward.x >= m_rigInitialForward.x) ? 1f : -1f;
        m_alreadyRotated = true;
    }

    public void SetLocomotionOptions(bool mov, bool rot) {
        m_movementAllowed = mov;
        m_rotationAllowed = rot;
    }

    public void Teleport(Collider platform, Collider trigger) {
        m_rig.position = new Vector3(m_rig.position.x, m_rig.position.y, m_rig.position.z + 5f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SphereLocomotion : MonoBehaviour
{
    /*
    private enum NumHands {
        One_Handed,Two_Handed
    }
    [SerializeField] [Tooltip("Number of hand operations")]
    private NumHands m_numHands = NumHands.Two_Handed;
    */

    [SerializeField] [Tooltip("Reference to point of reference for transform")]
    private Transform m_centerPoint;
    public Transform centerPoint {
        get {   return m_centerPoint;   }
        set {   m_centerPoint = value;  }
    }
    [SerializeField] [Tooltip("Reference to object attached to centerpoint that acts as forwad indicator")]
    private Transform m_forwardPoint;
    [SerializeField] [Tooltip("Reference to forward position's floor")]
    private Transform m_forwardFloorPoint;

    [SerializeField] [Tooltip("Radius of our sphere detection")]
    private float m_radius = 0.25f;
    public float radius {
        get {   return m_radius;    }
        set {   m_radius = value;   }
    }

    [SerializeField] [Tooltip("Reference to hand for the locomotion")]
    private HandLineMapping m_toggleHand, m_otherHand;

    [System.Serializable]
    public class HandLineMapping {
        public enum WhichHand {
            Left,
            Right
        }
        public WhichHand m_hand;
        public LineRenderer _lineRenderer;
        private XRHand m_XRHand;
        public XRHand hand {
            get {   return m_XRHand;    }
            set {}
        }

        public void HandReferenceSet() {
            m_XRHand = (m_hand == WhichHand.Left) ? XRController.current.leftHand : XRController.current.rightHand;
        }
        public void UpdateLine(bool isActive, Transform origin) {
            // Set line renderer to match isActive status
            _lineRenderer.enabled = isActive;
            // Return early if line renderer is disabled
            if (!_lineRenderer.enabled) return;
            // Get world position of hand
            Vector3 handPos = m_XRHand.transform.position;
            // points are origin and handPos. Set the line renderer
            Vector3[] positions = new Vector3[2] {handPos, origin.position};
            _lineRenderer.SetPositions(positions);
        }
    }

    [SerializeField] [Tooltip("Rate at which the rotation occurs. Overrides Custom Lcoomotion's rotation rate")]
    private float m_rotationRate = 45f;

    private enum MovementMagnifier {
        Linear,
        Exponential
    }
    [SerializeField] [Tooltip("How should the forward distance be magnified?")]
    private MovementMagnifier m_movementMagnifier = MovementMagnifier.Exponential;
    [SerializeField] [Tooltip("Exponent, if using Exponentail movement magnifier")]
    private float m_exponentialMagnifier = 2f;

    // NOT SERIALIZED [Tooltip("IEnumerator that should be run, depending on m_numHands)]
    private IEnumerator m_updateLoop;
    // NOT SERIALIZED [Tooltip("Coroutine for the custom update")]
    private Coroutine m_customUpdate = null;

    [SerializeField] [Tooltip("Activation state")]
    private bool m_activated = false;

    [SerializeField] [Tooltip("Debug toggle")]
    private bool m_debugMode = false;

    // NOT SERIALIZED [Tooltip("Saving the original position of the hand - only for one-handed locomotion")]
    private Vector3 originalHandPosition = Vector3.zero;

    private void Start() {
        // Set debug mode based on existence of DebugLogger in the scene
        m_debugMode = (DebugLogger.current == null) ? false : m_debugMode;
        // Set centerpoint to this script's game object
        if (m_centerPoint == null) m_centerPoint = this.transform;
        // Ensure that centerpoint is parented under the main camera
        m_centerPoint.SetParent(Camera.main.transform);
        m_centerPoint.localPosition = Vector3.zero;
        m_centerPoint.localEulerAngles = Vector3.zero;
        // Ensure that hand references are valid
        m_toggleHand.HandReferenceSet();
        m_otherHand.HandReferenceSet();
        // Add MovementInitialize as listener to Two (B/Y) buttin press down
        //XRController.current.onTwoDown += MovementInitialize;
        //XRController.current.onOneDown += MovementToggle;
        /*XRController.current.onStartDown += MovementSwitch;*/
        // Start our coroutine, if we're set to be active from the get-go
        MovementReset();
    }

    private IEnumerator TwoHandCustomUpdate() {
        while(true) {
            yield return null;
            // Update our lines
            m_toggleHand.UpdateLine(true, m_centerPoint);
            m_otherHand.UpdateLine(true, m_centerPoint);
            // Update centerpoint's position in world space to the midpoint between the two hands
            m_centerPoint.position = (m_toggleHand.hand.transform.position + m_otherHand.hand.transform.position)/2f;
            // rotation depends on which hand is considered the toggle hand
            float rotMagSign = 1f;
            Vector3 dir = Vector3.zero; 
            if (m_toggleHand.hand.inputCharacteristics == InputDeviceCharacteristics.Left) {
                dir = (m_toggleHand.hand.transform.localPosition - m_otherHand.hand.transform.localPosition).normalized;
            } else {
                dir = (m_otherHand.hand.transform.localPosition - m_toggleHand.hand.transform.localPosition).normalized * -1f;
                rotMagSign = -1f;
            }

            // Get magnitude by finding average of distance between togglehand and center, and otherhand and center
            float magDistance = Mathf.Abs(Vector3.Distance(m_toggleHand.hand.transform.position, m_centerPoint.position));
            float otherDistance = Mathf.Abs(Vector3.Distance(m_otherHand.hand.transform.position, m_centerPoint.position));
            float averageDistance = (magDistance + otherDistance) / 2f;
            switch(m_movementMagnifier) {
                case(MovementMagnifier.Linear):
                    m_forwardPoint.localPosition = new Vector3(0f, 0f, averageDistance * 200f);
                    break;
                case(MovementMagnifier.Exponential):
                    m_forwardPoint.localPosition = new Vector3(0f, 0f, Mathf.Pow((averageDistance*100f),m_exponentialMagnifier));
                    break;
            }
            m_forwardFloorPoint.position = new Vector3(m_forwardPoint.position.x, 0f, m_forwardPoint.position.z);
            m_forwardFloorPoint.eulerAngles = Vector3.zero;

            // Rotation is determiend by the rotation of the dir value
            // generally, if left hand higher, then magnitude > 0. If the left hand lower, magnitude < 0
            // if the hands are flipped, the magnitude needs to be flipped as well.
            float rotMagnitude = rotMagSign * dir.y;
            if (rotMagnitude > 1f) rotMagnitude = 1f;
            XRLocomotion.current.RotationDirectly(new Vector2(rotMagnitude,0f),m_rotationRate);
        }
    }

    private IEnumerator OneHandCustomUpdate() {
        m_otherHand._lineRenderer.enabled = false;
        // Save the hand's original position
        originalHandPosition = m_toggleHand.hand.transform.position;
        while(true) {
            yield return null;
            // Make sure the centerpoint still remains at its original position
            m_centerPoint.position = originalHandPosition;
            // Update our lines
            m_toggleHand.UpdateLine(true, m_centerPoint);
            // Update centerpoint's position in world space to the midpoint between the two hands
            //m_centerPoint.position = (m_toggleHand.hand.transform.position + m_otherHand.hand.transform.position)/2f;
            // rotation depends on which hand is considered the toggle hand
            Vector3 dir = Vector3.zero; 
            if (m_toggleHand.hand.inputCharacteristics == InputDeviceCharacteristics.Left) {
                dir = (m_toggleHand.hand.transform.localPosition - m_centerPoint.localPosition).normalized;
            } else {
                dir = (m_centerPoint.localPosition - m_toggleHand.hand.transform.localPosition).normalized * -1f;
            }

            // Get magnitude by finding average of distance between togglehand and center, and otherhand and center
            float magDistance = Mathf.Abs(Vector3.Distance(m_toggleHand.hand.transform.position, m_centerPoint.position));
            switch(m_movementMagnifier) {
                case(MovementMagnifier.Linear):
                    m_forwardPoint.localPosition = new Vector3(0f, 0f, magDistance * 200f);
                    break;
                case(MovementMagnifier.Exponential):
                    m_forwardPoint.localPosition = new Vector3(0f, 0f, Mathf.Pow((magDistance*100f),m_exponentialMagnifier));
                    break;
            }
            m_forwardFloorPoint.position = new Vector3(m_forwardPoint.position.x, 0f, m_forwardPoint.position.z);
            m_forwardFloorPoint.eulerAngles = Vector3.zero;

            // Rotation is determiend by the rotation of the toggle hand
            // To determine this, we will use the transform.right of the toggle hand
            // if the right Y-value is 
            // generally, if left hand higher, then magnitude > 0. If the left hand lower, magnitude < 0
            // if the hands are flipped, the magnitude needs to be flipped as well.
            Vector3 toggleHandRight = m_toggleHand.hand.transform.right;
            float rotMagnitude = (Mathf.Abs(toggleHandRight.y) < 0.1f) ? 0f : toggleHandRight.y * -1f;
            if (Mathf.Abs(rotMagnitude) > 1f) rotMagnitude = Mathf.Sign(rotMagnitude) * 1f;
            XRLocomotion.current.RotationDirectly(new Vector2(rotMagnitude,0f),m_rotationRate);
            if (m_debugMode) DebugLogger.current.AddLine(toggleHandRight.ToString());
            
            /*
            float rotMagnitutde = rotMagSign * dir.y;
            if (rotMagnitutde > 1f) rotMagnitutde = 1f;
            XRLocomotion.current.RotationDirectly(new Vector2(rotMagnitutde,0f),m_rotationRate);
            */
        }
    }

    public void MovementInitialize() {
        if (!m_activated) return;
        Vector3 destination = new Vector3(m_forwardPoint.position.x, 0f, m_forwardPoint.position.z);
        XRLocomotion.current.MovementInstant(destination);
        //if (m_numHands == NumHands.One_Handed) originalHandPosition = m_toggleHand.hand.transform.position;
    }

    public void MovementToggle() {
        m_activated = !m_activated;
        MovementReset();
    }


    /*
    private void MovementSwitch(InputDevice device) {
        if (device != m_toggleHand.hand.XRdevice) return;
        m_numHands = (m_numHands == NumHands.One_Handed) ? NumHands.Two_Handed : NumHands.One_Handed;
        MovementReset();
    }
    */

    public void MovementReset() {
        m_centerPoint.gameObject.SetActive(m_activated);
        m_toggleHand._lineRenderer.enabled = m_activated;
        m_otherHand._lineRenderer.enabled = m_activated;
        //m_updateLoop = (m_numHands == NumHands.One_Handed) ? OneHandCustomUpdate() : TwoHandCustomUpdate();
        m_updateLoop = TwoHandCustomUpdate();
        if (m_activated) {
            if (m_customUpdate != null) {
                StopCoroutine(m_customUpdate);
            }
            m_customUpdate = StartCoroutine(m_updateLoop);
        } else {
            if (m_customUpdate != null) {
                StopCoroutine(m_customUpdate);
                m_customUpdate = null;
            }
        }
    }

}

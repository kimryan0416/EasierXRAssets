using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[System.Serializable]
public class Axis2DMapping {
    public InputFeatureUsage<Vector2> input;
    public InputDevice device;
    
    public Vector2 thumbPosition = Vector2.zero;
    public float distance = 0f;
    public float angle = 0f;
    
    public string label = "";
    public bool registered = false;
    public bool debugMode = false;
    
    // Constructor
    public Axis2DMapping(InputDevice d, InputFeatureUsage<Vector2> inValue, string l, bool debug = false) {
        device = d;
        input = inValue;
        label = l;
        debugMode = debug;
    }
    
    // Called when value needs to be updated
    public void CheckPressed() {
        if (!device.TryGetFeatureValue(input, out thumbPosition)) {
            if (debugMode) DebugLogger.current.AddLine("Error with getting thumb position: " + label);
            // End early
            return;
        }
        
        // Get the value of the vector2 clamped to 1f to be safe
        thumbPosition = Vector2.ClampMagnitude(thumbPosition, 1f);
        // Get distance of joystick to center + angle (0 degrees = West)
        distance = Vector2.Distance(Vector2.zero, thumbPosition);
        angle = CommonFunctions.GetAngleFromVector2(thumbPosition);

        if (debugMode) DebugLogger.current.AddLine("Pos: " + thumbPosition.ToString() + "\nD: " + distance.ToString() + "\nA: " + angle.ToString());

        // End
        return;
    }
}
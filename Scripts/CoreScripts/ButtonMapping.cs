using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[System.Serializable]
public class ButtonMapping
{
    public InputFeatureUsage<bool> input;
    public InputDevice device;
    
    public float value = 0f;
    public float timePressed, timeReleased;

    public string label = "";
    public bool registered = false;
    public bool debugMode = false;
    
    // Constructor
    public ButtonMapping(InputDevice d, InputFeatureUsage<bool> inValue, string l, bool debug = false) {
        device = d;
        input = inValue;
        label = l;
        debugMode = debug;
    }
    
    // Called when value needs to be updated
    public void CheckPressed() {
        // Useless value, just for this scope
        bool discardedValue;
        // We actually toggle between 0, 1, and -1
        // 0 = no action this update, 1 = pressed down this frame, -1 = released this frame
        if (device.TryGetFeatureValue(input, out discardedValue) && discardedValue) {
            if (discardedValue != registered) {
                value = 1f;
                timePressed = Time.time;
                registered = discardedValue;
                if (debugMode) DebugLogger.current.AddLine(label + " Down");
            } else {
                value = 0f;
            }
        } 
        else if (discardedValue != registered) {
            value = -1f;
            timeReleased = Time.time;
            registered = discardedValue;
            if (debugMode) DebugLogger.current.AddLine(label + " Up");
        }
        else {
            value = 0f;
        }
    }
}

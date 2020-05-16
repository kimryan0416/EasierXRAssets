using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyScript : MonoBehaviour
{
    public void ButtonPressed() {
        DebugLogger.current.AddLine("Pressed");
    }
    public void ButtonReleased() {
        DebugLogger.current.AddLine("Released");
    }
}

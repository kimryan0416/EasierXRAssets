using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Linq;

[System.Serializable]
public class CommonFunctions : MonoBehaviour
{
    // Simply get a list of gameObjects (or the components in them) that fit a certain radius
    public static List<Out> GetInRange<Out>(Transform origin, float rad) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        return inRange;
    }
    // Get a list of gameObjects (or the components in them) that contains a specific component
    public static List<Out> GetInRange<Out, Check>(Transform origin, float rad) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null) {
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        return inRange;
    }
    // Get a list of gameObjects (or the components in them) that contains a specific component and does not contain certain layers
    public static List<Out> GetInRangeIgnoreLayers<Out, Check>(Transform origin, float rad, List<int> layers) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null && !layers.Contains(c.gameObject.layer)) {
                //inRange.Add(c.GetComponent<Out>());
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }
    // Get a list of gameObjects (or the components in them) that contains a specific component and contains certain layers
    public static List<Out> GetInRangeCheckLayers<Out, Check>(Transform origin, float rad, List<int> layers) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null && layers.Contains(c.gameObject.layer)) {
                //inRange.Add(c.GetComponent<Out>());
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }
    // Get a list of gameObjects (or the components in them) that contains a specific component and may or may not have layers depending on a boolean "ignoreLayers"
    public static List<Out> GetInRangeLayers<Out, Check>(Transform origin, float rad, List<int> layersToIgnore, List<int> layersToCheck) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null && !layersToIgnore.Contains(c.gameObject.layer) && layersToCheck.Contains(c.gameObject.layer)) {
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }
    // Get a list of gameObjects (or the components in them) that contains a specific component and does not contain certain Transforms
    public static List<Out> GetInRangeIgnoreTransforms<Out, Check>(Transform origin, float rad, List<int> transforms) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null && !transforms.Contains(c.transform.GetInstanceID())) {
                //inRange.Add(c.GetComponent<Out>());
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }
    // Get a list of gameObjects (or the components in them) that contains a specific component and does not contain certain Transforms
    public static List<Out> GetInRangeIgnoreTransforms<Out, Check>(Transform origin, float rad, List<Transform> transforms) {
        List<int> tIDs = new List<int>();
        foreach(Transform t in transforms) {
            tIDs.Add(t.GetInstanceID());
        }
        return GetInRangeIgnoreTransforms<Out, Check>(origin, rad, tIDs);
    }
    // Get a list of gameObjects (or the components in them) that contains a specific component and contains certain Transforms
    public static List<Out> GetInRangeCheckTransforms<Out, Check>(Transform origin, float rad, List<int> transforms) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null && transforms.Contains(c.transform.GetInstanceID())) {
                //inRange.Add(c.GetComponent<Out>());
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }
    // Get a list of gameObjects (or the components in them) that contains a specific component and contains certain Transforms
    public static List<Out> GetInRangeCheckTransforms<Out, Check>(Transform origin, float rad, List<Transform> transforms) {
        List<int> tIDs = new List<int>();
        foreach(Transform t in transforms) {
            tIDs.Add(t.GetInstanceID());
        }
        return GetInRangeCheckTransforms<Out, Check>(origin, rad, tIDs);
    }
    // Get a list of gameObjects (or the components in them) that contains a specific component and may or may not have certain Transforms depending on a boolean "ignoreTransforms"
    public static List<Out> GetInRangeTransforms<Out, Check>(Transform origin, float rad, List<int> transformsToAvoid, List<int> transformsToCheck) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null && !transformsToAvoid.Contains(c.transform.GetInstanceID()) && transformsToCheck.Contains(c.transform.GetInstanceID())) {
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }
    // Get a list of gameObjects (or the components in them) that contains a specific component and may or may not have certain Transforms depending on a boolean "ignoreTransforms"
    public static List<Out> GetInRangeTransforms<Out, Check>(Transform origin, float rad, List<Transform> transformsToAvoid, List<Transform> transformsToCheck) {
        List<int> tIDsAvoid = new List<int>();
        List<int> tIDsCheck = new List<int>();
        foreach(Transform t in transformsToAvoid) {
            tIDsAvoid.Add(t.GetInstanceID());
        }
        foreach(Transform t in transformsToCheck) {
            tIDsCheck.Add(t.GetInstanceID());
        }
        return GetInRangeTransforms<Out, Check>(origin, rad, tIDsAvoid, tIDsCheck);
    }
    // Get a list of gameObjects (or the components in them) that ignores layers and ignores transforms
    public static List<Out> GetInRangeIgnoreLayersIgnoreTransforms<Out, Check>(Transform origin, float rad, List<int> layersToAvoid, List<int> transformsToAvoid) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null && !layersToAvoid.Contains(c.gameObject.layer) && !transformsToAvoid.Contains(c.transform.GetInstanceID())) {
                //inRange.Add(c.GetComponent<Out>());
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }
    // Get a list of gameObjects (or the components in them) that ignores layers and ignores transforms
    public static List<Out> GetInRangeIgnoreLayersIgnoreTransforms<Out, Check>(Transform origin, float rad, List<int> layersToAvoid, List<Transform> transformsToAvoid) {
        List<int> tIDsAvoid = new List<int>();
        foreach(Transform t in transformsToAvoid) {
            tIDsAvoid.Add(t.GetInstanceID());
        }
        return GetInRangeIgnoreLayersIgnoreTransforms<Out, Check>(origin, rad, layersToAvoid, tIDsAvoid);
    }
    // Get a list of gameObjects (or the components in them) that ignores layers and contains transforms
    public static List<Out> GetInRangeIgnoreLayersCheckTransforms<Out, Check>(Transform origin, float rad, List<int> layersToAvoid, List<int> transformsToCheck) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null && !layersToAvoid.Contains(c.gameObject.layer) && transformsToCheck.Contains(c.transform.GetInstanceID())) {
                //inRange.Add(c.GetComponent<Out>());
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }
    // Get a list of gameObjects (or the components in them) that ignores layers and contains transforms
    public static List<Out> GetInRangeIgnoreLayersCheckTransforms<Out, Check>(Transform origin, float rad, List<int> layersToAvoid, List<Transform> transformsToCheck) {
        List<int> tIDsCheck = new List<int>();
        foreach(Transform t in transformsToCheck) {
            tIDsCheck.Add(t.GetInstanceID());
        }
        return GetInRangeIgnoreLayersCheckTransforms<Out, Check>(origin, rad, layersToAvoid, tIDsCheck);
    }
    // Get a list of gameObjects (or the components in them) that contains layers and ignores transforms
    public static List<Out> GetInRangeCheckLayersIgnoreTransforms<Out, Check>(Transform origin, float rad, List<int> layersToCheck, List<int> transformsToAvoid) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null && layersToCheck.Contains(c.gameObject.layer) && !transformsToAvoid.Contains(c.transform.GetInstanceID())) {
                //inRange.Add(c.GetComponent<Out>());
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }
    // Get a list of gameObjects (or the components in them) that contains layers and ignores transforms
    public static List<Out> GetInRangeCheckLayersIgnoreTransforms<Out, Check>(Transform origin, float rad, List<int> layersToCheck, List<Transform> transformsToAvoid) {
        List<int> tIDsAvoid = new List<int>();
        foreach(Transform t in transformsToAvoid) {
            tIDsAvoid.Add(t.GetInstanceID());
        }
        return GetInRangeCheckLayersIgnoreTransforms<Out, Check>(origin, rad, layersToCheck, tIDsAvoid);
    }
    // Get a list of gameObjects (or the components in them) that contains layers and contains transforms
    public static List<Out> GetInRangeCheckLayersCheckTransforms<Out, Check>(Transform origin, float rad, List<int> layersToCheck, List<int> transformsToCheck) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null && layersToCheck.Contains(c.gameObject.layer) && transformsToCheck.Contains(c.transform.GetInstanceID())) {
                //inRange.Add(c.GetComponent<Out>());
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }
    // Get a list of gameObjects (or the components in them) that contains layers and contains transforms
    public static List<Out> GetInRangeCheckLayersCheckTransforms<Out, Check>(Transform origin, float rad, List<int> layersToCheck, List<Transform> transformsToCheck) {
        List<int> tIDsCheck = new List<int>();
        foreach(Transform t in transformsToCheck) {
            tIDsCheck.Add(t.GetInstanceID());
        }
        return GetInRangeCheckLayersCheckTransforms<Out, Check>(origin, rad, layersToCheck, tIDsCheck);
    }
    // Get a list of gameObjects (or the components in them) that contains and/or ignores layers and contains and/or ignores transforms
    public static List<Out> GetInRange<Out, Check>(Transform origin, float rad, List<int> layersToIgnore, List<int> layersToCheck, List<int> transformsToIgnore, List<int> transformsToCheck) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (
                c.GetComponent<Check>() != null && 
                !layersToIgnore.Contains(c.gameObject.layer) && 
                layersToCheck.Contains(c.gameObject.layer) && 
                !transformsToIgnore.Contains(c.transform.GetInstanceID()) && 
                transformsToCheck.Contains(c.transform.GetInstanceID())
            ) {
                //inRange.Add(c.GetComponent<Out>());
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }
    // Get a list of gameObjects (or the components in them) that contains and/or ignores layers and contains and/or ignores transforms
    public static List<Out> GetInRange<Out, Check>(Transform origin, float rad, List<int> layersToIgnore, List<int> layersToCheck, List<Transform> transformsToIgnore, List<Transform> transformsToCheck) {
        List<int> tIDsAvoid = new List<int>();
        List<int> tIDsCheck = new List<int>();
        foreach(Transform t in transformsToIgnore) {
            tIDsAvoid.Add(t.GetInstanceID());
        }
        foreach(Transform t in transformsToCheck) {
            tIDsCheck.Add(t.GetInstanceID());
        }
        return GetInRange<Out, Check>(origin, rad, layersToIgnore, layersToCheck, tIDsAvoid, tIDsCheck);
    }
    
    // Get a list of gameObjects (or the components in them) that contains layers and contains transforms
    public static List<Out> GetInRange<Out, Check, Exclude>(Transform origin, float rad) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null && c.GetComponent<Exclude>() == null) {
                //inRange.Add(c.GetComponent<Out>());
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }
    public static List<Out> GetInRange<Out, Check, Exclude>(Transform origin, float rad, int layerToAvoid) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null && c.GetComponent<Exclude>() == null && layerToAvoid != c.gameObject.layer) {
                //inRange.Add(c.GetComponent<Out>());
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }
    public static List<Out> GetInRange<Out, Check, Exclude>(Transform origin, float rad, List<int> layersToAvoid) {
        if (origin == null) return null;
        Dictionary<Out, float> possible = new Dictionary<Out, float>();
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, rad);
        foreach (Collider c in hitColliders) {
            if (c.GetComponent<Check>() != null && c.GetComponent<Exclude>() == null && !layersToAvoid.Contains(c.gameObject.layer)) {
                //inRange.Add(c.GetComponent<Out>());
                possible.Add(c.GetComponent<Out>(), Vector3.Distance(origin.position, c.transform.position));
            }
        }
        List<Out> inRange = possible.OrderBy(x => x.Value).Select(pair => pair.Key).ToList();
        
        //inRange = inRange.OrderBy(x => Vector3.Distance(origin.position, x.transform.position)).ToList();
        return inRange;
    }

    public static float GetAngleFromVector2(Vector2 original, string source = "None" ) {
        // Derive angle from y and x
        float angle = Mathf.Atan2(original.y, original.x) * Mathf.Rad2Deg + 180f;
        // We need to do some offsettting becuase for some inane reason the thumbsticks have a ~5-degree offset
        switch(source) {
            case("Left"):
                // need to add 5 degrees
                angle += 5f;
                break;
            case("right"):
                // Need to subtract 5 degrees
                angle -= 5f;
                break;
        }
        // We need to recenter the angle so that it's between 0 and 360, not 5 and 365
        angle = (angle > 360f) ? angle - 360 : angle;
        // Return
        return angle;
    }
}

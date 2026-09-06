// PropPlacer.cs — Activity 1.2: Mock View Studio
// A "blocking-out" tool: press a key and a primitive of a chosen size appears at a chosen distance in front of
// your eyes, resting on the floor, with an AnnotationPin that reports its distance, its angular size, and which
// interaction zone (near / medium / far) it sits in. Use it to test "how big must a sign be at 4 m?" by placing
// the object and looking at it, instead of guessing.
// Attached to: "Prop Placer" in the starter scene (Material and Parent are pre-set).
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PropPlacer : MonoBehaviour
{
    [Header("What to place")]
    [Tooltip("Primitive shape of the next prop. Cycle it with the Shape key.")]
    public PrimitiveType shape = PrimitiveType.Cube;

    [Tooltip("Edge length / diameter of the next prop, in meters.")]
    public float sizeMeters = 0.5f;

    [Tooltip("Horizontal distance from the eyes to the prop's center, in meters.")]
    public float distanceMeters = 2f;

    [Tooltip("Material for placed props. The builder assigns a warm clay color.")]
    public Material material;

    [Tooltip("Parent for placed props so the Hierarchy stays tidy. The builder assigns 'Placed Props'.")]
    public Transform parent;

    [Header("Zones (meters)")]
    [Tooltip("Objects closer than this are in the NEAR zone: arm's reach, where you grab things and stereo depth is strongest.")]
    public float nearZoneMax = 1.0f;

    [Tooltip("Objects closer than this (and beyond the near zone) are in the MEDIUM zone: comfortable for reading and UI.")]
    public float mediumZoneMax = 3.5f;

    [Header("Keys (Input System)")]
    [Tooltip("Place a prop.")]
    public Key placeKey = Key.Enter;
    [Tooltip("Remove the most recently placed prop.")]
    public Key undoKey = Key.Backspace;
    [Tooltip("Move the placement distance closer / farther by the step below.")]
    public Key nearerKey = Key.Minus;
    public Key fartherKey = Key.Equals;
    [Tooltip("Shrink / grow the size by 25%.")]
    public Key smallerKey = Key.Comma;
    public Key biggerKey = Key.Period;
    [Tooltip("Cycle Cube -> Sphere -> Cylinder -> Capsule.")]
    public Key shapeKey = Key.Slash;

    [Tooltip("Distance step per key press, in meters.")]
    public float distanceStep = 0.25f;

    readonly List<GameObject> placed = new List<GameObject>();
    Transform head;

    void Start()
    {
        head = Camera.main != null ? Camera.main.transform : null;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null || head == null) return;

        // TODO 1: Adjust the settings from the keyboard, then place / undo.
        //         if (kb[nearerKey].wasPressedThisFrame)  distanceMeters = Mathf.Max(0.25f, distanceMeters - distanceStep);
        //         if (kb[fartherKey].wasPressedThisFrame) distanceMeters += distanceStep;
        //         if (kb[smallerKey].wasPressedThisFrame) sizeMeters = Mathf.Max(0.05f, sizeMeters / 1.25f);
        //         if (kb[biggerKey].wasPressedThisFrame)  sizeMeters *= 1.25f;
        //         if (kb[shapeKey].wasPressedThisFrame)   CycleShape();
        //         if (kb[placeKey].wasPressedThisFrame)   Place();
        //         if (kb[undoKey].wasPressedThisFrame)    Undo();
        //         Look at: Keyboard.current[Key].wasPressedThisFrame (fires once per press, unlike isPressed).
        //         Check: the Inspector values change as you press - = , . while playing; Enter drops a prop.
    }

    /// <summary>Spawns one prop in front of the eyes, resting on the floor, and pins a measurement label to it.</summary>
    public void Place()
    {
        if (head == null) return;

        // TODO 2: Where does it go? Flatten the head's forward onto the floor plane so looking down does not push
        //         the prop into the ground, then step out distanceMeters and rest the prop on the floor:
        //             Vector3 forward = head.forward; forward.y = 0f; forward.Normalize();
        //             Vector3 footprint = head.position + forward * distanceMeters;   // still at eye height here
        //             Vector3 center = new Vector3(footprint.x, sizeMeters * 0.5f, footprint.z);   // bottom on y = 0
        //         Why measure distance horizontally: "3 m away" in a design document never includes the vertical drop
        //         from your eyes to the floor.
        //         Note: Cylinder and Capsule primitives are 2 units tall at scale 1, so for those use center.y = sizeMeters.
        //         Check: a cube placed at 2 m has its bottom exactly on the floor (select it: y == size / 2).
        Vector3 center = Vector3.zero;

        // TODO 3: Create it. GameObject.CreatePrimitive(shape), set name to shape + " @ " + distanceMeters.ToString("F2") + " m",
        //         position = center, localScale = Vector3.one * sizeMeters, parent = parent (use SetParent(parent, true)),
        //         sharedMaterial = material when material != null (Renderer.sharedMaterial), then placed.Add(go).
        //         Look at: GameObject.CreatePrimitive, Transform.SetParent, Renderer.sharedMaterial.
        //         Check: props appear under "Placed Props" in the Hierarchy with readable names.
        GameObject go = null;

        // TODO 4: Measure and annotate. Compute the angular size with AngularSizeDegrees(sizeMeters, distanceMeters),
        //         classify the distance with ZoneName(distanceMeters), then add an AnnotationPin to the new prop:
        //             var pin = go.AddComponent<AnnotationPin>();
        //             pin.labelHeight = sizeMeters * 0.5f + 0.25f;    // just above the top of the prop
        //             pin.text = sizeMeters.ToString("F2") + " m at " + distanceMeters.ToString("F2") + " m\n"
        //                      + angle.ToString("F1") + " deg - " + zone + " zone";
        //         and Debug.Log the same string so you can copy the numbers into your report.
        //         Check: Enter at the default settings logs "0.50 m at 2.00 m / 14.3 deg - medium zone".
        if (go != null) Debug.Log("[PropPlacer] placed " + go.name);
    }

    /// <summary>Removes the most recently placed prop.</summary>
    public void Undo()
    {
        if (placed.Count == 0) return;
        var last = placed[placed.Count - 1];
        placed.RemoveAt(placed.Count - 1);
        if (last != null) Destroy(last);
    }

    /// <summary>Angular size in degrees of an object of size s seen from distance d: theta = 2 atan(s / 2d).</summary>
    public static float AngularSizeDegrees(float sizeMeters, float distanceMeters)
    {
        // TODO 4 (continued): Same formula as EyeHeightProbe in 1.1. Return 180 when distance <= 0.
        //         Look at: Mathf.Atan (radians!), Mathf.Rad2Deg.
        //         Check: AngularSizeDegrees(0.5f, 2f) is about 14.3; AngularSizeDegrees(2f, 7f) is about 16.3.
        return 0f;
    }

    /// <summary>"near", "medium", or "far" for a horizontal distance, using the thresholds above.</summary>
    public string ZoneName(float distance)
    {
        // TODO 5: Return "near" if distance < nearZoneMax, "medium" if distance < mediumZoneMax, otherwise "far".
        //         Why these zones matter: near = grab and touch; medium = read and choose; far = navigate by landmarks.
        //         A menu in the far zone or a landmark in the near zone is a design mistake you can now name.
        //         Check: 0.6 -> near, 2.0 -> medium, 5.0 -> far.
        return "unknown";
    }

    void CycleShape()
    {
        switch (shape)
        {
            case PrimitiveType.Cube:     shape = PrimitiveType.Sphere;   break;
            case PrimitiveType.Sphere:   shape = PrimitiveType.Cylinder; break;
            case PrimitiveType.Cylinder: shape = PrimitiveType.Capsule;  break;
            default:                     shape = PrimitiveType.Cube;     break;
        }
    }
}

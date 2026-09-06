// EyeHeightProbe.cs — Activity 1.1: Hello, Elaria
// Measures the player's eye height, the horizontal distance to a target, and the target's angular size,
// and prints them to a floating TextMesh. These are the numbers you will quote when you justify scale
// and NPC placement in your Game Design Document.
// Attached to: Probe Readout in the starter scene (Target is pre-set to the Mysterious Guide).
// Created by Isac Artzi

using UnityEngine;

public class EyeHeightProbe : MonoBehaviour
{
    [Header("What to measure")]
    [Tooltip("The object whose angular size you want. The starter scene points this at the Guide.")]
    public Transform target;

    [Tooltip("Real-world height of the target in meters. The Guide capsule is 2 m tall.")]
    public float targetHeightMeters = 2f;

    [Header("Where to print")]
    [Tooltip("The TextMesh that shows the readout. The starter scene assigns the one on this GameObject.")]
    public TextMesh readout;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (readout == null) readout = GetComponent<TextMesh>();
    }

    void Update()
    {
        if (cam == null || target == null || readout == null) return;

        // TODO 1: Eye height. The XR Origin sits at floor level (y = 0), so the camera's world y IS the eye height.
        //             float eyeHeight = cam.transform.position.y;
        //         Check: about 1.6 in the simulator; your real height on the Quest.

        // TODO 2: Horizontal distance to the target. Copy both positions, set their y to 0, then
        //         Vector3.Distance(a, b). Why drop y: leaning up or down should not change "how far away" it is.

        // TODO 3: Angular size. Call AngularSizeDegrees(targetHeightMeters, distance) — implement it below.

        // TODO 4: Print. Build a string with one decimal place per value, e.g.
        //             "Eye height: " + eyeHeight.ToString("F1") + " m\n" +
        //             "Distance: "   + distance.ToString("F1")  + " m\n" +
        //             "Angular size: " + angle.ToString("F1") + " deg\n" +
        //             "Camera FOV (vertical): " + cam.fieldOfView.ToString("F0") + " deg"
        //         and assign it to readout.text.
        //         Check: at the origin you read roughly 1.6 m / 7.0 m / 16.3 deg. Walk halfway to the Guide and
        //         the angular size roughly doubles.
        readout.text = "Probe: implement TODO 1-4 in EyeHeightProbe.cs";
    }

    /// <summary>
    /// Angular size in degrees of an object of the given height (m) seen from the given distance (m),
    /// using theta = 2 * atan(h / (2 d)).
    /// </summary>
    public static float AngularSizeDegrees(float heightMeters, float distanceMeters)
    {
        // TODO 3 (continued): Implement the formula.
        //         - If distanceMeters <= 0f return 180f (the object fills the view).
        //         - Otherwise: 2f * Mathf.Atan(heightMeters / (2f * distanceMeters)) * Mathf.Rad2Deg
        //         Look at: Mathf.Atan (returns RADIANS), Mathf.Rad2Deg (57.2958).
        //         Check with a calculator: AngularSizeDegrees(2f, 7f) should be about 16.3.
        return 0f;
    }
}

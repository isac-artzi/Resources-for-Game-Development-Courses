// TeleportArcPreview.cs — Activity 5.1: Teleport, Properly
// Draws your own teleport arc: a projectile curve p(t) = p0 + v t + 1/2 g t^2 launched from the hand
// (or the camera on desktop), sampled into a LineRenderer and cut off where it first hits geometry.
// The landing point is passed to LandingValidator, which decides the arc's color and the label text.
// This is the same math the XRI ray interactor uses for its "Projectile Curve" line type — building it
// yourself is how you learn what its Velocity and Acceleration settings actually do.
// Attached to: Arc Preview in the starter scene (Aim Source is pre-set to the right controller, or the camera).
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class TeleportArcPreview : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("Where the arc launches from and in which direction (its forward). Right controller on the headset; the Main Camera on desktop.")]
    public Transform aimSource;

    [Tooltip("The LineRenderer that draws the arc. Assigned automatically from this GameObject.")]
    public LineRenderer line;

    [Tooltip("Optional: the validator that judges the landing point. Found on this GameObject if left empty.")]
    public LandingValidator validator;

    [Tooltip("Optional: a TextMesh that shows 'OK' or the rejection reason at the landing point.")]
    public TextMesh statusLabel;

    [Header("Projectile")]
    [Tooltip("Launch speed in m/s. Higher reaches farther; 6–10 is typical for a teleport arc.")]
    public float launchSpeed = 8f;

    [Tooltip("Vertical acceleration in m/s^2. Real gravity is -9.81; a stronger pull (-15) makes a tighter, easier-to-aim arc.")]
    public float gravity = -9.81f;

    [Tooltip("How many points to sample along the curve.")]
    [Range(4, 100)] public int samples = 30;

    [Tooltip("Time between samples in seconds. samples * timeStep is the longest flight time drawn.")]
    public float timeStep = 0.05f;

    [Tooltip("Which layers the arc can land on. Default = everything.")]
    public LayerMask hitMask = ~0;

    [Header("Look")]
    public Color validColor = new Color(0.3f, 1f, 0.5f);
    public Color invalidColor = new Color(1f, 0.3f, 0.3f);

    [Header("Desktop")]
    [Tooltip("Keyboard key that toggles the preview on and off (Input System). Useful for the screencast.")]
    public Key toggleKey = Key.P;

    [Tooltip("Whether the preview is currently drawn.")]
    public bool visible = true;

    // Reused every frame so we do not allocate an array per frame.
    Vector3[] points;

    /// <summary>World position where the arc last hit something, or the last sampled point if it hit nothing.</summary>
    public Vector3 LandingPoint { get; private set; }

    /// <summary>True when the arc hit geometry this frame.</summary>
    public bool HasLanding { get; private set; }

    void Start()
    {
        if (line == null) line = GetComponent<LineRenderer>();
        if (validator == null) validator = GetComponent<LandingValidator>();
        if (aimSource == null && Camera.main != null) aimSource = Camera.main.transform;
        points = new Vector3[samples];
        line.positionCount = 0;
    }

    void Update()
    {
        if (aimSource == null || line == null) return;

        // Desktop toggle. Keyboard.current is null on a headset with no keyboard, so guard it.
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame) visible = !visible;
        if (!visible) { line.positionCount = 0; if (statusLabel != null) statusLabel.text = ""; return; }

        if (points == null || points.Length != samples) points = new Vector3[samples];

        Vector3 p0 = aimSource.position;
        Vector3 v0 = aimSource.forward * launchSpeed;
        Vector3 g = new Vector3(0f, gravity, 0f);

        int count = 0;
        HasLanding = false;
        Vector3 hitNormal = Vector3.up;

        // TODO 1: Sample the curve. For i in 0..samples-1: float t = i * timeStep;
        //             points[i] = p0 + v0 * t + 0.5f * g * t * t;   count = i + 1;
        //         Look at: the projectile formula in the README's Math foundation.
        //         Check: with the toggle on you see a green arc leaving the hand (or the camera) and curving to the floor
        //         — it may pass through the floor for now; that is TODO 2.

        // TODO 2: Cut the arc where it first hits geometry. Inside the same loop, for i >= 1, raycast from points[i-1]
        //         toward points[i]:
        //             Vector3 seg = points[i] - points[i - 1];  RaycastHit hit;
        //             if (Physics.Raycast(points[i - 1], seg.normalized, out hit, seg.magnitude, hitMask)) {
        //                 points[i] = hit.point; hitNormal = hit.normal; HasLanding = true; count = i + 1; break; }
        //         Look at: Physics.Raycast(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, int layerMask).
        //         Why segment-by-segment: the curve is not a straight line, so one long ray would miss the floor.
        //         Check: the arc now ends exactly on the floor, the ramp, or a wall; it never pokes through.

        // TODO 3: Publish the landing point and ask the validator. LandingPoint = points[count - 1] (if count > 0).
        //             string reason = ""; bool ok = HasLanding && (validator == null || validator.IsValid(LandingPoint, hitNormal, out reason));
        //         Color the whole line: line.startColor = line.endColor = ok ? validColor : invalidColor;
        //         Look at: LineRenderer.startColor / endColor (works because the arc material uses vertex colors).
        //         Check: aim at flat floor → green; at the stone ramp → red once LandingValidator's slope test is done.

        // TODO 4: Draw it: line.positionCount = count; line.SetPositions(points) — SetPositions copies positionCount entries.
        //         Then update the label if one is assigned: place it at LandingPoint + Vector3.up * 0.3f, make it face the
        //         camera (statusLabel.transform.rotation = Quaternion.LookRotation(statusLabel.transform.position - Camera.main.transform.position)
        //         with the y of that direction zeroed), and set its text to ok ? "OK" : reason.
        //         Check: the label rides along the landing point and reads OK / Too steep / Too close to a wall.
    }
}

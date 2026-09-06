// DashLocomotion.cs — Activity 5.3: Hybrid: Dash and Blink
// A dash: on a button press the XR Origin slides a few meters in the direction the hand (or head) points,
// over about 0.2 s, with the tunnel vignette closing in during the move. The distance is clamped by a
// SphereCast so you stop short of walls and pillars instead of passing through them.
// This moves the rig's Transform directly — simple and robust for a prototype. The README explains what the
// "proper" XRI LocomotionProvider route adds and when you would switch to it.
// Attached to: XR Origin (XR Rig) in the starter scene.
// Created by Isac Artzi

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

public class DashLocomotion : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("The XR Origin to move. Found on this GameObject if empty.")]
    public XROrigin origin;

    [Tooltip("Direction source: the Right Controller on the headset, the Main Camera on desktop. Only its yaw is used.")]
    public Transform aimSource;

    [Tooltip("The vignette controller under the camera. Optional but strongly recommended.")]
    public VignetteController vignette;

    [Header("Dash")]
    [Tooltip("How far a dash tries to travel, in meters, before collision clamping.")]
    public float dashDistance = 3f;

    [Tooltip("Duration of the slide in seconds. 0.15–0.25 is short enough that most players feel nothing.")]
    public float dashSeconds = 0.2f;

    [Tooltip("Minimum time between dashes, in seconds.")]
    public float cooldownSeconds = 0.4f;

    [Tooltip("Vignette strength (0–1) while dashing.")]
    [Range(0f, 1f)] public float vignetteStrength = 0.8f;

    [Header("Collision")]
    [Tooltip("Radius of the body capsule used for the SphereCast. Slightly less than a shoulder width.")]
    public float bodyRadius = 0.3f;

    [Tooltip("Height above the rig's floor at which the cast is made (chest).")]
    public float castHeight = 1.0f;

    [Tooltip("Extra gap to keep between the body and the wall you stop at.")]
    public float wallMargin = 0.1f;

    [Tooltip("Layers that block a dash. Default = everything.")]
    public LayerMask obstacleMask = ~0;

    [Header("Input")]
    [Tooltip("Desktop key (Input System) that triggers a dash.")]
    public Key dashKey = Key.F;

    [Tooltip("Optional controller action (e.g. the right-hand primary button). Leave empty on desktop.")]
    public InputActionReference dashAction;

    [Header("Debug (read-only)")]
    public int dashCount;
    public float lastDashDistance;

    Coroutine running;
    float lastDashTime = -999f;
    CharacterController body;

    void Start()
    {
        if (origin == null) origin = GetComponent<XROrigin>();
        if (aimSource == null && Camera.main != null) aimSource = Camera.main.transform;
        body = GetComponent<CharacterController>();   // the Starter Assets rig has one; null on the fallback rig
    }

    void OnEnable()
    {
        if (dashAction != null && dashAction.action != null) dashAction.action.Enable();
    }

    void Update()
    {
        bool pressed = false;
        if (Keyboard.current != null && Keyboard.current[dashKey].wasPressedThisFrame) pressed = true;
        if (dashAction != null && dashAction.action != null && dashAction.action.WasPressedThisFrame()) pressed = true;

        // TODO 1: If pressed, not already dashing (running == null), and Time.time - lastDashTime >= cooldownSeconds:
        //             Vector3 dir = FlatForward(); if (dir == Vector3.zero) return;
        //             float dist = ClampDistance(dir, dashDistance);
        //             running = StartCoroutine(Dash(dir, dist));
        //         Check: press F — you slide about 3 m toward where the camera (desktop) or right hand (headset) points.
    }

    /// <summary>The aim source's forward projected onto the ground plane and normalized. Zero if looking straight down.</summary>
    public Vector3 FlatForward()
    {
        if (aimSource == null) return Vector3.zero;
        // TODO 2: Vector3 f = aimSource.forward; f.y = 0f; return f.sqrMagnitude < 0.0001f ? Vector3.zero : f.normalized;
        //         Why: a dash must never move you up or down; only the yaw of the hand or head should count.
        //         Check: look at the floor and dash — you still travel horizontally.
        return Vector3.forward;
    }

    /// <summary>Shortens the requested distance so the body stops wallMargin short of the first obstacle.</summary>
    public float ClampDistance(Vector3 dir, float requested)
    {
        // TODO 3: Vector3 start = origin.transform.position + Vector3.up * castHeight;   RaycastHit hit;
        //         if (Physics.SphereCast(start, bodyRadius, dir, out hit, requested, obstacleMask, QueryTriggerInteraction.Ignore))
        //             return Mathf.Max(0f, hit.distance - wallMargin);
        //         return requested;
        //         Look at: Physics.SphereCast(origin, radius, direction, out hit, maxDistance, layerMask, queryTriggerInteraction).
        //         Why a sphere and not a ray: a ray slips between a pillar and a wall where a body would not fit.
        //         Note: hit.distance is how far the SPHERE traveled before touching, so it already accounts for the radius.
        //         Check: dash straight at a pillar from 2 m — you stop about 0.4 m short of it and never clip through.
        return requested;
    }

    IEnumerator Dash(Vector3 dir, float distance)
    {
        dashCount++;
        lastDashDistance = distance;
        lastDashTime = Time.time;
        Vector3 startPos = origin.transform.position;
        Vector3 endPos = startPos + dir * distance;

        // TODO 4: Close the vignette: if (vignette != null) vignette.targetVignette = vignetteStrength;
        //         Then slide: float t = 0f;
        //             while (t < dashSeconds) { t += Time.deltaTime; float s = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / dashSeconds));
        //                                        MoveRigTo(Vector3.Lerp(startPos, endPos, s)); yield return null; }
        //             MoveRigTo(endPos);
        //         Finally open the vignette: vignette.targetVignette = 0f; and set running = null.
        //         Look at: Mathf.SmoothStep for the S-curve; Vector3.Lerp.
        //         Why SmoothStep: zero acceleration at both ends of the slide — the same reason as the ramp in 5.2.
        //         Check: the dash eases in and out over 0.2 s, the edges darken during it, and clear right after.
        MoveRigTo(endPos);   // placeholder: instant hop until TODO 4 is done
        running = null;
        yield return null;
    }

    /// <summary>
    /// Moves the rig to an absolute position. If the rig has a CharacterController, moves through it so Unity keeps
    /// its collision state consistent; otherwise sets the transform directly.
    /// </summary>
    public void MoveRigTo(Vector3 worldPos)
    {
        // TODO 5: Vector3 delta = worldPos - origin.transform.position;
        //         if (body != null && body.enabled) body.Move(delta); else origin.transform.position = worldPos;
        //         Look at: CharacterController.Move(Vector3 motion) — moves with collision, no gravity applied.
        //         Why not always set the transform: with a CharacterController present, teleporting the transform under it
        //         can leave the controller "inside" a collider for a frame. Going through Move avoids that.
        origin.transform.position = worldPos;
    }
}

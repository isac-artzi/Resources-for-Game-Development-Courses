// FollowHeadLazy.cs — Activity 2.3: Menus in Space
// Keeps a world-space canvas in front of the player WITHOUT gluing it to the head. The panel only re-targets when the
// head has turned far enough away, then glides to the new spot with frame-rate-independent smoothing. Uses unscaled
// time so it still works while the game is paused with Time.timeScale = 0.
// Attached to: Menu Canvas.
// Created by Isac Artzi

using UnityEngine;

public class FollowHeadLazy : MonoBehaviour
{
    [Header("Head")]
    [Tooltip("The player's head. Leave empty to use Camera.main.")]
    public Transform head;

    [Header("Placement")]
    [Tooltip("How far in front of the head the panel rests (meters). 1.2-2 m is the comfortable UI range.")]
    public float distance = 1.5f;

    [Tooltip("Vertical offset from eye level (meters). Slightly below eye level is easier to read than dead center.")]
    public float heightOffset = -0.15f;

    [Header("Laziness")]
    [Tooltip("The panel stays put until the head's forward is more than this many degrees away from it.")]
    public float angleThreshold = 30f;

    [Tooltip("Smoothing rate k (per second) for the glide. 4 closes ~63% of the gap in 0.25 s.")]
    public float smoothing = 4f;

    [Tooltip("Always rotate to face the head (around the vertical axis).")]
    public bool faceHead = true;

    Vector3 targetPosition;

    /// <summary>True while the panel is still gliding toward a new target.</summary>
    public bool IsMoving { get; private set; }

    void Start()
    {
        if (head == null && Camera.main != null) head = Camera.main.transform;
        if (head == null)
        {
            Debug.LogWarning(name + ": FollowHeadLazy found no head (Camera.main is null).");
            return;
        }
        targetPosition = ComputeTarget();
        transform.position = targetPosition;
    }

    void LateUpdate()
    {
        if (head == null) return;
        float dt = Time.unscaledDeltaTime;   // NOT Time.deltaTime: this must run while paused

        // TODO 1: Decide whether to re-target. Compare the head's flat forward with the flat direction to the panel:
        //             Vector3 headFwd = Flatten(head.forward);
        //             Vector3 toPanel = Flatten(transform.position - head.position);
        //             float angle = Vector3.Angle(headFwd, toPanel);
        //             if (angle > angleThreshold) targetPosition = ComputeTarget();
        //         Look at: Vector3.Angle (degrees, 0..180). Flatten() is provided below.
        //         Why lazy: a panel that re-centers every frame is head-locked — it never leaves your face, occludes the
        //         world, and (worse) contradicts the room around it. A dead zone lets you look at the panel's edges
        //         without it running away.
        //         Check: turn your head 20 degrees -> the panel stays; 40 degrees -> it slides to the new center.

        // TODO 2: Glide toward the target with exponential smoothing:
        //             float t = 1f - Mathf.Exp(-smoothing * dt);
        //             transform.position = Vector3.Lerp(transform.position, targetPosition, t);
        //             IsMoving = (transform.position - targetPosition).sqrMagnitude > 0.0001f;
        //         Look at: Mathf.Exp, Vector3.Lerp. Same formula as the vignette in 2.2 — learn it once, use it everywhere.
        //         Check: the glide takes about half a second and never overshoots.

        // TODO 3: Face the head. A Canvas reads from its -z side, so its forward must point AWAY from the viewer:
        //             if (faceHead) { Vector3 away = Flatten(transform.position - head.position);
        //                             if (away.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(away); }
        //         Look at: Quaternion.LookRotation. Mirrored text means you subtracted in the wrong order.
        //         Check: walk around while the menu is open — the panel stays upright and legible from wherever you stand.
    }

    /// <summary>The spot 'distance' meters ahead of the head along its flat forward, at eye height + heightOffset.</summary>
    public Vector3 ComputeTarget()
    {
        Vector3 fwd = Flatten(head.forward);
        if (fwd.sqrMagnitude < 0.0001f) fwd = Vector3.forward;   // looking straight up or down: pick something sane
        return head.position + fwd * distance + Vector3.up * heightOffset;
    }

    /// <summary>Removes the vertical component and normalizes (a direction in the horizontal plane).</summary>
    public static Vector3 Flatten(Vector3 v)
    {
        v.y = 0f;
        return v.sqrMagnitude > 0.0001f ? v.normalized : Vector3.zero;
    }
}

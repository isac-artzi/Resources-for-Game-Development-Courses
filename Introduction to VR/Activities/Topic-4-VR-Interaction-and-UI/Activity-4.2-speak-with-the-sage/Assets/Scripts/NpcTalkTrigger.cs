// NpcTalkTrigger.cs — Activity 4.2: Speak with the Sage: NPC Dialogue
// Decides WHEN a conversation starts and ends: the player must be close to the NPC AND looking at her.
// Uses a distance test plus a gaze-cone test (dot product), with hysteresis so the box does not flicker.
// Attached to: "Forest Sage" in the starter scene. Its Runner field points at the Sage Dialogue canvas.
// Created by Isac Artzi

using UnityEngine;

public class NpcTalkTrigger : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The dialogue runner to open and close.")]
    public DialogueRunner runner;

    [Tooltip("The player's head (XR camera). If empty, Camera.main is used.")]
    public Transform head;

    [Tooltip("Optional 3D nameplate above the NPC; it brightens when you are in range as a 'you can talk' hint.")]
    public TextMesh nameplate;

    [Header("Proximity")]
    [Tooltip("Open the conversation when the player is closer than this (meters, horizontal).")]
    public float talkDistance = 3.0f;

    [Tooltip("Close it only when the player is farther than this. Larger than Talk Distance = hysteresis.")]
    public float leaveDistance = 4.0f;

    [Header("Gaze")]
    [Tooltip("Half-angle of the gaze cone in degrees. 30 means the NPC must be within 30 degrees of where you look.")]
    public float gazeHalfAngle = 30f;

    [Tooltip("Where the player should look to count as 'looking at the NPC'. Defaults to this transform + 1.5 m up.")]
    public Transform gazeTarget;

    [Header("Debug (read-only)")]
    public float currentDistance;
    public float currentGazeDot;

    Color nameplateIdle = Color.gray;
    Color nameplateReady = new Color(1f, 0.95f, 0.6f);

    void Start()
    {
        if (head == null && Camera.main != null) head = Camera.main.transform;
        if (nameplate != null) nameplate.color = nameplateIdle;
    }

    void Update()
    {
        if (head == null || runner == null) return;

        Vector3 target = gazeTarget != null ? gazeTarget.position : transform.position + Vector3.up * 1.5f;

        // TODO 1: Horizontal distance from the head to the NPC.
        //             Vector3 toNpc = transform.position - head.position;  toNpc.y = 0f;
        //             currentDistance = toNpc.magnitude;
        //         Why horizontal: crouching should not count as "walking away".
        //         Check: watch Current Distance in the Inspector fall as you walk toward the Sage.
        currentDistance = 0f;

        // TODO 2: Gaze test with a dot product. Flatten both vectors (y = 0) and normalize them:
        //             Vector3 look = head.forward;  look.y = 0f;  look.Normalize();
        //             Vector3 dir = target - head.position;  dir.y = 0f;  dir.Normalize();
        //             currentGazeDot = Vector3.Dot(look, dir);
        //         The player is "looking at" the NPC when currentGazeDot >= Mathf.Cos(gazeHalfAngle * Mathf.Deg2Rad).
        //         Why cos: dot(a,b) = cos(angle) for unit vectors, so comparing to cos(30°) = 0.866 is the same as
        //         asking "angle <= 30°" without ever calling Acos (cheaper, and no NaN worries).
        //         Look at: Vector3.Dot, Mathf.Cos, Mathf.Deg2Rad.
        //         Check: face the Sage, Current Gaze Dot is near 1.0; turn 90°, it is near 0; turn away, negative.
        currentGazeDot = 1f;
        bool looking = currentGazeDot >= Mathf.Cos(gazeHalfAngle * Mathf.Deg2Rad);

        // TODO 3: Open and close with hysteresis.
        //             bool inRange = currentDistance <= talkDistance;
        //             bool farAway = currentDistance >= leaveDistance;
        //             if (!runner.IsOpen && inRange && looking) runner.Open();
        //             else if (runner.IsOpen && farAway) runner.Close();
        //         Why two distances: with a single threshold, standing right on the line makes the box open and
        //         close every frame as tracking jitters. Note that looking away does NOT close the box — once a
        //         conversation has started, only leaving ends it.
        //         Check: walk in while looking: opens at 3 m. Turn away: stays open. Walk out: closes at 4 m.

        // TODO 4: Hint the affordance. Set nameplate.color to nameplateReady when the player is within Talk
        //         Distance (whether or not they are looking) and nameplateIdle otherwise (null-check nameplate).
        //         Why: players need to know an NPC is talkable BEFORE they figure out the gaze rule.
        //         Look at: TextMesh.color, Color.Lerp if you want it to fade.
        //         Check: the "Forest Sage" nameplate turns warm yellow as you approach.
    }
}

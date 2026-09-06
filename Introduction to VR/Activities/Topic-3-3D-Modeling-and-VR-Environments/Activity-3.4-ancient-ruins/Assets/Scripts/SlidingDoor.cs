// SlidingDoor.cs — Activity 3.4: Ancient Ruins: Modular Kit and Mechanisms
// Moves a stone slab from its closed position to closed + Open Offset over a fixed duration with eased motion,
// and back. It knows nothing about levers or altars: it only exposes Open(), Close() and Toggle() for UnityEvents.
// The same component runs the main door (slides down into the floor) and the hidden chamber's false wall.
// Attached to: "Stone Door" and "False Wall" in the starter scene.
// Created by Isac Artzi

using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("Motion")]
    [Tooltip("Where the door ends up when open, relative to its closed local position. (0, -2.8, 0) sinks a 2.8 m slab into the floor.")]
    public Vector3 openOffset = new Vector3(0f, -2.8f, 0f);

    [Tooltip("Seconds for a full open or close.")]
    public float duration = 2.5f;

    [Header("Feedback")]
    [Tooltip("Optional: played once when the door starts moving (a stone rumble).")]
    public AudioSource rumble;

    /// <summary>0 = closed, 1 = open. Read this from other scripts if you need to know how far along the door is.</summary>
    public float Progress { get; private set; }

    /// <summary>True when the door is heading toward (or resting at) open.</summary>
    public bool IsOpening { get; private set; }

    Vector3 closedLocalPosition;

    void Awake()
    {
        closedLocalPosition = transform.localPosition;
    }

    /// <summary>Starts moving toward the open position. Safe to call repeatedly.</summary>
    public void Open()
    {
        if (IsOpening) return;
        IsOpening = true;
        if (rumble != null) rumble.Play();
        Debug.Log("[SlidingDoor] " + name + " opening");
    }

    /// <summary>Starts moving back toward the closed position.</summary>
    public void Close()
    {
        if (!IsOpening) return;
        IsOpening = false;
        if (rumble != null) rumble.Play();
        Debug.Log("[SlidingDoor] " + name + " closing");
    }

    /// <summary>Open if closed, close if open — handy for a two-way lever or a debug button.</summary>
    public void Toggle()
    {
        if (IsOpening) Close(); else Open();
    }

    void Update()
    {
        // TODO 1: Advance Progress toward its target at a rate of 1 / duration per second.
        //             float target = IsOpening ? 1f : 0f;
        //             Progress = Mathf.MoveTowards(Progress, target, Time.deltaTime / Mathf.Max(0.01f, duration));
        //         Look at: Mathf.MoveTowards(current, target, maxDelta) — moves a value without overshooting.
        //         Why not a coroutine: a single Progress value that can reverse mid-way handles "pull the lever back while
        //         the door is still moving" for free. A coroutine would need to be stopped and restarted.

        // TODO 2: Ease it. float eased = Mathf.SmoothStep(0f, 1f, Progress);
        //         SmoothStep starts slow, speeds up, and settles — a multi-ton slab should not start and stop instantly.
        //         Compare with plain Progress once to see the difference.

        // TODO 3: Place the slab.  transform.localPosition = closedLocalPosition + openOffset * eased;
        //         (Vector3.Lerp(closedLocalPosition, closedLocalPosition + openOffset, eased) is the same thing.)
        //         Look at: Transform.localPosition. Local, not world, so a door parented to a moving platform still works.
        //         Check: pull the lever — the door sinks into the floor over 2.5 s, easing in and out; the doorway is clear.
        //         Set Duration to 0.3 and it slams; set Open Offset to (2, 0, 0) and it slides sideways into the wall instead.
    }
}

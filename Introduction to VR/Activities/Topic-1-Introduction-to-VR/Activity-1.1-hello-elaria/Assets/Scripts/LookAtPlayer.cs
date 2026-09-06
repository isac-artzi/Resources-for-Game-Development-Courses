// LookAtPlayer.cs — Activity 1.1: Hello, Elaria
// Turns a 3D label (TextMesh) so the player can always read it. This is a "billboard".
// Attached to: Guide Label and Probe Readout in the starter scene.
// Created by Isac Artzi

using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("What to face. Leave empty to use the main camera (the player's head) automatically.")]
    public Transform target;

    [Header("Behavior")]
    [Tooltip("When true, the label only turns around the vertical axis and never tilts up or down.")]
    public bool lockVertical = true;

    void Start()
    {
        // TODO 1: If target is null, find the main camera and use its transform.
        //         Look at: Camera.main (returns the enabled camera tagged "MainCamera" — the XR Origin's camera).
        //         Why here and not in the Inspector: the XR camera lives inside a prefab that is instantiated
        //         at build time, so "find it at runtime" is the robust choice.
        //         Check: with the Target field empty in the Inspector, the label still turns toward you in Play mode.
    }

    void LateUpdate()
    {
        // LateUpdate runs after all Update() calls, so the camera has already moved this frame.
        if (target == null) return;

        // TODO 2: Compute the direction the label should face.
        //         A TextMesh is readable from its -z side, so its forward (+z) must point AWAY from the viewer:
        //             Vector3 away = transform.position - target.position;
        //         (If you get mirrored text, you subtracted in the other order.)

        // TODO 3: If lockVertical is true, set away.y = 0 so the label stays upright, then guard against a
        //         zero-length vector (if away.sqrMagnitude < 0.0001f return;).
        //         Finally: transform.rotation = Quaternion.LookRotation(away);
        //         Look at: Quaternion.LookRotation(Vector3 forward) — builds a rotation whose +z is 'forward'.
        //         Check: walk around the Guide; its label keeps facing you. Crouch with Lock Vertical on:
        //         the text never tilts. Turn Lock Vertical off: it now tilts to face your eyes.
    }
}

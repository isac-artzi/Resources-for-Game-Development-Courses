// BlinkStep.cs — Activity 5.3: Hybrid: Dash and Blink
// A blink step: fade to black in ~50 ms, hop the XR Origin 2 m in the pointed direction while the view is black,
// hold a beat, fade back in over ~100 ms. It is a tiny teleport with a built-in fade — the most comfortable way
// to cover short distances, and a natural partner to the dash for players who cannot tolerate any slide at all.
// Attached to: XR Origin (XR Rig) in the starter scene, next to DashLocomotion (whose FlatForward/ClampDistance it reuses).
// Created by Isac Artzi

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

public class BlinkStep : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("The XR Origin to move. Found on this GameObject if empty.")]
    public XROrigin origin;

    [Tooltip("Reused for aim direction and collision clamping. Found on this GameObject if empty.")]
    public DashLocomotion dash;

    [Tooltip("The vignette controller under the camera (its blink quad does the fade).")]
    public VignetteController vignette;

    [Header("Blink")]
    [Tooltip("Step length in meters before collision clamping. 1.5–2.5 m is a natural stride-and-a-half.")]
    public float stepDistance = 2f;

    [Tooltip("Fade to black, in seconds.")]
    public float fadeOutSeconds = 0.05f;

    [Tooltip("Time fully black before the fade-in, in seconds.")]
    public float holdSeconds = 0.05f;

    [Tooltip("Fade from black back to clear, in seconds.")]
    public float fadeInSeconds = 0.10f;

    [Header("Input")]
    [Tooltip("Desktop key (Input System) that triggers a blink step.")]
    public Key blinkKey = Key.B;

    [Tooltip("Optional controller action (e.g. the right-hand secondary button). Leave empty on desktop.")]
    public InputActionReference blinkAction;

    [Header("Debug (read-only)")]
    public int blinkCount;

    Coroutine running;

    void Start()
    {
        if (origin == null) origin = GetComponent<XROrigin>();
        if (dash == null) dash = GetComponent<DashLocomotion>();
    }

    void OnEnable()
    {
        if (blinkAction != null && blinkAction.action != null) blinkAction.action.Enable();
    }

    void Update()
    {
        bool pressed = false;
        if (Keyboard.current != null && Keyboard.current[blinkKey].wasPressedThisFrame) pressed = true;
        if (blinkAction != null && blinkAction.action != null && blinkAction.action.WasPressedThisFrame()) pressed = true;

        // TODO 1: If pressed and running == null and dash != null:
        //             Vector3 dir = dash.FlatForward(); if (dir == Vector3.zero) return;
        //             float dist = dash.ClampDistance(dir, stepDistance);
        //             running = StartCoroutine(Blink(dir, dist));
        //         Why reuse DashLocomotion's helpers: one definition of "forward" and one collision rule for both moves —
        //         if you tune bodyRadius on the dash, the blink follows.
        //         Check: press B — you hop 2 m forward behind a quick blink; at a wall you stop short just like the dash.
    }

    IEnumerator Blink(Vector3 dir, float distance)
    {
        blinkCount++;

        // TODO 2: Fade out: if (vignette != null) yield return vignette.FadeBlackout(0f, 1f, fadeOutSeconds);
        //         (yield return on an IEnumerator runs it to completion before continuing — a nested coroutine.)
        //         Check: the view goes black BEFORE you move — pause Play right after pressing B to confirm you have not moved yet.

        // TODO 3: Move while black: dash.MoveRigTo(origin.transform.position + dir * distance);
        //         then hold: yield return new WaitForSeconds(holdSeconds);
        //         Why move during black: this is what makes a blink more comfortable than a bare teleport — no frame ever
        //         shows the world jumping. Why go through dash.MoveRigTo: one place decides how the rig is moved
        //         (CharacterController.Move when there is one, transform otherwise).
        //         Check: after the black you are 2 m ahead; nothing was seen moving.
        dash.MoveRigTo(origin.transform.position + dir * distance);   // placeholder: instant hop until the fades are in place

        // TODO 4: Fade in: if (vignette != null) yield return vignette.FadeBlackout(1f, 0f, fadeInSeconds);
        //         Finally running = null.
        //         Check: total blink lasts about 0.2 s; three quick presses do not overlap or leave the screen black.
        running = null;
        yield return null;
    }
}

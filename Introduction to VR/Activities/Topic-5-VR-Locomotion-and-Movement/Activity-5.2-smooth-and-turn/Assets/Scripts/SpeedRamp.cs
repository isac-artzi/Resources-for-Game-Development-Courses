// SpeedRamp.cs — Activity 5.2: Smooth Move and Turn
// Eases the ContinuousMoveProvider's speed in over a short ramp instead of jumping from 0 to full speed the
// instant the stick moves. Sudden acceleration is the single strongest trigger of motion sickness in smooth
// locomotion; a 0.3 s ease-in removes most of it while feeling almost instant.
// Also measures the rig's real ground speed every frame so you can see the ramp on a label.
// Attached to: XR Origin (XR Rig) in the starter scene (the provider is found automatically).
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

public class SpeedRamp : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("The move provider to ramp. Found on this GameObject's children if empty.")]
    public ContinuousMoveProvider moveProvider;

    [Tooltip("Optional TextMesh that shows target speed, current provider speed, and measured speed.")]
    public TextMesh readout;

    [Header("Ramp")]
    [Tooltip("Full speed in m/s once the ramp is complete. MoveTuner's speed slider writes this value.")]
    public float targetSpeed = 2f;

    [Tooltip("Seconds to go from standstill to full speed. 0.2–0.4 feels responsive but soft. 0 = no ramp.")]
    public float rampSeconds = 0.3f;

    [Header("Debug (read-only)")]
    [Tooltip("Ground speed of the rig measured from its position change, in m/s.")]
    public float measuredSpeed;

    bool isMoving;
    float moveStartTime;
    Vector3 lastPosition;

    void Start()
    {
        if (moveProvider == null) moveProvider = GetComponentInChildren<ContinuousMoveProvider>(true);
        if (moveProvider != null) targetSpeed = moveProvider.moveSpeed;
        lastPosition = transform.position;

        // TODO 1: Subscribe to the provider's events (null-check first):
        //             moveProvider.locomotionStarted += OnMoveStarted;
        //             moveProvider.locomotionEnded   += OnMoveEnded;
        //         The continuous move provider raises 'started' when the stick leaves center and 'ended' when it returns.
        //         Look at: LocomotionProvider.locomotionStarted / locomotionEnded (Action<LocomotionProvider>).
        //         Unsubscribe in OnDestroy (already written below) — fill in the two -= lines there.
        //         Check: Debug.Log in each handler prints once when you start moving and once when you stop.
    }

    void OnDestroy()
    {
        // TODO 1 (continued): moveProvider.locomotionStarted -= OnMoveStarted; moveProvider.locomotionEnded -= OnMoveEnded; (if not null)
    }

    void OnMoveStarted(LocomotionProvider p)
    {
        // TODO 2: If not already moving: isMoving = true; moveStartTime = Time.time.
        //         Why the guard: if the provider ever raises 'started' on consecutive frames, the ramp must not restart each time.
        //         Check: the readout's "provider" number starts near 0 the moment you push the stick.
    }

    void OnMoveEnded(LocomotionProvider p)
    {
        // TODO 3: isMoving = false; and restore moveProvider.moveSpeed = targetSpeed so the next push has the right ceiling
        //         (and so MoveTuner's slider value is what the Inspector shows while idle).
        //         Check: release the stick, look at the provider's Move Speed in the Inspector — it equals the slider.
    }

    void Update()
    {
        // Measured speed: horizontal displacement per second, independent of how the rig was moved.
        // TODO 4: Vector3 delta = transform.position - lastPosition; delta.y = 0f;
        //         measuredSpeed = Time.deltaTime > 0f ? delta.magnitude / Time.deltaTime : 0f; lastPosition = transform.position;
        //         Why: this is the number your body actually experiences; comparing it to moveSpeed tells you whether
        //         the provider (and later your dash in 5.3) does what you think.
        //         Check: walking at 2 m/s shows about 2.0; standing still shows 0.0.

        // TODO 5: The ramp itself. If isMoving and moveProvider != null:
        //             float t = rampSeconds <= 0f ? 1f : Mathf.Clamp01((Time.time - moveStartTime) / rampSeconds);
        //             moveProvider.moveSpeed = targetSpeed * Mathf.SmoothStep(0f, 1f, t);
        //         Look at: Mathf.SmoothStep(from, to, t) — an S-curve: slow start, quick middle, soft arrival at 1.
        //         Why SmoothStep and not Lerp: Lerp still has an acceleration "kink" at t = 0 (speed jumps from 0 to a constant
        //         acceleration); SmoothStep starts with zero acceleration, which is what the inner ear wants.
        //         Check: set rampSeconds to 2 temporarily — you clearly feel the glide up to speed; with 0.3 it feels responsive.

        if (readout != null)
        {
            float providerSpeed = moveProvider != null ? moveProvider.moveSpeed : 0f;
            readout.text = "target " + targetSpeed.ToString("F1") + " m/s\n" +
                           "provider " + providerSpeed.ToString("F2") + " m/s\n" +
                           "measured " + measuredSpeed.ToString("F2") + " m/s";
        }
    }
}

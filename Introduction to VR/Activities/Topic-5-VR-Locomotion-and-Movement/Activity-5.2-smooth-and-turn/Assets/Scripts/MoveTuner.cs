// MoveTuner.cs — Activity 5.2: Smooth Move and Turn
// Connects three world-space sliders and one toggle to the rig's locomotion providers so you can tune
// move speed, snap-turn angle, smooth-turn speed, and head- vs hand-relative movement while playing.
// Keyboard nudges are provided for desktop so you can tune without aiming at the panel.
// Attached to: Tuner Panel (the world-space Canvas) in the starter scene; all references are pre-wired.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class MoveTuner : MonoBehaviour
{
    [Header("Providers (found on the rig at runtime if empty)")]
    public ContinuousMoveProvider moveProvider;
    public SnapTurnProvider snapTurnProvider;
    public ContinuousTurnProvider smoothTurnProvider;

    [Tooltip("Optional: the SpeedRamp on the rig. When present, the speed slider sets its target instead of the provider directly.")]
    public SpeedRamp speedRamp;

    [Header("UI")]
    [Tooltip("Move speed slider, 1–4 m/s.")]
    public Slider speedSlider;
    [Tooltip("Snap angle slider with whole numbers 0, 1, 2 → 30°, 45°, 90°.")]
    public Slider snapSlider;
    [Tooltip("Smooth turn speed slider, 30–180 degrees per second.")]
    public Slider turnSpeedSlider;
    [Tooltip("Head-relative (off) vs hand-relative (on) movement direction.")]
    public Toggle handRelativeToggle;
    public Text speedText;
    public Text snapText;
    public Text turnSpeedText;

    [Header("Forward sources")]
    [Tooltip("The Main Camera transform — movement follows the head when the toggle is off.")]
    public Transform headForward;
    [Tooltip("The Left Controller transform — movement follows the hand when the toggle is on.")]
    public Transform handForward;

    [Header("Desktop nudge keys (Input System)")]
    public Key speedDownKey = Key.Minus;
    public Key speedUpKey = Key.Equals;
    public Key snapCycleKey = Key.LeftBracket;
    public Key handRelativeKey = Key.RightBracket;

    /// <summary>The three snap angles the slider maps to. Index = slider value.</summary>
    public static readonly float[] SnapAngles = { 30f, 45f, 90f };

    void Start()
    {
        // TODO 1: Find the providers when the fields are empty. The rig is an XROrigin, and the providers live on its
        //         "Locomotion" child, so:
        //             var origin = Object.FindFirstObjectByType<XROrigin>();
        //             if (origin != null && moveProvider == null) moveProvider = origin.GetComponentInChildren<ContinuousMoveProvider>(true);
        //         Same for snapTurnProvider and smoothTurnProvider. Null-check everything you use afterwards.
        //         Look at: Component.GetComponentInChildren<T>(bool includeInactive) — pass true; some providers start disabled.
        //         Check: select Tuner Panel in Play mode — the three provider fields are filled in.

        // TODO 2: Initialize the sliders FROM the providers (so the panel shows the truth, not a default), then listen:
        //             if (speedSlider != null && moveProvider != null) { speedSlider.value = moveProvider.moveSpeed;
        //                 speedSlider.onValueChanged.AddListener(OnSpeedChanged); }
        //             snapSlider.value = index of the SnapAngles entry closest to snapTurnProvider.turnAmount; add OnSnapChanged
        //             turnSpeedSlider.value = smoothTurnProvider.turnSpeed; add OnTurnSpeedChanged
        //             handRelativeToggle.onValueChanged.AddListener(OnHandRelativeChanged)
        //         Finally call the three On...Changed methods once so the labels are correct before you touch anything.
        //         Look at: Slider.onValueChanged (UnityEvent<float>), Toggle.onValueChanged (UnityEvent<bool>).
        //         Check: at Play the labels read the rig's actual defaults (Starter Assets: 1 m/s, 45°, 60°/s or similar).
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // TODO 3: Desktop nudges. If speedUpKey was pressed this frame, speedSlider.value += 0.5f (the slider clamps and
        //         fires onValueChanged for you, so the provider updates through OnSpeedChanged). speedDownKey: -= 0.5f.
        //         snapCycleKey: snapSlider.value = (snapSlider.value + 1) % 3. handRelativeKey: flip handRelativeToggle.isOn.
        //         Look at: Keyboard.current[Key].wasPressedThisFrame.
        //         Why: aiming a ray at a tiny slider handle while also moving is fiddly on desktop; keys let you tune fast.
        //         Check: pressing = and - changes the speed label in 0.5 steps and you feel the change when you move.
    }

    /// <summary>Slider → provider. Speed in m/s.</summary>
    public void OnSpeedChanged(float value)
    {
        // TODO 4: If speedRamp != null set speedRamp.targetSpeed = value; else if moveProvider != null set moveProvider.moveSpeed = value.
        //         (When the ramp is present it owns moveSpeed while moving, so writing the provider directly would be overwritten.)
        //         Update speedText.text = value.ToString("F1") + " m/s".
        //         Check: at 4 m/s you cross the whole clearing in about 5 s; at 1 m/s it is a slow walk.
        if (speedText != null) speedText.text = value.ToString("F1") + " m/s";
    }

    /// <summary>Slider (0,1,2) → snap angle (30, 45, 90).</summary>
    public void OnSnapChanged(float value)
    {
        int index = Mathf.Clamp(Mathf.RoundToInt(value), 0, SnapAngles.Length - 1);
        // TODO 5: snapTurnProvider.turnAmount = SnapAngles[index] (null-check), and snapText.text = SnapAngles[index] + " deg".
        //         Check: with 90° it takes four snaps to face where you started; with 30° it takes twelve.
        if (snapText != null) snapText.text = SnapAngles[index].ToString("F0") + " deg";
    }

    /// <summary>Slider → smooth turn speed in degrees per second.</summary>
    public void OnTurnSpeedChanged(float value)
    {
        // TODO 6: smoothTurnProvider.turnSpeed = value (null-check); turnSpeedText.text = value.ToString("F0") + " deg/s".
        //         Note: on the Starter Assets rig smooth turn only takes input when it is enabled on the controller
        //         (see the README, Task 3), otherwise the slider changes a number you cannot feel yet.
        //         Check: at 180 deg/s a full circle takes 2 s; at 45 deg/s it takes 8 s.
        if (turnSpeedText != null) turnSpeedText.text = value.ToString("F0") + " deg/s";
    }

    /// <summary>Toggle → which transform's forward the move provider follows.</summary>
    public void OnHandRelativeChanged(bool handRelative)
    {
        // TODO 7: moveProvider.forwardSource = handRelative ? handForward : headForward (null-check the provider).
        //         Look at: ContinuousMoveProvider.forwardSource — the Transform whose yaw defines "forward" for the stick.
        //         Why it matters: head-relative means "forward" is wherever you look, so glancing left while pushing forward
        //         veers you left. Hand-relative decouples gaze from travel: you can look at the trees while walking the path.
        //         Many players prefer it once they learn it. Try both on the S-curve path.
        //         Check: hand-relative on: push forward, then turn your head — you keep walking the same way.
    }
}

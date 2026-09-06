// LocomotionModeSwitch.cs — Activity 2.2: First Steps: Teleport and Move
// Cycles the rig between Teleport-only, Smooth-only and Hybrid locomotion at runtime by enabling and disabling the
// providers that already live on the Starter Assets rig. No new locomotion code — this is a "settings" script, and it
// is the seed of the comfort menu you will build in Topic 5.
// Attached to: Locomotion Controls (an empty GameObject). Rig, label and teleport interactors are pre-wired.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using Unity.XR.CoreUtils;

public class LocomotionModeSwitch : MonoBehaviour
{
    /// <summary>The three ways a player may travel. Hybrid = both at once (left stick walks, right stick teleports).</summary>
    public enum Mode { TeleportOnly, SmoothOnly, Hybrid }

    [Header("Rig")]
    [Tooltip("The XR Origin (XR Rig) root. Its Locomotion children carry the providers we toggle. Found automatically if empty.")]
    public GameObject rig;

    [Header("Mode")]
    [Tooltip("The mode applied when Play starts and whenever you press the cycle key.")]
    public Mode mode = Mode.Hybrid;

    [Tooltip("Desktop key that cycles the mode (Input System Key enum).")]
    public Key cycleKey = Key.M;

    [Tooltip("Optional controller button for the headset, e.g. an XRI 'Primary Button' action. Leave empty on desktop.")]
    public InputActionReference cycleAction;

    [Header("Optional")]
    [Tooltip("The rig's 'Teleport Interactor' GameObjects. Hidden in Smooth-only so the teleport arc does not appear.")]
    public GameObject[] teleportInteractorObjects;

    [Tooltip("Shows the current mode. Leave empty to skip.")]
    public TextMesh label;

    ContinuousMoveProvider moveProvider;
    TeleportationProvider teleportProvider;

    /// <summary>The mode currently applied to the rig.</summary>
    public Mode CurrentMode { get { return mode; } }

    void Start()
    {
        if (rig == null)
        {
            var origin = Object.FindFirstObjectByType<XROrigin>();
            if (origin != null) rig = origin.gameObject;
        }
        if (rig == null)
        {
            Debug.LogWarning("LocomotionModeSwitch: no XR Origin in the scene.");
            return;
        }

        // TODO 1: Find the two providers on the rig. They sit on children of the rig, some possibly disabled, so use
        //             moveProvider = rig.GetComponentInChildren<ContinuousMoveProvider>(true);
        //             teleportProvider = rig.GetComponentInChildren<TeleportationProvider>(true);
        //         and Debug.LogWarning a clear message for each one that is null (the Starter Assets sample is missing).
        //         Look at: Component.GetComponentInChildren<T>(bool includeInactive).
        //         Why not write our own LocomotionProvider: the rig already ships robust, tuned providers; the design
        //         work is deciding WHEN each is active, not re-implementing movement.
        //         Check: expand XR Origin (XR Rig) > Locomotion in the Hierarchy while playing — the components you
        //         found are the ones whose checkbox flips when you press M (after TODO 3).

        if (cycleAction != null && cycleAction.action != null) cycleAction.action.Enable();
        Apply();
    }

    void Update()
    {
        // TODO 2: If WasCyclePressed() is true, advance to the next mode and call Apply():
        //             mode = (Mode)(((int)mode + 1) % 3);
        //         Check: pressing M in Play mode changes the label text each time and wraps around after Hybrid.
    }

    /// <summary>True on the frame the desktop key or the optional controller action was pressed.</summary>
    bool WasCyclePressed()
    {
        // TODO 2 (continued): Read the Input System, never the legacy Input class.
        //             bool key = Keyboard.current != null && Keyboard.current[cycleKey].wasPressedThisFrame;
        //             bool button = cycleAction != null && cycleAction.action != null && cycleAction.action.WasPressedThisFrame();
        //             return key || button;
        //         Look at: Keyboard.current (null when no keyboard, e.g. on the headset), KeyControl.wasPressedThisFrame,
        //         InputAction.WasPressedThisFrame.
        return false;
    }

    /// <summary>Enables and disables providers to match the current mode, then updates the label.</summary>
    public void Apply()
    {
        // TODO 3: Translate the mode into two booleans and push them to the components:
        //             bool teleport = mode != Mode.SmoothOnly;
        //             bool smooth   = mode != Mode.TeleportOnly;
        //             if (moveProvider != null)     moveProvider.enabled = smooth;
        //             if (teleportProvider != null) teleportProvider.enabled = teleport;
        //             foreach (var go in teleportInteractorObjects) if (go != null) go.SetActive(teleport);
        //         Why also hide the interactors: a disabled TeleportationProvider still lets the ray draw an arc and
        //         queue a request; hiding the interactor removes the false promise (and the stale request).
        //         Look at: Behaviour.enabled, GameObject.SetActive.
        //         Check: in Teleport-only the left stick does nothing; in Smooth-only no arc appears when you push the
        //         right stick forward; in Hybrid both work.
        if (label != null) label.text = "Locomotion: " + mode + "  [" + cycleKey + "]";
    }
}

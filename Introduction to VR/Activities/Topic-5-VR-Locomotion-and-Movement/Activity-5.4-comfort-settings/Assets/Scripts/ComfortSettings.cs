// ComfortSettings.cs — Activity 5.4: Comfort Settings and the Rest Point
// The single source of truth for the player's comfort choices: locomotion mode, move speed, turn type, vignette
// strength, and a height offset. Loads them from PlayerPrefs at startup, saves them on request, and applies them
// to the rig's XRI providers and to the vignette quad. SettingsMenu writes to this; ComfortLog reads from it.
// Attached to: XR Origin (XR Rig) in the starter scene (providers and the vignette quad are pre-assigned).
// Created by Isac Artzi

using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class ComfortSettings : MonoBehaviour
{
    /// <summary>Which locomotion the player gets. "Both" is teleport plus smooth; a merged project would add the 5.3 dash as "Hybrid".</summary>
    public enum LocomotionMode { TeleportOnly = 0, SmoothOnly = 1, Both = 2 }

    /// <summary>Snap turn (comfortable default) or smooth turn.</summary>
    public enum TurnType { Snap = 0, Smooth = 1 }

    [Header("Current values")]
    public LocomotionMode mode = LocomotionMode.TeleportOnly;
    [Tooltip("Smooth move speed in m/s (1–4).")]
    public float moveSpeed = 1.5f;
    public TurnType turnType = TurnType.Snap;
    [Tooltip("Vignette darkness while moving, 0 = off, 1 = full tunnel.")]
    [Range(0f, 1f)] public float vignetteStrength = 0.6f;
    [Tooltip("Vertical offset added to the camera floor offset, in meters (−0.3 … +0.3). Lets a seated player stand.")]
    public float heightOffset = 0f;

    [Header("Rig wiring (found on this GameObject's children if empty)")]
    public XROrigin origin;
    public TeleportationProvider teleportProvider;
    public ContinuousMoveProvider moveProvider;
    public SnapTurnProvider snapTurnProvider;
    public ContinuousTurnProvider smoothTurnProvider;

    [Tooltip("The vignette quad under the camera. Its material alpha is set to vignetteStrength while the move provider is active.")]
    public Renderer vignetteQuad;

    // PlayerPrefs keys. Keeping them in one place means Load and Save cannot drift apart.
    const string KeyMode = "comfort.mode";
    const string KeySpeed = "comfort.speed";
    const string KeyTurn = "comfort.turn";
    const string KeyVignette = "comfort.vignette";
    const string KeyHeight = "comfort.height";

    Material vignetteMaterial;
    bool moving;
    float baseFloorOffsetY;
    bool baseOffsetCaptured;

    void Awake()
    {
        if (origin == null) origin = GetComponent<XROrigin>();
        if (teleportProvider == null) teleportProvider = GetComponentInChildren<TeleportationProvider>(true);
        if (moveProvider == null) moveProvider = GetComponentInChildren<ContinuousMoveProvider>(true);
        if (snapTurnProvider == null) snapTurnProvider = GetComponentInChildren<SnapTurnProvider>(true);
        if (smoothTurnProvider == null) smoothTurnProvider = GetComponentInChildren<ContinuousTurnProvider>(true);
        if (vignetteQuad != null) vignetteMaterial = vignetteQuad.material;
    }

    void Start()
    {
        Load();
        Apply();

        // TODO 4: Subscribe to the move provider so the vignette only shows while moving (null-check):
        //             moveProvider.locomotionStarted += OnMoveStarted;  moveProvider.locomotionEnded += OnMoveEnded;
        //         and unsubscribe in OnDestroy (fill in the two -= lines below).
        //         Look at: LocomotionProvider.locomotionStarted / locomotionEnded — the same events you used in 5.2.
        //         Check: push the move stick — the edges darken by vignetteStrength; release — they clear.
    }

    void OnDestroy()
    {
        // TODO 4 (continued): moveProvider.locomotionStarted -= OnMoveStarted; moveProvider.locomotionEnded -= OnMoveEnded; (if not null)
    }

    void OnMoveStarted(LocomotionProvider p) { moving = true; ApplyVignette(); }
    void OnMoveEnded(LocomotionProvider p) { moving = false; ApplyVignette(); }

    /// <summary>Reads every setting from PlayerPrefs, keeping the Inspector value as the default when a key is absent.</summary>
    public void Load()
    {
        // TODO 1: mode = (LocomotionMode)PlayerPrefs.GetInt(KeyMode, (int)mode);
        //         moveSpeed = PlayerPrefs.GetFloat(KeySpeed, moveSpeed);
        //         turnType = (TurnType)PlayerPrefs.GetInt(KeyTurn, (int)turnType);
        //         vignetteStrength = PlayerPrefs.GetFloat(KeyVignette, vignetteStrength);
        //         heightOffset = PlayerPrefs.GetFloat(KeyHeight, heightOffset);
        //         Look at: PlayerPrefs.GetInt(key, defaultValue), PlayerPrefs.GetFloat(key, defaultValue).
        //         Why defaults from the Inspector: the first run has no saved keys and must still produce sane values.
        //         Check: after Save() once, stop and restart Play — the values come back instead of resetting.
    }

    /// <summary>Writes every setting to PlayerPrefs and flushes to disk.</summary>
    public void Save()
    {
        // TODO 2: PlayerPrefs.SetInt(KeyMode, (int)mode); SetFloat(KeySpeed, moveSpeed); SetInt(KeyTurn, (int)turnType);
        //         SetFloat(KeyVignette, vignetteStrength); SetFloat(KeyHeight, heightOffset); then PlayerPrefs.Save().
        //         Look at: PlayerPrefs.Save() — without it, a crash before quitting can lose the values.
        //         Check: the Console line below appears, and the values survive a restart of Play mode.
        Debug.Log("[ComfortSettings] Saved: " + Describe());
    }

    /// <summary>Restores the shipped defaults (does not save — the player must confirm by pressing Save).</summary>
    public void ResetToDefaults()
    {
        mode = LocomotionMode.TeleportOnly;
        moveSpeed = 1.5f;
        turnType = TurnType.Snap;
        vignetteStrength = 0.6f;
        heightOffset = 0f;
        Apply();
    }

    /// <summary>Pushes the current values onto the providers, the vignette, and the rig height.</summary>
    public void Apply()
    {
        // TODO 3: Providers (each null-checked):
        //             teleportProvider.enabled   = mode != LocomotionMode.SmoothOnly;
        //             moveProvider.enabled       = mode != LocomotionMode.TeleportOnly;
        //             moveProvider.moveSpeed     = moveSpeed;
        //             snapTurnProvider.enabled   = turnType == TurnType.Snap;
        //             smoothTurnProvider.enabled = turnType == TurnType.Smooth;
        //         Why enable/disable the provider: a disabled provider never runs Update, so its input is ignored — one line
        //         instead of hunting for the interactor and action references inside the rig prefab.
        //         Check: in TeleportOnly the move stick does nothing; in SmoothOnly the teleport ray does nothing.

        // TODO 5: Height offset. The rig's Camera Floor Offset object sits at local y = 0 in Floor tracking mode; add the offset:
        //             var off = origin != null ? origin.CameraFloorOffsetObject : null;
        //             if (off != null) { if (!baseOffsetCaptured) { baseFloorOffsetY = off.transform.localPosition.y; baseOffsetCaptured = true; }
        //                                var lp = off.transform.localPosition; lp.y = baseFloorOffsetY + heightOffset; off.transform.localPosition = lp; }
        //         Look at: XROrigin.CameraFloorOffsetObject. Note for the headset: in Floor mode XROrigin may reset this object
        //         when tracking (re)starts — if your offset vanishes on the Quest, apply it again from OnMoveEnded or a short delay.
        //         Check: set heightOffset to 0.3 in the Inspector and call Apply — your eye height in the simulator reads 1.9 m.

        ApplyVignette();
    }

    void ApplyVignette()
    {
        if (vignetteMaterial == null) return;
        var c = vignetteMaterial.color;
        c.a = moving ? Mathf.Clamp01(vignetteStrength) : 0f;
        vignetteMaterial.color = c;
    }

    /// <summary>One-line summary used by Save() and by ComfortLog's CSV.</summary>
    public string Describe()
    {
        return mode + ", " + moveSpeed.ToString("F1") + " m/s, " + turnType + " turn, vignette " +
               vignetteStrength.ToString("F2") + ", height " + heightOffset.ToString("F2");
    }
}

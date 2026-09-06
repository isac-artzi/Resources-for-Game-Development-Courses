// SettingsMenu.cs — Activity 5.4: Comfort Settings and the Rest Point
// The world-space comfort menu: toggles for locomotion mode and turn type, sliders for speed, vignette, and height,
// and Save / Reset buttons. It shows what ComfortSettings currently holds and writes back on every change, so the
// player feels the effect immediately and only commits with Save.
// Attached to: Settings Panel (the world-space Canvas) in the starter scene; every control is pre-assigned.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("The ComfortSettings on the rig. Found at runtime if empty.")]
    public ComfortSettings settings;

    [Header("Locomotion mode (a ToggleGroup: exactly one on)")]
    public Toggle teleportToggle;
    public Toggle smoothToggle;
    public Toggle bothToggle;

    [Header("Turn type")]
    public Toggle snapToggle;
    public Toggle smoothTurnToggle;

    [Header("Sliders")]
    public Slider speedSlider;
    public Slider vignetteSlider;
    public Slider heightSlider;
    public Text speedText;
    public Text vignetteText;
    public Text heightText;

    [Header("Buttons")]
    public Button saveButton;
    public Button resetButton;
    public Text statusText;

    // True while we are pushing values INTO the UI, so the listeners do not write them straight back.
    bool refreshing;

    void Start()
    {
        if (settings == null) settings = Object.FindFirstObjectByType<ComfortSettings>();
        if (settings == null) { Debug.LogWarning("[SettingsMenu] No ComfortSettings found on the rig."); return; }

        // TODO 1: Listen to every control (null-check each):
        //             teleportToggle.onValueChanged.AddListener(on => { if (on) SetMode(ComfortSettings.LocomotionMode.TeleportOnly); });
        //             smoothToggle / bothToggle likewise with SmoothOnly / Both;
        //             snapToggle → SetTurn(TurnType.Snap) when on; smoothTurnToggle → SetTurn(TurnType.Smooth) when on;
        //             speedSlider.onValueChanged.AddListener(OnSpeed); vignetteSlider → OnVignette; heightSlider → OnHeight;
        //             saveButton.onClick.AddListener(OnSave); resetButton.onClick.AddListener(OnReset);
        //         Look at: Toggle.onValueChanged (UnityEvent<bool>), Slider.onValueChanged (UnityEvent<float>), Button.onClick.
        //         Then call RefreshFromSettings() once so the menu shows the loaded values.
        //         Check: at Play the toggles and sliders match the values on the rig's ComfortSettings component.
    }

    /// <summary>Pushes the settings values into the controls without triggering writes back.</summary>
    public void RefreshFromSettings()
    {
        if (settings == null) return;
        refreshing = true;

        // TODO 2: Set each control from settings:
        //             teleportToggle.isOn = settings.mode == ComfortSettings.LocomotionMode.TeleportOnly; (and the other two)
        //             snapToggle.isOn = settings.turnType == ComfortSettings.TurnType.Snap; smoothTurnToggle.isOn = !snapToggle.isOn;
        //             speedSlider.value = settings.moveSpeed; vignetteSlider.value = settings.vignetteStrength; heightSlider.value = settings.heightOffset;
        //         then update the three value labels (see the label formats in OnSpeed/OnVignette/OnHeight).
        //         Why the 'refreshing' flag: setting isOn/value fires onValueChanged; the guard in each handler skips the write-back.
        //         Check: press Reset — every control jumps to the defaults in one go.

        refreshing = false;
    }

    void SetMode(ComfortSettings.LocomotionMode m)
    {
        if (refreshing || settings == null) return;
        // TODO 3: settings.mode = m; settings.Apply(); SetStatus("Unsaved changes");
        //         Check: choose Smooth only — the teleport ray no longer teleports; choose Teleport only — the stick stops moving you.
    }

    void SetTurn(ComfortSettings.TurnType t)
    {
        if (refreshing || settings == null) return;
        // TODO 3 (continued): settings.turnType = t; settings.Apply(); SetStatus("Unsaved changes");
    }

    void OnSpeed(float v)
    {
        if (speedText != null) speedText.text = v.ToString("F1") + " m/s";
        if (refreshing || settings == null) return;
        // TODO 3 (continued): settings.moveSpeed = v; settings.Apply(); SetStatus("Unsaved changes");
    }

    void OnVignette(float v)
    {
        if (vignetteText != null) vignetteText.text = Mathf.RoundToInt(v * 100f) + " %";
        if (refreshing || settings == null) return;
        // TODO 3 (continued): settings.vignetteStrength = v; settings.Apply(); SetStatus("Unsaved changes");
    }

    void OnHeight(float v)
    {
        if (heightText != null) heightText.text = (v >= 0f ? "+" : "") + v.ToString("F2") + " m";
        if (refreshing || settings == null) return;
        // TODO 3 (continued): settings.heightOffset = v; settings.Apply(); SetStatus("Unsaved changes");
    }

    void OnSave()
    {
        // TODO 4: settings.Save(); SetStatus("Saved: " + settings.Describe());
        //         Check: stop Play, start again — the menu comes up with your saved values, not the defaults.
    }

    void OnReset()
    {
        // TODO 4 (continued): settings.ResetToDefaults(); RefreshFromSettings(); SetStatus("Defaults restored (not saved)");
    }

    void SetStatus(string s)
    {
        if (statusText != null) statusText.text = s;
    }
}

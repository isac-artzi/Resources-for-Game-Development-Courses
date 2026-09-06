// BeatStation.cs — Activity 1.3: Story Beats in Space
// One physical station where a story beat is staged. When the sequencer activates it, its beacon light brightens
// and its title label fades in over a second; when the story moves on, both ease back down. Light and motion are
// how a VR scene says "look here" without a HUD arrow.
// Attached to: Station 1–5 in the starter scene (Beacon and Title Label are pre-set on each).
// Created by Isac Artzi

using UnityEngine;

public class BeatStation : MonoBehaviour
{
    [Header("Parts")]
    [Tooltip("The point light above the station. Brightens when the beat is active.")]
    public Light beacon;

    [Tooltip("The TextMesh above the station showing the beat's title.")]
    public TextMesh titleLabel;

    [Header("Levels")]
    [Tooltip("Beacon intensity while this beat is active.")]
    public float litIntensity = 3f;

    [Tooltip("Beacon intensity while the story is elsewhere. Not zero: a faint glow keeps the path readable.")]
    public float dimIntensity = 0.15f;

    [Tooltip("Label alpha while active / inactive.")]
    [Range(0f, 1f)] public float litLabelAlpha = 1f;
    [Range(0f, 1f)] public float dimLabelAlpha = 0.15f;

    [Header("Timing")]
    [Tooltip("Seconds for the fade in either direction.")]
    public float fadeSeconds = 1.2f;

    /// <summary>True while this station is the active beat.</summary>
    public bool IsActive { get; private set; }

    // Where the fade started and where it is going, and how long it has been running.
    float fromIntensity, toIntensity;
    float fromAlpha, toAlpha;
    float elapsed;

    void Start()
    {
        if (beacon != null) beacon.intensity = dimIntensity;
        SetLabelAlpha(dimLabelAlpha);
        fromIntensity = toIntensity = dimIntensity;
        fromAlpha = toAlpha = dimLabelAlpha;
        elapsed = fadeSeconds;   // nothing in flight
    }

    /// <summary>Writes the beat's title into the label (called by the sequencer before activating).</summary>
    public void SetTitle(string title)
    {
        if (titleLabel != null) titleLabel.text = title;
    }

    /// <summary>Starts a fade toward the lit or the dim state. Safe to call every time the story advances.</summary>
    public void SetActive(bool active)
    {
        IsActive = active;

        // TODO 1: Begin a new fade from the CURRENT values (not from the old target) so a fast [ ] [ ] does not snap.
        //             fromIntensity = beacon != null ? beacon.intensity : dimIntensity;
        //             toIntensity   = active ? litIntensity : dimIntensity;
        //             fromAlpha     = titleLabel != null ? titleLabel.color.a : dimLabelAlpha;
        //             toAlpha       = active ? litLabelAlpha : dimLabelAlpha;
        //             elapsed       = 0f;
        //         Why start from the current value: if the previous fade was half done, jumping to its end first
        //         produces a visible pop, and pops pull attention exactly when you want it elsewhere.
        //         Check: mash ] and [ quickly — lights glide, they never flash.
    }

    void Update()
    {
        if (elapsed >= fadeSeconds) return;
        elapsed += Time.deltaTime;

        // Progress of the fade, 0..1. Until TODO 3 it is always 1, so changes snap instantly.
        float s = 1f;

        // TODO 2: Apply the progress to the beacon and the label.
        //             if (beacon != null) beacon.intensity = Mathf.Lerp(fromIntensity, toIntensity, s);
        //             SetLabelAlpha(Mathf.Lerp(fromAlpha, toAlpha, s));
        //         Look at: Mathf.Lerp(a, b, t) = a + (b - a) * t.
        //         Check: with s still 1, the active station snaps to full brightness and the previous one snaps to dim.

        // TODO 3: Replace the constant s above with an EASED progress computed from elapsed time:
        //             float t = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, fadeSeconds));
        //             s = Mathf.SmoothStep(0f, 1f, t);
        //         (Move these two lines ABOVE TODO 2 so the Lerps use them.)
        //         Why SmoothStep and not plain t: SmoothStep starts and ends with zero speed (3t^2 - 2t^3), so the light
        //         "settles" instead of stopping dead. Linear fades look mechanical; eased fades look like light.
        //         Look at: Mathf.SmoothStep, Mathf.Clamp01.
        //         Check: the active station's light takes about 1.2 s to reach full brightness and the previous one
        //         dims over the same time; change Fade Seconds to 3 in the Inspector and both slow down.
        if (s >= 1f) elapsed = fadeSeconds;
    }

    /// <summary>Sets only the alpha of the label color (TextMesh has no separate alpha property).</summary>
    void SetLabelAlpha(float a)
    {
        if (titleLabel == null) return;
        Color c = titleLabel.color;
        c.a = a;
        titleLabel.color = c;
    }
}

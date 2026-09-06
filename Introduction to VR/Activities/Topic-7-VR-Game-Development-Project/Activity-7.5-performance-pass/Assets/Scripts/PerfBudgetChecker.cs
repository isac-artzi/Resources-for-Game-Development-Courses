// PerfBudgetChecker.cs — Activity 7.5: Performance Pass for Quest 3
// A small heads-up readout that lives in front of the camera and tells you, every frame, whether you are
// inside the frame budget: smoothed frame time in ms against 1000 / target Hz, colored green / yellow / red,
// plus a once-a-second count of visible renderers and unique materials (a lower bound on draw calls).
// Attached to: "Perf HUD" (a TextMesh parented to the XR camera) in the starter scene.
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PerfBudgetChecker : MonoBehaviour
{
    [Header("Budget")]
    [Tooltip("Target refresh rate. Quest 3 default is 72 Hz (13.9 ms); 90 Hz = 11.1 ms; 120 Hz = 8.3 ms.")]
    public float targetHz = 72f;

    [Tooltip("Fraction of the budget above which the readout turns yellow (leave headroom for the compositor).")]
    [Range(0.5f, 1f)] public float warnFraction = 0.85f;

    [Header("Smoothing")]
    [Tooltip("Exponential moving average weight for the newest frame. 0.1 = smooth, 0.5 = twitchy.")]
    [Range(0.01f, 1f)] public float smoothing = 0.1f;

    [Header("Scene counts")]
    [Tooltip("Seconds between renderer/material counts (they walk the whole scene — do not do it every frame).")]
    public float countIntervalSeconds = 1f;

    [Header("Output")]
    [Tooltip("The TextMesh to write into. Empty = the one on this GameObject.")]
    public TextMesh hud;

    public Color okColor = new Color(0.5f, 1f, 0.5f);
    public Color warnColor = new Color(1f, 0.9f, 0.3f);
    public Color overColor = new Color(1f, 0.35f, 0.3f);

    [Tooltip("Desktop key that hides/shows the HUD.")]
    public Key toggleKey = Key.F1;

    [Header("Read-only")]
    public float smoothedMs;
    public float worstMs;
    public int visibleRenderers;
    public int uniqueMaterials;

    float countTimer;
    bool visible = true;

    /// <summary>Frame budget in milliseconds: 1000 / targetHz.</summary>
    public float BudgetMs
    {
        get { return 1000f / Mathf.Max(1f, targetHz); }
    }

    void Start()
    {
        if (hud == null) hud = GetComponent<TextMesh>();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb != null && kb[toggleKey].wasPressedThisFrame)
        {
            visible = !visible;
            if (hud != null) hud.gameObject.SetActive(visible);
        }

        // TODO 1: Measure. float ms = Time.unscaledDeltaTime * 1000f;
        //         smoothedMs = (smoothedMs <= 0f) ? ms : Mathf.Lerp(smoothedMs, ms, smoothing);
        //         if (ms > worstMs) worstMs = ms;
        //         Look at: Time.unscaledDeltaTime (real time, unaffected by Time.timeScale). Why smooth: single frames
        //         jitter by a millisecond or two; an exponential moving average shows the trend without hiding spikes,
        //         and worstMs keeps the spike so you can go looking for its cause.
        //         Check: the HUD shows a steady number around 3–8 ms on a laptop in the Editor; alt-tab away and back
        //         and 'Worst Ms' jumps.

        // TODO 2: Count, once a second: countTimer += Time.unscaledDeltaTime; if (countTimer >= countIntervalSeconds)
        //         { countTimer = 0f; CountScene(); }
        //         Check: Visible Renderers changes as you turn the head away from the courtyard.

        // TODO 3: Write. Build the text (one value per line):
        //             "Frame " + smoothedMs.ToString("F1") + " ms  (" + (1000f / Mathf.Max(0.01f, smoothedMs)).ToString("F0") + " fps)"
        //             "Budget " + BudgetMs.ToString("F1") + " ms @ " + targetHz + " Hz"
        //             "Worst " + worstMs.ToString("F1") + " ms"
        //             "Visible renderers " + visibleRenderers + "   unique materials " + uniqueMaterials
        //         Then hud.text = that, and hud.color = ColorFor(smoothedMs).
        //         Check: the text is green in the starter scene on a desktop; it turns yellow, then red if you set
        //         Target Hz to 500 (a deliberately impossible budget) — put it back afterwards.
        if (hud != null && hud.text.Length == 0) hud.text = "Perf HUD: implement PerfBudgetChecker TODO 1-3";
    }

    /// <summary>Green under warnFraction of the budget, yellow up to the budget, red above it.</summary>
    public Color ColorFor(float ms)
    {
        // TODO 4: float ratio = ms / BudgetMs;  return ratio < warnFraction ? okColor : (ratio < 1f ? warnColor : overColor);
        //         Why a warn band: the headset's compositor needs ~1–2 ms of every frame too, so "just under 13.9" is already late.
        //         Check: see TODO 3.
        return okColor;
    }

    /// <summary>Walks the scene once: visible renderers and distinct shared materials among them.</summary>
    public void CountScene()
    {
        var renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        var materials = new HashSet<Material>();
        int vis = 0;
        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r.isVisible) continue;
            vis++;
            var mats = r.sharedMaterials;
            for (int m = 0; m < mats.Length; m++) if (mats[m] != null) materials.Add(mats[m]);
        }
        visibleRenderers = vis;
        uniqueMaterials = materials.Count;
    }

    /// <summary>Resets the worst-frame record (call after loading, when spikes are expected).</summary>
    public void ResetWorst()
    {
        worstMs = 0f;
    }
}

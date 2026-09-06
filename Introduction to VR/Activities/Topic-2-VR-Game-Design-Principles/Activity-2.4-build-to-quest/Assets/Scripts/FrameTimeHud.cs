// FrameTimeHud.cs — Activity 2.4: Build to Quest and Measure
// A head-locked debug readout of frame time (ms) and frames per second, smoothed over a window of recent frames and
// colored against the 72 Hz budget. Head-locked UI is normally avoided in VR (see the README), but a tiny debug
// HUD you can read while walking around a stress test is the one place it earns its keep.
// Attached to: Frame Time HUD (a TextMesh parented under the Main Camera, 1.5 m ahead, 0.35 m below center).
// Created by Isac Artzi

using UnityEngine;

public class FrameTimeHud : MonoBehaviour
{
    [Header("Where to print")]
    [Tooltip("The TextMesh to write into. Pre-set to the one on this GameObject.")]
    public TextMesh label;

    [Header("Smoothing")]
    [Tooltip("How many recent frames to average. 60 frames is ~0.8 s at 72 Hz — steady but still responsive.")]
    public int windowSize = 60;

    [Header("Budget")]
    [Tooltip("Frame budget in milliseconds. 13.9 ms = 72 Hz, 11.1 ms = 90 Hz, 8.3 ms = 120 Hz.")]
    public float budgetMs = 13.9f;

    public Color okColor = new Color(0.6f, 1f, 0.6f);
    public Color overBudgetColor = new Color(1f, 0.45f, 0.4f);

    [Header("Optional")]
    [Tooltip("If set, the HUD also shows how many stress objects exist.")]
    public StressSpawner spawner;

    // Ring buffer of recent frame times in seconds.
    float[] samples;
    int nextIndex;
    int filled;
    float sum;

    /// <summary>Average frame time over the window, in milliseconds.</summary>
    public float SmoothedMs { get; private set; }

    /// <summary>Frames per second implied by SmoothedMs.</summary>
    public float SmoothedFps { get; private set; }

    /// <summary>True when the smoothed frame time exceeds the budget.</summary>
    public bool OverBudget { get { return SmoothedMs > budgetMs; } }

    void Start()
    {
        if (label == null) label = GetComponent<TextMesh>();
        samples = new float[Mathf.Max(1, windowSize)];
    }

    void Update()
    {
        // TODO 1: Record this frame's duration in the ring buffer. Use UNSCALED time so a paused game still measures:
        //             float dt = Time.unscaledDeltaTime;
        //             sum -= samples[nextIndex];            // drop the value we are about to overwrite
        //             samples[nextIndex] = dt;  sum += dt;
        //             nextIndex = (nextIndex + 1) % samples.Length;
        //             filled = Mathf.Min(filled + 1, samples.Length);
        //         Look at: Time.unscaledDeltaTime. Why a ring buffer: a running sum over the last N values costs one
        //         subtraction and one addition per frame, no matter how large N is.
        //         Check: after one second 'filled' equals windowSize (watch it in the Inspector's Debug mode).

        // TODO 2: Moving average -> milliseconds and FPS:
        //             if (filled > 0) { float avg = sum / filled; SmoothedMs = avg * 1000f; SmoothedFps = 1f / avg; }
        //         Check: in the Editor with VSync on you read roughly 16.7 ms / 60 FPS (or your monitor's rate).

        // TODO 3: Print and color. Build the text with two lines, e.g.
        //             SmoothedMs.ToString("F1") + " ms   " + SmoothedFps.ToString("F0") + " FPS\n" +
        //             "budget " + budgetMs.ToString("F1") + " ms   objects " + (spawner != null ? spawner.Count : 0)
        //         and set label.color = OverBudget ? overBudgetColor : okColor.
        //         Check: spawn wisps (B) until the text turns red; clear (C) and it turns green again.
    }
}

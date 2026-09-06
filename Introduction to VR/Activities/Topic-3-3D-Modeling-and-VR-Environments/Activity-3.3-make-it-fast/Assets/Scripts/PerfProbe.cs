// PerfProbe.cs — Activity 3.3: Make It Fast: LOD, Occlusion, Lightmaps
// A head-locked debug HUD: smoothed frame time in milliseconds and FPS, plus the CullingReporter's counts.
// Press P to write a snapshot row (label, ms, fps, visible, in-frustum, total) to the Console and to a CSV in
// Application.persistentDataPath — the rows for your before/after table.
// Attached to: "Perf" in the starter scene (Readout is a TextMesh parented to the camera; Reporter is pre-set).
// Created by Isac Artzi

using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class PerfProbe : MonoBehaviour
{
    [Header("Display")]
    [Tooltip("The TextMesh HUD parented to the camera. Set in the starter scene.")]
    public TextMesh readout;

    [Tooltip("Smoothing factor for the frame-time average, 0..1. Smaller = smoother but slower to react. 0.05 ~ 1 second.")]
    [Range(0.01f, 1f)]
    public float smoothing = 0.05f;

    [Tooltip("Frame budget to compare against, in ms. 13.9 ms is 72 Hz on the Quest 3.")]
    public float budgetMs = 13.9f;

    [Header("Sources")]
    [Tooltip("Where the renderer counts come from. Set in the starter scene.")]
    public CullingReporter reporter;

    [Header("Snapshots")]
    [Tooltip("Label written with each snapshot row, e.g. 'before', 'lod', 'lod+occlusion', 'lod+occlusion+lightmaps'.")]
    public string snapshotLabel = "before";

    [Tooltip("File name inside Application.persistentDataPath.")]
    public string csvFileName = "perf_3_3.csv";

    /// <summary>Exponentially smoothed frame time in milliseconds.</summary>
    public float SmoothedMs { get; private set; }

    void Start()
    {
        SmoothedMs = Time.unscaledDeltaTime * 1000f;
    }

    void Update()
    {
        // TODO 1: Smooth the frame time. Raw deltaTime jumps around; an exponential moving average is stable enough to read:
        //             float ms = Time.unscaledDeltaTime * 1000f;
        //             SmoothedMs += (ms - SmoothedMs) * smoothing;
        //         Look at: Time.unscaledDeltaTime (ignores Time.timeScale, so pausing does not fake a good number).
        //         Check: the HUD settles to a steady value within a second and does not flicker every frame.

        // TODO 2: Write the HUD. Two lines:
        //             line 1: SmoothedMs.ToString("F1") + " ms  (" + (1000f / SmoothedMs).ToString("F0") + " fps)  budget " + budgetMs + " ms"
        //             line 2: reporter != null ? reporter.Summary() : "no reporter"
        //         Color the text red when SmoothedMs > budgetMs, white otherwise (readout.color).
        //         Check: on a laptop the first line reads a few ms and stays white; maximize the Game view at high resolution
        //         and add shadows to the sun and it may cross the budget and turn red.
        if (readout != null) readout.text = "PerfProbe: implement TODO 1-3";

        // TODO 3: Snapshot on P (Input System). if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame) Snapshot();
        //         Check: pressing P prints a CSV row to the Console and appends it to the file whose path is also printed.
    }

    /// <summary>Appends one CSV row for the current view. Change snapshotLabel between measurements.</summary>
    public void Snapshot()
    {
        string row = snapshotLabel + "," + SmoothedMs.ToString("F2") + "," + (1000f / Mathf.Max(0.01f, SmoothedMs)).ToString("F0") + ","
                   + (reporter != null ? reporter.Visible.ToString() : "") + ","
                   + (reporter != null ? reporter.InFrustum.ToString() : "") + ","
                   + (reporter != null ? reporter.TotalRenderers.ToString() : "");

        string path = Path.Combine(Application.persistentDataPath, csvFileName);
        if (!File.Exists(path))
            File.WriteAllText(path, "label,ms,fps,visible,inFrustum,total\n");
        File.AppendAllText(path, row + "\n");

        Debug.Log("[PerfProbe] " + row + "   -> " + path);
    }
}

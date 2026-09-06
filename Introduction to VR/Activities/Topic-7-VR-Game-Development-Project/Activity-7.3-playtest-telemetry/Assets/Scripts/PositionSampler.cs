// PositionSampler.cs — Activity 7.3: Playtest Telemetry and Heatmap
// Records where the player's head is every few tenths of a second, feeds each sample to the HeatmapGrid,
// and writes all samples to a CSV file in Application.persistentDataPath when the session ends.
// Attached to: "Telemetry" in the starter scene (Grid is pre-wired; Head is found at runtime).
// Created by Isac Artzi

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class PositionSampler : MonoBehaviour
{
    [Header("Sampling")]
    [Tooltip("Seconds between samples. 0.5 s at walking speed (1.5 m/s) is one sample every 0.75 m.")]
    public float intervalSeconds = 0.5f;

    [Tooltip("What to sample. Empty = the main camera (the head).")]
    public Transform head;

    [Tooltip("Every sample is also binned into this grid.")]
    public HeatmapGrid grid;

    [Header("Desktop keys")]
    [Tooltip("Keyboard key that spawns the heatmap cubes in the world (HeatmapGrid.SpawnCubes).")]
    public Key spawnCubesKey = Key.H;

    [Header("Output")]
    [Tooltip("CSV file name inside Application.persistentDataPath. Written when Play stops.")]
    public string fileName = "positions.csv";

    [Tooltip("Write the CSV at the end of the session.")]
    public bool writeCsv = true;

    [Header("Read-only")]
    [Tooltip("Samples taken so far this session.")]
    public int sampleCount;

    readonly List<Vector3> samples = new List<Vector3>();
    readonly List<float> times = new List<float>();
    float timer;

    void Start()
    {
        if (head == null && Camera.main != null) head = Camera.main.transform;
        if (grid == null) grid = GetComponent<HeatmapGrid>();
    }

    void Update()
    {
        if (head == null) return;

        // TODO 1: A simple timer. Add Time.deltaTime to 'timer'; when timer >= intervalSeconds, subtract
        //         intervalSeconds (do not reset to 0 — subtracting keeps the average rate exact) and call SampleNow().
        //         Look at: Time.deltaTime, Time.time. Why sample at intervals and not every frame: 72 samples per
        //         second would be 4 000 rows a minute and would weight standing still exactly like walking — the
        //         heatmap should show WHERE time was spent, and 2 Hz is plenty for that.
        //         Check: 'Sample Count' in the Inspector rises by 2 every second while you play.

        // Desktop convenience: H spawns the cubes so you can see the heatmap in the Game view.
        var kb = Keyboard.current;
        if (kb != null && grid != null && kb[spawnCubesKey].wasPressedThisFrame) grid.SpawnCubes();
    }

    /// <summary>Records one sample: stores it and hands it to the grid.</summary>
    public void SampleNow()
    {
        Vector3 p = head.position;
        samples.Add(p);
        times.Add(Time.time);
        sampleCount = samples.Count;
        if (grid != null) grid.AddSample(p);
    }

    void OnDisable()
    {
        // Runs when Play stops (and when the object is disabled). Good moment to flush to disk.
        if (writeCsv && samples.Count > 0) WriteCsv();
    }

    /// <summary>Full path of the CSV.</summary>
    public string FullPath
    {
        get { return Path.Combine(Application.persistentDataPath, fileName); }
    }

    /// <summary>Writes "t,x,y,z" rows to FullPath.</summary>
    public void WriteCsv()
    {
        // TODO 2: Build the text with a StringBuilder: first line "t,x,y,z", then one line per sample:
        //             times[i].ToString("F2", CultureInfo.InvariantCulture) + "," + x + "," + y + "," + z
        //         (format each float with "F3" and CultureInfo.InvariantCulture so the decimal point is always '.').
        //         Then File.WriteAllText(FullPath, sb.ToString()) and Debug.Log the path and the row count.
        //         Look at: System.Text.StringBuilder, File.WriteAllText, CultureInfo.InvariantCulture.
        //         Why invariant culture: on a laptop set to German or French, 1.5 would print as "1,5" and break the CSV.
        //         Check: stop Play, open the file in a spreadsheet — as many rows as Sample Count, 4 columns.
        Debug.Log("[PositionSampler] " + samples.Count + " samples — implement TODO 2 to write " + FullPath);
    }
}

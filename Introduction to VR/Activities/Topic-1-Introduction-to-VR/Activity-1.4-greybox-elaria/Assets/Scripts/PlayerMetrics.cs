// PlayerMetrics.cs — Activity 1.4: Greybox Elaria
// Measures how the player actually uses the greybox: seconds spent and meters walked in each zone, plus the totals.
// Press a key to print the table to the Console. These numbers ("the forest path takes 25 s to walk") go into
// your project plan and later into the comfort and playtest reports.
// Attached to: "Player Metrics" in the starter scene (Readout is pre-set).
// Created by Isac Artzi

using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMetrics : MonoBehaviour
{
    /// <summary>Accumulated numbers for one zone.</summary>
    public class ZoneStats
    {
        public float seconds;
        public float meters;
        public int entries;
    }

    [Header("Readout")]
    [Tooltip("Optional TextMesh showing the current zone and running totals. Leave empty to log only.")]
    public TextMesh readout;

    [Header("Filtering")]
    [Tooltip("A per-frame move longer than this (meters) is treated as a teleport or a reset, not as walking, and is ignored.")]
    public float teleportThreshold = 3f;

    [Header("Keys (Input System)")]
    [Tooltip("Print the metrics table to the Console.")]
    public Key reportKey = Key.Enter;

    [Tooltip("Clear all metrics and start over.")]
    public Key resetKey = Key.Backspace;

    /// <summary>Name of the zone the head is currently in, or "Outside".</summary>
    public string CurrentZone { get; private set; }

    readonly Dictionary<string, ZoneStats> stats = new Dictionary<string, ZoneStats>();
    Transform head;
    Vector3 lastHeadPos;
    float totalSeconds;
    float totalMeters;

    void Start()
    {
        head = Camera.main != null ? Camera.main.transform : null;
        if (head != null) lastHeadPos = head.position;
        CurrentZone = "Outside";
    }

    /// <summary>Called by a ZoneTrigger when the head enters it.</summary>
    public void EnterZone(string zoneName)
    {
        CurrentZone = zoneName;
        Get(zoneName).entries++;
        Debug.Log("[Metrics] enter " + zoneName);
    }

    /// <summary>Called by a ZoneTrigger when the head leaves it.</summary>
    public void ExitZone(string zoneName)
    {
        if (CurrentZone == zoneName) CurrentZone = "Outside";
        Debug.Log("[Metrics] exit " + zoneName);
    }

    void Update()
    {
        if (head == null) return;

        // TODO 1: Distance walked this frame, measured on the floor plane (ignore vertical head bobbing and crouching):
        //             Vector3 delta = head.position - lastHeadPos; delta.y = 0f;
        //             float step = delta.magnitude;
        //             lastHeadPos = head.position;
        //             if (step > teleportThreshold) step = 0f;     // a jump, not a walk
        //         Why filter jumps: when the scene starts or the rig is snapped somewhere, one frame can report 20 m
        //         of "walking". A human walks about 1.4 m/s, so anything over 3 m in one frame is not a step.
        //         Look at: Vector3.magnitude.
        float step = 0f;

        // TODO 2: Accumulate. Always add to the totals; add to the current zone's stats when inside one:
        //             totalSeconds += Time.deltaTime; totalMeters += step;
        //             if (CurrentZone != "Outside") { var z = Get(CurrentZone); z.seconds += Time.deltaTime; z.meters += step; }
        //         Check: after walking the forest path, Enter prints a Forest row with roughly 20 m and 15-30 s.
        totalMeters += step;   // placeholder that compiles; TODO 2 replaces it

        // TODO 3: Keys. if (Keyboard.current != null) { if (Keyboard.current[reportKey].wasPressedThisFrame) Debug.Log(BuildReport());
        //                                              if (Keyboard.current[resetKey].wasPressedThisFrame) ResetAll(); }
        //         Check: Enter prints the table; Backspace empties it.

        // TODO 4: Live readout (if readout != null): CurrentZone + "\n" + totalMeters.ToString("F1") + " m   " +
        //         totalSeconds.ToString("F0") + " s". Position it like the announcer's banner? No — this one can be a fixed
        //         sign near the start, so leave its transform alone and only set the text.
        //         Check: the sign near the start shows your current zone and your running distance and time.
        if (readout != null) readout.text = "Metrics: implement TODO 1-5 in PlayerMetrics.cs";
    }

    /// <summary>Formats the metrics as a text table: zone, entries, seconds, meters, average speed.</summary>
    public string BuildReport()
    {
        var sb = new StringBuilder();
        sb.AppendLine("[Metrics] zone | entries | seconds | meters | m/s");

        // TODO 5: One line per zone. foreach (var kv in stats) with kv.Key and kv.Value:
        //             float speed = kv.Value.seconds > 0f ? kv.Value.meters / kv.Value.seconds : 0f;
        //             sb.AppendLine(kv.Key + " | " + kv.Value.entries + " | " + kv.Value.seconds.ToString("F1") + " | "
        //                           + kv.Value.meters.ToString("F1") + " | " + speed.ToString("F2"));
        //         Then a TOTAL line with totalSeconds and totalMeters.
        //         Why m/s: the switchback should come out slower than the forest path; if it does not, your ramp is
        //         too easy to be a mountain. Design decisions hide in this column.
        //         Check: the Console shows one row per zone you visited, and a walking speed near 1-1.5 m/s in the simulator.

        return sb.ToString();
    }

    /// <summary>Clears every number (used by the reset key).</summary>
    public void ResetAll()
    {
        stats.Clear();
        totalSeconds = 0f;
        totalMeters = 0f;
        Debug.Log("[Metrics] reset");
    }

    ZoneStats Get(string zoneName)
    {
        ZoneStats z;
        if (!stats.TryGetValue(zoneName, out z))
        {
            z = new ZoneStats();
            stats[zoneName] = z;
        }
        return z;
    }

    void OnDisable()
    {
        // Leaving Play mode prints the final table so you never lose a run.
        if (totalSeconds > 0f) Debug.Log(BuildReport());
    }
}

// PerfLogger.cs — Activity 2.4: Build to Quest and Measure
// Samples frame time and object count at a fixed interval and writes them as CSV to Application.persistentDataPath —
// on the desktop a folder under your user profile, on the Quest the app's private files folder you fetch with adb.
// This CSV is the raw data for the optional performance metrics in Milestone 2 and for the before/after table in 3.3.
// Attached to: Perf Tools (next to StressSpawner). HUD and spawner are pre-wired.
// Created by Isac Artzi

using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class PerfLogger : MonoBehaviour
{
    [Header("Sources")]
    [Tooltip("Provides the smoothed frame time.")]
    public FrameTimeHud hud;

    [Tooltip("Provides the object count and the unique-materials flag.")]
    public StressSpawner spawner;

    [Header("Sampling")]
    [Tooltip("Seconds between rows. 0.5 s gives ~120 rows per minute — plenty for a slope.")]
    public float sampleInterval = 0.5f;

    [Tooltip("Start logging as soon as Play begins.")]
    public bool logOnStart = true;

    [Header("Output")]
    [Tooltip("File name prefix; a timestamp and .csv are appended.")]
    public string fileNamePrefix = "perf";

    [Tooltip("Desktop key that saves the CSV now (it is also saved automatically when Play stops).")]
    public Key saveKey = Key.L;

    readonly StringBuilder rows = new StringBuilder();
    float timer;
    float elapsed;
    int rowCount;

    /// <summary>True while rows are being collected.</summary>
    public bool IsLogging { get; private set; }

    /// <summary>Full path of the last CSV written, or null.</summary>
    public string LastPath { get; private set; }

    void Start()
    {
        rows.Length = 0;
        rows.AppendLine("time_s,object_count,frame_ms,fps,unique_materials,platform");
        IsLogging = logOnStart;
        Debug.Log("PerfLogger: CSV files go to " + Application.persistentDataPath);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current[saveKey].wasPressedThisFrame) Save();
        if (!IsLogging || hud == null) return;

        elapsed += Time.unscaledDeltaTime;
        timer += Time.unscaledDeltaTime;
        if (timer < sampleInterval) return;
        timer = 0f;

        // TODO 1: Append one CSV row. Use the invariant culture so the decimal separator is always '.', otherwise a
        //         spreadsheet on a German or Turkish laptop reads "13,9" as two columns:
        //             var ic = System.Globalization.CultureInfo.InvariantCulture;
        //             rows.AppendLine(string.Join(",", new string[] {
        //                 elapsed.ToString("F2", ic),
        //                 (spawner != null ? spawner.Count : 0).ToString(ic),
        //                 hud.SmoothedMs.ToString("F2", ic),
        //                 hud.SmoothedFps.ToString("F1", ic),
        //                 (spawner != null && spawner.uniqueMaterials) ? "1" : "0",
        //                 Application.platform.ToString() }));
        //             rowCount++;
        //         Look at: StringBuilder.AppendLine, string.Join, float.ToString(string format, IFormatProvider).
        //         Check: after 10 s of Play, rowCount reads about 20 in the Inspector (Debug mode).
    }

    /// <summary>Writes the collected rows to a timestamped CSV in persistentDataPath and logs the path.</summary>
    public void Save()
    {
        // TODO 2: Write the file:
        //             string name = fileNamePrefix + "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        //             LastPath = Path.Combine(Application.persistentDataPath, name);
        //             File.WriteAllText(LastPath, rows.ToString());
        //             Debug.Log("PerfLogger: wrote " + rowCount + " rows to " + LastPath);
        //         Look at: Application.persistentDataPath (writable on every platform, survives app updates),
        //         System.IO.File.WriteAllText, System.IO.Path.Combine.
        //         On the Quest the folder is /sdcard/Android/data/<your bundle id>/files/ — fetch it with
        //             adb pull /sdcard/Android/data/<bundle id>/files/ .
        //         Check: press L, then open the folder printed in the Console — the CSV opens in any spreadsheet.
        Debug.Log("PerfLogger.Save: implement TODO 2 (" + rowCount + " rows collected)");
    }

    /// <summary>
    /// Slope of frame time against object count, in milliseconds per 100 objects, from two samples.
    /// Example: 200 objects at 9.0 ms and 1200 objects at 14.0 ms -> 0.5 ms per 100 objects.
    /// </summary>
    public static float SlopeMsPerHundred(int count1, float ms1, int count2, float ms2)
    {
        // TODO 3: if (count2 == count1) return 0f;  return (ms2 - ms1) / (count2 - count1) * 100f;
        //         You will apply this by hand to two rows of your CSV for the Padlet post; having it in code lets you
        //         sanity-check the arithmetic in the Console.
        //         Check: SlopeMsPerHundred(200, 9f, 1200, 14f) == 0.5f.
        return 0f;
    }

    void OnDisable()
    {
        // Save when Play stops or the app quits, so a headset session is never lost.
        if (rowCount > 0) Save();
    }
}

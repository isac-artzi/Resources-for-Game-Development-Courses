// HeatmapGrid.cs — Activity 7.3: Playtest Telemetry and Heatmap
// Bins world positions into a grid of square cells on the floor and draws the result two ways:
//   1. as Gizmos in the Scene view (OnDrawGizmos) — live in Play mode, and in Edit mode after "Load CSV";
//   2. as real colored cubes in the world (Spawn Cubes) so a tester can see it inside the headset.
// Attached to: "Telemetry" in the starter scene (origin/size already cover the whole course).
// Created by Isac Artzi

using System.Globalization;
using System.IO;
using UnityEngine;

public class HeatmapGrid : MonoBehaviour
{
    [Header("Grid")]
    [Tooltip("World position of the grid's minimum corner (x, z). y is the floor height.")]
    public Vector3 origin = new Vector3(-15f, 0f, -15f);

    [Tooltip("Cell size in meters. 1 m is coarse but readable; 0.5 m shows paths more sharply.")]
    public float cellSize = 1f;

    [Tooltip("Cells along x.")]
    public int columns = 30;

    [Tooltip("Cells along z.")]
    public int rows = 30;

    [Header("Look")]
    [Tooltip("Color for the least-visited cells that were visited at all.")]
    public Color cold = new Color(0.2f, 0.4f, 1f, 0.6f);

    [Tooltip("Color for the most-visited cell.")]
    public Color hot = new Color(1f, 0.2f, 0.1f, 0.9f);

    [Tooltip("Height in meters of the most-visited cell's cube.")]
    public float maxCubeHeight = 1.5f;

    [Tooltip("Draw the gizmo heatmap while playing (turn off if the Scene view gets busy).")]
    public bool drawGizmos = true;

    [Header("Data source for Edit mode")]
    [Tooltip("CSV written by PositionSampler (file name inside persistentDataPath) for the 'Load CSV' context menu.")]
    public string csvFileName = "positions.csv";

    [Header("Read-only")]
    public int maxCount;
    public int totalSamples;

    int[] counts;
    Transform cubesRoot;

    void EnsureArray()
    {
        if (counts == null || counts.Length != columns * rows) counts = new int[columns * rows];
    }

    /// <summary>Adds one world-space sample to the grid. Samples outside the grid are ignored.</summary>
    public void AddSample(Vector3 world)
    {
        EnsureArray();
        // TODO 1: Bin the sample. Compute
        //             int ix = Mathf.FloorToInt((world.x - origin.x) / cellSize);
        //             int iz = Mathf.FloorToInt((world.z - origin.z) / cellSize);
        //         If ix or iz is outside 0..columns-1 / 0..rows-1, return. Otherwise
        //             int idx = iz * columns + ix; counts[idx]++; totalSamples++;
        //             if (counts[idx] > maxCount) maxCount = counts[idx];
        //         Look at: Mathf.FloorToInt (NOT (int) casting — casting rounds toward zero, which puts -0.4 and +0.4 in the same cell).
        //         Check: walk in a small circle for 10 s; 'Max Count' and 'Total Samples' rise in the Inspector.
    }

    /// <summary>Normalized heat 0..1 for a cell count.</summary>
    public float Heat(int count)
    {
        if (maxCount <= 0 || count <= 0) return 0f;
        return Mathf.Clamp01((float)count / maxCount);
    }

    /// <summary>Color for a count: a ramp from cold to hot.</summary>
    public Color ColorFor(int count)
    {
        // TODO 2: Return Color.Lerp(cold, hot, t) where t = Heat(count).
        //         Optional but recommended: use Mathf.Sqrt(t) instead of t. A square-root ramp lifts the low
        //         values so that a cell visited once still shows as clearly blue instead of nearly invisible.
        //         Look at: Color.Lerp, Mathf.Sqrt. Check: the path you walked most is red, side trips are blue.
        return cold;
    }

    /// <summary>World-space center of a cell's floor square.</summary>
    public Vector3 CellCenter(int ix, int iz)
    {
        return origin + new Vector3((ix + 0.5f) * cellSize, 0f, (iz + 0.5f) * cellSize);
    }

    void OnDrawGizmos()
    {
        // The grid outline is always drawn so you can see what the sampler covers.
        Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
        Vector3 size = new Vector3(columns * cellSize, 0.02f, rows * cellSize);
        Gizmos.DrawWireCube(origin + new Vector3(size.x * 0.5f, 0f, size.z * 0.5f), size);

        if (!drawGizmos || counts == null) return;

        // TODO 3: Draw one cube per visited cell. Loop iz over rows and ix over columns; int c = counts[iz*columns+ix];
        //         skip if c == 0. Then:
        //             float h = Mathf.Lerp(0.05f, maxCubeHeight, Heat(c));
        //             Gizmos.color = ColorFor(c);
        //             Gizmos.DrawCube(CellCenter(ix, iz) + Vector3.up * h * 0.5f, new Vector3(cellSize * 0.9f, h, cellSize * 0.9f));
        //         Look at: Gizmos.DrawCube, Gizmos.color. Gizmos appear in the Scene view (and in the Game view
        //         only if its Gizmos toggle is on) — they cost nothing in a build.
        //         Check: while playing, the Scene view shows a trail of colored blocks where your head has been.
    }

    /// <summary>Spawns real colored cubes so the heatmap is visible inside the headset. Replaces previous cubes.</summary>
    [ContextMenu("Spawn Cubes")]
    public void SpawnCubes()
    {
        if (counts == null) { Debug.LogWarning("[HeatmapGrid] no data yet"); return; }
        if (cubesRoot != null) DestroyImmediate(cubesRoot.gameObject);
        cubesRoot = new GameObject("Heatmap Cubes").transform;
        cubesRoot.SetParent(transform, false);

        // TODO 4: Same loop as TODO 3, but with real objects:
        //             var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //             cube.transform.SetParent(cubesRoot, false);
        //             cube.transform.position = CellCenter(ix, iz) + Vector3.up * h * 0.5f;
        //             cube.transform.localScale = new Vector3(cellSize * 0.9f, h, cellSize * 0.9f);
        //             Destroy(cube.GetComponent<Collider>());          // so it never blocks the player or the ray
        //             cube.GetComponent<Renderer>().material.color = ColorFor(c);   // .material = a per-cube instance
        //         Look at: GameObject.CreatePrimitive, Renderer.material. Why destroy the collider: a heatmap is
        //         a debug view; it must not change how the level plays.
        //         Check: press the key mapped in the README (or right-click the component > Spawn Cubes) — the
        //         blocks appear in the Game view and in the headset. Press again: the old ones are replaced.
        Debug.Log("[HeatmapGrid] SpawnCubes — implement TODO 4");
    }

    /// <summary>Removes the spawned cubes and zeroes the counts.</summary>
    [ContextMenu("Clear")]
    public void Clear()
    {
        if (cubesRoot != null) DestroyImmediate(cubesRoot.gameObject);
        counts = null;
        maxCount = 0;
        totalSamples = 0;
    }

    /// <summary>Edit-mode helper: reads the sampler's CSV back into the grid so the gizmos show last session's data.</summary>
    [ContextMenu("Load CSV")]
    public void LoadCsv()
    {
        string path = Path.Combine(Application.persistentDataPath, csvFileName);
        if (!File.Exists(path)) { Debug.LogWarning("[HeatmapGrid] no file at " + path); return; }
        Clear();
        EnsureArray();

        // TODO 5: Parse the file. string[] lines = File.ReadAllLines(path); skip lines[0] (the header). For each
        //         other line: string[] f = line.Split(','); if f.Length < 4 continue;
        //             float x = float.Parse(f[1], CultureInfo.InvariantCulture); float z = float.Parse(f[3], ...);
        //             AddSample(new Vector3(x, 0f, z));
        //         Then Debug.Log how many rows loaded.
        //         Look at: File.ReadAllLines, string.Split, float.Parse with CultureInfo.InvariantCulture.
        //         Why: Play mode data dies when you press Stop. Reloading the CSV in Edit mode lets you study the
        //         heatmap calmly, screenshot it for the report, and compare two testers' files.
        //         Check: stop Play, right-click HeatmapGrid > Load CSV — the Scene view shows the session's heatmap.
        Debug.Log("[HeatmapGrid] LoadCsv — implement TODO 5 (" + path + ")");
    }
}

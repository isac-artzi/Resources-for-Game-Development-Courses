// RandomScatter.cs — Activity 3.1: Plant the Forest
// Plants N prefabs (trees, rocks, bushes) at random inside a circle, with random yaw and scale,
// while keeping a strip along the forest path clear so the player can always walk through.
// This is the tool you will reuse to populate the Enchanted Forest for Milestone 3.
// Attached to: "Scattered Forest" in the starter scene (Prefabs, Path Start and Path End are pre-set).
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RandomScatter : MonoBehaviour
{
    [Header("What to place")]
    [Tooltip("Prefabs to pick from at random. List a prefab twice to make it twice as common. " +
             "The starter scene uses greybox primitives; swap in your imported trees and rocks here.")]
    public GameObject[] prefabs;

    [Tooltip("How many props to place. 60 fills a 25 m clearing nicely; 300 starts to feel like a forest.")]
    public int count = 60;

    [Header("Where")]
    [Tooltip("Radius in meters of the disk (centered on this GameObject) that props are scattered in.")]
    public float radius = 25f;

    [Tooltip("Start of the path segment that must stay clear. Set in the starter scene.")]
    public Transform pathStart;

    [Tooltip("End of the path segment that must stay clear. Set in the starter scene.")]
    public Transform pathEnd;

    [Tooltip("Half the width of the clear strip, in meters. 2.5 leaves a 5 m corridor.")]
    public float pathHalfWidth = 2.5f;

    [Header("Variation")]
    [Tooltip("Smallest scale multiplier applied to a placed prop (1 = the prefab's own size).")]
    public float minScale = 0.8f;

    [Tooltip("Largest scale multiplier applied to a placed prop.")]
    public float maxScale = 1.25f;

    [Tooltip("Seed for the random generator. The same seed always produces the same forest — handy when you " +
             "want to compare two settings or reproduce a bug.")]
    public int seed = 42;

    [Header("When")]
    [Tooltip("Scatter automatically when Play starts. Turn off if you scatter in the Editor via the context menu.")]
    public bool scatterOnStart = true;

    /// <summary>How many props the last Scatter() call actually placed (may be below count if the path is wide).</summary>
    public int PlacedCount { get; private set; }

    void Start()
    {
        if (scatterOnStart) Scatter();
    }

    void Update()
    {
        // TODO 5: Re-scatter on demand so you can show variation in your screencast.
        //         When the R key was pressed this frame, increase seed by 1 and call Scatter().
        //         Look at: UnityEngine.InputSystem.Keyboard.current (null-check it!) and
        //         Keyboard.current.rKey.wasPressedThisFrame. Do NOT use the legacy Input class.
        //         Check: each press of R grows a different forest; the path stays clear every time.
    }

    /// <summary>
    /// Removes any previous props and places <see cref="count"/> new ones. Works in Play mode and,
    /// via the component's context menu (the three dots in the Inspector), in Edit mode.
    /// </summary>
    [ContextMenu("Scatter")]
    public void Scatter()
    {
        Clear();
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning("[RandomScatter] No prefabs assigned on " + name + ".");
            return;
        }

        Random.InitState(seed);
        int attempts = 0;
        int maxAttempts = count * 10;   // guard: if the path covers most of the disk we must not loop forever
        int placed = 0;

        while (placed < count && attempts < maxAttempts)
        {
            attempts++;

            Vector3 local = RandomPointInDisk(radius);            // TODO 1 lives inside this method
            Vector3 world = transform.position + local;

            if (IsTooCloseToPath(world)) continue;                 // TODO 2 lives inside DistancePointToSegment

            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            if (prefab == null) continue;

            // TODO 3: Random yaw. Replace Quaternion.identity with a rotation of Random.Range(0f, 360f)
            //         degrees around the world up axis: Quaternion.Euler(0f, yaw, 0f).
            //         Why: identical orientation is the number-one giveaway of a copy-pasted forest.
            //         Check: after scattering, look down at two trees of the same prefab — their crowns are turned differently.
            Quaternion rotation = Quaternion.identity;

            // TODO 4: Random scale. Replace 1f with Random.Range(minScale, maxScale).
            //         Stretch: average two Random.Range draws to get a bell-shaped distribution (most trees near 1.0,
            //         few very small or very large) instead of a flat one.
            //         Check: trees differ visibly in height; none is smaller than minScale times the prefab height.
            float scale = 1f;

            GameObject go = Instantiate(prefab, world, rotation, transform);
            go.name = prefab.name + " " + (placed + 1);
            go.transform.localScale = prefab.transform.localScale * scale;
            placed++;
        }

        PlacedCount = placed;
        Debug.Log("[RandomScatter] Placed " + placed + " of " + count + " props in " + attempts + " attempts.");
    }

    /// <summary>Destroys every child of this GameObject (everything Scatter placed).</summary>
    [ContextMenu("Clear")]
    public void Clear()
    {
        var children = new List<Transform>();
        foreach (Transform child in transform) children.Add(child);
        foreach (Transform child in children)
        {
            if (Application.isPlaying) Destroy(child.gameObject);
            else DestroyImmediate(child.gameObject);
        }
        PlacedCount = 0;
    }

    /// <summary>
    /// A uniformly distributed random point inside a disk of the given radius, on the XZ plane (y = 0).
    /// </summary>
    public static Vector3 RandomPointInDisk(float radius)
    {
        // TODO 1: Uniform random point in a disk.
        //         Draw u = Random.value and v = Random.value (both 0..1). Then
        //             float r     = radius * Mathf.Sqrt(u);
        //             float theta = v * 2f * Mathf.PI;
        //             return new Vector3(r * Mathf.Cos(theta), 0f, r * Mathf.Sin(theta));
        //         Why the square root: without it half of your trees land inside the inner 50 % of the radius,
        //         which is only 25 % of the area — a dense clump in the middle and a bare rim. (See Math foundation.)
        //         Look at: Random.value, Mathf.Sqrt, Mathf.Cos / Mathf.Sin (radians).
        //         Check: before this TODO every prop piles up at the center; after it they spread evenly to the edge.
        return Vector3.zero;
    }

    /// <summary>
    /// Shortest distance from point p to the line segment a–b, measured on the XZ plane (y is ignored).
    /// </summary>
    public static float DistancePointToSegment(Vector3 p, Vector3 a, Vector3 b)
    {
        // TODO 2: Point-to-segment distance.
        //         1. Flatten: set p.y = a.y = b.y = 0f so height does not matter.
        //         2. Vector3 ab = b - a;  Vector3 ap = p - a;
        //         3. float t = Vector3.Dot(ap, ab) / ab.sqrMagnitude;  (guard: if ab.sqrMagnitude < 1e-6f return Vector3.Distance(p, a))
        //         4. t = Mathf.Clamp01(t);   // clamp so the ends of the path are handled, not the infinite line
        //         5. Vector3 closest = a + ab * t;  return Vector3.Distance(p, closest);
        //         Look at: Vector3.Dot, Vector3.sqrMagnitude, Mathf.Clamp01.
        //         Check: with pathHalfWidth = 2.5 a clean 5 m corridor runs from Path Start to Path End and nothing
        //         spawns on it; set pathHalfWidth to 0 and trees appear on the path again.
        return float.MaxValue;
    }

    bool IsTooCloseToPath(Vector3 world)
    {
        if (pathStart == null || pathEnd == null) return false;
        return DistancePointToSegment(world, pathStart.position, pathEnd.position) < pathHalfWidth;
    }
}

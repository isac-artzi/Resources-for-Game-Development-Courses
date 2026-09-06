// CullingReporter.cs — Activity 3.3: Make It Fast: LOD, Occlusion, Lightmaps
// Counts renderers three ways a few times per second: how many exist, how many are inside the camera's view
// frustum (a test you write), and how many Unity actually considers visible after frustum AND occlusion culling
// (Renderer.isVisible). The gap between the last two numbers is what baking occlusion culling buys you.
// Attached to: "Perf" in the starter scene (PerfProbe reads these numbers for the HUD).
// Created by Isac Artzi

using UnityEngine;

public class CullingReporter : MonoBehaviour
{
    [Header("Sampling")]
    [Tooltip("Seconds between counts. Counting 500 renderers is cheap, but there is no need to do it every frame.")]
    public float sampleInterval = 0.25f;

    [Tooltip("Camera to test against. Leave empty to use the main camera.")]
    public Camera cam;

    /// <summary>All renderers found in the scene at Start (static walls, every LOD level, everything).</summary>
    public int TotalRenderers { get; private set; }

    /// <summary>Renderers whose bounds intersect the camera frustum right now (your test, TODO 2).</summary>
    public int InFrustum { get; private set; }

    /// <summary>Renderers Unity marked visible in the last rendered frame — after LOD selection, frustum and occlusion culling.</summary>
    public int Visible { get; private set; }

    Renderer[] all = new Renderer[0];
    float nextSampleTime;

    void Start()
    {
        if (cam == null) cam = Camera.main;

        // TODO 1: Gather every renderer once. LodBuilder runs in Awake, so by Start all LODGroups exist.
        //             all = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        //             TotalRenderers = all.Length;
        //         Look at: Object.FindObjectsByType<T>(FindObjectsSortMode) — the modern replacement for FindObjectsOfType.
        //         Check: the HUD's "total" shows a few hundred (64 pillars x 7 renderers + walls + props).
    }

    void Update()
    {
        if (cam == null || Time.time < nextSampleTime) return;
        nextSampleTime = Time.time + sampleInterval;

        // TODO 2: Frustum test. Build the six planes once per sample, then test every renderer's bounds:
        //             Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        //             int inFrustum = 0, visible = 0;
        //             for each Renderer r in all: if (r == null) continue;
        //                 if (GeometryUtility.TestPlanesAABB(planes, r.bounds)) inFrustum++;
        //                 if (r.isVisible) visible++;
        //             InFrustum = inFrustum;  Visible = visible;
        //         Look at: GeometryUtility.CalculateFrustumPlanes, GeometryUtility.TestPlanesAABB, Renderer.isVisible.
        //         Why two counts: your frustum test is pure geometry — everything in front of you counts, even behind a wall.
        //         isVisible is what Unity really rendered: after the LODGroup picked one level, after frustum culling AND,
        //         once you bake it, after occlusion culling. Before the bake the two numbers are close; after it, "visible"
        //         drops whenever walls hide pillars.
        //         Gotcha: isVisible is true if ANY camera sees the renderer, including the Scene view. Close or hide the
        //         Scene tab (maximize the Game view) when you take measurements.
        //         Check: face a long wall from 2 m away — in-frustum stays high, visible drops sharply after the occlusion bake.
    }

    /// <summary>One-line summary for the HUD and the CSV.</summary>
    public string Summary()
    {
        return "renderers " + Visible + " visible / " + InFrustum + " in frustum / " + TotalRenderers + " total";
    }
}

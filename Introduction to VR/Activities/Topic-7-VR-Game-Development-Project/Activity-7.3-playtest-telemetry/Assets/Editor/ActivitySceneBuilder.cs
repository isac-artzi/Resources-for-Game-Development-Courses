// ActivitySceneBuilder.cs — Activity 7.3: Playtest Telemetry and Heatmap
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a small "test course" — a forest path that forks, one branch
// a dead end, the other leading to a goal shard — plus a Telemetry object carrying PositionSampler,
// HeatmapGrid (covering the whole course) and BugReporter, and a confirmation label on the camera.
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using ActivityTools;

public static class ActivitySceneBuilder
{
    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1f, 0.95f, 0.85f), 1.0f, new Vector3(50f, -30f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.55f, 0.70f, 0.60f), 0.015f);

        var grass  = SceneBuilderUtil.MakeMaterial("Grass", new Color(0.25f, 0.50f, 0.20f));
        var slab   = SceneBuilderUtil.MakeMaterial("PathSlab", new Color(0.60f, 0.58f, 0.50f));
        var hedge  = SceneBuilderUtil.MakeMaterial("Hedge", new Color(0.12f, 0.35f, 0.15f));
        var bark   = SceneBuilderUtil.MakeMaterial("Bark", new Color(0.35f, 0.25f, 0.15f));
        var leaves = SceneBuilderUtil.MakeMaterial("Leaves", new Color(0.15f, 0.40f, 0.18f));
        var goalMat = SceneBuilderUtil.MakeEmissiveMaterial("GoalShard", new Color(0.4f, 0.8f, 1f), 2f);

        SceneBuilderUtil.AddFloor(30f, grass);
        var rig = SceneBuilderUtil.AddXrRig(new Vector3(0f, 0f, -10f));

        // Path: start -> fork -> left dead end / right -> goal --------------------
        var course = SceneBuilderUtil.AddEmpty("Course", Vector3.zero);
        LaySlabs(new Vector3(0f, 0f, -9f), new Vector3(0f, 0f, 0f), slab, course.transform);      // approach
        LaySlabs(new Vector3(0f, 0f, 0f), new Vector3(-6f, 0f, 5f), slab, course.transform);      // left branch (dead end)
        LaySlabs(new Vector3(0f, 0f, 0f), new Vector3(6f, 0f, 5f), slab, course.transform);       // right branch
        LaySlabs(new Vector3(6f, 0f, 5f), new Vector3(6f, 0f, 12f), slab, course.transform);      // to the goal

        // Dead end: a hedge wall closing the left branch (players discover it only when they arrive).
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Dead End Hedge", new Vector3(-7.2f, 1f, 6f), new Vector3(4f, 2f, 0.6f), hedge, course.transform)
                        .transform.rotation = Quaternion.Euler(0f, -40f, 0f);
        // Hedges lining the goal path so it reads as a corridor.
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Hedge L", new Vector3(4.6f, 0.75f, 8.5f), new Vector3(0.5f, 1.5f, 7f), hedge, course.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Hedge R", new Vector3(7.4f, 0.75f, 8.5f), new Vector3(0.5f, 1.5f, 7f), hedge, course.transform);

        // Trees around the clearing
        for (int i = 0; i < 12; i++)
        {
            float a = i * Mathf.PI * 2f / 12f;
            var p = new Vector3(Mathf.Cos(a) * 13f, 0f, Mathf.Sin(a) * 13f + 1f);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + i, p + Vector3.up * 2f, new Vector3(0.5f, 2f, 0.5f), bark, course.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + i, p + Vector3.up * 5f, Vector3.one * 3f, leaves, course.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(course);

        // Goal shard
        var goal = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Goal Shard", new Vector3(6f, 1.3f, 12.5f), new Vector3(0.2f, 0.4f, 0.2f), goalMat);
        goal.transform.rotation = Quaternion.Euler(45f, 0f, 45f);
        SceneBuilderUtil.AddPointLight("Goal Light", new Vector3(6f, 1.6f, 12.5f), new Color(0.4f, 0.8f, 1f), 4f, 1.5f, goal.transform);
        SceneBuilderUtil.AddLabel("Goal Sign", "The shard. You made it.", new Vector3(6f, 2.0f, 13f), 0.04f, Color.white);

        // Task sign for the tester (no hints about the dead end — that is the point).
        SceneBuilderUtil.AddLabel("Task Sign", "PLAYTEST TASK\nFind the shard.\n(B = report a bug, H = show heatmap)",
                                  new Vector3(0f, 2.0f, -6f), 0.045f, new Color(1f, 0.95f, 0.6f));

        // Confirmation label on the camera ---------------------------------
        var cam = rig.GetComponentInChildren<Camera>(true);
        var confirm = SceneBuilderUtil.AddLabel("Confirm Label", "", Vector3.zero, 0.012f, new Color(1f, 0.4f, 0.4f));
        if (cam != null)
        {
            confirm.transform.SetParent(cam.transform, false);
            confirm.transform.localPosition = new Vector3(0f, -0.25f, 1.2f);
            confirm.transform.localRotation = Quaternion.identity;
        }

        // Telemetry object ---------------------------------------------------
        var telemetry = SceneBuilderUtil.AddEmpty("Telemetry", Vector3.zero);
        var grid = telemetry.AddComponent<HeatmapGrid>();
        grid.origin = new Vector3(-15f, 0f, -15f);
        grid.cellSize = 1f;
        grid.columns = 30;
        grid.rows = 30;
        var sampler = telemetry.AddComponent<PositionSampler>();
        sampler.grid = grid;
        sampler.intervalSeconds = 0.5f;
        var bugs = telemetry.AddComponent<BugReporter>();
        bugs.confirmLabel = confirm.GetComponent<TextMesh>();

        SceneBuilderUtil.SaveScene(scene, "Activity_7_3");
        Selection.activeGameObject = telemetry;
    }

    /// <summary>Flat 1 m stone slabs every 1.2 m from a to b, so the path reads as a path.</summary>
    static void LaySlabs(Vector3 a, Vector3 b, Material mat, Transform parent)
    {
        float length = Vector3.Distance(a, b);
        int n = Mathf.Max(1, Mathf.RoundToInt(length / 1.2f));
        for (int i = 0; i <= n; i++)
        {
            var p = Vector3.Lerp(a, b, (float)i / n);
            var s = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Slab", p + Vector3.up * 0.02f, new Vector3(0.9f, 0.04f, 0.9f), mat, parent);
            s.transform.rotation = Quaternion.Euler(0f, Random.Range(-15f, 15f), 0f);
        }
    }
}

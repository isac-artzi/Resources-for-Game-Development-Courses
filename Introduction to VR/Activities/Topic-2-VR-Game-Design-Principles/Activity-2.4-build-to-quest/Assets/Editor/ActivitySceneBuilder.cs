// ActivitySceneBuilder.cs — Activity 2.4: Build to Quest and Measure
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: the Hermit's ledge on the Mountain Pass with two real-time point lights,
// a frame-time HUD parented under the camera, a stress spawner that fills the sky with lit "wisp" cubes on demand,
// and a CSV performance logger — the scene you will build to the Quest 3 and measure.
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ActivityTools;

public static class ActivitySceneBuilder
{
    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(0.85f, 0.88f, 1.0f), 0.8f, new Vector3(25f, 60f, 0f));   // cold, low mountain light
        SceneBuilderUtil.SetFog(new Color(0.45f, 0.50f, 0.62f), 0.015f);

        // Materials -------------------------------------------------------
        var slate    = SceneBuilderUtil.MakeMaterial("Slate",    new Color(0.32f, 0.33f, 0.38f));
        var rock     = SceneBuilderUtil.MakeMaterial("Rock",     new Color(0.36f, 0.34f, 0.36f));
        var snowRock = SceneBuilderUtil.MakeMaterial("SnowRock", new Color(0.72f, 0.74f, 0.80f));
        var ember    = SceneBuilderUtil.MakeEmissiveMaterial("Ember",  new Color(1.0f, 0.55f, 0.2f), 2.5f);
        var wisp     = SceneBuilderUtil.MakeEmissiveMaterial("Wisp",   new Color(0.6f, 0.9f, 1.0f), 1.0f);
        var hermitMat = SceneBuilderUtil.MakeEmissiveMaterial("Hermit", new Color(0.8f, 0.9f, 1.0f), 0.8f);

        // Floor, player, interaction manager ---------------------------------
        SceneBuilderUtil.AddFloor(40f, slate);
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);
        if (Object.FindFirstObjectByType<XRInteractionManager>() == null)
            new GameObject("XR Interaction Manager", typeof(XRInteractionManager));

        // The ledge: a low wall of rocks around a fire pit ---------------------------------------------
        var rocks = SceneBuilderUtil.AddEmpty("Rocks", Vector3.zero);
        for (int i = 0; i < 14; i++)
        {
            float angle = Mathf.Lerp(-100f, 100f, i / 13f) * Mathf.Deg2Rad;   // an open arc facing +z
            float radius = 7f + Mathf.Sin(i * 1.7f) * 0.8f;
            var pos = new Vector3(Mathf.Sin(angle) * radius, 0f, Mathf.Cos(angle) * radius + 2f);
            float h = 1.5f + Mathf.Abs(Mathf.Cos(i * 2.3f)) * 2.5f;
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Rock " + (i + 1), pos + Vector3.up * h * 0.5f,
                                          new Vector3(1.6f, h, 1.4f), (i % 4 == 0) ? snowRock : rock, rocks.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(rocks);

        var fire = SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Fire Pit", new Vector3(1.5f, 0.25f, 2.5f),
                                                 Vector3.one * 0.5f, ember);
        SceneBuilderUtil.AddPointLight("Fire Light", new Vector3(1.5f, 0.9f, 2.5f), new Color(1f, 0.6f, 0.3f), 8f, 2.0f,
                                       fire.transform);
        // A second real-time light: in the Built-in forward renderer every extra pixel light adds a pass per object,
        // which is exactly the cost the stress spawner will make visible.
        SceneBuilderUtil.AddPointLight("Moon Shard Light", new Vector3(-2f, 2.5f, 5f), new Color(0.5f, 0.7f, 1f), 12f, 1.5f);

        var hermit = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Mountain Hermit",
                                                   new Vector3(-1.5f, 1f, 3.5f), new Vector3(0.55f, 0.9f, 0.55f), hermitMat);
        SceneBuilderUtil.AddLabel("Hermit Label", "Count the wisps, traveler. Count them until the sky stutters.",
                                  new Vector3(-1.5f, 2.3f, 3.5f), 0.045f, new Color(0.85f, 0.92f, 1f));

        // Frame-time HUD: a TextMesh parented under the camera, 1.5 m ahead and a little below the center -------
        var cam = rig.GetComponentInChildren<Camera>(true);
        var hudGo = SceneBuilderUtil.AddLabel("Frame Time HUD", "HUD: implement FrameTimeHud.cs",
                                              Vector3.zero, 0.02f, new Color(0.6f, 1f, 0.6f),
                                              cam != null ? cam.transform : null);
        hudGo.transform.localPosition = new Vector3(0f, -0.35f, 1.5f);
        hudGo.transform.localRotation = Quaternion.identity;
        hudGo.layer = LayerMask.NameToLayer("Ignore Raycast");
        var hud = hudGo.AddComponent<FrameTimeHud>();
        hud.label = hudGo.GetComponent<TextMesh>();

        // Stress spawner + logger ----------------------------------------------------------------------
        var spawnCenter = SceneBuilderUtil.AddEmpty("Wisp Cloud Center", new Vector3(0f, 3.5f, 6f));
        var tools = SceneBuilderUtil.AddEmpty("Perf Tools", Vector3.zero);
        var spawner = tools.AddComponent<StressSpawner>();
        spawner.spawnCenter = spawnCenter.transform;
        spawner.sharedMaterial = wisp;
        spawner.cloudRadius = 3.5f;
        spawner.batchSize = 50;

        var logger = tools.AddComponent<PerfLogger>();
        logger.hud = hud;
        logger.spawner = spawner;
        hud.spawner = spawner;

        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 2.4 - Build to Quest and Measure\nB: +50 wisps   C: clear   U: unique materials on/off   L: save CSV\nWatch the HUD. Then build to the headset.",
                                  new Vector3(0f, 2.6f, 4f), 0.04f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_2_4");
        Selection.activeGameObject = rig;
    }
}

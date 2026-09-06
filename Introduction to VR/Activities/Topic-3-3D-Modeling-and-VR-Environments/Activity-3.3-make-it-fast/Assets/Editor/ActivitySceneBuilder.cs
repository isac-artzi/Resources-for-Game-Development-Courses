// ActivitySceneBuilder.cs — Activity 3.3: Make It Fast: LOD, Occlusion, Lightmaps
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a dense ruins yard — 80 identical pillars (each with LOD0/LOD1/LOD2
// children and a LodBuilder), long static walls that occlude, six torches (point lights to bake), a many-part Guide
// statue, and a "Perf" object with CullingReporter + PerfProbe wired to a head-locked HUD. Everything is static so
// occlusion culling and lightmaps can be baked from the Unity windows named in the README.
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
        SceneBuilderUtil.SetSun(new Color(1f, 0.93f, 0.80f), 1.0f, new Vector3(40f, 210f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.70f, 0.68f, 0.62f), 0.004f);

        // Materials -------------------------------------------------------
        var ground  = SceneBuilderUtil.MakeMaterial("Ruins Ground", new Color(0.42f, 0.40f, 0.35f));
        var stone   = SceneBuilderUtil.MakeMaterial("Ruins Stone",  new Color(0.62f, 0.60f, 0.56f));
        var wall    = SceneBuilderUtil.MakeMaterial("Ruins Wall",   new Color(0.50f, 0.47f, 0.42f));
        var moss    = SceneBuilderUtil.MakeMaterial("Moss",         new Color(0.30f, 0.45f, 0.25f));
        var orbMat  = SceneBuilderUtil.MakeEmissiveMaterial("Pillar Orb", new Color(0.55f, 0.85f, 1f), 1.5f);
        var torch   = SceneBuilderUtil.MakeEmissiveMaterial("Torch Flame", new Color(1f, 0.6f, 0.2f), 2f);

        // Floor and player ------------------------------------------------
        SceneBuilderUtil.AddFloor(90f, ground);
        var rig = SceneBuilderUtil.AddXrRig(new Vector3(0f, 0f, -8f));
        var cam = rig.GetComponentInChildren<Camera>(true);

        // Walls: three long occluders across the yard (each split by a 4 m doorway) plus two side walls -------
        var walls = SceneBuilderUtil.AddEmpty("Ruins Walls", Vector3.zero);
        float[] wallRows = { 5f, 20f, 35f };
        for (int i = 0; i < wallRows.Length; i++)
        {
            float z = wallRows[i];
            // left and right halves, leaving x in [-2, 2] open
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Long Wall " + (char)('A' + i) + " (west)", new Vector3(-12f, 2.25f, z),
                                          new Vector3(20f, 4.5f, 0.6f), wall, walls.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Long Wall " + (char)('A' + i) + " (east)", new Vector3(12f, 2.25f, z),
                                          new Vector3(20f, 4.5f, 0.6f), wall, walls.transform);
            // a mossy lintel over the doorway
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Lintel " + (char)('A' + i), new Vector3(0f, 4.2f, z),
                                          new Vector3(4.6f, 0.6f, 0.7f), moss, walls.transform);
        }
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Side Wall (west)", new Vector3(-22.3f, 2.25f, 20f), new Vector3(0.6f, 4.5f, 50f), wall, walls.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Side Wall (east)", new Vector3(22.3f, 2.25f, 20f), new Vector3(0.6f, 4.5f, 50f), wall, walls.transform);
        SceneBuilderUtil.MarkStaticRecursive(walls);

        // Torches: six point lights on the long walls — the lights you will bake ---------------------------
        var torches = SceneBuilderUtil.AddEmpty("Torches", Vector3.zero);
        for (int i = 0; i < wallRows.Length; i++)
        {
            float z = wallRows[i] - 0.8f;
            AddTorch("Torch " + (i * 2 + 1), new Vector3(-6f, 2.6f, z), torch, torches.transform);
            AddTorch("Torch " + (i * 2 + 2), new Vector3(6f, 2.6f, z), torch, torches.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(torches);

        // Pillars: 8 columns x 10 rows, each with LOD0 / LOD1 / LOD2 children and a LodBuilder ----------------
        var pillars = SceneBuilderUtil.AddEmpty("Pillars", Vector3.zero);
        float[] pillarRows = { -2f, 2f, 9f, 13f, 17f, 23f, 27f, 31f, 38f, 42f };
        int index = 0;
        for (int r = 0; r < pillarRows.Length; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                float x = -17.5f + c * 5f;
                index++;
                MakePillar("Pillar " + index, new Vector3(x, 0f, pillarRows[r]), stone, orbMat, pillars.transform);
            }
        }
        SceneBuilderUtil.MarkStaticRecursive(pillars);

        // The Guide statue: one deliberately heavy object (many renderers, no LODs yet) -----------------------
        var statue = SceneBuilderUtil.AddEmpty("Guide Statue", new Vector3(0f, 0f, 20f + 8f));
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Plinth", statue.transform.position + Vector3.up * 0.4f,
                                      new Vector3(2.5f, 0.4f, 2.5f), stone, statue.transform);
        for (int i = 0; i < 48; i++)
        {
            float t = i / 48f;
            float angle = t * Mathf.PI * 6f;
            float radius = Mathf.Lerp(0.9f, 0.15f, t);
            var p = statue.transform.position + new Vector3(Mathf.Cos(angle) * radius, 0.8f + t * 4.5f, Mathf.Sin(angle) * radius);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Statue Bead " + (i + 1), p, Vector3.one * Mathf.Lerp(0.45f, 0.15f, t),
                                          i % 6 == 0 ? orbMat : stone, statue.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(statue);
        SceneBuilderUtil.AddLabel("Statue Label", "The Guide, in stone\n(48 renderers, no LODs yet)",
                                  statue.transform.position + Vector3.up * 6f, 0.06f, new Color(0.9f, 0.95f, 1f));

        // Perf tools ------------------------------------------------------------------------------------
        var perfGo = SceneBuilderUtil.AddEmpty("Perf", Vector3.zero);
        var reporter = perfGo.AddComponent<CullingReporter>();
        reporter.cam = cam;
        reporter.sampleInterval = 0.25f;

        var probe = perfGo.AddComponent<PerfProbe>();
        probe.reporter = reporter;
        probe.snapshotLabel = "before";
        probe.budgetMs = 13.9f;

        if (cam != null)
        {
            var hud = SceneBuilderUtil.AddLabel("Perf HUD", "PerfProbe: implement TODO 1-3",
                                                cam.transform.TransformPoint(new Vector3(0f, -0.20f, 0.9f)), 0.011f,
                                                Color.white, cam.transform);
            hud.transform.localRotation = Quaternion.identity;
            probe.readout = hud.GetComponent<TextMesh>();
        }

        // Signs ------------------------------------------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 3.3 - Make It Fast\n80 pillars x 3 LOD levels, 3 walls, 6 torches\nP = snapshot a perf row",
                                  new Vector3(0f, 2.6f, -3f), 0.05f, Color.white);
        SceneBuilderUtil.AddLabel("Doorway Sign", "Stand here, face the wall:\nhow many pillars does Unity still draw?",
                                  new Vector3(0f, 2.2f, 3.2f), 0.04f, new Color(1f, 0.95f, 0.7f));

        SceneBuilderUtil.SaveScene(scene, "Activity_3_3");
        Selection.activeGameObject = perfGo;
    }

    // ----------------------------------------------------------------------
    // A pillar with three detail levels as children. Pivot at the base. LOD0 = 4 renderers, LOD1 = 2, LOD2 = 1.
    // ----------------------------------------------------------------------
    static void MakePillar(string name, Vector3 basePos, Material stone, Material orb, Transform parent)
    {
        var root = SceneBuilderUtil.AddEmpty(name, basePos, parent);

        var lod0 = SceneBuilderUtil.AddEmpty("LOD0", basePos, root.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube,     "Base",    basePos + Vector3.up * 0.15f, new Vector3(1.2f, 0.3f, 1.2f), stone, lod0.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Shaft",   basePos + Vector3.up * 1.95f, new Vector3(0.5f, 1.65f, 0.5f), stone, lod0.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube,     "Capital", basePos + Vector3.up * 3.75f, new Vector3(1.2f, 0.3f, 1.2f), stone, lod0.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere,   "Orb",     basePos + Vector3.up * 4.2f,  Vector3.one * 0.5f,            orb,   lod0.transform);

        var lod1 = SceneBuilderUtil.AddEmpty("LOD1", basePos, root.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Shaft",   basePos + Vector3.up * 1.9f,  new Vector3(0.6f, 1.9f, 0.6f),  stone, lod1.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube,     "Capital", basePos + Vector3.up * 3.9f,  new Vector3(1.2f, 0.6f, 1.2f),  stone, lod1.transform);

        var lod2 = SceneBuilderUtil.AddEmpty("LOD2", basePos, root.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube,     "Block",   basePos + Vector3.up * 2.2f,  new Vector3(0.9f, 4.4f, 0.9f),  stone, lod2.transform);

        var builder = root.AddComponent<LodBuilder>();
        builder.levelNames = new string[] { "LOD0", "LOD1", "LOD2" };
        builder.transitionHeights = new float[] { 0.40f, 0.15f, 0.03f };
        builder.buildOnAwake = true;
    }

    static void AddTorch(string name, Vector3 pos, Material flame, Transform parent)
    {
        var holder = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, name, pos - Vector3.up * 0.3f,
                                                   new Vector3(0.08f, 0.3f, 0.08f), null, parent);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Flame", pos + Vector3.up * 0.1f, Vector3.one * 0.25f, flame, holder.transform);
        SceneBuilderUtil.AddPointLight("Torch Light", pos + Vector3.up * 0.2f, new Color(1f, 0.65f, 0.3f), 9f, 2.5f, holder.transform);
    }
}

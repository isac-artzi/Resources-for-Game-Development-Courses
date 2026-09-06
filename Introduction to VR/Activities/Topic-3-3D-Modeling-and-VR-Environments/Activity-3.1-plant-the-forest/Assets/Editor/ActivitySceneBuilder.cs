// ActivitySceneBuilder.cs — Activity 3.1: Plant the Forest
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a 60 m forest clearing with a dirt path, three greybox prefabs
// (tree, rock, bush) saved under Assets/Prefabs, a "Scattered Forest" root carrying RandomScatter, and a
// Scale Gauge station with a 2 m post and a deliberately mis-scaled test tree carrying ScaleNormalizer.
// Everything runs with primitives; the README tells you how to swap in free assets.
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using ActivityTools;

public static class ActivitySceneBuilder
{
    const string PrefabFolder = "Assets/Prefabs";

    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1f, 0.96f, 0.88f), 1.1f, new Vector3(50f, -35f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.60f, 0.74f, 0.62f), 0.012f);

        // Materials -------------------------------------------------------
        var grass  = SceneBuilderUtil.MakeMaterial("Grass",  new Color(0.27f, 0.50f, 0.22f));
        var dirt   = SceneBuilderUtil.MakeMaterial("Dirt",   new Color(0.45f, 0.36f, 0.24f));
        var bark   = SceneBuilderUtil.MakeMaterial("Bark",   new Color(0.35f, 0.25f, 0.15f));
        var leaves = SceneBuilderUtil.MakeMaterial("Leaves", new Color(0.16f, 0.42f, 0.19f));
        var bush   = SceneBuilderUtil.MakeMaterial("Bush",   new Color(0.22f, 0.50f, 0.24f));
        var stone  = SceneBuilderUtil.MakeMaterial("Stone",  new Color(0.52f, 0.52f, 0.55f));
        var post   = SceneBuilderUtil.MakeEmissiveMaterial("GaugePost", new Color(1f, 0.85f, 0.35f), 0.8f);

        // Floor, path and player ------------------------------------------
        SceneBuilderUtil.AddFloor(60f, grass);

        var path = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Forest Path", new Vector3(0f, 0.01f, 2.5f),
                                                 new Vector3(3f, 0.02f, 25f), dirt);
        path.isStatic = true;
        var pathStart = SceneBuilderUtil.AddEmpty("Path Start", new Vector3(0f, 0f, -10f), path.transform);
        var pathEnd   = SceneBuilderUtil.AddEmpty("Path End",   new Vector3(0f, 0f, 15f),  path.transform);

        var rig = SceneBuilderUtil.AddXrRig(new Vector3(0f, 0f, -6f));

        // Greybox prefabs (saved as assets so RandomScatter has something to plant) -----------------------
        var treePrefab = MakeTreePrefab(bark, leaves);
        var rockPrefab = MakeRockPrefab(stone);
        var bushPrefab = MakeBushPrefab(bush);

        // Old forest edge: a static ring of hand-placed trees for enclosure and scale ---------------------
        var edge = SceneBuilderUtil.AddEmpty("Old Forest Edge", Vector3.zero);
        const int edgeCount = 14;
        for (int i = 0; i < edgeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / edgeCount;
            float r = 27f + Mathf.Sin(i * 2.3f) * 1.5f;
            var basePos = new Vector3(Mathf.Cos(angle) * r, 0f, Mathf.Sin(angle) * r);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Edge Trunk " + (i + 1), basePos + Vector3.up * 2.5f,
                                          new Vector3(0.6f, 2.5f, 0.6f), bark, edge.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Edge Crown " + (i + 1), basePos + Vector3.up * 6.5f,
                                          Vector3.one * 4f, leaves, edge.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(edge);

        // The scatter tool --------------------------------------------------
        var forest = SceneBuilderUtil.AddEmpty("Scattered Forest", Vector3.zero);
        var scatter = forest.AddComponent<RandomScatter>();
        scatter.prefabs = new GameObject[] { treePrefab, treePrefab, rockPrefab, bushPrefab };
        scatter.count = 60;
        scatter.radius = 25f;
        scatter.pathStart = pathStart.transform;
        scatter.pathEnd = pathEnd.transform;
        scatter.pathHalfWidth = 2.5f;
        scatter.minScale = 0.8f;
        scatter.maxScale = 1.25f;
        scatter.seed = 42;
        scatter.scatterOnStart = true;

        // Scale Gauge station -----------------------------------------------
        var gauge = SceneBuilderUtil.AddEmpty("Scale Gauge", new Vector3(-6f, 0f, -4f));
        var pole = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Human Height Post (2 m)",
                                                 gauge.transform.position + Vector3.up * 1f,
                                                 new Vector3(0.08f, 1f, 0.08f), post, gauge.transform);
        pole.isStatic = true;
        SceneBuilderUtil.AddLabel("Post Label", "2 m post\n(a tall adult)", gauge.transform.position + new Vector3(0f, 2.3f, 0f),
                                  0.04f, new Color(1f, 0.9f, 0.5f), gauge.transform);

        // A copy of the tree prefab at scale 0.02 — about 13 cm tall. This is what a centimeter-authored model
        // looks like when Unity reads it as meters. ScaleNormalizer measures it and (once implemented) fixes it.
        var subject = (GameObject)PrefabUtility.InstantiatePrefab(treePrefab);
        subject.name = "Scale Test Subject";
        subject.transform.SetParent(gauge.transform, false);
        subject.transform.position = gauge.transform.position + new Vector3(-2f, 0f, 0f);
        subject.transform.localScale = Vector3.one * 0.02f;
        var normalizer = subject.AddComponent<ScaleNormalizer>();
        normalizer.targetHeightMeters = 8f;
        normalizer.applyOnStart = true;
        var gaugeLabel = SceneBuilderUtil.AddLabel("Gauge Readout", "Scale gauge: implement TODO 1-3 in ScaleNormalizer.cs",
                                                   gauge.transform.position + new Vector3(-2f, 1.4f, -1f), 0.035f,
                                                   new Color(1f, 0.95f, 0.6f), gauge.transform);
        normalizer.readout = gaugeLabel.GetComponent<TextMesh>();

        // Signs --------------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 3.1 - Plant the Forest\nPress Play: 60 props scatter (they pile up until TODO 1)\nR = re-scatter with a new seed",
                                  new Vector3(0f, 2.4f, -2f), 0.045f, Color.white);
        SceneBuilderUtil.AddLabel("Path Sign", "The path must stay clear\n(RandomScatter TODO 2)",
                                  new Vector3(2.2f, 1.2f, 4f), 0.04f, new Color(0.9f, 0.9f, 0.8f));

        SceneBuilderUtil.SaveScene(scene, "Activity_3_1");
        Selection.activeGameObject = forest;
    }

    // ----------------------------------------------------------------------
    // Greybox prefabs. Each has its pivot at the BASE (y = 0) so scattering on the floor and swaying both work.
    // ----------------------------------------------------------------------

    static GameObject MakeTreePrefab(Material bark, Material leaves)
    {
        var root = new GameObject("Greybox Tree");
        // Cylinder is 2 m tall at scale 1, so scale y = 2 gives a 4 m trunk centered at y = 2.
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk", new Vector3(0f, 2f, 0f),
                                      new Vector3(0.5f, 2f, 0.5f), bark, root.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown", new Vector3(0f, 5.2f, 0f),
                                      new Vector3(3.2f, 3.6f, 3.2f), leaves, root.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown Top", new Vector3(0.4f, 6.8f, 0.2f),
                                      new Vector3(1.8f, 1.8f, 1.8f), leaves, root.transform);
        var sway = root.AddComponent<WindSway>();
        sway.maxAngleDegrees = 2.5f;
        sway.frequencyHz = 0.4f;
        return SavePrefab(root, "Greybox Tree");
    }

    static GameObject MakeRockPrefab(Material stone)
    {
        var root = new GameObject("Greybox Rock");
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Boulder", new Vector3(0f, 0.3f, 0f),
                                      new Vector3(1.4f, 0.8f, 1.1f), stone, root.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Pebble", new Vector3(0.7f, 0.15f, 0.3f),
                                      new Vector3(0.5f, 0.35f, 0.45f), stone, root.transform);
        return SavePrefab(root, "Greybox Rock");
    }

    static GameObject MakeBushPrefab(Material bush)
    {
        var root = new GameObject("Greybox Bush");
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Bush Body", new Vector3(0f, 0.5f, 0f),
                                      new Vector3(1.2f, 1.0f, 1.2f), bush, root.transform);
        var sway = root.AddComponent<WindSway>();
        sway.maxAngleDegrees = 5f;
        sway.frequencyHz = 0.7f;
        return SavePrefab(root, "Greybox Bush");
    }

    /// <summary>Saves a scene GameObject as a prefab asset under Assets/Prefabs and removes the scene copy.</summary>
    static GameObject SavePrefab(GameObject sceneObject, string prefabName)
    {
        SceneBuilderUtil.EnsureFolder(PrefabFolder);
        string path = PrefabFolder + "/" + prefabName + ".prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(sceneObject, path);
        Object.DestroyImmediate(sceneObject);
        return prefab;
    }
}

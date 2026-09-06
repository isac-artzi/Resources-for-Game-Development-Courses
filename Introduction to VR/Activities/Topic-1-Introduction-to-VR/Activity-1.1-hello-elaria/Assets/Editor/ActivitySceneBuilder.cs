// ActivitySceneBuilder.cs — Activity 1.1: Hello, Elaria
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a forest clearing at human scale with three spinning shards,
// a placeholder Guide, and a floating measurement readout.
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
        SceneBuilderUtil.SetSun(new Color(1f, 0.95f, 0.85f), 1.1f, new Vector3(45f, -30f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.55f, 0.70f, 0.60f), 0.02f);

        // Materials -------------------------------------------------------
        var grass  = SceneBuilderUtil.MakeMaterial("Grass",  new Color(0.25f, 0.50f, 0.20f));
        var bark   = SceneBuilderUtil.MakeMaterial("Bark",   new Color(0.35f, 0.25f, 0.15f));
        var leaves = SceneBuilderUtil.MakeMaterial("Leaves", new Color(0.15f, 0.40f, 0.18f));
        var stone  = SceneBuilderUtil.MakeMaterial("Stone",  new Color(0.55f, 0.55f, 0.58f));
        var shardMat = SceneBuilderUtil.MakeEmissiveMaterial("Shard", new Color(0.40f, 0.80f, 1.00f), 2f);
        var guideMat = SceneBuilderUtil.MakeEmissiveMaterial("Guide", new Color(0.90f, 0.85f, 1.00f), 1.2f);

        // Floor and player ------------------------------------------------
        SceneBuilderUtil.AddFloor(30f, grass);
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);

        // Ring of trees: trunk = cylinder (2 m tall at scale 1), crown = sphere ---------------------------
        var trees = SceneBuilderUtil.AddEmpty("Trees", Vector3.zero);
        const int treeCount = 12;
        for (int i = 0; i < treeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / treeCount;
            float radius = 9f + Mathf.Sin(i * 1.7f) * 1.5f;
            var basePos = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + (i + 1), basePos + Vector3.up * 2f,
                                          new Vector3(0.5f, 2f, 0.5f), bark, trees.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + (i + 1), basePos + Vector3.up * 5f,
                                          Vector3.one * 3f, leaves, trees.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(trees);

        // Shards on pedestals ---------------------------------------------
        Vector3[] shardSpots =
        {
            new Vector3(-2.0f, 0f, 3.0f),
            new Vector3( 2.5f, 0f, 2.5f),
            new Vector3( 0.0f, 0f, 5.0f),
        };
        var shards = SceneBuilderUtil.AddEmpty("Shards", Vector3.zero);
        for (int i = 0; i < shardSpots.Length; i++)
        {
            var spot = shardSpots[i];
            var pedestal = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Pedestal " + (i + 1),
                                                         spot + Vector3.up * 0.5f, new Vector3(0.4f, 0.5f, 0.4f),
                                                         stone, shards.transform);
            pedestal.isStatic = true;

            var shard = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Shard " + (i + 1),
                                                      spot + Vector3.up * 1.3f, new Vector3(0.15f, 0.30f, 0.15f),
                                                      shardMat, shards.transform);
            shard.transform.rotation = Quaternion.Euler(45f, 0f, 45f);
            var spin = shard.AddComponent<SpinCollectible>();
            spin.degreesPerSecond = 40f + 25f * i;   // 40, 65, 90 deg/s so the three are distinguishable

            SceneBuilderUtil.AddPointLight("Shard Light", spot + Vector3.up * 1.5f, new Color(0.4f, 0.8f, 1f),
                                           3f, 1.2f, shard.transform);
        }

        // The Mysterious Guide (capsule is 2 m tall at scale 1) -----------
        var guide = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Mysterious Guide",
                                                  new Vector3(0f, 1f, 7f), new Vector3(0.6f, 1f, 0.6f), guideMat);
        SceneBuilderUtil.AddPointLight("Guide Glow", new Vector3(0f, 1.5f, 6.5f), new Color(0.8f, 0.7f, 1f), 5f, 1.5f,
                                       guide.transform);
        var guideLabel = SceneBuilderUtil.AddLabel("Guide Label", "The Guide waits in the mist", new Vector3(0f, 2.4f, 7f),
                                                   0.06f, new Color(0.95f, 0.9f, 1f));
        guideLabel.AddComponent<LookAtPlayer>();

        // Measurement probe -----------------------------------------------
        var probeGo = SceneBuilderUtil.AddLabel("Probe Readout", "Probe: implement TODO 1-4 in EyeHeightProbe.cs",
                                                new Vector3(0f, 1.4f, 2f), 0.035f, new Color(1f, 0.95f, 0.5f));
        var probe = probeGo.AddComponent<EyeHeightProbe>();
        probe.target = guide.transform;
        probe.targetHeightMeters = 2f;
        probe.readout = probeGo.GetComponent<TextMesh>();
        probeGo.AddComponent<LookAtPlayer>();

        // Welcome sign (static, faces the player at the origin) -----------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Welcome to Elaria\nActivity 1.1 - Hello, Elaria\nOpen README.md (Activity > Open README)",
                                  new Vector3(0f, 2.3f, 4f), 0.05f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_1_1");
        Selection.activeGameObject = rig;
    }
}

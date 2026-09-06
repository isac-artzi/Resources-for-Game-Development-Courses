// ActivitySceneBuilder.cs — Activity 6.1: Hear the Forest: Spatial Audio
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a forest clearing with three 3D sound sources (stream, bird, fire),
// a hidden audio beacon behind the trees, a listener audit label, and the ToneFactory that makes everything
// audible before any audio file is downloaded.
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
        SceneBuilderUtil.SetSun(new Color(1f, 0.93f, 0.80f), 0.9f, new Vector3(35f, -40f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.50f, 0.66f, 0.56f), 0.025f);

        // Materials -------------------------------------------------------
        var grass   = SceneBuilderUtil.MakeMaterial("Grass",  new Color(0.24f, 0.48f, 0.20f));
        var bark    = SceneBuilderUtil.MakeMaterial("Bark",   new Color(0.35f, 0.25f, 0.15f));
        var leaves  = SceneBuilderUtil.MakeMaterial("Leaves", new Color(0.15f, 0.40f, 0.18f));
        var stone   = SceneBuilderUtil.MakeMaterial("Stone",  new Color(0.55f, 0.55f, 0.58f));
        var water   = SceneBuilderUtil.MakeEmissiveMaterial("Water", new Color(0.25f, 0.55f, 0.85f), 0.3f);
        var feather = SceneBuilderUtil.MakeEmissiveMaterial("Feather", new Color(0.95f, 0.75f, 0.25f), 0.6f);
        var ember   = SceneBuilderUtil.MakeEmissiveMaterial("Ember", new Color(1.0f, 0.45f, 0.10f), 2.5f);
        var shardMat = SceneBuilderUtil.MakeEmissiveMaterial("Shard", new Color(0.40f, 0.80f, 1.00f), 2f);
        var ringMin = SceneBuilderUtil.MakeEmissiveMaterial("Ring Min", new Color(1.0f, 0.85f, 0.30f), 1.5f);
        var ringMax = SceneBuilderUtil.MakeEmissiveMaterial("Ring Max", new Color(0.30f, 0.80f, 1.00f), 1.5f);

        // Floor, player, tools -------------------------------------------
        SceneBuilderUtil.AddFloor(40f, grass);
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);

        var tools = SceneBuilderUtil.AddEmpty("Audio Tools", Vector3.zero);
        var tone = tools.AddComponent<ToneFactory>();
        tone.sampleRate = 44100;
        tone.amplitude = 0.4f;

        // Ring of trees so the clearing has walls for your ears to bounce off (visually, at least) --------
        var trees = SceneBuilderUtil.AddEmpty("Trees", Vector3.zero);
        const int treeCount = 14;
        for (int i = 0; i < treeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / treeCount;
            float radius = 12f + Mathf.Sin(i * 2.3f) * 2f;
            var basePos = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + (i + 1), basePos + Vector3.up * 2.5f,
                                          new Vector3(0.6f, 2.5f, 0.6f), bark, trees.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + (i + 1), basePos + Vector3.up * 6f,
                                          Vector3.one * 3.5f, leaves, trees.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(trees);

        // Three spatialized sources ---------------------------------------
        // Stream: a long shallow box to the left. Broadband sound, wide min distance (it is a big source).
        var stream = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Stream", new Vector3(-7f, 0.1f, 4f),
                                                   new Vector3(6f, 0.2f, 1.2f), water);
        var streamSrc = AddSpatialSource(stream, 2.0f, 18f, 0.9f);
        var streamViz = stream.AddComponent<RolloffVisualizer>();
        streamViz.placeholderIsNoise = true;
        streamViz.placeholderHz = 180f;
        streamViz.minRingMaterial = ringMin;
        streamViz.maxRingMaterial = ringMax;
        streamViz.readout = SceneBuilderUtil.AddLabel("Stream Readout", "Stream", new Vector3(-7f, 1.6f, 4f), 0.04f,
                                                      new Color(0.7f, 0.85f, 1f)).GetComponent<TextMesh>();

        // Bird: a small point source high in a tree to the right. Narrow, high, easy to localize.
        var perchPos = new Vector3(6f, 4.5f, 6f);
        var bird = SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Bird", perchPos, Vector3.one * 0.25f, feather);
        var birdSrc = AddSpatialSource(bird, 1.0f, 14f, 0.7f);
        var birdViz = bird.AddComponent<RolloffVisualizer>();
        birdViz.placeholderHz = 1320f;
        birdViz.minRingMaterial = ringMin;
        birdViz.maxRingMaterial = ringMax;
        birdViz.readout = SceneBuilderUtil.AddLabel("Bird Readout", "Bird", perchPos + Vector3.up * 0.6f, 0.04f,
                                                    new Color(1f, 0.9f, 0.6f)).GetComponent<TextMesh>();
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Perch Trunk", new Vector3(6f, 2.2f, 6f),
                                      new Vector3(0.5f, 2.2f, 0.5f), bark);

        // Fire: a low crackling source straight ahead, with a light. Medium min distance.
        var firePos = new Vector3(0f, 0.3f, 6f);
        var fire = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Fire", firePos, new Vector3(0.8f, 0.3f, 0.8f), ember);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Fire Ring", new Vector3(0f, 0.1f, 6f),
                                      new Vector3(1.3f, 0.1f, 1.3f), stone);
        SceneBuilderUtil.AddPointLight("Fire Light", firePos + Vector3.up * 0.8f, new Color(1f, 0.6f, 0.2f), 8f, 2f, fire.transform);
        var fireSrc = AddSpatialSource(fire, 1.5f, 12f, 0.8f);
        var fireViz = fire.AddComponent<RolloffVisualizer>();
        fireViz.placeholderIsNoise = true;
        fireViz.placeholderHz = 120f;
        fireViz.minRingMaterial = ringMin;
        fireViz.maxRingMaterial = ringMax;
        fireViz.readout = SceneBuilderUtil.AddLabel("Fire Readout", "Fire", firePos + Vector3.up * 1.4f, 0.04f,
                                                    new Color(1f, 0.75f, 0.5f)).GetComponent<TextMesh>();

        // The lost shard: an audio beacon tucked behind the trees, behind and to the right of the start -----
        var beaconPos = new Vector3(8f, 1.2f, -6f);
        var beacon = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Lost Shard (Beacon)", beaconPos,
                                                   new Vector3(0.15f, 0.30f, 0.15f), shardMat);
        beacon.transform.rotation = Quaternion.Euler(45f, 0f, 45f);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Beacon Pedestal", new Vector3(8f, 0.45f, -6f),
                                      new Vector3(0.4f, 0.45f, 0.4f), stone);
        var beaconSrc = AddSpatialSource(beacon, 1.0f, 25f, 1.0f);
        beaconSrc.volume = 0.5f;
        var beaconLight = SceneBuilderUtil.AddPointLight("Beacon Glow", beaconPos + Vector3.up * 0.3f,
                                                         new Color(0.4f, 0.8f, 1f), 4f, 2f, beacon.transform);
        var beaconScript = beacon.AddComponent<AudioBeacon>();
        beaconScript.placeholderHz = 660f;
        beaconScript.glow = beaconLight.GetComponent<Light>();
        beaconScript.readout = SceneBuilderUtil.AddLabel("Beacon Readout", "Beacon: implement TODO 1-4 in AudioBeacon.cs",
                                                         beaconPos + Vector3.up * 0.9f, 0.035f,
                                                         new Color(0.6f, 0.9f, 1f)).GetComponent<TextMesh>();
        // A screen of bushes so you cannot simply see the shard from the start
        for (int i = 0; i < 4; i++)
        {
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Bush " + (i + 1),
                                          new Vector3(5.5f + i * 0.9f, 0.7f, -3.5f - i * 0.4f), Vector3.one * 1.5f, leaves);
        }

        // Listener audit label near the start -----------------------------
        var checkGo = SceneBuilderUtil.AddLabel("Listener Check", "Listener audit: implement TODO 1-3 in ListenerCheck.cs",
                                                new Vector3(0f, 1.9f, 2.5f), 0.035f, new Color(1f, 0.95f, 0.5f));
        var check = checkGo.AddComponent<ListenerCheck>();
        check.readout = checkGo.GetComponent<TextMesh>();
        check.sources = new RolloffVisualizer[] { streamViz, birdViz, fireViz };

        // Welcome sign -----------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 6.1 - Hear the Forest\nStream to your left, bird up to your right, fire ahead.\nSomething else is calling from behind you...",
                                  new Vector3(0f, 2.6f, 4f), 0.05f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_6_1");
        Selection.activeGameObject = rig;

        // Keep the compiler quiet about the sources we configured but do not otherwise reference here.
        if (streamSrc == null || birdSrc == null || fireSrc == null) Debug.LogWarning("[Activity] An AudioSource failed to attach.");
    }

    /// <summary>Adds a fully 3D, looping AudioSource with logarithmic rolloff and NO clip (the scripts add a placeholder).</summary>
    static AudioSource AddSpatialSource(GameObject go, float minDistance, float maxDistance, float volume)
    {
        var src = go.AddComponent<AudioSource>();
        src.clip = null;                       // students drop a real file here; ToneFactory fills in until then
        src.playOnAwake = true;
        src.loop = true;
        src.spatialBlend = 1f;                 // 1 = fully 3D; 0 = 2D (same in both ears regardless of position)
        src.rolloffMode = AudioRolloffMode.Logarithmic;
        src.minDistance = minDistance;
        src.maxDistance = maxDistance;
        src.dopplerLevel = 1f;
        src.spread = 0f;
        src.volume = volume;
        return src;
    }
}

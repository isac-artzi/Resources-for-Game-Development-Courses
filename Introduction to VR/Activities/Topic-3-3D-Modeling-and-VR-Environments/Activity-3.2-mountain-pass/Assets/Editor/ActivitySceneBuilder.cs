// ActivitySceneBuilder.cs — Activity 3.2: Mountain Pass: Terrain and Weather
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a real Unity Terrain (200 x 200 m, procedural heightmap with
// sin/cos hills, side ridges and a ramp climbing toward +z), two solid-color terrain layers painted by altitude,
// a TeleportationArea on the terrain, cairn markers and the Hermit's hut at the top, a snow ParticleSystem that
// follows the player (SnowEmitter), and a Weather object carrying WeatherController + AltitudeFog with a HUD label.
// Created by Isac Artzi

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using ActivityTools;

public static class ActivitySceneBuilder
{
    const string TerrainFolder = "Assets/Terrain";
    const int HeightmapResolution = 129;      // must be 2^n + 1
    const int AlphamapResolution = 128;
    const float TerrainSize = 200f;           // meters, x and z
    const float TerrainMaxHeight = 60f;       // meters, y at heightmap value 1.0
    static readonly Vector3 TerrainOrigin = new Vector3(-100f, 0f, -100f);   // so world (0,0) is the terrain center

    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        var sun = Object.FindFirstObjectByType<Light>();   // the directional light NewScene created
        SceneBuilderUtil.SetSun(new Color(0.95f, 0.95f, 1f), 1.2f, new Vector3(35f, 160f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.72f, 0.80f, 0.92f), 0.006f);

        // Terrain ------------------------------------------------------------
        var terrainGo = BuildTerrain();

        // Player at the low end of the valley, on the terrain surface -----------------------------------
        var rigPos = WorldPoint(0.5f, 0.15f);
        var rig = SceneBuilderUtil.AddXrRig(rigPos);
        var cam = rig.GetComponentInChildren<Camera>(true);

        // Materials for props -----------------------------------------------
        var stone = SceneBuilderUtil.MakeMaterial("Cairn Stone", new Color(0.45f, 0.45f, 0.48f));
        var wood  = SceneBuilderUtil.MakeMaterial("Hut Wood", new Color(0.40f, 0.28f, 0.16f));
        var glow  = SceneBuilderUtil.MakeEmissiveMaterial("Hermit Glow", new Color(1f, 0.8f, 0.5f), 1.5f);

        // Cairns marking the route up the pass (u, v are terrain-relative 0..1) -------------------------
        var cairns = SceneBuilderUtil.AddEmpty("Cairns", Vector3.zero);
        Vector2[] route = { new Vector2(0.50f, 0.25f), new Vector2(0.42f, 0.38f), new Vector2(0.56f, 0.50f),
                            new Vector2(0.46f, 0.62f), new Vector2(0.54f, 0.74f), new Vector2(0.50f, 0.85f) };
        for (int i = 0; i < route.Length; i++)
        {
            var p = WorldPoint(route[i].x, route[i].y);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Cairn " + (i + 1), p + Vector3.up * 0.5f,
                                          new Vector3(0.7f, 1.0f, 0.7f), stone, cairns.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Cairn Top " + (i + 1), p + Vector3.up * 1.2f,
                                          new Vector3(0.4f, 0.4f, 0.4f), stone, cairns.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(cairns);

        // The Hermit's hut at the top of the pass --------------------------------------------------------
        var hutPos = WorldPoint(0.5f, 0.92f);
        var hut = SceneBuilderUtil.AddEmpty("Hermit's Hut", hutPos);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Hut Body", hutPos + Vector3.up * 1.5f, new Vector3(4f, 3f, 4f), wood, hut.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Mountain Hermit", hutPos + new Vector3(0f, 1f, -3f),
                                      new Vector3(0.5f, 0.9f, 0.5f), glow, hut.transform);
        SceneBuilderUtil.AddPointLight("Hermit Lantern", hutPos + new Vector3(0f, 2.2f, -3f), new Color(1f, 0.8f, 0.5f), 12f, 2f, hut.transform);
        SceneBuilderUtil.AddLabel("Hut Label", "The Mountain Hermit\n(top of the pass)", hutPos + new Vector3(0f, 3.8f, -3f), 0.08f,
                                  new Color(1f, 0.9f, 0.7f), hut.transform);
        SceneBuilderUtil.MarkStaticRecursive(hut);

        // Snow ------------------------------------------------------------------
        var snowGo = SceneBuilderUtil.AddEmpty("Snow", rigPos + Vector3.up * 8f);
        var ps = snowGo.AddComponent<ParticleSystem>();
        ConfigureSnow(ps);
        var snow = snowGo.AddComponent<SnowEmitter>();
        snow.target = cam != null ? cam.transform : null;
        snow.heightAboveHead = 6f;
        snow.leadDistance = 4f;
        snow.maxRate = 800f;

        // Weather + altitude fog ----------------------------------------------------
        var weatherGo = SceneBuilderUtil.AddEmpty("Weather", Vector3.zero);
        var weather = weatherGo.AddComponent<WeatherController>();
        weather.sun = sun;
        weather.snow = snow;
        weather.transitionSeconds = 4f;
        weather.startWeather = WeatherController.Kind.Clear;

        var altitude = weatherGo.AddComponent<AltitudeFog>();
        altitude.weather = weather;
        altitude.lowAltitude = 8f;
        altitude.highAltitude = 40f;
        altitude.maxExtraMultiplier = 2f;

        // HUD labels parented to the camera so they travel with the head ---------------------------------
        if (cam != null)
        {
            var hud = SceneBuilderUtil.AddLabel("Altitude HUD", "AltitudeFog: implement TODO 1-4",
                                                cam.transform.TransformPoint(new Vector3(0f, -0.20f, 0.9f)), 0.012f,
                                                new Color(1f, 0.95f, 0.6f), cam.transform);
            hud.transform.localRotation = Quaternion.identity;
            altitude.readout = hud.GetComponent<TextMesh>();

            var weatherHud = SceneBuilderUtil.AddLabel("Weather HUD", "Weather: Clear   (1 Clear  2 Overcast  3 Blizzard)",
                                                       cam.transform.TransformPoint(new Vector3(0f, 0.22f, 0.9f)), 0.012f,
                                                       Color.white, cam.transform);
            weatherHud.transform.localRotation = Quaternion.identity;
            weather.readout = weatherHud.GetComponent<TextMesh>();
        }

        // Welcome sign -------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 3.2 - Mountain Pass\nTeleport up the valley to the Hermit's hut\nKeys 1 / 2 / 3 change the weather (after TODO 3)",
                                  rigPos + new Vector3(0f, 2.4f, 4f), 0.05f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_3_2");
        Selection.activeGameObject = weatherGo;
    }

    // ----------------------------------------------------------------------
    // Heightmap: u = x fraction (0..1), v = z fraction (0..1). Returns 0..1 (fraction of TerrainMaxHeight).
    // A valley floor climbs from the start (v ~ 0.15) toward the pass at v ~ 0.9; ridges rise to both sides.
    // ----------------------------------------------------------------------
    static float Height01(float u, float v)
    {
        float climb = Mathf.InverseLerp(0.15f, 0.90f, v);
        float ramp = Mathf.SmoothStep(0f, 1f, climb) * 0.60f;
        float fade = Mathf.Clamp01((v - 0.08f) * 4f);   // keep the start area flat
        float hills = (Mathf.Sin(u * Mathf.PI * 3f) * Mathf.Cos(v * Mathf.PI * 2.5f) * 0.10f
                     + Mathf.Sin((u * 2f + v) * Mathf.PI * 5f) * 0.04f) * fade;
        float ridges = Mathf.SmoothStep(0f, 1f, Mathf.Abs(u - 0.5f) * 2.2f) * 0.22f;
        return Mathf.Clamp01(0.04f + ramp + hills + ridges);
    }

    /// <summary>World position on the terrain surface for terrain-relative (u, v).</summary>
    static Vector3 WorldPoint(float u, float v)
    {
        return TerrainOrigin + new Vector3(u * TerrainSize, Height01(u, v) * TerrainMaxHeight, v * TerrainSize);
    }

    static GameObject BuildTerrain()
    {
        SceneBuilderUtil.EnsureFolder(TerrainFolder);

        // 1. Heightmap ---------------------------------------------------------
        var data = new TerrainData();
        data.heightmapResolution = HeightmapResolution;                 // set BEFORE size (it resets size)
        data.size = new Vector3(TerrainSize, TerrainMaxHeight, TerrainSize);
        var heights = new float[HeightmapResolution, HeightmapResolution];   // [z, x]
        for (int z = 0; z < HeightmapResolution; z++)
        {
            float v = z / (float)(HeightmapResolution - 1);
            for (int x = 0; x < HeightmapResolution; x++)
            {
                float u = x / (float)(HeightmapResolution - 1);
                heights[z, x] = Height01(u, v);
            }
        }
        data.SetHeights(0, 0, heights);

        // 2. Two placeholder layers (solid colors). Replace their textures with real PBR sets (README Task 5).
        var rockLayer = MakeSolidLayer("Rock", new Color(0.42f, 0.40f, 0.40f), 12f);
        var snowLayer = MakeSolidLayer("Snow", new Color(0.92f, 0.94f, 0.98f), 8f);
        data.terrainLayers = new TerrainLayer[] { rockLayer, snowLayer };

        // 3. Paint by altitude: snow above ~55 % of max height, rock below, blended over a band.
        data.alphamapResolution = AlphamapResolution;
        var maps = new float[AlphamapResolution, AlphamapResolution, 2];
        for (int z = 0; z < AlphamapResolution; z++)
        {
            float v = z / (float)(AlphamapResolution - 1);
            for (int x = 0; x < AlphamapResolution; x++)
            {
                float u = x / (float)(AlphamapResolution - 1);
                float snowWeight = Mathf.InverseLerp(0.45f, 0.65f, Height01(u, v));
                maps[z, x, 0] = 1f - snowWeight;
                maps[z, x, 1] = snowWeight;
            }
        }
        data.SetAlphamaps(0, 0, maps);

        AssetDatabase.CreateAsset(data, TerrainFolder + "/MountainPass.asset");

        // 4. The Terrain GameObject (comes with Terrain + TerrainCollider) ---------------------------------
        var terrainGo = Terrain.CreateTerrainGameObject(data);
        terrainGo.name = "Mountain Pass Terrain";
        terrainGo.transform.position = TerrainOrigin;
        terrainGo.isStatic = true;

        // Teleport anywhere on the terrain with the rig's teleport interactor.
        var area = terrainGo.AddComponent<TeleportationArea>();
        area.matchOrientation = MatchOrientation.WorldSpaceUp;

        return terrainGo;
    }

    /// <summary>A TerrainLayer whose diffuse texture is a tiny solid-color PNG saved under Assets/Terrain.</summary>
    static TerrainLayer MakeSolidLayer(string name, Color color, float tileMeters)
    {
        string texPath = TerrainFolder + "/" + name + "Placeholder.png";
        var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        var pixels = new Color[16];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        File.WriteAllBytes(Path.Combine(Directory.GetParent(Application.dataPath).FullName, texPath), tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(texPath);
        var diffuse = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);

        var layer = new TerrainLayer();
        layer.diffuseTexture = diffuse;
        layer.tileSize = new Vector2(tileMeters, tileMeters);
        AssetDatabase.CreateAsset(layer, TerrainFolder + "/" + name + "Layer.terrainlayer");
        return layer;
    }

    /// <summary>Slow, small, white flakes in world space spawning from a wide flat box.</summary>
    static void ConfigureSnow(ParticleSystem ps)
    {
        var main = ps.main;
        main.loop = true;
        main.startLifetime = 8f;
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.04f, 0.09f);
        main.startColor = Color.white;
        main.maxParticles = 6000;
        main.gravityModifier = 0.05f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0f;          // WeatherController turns this up via SnowEmitter.SetIntensity

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(30f, 0.5f, 30f);

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = 0f;
        velocity.y = -1.2f;
        velocity.z = 0f;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sharedMaterial = MakeSnowflakeMaterial();
    }

    static Material MakeSnowflakeMaterial()
    {
        SceneBuilderUtil.EnsureFolder(SceneBuilderUtil.MaterialsFolder);
        string path = SceneBuilderUtil.MaterialsFolder + "/Snowflake.mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;

        Shader shader = null;
        if (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null)
            shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Particles/Standard Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        var mat = new Material(shader);
        mat.color = Color.white;
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }
}

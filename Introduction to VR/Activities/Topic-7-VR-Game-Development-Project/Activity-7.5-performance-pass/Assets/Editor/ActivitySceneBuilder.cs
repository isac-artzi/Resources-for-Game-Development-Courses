// ActivitySceneBuilder.cs — Activity 7.5: Performance Pass for Quest 3
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a deliberately wasteful ruins courtyard — 30 rubble blocks each
// with its own material, 40 identical non-static pillars, six shadow-casting torches, and a mural with a
// 2048 px uncompressed texture — plus a Perf HUD on the camera. The audits (Activity > Audit) find the waste;
// the student fixes it and measures the difference.
// Created by Isac Artzi

using System.IO;
using UnityEditor;
using UnityEngine;
using ActivityTools;

public static class ActivitySceneBuilder
{
    const string TexturesFolder = "Assets/Textures";

    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1f, 0.9f, 0.75f), 0.8f, new Vector3(35f, 40f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.75f, 0.65f, 0.50f), 0.01f);

        var sand   = SceneBuilderUtil.MakeMaterial("Sand", new Color(0.70f, 0.62f, 0.45f));
        var stone  = SceneBuilderUtil.MakeMaterial("RuinStone", new Color(0.55f, 0.50f, 0.42f));
        var pillar = SceneBuilderUtil.MakeMaterial("Pillar", new Color(0.62f, 0.58f, 0.50f));   // instancing OFF on purpose
        pillar.enableInstancing = false;
        EditorUtility.SetDirty(pillar);

        SceneBuilderUtil.AddFloor(40f, sand);
        var rig = SceneBuilderUtil.AddXrRig(new Vector3(0f, 0f, -6f));

        // Walls (shared material, static) --------------------------------------------
        var walls = SceneBuilderUtil.AddEmpty("Walls", Vector3.zero);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall N", new Vector3(0f, 2f, 12f), new Vector3(24f, 4f, 1f), stone, walls.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall E", new Vector3(12f, 2f, 2f), new Vector3(1f, 4f, 20f), stone, walls.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall W", new Vector3(-12f, 2f, 2f), new Vector3(1f, 4f, 20f), stone, walls.transform);
        SceneBuilderUtil.MarkStaticRecursive(walls);

        // 40 identical pillars, NOT static, material without instancing -> instancing candidates ----
        var pillars = SceneBuilderUtil.AddEmpty("Pillars", Vector3.zero);
        for (int i = 0; i < 40; i++)
        {
            float x = -10f + (i % 10) * 2.2f;
            float z = 2f + (i / 10) * 2.5f;
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Pillar", new Vector3(x, 1.5f, z), new Vector3(0.5f, 1.5f, 0.5f), pillar, pillars.transform);
        }

        // 30 rubble blocks, each with a UNIQUE material -> single-use materials ---------------
        var rubble = SceneBuilderUtil.AddEmpty("Rubble", Vector3.zero);
        for (int i = 0; i < 30; i++)
        {
            float shade = 0.4f + 0.02f * i;
            var uniqueMat = SceneBuilderUtil.MakeMaterial("Unique_" + i.ToString("00"), new Color(shade, shade * 0.95f, shade * 0.85f));
            float a = i * 0.7f;
            var p = new Vector3(Mathf.Cos(a) * (3f + i * 0.2f), 0.25f, 2f + Mathf.Sin(a) * (3f + i * 0.15f));
            var b = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Rubble " + i, p, new Vector3(0.5f, 0.5f, 0.5f), uniqueMat, rubble.transform);
            b.transform.rotation = Quaternion.Euler(0f, i * 37f, 0f);
        }

        // Six torches with real-time shadows ------------------------------------------
        for (int i = 0; i < 6; i++)
        {
            var p = new Vector3(-10f + i * 4f, 2.5f, 11f);
            var torch = SceneBuilderUtil.AddPointLight("Torch " + i, p, new Color(1f, 0.7f, 0.4f), 8f, 2f);
            torch.GetComponent<Light>().shadows = LightShadows.Soft;
        }

        // Mural with a big uncompressed texture -----------------------------------------
        var bigTex = MakeNoiseTexture("BigMural", 2048);
        var smallTex = MakeCheckerTexture("SmallTile", 512);
        var muralMat = SceneBuilderUtil.MakeMaterial("Mural", Color.white);
        muralMat.mainTexture = bigTex;
        EditorUtility.SetDirty(muralMat);
        var tileMat = SceneBuilderUtil.MakeMaterial("Tile", Color.white);
        tileMat.mainTexture = smallTex;
        EditorUtility.SetDirty(tileMat);
        var mural = SceneBuilderUtil.AddPrimitive(PrimitiveType.Quad, "Mural", new Vector3(0f, 2f, 11.4f), new Vector3(6f, 3f, 1f), muralMat);
        mural.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        mural.isStatic = true;
        var tile = SceneBuilderUtil.AddPrimitive(PrimitiveType.Quad, "Floor Tile", new Vector3(0f, 0.01f, 0f), new Vector3(4f, 4f, 1f), tileMat);
        tile.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        tile.isStatic = true;

        // Perf HUD on the camera -----------------------------------------------------------
        var cam = rig.GetComponentInChildren<Camera>(true);
        var hudGo = SceneBuilderUtil.AddLabel("Perf HUD", "", Vector3.zero, 0.01f, new Color(0.5f, 1f, 0.5f));
        var tm = hudGo.GetComponent<TextMesh>();
        tm.anchor = TextAnchor.UpperLeft;
        tm.alignment = TextAlignment.Left;
        if (cam != null)
        {
            hudGo.transform.SetParent(cam.transform, false);
            hudGo.transform.localPosition = new Vector3(-0.45f, 0.35f, 1.2f);
            hudGo.transform.localRotation = Quaternion.identity;
        }
        var checker = hudGo.AddComponent<PerfBudgetChecker>();
        checker.hud = tm;
        checker.targetHz = 72f;

        // BatchingAudit and TextureAudit are editor menu items (Activity > Audit > ...), not components;
        // there is nothing to AddComponent<BatchingAudit> or AddComponent<TextureAudit> here.

        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 7.5 - Performance Pass\nHUD top-left (F1 toggles)\nActivity > Audit > Batching / Texture Audit",
                                  new Vector3(0f, 2.2f, -2f), 0.04f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_7_5");
        Selection.activeGameObject = rig;
    }

    /// <summary>Writes a PNG into Assets/Textures and imports it, so it has a real TextureImporter to audit.</summary>
    static Texture2D SaveAndImport(string name, Texture2D tex)
    {
        SceneBuilderUtil.EnsureFolder(TexturesFolder);
        string path = TexturesFolder + "/" + name + ".png";
        if (!File.Exists(path))
        {
            File.WriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path);
        }
        Object.DestroyImmediate(tex);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    static Texture2D MakeNoiseTexture(string name, int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float n = Mathf.PerlinNoise(x * 0.01f, y * 0.01f) * 0.6f + Mathf.PerlinNoise(x * 0.08f, y * 0.08f) * 0.4f;
                byte v = (byte)(n * 255f);
                pixels[y * size + x] = new Color32((byte)(v * 0.9f), (byte)(v * 0.8f), (byte)(v * 0.6f), 255);
            }
        tex.SetPixels32(pixels);
        tex.Apply();
        return SaveAndImport(name, tex);
    }

    static Texture2D MakeCheckerTexture(string name, int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                bool on = ((x / 64) + (y / 64)) % 2 == 0;
                pixels[y * size + x] = on ? new Color32(200, 190, 170, 255) : new Color32(120, 110, 95, 255);
            }
        tex.SetPixels32(pixels);
        tex.Apply();
        return SaveAndImport(name, tex);
    }
}

// ActivitySceneBuilder.cs — Activity 5.3: Hybrid: Dash and Blink
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: an Ancient Ruins courtyard with a colonnade, a wall with a
// doorway, and rubble to dash around; two overlay quads (vignette + blink) under the camera driven by
// VignetteController; and DashLocomotion + BlinkStep on the rig with the right controller as aim source.
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using ActivityTools;

public static class ActivitySceneBuilder
{
    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1f, 0.90f, 0.75f), 0.9f, new Vector3(25f, 140f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.60f, 0.55f, 0.50f), 0.012f);

        // Materials -------------------------------------------------------
        var flagstone = SceneBuilderUtil.MakeMaterial("Flagstone", new Color(0.50f, 0.47f, 0.42f));
        var pillarMat = SceneBuilderUtil.MakeMaterial("Pillar",    new Color(0.70f, 0.66f, 0.58f));
        var wallMat   = SceneBuilderUtil.MakeMaterial("RuinWall",  new Color(0.45f, 0.42f, 0.38f));
        var rubbleMat = SceneBuilderUtil.MakeMaterial("Rubble",    new Color(0.38f, 0.36f, 0.34f));
        var guideMat  = SceneBuilderUtil.MakeEmissiveMaterial("Guide", new Color(0.90f, 0.85f, 1.00f), 1.2f);
        var vignetteMat = MakeOverlayMaterial("VignetteBlack", new Color(0f, 0f, 0f, 0f), 50);
        var blinkMat    = MakeOverlayMaterial("BlinkBlack",    new Color(0f, 0f, 0f, 0f), 60);   // drawn after the vignette

        // Floor (also teleportable, so you can compare teleport vs dash vs blink) ---------------------------
        var floor = SceneBuilderUtil.AddFloor(40f, flagstone);
        var area = floor.AddComponent<TeleportationArea>();
        area.matchOrientation = MatchOrientation.WorldSpaceUp;

        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);
        Camera cam = rig.GetComponentInChildren<Camera>(true);
        Transform rightHand = FindChildByName(rig.transform, "Right Controller");

        // Colonnade: two rows of pillars 3 m apart, 2.5 m spacing — dash between them, clamp against them ----
        var ruins = SceneBuilderUtil.AddEmpty("Ruins", Vector3.zero);
        for (int i = 0; i < 6; i++)
        {
            float z = 3f + i * 2.5f;
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Pillar L" + (i + 1), new Vector3(-1.5f, 1.5f, z), new Vector3(0.5f, 1.5f, 0.5f), pillarMat, ruins.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Pillar R" + (i + 1), new Vector3( 1.5f, 1.5f, z), new Vector3(0.5f, 1.5f, 0.5f), pillarMat, ruins.transform);
        }
        // One fallen pillar across the path at z = 10.5 — dash straight at it and the clamp should stop you.
        var fallen = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Fallen Pillar", new Vector3(0f, 0.25f, 10.5f), new Vector3(0.5f, 1.4f, 0.5f), pillarMat, ruins.transform);
        fallen.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        // End wall with a 1.2 m doorway at z = 19 ----------------------------------------------------------
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall Left",  new Vector3(-4.1f, 1.75f, 19f), new Vector3(7f, 3.5f, 0.5f), wallMat, ruins.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall Right", new Vector3( 4.1f, 1.75f, 19f), new Vector3(7f, 3.5f, 0.5f), wallMat, ruins.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Lintel",     new Vector3( 0f, 3.0f, 19f), new Vector3(1.2f, 1.0f, 0.5f), wallMat, ruins.transform);

        // Rubble scattered in the courtyard beyond the colonnade ------------------------------------------------
        Vector3[] rubbleSpots = { new Vector3(-3f, 0f, 14f), new Vector3(2.5f, 0f, 16f), new Vector3(-1f, 0f, 17f), new Vector3(4f, 0f, 13f) };
        for (int i = 0; i < rubbleSpots.Length; i++)
        {
            float s = 0.6f + 0.3f * (i % 3);
            var block = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Rubble " + (i + 1), rubbleSpots[i] + Vector3.up * s * 0.5f, Vector3.one * s, rubbleMat, ruins.transform);
            block.transform.rotation = Quaternion.Euler(0f, 20f * i, 0f);
        }
        SceneBuilderUtil.MarkStaticRecursive(ruins);

        // The Guide beyond the doorway -------------------------------------------------------------------------
        var guide = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Mysterious Guide", new Vector3(0f, 1f, 23f), new Vector3(0.6f, 1f, 0.6f), guideMat);
        SceneBuilderUtil.AddPointLight("Guide Glow", new Vector3(0f, 1.6f, 22.5f), new Color(0.8f, 0.7f, 1f), 5f, 1.5f, guide.transform);
        SceneBuilderUtil.AddLabel("Guide Label", "Quick, but never careless", new Vector3(0f, 2.5f, 23f), 0.06f, new Color(0.95f, 0.9f, 1f));

        // Overlays under the camera -----------------------------------------------------------------------------
        VignetteController vignette = null;
        if (cam != null)
        {
            cam.nearClipPlane = Mathf.Min(cam.nearClipPlane, 0.05f);
            var overlays = SceneBuilderUtil.AddEmpty("Comfort Overlays", Vector3.zero, cam.transform);
            overlays.transform.localPosition = Vector3.zero;
            overlays.transform.localRotation = Quaternion.identity;

            // Vignette quad: 0.7 m wide at 0.2 m → about 120 degrees of view, wider than the headset's FOV.
            var vq = MakeOverlayQuad("Vignette Quad", overlays.transform, 0.20f, 0.7f, vignetteMat);
            // Blink quad: 4 m wide at 0.25 m → covers everything.
            var bq = MakeOverlayQuad("Blink Quad", overlays.transform, 0.25f, 4f, blinkMat);

            vignette = overlays.AddComponent<VignetteController>();
            vignette.vignetteQuad = vq.GetComponent<Renderer>();
            vignette.blinkQuad = bq.GetComponent<Renderer>();
        }
        else
        {
            Debug.LogWarning("[Activity] No camera found under the rig; overlays were not created.");
        }

        // Locomotion scripts on the rig ----------------------------------------------------------------------------
        var dash = rig.AddComponent<DashLocomotion>();
        dash.origin = rig.GetComponent<Unity.XR.CoreUtils.XROrigin>();
        dash.aimSource = rightHand != null ? rightHand : (cam != null ? cam.transform : null);
        dash.vignette = vignette;
        dash.dashDistance = 3f;
        dash.dashSeconds = 0.2f;

        var blink = rig.AddComponent<BlinkStep>();
        blink.origin = dash.origin;
        blink.dash = dash;
        blink.vignette = vignette;
        blink.stepDistance = 2f;

        // Signs ----------------------------------------------------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Ancient Ruins\nActivity 5.3 - Hybrid: Dash and Blink\nF = dash   B = blink\nOpen README.md (Activity > Open README)",
                                  new Vector3(0f, 2.6f, 2.5f), 0.05f, Color.white);
        SceneBuilderUtil.AddLabel("Fallen Sign", "Dash at me", new Vector3(0f, 1.2f, 10.5f), 0.04f, new Color(1f, 0.8f, 0.6f));
        SceneBuilderUtil.AddLabel("Door Sign", "1.2 m doorway", new Vector3(0f, 3.9f, 19f), 0.04f, new Color(1f, 0.8f, 0.6f));

        SceneBuilderUtil.SaveScene(scene, "Activity_5_3");
        Selection.activeGameObject = rig;
    }

    // ------------------------------------------------------------------------------------------------------------
    // Local helpers
    // ------------------------------------------------------------------------------------------------------------

    static Transform FindChildByName(Transform root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    /// <summary>A quad parented under the camera at the given local distance, collider removed, shadows off.</summary>
    static GameObject MakeOverlayQuad(string name, Transform parent, float distance, float size, Material mat)
    {
        var go = SceneBuilderUtil.AddPrimitive(PrimitiveType.Quad, name, Vector3.zero, Vector3.one * size, mat, parent);
        go.transform.localPosition = new Vector3(0f, 0f, distance);
        go.transform.localRotation = Quaternion.identity;
        var col = go.GetComponent<Collider>();
        if (col != null) Object.DestroyImmediate(col);   // a collider on the camera would block rays and casts
        var rend = go.GetComponent<Renderer>();
        rend.shadowCastingMode = ShadowCastingMode.Off;
        rend.receiveShadows = false;
        return go;
    }

    /// <summary>
    /// A transparent unlit material with a texture slot (the vignette needs one). URP/Unlit set to Transparent when a
    /// render pipeline is active, otherwise the built-in Sprites/Default. queueOffset orders overlays among themselves.
    /// </summary>
    static Material MakeOverlayMaterial(string name, Color color, int queueOffset)
    {
        SceneBuilderUtil.EnsureFolder(SceneBuilderUtil.MaterialsFolder);
        string path = SceneBuilderUtil.MaterialsFolder + "/" + name + ".mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null)
        {
            existing.color = color;
            EditorUtility.SetDirty(existing);
            return existing;
        }

        Shader shader = null;
        if (GraphicsSettings.currentRenderPipeline != null) shader = Shader.Find("Universal Render Pipeline/Unlit");
        Material mat;
        if (shader != null)
        {
            mat = new Material(shader);
            mat.SetFloat("_Surface", 1f);   // 1 = Transparent
            mat.SetFloat("_Blend", 0f);     // 0 = Alpha
            mat.SetFloat("_ZWrite", 0f);
            mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.SetOverrideTag("RenderType", "Transparent");
        }
        else
        {
            mat = new Material(Shader.Find("Sprites/Default"));
        }
        mat.renderQueue = (int)RenderQueue.Transparent + queueOffset;
        mat.color = color;
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }
}

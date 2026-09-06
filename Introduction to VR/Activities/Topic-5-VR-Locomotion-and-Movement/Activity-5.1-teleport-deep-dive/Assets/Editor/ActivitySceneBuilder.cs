// ActivitySceneBuilder.cs — Activity 5.1: Teleport, Properly
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a Mountain Pass ledge with a teleportable floor, a too-steep
// stone ramp, a broken wall with a narrow gap, three anchor platforms that face the Guide on arrival,
// a black fade quad under the camera, and an arc-preview object wired to the right controller.
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
        SceneBuilderUtil.SetSun(new Color(0.95f, 0.95f, 1f), 1.0f, new Vector3(35f, 20f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.70f, 0.75f, 0.85f), 0.015f);

        // Materials -------------------------------------------------------
        var rock     = SceneBuilderUtil.MakeMaterial("Rock",      new Color(0.45f, 0.45f, 0.48f));
        var ramp     = SceneBuilderUtil.MakeMaterial("RampRock",  new Color(0.55f, 0.50f, 0.45f));
        var wallMat  = SceneBuilderUtil.MakeMaterial("OldWall",   new Color(0.40f, 0.38f, 0.35f));
        var platform = SceneBuilderUtil.MakeEmissiveMaterial("AnchorStone", new Color(0.35f, 0.75f, 1.0f), 0.8f);
        var guideMat = SceneBuilderUtil.MakeEmissiveMaterial("Guide", new Color(0.90f, 0.85f, 1.00f), 1.2f);
        var fadeMat  = MakeOverlayMaterial("FadeBlack", new Color(0f, 0f, 0f, 0f), false);
        var lineMat  = MakeOverlayMaterial("ArcLine", Color.white, true);   // vertex colors → LineRenderer.startColor works

        // Floor: one big TeleportationArea -----------------------------------
        var floor = SceneBuilderUtil.AddFloor(40f, rock);
        var floorArea = floor.AddComponent<TeleportationArea>();
        floorArea.matchOrientation = MatchOrientation.WorldSpaceUp;

        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);

        // The too-steep ramp (30 degrees) — XRI happily lets you teleport onto it; your validator should not ---------
        var rampGo = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Stone Ramp", new Vector3(-5f, 1.0f, 6f),
                                                   new Vector3(3f, 0.2f, 5f), ramp);
        rampGo.transform.rotation = Quaternion.Euler(-30f, 0f, 0f);
        rampGo.isStatic = true;
        var rampArea = rampGo.AddComponent<TeleportationArea>();
        rampArea.matchOrientation = MatchOrientation.WorldSpaceUp;

        // Broken wall with a 0.7 m gap — landing next to it should fail the clearance test ----------------------
        var walls = SceneBuilderUtil.AddEmpty("Broken Wall", Vector3.zero);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall Left",  new Vector3(2.0f, 1.0f, 5f), new Vector3(3.0f, 2.0f, 0.4f), wallMat, walls.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall Right", new Vector3(5.35f, 1.0f, 5f), new Vector3(2.0f, 2.0f, 0.4f), wallMat, walls.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Pillar",     new Vector3(0.0f, 1.25f, 9f), new Vector3(0.6f, 2.5f, 0.6f), wallMat, walls.transform);
        SceneBuilderUtil.MarkStaticRecursive(walls);

        // The Guide at the far end of the ledge ---------------------------------------------------------------
        var guide = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Mysterious Guide", new Vector3(0f, 1f, 14f),
                                                  new Vector3(0.6f, 1f, 0.6f), guideMat);
        SceneBuilderUtil.AddPointLight("Guide Glow", new Vector3(0f, 1.6f, 13.5f), new Color(0.8f, 0.7f, 1f), 5f, 1.5f, guide.transform);
        SceneBuilderUtil.AddLabel("Guide Label", "Cross the pass one step at a time", new Vector3(0f, 2.5f, 14f), 0.06f,
                                  new Color(0.95f, 0.9f, 1f));

        // Anchor platforms: each has a child whose forward points at the Guide → arrival orientation --------------
        Vector3[] anchorSpots = { new Vector3(-3f, 0f, 3f), new Vector3(3.5f, 0f, 8f), new Vector3(-2f, 0f, 11.5f) };
        var anchors = SceneBuilderUtil.AddEmpty("Anchors", Vector3.zero);
        for (int i = 0; i < anchorSpots.Length; i++)
        {
            var spot = anchorSpots[i];
            var plat = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Anchor Platform " + (i + 1),
                                                     spot + Vector3.up * 0.1f, new Vector3(1.2f, 0.1f, 1.2f), platform, anchors.transform);
            var anchor = plat.AddComponent<TeleportationAnchor>();
            anchor.matchOrientation = MatchOrientation.TargetUpAndForward;

            var point = SceneBuilderUtil.AddEmpty("Anchor Point", spot + Vector3.up * 0.2f, plat.transform);
            Vector3 toGuide = guide.transform.position - point.transform.position;
            toGuide.y = 0f;
            point.transform.rotation = Quaternion.LookRotation(toGuide);
            anchor.teleportAnchorTransform = point.transform;

            SceneBuilderUtil.AddLabel("Anchor Label " + (i + 1), "Anchor " + (i + 1), spot + Vector3.up * 1.2f, 0.04f,
                                      new Color(0.6f, 0.9f, 1f), plat.transform);
        }

        // Fade quad parented under the camera --------------------------------------------------------------------
        Camera cam = rig.GetComponentInChildren<Camera>(true);
        if (cam != null)
        {
            cam.nearClipPlane = Mathf.Min(cam.nearClipPlane, 0.05f);
            var fadeGo = SceneBuilderUtil.AddPrimitive(PrimitiveType.Quad, "Teleport Fade", Vector3.zero, Vector3.one * 4f, fadeMat, cam.transform);
            fadeGo.transform.localPosition = new Vector3(0f, 0f, 0.2f);
            fadeGo.transform.localRotation = Quaternion.identity;
            var col = fadeGo.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);   // a collider on the camera would block every teleport ray
            var rend = fadeGo.GetComponent<Renderer>();
            rend.shadowCastingMode = ShadowCastingMode.Off;
            rend.receiveShadows = false;
            var fade = fadeGo.AddComponent<TeleportFade>();
            fade.fadePanel = rend;
            fade.provider = rig.GetComponentInChildren<TeleportationProvider>(true);
        }
        else
        {
            Debug.LogWarning("[Activity] No camera found under the rig; Teleport Fade was not created.");
        }

        // Arc preview + validator + status label -------------------------------------------------------------------
        var arcGo = SceneBuilderUtil.AddEmpty("Arc Preview", Vector3.zero);
        var lr = arcGo.AddComponent<LineRenderer>();
        lr.material = lineMat;
        lr.widthMultiplier = 0.02f;
        lr.positionCount = 0;
        lr.useWorldSpace = true;
        lr.shadowCastingMode = ShadowCastingMode.Off;
        lr.receiveShadows = false;
        var validator = arcGo.AddComponent<LandingValidator>();
        validator.maxSlopeDegrees = 20f;
        var arc = arcGo.AddComponent<TeleportArcPreview>();
        arc.line = lr;
        arc.validator = validator;
        arc.aimSource = FindChildByName(rig.transform, "Right Controller");
        if (arc.aimSource == null && cam != null) arc.aimSource = cam.transform;   // desktop fallback
        var status = SceneBuilderUtil.AddLabel("Landing Status", "", new Vector3(0f, 0.3f, 3f), 0.04f, Color.white, arcGo.transform);
        arc.statusLabel = status.GetComponent<TextMesh>();

        // Signs ---------------------------------------------------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Mountain Pass\nActivity 5.1 - Teleport, Properly\nOpen README.md (Activity > Open README)",
                                  new Vector3(0f, 2.3f, 3f), 0.05f, Color.white);
        SceneBuilderUtil.AddLabel("Ramp Sign", "30 deg ramp\n(should be rejected)", new Vector3(-5f, 2.8f, 6f), 0.04f,
                                  new Color(1f, 0.8f, 0.6f));
        SceneBuilderUtil.AddLabel("Gap Sign", "0.7 m gap", new Vector3(3.7f, 2.4f, 5f), 0.04f, new Color(1f, 0.8f, 0.6f));

        SceneBuilderUtil.SaveScene(scene, "Activity_5_1");
        Selection.activeGameObject = rig;
    }

    // ------------------------------------------------------------------------------------------------------------
    // Local helpers (kept here rather than in common/ because only the locomotion activities need them)
    // ------------------------------------------------------------------------------------------------------------

    /// <summary>Depth-first search for a child (any depth) with the given name.</summary>
    static Transform FindChildByName(Transform root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    /// <summary>
    /// A transparent, unlit material that draws late in the transparent queue.
    /// needVertexColor = true always uses Sprites/Default (it multiplies by vertex color, which LineRenderer needs).
    /// Otherwise uses URP/Unlit set to Transparent when a render pipeline is active, else Sprites/Default.
    /// </summary>
    static Material MakeOverlayMaterial(string name, Color color, bool needVertexColor)
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
        if (!needVertexColor && GraphicsSettings.currentRenderPipeline != null)
            shader = Shader.Find("Universal Render Pipeline/Unlit");
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
        mat.renderQueue = (int)RenderQueue.Transparent + 50;
        mat.color = color;
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }
}

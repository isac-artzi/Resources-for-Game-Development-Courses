// ActivitySceneBuilder.cs — Activity 7.1: Portals and Scene Flow
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a Hub scene (Activity_7_1) with three portals, plus three
// small destination scenes (Forest_Stub, Mountain_Stub, Ruins_Stub), each with its own XR rig, a fade quad
// on the camera, a return portal, and one artifact piece. All four scenes are added to the Build Settings.
// Created by Isac Artzi

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using ActivityTools;

public static class ActivitySceneBuilder
{
    const string HubScene      = "Activity_7_1";
    const string ForestScene   = "Forest_Stub";
    const string MountainScene = "Mountain_Stub";
    const string RuinsScene    = "Ruins_Stub";

    static readonly Color ForestColor   = new Color(0.30f, 0.85f, 0.45f);
    static readonly Color MountainColor = new Color(0.70f, 0.85f, 1.00f);
    static readonly Color RuinsColor    = new Color(1.00f, 0.75f, 0.35f);

    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        // The Hub is built first so it lands at index 0 of the Scene List (the scene a build starts in).
        string hubPath = BuildHub();

        BuildStub(ForestScene,   "Enchanted Forest", ForestColor,   new Color(0.20f, 0.40f, 0.18f), new Color(0.55f, 0.70f, 0.60f), "forest_piece");
        BuildStub(MountainScene, "Mountain Pass",    MountainColor, new Color(0.75f, 0.78f, 0.82f), new Color(0.80f, 0.85f, 0.95f), "mountain_piece");
        BuildStub(RuinsScene,    "Ancient Ruins",    RuinsColor,    new Color(0.60f, 0.52f, 0.40f), new Color(0.75f, 0.65f, 0.50f), "ruins_piece");

        // Come back to the Hub so the student presses Play in the right place.
        EditorSceneManager.OpenScene(hubPath);
        Selection.activeGameObject = GameObject.Find("XR Origin (XR Rig)");
        Debug.Log("[Activity] Built Hub + 3 stub scenes. Press Play in " + HubScene + " and walk into a portal.");
    }

    // ------------------------------------------------------------------
    // Hub
    // ------------------------------------------------------------------
    static string BuildHub()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(0.95f, 0.92f, 1.00f), 0.9f, new Vector3(50f, -20f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.35f, 0.35f, 0.45f), 0.03f);

        var floorMat = SceneBuilderUtil.MakeMaterial("HubStone", new Color(0.40f, 0.40f, 0.46f));
        var pillarMat = SceneBuilderUtil.MakeMaterial("HubPillar", new Color(0.30f, 0.30f, 0.36f));
        SceneBuilderUtil.AddFloor(24f, floorMat);

        var rig = AddRigWithFadeAndProbe(new Vector3(0f, 0f, -2f));

        // Persistent systems: GameManager (+ SceneLoader via RequireComponent). The Hub is the only scene that
        // contains one; stubs fall back to GameManager.Instance if played directly.
        var systems = SceneBuilderUtil.AddEmpty("Game Systems", Vector3.zero);
        var gm = systems.AddComponent<GameManager>();
        gm.totalPieces = 3;
        var loader = systems.GetComponent<SceneLoader>();
        if (loader == null) loader = systems.AddComponent<SceneLoader>();
        loader.fadeSeconds = 0.4f;
        var fadeGo = GameObject.Find("Fade Quad");
        if (fadeGo != null) loader.fadeRenderer = fadeGo.GetComponent<Renderer>();

        // Three portals in an arc ahead of the player.
        AddPortal("Portal Forest",   new Vector3(-4.5f, 0f, 5f),  35f,  ForestScene,   "Enchanted Forest", ForestColor,   pillarMat);
        AddPortal("Portal Mountain", new Vector3( 0.0f, 0f, 6.5f), 0f,  MountainScene, "Mountain Pass",    MountainColor, pillarMat);
        AddPortal("Portal Ruins",    new Vector3( 4.5f, 0f, 5f), -35f,  RuinsScene,    "Ancient Ruins",    RuinsColor,    pillarMat);

        // Status sign — GameManager rewrites this after every scene load.
        var sign = SceneBuilderUtil.AddLabel("Status Sign", "Artifact pieces: 0 / 3\n(implement GameManager TODO 2-4)",
                                             new Vector3(0f, 2.6f, 3.5f), 0.05f, new Color(1f, 0.95f, 0.6f));
        sign.transform.rotation = Quaternion.identity;

        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "The Hub of Elaria\nActivity 7.1 - Portals and Scene Flow\nWalk into a portal to travel",
                                  new Vector3(0f, 1.6f, 2.2f), 0.035f, Color.white);

        string path = SceneBuilderUtil.SaveScene(scene, HubScene);
        Selection.activeGameObject = rig;
        return path;
    }

    // ------------------------------------------------------------------
    // One destination stub
    // ------------------------------------------------------------------
    static void BuildStub(string sceneName, string worldName, Color accent, Color floorColor, Color fogColor, string pieceId)
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(Color.white, 1.0f, new Vector3(45f, 30f, 0f));
        SceneBuilderUtil.SetFog(fogColor, 0.02f);

        var floorMat = SceneBuilderUtil.MakeMaterial(sceneName + "_Floor", floorColor);
        var propMat  = SceneBuilderUtil.MakeMaterial(sceneName + "_Prop", floorColor * 0.7f);
        var pieceMat = SceneBuilderUtil.MakeEmissiveMaterial("ArtifactPiece", new Color(0.9f, 0.6f, 1.0f), 2.5f);
        SceneBuilderUtil.AddFloor(20f, floorMat);

        AddRigWithFadeAndProbe(Vector3.zero);

        // A few props so each world reads differently at a glance (all primitives, all static).
        var props = SceneBuilderUtil.AddEmpty("Props", Vector3.zero);
        for (int i = 0; i < 8; i++)
        {
            float a = i * Mathf.PI * 2f / 8f;
            var p = new Vector3(Mathf.Cos(a) * 7f, 0f, Mathf.Sin(a) * 7f);
            if (sceneName == ForestScene)
            {
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + i, p + Vector3.up * 2f, new Vector3(0.5f, 2f, 0.5f), propMat, props.transform);
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + i, p + Vector3.up * 5f, Vector3.one * 3f, floorMat, props.transform);
            }
            else if (sceneName == MountainScene)
            {
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Peak " + i, p + Vector3.up * 2f, new Vector3(2.5f, 4f + (i % 3), 2.5f), propMat, props.transform)
                                .transform.rotation = Quaternion.Euler(0f, i * 23f, 0f);
            }
            else
            {
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Column " + i, p + Vector3.up * 1.5f, new Vector3(0.6f, 1.5f, 0.6f), propMat, props.transform);
            }
        }
        SceneBuilderUtil.MarkStaticRecursive(props);

        // The artifact piece: an emissive cube on a pedestal, with a trigger you walk into.
        var pedestal = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Pedestal", new Vector3(0f, 0.5f, 3f), new Vector3(0.5f, 0.5f, 0.5f), propMat);
        pedestal.isStatic = true;
        var piece = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Artifact Piece", new Vector3(0f, 1.4f, 3f), new Vector3(0.25f, 0.4f, 0.25f), pieceMat);
        piece.transform.rotation = Quaternion.Euler(45f, 0f, 45f);
        var pieceCol = piece.GetComponent<BoxCollider>();
        pieceCol.isTrigger = true;
        pieceCol.size = new Vector3(3f, 4f, 3f);   // generous: you "walk through" the glow to take it
        var ap = piece.AddComponent<ArtifactPiece>();
        ap.pieceId = pieceId;
        ap.degreesPerSecond = 60f;
        var glow = SceneBuilderUtil.AddPointLight("Piece Glow", new Vector3(0f, 1.8f, 3f), new Color(0.9f, 0.6f, 1f), 4f, 1.5f, piece.transform);
        ap.glow = glow.GetComponent<Light>();

        // Return portal, off to the left so you do not fall straight back into it.
        AddPortal("Return Portal", new Vector3(-3.5f, 0f, 1.5f), -90f, HubScene, "The Hub", new Color(0.8f, 0.8f, 1f), propMat);

        SceneBuilderUtil.AddLabel("Status Sign", "Artifact pieces: ? / 3", new Vector3(0f, 2.6f, 4.5f), 0.05f, new Color(1f, 0.95f, 0.6f));
        SceneBuilderUtil.AddLabel("World Sign", worldName + " (stub)\nWalk through the glowing piece.\nThe Return Portal is to your left.",
                                  new Vector3(0f, 1.9f, 5f), 0.045f, accent);

        SceneBuilderUtil.SaveScene(scene, sceneName);
    }

    // ------------------------------------------------------------------
    // Shared pieces
    // ------------------------------------------------------------------

    /// <summary>XR rig + a "Head Probe" trigger sphere on the camera (so portals can detect the head) +
    /// a black "Fade Quad" 0.5 m in front of the camera for the SceneLoader to darken.</summary>
    static GameObject AddRigWithFadeAndProbe(Vector3 position)
    {
        var rig = SceneBuilderUtil.AddXrRig(position);
        var cam = rig.GetComponentInChildren<Camera>(true);
        if (cam == null) return rig;

        var probe = SceneBuilderUtil.AddEmpty("Head Probe", cam.transform.position, cam.transform);
        probe.layer = 2; // Ignore Raycast, so the XR ray interactors never hit the player's own head
        var sphere = probe.AddComponent<SphereCollider>();
        sphere.radius = 0.25f;
        sphere.isTrigger = true;
        var rb = probe.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "Fade Quad";
        quad.layer = 2;
        Object.DestroyImmediate(quad.GetComponent<Collider>());
        quad.transform.SetParent(cam.transform, false);
        quad.transform.localPosition = new Vector3(0f, 0f, 0.5f);
        quad.transform.localRotation = Quaternion.identity;
        quad.transform.localScale = new Vector3(4f, 4f, 1f);
        var mr = quad.GetComponent<MeshRenderer>();
        mr.sharedMaterial = MakeFadeMaterial();
        mr.shadowCastingMode = ShadowCastingMode.Off;
        mr.receiveShadows = false;
        return rig;
    }

    /// <summary>An unlit, alpha-blended black material that draws on top of everything (render queue 4000).</summary>
    static Material MakeFadeMaterial()
    {
        SceneBuilderUtil.EnsureFolder(SceneBuilderUtil.MaterialsFolder);
        string path = SceneBuilderUtil.MaterialsFolder + "/Fade.mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;

        var shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Unlit/Transparent");
        var mat = new Material(shader);
        mat.color = new Color(0f, 0f, 0f, 0f);
        mat.renderQueue = 4000;
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    /// <summary>A doorway: two pillars, a lintel, a glowing surface, and the trigger volume carrying Portal.</summary>
    static GameObject AddPortal(string name, Vector3 basePos, float yaw, string targetScene, string displayName,
                                Color accent, Material pillarMat)
    {
        var root = SceneBuilderUtil.AddEmpty(name, basePos);
        root.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        var t = root.transform;

        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Pillar L", Vector3.zero, new Vector3(0.3f, 2.6f, 0.3f), pillarMat, t)
                        .transform.localPosition = new Vector3(-1.0f, 1.3f, 0f);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Pillar R", Vector3.zero, new Vector3(0.3f, 2.6f, 0.3f), pillarMat, t)
                        .transform.localPosition = new Vector3(1.0f, 1.3f, 0f);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Lintel", Vector3.zero, new Vector3(2.3f, 0.3f, 0.3f), pillarMat, t)
                        .transform.localPosition = new Vector3(0f, 2.75f, 0f);

        var surfMat = SceneBuilderUtil.MakeEmissiveMaterial(name.Replace(' ', '_') + "_Surface", accent, 1.5f);
        var surf = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Surface", Vector3.zero, new Vector3(1.7f, 2.3f, 0.05f), surfMat, t);
        surf.transform.localPosition = new Vector3(0f, 1.3f, 0f);
        Object.DestroyImmediate(surf.GetComponent<Collider>());   // the trigger below does the sensing

        var trigger = SceneBuilderUtil.AddTriggerVolume("Trigger", Vector3.zero, new Vector3(1.7f, 2.6f, 1.2f), t);
        trigger.transform.localPosition = new Vector3(0f, 1.3f, 0f);
        var portal = trigger.AddComponent<Portal>();
        portal.targetScene = targetScene;
        portal.displayName = displayName;
        portal.surface = surf.GetComponent<Renderer>();

        SceneBuilderUtil.AddPointLight("Glow", Vector3.zero, accent, 4f, 1.2f, t).transform.localPosition = new Vector3(0f, 1.5f, 0.6f);
        var label = SceneBuilderUtil.AddLabel("Label", displayName, Vector3.zero, 0.05f, accent, t);
        label.transform.localPosition = new Vector3(0f, 3.1f, 0f);
        label.transform.localRotation = Quaternion.identity;
        return root;
    }
}

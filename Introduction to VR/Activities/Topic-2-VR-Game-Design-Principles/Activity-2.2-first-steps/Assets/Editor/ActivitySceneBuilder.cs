// ActivitySceneBuilder.cs — Activity 2.2: First Steps: Teleport and Move
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: the trail toward the Mountain Pass with a teleportable floor,
// four waystone teleport anchors, rock walls for optic flow, a comfort-vignette quad on the camera, and
// the locomotion mode switch wired to the Starter Assets rig's providers.
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using ActivityTools;

public static class ActivitySceneBuilder
{
    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(0.95f, 0.95f, 1.0f), 1.1f, new Vector3(35f, 20f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.62f, 0.68f, 0.78f), 0.012f);

        // Materials -------------------------------------------------------
        var gravel   = SceneBuilderUtil.MakeMaterial("Gravel",   new Color(0.42f, 0.40f, 0.36f));
        var rock     = SceneBuilderUtil.MakeMaterial("Rock",     new Color(0.36f, 0.34f, 0.36f));
        var snowRock = SceneBuilderUtil.MakeMaterial("SnowRock", new Color(0.70f, 0.72f, 0.78f));
        var disc     = SceneBuilderUtil.MakeEmissiveMaterial("WaystoneDisc", new Color(0.35f, 0.85f, 1.0f), 1.2f);
        var rune     = SceneBuilderUtil.MakeEmissiveMaterial("WaystoneRune", new Color(0.55f, 0.75f, 1.0f), 0.8f);
        var guideMat = SceneBuilderUtil.MakeEmissiveMaterial("Guide", new Color(0.90f, 0.85f, 1.00f), 1.2f);

        // The vignette material: an unlit, transparent sprite shader whose color alpha we animate from script.
        var vignetteMat = SceneBuilderUtil.MakeMaterial("Vignette", new Color(0f, 0f, 0f, 0f));
        var spriteShader = Shader.Find("Sprites/Default");
        if (spriteShader != null) vignetteMat.shader = spriteShader;
        vignetteMat.color = new Color(0f, 0f, 0f, 0f);
        EditorUtility.SetDirty(vignetteMat);

        // Floor (teleportable), player, interaction manager ------------------------------------------
        var floor = SceneBuilderUtil.AddFloor(50f, gravel);
        var area = floor.AddComponent<TeleportationArea>();
        area.matchOrientation = MatchOrientation.WorldSpaceUp;   // keep your facing when you land anywhere on the floor
        UseTeleportLayer(area);

        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);
        if (Object.FindFirstObjectByType<XRInteractionManager>() == null)
            new GameObject("XR Interaction Manager", typeof(XRInteractionManager));

        // The Starter Assets rig maps the LEFT thumbstick to teleport by default. Flip its Controller Input Action
        // Manager to smooth motion so the left stick drives the ContinuousMoveProvider; the right stick keeps
        // teleport (push forward) and snap turn (push sideways). Done through SerializedObject so it does not
        // require a compile-time reference to the sample script.
        EnableSmoothMotion(rig, "Left");

        // Rock walls on both sides of the trail: strong optic flow when you move smoothly -----------------
        var walls = SceneBuilderUtil.AddEmpty("Rock Walls", Vector3.zero);
        for (int i = 0; i < 9; i++)
        {
            float z = -3f + i * 3.2f;
            for (int side = -1; side <= 1; side += 2)
            {
                float h = 2.5f + Mathf.Abs(Mathf.Sin(i * 1.3f + side)) * 3f;
                float x = side * (6.5f + Mathf.Cos(i * 0.9f) * 1.2f);
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Rock " + i + (side < 0 ? "L" : "R"),
                                              new Vector3(x, h * 0.5f, z), new Vector3(2.5f, h, 3f),
                                              (i % 3 == 0) ? snowRock : rock, walls.transform);
            }
        }
        SceneBuilderUtil.MarkStaticRecursive(walls);

        // Waystones: teleport anchors along a switchback trail --------------------------------------------
        Vector3[] spots =
        {
            new Vector3( 0.0f, 0f,  5.0f),
            new Vector3( 3.5f, 0f,  9.5f),
            new Vector3(-2.5f, 0f, 14.0f),
            new Vector3( 0.0f, 0f, 19.0f),
        };
        var waystonesRoot = SceneBuilderUtil.AddEmpty("Waystones", Vector3.zero);
        var anchors = new TeleportationAnchor[spots.Length];
        for (int i = 0; i < spots.Length; i++)
        {
            var group = SceneBuilderUtil.AddEmpty("Waystone " + (i + 1), spots[i], waystonesRoot.transform);
            // Face the next waystone (the last one faces the Guide). TargetUpAndForward uses this rotation on arrival.
            Vector3 lookTarget = (i + 1 < spots.Length) ? spots[i + 1] : new Vector3(0f, 0f, 21f);
            Vector3 fwd = lookTarget - spots[i]; fwd.y = 0f;
            if (fwd.sqrMagnitude > 0.001f) group.transform.rotation = Quaternion.LookRotation(fwd);

            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Disc", spots[i] + Vector3.up * 0.02f,
                                          new Vector3(0.9f, 0.02f, 0.9f), disc, group.transform);
            var stone = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Standing Stone", Vector3.zero,
                                                      new Vector3(0.35f, 1.3f, 0.25f), rune, group.transform);
            stone.transform.localPosition = new Vector3(0.75f, 0.65f, 0f);
            var label = SceneBuilderUtil.AddLabel("Label", "Waystone " + (i + 1), Vector3.zero, 0.05f,
                                                  new Color(0.8f, 0.95f, 1f), group.transform);
            label.transform.localPosition = new Vector3(0.75f, 1.6f, 0f);
            label.transform.localRotation = Quaternion.identity; // a TextMesh reads from its -z side: the trail behind it

            var anchor = group.AddComponent<TeleportationAnchor>();      // colliders are collected from the children
            anchor.matchOrientation = MatchOrientation.TargetUpAndForward;
            UseTeleportLayer(anchor);
            anchors[i] = anchor;
        }

        // The Guide waits at the top of the trail ------------------------------------------------------
        var guide = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Mysterious Guide",
                                                  new Vector3(0f, 1f, 21f), new Vector3(0.6f, 1f, 0.6f), guideMat);
        SceneBuilderUtil.AddPointLight("Guide Glow", new Vector3(0f, 1.6f, 20.5f), new Color(0.8f, 0.7f, 1f), 6f, 1.8f,
                                       guide.transform);
        SceneBuilderUtil.AddLabel("Guide Label", "The pass is long. Choose how you travel.",
                                  new Vector3(0f, 2.4f, 21f), 0.06f, new Color(0.95f, 0.9f, 1f));

        // Comfort vignette: a quad glued to the camera ---------------------------------------------------
        var cam = rig.GetComponentInChildren<Camera>(true);
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "Comfort Vignette";
        Object.DestroyImmediate(quad.GetComponent<Collider>());           // rays must never hit it
        quad.layer = LayerMask.NameToLayer("Ignore Raycast");
        quad.GetComponent<Renderer>().sharedMaterial = vignetteMat;
        quad.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        if (cam != null) quad.transform.SetParent(cam.transform, false);
        quad.transform.localPosition = new Vector3(0f, 0f, 0.35f);
        quad.transform.localRotation = Quaternion.identity;
        quad.transform.localScale = new Vector3(1.2f, 1.2f, 1f);         // ~120 degrees wide at 0.35 m
        var vignette = quad.AddComponent<ComfortVignette>();
        vignette.rig = rig.transform;

        // Locomotion mode switch + labels --------------------------------------------------------------
        var modeLabel = SceneBuilderUtil.AddLabel("Mode Label", "Locomotion: (implement LocomotionModeSwitch)  [M]",
                                                  new Vector3(0f, 2.1f, 3.0f), 0.045f, new Color(1f, 0.95f, 0.5f));
        var controls = SceneBuilderUtil.AddEmpty("Locomotion Controls", Vector3.zero);
        var modeSwitch = controls.AddComponent<LocomotionModeSwitch>();
        modeSwitch.rig = rig;
        modeSwitch.label = modeLabel.GetComponent<TextMesh>();
        modeSwitch.teleportInteractorObjects = FindTeleportInteractors(rig);

        var countLabel = SceneBuilderUtil.AddLabel("Teleport Count", "Teleports: 0",
                                                   new Vector3(0f, 1.75f, 3.0f), 0.04f, new Color(0.8f, 0.95f, 1f));
        var randomizer = waystonesRoot.AddComponent<TeleportSpotRandomizer>();
        randomizer.anchors = anchors;
        randomizer.rig = rig.transform;
        randomizer.center = new Vector3(0f, 0f, 11f);
        randomizer.radius = 7f;
        randomizer.label = countLabel.GetComponent<TextMesh>();

        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 2.2 - First Steps\nLeft stick: walk. Right stick forward: teleport arc.\nM cycles Teleport / Smooth / Hybrid.",
                                  new Vector3(0f, 2.7f, 3.0f), 0.045f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_2_2");
        Selection.activeGameObject = rig;
    }

    /// <summary>Puts a teleport interactable on the Starter Assets "Teleport" interaction layer if that layer exists,
    /// so the grab ray ignores it and only the teleport ray can select it.</summary>
    static void UseTeleportLayer(XRBaseInteractable interactable)
    {
        int layer = InteractionLayerMask.NameToLayer("Teleport");
        if (layer >= 0) interactable.interactionLayers = 1 << layer;
        else Debug.LogWarning("[Activity] Interaction layer 'Teleport' not found; leaving default layers on " + interactable.name);
    }

    /// <summary>Collects the rig's "Teleport Interactor" GameObjects so the mode switch can hide the teleport ray.</summary>
    static GameObject[] FindTeleportInteractors(GameObject rig)
    {
        var list = new System.Collections.Generic.List<GameObject>();
        foreach (var t in rig.GetComponentsInChildren<Transform>(true))
            if (t.name.Contains("Teleport Interactor")) list.Add(t.gameObject);
        return list.ToArray();
    }

    /// <summary>Sets Smooth Motion Enabled on the ControllerInputActionManager of the named hand (Starter Assets sample).</summary>
    static void EnableSmoothMotion(GameObject rig, string handNameContains)
    {
        bool found = false;
        foreach (var mb in rig.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (mb == null || mb.GetType().Name != "ControllerInputActionManager") continue;
            if (!mb.gameObject.name.Contains(handNameContains)) continue;
            var so = new SerializedObject(mb);
            var prop = so.FindProperty("m_SmoothMotionEnabled");
            if (prop == null) continue;
            prop.boolValue = true;
            so.ApplyModifiedProperties();
            found = true;
            Debug.Log("[Activity] Smooth motion enabled on " + mb.gameObject.name + " (left stick now walks).");
        }
        if (!found)
            Debug.LogWarning("[Activity] Could not find ControllerInputActionManager on the " + handNameContains +
                             " controller. Select it in the Hierarchy and tick 'Smooth Motion Enabled' by hand.");
    }
}

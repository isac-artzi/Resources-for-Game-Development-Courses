// ActivitySceneBuilder.cs — Activity 6.4: Dark Mode: Navigate by Sound
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a walled ruins courtyard with inner walls, an audio-beacon objective
// (Artifact Shard) behind them, two buzzing hazards (Void Pits), the Sonar (ping + echoes), the Dark Mode controller
// with its feedback source, and small glowing hand markers on the rig's controllers that stay visible in the dark.
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
        SceneBuilderUtil.SetSun(new Color(0.85f, 0.80f, 0.95f), 0.7f, new Vector3(40f, 160f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.30f, 0.28f, 0.35f), 0.02f);

        // Materials -------------------------------------------------------
        var flagstone = SceneBuilderUtil.MakeMaterial("Flagstone", new Color(0.40f, 0.38f, 0.36f));
        var ruinStone = SceneBuilderUtil.MakeMaterial("Ruin Stone", new Color(0.55f, 0.50f, 0.42f));
        var shardMat  = SceneBuilderUtil.MakeEmissiveMaterial("Shard", new Color(0.40f, 0.80f, 1.00f), 2f);
        var voidMat   = SceneBuilderUtil.MakeEmissiveMaterial("Void", new Color(0.35f, 0.05f, 0.10f), 1.2f);
        var handMat   = SceneBuilderUtil.MakeEmissiveMaterial("Hand Marker", new Color(0.9f, 0.9f, 1.0f), 3f);

        // Courtyard: 20 x 20 m floor with a boundary wall and a few inner walls to hide behind ---------------
        SceneBuilderUtil.AddFloor(20f, flagstone);
        var walls = SceneBuilderUtil.AddEmpty("Walls", Vector3.zero);
        Wall(walls, "North Wall", new Vector3(0f, 1.5f, 10f),  new Vector3(20f, 3f, 0.4f), ruinStone);
        Wall(walls, "South Wall", new Vector3(0f, 1.5f, -10f), new Vector3(20f, 3f, 0.4f), ruinStone);
        Wall(walls, "East Wall",  new Vector3(10f, 1.5f, 0f),  new Vector3(0.4f, 3f, 20f), ruinStone);
        Wall(walls, "West Wall",  new Vector3(-10f, 1.5f, 0f), new Vector3(0.4f, 3f, 20f), ruinStone);
        // Inner walls: a broken screen between the start and the shard
        Wall(walls, "Screen A", new Vector3(-2.5f, 1.25f, 3f), new Vector3(5f, 2.5f, 0.4f), ruinStone);
        Wall(walls, "Screen B", new Vector3(4.5f, 1.25f, 3f),  new Vector3(4f, 2.5f, 0.4f), ruinStone);
        Wall(walls, "Screen C", new Vector3(1.5f, 1.25f, 6.5f), new Vector3(0.4f, 2.5f, 4f), ruinStone);
        Wall(walls, "Pillar 1", new Vector3(-6f, 1.5f, 0f),   new Vector3(0.8f, 3f, 0.8f), ruinStone);
        Wall(walls, "Pillar 2", new Vector3(6.5f, 1.5f, -3f), new Vector3(0.8f, 3f, 0.8f), ruinStone);
        SceneBuilderUtil.MarkStaticRecursive(walls);

        var rig = SceneBuilderUtil.AddXrRig(new Vector3(0f, 0f, -6f));

        // Objective: the Artifact Shard, behind Screen C -------------------
        var shardPos = new Vector3(5.5f, 1.1f, 7.5f);
        var shard = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Artifact Shard", shardPos, new Vector3(0.15f, 0.30f, 0.15f), shardMat);
        shard.transform.rotation = Quaternion.Euler(45f, 0f, 45f);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Shard Pedestal", new Vector3(5.5f, 0.4f, 7.5f), new Vector3(0.4f, 0.4f, 0.4f), ruinStone);
        SceneBuilderUtil.AddPointLight("Shard Glow", shardPos + Vector3.up * 0.3f, new Color(0.4f, 0.8f, 1f), 4f, 2f, shard.transform);
        AddBeacon(shard, 440f, 1.0f, 30f, 0.7f, rig.transform);

        // Hazards: two Void Pits, flat dark discs with a low buzz ---------
        var hazardsRoot = SceneBuilderUtil.AddEmpty("Hazards", Vector3.zero);
        var pit1 = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Void Pit 1", new Vector3(-3f, 0.03f, -1f), new Vector3(1.6f, 0.03f, 1.6f), voidMat, hazardsRoot.transform);
        var pit2 = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Void Pit 2", new Vector3(3f, 0.03f, 0.5f), new Vector3(1.6f, 0.03f, 1.6f), voidMat, hazardsRoot.transform);
        foreach (var pit in new GameObject[] { pit1, pit2 })
        {
            var col = pit.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;    // do not trip the player; the controller judges by distance
            AddBeacon(pit, 90f, 0.8f, 10f, 0.5f, rig.transform);
        }

        // Sonar --------------------------------------------------------------
        var sonarGo = SceneBuilderUtil.AddEmpty("Sonar", Vector3.zero);
        var sonar = sonarGo.AddComponent<SonarPing>();
        sonar.scanRadius = 12f;
        sonar.maxEchoes = 6;
        sonar.timeStretch = 20f;
        sonar.ignoreRoot = rig.transform;
        var sonarLabel = SceneBuilderUtil.AddLabel("Sonar Readout", "Sonar: implement TODO 1-4 in SonarPing.cs (P to ping)",
                                                   new Vector3(-2f, 2.0f, -3.5f), 0.035f, new Color(0.6f, 0.95f, 1f));
        sonar.readout = sonarLabel.GetComponent<TextMesh>();

        // Dark Mode controller --------------------------------------------------
        var darkGo = SceneBuilderUtil.AddEmpty("Dark Mode", Vector3.zero);
        var feedback = darkGo.AddComponent<AudioSource>();
        feedback.playOnAwake = false;
        feedback.spatialBlend = 0f;
        var dark = darkGo.AddComponent<DarkModeController>();
        dark.keepVisibleRoot = rig.transform;
        dark.objective = shard.transform;
        dark.hazards = new Transform[] { pit1.transform, pit2.transform };
        dark.feedback = feedback;
        dark.objectiveRadius = 0.9f;
        dark.hazardRadius = 0.8f;
        var darkLabel = SceneBuilderUtil.AddLabel("Dark Mode Readout", "Lights on\nK toggles dark mode (TODO 1-4 in DarkModeController.cs)",
                                                  new Vector3(2f, 2.0f, -3.5f), 0.035f, new Color(1f, 0.95f, 0.6f));
        dark.readout = darkLabel.GetComponent<TextMesh>();

        // Hand markers: tiny glowing spheres on the controllers, so you can still see your hands in the dark ------
        AddHandMarker(rig.transform, "Left Controller", handMat);
        AddHandMarker(rig.transform, "Right Controller", handMat);

        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 6.4 - Dark Mode\nThe shard hums behind the walls; the pits buzz. Press K for darkness, P to ping.\nReach the shard by ear.",
                                  new Vector3(0f, 2.6f, -2f), 0.045f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_6_4");
        Selection.activeGameObject = rig;
    }

    static void Wall(GameObject parent, string name, Vector3 center, Vector3 size, Material mat)
    {
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, name, center, size, mat, parent.transform);
    }

    /// <summary>A looping 3D beacon with a low-pass filter and OcclusionFilter, no clip (placeholder tone at runtime).</summary>
    static void AddBeacon(GameObject go, float placeholderHz, float minDistance, float maxDistance, float volume, Transform rig)
    {
        var src = go.AddComponent<AudioSource>();
        src.clip = null;
        src.playOnAwake = true;
        src.loop = true;
        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Logarithmic;
        src.minDistance = minDistance;
        src.maxDistance = maxDistance;
        src.volume = volume;

        var lowPass = go.AddComponent<AudioLowPassFilter>();
        lowPass.cutoffFrequency = 22000f;   // Unity's default of 5000 Hz would muffle the beacon even in the open

        var occ = go.AddComponent<OcclusionFilter>();
        occ.placeholderHz = placeholderHz;
        occ.ignoreRoot = rig;
        occ.openCutoffHz = 22000f;
        occ.occludedCutoffHz = 600f;
    }

    /// <summary>Finds a controller object by name anywhere under the rig and parents a small emissive sphere to it.</summary>
    static void AddHandMarker(Transform rig, string controllerName, Material mat)
    {
        Transform controller = FindDeep(rig, controllerName);
        if (controller == null)
        {
            Debug.LogWarning("[Activity] Could not find '" + controllerName + "' under the rig; no hand marker added. " +
                             "The controller models themselves stay visible in dark mode anyway.");
            return;
        }
        var marker = SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, controllerName + " Marker", controller.position,
                                                   Vector3.one * 0.05f, mat, controller);
        var col = marker.GetComponent<Collider>();
        if (col != null) Object.DestroyImmediate(col);   // a collider on the hand would block grabs and echo scans
    }

    static Transform FindDeep(Transform root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }
}

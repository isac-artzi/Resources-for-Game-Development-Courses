// ActivitySceneBuilder.cs — Activity 4.3: The Runestone Puzzle
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a ruined stone chamber with an altar carrying three XR sockets,
// three grabbable runestones on a side table, three glyph cubes on the wall that show the target order,
// and a PuzzleController + PuzzleFeedback pair wired to all of them.
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using ActivityTools;

public static class ActivitySceneBuilder
{
    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(0.85f, 0.85f, 1.0f), 0.35f, new Vector3(60f, 20f, 0f));   // dim: a torch-lit ruin
        SceneBuilderUtil.SetFog(new Color(0.12f, 0.12f, 0.15f), 0.04f);

        // Materials -------------------------------------------------------
        var flagstone = SceneBuilderUtil.MakeMaterial("Flagstone", new Color(0.35f, 0.34f, 0.36f));
        var wallStone = SceneBuilderUtil.MakeMaterial("WallStone", new Color(0.42f, 0.40f, 0.38f));
        var altarMat  = SceneBuilderUtil.MakeMaterial("Altar", new Color(0.28f, 0.27f, 0.32f));
        var wood      = SceneBuilderUtil.MakeMaterial("OldWood", new Color(0.36f, 0.26f, 0.16f));
        var runeMat   = SceneBuilderUtil.MakeEmissiveMaterial("Runestone", Color.white, 1.0f);   // Runestone.ApplyColor tints per stone
        var glyphMat  = SceneBuilderUtil.MakeEmissiveMaterial("Glyph", Color.white, 1.5f);       // PuzzleController.ShowTarget tints per glyph
        var ringMat   = SceneBuilderUtil.MakeEmissiveMaterial("SocketRing", new Color(0.6f, 0.7f, 1.0f), 0.8f);
        var torchMat  = SceneBuilderUtil.MakeEmissiveMaterial("Torch", new Color(1.0f, 0.65f, 0.3f), 2.5f);

        // Chamber: floor, four walls, pillars ---------------------------------
        SceneBuilderUtil.AddFloor(12f, flagstone);
        var rig = SceneBuilderUtil.AddXrRig(new Vector3(0f, 0f, -1.5f));

        var chamber = SceneBuilderUtil.AddEmpty("Chamber", Vector3.zero);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall Back",  new Vector3(0f, 2f, 4.5f),  new Vector3(9f, 4f, 0.4f), wallStone, chamber.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall Left",  new Vector3(-4.5f, 2f, 0f), new Vector3(0.4f, 4f, 9f), wallStone, chamber.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall Right", new Vector3(4.5f, 2f, 0f),  new Vector3(0.4f, 4f, 9f), wallStone, chamber.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall Front", new Vector3(0f, 2f, -4.5f), new Vector3(9f, 4f, 0.4f), wallStone, chamber.transform);
        foreach (var x in new[] { -3f, 3f })
            foreach (var z in new[] { -2.5f, 2.5f })
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Pillar", new Vector3(x, 2f, z), new Vector3(0.5f, 2f, 0.5f), wallStone, chamber.transform);
        SceneBuilderUtil.MarkStaticRecursive(chamber);

        // Torches on the side walls
        foreach (var x in new[] { -4.2f, 4.2f })
        {
            var torch = SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Torch", new Vector3(x, 2.2f, 1f), Vector3.one * 0.2f, torchMat);
            Object.DestroyImmediate(torch.GetComponent<Collider>());
            SceneBuilderUtil.AddPointLight("Torch Light", new Vector3(x * 0.9f, 2.2f, 1f), new Color(1f, 0.7f, 0.4f), 6f, 1.3f, torch.transform);
        }

        // The altar with three sockets ----------------------------------------
        var altar = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Altar", new Vector3(0f, 0.45f, 2.2f), new Vector3(1.4f, 0.9f, 0.6f), altarMat);
        altar.isStatic = true;
        var altarLightGo = SceneBuilderUtil.AddPointLight("Altar Light", new Vector3(0f, 1.8f, 2.2f), new Color(0.35f, 0.65f, 1f), 5f, 1.6f, altar.transform);
        var altarAudio = altar.AddComponent<AudioSource>();
        altarAudio.playOnAwake = false;
        altarAudio.spatialBlend = 1f;

        var controller = altar.AddComponent<PuzzleController>();
        var feedback = altar.AddComponent<PuzzleFeedback>();

        float[] socketX = { -0.4f, 0f, 0.4f };
        var sockets = new RuneSocket[socketX.Length];
        for (int i = 0; i < socketX.Length; i++)
        {
            var pos = new Vector3(socketX[i], 0.97f, 2.2f);
            var socketGo = SceneBuilderUtil.AddEmpty("Socket " + (i + 1), pos, altar.transform);

            var trigger = socketGo.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 0.12f;

            var socket = socketGo.AddComponent<XRSocketInteractor>();
            socket.showInteractableHoverMeshes = true;

            var rs = socketGo.AddComponent<RuneSocket>();
            rs.controller = controller;
            rs.socketIndex = i;
            sockets[i] = rs;

            // a flat glowing ring marks the socket (no collider: it must not block the stone)
            var ring = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Socket Ring", pos + Vector3.down * 0.06f,
                                                     new Vector3(0.22f, 0.01f, 0.22f), ringMat, socketGo.transform);
            Object.DestroyImmediate(ring.GetComponent<Collider>());
        }

        // Glyph cubes on the back wall (the clue) ------------------------------
        var glyphs = new Renderer[3];
        for (int i = 0; i < 3; i++)
        {
            var g = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Glyph " + (i + 1), new Vector3(-0.5f + 0.5f * i, 2.1f, 4.2f),
                                                  Vector3.one * 0.32f, glyphMat);
            g.transform.rotation = Quaternion.Euler(0f, 0f, 45f);
            glyphs[i] = g.GetComponent<Renderer>();
        }
        SceneBuilderUtil.AddLabel("Glyph Sign", "Set the stones in this order", new Vector3(0f, 2.6f, 4.2f), 0.04f, new Color(0.8f, 0.85f, 1f));

        // Side table with the three runestones ---------------------------------
        var table = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Side Table", new Vector3(-2.2f, 0.4f, 0.8f), new Vector3(1.2f, 0.8f, 0.6f), wood);
        table.isStatic = true;
        var stonesRoot = SceneBuilderUtil.AddEmpty("Runestones", Vector3.zero);
        var stones = new Runestone[3];
        stones[0] = MakeStone(stonesRoot.transform, runeMat, 0, "Tide",  new Color(0.3f, 0.8f, 1.0f), new Vector3(-2.55f, 0.9f, 0.8f));
        stones[1] = MakeStone(stonesRoot.transform, runeMat, 1, "Ember", new Color(1.0f, 0.65f, 0.2f), new Vector3(-2.2f, 0.9f, 0.8f));
        stones[2] = MakeStone(stonesRoot.transform, runeMat, 2, "Dusk",  new Color(0.7f, 0.4f, 1.0f), new Vector3(-1.85f, 0.9f, 0.8f));
        SceneBuilderUtil.AddLabel("Table Sign", "Runestones: Tide, Ember, Dusk", new Vector3(-2.2f, 1.4f, 1.0f), 0.035f, new Color(1f, 0.95f, 0.7f));

        // Wire the controller and feedback ----------------------------------------
        controller.targetSequence = new[] { 2, 0, 1 };     // Dusk, Tide, Ember
        controller.stones = stones;
        controller.sockets = sockets;
        controller.glyphs = glyphs;
        controller.feedback = feedback;

        feedback.altarLight = altarLightGo.GetComponent<Light>();
        feedback.glyphs = glyphs;
        feedback.audioSource = altarAudio;

        // Signs ----------------------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 4.3 - The Runestone Puzzle\nGrab a stone, drop it into a socket on the altar\nOpen README.md (Activity > Open README)",
                                  new Vector3(2.4f, 2.0f, 2.0f), 0.045f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_4_3");
        Selection.activeGameObject = rig;
    }

    static Runestone MakeStone(Transform parent, Material mat, int id, string runeName, Color color, Vector3 position)
    {
        var go = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Runestone " + runeName, position, new Vector3(0.12f, 0.16f, 0.12f), mat, parent);
        var rb = go.AddComponent<Rigidbody>();
        rb.mass = 0.8f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var grab = go.AddComponent<XRGrabInteractable>();
        grab.movementType = XRBaseInteractable.MovementType.Kinematic;
        grab.useDynamicAttach = true;
        grab.throwOnDetach = false;          // stones should not be thrown across the ruin

        var stone = go.AddComponent<Runestone>();
        stone.runeId = id;
        stone.runeName = runeName;
        stone.color = color;
        return stone;
    }
}

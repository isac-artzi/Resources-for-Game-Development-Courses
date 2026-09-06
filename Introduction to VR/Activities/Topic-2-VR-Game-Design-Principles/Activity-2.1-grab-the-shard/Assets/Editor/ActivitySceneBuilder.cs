// ActivitySceneBuilder.cs — Activity 2.1: Grab the Shard
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a forest-edge Altar of Binding with three shard sockets,
// three grabbable artifact shards on low pedestals, a decoy river stone, and a floating progress counter.
// Created by Isac Artzi

using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using ActivityTools;

public static class ActivitySceneBuilder
{
    // The socket filter in ShardSocketCheck compares against this tag. The builder registers it in the
    // project's Tag Manager so the scene works on a fresh project.
    const string ShardTag = "Shard";

    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        EnsureTag(ShardTag);

        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1f, 0.93f, 0.80f), 1.0f, new Vector3(40f, -25f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.50f, 0.66f, 0.58f), 0.018f);

        // Materials -------------------------------------------------------
        var grass    = SceneBuilderUtil.MakeMaterial("Grass",      new Color(0.25f, 0.50f, 0.20f));
        var bark     = SceneBuilderUtil.MakeMaterial("Bark",       new Color(0.35f, 0.25f, 0.15f));
        var leaves   = SceneBuilderUtil.MakeMaterial("Leaves",     new Color(0.15f, 0.40f, 0.18f));
        var stone    = SceneBuilderUtil.MakeMaterial("Stone",      new Color(0.55f, 0.55f, 0.58f));
        var darkStone = SceneBuilderUtil.MakeMaterial("DarkStone", new Color(0.30f, 0.30f, 0.34f));
        var riverStone = SceneBuilderUtil.MakeMaterial("RiverStone", new Color(0.45f, 0.42f, 0.40f));
        var shardIdle  = SceneBuilderUtil.MakeEmissiveMaterial("ShardIdle",  new Color(0.40f, 0.80f, 1.00f), 1.5f);
        var shardHover = SceneBuilderUtil.MakeEmissiveMaterial("ShardHover", new Color(1.00f, 0.90f, 0.35f), 2.5f);
        var shardHeld  = SceneBuilderUtil.MakeEmissiveMaterial("ShardHeld",  new Color(1.00f, 1.00f, 1.00f), 2.0f);
        var ringIdle   = SceneBuilderUtil.MakeEmissiveMaterial("RingIdle",   new Color(0.35f, 0.45f, 0.60f), 0.6f);
        var ringAccept = SceneBuilderUtil.MakeEmissiveMaterial("RingAccept", new Color(0.30f, 1.00f, 0.50f), 2.0f);
        var ringReject = SceneBuilderUtil.MakeEmissiveMaterial("RingReject", new Color(1.00f, 0.25f, 0.20f), 2.0f);
        var guideMat   = SceneBuilderUtil.MakeEmissiveMaterial("Guide",      new Color(0.90f, 0.85f, 1.00f), 1.2f);

        // Floor, player, interaction manager ---------------------------------
        SceneBuilderUtil.AddFloor(30f, grass);
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);
        if (UnityEngine.Object.FindFirstObjectByType<XRInteractionManager>() == null)
            new GameObject("XR Interaction Manager", typeof(XRInteractionManager));

        // Ring of trees for scale and enclosure -----------------------------
        var trees = SceneBuilderUtil.AddEmpty("Trees", Vector3.zero);
        const int treeCount = 10;
        for (int i = 0; i < treeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / treeCount + 0.3f;
            float radius = 9f + Mathf.Sin(i * 2.1f) * 1.5f;
            var basePos = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + (i + 1), basePos + Vector3.up * 2f,
                                          new Vector3(0.5f, 2f, 0.5f), bark, trees.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + (i + 1), basePos + Vector3.up * 5f,
                                          Vector3.one * 3f, leaves, trees.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(trees);

        // The Altar of Binding: a 1 m tall, 1.2 m wide stone drum 1.4 m ahead of the player -----------
        var altarRoot = SceneBuilderUtil.AddEmpty("Altar", new Vector3(0f, 0f, 1.4f));
        var altar = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Altar Drum", new Vector3(0f, 0.5f, 1.4f),
                                                  new Vector3(1.2f, 0.5f, 1.2f), darkStone, altarRoot.transform);
        altar.isStatic = true;

        // Three sockets on the altar top (y = 1.0). Each socket is an empty with a trigger sphere,
        // an XRSocketInteractor, and the ShardSocketCheck script; a flat ring below it shows its state.
        float[] socketX = { -0.32f, 0f, 0.32f };
        var socketChecks = new ShardSocketCheck[socketX.Length];
        for (int i = 0; i < socketX.Length; i++)
        {
            var center = new Vector3(socketX[i], 1.15f, 1.4f);

            var ring = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Socket Ring " + (i + 1),
                                                     new Vector3(socketX[i], 1.005f, 1.4f), new Vector3(0.24f, 0.005f, 0.24f),
                                                     ringIdle, altarRoot.transform);
            UnityEngine.Object.DestroyImmediate(ring.GetComponent<Collider>()); // visual only: rays must not hit it

            var socketGo = SceneBuilderUtil.AddEmpty("Shard Socket " + (i + 1), center, altarRoot.transform);
            var trigger = socketGo.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 0.12f;

            var socket = socketGo.AddComponent<XRSocketInteractor>();
            socket.showInteractableHoverMeshes = true;   // the translucent "ghost" preview when a shard is close
            socket.recycleDelayTime = 1.0f;             // after a release, wait 1 s before grabbing again

            var check = socketGo.AddComponent<ShardSocketCheck>();
            check.requiredTag = ShardTag;
            check.ringRenderer = ring.GetComponent<Renderer>();
            check.ringIdle = ringIdle;
            check.ringAccept = ringAccept;
            check.ringReject = ringReject;
            socketChecks[i] = check;
        }

        // Shards on low pedestals: two ahead-left/right, one behind you -------------------------------
        Vector3[] shardSpots =
        {
            new Vector3(-1.5f, 0f, 0.4f),
            new Vector3( 1.5f, 0f, 0.4f),
            new Vector3( 0.0f, 0f, -1.4f),
        };
        var shards = SceneBuilderUtil.AddEmpty("Shards", Vector3.zero);
        for (int i = 0; i < shardSpots.Length; i++)
        {
            var spot = shardSpots[i];
            var pedestal = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Pedestal " + (i + 1),
                                                         spot + Vector3.up * 0.4f, new Vector3(0.35f, 0.4f, 0.35f),
                                                         stone, shards.transform);
            pedestal.isStatic = true;

            var shard = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Shard " + (i + 1),
                                                      spot + Vector3.up * 0.98f, new Vector3(0.12f, 0.30f, 0.12f),
                                                      shardIdle, shards.transform);
            shard.tag = ShardTag;
            MakeGrabbable(shard, 0.4f);
            var hl = shard.AddComponent<GrabHighlighter>();
            hl.idleMaterial = shardIdle;
            hl.hoverMaterial = shardHover;
            hl.heldMaterial = shardHeld;

            SceneBuilderUtil.AddPointLight("Shard Light", spot + Vector3.up * 1.3f, new Color(0.4f, 0.8f, 1f),
                                           2.5f, 1.0f, pedestal.transform);
        }

        // Decoy: a river stone with no tag. The altar must refuse it. ---------------------------------
        var decoySpot = new Vector3(1.1f, 0f, -0.9f);
        var decoyPedestal = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Decoy Pedestal",
                                                          decoySpot + Vector3.up * 0.4f, new Vector3(0.35f, 0.4f, 0.35f),
                                                          stone, shards.transform);
        decoyPedestal.isStatic = true;
        var decoy = SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "River Stone",
                                                  decoySpot + Vector3.up * 0.9f, Vector3.one * 0.16f,
                                                  riverStone, shards.transform);
        MakeGrabbable(decoy, 0.8f);
        var decoyHl = decoy.AddComponent<GrabHighlighter>();
        decoyHl.idleMaterial = riverStone;
        decoyHl.hoverMaterial = shardHover;
        decoyHl.heldMaterial = shardHeld;

        // The Guide, dark until the altar is complete --------------------------------------------------
        var guide = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Mysterious Guide",
                                                  new Vector3(0f, 1f, 5.5f), new Vector3(0.6f, 1f, 0.6f), guideMat);
        var guideGlow = SceneBuilderUtil.AddPointLight("Guide Glow", new Vector3(0f, 1.6f, 5.0f),
                                                       new Color(0.8f, 0.7f, 1f), 6f, 2.0f, guide.transform);
        guideGlow.SetActive(false);   // GrabCounter turns this on when all three shards are seated
        SceneBuilderUtil.AddLabel("Guide Label", "Bind the three shards and I will speak",
                                  new Vector3(0f, 2.4f, 5.5f), 0.05f, new Color(0.95f, 0.9f, 1f));

        // Progress counter above the altar -----------------------------------------------------------
        var counterGo = SceneBuilderUtil.AddLabel("Shard Counter", "0/3 placed  (implement GrabCounter)",
                                                  new Vector3(0f, 1.75f, 1.4f), 0.045f, new Color(1f, 0.95f, 0.5f));
        var counter = counterGo.AddComponent<GrabCounter>();
        counter.sockets = socketChecks;
        counter.label = counterGo.GetComponent<TextMesh>();
        counter.revealOnComplete = guideGlow;

        // Welcome sign ------------------------------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 2.1 - Grab the Shard\nTab to a controller, aim, Grip to grab.\nSeat all three shards on the altar.",
                                  new Vector3(0f, 2.4f, 3.2f), 0.045f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_2_1");
        Selection.activeGameObject = rig;
    }

    /// <summary>Turns a primitive into an XRI grab interactable: Rigidbody + XRGrabInteractable with sane defaults.</summary>
    static XRGrabInteractable MakeGrabbable(GameObject go, float massKg)
    {
        var rb = go.AddComponent<Rigidbody>();
        rb.mass = massKg;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        var grab = go.AddComponent<XRGrabInteractable>();
        grab.movementType = XRBaseInteractable.MovementType.Kinematic; // follows the hand, still collides with the altar
        grab.useDynamicAttach = true;                                  // pick it up where you touched it
        grab.throwOnDetach = true;
        return grab;
    }

    /// <summary>Registers a tag in the project's Tag Manager if it is not there yet.</summary>
    static void EnsureTag(string tag)
    {
        if (Array.IndexOf(InternalEditorUtility.tags, tag) >= 0) return;
        InternalEditorUtility.AddTag(tag);
        Debug.Log("[Activity] Added tag '" + tag + "' to the project.");
    }
}

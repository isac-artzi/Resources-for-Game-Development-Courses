// ActivitySceneBuilder.cs — Activity 3.4: Ancient Ruins: Modular Kit and Mechanisms
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a ruins room assembled from 1 m and 2 m wall modules on a whole-meter
// grid, a stone door (SlidingDoor) in the front wall, a physical lever outside (XRGrabInteractable + HingeJoint +
// LeverInteractable, wired to the door through UnityEvents), an altar with an XRSocketInteractor watched by
// HiddenChamberReveal, a false wall (SlidingDoor) hiding a chamber with a lore scroll, plus the Sun Disc (ArtifactPiece)
// and a decoy Loose Stone to place. Everything is greybox; the README swaps in a free modular kit.
// Created by Isac Artzi

using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using ActivityTools;

public static class ActivitySceneBuilder
{
    const float WallHeight = 3f;
    const float WallThickness = 0.5f;

    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1f, 0.92f, 0.78f), 0.9f, new Vector3(55f, 200f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.55f, 0.55f, 0.50f), 0.01f);

        // Materials -------------------------------------------------------
        var ground = SceneBuilderUtil.MakeMaterial("Ruins Ground", new Color(0.40f, 0.38f, 0.33f));
        var wall   = SceneBuilderUtil.MakeMaterial("Ruins Wall",   new Color(0.58f, 0.55f, 0.50f));
        var door   = SceneBuilderUtil.MakeMaterial("Door Slab",    new Color(0.42f, 0.40f, 0.44f));
        var wood   = SceneBuilderUtil.MakeMaterial("Lever Wood",   new Color(0.40f, 0.27f, 0.15f));
        var iron   = SceneBuilderUtil.MakeMaterial("Iron",         new Color(0.25f, 0.25f, 0.28f));
        var stone  = SceneBuilderUtil.MakeMaterial("Loose Stone",  new Color(0.50f, 0.48f, 0.45f));
        var gold   = SceneBuilderUtil.MakeEmissiveMaterial("Sun Disc Gold", new Color(1f, 0.80f, 0.30f), 1.5f);
        var scroll = SceneBuilderUtil.MakeEmissiveMaterial("Lore Scroll",   new Color(0.95f, 0.90f, 0.70f), 0.8f);

        // Floor (teleportable) and player -----------------------------------
        var floor = SceneBuilderUtil.AddFloor(30f, ground);
        var area = floor.AddComponent<TeleportationArea>();
        area.matchOrientation = MatchOrientation.WorldSpaceUp;
        var rig = SceneBuilderUtil.AddXrRig(new Vector3(0f, 0f, -5f));

        // Room from modules on a whole-meter grid ---------------------------------------------------------
        // Interior: x in [-4, 4], z in [0, 6]. Hidden chamber: z in [6.5, 10]. Doorway: x in [-1, 1] at z = 0.
        var walls = SceneBuilderUtil.AddEmpty("Ruins Walls (modular, 1 m grid)", Vector3.zero);

        // Front wall (z = 0): 1 m modules, gap at x in [-1, 1]
        float[] frontX = { -3.5f, -2.5f, -1.5f, 1.5f, 2.5f, 3.5f };
        for (int i = 0; i < frontX.Length; i++)
            WallModule("Front Wall 1m", new Vector3(frontX[i], 0f, 0f), 1f, wall, walls.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Door Lintel", new Vector3(0f, WallHeight - 0.25f, 0f),
                                      new Vector3(2f, 0.5f, WallThickness), wall, walls.transform);

        // Side walls (x = +-4.25): 2 m modules from z = 0 to z = 10 (room + chamber)
        for (int i = 0; i < 5; i++)
        {
            float z = 1f + i * 2f;
            WallModule("Side Wall 2m (west)", new Vector3(-4.25f, 0f, z), 2f, wall, walls.transform, true);
            WallModule("Side Wall 2m (east)", new Vector3(4.25f, 0f, z), 2f, wall, walls.transform, true);
        }

        // Back wall of the main room (z = 6.25): 1 m modules, gap at x in [-1, 1] for the False Wall
        for (int i = 0; i < frontX.Length; i++)
            WallModule("Back Wall 1m", new Vector3(frontX[i], 0f, 6.25f), 1f, wall, walls.transform);

        // Chamber back wall (z = 10.25): 2 m modules
        float[] chamberX = { -3f, -1f, 1f, 3f };
        for (int i = 0; i < chamberX.Length; i++)
            WallModule("Chamber Wall 2m", new Vector3(chamberX[i], 0f, 10.25f), 2f, wall, walls.transform);
        // Corner posts close the 0.5 m gap where the side walls meet the chamber's back wall.
        WallModule("Corner Post (west)", new Vector3(-4.25f, 0f, 10.25f), WallThickness, wall, walls.transform);
        WallModule("Corner Post (east)", new Vector3(4.25f, 0f, 10.25f), WallThickness, wall, walls.transform);

        SceneBuilderUtil.MarkStaticRecursive(walls);

        // Mechanisms root (things that move are NOT static) ---------------------------------------------
        var mechanisms = SceneBuilderUtil.AddEmpty("Mechanisms", Vector3.zero);

        // Stone door: a 2 x 2.8 m slab in the doorway that sinks into the floor
        var doorGo = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Stone Door", new Vector3(0f, 1.4f, 0f),
                                                   new Vector3(2f, 2.8f, 0.3f), door, mechanisms.transform);
        var slidingDoor = doorGo.AddComponent<SlidingDoor>();
        slidingDoor.openOffset = new Vector3(0f, -2.85f, 0f);
        slidingDoor.duration = 2.5f;

        // False wall: fills the gap in the back wall; sinks when the chamber is revealed
        var falseWallGo = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "False Wall", new Vector3(0f, WallHeight * 0.5f, 6.25f),
                                                        new Vector3(2f, WallHeight, WallThickness), wall, mechanisms.transform);
        var falseWall = falseWallGo.AddComponent<SlidingDoor>();
        falseWall.openOffset = new Vector3(0f, -(WallHeight + 0.1f), 0f);
        falseWall.duration = 3.5f;

        // The lever -----------------------------------------------------------------------------------
        var leverPos = new Vector3(2.5f, 0f, -1.5f);
        var leverBase = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Lever Base", leverPos + Vector3.up * 0.5f,
                                                      new Vector3(0.4f, 1f, 0.4f), iron, mechanisms.transform);
        var baseRb = leverBase.AddComponent<Rigidbody>();
        baseRb.isKinematic = true;

        // Cylinder mesh spans local y -1..1; at scale y 0.4 it is 0.8 m long. Center it 0.4 m above the pivot.
        var pivot = leverPos + Vector3.up * 1.05f;
        var handle = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Lever Handle", pivot + Vector3.up * 0.4f,
                                                   new Vector3(0.06f, 0.4f, 0.06f), wood, mechanisms.transform);
        // Knob: child of a non-uniformly scaled parent, so counter-scale it to a 0.12 m sphere.
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Knob", pivot + Vector3.up * 0.8f, new Vector3(2f, 0.3f, 2f), iron, handle.transform);

        var handleRb = handle.AddComponent<Rigidbody>();
        handleRb.useGravity = false;
        handleRb.mass = 2f;
        handleRb.interpolation = RigidbodyInterpolation.Interpolate;

        var hinge = handle.AddComponent<HingeJoint>();
        hinge.connectedBody = baseRb;
        hinge.anchor = new Vector3(0f, -1f, 0f);      // bottom of the cylinder (local, before scale) = the pivot
        hinge.axis = Vector3.right;                    // swings toward/away from the player (along z)
        hinge.useLimits = true;
        var limits = hinge.limits;
        limits.min = -60f;
        limits.max = 60f;
        hinge.limits = limits;

        var grab = handle.AddComponent<XRGrabInteractable>();
        grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;   // let the joint constrain the motion
        grab.throwOnDetach = false;
        grab.useDynamicAttach = true;                                            // grab anywhere on the handle, no snap
        grab.trackRotation = true;

        var indicatorGo = SceneBuilderUtil.AddPointLight("Lever Indicator", leverPos + new Vector3(0f, 1.1f, -0.45f), Color.red, 2.5f, 1.5f, leverBase.transform);

        var lever = handle.AddComponent<LeverInteractable>();
        lever.hinge = hinge;
        lever.restAngle = -60f;
        lever.pulledAngle = 60f;
        lever.threshold = 0.8f;
        lever.releaseBelow = 0.5f;
        lever.indicator = indicatorGo.GetComponent<Light>();
        // Persistent listeners survive saving the scene (a plain AddListener would be lost on save).
        UnityEventTools.AddPersistentListener(lever.onPulled, slidingDoor.Open);
        UnityEventTools.AddPersistentListener(lever.onReleased, slidingDoor.Close);

        SceneBuilderUtil.AddLabel("Lever Label", "Pull the lever\n(LeverInteractable TODO 1-4)", leverPos + new Vector3(0f, 2.1f, 0f), 0.04f,
                                  new Color(1f, 0.95f, 0.7f), mechanisms.transform);

        // Altar + socket inside the room -----------------------------------------------------------------
        var altarPos = new Vector3(0f, 0f, 4f);
        var altar = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Altar", altarPos + Vector3.up * 0.5f, new Vector3(0.6f, 1f, 0.6f), wall, mechanisms.transform);
        altar.isStatic = true;
        var socketGo = SceneBuilderUtil.AddEmpty("Altar Socket", altarPos + Vector3.up * 1.15f, altar.transform);
        var socketCollider = socketGo.AddComponent<SphereCollider>();
        socketCollider.isTrigger = true;
        socketCollider.radius = 0.2f;
        var socket = socketGo.AddComponent<XRSocketInteractor>();
        socket.showInteractableHoverMeshes = true;

        var altarLabel = SceneBuilderUtil.AddLabel("Altar Message", "Place the Sun Disc on the altar", altarPos + Vector3.up * 1.9f, 0.04f,
                                                   new Color(0.9f, 0.95f, 1f), mechanisms.transform);

        // Hidden chamber contents ----------------------------------------------------------------------
        var chamberPos = new Vector3(0f, 0f, 8.5f);
        var chamber = SceneBuilderUtil.AddEmpty("Hidden Chamber", chamberPos, mechanisms.transform);
        var scrollPedestal = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Scroll Pedestal", chamberPos + Vector3.up * 0.5f,
                                                           new Vector3(0.5f, 0.5f, 0.5f), wall, chamber.transform);
        scrollPedestal.isStatic = true;
        var loreScroll = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Lore Scroll", chamberPos + Vector3.up * 1.1f,
                                                       new Vector3(0.06f, 0.18f, 0.06f), scroll, chamber.transform);
        loreScroll.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        var revealLightGo = SceneBuilderUtil.AddPointLight("Reveal Light", chamberPos + Vector3.up * 2.2f, new Color(1f, 0.9f, 0.6f), 6f, 2.5f, chamber.transform);
        var revealLight = revealLightGo.GetComponent<Light>();
        revealLight.enabled = false;

        var reveal = chamber.AddComponent<HiddenChamberReveal>();
        reveal.socket = socket;
        reveal.requiredPieceId = "sun";
        reveal.revealLight = revealLight;
        reveal.message = altarLabel.GetComponent<TextMesh>();
        UnityEventTools.AddPersistentListener(reveal.onRevealed, falseWall.Open);

        // Things to pick up: the Sun Disc (the key) and a Loose Stone (a decoy) ----------------------------
        var table = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Offering Table", new Vector3(-2.5f, 0.5f, -1.5f),
                                                  new Vector3(1.6f, 1f, 0.5f), wall, mechanisms.transform);
        table.isStatic = true;

        var disc = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Sun Disc", new Vector3(-3.0f, 1.05f, -1.5f),
                                                 new Vector3(0.25f, 0.05f, 0.25f), gold, mechanisms.transform);
        disc.AddComponent<Rigidbody>();
        var discGrab = disc.AddComponent<XRGrabInteractable>();
        discGrab.useDynamicAttach = true;
        var piece = disc.AddComponent<ArtifactPiece>();
        piece.pieceId = "sun";
        piece.displayName = "Sun Disc";

        var loose = SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Loose Stone", new Vector3(-2.0f, 1.1f, -1.5f),
                                                  Vector3.one * 0.2f, stone, mechanisms.transform);
        loose.AddComponent<Rigidbody>();
        var looseGrab = loose.AddComponent<XRGrabInteractable>();
        looseGrab.useDynamicAttach = true;

        // Signs -----------------------------------------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 3.4 - Ancient Ruins\nPull the lever to open the door.\nCarry the right offering to the altar.",
                                  new Vector3(0f, 2.4f, -3f), 0.045f, Color.white);
        SceneBuilderUtil.AddLabel("Caretaker Label", "The caretaker of the ruins:\n\"Every wall here answers to something.\"",
                                  new Vector3(-3f, 2.2f, 3f), 0.035f, new Color(0.9f, 0.95f, 1f));

        SceneBuilderUtil.SaveScene(scene, "Activity_3_4");
        Selection.activeGameObject = handle;
    }

    /// <summary>
    /// One wall module standing on the floor with its base at the given grid position. Width is along x, or along z
    /// when alongZ is true. Sizes are whole meters so modules meet exactly on the grid.
    /// </summary>
    static GameObject WallModule(string name, Vector3 basePos, float width, Material mat, Transform parent, bool alongZ = false)
    {
        var size = alongZ ? new Vector3(WallThickness, WallHeight, width) : new Vector3(width, WallHeight, WallThickness);
        return SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, name, basePos + Vector3.up * WallHeight * 0.5f, size, mat, parent);
    }
}

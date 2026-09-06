// ActivitySceneBuilder.cs — Activity 6.3: The Sound of Touch: SFX and Haptics
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a stone workshop with a table, three grabbable props (shard, river stone,
// rune tablet) that carry ImpactSound + HapticPulse, a gong to hit them against, and the SFX Pool with its readout.
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using ActivityTools;

public static class ActivitySceneBuilder
{
    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1f, 0.92f, 0.80f), 1.0f, new Vector3(55f, 20f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.45f, 0.42f, 0.40f), 0.02f);

        // Materials -------------------------------------------------------
        var flagstone = SceneBuilderUtil.MakeMaterial("Flagstone", new Color(0.42f, 0.40f, 0.38f));
        var wall      = SceneBuilderUtil.MakeMaterial("Wall",      new Color(0.55f, 0.50f, 0.42f));
        var oak       = SceneBuilderUtil.MakeMaterial("Oak",       new Color(0.45f, 0.30f, 0.15f));
        var shardMat  = SceneBuilderUtil.MakeEmissiveMaterial("Shard", new Color(0.40f, 0.80f, 1.00f), 2f);
        var stoneMat  = SceneBuilderUtil.MakeMaterial("River Stone", new Color(0.50f, 0.52f, 0.55f));
        var runeMat   = SceneBuilderUtil.MakeEmissiveMaterial("Rune", new Color(0.75f, 0.55f, 0.30f), 0.4f);
        var bronze    = SceneBuilderUtil.MakeEmissiveMaterial("Bronze", new Color(0.80f, 0.55f, 0.20f), 0.3f);

        // Room: floor + three walls so throws do not vanish into the fog --------------------------------
        SceneBuilderUtil.AddFloor(12f, flagstone);
        var room = SceneBuilderUtil.AddEmpty("Workshop Walls", Vector3.zero);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Back Wall",  new Vector3(0f, 1.5f, 5f),  new Vector3(10f, 3f, 0.3f), wall, room.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Left Wall",  new Vector3(-5f, 1.5f, 1f), new Vector3(0.3f, 3f, 8f), wall, room.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Right Wall", new Vector3(5f, 1.5f, 1f),  new Vector3(0.3f, 3f, 8f), wall, room.transform);
        SceneBuilderUtil.MarkStaticRecursive(room);

        var rig = SceneBuilderUtil.AddXrRig(new Vector3(0f, 0f, 0f));

        // SFX Pool ----------------------------------------------------------
        var poolGo = SceneBuilderUtil.AddEmpty("SFX Pool", Vector3.zero);
        var pool = poolGo.AddComponent<SfxPool>();
        pool.poolSize = 8;
        pool.minDistance = 0.5f;
        pool.maxDistance = 20f;
        pool.placeholderHz = 700f;
        var poolLabel = SceneBuilderUtil.AddLabel("Pool Readout", "SFX Pool: implement TODO 1-3 in SfxPool.cs",
                                                  new Vector3(-2.5f, 1.8f, 3f), 0.04f, new Color(1f, 0.95f, 0.6f));
        pool.readout = poolLabel.GetComponent<TextMesh>();

        // Table at a comfortable 0.9 m ---------------------------------------
        var table = SceneBuilderUtil.AddEmpty("Table", new Vector3(0f, 0f, 1.2f));
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Table Top", new Vector3(0f, 0.9f, 1.2f), new Vector3(1.6f, 0.05f, 0.8f), oak, table.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Table Base", new Vector3(0f, 0.44f, 1.2f), new Vector3(1.2f, 0.88f, 0.5f), oak, table.transform);
        SceneBuilderUtil.MarkStaticRecursive(table);

        // Grabbable props ---------------------------------------------------
        var props = SceneBuilderUtil.AddEmpty("Props", Vector3.zero);

        var shard = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Shard", new Vector3(-0.45f, 1.06f, 1.2f),
                                                  new Vector3(0.12f, 0.26f, 0.12f), shardMat, props.transform);
        MakeGrabbable(shard, 0.3f, placeholderPitch: 1.8f);

        var stone = SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "River Stone", new Vector3(0f, 1.05f, 1.2f),
                                                  Vector3.one * 0.22f, stoneMat, props.transform);
        MakeGrabbable(stone, 0.8f, placeholderPitch: 0.7f);

        var tablet = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Rune Tablet", new Vector3(0.45f, 0.95f, 1.2f),
                                                   new Vector3(0.26f, 0.05f, 0.36f), runeMat, props.transform);
        MakeGrabbable(tablet, 1.2f, placeholderPitch: 1.1f);

        // The gong: a static-ish bronze disc on a frame. Kinematic Rigidbody so it also reports impacts. -----
        var gongRoot = SceneBuilderUtil.AddEmpty("Gong", new Vector3(0f, 0f, 3.2f));
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Gong Post L", new Vector3(-0.9f, 1.1f, 3.2f), new Vector3(0.1f, 2.2f, 0.1f), oak, gongRoot.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Gong Post R", new Vector3(0.9f, 1.1f, 3.2f),  new Vector3(0.1f, 2.2f, 0.1f), oak, gongRoot.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Gong Beam",   new Vector3(0f, 2.2f, 3.2f),    new Vector3(1.9f, 0.1f, 0.1f), oak, gongRoot.transform);
        var gong = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Gong Disc", new Vector3(0f, 1.3f, 3.2f),
                                                 new Vector3(1.2f, 0.04f, 1.2f), bronze, gongRoot.transform);
        gong.transform.rotation = Quaternion.Euler(90f, 0f, 0f);   // cylinder axis along z so the face looks at the player
        var gongBody = gong.AddComponent<Rigidbody>();
        gongBody.isKinematic = true;
        var gongSound = gong.AddComponent<ImpactSound>();
        gongSound.minImpactSpeed = 0.2f;
        gongSound.maxImpactSpeed = 5f;
        gongSound.placeholderPitch = 0.35f;
        gongSound.pitchRange = new Vector2(0.97f, 1.03f);
        SceneBuilderUtil.AddPointLight("Gong Light", new Vector3(0f, 1.3f, 2.6f), new Color(1f, 0.8f, 0.5f), 4f, 1.2f, gongRoot.transform);

        // Signs -------------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 6.3 - The Sound of Touch\nGrab a prop (Tab to a controller, aim, grip). Drop it. Throw it at the gong.\nEvery touch should sound - and, on the headset, be felt.",
                                  new Vector3(0f, 2.7f, 4.6f), 0.05f, Color.white);
        SceneBuilderUtil.AddLabel("Table Sign", "Shard - River Stone - Rune Tablet\nlight, heavy, flat", new Vector3(0f, 1.45f, 1.7f), 0.035f,
                                  new Color(0.9f, 0.9f, 1f));

        SceneBuilderUtil.SaveScene(scene, "Activity_6_3");
        Selection.activeGameObject = rig;
    }

    /// <summary>Turns a primitive into a throwable XRI grab interactable with ImpactSound and HapticPulse attached.</summary>
    static void MakeGrabbable(GameObject go, float massKg, float placeholderPitch)
    {
        var body = go.AddComponent<Rigidbody>();
        body.mass = massKg;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;   // small fast objects must not tunnel through the table
        body.interpolation = RigidbodyInterpolation.Interpolate;

        var grab = go.AddComponent<XRGrabInteractable>();
        grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;      // physical movement -> real collisions while held
        grab.throwOnDetach = true;
        grab.useDynamicAttach = true;

        var haptics = go.AddComponent<HapticPulse>();
        haptics.grabAmplitude = 0.4f;
        haptics.grabDuration = 0.06f;

        var impact = go.AddComponent<ImpactSound>();
        impact.minImpactSpeed = 0.3f;
        impact.maxImpactSpeed = 4f;
        impact.placeholderPitch = placeholderPitch;
        impact.haptics = haptics;
    }
}

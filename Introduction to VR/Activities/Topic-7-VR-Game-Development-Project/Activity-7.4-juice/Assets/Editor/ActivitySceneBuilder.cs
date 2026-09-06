// ActivitySceneBuilder.cs — Activity 7.4: Juice: Feedback and Polish
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a forest clearing with three grabbable shards, each carrying
// PickupBurst + TweenScale + a particle burst + a flash light + an AudioSource; a shared WorldToast; and a
// hidden Mysterious Guide that a "Director" (GuideReveal) brings on stage after the third pickup.
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
        SceneBuilderUtil.SetSun(new Color(0.9f, 0.85f, 1.0f), 0.6f, new Vector3(30f, -30f, 0f));   // dusk: flashes read better
        SceneBuilderUtil.SetFog(new Color(0.30f, 0.40f, 0.45f), 0.03f);

        var grass    = SceneBuilderUtil.MakeMaterial("Grass", new Color(0.18f, 0.35f, 0.16f));
        var stone    = SceneBuilderUtil.MakeMaterial("Stone", new Color(0.45f, 0.45f, 0.50f));
        var bark     = SceneBuilderUtil.MakeMaterial("Bark", new Color(0.30f, 0.22f, 0.14f));
        var leaves   = SceneBuilderUtil.MakeMaterial("Leaves", new Color(0.12f, 0.30f, 0.15f));
        var shardMat = SceneBuilderUtil.MakeEmissiveMaterial("Shard", new Color(0.40f, 0.80f, 1.00f), 2f);
        var guideMat = SceneBuilderUtil.MakeEmissiveMaterial("Guide", new Color(0.90f, 0.85f, 1.00f), 1.2f);

        SceneBuilderUtil.AddFloor(30f, grass);
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);

        var trees = SceneBuilderUtil.AddEmpty("Trees", Vector3.zero);
        for (int i = 0; i < 12; i++)
        {
            float a = i * Mathf.PI * 2f / 12f;
            var p = new Vector3(Mathf.Cos(a) * 10f, 0f, Mathf.Sin(a) * 10f);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + i, p + Vector3.up * 2f, new Vector3(0.5f, 2f, 0.5f), bark, trees.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + i, p + Vector3.up * 5f, Vector3.one * 3f, leaves, trees.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(trees);

        // Shared toast ---------------------------------------------------------
        var toastGo = SceneBuilderUtil.AddLabel("Toast", "", new Vector3(0f, 1.5f, 2f), 0.04f, new Color(1f, 0.95f, 0.6f));
        var toast = toastGo.AddComponent<WorldToast>();
        toast.label = toastGo.GetComponent<TextMesh>();

        // The Guide (hidden) and the director -----------------------------------
        var guide = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Mysterious Guide", new Vector3(0f, 1f, 6f), new Vector3(0.6f, 1f, 0.6f), guideMat);
        var guideTween = guide.AddComponent<TweenScale>();
        var revealLightGo = SceneBuilderUtil.AddPointLight("Reveal Light", new Vector3(0f, 1.8f, 5.5f), new Color(0.8f, 0.7f, 1f), 8f, 0.6f);
        var revealBurst = AddBurst("Reveal Burst", new Vector3(0f, 0.2f, 6f), new Color(0.9f, 0.8f, 1f), 120, null);
        var guideAudio = revealLightGo.AddComponent<AudioSource>();
        guideAudio.spatialBlend = 1f;
        guideAudio.playOnAwake = false;
        guideAudio.minDistance = 1f;
        guideAudio.maxDistance = 25f;

        var director = SceneBuilderUtil.AddEmpty("Director", Vector3.zero);
        var reveal = director.AddComponent<GuideReveal>();
        reveal.piecesNeeded = 3;
        reveal.guideRenderer = guide.GetComponent<Renderer>();
        reveal.guideTween = guideTween;
        reveal.revealLight = revealLightGo.GetComponent<Light>();
        reveal.revealBurst = revealBurst;
        reveal.audioSource = guideAudio;
        reveal.toast = toast;
        var status = SceneBuilderUtil.AddLabel("Status Sign", "Shards: 0 / 3", new Vector3(0f, 2.6f, 6f), 0.05f, new Color(1f, 0.95f, 0.6f));
        reveal.statusLabel = status.GetComponent<TextMesh>();

        // Three shards ---------------------------------------------------------------
        Vector3[] spots = { new Vector3(-2.0f, 0f, 2.5f), new Vector3(0.0f, 0f, 3.5f), new Vector3(2.0f, 0f, 2.5f) };
        float[] notes = { 659.25f, 783.99f, 987.77f };   // E5, G5, B5 — a chord when all three ring
        var shards = SceneBuilderUtil.AddEmpty("Shards", Vector3.zero);
        for (int i = 0; i < spots.Length; i++)
        {
            var spot = spots[i];
            var ped = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Pedestal " + (i + 1), spot + Vector3.up * 0.5f, new Vector3(0.4f, 0.5f, 0.4f), stone, shards.transform);
            ped.isStatic = true;

            var shard = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Shard " + (i + 1), spot + Vector3.up * 1.15f, new Vector3(0.12f, 0.25f, 0.12f), shardMat, shards.transform);
            shard.transform.rotation = Quaternion.Euler(45f, 0f, 45f);
            var rb = shard.AddComponent<Rigidbody>();
            rb.mass = 0.5f;
            shard.AddComponent<XRGrabInteractable>();

            var tween = shard.AddComponent<TweenScale>();
            tween.curve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.15f, 1.35f), new Keyframe(0.5f, 1f));
            tween.duration = 0.5f;

            var lightGo = SceneBuilderUtil.AddPointLight("Flash Light", spot + Vector3.up * 1.35f, new Color(0.4f, 0.8f, 1f), 4f, 1.0f, shard.transform);
            var burst = AddBurst("Burst", spot + Vector3.up * 1.15f, new Color(0.5f, 0.85f, 1f), 40, shard.transform);

            var audio = shard.AddComponent<AudioSource>();
            audio.spatialBlend = 1f;
            audio.playOnAwake = false;
            audio.minDistance = 0.5f;
            audio.maxDistance = 15f;

            var pb = shard.AddComponent<PickupBurst>();
            pb.burst = burst;
            pb.flashLight = lightGo.GetComponent<Light>();
            pb.tween = tween;
            pb.toast = toast;
            pb.reveal = reveal;
            pb.audioSource = audio;
            pb.chimeHz = notes[i];
            pb.toastText = "Shard " + (i + 1) + " taken!";
        }

        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 7.4 - Juice\nGrab the three shards (Tab to a controller, G)\nJ = test all bursts - R = test the reveal",
                                  new Vector3(0f, 2.0f, 4.5f), 0.04f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_7_4");
        Selection.activeGameObject = rig;
    }

    /// <summary>A one-shot particle burst: no looping, not playing on awake, sphere emitter, fades out over life.</summary>
    static ParticleSystem AddBurst(string name, Vector3 pos, Color color, int count, Transform parent)
    {
        var go = new GameObject(name, typeof(ParticleSystem));
        go.transform.SetParent(parent, false);
        go.transform.position = pos;
        var ps = go.GetComponent<ParticleSystem>();

        var main = ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.duration = 0.5f;
        main.startLifetime = 0.8f;
        main.startSpeed = 2.5f;
        main.startSize = 0.06f;
        main.startColor = color;
        main.gravityModifier = 0.4f;
        main.maxParticles = 300;

        var em = ps.emission;
        em.rateOverTime = 0f;
        em.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)count) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        var g = new Gradient();
        g.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                  new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
        var col = ps.colorOverLifetime;
        col.enabled = true;
        col.color = new ParticleSystem.MinMaxGradient(g);

        var r = go.GetComponent<ParticleSystemRenderer>();
        r.sharedMaterial = ParticleMaterial();
        return ps;
    }

    /// <summary>Unity's default particle material if available, otherwise a simple additive one.</summary>
    static Material ParticleMaterial()
    {
        var builtin = AssetDatabase.GetBuiltinExtraResource<Material>("Default-ParticleSystem.mat");
        if (builtin != null) return builtin;

        SceneBuilderUtil.EnsureFolder(SceneBuilderUtil.MaterialsFolder);
        string path = SceneBuilderUtil.MaterialsFolder + "/Spark.mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;
        var shader = Shader.Find("Legacy Shaders/Particles/Additive");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        var mat = new Material(shader);
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }
}

// ActivitySceneBuilder.cs — Activity 6.2: Soundscapes and Music Zones
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a trailhead and three 8 m zone pads in a row (Forest, Mountain, Ruins),
// each with a ZoneVolume trigger box and a few props; an Audio Director with two music sources and three
// AmbientLayers; labels; and a readout. No AudioMixer asset (you create that by hand — see README Task 5).
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using ActivityTools;

public static class ActivitySceneBuilder
{
    const float ZoneSize = 8f;

    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1f, 0.95f, 0.88f), 1.0f, new Vector3(50f, -25f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.65f, 0.70f, 0.75f), 0.012f);

        // Materials -------------------------------------------------------
        var dirt      = SceneBuilderUtil.MakeMaterial("Dirt",       new Color(0.40f, 0.33f, 0.25f));
        var grass     = SceneBuilderUtil.MakeMaterial("Grass",      new Color(0.24f, 0.48f, 0.20f));
        var snow      = SceneBuilderUtil.MakeMaterial("Snow",       new Color(0.85f, 0.88f, 0.92f));
        var sandstone = SceneBuilderUtil.MakeMaterial("Sandstone",  new Color(0.70f, 0.58f, 0.40f));
        var bark      = SceneBuilderUtil.MakeMaterial("Bark",       new Color(0.35f, 0.25f, 0.15f));
        var leaves    = SceneBuilderUtil.MakeMaterial("Leaves",     new Color(0.15f, 0.40f, 0.18f));
        var rock      = SceneBuilderUtil.MakeMaterial("Rock",       new Color(0.45f, 0.45f, 0.50f));
        var ruinStone = SceneBuilderUtil.MakeMaterial("Ruin Stone", new Color(0.55f, 0.50f, 0.42f));

        // Floor and player: the trail runs along +z; the player starts at the trailhead ------------------
        SceneBuilderUtil.AddFloor(60f, dirt);
        var rig = SceneBuilderUtil.AddXrRig(new Vector3(0f, 0f, -4f));

        // Zones: pads centered at z = 4, 12, 20 (each 8 x 8 m) -------------
        var zonesRoot = SceneBuilderUtil.AddEmpty("Zones", Vector3.zero);
        var forestZone   = MakeZone(zonesRoot.transform, ZoneVolume.ZoneId.Forest,   "Forest",   4f,  grass);
        var mountainZone = MakeZone(zonesRoot.transform, ZoneVolume.ZoneId.Mountain, "Mountain", 12f, snow);
        var ruinsZone    = MakeZone(zonesRoot.transform, ZoneVolume.ZoneId.Ruins,    "Ruins",    20f, sandstone);

        // Props so each zone reads at a glance ------------------------------
        var props = SceneBuilderUtil.AddEmpty("Props", Vector3.zero);
        for (int i = 0; i < 6; i++)
        {
            float x = (i % 2 == 0 ? -1f : 1f) * (3.2f + 0.3f * (i % 3));
            float z = 1.5f + (i / 2) * 2.4f;
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + (i + 1), new Vector3(x, 1.8f, z),
                                          new Vector3(0.4f, 1.8f, 0.4f), bark, props.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + (i + 1), new Vector3(x, 4.2f, z),
                                          Vector3.one * 2.4f, leaves, props.transform);
        }
        for (int i = 0; i < 5; i++)
        {
            float x = Mathf.Sin(i * 2.1f) * 3.2f;
            float z = 9.5f + i * 1.2f;
            float s = 0.8f + 0.5f * (i % 3);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Boulder " + (i + 1), new Vector3(x, s * 0.4f, z),
                                          new Vector3(s, s * 0.8f, s), rock, props.transform);
        }
        for (int i = 0; i < 4; i++)
        {
            float x = (i % 2 == 0 ? -1f : 1f) * 3f;
            float z = 17.5f + (i / 2) * 5f;
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Column " + (i + 1), new Vector3(x, 1.75f, z),
                                          new Vector3(0.6f, 1.75f, 0.6f), ruinStone, props.transform);
        }
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Lintel", new Vector3(0f, 3.6f, 22.5f),
                                      new Vector3(7f, 0.4f, 0.8f), ruinStone, props.transform);
        SceneBuilderUtil.MarkStaticRecursive(props);

        // Audio Director: music + ambience ---------------------------------
        var director = SceneBuilderUtil.AddEmpty("Audio Director", Vector3.zero);
        var manager = director.AddComponent<MusicZoneManager>();

        var musicRoot = SceneBuilderUtil.AddEmpty("Music", Vector3.zero, director.transform);
        manager.musicA = Add2DSource(SceneBuilderUtil.AddEmpty("Music A", Vector3.zero, musicRoot.transform));
        manager.musicB = Add2DSource(SceneBuilderUtil.AddEmpty("Music B", Vector3.zero, musicRoot.transform));
        manager.crossfadeSeconds = 3f;
        manager.snapshotSeconds = 2f;

        var ambienceRoot = SceneBuilderUtil.AddEmpty("Ambience", Vector3.zero, director.transform);
        manager.ambientLayers = new AmbientLayer[]
        {
            MakeAmbientLayer(ambienceRoot.transform, "Forest Ambience",   110f, 880f,  4f, new Vector2(2f, 5f)),
            MakeAmbientLayer(ambienceRoot.transform, "Mountain Ambience",  82f, 330f,  7f, new Vector2(1f, 3f)),
            MakeAmbientLayer(ambienceRoot.transform, "Ruins Ambience",     65f, 1760f, 5f, new Vector2(0.3f, 2f)),
        };

        forestZone.manager = manager;
        mountainZone.manager = manager;
        ruinsZone.manager = manager;

        // Readout that follows nothing — it stands at the trailhead so you can glance back at it ----------
        var readoutGo = SceneBuilderUtil.AddLabel("Director Readout", "Zone: None\nimplement TODO 1-5 in MusicZoneManager.cs",
                                                  new Vector3(0f, 2.2f, -1f), 0.04f, new Color(1f, 0.95f, 0.6f));
        manager.readout = readoutGo.GetComponent<TextMesh>();

        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 6.2 - Soundscapes and Music Zones\nWalk the trail: Forest, then Mountain, then Ruins.\nListen for the music to change under your feet.",
                                  new Vector3(0f, 2.8f, 1f), 0.05f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_6_2");
        Selection.activeGameObject = rig;
    }

    /// <summary>A colored 8 x 8 m pad, a ZoneVolume trigger box 4 m tall above it, and a label.</summary>
    static ZoneVolume MakeZone(Transform parent, ZoneVolume.ZoneId id, string label, float centerZ, Material padMat)
    {
        var pad = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, label + " Pad", new Vector3(0f, 0.02f, centerZ),
                                                new Vector3(ZoneSize, 0.04f, ZoneSize), padMat, parent);
        pad.isStatic = true;

        var volume = SceneBuilderUtil.AddTriggerVolume(label + " Zone", new Vector3(0f, 2f, centerZ),
                                                       new Vector3(ZoneSize, 4f, ZoneSize), parent);
        var zone = volume.AddComponent<ZoneVolume>();
        zone.zone = id;

        SceneBuilderUtil.AddLabel(label + " Sign", label, new Vector3(0f, 3.2f, centerZ - ZoneSize * 0.5f + 0.5f), 0.08f,
                                  new Color(1f, 1f, 0.85f), parent);
        return zone;
    }

    /// <summary>A non-spatial (2D) looping music source with no clip.</summary>
    static AudioSource Add2DSource(GameObject go)
    {
        var src = go.AddComponent<AudioSource>();
        src.clip = null;
        src.playOnAwake = false;
        src.loop = true;
        src.spatialBlend = 0f;
        src.volume = 0f;
        return src;
    }

    /// <summary>An AmbientLayer with a bed source (mostly 2D) and no clips.</summary>
    static AmbientLayer MakeAmbientLayer(Transform parent, string name, float bedHz, float blipHz, float meanGap, Vector2 heights)
    {
        var go = SceneBuilderUtil.AddEmpty(name, Vector3.zero, parent);
        var bedGo = SceneBuilderUtil.AddEmpty("Bed", Vector3.zero, go.transform);
        var bed = bedGo.AddComponent<AudioSource>();
        bed.clip = null;
        bed.playOnAwake = false;
        bed.loop = true;
        bed.spatialBlend = 0.15f;   // almost 2D: a bed surrounds you, it does not sit at a point
        bed.volume = 0f;

        var layer = go.AddComponent<AmbientLayer>();
        layer.bed = bed;
        layer.bedVolume = 0.5f;
        layer.fadeSeconds = 2f;
        layer.placeholderBedHz = bedHz;
        layer.placeholderOneShotHz = blipHz;
        layer.meanSecondsBetween = meanGap;
        layer.heightRange = heights;
        layer.voices = 3;
        return layer;
    }
}

// ActivitySceneBuilder.cs — Activity 1.3: Story Beats in Space
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: five story stations on an arc (forest edge, enchanted forest,
// mountain pass, ancient ruins, restoration altar), the Guide at the center, and a sequencer pre-filled with a
// sample five-beat storyline for you to replace with your own.
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ActivityTools;

public static class ActivitySceneBuilder
{
    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        // Dusk: a dim sun so the beacons carry the attention.
        SceneBuilderUtil.SetSun(new Color(0.55f, 0.55f, 0.75f), 0.35f, new Vector3(20f, -60f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.12f, 0.14f, 0.22f), 0.03f);
        RenderSettings.ambientLight = new Color(0.18f, 0.20f, 0.28f);

        // Materials -------------------------------------------------------
        var ground  = SceneBuilderUtil.MakeMaterial("DuskGround", new Color(0.16f, 0.22f, 0.18f));
        var bark    = SceneBuilderUtil.MakeMaterial("Bark",       new Color(0.30f, 0.22f, 0.14f));
        var leaves  = SceneBuilderUtil.MakeMaterial("Leaves",     new Color(0.12f, 0.32f, 0.16f));
        var stone   = SceneBuilderUtil.MakeMaterial("Stone",      new Color(0.50f, 0.50f, 0.54f));
        var rock    = SceneBuilderUtil.MakeMaterial("Rock",       new Color(0.36f, 0.38f, 0.44f));
        var disc    = SceneBuilderUtil.MakeMaterial("StationDisc", new Color(0.22f, 0.26f, 0.34f));
        var shard   = SceneBuilderUtil.MakeEmissiveMaterial("Shard", new Color(0.40f, 0.80f, 1.00f), 2f);
        var guideMat = SceneBuilderUtil.MakeEmissiveMaterial("Guide", new Color(0.90f, 0.85f, 1.00f), 1.2f);

        // Floor and player ------------------------------------------------
        SceneBuilderUtil.AddFloor(40f, ground);
        var rig = SceneBuilderUtil.AddXrRig(new Vector3(0f, 0f, -1f));

        // Stations on an arc, left to right, about 8 m from the player -----
        var stationsRoot = SceneBuilderUtil.AddEmpty("Stations", Vector3.zero);
        Vector3[] spots =
        {
            new Vector3(-8.0f, 0f, 4.0f),
            new Vector3(-4.5f, 0f, 8.0f),
            new Vector3( 0.0f, 0f, 10.0f),
            new Vector3( 4.5f, 0f, 8.0f),
            new Vector3( 8.0f, 0f, 4.0f),
        };
        Color[] beaconColors =
        {
            new Color(0.55f, 0.85f, 0.60f),   // forest edge, green
            new Color(0.40f, 0.80f, 1.00f),   // enchanted forest, shard blue
            new Color(0.85f, 0.90f, 1.00f),   // mountain, cold white
            new Color(1.00f, 0.75f, 0.45f),   // ruins, torch amber
            new Color(1.00f, 0.90f, 0.60f),   // restoration, gold
        };
        var stations = new BeatStation[spots.Length];
        for (int i = 0; i < spots.Length; i++)
        {
            var root = SceneBuilderUtil.AddEmpty("Station " + (i + 1), spots[i], stationsRoot.transform);
            var pad = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Disc", spots[i] + Vector3.up * 0.02f,
                                                    new Vector3(3.5f, 0.02f, 3.5f), disc, root.transform);
            pad.isStatic = true;
            var beacon = SceneBuilderUtil.AddPointLight("Beacon", spots[i] + Vector3.up * 3.5f, beaconColors[i], 9f, 0.15f, root.transform);
            var label = SceneBuilderUtil.AddLabel("Title Label", "Station " + (i + 1) + " (title set by StoryBeatSequencer TODO 1)",
                                                  spots[i] + Vector3.up * 2.7f, 0.05f, beaconColors[i], root.transform);
            // Labels face the player's starting position; walk-around readability is a stretch goal.
            Vector3 away = spots[i] - new Vector3(0f, 0f, -1f); away.y = 0f;
            label.transform.rotation = Quaternion.LookRotation(away);

            var st = root.AddComponent<BeatStation>();
            st.beacon = beacon.GetComponent<Light>();
            st.titleLabel = label.GetComponent<TextMesh>();
            stations[i] = st;

            DressStation(i, root.transform, spots[i], bark, leaves, stone, rock, shard);
        }

        // The Guide at the center of the arc ---------------------------------
        var guideRoot = SceneBuilderUtil.AddEmpty("Mysterious Guide", new Vector3(0f, 0f, 3f));
        var body = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Body", new Vector3(0f, 1f, 3f),
                                                 new Vector3(0.6f, 1f, 0.6f), guideMat, guideRoot.transform);
        // A small "nose" so you can see which way she faces.
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Nose", new Vector3(0f, 1.6f, 3.32f), Vector3.one * 0.12f, shard, body.transform);
        SceneBuilderUtil.AddPointLight("Guide Glow", new Vector3(0f, 1.6f, 2.6f), new Color(0.8f, 0.7f, 1f), 4f, 1.2f, guideRoot.transform);
        var speech = SceneBuilderUtil.AddLabel("Speech Label", "", new Vector3(0f, 2.4f, 3f), 0.04f, new Color(0.95f, 0.92f, 1f), guideRoot.transform);
        var voice = guideRoot.AddComponent<GuideVoice>();
        voice.speechLabel = speech.GetComponent<TextMesh>();
        var blip = guideRoot.AddComponent<AudioSource>();
        blip.playOnAwake = false;
        blip.spatialBlend = 1f;
        blip.volume = 0.4f;
        voice.blip = blip;   // no clip yet: drag any short click into AudioSource > AudioClip to hear it

        // The sequencer with a sample storyline ------------------------------
        var seqGo = SceneBuilderUtil.AddEmpty("Story Sequencer", Vector3.zero);
        var seq = seqGo.AddComponent<StoryBeatSequencer>();
        seq.guide = voice;
        // StoryBeat is a plain [Serializable] data class, not a component, so it is built here rather than AddComponent-ed.
        var sample = new List<StoryBeat>
        {
            new StoryBeat { title = "The Awakening", act = StoryBeat.Act.Beginning, station = stations[0],
                            guideLine = "You wake at the edge of the Enchanted Forest. The balance of Elaria is broken, and the artifact that held it lies in pieces." },
            new StoryBeat { title = "The First Shard", act = StoryBeat.Act.Beginning, station = stations[1],
                            guideLine = "The Forest Sage has kept one shard safe. Earn her trust, and she will show you the way into the trees." },
            new StoryBeat { title = "The Mountain Pass", act = StoryBeat.Act.Middle, station = stations[2],
                            guideLine = "Higher, where the wind speaks in riddles, a hermit guards the second shard. He will test your patience before your courage." },
            new StoryBeat { title = "The Ancient Ruins", act = StoryBeat.Act.Middle, station = stations[3],
                            guideLine = "Beneath the ruins the caretaker waits. The last shard is locked behind a pattern only the attentive can read." },
            new StoryBeat { title = "Restoration", act = StoryBeat.Act.End, station = stations[4],
                            guideLine = "Bring the shards together at the altar. What you restore will not be the world as it was, but the world as you have made it." },
        };
        seq.beats = sample.ToArray();

        // Welcome sign ----------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Story Beats in Space\nActivity 1.3\n] next beat   [ previous   Backspace restart",
                                  new Vector3(0f, 2.0f, 1.2f), 0.04f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_1_3");
        Selection.activeGameObject = rig;
    }

    /// <summary>Puts a few primitives on each station so the five places read as five different environments.</summary>
    static void DressStation(int i, Transform parent, Vector3 p, Material bark, Material leaves, Material stone, Material rock, Material shard)
    {
        switch (i)
        {
            case 0: // Forest edge: three trees and a mossy boulder
                for (int t = 0; t < 3; t++)
                {
                    var b = p + new Vector3(-1.2f + t * 1.2f, 0f, 1.0f + (t % 2) * 0.8f);
                    SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk", b + Vector3.up * 1.8f, new Vector3(0.4f, 1.8f, 0.4f), bark, parent);
                    SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown", b + Vector3.up * 4.4f, Vector3.one * 2.6f, leaves, parent);
                }
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Boulder", p + new Vector3(0.8f, 0.35f, -0.6f), new Vector3(1.2f, 0.7f, 1.0f), rock, parent);
                break;
            case 1: // Enchanted forest: a pedestal with a shard among two trees
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Pedestal", p + Vector3.up * 0.5f, new Vector3(0.4f, 0.5f, 0.4f), stone, parent);
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Shard", p + Vector3.up * 1.3f, new Vector3(0.15f, 0.3f, 0.15f), shard, parent)
                                .transform.rotation = Quaternion.Euler(45f, 0f, 45f);
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk", p + new Vector3(-1.3f, 1.8f, 1.0f), new Vector3(0.4f, 1.8f, 0.4f), bark, parent);
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown", p + new Vector3(-1.3f, 4.4f, 1.0f), Vector3.one * 2.6f, leaves, parent);
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk", p + new Vector3(1.3f, 1.8f, 0.8f), new Vector3(0.4f, 1.8f, 0.4f), bark, parent);
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown", p + new Vector3(1.3f, 4.4f, 0.8f), Vector3.one * 2.6f, leaves, parent);
                break;
            case 2: // Mountain pass: a 15-degree ramp and a rock face
                var ramp = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Ramp (15 deg)", p + new Vector3(0f, 0.55f, 0.5f), new Vector3(1.6f, 0.2f, 4f), rock, parent);
                ramp.transform.rotation = Quaternion.Euler(-15f, 0f, 0f);
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Rock Face", p + new Vector3(0f, 2.0f, 3.2f), new Vector3(5f, 4f, 1.2f), rock, parent);
                break;
            case 3: // Ancient ruins: four pillars and a lintel
                for (int c = 0; c < 4; c++)
                {
                    var cp = p + new Vector3((c % 2 == 0 ? -1f : 1f) * 1.1f, 1.4f, (c < 2 ? -0.8f : 0.8f));
                    SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Pillar", cp, new Vector3(0.35f, 1.4f, 0.35f), stone, parent);
                }
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Lintel", p + new Vector3(0f, 2.95f, -0.8f), new Vector3(2.8f, 0.3f, 0.4f), stone, parent);
                break;
            default: // Restoration: an altar with three shard sockets
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Altar", p + Vector3.up * 0.5f, new Vector3(1.6f, 1.0f, 0.8f), stone, parent);
                for (int s = 0; s < 3; s++)
                    SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Shard", p + new Vector3(-0.5f + s * 0.5f, 1.25f, 0f), new Vector3(0.12f, 0.25f, 0.12f), shard, parent)
                                    .transform.rotation = Quaternion.Euler(45f, 0f, 45f);
                break;
        }
    }
}

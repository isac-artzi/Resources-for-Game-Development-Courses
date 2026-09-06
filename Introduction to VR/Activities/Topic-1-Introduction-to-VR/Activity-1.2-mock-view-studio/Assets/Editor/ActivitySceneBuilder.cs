// ActivitySceneBuilder.cs — Activity 1.2: Mock View Studio
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a diorama stage — the Forest Sage's hut blocked out with primitives,
// five camera stations at designer-chosen distances, three annotation pins, and a prop placer.
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
        SceneBuilderUtil.SetSun(new Color(1f, 0.93f, 0.82f), 1.0f, new Vector3(40f, 35f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.60f, 0.72f, 0.66f), 0.012f);

        // Materials -------------------------------------------------------
        var grass   = SceneBuilderUtil.MakeMaterial("Grass",     new Color(0.28f, 0.48f, 0.22f));
        var timber  = SceneBuilderUtil.MakeMaterial("Timber",    new Color(0.45f, 0.32f, 0.20f));
        var plaster = SceneBuilderUtil.MakeMaterial("Plaster",   new Color(0.80f, 0.76f, 0.66f));
        var parch   = SceneBuilderUtil.MakeMaterial("Parchment", new Color(0.92f, 0.85f, 0.62f));
        var rock    = SceneBuilderUtil.MakeMaterial("Rock",      new Color(0.42f, 0.44f, 0.50f));
        var disc    = SceneBuilderUtil.MakeEmissiveMaterial("StationDisc", new Color(0.30f, 0.75f, 1.00f), 1.5f);
        var clay    = SceneBuilderUtil.MakeMaterial("Clay",      new Color(0.85f, 0.55f, 0.35f));
        var sageMat = SceneBuilderUtil.MakeEmissiveMaterial("Sage",  new Color(0.65f, 0.90f, 0.70f), 0.8f);

        // Floor and player ------------------------------------------------
        SceneBuilderUtil.AddFloor(40f, grass);
        var rig = SceneBuilderUtil.AddXrRig(new Vector3(0f, 0f, -2.5f));

        // The Forest Sage's hut, blocked out at true scale ----------------
        // Interior 6 m wide (x), 5 m deep (z), 3 m tall; front wall at z = 5.5 with a 1.0 x 2.1 m doorway.
        var hut = SceneBuilderUtil.AddEmpty("Sage Hut (blockout)", Vector3.zero);
        const float wallT = 0.2f;
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Back Wall",  new Vector3(0f, 1.5f, 10.5f), new Vector3(6.4f, 3f, wallT), plaster, hut.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Left Wall",  new Vector3(-3.1f, 1.5f, 8f), new Vector3(wallT, 3f, 5f), plaster, hut.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Right Wall", new Vector3( 3.1f, 1.5f, 8f), new Vector3(wallT, 3f, 5f), plaster, hut.transform);
        // Front wall = two pieces + lintel, leaving a 1.0 m wide, 2.1 m tall opening centered on x = 0.
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Front Wall L", new Vector3(-1.85f, 1.5f, 5.5f), new Vector3(2.7f, 3f, wallT), plaster, hut.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Front Wall R", new Vector3( 1.85f, 1.5f, 5.5f), new Vector3(2.7f, 3f, wallT), plaster, hut.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Lintel",      new Vector3(0f, 2.55f, 5.5f),   new Vector3(1.0f, 0.9f, wallT), timber,  hut.transform);
        var doorway = SceneBuilderUtil.AddEmpty("Doorway (1.0 x 2.1 m)", new Vector3(0f, 1.05f, 5.5f), hut.transform);
        // Roof beams so the interior reads as a room from the far station.
        for (int i = 0; i < 4; i++)
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Beam " + (i + 1), new Vector3(0f, 3.05f, 6.2f + i * 1.3f), new Vector3(6.4f, 0.12f, 0.12f), timber, hut.transform);

        // Table (0.75 m high — a real table) with a scroll on it.
        var table = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Table", new Vector3(0f, 0.375f, 8f), new Vector3(1.2f, 0.75f, 0.6f), timber, hut.transform);
        var scroll = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Lore Scroll", new Vector3(0f, 0.78f, 7.9f), new Vector3(0.30f, 0.05f, 0.10f), parch, hut.transform);
        // The Forest Sage: a 1.7 m capsule (capsule is 2 m tall at scale 1 -> y scale 0.85).
        var sage = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Forest Sage", new Vector3(1.5f, 0.85f, 9f), new Vector3(0.45f, 0.85f, 0.45f), sageMat, hut.transform);
        SceneBuilderUtil.AddPointLight("Hearth Light", new Vector3(-2f, 1.2f, 9.5f), new Color(1f, 0.75f, 0.45f), 7f, 1.6f, hut.transform);
        SceneBuilderUtil.MarkStaticRecursive(hut);
        table.isStatic = true; scroll.isStatic = true; sage.isStatic = false;

        // A far landmark: a mountain dome 45 m out (only its top shows above the fog).
        var mountain = SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Mountain", new Vector3(6f, -14f, 45f), Vector3.one * 44f, rock);
        mountain.isStatic = true;

        // Camera stations -------------------------------------------------
        // Each station is an empty at floor level whose forward is the intended viewing direction,
        // with a glowing floor disc and a small label so you can find it on foot.
        var stationsRoot = SceneBuilderUtil.AddEmpty("Stations", Vector3.zero);
        string[] names =
        {
            "Station 1 - Approach (doorway at 8 m, far zone)",
            "Station 2 - Threshold (doorway at 2.5 m, medium zone)",
            "Station 3 - The Scroll (0.75 m, near zone)",
            "Station 4 - Speaking with the Sage (1.5 m)",
            "Station 5 - Overview (hut at 12 m)",
        };
        Vector3[] positions =
        {
            new Vector3(0f, 0f, -2.5f),
            new Vector3(0f, 0f,  3.0f),
            new Vector3(0f, 0f,  6.95f),
            new Vector3(1.5f, 0f, 7.5f),
            new Vector3(-8.5f, 0f, -0.5f),
        };
        float[] yaws = { 0f, 0f, 0f, 0f, 45f };
        var stations = new Transform[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            var st = SceneBuilderUtil.AddEmpty(names[i], positions[i], stationsRoot.transform);
            st.transform.rotation = Quaternion.Euler(0f, yaws[i], 0f);
            var pad = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Disc", positions[i] + Vector3.up * 0.01f,
                                                    new Vector3(0.6f, 0.01f, 0.6f), disc, st.transform);
            pad.isStatic = true;
            SceneBuilderUtil.AddLabel("Number", (i + 1).ToString(), positions[i] + Vector3.up * 0.02f, 0.06f,
                                      new Color(0.1f, 0.2f, 0.3f), st.transform)
                            .transform.rotation = Quaternion.Euler(90f, yaws[i], 0f);   // flat on the floor, readable when facing the station's forward
            stations[i] = st.transform;
        }

        // The cycler and its readout ---------------------------------------
        var readout = SceneBuilderUtil.AddLabel("Station Readout", "Press ] for the next station, [ for the previous",
                                                new Vector3(0f, 1.35f, -1f), 0.03f, new Color(1f, 0.95f, 0.5f));
        var cyclerGo = SceneBuilderUtil.AddEmpty("Viewpoint Cycler", Vector3.zero);
        var cycler = cyclerGo.AddComponent<ViewpointCycler>();
        cycler.stations = stations;
        cycler.startStation = 0;
        cycler.stationLabel = readout.GetComponent<TextMesh>();

        // Annotation pins ---------------------------------------------------
        var pinsRoot = SceneBuilderUtil.AddEmpty("Pins", Vector3.zero);
        AddPin(pinsRoot.transform, "Pin - Doorway", doorway.transform.position + Vector3.up * 1.05f,
               "Doorway 1.0 x 2.1 m\nhuman scale reference", 0.5f);
        AddPin(pinsRoot.transform, "Pin - Scroll", scroll.transform.position + Vector3.up * 0.03f,
               "Lore scroll\nmust be readable at 0.75 m", 0.35f);
        AddPin(pinsRoot.transform, "Pin - Mountain", new Vector3(6f, 8f, 45f),
               "Mountain (far zone)\ndepth from perspective, not stereo", 2.0f);

        // Prop placer ---------------------------------------------------------
        var placedRoot = SceneBuilderUtil.AddEmpty("Placed Props", Vector3.zero);
        var placerGo = SceneBuilderUtil.AddEmpty("Prop Placer", Vector3.zero);
        var placer = placerGo.AddComponent<PropPlacer>();
        placer.material = clay;
        placer.parent = placedRoot.transform;
        placer.sizeMeters = 0.5f;
        placer.distanceMeters = 2f;

        // Welcome sign --------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Mock View Studio\nActivity 1.2\n] / [ stations   Enter place prop   - = distance   , . size",
                                  new Vector3(-3.5f, 2.2f, 2.5f), 0.045f, Color.white)
                        .transform.rotation = Quaternion.Euler(0f, -25f, 0f);

        SceneBuilderUtil.SaveScene(scene, "Activity_1_2");
        Selection.activeGameObject = rig;
    }

    /// <summary>A pin = marker sphere at the anchor + TextMesh label + LineRenderer leader, wired into AnnotationPin.</summary>
    static GameObject AddPin(Transform parent, string name, Vector3 anchorPos, string text, float labelHeight)
    {
        var pinMat = SceneBuilderUtil.MakeEmissiveMaterial("PinMarker", new Color(1f, 0.85f, 0.3f), 2f);
        var pin = SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, name, anchorPos, Vector3.one * 0.04f, pinMat, parent);
        Object.DestroyImmediate(pin.GetComponent<Collider>());

        // The label is a sibling, not a child, of the marker: the marker is scaled to 4 cm and would shrink the text.
        var labelGo = SceneBuilderUtil.AddLabel(name + " Label", "Pin: implement TODO 1-5 in AnnotationPin.cs",
                                                anchorPos + Vector3.up * labelHeight, 0.02f, new Color(1f, 0.95f, 0.6f), parent);
        var tm = labelGo.GetComponent<TextMesh>();
        tm.anchor = TextAnchor.LowerCenter;

        var leaderGo = SceneBuilderUtil.AddEmpty(name + " Leader", anchorPos, parent);
        var lr = leaderGo.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPosition(0, anchorPos);
        lr.SetPosition(1, anchorPos + Vector3.up * labelHeight);
        lr.startWidth = 0.004f;
        lr.endWidth = 0.004f;
        lr.startColor = tm.color;
        lr.endColor = tm.color;
        lr.sharedMaterial = SceneBuilderUtil.MakeMaterial("PinLeader", tm.color);

        var comp = pin.AddComponent<AnnotationPin>();
        comp.anchor = pin.transform;
        comp.text = text;
        comp.labelHeight = labelHeight;
        comp.label = tm;
        comp.leader = lr;
        return pin;
    }
}

// ActivitySceneBuilder.cs — Activity 1.4: Greybox Elaria
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a true-scale greybox of all three environments in one continuous route —
// a winding 2.5 m forest path, a mountain switchback (stairs, two 15-degree ramps, handrails) and a 6 x 8 x 4 m ruins
// room with a 1.0 x 2.1 m doorway — plus zone volumes, an announcer, and a metrics logger.
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using ActivityTools;

public static class ActivitySceneBuilder
{
    // Human-metric constants used below (meters). Change them here and rebuild to feel the difference.
    const float PathWidth   = 2.5f;
    const float StepRise    = 0.17f;
    const float StepTread   = 0.28f;
    const int   StepCount   = 6;
    const float RampRun     = 6.0f;     // horizontal length of each ramp
    const float RampSlope   = 15f;      // degrees
    const float RampWidth   = 2.0f;
    const float RailHeight  = 0.9f;
    const float DoorWidth   = 1.0f;
    const float DoorHeight  = 2.1f;
    const float RoomWidth   = 6.0f;     // z extent
    const float RoomDepth   = 8.0f;     // x extent
    const float RoomHeight  = 4.0f;

    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1f, 0.96f, 0.9f), 1.0f, new Vector3(50f, -20f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.70f, 0.75f, 0.78f), 0.008f);

        // Materials: greybox means grey, with a slight tint per environment so zones are distinguishable on video.
        var ground = SceneBuilderUtil.MakeMaterial("GreyGround", new Color(0.45f, 0.47f, 0.44f));
        var path   = SceneBuilderUtil.MakeMaterial("GreyPath",   new Color(0.60f, 0.58f, 0.52f));
        var tree   = SceneBuilderUtil.MakeMaterial("GreyTree",   new Color(0.50f, 0.56f, 0.48f));
        var rock   = SceneBuilderUtil.MakeMaterial("GreyRock",   new Color(0.52f, 0.54f, 0.60f));
        var ruin   = SceneBuilderUtil.MakeMaterial("GreyRuin",   new Color(0.62f, 0.58f, 0.50f));
        var rail   = SceneBuilderUtil.MakeMaterial("GreyRail",   new Color(0.30f, 0.30f, 0.32f));
        var marker = SceneBuilderUtil.MakeEmissiveMaterial("Marker", new Color(1f, 0.85f, 0.3f), 1.5f);

        SceneBuilderUtil.AddFloor(120f, ground);
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);

        // Shared systems ---------------------------------------------------------
        var announcerGo = SceneBuilderUtil.AddEmpty("Zone Announcer", Vector3.zero);
        var announcer = announcerGo.AddComponent<ZoneAnnouncer>();
        var bannerGo = SceneBuilderUtil.AddLabel("Zone Banner", "", new Vector3(0f, 1.3f, 2f), 0.08f, Color.white);
        announcer.banner = bannerGo.GetComponent<TextMesh>();
        var chime = announcerGo.AddComponent<AudioSource>();
        chime.playOnAwake = false;
        chime.spatialBlend = 0f;
        announcer.chime = chime;   // no clip yet; drag a short chime onto AudioSource > AudioClip

        var metricsGo = SceneBuilderUtil.AddEmpty("Player Metrics", Vector3.zero);
        var metrics = metricsGo.AddComponent<PlayerMetrics>();
        var readout = SceneBuilderUtil.AddLabel("Metrics Readout", "Metrics: implement TODO 1-5 in PlayerMetrics.cs",
                                                new Vector3(-2.5f, 1.5f, 1.5f), 0.035f, new Color(1f, 0.95f, 0.5f));
        readout.transform.rotation = Quaternion.Euler(0f, -30f, 0f);
        metrics.readout = readout.GetComponent<TextMesh>();

        // 1. Enchanted Forest: a winding 2.5 m path with trees on both sides -----------
        var forest = SceneBuilderUtil.AddEmpty("Enchanted Forest (greybox)", Vector3.zero);
        float[] headings = { 15f, -15f, -15f, 15f };      // degrees; net heading 0 and net lateral offset 0
        const float segLen = 5f;
        Vector3 p = new Vector3(0f, 0f, 2f);
        for (int i = 0; i < headings.Length; i++)
        {
            Quaternion q = Quaternion.Euler(0f, headings[i], 0f);
            Vector3 dir = q * Vector3.forward;
            Vector3 center = p + dir * (segLen * 0.5f);
            var seg = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Path Segment " + (i + 1), center + Vector3.up * 0.025f,
                                                    new Vector3(PathWidth, 0.05f, segLen + 0.6f), path, forest.transform);
            seg.transform.rotation = q;
            Vector3 side = q * Vector3.right;
            for (int t = 0; t < 2; t++)
            {
                Vector3 along = p + dir * (1.5f + t * 2.5f);
                foreach (float s in new float[] { -1f, 1f })
                {
                    Vector3 b = along + side * s * (PathWidth * 0.5f + 0.9f + (t * 0.4f));
                    SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk", b + Vector3.up * 2f, new Vector3(0.45f, 2f, 0.45f), tree, forest.transform);
                    SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown", b + Vector3.up * 5f, Vector3.one * 3f, tree, forest.transform);
                }
            }
            p += dir * segLen;
        }
        Vector3 forestEnd = new Vector3(0f, 0f, p.z);   // the path ends on the z axis
        SceneBuilderUtil.MarkStaticRecursive(forest);
        AddZone("Zone - Enchanted Forest", "Enchanted Forest", new Color(0.45f, 0.85f, 0.5f),
                new Vector3(0f, 1.5f, (1f + forestEnd.z) * 0.5f), new Vector3(7f, 3f, forestEnd.z - 1f), announcer, metrics);
        SceneBuilderUtil.AddLabel("Forest Sign", "Enchanted Forest path\n2.5 m wide - two people can pass",
                                  new Vector3(0f, 2.2f, 1.6f), 0.045f, Color.white);

        // 2. Mountain Pass: stairs, landing, two 15-degree ramps in a switchback, handrails -----------
        var mountain = SceneBuilderUtil.AddEmpty("Mountain Pass (greybox)", Vector3.zero);
        float z0 = forestEnd.z;
        for (int i = 0; i < StepCount; i++)
        {
            float top = (i + 1) * StepRise;
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Step " + (i + 1),
                                          new Vector3(0f, top * 0.5f, z0 + StepTread * 0.5f + i * StepTread),
                                          new Vector3(RampWidth + 0.4f, top, StepTread), rock, mountain.transform);
        }
        float zSteps = z0 + StepCount * StepTread;           // where the stairs end
        float h1 = StepCount * StepRise;                     // 1.02 m
        float landingDepth = 2f;
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Landing 1", new Vector3(0f, h1 * 0.5f, zSteps + landingDepth * 0.5f),
                                      new Vector3(RampWidth + 0.4f, h1, landingDepth), rock, mountain.transform);
        float zc = zSteps + landingDepth * 0.5f;             // center line of ramp 1
        float rise = RampRun * Mathf.Tan(RampSlope * Mathf.Deg2Rad);          // 1.61 m
        float slopeLen = RampRun / Mathf.Cos(RampSlope * Mathf.Deg2Rad);      // 6.21 m
        float x0 = (RampWidth + 0.4f) * 0.5f;               // ramp 1 starts at the landing's +x edge

        // Ramp 1 climbs toward +x.
        var ramp1 = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Ramp 1 (15 deg)",
                                                  new Vector3(x0 + RampRun * 0.5f, h1 + rise * 0.5f - 0.15f, zc),
                                                  new Vector3(RampWidth, 0.3f, slopeLen), rock, mountain.transform);
        ramp1.transform.rotation = Quaternion.Euler(-RampSlope, 90f, 0f);
        AddRail(ramp1.transform, new Vector3(0f, RailHeight + 0.15f, 0f), RampWidth * 0.5f - 0.05f, slopeLen, rail, mountain.transform);   // open (outer) edge

        // Landing 2 at the far end, spanning both ramps.
        float h2 = h1 + rise;
        float zc2 = zc + RampWidth + 0.2f;                   // center line of ramp 2
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Landing 2",
                                      new Vector3(x0 + RampRun + landingDepth * 0.5f, h2 * 0.5f, (zc + zc2) * 0.5f),
                                      new Vector3(landingDepth, h2, (zc2 - zc) + RampWidth), rock, mountain.transform);

        // Ramp 2 climbs back toward -x.
        var ramp2 = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Ramp 2 (15 deg)",
                                                  new Vector3(x0 + RampRun * 0.5f, h2 + rise * 0.5f - 0.15f, zc2),
                                                  new Vector3(RampWidth, 0.3f, slopeLen), rock, mountain.transform);
        ramp2.transform.rotation = Quaternion.Euler(-RampSlope, -90f, 0f);
        AddRail(ramp2.transform, new Vector3(0f, RailHeight + 0.15f, 0f), RampWidth * 0.5f - 0.05f, slopeLen, rail, mountain.transform);   // open (outer) edge

        // Rock spine between the ramps, and a cliff behind.
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Rock Spine", new Vector3(x0 + RampRun * 0.5f, 2.4f, (zc + zc2) * 0.5f),
                                      new Vector3(RampRun, 4.8f, 0.2f), rock, mountain.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Cliff", new Vector3(x0 + RampRun * 0.5f + 1f, 4f, zc2 + RampWidth * 0.5f + 1.8f),
                                      new Vector3(RampRun + landingDepth + 2f, 8f, 3f), rock, mountain.transform);

        // Top landing where ramp 2 arrives.
        float h3 = h2 + rise;                                // 4.24 m
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Landing 3", new Vector3(x0 - landingDepth * 0.5f, h3 * 0.5f, zc2),
                                      new Vector3(landingDepth, h3, RampWidth + 0.4f), rock, mountain.transform);
        SceneBuilderUtil.MarkStaticRecursive(mountain);
        // The zone covers stairs, landings and both ramps: x from the stairs' left edge to Landing 2, z from the first step to Ramp 2.
        float mzMinX = -x0 - 0.2f, mzMaxX = x0 + RampRun + landingDepth + 0.2f;
        float mzMinZ = z0, mzMaxZ = zc2 + RampWidth * 0.5f + 0.2f;
        AddZone("Zone - Mountain Pass", "Mountain Pass", new Color(0.75f, 0.85f, 1f),
                new Vector3((mzMinX + mzMaxX) * 0.5f, 3.5f, (mzMinZ + mzMaxZ) * 0.5f),
                new Vector3(mzMaxX - mzMinX, 7f, mzMaxZ - mzMinZ), announcer, metrics);
        SceneBuilderUtil.AddLabel("Stairs Sign", "Stairs: " + StepRise + " m rise, " + StepTread + " m tread",
                                  new Vector3(-1.9f, 1.4f, z0 + 0.6f), 0.04f, Color.white)
                        .transform.rotation = Quaternion.Euler(0f, -35f, 0f);
        SceneBuilderUtil.AddLabel("Ramp Sign", "Ramp: " + RampSlope + " deg, run " + RampRun + " m, rise " + rise.ToString("F2") + " m\nhandrail " + RailHeight + " m",
                                  new Vector3(x0 + 1.5f, h1 + 2.2f, zc - 1.6f), 0.04f, Color.white)
                        .transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        // 3. Ancient Ruins: a 6 x 8 x 4 m room on the summit with a 1.0 x 2.1 m doorway -----------
        var ruins = SceneBuilderUtil.AddEmpty("Ancient Ruins (greybox)", Vector3.zero);
        const float wallT = 0.3f;
        float doorX = x0 - landingDepth - wallT * 0.5f;       // doorway wall, facing the top landing
        float roomCx = doorX - wallT * 0.5f - RoomDepth * 0.5f;
        float roomCz = zc2;
        // Solid plinth under the room so it does not float.
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Summit Plinth", new Vector3(roomCx, h3 * 0.5f, roomCz),
                                      new Vector3(RoomDepth + 2f * wallT, h3, RoomWidth + 2f * wallT), rock, ruins.transform);
        float wallCy = h3 + RoomHeight * 0.5f;
        // Doorway wall: two pieces and a lintel.
        float pieceW = (RoomWidth - DoorWidth) * 0.5f;
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Door Wall A", new Vector3(doorX, wallCy, roomCz - DoorWidth * 0.5f - pieceW * 0.5f),
                                      new Vector3(wallT, RoomHeight, pieceW), ruin, ruins.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Door Wall B", new Vector3(doorX, wallCy, roomCz + DoorWidth * 0.5f + pieceW * 0.5f),
                                      new Vector3(wallT, RoomHeight, pieceW), ruin, ruins.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Lintel", new Vector3(doorX, h3 + DoorHeight + (RoomHeight - DoorHeight) * 0.5f, roomCz),
                                      new Vector3(wallT, RoomHeight - DoorHeight, DoorWidth), ruin, ruins.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Back Wall", new Vector3(roomCx - RoomDepth * 0.5f - wallT * 0.5f, wallCy, roomCz),
                                      new Vector3(wallT, RoomHeight, RoomWidth + 2f * wallT), ruin, ruins.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Side Wall A", new Vector3(roomCx, wallCy, roomCz - RoomWidth * 0.5f - wallT * 0.5f),
                                      new Vector3(RoomDepth, RoomHeight, wallT), ruin, ruins.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Side Wall B", new Vector3(roomCx, wallCy, roomCz + RoomWidth * 0.5f + wallT * 0.5f),
                                      new Vector3(RoomDepth, RoomHeight, wallT), ruin, ruins.transform);
        // Broken pillars and the altar.
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Pillar", new Vector3(roomCx + 1.5f, h3 + 1.2f, roomCz - 1.8f), new Vector3(0.4f, 1.2f, 0.4f), ruin, ruins.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Pillar", new Vector3(roomCx - 1.5f, h3 + 1.8f, roomCz + 1.8f), new Vector3(0.4f, 1.8f, 0.4f), ruin, ruins.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Altar", new Vector3(roomCx - 2.5f, h3 + 0.5f, roomCz), new Vector3(1.6f, 1f, 0.8f), ruin, ruins.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Shard", new Vector3(roomCx - 2.5f, h3 + 1.3f, roomCz), new Vector3(0.15f, 0.3f, 0.15f), marker, ruins.transform)
                        .transform.rotation = Quaternion.Euler(45f, 0f, 45f);
        SceneBuilderUtil.AddPointLight("Ruins Light", new Vector3(roomCx, h3 + 3f, roomCz), new Color(1f, 0.8f, 0.55f), 10f, 1.5f, ruins.transform);
        SceneBuilderUtil.MarkStaticRecursive(ruins);
        AddZone("Zone - Ruins Doorway", "Ruins Doorway", new Color(1f, 0.9f, 0.6f),
                new Vector3(doorX, h3 + 1.25f, roomCz), new Vector3(0.6f, 2.5f, DoorWidth), announcer, metrics);
        AddZone("Zone - Ancient Ruins", "Ancient Ruins", new Color(1f, 0.75f, 0.45f),
                new Vector3(roomCx, h3 + RoomHeight * 0.5f, roomCz), new Vector3(RoomDepth, RoomHeight, RoomWidth), announcer, metrics);
        SceneBuilderUtil.AddLabel("Door Sign", "Doorway " + DoorWidth + " x " + DoorHeight + " m\nroom " + RoomWidth + " x " + RoomDepth + " x " + RoomHeight + " m",
                                  new Vector3(doorX + 0.6f, h3 + 2.6f, roomCz), 0.04f, Color.white)
                        .transform.rotation = Quaternion.Euler(0f, -90f, 0f);

        // Start sign -----------------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Greybox Elaria\nActivity 1.4\nWalk the route: forest -> stairs -> ramps -> ruins\nEnter prints metrics   Backspace resets",
                                  new Vector3(2.5f, 1.6f, 1.5f), 0.04f, Color.white)
                        .transform.rotation = Quaternion.Euler(0f, 30f, 0f);

        SceneBuilderUtil.SaveScene(scene, "Activity_1_4");
        Selection.activeGameObject = rig;
    }

    /// <summary>A zone = trigger BoxCollider + ZoneTrigger wired to the announcer and metrics, with a tinted light.</summary>
    static GameObject AddZone(string objectName, string zoneName, Color color, Vector3 center, Vector3 size,
                              ZoneAnnouncer announcer, PlayerMetrics metrics)
    {
        var go = SceneBuilderUtil.AddTriggerVolume(objectName, center, size);
        var zt = go.AddComponent<ZoneTrigger>();
        zt.zoneName = zoneName;
        zt.zoneColor = color;
        zt.announcer = announcer;
        zt.metrics = metrics;
        return go;
    }

    /// <summary>A handrail parallel to a ramp: a thin bar at RailHeight above the ramp surface along one edge.</summary>
    static void AddRail(Transform ramp, Vector3 localUp, float localX, float length, Material mat, Transform parent)
    {
        var railGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        railGo.name = "Handrail (" + RailHeight + " m)";
        railGo.transform.SetParent(parent, false);
        railGo.transform.rotation = ramp.rotation;
        railGo.transform.position = ramp.position + ramp.rotation * (localUp + new Vector3(localX, 0f, 0f));
        railGo.transform.localScale = new Vector3(0.05f, 0.05f, length);
        railGo.GetComponent<Renderer>().sharedMaterial = mat;
        // Two posts so the rail reads as a rail.
        for (int i = -1; i <= 1; i += 2)
        {
            var post = GameObject.CreatePrimitive(PrimitiveType.Cube);
            post.name = "Rail Post";
            post.transform.SetParent(parent, false);
            post.transform.rotation = ramp.rotation;
            post.transform.position = ramp.position + ramp.rotation * (new Vector3(localX, localUp.y * 0.5f, i * length * 0.4f));
            post.transform.localScale = new Vector3(0.05f, localUp.y, 0.05f);
            post.GetComponent<Renderer>().sharedMaterial = mat;
        }
    }
}

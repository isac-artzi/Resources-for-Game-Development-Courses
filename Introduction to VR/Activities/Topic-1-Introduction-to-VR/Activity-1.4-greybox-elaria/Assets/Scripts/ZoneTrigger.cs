// ZoneTrigger.cs — Activity 1.4: Greybox Elaria
// Marks a region of the greybox (Enchanted Forest, Mountain Pass, Ancient Ruins, the Ruins doorway) and notices
// when the player's HEAD enters or leaves it. It tests the head against its own BoxCollider every frame in local
// space, so it works no matter how the rig was moved (simulator keys, teleport, or a script) and does not depend
// on the physics engine seeing a Rigidbody move.
// Attached to: each Zone object in the starter scene (Announcer and Metrics are pre-set).
// Created by Isac Artzi

using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ZoneTrigger : MonoBehaviour
{
    [Header("Zone")]
    [Tooltip("Name shown by the announcer and used as the key in the metrics table.")]
    public string zoneName = "Zone";

    [Tooltip("Color for the announcement banner and the zone's light.")]
    public Color zoneColor = Color.white;

    [Header("Listeners")]
    [Tooltip("Shows the zone name when the player enters. Pre-set by the builder.")]
    public ZoneAnnouncer announcer;

    [Tooltip("Logs time and distance per zone. Pre-set by the builder.")]
    public PlayerMetrics metrics;

    /// <summary>True while the player's head is inside this volume.</summary>
    public bool HeadInside { get; private set; }

    BoxCollider box;
    Transform head;

    void Start()
    {
        box = GetComponent<BoxCollider>();
        head = Camera.main != null ? Camera.main.transform : null;
    }

    void Update()
    {
        if (box == null || head == null) return;

        // TODO 1: Is the head inside the box? Convert the head's WORLD position into this object's LOCAL space, then
        //         compare each axis with the box's center and half-size (BoxCollider.center / .size are local values):
        //             Vector3 local = transform.InverseTransformPoint(head.position);
        //             Vector3 half = box.size * 0.5f;
        //             Vector3 d = local - box.center;
        //             bool inside = Mathf.Abs(d.x) <= half.x && Mathf.Abs(d.y) <= half.y && Mathf.Abs(d.z) <= half.z;
        //         Why local space and not box.bounds.Contains: bounds are world-axis-aligned, so a rotated zone
        //         (the switchback ramps) would get a bloated, wrong box. InverseTransformPoint handles rotation and scale.
        //         Look at: Transform.InverseTransformPoint, BoxCollider.center, BoxCollider.size.
        bool inside = false;

        // TODO 2: Edge detection. Only act on the FRAME the state changes:
        //             if (inside && !HeadInside) { announcer?.Announce(zoneName, zoneColor); metrics?.EnterZone(zoneName); }
        //             if (!inside && HeadInside) { metrics?.ExitZone(zoneName); }
        //             HeadInside = inside;
        //         (?. is the null-conditional operator: it skips the call when the reference is null.)
        //         Why edges and not "while inside": announcing 72 times a second would be noise, and the metrics need
        //         one enter and one exit to compute time in zone.
        //         Check: walking the head into the Forest zone prints "[Metrics] enter Enchanted Forest" once, and the
        //         banner shows the zone name once; leaving prints the exit once.
        HeadInside = inside;   // placeholder that compiles; TODO 2 adds the enter/exit calls above it
    }

    /// <summary>Draws the zone as a translucent box in the Scene view so you can see and resize it while not playing.</summary>
    void OnDrawGizmos()
    {
        var b = GetComponent<BoxCollider>();
        if (b == null) return;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(zoneColor.r, zoneColor.g, zoneColor.b, 0.15f);
        Gizmos.DrawCube(b.center, b.size);
        Gizmos.color = new Color(zoneColor.r, zoneColor.g, zoneColor.b, 0.6f);
        Gizmos.DrawWireCube(b.center, b.size);
    }
}

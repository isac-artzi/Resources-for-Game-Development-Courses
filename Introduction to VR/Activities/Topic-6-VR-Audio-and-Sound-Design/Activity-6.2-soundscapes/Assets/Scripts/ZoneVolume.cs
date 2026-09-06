// ZoneVolume.cs — Activity 6.2: Soundscapes and Music Zones
// An invisible box that knows which environment it is (Forest, Mountain, Ruins) and tells the
// MusicZoneManager when the player's head enters or leaves it. It polls the head position against the
// box bounds every frame, which is simpler and more predictable than physics trigger messages for a rig
// that is moved by teleport and by the simulator.
// Attached to: Forest Zone, Mountain Zone, Ruins Zone (each has a BoxCollider set to Is Trigger).
// Created by Isac Artzi

using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ZoneVolume : MonoBehaviour
{
    /// <summary>The three environments of Realm of Legends. The integer values index arrays in the manager.</summary>
    public enum ZoneId { None = -1, Forest = 0, Mountain = 1, Ruins = 2 }

    [Header("Identity")]
    [Tooltip("Which environment this box represents.")]
    public ZoneId zone = ZoneId.Forest;

    [Header("Wiring")]
    [Tooltip("The manager to notify. Set by the builder; if empty the script looks for one at Start.")]
    public MusicZoneManager manager;

    [Tooltip("What counts as 'the player'. Leave empty to use the main camera (the head).")]
    public Transform head;

    [Header("Debug")]
    [Tooltip("Log enter/exit to the Console.")]
    public bool logTransitions = true;

    /// <summary>True while the head is inside this box.</summary>
    public bool HeadInside { get; private set; }

    BoxCollider box;

    void Start()
    {
        box = GetComponent<BoxCollider>();
        if (manager == null) manager = FindFirstObjectByType<MusicZoneManager>();
        if (head == null && Camera.main != null) head = Camera.main.transform;
    }

    void Update()
    {
        if (box == null || head == null || manager == null) return;

        // TODO 1: Is the head inside the box right now?
        //             bool nowInside = box.bounds.Contains(head.position);
        //         Look at: Collider.bounds (a world-space axis-aligned Bounds), Bounds.Contains(Vector3).
        //         Why bounds and not OnTriggerEnter: trigger messages need a Rigidbody on one side and fire only when
        //         physics notices a crossing; a teleport can skip straight over a thin box. Polling one point per frame
        //         is cheap and never misses. (Limitation: bounds are axis-aligned, so do not rotate the zone boxes.)
        //         Check: add a temporary Debug.Log(nowInside) and walk in and out of the Forest pad.

        // TODO 2: Fire on the edges only — once when entering, once when leaving.
        //             if (nowInside && !HeadInside) { HeadInside = true;  manager.EnterZone(zone); if (logTransitions) Debug.Log("[Zone] enter " + zone); }
        //             else if (!nowInside && HeadInside) { HeadInside = false; manager.ExitZone(zone); if (logTransitions) Debug.Log("[Zone] exit " + zone); }
        //         Why edge detection: EnterZone starts a crossfade; calling it every frame would restart the fade forever.
        //         Check: the Console shows exactly one "enter Forest" when you step onto the green pad and one
        //         "exit Forest" when you leave it, and the manager's readout changes zone.
    }

    void OnDrawGizmos()
    {
        var b = GetComponent<BoxCollider>();
        if (b == null) return;
        Gizmos.color = zone == ZoneId.Forest ? new Color(0.2f, 0.8f, 0.3f, 0.25f)
                     : zone == ZoneId.Mountain ? new Color(0.7f, 0.8f, 1f, 0.25f)
                     : new Color(0.9f, 0.7f, 0.3f, 0.25f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(b.center, b.size);
    }
}

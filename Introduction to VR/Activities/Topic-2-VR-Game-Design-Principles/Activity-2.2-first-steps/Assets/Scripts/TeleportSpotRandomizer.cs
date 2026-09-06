// TeleportSpotRandomizer.cs — Activity 2.2: First Steps: Teleport and Move
// Listens for teleports onto the waystones and scatters the OTHER waystones to new random spots on the trail, so the
// path up the pass is never the same twice. Also counts teleports for the label. The randomization uses the
// uniform-in-a-disk formula you will meet again when you scatter trees in Topic 3.
// Attached to: Waystones (the parent of Waystone 1-4). Anchors, rig and label are pre-wired.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class TeleportSpotRandomizer : MonoBehaviour
{
    [Header("Waystones")]
    [Tooltip("Every TeleportationAnchor that should re-randomize. The starter scene fills this with Waystone 1-4.")]
    public TeleportationAnchor[] anchors;

    [Header("Where they may land")]
    [Tooltip("Center of the disk in which waystones are scattered (world space, y is ignored).")]
    public Vector3 center = new Vector3(0f, 0f, 11f);

    [Tooltip("Radius of that disk in meters.")]
    public float radius = 7f;

    [Tooltip("Minimum distance between two waystones (meters).")]
    public float minSpacing = 3.5f;

    [Tooltip("Minimum distance from the player (meters) — a waystone under your feet is useless.")]
    public float keepAwayFromPlayer = 2.5f;

    [Tooltip("The XR Origin root, used for the keep-away test.")]
    public Transform rig;

    [Header("Feedback")]
    [Tooltip("Shows the teleport count. Leave empty to skip.")]
    public TextMesh label;

    public bool logEvents = true;

    /// <summary>How many teleports onto waystones have happened this session.</summary>
    public int TeleportCount { get; private set; }

    void OnEnable()
    {
        if (anchors == null) return;

        // TODO 1: Subscribe to each anchor's 'teleporting' UnityEvent:
        //             foreach (var a in anchors) if (a != null) a.teleporting.AddListener(OnTeleporting);
        //         Look at: BaseTeleportationInteractable.teleporting (UnityEvent<TeleportingEventArgs>). It fires when the
        //         teleport request is queued — i.e. the moment the player releases the stick on a valid target.
        //         OnDisable below already unsubscribes.
        //         Check: teleport onto a waystone -> Console prints "teleporting to Waystone 2".
    }

    void OnDisable()
    {
        if (anchors == null) return;
        foreach (var a in anchors) if (a != null) a.teleporting.RemoveListener(OnTeleporting);
    }

    /// <summary>Called by XRI as the player teleports onto one of the anchors.</summary>
    public void OnTeleporting(TeleportingEventArgs args)
    {
        var landingOn = args.interactableObject as TeleportationAnchor;
        if (logEvents && landingOn != null) Debug.Log("teleporting to " + landingOn.name);
        TeleportCount++;
        if (label != null) label.text = "Teleports: " + TeleportCount;

        // TODO 3: Scatter every anchor EXCEPT the one you are landing on:
        //             foreach (var a in anchors) if (a != null && a != landingOn) Relocate(a);
        //         Why not move the landing anchor: the teleport request already holds its position; moving it now would
        //         put you somewhere the arc never pointed — a guaranteed moment of nausea and confusion.
        //         Check: after each teleport the other three waystones jump to new places; the one under you stays.
    }

    /// <summary>
    /// A uniformly distributed random point inside a horizontal disk of the given radius around c.
    /// </summary>
    public static Vector3 RandomPointInDisk(Vector3 c, float r)
    {
        // TODO 2: Uniform in a disk is NOT "random radius, random angle" — that piles points near the center.
        //         Use r * sqrt(u) for the radius:
        //             float u = Random.value, v = Random.value;
        //             float rr = r * Mathf.Sqrt(u);
        //             float theta = v * 2f * Mathf.PI;
        //             return new Vector3(c.x + rr * Mathf.Cos(theta), c.y, c.z + rr * Mathf.Sin(theta));
        //         Look at: Random.value (0..1), Mathf.Sqrt, Mathf.Cos / Mathf.Sin (radians).
        //         Check: call it 1000 times in a quick test loop and count how many land within r/2 of the center —
        //         about 25% (area ratio 1/4), not 50%.
        return c;
    }

    /// <summary>Moves one anchor to a valid random spot: inside the disk, away from the player and from other anchors.</summary>
    void Relocate(TeleportationAnchor a)
    {
        // TODO 3 (continued): Try up to 20 candidates from RandomPointInDisk(center, radius); accept the first that is
        //         at least keepAwayFromPlayer from rig.position (horizontal distance) and at least minSpacing from every
        //         other anchor's position. Then:
        //             a.transform.position = new Vector3(p.x, 0f, p.z);
        //         and rotate the group to face the center so arrival orientation still makes sense:
        //             Vector3 fwd = center - p; fwd.y = 0f; if (fwd.sqrMagnitude > 0.01f) a.transform.rotation = Quaternion.LookRotation(fwd);
        //         If no candidate passes after 20 tries, leave the anchor where it is (log it).
        //         Look at: Vector3.Distance, Quaternion.LookRotation. Why 20 tries and not a while(true): a bad radius or
        //         spacing would otherwise freeze Unity.
        //         Check: waystones never overlap each other or land on top of you.
    }
}

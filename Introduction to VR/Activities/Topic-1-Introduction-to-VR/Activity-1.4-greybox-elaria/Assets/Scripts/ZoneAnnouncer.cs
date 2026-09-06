// ZoneAnnouncer.cs — Activity 1.4: Greybox Elaria
// Shows a short banner ("Enchanted Forest") in front of the player when they enter a zone, holds it for a moment,
// then fades it out. The banner is placed in the WORLD at the moment of entry, not glued to the head, so the
// player can glance at it and look away — the same rule as the station label in Activity 1.2.
// Attached to: "Zone Announcer" in the starter scene (Banner is pre-set; Chime is optional).
// Created by Isac Artzi

using UnityEngine;

public class ZoneAnnouncer : MonoBehaviour
{
    [Header("Banner")]
    [Tooltip("The TextMesh used as the banner.")]
    public TextMesh banner;

    [Tooltip("Distance in front of the eyes where the banner appears, in meters (medium zone).")]
    public float bannerDistance = 2f;

    [Tooltip("Vertical offset from eye level, in meters. Slightly below eye level reads as 'in the world', not 'in your face'.")]
    public float bannerDrop = 0.3f;

    [Header("Timing")]
    [Tooltip("Seconds the banner stays fully visible.")]
    public float holdSeconds = 2f;

    [Tooltip("Seconds the fade-out takes after the hold.")]
    public float fadeSeconds = 1f;

    [Header("Sound (optional)")]
    [Tooltip("An AudioSource with a short chime. Leave empty for silence.")]
    public AudioSource chime;

    Transform head;
    float timer = float.MaxValue;   // seconds since the last announcement
    Color baseColor = Color.white;

    void Start()
    {
        head = Camera.main != null ? Camera.main.transform : null;
        if (banner != null) banner.text = "";
    }

    /// <summary>Shows the zone name in front of the player and starts the hold-then-fade timer.</summary>
    public void Announce(string zoneName, Color color)
    {
        if (banner == null || head == null) return;

        // TODO 1: Place the banner. Flatten the head's forward, step out bannerDistance, drop by bannerDrop, and face
        //         the banner toward the head (TextMesh reads from its -z side, so its forward points AWAY from the head):
        //             Vector3 fwd = head.forward; fwd.y = 0f; fwd.Normalize();
        //             banner.transform.position = head.position + fwd * bannerDistance + Vector3.down * bannerDrop;
        //             banner.transform.rotation = Quaternion.LookRotation(fwd);
        //         Check: entering a zone drops a readable sign 2 m ahead of you; keep walking and you pass it.

        // TODO 2: Fill it. banner.text = zoneName; baseColor = color; banner.color = color (full alpha); timer = 0f;
        //         if (chime != null) chime.Play();
        //         Check: the banner shows the zone's name in the zone's color.
        Debug.Log("[Announcer] " + zoneName + " (implement TODO 1-3 in ZoneAnnouncer.cs)");
    }

    void Update()
    {
        if (banner == null) return;
        timer += Time.deltaTime;

        // TODO 3: Hold, then fade. Alpha stays 1 for holdSeconds, then eases to 0 over fadeSeconds:
        //             float a;
        //             if (timer <= holdSeconds) a = 1f;
        //             else a = 1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((timer - holdSeconds) / Mathf.Max(0.01f, fadeSeconds)));
        //             banner.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
        //         Look at: Mathf.SmoothStep (from 1.3), Mathf.Clamp01. TextMesh.color has no separate alpha setter, so build a new Color.
        //         Check: the banner is solid for 2 s, then fades out over 1 s; set Hold Seconds to 0 and it starts fading at once.
    }
}

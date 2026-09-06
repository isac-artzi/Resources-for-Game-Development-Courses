// RestPoint.cs — Activity 5.4: Comfort Settings and the Rest Point
// A place to stop. When the player's head comes within a few meters of the bench, the lantern warms and
// brightens, the ambient wind drops to a hush, and the Guide's invitation appears. The script also counts how
// long the player rested, which ComfortLog records — rest is part of a comfort test, not a failure of one.
// Uses a distance test on the camera rather than a trigger collider so it works with any rig, colliders or not.
// Attached to: Rest Point (an empty next to the bench) in the starter scene; light, audio, and label pre-assigned.
// Created by Isac Artzi

using UnityEngine;

public class RestPoint : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("The player's head. Found via Camera.main if empty.")]
    public Transform head;

    [Tooltip("The lantern light by the bench.")]
    public Light lantern;

    [Tooltip("Looping ambience (wind). If it has no clip, a soft noise loop is generated at startup.")]
    public AudioSource ambience;

    [Tooltip("The Guide's invitation, shown while resting.")]
    public TextMesh inviteLabel;

    [Header("Zone")]
    [Tooltip("Horizontal distance (m) from this object within which the player counts as resting.")]
    public float restRadius = 2.0f;

    [Header("Resting look and sound")]
    public float lanternIdleIntensity = 0.8f;
    public float lanternRestIntensity = 2.5f;
    public Color lanternIdleColor = new Color(1f, 0.85f, 0.6f);
    public Color lanternRestColor = new Color(1f, 0.75f, 0.45f);
    [Range(0f, 1f)] public float ambienceIdleVolume = 0.5f;
    [Range(0f, 1f)] public float ambienceRestVolume = 0.12f;
    [Tooltip("Seconds for the light and sound to ease between states.")]
    public float easeSeconds = 1.5f;

    [Header("Text")]
    [TextArea] public string inviteText = "Rest here as long as you need.\nThe path will wait.";

    [Header("Debug (read-only)")]
    public bool isResting;
    public float totalRestSeconds;

    // 0 = fully idle, 1 = fully resting; eased toward the target every frame.
    float restBlend;

    void Start()
    {
        if (head == null && Camera.main != null) head = Camera.main.transform;
        if (inviteLabel != null) inviteLabel.text = "";

        if (ambience != null && ambience.clip == null)
        {
            ambience.clip = MakeWindLoop(2f);
            ambience.loop = true;
            ambience.volume = ambienceIdleVolume;
            ambience.Play();
        }
    }

    /// <summary>A soft, loopable wind-like noise: random samples smoothed so the hiss is low and gentle.</summary>
    public static AudioClip MakeWindLoop(float seconds)
    {
        int rate = 22050;
        int count = Mathf.Max(1, Mathf.RoundToInt(rate * seconds));
        float[] data = new float[count];

        // TODO 1: Fill data with smoothed noise:
        //             float last = 0f;
        //             for (int i = 0; i < count; i++) { float white = Random.Range(-1f, 1f); last = Mathf.Lerp(last, white, 0.02f); data[i] = last * 2f; }
        //         (Lerp with a small factor is a one-pole low-pass: it turns harsh white noise into a soft rumble. The * 2 restores volume.)
        //         Then: var clip = AudioClip.Create("Wind", count, 1, rate, false); clip.SetData(data, 0); return clip;
        //         Look at: Random.Range(float, float), Mathf.Lerp as a filter, AudioClip.Create / SetData.
        //         Check: at Play you hear a low hiss; walk to the bench and it fades to a hush.
        var placeholder = AudioClip.Create("Wind", count, 1, rate, false);
        placeholder.SetData(data, 0);   // silence until TODO 1 is done
        return placeholder;
    }

    void Update()
    {
        if (head == null) return;

        // TODO 2: Decide whether the player is resting. Horizontal distance only (drop y — a tall player is not "farther away"):
        //             Vector3 a = head.position; a.y = 0f;  Vector3 b = transform.position; b.y = 0f;
        //             isResting = Vector3.Distance(a, b) <= restRadius;
        //         If resting, totalRestSeconds += Time.deltaTime.
        //         Check: the Is Resting box in the Inspector ticks when you walk up to the bench and clears when you leave.

        // TODO 3: Ease the blend and apply it:
        //             float target = isResting ? 1f : 0f;
        //             restBlend = Mathf.MoveTowards(restBlend, target, Time.deltaTime / Mathf.Max(0.01f, easeSeconds));
        //             lantern.intensity = Mathf.Lerp(lanternIdleIntensity, lanternRestIntensity, restBlend);
        //             lantern.color = Color.Lerp(lanternIdleColor, lanternRestColor, restBlend);
        //             ambience.volume = Mathf.Lerp(ambienceIdleVolume, ambienceRestVolume, restBlend);   (null-check light and audio)
        //         Why ease over 1.5 s: arriving at a rest point should feel like exhaling, not like tripping a switch.
        //         Check: the lantern warms up over about a second and a half as you arrive; the wind quiets at the same pace.

        // TODO 4: The invitation. inviteLabel.text = isResting ? inviteText : ""; and make the label face the head
        //         (Quaternion.LookRotation(labelPos - head.position) with the y of that direction zeroed, as in 1.1).
        //         Check: the Guide's words appear when you sit down at the bench and vanish when you walk away.
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.8f, 0.4f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, restRadius);
    }
}

// AudioBeacon.cs — Activity 6.1: Hear the Forest
// A hidden collectible that "calls" to the player: its volume swells as the player's head turns toward it,
// it pulses so it reads as alive, and a small light breathes in step with the sound. In Realm of Legends
// this is how the Guide leads a hero to a shard they cannot yet see.
// Attached to: Lost Shard (Beacon) in the starter scene.
// Created by Isac Artzi

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioBeacon : MonoBehaviour
{
    [Header("Placeholder sound (used only while Clip is empty)")]
    [Tooltip("Frequency of the placeholder tone. A high, clear tone is easiest to localize.")]
    public float placeholderHz = 660f;

    [Header("Facing")]
    [Tooltip("Whose facing direction counts. Leave empty to use the main camera (the player's head).")]
    public Transform listener;

    [Tooltip("Volume when the beacon is directly behind the player.")]
    [Range(0f, 1f)]
    public float minVolume = 0.15f;

    [Tooltip("Volume when the player looks straight at the beacon.")]
    [Range(0f, 1f)]
    public float maxVolume = 1f;

    [Tooltip("Maps 'how much you face it' (0 = behind, 1 = straight ahead) to a 0..1 blend between min and max volume.")]
    public AnimationCurve facingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Pulse")]
    [Tooltip("How many volume pulses per second. 1–2 reads as a heartbeat; 4+ reads as an alarm.")]
    public float pulsesPerSecond = 1.5f;

    [Tooltip("How deep the pulse dips: 0 = steady, 1 = all the way to silence at the trough.")]
    [Range(0f, 1f)]
    public float pulseDepth = 0.6f;

    [Tooltip("Optional light that breathes with the pulse. Set by the builder.")]
    public Light glow;

    [Tooltip("Light intensity at the peak of a pulse.")]
    public float glowPeakIntensity = 2f;

    [Header("Readout (optional)")]
    [Tooltip("A TextMesh that shows the off-axis angle and the current volume. Set by the builder.")]
    public TextMesh readout;

    AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
        ToneFactory.EnsureClip(source, placeholderHz);
        source.loop = true;
        if (!source.isPlaying) source.Play();
        if (listener == null && Camera.main != null) listener = Camera.main.transform;
    }

    void Update()
    {
        if (source == null || listener == null) return;

        // TODO 1: How much is the player facing the beacon?
        //             Vector3 toBeacon = (transform.position - listener.position).normalized;
        //             float facing = Vector3.Dot(listener.forward, toBeacon);   // 1 = straight ahead, 0 = beside, -1 = behind
        //         Look at: Vector3.Dot, Transform.forward. Why the dot product: for unit vectors it equals cos(angle),
        //         so it turns "the angle between where I look and where the sound is" into one number without trig.
        //         Check: print 'facing' with Debug.Log for a moment — it rises toward 1 as you turn to face the shard.

        // TODO 2: Map facing to a target volume.
        //             float f01 = Mathf.InverseLerp(-1f, 1f, facing);           // -1..1 -> 0..1
        //             float target = Mathf.Lerp(minVolume, maxVolume, facingCurve.Evaluate(f01));
        //         Look at: Mathf.InverseLerp, AnimationCurve.Evaluate. Why a curve: an ease-in-out keeps the beacon
        //         quiet until you are roughly pointed at it, which makes the "found it" moment clearer.
        //         Check: turn slowly with the mouse; the tone swells as the shard comes toward the center of your view.

        // TODO 3: Add the pulse and apply it.
        //             float wave = 0.5f * (1f + Mathf.Sin(2f * Mathf.PI * pulsesPerSecond * Time.time));  // 0..1
        //             float pulse = 1f - pulseDepth * wave;                                                  // 1 -> 1 - depth
        //             source.volume = target * pulse;
        //             if (glow != null) glow.intensity = glowPeakIntensity * pulse;
        //         Check: the tone throbs about 1.5 times per second and the light breathes with it. Set Pulse Depth
        //         to 0 and both go steady.

        // TODO 4: Show the numbers on the readout.
        //             float angle = Mathf.Acos(Mathf.Clamp(facing, -1f, 1f)) * Mathf.Rad2Deg;
        //             readout.text = "Beacon: " + angle.ToString("F0") + " deg off-axis\nvolume " + source.volume.ToString("F2");
        //         Look at: Mathf.Acos (radians), Mathf.Rad2Deg. Why Clamp: floating-point error can produce 1.0000001,
        //         and Acos of that is NaN.
        //         Check: facing the shard reads 0–5 deg; turning your back reads about 180 deg.
    }
}

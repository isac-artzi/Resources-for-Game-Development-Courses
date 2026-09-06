// RolloffVisualizer.cs — Activity 6.1: Hear the Forest
// Draws two rings around a 3D AudioSource — one at Min Distance, one at Max Distance — and prints the
// attenuation the listener should be hearing right now (as a gain 0..1 and in decibels). It also gives
// the source a placeholder clip so the scene is audible with zero downloads.
// Attached to: Stream, Bird, and Fire in the starter scene.
// Created by Isac Artzi

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RolloffVisualizer : MonoBehaviour
{
    [Header("Placeholder sound (used only while Clip is empty)")]
    [Tooltip("Frequency of the placeholder sine tone. Give each source a different pitch so you can tell them apart.")]
    public float placeholderHz = 440f;

    [Tooltip("Use filtered noise instead of a tone — right for water, wind and fire.")]
    public bool placeholderIsNoise = false;

    [Header("Rings")]
    [Tooltip("Points per ring. 48 looks round; 12 looks like a gem.")]
    [Range(8, 128)]
    public int segments = 48;

    [Tooltip("Height of the rings above the source's pivot, in meters.")]
    public float ringHeight = 0.05f;

    [Tooltip("Line width in meters.")]
    public float lineWidth = 0.02f;

    [Tooltip("Material for the Min Distance ring (set by the builder). Leave empty to use a default unlit material.")]
    public Material minRingMaterial;

    [Tooltip("Material for the Max Distance ring (set by the builder).")]
    public Material maxRingMaterial;

    [Header("Readout (optional)")]
    [Tooltip("A TextMesh above the source that shows distance, gain and dB. Set by the builder.")]
    public TextMesh readout;

    AudioSource source;
    LineRenderer minRing, maxRing;
    Transform listener;

    void Start()
    {
        source = GetComponent<AudioSource>();
        if (placeholderIsNoise) ToneFactory.EnsureNoise(source);
        else ToneFactory.EnsureClip(source, placeholderHz);
        source.loop = true;
        if (!source.isPlaying) source.Play();

        minRing = MakeRing("Min Distance Ring", minRingMaterial, new Color(1f, 0.85f, 0.3f));
        maxRing = MakeRing("Max Distance Ring", maxRingMaterial, new Color(0.3f, 0.8f, 1f));

        var l = FindFirstObjectByType<AudioListener>();
        if (l != null) listener = l.transform;
    }

    LineRenderer MakeRing(string ringName, Material material, Color fallbackColor)
    {
        var go = new GameObject(ringName);
        go.transform.SetParent(transform, false);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;          // the source may be a scaled primitive; world space keeps the ring round
        lr.loop = true;
        lr.widthMultiplier = lineWidth;
        lr.positionCount = segments;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        if (material != null)
        {
            lr.sharedMaterial = material;
        }
        else
        {
            var shader = Shader.Find("Sprites/Default");
            if (shader != null) lr.material = new Material(shader);
            lr.startColor = fallbackColor;
            lr.endColor = fallbackColor;
        }
        return lr;
    }

    void Update()
    {
        if (source == null) return;

        // TODO 1: Redraw both rings every frame so they follow Inspector changes while you play:
        //             DrawRing(minRing, source.minDistance);
        //             DrawRing(maxRing, source.maxDistance);
        //         Look at: AudioSource.minDistance / AudioSource.maxDistance (the two handles on the 3D Sound
        //         Settings curve in the Inspector).
        //         Check: two rings appear around each source; dragging Min Distance in the Inspector during Play
        //         moves the inner ring.

        // TODO 3: Compute what the listener should hear and print it (needs LogarithmicGain below):
        //             if (listener == null || readout == null) return;
        //             float d = Vector3.Distance(listener.position, transform.position);
        //             float gain = LogarithmicGain(d, source.minDistance);
        //             if (d > source.maxDistance) gain = LogarithmicGain(source.maxDistance, source.minDistance);
        //             readout.text = name + "\n" + d.ToString("F1") + " m   gain " + gain.ToString("F2") +
        //                            "   " + ToDecibels(gain).ToString("F1") + " dB";
        //         Why clamp at maxDistance: with Logarithmic rolloff Unity stops attenuating beyond Max Distance
        //         (the curve goes flat), it does not go silent.
        //         Check: standing on the inner ring you read gain 1.00 / 0.0 dB; at twice Min Distance you read
        //         0.50 / -6.0 dB; at four times, 0.25 / -12.0 dB.
    }

    /// <summary>Writes 'segments' points on a horizontal circle of the given radius around this source.</summary>
    void DrawRing(LineRenderer ring, float radius)
    {
        if (ring == null) return;
        if (ring.positionCount != segments) ring.positionCount = segments;

        // TODO 2: For i in 0..segments-1: angle = i / (float)segments * 2π;
        //             Vector3 p = transform.position + new Vector3(Mathf.Cos(angle) * radius, ringHeight, Mathf.Sin(angle) * radius);
        //             ring.SetPosition(i, p);
        //         Look at: LineRenderer.SetPosition, Mathf.Cos / Mathf.Sin (radians!).
        //         Why world space: the Stream is a box scaled 6 x 0.2 x 1.2 — a local-space ring would be an ellipse.
        //         Check: the ring is a circle whose radius matches the Min/Max Distance values in the Inspector
        //         (Scene view: select the source and compare with the blue sphere gizmo Unity draws for Max Distance).
    }

    /// <summary>
    /// Unity's Logarithmic rolloff: gain = minDistance / distance, clamped to 1 inside minDistance.
    /// </summary>
    public static float LogarithmicGain(float distance, float minDistance)
    {
        // TODO 3 (continued): return minDistance / Mathf.Max(distance, minDistance);
        //         Guard: if minDistance <= 0 return 1f (avoid dividing by zero).
        //         Check with a calculator: LogarithmicGain(4f, 2f) = 0.5; LogarithmicGain(1f, 2f) = 1.0.
        return 1f;
    }

    /// <summary>Converts a linear amplitude gain (0..1) to decibels relative to full scale: dB = 20 log10(gain).</summary>
    public static float ToDecibels(float gain)
    {
        // TODO 4: return 20f * Mathf.Log10(Mathf.Max(gain, 0.00001f));
        //         Look at: Mathf.Log10. Why the Max: log10(0) is -infinity, which prints as "-Infinity".
        //         Check: ToDecibels(0.5f) is about -6.0; ToDecibels(0.1f) is -20.0; ToDecibels(1f) is 0.
        return 0f;
    }
}

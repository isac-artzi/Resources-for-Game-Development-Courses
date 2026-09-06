// OcclusionFilter.cs — Activity 6.4: Dark Mode
// Makes a looping 3D beacon sound muffled when a wall stands between it and the listener: a Linecast from the
// source to the head decides "occluded or not", and an AudioLowPassFilter's cutoff frequency (plus a small
// volume dip) glides between open and muffled values. Also gives the beacon a placeholder tone.
// Attached to: Artifact Shard (objective) and both Void Pits (hazards) in the starter scene.
// Created by Isac Artzi

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
public class OcclusionFilter : MonoBehaviour
{
    [Header("Beacon sound (placeholder while Clip is empty)")]
    [Tooltip("Frequency of the placeholder loop. High and clear for the objective; low and rough for hazards.")]
    public float placeholderHz = 440f;

    [Header("Occlusion test")]
    [Tooltip("Which layers can block sound.")]
    public LayerMask occluderLayers = ~0;

    [Tooltip("Colliders under this transform never count as occluders (set to the XR rig so the player's body does not muffle everything).")]
    public Transform ignoreRoot;

    [Tooltip("Seconds between Linecasts. 0.1 is plenty; the smoothing below hides the steps.")]
    public float checkInterval = 0.1f;

    [Header("Filter values")]
    [Tooltip("Low-pass cutoff (Hz) when nothing is in the way. 22000 = effectively off.")]
    public float openCutoffHz = 22000f;

    [Tooltip("Low-pass cutoff (Hz) when a wall is in the way. 300–800 sounds like 'through a door'.")]
    public float occludedCutoffHz = 600f;

    [Tooltip("Volume multiplier while occluded (walls also absorb energy).")]
    [Range(0.1f, 1f)]
    public float occludedVolumeScale = 0.6f;

    [Tooltip("Time constant of the glide between open and occluded, in seconds.")]
    public float smoothingSeconds = 0.15f;

    [Header("Debug")]
    [Tooltip("Draw the line to the listener: green = clear, red = blocked.")]
    public bool drawDebugLine = true;

    /// <summary>True when the last Linecast hit something between source and listener.</summary>
    public bool IsOccluded { get; private set; }

    /// <summary>The source's volume as set in the Inspector, before any occlusion dip.</summary>
    public float BaseVolume { get { return baseVolume; } }

    /// <summary>Seconds until the next Linecast (negative = due now).</summary>
    public float SecondsToNextCheck { get { return nextCheck - Time.time; } }

    AudioSource source;
    AudioLowPassFilter lowPass;
    Transform listener;
    float baseVolume;
    float nextCheck;

    void Start()
    {
        source = GetComponent<AudioSource>();
        lowPass = GetComponent<AudioLowPassFilter>();
        if (source.clip == null) source.clip = SonarPing.MakeTone(placeholderHz, 1f, false);
        source.loop = true;
        if (!source.isPlaying) source.Play();
        baseVolume = source.volume;
        lowPass.cutoffFrequency = openCutoffHz;   // Unity's default is 5000 Hz, which muffles everything a little

        var l = FindFirstObjectByType<AudioListener>();
        if (l != null) listener = l.transform;
    }

    void Update()
    {
        if (source == null || lowPass == null || listener == null) return;

        // TODO 1: Re-test occlusion a few times per second, not every frame.
        //             if (Time.time >= nextCheck) { nextCheck = Time.time + Mathf.Max(0.02f, checkInterval); IsOccluded = CheckOccluded(); }
        //         Why: a Linecast is cheap, but three beacons x 72 Hz adds up on Quest, and the ear cannot follow faster changes anyway.

        // TODO 3: Glide the cutoff toward its target with exponential smoothing (frame-rate independent).
        //             float targetCutoff = IsOccluded ? occludedCutoffHz : openCutoffHz;
        //             float k = 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.01f, smoothingSeconds));
        //             lowPass.cutoffFrequency = Mathf.Lerp(lowPass.cutoffFrequency, targetCutoff, k);
        //         Look at: AudioLowPassFilter.cutoffFrequency (10..22000 Hz). Why smooth: a hard switch clicks and sounds like
        //         a bug; 150 ms feels like walking past a doorway.
        //         Check: walk behind a wall while the shard hums — it dulls over a fraction of a second; step out, it opens up.

        // TODO 4: Dip the volume too, on the same glide.
        //             float targetVolume = baseVolume * (IsOccluded ? occludedVolumeScale : 1f);
        //             source.volume = Mathf.Lerp(source.volume, targetVolume, k);
        //         Check: the muffled beacon is also quieter, and the Inspector volume slider moves smoothly, not in steps.
    }

    /// <summary>Is there a collider between this source and the listener that is not part of the player?</summary>
    bool CheckOccluded()
    {
        // TODO 2: Linecast from the source to the listener.
        //             RaycastHit hit;
        //             bool blocked = false;
        //             if (Physics.Linecast(transform.position, listener.position, out hit, occluderLayers, QueryTriggerInteraction.Ignore))
        //             {
        //                 blocked = !(ignoreRoot != null && hit.transform.IsChildOf(ignoreRoot));   // the player's own collider does not count
        //             }
        //             if (drawDebugLine) Debug.DrawLine(transform.position, listener.position, blocked ? Color.red : Color.green);
        //             return blocked;
        //         Look at: Physics.Linecast(Vector3 start, Vector3 end, out RaycastHit, int layerMask, QueryTriggerInteraction).
        //         Why the ignoreRoot test: the line ends INSIDE the rig's CharacterController capsule, so with no wall in between
        //         the first hit is the player — which must read as "clear". A wall is closer to the source than the player, so
        //         when one exists it is hit first.
        //         Check: in the Scene view the line from the shard to your head is green in the open and red behind a wall.
        return false;
    }
}

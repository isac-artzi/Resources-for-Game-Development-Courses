// SnapTurnFeedback.cs — Activity 5.2: Smooth Move and Turn
// Plays a soft click each time the SnapTurnProvider rotates the rig, counts the turns, and reports the head's
// current heading. The click is generated in code (a short decaying sine) so this activity needs no audio files.
// Why a click: a snap turn is a discontinuity the eyes cannot smooth over; a matching sound tells the brain
// "that was a deliberate step" and makes the jump read as intentional rather than as a glitch.
// Attached to: XR Origin (XR Rig) in the starter scene, next to SpeedRamp.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

[RequireComponent(typeof(AudioSource))]
public class SnapTurnFeedback : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("The snap turn provider. Found on this GameObject's children if empty.")]
    public SnapTurnProvider snapTurnProvider;

    [Tooltip("The head (Main Camera transform) used to report heading. Found via Camera.main if empty.")]
    public Transform head;

    [Tooltip("Optional TextMesh that shows turn count and heading.")]
    public TextMesh readout;

    [Header("Click sound")]
    [Tooltip("Frequency of the generated click in Hz. 800–1500 reads as a soft tick.")]
    public float clickFrequency = 1200f;

    [Tooltip("Length of the click in seconds. 0.03–0.06 is a tick; longer becomes a beep.")]
    public float clickSeconds = 0.04f;

    [Range(0f, 1f)] public float clickVolume = 0.5f;

    [Header("Debug (read-only)")]
    public int turnCount;

    AudioSource source;
    AudioClip click;

    void Start()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;   // a UI-like sound: no 3D position, plays "in the head"
        if (head == null && Camera.main != null) head = Camera.main.transform;
        if (snapTurnProvider == null) snapTurnProvider = GetComponentInChildren<SnapTurnProvider>(true);

        click = MakeClick(clickFrequency, clickSeconds);

        // TODO 2: Subscribe: snapTurnProvider.locomotionStarted += OnSnapTurn; (null-check). Unsubscribe in OnDestroy.
        //         The snap turn provider raises 'started' once per snap (and 'ended' the same frame), so 'started' is the click.
        //         Look at: LocomotionProvider.locomotionStarted (Action<LocomotionProvider>).
        //         Check: Debug.Log in OnSnapTurn prints once per snap and never while you merely push the move stick.
    }

    void OnDestroy()
    {
        // TODO 2 (continued): snapTurnProvider.locomotionStarted -= OnSnapTurn; (if not null)
    }

    /// <summary>
    /// Builds a short click: a sine wave at the given frequency whose amplitude decays to zero over the duration.
    /// </summary>
    public static AudioClip MakeClick(float frequency, float seconds)
    {
        int sampleRate = 44100;
        int count = Mathf.Max(1, Mathf.RoundToInt(sampleRate * seconds));
        float[] data = new float[count];

        // TODO 1: Fill data[i] for i in 0..count-1:
        //             float t = (float)i / sampleRate;                       // time of this sample in seconds
        //             float envelope = 1f - (float)i / count;                // linear fade from 1 to 0
        //             data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * envelope;   // squared = faster decay
        //         Then: var clip = AudioClip.Create("Click", count, 1, sampleRate, false); clip.SetData(data, 0); return clip;
        //         Look at: AudioClip.Create(name, lengthSamples, channels, frequency, stream), AudioClip.SetData.
        //         Why square the envelope: a linear fade leaves an audible "tail"; squaring it makes the click crisp.
        //         Check: select XR Origin in Play mode and press the Preview button on the AudioSource — you hear a tick.
        var placeholder = AudioClip.Create("Click", count, 1, sampleRate, false);
        placeholder.SetData(data, 0);   // silence until TODO 1 is done
        return placeholder;
    }

    void OnSnapTurn(LocomotionProvider p)
    {
        // TODO 3: turnCount++; if (source != null && click != null) source.PlayOneShot(click, clickVolume);
        //         Look at: AudioSource.PlayOneShot(AudioClip, float volumeScale) — lets clicks overlap instead of cutting each other off.
        //         Check: rapid snaps each produce a tick; the Turn Count field in the Inspector climbs.
    }

    void Update()
    {
        if (readout == null) return;

        // TODO 4: Heading. The head's yaw is head.eulerAngles.y (0–360, clockwise from +z). Show it and the turn count:
        //             readout.text = "snaps " + turnCount + "\nheading " + Mathf.Round(head.eulerAngles.y) + " deg";
        //         Bonus check for the math section: after N snaps at 45° the heading should have changed by N*45 (mod 360),
        //         assuming you did not also turn your head with the mouse.
        //         Check: four 90° snaps bring the heading back to where it started.
        readout.text = "snaps " + turnCount;
    }
}

// SfxPool.cs — Activity 6.3: The Sound of Touch
// A small pool of reusable 3D AudioSources for one-shot effects. Anything in the scene calls
// SfxPool.Instance.Play(clip, position, volume, pitch) instead of creating its own AudioSource per sound,
// so a hundred impacts a minute cost eight AudioSources instead of a hundred GameObjects.
// Also supplies a synthesized "tick" clip so the scene is testable before you download any SFX.
// Attached to: SFX Pool in the starter scene.
// Created by Isac Artzi

using UnityEngine;

public class SfxPool : MonoBehaviour
{
    /// <summary>The one pool in the scene (set in Awake).</summary>
    public static SfxPool Instance { get; private set; }

    [Header("Pool")]
    [Tooltip("Number of AudioSources to create. Each can play one sound at a time. 8 is plenty for one player's hands.")]
    [Range(1, 32)]
    public int poolSize = 8;

    [Tooltip("Min Distance for every voice (full volume inside this radius).")]
    public float minDistance = 0.5f;

    [Tooltip("Max Distance for every voice (attenuation flattens beyond it).")]
    public float maxDistance = 20f;

    [Header("Placeholder (used when a caller passes a null clip)")]
    [Tooltip("Frequency of the synthesized tick.")]
    public float placeholderHz = 700f;

    [Tooltip("Length of the synthesized tick in seconds. Short = a click; 0.3 = a soft 'bonk'.")]
    public float placeholderSeconds = 0.12f;

    [Header("Readout (optional)")]
    [Tooltip("Shows how many voices are busy and how many requests were dropped or stolen. Set by the builder.")]
    public TextMesh readout;

    /// <summary>Total Play calls since start.</summary>
    public int Requests { get; private set; }

    /// <summary>How many times every voice was busy and the oldest was stolen.</summary>
    public int Steals { get; private set; }

    AudioSource[] voices;
    int nextVoice;
    AudioClip placeholder;

    void Awake()
    {
        Instance = this;

        // TODO 1: Build the pool.
        //             voices = new AudioSource[Mathf.Max(1, poolSize)];
        //             for (int i = 0; i < voices.Length; i++)
        //             {
        //                 var go = new GameObject("Voice " + (i + 1));
        //                 go.transform.SetParent(transform, false);
        //                 var src = go.AddComponent<AudioSource>();
        //                 src.playOnAwake = false;
        //                 src.loop = false;
        //                 src.spatialBlend = 1f;
        //                 src.rolloffMode = AudioRolloffMode.Logarithmic;
        //                 src.minDistance = minDistance;
        //                 src.maxDistance = maxDistance;
        //                 voices[i] = src;
        //             }
        //         Look at: GameObject.AddComponent<AudioSource>(), the AudioSource 3D settings you met in 6.1.
        //         Why a pool: creating and destroying GameObjects per sound allocates memory and stutters on Quest;
        //         a fixed set of voices is predictable.
        //         Check: in Play mode the SFX Pool object has 8 children named Voice 1..8 in the Hierarchy.
    }

    /// <summary>
    /// Plays a one-shot at a world position. Passing a null clip plays the placeholder tick.
    /// Returns the voice used (or null when falling back to PlayClipAtPoint).
    /// </summary>
    public AudioSource Play(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        Requests++;
        if (clip == null) clip = Placeholder();
        volume = Mathf.Clamp01(volume);
        pitch = Mathf.Clamp(pitch, 0.25f, 3f);

        if (voices == null || voices.Length == 0)
        {
            // Naive fallback until TODO 1 exists: Unity spawns a temporary "One shot audio" GameObject per call.
            // Watch the Hierarchy fill up while you throw things — that is what the pool prevents.
            AudioSource.PlayClipAtPoint(clip, position, volume);
            return null;
        }

        AudioSource voice = null;

        // TODO 2: Choose a voice. Prefer one that is idle; if all are busy, steal the next one in round-robin
        //         order (the one that started longest ago) and count the steal.
        //             for (int i = 0; i < voices.Length; i++)
        //             {
        //                 int idx = (nextVoice + i) % voices.Length;
        //                 if (!voices[idx].isPlaying) { voice = voices[idx]; nextVoice = (idx + 1) % voices.Length; break; }
        //             }
        //             if (voice == null) { voice = voices[nextVoice]; nextVoice = (nextVoice + 1) % voices.Length; Steals++; }
        //         Look at: AudioSource.isPlaying. Why steal rather than drop: a missing impact sound is more noticeable
        //         than a cut-off tail on the oldest one.
        //         Check: drop all three props at once — three voices light up (isPlaying) in the Inspector; the readout's
        //         "steals" stays 0 until you exceed the pool size.

        // TODO 3: Configure and fire the voice.
        //             voice.transform.position = position;
        //             voice.pitch = pitch;
        //             voice.Stop();                        // in case we stole it
        //             voice.PlayOneShot(clip, volume);
        //         Look at: AudioSource.PlayOneShot(AudioClip, float volumeScale) — plays without changing the source's
        //         own clip, so the same voice can carry a different clip next time.
        //         Check: impacts sound from where the object hit, not from the SFX Pool's origin.

        if (voice == null) voice = voices[nextVoice % voices.Length];   // placeholder so the method compiles before TODO 2; remove it after
        return voice;
    }

    /// <summary>How many voices are currently playing.</summary>
    public int BusyVoices()
    {
        if (voices == null) return 0;
        int n = 0;
        for (int i = 0; i < voices.Length; i++) if (voices[i] != null && voices[i].isPlaying) n++;
        return n;
    }

    void Update()
    {
        if (readout == null) return;
        int size = voices != null ? voices.Length : 0;
        readout.text = "SFX Pool  " + BusyVoices() + "/" + size + " voices busy\nrequests " + Requests + "   steals " + Steals;
    }

    /// <summary>A short sine tick with an exponential decay — the stand-in for every missing clip.</summary>
    AudioClip Placeholder()
    {
        if (placeholder != null) return placeholder;
        const int rate = 44100;
        float seconds = Mathf.Clamp(placeholderSeconds, 0.02f, 1f);
        int n = Mathf.RoundToInt(rate * seconds);
        float[] data = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)rate;
            float env = Mathf.Exp(-t * 6f / seconds);          // fast decay so it reads as a "tick"
            data[i] = 0.6f * env * Mathf.Sin(2f * Mathf.PI * placeholderHz * t);
        }
        placeholder = AudioClip.Create("Tick " + Mathf.RoundToInt(placeholderHz) + " Hz", n, 1, rate, false);
        placeholder.SetData(data, 0);
        return placeholder;
    }
}

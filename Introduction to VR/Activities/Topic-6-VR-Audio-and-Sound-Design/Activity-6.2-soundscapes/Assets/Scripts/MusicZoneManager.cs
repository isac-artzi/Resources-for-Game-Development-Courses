// MusicZoneManager.cs — Activity 6.2: Soundscapes and Music Zones
// The conductor. Owns two music AudioSources and crossfades between them with equal power whenever the
// player enters a new zone, switches the matching AmbientLayer on (and the others off), and moves the
// AudioMixer to that zone's snapshot — or, if you have not created a mixer yet, lerps an overall music
// level instead. Also synthesizes placeholder "music" (a three-note chord) for zones without a clip.
// Attached to: Audio Director in the starter scene.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.Audio;

public class MusicZoneManager : MonoBehaviour
{
    [Header("Music players (two, so one can fade out while the other fades in)")]
    [Tooltip("First music source. 2D (Spatial Blend 0), Loop on, no clip. Set by the builder.")]
    public AudioSource musicA;

    [Tooltip("Second music source, same settings. Set by the builder.")]
    public AudioSource musicB;

    [Header("Music per zone (leave empty to hear placeholder chords)")]
    public AudioClip forestMusic;
    public AudioClip mountainMusic;
    public AudioClip ruinsMusic;

    [Tooltip("Root note (Hz) of the placeholder chord for Forest, Mountain, Ruins. A = 220, E = 164.8, A = 110.")]
    public float[] placeholderRootHz = new float[] { 220f, 164.8f, 110f };

    [Tooltip("Seconds for a complete crossfade. 2–4 s feels like a scene change; 8+ s feels like weather.")]
    public float crossfadeSeconds = 3f;

    [Header("Ambience")]
    [Tooltip("One AmbientLayer per zone, index = (int)ZoneId: 0 Forest, 1 Mountain, 2 Ruins. Set by the builder.")]
    public AmbientLayer[] ambientLayers = new AmbientLayer[3];

    [Header("Mixer snapshots (optional — create an Audio Mixer asset, see README Task 5)")]
    public AudioMixerSnapshot forestSnapshot;
    public AudioMixerSnapshot mountainSnapshot;
    public AudioMixerSnapshot ruinsSnapshot;

    [Tooltip("Seconds for a snapshot transition (and for the fallback level lerp).")]
    public float snapshotSeconds = 2f;

    [Tooltip("Fallback when a zone has no snapshot: overall music level for Forest, Mountain, Ruins.")]
    public float[] musicLevelPerZone = new float[] { 0.8f, 0.6f, 0.45f };

    [Header("Readout (optional)")]
    [Tooltip("A TextMesh that shows the current zone, fade progress and music level. Set by the builder.")]
    public TextMesh readout;

    /// <summary>The zone the head is in, or None between zones.</summary>
    public ZoneVolume.ZoneId CurrentZone { get; private set; }

    /// <summary>0..1 progress of the crossfade in flight (1 = idle).</summary>
    public float FadeProgress { get { return Mathf.Clamp01(fadeT); } }

    AudioSource active;      // the source currently carrying the music (fading OUT during a crossfade)
    AudioSource incoming;    // the idle source (fading IN during a crossfade)
    float fadeT = 1f;        // 0 = crossfade just started, 1 = finished
    float musicLevel = 0.8f; // current overall music level (fallback for snapshots)
    float targetMusicLevel = 0.8f;

    void Awake()
    {
        CurrentZone = ZoneVolume.ZoneId.None;
    }

    void Start()
    {
        active = musicA;
        incoming = musicB;
        foreach (var s in new AudioSource[] { musicA, musicB })
        {
            if (s == null) continue;
            s.loop = true;
            s.playOnAwake = false;
            s.spatialBlend = 0f;   // music is not "in the world"; it is in the player's head
            s.volume = 0f;
        }
        foreach (var layer in ambientLayers) if (layer != null) layer.SetActive(false);
    }

    /// <summary>Called by a ZoneVolume when the head enters it.</summary>
    public void EnterZone(ZoneVolume.ZoneId zone)
    {
        if (zone == CurrentZone) return;
        CurrentZone = zone;

        // TODO 1: Start the crossfade toward this zone's music.
        //             StartCrossfade(ClipFor(zone));
        //         Then apply the rest of the zone's soundscape:
        //             ApplyZoneMix(zone);
        //         Check: stepping onto the Forest pad, the Console (from ZoneVolume) says "enter Forest" and, once
        //         TODO 2 is done, a chord fades in over crossfadeSeconds.
    }

    /// <summary>Called by a ZoneVolume when the head leaves it. Between zones the music fades to silence.</summary>
    public void ExitZone(ZoneVolume.ZoneId zone)
    {
        if (zone != CurrentZone) return;   // we already entered another zone; ignore the late exit
        CurrentZone = ZoneVolume.ZoneId.None;
        StartCrossfade(null);
        ApplyZoneMix(ZoneVolume.ZoneId.None);
    }

    /// <summary>Puts 'clip' (or silence, if null) on the idle source and begins fading toward it.</summary>
    void StartCrossfade(AudioClip clip)
    {
        if (active == null || incoming == null) return;
        if (fadeT < 1f)
        {
            // A fade is already running: finish it instantly so we never have three things playing.
            active.Stop();
            var t = active; active = incoming; incoming = t;
        }
        incoming.Stop();
        incoming.clip = clip;
        incoming.volume = 0f;
        if (clip != null) incoming.Play();
        fadeT = 0f;
    }

    void Update()
    {
        if (active == null || incoming == null) return;

        // TODO 3: Fallback for the mixer — glide the overall music level toward its target.
        //             musicLevel = Mathf.MoveTowards(musicLevel, targetMusicLevel, Time.deltaTime / Mathf.Max(0.05f, snapshotSeconds));
        //         Look at: Mathf.MoveTowards (constant-speed approach; here the speed is 1 unit per snapshotSeconds).
        //         Check: with no snapshots assigned, the readout's "level" number slides from 0.80 to 0.60 over about 2 s
        //         when you walk from Forest into Mountain.

        // TODO 2: The equal-power crossfade.
        //         if (fadeT < 1f)
        //         {
        //             fadeT += Time.deltaTime / Mathf.Max(0.05f, crossfadeSeconds);
        //             float g = Mathf.Clamp01(fadeT);
        //             float outGain = Mathf.Cos(g * Mathf.PI * 0.5f);   // 1 -> 0
        //             float inGain  = Mathf.Sin(g * Mathf.PI * 0.5f);   // 0 -> 1
        //             active.volume   = outGain * musicLevel;
        //             incoming.volume = inGain  * musicLevel;
        //             if (g >= 1f) { active.Stop(); var t = active; active = incoming; incoming = t; }
        //         }
        //         else active.volume = musicLevel;
        //         Why cos/sin and not a straight line: cos^2 + sin^2 = 1, so the summed POWER stays constant and there
        //         is no dip in the middle of the fade. Try (1 - g) and g once to hear the hole.
        //         Check: walking from Forest to Mountain, one chord melts into the other with no dip or bump in loudness;
        //         the readout's "fade" number runs 0.00 -> 1.00 in crossfadeSeconds.

        if (readout != null)
        {
            readout.text = "Zone: " + CurrentZone + "\nfade " + FadeProgress.ToString("F2") +
                           "   level " + musicLevel.ToString("F2") + " -> " + targetMusicLevel.ToString("F2") +
                           "\nA: " + Describe(musicA) + "\nB: " + Describe(musicB);
        }
    }

    /// <summary>Switches ambience layers and the mixer snapshot (or the fallback level) for a zone.</summary>
    void ApplyZoneMix(ZoneVolume.ZoneId zone)
    {
        int index = (int)zone;

        // TODO 4: Ambience — exactly one layer on.
        //             for (int i = 0; i < ambientLayers.Length; i++)
        //                 if (ambientLayers[i] != null) ambientLayers[i].SetActive(i == index);
        //         Check: the Forest bed (a low hum) fades in on the green pad and out when you leave it; the Mountain
        //         bed replaces it on the grey pad.

        // TODO 5: Mixer snapshot, with a fallback.
        //             AudioMixerSnapshot snap = index == 0 ? forestSnapshot : index == 1 ? mountainSnapshot : index == 2 ? ruinsSnapshot : null;
        //             if (snap != null) snap.TransitionTo(snapshotSeconds);
        //             else if (index >= 0 && index < musicLevelPerZone.Length) targetMusicLevel = musicLevelPerZone[index];
        //         Look at: AudioMixerSnapshot.TransitionTo(float timeToReach). Why a fallback: the mixer is an asset you
        //         create by hand (README Task 5); until then the level lerp stands in for the snapshot.
        //         Check: without snapshots the level changes per zone; after you create the mixer and assign the three
        //         snapshots, the Audio Mixer window (Window > Audio > Audio Mixer) shows the faders sliding as you walk.
    }

    /// <summary>The clip for a zone, or a synthesized chord if none was assigned.</summary>
    AudioClip ClipFor(ZoneVolume.ZoneId zone)
    {
        int index = (int)zone;
        if (index < 0) return null;
        AudioClip clip = index == 0 ? forestMusic : index == 1 ? mountainMusic : ruinsMusic;
        if (clip != null) return clip;
        float root = index < placeholderRootHz.Length ? placeholderRootHz[index] : 220f;
        return PlaceholderChord(root, 2f);
    }

    static string Describe(AudioSource s)
    {
        if (s == null) return "-";
        return (s.clip != null ? s.clip.name : "(no clip)") + (s.isPlaying ? " playing " : " stopped ") + s.volume.ToString("F2");
    }

    /// <summary>
    /// Minimal placeholder music: root + fifth + octave as sines, looped seamlessly. (A fuller version of this idea
    /// lives in Activity 6.1's ToneFactory.) Replace with real music by assigning the clip fields above.
    /// </summary>
    public static AudioClip PlaceholderChord(float rootHz, float seconds)
    {
        const int rate = 44100;
        seconds = Mathf.Clamp(seconds, 0.5f, 8f);
        int n = Mathf.RoundToInt(rate * seconds);
        float[] data = new float[n];
        float[] ratios = { 1f, 1.5f, 2f };
        float[] amps = { 0.30f, 0.18f, 0.10f };
        for (int k = 0; k < ratios.Length; k++)
        {
            float f = Mathf.Max(1f, Mathf.Round(rootHz * ratios[k] * seconds)) / seconds;   // whole cycles -> seamless loop
            for (int i = 0; i < n; i++)
                data[i] += amps[k] * Mathf.Sin(2f * Mathf.PI * f * i / rate);
        }
        var clip = AudioClip.Create("Chord " + Mathf.RoundToInt(rootHz) + " Hz", n, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }
}

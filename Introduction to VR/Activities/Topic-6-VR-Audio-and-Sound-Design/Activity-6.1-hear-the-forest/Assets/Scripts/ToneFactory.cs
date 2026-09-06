// ToneFactory.cs — Activity 6.1: Hear the Forest
// Generates placeholder AudioClips in code (a sine tone or filtered noise) so every AudioSource in the
// starter scene makes a sound even before you download a single file. Also caches the clips so that
// several sources asking for the same frequency share one clip.
// Attached to: "Audio Tools" in the starter scene. Other scripts call the static methods.
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEngine;

public class ToneFactory : MonoBehaviour
{
    [Header("Generated clips")]
    [Tooltip("Samples per second for generated clips. 44100 is CD quality; 22050 is plenty for a test tone.")]
    public int sampleRate = 44100;

    [Tooltip("Peak amplitude of generated clips (0..1). Keep it below 1 so several placeholders do not clip when they add up.")]
    [Range(0.05f, 1f)]
    public float amplitude = 0.4f;

    [Tooltip("Log a Console line each time a placeholder is created, so you can see which sources still lack a real clip.")]
    public bool logPlaceholders = true;

    static ToneFactory instance;
    static readonly Dictionary<string, AudioClip> cache = new Dictionary<string, AudioClip>();

    void Awake()
    {
        instance = this;
    }

    static int Rate { get { return instance != null ? Mathf.Max(8000, instance.sampleRate) : 44100; } }
    static float Amp { get { return instance != null ? instance.amplitude : 0.4f; } }

    /// <summary>
    /// Returns a looping sine tone. The frequency is rounded so a whole number of cycles fits in the clip,
    /// which makes the loop seamless (no click at the wrap point).
    /// </summary>
    public static AudioClip Tone(float frequencyHz, float seconds = 1f)
    {
        seconds = Mathf.Clamp(seconds, 0.05f, 10f);
        float cycles = Mathf.Max(1f, Mathf.Round(frequencyHz * seconds));
        float f = cycles / seconds;                       // adjusted frequency: an integer number of cycles
        string key = "tone_" + f.ToString("F1") + "_" + seconds.ToString("F2");
        AudioClip cached;
        if (cache.TryGetValue(key, out cached) && cached != null) return cached;

        int rate = Rate;
        int n = Mathf.RoundToInt(rate * seconds);
        float[] data = new float[n];
        float amp = Amp;
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)rate;                    // time of this sample in seconds
            data[i] = amp * Mathf.Sin(2f * Mathf.PI * f * t);
        }

        // TODO 1 (optional, stretch): a pure sine is thin. Add a quieter second harmonic so sources are
        //         easier to tell apart:  data[i] += 0.3f * amp * Mathf.Sin(2f * Mathf.PI * 2f * f * t);
        //         Because 2f also fits a whole number of cycles, the loop stays seamless.
        //         Check: the fire and the bird sound "richer" and you can still tell them apart with eyes closed.

        var clip = AudioClip.Create("Tone " + Mathf.RoundToInt(f) + " Hz", n, 1, rate, false);
        clip.SetData(data, 0);
        cache[key] = clip;
        Log("sine " + Mathf.RoundToInt(f) + " Hz");
        return clip;
    }

    /// <summary>
    /// Returns a looping noise clip — the placeholder for water, wind or a crackling fire.
    /// Until TODO 2 is done it falls back to a low tone so the source is still audible.
    /// </summary>
    public static AudioClip Noise(float seconds = 2f, float smoothing = 0.2f)
    {
        seconds = Mathf.Clamp(seconds, 0.1f, 10f);
        string key = "noise_" + seconds.ToString("F2") + "_" + smoothing.ToString("F2");
        AudioClip cached;
        if (cache.TryGetValue(key, out cached) && cached != null) return cached;

        int rate = Rate;
        int n = Mathf.RoundToInt(rate * seconds);
        float[] data = new float[n];
        float amp = Amp;

        // TODO 2: Fill 'data' with white noise, then soften it with a one-pole low-pass so it sounds like water
        //         rather than static:
        //             float y = 0f;
        //             for (int i = 0; i < n; i++)
        //             {
        //                 float x = Random.Range(-1f, 1f);          // white noise sample
        //                 y += smoothing * (x - y);                 // low-pass: y follows x slowly
        //                 data[i] = amp * y;
        //             }
        //         Look at: Random.Range(float, float). The loop wrap will click slightly; that is fine for a placeholder.
        //         Why: a stream is broadband (many frequencies at once); a sine is one frequency. Localization
        //         is also easier with broadband sounds — you will hear that when you compare the stream to the bird.
        //         Check: the Stream hisses like water instead of humming. Delete the placeholder body below when done.
        var fallback = Tone(180f, seconds);
        return fallback;

        // (unreachable until you replace the fallback above; keep it as the template for the real clip)
        // var clip = AudioClip.Create("Noise", n, 1, rate, false);
        // clip.SetData(data, 0);
        // cache[key] = clip;
        // Log("noise");
        // return clip;
    }

    /// <summary>If the source has no clip, gives it a looping sine placeholder. Returns true if a placeholder was assigned.</summary>
    public static bool EnsureClip(AudioSource source, float placeholderHz, float seconds = 1f)
    {
        if (source == null || source.clip != null) return false;
        source.clip = Tone(placeholderHz, seconds);
        return true;
    }

    /// <summary>If the source has no clip, gives it a looping noise placeholder. Returns true if a placeholder was assigned.</summary>
    public static bool EnsureNoise(AudioSource source, float seconds = 2f)
    {
        if (source == null || source.clip != null) return false;
        source.clip = Noise(seconds);
        return true;
    }

    static void Log(string what)
    {
        if (instance != null && !instance.logPlaceholders) return;
        Debug.Log("[ToneFactory] Created placeholder clip (" + what + "). Drop a real .wav/.ogg into the AudioSource's Clip field to replace it.");
    }
}

// GuideReveal.cs — Activity 7.4: Juice: Feedback and Polish
// The director of the scene's one big moment. Counts pickups; when the last shard is taken it plays an
// anticipation (the clearing's light swells, a low hum rises) and then the reaction (the Mysterious Guide
// pops into existence with an overshooting scale, a burst, and a toast).
// Attached to: "Director" in the starter scene. The Guide itself starts with its renderer disabled.
// Created by Isac Artzi

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GuideReveal : MonoBehaviour
{
    [Header("Progress")]
    [Tooltip("Pickups needed before the reveal.")]
    public int piecesNeeded = 3;

    [Tooltip("Label that shows 'Shards: 1 / 3'.")]
    public TextMesh statusLabel;

    [Header("The Guide (pre-wired)")]
    [Tooltip("Renderer of the Guide, disabled until the reveal.")]
    public Renderer guideRenderer;

    [Tooltip("TweenScale on the Guide, used for GrowFromZero.")]
    public TweenScale guideTween;

    [Tooltip("Point light near the Guide used for the anticipation swell.")]
    public Light revealLight;

    [Tooltip("Particle burst at the Guide's feet.")]
    public ParticleSystem revealBurst;

    [Tooltip("AudioSource near the Guide for the hum and the chord.")]
    public AudioSource audioSource;

    [Tooltip("Shared toast.")]
    public WorldToast toast;

    [Header("Timing")]
    [Tooltip("Seconds of anticipation before the Guide appears.")]
    public float anticipationSeconds = 1.2f;

    [Tooltip("Seconds for the Guide's grow-in.")]
    public float growSeconds = 0.6f;

    [Tooltip("Light intensity at the peak of the anticipation.")]
    public float peakIntensity = 8f;

    [Tooltip("Desktop test key: triggers the reveal immediately.")]
    public Key testKey = Key.R;

    [Header("Read-only")]
    public int pickups;
    public bool revealed;

    float restingIntensity;

    void Awake()
    {
        if (revealLight != null) restingIntensity = revealLight.intensity;
        if (guideRenderer != null) guideRenderer.enabled = false;
    }

    void Start()
    {
        RefreshLabel();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb != null && kb[testKey].wasPressedThisFrame && !revealed) StartCoroutine(Reveal());
    }

    /// <summary>Called by each PickupBurst on a real pickup.</summary>
    public void NotifyPickup()
    {
        // TODO 1: pickups++; RefreshLabel(); if (!revealed && pickups >= piecesNeeded) StartCoroutine(Reveal());
        //         Check: the status sign counts 1 / 3, 2 / 3, and the third pickup starts the reveal.
    }

    void RefreshLabel()
    {
        if (statusLabel != null) statusLabel.text = "Shards: " + pickups + " / " + piecesNeeded;
    }

    /// <summary>Anticipation, then reaction.</summary>
    public IEnumerator Reveal()
    {
        revealed = true;
        Debug.Log("[GuideReveal] the Guide appears");

        // TODO 2: Anticipation. Over anticipationSeconds, raise revealLight.intensity from resting to peakIntensity with an
        //         EASE-IN (k*k*k, slow start, fast finish) and, if audioSource is set, play a low hum:
        //             var hum = PickupBurst.MakeChime(110f, anticipationSeconds); audioSource.PlayOneShot(hum, 0.5f);
        //             float t = 0; while (t < 1f) { t += Time.deltaTime / anticipationSeconds; float k = Mathf.Clamp01(t);
        //                 if (revealLight != null) revealLight.intensity = Mathf.Lerp(restingIntensity, peakIntensity, k * k * k);
        //                 yield return null; }
        //         Why anticipation: the player's attention must already be on the spot when the thing happens. A build-up
        //         of about a second is the difference between "a capsule appeared" and "the Guide arrived".
        //         Check: pressing R makes the far end of the clearing brighten over a second before anything appears.

        // TODO 3: Reaction, all in the same frame: guideRenderer.enabled = true; guideTween.GrowFromZero(growSeconds);
        //         revealBurst.Play(); a chord: audioSource.PlayOneShot(PickupBurst.MakeChime(440f, 0.8f), 0.8f);
        //         toast.Show("The Guide appears", guideTween.transform.position + Vector3.up * 1.2f);
        //         Then ease the light back to restingIntensity over ~1 s (ease-out, like PickupBurst.Flash).
        //         Look at: Renderer.enabled, the TweenScale.GrowFromZero you wrote.
        //         Check: the Guide pops in with a slight overshoot, sparks fly, the chord rings, the light settles.
        if (guideRenderer != null) guideRenderer.enabled = true;   // placeholder: a plain pop-in until TODO 2-3 are done
        yield return null;
    }
}

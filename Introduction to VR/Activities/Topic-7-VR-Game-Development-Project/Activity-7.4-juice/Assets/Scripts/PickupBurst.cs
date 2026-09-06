// PickupBurst.cs — Activity 7.4: Juice: Feedback and Polish
// Everything that happens in the first half second after a shard is grabbed: a particle burst, a light flash
// that decays, a scale punch, a procedural chime (no audio file needed), a world-space toast, and a message
// to the GuideReveal director. One event, five senses' worth of confirmation.
// Attached to: each Shard in the starter scene (all references pre-wired by the builder).
// Created by Isac Artzi

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PickupBurst : MonoBehaviour
{
    [Header("Parts (pre-wired)")]
    [Tooltip("Particle system child that plays one burst.")]
    public ParticleSystem burst;

    [Tooltip("Point light child that flashes and decays.")]
    public Light flashLight;

    [Tooltip("Scale punch on this shard.")]
    public TweenScale tween;

    [Tooltip("The shared world-space toast.")]
    public WorldToast toast;

    [Tooltip("The director that counts pickups and reveals the Guide.")]
    public GuideReveal reveal;

    [Tooltip("AudioSource on this shard (2D or 3D; the builder sets spatial blend 1).")]
    public AudioSource audioSource;

    [Header("Flash")]
    [Tooltip("Peak light intensity at the moment of pickup.")]
    public float flashIntensity = 6f;

    [Tooltip("Seconds for the flash to decay back to its resting intensity.")]
    public float flashSeconds = 0.4f;

    [Header("Chime (generated in code)")]
    [Tooltip("Pitch of the chime in Hz. 880 = A5. Give each shard a different note.")]
    public float chimeHz = 880f;

    [Tooltip("Length of the chime in seconds.")]
    public float chimeSeconds = 0.35f;

    [Header("Text")]
    public string toastText = "Shard taken!";

    [Header("Behavior")]
    [Tooltip("Only the first grab of this shard counts as a pickup (re-grabs stay silent).")]
    public bool onlyFirstGrab = true;

    [Tooltip("Desktop test key: fires Play() on every shard at once.")]
    public Key testKey = Key.J;

    AudioClip chime;
    float restingIntensity;
    bool pickedUp;

    void Awake()
    {
        if (flashLight != null) restingIntensity = flashLight.intensity;
        chime = MakeChime(chimeHz, chimeSeconds);
    }

    void Start()
    {
        // TODO 1: Hook the XRI grab event: var grab = GetComponent<XRGrabInteractable>();
        //         if (grab != null) grab.selectEntered.AddListener(OnGrabbed);
        //         Look at: XRGrabInteractable.selectEntered (UnityEvent<SelectEnterEventArgs>).
        //         Check: grabbing a shard in the simulator (Tab to a controller, move it onto the shard, G) prints
        //         "[PickupBurst] ..." in the Console and, after TODO 2, fires the effects.
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb != null && kb[testKey].wasPressedThisFrame) Play();
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        if (onlyFirstGrab && pickedUp) return;
        pickedUp = true;
        Play();
    }

    /// <summary>Fires every feedback channel at once.</summary>
    public void Play()
    {
        Debug.Log("[PickupBurst] " + name + " pickup");
        // TODO 2: One line per channel, each null-guarded:
        //             if (burst != null) burst.Play();
        //             if (flashLight != null) StartCoroutine(Flash());
        //             if (tween != null) tween.Punch();
        //             if (audioSource != null && chime != null) audioSource.PlayOneShot(chime, 0.8f);
        //             if (toast != null) toast.Show(toastText, transform.position);
        //             if (reveal != null) reveal.NotifyPickup();
        //         Look at: ParticleSystem.Play, AudioSource.PlayOneShot(clip, volumeScale).
        //         Why all at once: feedback works by agreement between senses — sparks + flash + sound + motion within
        //         the same 100 ms read as ONE event. Spread them out and it reads as five small events.
        //         Check: press J — every shard sparks, flashes, punches, chimes and shows a toast together.
    }

    IEnumerator Flash()
    {
        // TODO 3: Ease-out decay. float t = 0; while (t < 1f) { t += Time.deltaTime / flashSeconds;
        //             float k = 1f - Mathf.Clamp01(t); flashLight.intensity = restingIntensity + (flashIntensity - restingIntensity) * k * k * k;
        //             yield return null; }  then flashLight.intensity = restingIntensity.
        //         Look at: Light.intensity. Why cubic: a light that drops fast then lingers matches how a real spark cools;
        //         a linear fade looks like a dimmer switch.
        //         Check: the shard's light spikes bright and is back to normal within half a second.
        yield return null;
    }

    /// <summary>Builds a short decaying sine-wave clip in memory: no audio asset required.</summary>
    public static AudioClip MakeChime(float hz, float seconds)
    {
        int rate = 44100;
        int n = Mathf.Max(1, Mathf.RoundToInt(rate * seconds));
        float[] data = new float[n];
        // TODO 4: Fill the buffer. For i in 0..n-1: float t = (float)i / rate;
        //             data[i] = Mathf.Sin(2f * Mathf.PI * hz * t) * Mathf.Exp(-6f * t / seconds) * 0.6f;
        //         Look at: Mathf.Sin, Mathf.Exp. A sine at 'hz' cycles per second, multiplied by an exponential envelope
        //         that has fallen to e^-6 (0.25 %) by the end — no click at the cutoff.
        //         Check: pickups make a clean bell-like "ting"; change Chime Hz to 660 on one shard and it sounds lower.
        var clip = AudioClip.Create("Chime_" + Mathf.RoundToInt(hz), n, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }
}

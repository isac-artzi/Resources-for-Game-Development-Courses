// SonarPing.cs — Activity 6.4: Dark Mode
// The player's "echolocation": on a key or controller button it emits a click at the head, finds the nearest
// surfaces around the player, and plays an echo FROM each surface after a delay proportional to its distance
// (round trip 2d / 343 m/s, stretched so human ears can hear the difference). Also holds the placeholder
// tone generator that every script in this activity uses.
// Attached to: Sonar in the starter scene.
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SonarPing : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Desktop key that fires a ping.")]
    public Key pingKey = Key.P;

    [Tooltip("Controller action for the headset (e.g. the right-hand Primary Button). Leave empty on desktop.")]
    public InputActionReference pingAction;

    [Tooltip("Seconds between pings. Echoes need time to arrive.")]
    public float cooldownSeconds = 1.2f;

    [Header("Scan")]
    [Tooltip("How far around the head to look for surfaces (meters).")]
    public float scanRadius = 12f;

    [Tooltip("How many echoes to play per ping — the nearest N surfaces.")]
    [Range(1, 12)]
    public int maxEchoes = 6;

    [Tooltip("Which layers count as surfaces.")]
    public LayerMask surfaceLayers = ~0;

    [Tooltip("Colliders under this transform are ignored (set to the XR rig so the player's own body is not a wall).")]
    public Transform ignoreRoot;

    [Header("Sound")]
    [Tooltip("The click emitted at the head. Leave empty for a synthesized 2 kHz tick.")]
    public AudioClip pingClip;

    [Tooltip("The echo played at each surface. Leave empty for a synthesized 900 Hz tick.")]
    public AudioClip echoClip;

    [Tooltip("Speed of sound in air, m/s.")]
    public float speedOfSound = 343f;

    [Tooltip("Real echo delays are a few milliseconds — too short to hear. Multiply them by this so 1 m reads as ~0.1 s.")]
    public float timeStretch = 20f;

    [Tooltip("Echo volume for a surface at distance 0..scanRadius (normalized on the x axis).")]
    public AnimationCurve volumeByDistance = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.15f);

    [Header("Debug")]
    [Tooltip("Draw a line from the head to each echo point for 1 s (Scene view).")]
    public bool drawDebugLines = true;

    [Tooltip("Shows the last ping's echo count and nearest distance. Set by the builder.")]
    public TextMesh readout;

    /// <summary>Number of pings fired so far.</summary>
    public int PingCount { get; private set; }

    struct EchoTarget
    {
        public Vector3 point;
        public float distance;
        public string surfaceName;
    }

    Transform head;
    AudioSource pingSource;
    AudioSource[] echoVoices;
    float lastPingTime = -10f;
    readonly List<EchoTarget> targets = new List<EchoTarget>();

    void Start()
    {
        if (Camera.main != null) head = Camera.main.transform;

        pingSource = gameObject.AddComponent<AudioSource>();
        pingSource.playOnAwake = false;
        pingSource.spatialBlend = 0f;                 // the click is "in your head"
        if (pingClip == null) pingClip = MakeTone(2000f, 0.05f, true);
        if (echoClip == null) echoClip = MakeTone(900f, 0.09f, true);

        echoVoices = new AudioSource[Mathf.Max(1, maxEchoes)];
        for (int i = 0; i < echoVoices.Length; i++)
        {
            var go = new GameObject("Echo Voice " + (i + 1));
            go.transform.SetParent(transform, false);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 1f;                    // echoes come FROM the wall
            src.rolloffMode = AudioRolloffMode.Linear;
            src.minDistance = 1f;
            src.maxDistance = scanRadius * 2f;        // distance is expressed by delay and our own volume curve, not by rolloff
            src.clip = echoClip;
            echoVoices[i] = src;
        }

        if (pingAction != null && pingAction.action != null) pingAction.action.Enable();
    }

    void Update()
    {
        // TODO 1: Read the input (Input System only), respect the cooldown, and ping.
        //             bool pressed = Keyboard.current != null && Keyboard.current[pingKey].wasPressedThisFrame;
        //             if (pingAction != null && pingAction.action != null && pingAction.action.WasPressedThisFrame()) pressed = true;
        //             if (pressed && Time.time - lastPingTime >= cooldownSeconds) Ping();
        //         Look at: UnityEngine.InputSystem.Keyboard.current, KeyControl.wasPressedThisFrame, InputAction.WasPressedThisFrame.
        //         Why a cooldown: echoes are scheduled up to (2 * scanRadius / 343) * timeStretch seconds ahead; a second
        //         ping before they land makes an unreadable mess.
        //         Check: pressing P plays a click; pressing it again within 1.2 s does nothing.
    }

    /// <summary>Emits the click, scans for surfaces, and schedules the echoes.</summary>
    public void Ping()
    {
        if (head == null) return;
        lastPingTime = Time.time;
        PingCount++;
        pingSource.PlayOneShot(pingClip, 1f);
        targets.Clear();

        // TODO 2: Gather candidate surfaces around the head.
        //             Collider[] hits = Physics.OverlapSphere(head.position, scanRadius, surfaceLayers, QueryTriggerInteraction.Ignore);
        //             foreach (var c in hits)
        //             {
        //                 if (ignoreRoot != null && c.transform.IsChildOf(ignoreRoot)) continue;   // not our own body
        //                 Vector3 p = c.ClosestPoint(head.position);                                // nearest point on that surface
        //                 float d = Vector3.Distance(head.position, p);
        //                 if (d < 0.05f) continue;                                                  // we are inside it; skip
        //                 targets.Add(new EchoTarget { point = p, distance = d, surfaceName = c.name });
        //             }
        //         Look at: Physics.OverlapSphere, Collider.ClosestPoint (works for primitive and convex colliders),
        //         QueryTriggerInteraction.Ignore (skip the zone/hazard trigger boxes).
        //         Check: Debug.Log(targets.Count) — standing at the start you get roughly 8–15 candidates (floor, walls, pits).

        // TODO 3: Keep the nearest maxEchoes.
        //             targets.Sort((a, b) => a.distance.CompareTo(b.distance));
        //             int count = Mathf.Min(maxEchoes, targets.Count);
        //         Look at: List<T>.Sort(Comparison<T>). Why nearest-N: the nearest surfaces are the ones you might walk into;
        //         far ones only add clutter — and the ear cannot separate more than a handful of echoes anyway.

        // TODO 4: Schedule one echo per surface with delay = round trip / speed of sound, stretched.
        //             for (int i = 0; i < count; i++)
        //             {
        //                 var t = targets[i];
        //                 var voice = echoVoices[i];
        //                 voice.Stop();
        //                 voice.transform.position = t.point;
        //                 voice.volume = volumeByDistance.Evaluate(Mathf.Clamp01(t.distance / scanRadius));
        //                 float delay = (2f * t.distance / speedOfSound) * timeStretch;
        //                 voice.PlayDelayed(delay);
        //                 if (drawDebugLines) Debug.DrawLine(head.position, t.point, Color.cyan, 1f);
        //             }
        //             if (readout != null) readout.text = "Ping " + PingCount + ": " + count + " echoes\nnearest " +
        //                                                 (count > 0 ? targets[0].distance.ToString("F1") + " m (" + targets[0].surfaceName + ")" : "-");
        //         Look at: AudioSource.PlayDelayed(float seconds) — schedules the source's own clip; this is why each echo
        //         voice has echoClip assigned. Why 2d: the click travels to the wall and back.
        //         Check: facing a wall 2 m away you hear click ... echo about 0.23 s later from the wall's direction; step back
        //         to 4 m and the gap doubles. Cyan lines in the Scene view point at each echoing surface.
    }

    /// <summary>
    /// Minimal placeholder tone: a sine at 'hz' for 'seconds', optionally with an exponential decay so it reads as a click.
    /// (A fuller generator lives in Activity 6.1's ToneFactory.) Shared by the other scripts in this activity.
    /// </summary>
    public static AudioClip MakeTone(float hz, float seconds, bool decay)
    {
        const int rate = 44100;
        seconds = Mathf.Clamp(seconds, 0.02f, 4f);
        if (!decay) hz = Mathf.Max(1f, Mathf.Round(hz * seconds)) / seconds;   // whole cycles -> seamless loop
        int n = Mathf.RoundToInt(rate * seconds);
        float[] data = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)rate;
            float env = decay ? Mathf.Exp(-t * 7f / seconds) : 1f;
            data[i] = 0.5f * env * Mathf.Sin(2f * Mathf.PI * hz * t);
        }
        var clip = AudioClip.Create((decay ? "Tick " : "Tone ") + Mathf.RoundToInt(hz) + " Hz", n, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }
}

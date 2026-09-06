// HapticPulse.cs — Activity 6.3: The Sound of Touch
// Sends a short vibration to whichever controller is holding this object: a fixed pulse on grab, and
// impact-scaled pulses on request (ImpactSound calls Pulse when the held object hits something).
// Fully guarded: with the desktop simulator there is no haptic device, so it logs instead of failing.
// Attached to: Shard, River Stone, Rune Tablet in the starter scene (alongside XRGrabInteractable).
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class HapticPulse : MonoBehaviour
{
    [Header("Grab pulse")]
    [Tooltip("Vibration strength on grab (0..1). 0.3–0.5 reads as 'contact'; 1.0 reads as 'alarm'.")]
    [Range(0f, 1f)]
    public float grabAmplitude = 0.4f;

    [Tooltip("Length of the grab pulse in seconds. Keep it under 0.1 s — longer feels like a phone buzzing.")]
    public float grabDuration = 0.06f;

    [Header("Impact pulses (requested by ImpactSound)")]
    [Tooltip("Strength of a full-speed impact pulse (0..1).")]
    [Range(0f, 1f)]
    public float maxImpactAmplitude = 0.8f;

    [Tooltip("Ignore pulse requests closer together than this, so a bounce does not become a rattle.")]
    public float minIntervalSeconds = 0.05f;

    [Header("Debug")]
    [Tooltip("Log a Console line for every pulse. Useful on desktop where you cannot feel anything.")]
    public bool logPulses = true;

    /// <summary>Pulses actually sent to a device (not counting desktop no-ops).</summary>
    public int PulsesSent { get; private set; }

    /// <summary>True while a controller with a HapticImpulsePlayer is holding this object.</summary>
    public bool HasDevice { get { return player != null; } }

    /// <summary>Seconds since the last pulse request that went through the rate limit.</summary>
    public float SecondsSincePulse { get { return Time.time - lastPulseTime; } }

    XRGrabInteractable grab;
    HapticImpulsePlayer player;   // the haptic output of the controller currently holding us; null when not held or on desktop
    float lastPulseTime = -10f;

    void OnEnable()
    {
        grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrabbed);
            grab.selectExited.AddListener(OnReleased);
        }
    }

    void OnDisable()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrabbed);
            grab.selectExited.RemoveListener(OnReleased);
        }
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        // TODO 1: Find the haptic output on the hand that grabbed us.
        //             player = args.interactorObject.transform.GetComponentInParent<HapticImpulsePlayer>();
        //         Look at: SelectEnterEventArgs.interactorObject (the interactor that selected us), Transform.GetComponentInParent.
        //         Why GetComponentInParent: the Near-Far Interactor sits on a child of the "Left/Right Controller" object,
        //         and the HapticImpulsePlayer lives on that controller object (Starter Assets rig). With the desktop
        //         simulator the component exists but drives no real motor; on a headset it drives the controller.
        //         Check: add Debug.Log(player) right after — on desktop and headset alike it prints the component's name;
        //         if it prints "null", check the rig hierarchy for a Haptic Impulse Player component.

        // TODO 2: Confirm the grab with a short pulse.
        //             Pulse(grabAmplitude, grabDuration);
        //         Check: on the headset the grabbing hand ticks once. (verify on headset)
    }

    void OnReleased(SelectExitEventArgs args)
    {
        player = null;
    }

    /// <summary>
    /// Sends one haptic impulse to the holding controller. amplitude01 is 0..1 (scaled by maxImpactAmplitude),
    /// duration in seconds. Safe to call when nothing is held or no device exists.
    /// </summary>
    public void Pulse(float amplitude01, float duration)
    {
        // TODO 3: Rate-limit, clamp, and send — or log when there is nothing to send to.
        //             if (Time.time - lastPulseTime < minIntervalSeconds) return;
        //             lastPulseTime = Time.time;
        //             float amp = Mathf.Clamp01(amplitude01) * maxImpactAmplitude;
        //             duration = Mathf.Clamp(duration, 0.01f, 0.5f);
        //             if (player != null)
        //             {
        //                 player.SendHapticImpulse(amp, duration);
        //                 PulsesSent++;
        //                 if (logPulses) Debug.Log("[Haptics] " + name + " pulse amp " + amp.ToString("F2") + " for " + duration.ToString("F2") + " s");
        //             }
        //             else if (logPulses) Debug.Log("[Haptics] " + name + " desktop no-op (no HapticImpulsePlayer holding this object)");
        //         Look at: HapticImpulsePlayer.SendHapticImpulse(float amplitude, float duration) — XRI 3.x, found under
        //         UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics (already in the using lines above). Why clamp the duration: controllers queue impulses;
        //         a long one blocks the next.
        //         Check: hold the shard and tap the gong — on desktop the Console shows one "[Haptics]" line per hit and
        //         never more than 20 per second; on the headset your hand buzzes harder for harder hits. (verify on headset)
    }
}

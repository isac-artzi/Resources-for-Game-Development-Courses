// ImpactSound.cs — Activity 6.3: The Sound of Touch
// Gives a physical object a voice: a sound when it is grabbed, another when it is released, and an impact
// sound whenever it collides, with volume shaped by impact speed and a small random pitch so no two hits are
// identical. All playback goes through SfxPool. The XRI event hookup is written for you; the sound logic is yours.
// Attached to: Shard, River Stone, Rune Tablet (grabbable) and the Gong (kinematic) in the starter scene.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class ImpactSound : MonoBehaviour
{
    [Header("Clips (leave empty to hear placeholder ticks)")]
    [Tooltip("Played once when an interactor grabs this object.")]
    public AudioClip grabClip;

    [Tooltip("Played once when the object is released.")]
    public AudioClip releaseClip;

    [Tooltip("One of these is chosen at random per impact. Give 2–4 variations of the same material.")]
    public AudioClip[] impactClips;

    [Header("Impact speed -> volume")]
    [Tooltip("Impacts slower than this (m/s) make no sound — stops resting contacts from ticking.")]
    public float minImpactSpeed = 0.3f;

    [Tooltip("Impacts at or above this speed (m/s) play at full volume.")]
    public float maxImpactSpeed = 4f;

    [Tooltip("Shapes normalized speed (0..1) into volume (0..1). Ease-in keeps light taps quiet.")]
    public AnimationCurve volumeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Tooltip("Ignore further impacts for this long after one plays (a bounce produces several contacts in a row).")]
    public float cooldownSeconds = 0.08f;

    [Header("Variation")]
    [Tooltip("Random pitch multiplier range per sound. 0.95–1.05 is subtle; 0.8–1.2 is cartoonish.")]
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    [Tooltip("Extra pitch multiplier applied ONLY to placeholder ticks, so each material sounds different before you have clips.")]
    public float placeholderPitch = 1f;

    [Header("Haptics (optional)")]
    [Tooltip("If set, impacts while held also send a controller pulse scaled by the impact. Set by the builder.")]
    public HapticPulse haptics;

    /// <summary>True while an interactor holds this object.</summary>
    public bool IsHeld { get; private set; }

    /// <summary>Speed of the most recent impact (m/s), for the readout.</summary>
    public float LastImpactSpeed { get; private set; }

    /// <summary>Seconds since the last impact that made a sound.</summary>
    public float SecondsSinceImpact { get { return Time.time - lastImpactTime; } }

    XRGrabInteractable grab;
    float lastImpactTime = -10f;

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
        IsHeld = true;

        // TODO 1: Play the grab sound through the pool.
        //             PlaySfx(grabClip, transform.position, 0.8f);
        //         (PlaySfx is written below; it adds the random pitch and the null-clip fallback.)
        //         Check: pressing grip on a prop clicks once. Nothing spams the Hierarchy — the pool is doing the work.
    }

    void OnReleased(SelectExitEventArgs args)
    {
        IsHeld = false;

        // TODO 1 (continued): PlaySfx(releaseClip, transform.position, 0.6f);
        //         Check: letting go clicks once, slightly quieter than the grab.
    }

    void OnCollisionEnter(Collision collision)
    {
        // TODO 2: Measure the hit and filter out the noise.
        //             float speed = collision.relativeVelocity.magnitude;
        //             LastImpactSpeed = speed;
        //             if (speed < minImpactSpeed) return;
        //             if (Time.time - lastImpactTime < cooldownSeconds) return;
        //             lastImpactTime = Time.time;
        //         Look at: Collision.relativeVelocity (closing speed of the two bodies, in m/s). Why the cooldown: one drop
        //         produces a burst of contacts a few ms apart; without it you hear a machine-gun.
        //         Check: add Debug.Log(speed) for a moment — a drop from table height (~1 m) reads about 4.4 m/s; a nudge
        //         reads under 0.3 and is skipped.

        // TODO 3: Speed -> volume through the curve, then play a random variation at the contact point.
        //             float v = volumeCurve.Evaluate(Mathf.InverseLerp(minImpactSpeed, maxImpactSpeed, speed));
        //             AudioClip clip = (impactClips != null && impactClips.Length > 0) ? impactClips[Random.Range(0, impactClips.Length)] : null;
        //             Vector3 where = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
        //             PlaySfx(clip, where, v);
        //         Look at: AnimationCurve.Evaluate, Mathf.InverseLerp, Collision.GetContact(int).
        //         Check: a gentle set-down is nearly silent; a throw at the gong is loud; drop the same stone five times
        //         and no two hits sound alike.

        // TODO 4: Tell the hand. If we are being held (you hit something WITH the object), send a haptic pulse
        //         proportional to the impact:
        //             if (IsHeld && haptics != null) haptics.Pulse(v, 0.05f);
        //         Check: on the headset, tapping the gong with the shard buzzes the holding hand; on desktop the Console
        //         logs "[Haptics] desktop no-op" instead. (verify on headset)
    }

    /// <summary>Plays a clip through the pool with random pitch; null clips get the placeholder tick at this object's pitch.</summary>
    void PlaySfx(AudioClip clip, Vector3 position, float volume)
    {
        if (SfxPool.Instance == null) return;
        float pitch = RandomPitch();
        if (clip == null) pitch *= placeholderPitch;
        SfxPool.Instance.Play(clip, position, volume, pitch);
    }

    /// <summary>A random pitch multiplier inside pitchRange.</summary>
    float RandomPitch()
    {
        // TODO 3 (continued): return Random.Range(pitchRange.x, pitchRange.y);
        //         Look at: Random.Range(float, float) — max is inclusive for floats.
        //         Check: set the range to (0.5, 2) temporarily and every hit is obviously a different note.
        return 1f;
    }
}

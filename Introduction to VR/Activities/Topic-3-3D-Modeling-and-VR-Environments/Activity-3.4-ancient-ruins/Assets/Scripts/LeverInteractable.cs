// LeverInteractable.cs — Activity 3.4: Ancient Ruins: Modular Kit and Mechanisms
// Reads a physical lever's angle from its HingeJoint, normalizes it to 0..1, drives an indicator light, and fires a
// UnityEvent exactly once when the handle crosses a threshold (and another when it comes back). The lever itself is
// an XRGrabInteractable held by a HingeJoint — XRI moves it, physics constrains it, this script only *reads* it.
// Attached to: "Lever Handle" in the starter scene (Hinge and Indicator pre-set; On Pulled is wired to the door).
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.Events;

public class LeverInteractable : MonoBehaviour
{
    [Header("Physics")]
    [Tooltip("The hinge that constrains the handle. Set in the starter scene; found on this GameObject if empty.")]
    public HingeJoint hinge;

    [Tooltip("Hinge angle (degrees) that counts as fully released (value 0). Matches the joint's lower limit.")]
    public float restAngle = -60f;

    [Tooltip("Hinge angle (degrees) that counts as fully pulled (value 1). Matches the joint's upper limit.")]
    public float pulledAngle = 60f;

    [Header("Trigger")]
    [Range(0f, 1f)]
    [Tooltip("Normalized value above which On Pulled fires. 0.8 means the handle must travel 80 % of the way.")]
    public float threshold = 0.8f;

    [Range(0f, 1f)]
    [Tooltip("Value below which On Released fires after a pull. Lower than threshold so the lever does not chatter at the edge.")]
    public float releaseBelow = 0.5f;

    [Header("Feedback")]
    [Tooltip("Optional light whose color follows the lever value (red at rest, green when pulled).")]
    public Light indicator;

    [Header("Events")]
    [Tooltip("Fires once when the lever crosses the threshold. The starter scene wires this to SlidingDoor.Open.")]
    public UnityEvent onPulled = new UnityEvent();

    [Tooltip("Fires once when the lever drops back below Release Below. The starter scene wires this to SlidingDoor.Close (a two-way lever).")]
    public UnityEvent onReleased = new UnityEvent();

    /// <summary>Normalized lever position: 0 at rest, 1 fully pulled. Updated every frame.</summary>
    public float Value { get; private set; }

    /// <summary>True between an On Pulled and the following On Released.</summary>
    public bool IsPulled { get; private set; }

    void Awake()
    {
        if (hinge == null) hinge = GetComponent<HingeJoint>();
    }

    void Update()
    {
        if (hinge == null) return;

        // TODO 1: Read the hinge angle.  float angle = hinge.angle;
        //         Look at: HingeJoint.angle — degrees of rotation from the joint's starting pose, positive along the joint axis.
        //         Debug.Log it once while grabbing the handle in the simulator to see which way is positive; if pulling toward
        //         you gives negative numbers, swap Rest Angle and Pulled Angle in the Inspector rather than editing code.
        float angle = 0f;

        // TODO 2: Normalize.  Value = Mathf.InverseLerp(restAngle, pulledAngle, angle);
        //         InverseLerp clamps to 0..1 so a joint that overshoots its limit a little never gives 1.02.
        //         Check: with the handle at rest the Value shown in the Inspector (Debug mode) is 0; pulled all the way it is 1.
        Value = 0f;

        // TODO 3: Edge detection — fire each event once per crossing, not every frame:
        //             if (!IsPulled && Value >= threshold) { IsPulled = true;  onPulled.Invoke(); }
        //             else if (IsPulled && Value <= releaseBelow) { IsPulled = false; onReleased.Invoke(); }
        //         Why two thresholds (0.8 and 0.5): a single threshold makes a handle resting exactly on it flicker
        //         between states every frame. The gap is called hysteresis.
        //         Look at: UnityEvent.Invoke. The listeners were added in the Editor (see the Inspector's On Pulled list).
        //         Check: pulling the lever past 80 % opens the door once; wiggling it near the top does not re-fire.

        // TODO 4: Feedback. If indicator != null: indicator.color = Color.Lerp(Color.red, Color.green, Value);
        //         Optional: indicator.intensity = Mathf.Lerp(1f, 3f, Value).
        //         Check: the lamp on the lever base turns from red to green as you pull.
    }
}

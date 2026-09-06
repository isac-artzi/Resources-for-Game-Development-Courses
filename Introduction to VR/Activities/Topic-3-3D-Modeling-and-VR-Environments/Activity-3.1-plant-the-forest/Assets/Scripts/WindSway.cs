// WindSway.cs — Activity 3.1: Plant the Forest
// Tilts a tree or bush back and forth by a few degrees so the forest reads as alive instead of frozen.
// Attached to: the root of the "Greybox Tree" and "Greybox Bush" prefabs (so every scattered copy sways).
// Put it on the ROOT of your imported tree too — the pivot must sit at the base of the trunk.
// Created by Isac Artzi

using UnityEngine;

public class WindSway : MonoBehaviour
{
    [Header("Wind")]
    [Tooltip("Maximum tilt away from the rest pose, in degrees. 2-3 degrees is a breeze; 8 is a storm.")]
    public float maxAngleDegrees = 2.5f;

    [Tooltip("Full sway cycles per second (Hz). Real trees sway slowly: 0.3-0.6 Hz.")]
    public float frequencyHz = 0.4f;

    [Tooltip("Horizontal direction the wind blows toward. Only x and z are used.")]
    public Vector3 windDirection = new Vector3(1f, 0f, 0.3f);

    [Header("Variation")]
    [Tooltip("Adds a faster, smaller gust on top of the main sway so trees do not look like metronomes.")]
    public bool addGust = true;

    // The rotation the prefab had when placed. All sway is applied on top of this.
    Quaternion restRotation;

    // Per-tree phase offset in radians so neighbours are not synchronized.
    float phase;

    void Start()
    {
        restRotation = transform.localRotation;

        // TODO 1: Give every tree its own phase so the forest does not sway in lockstep.
        //         Derive it from the world position, which is different for every copy:
        //             phase = (transform.position.x * 0.37f + transform.position.z * 0.91f) % (2f * Mathf.PI);
        //         (Random.Range(0f, 2f * Mathf.PI) also works, but position-based is repeatable.)
        //         Check: after TODO 2, neighbouring trees reach their extreme tilt at different moments.
        phase = 0f;
    }

    void Update()
    {
        // TODO 2: Apply the sway.
        //         1. float t = Time.time * 2f * Mathf.PI * frequencyHz + phase;
        //         2. float angle = Mathf.Sin(t) * maxAngleDegrees;
        //         3. The tilt axis is perpendicular to the wind, in the ground plane:
        //                Vector3 wind = new Vector3(windDirection.x, 0f, windDirection.z).normalized;
        //                Vector3 axis = Vector3.Cross(Vector3.up, wind);
        //            (Cross of up and the wind gives a horizontal axis; rotating about it leans the tree with the wind.)
        //         4. transform.localRotation = Quaternion.AngleAxis(angle, axis) * restRotation;
        //         Look at: Mathf.Sin, Vector3.Cross, Quaternion.AngleAxis. Order matters: the tilt is applied
        //         in world/parent space first, then the tree's own rest rotation.
        //         Check: trees lean toward +x/+z and back once every 2.5 s (1 / 0.4 Hz). The base of the trunk
        //         stays planted — if the whole tree slides sideways, the prefab's pivot is not at its base.

        // TODO 3 (if addGust): add a second, faster term before computing the rotation:
        //             angle += Mathf.Sin(t * 2.7f) * maxAngleDegrees * 0.3f;
        //         A non-integer multiple (2.7) keeps the two waves from repeating in a short loop.
        //         Check: the motion now has a small flutter riding on the slow sway.
    }
}

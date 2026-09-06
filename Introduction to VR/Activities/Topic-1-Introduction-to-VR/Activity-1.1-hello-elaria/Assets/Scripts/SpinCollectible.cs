// SpinCollectible.cs — Activity 1.1: Hello, Elaria
// Makes an artifact shard spin and gently bob so it reads as "pick me up" from across the clearing.
// Attached to: Shard 1, Shard 2, Shard 3 in the starter scene (each with a different speed).
// Created by Isac Artzi

using UnityEngine;

public class SpinCollectible : MonoBehaviour
{
    [Header("Spin")]
    [Tooltip("Rotation speed around the shard's local up axis, in degrees per second.")]
    public float degreesPerSecond = 45f;

    [Header("Bob")]
    [Tooltip("How far above and below the start position the shard floats, in meters.")]
    public float bobAmplitude = 0.05f;

    [Tooltip("Full up-and-down cycles per second (Hz). 0.5 means one cycle every two seconds.")]
    public float bobFrequency = 0.5f;

    // The position the shard was placed at in the scene. The bob is measured from here.
    Vector3 startPosition;

    void Start()
    {
        // TODO 2: Remember where this shard starts so the bob has a fixed center.
        //         Store transform.position into startPosition.
        //         Why: if you bob relative to the *current* position every frame, the error accumulates
        //         and the shard slowly drifts away. Always bob around a stored anchor.
        startPosition = transform.position; // placeholder that compiles; keep or replace
    }

    void Update()
    {
        // TODO 1: Spin. Rotate this transform around its local up axis by degreesPerSecond * Time.deltaTime.
        //         Look at: Transform.Rotate(Vector3 axis, float angle, Space relativeTo) with Space.Self.
        //         Why Time.deltaTime: the rotation per frame must shrink when frames come faster, so the
        //         shard turns at the same speed at 72 Hz on the Quest and at 144 Hz on a gaming laptop.
        //         Check: the shard spins smoothly in Play mode; changing Degrees Per Second in the Inspector
        //         while playing changes the speed immediately.

        // TODO 3: Bob. Compute a new y from a sine wave and write it back, leaving x and z untouched:
        //             y = startPosition.y + bobAmplitude * Mathf.Sin(2f * Mathf.PI * bobFrequency * Time.time)
        //         Then: transform.position = new Vector3(startPosition.x, y, startPosition.z);
        //         Look at: Mathf.Sin, Mathf.PI, Time.time (seconds since the scene started).
        //         Check: the shard rises and falls about 5 cm every two seconds and never drifts sideways.
    }
}

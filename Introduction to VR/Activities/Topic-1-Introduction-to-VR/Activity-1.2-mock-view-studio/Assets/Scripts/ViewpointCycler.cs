// ViewpointCycler.cs — Activity 1.2: Mock View Studio
// Snaps the player's viewpoint to preset "camera stations" on a key press so you can compose mock views
// from the exact spot a player would stand. It moves the WHOLE XR Origin (the rig), never the camera:
// the camera's transform is overwritten by head tracking every frame, so writing to it does nothing
// useful on a headset and, in the simulator, desynchronizes the head from the controllers.
// Attached to: "Viewpoint Cycler" in the starter scene (Stations and Station Label are pre-set).
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

public class ViewpointCycler : MonoBehaviour
{
    [Header("Stations")]
    [Tooltip("Floor-level markers, in order. Their forward (+z, the blue arrow) is the direction the player should face on arrival.")]
    public Transform[] stations;

    [Tooltip("Which station to jump to when Play starts. -1 leaves the rig where the scene placed it.")]
    public int startStation = 0;

    [Header("Keys (Input System)")]
    [Tooltip("Key that jumps to the next station. Change it here if the XR Interaction Simulator already uses it.")]
    public Key nextKey = Key.RightBracket;

    [Tooltip("Key that jumps to the previous station.")]
    public Key previousKey = Key.LeftBracket;

    [Header("Readout")]
    [Tooltip("A TextMesh that shows the current station's name. The cycler places it in front of you on arrival.")]
    public TextMesh stationLabel;

    [Tooltip("How far in front of the eyes the station label is placed, in meters. 1.5 m is inside the comfortable reading zone.")]
    public float labelDistance = 1.5f;

    /// <summary>Index of the station the rig currently stands on (-1 before the first jump).</summary>
    public int CurrentIndex { get; private set; }

    XROrigin xrOrigin;
    Transform head;

    void Start()
    {
        CurrentIndex = -1;

        // TODO 1: Find the rig and its camera.
        //         xrOrigin = Object.FindFirstObjectByType<XROrigin>();  (using Unity.XR.CoreUtils)
        //         head = xrOrigin != null ? xrOrigin.Camera.transform : null;
        //         Why find it at runtime: the rig is a prefab instance that the builder drops in; a scene
        //         reference would break the moment someone rebuilds the scene.
        //         Then, if startStation is a valid index, call GoToStation(startStation).
        //         Check: on Play you are standing on Station 1 looking through the doorway, not at the origin.
    }

    void Update()
    {
        // TODO 2: Read the keyboard through the Input System and step through the stations.
        //         var kb = Keyboard.current; if (kb == null) return;      // no keyboard attached (headset build)
        //         if (kb[nextKey].wasPressedThisFrame)     GoToStation(CurrentIndex + 1);
        //         if (kb[previousKey].wasPressedThisFrame) GoToStation(CurrentIndex - 1);
        //         Look at: UnityEngine.InputSystem.Keyboard.current, the Keyboard indexer this[Key], KeyControl.wasPressedThisFrame.
        //         Why not the legacy Input class: this project runs the new Input System; the old calls throw or return false.
        //         Check: pressing ] and [ moves you between stations; holding the key does NOT keep jumping.
    }

    /// <summary>
    /// Moves the XR Origin so the player's eyes end up above the station marker, facing the marker's forward.
    /// Indices wrap around, so GoToStation(stations.Length) lands on station 0.
    /// </summary>
    public void GoToStation(int index)
    {
        if (stations == null || stations.Length == 0 || xrOrigin == null || head == null) return;

        // Wrap the index so cycling past the last station returns to the first.
        index = ((index % stations.Length) + stations.Length) % stations.Length;
        Transform station = stations[index];
        if (station == null) return;

        // TODO 3: Position. We want the HEAD over the marker, but we may only move the RIG. The head sits at some
        //         horizontal offset from the rig root (you walked around, or the simulator moved the head), so:
        //             Vector3 headOffset = head.position - xrOrigin.transform.position;   // where the head is relative to the rig
        //             headOffset.y = 0f;                                                  // keep the rig on the floor (y = 0)
        //             xrOrigin.transform.position = station.position - headOffset;
        //         Why y = 0: the XR Origin's y is the FLOOR. Eye height comes from tracking (or the simulator's 1.6 m);
        //         if you added the head's y here the player would float 1.6 m above the ground.
        //         Check: after a jump, the Probe-style readout from 1.1 would show eye height unchanged (about 1.6 m),
        //         and looking straight down you see the station's floor disc under you.

        // TODO 4: Facing. Rotate the rig about the vertical axis so the head's forward matches the station's forward.
        //         Flatten both forwards onto the floor plane and measure their headings with atan2:
        //             Vector3 f = head.forward;     f.y = 0f;
        //             Vector3 g = station.forward;  g.y = 0f;
        //             float headYaw    = Mathf.Atan2(f.x, f.z) * Mathf.Rad2Deg;
        //             float stationYaw = Mathf.Atan2(g.x, g.z) * Mathf.Rad2Deg;
        //             xrOrigin.transform.RotateAround(head.position, Vector3.up, stationYaw - headYaw);
        //         Why RotateAround the HEAD and not the rig pivot: the head is not at the rig's pivot, so rotating the
        //         rig about its own pivot would swing the head sideways off the marker you just placed it on.
        //         Look at: Transform.RotateAround(Vector3 point, Vector3 axis, float angle), Mathf.Atan2.
        //         Check: arriving at Station 1 you look straight at the doorway; at Station 5 you look diagonally at the hut.

        CurrentIndex = index;
        PlaceLabel(station.name);
    }

    /// <summary>Places the station label in front of the eyes and writes the station name into it.</summary>
    void PlaceLabel(string stationName)
    {
        if (stationLabel == null || head == null) return;

        // TODO 5: Put the label labelDistance meters in front of the eyes, slightly below eye level, facing the head.
        //             Vector3 forward = head.forward; forward.y = 0f; forward.Normalize();
        //             stationLabel.transform.position = head.position + forward * labelDistance + Vector3.down * 0.25f;
        //             stationLabel.transform.rotation = Quaternion.LookRotation(forward);   // TextMesh reads from its -z side
        //         Why world-locked rather than a child of the camera: a label glued to the head cannot be looked at, only stared
        //         through; placing it in the world lets you glance at it and then look away, like a sign.
        //         Also set stationLabel.text = stationName.
        //         Check: each jump drops a readable sign 1.5 m ahead of you with the station's name; walk toward it and it stays put.
        stationLabel.text = stationName;
    }
}

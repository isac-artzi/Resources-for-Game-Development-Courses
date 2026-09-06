// StressSpawner.cs — Activity 2.4: Build to Quest and Measure
// Adds N lit "wisp" cubes to the sky on each press, so you can watch frame time climb with object count. A toggle
// switches between one shared material (batchable) and a unique material instance per cube (one draw call each) —
// the single most common performance mistake in student VR projects, made visible.
// Attached to: Perf Tools (an empty GameObject). Spawn center and the shared material are pre-wired.
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StressSpawner : MonoBehaviour
{
    [Header("Where")]
    [Tooltip("Center of the cloud of cubes. The starter scene puts it 3.5 m up and 6 m ahead so it stays in view.")]
    public Transform spawnCenter;

    [Tooltip("Radius of the spherical cloud in meters.")]
    public float cloudRadius = 3.5f;

    [Tooltip("Edge length of each cube in meters.")]
    public float cubeSize = 0.2f;

    [Header("How many")]
    [Tooltip("Cubes added per press.")]
    public int batchSize = 50;

    [Tooltip("Safety cap so a stuck key cannot freeze the headset.")]
    public int maxObjects = 4000;

    [Header("Materials")]
    [Tooltip("Material used by every cube when Unique Materials is off (lets Unity batch them).")]
    public Material sharedMaterial;

    [Tooltip("When on, each cube gets its own material instance with a random color — one draw call per cube.")]
    public bool uniqueMaterials = false;

    [Header("Input (desktop keys use the Input System)")]
    public Key spawnKey = Key.B;
    public Key clearKey = Key.C;
    public Key toggleUniqueKey = Key.U;

    [Tooltip("Optional controller button for the headset (e.g. a Primary Button action). Leave empty on desktop.")]
    public InputActionReference spawnAction;

    [Header("Headset without buttons")]
    [Tooltip("When on, a batch is spawned automatically every Auto Ramp Interval seconds — handy on the headset.")]
    public bool autoRamp = false;
    public float autoRampInterval = 3f;

    readonly List<GameObject> spawned = new List<GameObject>();
    float autoTimer;

    /// <summary>How many stress cubes currently exist.</summary>
    public int Count { get { return spawned.Count; } }

    void Start()
    {
        if (spawnAction != null && spawnAction.action != null) spawnAction.action.Enable();
    }

    void Update()
    {
        // TODO 1: Read input with the Input System (never the legacy Input class) and act:
        //             var kb = Keyboard.current;
        //             bool spawn  = (kb != null && kb[spawnKey].wasPressedThisFrame)
        //                        || (spawnAction != null && spawnAction.action != null && spawnAction.action.WasPressedThisFrame());
        //             if (spawn) SpawnBatch();
        //             if (kb != null && kb[clearKey].wasPressedThisFrame) Clear();
        //             if (kb != null && kb[toggleUniqueKey].wasPressedThisFrame) uniqueMaterials = !uniqueMaterials;
        //         Then the auto ramp:
        //             if (autoRamp) { autoTimer += Time.unscaledDeltaTime;
        //                             if (autoTimer >= autoRampInterval) { autoTimer = 0f; SpawnBatch(); } }
        //         Look at: Keyboard.current (null on the headset — always null-check), KeyControl.wasPressedThisFrame.
        //         Check: B adds 50 cubes to the cloud, C removes them all, U flips Unique Materials in the Inspector.
    }

    /// <summary>Adds batchSize cubes at random points inside the cloud.</summary>
    public void SpawnBatch()
    {
        // TODO 2: For i in 0..batchSize-1, while Count < maxObjects:
        //             var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //             cube.name = "Wisp";
        //             cube.transform.SetParent(transform, true);
        //             cube.transform.position = spawnCenter.position + Random.insideUnitSphere * cloudRadius;
        //             cube.transform.rotation = Random.rotation;
        //             cube.transform.localScale = Vector3.one * cubeSize;
        //             Destroy(cube.GetComponent<Collider>());              // rendering test, not a physics test
        //             var r = cube.GetComponent<Renderer>();
        //             if (uniqueMaterials) { r.material = sharedMaterial;  r.material.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.8f, 1f); }
        //             else                 { r.sharedMaterial = sharedMaterial; }
        //             spawned.Add(cube);
        //         Look at: GameObject.CreatePrimitive, Random.insideUnitSphere, Random.rotation, Random.ColorHSV.
        //         Why 'r.material' creates a cost: reading Renderer.material clones the material for that renderer, so
        //         Unity can no longer batch it with its neighbours. Renderer.sharedMaterial keeps them all identical.
        //         Check: the Stats overlay (Game view > Stats) shows Batches barely moving with shared materials and
        //         climbing by ~50 per press with unique ones.
        Debug.Log("SpawnBatch: implement TODO 2 (count = " + Count + ")");
    }

    /// <summary>Removes every spawned cube (and the material instances they may own).</summary>
    public void Clear()
    {
        // TODO 3: foreach cube in spawned: if it has a Renderer whose sharedMaterial is not our sharedMaterial, Destroy that
        //         material instance first (Destroy(r.sharedMaterial)) — unique instances are assets in memory and leak
        //         otherwise; then Destroy(cube). Finally spawned.Clear().
        //         Look at: Object.Destroy (deferred to end of frame — do not read the object afterwards).
        //         Check: C empties the cloud; the HUD's object count returns to 0 and frame time drops back.
    }
}

// DarkModeController.cs — Activity 6.4: Dark Mode
// Turns the lights out: on a key or controller button the camera clears to solid black and every MeshRenderer is
// hidden except the player's own (hands/controllers) — then restores it all when toggled back. While dark it also
// judges the run: a chime when the head reaches the objective, a buzz when it strays into a hazard.
// Attached to: Dark Mode in the starter scene.
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DarkModeController : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Desktop key that toggles the blackout.")]
    public Key toggleKey = Key.K;

    [Tooltip("Controller action for the headset (e.g. the left-hand Menu or Primary Button). Leave empty on desktop.")]
    public InputActionReference toggleAction;

    [Tooltip("Start the scene already dark.")]
    public bool startDark = false;

    [Header("What stays visible")]
    [Tooltip("Renderers under this transform are never hidden — set to the XR rig so hands and controllers remain.")]
    public Transform keepVisibleRoot;

    [Tooltip("Extra renderers to keep (e.g. a wrist readout).")]
    public Renderer[] alsoKeepVisible;

    [Header("Objective and hazards")]
    [Tooltip("The thing to reach. Set by the builder (Artifact Shard).")]
    public Transform objective;

    [Tooltip("Horizontal distance (m) from the head at which the objective counts as reached.")]
    public float objectiveRadius = 0.9f;

    [Tooltip("Places to avoid. Set by the builder (Void Pits).")]
    public Transform[] hazards;

    [Tooltip("Horizontal distance (m) at which a hazard counts as touched.")]
    public float hazardRadius = 0.8f;

    [Tooltip("Do not count the same hazard again for this many seconds.")]
    public float hazardCooldown = 2f;

    [Header("Feedback")]
    [Tooltip("2D AudioSource for the chime and the buzz. Set by the builder.")]
    public AudioSource feedback;

    [Tooltip("Placeholder chime frequency (objective reached).")]
    public float chimeHz = 1320f;

    [Tooltip("Placeholder buzz frequency (hazard touched).")]
    public float buzzHz = 90f;

    [Tooltip("Shows dark state, hazard count and result. Set by the builder.")]
    public TextMesh readout;

    /// <summary>True while the blackout is active.</summary>
    public bool IsDark { get; private set; }

    /// <summary>True once the head has reached the objective during this dark run.</summary>
    public bool ObjectiveReached { get; private set; }

    /// <summary>Hazards touched during this dark run.</summary>
    public int HazardHits { get; private set; }

    /// <summary>Seconds spent in the dark during this run.</summary>
    public float DarkSeconds { get; private set; }

    Camera cam;
    Transform head;
    CameraClearFlags savedClearFlags;
    Color savedBackground;
    bool savedFog;
    readonly List<MeshRenderer> hidden = new List<MeshRenderer>();
    float lastHazardTime = -10f;
    AudioClip chime, buzz;

    void Start()
    {
        cam = Camera.main;
        if (cam != null) head = cam.transform;
        if (toggleAction != null && toggleAction.action != null) toggleAction.action.Enable();
        chime = SonarPing.MakeTone(chimeHz, 0.6f, true);
        buzz = SonarPing.MakeTone(buzzHz, 0.4f, true);
        if (startDark) SetDark(true);
    }

    void Update()
    {
        if (cam == null) return;

        // TODO 1: Toggle on input (Input System only — never the legacy Input class).
        //             bool pressed = Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame;
        //             if (toggleAction != null && toggleAction.action != null && toggleAction.action.WasPressedThisFrame()) pressed = true;
        //             if (pressed) SetDark(!IsDark);
        //         Look at: Keyboard.current[Key] (a KeyControl), ButtonControl.wasPressedThisFrame.
        //         Check: pressing K blacks out the view (once TODO 2 is done) and pressing K again brings it back.

        if (IsDark)
        {
            DarkSeconds += Time.deltaTime;
            CheckObjectiveAndHazards();   // body is TODO 4
        }

        if (readout != null)
        {
            readout.text = (IsDark ? "DARK  " + DarkSeconds.ToString("F0") + " s" : "Lights on") +
                           "\nhazards touched " + HazardHits +
                           (ObjectiveReached ? "\nOBJECTIVE REACHED" : "");
        }
    }

    /// <summary>Applies or removes the blackout.</summary>
    public void SetDark(bool dark)
    {
        if (dark == IsDark || cam == null) return;
        IsDark = dark;

        if (dark)
        {
            ObjectiveReached = false;
            HazardHits = 0;
            DarkSeconds = 0f;

            // TODO 2: Black out the camera and remember what it was.
            //             savedClearFlags = cam.clearFlags;  savedBackground = cam.backgroundColor;  savedFog = RenderSettings.fog;
            //             cam.clearFlags = CameraClearFlags.SolidColor;
            //             cam.backgroundColor = Color.black;
            //             RenderSettings.fog = false;
            //         Look at: Camera.clearFlags, Camera.backgroundColor, RenderSettings.fog. Why SolidColor: the default Skybox
            //         flag would still paint the sky.
            //         Check: with only TODO 2 done the sky goes black but lit objects still show — that is TODO 3's job.

            // TODO 3: Hide every MeshRenderer that is not part of the player, and remember which ones you hid.
            //             hidden.Clear();
            //             foreach (var r in FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
            //             {
            //                 if (!r.enabled) continue;
            //                 if (keepVisibleRoot != null && r.transform.IsChildOf(keepVisibleRoot)) continue;
            //                 if (IsInKeepList(r)) continue;
            //                 r.enabled = false;
            //                 hidden.Add(r);
            //             }
            //         Look at: Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None), Transform.IsChildOf.
            //         Why MeshRenderer and not Renderer: TextMesh labels are MeshRenderers too (so they vanish, good), while
            //         the controller models under the rig are skipped by the IsChildOf test. Why remember: only re-enable what
            //         YOU disabled — a renderer that was already off must stay off.
            //         Check: press K — total darkness except your controllers; the readout label disappears too, so watch the
            //         Console or listen. Press K again: everything returns exactly as before.
        }
        else
        {
            // Restore (works as soon as TODO 2–3 fill the saved fields and the hidden list).
            cam.clearFlags = savedClearFlags;
            cam.backgroundColor = savedBackground;
            RenderSettings.fog = savedFog;
            foreach (var r in hidden) if (r != null) r.enabled = true;
            hidden.Clear();
        }
    }

    bool IsInKeepList(Renderer r)
    {
        if (alsoKeepVisible == null) return false;
        for (int i = 0; i < alsoKeepVisible.Length; i++) if (alsoKeepVisible[i] == r) return true;
        return false;
    }

    /// <summary>Horizontal (XZ) distance from the head to a target — leaning down should not "reach" a pit.</summary>
    float HorizontalDistance(Transform target)
    {
        if (head == null || target == null) return float.MaxValue;
        Vector3 a = head.position; a.y = 0f;
        Vector3 b = target.position; b.y = 0f;
        return Vector3.Distance(a, b);
    }

    /// <summary>Judges the run: chime once at the objective, buzz (with cooldown) at hazards.</summary>
    void CheckObjectiveAndHazards()
    {
        // TODO 4: Objective first, then hazards.
        //             if (!ObjectiveReached && HorizontalDistance(objective) <= objectiveRadius)
        //             {
        //                 ObjectiveReached = true;
        //                 if (feedback != null) feedback.PlayOneShot(chime, 1f);
        //                 Debug.Log("[DarkMode] Objective reached after " + DarkSeconds.ToString("F1") + " s with " + HazardHits + " hazard hits");
        //             }
        //             if (hazards != null && Time.time - lastHazardTime >= hazardCooldown)
        //                 foreach (var h in hazards)
        //                     if (HorizontalDistance(h) <= hazardRadius)
        //                     {
        //                         HazardHits++;
        //                         lastHazardTime = Time.time;
        //                         if (feedback != null) feedback.PlayOneShot(buzz, 1f);
        //                         break;
        //                     }
        //         Why horizontal distance: the head is 1.6 m above the pit; a 3D distance would never get within 0.8 m.
        //         Why the cooldown: standing in a pit should register once, not 72 times a second.
        //         Check: in the dark, walk into a pit — one buzz, then silence for 2 s; reach the shard — one chime and the
        //         Console line with your time and hit count. Turn the lights on to read the label.
    }
}

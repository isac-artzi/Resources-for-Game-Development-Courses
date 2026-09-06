// BugReporter.cs — Activity 7.3: Playtest Telemetry and Heatmap
// One button, one report: saves a screenshot, appends a CSV line (time, scene, head position, yaw) and drops
// a red marker where the tester stood, so that "it looked wrong over there" becomes a file you can open.
// Attached to: "Telemetry" in the starter scene. Desktop key: B. On the headset: assign an InputActionReference
// (e.g. XRI Right Interaction/Primary Button) — the keyboard still works as a fallback.
// Created by Isac Artzi

using System.Collections;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BugReporter : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Desktop key that files a report.")]
    public Key reportKey = Key.B;

    [Tooltip("Optional controller button for the headset (e.g. the right controller's primary button). Leave empty on desktop.")]
    public InputActionReference reportAction;

    [Header("Output")]
    [Tooltip("CSV file (inside persistentDataPath) that gets one line per report.")]
    public string fileName = "bugs.csv";

    [Tooltip("Prefix for screenshot files, e.g. bug_20260421_140309.png")]
    public string screenshotPrefix = "bug_";

    [Header("Feedback")]
    [Tooltip("Small label in front of the camera that confirms the report for a moment.")]
    public TextMesh confirmLabel;

    [Tooltip("Seconds the confirmation stays visible.")]
    public float confirmSeconds = 2f;

    [Header("Read-only")]
    public int reportCount;

    Transform head;

    void OnEnable()
    {
        if (reportAction != null && reportAction.action != null) reportAction.action.Enable();
    }

    void Start()
    {
        if (Camera.main != null) head = Camera.main.transform;
        if (confirmLabel != null) confirmLabel.text = "";
    }

    void Update()
    {
        // TODO 1: Detect the press. Keyboard: var kb = Keyboard.current; if (kb != null && kb[reportKey].wasPressedThisFrame) Report();
        //         Controller: if (reportAction != null && reportAction.action != null && reportAction.action.WasPressedThisFrame()) Report();
        //         Look at: Keyboard.current[Key] (a KeyControl), ButtonControl.wasPressedThisFrame, InputAction.WasPressedThisFrame.
        //         Why both: the keyboard makes it testable in the simulator today; the action makes it usable on the Quest.
        //         Check: press B in Play mode — the Console prints "[BugReporter] report #1 ..." once per press, not once per frame.
    }

    /// <summary>Files one report: screenshot + CSV line + marker + confirmation.</summary>
    public void Report()
    {
        reportCount++;
        string stamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        Vector3 pos = head != null ? head.position : Vector3.zero;
        float yaw = head != null ? head.eulerAngles.y : 0f;

        // TODO 2: Screenshot. string shot = Path.Combine(Application.persistentDataPath, screenshotPrefix + stamp + ".png");
        //         ScreenCapture.CaptureScreenshot(shot);
        //         Look at: ScreenCapture.CaptureScreenshot(string path) — it captures at the END of the frame, so the
        //         file appears a moment later; in VR it captures the eye buffer (what the tester saw).
        //         Check: the .png appears in persistentDataPath and shows the Game view at the moment of the press.

        // TODO 3: CSV line. Build "time,scene,x,y,z,yaw,screenshot" — write the header first if the file does not exist:
        //             string path = Path.Combine(Application.persistentDataPath, fileName);
        //             if (!File.Exists(path)) File.AppendAllText(path, "time,scene,x,y,z,yaw,screenshot\n");
        //             File.AppendAllText(path, stamp + "," + SceneManager.GetActiveScene().name + "," + F(pos.x) + "," + F(pos.y) + "," + F(pos.z) + "," + F(yaw) + "," + shotFileName + "\n");
        //         (F() below formats a float with the invariant culture.)
        //         Look at: File.AppendAllText — append, so ten reports make ten lines, not one overwritten file.
        //         Check: open bugs.csv in a spreadsheet: one header row plus one row per press.

        // TODO 4: Marker + confirmation. Call DropMarker(pos) and StartCoroutine(Confirm("Bug #" + reportCount + " saved")).
        //         Why: in VR the tester cannot see the Console. Feedback must be in the world, at the moment it happens.
        //         Check: a small red sphere appears at head height where you stood and the label reads "Bug #1 saved" for 2 s.

        Debug.Log("[BugReporter] report #" + reportCount + " at " + pos + " — implement TODO 2-4");
    }

    static string F(float v)
    {
        return v.ToString("F3", CultureInfo.InvariantCulture);
    }

    /// <summary>A small red sphere, no collider, at the reported position.</summary>
    public void DropMarker(Vector3 pos)
    {
        var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = "Bug Marker " + reportCount;
        marker.transform.position = pos;
        marker.transform.localScale = Vector3.one * 0.15f;
        Destroy(marker.GetComponent<Collider>());
        marker.GetComponent<Renderer>().material.color = Color.red;
    }

    IEnumerator Confirm(string text)
    {
        if (confirmLabel == null) yield break;
        confirmLabel.text = text;
        yield return new WaitForSeconds(confirmSeconds);
        confirmLabel.text = "";
    }
}

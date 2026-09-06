// ScreenshotTool.cs — Activity 7.6: Build, Record, Present
// Takes clean, timestamped screenshots for the presentation slides and the GDD: hides the HUD objects for one
// frame, captures at 1x or 2x resolution, saves to persistentDataPath, and confirms in the world.
// Attached to: "Demo Tools" in every scene. Desktop key: P. Headset: assign an InputActionReference.
// Created by Isac Artzi

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScreenshotTool : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Desktop key.")]
    public Key captureKey = Key.P;

    [Tooltip("Optional controller button for the headset (e.g. XRI Left Interaction / Primary Button).")]
    public InputActionReference captureAction;

    [Header("Output")]
    [Tooltip("File name prefix; the scene name and a timestamp are appended.")]
    public string prefix = "demo_";

    [Tooltip("1 = native resolution, 2 = double (crisp for slides; only works on desktop).")]
    [Range(1, 4)] public int superSize = 1;

    [Header("Clean frame")]
    [Tooltip("Objects hidden for the captured frame (the timer label, the menu, this tool's own label).")]
    public GameObject[] hideDuringCapture = new GameObject[0];

    [Tooltip("Label that confirms the capture for a moment.")]
    public TextMesh confirmLabel;

    [Header("Read-only")]
    public int captured;

    void OnEnable()
    {
        if (captureAction != null && captureAction.action != null) captureAction.action.Enable();
    }

    void Update()
    {
        // TODO 1: var kb = Keyboard.current; bool pressed = kb != null && kb[captureKey].wasPressedThisFrame;
        //         if (captureAction != null && captureAction.action != null && captureAction.action.WasPressedThisFrame()) pressed = true;
        //         if (pressed) StartCoroutine(Capture());
        //         Check: pressing P prints "[ScreenshotTool] saved ..." once per press.
    }

    /// <summary>Full path for the next screenshot.</summary>
    public string NextPath()
    {
        string stamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return Path.Combine(Application.persistentDataPath, prefix + SceneManager.GetActiveScene().name + "_" + stamp + ".png");
    }

    IEnumerator Capture()
    {
        // TODO 2: Hide the clutter: foreach go in hideDuringCapture: if (go != null) go.SetActive(false);
        //         yield return null;                          // let one frame render without them
        //         string path = NextPath();
        //         ScreenCapture.CaptureScreenshot(path, superSize);
        //         yield return new WaitForEndOfFrame();       // the capture happens at end of frame
        //         yield return null;
        //         then re-enable everything and captured++.
        //         Look at: ScreenCapture.CaptureScreenshot(string, int superSize). Why hide first: a slide with a debug timer
        //         in the corner looks unfinished; capture the world, not the tooling.
        //         Check: the PNG shows the scene without the timer or menu; at superSize 2 it is twice the Game view's size.

        // TODO 3: Confirm: Debug.Log("[ScreenshotTool] saved " + path); if (confirmLabel != null) { confirmLabel.text = "Saved " + Path.GetFileName(path);
        //         yield return new WaitForSeconds(1.5f); confirmLabel.text = ""; }
        //         Check: the label under your view names the file for 1.5 s, then clears.
        Debug.Log("[ScreenshotTool] would save " + NextPath() + " — implement TODO 2-3");
        yield return null;
    }
}

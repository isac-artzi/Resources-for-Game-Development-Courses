// ListenerCheck.cs — Activity 6.1: Hear the Forest
// Audits the scene's ears: there must be exactly one AudioListener and it must ride on the player's head
// (the XR camera). Prints the result on a label, warns in the Console, and then shows live which 3D source
// should currently be the loudest — a quick way to test your rolloff settings by ear.
// Attached to: Listener Check (a floating label near the start position).
// Created by Isac Artzi

using UnityEngine;

public class ListenerCheck : MonoBehaviour
{
    [Header("Where to print")]
    [Tooltip("The TextMesh that shows the audit. The starter scene assigns the one on this GameObject.")]
    public TextMesh readout;

    [Header("What to compare")]
    [Tooltip("The spatialized sources to rank by loudness. The builder fills this with Stream, Bird and Fire.")]
    public RolloffVisualizer[] sources;

    [Tooltip("Seconds between updates of the loudness ranking (no need to do it every frame).")]
    public float refreshSeconds = 0.25f;

    AudioListener listener;
    string auditLine = "Listener audit: implement TODO 1-3 in ListenerCheck.cs";
    float nextRefresh;

    void Start()
    {
        if (readout == null) readout = GetComponent<TextMesh>();
        listener = FindFirstObjectByType<AudioListener>();   // the ranking below needs one even before the audit is written

        // TODO 1: Count the listeners.
        //             AudioListener[] all = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        //             int count = all.Length;
        //             if (count > 0) listener = all[0];
        //         Look at: Object.FindObjectsByType<T>(FindObjectsSortMode). Why it matters: Unity mixes audio for
        //         ONE listener; a second one (a leftover Main Camera, a debug camera) makes every 3D sound play from
        //         the wrong place or double up. The Console shows "There are 2 audio listeners in the scene".
        //         Check: count is 1 with the Starter Assets rig. Add a Camera to an empty GameObject and it reads 2.

        // TODO 2: Is the listener on the head?
        //             bool onHead = listener != null && Camera.main != null && listener.gameObject == Camera.main.gameObject;
        //         Look at: Camera.main (the enabled camera tagged MainCamera — the XR Origin's camera).
        //         Why: spatial audio is computed relative to the listener; if it sits on the XR Origin's root at floor
        //         level instead of the camera, sounds pan correctly but distances and heights are all off by 1.6 m.

        // TODO 3: Report.
        //             auditLine = "Listeners: " + count + (count == 1 ? "  OK" : "  PROBLEM") +
        //                         "\nOn head: " + (onHead ? "yes" : "NO");
        //             if (count != 1 || !onHead) Debug.LogWarning("[ListenerCheck] " + auditLine.Replace("\n", " | "));
        //         Check: the label near your start position reads "Listeners: 1  OK / On head: yes".
    }

    void Update()
    {
        if (readout == null) return;
        if (Time.time < nextRefresh) return;
        nextRefresh = Time.time + Mathf.Max(0.05f, refreshSeconds);

        string ranking = "";

        // TODO 4: Rank the sources by the gain the listener should be hearing right now.
        //         For each RolloffVisualizer s in sources (skip null):
        //             var src = s.GetComponent<AudioSource>();
        //             float d = Vector3.Distance(listener.transform.position, s.transform.position);
        //             float gain = RolloffVisualizer.LogarithmicGain(Mathf.Min(d, src.maxDistance), src.minDistance) * src.volume;
        //         Remember the loudest one's name and append a line per source:
        //             ranking += "\n" + s.name + ": " + gain.ToString("F2");
        //         Then prefix "Loudest: <name>". Guard: if listener == null, skip all of this.
        //         Why multiply by src.volume: the Inspector Volume slider scales the whole curve.
        //         Check: walk from the fire to the stream — "Loudest" flips from Fire to Stream at about the point where
        //         you also hear it flip. If your ears and the label disagree, one of the Min Distance values is off.

        readout.text = auditLine + ranking;
    }
}

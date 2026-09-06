// DemoTimer.cs — Activity 7.6: Build, Record, Present
// A countdown in the corner of your view so the presenter in the headset knows how much demo time is left
// without anyone shouting. Turns yellow in the last minute and shows "+mm:ss" in red once over.
// Attached to: "Timer Label" (a TextMesh parented to the XR camera) in every scene. Desktop key: T starts/pauses, Y resets.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.InputSystem;

public class DemoTimer : MonoBehaviour
{
    [Header("Time")]
    [Tooltip("Length of the demo slot in minutes.")]
    public float totalMinutes = 5f;

    [Tooltip("Seconds left at which the label turns yellow.")]
    public float warnSeconds = 60f;

    [Header("Keys")]
    public Key startPauseKey = Key.T;
    public Key resetKey = Key.Y;

    [Header("Look")]
    [Tooltip("Label to write into. Empty = the TextMesh on this GameObject.")]
    public TextMesh label;
    public Color okColor = new Color(0.8f, 0.9f, 1f);
    public Color warnColor = new Color(1f, 0.9f, 0.3f);
    public Color overColor = new Color(1f, 0.35f, 0.3f);

    [Header("Read-only")]
    public bool running;
    public float secondsLeft;

    void Start()
    {
        if (label == null) label = GetComponent<TextMesh>();
        secondsLeft = totalMinutes * 60f;
        Render();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb != null)
        {
            // TODO 1: if (kb[startPauseKey].wasPressedThisFrame) running = !running;
            //         if (kb[resetKey].wasPressedThisFrame) { running = false; secondsLeft = totalMinutes * 60f; }
            //         Check: T starts the countdown, T again freezes it, Y puts it back to 05:00.
        }

        // TODO 2: if (running) secondsLeft -= Time.unscaledDeltaTime;   (unscaled: a paused game must not pause the clock —
        //         the audience's time keeps running whatever your Time.timeScale does)
        //         then Render().
        //         Check: the label counts down once per second while running.
    }

    /// <summary>Formats the remaining (or overrun) time and colors the label.</summary>
    public void Render()
    {
        if (label == null) return;
        // TODO 3: float s = Mathf.Abs(secondsLeft); string mmss = Mathf.FloorToInt(s / 60f).ToString("00") + ":" + Mathf.FloorToInt(s % 60f).ToString("00");
        //         if (secondsLeft < 0f) { label.text = "+" + mmss; label.color = overColor; }
        //         else { label.text = mmss; label.color = secondsLeft < warnSeconds ? warnColor : okColor; }
        //         Look at: Mathf.FloorToInt, the "00" numeric format string (zero-padded two digits).
        //         Check: 05:00 pale blue, 00:59 yellow, then +00:01 red counting UP.
        label.text = "Timer: implement DemoTimer TODO 1-3";
    }
}

// StoryBeatSequencer.cs — Activity 1.3: Story Beats in Space
// Holds your storyline as an ordered list of StoryBeats and walks through it on a key press: it dims the previous
// station, lights the new one, and hands the Guide her line. This is the spine of your concept video — a spatial
// storyboard you can walk through and narrate.
// Attached to: "Story Sequencer" in the starter scene (Beats and Guide are pre-set; replace the sample text with yours).
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.InputSystem;

public class StoryBeatSequencer : MonoBehaviour
{
    [Header("Story")]
    [Tooltip("The beats in the order the player should experience them. Edit the titles and Guide lines to match your GDD.")]
    public StoryBeat[] beats;

    [Tooltip("The Guide who speaks each beat's line.")]
    public GuideVoice guide;

    [Header("Keys (Input System)")]
    [Tooltip("Advance to the next beat.")]
    public Key nextKey = Key.RightBracket;

    [Tooltip("Go back one beat.")]
    public Key previousKey = Key.LeftBracket;

    [Tooltip("Restart from before the first beat (all stations dim).")]
    public Key restartKey = Key.Backspace;

    [Header("Playback")]
    [Tooltip("When true, beats advance on their own after each beat's Hold Seconds (Stretch goal).")]
    public bool autoAdvance = false;

    /// <summary>Index of the active beat, or -1 before the story starts.</summary>
    public int CurrentIndex { get; private set; }

    void Start()
    {
        CurrentIndex = -1;

        // TODO 1: Prime the stations. For every beat with a station: station.SetTitle(beat.title) so the labels show
        //         YOUR titles instead of the builder's placeholders, then station.SetActive(false) so everything starts dim.
        //         Look at: foreach over an array, null checks (a beat's Station may be unassigned).
        //         Check: on Play all five stations glow faintly and carry the titles from the Inspector's Beats list.
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // TODO 2: Keys. if (kb[nextKey].wasPressedThisFrame) Advance(+1); if (kb[previousKey].wasPressedThisFrame) Advance(-1);
        //         if (kb[restartKey].wasPressedThisFrame) Restart();
        //         Check: ] lights Station 1 and the Guide speaks; ] again moves to Station 2 and Station 1 dims; [ goes back.

        // TODO 5 (Stretch): if autoAdvance is on, count Time.deltaTime and call Advance(+1) once the active beat's
        //         holdSeconds have passed (you added holdSeconds to StoryBeat in its TODO 1). Stop at the last beat.
    }

    /// <summary>Moves the story by delta beats (usually +1 or -1), clamped to the list. Lights, dims, and speaks.</summary>
    public void Advance(int delta)
    {
        if (beats == null || beats.Length == 0) return;

        int next = Mathf.Clamp(CurrentIndex + delta, 0, beats.Length - 1);
        if (next == CurrentIndex) return;   // already at an end

        // TODO 3: Dim the station of the beat we are leaving (if CurrentIndex >= 0 and its station is not null):
        //             beats[CurrentIndex].station.SetActive(false);
        //         Then light the new one and remember where we are:
        //             CurrentIndex = next;
        //             var beat = beats[CurrentIndex];
        //             if (beat.station != null) beat.station.SetActive(true);
        //         Why dim first, then light: the eye follows the brightest change; if the new station lit first the
        //         player might catch the old one dimming and look back.
        //         Check: exactly one station is bright at any time; the Console logs beat.Summary().

        // TODO 4: Speak. if (guide != null && beat.station != null) guide.Say(beat.guideLine, beat.station.transform);
        //         (Say also turns the Guide toward the station — that is a second, subtler "look here" cue.)
        //         Check: the Guide's speech label types out the line for the active beat and the Guide turns to face the station.

        Debug.Log("[Story] advance requested -> " + next + " (implement TODO 3-4 in StoryBeatSequencer.cs)");
    }

    /// <summary>Dims every station and returns to the state before the first beat.</summary>
    public void Restart()
    {
        if (beats != null)
        {
            foreach (var b in beats)
                if (b != null && b.station != null) b.station.SetActive(false);
        }
        CurrentIndex = -1;
        if (guide != null) guide.Say("", null);
    }
}

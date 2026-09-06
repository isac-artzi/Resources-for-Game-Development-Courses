// StoryBeat.cs — Activity 1.3: Story Beats in Space
// A tiny data class: one beat of your storyline (a moment the player must experience), which act it belongs to,
// what the Guide says there, and which physical station in the scene it is staged at.
// Not a MonoBehaviour — the sequencer holds an array of these and you edit them in its Inspector.
// Created by Isac Artzi

using System;
using UnityEngine;

/// <summary>One story beat: a titled moment, the Guide's line for it, and the station where it is staged.</summary>
[Serializable]
public class StoryBeat
{
    /// <summary>The three acts of a classic storyline. Used to color the station label and to sanity-check your order.</summary>
    public enum Act { Beginning, Middle, End }

    [Tooltip("Short name of the moment, e.g. 'The Awakening'. Shown on the station label.")]
    public string title = "Untitled beat";

    [Tooltip("Which act this beat belongs to. Beats should run Beginning -> Middle -> End.")]
    public Act act = Act.Beginning;

    [Tooltip("What the Guide says when this beat becomes active. One or two sentences; it is read aloud in the player's head.")]
    [TextArea(2, 4)]
    public string guideLine = "...";

    [Tooltip("The station in the scene that stages this beat. The sequencer lights it and dims the others.")]
    public BeatStation station;

    // TODO 1: Add a public float field 'holdSeconds' (default 6f) with a [Tooltip]: how long the beat should stay active
    //         when the sequencer auto-advances (Stretch goal). Serializable fields show up in the Inspector automatically.
    //         Look at: the [Serializable] attribute above and how Unity draws public fields of a plain class inside an array.
    //         Check: the Story Sequencer's Beats list now shows a Hold Seconds slot for each beat.

    /// <summary>A one-line description for Console logging, e.g. "[Middle] The Mountain Pass".</summary>
    public string Summary()
    {
        // TODO 2: Return "[" + act + "] " + title. Enum.ToString() gives the act's name.
        //         Check: the Console line printed by the sequencer on each advance reads like "[Beginning] The Awakening".
        return title;
    }
}

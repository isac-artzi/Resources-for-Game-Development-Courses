// QuestStep.cs — Activity 7.2: Quests and Saves
// A single objective in a quest: an id the code uses, a title the player reads, and a state.
// This is a plain data class (not a MonoBehaviour). QuestManager keeps a List<QuestStep>, and because the
// class is [System.Serializable] the list shows up in the Inspector and JsonUtility can serialize it.
// Created by Isac Artzi

using UnityEngine;

[System.Serializable]
public class QuestStep
{
    /// <summary>Where a step is in its life. Locked = not yet available, Active = the current objective, Done = finished.</summary>
    public enum State
    {
        Locked = 0,
        Active = 1,
        Done = 2
    }

    [Tooltip("Stable id used by code and by the save file, e.g. take_shard. Never rename an id after players have save files.")]
    public string id = "step_id";

    [Tooltip("What the player reads on the wrist label, e.g. 'Take the shard from the pedestal'.")]
    public string title = "Untitled step";

    [Tooltip("Current state. The manager changes this; you should not need to edit it by hand.")]
    public State state = State.Locked;

    public QuestStep() { }

    public QuestStep(string id, string title)
    {
        this.id = id;
        this.title = title;
        this.state = State.Locked;
    }

    /// <summary>One character the label uses in front of the title: [ ] locked, [>] active, [x] done.</summary>
    public string Marker()
    {
        // TODO 1: Return "[ ]" for Locked, "[>]" for Active and "[x]" for Done (a switch on 'state').
        //         Why a method here and not in QuestManager: the step knows how to describe itself; the
        //         manager only lays the lines out. Keep knowledge next to the data it describes.
        //         Check: after QuestManager.RefreshLabel works, the wrist label shows three lines with markers.
        return "[?]";
    }

    /// <summary>True when the step is the current objective.</summary>
    public bool IsActive
    {
        get { return state == State.Active; }
    }

    // TODO 2 (stretch): Add an optional "hint" string that the Guide speaks when the step has been active for
    //         more than 60 s (QuestManager would track the time). Add the field here and a [Tooltip], then
    //         bump SaveData.version if you also store it in the save file.
}

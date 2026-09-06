// SaveData.cs — Activity 7.2: Quests and Saves
// Everything that goes into the save file, as one plain [System.Serializable] class. JsonUtility turns an
// instance of this class into text (JsonUtility.ToJson) and back (JsonUtility.FromJson<SaveData>).
// Rules JsonUtility enforces: public fields only (no properties, no Dictionary), and the class must be marked
// [System.Serializable]. Lists of strings, ints, and Vector3 all work.
// Used by: SaveSystem (writes/reads it) and QuestManager (fills it / applies it).
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [Tooltip("Bump this whenever you add, remove, or rename a field. SaveSystem refuses files with an older version.")]
    public int version = 1;

    [Tooltip("Ids of the quest steps, in order, so the states below can be matched even if you reorder steps later.")]
    public List<string> stepIds = new List<string>();

    [Tooltip("QuestStep.State of each step as an int (0 Locked, 1 Active, 2 Done), same order as stepIds.")]
    public List<int> stepStates = new List<int>();

    [Tooltip("World position of the shard when the game was saved, so it is where the player left it.")]
    public Vector3 shardPosition;

    [Tooltip("True if the shard was sitting in the altar socket when saved.")]
    public bool shardPlaced;

    // TODO 1: Add a timestamp:  public string savedAt;
    //         QuestManager.BuildSaveData() should fill it with System.DateTime.Now.ToString("s") (sortable ISO
    //         format, e.g. 2026-04-21T14:03:09). Show it on the Continue button's status line so a tester knows
    //         which save they are resuming.
    //         Look at: System.DateTime.Now, DateTime.ToString(string format).
    //         Check: open the .json file in a text editor — the timestamp is there and readable.

    // TODO 2: When you add savedAt, set version = 2 above and make SaveSystem.Load() treat version < 2 as
    //         "too old" (return null and log a warning). Why: a JSON file from last week has no savedAt field;
    //         JsonUtility would silently leave it empty, and other missing fields could leave the game in a
    //         half-valid state. A version number turns a silent mismatch into an explicit decision.
    //         Check: edit "version": 1 into your save file by hand — Continue is refused with a clear message.
}

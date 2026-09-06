// SaveSystem.cs — Activity 7.2: Quests and Saves
// Writes a SaveData object to disk as JSON and reads it back. Knows nothing about quests — it only moves a
// SaveData between memory and Application.persistentDataPath, which is the one folder that is writable on
// desktop AND on the Quest (on Android: /storage/emulated/0/Android/data/<package>/files/).
// Attached to: "Quest" in the starter scene, next to QuestManager.
// Created by Isac Artzi

using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    [Header("File")]
    [Tooltip("File name inside Application.persistentDataPath.")]
    public string fileName = "realm_save.json";

    [Tooltip("Pretty-print the JSON (indented). Larger file, but you can read it in class. Turn off for release.")]
    public bool prettyPrint = true;

    /// <summary>Full path of the save file.</summary>
    public string FullPath
    {
        get { return Path.Combine(Application.persistentDataPath, fileName); }
    }

    /// <summary>True if a save file exists on disk.</summary>
    public bool HasSave
    {
        get { return File.Exists(FullPath); }
    }

    /// <summary>Serializes 'data' to JSON and writes it to FullPath. Returns true on success.</summary>
    public bool Save(SaveData data)
    {
        if (data == null) return false;

        // TODO 1: Turn the object into text and write it:
        //             string json = JsonUtility.ToJson(data, prettyPrint);
        //             File.WriteAllText(FullPath, json);
        //             Debug.Log("[SaveSystem] saved " + json.Length + " chars to " + FullPath);
        //         Wrap the write in try { ... } catch (System.Exception e) { Debug.LogError(e.Message); return false; }
        //         Look at: JsonUtility.ToJson(object, bool prettyPrint), System.IO.File.WriteAllText.
        //         Why persistentDataPath: Application.dataPath is read-only in a build; persistentDataPath is not.
        //         Check: the Console prints the path; open that file in a text editor and read your quest states.
        Debug.Log("[SaveSystem] Save() not implemented yet (TODO 1). Would write to " + FullPath);
        return false;
    }

    /// <summary>Reads FullPath and returns the SaveData, or null if there is no file or it is unusable.</summary>
    public SaveData Load()
    {
        // TODO 2: If !HasSave return null. Otherwise read the text with File.ReadAllText(FullPath) and turn it
        //         back into an object: JsonUtility.FromJson<SaveData>(json). Wrap in try/catch like Save().
        //         Look at: JsonUtility.FromJson<T>(string). A malformed file throws — catch it, log, return null.
        //         Check: after saving, Load() returns an object whose stepStates match what you saved.

        // TODO 3: Version check. If data.version < SaveDataVersionRequired, log a warning
        //         "[SaveSystem] save file is version X, need Y" and return null.
        //         Why: a stale file must fail loudly, not load half-correctly (see SaveData.cs TODO 2).
        //         Check: hand-edit "version" in the file to 0 — Continue refuses it with your message.
        return null;
    }

    /// <summary>The version this build understands. Keep it equal to the default in SaveData.version.</summary>
    public const int SaveDataVersionRequired = 1;

    /// <summary>Deletes the save file, e.g. for "New Game".</summary>
    public void Delete()
    {
        if (HasSave) File.Delete(FullPath);
        Debug.Log("[SaveSystem] save deleted");
    }
}

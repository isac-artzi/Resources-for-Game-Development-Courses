// QuestManager.cs — Activity 7.2: Quests and Saves
// Runs one linear quest: a list of QuestSteps where exactly one is Active at a time. Objectives are completed
// by three different kinds of signal — an XRI grab event (take the shard), an XRI socket event (place it on
// the altar), and a distance check (walk up to the Sage). Shows progress on a wrist label and talks to
// SaveSystem for Save / Continue / New Game.
// Attached to: "Quest" in the starter scene (all references are pre-wired by the builder).
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class QuestManager : MonoBehaviour
{
    [Header("Quest")]
    [Tooltip("The steps in order. The builder fills three: take_shard, place_shard, meet_sage.")]
    public List<QuestStep> steps = new List<QuestStep>();

    [Header("Objective sources")]
    [Tooltip("Grabbing this completes 'take_shard'.")]
    public XRGrabInteractable shard;

    [Tooltip("When this socket selects the shard, 'place_shard' completes.")]
    public XRSocketInteractor altar;

    [Tooltip("Walking within talkRadius of the Sage completes 'meet_sage'.")]
    public Transform sage;

    [Tooltip("Horizontal distance (m) from the head to the Sage that counts as 'talking'.")]
    public float talkRadius = 1.5f;

    [Header("UI")]
    [Tooltip("TextMesh on the left wrist that lists the steps.")]
    public TextMesh wristLabel;

    [Tooltip("Status line on the save panel (save path, last action).")]
    public Text statusText;

    public Button continueButton;
    public Button newGameButton;

    [Header("Saving")]
    public SaveSystem saveSystem;

    [Tooltip("Save automatically every time a step completes.")]
    public bool autoSaveOnComplete = true;

    Camera head;

    void Start()
    {
        head = Camera.main;
        if (saveSystem == null) saveSystem = GetComponent<SaveSystem>();

        // TODO 1: Start the quest. If no step is Active or Done yet, set steps[0].state = QuestStep.State.Active.
        //         Then RefreshLabel().
        //         Check: the wrist label shows the first step with [>] and the others with [ ].

        // TODO 2: Wire the objective events (runtime listeners; they are not saved in the scene, which is fine):
        //             if (shard != null) shard.selectEntered.AddListener(OnShardGrabbed);
        //             if (altar != null) altar.selectEntered.AddListener(OnAltarFilled);
        //             if (continueButton != null) continueButton.onClick.AddListener(ContinueFromSave);
        //             if (newGameButton != null) newGameButton.onClick.AddListener(NewGame);
        //         Look at: XRGrabInteractable.selectEntered (UnityEvent<SelectEnterEventArgs>), Button.onClick.
        //         Why events and not polling: the grab already knows the exact frame it happened; asking
        //         "is it grabbed?" every frame would be slower and would miss a grab-and-release inside one frame.
        //         Check: grabbing the shard prints "[Quest] take_shard done" and the label updates.

        RefreshStatus("Ready. F5 = save, F9 = continue, Delete = new game.");
    }

    void Update()
    {
        // TODO 3: The Sage objective is a distance check (there is no event for "walked up to someone"):
        //         if the step "meet_sage" IsActive, take head.transform.position and sage.position, zero their y,
        //         and if Vector3.Distance(...) < talkRadius call Complete("meet_sage").
        //         Look at: Vector3.Distance. Why zero y: the Sage's pivot is at chest height; you care about floor distance.
        //         Check: walk up to the Sage — the third step flips to [x] the moment you are within 1.5 m.

        // Keyboard fallbacks so the whole loop works in the simulator without aiming at buttons.
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.f5Key.wasPressedThisFrame) SaveNow();
        if (kb.f9Key.wasPressedThisFrame) ContinueFromSave();
        if (kb.deleteKey.wasPressedThisFrame) NewGame();
    }

    // ----------------------------------------------------------------- objectives

    void OnShardGrabbed(SelectEnterEventArgs args)
    {
        Complete("take_shard");
    }

    void OnAltarFilled(SelectEnterEventArgs args)
    {
        // Only the shard counts; a socket would also accept any other grabbable.
        if (shard != null && args.interactableObject.transform != shard.transform) return;
        Complete("place_shard");
    }

    /// <summary>Finds the step by id. Returns null if none.</summary>
    public QuestStep Find(string id)
    {
        for (int i = 0; i < steps.Count; i++)
            if (steps[i].id == id) return steps[i];
        return null;
    }

    /// <summary>Marks the step Done if it is the Active one, then activates the next Locked step.</summary>
    public void Complete(string id)
    {
        // TODO 4: Find the step. If it is null or not Active, return (completing a locked step out of order is a
        //         design decision — here the quest is linear, so ignore it). Set state = Done, log
        //         "[Quest] " + id + " done", then walk the list and set the FIRST step that is still Locked to Active.
        //         RefreshLabel(); if autoSaveOnComplete, SaveNow().
        //         Look at: List<T> indexing, the QuestStep.State enum.
        //         Check: grab → place → approach completes the steps in order; placing the shard BEFORE grabbing
        //         it is impossible, but try approaching the Sage first — nothing happens until the earlier steps are done.
        Debug.Log("[Quest] Complete(" + id + ") called — implement TODO 4");
    }

    // ----------------------------------------------------------------- label

    /// <summary>Rewrites the wrist label: one line per step with its marker.</summary>
    public void RefreshLabel()
    {
        if (wristLabel == null) return;
        // TODO 5: Build the text. Use a System.Text.StringBuilder (or string +=): for each step append
        //         step.Marker() + " " + step.title + "\n". Put "QUEST: Restore the artifact" as the first line.
        //         Then wristLabel.text = the result. If every step is Done, add a last line "Complete!".
        //         Check: three lines with [x] [>] [ ] markers that change as you play.
        wristLabel.text = "QUEST (implement QuestManager TODO 5)";
    }

    void RefreshStatus(string line)
    {
        if (statusText == null) return;
        string path = saveSystem != null ? saveSystem.FullPath : "(no SaveSystem)";
        statusText.text = line + "\n" + path;
    }

    // ----------------------------------------------------------------- save / load

    /// <summary>Collects the current state into a SaveData.</summary>
    public SaveData BuildSaveData()
    {
        var data = new SaveData();
        // TODO 6: Fill it. For each step: data.stepIds.Add(step.id); data.stepStates.Add((int)step.state);
        //         data.shardPosition = shard.transform.position; data.shardPlaced = (altar != null && altar.hasSelection).
        //         Look at: XRSocketInteractor.hasSelection, explicit enum-to-int cast.
        //         Check: the JSON file lists three ids and three ints.
        return data;
    }

    /// <summary>Applies a loaded SaveData to the scene.</summary>
    public void ApplySaveData(SaveData data)
    {
        if (data == null) return;
        // TODO 7: For each i in data.stepIds: var step = Find(data.stepIds[i]); if (step != null)
        //         step.state = (QuestStep.State)data.stepStates[i]. Match by id, not by index, so a reordered
        //         step list still loads. Then, if the shard is not currently held or socketed, move it:
        //             shard.transform.position = data.shardPosition;
        //         and zero its Rigidbody velocity (shard.GetComponent<Rigidbody>().linearVelocity = Vector3.zero).
        //         Finally RefreshLabel().
        //         Look at: explicit int-to-enum cast, Rigidbody.linearVelocity (Unity 6 name for velocity).
        //         Check: grab and drop the shard somewhere odd, F5, exit Play, Play again, F9 — the shard is back
        //         where you dropped it and the steps show the saved markers.
    }

    /// <summary>Saves now (F5 or auto-save).</summary>
    public void SaveNow()
    {
        if (saveSystem == null) return;
        bool ok = saveSystem.Save(BuildSaveData());
        RefreshStatus(ok ? "Saved." : "Save failed (see Console).");
    }

    /// <summary>Loads the file and applies it (Continue button / F9).</summary>
    public void ContinueFromSave()
    {
        if (saveSystem == null) return;
        var data = saveSystem.Load();
        if (data == null)
        {
            RefreshStatus("No usable save file.");
            return;
        }
        ApplySaveData(data);
        RefreshStatus("Continued from save.");
    }

    /// <summary>Deletes the save and resets every step (New Game button / Delete key).</summary>
    public void NewGame()
    {
        if (saveSystem != null) saveSystem.Delete();
        for (int i = 0; i < steps.Count; i++) steps[i].state = QuestStep.State.Locked;
        if (steps.Count > 0) steps[0].state = QuestStep.State.Active;
        RefreshLabel();
        RefreshStatus("New game.");
    }
}

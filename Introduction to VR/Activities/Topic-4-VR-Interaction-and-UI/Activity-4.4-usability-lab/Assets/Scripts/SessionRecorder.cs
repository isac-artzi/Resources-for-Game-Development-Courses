// SessionRecorder.cs — Activity 4.4: Usability Lab
// The instrument. Every grab, release, socket-in and socket-out in the scene is routed here (the builder wires the
// XRI UnityEvents to the four On... methods below). The recorder timestamps each event, classifies errors,
// detects task success, and appends everything as CSV rows to a file in Application.persistentDataPath.
// It also carries the small statistics helpers (mean, standard deviation) you will use on the results.
// Attached to: "Session Recorder" (an empty GameObject) in the starter scene.
// Created by Isac Artzi

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SessionRecorder : MonoBehaviour
{
    [Header("Session")]
    [Tooltip("Who is being tested. Use a code like P1, P2 — never a real name (see the README on consent and privacy).")]
    public string participantId = "P1";

    [Tooltip("Which version of the design this is, e.g. 'A' or 'B'. Lets you compare two designs later.")]
    public string designVariant = "A";

    [Header("Wiring")]
    [Tooltip("The prompt to notify when the current task's target enters the socket.")]
    public TaskPrompt taskPrompt;

    [Header("State (read-only at runtime)")]
    public int currentTaskIndex = -1;
    public string currentTarget = "";
    public int errorsThisTask;
    public string objectInSocket = "";
    [Tooltip("Full path of the CSV being written. Printed to the Console at Start.")]
    public string filePath;

    float taskStartTime;
    bool taskActive;
    float sessionStart;

    void Start()
    {
        sessionStart = Time.time;
        string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        filePath = Path.Combine(Application.persistentDataPath, "usability_" + participantId + "_" + designVariant + "_" + stamp + ".csv");
        WriteLine("participant,variant,time_s,task,event,object,detail");
        Debug.Log("[SessionRecorder] Writing to " + filePath);
    }

    // ------------------------------------------------------------------
    // Logging
    // ------------------------------------------------------------------

    /// <summary>Appends one event row: participant, variant, seconds since session start, task index, event, object, detail.</summary>
    public void Log(string eventName, string objectName, string detail)
    {
        // TODO 1: Build the CSV row and append it.
        //             float t = Time.time - sessionStart;
        //             string row = participantId + "," + designVariant + "," + t.ToString("F2") + "," + currentTaskIndex + ","
        //                        + eventName + "," + Csv(objectName) + "," + Csv(detail);
        //             WriteLine(row);
        //         Why F2: hundredths of a second are plenty for human tasks; fewer digits keep the file readable.
        //         Look at: string concatenation, float.ToString("F2"), the Csv() helper below (quotes fields with commas).
        //         Check: after one grab, open the CSV (path in the Console) — it has the header plus a "grab" row.
    }

    /// <summary>Called by TaskPrompt when a task shows. Resets the per-task counters.</summary>
    public void BeginTask(int index, string targetObjectName)
    {
        // TODO 2: Start the clock for this task.
        //             currentTaskIndex = index;  currentTarget = targetObjectName;
        //             errorsThisTask = 0;  taskStartTime = Time.time;  taskActive = true;
        //             Log("task_start", targetObjectName, "");
        //         Check: the Inspector shows Current Target = "Blue Shard" and Errors This Task = 0 when task 1 starts.
    }

    /// <summary>Writes the per-task summary row. Called on success (or by EndSession for an unfinished task).</summary>
    void EndTask(bool success)
    {
        if (!taskActive) return;
        taskActive = false;

        // TODO 4: Summarize the task in one row so you never have to compute time-on-task by hand:
        //             float timeOnTask = Time.time - taskStartTime;
        //             Log("summary", currentTarget, "time_on_task=" + timeOnTask.ToString("F2") + ";errors=" + errorsThisTask + ";success=" + (success ? 1 : 0));
        //         Check: the CSV has one "summary" row per task, e.g. detail "time_on_task=8.40;errors=1;success=1".
    }

    /// <summary>Called by TaskPrompt after the last task. Closes an unfinished task and logs the end.</summary>
    public void EndSession()
    {
        EndTask(false);
        Log("session_end", "", "");
    }

    /// <summary>Called by QuickSurvey with the five answers (1-5; 0 = unanswered).</summary>
    public void LogSurvey(int[] answers)
    {
        if (answers == null) return;
        for (int i = 0; i < answers.Length; i++)
            Log("survey", "q" + (i + 1), answers[i].ToString());
        Log("survey_mean", "", Mean(answers).ToString("F2"));
    }

    // ------------------------------------------------------------------
    // XRI event handlers — wired by the builder as persistent listeners
    // ------------------------------------------------------------------

    /// <summary>A hand grabbed an object (XRGrabInteractable.selectEntered).</summary>
    public void OnGrab(SelectEnterEventArgs args)
    {
        string objectName = NameOf(args.interactableObject);
        // TODO 3: Log it and classify. A grab is an ERROR when a task is active and the object is neither the
        //         current target nor the object sitting in the socket (which the participant must remove in the swap tasks):
        //             bool wrong = taskActive && objectName != currentTarget && objectName != objectInSocket;
        //             if (wrong) errorsThisTask++;
        //             Log("grab", objectName, wrong ? "wrong_object" : "");
        //         Why count errors at all: time alone hides whether the participant understood; a fast task with
        //         three wrong grabs is a design problem you would otherwise miss.
        //         Look at: SelectEnterEventArgs.interactableObject (see NameOf below).
        //         Check: during task 1 grab the red shard — Errors This Task becomes 1 and the CSV row says wrong_object.
    }

    /// <summary>A hand released an object (XRGrabInteractable.selectExited).</summary>
    public void OnRelease(SelectExitEventArgs args)
    {
        // TODO 3 (continued): Log("release", NameOf(args.interactableObject), "").
    }

    /// <summary>The altar socket took an object (XRSocketInteractor.selectEntered).</summary>
    public void OnSocketEnter(SelectEnterEventArgs args)
    {
        string objectName = NameOf(args.interactableObject);
        objectInSocket = objectName;

        // TODO 3 (continued): This is where SUCCESS is detected.
        //             bool isTarget = taskActive && objectName == currentTarget;
        //             if (taskActive && !isTarget) errorsThisTask++;
        //             Log("socket_in", objectName, isTarget ? "target" : "wrong_object");
        //             if (isTarget) { EndTask(true); if (taskPrompt != null) taskPrompt.OnTaskCompleted(); }
        //         Check: placing the blue shard during task 1 freezes the timer on the panel and the button reads "Next".
    }

    /// <summary>An object left the altar socket (XRSocketInteractor.selectExited).</summary>
    public void OnSocketExit(SelectExitEventArgs args)
    {
        string objectName = NameOf(args.interactableObject);
        if (objectInSocket == objectName) objectInSocket = "";
        // TODO 3 (continued): Log("socket_out", objectName, "").
    }

    // ------------------------------------------------------------------
    // Statistics helpers (used by QuickSurvey and by you on the class data)
    // ------------------------------------------------------------------

    /// <summary>Arithmetic mean of the values; 0 for an empty array.</summary>
    public static float Mean(IList<int> values)
    {
        // TODO 5: Sum the values and divide by the count (guard against null/empty → return 0f).
        //         Look at: a for loop, (float) cast before dividing so 7/2 is 3.5 and not 3.
        //         Check: Mean(new[]{4,5,3,4,5}) = 4.2.
        return 0f;
    }

    /// <summary>Sample standard deviation (divides by n - 1); 0 when fewer than two values.</summary>
    public static float StdDev(IList<int> values)
    {
        // TODO 5 (continued): m = Mean(values); sum (v - m)^2 over all values; divide by (Count - 1); Mathf.Sqrt.
        //         Why n - 1: with a handful of participants you are ESTIMATING the spread of a larger population,
        //         and n - 1 corrects the bias of a small sample.
        //         Check: StdDev(new[]{4,5,3,4,5}) = 0.84 (rounded).
        return 0f;
    }

    // ------------------------------------------------------------------
    // Internals
    // ------------------------------------------------------------------

    static string NameOf(IXRInteractable interactable)
    {
        return interactable != null && interactable.transform != null ? interactable.transform.name : "unknown";
    }

    static string Csv(string field)
    {
        if (string.IsNullOrEmpty(field)) return "";
        if (field.Contains(",") || field.Contains("\""))
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        return field;
    }

    void WriteLine(string line)
    {
        try
        {
            File.AppendAllText(filePath, line + Environment.NewLine, Encoding.UTF8);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[SessionRecorder] Could not write CSV: " + e.Message);
        }
    }
}

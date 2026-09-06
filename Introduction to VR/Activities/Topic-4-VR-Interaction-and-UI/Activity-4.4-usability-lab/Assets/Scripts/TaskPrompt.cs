// TaskPrompt.cs — Activity 4.4: Usability Lab
// The moderator's voice inside the headset: a world-space panel that shows the participant one task at a time
// ("Pick up the BLUE shard and place it on the altar"), a Start/Next button, and a running timer.
// It tells the SessionRecorder when each task begins and hands over to the QuickSurvey when all tasks are done.
// Attached to: "Task Panel" (a world-space Canvas) in the starter scene; the builder fills in three tasks.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TaskPrompt : MonoBehaviour
{
    /// <summary>One task the participant must perform. Success = the target object enters the altar socket.</summary>
    [System.Serializable]
    public class Task
    {
        [Tooltip("What the participant reads. Name the object by COLOR and the goal by PLACE; avoid jargon.")]
        [TextArea(1, 3)]
        public string instruction = "Pick up the blue shard and place it on the altar.";

        [Tooltip("Exact GameObject name of the object that must end up in the socket for this task to succeed.")]
        public string targetObjectName = "Blue Shard";
    }

    [Header("Tasks")]
    [Tooltip("Tasks in order. Keep it to 3-5 for a 5-minute session.")]
    public Task[] tasks;

    [Header("Wiring")]
    [Tooltip("Records events and computes time-on-task. BeginTask() is called when a task shows.")]
    public SessionRecorder recorder;

    [Tooltip("Shown when the last task is completed.")]
    public QuickSurvey survey;

    [Header("UI references (set by the builder)")]
    public Text instructionText;
    public Text statusText;
    public Button startButton;
    public Text startButtonText;

    [Header("State (read-only at runtime)")]
    [Tooltip("Index of the task in progress, or -1 before Start.")]
    public int currentIndex = -1;

    [Tooltip("True between Start/Next and the task's success.")]
    public bool taskRunning;

    float taskStartTime;
    bool sessionFinished;

    void Start()
    {
        if (startButton != null) startButton.onClick.AddListener(OnStartOrNextPressed);
        if (instructionText != null) instructionText.text = "Welcome. When you are ready, press Start.";
        if (statusText != null) statusText.text = "You can stop at any time.";
        if (startButtonText != null) startButtonText.text = "Start";
    }

    /// <summary>Start button (or Space on the keyboard): begins the first task, or advances after a success.</summary>
    public void OnStartOrNextPressed()
    {
        if (sessionFinished) return;
        if (taskRunning) return;           // ignore presses while a task is in progress

        // TODO 2: Advance. currentIndex++ ; if currentIndex < tasks.Length → ShowTask(currentIndex);
        //         else → FinishSession().
        //         Why one button for Start and Next: fewer things to learn for the participant; the label changes.
        //         Check: pressing Start shows task 1; after you complete it, the button reads "Next" and shows task 2.
    }

    /// <summary>Displays a task and tells the recorder the clock is running.</summary>
    public void ShowTask(int index)
    {
        if (tasks == null || index < 0 || index >= tasks.Length) return;
        Task t = tasks[index];

        // TODO 1: Show the task and start the clock.
        //             instructionText.text = "Task " + (index + 1) + " of " + tasks.Length + "\n" + t.instruction;
        //             taskRunning = true;  taskStartTime = Time.time;
        //             if (startButton != null) startButton.interactable = false;      // no skipping while running
        //             if (recorder != null) recorder.BeginTask(index, t.targetObjectName);
        //         Look at: Time.time, Selectable.interactable.
        //         Check: the panel reads "Task 1 of 3 / Pick up the BLUE shard ..." and the Start button greys out.
    }

    /// <summary>Called by SessionRecorder the moment the target object enters the socket.</summary>
    public void OnTaskCompleted()
    {
        if (!taskRunning) return;
        taskRunning = false;
        float elapsed = Time.time - taskStartTime;
        if (statusText != null) statusText.text = "Done in " + elapsed.ToString("F1") + " s. Press Next.";
        if (startButton != null) startButton.interactable = true;
        if (startButtonText != null)
            startButtonText.text = (currentIndex + 1 < (tasks != null ? tasks.Length : 0)) ? "Next" : "Finish";
    }

    void FinishSession()
    {
        sessionFinished = true;
        if (instructionText != null) instructionText.text = "All tasks done. Thank you!\nPlease answer the five questions on the survey panel.";
        if (statusText != null) statusText.text = "";
        if (startButton != null) startButton.interactable = false;
        if (recorder != null) recorder.EndSession();
        if (survey != null) survey.Show();
    }

    void Update()
    {
        // TODO 4: Live timer while a task runs, so the moderator (you) can see it over the participant's shoulder:
        //             if (taskRunning && statusText != null)
        //                 statusText.text = (Time.time - taskStartTime).ToString("F1") + " s";
        //         Check: the status line counts up in tenths of a second during a task and freezes at "Done in ..." on success.

        // TODO 3: Keyboard fallback for the moderator: Space does what the Start/Next button does.
        //             var kb = Keyboard.current;
        //             if (kb != null && kb.spaceKey.wasPressedThisFrame) OnStartOrNextPressed();
        //         Look at: UnityEngine.InputSystem.Keyboard (the new Input System; not the legacy Input class).
        //         Why: during a real test you stand next to the participant; you should be able to advance from the
        //         keyboard without taking their controller.
        //         Check: press Space with the Game view focused — same effect as clicking Start.
    }
}

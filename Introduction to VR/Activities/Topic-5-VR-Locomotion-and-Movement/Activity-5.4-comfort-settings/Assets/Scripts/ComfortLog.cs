// ComfortLog.cs — Activity 5.4: Comfort Settings and the Rest Point
// An SSQ-lite comfort questionnaire: four symptoms rated 0–3 after a timed 5-minute test, normalized to a 0–100
// score and appended with the current comfort settings to a CSV in Application.persistentDataPath. This is the
// evidence your milestone's "comfort testing" section is built on — one row per tester per settings profile.
// Attached to: Comfort Log Panel (the second world-space Canvas) in the starter scene; buttons are pre-assigned.
// Created by Isac Artzi

using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ComfortLog : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("The ComfortSettings on the rig, so each row records which profile was tested. Found at runtime if empty.")]
    public ComfortSettings settings;

    [Tooltip("The rest point in the scene, so each row records how long the tester rested. Optional.")]
    public RestPoint restPoint;

    [Header("Test timer")]
    [Tooltip("Length of the comfort test in seconds. 300 = 5 minutes.")]
    public float testSeconds = 300f;
    public Button startButton;
    public Text timerText;

    [Header("Questions (0 = none, 1 = slight, 2 = moderate, 3 = severe)")]
    public string[] questions = { "General discomfort", "Nausea", "Dizziness", "Eye strain" };

    [Tooltip("16 buttons, row-major: question 0 ratings 0–3, then question 1 ratings 0–3, ...")]
    public Button[] answerButtons;

    [Tooltip("One text per question that echoes the chosen rating.")]
    public Text[] answerTexts;

    public Button submitButton;
    public Text statusText;

    [Header("Desktop fallback")]
    [Tooltip("When true, keys 0–3 answer the current question and advance to the next.")]
    public bool keyboardAnswers = true;

    [Header("Debug (read-only)")]
    public int[] answers = { -1, -1, -1, -1 };
    public float elapsed;
    public bool running;
    public int currentQuestion;

    public const string FileName = "comfort_log.csv";

    void Start()
    {
        if (settings == null) settings = Object.FindFirstObjectByType<ComfortSettings>();
        if (restPoint == null) restPoint = Object.FindFirstObjectByType<RestPoint>();

        // TODO 1: Wire the buttons (null-check the arrays and each element):
        //             startButton.onClick.AddListener(StartTest);  submitButton.onClick.AddListener(Submit);
        //             for (int i = 0; i < answerButtons.Length; i++) { int q = i / 4; int v = i % 4;
        //                 answerButtons[i].onClick.AddListener(() => SetAnswer(q, v)); }
        //         Why copy i into q and v first: a lambda captures VARIABLES, not values — without the copies every button
        //         would use the final value of i.
        //         Check: clicking any rating button updates that question's echo text.
        UpdateStatus("Press Start, run the test, then rate and Submit");
    }

    void Update()
    {
        // TODO 2: Timer. If running: elapsed += Time.deltaTime; if (elapsed >= testSeconds) { running = false; UpdateStatus("Time! Rate the four questions."); }
        //         Show the time either way: int s = Mathf.FloorToInt(elapsed); timerText.text = (s / 60).ToString("00") + ":" + (s % 60).ToString("00");
        //         Look at: Time.deltaTime; integer division and modulo for mm:ss.
        //         Check: Start → the readout counts up from 00:00; at 05:00 it stops and the status changes.

        // TODO 3: Keyboard fallback. If keyboardAnswers and Keyboard.current != null: for v in 0..3, if the digit key v was pressed
        //         (Keyboard.current[Key.Digit0 + v].wasPressedThisFrame — Key is an enum, so Digit0 + v works), call
        //         SetAnswer(currentQuestion, v) and advance currentQuestion = Mathf.Min(currentQuestion + 1, questions.Length - 1).
        //         Check: pressing 2, 0, 1, 1 fills the four echo texts in order.
    }

    /// <summary>Starts (or restarts) the 5-minute timer and clears previous answers.</summary>
    public void StartTest()
    {
        elapsed = 0f;
        running = true;
        currentQuestion = 0;
        for (int i = 0; i < answers.Length; i++) answers[i] = -1;
        for (int i = 0; i < answerTexts.Length; i++) if (answerTexts[i] != null) answerTexts[i].text = "-";
        UpdateStatus("Test running: walk the ring, rest if you need to");
    }

    /// <summary>Records one rating and echoes it on the panel.</summary>
    public void SetAnswer(int question, int rating)
    {
        if (question < 0 || question >= answers.Length) return;
        answers[question] = Mathf.Clamp(rating, 0, 3);
        if (answerTexts != null && question < answerTexts.Length && answerTexts[question] != null)
            answerTexts[question].text = answers[question].ToString();
        currentQuestion = question;
    }

    /// <summary>Normalizes the four 0–3 ratings to 0–100. Returns -1 if any question is unanswered.</summary>
    public static float Score(int[] ratings)
    {
        // TODO 4: int total = 0; foreach rating: if (r < 0) return -1f; total += r;
        //         return 100f * total / (3f * ratings.Length);        // 4 questions × 3 = 12 is the maximum
        //         Worked check: ratings {1, 0, 2, 1} → total 4 → 100 * 4 / 12 = 33.3.
        return -1f;
    }

    /// <summary>Appends one CSV row (with a header if the file is new) and reports the score.</summary>
    public void Submit()
    {
        float score = Score(answers);
        if (score < 0f) { UpdateStatus("Answer all four questions first"); return; }

        // TODO 5: Build the row and append it:
        //             string path = Path.Combine(Application.persistentDataPath, FileName);
        //             bool isNew = !File.Exists(path);
        //             string header = "timestamp,mode,speed,turn,vignette,height,elapsed_s,rest_s,q_discomfort,q_nausea,q_dizziness,q_eyestrain,score\n";
        //             string row = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + settings.mode + "," + settings.moveSpeed.ToString("F1") + "," +
        //                          settings.turnType + "," + settings.vignetteStrength.ToString("F2") + "," + settings.heightOffset.ToString("F2") + "," +
        //                          elapsed.ToString("F0") + "," + (restPoint != null ? restPoint.totalRestSeconds.ToString("F0") : "0") + "," +
        //                          answers[0] + "," + answers[1] + "," + answers[2] + "," + answers[3] + "," + score.ToString("F1") + "\n";
        //             File.AppendAllText(path, (isNew ? header : "") + row);
        //         Look at: System.IO.File.AppendAllText, Application.persistentDataPath (the README says where that is per OS).
        //         Why CSV: opens in any spreadsheet, and two testers' rows can be compared side by side in your milestone report.
        //         Check: the Console shows the path; open the file and see the header plus one row per Submit.
        UpdateStatus("Score " + score.ToString("F0") + " / 100 — saved to " + FileName);
        Debug.Log("[ComfortLog] " + Path.Combine(Application.persistentDataPath, FileName));
        running = false;
    }

    void UpdateStatus(string s)
    {
        if (statusText != null) statusText.text = s;
    }
}

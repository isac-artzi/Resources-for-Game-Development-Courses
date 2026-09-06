// QuickSurvey.cs — Activity 4.4: Usability Lab
// A five-question, 1-5 agreement survey the participant answers INSIDE the headset right after the tasks, while
// the experience is fresh. Five rows of five buttons; the chosen button lights up; Submit writes the answers and
// their mean to the same CSV as the task events.
// Attached to: "Survey Panel" (a world-space Canvas) in the starter scene, hidden until TaskPrompt finishes.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class QuickSurvey : MonoBehaviour
{
    [Header("Questions (1 = strongly disagree, 5 = strongly agree)")]
    [Tooltip("Five statements. Keep each to one line; mix task, interaction and COMFORT questions.")]
    public string[] questions =
    {
        "I understood what I was asked to do.",
        "Picking up the shards felt natural.",
        "Placing a shard on the altar was easy.",
        "I felt physically comfortable (no dizziness or eye strain).",
        "I would like to play more of this.",
    };

    [Header("Wiring")]
    public SessionRecorder recorder;

    [Header("UI references (set by the builder)")]
    [Tooltip("The panel root to show/hide.")]
    public GameObject panelRoot;

    [Tooltip("One label per question row.")]
    public Text[] questionTexts;

    [Tooltip("25 buttons, row-major: index = row * 5 + (value - 1).")]
    public Button[] answerButtons;

    public Button submitButton;
    public Text footerText;

    [Header("Colors")]
    public Color unselectedColor = new Color(0.25f, 0.30f, 0.45f, 1f);
    public Color selectedColor = new Color(1.0f, 0.80f, 0.30f, 1f);

    [Header("State (read-only at runtime)")]
    [Tooltip("Answer per row, 1-5; 0 = not answered yet.")]
    public int[] answers = new int[5];

    bool submitted;

    void Start()
    {
        if (questionTexts != null)
            for (int i = 0; i < questionTexts.Length && i < questions.Length; i++)
                if (questionTexts[i] != null) questionTexts[i].text = (i + 1) + ". " + questions[i];

        if (answerButtons != null)
        {
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (answerButtons[i] == null) continue;
                int row = i / 5;
                int value = i % 5 + 1;
                answerButtons[i].onClick.AddListener(() => OnAnswer(row, value));   // locals per iteration, so the capture is safe
            }
        }
        if (submitButton != null) submitButton.onClick.AddListener(OnSubmit);
        if (footerText != null) footerText.text = "Answer all five, then press Submit. Keys 1-5 answer the next open row.";
        Hide();
    }

    /// <summary>Shows the survey. Called by TaskPrompt when the tasks are done.</summary>
    public void Show()
    {
        if (panelRoot != null) panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    /// <summary>Records an answer for a row and highlights the chosen button.</summary>
    public void OnAnswer(int row, int value)
    {
        if (submitted || answers == null || row < 0 || row >= answers.Length) return;

        // TODO 1: Store and show the answer.
        //             answers[row] = value;
        //             for (int v = 1; v <= 5; v++)
        //             {
        //                 var img = answerButtons[row * 5 + (v - 1)].GetComponent<Image>();
        //                 if (img != null) img.color = (v == value) ? selectedColor : unselectedColor;
        //             }
        //         Why highlight: a Likert row with no visible selection makes participants re-click and doubt the system;
        //         the highlight is the confirmation.
        //         Look at: Image.color, the row-major index formula row * 5 + (value - 1).
        //         Check: clicking "4" on row 2 turns that button gold and the others in the row back to blue.
    }

    void Update()
    {
        if (panelRoot == null || !panelRoot.activeSelf || submitted) return;

        // TODO 2: Keyboard fallback: keys 1-5 answer the FIRST unanswered row (so the moderator can enter answers a
        //         participant says aloud). Find the row: the first i with answers[i] == 0; if none, return.
        //             var kb = Keyboard.current;  if (kb == null) return;
        //             if (kb.digit1Key.wasPressedThisFrame) OnAnswer(row, 1);   ... through digit5Key → 5
        //         Look at: UnityEngine.InputSystem.Keyboard.current and the digitNKey properties.
        //         Check: with the survey open, press 5, 4, 5, 3, 4 — rows 1-5 light up in turn.
    }

    /// <summary>Submit: refuses until every row is answered, then logs and computes the mean.</summary>
    public void OnSubmit()
    {
        if (submitted) return;

        // TODO 3: Validate, log, and close out.
        //             for (int i = 0; i < answers.Length; i++)
        //                 if (answers[i] == 0) { if (footerText != null) footerText.text = "Please answer question " + (i + 1) + "."; return; }
        //             if (recorder != null) recorder.LogSurvey(answers);
        //             float mean = SessionRecorder.Mean(answers);
        //             if (footerText != null) footerText.text = "Thank you! Mean rating " + mean.ToString("F1") + " / 5. You may take the headset off.";
        //             submitted = true;
        //         Why validate in the UI and not in the recorder: the person who can fix a missing answer is standing
        //         right there — tell THEM, at the moment it matters.
        //         Look at: SessionRecorder.Mean (you implement it in SessionRecorder.cs TODO 5).
        //         Check: press Submit with row 3 blank → the footer asks for question 3; answer it → footer shows the mean
        //         and five "survey" rows plus a "survey_mean" row appear at the end of the CSV.
    }
}

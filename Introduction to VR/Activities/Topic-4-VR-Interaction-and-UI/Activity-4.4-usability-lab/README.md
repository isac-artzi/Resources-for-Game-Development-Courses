# Activity 4.4 — Usability Lab

Topic 4: VR Interaction and User Interfaces · Week 8, Thursday · Stepping stone to Milestone 4 — UI and Interaction Design

## Where this fits

By the end of class you will have a test harness you can drop into any scene: a panel that gives a participant one task at a time, a recorder that timestamps every grab, release, and socket event and writes time-on-task, error count, and success for each task to a CSV file, and a five-question survey the participant answers inside the headset before taking it off. Then you will *use* it: run two classmates through three shard-placing tasks, read the two CSVs, and compute a mean and a standard deviation. Milestone 4 requires user testing and an iteration based on it. This activity is how you collect the evidence; the numbers you gather today are the "before" in your milestone's before/after comparison, and the harness is what you will point at your own inventory, dialogue, and puzzle next week.

*Elaria hook:* even legends get rehearsed. Before the hero of Realm of Legends faces the altar for real, the Guide runs a quieter trial in a plain stone room — three shards, one altar, no monsters — and watches. Not to judge the hero, but to see where the *world* confuses them. That is what a usability test is: you are testing the design, never the person.

## Learning goals

- You can explain the five core usability measures — task success, time on task, errors, satisfaction, and comfort — and say what each one tells you that the others do not.
- You can wire XR Interaction Toolkit `UnityEvent`s (grab and socket `selectEntered` / `selectExited`) to a central recorder and explain why the builder uses *persistent* listeners.
- You can write structured event data to a CSV in `Application.persistentDataPath`, find the file, and open it in a spreadsheet.
- You can run a short think-aloud test ethically: consent, a stop-anytime rule, no names in the data, and no coaching.
- You can compute the mean and sample standard deviation of a small set of measurements and compare two design variants honestly.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-4.4-usability-lab`). Open it with Unity **6000.5.x** and wait for the packages to resolve.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** tab → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup**. Both lines should read `OK`.
6. **Activity → Build Starter Scene**. The scene `Activity_4_4` is created and opened, with *Session Recorder* selected.
7. Press **Play**. A bright, plain room: a table with blue, red and green shards on the left, an altar with a glowing ring on the right, a dark panel ahead that says *Welcome. When you are ready, press Start.* The Console prints the path of the CSV the recorder created.

Simulator controls that matter today: **Tab** to a controller, aim with the **mouse**, move the hand with **W A S D** and the vertical keys; **Grip** (see the simulator's on-screen panel) grabs a shard and releasing it over the ring seats it; **Trigger** clicks the panel buttons with the ray. The moderator's shortcuts you will add: **Space** = Start / Next, **1–5** = answer the next survey row. During the actual tests you are the moderator at the keyboard while your classmate uses the mouse and keyboard as the participant — or, if a headset is available, wears it.

## VR theory

**What a usability test measures.** A usability test asks a representative person to do realistic tasks with your design while you observe and measure. Five measures cover most of what matters. **Task success** — did they finish, and did they need help? **Time on task** — seconds from reading the instruction to completing it; slow times point at confusion even when the participant eventually succeeds. **Errors** — wrong actions: grabbing the wrong object, placing it in the wrong place, dropping it. Errors are the most *diagnostic* measure because each one tells you *where* the design misled someone. **Satisfaction** — how they *felt*, captured right after the session with a short rating scale, because feelings fade and get rationalized within minutes. And in VR, a fifth measure no flat-screen test has: **comfort** — dizziness, eye strain, sweating, disorientation. A design that scores perfectly on the first four and makes one in five people queasy has failed. Braun and Rizzo touch on testing throughout Chapter 8 of *XR Development with Unity*; the practices below come from the wider usability tradition (Nielsen's small-sample testing, ISO 9241's definition of usability as effectiveness, efficiency and satisfaction).

**Think-aloud.** Ask the participant to say what they are thinking as they go: "I'm looking for... I thought this would..." It feels awkward for thirty seconds and then becomes the richest data you will get, because it exposes the *model in their head*, which is what you are actually testing. Your job as moderator is to stay quiet. Do not explain, do not hint, do not say "no, the other one". If they are stuck for a long time, note it, mark the task as failed, and move on. Every time you help, you erase a finding.

**Why five participants.** Nielsen's classic argument is that the first five testers find about 85% of the usability problems, and each additional tester mostly re-finds the same ones. For a class prototype, two participants today and three more next week is a real study. Small samples also mean your statistics are *descriptive* — a mean and a spread tell you "roughly 12 s, varying by 4" — not proof. Say so in your milestone write-up.

**Ethics of testing, even among friends.** Testing people is a relationship with rules. *Consent*: tell them what you are testing, what you will record (timings and button presses, no video unless they agree), and how long it takes. *Stop anytime*: say it out loud before you start, and mean it — especially in VR, where discomfort can arrive fast. *No names in the data*: use P1, P2 — the `Participant Id` field — so the CSV cannot embarrass anyone. *Test the design, not the person*: never say "you did that wrong"; say "the design didn't make that clear". *Debrief*: after the survey, tell them what you learned and thank them. These rules are not bureaucracy; they are what makes participants relax enough to give you honest behavior.

**The iteration loop.** Observe → find the worst problem → change one thing → test again. The `Design Variant` field on the recorder exists so that when you change the altar's position or the instruction wording, you can label the runs A and B and compare their numbers rather than your memory of them. Change *one* thing between variants, or you will not know which change moved the numbers.

**Events, not polling.** The recorder never asks "is something being grabbed?" every frame. It waits for XRI to *tell* it, through the `selectEntered` and `selectExited` events on each `XRGrabInteractable` and on the `XRSocketInteractor`. The builder wired those with `UnityEventTools.AddPersistentListener`, which is the Editor API that writes a listener into the scene file exactly as if you had dragged the recorder into the event's Inspector slot. Open *Blue Shard* and look at its `XRGrabInteractable` → *Select Entered* — you will see `SessionRecorder.OnGrab` there. A plain `AddListener` in Editor code would run once and be forgotten when the scene saved.

## Math foundation

**Time on task.** $T = t_{success} - t_{start}$, where $t_{start}$ is the moment the instruction appeared and $t_{success}$ the moment the target entered the socket. Both are `Time.time` values, so the difference is in seconds regardless of frame rate.

**Success rate.** For $n$ attempts of a task, $\text{success rate} = \frac{\text{successes}}{n}$. Two participants, both complete task 2: $2/2 = 100\%$. One gives up: $50\%$. With $n = 2$ the only possible values are 0, 50 and 100%, which is a reminder of how coarse tiny samples are.

**Mean.** For values $x_1, \dots, x_n$:

$$\bar{x} = \frac{1}{n}\sum_{i=1}^{n} x_i$$

**Sample standard deviation.**

$$s = \sqrt{\frac{1}{n-1}\sum_{i=1}^{n} (x_i - \bar{x})^2}$$

The $n - 1$ (rather than $n$) is Bessel's correction: with a small sample you are estimating the spread of a larger population, and dividing by $n$ would systematically underestimate it.

Worked example — survey answers from one participant: 4, 5, 3, 4, 5. Mean: $(4+5+3+4+5)/5 = 21/5 = 4.2$. Deviations: $-0.2, 0.8, -1.2, -0.2, 0.8$. Squares: $0.04, 0.64, 1.44, 0.04, 0.64$, sum $2.80$. Divide by $n - 1 = 4$: $0.70$. Square root: $s = 0.84$. So "4.2 ± 0.8" — mostly positive, with question 3 the outlier worth asking about in the debrief.

Worked example — time on task for task 1 across two participants in variant A: 14.2 s and 9.8 s. Mean $12.0$ s; deviations $\pm 2.2$; squares $4.84$ each, sum $9.68$; divide by 1: $9.68$; $s = 3.1$ s. If variant B (say, the altar moved closer to the table) gives 8.1 s and 7.5 s — mean 7.8 s, $s = 0.4$ s — the means differ by 4.2 s, larger than either spread, which is a reasonable *hint* that B is faster. With two people per group it is not proof; report it as "B looked faster (7.8 s vs 12.0 s, n = 2 each)" and test more.

**Integer division trap.** In C#, `21 / 5` is `4`, not `4.2`. Cast before dividing: `(float)sum / values.Count`. This is TODO 5's most common bug.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_4_4.unity` with:

- **Room** — a 10 m floor with three light walls; even lighting and faint fog. Deliberately plain: a test room should not compete with the task.
- **XR Origin (XR Rig)** at the origin with the **XR Interaction Simulator**, and an **EventSystem** with `XRUIInputModule`.
- **Session Recorder** — an empty GameObject carrying **`SessionRecorder`** (*Participant Id* P1, *Design Variant* A, *Task Prompt* assigned).
- **Shard Table** (left) with **Blue Shard**, **Red Shard**, **Green Shard** — `Rigidbody` + `XRGrabInteractable` (kinematic, no throw). Each shard's *Select Entered* and *Select Exited* events are wired to `SessionRecorder.OnGrab` / `OnRelease`.
- **Altar** (right) with **Altar Socket** — a trigger `SphereCollider` (r = 0.14) and an `XRSocketInteractor` whose *Select Entered* / *Select Exited* are wired to `OnSocketEnter` / `OnSocketExit`; a glowing **Socket Ring** marks it.
- **Task Panel** — a world-space `Canvas` (680 × 380 px, scale 0.001) 2.6 m ahead at 1.55 m, facing the origin, carrying **`TaskPrompt`** with three tasks typed in (*Blue Shard* → altar; swap to *Red Shard*; swap to *Green Shard*), *Recorder* and *Survey* assigned, and *Instruction*, *Status*, *Start Button* references set.
- **Survey Panel** — a world-space `Canvas` (960 × 640 px) to your right, turned to face you, carrying **`QuickSurvey`** with five question labels, 25 answer buttons, a Submit button and a footer. Hidden until the tasks finish.
- **Welcome Sign**, **Table Sign**, **Altar Sign**.

Scripts live in `Assets/Scripts/`: `TaskPrompt.cs` (the participant's instructions and timer), `SessionRecorder.cs` (events → CSV, statistics), `QuickSurvey.cs` (the in-VR questionnaire).

The flow: Start → `TaskPrompt.ShowTask(0)` → `SessionRecorder.BeginTask` (clock starts) → the participant grabs and places → XRI events arrive at `OnGrab` / `OnRelease` / `OnSocketEnter` / `OnSocketExit` → when the target enters the socket, `EndTask(true)` writes a summary row and `TaskPrompt.OnTaskCompleted` freezes the timer → Next → … → after task 3, `FinishSession` → `recorder.EndSession()` and `survey.Show()` → answers → Submit → `recorder.LogSurvey`.

The CSV has one header and one row per event: `participant,variant,time_s,task,event,object,detail`. Event names: `task_start`, `grab`, `release`, `socket_in`, `socket_out`, `summary`, `session_end`, `survey`, `survey_mean`. The file lives in `Application.persistentDataPath` — on Windows `%USERPROFILE%\AppData\LocalLow\<Company>\<Product>\`, on macOS `~/Library/Application Support/<Company>/<Product>/` — and the exact path is printed to the Console at Play.

## Your tasks (about 70 min)

**Task 1 (10 min) — Prompt and clock** · file: `Scripts/TaskPrompt.cs`, TODO 1–2
In `ShowTask`, write the numbered instruction into the panel, mark the task running, record the start time, disable the button, and call `recorder.BeginTask`. In `OnStartOrNextPressed`, advance the index and either show the next task or call `FinishSession`.
**Check:** click Start (Tab to a controller, aim the ray, Trigger). The panel reads *Task 1 of 3 — Pick up the BLUE shard…* and the button greys out.

**Task 2 (10 min) — Rows in a file** · file: `Scripts/SessionRecorder.cs`, TODO 1–2
In `Log`, build the CSV row (participant, variant, seconds since session start with two decimals, task index, event, object, detail — run the last two through `Csv()`) and pass it to `WriteLine`. In `BeginTask`, set the current task fields, reset the error count, note the start time, and log `task_start`.
**Check:** press Start, stop Play, open the file at the path in the Console. It has the header plus a `task_start` row naming *Blue Shard*.

**Task 3 (15 min) — Hear every grab and placement** · file: `Scripts/SessionRecorder.cs`, TODO 3
Fill the four handlers. A grab is a *wrong_object* error when a task is active and the object is neither the target nor the thing currently in the socket. A socket entry with the target is a success: call `EndTask(true)` and `taskPrompt.OnTaskCompleted()`; any other object is an error. Log releases and socket exits plainly.
**Check:** during task 1 grab the red shard: *Errors This Task* becomes 1. Seat the blue shard: the status line shows *Done in x.x s. Press Next.* Play through all three tasks; the CSV shows grabs, releases, `socket_in … target`, and the survey panel appears after the third.

**Task 4 (10 min) — Summaries and the live timer** · files: `Scripts/SessionRecorder.cs` TODO 4, `Scripts/TaskPrompt.cs` TODO 3–4
In `EndTask`, log a `summary` row with `time_on_task`, `errors`, and `success`. In `TaskPrompt.Update`, show the running seconds in the status line while a task runs, and make Space do what the button does.
**Check:** the status counts up during a task; the CSV has three `summary` rows like `time_on_task=8.40;errors=1;success=1`; Space advances the tasks with the Game view focused.

**Task 5 (10 min) — The survey** · files: `Scripts/QuickSurvey.cs` TODO 1–3, `Scripts/SessionRecorder.cs` TODO 5
In `OnAnswer`, store the value and recolor the row so the chosen button is gold. In `Update`, let keys 1–5 answer the first unanswered row. In `OnSubmit`, refuse until all five rows are answered, then log the answers and show the mean — which means implementing `Mean` and `StdDev` in the recorder (cast to `float` before dividing; divide the squared deviations by `n − 1`).
**Check:** after task 3, click 4 on row 1 — it turns gold. Press 5, 3, 4, 5 for the rest. Submit → footer reads *Thank you! Mean rating 4.2 / 5*; the CSV ends with five `survey` rows and a `survey_mean` of 4.20. Test `StdDev` on {4,5,3,4,5} in a `Debug.Log`: 0.84.

**Task 6 (15 min) — Run two classmates** · no new code
Set *Participant Id* to P1 on *Session Recorder*, press Play, and hand the mouse and keyboard (or the headset) to a classmate. Read them the consent script from the theory section, ask them to think aloud, press Space to start each task, and *say nothing else*. After the survey, thank them and swap roles: you are their participant with their build. Then run P2. Open both CSVs in a spreadsheet, pull the three `time_on_task` values per participant, and compute the mean and standard deviation of task 1 across both. Record your screencast during one of the runs (their consent for the recording first).

## Stretch goals

- Add a `drop` error: in `OnRelease`, if the object is not inside the socket 0.5 s later (a coroutine and a distance check), log `release … dropped` and count it.
- Make a variant B: move the altar to within arm's reach of the table, set *Design Variant* to B, rerun both participants, and put the four task-1 times into the two-group comparison from the math section.
- Add a comfort question with a 0–3 scale (a small SSQ-lite: nausea, eye strain, dizziness, sweating) as a second survey page; Activity 5.4 will build on it.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. On the headset, `Application.persistentDataPath` is inside the app's private storage; pull the CSVs with `adb pull /sdcard/Android/data/<package>/files/` (the exact path is printed to the device log — use `adb logcat -s Unity` to read it) or, simpler, copy them from the Console path when you run the same test on desktop. As moderator you cannot see the panel your participant sees; enable **Cast** in the Meta Horizon app or the headset's Quick Settings so you can watch the mirrored view, and stand where you can reach the keyboard-free alternative: the participant presses Start/Next themselves with the ray. Comfort questions matter twice as much on hardware — watch for pauses, hand-to-face gestures, and ask.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: a full session — Start, three tasks with the timer running, at least one recorded error, the survey, and the CSV open in a spreadsheet with the `summary` rows visible.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus the task-1 mean and standard deviation across your two participants and the single most surprising thing a participant said while thinking aloud.
- The two CSV files your recorder produced (P1 and P2), attached to the same post.

## Troubleshooting

| Symptom | Fix |
|---|---|
| **Activity** menu is missing | A compile error; open the Console, fix the red line, wait for the spinner to finish |
| The CSV has only the header | `Log` (TODO 1) is still empty — `WriteLine` is only called for the header until you build the rows |
| I cannot find the CSV | Copy the path printed as `[SessionRecorder] Writing to …` from the Console; on Windows it is under `AppData\LocalLow`, which is hidden by default |
| Grabs are not recorded | The persistent listeners are on each shard's `XRGrabInteractable` → *Select Entered / Exited*; if you added shards by hand, drag *Session Recorder* into those slots and pick `OnGrab` / `OnRelease` |
| Seating the target does not complete the task | `OnSocketEnter` TODO 3 must compare `objectName == currentTarget` — names are case-sensitive and must match the task's *Target Object Name* exactly (*Blue Shard*) |
| Every grab counts as an error during swap tasks | In `OnGrab`, allow the object currently in the socket (`objectName != objectInSocket`) — removing it is part of the task |
| Mean shows 4.0 instead of 4.2 | Integer division: cast the sum to `float` before dividing |
| Survey buttons do not highlight | `OnAnswer` (TODO 1) is empty, or the button index formula is off — it is `row * 5 + (value - 1)` |
| Space or 1–5 do nothing | Click the Game view to give it focus; make sure you read `Keyboard.current` from the Input System |

Created by Isac Artzi

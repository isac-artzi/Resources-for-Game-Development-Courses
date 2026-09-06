# Activity 7.3 — Playtest Telemetry and Heatmap

Topic 7: VR Game Development Project · Week 14, Tuesday · Stepping stone to Milestone 7 — Final Integration

## Where this fits

Milestone 7 asks you to playtest the integrated game, analyze the feedback, and refine. Feedback from a VR tester is hard to collect: they cannot see your Console, they cannot type, and afterwards they remember "somewhere near the trees". Today you build three tools that turn a session into evidence. A `PositionSampler` logs where the tester's head was every half second. A `HeatmapGrid` bins those samples into one-meter cells and draws them — as gizmos in the Scene view while you watch, as colored cubes in the world for the tester, and again in Edit mode from the saved CSV after the session. A `BugReporter` gives the tester one button that saves a screenshot, the position, and a timestamp. On Thursday you will polish the game; next week you will profile it. This is the day you learn what testers actually do in it.

*Elaria hook:* the Guide can see every footprint the hero leaves in the Enchanted Forest, glowing on the moss long after the hero has gone. Where the moss burns red, many heroes lingered — or got lost. Today you borrow the Guide's eyes.

## Learning goals

- You can explain the difference between observing a playtest and instrumenting it, and name one thing each catches that the other misses.
- You can sample a position at a fixed rate with a `Time.deltaTime` timer and justify the rate.
- You can bin a world position into a grid cell with floor division and map a count to a color ramp.
- You can draw debug geometry with `OnDrawGizmos` and explain why gizmos cost nothing in a build.
- You can write and read a CSV with invariant culture and capture a screenshot from code on desktop and on the Quest.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-7.3-playtest-telemetry`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup** — two `OK` lines.
6. **Activity → Build Starter Scene** → `Activity_7_3` opens with the **Telemetry** object selected.
7. Arrange your windows so the **Scene** view and the **Game** view are both visible (drag the Scene tab next to the Game tab, or use *Window → Layouts → 2 by 3*). In the Scene view, zoom out until you see the whole 30 m grid outline drawn by `HeatmapGrid`. Press **Play**.

Simulator controls that matter today: **Tab** to select the head, **W A S D** to walk, **mouse** to look, **Esc** to release the cursor. Script keys: **B** files a bug report, **H** spawns the heatmap cubes. Keep the Scene view visible while you play — the gizmo heatmap draws there.

## VR theory

**Observation versus telemetry.** Watching a tester (over their shoulder on a cast screen, or standing beside them) catches what they *say* and *feel*: hesitation, "where am I supposed to go?", a flinch when something moves too fast. Telemetry — data the game records about itself — catches what they *did*: the path they walked, how long they stood at the fork, where they were when they pressed the bug button. Neither is sufficient alone. Observation is rich but biased by the observer's expectations and the tester's politeness; telemetry is objective but silent about *why*. Braun and Rizzo's Chapter 9 frames testing as part of the build-measure-learn loop of a project; today's tools are the "measure" step.

**What a heatmap reveals.** A heatmap of head positions has three signatures worth learning to read. *Hot spots* where the count is high: either a point of interest (good) or a place where testers were stuck or confused (bad) — only observation tells you which. *Dead zones* the color never reaches: content nobody found, or a place that reads as "not for me" (a dark corner, a step that looks too high). *Paths*: the actual routes compared with the routes you designed. Today's course has a fork with a dead end on the left. If the left branch glows, your signposting failed; if it stays cold, the right branch reads as the obvious way — and you should ask whether the dead end is worth building at all.

**Sampling.** A sample every 0.5 s at walking speed places one dot every 0.75 m — dense enough to see a path in 1 m cells, sparse enough that a 10-minute session is 1 200 rows. Sampling every frame would weight a tester standing still for 5 s at 72 Hz as 360 samples — technically correct (they *did* spend time there) but it swamps everything else and produces enormous files. Sampling by time, not by frame, also makes two testers' files comparable even if one laptop ran at 144 Hz.

**Bias and ethics.** Telemetry is only as honest as its collection. Tell the tester what is being recorded (position, screenshots on their button press) and that they may stop at any time. Do not record more than you will analyze. Numbers from three classmates who all know the level are not "user data" — they are a smoke test. Write that limitation into your Milestone 7 report.

**In-VR bug reporting.** A tester in a headset cannot write a note. The best they can do is *mark the moment*: one button that saves a screenshot of what they saw, where they stood, and when. Later, in the debrief, you scroll through the screenshots together and they remember. That is what `BugReporter` does. The screenshot is taken with `ScreenCapture.CaptureScreenshot`, which on the Quest captures the eye buffer — literally what the tester saw.

## Math foundation

**Binning** a world position into a grid cell of size $c$ with minimum corner $(x_0, z_0)$:

$$i_x = \left\lfloor \frac{x - x_0}{c} \right\rfloor, \qquad i_z = \left\lfloor \frac{z - z_0}{c} \right\rfloor$$

Worked example: $x_0 = z_0 = -15$, $c = 1$, head at $(6.3, 1.6, 12.8)$: $i_x = \lfloor 21.3 \rfloor = 21$, $i_z = \lfloor 27.8 \rfloor = 27$. The one-dimensional index is $i_z \cdot \text{columns} + i_x = 27 \times 30 + 21 = 831$. Use `Mathf.FloorToInt`, not an `(int)` cast: for $x - x_0 = -0.4$ the cast gives 0 and the floor gives $-1$ (out of bounds, correctly rejected).

**Normalization and the color ramp.** Heat $t = \text{count} / \text{maxCount} \in [0, 1]$, color $= \text{Lerp}(\text{cold}, \text{hot}, t)$. A linear ramp makes a cell visited 2 times out of a maximum of 40 almost invisible ($t = 0.05$). A square-root ramp lifts it: $\sqrt{0.05} = 0.22$ — clearly blue. Cube height uses the same $t$: $h = \text{Lerp}(0.05, 1.5, t)$ meters.

**Rows per session.** At 2 Hz a 10-minute session produces $10 \times 60 \times 2 = 1\,200$ samples of about 30 bytes each — a 36 KB file. Twenty testers fit in under a megabyte. There is no need for anything cleverer than CSV.

**Time at the fork.** With samples every 0.5 s, a cell count of 14 in the fork cell means the tester spent $14 \times 0.5 = 7$ s within one meter of the fork — a hesitation you would never notice with a stopwatch.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_7_3.unity` with:

- **Floor** (30 m), a ring of **Trees**, sun and light fog; the **XR Origin (XR Rig)** at $(0, 0, -10)$ facing the course, with the simulator.
- **Course** — stone **Slabs** marking a path from the start to a fork at the origin; the **left** branch ends at a **Dead End Hedge**, the **right** branch runs between two hedges to the **Goal Shard** at $(6, 1.3, 12.5)$.
- **Task Sign** at the start: *Find the shard* (deliberately no hint about the dead end).
- **Confirm Label** — an empty red `TextMesh` parented to the camera, 1.2 m ahead and slightly below eye level; `BugReporter` writes into it.
- **Telemetry** — carries `HeatmapGrid` (origin $(-15, 0, -15)$, 30 × 30 cells of 1 m — the white outline in the Scene view), `PositionSampler` (interval 0.5 s, *Grid* pre-wired) and `BugReporter` (*Confirm Label* pre-wired).

Scripts live in `Assets/Scripts/`: `PositionSampler.cs`, `HeatmapGrid.cs`, `BugReporter.cs`.

## Your tasks (about 70 min)

**Task 1 (10 min) — Sample on a timer** · file: `Scripts/PositionSampler.cs`, TODO 1
Accumulate `Time.deltaTime` in `timer`; when it reaches `intervalSeconds`, subtract the interval (do not zero it) and call `SampleNow()`.
**Check:** with **Telemetry** selected, *Sample Count* climbs by two every second in Play mode.

**Task 2 (10 min) — Bin into cells** · file: `Scripts/HeatmapGrid.cs`, TODO 1–2
Implement `AddSample` with `Mathf.FloorToInt`, bounds checks, and the `maxCount` update; implement `ColorFor` as a cold→hot `Color.Lerp` (try the square-root ramp).
**Check:** walk in a small circle for ten seconds — *Max Count* and *Total Samples* rise; stand still and *Max Count* climbs by 2 per second while *Total Samples* does the same.

**Task 3 (15 min) — Draw it** · file: `Scripts/HeatmapGrid.cs`, TODO 3
In `OnDrawGizmos`, loop over the cells, skip empties, and draw a `Gizmos.DrawCube` per visited cell with height and color from the heat.
**Check:** while playing, the Scene view shows a trail of colored blocks behind your head; the cell where you stood longest is tallest and reddest. If you see nothing, check the Scene view's *Gizmos* toggle is on.

**Task 4 (15 min) — Persist and reload** · files: `Scripts/PositionSampler.cs`, TODO 2 · `Scripts/HeatmapGrid.cs`, TODO 5
Write the CSV in `WriteCsv` (header `t,x,y,z`, `CultureInfo.InvariantCulture`, `File.WriteAllText`) — it is called automatically when Play stops. Then implement `LoadCsv` so the grid can be filled in Edit mode from the file.
**Check:** walk the course once, stop Play (the Console prints the path and row count), then right-click the `HeatmapGrid` component header → **Load CSV** — the Scene view shows the heatmap of the session you just finished, with Play stopped.

**Task 5 (10 min) — Cubes in the world** · file: `Scripts/HeatmapGrid.cs`, TODO 4
Implement `SpawnCubes`: one `CreatePrimitive` cube per visited cell, collider destroyed, `.material.color` from `ColorFor`.
**Check:** press **H** while playing — the heatmap appears as physical blocks in the Game view (walk through them: no collision). Press **H** again: the old cubes are replaced, not doubled.

**Task 6 (10 min) — Bug button** · file: `Scripts/BugReporter.cs`, TODO 1–4
Detect the key (and the optional action), capture a screenshot with a timestamped name, append a CSV line (header first if the file is new), drop a marker, and show the confirmation.
**Check:** press **B** at the dead end — *Bug #1 saved* appears for two seconds, a red sphere floats where your head was, and `bugs.csv` plus `bug_<stamp>.png` appear in `persistentDataPath`. Then swap seats with a classmate: they play the course cold while you watch the Scene view, and you record the screencast.

## Stretch goals

- Time-at-cell: store `float[] seconds` next to `counts` and add `intervalSeconds` per sample; show seconds in a tooltip-style label above the hottest cell.
- Two-tester overlay: load two CSVs into two `HeatmapGrid` components with different `hot` colors and compare in the Scene view.
- Gaze heat: sample `Camera.main.transform.forward` too, raycast 10 m, and bin the *hit point* instead of the head — now the heatmap shows what testers looked at, not where they stood.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. On the headset there is no B key: drag an `InputActionReference` into `BugReporter → Report Action` — from the Starter Assets input actions, *XRI Right Interaction → Primary Button* (the A button) works well. Files land in the app's private folder; pull them with `adb pull /sdcard/Android/data/<your package name>/files/ .` and open the CSV on your laptop. The gizmo heatmap does not exist in a build (gizmos are Editor-only), so **Spawn Cubes** is how a tester sees the heatmap in VR — bind it to a second button if you want it live.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the gizmo heatmap growing in the Scene view while a classmate walks the course, a **Load CSV** after Play stopped, the spawned cubes in the Game view, and one bug report (label, marker, and the files on disk).
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), and one observation from your classmate's heatmap (did they take the dead end? how long at the fork?).
- (optional) Your `positions.csv` and one bug screenshot.

## Troubleshooting

| Symptom | Fix |
|---|---|
| *Sample Count* stays at 0 | `head` is null — the camera must be tagged *MainCamera* (the rig's camera is); or TODO 1 never calls `SampleNow()` |
| Heatmap never draws | Scene view *Gizmos* toggle off, `Draw Gizmos` unchecked, or `counts` still null because `AddSample` returns early — check `origin` and grid size cover where you walk |
| All cells the same color | `maxCount` is never updated in `AddSample`, so `Heat` returns 0 |
| CSV opens with one column or wrong numbers | Missing `CultureInfo.InvariantCulture` (locale uses `,` as decimal) — or the spreadsheet importer needs "comma" as the separator |
| **Load CSV** logs *no file at …* | Play was stopped before any sample, or `csvFileName` differs from the sampler's `fileName` |
| Screenshot file missing | `CaptureScreenshot` writes at end of frame — wait a second; on Windows check `%USERPROFILE%\AppData\LocalLow\<Company>\<Product>` |

Created by Isac Artzi

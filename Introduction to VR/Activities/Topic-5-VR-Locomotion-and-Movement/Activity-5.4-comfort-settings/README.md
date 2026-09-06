# Activity 5.4 — Comfort Settings and the Rest Point

Topic 5: VR Locomotion and Movement · Week 10, Thursday · Stepping stone to Milestone 5 — Movement Mechanics

## Where this fits

Over the last three meetings you built teleport with a blink, tuned smooth movement and turning, and added a dash. Every one of those had numbers in it — speed, snap angle, vignette strength, fade time — and so far *you* chose them. Milestone 5 asks you to hand those choices to the player and then to test, with real people, whether the result is comfortable. Today you build both halves. A **settings menu** exposes locomotion mode, speed, turn type, vignette strength, and a height offset; `ComfortSettings` saves them with `PlayerPrefs` and applies them to the rig's XRI providers. A **rest point** — a bench, a lantern, and the Guide — gives a player somewhere to stop. A **comfort log** runs a timed 5-minute test and records four symptom ratings, normalized to a 0–100 score, to a CSV you can quote in the milestone. You will run the test on a classmate before class ends.

*Elaria hook:* the glade at dusk is quiet. A ring of marker posts circles the meadow, and off to one side a lantern burns beside a bench. The Guide is sitting there. "You do not have to cross the whole world tonight," the Guide says. "Rest here as long as you need. The path will wait."

## Learning goals

- You can explain why comfort options are an accessibility feature, why defaults matter more than options, and why individual variation makes testing necessary.
- You can persist a small set of settings with `PlayerPrefs` and apply them to `TeleportationProvider`, `ContinuousMoveProvider`, `SnapTurnProvider`, and `ContinuousTurnProvider` at runtime.
- You can drive a world-space UI (toggles, sliders, buttons) from a settings object and back without feedback loops.
- You can run a short structured comfort test, normalize a 0–3 questionnaire to a 0–100 score, and write the result to a CSV file.
- You can design a rest point that invites rather than interrupts, and argue for its place in your project.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-5.4-comfort-settings`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → import **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup**: both lines `OK`.
6. **Activity → Build Starter Scene**. `Activity_5_4` opens: a dim glade, two dark panels floating ahead of you (Settings on the left, Comfort Log on the right), eight numbered posts in a ring, and a lantern-lit bench to the right with the Guide beside it.
7. Press **Play**. **Tab** to a controller, aim its ray at a panel, and hold the trigger key to drag a slider or click a toggle. Nothing changes yet — the menu is wired but the scripts are placeholders. Walk over to the bench (head selected, **W A S D**) and back.

Simulator controls that matter today: **Tab** (cycle head / left / right controller), **mouse** (aim), the **trigger** key while a controller ray points at a UI control (click / drag), **left-controller thumbstick** (smooth move, once you enable it in the menu), **right-controller thumbstick** (teleport forward, snap turn left/right), keys **0–3** (answer the current comfort-log question), **Esc** (release the cursor).

## VR theory

**Comfort options are accessibility.** Motion sickness in VR is not a matter of toughness; it depends on vestibular sensitivity, which varies enormously between people and is not something a player can train away in one session. Around a third of first-time users feel some discomfort with smooth locomotion; a smaller group cannot tolerate it at all; another group finds teleport disorienting and *prefers* smooth. There is no single setting that suits everyone, so a well-designed VR game ships a small set of well-chosen options — locomotion mode, turn type, speed, vignette, and often a seated/standing height adjustment — and makes them reachable without leaving the experience. The XRI providers you have used all week were designed to be toggled and tuned at runtime for exactly this reason. Braun and Rizzo's Chapter 3 covers the providers; the rest of this section is about the people using them.

**Defaults matter more than options.** Most players never open a settings menu, and the ones who need it most — a first-time user who is beginning to feel unwell — are the least likely to go looking for it. So the default profile should be the *safest* one: teleport, snap turn, moderate vignette, modest speed. Experienced players will find the menu and turn things up; a nauseated beginner will not find it and turn things down. Your `ComfortSettings` ships with those safe defaults, and *Reset* returns to them.

**Individual variation and testing.** Because you cannot predict a given person's response, you measure it. The full Simulator Sickness Questionnaire (SSQ) has sixteen items; an SSQ-lite of four — general discomfort, nausea, dizziness, eye strain — rated 0–3 catches most of the signal in thirty seconds and is standard practice for quick iteration. The protocol matters as much as the questions: the same task (laps of the ring) for the same time (5 minutes) under a named settings profile, then the questionnaire immediately, then a rest. Test one variable at a time — smooth at 1.5 m/s with vignette versus without, not smooth-with-vignette versus teleport-without. Two testers give you an anecdote; a class gives you a distribution.

**Onset and rest.** Symptoms build gradually and fade slowly; the worst mistake in a comfort test is to push through. Tell testers before you start that they may stop at any moment and that stopping is useful data, not failure. In the *game* the same principle becomes the rest point: a place that is clearly safe, where nothing is asked of the player, that quiets the world (lower ambience, warmer light) and lets them stay as long as they want. Good rest points are placed where players will naturally arrive tired — after a long traversal, before a big reveal — and they are invitations, never gates.

**Designing for those who need it most.** If a design works for the player who is most sensitive to motion, most easily disoriented, or seated in a wheelchair with a lower eye height, it works for everyone else too. The height offset in today's menu exists for that seated player; the rest point exists for the sensitive one; the vignette exists for the person who wants smooth locomotion but cannot yet handle the periphery. Designing this way is not a constraint on your creativity — it is where much of the craft of VR lives.

## Math foundation

**PlayerPrefs mapping.** `PlayerPrefs` stores ints, floats, and strings under string keys. Enums are ints underneath, so `LocomotionMode.Both` saves as `2` and loads with a cast: `(LocomotionMode)PlayerPrefs.GetInt("comfort.mode", 0)`. Every read takes a default so the first run (no keys yet) still produces valid values. Keep all keys as constants in one class so `Load` and `Save` can never disagree on a name.

**Questionnaire normalization.** Four items each rated $0$–$3$ give a total $T \in [0, 12]$. The normalized score is

$$\text{score} = 100 \cdot \frac{T}{3 \cdot n} = 100 \cdot \frac{T}{12}$$

Worked example: ratings $\{1, 0, 2, 1\}$ give $T = 4$ and a score of $33.3$. Ratings $\{0,0,0,0\}$ are $0$ (no symptoms); $\{3,3,3,3\}$ are $100$. Normalizing lets you compare with a colleague who used five questions ($3 \cdot 5 = 15$ in the denominator) and makes averages meaningful: three testers scoring 8, 17, and 25 under one profile average 16.7 — and if the same three score 33, 42, and 58 under another, you have a result, not an impression.

**Timer.** Elapsed seconds $s$ display as mm:ss with integer arithmetic: minutes $= \lfloor s / 60 \rfloor$, seconds $= s \bmod 60$. In C#, `s / 60` with `int s` is already the floor; `.ToString("00")` pads to two digits.

**Rest zone.** The player is resting when the horizontal distance from head to bench is at most $R = 2$ m: $|(h_x - b_x,\; h_z - b_z)| \le R$. Dropping $y$ means a tall player is not "farther away" than a short one. The light and sound ease with `MoveTowards` at a rate of $1 / t_{\text{ease}}$ per second, so a 1.5 s ease reaches full warmth 1.5 s after arrival.

**Low-pass noise.** The wind bed is white noise passed through a one-pole filter, $y_i = y_{i-1} + \alpha (x_i - y_{i-1})$ with $\alpha = 0.02$ — the same formula as `Mathf.Lerp(last, white, 0.02f)`. Small $\alpha$ keeps only slow changes, turning a hiss into a rumble; that is all a "filter" is.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_5_4.unity` with:

- **Floor** — a 50 m dusk-grass plane with a `TeleportationArea`.
- **XR Origin (XR Rig)** at the origin with the **XR Interaction Simulator**. The rig root carries `ComfortSettings` with *Origin*, all four providers, and *Vignette Quad* pre-assigned.
- **Vignette Quad** — a 0.7 m quad 0.2 m in front of the **Main Camera** whose material already has a radial ring texture (generated and saved by the builder as `Assets/Materials/VignetteRing.asset`); alpha 0 until movement.
- **Test Ring** — eight numbered posts on an 8 m radius, 10 m ahead, for laps.
- **Rest Point** — 11 m to the right: a wooden bench, a lantern post with a warm point light, the **Mysterious Guide**, an **Invite Label**, and a 2D **Wind Ambience** AudioSource (no clip; `RestPoint` generates one). The empty carries `RestPoint` with *Head*, *Lantern*, *Ambience*, and *Invite Label* pre-assigned.
- **Settings Panel** — a world-space Canvas (720 × 620 px at 0.001 scale) to your left, turned toward you, with two `ToggleGroup`s (Teleport / Smooth / Both; Snap / Smooth turn), three sliders (speed 1–4, vignette 0–1, height −0.3–0.3), Save and Reset buttons, and a status line. It carries `SettingsMenu` with every control assigned.
- **Comfort Log Panel** — to your right: Start button and timer, four question rows with 0–3 buttons and an echo text each, Submit, and a status line. It carries `ComfortLog` with *Settings*, *Rest Point*, all sixteen answer buttons, and the texts assigned.
- **EventSystem** with `XRUIInputModule`; signs for the ring and the rest point; low orange sun and blue-grey fog.

Scripts live in `Assets/Scripts/`.

## Your tasks (about 70 min)

**Task 1 (15 min) — Load, Save, Apply** · file: `Scripts/ComfortSettings.cs`, TODO 1–3
Read every value from `PlayerPrefs` with the Inspector value as default; write them all back and `PlayerPrefs.Save()`; in `Apply`, enable or disable the providers according to `mode` and `turnType`, and set `moveProvider.moveSpeed`. Test by changing *Mode* on the rig's component in Play mode and calling `Apply` through the menu (Task 2), or temporarily from `Update`.
**Check:** in *Teleport only* the move stick does nothing and the teleport ray works; in *Smooth only* the opposite; in *Both* both work. Snap vs smooth turn switches which thumbstick behavior you get.

**Task 2 (15 min) — The menu talks both ways** · file: `Scripts/SettingsMenu.cs`, TODO 1–4
Add listeners to every control; each handler writes one field on `settings`, calls `Apply`, and marks the status "Unsaved changes" — unless `refreshing` is true. `RefreshFromSettings` sets `isOn` and `value` on every control inside the `refreshing` guard so nothing writes back. Save calls `settings.Save()`; Reset calls `ResetToDefaults()` then refreshes.
**Check:** drag the speed slider — the label updates and the rig moves at that speed immediately. Press Save, stop Play, start again: the menu shows your values. Press Reset: everything snaps to Teleport / Snap / 1.5 / 60 % / 0.

**Task 3 (10 min) — Vignette and height** · file: `Scripts/ComfortSettings.cs`, TODO 4–5
Subscribe to the move provider's `locomotionStarted`/`locomotionEnded` so the vignette alpha equals `vignetteStrength` only while moving. Apply the height offset to `origin.CameraFloorOffsetObject`'s local y (capturing the base value first so repeated `Apply` calls do not stack).
**Check:** in *Smooth* mode, push the stick — edges darken by the slider amount — release — clear. Set height to +0.3: you stand taller (the ring posts look shorter); −0.3: shorter.

**Task 4 (10 min) — The rest point** · file: `Scripts/RestPoint.cs`, TODO 1–4
Generate the smoothed-noise wind loop, detect resting with a horizontal distance test, ease `restBlend` with `MoveTowards` and drive the lantern's intensity and color and the wind's volume from it, and show the Guide's invitation (facing the head) while resting. *Total Rest Seconds* accumulates for the log.
**Check:** walk to the bench: over about 1.5 s the lantern warms and brightens, the wind hushes, and the Guide's words appear. Walk away and it all eases back.

**Task 5 (20 min) — Comfort log and a real test** · file: `Scripts/ComfortLog.cs`, TODO 1–5
Wire the Start, Submit, and sixteen rating buttons (copy the loop index into locals before the lambda). Run the mm:ss timer and stop at `testSeconds`. Implement `Score` and the CSV append with a header on first write. Then **run the protocol on a classmate**: set a profile (say *Smooth, 2 m/s, snap, vignette 60 %*), press Save, press Start, have them walk laps of the ring 1 → 8 for five minutes (they may rest at the bench any time and may stop any time), then rate the four questions and Submit. For the screencast, a 1-minute test is fine — set *Test Seconds* to 60 and say so. Record the panel, a lap, the rest point, and the CSV opened in a text editor or spreadsheet.
**Check:** the status shows *Score NN / 100 — saved to comfort_log.csv*; the file in `Application.persistentDataPath` (Windows: `%USERPROFILE%\AppData\LocalLow\<Company>\<Product>\`; macOS: `~/Library/Application Support/<Company>/<Product>/`) has the header and one row per Submit, with the settings profile in the row.

## Stretch goals

- Add a fifth question, *Headache*, and confirm your `Score` still normalizes to 0–100 (the denominator becomes 15).
- Bring in your 5.3 `DashLocomotion` and add a *Hybrid* mode that enables it alongside teleport; record it as `3` in PlayerPrefs.
- Compute, in the log status, the running **mean score per profile** by reading the CSV back (`File.ReadAllLines`, split on commas) so testers see how their profile compares.

## Reflection (for the separate worldview scene assignment — not part of today's Padlet post)

Your syllabus asks you to analyze how elements of a Christian worldview can be integrated into VR game design. This week's work offers a concrete way in, and it has nothing to do with putting religious content on screen. Consider what the rest point and the comfort menu *are*: a bench that asks nothing of the player; a light that warms when someone tired arrives; defaults chosen for the person most likely to be hurt rather than the one most likely to complain; a test protocol whose first rule is that anyone may stop. Traditions across the world — including the Christian one, with its language of hospitality toward the stranger, of rest as a good in itself, and of attending first to those with the least — give names to those design instincts. Whether or not you share that tradition, the assignment invites you to notice where such instincts show up in your own design choices, and to say so plainly. Use these questions as a starting point for that write-up:

1. Where in your Realm of Legends project does the design *care for* the player — by preventing harm, offering rest, or making room for someone less able than the median tester? Where does it not yet, and what would change?
2. The Guide says "the path will wait." What does it mean, in game-design terms, to build an experience that is hospitable — that welcomes rather than tests — and where is the line between hospitality and removing all challenge?
3. Who is the player your project currently serves *least* well (motion-sensitive, seated, new to VR, easily disoriented)? Describe one change you would make if you designed for that person first, and what everyone else would gain from it.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. On the headset, both panels are driven by the controller ray and trigger; the keyboard fallbacks are irrelevant. Two things to verify there: the height offset — in Floor tracking mode `XROrigin` may reset the floor-offset object when tracking starts, so if your offset vanishes, re-apply it a second after the scene loads; and the CSV path, which on Android is `/storage/emulated/0/Android/data/<package>/files/comfort_log.csv` — pull it with `adb pull`. Run the real 5-minute protocol on the headset for the milestone; desktop results tell you the pipeline works, not how a person feels.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the settings menu changing locomotion mode and speed with the effect visible, the vignette while moving, Save then a restart of Play with the values restored, the rest point warming as you arrive, and a comfort-log run ending in a Submit with the CSV opened.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus your classmate's score and the profile it was recorded under.
- (optional) the `comfort_log.csv` your test produced.

## Troubleshooting

| Symptom | Fix |
|---|---|
| **Activity** menu is missing from the menu bar | A script has a compile error; open the Console, fix the red line, wait for the spinner at bottom-right to finish |
| The controller ray will not click a toggle or button | The panel must have `TrackedDeviceGraphicRaycaster` and the scene an **EventSystem** with `XRUIInputModule`; rebuild the scene if you removed either. Also make sure the trigger key is held while the ray is on the control |
| Dragging a slider fires the status "Unsaved changes" during Reset | `RefreshFromSettings` is not setting `refreshing = true` before it writes to the controls, or a handler skips the guard |
| Settings do not survive a restart | `Save` never called `PlayerPrefs.Save()`, or the menu's Save button listener is missing (TODO 4) |
| Toggling *Smooth only* still lets me teleport | `Apply` does not disable `teleportProvider`, or the field is null — check the rig's ComfortSettings component in Play mode |
| The vignette darkens the whole view, not just the edges | The material lost its ring texture; re-run **Build Starter Scene** or assign `Assets/Materials/VignetteRing` to the material's texture slot |
| Submit says "Answer all four questions first" though I clicked them | Buttons were wired with the loop variable captured directly; copy `i` into `q` and `v` inside the loop before the lambda |
| No CSV appears | Look at the Console line printed by `Submit` for the exact path; on Windows the folder is under `AppData\LocalLow`, which is hidden by default |

Created by Isac Artzi

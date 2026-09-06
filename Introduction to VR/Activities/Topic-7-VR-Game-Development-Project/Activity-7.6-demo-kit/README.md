# Activity 7.6 — Build, Record, Present

Topic 7: VR Game Development Project · Week 15, Thursday · Stepping stone to Milestone 7 — Final Integration

## Where this fits

This is the last class meeting, and it is about the one thing every VR project eventually has to do: be shown to people who are not wearing the headset. By the end of class you will have a demo kit you can drop into your team's Milestone 7 project — a start scene with a world-space menu that jumps to any environment or feature, a screenshot tool that hides the tooling for a clean frame, and a countdown in your view so the presenter knows the time without anyone shouting. The rest of this README is the part most teams skip until it is too late: how to record on the Quest, how to cast to a laptop so the room can see, and a presentation checklist for the final milestone.

*Elaria hook:* the hero returns to the Hub with the artifact whole. The Guide has one last request: "Show them. Not the ending — the journey. And bring a lantern in case the mist closes in." The lantern is your backup video.

## Learning goals

- You can build a scene-jump menu with `Button.onClick` listeners and explain the closure-capture bug that makes every button load the last scene.
- You can capture a clean, super-sized screenshot from code and say where it lands on desktop and on the Quest.
- You can record gameplay on a Quest 3 with the built-in recorder and cast the headset view to a laptop for a live audience.
- You can structure a 5–7 minute technical presentation (overview, process, challenges, demo, reflection) and rehearse it against a timer.
- You can name the three failure modes of a live VR demo and the mitigation for each.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-7.6-demo-kit`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup** — two `OK` lines.
6. **Activity → Build Starter Scene**. The builder creates **three** scenes (`Activity_7_6`, `Demo_Forest`, `Demo_Ruins`), adds them to **File → Build Profiles → Scene List**, and reopens `Activity_7_6`.
7. Press **Play**. A dark panel floats 2 m ahead with three buttons. Top-right of your view is the (empty) timer; the buttons do nothing yet.

Simulator controls that matter today: **Tab** to a controller, aim its ray at a button, **T** (trigger) to click. Keys the scripts give you: **1 2 3** jump to scenes, **P** takes a screenshot, **T** starts/pauses the timer and **Y** resets it (with the head selected, so the trigger key does not fire). If keys collide in your simulator version, change them in the Inspector — every key is a field.

## VR theory

**Demoing VR is a broadcast problem.** Exactly one person is inside the experience; everyone else sees a flat rectangle of it, if anything. A live VR demo therefore has three parts, and you should plan all three: the *headset view* (cast to a screen so the room follows), the *bystander* (the presenter or a teammate narrating what the player feels — presence, scale, comfort — because the screen cannot show it), and the *script* (a fixed route through the game that shows each system once). Braun and Rizzo close their project chapter with the presentation of the finished work; treat it as part of the build, not an afterthought.

**The three ways a live demo dies**, and what to bring for each. *Tracking or pairing fails* (guardian lost, the Quest goes to sleep, casting drops) → a **backup video** recorded this week, on the laptop, ready to play. *The player gets lost or stuck* → the **demo menu**: jump straight to the next feature instead of walking there. *Time runs out* → the **timer** in the presenter's view and a script with a "shortest path" marked. None of these are signs of weakness; every professional VR demo has all three.

**Recording on the Quest 3.** The headset records what the wearer sees, with audio. Press the **Meta button** on the right controller to open the universal menu, choose **Camera**, then **Record video** (a countdown, then a red dot). Press the Meta button again and **Stop** when done. Videos are saved on the headset under `Movies`; transfer them with a USB cable (the Quest appears as a drive on Windows; on macOS use the *Android File Transfer* app) or share them through the Meta Horizon phone app. Default recordings are 1024 × 1024 at 24 fps and a bit fish-eyed — fine for a Padlet post; for the presentation, record twice and keep the calmer take. Move your head slowly: what feels smooth inside reads as frantic on a flat screen.

**Casting to a laptop.** Two routes. (1) In a browser on the laptop open **oculus.com/casting**, sign in with the same Meta account as the headset, then in the headset: Meta button → **Camera** → **Cast** → choose the computer. (2) Cast to the **Meta Horizon** phone app and mirror the phone to the projector. Both need the headset and the receiving device on the same Wi-Fi, and both add half a second of latency and a lot of bandwidth — on a crowded classroom network casting can stutter even when the headset is perfectly smooth. Say so to the audience before you start, or use a recorded video and demo live only for questions. A cable alternative is `adb` screen mirroring with **scrcpy**, which is more robust but shows both eyes side by side.

**Screenshots for slides.** `ScreenCapture.CaptureScreenshot` grabs the frame at the end of rendering; on desktop the `superSize` parameter renders at 2–4× for crisp slides. In the headset it captures the eye buffer at native size. Hide your tooling (timer, menu, debug labels) for that frame — a screenshot with a debug HUD in the corner tells the reviewer the project is not finished.

## Math foundation

Almost none today, by design. Two small pieces of arithmetic will keep you on time.

**Talk budget.** A 6-minute slot at a comfortable 140 words per minute is about 840 words — roughly one page. Allocate by section: overview 1 min (140 words), process 1 min, challenges 1.5 min, demo 2 min (*with* narration, so fewer words), reflection 0.5 min. The timer's `warnSeconds = 60` is the "start wrapping up" signal.

**Timer formatting.** For $s$ seconds remaining: minutes $= \lfloor s / 60 \rfloor$, seconds $= \lfloor s \bmod 60 \rfloor$, each zero-padded to two digits with the `"00"` format. Example: $s = 187.4 \rightarrow 03{:}07$. Past zero, use $|s|$ and prefix `+`: $s = -61 \rightarrow +01{:}01$.

**Video sizing.** A 3-minute Quest recording at 1024 × 1024, 24 fps, ~8 Mbit/s is about $3 \times 60 \times 8 / 8 = 180$ MB — too big for most upload forms. Trim to the 90 seconds you need, or re-encode to 1080p at 4 Mbit/s (~45 MB) with any free editor.

## The starter scene

**Activity → Build Starter Scene** creates and saves three scenes under `Assets/Scenes`, all in the Scene List:

- **Activity_7_6 (Demo Start)** — a stage of low blocks; **Demo_Forest** — trunks and crowns; **Demo_Ruins** — a ring of columns. Distinct floor colors and fog so a recording shows which is which.
- In **every** scene: the **XR Origin (XR Rig)** with the simulator; a **Demo Menu** world-space canvas (1 px = 1 mm) 2 m ahead with a title and three buttons *1. Demo Start*, *2. Enchanted Forest*, *3. Ancient Ruins*, carrying `DemoMenu` with *Scene Names* and *Buttons* pre-filled; an **EventSystem** with `XRUIInputModule`.
- **Timer Label** — a `TextMesh` parented to the camera (top-right of the view) carrying `DemoTimer` (5 minutes).
- **Capture Label** — a `TextMesh` under the view for the screenshot confirmation.
- **Demo Tools** — carries `ScreenshotTool` with *Confirm Label* set and *Hide During Capture* listing the timer, the menu, and the capture label.
- A **Scene Sign** naming the scene and the keys.

Scripts live in `Assets/Scripts/`: `DemoMenu.cs`, `ScreenshotTool.cs`, `DemoTimer.cs`.

## Your tasks (about 70 min)

**Task 1 (15 min) — The jump menu** · file: `Scripts/DemoMenu.cs`, TODO 1–2 and 4
Wire each button in `Start` — copy `sceneNames[i]` into a local before the lambda — implement `Load` with `SceneManager.LoadSceneAsync`, and color the current scene's button in `HighlightCurrent`.
**Check:** click *2. Enchanted Forest* with the controller ray — the forest loads, and its menu shows the second button green. Deliberately capture `i` instead of the local and click button 1: it loads the ruins (the last entry). Put the local back.

**Task 2 (5 min) — Number keys** · file: `Scripts/DemoMenu.cs`, TODO 3
Map keys 1–9 to the list with `(Key)((int)Key.Digit1 + i)`.
**Check:** with the head selected, **1 2 3** hop between the three scenes.

**Task 3 (15 min) — Clean screenshots** · file: `Scripts/ScreenshotTool.cs`, TODO 1–3
Detect the key/action, hide the listed objects for a frame, `CaptureScreenshot(path, superSize)`, wait for end of frame, restore, confirm.
**Check:** press **P** — the capture label names the file for 1.5 s; the PNG in `persistentDataPath` shows the scene *without* timer or menu. Set *Super Size* to 2 and the next file has twice the pixel dimensions.

**Task 4 (10 min) — The countdown** · file: `Scripts/DemoTimer.cs`, TODO 1–3
Start/pause on **T**, reset on **Y**, count down with `Time.unscaledDeltaTime`, format `mm:ss`, yellow in the last minute, red `+mm:ss` when over.
**Check:** set *Total Minutes* to 0.1 (6 s) in the Inspector and press **T**: `00:06` … `00:00`, then `+00:01` in red. Set it back to your real slot length.

**Task 5 (25 min) — Record and rehearse** · no new code
Put the kit to use. (a) Record a **backup video** of these three scenes with your screen recorder (or the Quest if you have one at your seat — see *Port to Quest 3*). (b) Write your demo script as five lines, one per checklist section below, with the scene number to jump to for each. (c) Rehearse once against the timer with a teammate watching the Game view as the "audience"; note where you ran long. This rehearsal *is* the screencast for today.

### Presentation checklist (Milestone 7)

Use this order; it is what reviewers expect from a technical project presentation, and it keeps the demo in the middle where attention is highest.

1. **Game overview (1 min)** — one sentence of premise (the hero, Elaria, the artifact), the three environments, the core loop (explore → collect → solve → return). Show a screenshot, not a video, so you control the pace.
2. **Development process (1 min)** — the milestones as stepping stones: greybox → environments → interaction and UI → locomotion → audio → integration. One slide with the timeline; say who did what.
3. **Challenges and solutions (1.5 min)** — pick two real ones (the frame budget in 7.5, the save-file design in 7.2, a comfort issue from testing) and for each: what broke, what you measured, what you changed. Numbers beat adjectives.
4. **Demo (2 min)** — cast or video. Use the demo menu: Forest (a pickup with juice), Ruins (the puzzle), Hub (quest and save). Narrate presence, scale, and comfort — the things the flat screen cannot show.
5. **Reflection (0.5 min)** — what you would do with two more weeks, and one thing you learned about designing *for a body* rather than for a screen.
6. **Backup** — the video is on the laptop, the build is installed on the headset and charged, the casting page is already logged in, and a teammate knows the shortest path through the demo if the presenter has to hand over the headset.

## Stretch goals

- Add the fade from Activity 7.1's `SceneLoader` in front of `DemoMenu.Load`, so scene jumps blink instead of pop on the recording.
- A **bystander camera**: a second `Camera` on a tripod object that renders to the desktop window (set its *Target Eye* to *None* and *Depth* higher than the XR camera) so the room sees a stable third-person view while the presenter's head moves.
- Autoplay demo: a `DemoScript` component with a list of (scene, seconds) that jumps automatically — a kiosk mode for an open-house table.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject in all three scenes and follow `Docs/PortingToQuest3.md`. Then do the two things this activity is about. **Record:** Meta button → **Camera** → **Record video**; play your route; Meta button → **Stop**; connect the USB cable and copy the file from `Movies`. **Cast:** open `oculus.com/casting` on the laptop (same Meta account), then Meta button → **Camera** → **Cast** → the computer; or cast to the **Meta Horizon** phone app. Assign `ScreenshotTool → Capture Action` to a controller button (e.g. *XRI Left Interaction → Primary Button*) so you can take slide screenshots in the headset — they land in the app's `files/` folder; pull them with `adb pull`.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet: your rehearsed run — the timer visible, at least two scene jumps through the menu, one screenshot taken, and your narration following the five checklist sections.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus where you ran long in the rehearsal and what you cut.
- (optional) Your backup video and two clean screenshots for the Milestone 7 slides.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Every button loads the same (last) scene | Classic closure bug: copy `sceneNames[i]` into a local variable inside the loop before the lambda |
| Button click does nothing | `Load` still has its placeholder; or the target scene is missing from **File → Build Profiles → Scene List** (Console shows the error) |
| Ray does not hit the menu | The canvas needs `TrackedDeviceGraphicRaycaster` (the builder adds it) and the `EventSystem` needs `XRUIInputModule` — delete any extra `EventSystem` |
| Screenshot still shows the timer | The hide happens in the same frame as the capture — `yield return null` once *before* `CaptureScreenshot` |
| Timer runs while paused or frozen while playing | Use `Time.unscaledDeltaTime`; the presenter's clock must not follow `Time.timeScale` |
| Casting stutters or drops | Classroom Wi-Fi; switch to the pre-recorded video and demo live only for questions, or mirror with a USB cable via scrcpy |

Created by Isac Artzi

# Activity 2.3 — Menus in Space

Topic 2: VR Game Design Principles on a Variety of Platforms · Week 4, Tuesday · Stepping stone to Milestone 2 — Platform Compatibility Design

## Where this fits

By the end of class you will have a pause menu that lives *in the world*: a 0.6 × 0.4 m panel floating 1.5 m in front of you with Start, Resume and Quit buttons that you press by pointing a controller ray and pulling the trigger. The panel follows your head lazily — it stays where it is while you glance around and glides back in front of you only when you turn away — and its buttons swell, tint and beep as you use them. Pausing freezes a cloud of fireflies mid-air while the menu keeps working. Milestone 2 asks for movement, interaction and UI on the headset; with 2.1 and 2.2 done, this is the UI, and it is the same Canvas recipe you will use for the inventory panel (4.1), dialogue boxes (4.2), settings (5.4) and the demo menu (7.6).

*Elaria hook:* the Mysterious Guide hands you a spellbook that opens in the air wherever you look. "The world will hold its breath while you read," the Guide says — and it does: when the book is open, the fireflies hang frozen over the clearing until you close it.

## Learning goals

- You can explain why Screen Space canvases are useless in VR and build a World Space canvas with `TrackedDeviceGraphicRaycaster` and an `EventSystem` running `XRUIInputModule`.
- You can convert between canvas pixels and meters (1 px = 1 mm at scale 0.001) and size text so it stays legible at 1.5 m.
- You can wire `Button.onClick` in code, pause and resume the simulation with `Time.timeScale`, and say which scripts must switch to unscaled time.
- You can implement a lazy follow with a dead-zone angle and exponential smoothing, and contrast it with head-locked UI.
- You can respond to `IPointerEnter/Exit/ClickHandler` events and generate a simple sine beep with `AudioClip.Create`.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-2.3-menus-in-space`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** tab → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. Menu bar → **Activity → Check Setup**. Both lines should say `OK`.
6. Menu bar → **Activity → Build Starter Scene**. The scene `Activity_2_3` opens with the menu panel already visible 1.5 m ahead.
7. Press **Play**. Fireflies drift around the Guide; the spellbook hangs in front of you.

Simulator controls that matter today: **Tab** to the **right controller**, aim its ray at a button (the Starter Assets *Near-Far Interactor* also drives UI), and press **Trigger** — the *UI Press* action — to click. Check the simulator's on-screen panel for the key; **T** in the default layout. **P** toggles pause once you have implemented it. Use the head (Tab) plus the mouse to look away from the panel and watch whether it follows.

## VR theory

**World space versus screen space.** A Unity Canvas in *Screen Space – Overlay* is painted on top of the final image, pixel-aligned to the display. In a headset there is no single display image: two eye images are rendered from slightly different positions and then warped for the lenses. An overlay canvas either appears in one eye only, floats at infinity with no stereo depth, or is not shown at all. So every VR canvas is a **World Space** canvas — a rectangle with a position, rotation and scale like any other object — and must be pointed at, not clicked with a cursor. Braun and Rizzo cover VR UI placement and interaction in Chapter 5 of *XR Development with Unity*.

**Rays as pointers.** The `EventSystem` is Unity's UI event dispatcher; it normally receives mouse positions from a *Standalone Input Module* and asks each canvas's `GraphicRaycaster` what is under the cursor. XRI swaps both halves: `XRUIInputModule` feeds the EventSystem with tracked-device rays and their *UI Press* button, and `TrackedDeviceGraphicRaycaster` on each canvas answers "what does this ray hit?" The beautiful consequence is that the standard `Button`, `Toggle`, `Slider` components and the `IPointer…Handler` interfaces work unchanged: hover is *pointer enter*, trigger is *click*.

**Scale: 1 pixel = 1 millimeter.** A canvas's RectTransform is measured in pixels; its world size is pixels × scale. Scaling the canvas by 0.001 makes a 600 × 400 canvas 0.6 × 0.4 m — a hardcover book held at arm's length — and lets you design in familiar pixel units (a 36-px font, an 80-px button) while thinking in millimeters. Never scale the *buttons*; scale the canvas once.

**Distance and legibility.** Comfortable UI sits between roughly 1 and 2 m: closer forces the eyes to converge hard and fights with stereo; farther makes text small and the panel easy to lose. Text needs about 1° of angular size per character height to be read comfortably; at 1.5 m that is about 2.6 cm, i.e. a 26-px font at 1 px = 1 mm. Our 36-px labels are generous on purpose — Quest 3's display is sharp but still far below a monitor's pixel density in the center of view.

**Head-locked versus lazy follow.** A **head-locked** panel is parented to the camera: it never leaves your face, blocks the world, and — because it moves *with* your head — appears frozen in a way nothing in reality is; many players find it uncomfortable within minutes, and it fights with the vestibular system exactly like smooth locomotion does. A **lazy follow** places the panel in the world and only re-targets when you have clearly turned away (a dead zone of 25–35°), then glides rather than snaps. The panel behaves like a helpful companion: nearby, but never in your face. World-anchored UI (a panel bolted to a wall or a wrist) is the third option and often the best when the context allows it — Topic 4 uses a wrist inventory.

**Pausing in VR.** `Time.timeScale = 0` stops everything driven by scaled time: physics, particles, animations, any script that multiplies by `Time.deltaTime`. It does *not* stop head tracking, XRI rays, or the EventSystem, which is exactly what you want: the world freezes, the player and the menu do not. The rule for your own scripts is simple — anything that must run during pause (the menu's follow, button animations) reads `Time.unscaledDeltaTime`. Never black the screen or stop tracking to "pause" in VR; a frozen view that ignores head motion is the fastest route to nausea.

## Math foundation

**Canvas size in meters.** With scale $s$ per pixel, a rectangle of $w \times h$ pixels spans $ws \times hs$ meters. For $s = 0.001$: the canvas is $600 \times 0.001 = 0.60$ m wide and 0.40 m tall; a button of $360 \times 80$ px is $0.36 \times 0.08$ m.

**Angular size of UI.** From 1.1, an object of height $h$ at distance $d$ subtends $\theta = 2\arctan\!\left(\frac{h}{2d}\right)$. At $d = 1.5$ m: the whole panel spans $2\arctan(0.30/1.5) = 22.6°$ wide and $2\arctan(0.20/1.5) = 15.2°$ tall — comfortably inside the roughly 60° you can take in without moving your head. A 36-px label is 0.036 m tall: $\theta = 2\arctan(0.018/1.5) = 1.37°$, above the 1° legibility floor. An 80-px button is $3.05°$ tall — an easy target for a ray that wobbles by about half a degree in an untrained hand.

**Exponential smoothing** (again, deliberately): the panel moves toward its target by the fraction $1 - e^{-k\Delta t}$ each frame,

$$x \leftarrow x + (x_{target} - x)\left(1 - e^{-k\,\Delta t}\right)$$

With $k = 4$ and $\Delta t = 0.0139$ s (72 Hz): $1 - e^{-0.0556} = 0.054$ — 5.4% of the remaining gap per frame; after 0.25 s the gap has shrunk to $e^{-1} \approx 37\%$, after 0.75 s to 5%. The time constant is $1/k = 0.25$ s. If you prefer `Vector3.Lerp(a, b, k·dt)`: at $k\,\Delta t = 0.0556$ the two are nearly identical, but the exponential form stays correct on a hitchy frame where $\Delta t$ spikes.

**The dead zone.** The angle between the head's flat forward $\hat f$ and the flat direction to the panel $\hat p$ is $\theta = \arccos(\hat f \cdot \hat p)$ — `Vector3.Angle` does this for you. With a 30° threshold and a panel 22.6° wide, you can look at either *edge* of the panel (11.3° off center) with margin to spare before it re-targets.

**A tone** is $y(t) = A\sin(2\pi f t)$. At $f = 880$ Hz sampled at 48 000 Hz, one cycle is $48000 / 880 \approx 54.5$ samples, and an 80 ms beep is $0.08 \times 48000 = 3840$ samples. Multiplying by a linear fade $(1 - i/n)$ avoids the click a hard cut-off would make.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_2_3.unity` with:

- **Floor** (30 m grass), a ring of ten trees, dusk lighting and blue fog.
- **XR Origin (XR Rig)** at the origin, the **XR Interaction Simulator**, and an **XR Interaction Manager**.
- **Mysterious Guide** 4.5 m ahead and **Fireflies** — a `ParticleSystem` of 300 glowing motes drifting through the clearing. They exist so you can *see* the pause.
- **Menu Canvas** — a World Space `Canvas` (600 × 400 px, scale 0.001, at (0, 1.4, 1.5)) with `CanvasScaler` and `TrackedDeviceGraphicRaycaster`. Children: a dark **Panel** image, **Title** and **Subtitle** texts (legacy `Text` with `LegacyRuntime.ttf`), and **Start / Resume / Quit Button**s (360 × 80 px, `Image` + `Button` with *Transition: None*, an `AudioSource` with *Ignore Listener Pause*, and `MenuButtonFeedback`). Label texts have *Raycast Target* off so the ray hits the button, not the label. The canvas carries `FollowHeadLazy` (distance 1.5 m, height −0.15 m).
- **EventSystem** with `XRUIInputModule`.
- **Menu Controller** — an empty GameObject with `PauseMenuController`; *Menu Root*, the three buttons, *Title Text* (the subtitle) and the **Status Label** TextMesh are pre-wired.
- A **Welcome Sign** with today's controls.

Scripts live in `Assets/Scripts/`.

## Your tasks (about 70 min)

**Task 1 (5 min) — Point at things** · no code
Press Play. Tab to the right controller and sweep the ray across the panel. Nothing reacts yet (the feedback script is empty), but the ray should visibly stop *on* the panel rather than pass through it. Select **EventSystem** while playing and confirm the `XRUIInputModule` is the active module.
**Check:** the ray's end point sits on the canvas. If it goes through, see Troubleshooting.

**Task 2 (15 min) — Wire the buttons and the states** · file: `Scripts/PauseMenuController.cs`, TODO 1–2
In `Start`, add `OnStartPressed` / `OnResumePressed` / `OnQuitPressed` as listeners on the three buttons' `onClick`. In `SetState`, show the menu when not Playing, set `Time.timeScale` to 0 while Paused (and `AudioListener.pause`), show Start only in *NotStarted* and Resume only in *Paused*, and rewrite `titleText` with a one-line hint.
**Check:** Trigger on Start hides the menu and the fireflies drift; the Status Label shows *State: Playing timeScale: 1.0*.

**Task 3 (10 min) — Toggle pause** · file: `Scripts/PauseMenuController.cs`, TODO 3
In `Update`, read `Keyboard.current[toggleKey].wasPressedThisFrame` (and the optional action) and flip between Playing and Paused.
**Check:** **P** freezes the fireflies mid-air and brings back the menu with *Resume*; **P** again or Trigger on Resume releases them. Quit stops Play mode and leaves `timeScale` at 1.

**Task 4 (20 min) — Lazy follow** · file: `Scripts/FollowHeadLazy.cs`, TODO 1–3
In `LateUpdate` (unscaled time!), measure the angle between the head's flat forward and the flat direction to the panel; when it exceeds `angleThreshold`, set a new target with `ComputeTarget()`. Glide with `1 − e^(−k·dt)`, then face the head with `Quaternion.LookRotation(panel − head)`.
**Check:** while paused, look 20° left — the panel stays; look 45° — it slides over in about half a second, upright and legible. Set *Angle Threshold* to 0 for a moment to feel what head-locked UI is like, then put it back to 30.

**Task 5 (15 min) — Hover, click and beep** · file: `Scripts/MenuButtonFeedback.cs`, TODO 1–4
Set `targetScale` and tint in `OnPointerEnter/Exit`, ease the scale each frame with unscaled time, dip the scale and play the beep in `OnPointerClick`, and implement `MakeBeep` with a sine wave and a linear fade.
**Check:** buttons grow 10% and turn light blue under the ray, bounce on click, and beep from their position in space — including while paused (the AudioSources ignore listener pause).

**Task 6 (5 min) — Screencast** · no new code
Record: Start, look around with the panel following lazily, pause with the fireflies frozen, hover and click feedback, Resume, Quit.

## Stretch goals

- Add a fourth button, **Settings**, that toggles a second panel (a child of the canvas) with a `Slider` that sets `FollowHeadLazy.distance` between 1 and 2 m — you will need `Slider.onValueChanged.AddListener`.
- Fade the panel in and out with a `CanvasGroup.alpha` tween on show/hide instead of `SetActive` (remember unscaled time).
- Make the beep pitch depend on the button: `MakeBeep(660, …)` for Quit, `MakeBeep(990, …)` for Start — a tiny audio language players learn without noticing.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. The ray and *UI Press* on the trigger work identically on Touch controllers. Assign *Toggle Action* on `PauseMenuController` to a menu-style button action from the *XRI Default Input Actions* asset (the left hand's menu button is the convention) so you can pause with the headset on. On the headset, read the panel at 1.5 m and then move it to 2.5 m in the Inspector (before building) or via a Settings slider: note which is easier on the eyes and whether 36-px text is still legible. Do *not* try the head-locked experiment (threshold 0) for more than a few seconds.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: pressing Start with the ray, the panel following lazily as you look around, pausing with the fireflies frozen while the buttons still swell and beep, Resume, and Quit.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus one sentence on what changed when you set *Angle Threshold* to 0.

## Troubleshooting

| Symptom | Fix |
|---|---|
| The ray passes straight through the panel | The canvas lacks `TrackedDeviceGraphicRaycaster`, or there is no `EventSystem` / it has a *Standalone Input Module* instead of `XRUIInputModule`; rebuild the scene |
| Buttons highlight but Trigger does nothing | TODO 1 in `PauseMenuController` is empty (no listeners), or the *UI Press* action is not bound — check the simulator panel for the Trigger key |
| Menu appears but text is invisible | The `Text` font is missing; the builder uses the legacy UI `Text` component with `LegacyRuntime.ttf` — keep it, do not swap in the TMP package's text component |
| Everything is frozen, including the menu's glide | `FollowHeadLazy` or `MenuButtonFeedback` uses `Time.deltaTime`; switch to `Time.unscaledDeltaTime` |
| Panel text is mirrored | TODO 3 in `FollowHeadLazy` subtracts in the wrong order; the canvas forward must point *away* from the head |
| Panel runs away every frame | *Angle Threshold* is 0, or TODO 1 re-targets unconditionally |
| No beep on click | `MakeBeep` still returns null, or the button's `AudioSource` lost *Ignore Listener Pause* while the game is paused |

Created by Isac Artzi

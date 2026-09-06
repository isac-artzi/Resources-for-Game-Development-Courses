# Activity 5.2 — Smooth Move and Turn

Topic 5: VR Locomotion and Movement · Week 9, Thursday · Stepping stone to Milestone 5 — Movement Mechanics

## Where this fits

Teleport is safe; smooth locomotion is immersive — and it is the mode that sends a third of first-time players looking for a chair. Milestone 5 needs both, and it needs you to *tune* smooth movement rather than accept the defaults. By the end of class the rig's `ContinuousMoveProvider`, `SnapTurnProvider`, and `ContinuousTurnProvider` will be driven by a world-space panel you can adjust while playing: move speed from 1 to 4 m/s, snap angle 30/45/90°, smooth-turn speed 30–180°/s, and a toggle between head-relative and hand-relative movement. Underneath, a `SpeedRamp` eases the speed in over 0.3 s so the stick never slams you from standstill to full speed, and every snap turn produces a soft click. In 5.4 these same knobs become the player's comfort settings; today you learn what each one does to your own stomach.

*Elaria hook:* the forest path bends between three stone arches, and the Guide is waiting at the far end. "Walk," the Guide says. "Do not lurch." The trees close to the path are there to be felt sliding past — that is optic flow, and it is the whole subject of today.

## Learning goals

- You can explain vection and optic flow, and say why acceleration — not speed — is the main trigger of motion sickness in smooth locomotion.
- You can set `moveSpeed`, `turnAmount`, `turnSpeed`, and `forwardSource` on the rig's locomotion providers from UI events at runtime.
- You can subscribe to a provider's `locomotionStarted`/`locomotionEnded` events and use them to drive a speed ramp and an audio cue.
- You can shape a speed ramp with `Mathf.SmoothStep` and explain why its zero initial acceleration matters.
- You can compare head-relative and hand-relative movement direction and argue which one your project should default to.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-5.2-smooth-and-turn`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → import **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup**: both lines `OK`.
6. **Activity → Build Starter Scene**. `Activity_5_2` opens: a forest clearing, a winding slab path through three glowing arches, and a dark tuning panel floating 1.6 m in front of you.
7. Press **Play**. Press **Tab** until the *left controller* is selected; the simulator's panel names the key bound to its thumbstick (*Primary 2D Axis*). Push it forward: you glide along the ground — this is the stock `ContinuousMoveProvider`. Select the *right controller* and push its thumbstick left or right: you snap-turn. Do both a few times before writing code so you know the baseline (instant acceleration, no sound).

Simulator controls that matter today: **Tab** (cycle head / left / right controller), **left-controller thumbstick** key (smooth move), **right-controller thumbstick** left/right (snap turn), **mouse** (rotate the selected device — with the head selected this turns your view, which is how you feel head-relative vs hand-relative), **trigger** key with a controller selected while pointing its ray at the panel (drag a slider), and the keyboard nudges your `MoveTuner` adds: **-** / **=** speed, **[** snap angle, **]** hand-relative toggle. **Esc** releases the cursor.

## VR theory

**Optic flow** is the pattern of motion across the retina as you move through a scene: objects stream outward from the point you are heading toward, faster the closer they are. **Vection** is the illusion of self-motion produced by optic flow alone — the feeling that *you* are moving when only the picture is. In real life vection is confirmed by the inner ear; in smooth VR locomotion it is not, and the mismatch is the **vestibular–visual conflict** introduced in 5.1. Smooth locomotion is the technique that lives entirely inside that conflict, so everything you tune today is about making the conflict smaller. Braun and Rizzo's Chapter 3 shows how the continuous move and turn providers are wired on the XRI rig; today is about the numbers on those components.

**Acceleration is the trigger, not speed.** The vestibular system senses *changes* in velocity, not velocity itself — sitting in a train at a constant 200 km/h feels like sitting still. So a constant 3 m/s glide is far more tolerable than repeatedly starting and stopping at 1.5 m/s. This has two consequences for design: ramp the speed in and out so the visual acceleration is gentle, and avoid designs that force the player to start-stop constantly (tight corners, narrow doorways, fiddly pickups mid-walk). The S-curve path in the starter scene deliberately makes you turn while moving so you can feel the difference between a snap and a smooth turn under way.

**Speed.** Real walking is about 1.4 m/s; most comfortable VR defaults sit at 1.5–2 m/s, with 3–4 m/s as a "sprint" that experienced players enjoy and new players do not. Faster speed means faster optic flow, especially from near objects — which is why the trees beside the path matter more than the ones at the edge of the clearing.

**Snap versus smooth turn.** A **snap turn** rotates the view by a fixed angle instantly. There is no rotational optic flow at all, so there is no vection, which makes it the comfortable default. Its cost is disorientation: after a 90° jump the brain has to re-find landmarks, and small angles (30°) mean many snaps to turn around. A **smooth turn** rotates continuously at some degrees per second; it feels natural and keeps landmarks in view, but rotational vection is the *most* provocative kind of visual motion, more than forward travel. If a player can tolerate only one thing, make it forward smooth movement with snap turning — that combination is the industry default for a reason. The click you add today gives each snap a sound, which makes the discontinuity read as a deliberate action rather than a glitch.

**Head-relative versus hand-relative.** The move provider needs to know which way "forward on the stick" points. **Head-relative** uses the camera's yaw: forward is where you look. It is intuitive for beginners and a problem the moment you want to look at something beside the path — your travel direction veers with your gaze. **Hand-relative** uses the controller's yaw: forward is where the hand points, and you may look anywhere. It takes a minute to learn and then most players prefer it, because it separates *looking* from *going* the way real walking does. XRI exposes this as the provider's *Forward Source* transform, and your toggle swaps it live.

**Feedback for discontinuities.** Every time the view changes in a way the eyes cannot predict — a snap, a blink, a dash — a matching sound (and on the headset, a small haptic pulse) turns "something happened to me" into "I did something". That is why `SnapTurnFeedback` exists, and why in 5.3 the dash will have a vignette.

## Math foundation

**Ease-in with SmoothStep.** During the ramp the provider's speed is

$$v(t) = v_{\max} \cdot S\!\left(\frac{t}{t_{\text{ramp}}}\right), \qquad S(x) = 3x^2 - 2x^3 \text{ for } x \in [0,1]$$

`Mathf.SmoothStep(0, 1, x)` computes $S(x)$. Its derivative $S'(x) = 6x - 6x^2$ is zero at $x = 0$ and $x = 1$: the speed starts with *no* acceleration and arrives at full speed with none, which is what the inner ear wants. A plain `Lerp` has $S(x) = x$ with a constant acceleration that switches on instantly at $t = 0$ — a small but noticeable kick.

Worked example: $v_{\max} = 2$ m/s, $t_{\text{ramp}} = 0.3$ s. At $t = 0.1$ s, $x = 0.333$, $S = 3(0.111) - 2(0.037) = 0.259$, so $v = 0.52$ m/s. At $t = 0.15$ s, $x = 0.5$, $S = 0.5$, $v = 1.0$ m/s. At $t = 0.3$ s, $v = 2.0$ m/s. Average speed during the ramp is exactly $v_{\max}/2$ (SmoothStep is symmetric), so a 0.3 s ramp costs you 0.3 m of travel compared with an instant start — nobody notices.

**Angular velocity thresholds.** A smooth turn at $\omega$ °/s completes a full circle in $360/\omega$ s: 60°/s takes 6 s, 180°/s takes 2 s. Rotational vection becomes strongly provocative for many people above roughly 60–90°/s; the slider goes to 180 so you can feel where your own limit is. For snap turns, $n$ snaps of angle $\alpha$ cover $n\alpha$ degrees; with $\alpha = 45°$ eight snaps make a full turn, and `SnapTurnFeedback`'s readout lets you verify $\text{heading}_{\text{after}} = (\text{heading}_{\text{before}} + n\alpha) \bmod 360$.

**Forward from yaw only.** A forward vector $\vec f$ taken from a tilted head has a vertical component; you do not want to fly into the ground when you look down. The move provider projects the source's forward onto the ground plane: $\vec f_{xz} = (f_x, 0, f_z) / |(f_x, 0, f_z)|$. That is the same "drop y and normalize" step you used for horizontal distance in 1.1.

**Measured speed.** `SpeedRamp` also computes ground speed from the rig's own displacement: $v = |\Delta \vec p_{xz}| / \Delta t$. When it matches `moveSpeed` you know the provider is doing what the slider says; in 5.3 the same readout will tell you how fast your dash really is.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_5_2.unity` with:

- **Floor** — a 50 m grass plane, static.
- **XR Origin (XR Rig)** at the origin with the **XR Interaction Simulator**. The rig root carries `SpeedRamp` (with *Move Provider* pre-assigned), an `AudioSource`, and `SnapTurnFeedback` (with *Snap Turn Provider* and *Head* pre-assigned).
- **Path** — sixteen flat slabs following an S-curve away from you, with three glowing **Arches** to steer through (they have colliders, so cutting corners bumps you).
- **Trees** — a wide ring for enclosure plus four trunks right beside the path for strong optic flow.
- **Mysterious Guide** at the end of the path, 26 m out.
- **Tuner Panel** — a world-space Canvas (700 × 420 px at 0.001 scale = 0.7 × 0.42 m) 1.6 m ahead at 1.35 m height with a `TrackedDeviceGraphicRaycaster`, three sliders, one toggle, and value labels. It carries `MoveTuner` with every provider, slider, text, and forward transform pre-assigned (*Hand Forward* is the **Left Controller**, or the camera if none was found).
- **EventSystem** with `XRUIInputModule` so the controller ray can drive the UI.
- **Speed Readout** and **Turn Readout** — two floating TextMeshes beside the panel that `SpeedRamp` and `SnapTurnFeedback` write to.

Scripts live in `Assets/Scripts/`.

## Your tasks (about 70 min)

**Task 1 (15 min) — Sliders talk to providers** · file: `Scripts/MoveTuner.cs`, TODO 1–2 and 4–6
Find the providers if any field is empty (`FindFirstObjectByType<XROrigin>()` then `GetComponentInChildren<...>(true)`). Initialize each slider *from* its provider so the panel shows the truth, then `AddListener` the three `On...Changed` methods and call each once. In the handlers, write `moveProvider.moveSpeed` (or `speedRamp.targetSpeed` when the ramp exists), `snapTurnProvider.turnAmount = SnapAngles[index]`, and `smoothTurnProvider.turnSpeed`, and update the value labels.
**Check:** at Play the labels show the rig's real defaults. Drag the speed slider with the controller ray (or wait for Task 2's keys) to 4 m/s — you cross the clearing in about five seconds; at 1 m/s it is a slow walk. Set snap to 90° — four snaps bring you back to your starting heading.

**Task 2 (5 min) — Keyboard nudges** · file: `Scripts/MoveTuner.cs`, TODO 3
Read `Keyboard.current[key].wasPressedThisFrame` for the four keys and change the slider *values* (not the providers directly) so `onValueChanged` does the rest. If the simulator already uses one of the keys on your machine, change the `Key` field in the Inspector.
**Check:** **-** and **=** step the speed label by 0.5; **[** cycles 30 → 45 → 90 → 30.

**Task 3 (10 min) — Head-relative vs hand-relative** · file: `Scripts/MoveTuner.cs`, TODO 7
Set `moveProvider.forwardSource` to `handForward` when the toggle is on and `headForward` when it is off. Then, on the Starter Assets rig, smooth turn is a per-hand option: select **Left Controller** and **Right Controller** under the rig, find the *Controller Input Action Manager* component, and tick **Smooth Turn** on the right hand so your turn-speed slider has something to drive (put it back to snap afterwards if you prefer).
**Check:** toggle on, push the stick forward and turn your head with the mouse — you keep walking the same line. Toggle off — your path bends with your gaze. With smooth turn enabled, 180°/s spins you around in two seconds.

**Task 4 (20 min) — The speed ramp** · file: `Scripts/SpeedRamp.cs`, TODO 1–5
Subscribe to the move provider's `locomotionStarted` / `locomotionEnded` (unsubscribe in `OnDestroy`). On start, record `moveStartTime` unless already moving; on end, clear the flag and restore `moveSpeed = targetSpeed`. In `Update`, measure ground speed from the rig's displacement, and while moving set `moveSpeed = targetSpeed * Mathf.SmoothStep(0, 1, elapsed / rampSeconds)`. Set *Ramp Seconds* to 2 once to see the glide clearly, then back to 0.3.
**Check:** the **Speed Readout** shows *provider* climbing from 0 to the target over the ramp and *measured* following it; releasing the stick restores *provider* to the target. With a 2 s ramp you clearly feel the ease-in; at 0.3 s it feels responsive but no longer kicks.

**Task 5 (15 min) — Click on snap, then record** · file: `Scripts/SnapTurnFeedback.cs`, TODO 1–4
Fill the click's sample array with a sine at `clickFrequency` under a squared linear envelope and create the `AudioClip`. Subscribe `OnSnapTurn` to `snapTurnProvider.locomotionStarted`, count and `PlayOneShot` the click, and show the count plus `head.eulerAngles.y` on the **Turn Readout**. Record your screencast: walk the S-curve with the ramp on and off (set *Ramp Seconds* to 0 for the "off" pass), show the sliders changing speed and snap angle, and do a lap with hand-relative on. In your bullets, write which default speed, turn type, and forward source *your* project will ship with, and one sentence on why.
**Check:** each snap ticks and increments the count; eight 45° snaps return the heading to its starting value.

## Stretch goals

- Add an ease-*out*: when the stick returns to center, the provider stops instantly and there is nothing to ramp. Explain in a bullet why that is (the provider only moves while there is input) and sketch how 5.3's direct-transform approach could keep gliding for 0.2 s.
- Give the snap turn a **haptic** pulse: find a `HapticImpulsePlayer` under the right controller (`GetComponentInChildren<HapticImpulsePlayer>()`, null-checked) and call `SendHapticImpulse(0.3f, 0.05f)` in `OnSnapTurn`. Verify on the headset.
- Log every slider change with a timestamp to the Console (`Time.time`, parameter, value) — the beginning of the settings log you will keep for comfort testing in 5.4.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. On the headset the left thumbstick moves and the right thumbstick turns exactly as in the simulator, and the panel is driven by pointing the controller ray at a slider and holding the trigger. The keyboard nudges do nothing there, which is fine — the panel is the real interface. Try the speed slider at 4 m/s standing up and sitting down; most people tolerate more speed seated, which is a useful note for 5.4's comfort defaults.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the sliders changing speed and snap angle with the labels updating, the Speed Readout climbing during a ramp, a snap turn with its click and count, and a lap of the path in hand-relative mode.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus the default speed / turn type / forward source your project will ship with and why.

## Troubleshooting

| Symptom | Fix |
|---|---|
| **Activity** menu is missing from the menu bar | A script has a compile error; open the Console, fix the red line, wait for the spinner at bottom-right to finish |
| Pushing the thumbstick does nothing | **Tab** to the *left controller* for move, *right controller* for turn; check the simulator panel for the *Primary 2D Axis* key |
| The controller ray will not grab a slider | The panel must have `TrackedDeviceGraphicRaycaster` (not `GraphicRaycaster`) and the scene an **EventSystem** with `XRUIInputModule`; rebuild the scene if you deleted either |
| Labels read `-- m/s` after Play starts | TODO 2 never called the `On...Changed` methods once at startup, or the provider fields are null — check the Console for a null reference |
| Speed slider moves but movement speed does not change | `SpeedRamp` owns `moveSpeed` while moving; make sure `OnSpeedChanged` writes `speedRamp.targetSpeed` when the ramp is assigned |
| Smooth turn slider has no effect | Smooth turn is off by default on the Starter Assets rig; enable it in the controller's *Controller Input Action Manager* (Task 3) |
| No click on snap turn | `MakeClick` still returns silence (TODO 1), or the `AudioSource` is muted / the Game view has audio muted (speaker icon in the Game view toolbar) |

Created by Isac Artzi

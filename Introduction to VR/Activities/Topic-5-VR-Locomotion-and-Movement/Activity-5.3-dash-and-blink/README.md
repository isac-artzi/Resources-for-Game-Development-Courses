# Activity 5.3 — Hybrid: Dash and Blink

Topic 5: VR Locomotion and Movement · Week 10, Tuesday · Stepping stone to Milestone 5 — Movement Mechanics

## Where this fits

Milestone 5 asks for a *hybrid* locomotion mode alongside teleport and smooth. Hybrids live in the gap between the two: they move you continuously, but so briefly that the inner ear never gets a chance to object. Today you build the two classic ones. A **dash** slides the rig 3 m in the direction you point over 0.2 s while a tunnel vignette closes in around the edges of your view. A **blink** fades to black in 50 ms, hops you 2 m, and fades back in — a teleport so short it feels like a step. Both clamp their distance with a `SphereCast` so a pillar stops you instead of letting you through. Both move the `XROrigin` transform directly, which is the right call for a prototype, and the theory section explains what the XR Interaction Toolkit's `LocomotionProvider` route adds and when you would graduate to it. Welcome back from spring break; this is the last piece of movement code before you turn the whole set into player-facing comfort settings on Thursday.

*Elaria hook:* the Ancient Ruins are a colonnade of cracked pillars and a doorway half-buried in rubble. The Guide is already on the other side. "Quick," the Guide says, "but never careless." A dash between pillars; a blink through the doorway.

## Learning goals

- You can explain why continuous motion shorter than roughly 250 ms is tolerated by most players when the same motion sustained for seconds is not.
- You can move an `XROrigin` over time with a coroutine, a `SmoothStep` easing, and a collision-clamped distance from `Physics.SphereCast`.
- You can generate a radial alpha texture in code and use it as a tunneling vignette whose strength eases in and out.
- You can sequence a fade-move-fade with nested coroutines so the world never visibly jumps.
- You can state the trade-offs between moving the rig's transform directly and implementing a custom XRI `LocomotionProvider`.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-5.3-dash-and-blink`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → import **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup**: both lines `OK`.
6. **Activity → Build Starter Scene**. `Activity_5_3` opens: a stone courtyard, two rows of pillars ahead with a fallen one across the path, rubble beyond, and a wall with a narrow doorway. The Guide glows past the door.
7. Press **Play**. Press **F**: nothing visible yet — but check the Console and the rig's `DashLocomotion` component: *Dash Count* went up and you hopped instantly (the placeholder). Press **B**: same for the blink. Your job is to make those hops comfortable.

Simulator controls that matter today: **Tab** to select the *head*, then **mouse** to turn — on desktop the dash and blink follow the **camera's** yaw (the builder falls back to the camera when it cannot find a right controller; on the headset the right hand aims). **F** dashes, **B** blinks. **W A S D** still move the selected device if you need to reposition. The rig's stock teleport (right-controller thumbstick forward) also works on the floor so you can compare all three. **Esc** releases the cursor.

## VR theory

**Hybrid locomotion** covers techniques that neither move you continuously for as long as you hold a stick nor jump you instantly, but do something in between: a short dash, a blink step, a "grab the world and pull" motion. They exist because the two pure techniques each fail someone — teleport breaks the sense of traversal for players who want to *move*, and smooth locomotion makes a third of new players ill. A dash keeps the feeling of covering ground; a blink keeps the gentleness of teleport while preserving the sense of stepping rather than jumping. Braun and Rizzo describe XRI's stock teleport and continuous providers in Chapter 3; hybrids are what you build when the stock components do not fit your game.

**Why short durations feel fine.** The vestibular system integrates acceleration over time before it signals "we are moving"; below roughly 200–300 ms a visual motion ends before the conflict is fully registered. That is why a 0.2 s dash is comfortable for many people who cannot tolerate 5 s of gliding at the same speed — the *speed* of the dash (15 m/s!) is irrelevant, the *duration* is what matters. Two design rules follow. First, keep dashes short and do not let them chain into continuous motion: a cooldown between dashes prevents players from holding the button and turning your dash into the worst possible smooth locomotion. Second, ease the dash with SmoothStep so its acceleration is gentle at both ends — the same argument as the speed ramp in 5.2, compressed into a fifth of a second.

**Tunneling / vignette.** Motion sickness is driven mostly by optic flow in the *peripheral* vision, which is tuned for motion detection. A **vignette** darkens the periphery during motion so the flow there disappears while the central view — where you are looking — stays clear. It is one of the most effective comfort tools known, cheap to render, and it should ease in and out over 100–200 ms rather than pop, because the vignette itself popping is a visual event. Yours is a quad in front of the camera with a radial alpha texture you generate in code: clear inside `innerRadius`, black beyond `outerRadius`. The simulator's 60° camera shows only the middle of the quad, so on desktop you see the vignette as darkened corners and edges; on the headset the ring closes in properly.

**Expectation management.** A blink is comfortable partly because the player *expects* the step: they pressed the button, the screen went dark, and they know they moved forward by about a stride. Keep the distance fixed and the direction obvious (where the hand or head points), and the brain treats each blink as an intentional act. Randomize the distance or let the direction drift and the same blink becomes disorienting. The same goes for the dash: fixed distance, fixed duration, always the same vignette.

**Moving the rig directly versus an XRI LocomotionProvider.** The `XROrigin` is a GameObject; moving its transform moves the player. That is what your dash and blink do, and for a prototype it is the right call: it is ten lines, it cannot break, and the stock teleport and smooth providers keep working alongside it. What you give up is coordination. XRI's providers do not move the rig themselves — they *request* a move through the `LocomotionMediator` on the rig, which arbitrates when two providers want to move at once (a snap turn during a dash, gravity during a teleport) and applies the result through an `XRBodyTransformer`. A custom provider subclasses `LocomotionProvider`, calls `TryStartLocomotionImmediately()`, queues a body transformation each frame (in 3.x, `TryQueueTransformation(new XROriginMovement { motion = delta })`), and calls `TryEndLocomotion()` when done; in return, other providers pause politely and the `locomotionStarted`/`locomotionEnded` events you used in 5.1 and 5.2 fire for *your* move too. You would switch to that route when your dash must coexist with snap turning mid-flight, when a vignette or fade system listens to provider events and should react to dashes as well, or when you ship. Until then, the transform is fine — and one more practical point: the Starter Assets rig has a `CharacterController`; if you set the transform while it is enabled, the controller can wake up "inside" a wall for a frame, so `DashLocomotion.MoveRigTo` goes through `CharacterController.Move` when it finds one.

## Math foundation

**Eased slide.** Over a dash of duration $T$ the rig's position is

$$p(t) = \text{Lerp}\big(p_0,\; p_1,\; S(t/T)\big), \qquad S(x) = 3x^2 - 2x^3$$

with $S$ from `Mathf.SmoothStep(0, 1, x)`. Because $S'(0) = S'(1) = 0$, the rig starts and stops with zero acceleration. Worked example: $p_0 = (0,0,0)$, $p_1 = (0,0,3)$, $T = 0.2$ s. At $t = 0.05$ s, $x = 0.25$, $S = 3(0.0625) - 2(0.0156) = 0.156$, so $z = 0.47$ m. At $t = 0.1$ s, $x = 0.5$, $S = 0.5$, $z = 1.5$ m. At $t = 0.15$ s, $z = 2.53$ m. The peak speed is at the middle: $v_{\max} = S'(0.5) \cdot d/T = 1.5 \cdot 3/0.2 = 22.5$ m/s. Ordinary optic flow at that speed for five seconds would be brutal; for 50 ms around the midpoint it passes unnoticed.

**Collision clamp.** A `SphereCast` sweeps a sphere of radius $r$ from a start point along direction $\hat d$ for at most distance $L$. If it hits at `hit.distance` $= h$ (the distance the *sphere center* traveled before contact), the safe travel distance is

$$L_{\text{safe}} = \max(0,\; h - m)$$

where $m$ is a small wall margin. The radius is already accounted for in $h$: a sphere of radius 0.3 m aimed at a wall 2.0 m away reports $h = 1.7$ m, and with $m = 0.1$ you stop 1.6 m in, leaving 0.4 m between your chest and the wall. Cast from chest height ($1.0$ m above the rig's floor) so the floor itself is never hit; use a sphere rather than a ray because a ray fits through a gap a body does not.

**Flattened direction.** From the aim source's forward $\vec f$: $\vec d = (f_x, 0, f_z) / |(f_x, 0, f_z)|$. If the hand points nearly straight down the flattened vector is near zero; guard with `sqrMagnitude < 0.0001` and do nothing.

**Blink timing.** Fade out $50$ ms + hold $50$ ms + fade in $100$ ms $= 200$ ms total, about one real eye blink. The move happens at the boundary between fade-out and hold, when alpha is exactly 1. A common bug is to move *after* the fade-in starts; then the player sees the world jump behind a half-transparent black — worse than no fade.

**Radial mask.** For a texel at normalized coordinates $(u, v)$, the radius from the center is $r = 2\,|(u - 0.5,\; v - 0.5)|$, so $r = 1$ at the middle of each edge and $\sqrt 2$ at the corners. Alpha is $\text{SmoothStep}(0, 1, \text{InverseLerp}(r_{\text{in}}, r_{\text{out}}, r))$: 0 inside $r_{\text{in}}$, 1 beyond $r_{\text{out}}$, soft between. With $r_{\text{in}} = 0.3$ and $r_{\text{out}} = 0.8$, a texel at $r = 0.55$ has `InverseLerp` $= 0.5$ and alpha $0.5$.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_5_3.unity` with:

- **Floor** — a 40 m flagstone plane with a `TeleportationArea`, so the stock teleport still works for comparison.
- **XR Origin (XR Rig)** at the origin with the **XR Interaction Simulator**. The rig root carries `DashLocomotion` (*Origin*, *Aim Source* = Right Controller or camera, *Vignette* pre-assigned) and `BlinkStep` (*Dash* and *Vignette* pre-assigned).
- **Ruins** — two rows of six pillars 3 m apart, a **Fallen Pillar** lying across the path at 10.5 m ("Dash at me"), four rubble blocks, and an end wall at 19 m with a 1.2 m doorway. All static, all with colliders.
- **Mysterious Guide** beyond the doorway at 23 m.
- **Comfort Overlays** — an empty under the **Main Camera** carrying `VignetteController`, with two children: **Vignette Quad** (0.7 m wide, 0.2 m ahead — about 120° of view) and **Blink Quad** (4 m wide, 0.25 m ahead). Both use transparent unlit materials starting at alpha 0, colliders removed, shadows off. The camera's near clip is lowered to 0.05 m.
- Signs for the controls, the fallen pillar, and the doorway; warm low sun and dusty fog.

Scripts live in `Assets/Scripts/`.

## Your tasks (about 70 min)

**Task 1 (10 min) — Direction and clamp** · file: `Scripts/DashLocomotion.cs`, TODO 2–3
Implement `FlatForward()` (drop y, normalize, zero if degenerate) and `ClampDistance()` with `Physics.SphereCast` from `origin.transform.position + up * castHeight`, returning `hit.distance - wallMargin` on a hit and the requested distance otherwise. Use `QueryTriggerInteraction.Ignore` so trigger volumes never block you.
**Check:** temporarily add `Debug.Log(ClampDistance(FlatForward(), dashDistance))` in `Update` (remove it afterwards): facing open floor it prints 3; facing the fallen pillar from 2 m it prints about 1.6.

**Task 2 (15 min) — The dash coroutine** · file: `Scripts/DashLocomotion.cs`, TODO 1, 4–5
In `Update`, on a press with no dash running and the cooldown elapsed, compute direction and clamped distance and start `Dash`. In `Dash`, set `vignette.targetVignette = vignetteStrength`, slide from start to end with `Vector3.Lerp(start, end, Mathf.SmoothStep(0, 1, t / dashSeconds))` each frame through `MoveRigTo`, land exactly on `endPos`, then set the target vignette back to 0 and clear `running`. Implement `MoveRigTo` to use `CharacterController.Move` when the rig has one.
**Check:** **F** slides you 3 m over 0.2 s with a soft start and stop. Dash straight at the fallen pillar: you stop about 0.4 m short and never clip through. Hold **F**: dashes come no faster than one per 0.4 s.

**Task 3 (15 min) — The vignette** · file: `Scripts/VignetteController.cs`, TODO 1–2
Fill the radial texture: clear inside `innerRadius`, black beyond `outerRadius`, `SmoothStep` between, using `InverseLerp` on the radius. In `Update`, ease `currentVignette` toward `targetVignette` with `Mathf.MoveTowards` and apply it. Then tune: set *Target Vignette* to 1 in the Inspector during Play and adjust *Inner Radius* / *Outer Radius* (rebuild the texture by re-entering Play) until the desktop view shows clearly darkened edges with a clear center.
**Check:** during a dash the edges of the view darken and clear again within about 0.2 s after landing; the material preview on Vignette Quad shows a soft black ring.

**Task 4 (15 min) — The blink** · file: `Scripts/VignetteController.cs`, TODO 3–4, then `Scripts/BlinkStep.cs`, TODO 1–4
Implement `SetBlackout` and the `FadeBlackout` coroutine (Lerp alpha over the duration, finish exactly at the target). In `BlinkStep`, on a press: `yield return vignette.FadeBlackout(0, 1, fadeOutSeconds)`, then move through `dash.MoveRigTo`, wait `holdSeconds`, then `yield return vignette.FadeBlackout(1, 0, fadeInSeconds)`. Pause Play immediately after pressing **B** once to confirm the screen is black *before* the position changes.
**Check:** **B** blinks you 2 m forward in about 0.2 s and you never see the world move. Blink toward the doorway: the clamp stops you at the wall unless you are lined up with the opening; through the gap, you emerge next to the Guide.

**Task 5 (15 min) — Compare and record** · no new code
Travel from the start to the Guide three times: once with the stock teleport, once with dashes only, once with blinks only. Then set *Dash Seconds* to 1.0 and *Vignette Strength* to 0 and dash once — that is what a hybrid feels like when it is *not* short and *not* tunneled. Restore 0.2 and 0.8. Record your screencast during this task, showing a dash into the fallen pillar (clamped), a dash with the vignette, a blink through the doorway, and the 1.0 s dash for contrast. In your bullets, say which of the three modes your project will offer as its default and which as an option.

## Stretch goals

- Turn the dash into a real XRI provider: create `DashProvider : LocomotionProvider`, call `TryStartLocomotionImmediately()` at the start, queue an `XROriginMovement` with the frame's delta each frame, and `TryEndLocomotion()` at the end. Then subscribe 5.1's `TeleportFade` logic to *its* events. Verify the exact method names against the XRI 3.5 scripting reference — this API moved between XRI 2 and 3.
- Bind the dash to a controller button: create an Input Action (right-hand *primaryButton*), assign it to *Dash Action*, and test on the headset. The keyboard path keeps working on desktop.
- Add a **step counter** label near the door showing dashes and blinks used to reach the Guide — the seed of the telemetry you will collect in Topic 7.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. On the headset the *Aim Source* is your right controller, so dashes go where your hand points, not where you look — try both by temporarily assigning the camera. Assign `InputActionReference`s for *Dash Action* and *Blink Action* (the Starter Assets input actions asset has right-hand primary and secondary button actions you can reuse). The vignette quad shows its full ring on the headset; if you can see its square edge at the far periphery, increase the quad's scale on **Vignette Quad** from 0.7 to 0.9.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: a dash with the vignette closing in, a dash clamped by the fallen pillar, a blink through the doorway with the fade, and the slow untunneled dash for contrast.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus which locomotion mode your project will default to and which it offers as an option.

## Troubleshooting

| Symptom | Fix |
|---|---|
| **Activity** menu is missing from the menu bar | A script has a compile error; open the Console, fix the red line, wait for the spinner at bottom-right to finish |
| F / B do nothing | The Game view must have focus (click it once); `Keyboard.current` is null until the Input System sees a keyboard. Check the Console for a null `origin` |
| I dash straight through pillars | `ClampDistance` still returns `requested`, or `obstacleMask` excludes the Default layer; also make sure the cast starts at chest height, not at the floor |
| Every dash returns distance 0 | The SphereCast starts inside a collider — usually the rig's own `CharacterController`. Increase `castHeight` slightly, or put the rig on a layer excluded from `obstacleMask` |
| The vignette never appears | TODO 1 still writes transparent pixels, or `targetVignette` is never set — watch *Current Vignette* in the Inspector during a dash |
| The whole view goes black during a dash | The vignette quad's material has no texture (the ring mask was not assigned) so alpha applies everywhere; check that `Awake` sets `mainTexture` |
| Blink shows the world jumping | You moved after the fade-in began; the move must happen after `FadeBlackout(0,1,...)` completes and before `FadeBlackout(1,0,...)` starts |

Created by Isac Artzi

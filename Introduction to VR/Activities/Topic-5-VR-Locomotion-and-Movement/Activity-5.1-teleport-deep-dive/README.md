# Activity 5.1 — Teleport, Properly

Topic 5: VR Locomotion and Movement · Week 9, Tuesday · Stepping stone to Milestone 5 — Movement Mechanics

## Where this fits

Milestone 5 asks for three ways to move through Elaria — teleport, smooth, and a hybrid — plus evidence that you tested them for comfort. Teleport is the one every VR player already trusts, so it is where we start, and we go deeper than "drag the TeleportationArea component onto the floor". By the end of class the teleport in your project will *blink*: the view cuts to black the instant you move and fades back in at the destination. You will have drawn your own projectile arc so you understand what the XR Interaction Toolkit's arc is doing, and you will have a validator that refuses landing spots that are too steep or too close to a wall — the two mistakes that make teleport feel broken in student projects. You will also have used teleport *anchors* to decide which way the player faces when they arrive, which is a storytelling tool as much as a locomotion one.

*Elaria hook:* the Mountain Pass is narrow, and the Guide is already on the far side. "Do not run," the Guide says. "Choose where you will stand, then be there." Each blink is a step chosen on purpose.

## Learning goals

- You can explain why an instant cut between two viewpoints is uncomfortable and why a 100–200 ms blink to black is not.
- You can subscribe to and unsubscribe from the `TeleportationProvider`'s `locomotionStarted` and `locomotionEnded` C# events and drive a visual from them.
- You can sample a projectile curve $p(t) = p_0 + v t + \tfrac{1}{2} g t^2$ into a `LineRenderer` and cut it where it first hits geometry.
- You can compute a surface's slope from its normal with a dot product and reject landing spots by slope and by body clearance.
- You can explain the difference between a `TeleportationArea` and a `TeleportationAnchor`, and use `matchOrientation` to control the player's facing on arrival.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-5.1-teleport-deep-dive`). Open it with Unity **6000.5.x** and wait for packages to finish importing.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** tab → import **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup**: both lines should read `OK`.
6. **Activity → Build Starter Scene**. `Activity_5_1` opens: a grey ledge in mountain mist, a slanted stone ramp on your left, a broken wall with a gap on your right, three glowing anchor platforms, and the Guide far ahead.
7. Press **Play**. Press **Tab** until the *right controller* is the selected device. The simulator's on-screen panel names the key bound to the controller's thumbstick (*Primary 2D Axis*) — push it **forward** to start aiming the rig's built-in teleport ray, move the mouse to aim, and release to teleport. Do this two or three times before writing any code so you know what the stock behaviour feels like: an instant cut, no fade.

Simulator controls that matter today: **Tab** (cycle head / left controller / right controller), **mouse** (rotate the selected device — this is how you aim), **W A S D** (move the selected device), the **thumbstick-forward** key on the right controller (teleport aim → release to go), **P** (toggles your own arc preview once you have written it), **Esc** (release the cursor). The arc preview launches from the right controller; if the builder could not find one, it falls back to the head, so aiming with the mouse still works.

## VR theory

**Locomotion** is any technique that moves the player's viewpoint through a virtual space larger than their physical room. The problem every technique must solve is the **vestibular–visual conflict**: your eyes report motion, your inner ear reports none, and the mismatch is what makes some players nauseous within minutes. Teleport sidesteps the conflict entirely by removing the motion. There is no optic flow to disagree with, only a change of place. Braun and Rizzo walk through setting up XRI's teleportation in Chapter 3 of *XR Development with Unity*; today assumes you have seen that setup and asks *why* it is built the way it is.

**Why blink.** An instant cut from one viewpoint to another is comfortable for the stomach but jarring for the brain: for a frame or two, nothing in the visual field matches the previous frame, and players report a "jolt" and a moment of disorientation about where they are. A short fade to black — roughly 100–200 ms, about the length of a real eye blink — gives the visual system a clean break: the old view ends, the new view begins, and nothing has to be reconciled. Shorter than 80 ms and the black is barely noticed; longer than 300 ms and the world feels like it is loading. The fade you build cuts to black on the same frame the rig moves and fades back in over 150 ms. That is the version most shipped VR games use.

**Target selection: arc versus straight ray.** A straight ray from the hand is precise at short range but hopeless for picking a spot on the floor ten meters away — you would have to point almost parallel to the ground, and a tiny wrist tremor moves the landing point by meters. A **projectile arc** solves this: you aim slightly upward, the arc falls under gravity, and the landing point moves smoothly with your wrist angle. Far spots are reached by raising the hand; near spots by lowering it. XRI's ray interactor offers this as its *Projectile Curve* line type with *Velocity* and *Acceleration* settings; the math is exactly the formula in the next section, and the arc you write today is the same thing with the numbers exposed.

**Orientation on arrival.** After a teleport, which way does the player face? `MatchOrientation.WorldSpaceUp` keeps the player's current heading — natural for open ground. `TargetUpAndForward` snaps the player's forward to the anchor's forward, which lets *you* decide what they see first: the Guide, the door, the puzzle. Used well this is a camera cut in a film; used badly it wrenches the player around and they lose their sense of direction. The rule of thumb: force orientation only at anchors that exist to reveal something, never on a general-purpose floor area.

**Areas versus anchors.** A `TeleportationArea` lets the player land anywhere on a collider — freedom, but also every problem spot in the collider is fair game. A `TeleportationAnchor` teleports to one fixed point (its `teleportAnchorTransform`), which is safer and also a *design* tool: a ring of anchors is a path, and a lone anchor on a ledge is an invitation. Most finished VR games mix both: areas for open floors, anchors for stairs, ledges, seats, and story beats.

**Validation.** XRI will happily teleport you onto a 30° ramp or into the 10 cm between a wall and a pillar, because a collider is a collider. A good teleport rejects those spots *before* the player commits — your arc turns red and the label says why. Two tests cover most cases: the surface must be near-horizontal, and an imaginary body cylinder standing on the spot must not intersect anything.

## Math foundation

**Projectile curve.** A point launched from $p_0$ with initial velocity $\vec v$ under constant acceleration $\vec g$ is at

$$p(t) = p_0 + \vec v\, t + \tfrac{1}{2}\, \vec g\, t^2$$

with $\vec g = (0, -9.81, 0)$ m/s² for real gravity (the preview lets you use a stronger pull like $-15$ for a tighter arc). You draw the curve by sampling $t = 0, \Delta t, 2\Delta t, \ldots$ for $N$ samples. With $N = 30$ and $\Delta t = 0.05$ s the arc covers 1.5 s of flight.

Worked example: the hand is at $p_0 = (0, 1.2, 0)$ and points 20° above horizontal along $+z$ at 8 m/s. Then $\vec v = (0,\; 8 \sin 20°,\; 8 \cos 20°) = (0, 2.74, 7.52)$. At $t = 0.5$ s:

$$p = (0, 1.2, 0) + (0, 1.37, 3.76) + \tfrac{1}{2}(0, -9.81, 0)(0.25) = (0,\; 1.2 + 1.37 - 1.23,\; 3.76) = (0,\; 1.34,\; 3.76)$$

At $t = 0.8$ s the height is $1.2 + 2.19 - 3.14 = 0.25$ m, and shortly after $t = 0.84$ s it reaches the floor about 6.3 m out. Raising the hand to 35° lands you at roughly 7.7 m; the landing point moves smoothly with wrist angle, which is the whole point of an arc.

**Cutting the arc.** Between consecutive samples $p_{i-1}$ and $p_i$ you cast a ray of length $|p_i - p_{i-1}|$ in direction $(p_i - p_{i-1})/|p_i - p_{i-1}|$. The first segment that hits ends the curve at `hit.point`. One long ray from $p_0$ would miss the floor entirely because the curve bends.

**Slope from a normal.** The angle between a unit surface normal $\hat n$ and straight up is

$$\theta = \arccos(\hat n \cdot \hat y)$$

For the flat floor $\hat n = (0,1,0)$, $\theta = \arccos 1 = 0°$. For the ramp, rotated 30° about the x-axis, $\hat n = (0, \cos 30°, \sin 30°) = (0, 0.866, 0.5)$ and $\theta = \arccos 0.866 = 30°$. `Mathf.Acos` returns radians — multiply by `Mathf.Rad2Deg`. Clamp the dot product to $[-1, 1]$ first; floating-point noise can produce 1.0000001 and $\arccos$ of that is NaN.

**Body clearance.** A sphere of radius $r = 0.45$ m centered $h = 1.0$ m above the landing point approximates the player's torso. `Physics.CheckSphere` returns true if any collider overlaps it. The floor cannot overlap because it is $h - r = 0.55$ m below the sphere's bottom; walls and pillars within 0.45 m of the spot do.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_5_1.unity` with:

- **Floor** — a 40 m rock plane with a `TeleportationArea` (`matchOrientation = WorldSpaceUp`), marked static.
- **XR Origin (XR Rig)** at the origin with the **XR Interaction Simulator**.
- **Stone Ramp** — a 3 × 5 m slab tilted 30°, *also* a `TeleportationArea`, so you can see that XRI accepts it and your validator should not.
- **Broken Wall** — two wall segments 2 m tall with a 0.7 m gap between them, and a pillar farther along. Landing next to them fails the clearance test.
- **Anchor Platform 1–3** — glowing stone discs with a `TeleportationAnchor` (`matchOrientation = TargetUpAndForward`) and an **Anchor Point** child whose forward already points at the Guide.
- **Mysterious Guide** — the 2 m capsule at the far end, 14 m out, with a label.
- **Teleport Fade** — a 4 × 4 m black quad 0.2 m in front of the **Main Camera**, alpha 0, no collider. It carries `TeleportFade` with *Fade Panel* and *Provider* pre-assigned. (The builder also lowers the camera's near clip plane to 0.05 m so the quad is never clipped.)
- **Arc Preview** — an empty with a `LineRenderer` (2 cm wide, vertex-colored material), `LandingValidator`, and `TeleportArcPreview`. *Aim Source* is the rig's **Right Controller** (or the camera if none was found) and *Status Label* is the child `Landing Status` TextMesh.
- Signs marking the ramp angle and the wall gap, and light mountain mist.

Scripts live in `Assets/Scripts/`. Open them via **Assets → Open C# Project**.

## Your tasks (about 70 min)

**Task 1 (15 min) — Hook the fade to the provider** · file: `Scripts/TeleportFade.cs`, TODO 1–3
In `Start()`, find the `TeleportationProvider` if the field is empty (`Object.FindFirstObjectByType<TeleportationProvider>()`), warn and return if it is still null, then subscribe `OnLocomotionStarted` and `OnLocomotionEnded` to `provider.locomotionStarted` and `provider.locomotionEnded` with `+=`. These are C# events with an `Action<LocomotionProvider>` signature, not UnityEvents, so there is no `AddListener`. In `OnDestroy()` unsubscribe with `-=`. Put a `Debug.Log` in each handler for now.
**Check:** every teleport prints two lines in the Console, in order: started, then ended.

**Task 2 (10 min) — Cut to black, fade back in** · file: `Scripts/TeleportFade.cs`, TODO 4–6
In `OnLocomotionStarted`, stop any running fade coroutine, call `SetAlpha(1f)`, and count the teleport. In `OnLocomotionEnded`, start the `FadeIn()` coroutine: wait `holdSeconds`, then over `fadeInSeconds` set alpha to `1 - t/fadeInSeconds` each frame, ending at 0. Temporarily set *Fade In Seconds* to 2 in the Inspector to see the fade clearly, then restore 0.15.
**Check:** each teleport is a blink — black on the frame you land, then the new spot fades in. Teleport twice in quick succession: no flicker, no stuck black screen.

**Task 3 (15 min) — Sample the arc** · file: `Scripts/TeleportArcPreview.cs`, TODO 1–2
Fill `points[i]` with $p_0 + v_0 t + \tfrac{1}{2} g t^2$ for $t = i \cdot$ `timeStep`. Then, starting at $i = 1$, `Physics.Raycast` from `points[i-1]` toward `points[i]` for the segment's length; on a hit, replace `points[i]` with `hit.point`, remember `hit.normal`, set `HasLanding = true`, set `count = i + 1`, and `break`. Try *Launch Speed* 6 and 10, and *Gravity* −9.81 and −15, and notice how far the arc reaches and how quickly it falls.
**Check:** with **P** toggled on, a smooth arc leaves the right controller (or the head), bends down, and ends exactly on the floor, ramp, or wall — never poking through. Raising the aim moves the landing point farther away without a jump.

**Task 4 (15 min) — Validate the landing** · file: `Scripts/LandingValidator.cs`, TODO 1–3, then `Scripts/TeleportArcPreview.cs`, TODO 3–4
Implement `SlopeDegrees` with the clamped dot product and `Mathf.Acos * Mathf.Rad2Deg`. In `IsValid`, reject when the slope exceeds `maxSlopeDegrees` ("Too steep (30 deg)") or when `Physics.CheckSphere` at `point + up * clearanceHeight` with `bodyRadius` finds anything ("Too close to a wall"). Back in the arc script, pass the landing point and normal to the validator, color the line green or red with `line.startColor`/`endColor`, call `SetPositions`, and park the status label 0.3 m above the landing point facing the camera.
**Check:** aiming at open floor → green arc, label *OK*. Aiming at the ramp → red, *Too steep (30 deg)*. Aiming into the wall gap → red, *Too close to a wall*. Aiming at an anchor platform → green.

**Task 5 (15 min) — Anchors and arrival orientation, then record** · no new code
Teleport to **Anchor Platform 2** with the rig's built-in teleport. You arrive facing the Guide, whatever direction you were looking before. Now select the anchor, change *Match Orientation* to *World Space Up*, Play again, and notice you keep your own heading. Put it back. Then select the **Floor**'s TeleportationArea and set *Match Orientation* to *Target Up And Forward*: every teleport on the floor now snaps you to face +z. Feel how disorienting that is for open ground, and set it back to *World Space Up*. Record your screencast during this task: show a blink, the arc turning red on the ramp and near the wall, and the anchor turning you toward the Guide. In your bullets, name one place in your own project where a forced arrival orientation would help and one where it would hurt.

## Stretch goals

- Make your validator *bite*: remove the `TeleportationArea` from the Stone Ramp and give the three platforms and the safe floor sections the only teleport components, so XRI itself can no longer land you on the slope. Compare the two approaches (reject at aim time vs. never offer the target) in a bullet.
- Set the `TeleportationProvider`'s *Delay Time* to 0.15 s in the Inspector and move your cut-to-black into a subscription on the teleport interactor's `selectExited` so the screen is already black *before* the move — a true blink. Verify in Unity which event fires first; the order matters.
- Draw a small ring (a second `LineRenderer` or a flattened cylinder) at `LandingPoint` sized to `bodyRadius`, so the player sees the footprint they are about to occupy.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. The fade quad and the arc preview need no changes. On the headset, the right controller is the real *Aim Source*, so the arc leaves your hand — check that it lines up with the rig's own teleport ray (both use the same forward). Hold the headset still and teleport: the black should arrive on the same frame as the move; if you see a flash of the destination before the black, your `OnLocomotionStarted` is doing work before `SetAlpha(1f)`.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: a teleport blink, the arc preview turning green on the floor and red on the ramp and next to the wall, and an anchor teleport that turns you toward the Guide.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus one place in your project where forced arrival orientation helps and one where it hurts.

## Troubleshooting

| Symptom | Fix |
|---|---|
| **Activity** menu is missing from the menu bar | A script has a compile error; open the Console, fix the red line, wait for the spinner at bottom-right to finish |
| Nothing happens when I push the thumbstick forward | Press **Tab** until the *right controller* is selected; check the simulator panel for the key bound to *Primary 2D Axis*. The teleport is on the stock rig, not your code |
| Teleport works but the screen never goes black | TODO 2 not done, or `provider` is null — read the Console warning; the builder assigns it, but re-run **Build Starter Scene** if you deleted the Locomotion child |
| Screen goes black and stays black | The coroutine was stopped before it reached `SetAlpha(0f)`; make sure `OnLocomotionEnded` starts a new coroutine every time and that `FadeIn` ends with `SetAlpha(0f)` |
| The arc is invisible or plain white | The `ArcLine` material must use vertex colors (Sprites/Default). If your project is URP and it draws nothing, set the material's shader to *Universal Render Pipeline/Particles/Unlit* |
| Slope reads 90° on the flat floor or NaN | You used `hit.point` where `hit.normal` was needed, or forgot to clamp the dot product before `Mathf.Acos` |
| Every spot says "Too close to a wall" | `clearanceHeight` is too low and the sphere touches the floor; keep it at least `bodyRadius` + 0.1 above the point |

Created by Isac Artzi

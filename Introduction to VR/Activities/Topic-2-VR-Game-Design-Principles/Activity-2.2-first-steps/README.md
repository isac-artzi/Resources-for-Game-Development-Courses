# Activity 2.2 — First Steps: Teleport and Move

Topic 2: VR Game Design Principles on a Variety of Platforms · Week 3, Thursday · Stepping stone to Milestone 2 — Platform Compatibility Design

## Where this fits

By the end of class you will travel the trail toward the Mountain Pass two different ways — blinking between glowing waystones with the teleport arc, and walking smoothly with the thumbstick — and you will switch between *teleport-only*, *smooth-only*, and *hybrid* while the game runs. A comfort vignette will darken the edges of your view exactly as fast as you move, and the waystones will scatter to new places after every jump. Milestone 2 asks for a prototype on the headset with movement; this is that movement, built from the locomotion providers that already ship on the Starter Assets rig. Topic 5 will return to locomotion in depth (dash, blink, comfort settings); today you learn the two basic modes and the one comfort aid every VR game needs.

*Elaria hook:* the forest thins and the ground tilts upward. Ancient waystones line the switchbacks to the Mountain Pass — step onto one and it carries you to the next in a heartbeat. But the stones are restless: each time you use one, the others wander. The Guide waits at the top and does not care how you arrive, only that you arrive with your stomach settled.

## Learning goals

- You can explain the vestibular–visual conflict and use it to justify why teleport is comfortable and smooth movement is immersive but risky.
- You can add `TeleportationArea` and `TeleportationAnchor` components to scene geometry and set their *Match Orientation* deliberately.
- You can find the rig's `ContinuousMoveProvider` and `TeleportationProvider` from code and enable or disable them at runtime, with a keyboard fallback that reads the Input System.
- You can measure the rig's artificial speed from frame to frame and map it to a visual effect with `Mathf.InverseLerp` and exponential smoothing.
- You can generate a uniformly distributed random point in a disk and say why $r\sqrt{u}$ is needed.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-2.2-first-steps`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** tab → *Import* **Starter Assets** and **XR Interaction Simulator**. If a dialog offers to add the *Teleport* interaction layer, accept it.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. Menu bar → **Activity → Check Setup**. Both lines should say `OK`.
6. Menu bar → **Activity → Build Starter Scene**. Read the Console: it should say *Smooth motion enabled on Left Controller*. If it warns that it could not find *ControllerInputActionManager*, select **XR Origin (XR Rig) → Left Controller** and tick **Smooth Motion Enabled** yourself.
7. Press **Play**. You stand at the foot of the trail; Waystone 1 glows 5 m ahead.

Simulator controls that matter today: **Tab** to the **left controller** and drive its thumbstick from the keyboard to walk (the simulator's on-screen panel shows the keys bound to the *Primary 2D Axis*; in the default layout they are the arrow keys); **Tab** to the **right controller**, aim at the floor or a waystone, push its thumbstick *forward* to show the teleport arc and release to jump; push it *left/right* for snap turns. **M** cycles the locomotion mode (your script). Keep the head selected when you only want to look around with the mouse.

## VR theory

**Vestibular–visual conflict.** Your inner ear (the vestibular system) senses acceleration; your eyes sense motion through **optic flow** — the streaming of the visual field past you. In real life the two agree. When the rig glides forward in VR, the eyes report motion and the inner ear reports stillness; the brain treats this mismatch as a sign of poisoning and answers with nausea. The conflict scales with *acceleration* and with how much of the peripheral view is flowing; steady speed in a small central window is far more tolerable than stop-start motion filling the whole display. Braun and Rizzo discuss comfort and locomotion choices in Chapter 5 of *XR Development with Unity*.

**Teleport** sidesteps the conflict entirely: there is no motion to see, only a cut. The cost is presence and spatial understanding — players lose the sense of distance travelled and can feel like a chess piece. **Smooth (continuous) movement** keeps the world coherent and is what most players expect from a game, at the price of comfort for a large minority. **Hybrid** designs offer both and let the player choose; nearly every commercial VR title does this, and so will *Realm of Legends*.

**The XRI locomotion stack.** A **LocomotionProvider** is a component that moves the **XR Origin**; the Starter Assets rig ships several on its *Locomotion* child: `TeleportationProvider`, `ContinuousMoveProvider`, `SnapTurnProvider`, `ContinuousTurnProvider`, coordinated by a `LocomotionMediator` so two providers do not fight for the rig at once. A **teleport interactable** (`TeleportationArea` for a whole surface, `TeleportationAnchor` for one exact spot) is selected by the teleport ray and hands the provider a `TeleportRequest`. **Match Orientation** decides your facing on arrival: *World Space Up* keeps your current facing (right for open ground), *Target Up And Forward* turns you to face the anchor's forward — a design tool for pointing players at what matters when they land.

**Snap turn** exists for the same reason as teleport: rotating the view smoothly produces strong optic flow with no vestibular counterpart. Turning in 30–45° steps trades smoothness for comfort. Players who can physically turn (standing, wireless) often never touch it.

**Vignetting (tunneling)** is the most common comfort aid for smooth movement: darken the periphery while moving so the flow reaches only the central few degrees where the eye is fixated anyway. It works because motion sensitivity is highest in peripheral vision. The effect should follow speed and fade quickly when you stop; a vignette that lingers is itself unsettling.

**Platform note.** On a laptop with the simulator nothing here can make you sick, so today you reason about comfort rather than feel it. On Quest 3 next week, you will. Build the switch and the vignette now so you can compare the modes honestly then — that comparison is a paragraph in your compatibility document.

## Math foundation

**Speed** is distance over time. The rig moved from $p_{t-1}$ to $p_t$ during the last frame of length $\Delta t$ seconds:

$$v = \frac{\lVert p_t - p_{t-1} \rVert}{\Delta t}$$

Worked example: at 72 Hz, $\Delta t = 0.0139$ s. The rig moved 0.0139 m → $v = 1.0$ m/s (the provider's default *Move Speed*). A teleport moves it 6 m in one frame → $v = 432$ m/s, which is why `ComfortVignette` treats any jump over 1 m as a teleport and reports 0.

**Mapping speed to alpha** uses `Mathf.InverseLerp(a, b, x)`, which returns where $x$ sits between $a$ and $b$ as a 0–1 fraction, clamped:

$$\alpha_{target} = \alpha_{max} \cdot \operatorname{clamp}\!\left(\frac{v - v_{min}}{v_{max} - v_{min}},\ 0,\ 1\right)$$

With $v_{min} = 0.2$, $v_{max} = 1.5$, $\alpha_{max} = 0.85$ and $v = 1.0$: fraction $= 0.8 / 1.3 = 0.615$, so $\alpha_{target} = 0.52$. Raise *Move Speed* to 2 m/s and the fraction clamps to 1: $\alpha_{target} = 0.85$.

**Exponential smoothing** eases the actual alpha toward the target the same way regardless of frame rate:

$$\alpha \leftarrow \alpha + (\alpha_{target} - \alpha)\left(1 - e^{-k\,\Delta t}\right)$$

With $k = 6$ and $\Delta t = 0.0139$: $1 - e^{-0.0833} = 0.080$, so each frame closes 8% of the remaining gap; after 0.4 s (29 frames) you have closed $1 - 0.92^{29} \approx 91\%$. The half-life is $\ln 2 / k = 0.116$ s. Compare `Lerp(a, b, k·dt)`: at 144 Hz it takes twice as many frames but each is half as strong — almost the same, but only *almost*, and it breaks when $k\,\Delta t > 1$.

**Uniform random point in a disk.** Picking a random radius in $[0, R]$ and a random angle piles points near the center (a thin ring near the rim has far more area than one near the middle). The fix: with $u, v$ uniform in $[0,1)$,

$$r = R\sqrt{u}, \qquad \theta = 2\pi v, \qquad (x, z) = (c_x + r\cos\theta,\ c_z + r\sin\theta)$$

Check: the inner half-radius disk has a quarter of the area, and $P(r < R/2) = P(\sqrt{u} < 1/2) = P(u < 1/4) = 1/4$. Correct.

**Radial gradient for the vignette.** For a pixel at normalized coordinates $(u, v) \in [0,1]^2$, its distance from the center scaled so that an edge midpoint is 1: $r = 2\lVert(u,v) - (0.5, 0.5)\rVert$. Alpha $= \operatorname{SmoothStep}(0, 1, \operatorname{InverseLerp}(r_{in}, r_{out}, r))$. With $r_{in} = 0.45$, $r_{out} = 0.85$, a pixel at $r = 0.65$ gets alpha 0.5; at the corners ($r = 1.41$) alpha is 1.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_2_2.unity` with:

- **Floor** — a 50 m gravel plane carrying a `TeleportationArea` (*Match Orientation: World Space Up*) on the *Teleport* interaction layer, so you can teleport anywhere and the grab ray ignores it.
- **XR Origin (XR Rig)** at the origin with the **XR Interaction Simulator** and an **XR Interaction Manager**. The builder switched the *left* controller's *Controller Input Action Manager* to **Smooth Motion Enabled**, so the left stick drives `ContinuousMoveProvider`; the right stick keeps teleport (forward) and snap turn (sideways).
- **Rock Walls** — two rows of static rock cubes 6–8 m to either side of the trail, there to give you optic flow.
- **Waystones** (parent) — **Waystone 1–4** along a switchback; each is a group with a glowing disc, a standing stone and a label, carrying a `TeleportationAnchor` (*Target Up And Forward*, rotated to face the next waystone). The parent carries `TeleportSpotRandomizer` with the four anchors, the rig, and a **Teleport Count** label wired.
- **Mysterious Guide** at the top of the trail, 21 m out.
- **Comfort Vignette** — a Quad parented to the *Main Camera*, 0.35 m ahead and 1.2 m wide, collider removed, on the *Ignore Raycast* layer, using the transparent `Vignette` material. It carries `ComfortVignette` with *Rig* set to the XR Origin.
- **Locomotion Controls** — an empty GameObject with `LocomotionModeSwitch` (rig, **Mode Label**, and the rig's *Teleport Interactor* objects pre-wired).
- A **Welcome Sign** with today's controls.

Scripts live in `Assets/Scripts/`.

## Your tasks (about 70 min)

**Task 1 (10 min) — Walk the trail both ways** · no code
Press Play. Teleport to Waystone 1 with the right stick and notice you are turned to face Waystone 2 on arrival (*Target Up And Forward*). Teleport onto plain floor and notice your facing is kept (*World Space Up*). Then walk with the left stick between the rock walls. Select **XR Origin (XR Rig) → Locomotion** in the Hierarchy while playing and find the four providers.
**Check:** both modes work from the simulator before you write anything. If walking does nothing, see *Before you start* step 6.

**Task 2 (15 min) — The mode switch** · file: `Scripts/LocomotionModeSwitch.cs`, TODO 1–3
Find the two providers with `GetComponentInChildren<…>(true)` and warn if either is missing. Read `Keyboard.current[cycleKey].wasPressedThisFrame` (plus the optional `InputActionReference`) to cycle `mode`, and in `Apply()` translate the mode into `enabled` flags on the providers and `SetActive` on the teleport interactor objects.
**Check:** the label reads *Locomotion: TeleportOnly / SmoothOnly / Hybrid* as you press **M**; in Smooth-only no arc appears; in Teleport-only the left stick is dead. The provider checkboxes flip in the Inspector.

**Task 3 (20 min) — Speed-driven vignette** · file: `Scripts/ComfortVignette.cs`, TODO 2–4
In `LateUpdate`, compute the rig's speed from its position change and `Time.deltaTime`, zeroing it for jumps over `teleportJumpMeters`. Map speed to a target alpha with `Mathf.InverseLerp(minSpeed, maxSpeed, speed) * maxAlpha`, ease `alpha` toward it with `1 − e^(−k·dt)`, and write it into `mat.color.a`.
**Check:** walking darkens the view within half a second and stopping clears it; teleporting does not flash. Raise the provider's *Move Speed* to 2 and the vignette hits its maximum.

**Task 4 (10 min) — A hole in the middle** · file: `Scripts/ComfortVignette.cs`, TODO 1
Implement `BuildGradient()`: a 128 × 128 `Texture2D` whose alpha rises from 0 inside `innerRadius` to 1 beyond `outerRadius` via `InverseLerp` and `SmoothStep`, assigned to `mat.mainTexture` in `Start`.
**Check:** the center of the view stays clear while the edges darken. Set *Inner Radius* to 0.2 to feel a tight tunnel; put it back to 0.45.

**Task 5 (15 min) — Restless waystones and screencast** · file: `Scripts/TeleportSpotRandomizer.cs`, TODO 1–3
Subscribe to each anchor's `teleporting` event, implement `RandomPointInDisk` with $r = R\sqrt{u}$, and in `Relocate` try up to 20 candidates that respect `keepAwayFromPlayer` and `minSpacing` before moving the anchor and turning it to face the center. Record your screencast now: switch modes, walk with the vignette, teleport twice and show the waystones scattering.

## Stretch goals

- Show the current speed and alpha on the **Mode Label** so viewers of your screencast can see the numbers move (`CurrentSpeed.ToString("F2")`).
- Make the vignette react to *turning* too: measure the rig's yaw change per second with `Quaternion.Angle` and feed the larger of the two normalized values into `InverseLerp`. Snap turns should be ignored the same way teleports are.
- Give the waystones a "cooldown": after relocating, scale them up from 0 to 1 over 0.3 s with `Mathf.SmoothStep` so the jump reads as magic rather than a glitch.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. On the headset the left stick walks and the right stick teleports and snap-turns without any change. Assign *Cycle Action* on `LocomotionModeSwitch` to a button action from the *XRI Default Input Actions* asset (any button on either hand) so you can switch modes with the headset on. Then do the real experiment: walk the trail in Smooth-only with *Max Alpha* at 0, then at 0.85, and write one honest sentence about each for your compatibility document. Stop the moment you feel warm or dizzy — comfort testing is not endurance testing.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: cycling the three modes with the label changing, walking with the vignette closing and opening, two teleports with the waystones scattering, and the arrival facing on a waystone.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus one sentence on which mode you would make the *default* in Realm of Legends and why.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Left stick does not walk | *Smooth Motion Enabled* is off on the Left Controller's *Controller Input Action Manager*; tick it, or rebuild the scene and read the Console |
| No teleport arc appears | You are aiming with the *left* controller (it now walks), or the mode is Smooth-only; Tab to the right controller and check the label |
| The arc appears but the floor never highlights | The *Teleport* interaction layer is missing: the Console warned at build time; add it in **Edit → Project Settings → XR Plug-in Management → XR Interaction Toolkit** (Interaction Layers, slot 31, "Teleport") and rebuild |
| View goes fully black while walking | TODO 1 (`BuildGradient`) not done, so the whole quad darkens; or *Inner Radius* ≥ *Outer Radius* |
| Vignette flashes after every teleport | The `teleportJumpMeters` guard in TODO 2 is missing or set above the teleport distance |
| Grab ray grabs the floor | The `TeleportationArea` is on the default interaction layer; see the layer fix above |
| Waystones pile up in one spot | `RandomPointInDisk` still returns `c`, or `minSpacing` is larger than the disk allows (try 3 m in a 7 m radius) |

Created by Isac Artzi

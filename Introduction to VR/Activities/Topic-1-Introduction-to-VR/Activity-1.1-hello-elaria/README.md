# Activity 1.1 — Hello, Elaria

Topic 1: Introduction to Virtual Reality · Week 1, Tuesday · Stepping stone to Milestone 1 — Project Proposal and Concept Development

## Where this fits

By the end of class you will have opened a Unity 6.5 XR project, imported the XR Interaction Toolkit samples, generated a starter scene, and stood inside it with the XR Interaction Simulator. Three artifact shards will spin on their pedestals, a placeholder Guide will glow at the far side of the clearing, and a floating readout will tell you your eye height, your distance to the Guide, and how large the Guide appears in your field of view. Those numbers are not decoration: when you write the Milestone 1 concept and record your pitch video, you will describe environments "at human scale" and NPCs "close enough to read their expression". This activity gives you the vocabulary and the measurements to back that up.

*Elaria hook:* the balance of the world has broken. You wake at the edge of the Enchanted Forest with three shards of the legendary artifact glinting on stone pedestals and a robed figure waiting in the mist. Before you can restore anything, you need to learn how this world measures itself.

## Learning goals

- You can create, open, and run a Unity 6.5 project that uses the XR Interaction Toolkit and OpenXR.
- You can explain the difference between 3DoF and 6DoF tracking and point to where each shows up in the scene.
- You can state why 1 Unity unit must equal 1 meter in VR and check an object's real-world size from the Inspector.
- You can compute the angular size of an object from its height and distance, and read that value live from a script you wrote.
- You can write two small `MonoBehaviour`s that use `Time.deltaTime`, `Transform.Rotate`, and `Quaternion.LookRotation`.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-1.1-hello-elaria`). Open it with Unity **6000.5.x**. The first open takes a few minutes while packages download.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager**. In the dropdown at the top-left choose *In Project*. Select **XR Interaction Toolkit**, open the **Samples** tab, and click *Import* on **Starter Assets** and on **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management**. On the desktop tab (Windows/Mac icon) check **OpenXR**. Ignore the yellow warning about interaction profiles for now — the simulator does not need them.
5. Menu bar → **Activity → Check Setup**. The Console should show two `OK` lines. If a line says `MISSING`, repeat step 3.
6. Menu bar → **Activity → Build Starter Scene**. A scene named `Activity_1_1` appears under `Assets/Scenes` and opens.
7. Press **Play**. You are standing at the origin, looking toward three glowing shards and the Guide.

Simulator controls that matter today: the simulator's on-screen panel lists every key. The ones you need are **Tab** (cycle which device the mouse and keyboard drive: head, left controller, right controller), **W A S D** (move the selected device), **mouse** (rotate it), and **Esc** to release the cursor. Move your head up and down and watch the eye-height readout change — that is 6DoF in action.

## VR theory

**Virtual reality (VR)** is a computer-generated environment presented to the senses — primarily sight and hearing — so that the user perceives it as a place they are *in* rather than an image they look *at*. Two ideas make that work: immersion and presence.

**Immersion** is the objective, technical side: how much of the user's sensory input the system replaces. A headset with a wide field of view, a display at 72–120 Hz, stereo rendering, and low-latency tracking is more immersive than a phone screen. **Presence** is the subjective result: the feeling of "being there". Presence is what you are designing for; immersion is one of the tools. Presence breaks the moment something contradicts the body's expectations — a wall you can walk through, an NPC three meters tall, a frame that arrives late. Braun and Rizzo introduce these ideas in Chapter 1 of *XR Development with Unity*, and Chapter 2 walks through the hardware that delivers them.

**Degrees of freedom (DoF)** count the independent ways a tracked object can move. **3DoF** tracking reports orientation only — pitch, yaw, and roll — which is what a cheap phone-based viewer or a stationary 360° video gives you: you can look around, but if you lean forward the world does not come closer. **6DoF** adds the three translations — forward/back, left/right, up/down — so leaning, crouching, and stepping all move your viewpoint. The Meta Quest 3 tracks the headset and both controllers in 6DoF using inside-out cameras. In this scene the eye-height readout only changes because the head is tracked in 6DoF; in a 3DoF system it would sit at a fixed value.

**Stereoscopic rendering** draws the scene twice per frame, once from each eye, offset by the **interpupillary distance (IPD)** — the distance between the pupils, about 63 mm on average and adjustable on the Quest 3 between 53 and 75 mm. The small difference between the two images is the strongest depth cue for objects within a few meters, which is exactly the range where players grab, read, and interact. Beyond roughly 10 m the two images converge and depth comes from other cues — perspective, occlusion, motion parallax.

**World scale** is why this course insists that **1 Unity unit = 1 meter**. Unity has no built-in unit; the number is only meaningful because the XR system reports tracked positions in meters. If your forest floor is a Plane scaled to 3 (30 m) and a door is 2.1 units tall, the player will judge both as correct because their own body is the ruler. Scale a tree to 40 units by mistake and the player feels like a mouse. Scale the whole scene down and they feel like a giant. Neither is "wrong" as a design choice — but it must be a choice.

**Field of view (FOV)** is the angular extent of what the display shows. Unity's `Camera.fieldOfView` is the *vertical* FOV in degrees; in an XR session the headset overrides it with its own optics (roughly 110° horizontal on Quest 3). In the desktop simulator you see whatever the camera's Inspector value says, usually 60°. Keep that gap in mind: the simulator shows a narrower window on the world than the headset will.

## Math foundation

The readout script computes the **angular size** of the Guide — how many degrees of your field of view it occupies. For an object of height $h$ at distance $d$ (both in meters), viewed with its center at eye level:

$$\theta = 2 \arctan\left(\frac{h}{2d}\right)$$

Worked example: the Guide capsule is 2.0 m tall and stands 7.0 m from the origin.

$$\theta = 2 \arctan\left(\frac{2.0}{14.0}\right) = 2 \arctan(0.1429) = 2 \times 8.13° = 16.3°$$

Walk to 3.5 m and the same Guide fills $2\arctan(2/7) = 31.9°$ — about the vertical span of a large monitor at arm's length. This is the number designers use when they say an NPC should be "readable": text and faces need a few degrees of angular size to register, and anything much above 40–50° at close range forces the head to scan.

Two implementation details. `Mathf.Atan` returns **radians**; multiply by `Mathf.Rad2Deg` (57.2958) to get degrees. And distance here is the *horizontal* distance between camera and target — drop the y component before calling `Vector3.Distance`, or set both y values equal — so that leaning up or down does not change the reading.

The spin script uses **frame-rate-independent motion**: an angular speed $\omega$ in degrees per second becomes a per-frame rotation of $\omega \cdot \Delta t$, where $\Delta t$ is `Time.deltaTime`, the seconds since the last frame. At 72 Hz, $\Delta t \approx 0.0139$ s, so 45°/s becomes 0.625° per frame; at 144 Hz it becomes 0.3125° per frame. Same speed, different step.

The bob uses a sine wave: $y(t) = y_0 + A \sin(2\pi f t)$ with amplitude $A$ in meters and frequency $f$ in Hz. With $A = 0.05$ and $f = 0.5$, the shard rises and falls 5 cm every 2 seconds.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_1_1.unity` with:

- **Floor** — a 30 m grass-colored plane at y = 0, marked static.
- **XR Origin (XR Rig)** at the origin, with the **XR Interaction Simulator** prefab next to it.
- **Trees** — twelve trunk-and-crown primitives in a loose ring about 9 m out, so you have a sense of enclosure and scale.
- **Pedestal 1–3** and **Shard 1–3** — three emissive cubes 1.3 m above the floor, each with a small point light. Every Shard carries `SpinCollectible` (the speeds differ so you can tell them apart).
- **Mysterious Guide** — a 2 m emissive capsule 7 m ahead, with a `Guide Label` above it that carries `LookAtPlayer`.
- **Probe Readout** — a floating `TextMesh` 2 m ahead at chest height carrying `EyeHeightProbe` and `LookAtPlayer`. Its *Target* field is already pointed at the Guide.
- **Welcome Sign** — static 3D text telling you where you are.
- Directional light and light exponential fog tinted forest green.

Scripts live in `Assets/Scripts/`. Open them in Visual Studio Code by double-clicking, or through **Assets → Open C# Project**.

## Your tasks (about 70 min)

**Task 1 (10 min) — Spin the shards** · file: `Scripts/SpinCollectible.cs`, TODO 1
Rotate the shard around its local up axis every frame. Use `Transform.Rotate(Vector3.up, degreesPerSecond * Time.deltaTime, Space.Self)`. Play, then change *Degrees Per Second* on one shard in the Inspector while playing and confirm the speed follows.
**Check:** all three shards spin; the one with the largest value spins fastest; pausing (the Pause button, not Esc) freezes them.

**Task 2 (10 min) — Make them float** · file: `Scripts/SpinCollectible.cs`, TODO 2–3
Record the shard's starting position in `Start()`, then in `Update()` set its y to `startY + bobAmplitude * Mathf.Sin(2 * Mathf.PI * bobFrequency * Time.time)`. Keep x and z untouched.
**Check:** shards drift up and down about 5 cm every two seconds and never wander sideways.

**Task 3 (15 min) — Labels that face you** · file: `Scripts/LookAtPlayer.cs`, TODO 1–3
In `Start()`, if no target is assigned, find the main camera (`Camera.main`). In `LateUpdate()` aim the label so the camera can read it: a `TextMesh` is readable from its *−z* side, so its forward should point **away** from the camera: `transform.rotation = Quaternion.LookRotation(transform.position - target.position)`. Add the *Lock Vertical* option: when true, zero the y component of that direction before building the rotation so the text stays upright.
**Check:** walk (WASD with the head selected) around the Guide — its label keeps facing you. Toggle *Lock Vertical* and crouch; with it on, the text never tilts.

**Task 4 (20 min) — The measuring probe** · file: `Scripts/EyeHeightProbe.cs`, TODO 1–4
Implement `AngularSizeDegrees(height, distance)` with the formula above (guard against distance ≤ 0 by returning 180). In `Update()`, read the camera's world y as the eye height, compute the horizontal distance to the target, call your function with `targetHeightMeters`, and write a three-line string into the `readout` TextMesh with one decimal place (`value.ToString("F1")`). Also show `cam.fieldOfView`.
**Check:** standing at the origin you read roughly *Eye height 1.6 m, Distance 7.0 m, Angular size 16.3°*. Walk halfway to the Guide and the angular size roughly doubles. Move the head up and down (select the head with Tab, then use the simulator's vertical keys) — eye height changes, angular size barely does.

**Task 5 (15 min) — Scale experiment and screencast** · no new code
Select **Mysterious Guide** and set its scale to (1.2, 2, 1.2) — a 4 m Guide. Note the angular size at 7 m. Set it back. Then select **Trees** and set its scale to 0.4. Walk around. Write one sentence in your Padlet bullets about what each change did to your sense of your own size. Record your screencast during this task so it shows the shards, the label, and the readout changing as you move.

## Stretch goals

- Give each shard a slightly different bob phase (add a `[Range(0,1)] float phase` and multiply it by `2π` inside the sine) so they do not move in lockstep.
- Show the angular size of the *nearest* shard instead of the Guide: keep an array of Transforms and pick the closest each frame.
- Make the Guide label fade its color from white to cyan as the player gets within 3 m (`Color.Lerp` on `TextMesh.color`).

## Port to Quest 3

Nothing in this activity needs the headset, but it is a good day to try the pipeline while the scene is tiny. Disable the **XR Interaction Simulator** GameObject, follow `Docs/PortingToQuest3.md`, and *Build And Run*. On the headset the readout will show your real eye height — compare it with the 1.6 m default the simulator assumes.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the shards spinning and bobbing, the Guide label following you, and the readout changing as you walk toward the Guide.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus one sentence each about the 4 m Guide and the 0.4× trees.

## Troubleshooting

| Symptom | Fix |
|---|---|
| **Activity** menu is missing from the menu bar | A script has a compile error; open the Console (Ctrl/Cmd+Shift+C), fix the red line, wait for the spinner at bottom-right to finish |
| Build Starter Scene warns `XR Interaction Toolkit rig not found` | Import the **Starter Assets** sample (Package Manager → XR Interaction Toolkit → Samples), then run the builder again |
| Play mode shows a plain camera and WASD does nothing | Import the **XR Interaction Simulator** sample and rebuild the scene, or check that OpenXR is ticked in XR Plug-in Management |
| Text is mirrored | Your `LookAtPlayer` used `target.position - transform.position`; flip the subtraction |
| Angular size shows 0 or NaN | `AngularSizeDegrees` still returns the placeholder, or distance is 0 because target and camera share a position |
| Everything is dark green | Fog density is 0.02 for a 30 m clearing; if you moved the rig far away lower it in **Window → Rendering → Lighting → Environment** |

Created by Isac Artzi

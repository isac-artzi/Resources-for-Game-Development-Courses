# Activity 6.4 — Dark Mode: Navigate by Sound

Topic 6: VR Audio and Sound Design · Week 12, Thursday · Stepping stone to Milestone 6 — Sound Design

## Where this fits

By the end of class you will be able to press one key and plunge a ruined courtyard into total darkness — and still find the artifact shard on the far side of the walls, because it hums, because the void pits between you and it buzz, because a wall in the way muffles what is behind it, and because you can *ping*: emit a click and hear echoes return from the nearest surfaces, later for the far ones. Milestone 6 asks for a "dark mode" in which the player navigates by sound alone; this activity is that mode, built as a toggle you can drop into any scene. It is also your first accessibility feature: everything you build today is what a blind or low-vision player would use to play Realm of Legends at all.

The scene is fully testable with zero downloads: the shard, the pits, the ping and the echoes all fall back to synthesized tones. Real clips make it beautiful; they are not required to make it work.

*Elaria hook:* at the heart of the Ancient Ruins the torches die. The caretaker's voice, unhurried: "Sight was always the least of your senses here. Listen. The shard sings. The pits answer. The walls will tell you where they stand, if you ask."

## Learning goals

- You can black out an XR camera (solid clear color, renderers hidden except the player's) and restore it exactly, without breaking the scene.
- You can implement a sonar ping: find the nearest N surfaces with `Physics.OverlapSphere` and `Collider.ClosestPoint`, and schedule an echo from each with a delay proportional to distance.
- You can compute acoustic time of flight ($t = 2d/c$) and explain why it must be time-stretched to be perceivable.
- You can detect audio occlusion with `Physics.Linecast` and apply it smoothly through an `AudioLowPassFilter`.
- You can describe how people navigate without sight (beacons, echoes, occlusion, texture) and translate that into design rules for an accessible VR level.

## Before you start (10 min)

1. Open the project folder (`Activity-6.4-dark-mode`) with Unity **6000.5.x**, wait for packages, accept the Input System restart if asked.
2. **Window → Package Manager → XR Interaction Toolkit → Samples**: make sure **Starter Assets** and **XR Interaction Simulator** are imported.
3. **Edit → Project Settings → XR Plug-in Management**: check **OpenXR** on the desktop tab.
4. **Activity → Check Setup** (two `OK` lines), then **Activity → Build Starter Scene**. `Activity_6_4` opens. If the Console warns that a controller could not be found for a hand marker, that is fine — the controller models remain visible anyway.
5. Press **Play** with headphones on. You are at the south end of a walled courtyard. A clear tone hums from somewhere ahead-right behind the broken screen walls; two low buzzes sit between you and it. Two readouts float ahead: Sonar and Dark Mode. Nothing responds to keys yet.

Simulator controls that matter today: **Tab** to select the head, **W A S D** to walk, **mouse** to turn. Two extra keys are yours: **K** toggles dark mode and **P** pings (both are read through the Input System and can be changed on the components). Keep the **Scene view** visible: the echo lines (cyan) and occlusion lines (green/red) draw there, and it is how you will see where you are while the Game view is black.

## VR theory

**Non-visual navigation.** People who cannot see navigate by an integrated set of sound cues. **Beacons** are continuous or repeating sounds at fixed places: a fountain, a ventilation hum, a doorbell — you steer toward or away from them. **Echoes and reflections** tell you about surfaces: a corridor sounds different from a hall, and a wall close on your left "presses" on the sound of your own footsteps. Trained echolocators go further and produce clicks with their tongue, reading the returns for position and even texture of objects several meters away. **Occlusion** — a source that dulls when you turn a corner — reveals the shape of the space between you and it. And **texture**: gravel, tile and grass under your feet are a map. Today you build the first three. Braun and Rizzo's Chapter 7 in *XR Development with Unity* gives the Unity components; the design comes from how people actually do this.

**Accessibility for blind and low-vision players** is not a niche: VR headsets are already used by low-vision players who benefit from the magnified, high-contrast world, and audio-only games (from *Papa Sangre* to *The Vale*) have shown that a full adventure can be played by ear. The design rules that fall out of today's work are simple: every objective needs a beacon; every hazard needs a distinct, unmistakable sound (low and rough, not pretty); the space must be readable by echo or by texture; and the player must be able to *ask* the world where things are — the ping — rather than wait for it to tell them. Everything that helps a blind player also helps a sighted one in a dark cave level.

**Echolocation cues.** When you emit a click, the reflection from a surface at distance $d$ returns after a round-trip time $t = 2d/c$ with $c \approx 343$ m/s. Humans can detect delays down to a few milliseconds as a change in *timbre* (the echo fuses with the click and colors it), but they only hear a separate echo above roughly 30–50 ms — a wall 5–8 m away. Games therefore **stretch time**: multiply delays by 10–30 so a wall at 2 m returns a clearly separate echo about a quarter of a second later. It is not physically accurate; it is *legible*, which matters more. The other legibility trick is a **budget**: only the nearest few surfaces echo, because six overlapping echoes are readable and thirty are noise.

**Audio occlusion** is the muffling of a sound by an obstacle between source and listener. High frequencies are absorbed and blocked more than low ones — you hear the bass of the party through the wall, not the vocals — so the standard simulation is a **low-pass filter** whose cutoff drops when a line from source to listener is blocked, plus a modest volume dip. Unity does not do this for you; you write the `Linecast`. Do it a few times a second and smooth the transition, or the switch clicks.

**Blackout mechanics in VR.** "Turning the lights off" must be done carefully: setting light intensity to zero still leaves the skybox and emissive materials; disabling the camera drops tracking and comfort. The robust approach is to clear the camera to solid black and disable the scene's `MeshRenderer`s — *except the player's own body*. Seeing your hands in the dark is not a cheat; it is the only thing standing between the player and vertigo, and blind players in real life still have proprioception.

## Math foundation

**Time of flight.** For a surface at distance $d$ meters and speed of sound $c = 343$ m/s, the round trip is

$$t = \frac{2d}{c}$$

Worked example: $d = 2$ m gives $t = 4/343 = 0.0117$ s — 11.7 ms, too short to hear as a separate echo. With a time stretch of 20, the echo plays $0.233$ s after the click. A wall at $d = 6$ m returns at $0.70$ s stretched. The gap between those two echoes — 0.47 s — is the *shape* of the space you hear.

**Nearest-N selection.** After collecting candidates, sort by distance ascending and take the first $N$. Sorting $k$ candidates costs $O(k \log k)$; with $k \approx 15$ and one ping a second it is negligible. `Collider.ClosestPoint(head)` gives the point on each surface nearest to you, which is both where the echo should play from and the correct distance for its delay.

**Echo volume by distance.** Real echoes lose energy with the square of distance both ways; here we use a design curve on $d / R_{scan}$ so that a wall at the edge of the scan is still faintly audible (0.15 at $d = R$).

**Cutoff-frequency mapping.** The filter glides toward its target with exponential smoothing:

$$f_{t+\Delta t} = f_t + (f_{target} - f_t)\left(1 - e^{-\Delta t / \tau}\right)$$

With time constant $\tau = 0.15$ s the cutoff covers 63 % of the way in 0.15 s and 95 % in 0.45 s, at any frame rate. Worked example: from 22 000 Hz toward 600 Hz, after one 72 Hz frame ($\Delta t = 0.0139$ s) the cutoff is $22000 + (600 - 22000)(1 - e^{-0.0926}) = 22000 - 21400 \times 0.0884 = 20\,108$ Hz.

**Horizontal distance for reach and hazard tests.** The head floats 1.6 m above a pit; comparing 3D distance with a 0.8 m radius would never trigger. Zero the y components first: $d_{xz} = \sqrt{(x_h - x_p)^2 + (z_h - z_p)^2}$.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_6_4.unity` with:

- **Floor** — a 20 × 20 m flagstone courtyard enclosed by four 3 m **Walls**, with three inner **Screen** walls and two **Pillars** to hide the goal and give the sonar something to echo from. All static.
- **XR Origin (XR Rig)** at (0, 0, −6) with the **XR Interaction Simulator**.
- **Artifact Shard** — the objective at (5.5, 1.1, 7.5) behind Screen C, with a glow light and a looping 3D `AudioSource` (no clip; 440 Hz placeholder), an `AudioLowPassFilter` at 22 000 Hz, and `OcclusionFilter` with `Ignore Root` set to the rig.
- **Hazards → Void Pit 1, Void Pit 2** — flat dark discs at (−3, 0, −1) and (3, 0, 0.5) with trigger colliders and the same beacon setup at 90 Hz.
- **Sonar** — carries `SonarPing` (scan 12 m, 6 echoes, stretch 20, `Ignore Root` = rig) and a **Sonar Readout**.
- **Dark Mode** — carries a 2D feedback `AudioSource` and `DarkModeController` with `Keep Visible Root` = rig, `Objective` = the shard, `Hazards` = both pits, and a **Dark Mode Readout**.
- **Left/Right Controller Marker** — small glowing spheres parented to the rig's controllers so your hands remain visible in the dark.
- **Welcome Sign**, dim purple fog, a low moon-like sun.

Scripts live in `Assets/Scripts/`. `SonarPing.MakeTone` is the shared placeholder generator for this activity.

## Your tasks (about 70 min)

**Task 1 (15 min) — Lights out** · file: `Scripts/DarkModeController.cs`, TODO 1–3
Read the toggle key through `Keyboard.current[toggleKey].wasPressedThisFrame` (and the optional action), then in `SetDark(true)` save and override the camera's clear flags, background and fog, and hide every `MeshRenderer` not under `keepVisibleRoot`, remembering each one you disabled. The restore branch is already written.
**Check:** K makes the Game view black except your two controller markers (and the controller models); K again restores the courtyard exactly — labels, shard glow, fog. Toggle five times; nothing leaks.

**Task 2 (15 min) — Ping and echoes** · file: `Scripts/SonarPing.cs`, TODO 1–4
Read the ping key with the cooldown, gather colliders with `Physics.OverlapSphere`, take each one's `ClosestPoint` to the head, skip anything under `ignoreRoot`, sort by distance, keep the nearest `maxEchoes`, and `PlayDelayed` each echo voice at its point with $t = (2d/343)\cdot\text{stretch}$ and a distance-based volume.
**Check:** facing Screen A from 2 m: click, then an echo about 0.23 s later from the wall's direction; back up to 4 m and the gap doubles. The Sonar Readout names the nearest surface; cyan lines in the Scene view fan out to each echo point. Standing in a corner, the two nearest echoes arrive almost together.

**Task 3 (15 min) — Walls muffle** · file: `Scripts/OcclusionFilter.cs`, TODO 1–4
Every `checkInterval` seconds `Linecast` from the source to the listener, treating a hit on the rig as "clear". Glide the low-pass cutoff and the volume toward their targets with the exponential smoothing factor.
**Check:** in the Scene view the shard's line to your head is green in the open and red behind a screen wall; the hum dulls and softens over a fraction of a second as you step behind Screen C, and opens up when you step out. The pits do the same.

**Task 4 (10 min) — Judge the run** · file: `Scripts/DarkModeController.cs`, TODO 4
Using `HorizontalDistance`, chime once when the head is within `objectiveRadius` of the shard, and buzz (with the cooldown) whenever it is within `hazardRadius` of a pit. Log the time and hit count.
**Check:** in the dark, walking into a pit gives one buzz then silence for 2 s; reaching the shard gives one chime and a Console line like *Objective reached after 41.3 s with 1 hazard hits*. Turn the lights on and the readout shows the same.

**Task 5 (15 min) — Play it blind, then screencast** · no new code
Press K at the start and reach the shard using only the hum, the buzzes, occlusion and pings. Then swap: have a classmate play your build while you watch the Scene view. Record the screencast during the second run so it shows the black Game view alongside the Scene view with echo lines, and your reaction to what went wrong. Note what cue you relied on most.

## Stretch goals

- Footstep texture: play a short click from the head every 0.7 m walked (track the head's XZ displacement), pitched differently when the head is within 1.5 m of a pit.
- Directional ping: use `Physics.SphereCastAll` along the head's forward instead of `OverlapSphere`, so the player can "point" the sonar like a flashlight.
- Reverb by room size: compute the mean of the nearest-6 distances after each ping and set an `AudioReverbZone`'s *Decay Time* from it — big rooms ring, corridors are dry.

## Assets you will need

See `Docs/FreeAssets.md`. Download into `Assets/Audio`:

- **Freesound** (License CC0): one "crystal hum loop" or "singing bowl" (10–20 s, for the shard), one "low drone" or "electric hum" (for the pits), one dry "click" or "tongue click" (for the ping), and one "stone tap" or short "impact" with a little tail (for the echoes).
- **Kenney Audio → *Impact Sounds* / *Interface Sounds***: usable click and tap alternatives.
- **Pixabay Sound Effects**: "sonar ping" if you want the stylized submarine sound instead of a click.

All four should be **mono** (tick *Force To Mono*) and *Decompress On Load*; the loops can be *Compressed In Memory*. Assign them to the shard's and pits' `AudioSource` clips and to `SonarPing`'s *Ping Clip* and *Echo Clip*.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. Then give the two actions real buttons: expand the Starter Assets input actions asset, and drag the right hand's *Primary Button* action into `SonarPing → Ping Action` and the left hand's *Menu* (or *Secondary Button*) into `DarkModeController → Toggle Action`; the keyboard fallbacks stay harmless. Consider enabling a spatializer (**Project Settings → Audio**) — HRTF processing makes the shard's elevation and front/back audible, which changes this activity from hard to fair. Warn testers before the blackout and keep a hand on K: total darkness in a headset is intense for some people.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the lights-out toggle (and restore), a ping with its echo lines in the Scene view, the occlusion line turning red behind a wall, and one blind run to the shard with the result line.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), and which cue (beacon, echo, occlusion) you relied on most and which you would change for a blind player.
- (optional) the time and hazard count of your best blind run, and your classmate's.

## Troubleshooting

| Symptom | Fix |
|---|---|
| K does nothing | TODO 1 not done; or the simulator captured the key — click in the Game view first; or another component binds K (change `Toggle Key`) |
| Dark mode hides my hands too | `Keep Visible Root` is empty; drag the XR Origin into it. Controller models live under the rig |
| Lights on, but some objects stayed hidden | You re-enabled only some renderers; make sure every renderer you disable is added to `hidden` |
| No echoes, or all from one spot | TODO 2–4 incomplete; check `targets.Count` with a log, and make sure each voice's position is set before `PlayDelayed` |
| Echoes come from the pits only / never from walls | `Surface Layers` excludes Default, or the walls lost their colliders; the pits are triggers and are ignored on purpose |
| Everything sounds muffled even in the open | The `AudioLowPassFilter` cutoff is at Unity's default 5000 Hz — the builder sets 22000; if you added filters by hand, fix the value |
| Shard always reads occluded | The Linecast hits the rig's CharacterController; set `Ignore Root` to the XR Origin so hits on the player count as clear |

Created by Isac Artzi

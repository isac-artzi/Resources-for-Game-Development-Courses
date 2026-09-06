# Activity 1.2 — Mock View Studio

Topic 1: Introduction to Virtual Reality · Week 1, Thursday · Stepping stone to Milestone 1 — Project Proposal and Concept Development

## Where this fits

Milestone 1 asks you for two things that need pictures: a concept video for *Realm of Legends* and a short report that re-imagines a familiar experience (a museum visit, a lecture, a shop, a campfire story) as a VR experience, illustrated with **mock views made in Unity**. A mock view is not a finished environment. It is a deliberately framed screenshot from where the player will stand, showing what is near enough to touch, what is close enough to read, and what is far enough to be scenery. Today you build the studio that produces those shots: preset camera stations you can snap to, annotation pins that label distances and sizes in the shot, and a prop placer that drops a box of any size at any distance so you can *see* what "a 40 cm sign at 4 m" looks like before you promise it in a document. Everything you learn here about viewing distances and text size comes straight back in Topic 2 when you build real menus and in Topic 4 when you build dialogue boxes.

*Elaria hook:* the Mysterious Guide cannot yet take you into the Enchanted Forest, so she shows you a memory instead: the Forest Sage's hut, drawn in grey stone and timber. "Stand here," she says, marking the ground with light. "Now here. Tell me what you can read, and what you can only sense." Every hero learns to see before they learn to act.

## Learning goals

- You can explain why a VR viewpoint is moved by translating the XR Origin (the rig) and never by writing to the camera's transform.
- You can name the three interaction zones (near, medium, far), give their approximate distances, and say what belongs in each.
- You can solve the angular-size formula for size or distance and use it to size text and props for a target viewing distance.
- You can read the Input System keyboard from a script and make a label face the player while keeping a constant angular size.
- You can compose and capture at least three annotated mock views from meaningful player positions.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-1.2-mock-view-studio`). Open it with Unity **6000.5.x**.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** tab → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup**. Both lines should read `OK`.
6. **Activity → Build Starter Scene**. The scene `Activity_1_2` opens; you stand on Station 1 looking at a grey hut with a doorway.
7. Press **Play**. Press **]** and **[**. Nothing happens yet — that is your first task.

Simulator controls that matter today: **Tab** to select the head, **W A S D** and **Q E** to move it, **mouse** to look, **Esc** to release the cursor. The activity adds its own keys, all read through the Input System and all changeable in the Inspector: **]** / **[** next / previous station, **Enter** place a prop, **Backspace** remove the last prop, **-** / **=** closer / farther, **,** / **.** smaller / bigger, **/** cycle the shape. If the simulator's on-screen panel shows that one of these keys is already taken, pick another key in the component's *Keys* section — that is exactly why they are exposed.

## VR theory

**A viewpoint is a rig position, not a camera position.** In an XR scene the camera is a child of the XR Origin (Unity.XR.CoreUtils.XROrigin). Every frame the tracking system writes the headset's pose into that camera's local position and rotation — in the simulator, the fake head does the same. Anything you write to `Camera.main.transform` is overwritten a few milliseconds later, and on a headset it would also tear the camera away from the tracked controllers. The only object you own is the rig's root. To "put the player here", you move the root so that the *tracked* head lands where you want it; to "turn the player", you rotate the root around the head. This rule holds for teleportation in Topic 5 and for every locomotion system in XRI, so learn it now with a keyboard.

**Three interaction zones.** Designers divide the space around the player by what the body can do there. The **near zone** (up to roughly 0.75–1.0 m) is arm's reach: objects can be grabbed, stereo depth is strongest, and anything that stays here for long strains the eyes because of the vergence–accommodation conflict (your eyes converge on a near object but the lenses focus at the headset's fixed focal distance, around 1.3–2 m). The **medium zone** (about 1–3.5 m) is where reading and choosing happen: menus, dialogue, signs, an NPC's face. Stereo still helps, focus is relaxed, and text can be made large enough without filling the view. The **far zone** (beyond 3.5 m) is scenery and navigation: stereo cues fade, depth comes from perspective, occlusion, and parallax, and details are wasted. Braun and Rizzo's discussion of designing for the medium in the opening chapters of *XR Development with Unity* covers the same ground; the numbers here are the common industry rules of thumb.

**Comfortable viewing distance.** Two limits bracket the medium zone. Below about 0.5 m, most people cannot fuse the two eye images comfortably for long. Beyond about 3.5–4 m, a UI element must be physically large to stay readable, and large surfaces far away start to feel like a cinema screen rather than an object in the room. Put persistent UI at 1–2 m and let it be a thing in the world.

**Text legibility is an angle, not a size.** A letter is readable when it spans enough of your visual field for the display to render it with enough pixels. The Quest 3 shows roughly 25 pixels per degree; a character 1° tall gets about 25 pixels, which reads cleanly; at 0.5° it gets 12 and becomes tiring; below about 0.3° it is a smudge. So the design rule is angular: aim for **about 1° per line of text as a minimum**, 1.5–2° for anything the player must read quickly. The same text is legible at 1 m and illegible at 6 m unless you scale it — which is what the annotation pin does automatically.

**Mock views are arguments.** A screenshot from a random flying camera proves nothing. A screenshot from a marked station at eye height, with pins that state distances and sizes, lets a reader verify your claims: "the exhibit label is 0.9 m from the visitor and its type is 2°, so it is readable without stepping closer". That is the standard your Milestone 1 report is aiming for.

## Math foundation

From Activity 1.1, the angular size of an object of size $s$ at distance $d$ is

$$\theta = 2\arctan\left(\frac{s}{2d}\right)$$

Today you use it backwards. Given a target angle, solve for the **size** you need at a given distance, or the **distance** at which a given size reaches the angle:

$$s = 2d\tan\left(\frac{\theta}{2}\right) \qquad\qquad d = \frac{s}{2\tan\left(\frac{\theta}{2}\right)}$$

Worked example — the lore scroll's text must read at Station 3 (0.75 m) at a comfortable 1.5° per line. Line height $s = 2 \times 0.75 \times \tan(0.75°) = 1.5 \times 0.01309 = 0.0196$ m — about 2 cm, the size of the type on a paperback. If instead you want the same 1.5° from the threshold at 2.5 m: $s = 5 \times 0.01309 = 0.065$ m, 6.5 cm letters, a road sign. Going the other way: a 0.5 m cube reaches 14.3° at 2 m ($2\arctan(0.5/4) = 2 \times 7.13°$); to shrink it to 5° you must stand at $d = 0.5 / (2\tan 2.5°) = 0.5 / 0.0873 = 5.7$ m.

**Pixels to meters.** With a display density of $P$ pixels per degree, a feature that must cover $n$ pixels needs an angle of $n / P$ degrees. For $n = 25$ and $P = 25$ that is 1°; plug 1° into the size formula at 1.5 m and you get $s = 3 \times \tan(0.5°) = 0.026$ m — 2.6 cm letters for a wrist panel or a menu at 1.5 m. Keep this triple (pixels → degrees → meters) handy; it is the whole theory of VR text sizing.

**Constant angular size by scaling.** If the label's scale is proportional to distance, $s = k d$, then $\theta = 2\arctan(k/2)$ — the distance cancels. The pin uses scale $= d / d_{ref}$, so at the reference distance $d_{ref} = 2$ m the scale is 1, at 4 m it is 2, at 1 m it is 0.5. Clamp it so a pin on the mountain does not become the size of the mountain.

**Heading from a direction vector.** To rotate the rig so the head faces the station's forward you need each direction's heading (yaw). For a horizontal vector $(x, 0, z)$ the heading measured from +z toward +x is $\psi = \operatorname{atan2}(x, z)$. Example: $(0.7, 0, 0.7)$ gives $\operatorname{atan2}(0.7, 0.7) = 45°$; $(-1, 0, 0)$ gives $-90°$. Rotate the rig about the vertical axis by $\psi_{station} - \psi_{head}$, pivoting on the head so it does not slide off the marker.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_1_2.unity` with:

- **Floor** — 40 m of grass, light green fog, warm sun.
- **XR Origin (XR Rig)** starting on Station 1, with the **XR Interaction Simulator** next to it.
- **Sage Hut (blockout)** — a 6 × 5 × 3 m room of plaster walls and timber beams with a **Doorway (1.0 × 2.1 m)**, a 0.75 m **Table**, a **Lore Scroll** on it, the **Forest Sage** (a 1.7 m capsule) and a warm hearth light. All static.
- **Mountain** — a rock dome 45 m out, mostly hidden in fog: your far-zone landmark.
- **Stations** — five floor-level empties with glowing discs and numbers. Their blue (+z) arrows are the viewing directions: 1 Approach (8 m to the doorway), 2 Threshold (2.5 m), 3 The Scroll (0.75 m), 4 Speaking with the Sage (1.5 m), 5 Overview (12 m, diagonal).
- **Viewpoint Cycler** — an empty carrying `ViewpointCycler`; *Stations* and *Station Label* (the **Station Readout** TextMesh) are pre-assigned.
- **Pins** — three `AnnotationPin` objects (Doorway, Scroll, Mountain), each a small gold marker with a separate label and a `LineRenderer` leader, all references pre-set.
- **Prop Placer** — an empty carrying `PropPlacer`, its *Material* set to clay and *Parent* set to the empty **Placed Props**.
- **Welcome Sign** with the key list.

Scripts live in `Assets/Scripts/`: `ViewpointCycler.cs`, `AnnotationPin.cs`, `PropPlacer.cs`.

## Your tasks (about 70 min)

**Task 1 (15 min) — Snap the rig to a station** · file: `Scripts/ViewpointCycler.cs`, TODO 1–3
Find the `XROrigin` and its camera in `Start()`, read **]** and **[** through `Keyboard.current[key].wasPressedThisFrame`, and implement the position half of `GoToStation`: compute the head's horizontal offset from the rig root, zero its y, and set the rig position to `station.position - offset`. Do not touch the camera. The wrap-around and the label call are already written.
**Check:** pressing **]** moves you from station to station in order and **[** goes back; look down and the glowing disc is under your feet; your eye height did not change.

**Task 2 (10 min) — Face the right way** · file: `Scripts/ViewpointCycler.cs`, TODO 4–5
Compute the head's and the station's headings with `Mathf.Atan2(x, z) * Mathf.Rad2Deg` on their flattened forward vectors, then `RotateAround(head.position, Vector3.up, stationYaw - headYaw)` on the rig. Finish `PlaceLabel` so the station name appears 1.5 m ahead of you, a little below eye level, facing you.
**Check:** Station 1 looks straight through the doorway, Station 3 looks down at the scroll, Station 5 looks diagonally at the hut. Turn your head with the mouse, press **]** again — you are re-aimed. The station sign is readable and stays put when you walk toward it.

**Task 3 (15 min) — Pins that read from anywhere** · file: `Scripts/AnnotationPin.cs`, TODO 1–5
Write the note into the label, float the label `labelHeight` above the anchor, make it face the head (review of 1.1's billboard), scale it by `distance / referenceDistance` clamped between *Min Scale* and *Max Scale*, and feed the two leader endpoints to the `LineRenderer`.
**Check:** all three pins show their own text, connected by a thin line to their marker. From Station 5 the Mountain pin is as readable as the Doorway pin from Station 2. Untick *Keep Angular Size* on one pin and watch it shrink with distance.

**Task 4 (15 min) — Place and measure props** · file: `Scripts/PropPlacer.cs`, TODO 1–5
Wire the seven keys, then implement `Place()`: flatten the head's forward, step out `distanceMeters`, rest the prop on the floor, create the primitive with the clay material under *Placed Props*, and attach an `AnnotationPin` whose text reports size, distance, angular size and zone. Implement `AngularSizeDegrees` (same formula as 1.1) and `ZoneName`.
**Check:** with the defaults, **Enter** drops a 0.5 m cube 2 m ahead and the Console reads `0.50 m at 2.00 m / 14.3 deg - medium zone`. Press **-** three times and **Enter**: a cube at 1.25 m is "medium"; at 0.75 m it is "near". **Backspace** removes the last one.

**Task 5 (15 min) — Compose three mock views and record** · no new code
Pick the experience you are re-imagining for the Milestone 1 report (or use the hut as a stand-in). Using props and pins, stage it: a far establishing view, a medium "decision" view with something to read, and a near "interaction" view with something to grab. Add at least one pin per view that states a distance and an angle. Take a screenshot from each station (Game view → the *Screenshot* option in the ⋮ menu, or your OS tool) and record your screencast while you cycle through them and explain each choice in one sentence.
**Check:** three screenshots, each taken from a numbered station at eye height, each with at least one pin whose numbers you could defend.

## Stretch goals

- Add number keys `Key.Digit1`–`Key.Digit5` in `ViewpointCycler` that jump straight to a station, and a `Key.Digit0` that returns to wherever the rig was before the first jump.
- Give `PropPlacer` a *Snap To Zone Boundary* key that sets `distanceMeters` to exactly `nearZoneMax` or `mediumZoneMax`, so you can photograph an object sitting right on a zone line.
- Add a `fadeWithDistance` option to `AnnotationPin`: beyond 10 m, lerp the label alpha toward 0.3 so far pins recede instead of cluttering the far zone. (`TextMesh.color` — build a new `Color` with the changed alpha.)

## Port to Quest 3

The scene needs no headset features. Disable the **XR Interaction Simulator** GameObject, follow `Docs/PortingToQuest3.md`, and *Build And Run*. There is no keyboard on the headset, so the cycler will not respond to **]**: for the headset add an `InputActionReference` field to `ViewpointCycler`, bind it to a controller button in the Starter Assets input actions, and call `action.action.WasPressedThisFrame()` alongside the keyboard check. On the headset, read the Scroll pin from Station 3 and decide honestly whether 2 cm text is comfortable for you.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: cycling through the five stations, at least one prop placed and measured, and your three composed mock views with their pins.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus one sentence naming the distance and angular size you chose for the text in your medium view and why.
- Your three annotated screenshots (drop them into the Padlet post as images); they go into the Milestone 1 report later.

## Troubleshooting

| Symptom | Fix |
|---|---|
| **]** does nothing, no errors | `Keyboard.current` is null because the Game view does not have focus — click inside it once. If the simulator uses the same key, change *Next Key* in the Inspector |
| After a jump you float 1.6 m above the floor | You forgot `headOffset.y = 0f`; the rig's y must stay at floor level |
| The jump lands you next to the disc, not on it | You rotated around the rig's pivot instead of the head, or rotated before positioning; do position first, then `RotateAround(head.position, …)` |
| You arrive facing the wrong way by 180° | Atan2 arguments swapped: it is `Mathf.Atan2(f.x, f.z)`, x first |
| Pin text is mirrored or tilted | Direction must be `label.position - head.position` with y zeroed before `LookRotation` |
| Placed prop is buried in the floor or floating | Center y must be `sizeMeters * 0.5f` for cube/sphere and `sizeMeters` for cylinder/capsule (they are 2 units tall at scale 1) |
| Console shows `0.0 deg` on every prop | `AngularSizeDegrees` still returns the placeholder; remember `Mathf.Rad2Deg` |

Created by Isac Artzi

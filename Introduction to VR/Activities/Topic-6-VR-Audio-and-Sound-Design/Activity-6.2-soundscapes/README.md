# Activity 6.2 — Soundscapes and Music Zones

Topic 6: VR Audio and Sound Design · Week 11, Thursday · Stepping stone to Milestone 6 — Sound Design

## Where this fits

By the end of class you will walk a short trail through the three environments of Realm of Legends — a Forest pad, a Mountain pad, a Ruins pad — and the world will *re-score itself* under your feet. Each zone has its own music, and the change is an equal-power crossfade rather than a cut. Each zone also has its own ambience built the way sound designers build them: a constant **bed** that fades in and out, and **detail** one-shots that fire at random moments, from random directions around your head, with slightly random pitch, so three short clips never sound like a loop. Finally, you will create an **Audio Mixer** with three snapshots and have the scene glide between them, which is how Milestone 6's "background music + ambient + SFX" will be balanced without touching a hundred individual volume sliders.

Like 6.1, everything is audible before you download anything: zones without a music clip play a synthesized chord, beds hum, and one-shots blip. Real recordings go in during the last task.

*Elaria hook:* the Guide says every place in Elaria has its own song. Step from the forest onto the mountain pass and the birds fall silent, the wind rises, and the strings thin out to a single cold note. The ruins hum with something older still.

## Learning goals

- You can describe a soundscape as layers (bed, mid-ground, detail) and say which Unity object type carries each layer.
- You can crossfade two `AudioSource`s with equal-power gains and explain why a linear crossfade dips in the middle.
- You can schedule random events with exponential inter-arrival times and place them at random 3D positions around the listener.
- You can detect a point entering and leaving an axis-aligned box with `Bounds.Contains` and fire an event only on the transition.
- You can create an `AudioMixer` with groups and snapshots, route sources to groups, and trigger `AudioMixerSnapshot.TransitionTo` from code with a null-safe fallback.

## Before you start (10 min)

1. Open the project folder (`Activity-6.2-soundscapes`) with Unity **6000.5.x** and wait for packages to resolve. Accept the Input System restart if asked.
2. **Window → Package Manager → XR Interaction Toolkit → Samples**: import **Starter Assets** and **XR Interaction Simulator** if they are not present.
3. **Edit → Project Settings → XR Plug-in Management**: check **OpenXR** on the desktop tab.
4. **Activity → Check Setup** (two `OK` lines), then **Activity → Build Starter Scene**. `Activity_6_2` opens.
5. Open **Window → Audio → Audio Mixer** and dock it next to the Console; you will use it in Task 5.
6. Press **Play** with headphones on. You are at the trailhead, facing three colored pads in a row. Nothing plays yet — the manager's readout at the trailhead says *Zone: None*. That is expected until Task 1.

Simulator controls that matter today: **Tab** to select the head, **W A S D** to walk the trail (about 24 m end to end — it takes a few seconds), **mouse** to turn. Keep the Scene view open to see the translucent zone gizmos and, later, the yellow ticks where one-shots land.

## VR theory

A **soundscape** is the total sonic environment of a place. Sound designers build one in layers. The **bed** (or ambience loop) is the constant floor: wind, water, room tone, distant traffic. It is usually stereo or nearly 2D, fades slowly, and should be almost unnoticeable — you notice it only when it stops. The **mid-ground** is recurring but variable texture: rustling leaves, a creek's gurgle changing as you walk. The **detail** layer is discrete events — a bird, a rockfall, a drip — that arrive irregularly and come from somewhere specific. In VR the detail layer does the most for presence, because each event is a spatialized point the head can turn toward. Braun and Rizzo's Chapter 7 in *XR Development with Unity* covers the Unity side (sources, listener, mixer); today's structure is the sound-design side.

**Loops and repetition fatigue.** A single 30 s ambience loop is recognized as a loop within a few minutes; a bird chirp that always sounds identical is noticed on the third hearing. The defense is variation on three axes: **when** (random timing), **where** (random position) and **how** (random pitch, and a small pool of alternative clips). Small pitch changes of ±5–10 % are enough; more than that and a bird turns into a different animal.

**Music transitions.** Cutting music when the player crosses a line is jarring. The standard fix is a **crossfade**: the old track fades out while the new one fades in. Do it with straight lines ($1-t$ and $t$) and the middle of the fade is noticeably quieter, because loudness tracks *power* (amplitude squared) and $(0.5)^2 + (0.5)^2 = 0.5$, not 1. An **equal-power** crossfade uses $\cos$ and $\sin$ so that the summed power is constant throughout. Beyond crossfades, games use *stingers* (a short phrase that bridges two tracks), *horizontal re-sequencing* (tracks composed to align at bar boundaries) and *vertical layering* (one piece whose stems fade in and out) — the last is a natural stretch goal here.

**Audio Mixer, groups and snapshots.** An `AudioMixer` asset is a mixing desk: **groups** are its channels (Music, Ambience, SFX, Voice), each `AudioSource` outputs to one group, and a group's fader scales everything routed through it. A **snapshot** is a saved state of every fader and effect parameter; `TransitionTo(seconds)` interpolates from the current state to a snapshot. Three snapshots — Forest, Mountain, Ruins — let you re-balance the whole mix per environment (quieter music in the ruins, more low end on the mountain) with one call. **Ducking** is a mixer effect that lowers one group when another gets loud (music dips under dialogue); Unity's mixer has *Duck Volume* for exactly that, and it is a stretch goal.

**Why polling instead of trigger messages.** `OnTriggerEnter` needs a `Rigidbody` on one side and fires only when the physics step notices an overlap; a teleport can move the rig 8 m in one frame and skip a thin volume entirely. Checking whether the head's position is inside a box each frame costs a few comparisons and never misses. The trade-off: `Bounds` are axis-aligned, so zone boxes must not be rotated. For odd shapes, use several boxes.

## Math foundation

**Equal-power crossfade.** For fade progress $t \in [0,1]$:

$$g_{out}(t) = \cos\left(\frac{\pi t}{2}\right), \qquad g_{in}(t) = \sin\left(\frac{\pi t}{2}\right)$$

Because $\cos^2 + \sin^2 = 1$, the summed power is 1 at every $t$. Worked example at the midpoint $t = 0.5$: $g_{out} = g_{in} = \cos(45°) = 0.707$, and $0.707^2 + 0.707^2 = 1.0$. A linear crossfade at the same moment gives $0.5^2 + 0.5^2 = 0.5$ — a 3 dB hole. Progress advances by $\Delta t / T_{fade}$ per frame, so a 3 s fade at 72 Hz takes 216 frames.

**Exponential inter-arrival times.** If independent events happen at an average rate of one per $\mu$ seconds, the gap between two of them follows an exponential distribution. To draw one, take $u$ uniform in $[0,1)$ and compute

$$w = -\mu \ln(1 - u)$$

Worked example with $\mu = 4$ s: $u = 0.10 \Rightarrow w = -4 \ln 0.9 = 0.42$ s; $u = 0.50 \Rightarrow w = 2.77$ s; $u = 0.90 \Rightarrow w = 9.21$ s. Half of all gaps are shorter than $\mu \ln 2 = 2.77$ s, yet the mean is exactly 4 s — clustered bursts with occasional silences, which is what nature sounds like. Clamp with a minimum gap so two clips never stack on the same frame.

**Random position on a ring around the head.** Pick $\alpha \in [0, 2\pi)$ and $r \in [r_{min}, r_{max}]$, then $p = (h_x + r\cos\alpha,\ y,\ h_z + r\sin\alpha)$ with $y$ drawn from a height range. Forest birds get $y \in [2, 5]$; ruins drips get $y \in [0.3, 2]$.

**Fade step.** `MoveTowards(current, target, step)` with $step = \Delta t \cdot V / T$ moves a volume from 0 to $V$ in exactly $T$ seconds.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_6_2.unity` with:

- **Floor** — a 60 m dirt plane, with the **XR Origin (XR Rig)** and **XR Interaction Simulator** at the trailhead (0, 0, −4).
- **Zones** — three 8 × 8 m pads centered at z = 4 (green *Forest*), z = 12 (white *Mountain*), z = 20 (tan *Ruins*). Above each pad is an invisible 4 m tall trigger box carrying `ZoneVolume` with its `Zone` id set and `Manager` wired. Each has a sign.
- **Props** — six trees on the forest pad, five boulders on the mountain pad, four columns and a lintel on the ruins pad, all static.
- **Audio Director** — carries `MusicZoneManager` with `Music A` and `Music B` (2D, looping, no clip) assigned and `Ambient Layers` pointing at three children under **Ambience**: **Forest Ambience**, **Mountain Ambience**, **Ruins Ambience**. Each carries `AmbientLayer` with its `Bed` source assigned (no clip), a distinct placeholder pitch, and its own mean gap and height range.
- **Director Readout** — a label at the trailhead showing current zone, fade progress, music level and what each music source is doing.
- **Welcome Sign**, light fog, a high sun.

No `AudioMixer` asset is created — Unity cannot make one from a script. You create it in Task 5. Scripts live in `Assets/Scripts/`.

## Your tasks (about 70 min)

**Task 1 (10 min) — Know where you are** · file: `Scripts/ZoneVolume.cs`, TODO 1–2 · `Scripts/MusicZoneManager.cs`, TODO 1
In `ZoneVolume.Update()` test `box.bounds.Contains(head.position)` and call `manager.EnterZone(zone)` on the frame the head enters and `manager.ExitZone(zone)` on the frame it leaves. In `MusicZoneManager.EnterZone` call `StartCrossfade(ClipFor(zone))` and `ApplyZoneMix(zone)`.
**Check:** walking onto the green pad logs one *enter Forest* and the readout switches to *Zone: Forest*; stepping off logs one *exit Forest*. Stand still on the pad: no repeated logs.

**Task 2 (15 min) — The crossfade** · file: `Scripts/MusicZoneManager.cs`, TODO 2
Implement the equal-power fade in `Update()`: advance `fadeT`, set the outgoing source to `cos(gπ/2) · musicLevel` and the incoming to `sin(gπ/2) · musicLevel`, and swap the two references when the fade completes. Then, as an experiment, temporarily use `1 - g` and `g` and listen to the middle of the fade.
**Check:** walking Forest → Mountain, the A-220 chord melts into the E-165 chord over 3 s with steady loudness; the readout's *fade* runs 0.00 → 1.00 and one source shows *stopped* afterward. With the linear version you hear a dip halfway.

**Task 3 (15 min) — Beds and random details** · file: `Scripts/AmbientLayer.cs`, TODO 1–3
Fade the bed with `MoveTowards` and stop it once silent. Replace the placeholder in `ScheduleNext()` with the exponential draw, clamped to `minSecondsBetween`, and fire `PlayOneShot()` from `Update()` when the time comes.
**Check:** each zone's bed hum rises over 2 s when you enter and falls when you leave; blips arrive irregularly — log the waits for a minute and their average lands near the zone's *Mean Seconds Between*.

**Task 4 (10 min) — Surround the player** · file: `Scripts/AmbientLayer.cs`, TODO 4
Place each one-shot at a random compass angle and distance around the head, at a random height from the layer's range, with a random pitch from `pitchRange`. Draw a debug ray at the spot.
**Check:** yellow ticks appear 3–8 m around you in the Scene view; blips come from different directions and pitches; forest blips are high up, ruins blips near the floor. Set *Pitch Range* to (1, 1) and hear it turn mechanical.

**Task 5 (10 min) — Mixer and snapshots** · file: `Scripts/MusicZoneManager.cs`, TODO 3–5 · plus editor work
First implement the fallback (TODO 3 and the `else` branch of TODO 5) so the overall music level glides per zone, and TODO 4 so exactly one ambience layer is active. Then in the Project window: **Assets → Create → Audio Mixer**, name it `ElariaMix`. In the Audio Mixer window add groups **Music** and **Ambience** under Master, and add snapshots named **Forest**, **Mountain**, **Ruins** (the *Snapshots* panel, + button). Select each snapshot and set faders, e.g. Forest: Music −4 dB, Ambience −6 dB; Mountain: Music −10 dB, Ambience −2 dB; Ruins: Music −14 dB, Ambience −8 dB. Set the *Output* of `Music A`/`Music B` to Music and each `Bed` to Ambience. Expand the mixer asset in the Project window and drag its three snapshots into the manager's snapshot fields.
**Check:** walking the trail, the Audio Mixer window's faders slide over 2 s at each pad; the fallback *level* number stops changing because snapshots now do the job.

**Task 6 (10 min) — Real sounds and screencast** · no new code
Drop your downloaded loops and one-shots into `Assets/Audio`, assign a music clip per zone on the Audio Director, a bed clip per `Bed`, and 2–3 one-shots per `AmbientLayer`. Tick **Force To Mono** on the one-shots (they are 3D) and leave music stereo. Record the screencast while walking the full trail twice.

## Stretch goals

- Vertical layering: give the Mountain zone two music sources (a drone and a melody) and fade the melody in only while the player is within 3 m of the boulders.
- Ducking: add a **Voice** group to the mixer, put *Duck Volume* on Music with Voice as its sidechain, and play a Guide line when the player enters the Ruins — the music should dip under it and recover.
- Make one-shots avoid the direction the player is facing (skip angles within ±30° of the head's forward), so detail sounds happen at the edge of attention instead of in front of the eyes.

## Assets you will need

See `Docs/FreeAssets.md`. Download into `Assets/Audio`:

- **Music (3 loops, 30–90 s, stereo)**: Kenney Audio *Music Jingles* / *Music Loops* packs, or Pixabay *Music* filtered by "ambient", "fantasy", "cinematic" — one calm forest piece, one sparse cold mountain piece, one slow low ruins piece.
- **Beds (3 loops, 20–40 s)**: Freesound (License CC0) — "forest ambience loop", "mountain wind loop", "cave drone" or "room tone".
- **Details (6–9 short clips, 0.5–3 s)**: Freesound — 3 bird chirps (reuse 6.1's), 2–3 "rock fall" or "gravel", 2–3 "water drip" or "stone echo". Kenney *RPG Audio* has usable clicks and thuds if Freesound is slow.

Import settings: music → *Streaming*, beds → *Compressed In Memory*, details → *Decompress On Load* and **Force To Mono**.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. On the headset, walk between pads with the thumbstick (smooth move) or teleport; the polling zone detection handles both. Set **Project Settings → Audio → Max Real Voices** to 32 (default) and keep each layer's *Voices* at 3 or fewer — a Quest 3 mix should stay under about 20 simultaneous voices. Long music should be *Streaming* to keep memory down.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the walk through all three zones with the music crossfading, the readout's fade/level numbers, yellow one-shot ticks around the player in the Scene view, and the Audio Mixer faders sliding between snapshots.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), and one sentence about the linear-vs-equal-power comparison.
- (optional) a one-line list of the clips you used and where they came from.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Readout stays *Zone: None* on a pad | `ZoneVolume` TODO 1–2 not done, or `Head` is empty because the camera is not tagged *MainCamera*; drag the rig's Main Camera into the field |
| *enter Forest* logs every frame | You call `EnterZone` whenever inside instead of on the transition; compare `nowInside` with `HeadInside` |
| Music never starts | `StartCrossfade` was not called from `EnterZone`, or TODO 2 never raises the incoming volume above 0 |
| Loudness dips halfway through a fade | You used linear gains (`1 - g`, `g`); switch to cos/sin |
| One-shots all come from one spot | TODO 4 placeholder still in place — remove the two placeholder lines under it |
| Snapshot fields will not accept the mixer | Drag a *snapshot* (expand the mixer asset with the small arrow), not the mixer itself |
| Everything is quiet after adding the mixer | The Master or a group fader is very low; select the current snapshot and reset faders to 0 dB |

Created by Isac Artzi

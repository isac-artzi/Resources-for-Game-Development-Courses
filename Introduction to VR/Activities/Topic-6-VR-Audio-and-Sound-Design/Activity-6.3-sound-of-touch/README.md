# Activity 6.3 — The Sound of Touch: SFX and Haptics

Topic 6: VR Audio and Sound Design · Week 12, Tuesday · Stepping stone to Milestone 6 — Sound Design

## Where this fits

By the end of class, every object you touch in a small stone workshop will answer you. Grabbing a shard clicks; letting it go clicks softer; setting it down gently is almost silent while hurling it at a bronze gong is loud — because the volume follows the impact speed through a curve you tune. No two hits sound the same, because each one gets a slightly different pitch and a random pick from a few clip variations. All those sounds go through one **pooled** one-shot player instead of spawning an `AudioSource` per hit, and on the headset the controller **vibrates** in time with the sound. Milestone 6 calls for SFX on interactions; Milestone 4's grab, socket and puzzle interactions were built without feedback. This activity gives you the two scripts — `ImpactSound` and `SfxPool` — you will drop onto every interactable in Realm of Legends.

As in 6.1 and 6.2, the scene is fully testable with zero downloads: missing clips fall back to a synthesized tick, pitched differently per material.

*Elaria hook:* the caretaker of the ruins hands you three relics and says nothing. "Weigh them," is all the Guide offers. In a world without a HUD, the weight of a thing is how it sounds when it lands — and how it kicks in your hand.

## Learning goals

- You can wire sound to XRI select events and to `OnCollisionEnter`, and map impact speed to volume with `InverseLerp` and an `AnimationCurve`.
- You can explain repetition fatigue and defeat it with pitch variation and clip pools.
- You can implement a fixed-size object pool with idle-first, round-robin-steal selection, and say why pooling matters on a mobile headset.
- You can send a guarded haptic impulse through XRI's `HapticImpulsePlayer` and describe what "multisensory reinforcement" adds to a grab.
- You can debounce a burst of physics contacts with a cooldown.

## Before you start (10 min)

1. Open the project folder (`Activity-6.3-sound-of-touch`) with Unity **6000.5.x**, wait for packages, accept the Input System restart if asked.
2. **Window → Package Manager → XR Interaction Toolkit → Samples**: make sure **Starter Assets** and **XR Interaction Simulator** are imported.
3. **Edit → Project Settings → XR Plug-in Management**: check **OpenXR** on the desktop tab.
4. **Activity → Check Setup** (two `OK` lines), then **Activity → Build Starter Scene**. `Activity_6_3` opens.
5. Press **Play** with headphones on. You stand in front of a table with three relics; a gong hangs behind it. Nothing makes a sound yet — but drop something and watch the Hierarchy: Unity's *One shot audio* objects appear and vanish. That is the naive fallback the pool will replace.

Simulator controls that matter today: **Tab** cycles head → left controller → right controller. With a controller selected, **W A S D** and the mouse move and aim it; the on-screen panel shows the **Grip** and **Trigger** keys — grip grabs a relic with the Near-Far Interactor, grip again releases. To *throw*, move the controller quickly while releasing. The pool readout floats at the left; the Console shows haptic logs.

## VR theory

**Feedback loops.** Every interaction is a loop: the player acts, the world responds, the player perceives the response and adjusts. In VR there is no controller rumble on the table, no cursor change, no "click" from a physical button — unless you add them. Sound is the cheapest, fastest response you can give: it arrives within a frame, works outside the field of view, and requires no gaze. Braun and Rizzo's Chapter 7 in *XR Development with Unity* treats audio as a component of feedback rather than decoration; that is the stance here.

**Multisensory reinforcement.** When two senses agree about one event — a click you hear *and* a tick you feel at the moment your hand closes — the brain fuses them into a single, more certain perception. Grabs feel more "real", and players make fewer accidental drops because they know when they have hold. The rule of thumb is *coincidence*: haptic, sound and visual must land within about 20–40 ms of each other, or the event splits into two. That is why `HapticPulse` fires in the same callback as the grab sound rather than a frame later.

**Haptics on Quest 3.** Each controller has a linear actuator that plays an impulse of some amplitude (0–1) for some duration. XRI 3 exposes it through a `HapticImpulsePlayer` component on each controller object; `SendHapticImpulse(amplitude, duration)` queues one impulse. Short and sharp (0.03–0.08 s) reads as a tap; long and steady reads as a phone buzzing in a pocket, which is almost never what you want. With the desktop simulator there is no motor, so the call is a harmless no-op — which is why every call in this activity is guarded and logged.

**Variation vs repetition fatigue.** A footstep sample repeated forty times is heard as a machine, even if it is a perfect recording. Three cheap tools fix it. **Pitch randomization** of ±5 % changes the perceived size of the object slightly and de-correlates repeats. **Round-robin or random clip selection** among 2–4 recordings of the same material breaks the exact-repeat pattern. **Velocity-scaled volume** makes each hit reflect how hard it was, which ties the sound to the physics and makes it informative rather than decorative. Together they are how AAA foley works, and they cost nothing at runtime.

**Object pooling.** `AudioSource.PlayClipAtPoint` creates a GameObject with an `AudioSource`, plays, and destroys it. On a desktop you never notice. On a Quest running at 72 Hz, creating and destroying objects mid-frame allocates garbage, and the collector eventually stalls a frame — a visible hitch in VR. A **pool** allocates a fixed set of voices once and reuses them. Two design questions follow: **how many** (enough for the busiest moment you expect — a handful of hands, a few bounces; 8 is generous for one player) and **what to do when all are busy** (steal the oldest: a truncated tail is less noticeable than a missing hit).

**Debouncing physics.** A single drop produces several `OnCollisionEnter` calls within a few milliseconds as the object settles. A cooldown — ignore contacts for ~80 ms after playing one — turns the burst into one sound. The cooldown is also your friend against the 0.1 m/s rattles a resting object emits when physics jitters; the minimum-speed threshold handles those.

## Math foundation

**Impact speed to volume.** Let $v$ be the closing speed from `Collision.relativeVelocity.magnitude`. Normalize it between a floor and a ceiling:

$$s = \text{InverseLerp}(v_{min}, v_{max}, v) = \text{clamp}\left(\frac{v - v_{min}}{v_{max} - v_{min}},\ 0,\ 1\right)$$

then shape it: $\text{volume} = \text{curve}(s)$. With an ease-in-out curve, soft taps are pushed toward silence and hard hits toward full volume.

Worked example: a stone dropped from table height falls $h = 0.9$ m; ignoring drag, impact speed $v = \sqrt{2gh} = \sqrt{2 \times 9.81 \times 0.9} = 4.2$ m/s. With $v_{min} = 0.3$ and $v_{max} = 4$, $s = 1.0$ → full volume. A 20 cm drop gives $v = 1.98$ m/s, $s = 0.45$, and an ease-in-out curve returns about 0.43. A resting jitter of $0.1$ m/s gives $s = 0$ → skipped.

**Pitch variation.** `pitch` multiplies playback speed and therefore frequency: pitch $p$ shifts every frequency by a factor $p$. A semitone is $2^{1/12} = 1.059$, so a range of $[0.95, 1.05]$ is a little under ±1 semitone — audible as variation, not as a different note. Playback length also scales by $1/p$.

**Pool sizing.** If sounds average $L = 0.4$ s and the busiest second produces $R = 12$ impacts (three bouncing objects), the expected number of simultaneous voices is $R \cdot L = 4.8$; a pool of 8 leaves headroom for grab/release clicks. With $N$ voices and round-robin stealing, a voice survives at least $N / R = 0.67$ s before it can be stolen — longer than $L$, so in practice nothing is cut.

**Haptic amplitude.** Reuse the same $s$ from the impact: amplitude $= s \cdot a_{max}$, duration fixed at 50 ms. Two channels from one number keeps them coincident by construction.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_6_3.unity` with:

- **Floor** and three static **Workshop Walls** (10 × 8 m room) so throws do not disappear.
- **XR Origin (XR Rig)** at the origin with the **XR Interaction Simulator**.
- **SFX Pool** — carries `SfxPool` (pool size 8, placeholder 700 Hz) with a **Pool Readout** label showing busy voices, requests and steals.
- **Table** — a 1.6 × 0.8 m oak top at 0.9 m.
- **Props** — three grabbables, each with a `Rigidbody` (continuous collision), `XRGrabInteractable` (Velocity Tracking, Throw On Detach), `HapticPulse`, and `ImpactSound` with `Haptics` wired: **Shard** (0.3 kg, placeholder pitch 1.8), **River Stone** (0.8 kg, 0.7), **Rune Tablet** (1.2 kg, 1.1).
- **Gong** — a bronze disc on posts at 3.2 m, with a *kinematic* `Rigidbody` and its own `ImpactSound` (placeholder pitch 0.35) so it also sounds when struck.
- **Welcome Sign** and a **Table Sign**.

Scripts live in `Assets/Scripts/`. `ImpactSound` and `HapticPulse` already subscribe to the grab interactable's `selectEntered`/`selectExited`; you write what happens inside.

## Your tasks (about 70 min)

**Task 1 (15 min) — Build the pool** · file: `Scripts/SfxPool.cs`, TODO 1–3
In `Awake()` create `poolSize` child GameObjects, each with a configured 3D `AudioSource`. In `Play()` pick an idle voice (or steal round-robin), move it to the position, set pitch, and `PlayOneShot`. Remove the placeholder `voice = voices[...]` line.
**Check:** in Play mode, **SFX Pool** has eight children *Voice 1..8*; dropping a prop no longer creates *One shot audio* objects; the readout counts requests and shows 1–3 voices busy.

**Task 2 (10 min) — Grab and release click** · file: `Scripts/ImpactSound.cs`, TODO 1
Call `PlaySfx(grabClip, transform.position, 0.8f)` on grab and `PlaySfx(releaseClip, transform.position, 0.6f)` on release.
**Check:** grip on the shard: one tick; grip again: a quieter tick. Shard, stone and tablet tick at different pitches (the placeholder pitch per material).

**Task 3 (15 min) — Impacts that mean something** · file: `Scripts/ImpactSound.cs`, TODO 2–3
Read `collision.relativeVelocity.magnitude`, skip if below `minImpactSpeed` or inside the cooldown, map through `InverseLerp` and `volumeCurve`, pick a random clip, play at the first contact point, and implement `RandomPitch()`.
**Check:** lowering the stone onto the table is nearly silent; dropping it from chest height is loud; throwing it at the gong makes both the stone and the gong sound. Five drops, five slightly different pitches. Temporarily set `cooldownSeconds` to 0 and hear the machine-gun.

**Task 4 (10 min) — Feel it** · file: `Scripts/HapticPulse.cs`, TODO 1–3 · `Scripts/ImpactSound.cs`, TODO 4
On grab, fetch the `HapticImpulsePlayer` from the interactor's parents and pulse. Implement `Pulse()` with the rate limit and the desktop log. In `ImpactSound`, call `haptics.Pulse(v, 0.05f)` on impacts while held.
**Check:** on desktop the Console prints one `[Haptics]` line per grab and per held impact, never more than ~20 per second. On the headset (see *Port to Quest 3*) the grabbing hand ticks on grab and buzzes harder for harder taps. *(verify on headset)*

**Task 5 (10 min) — Tune the curve** · no new code
Select **River Stone**, open `Volume Curve` in the Inspector, and try three shapes: linear, ease-in (quiet until hard), and a curve that plateaus at 0.6 (nothing is ever painfully loud). Set `Max Impact Speed` to 8 on the Shard so only a real throw reaches full volume. Note which felt "right" for each material.

**Task 6 (10 min) — Real foley and screencast** · no new code
Import your downloaded clips into `Assets/Audio` (**Force To Mono**, *Decompress On Load*). On each prop assign a grab clip, a release clip and 2–4 impact variations of the right material (glass/crystal for the shard, stone for the stone and gong, wood or clay for the tablet). Record: grab and release each relic, a gentle set-down, a hard drop, a throw at the gong, the pool readout, and the haptic log.

## Stretch goals

- Material pairs: give `ImpactSound` a `PhysicsMaterial`-style *material name* and pick the clip set by *both* materials (stone-on-wood vs stone-on-bronze) using `collision.collider.GetComponent<ImpactSound>()`.
- Slide sounds: in `OnCollisionStay`, play a looping scrape whose volume follows the rigidbody's tangential speed and stops when it rests.
- Haptic "texture": while holding the rune tablet, pulse gently at 0.1 amplitude every 0.15 s as the hand moves faster than 0.5 m/s, so it feels rough.

## Assets you will need

See `Docs/FreeAssets.md`. Download into `Assets/Audio`:

- **Kenney Audio → *Impact Sounds*** (CC0): a set of short wood, metal, glass and stone impacts — take 3–4 variations each of *glass*, *stone/rock* and *wood*.
- **Kenney Audio → *Interface Sounds*** or *UI Audio*: two short clicks for grab and release (the grab one slightly brighter).
- **Freesound** (License CC0): search "gong hit" for one 2–4 s gong strike; optionally "stone scrape loop" for the slide stretch goal.
- **Pixabay Sound Effects**: "glass clink" and "rock impact" as alternatives, no attribution required.

Trim silence from the start of every impact clip (Audacity is free) — even 20 ms of leading silence separates the sound from the haptic and breaks the fusion.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. Haptics only work here: confirm each controller object in the rig has a **Haptic Impulse Player** component (the Starter Assets rig includes one; if not, add it and, if XRI asks, its Haptic Impulse Provider), then grab a relic — you should feel the tick. If you feel nothing, check the Console on the device (adb logcat or the XR Interaction Debugger) for the `[Haptics]` lines: "desktop no-op" on a headset means `GetComponentInParent` did not find the player. Keep pool size at 8 and *Max Real Voices* at 32.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: eight pooled voices in the Hierarchy, gentle vs hard impacts, a throw at the gong, the pool readout, and the `[Haptics]` Console lines (or, if you built to the headset, you saying what you felt).
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), and the curve shape you chose for each material.
- (optional) your import settings for the impact clips and which Kenney/Freesound packs you used.

## Troubleshooting

| Symptom | Fix |
|---|---|
| No sound at all | `SfxPool.Instance` is null — the **SFX Pool** object is missing or disabled; rebuild the scene. Also check the Game view mute toggle |
| *One shot audio* objects keep appearing | TODO 1 not done (voices array is null) so `Play` falls back to `PlayClipAtPoint` |
| Machine-gun rattle on every drop | Cooldown is 0 or TODO 2 skipped the `lastImpactTime` check |
| Resting objects tick forever | `minImpactSpeed` too low; 0.3 m/s is a good floor. Also make sure Rigidbody interpolation is on |
| Objects fall through the table when thrown | Rigidbody *Collision Detection* must be *Continuous Dynamic*; the builder sets it — check you did not replace the component |
| `[Haptics] desktop no-op` on the headset | The interactor's parents have no `HapticImpulsePlayer`; add one to each controller object in the rig |
| Cannot grab with the simulator | Select a *controller* with Tab (not the head), aim the ray at the prop, press the Grip key shown in the simulator panel |

Created by Isac Artzi

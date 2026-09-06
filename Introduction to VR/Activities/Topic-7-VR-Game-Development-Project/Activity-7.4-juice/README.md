# Activity 7.4 — Juice: Feedback and Polish

Topic 7: VR Game Development Project · Week 14, Thursday · Stepping stone to Milestone 7 — Final Integration

## Where this fits

Your game works. Tuesday's heatmaps told you where testers go; today you make what they do there *feel* like something. "Juice" is the layer of feedback that confirms every action through several senses at once — sparks, a flash, a swell of scale, a chime, a word in the air — and it is the cheapest way to make a student project feel finished. By the end of class every shard in the clearing will react to being grabbed with five simultaneous effects, and taking the third shard will trigger the scene's one big moment: a second of anticipation, then the Mysterious Guide arriving on stage. You will drop the same `PickupBurst` and `TweenScale` components onto your real collectibles for Milestone 7, and `WorldToast` replaces every piece of screen-space text you were tempted to write.

*Elaria hook:* the shards remember the hands that lift them. Each one sings a note — E, G, B — and when the chord is complete the mist parts and the Guide, who was always there, lets you see her.

## Learning goals

- You can explain "game feel" as multisensory agreement and name the anticipation → action → reaction structure of a satisfying moment.
- You can drive a scale animation from an `AnimationCurve` and shape it in the Inspector without code changes.
- You can write ease-in, ease-out, and ease-out-back functions and say which reads as which.
- You can configure a one-shot `ParticleSystem` and a decaying `Light` from code, and synthesize a short audio clip with `AudioClip.Create`.
- You can explain why screen-space effects (flashes, vignettes, floating text glued to the view) are wrong in VR and place feedback in the world instead.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-7.4-juice`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup** — two `OK` lines.
6. **Activity → Build Starter Scene** → `Activity_7_4` opens. It is dusk in the clearing on purpose: flashes read better against a dim scene.
7. Press **Play**. Three shards glow on pedestals; the far end of the clearing is empty (the Guide is hidden). Turn your speakers on — the chimes are generated in code, so there is nothing to import.

Simulator controls that matter today: **Tab** to a controller, **W A S D** + mouse to move it onto a shard, **G** to grab. Script keys: **J** fires every shard's pickup effects at once (for tuning without grabbing), **R** triggers the Guide's reveal directly.

## VR theory

**Game feel, or "juice".** A grab that only moves the object is *correct*; a grab that also sparks, flashes, swells, and rings is *felt*. The difference is feedback density: several senses confirming the same event within about 100 ms. Below that window the brain fuses them into one cause. Spread the same effects over half a second and they read as five separate, weaker events. That is why `PickupBurst.Play()` fires everything in the same frame and why the durations are short (0.3–0.5 s): juice is a burst, not an ambience. Braun and Rizzo's Chapter 9 treats polish as a project phase in its own right — the one most teams run out of time for, which is why today's components are drop-in.

**Anticipation → action → reaction.** Animators have known for a century that a movement reads better if something precedes it (the wind-up) and something follows (the settle). For the Guide's reveal, the *anticipation* is a light swelling over a second and a low hum — it points every player's attention at the right spot before anything happens. The *action* is the Guide appearing. The *reaction* is the overshoot of her scale, the burst, the chord, and the light easing back down. Skip the anticipation and testers say "a capsule appeared"; include it and they say "the Guide arrived".

**Easing curves** are the grammar of motion. *Ease-in* (slow start, fast finish, $t^3$) is for build-ups and things gathering energy. *Ease-out* (fast start, gentle stop, $1 - (1-t)^3$) is for things arriving and settling — most feedback uses it. *Ease-out-back* overshoots the target before settling — it reads as weight and elasticity, and it is what makes a scale punch feel alive. Unity's `AnimationCurve` lets you draw any of these; today you use a curve for the punch (so you can tune it by dragging keys) and closed-form functions for the rest (so you can see the math).

**No screen-space effects in VR.** On a monitor, a white flash across the screen or "+1" text sliding up from the corner is standard. In a headset the screen *is the player's eyes*: a full-view flash is a physical shock, a vignette is a comfort tool (not a celebration), and text glued to the view forces the eyes to refocus and hides the world. Every effect today lives in the world: the flash is a point light *at the shard*, the text rises *from the shard* and faces you, the particles are *there*. This is a rule you can apply to your entire project: if an effect would be drawn on the camera, move it to the object.

**Procedural audio** (a sine wave with an exponential envelope) is not a replacement for designed sound — Topic 6 gave you Freesound and Kenney — but it is the fastest way to prove a feedback channel and it teaches what a "ting" *is*: a pure pitch that decays. Each shard gets a different note so that three pickups spell a chord.

## Math foundation

**Ease-out cubic** (rise of the toast, decay of the flash), for $t \in [0,1]$:

$$e_{\text{out}}(t) = 1 - (1 - t)^3$$

At $t = 0.5$: $1 - 0.125 = 0.875$ — 87 % of the way there at half time. The flash uses the mirror image, $(1-t)^3$: intensity $= I_{\text{rest}} + (I_{\text{peak}} - I_{\text{rest}}) (1-t)^3$. With $I_{\text{rest}} = 1$, $I_{\text{peak}} = 6$ and $t = 0.5$: $1 + 5 \times 0.125 = 1.63$ — most of the flash is gone in the first quarter second, and the tail lingers.

**Ease-in cubic** (the anticipation swell): $e_{\text{in}}(t) = t^3$. At $t = 0.5$ only $0.125$ — nothing seems to happen for the first half, then it surges. That withheld energy is the anticipation.

**Ease-out-back** (the Guide's grow-in), with $u = t - 1$ and the classic constants $c_1 = 1.70158$, $c_3 = c_1 + 1 = 2.70158$:

$$s(t) = 1 + c_3 u^3 + c_1 u^2$$

Worked example at $t = 0.7$: $u = -0.3$, $u^2 = 0.09$, $u^3 = -0.027$; $s = 1 - 0.0729 + 0.1531 = 1.080$ — the Guide is 8 % too large, then settles to exactly 1 at $t = 1$ ($u = 0$). At $t = 0$: $u = -1$, $s = 1 - 2.70158 + 1.70158 = 0$ — she starts from nothing.

**The chime.** A sine at frequency $f$ Hz with an exponential envelope over duration $D$:

$$y(t) = 0.6 \sin(2\pi f t)\, e^{-6t/D}$$

At $t = D$ the envelope is $e^{-6} \approx 0.0025$, so the clip ends silently — no click. At 44 100 samples/s a 0.35 s clip is 15 435 floats. The three notes E5, G5, B5 (659.25, 783.99, 987.77 Hz) form an E-minor chord; the reveal's chord uses A4 = 440 Hz and the hum A2 = 110 Hz — two octaves below.

**The punch curve.** Keys at $(0, 1)$, $(0.15, 1.35)$, $(0.5, 1)$: the object reaches 135 % of its size in 15 % of the duration (75 ms at $D = 0.5$ s) and is back at 100 % by 250 ms. Drag the middle key to 1.6 and it becomes cartoonish; to 1.1 and it becomes a shiver.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_7_4.unity` with:

- A dusk clearing: 30 m **Floor**, twelve **Trees**, dim sun, blue-grey fog; the **XR Origin (XR Rig)** with the simulator at the origin.
- **Shard 1–3** on pedestals, each with `Rigidbody`, `XRGrabInteractable`, `TweenScale` (punch curve set), an `AudioSource` (3D), a **Flash Light** child, a **Burst** child (`ParticleSystem`: 40 particles, one burst, sphere emitter, fades over 0.8 s), and `PickupBurst` with every reference set and a different *Chime Hz* (E5, G5, B5).
- **Toast** — a `TextMesh` carrying `WorldToast`, shared by everything.
- **Mysterious Guide** — a capsule at 6 m with `TweenScale`; its renderer is disabled at start by the director.
- **Reveal Light** (with an `AudioSource`) and **Reveal Burst** (120 particles) at the Guide's position.
- **Director** — carries `GuideReveal` with the Guide's renderer, tween, light, burst, audio, toast and the **Status Sign** wired.

Scripts live in `Assets/Scripts/`: `PickupBurst.cs`, `TweenScale.cs`, `WorldToast.cs`, `GuideReveal.cs`.

## Your tasks (about 70 min)

**Task 1 (10 min) — The punch** · file: `Scripts/TweenScale.cs`, TODO 1–2
Implement `Punch` (stop any running punch, reset scale, start the routine) and `PunchRoutine` (advance `t`, `localScale = baseScale * curve.Evaluate(t)`, finish at `baseScale`).
**Check:** in Play mode, right-click the `TweenScale` header on a shard → **Punch**. It swells and settles in half a second. Drag the curve's middle key higher and punch again.

**Task 2 (15 min) — Everything at once** · file: `Scripts/PickupBurst.cs`, TODO 1–3
Subscribe to `selectEntered` in `Start`, fire all channels in `Play()`, and implement the cubic flash decay in `Flash()`.
**Check:** press **J** — all three shards spark, flash, and punch together and the toast placeholder appears. Grab one with a controller — only that shard fires, and only on the first grab.

**Task 3 (10 min) — The chime** · file: `Scripts/PickupBurst.cs`, TODO 4
Fill the sample buffer with the decaying sine from the Math section.
**Check:** pickups ring; the three shards are audibly different notes; nothing clicks at the end of the sound. Move the head away from a shard and grab it with a far-out controller — it is quieter (3D audio).

**Task 4 (15 min) — Words in the world** · file: `Scripts/WorldToast.cs`, TODO 1–3
Implement `Show` (restart, set text, start the routine), `Routine` (ease-out rise, alpha from the curve, face the camera every frame), and `FaceCamera`.
**Check:** grab a shard from behind its pedestal — "Shard 2 taken!" rises 30 cm from the shard, faces you, and is gone in 1.2 s. It never appears glued to your view.

**Task 5 (15 min) — Anticipation and reveal** · files: `Scripts/GuideReveal.cs`, TODO 1–3 · `Scripts/TweenScale.cs`, TODO 3
Count pickups in `NotifyPickup`; in `Reveal` swell the light with an ease-in over 1.2 s (with the 110 Hz hum), then in one frame enable the Guide's renderer, call `GrowFromZero`, play the burst and the 440 Hz chord, show the toast, and ease the light back. Implement `GrowRoutine` with the ease-out-back formula.
**Check:** press **R** — a second of brightening, then the Guide pops in slightly too large and settles as sparks fly. Restart and grab all three shards instead — the third grab triggers the same moment and the sign reads *3 / 3*.

**Task 6 (5 min) — Tune and record** · no new code
Set one shard's *Flash Intensity* to 20 and its *Flash Seconds* to 1.5; notice how it stops reading as "pickup" and starts reading as "explosion". Set the punch curve's peak to 1.1. Pick values you like, then record the screencast: one grab close up, then the reveal.

## Stretch goals

- Haptics: in `PickupBurst.OnGrabbed`, find `args.interactorObject.transform.GetComponentInParent<HapticImpulsePlayer>()` (namespace `UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics`), null-check, and call `SendHapticImpulse(0.6f, 0.1f)` — verify on the headset.
- Randomize the chime by ±3 % pitch per pickup (`audioSource.pitch = Random.Range(0.97f, 1.03f)` before `PlayOneShot`) so repeated pickups do not sound like a sampler.
- A `TweenScale` on the **Status Sign** that punches every time the count changes.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. Particles and point lights are the two effects most likely to cost frame time on the Quest: keep bursts under ~150 particles, make sure the flash lights do not cast shadows (they do not, by default), and never leave a point light at intensity 8 permanently — the reveal light must ease back down. If you added the haptic stretch, this is where you feel it.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: one shard grabbed close up (sparks, flash, punch, chime, toast), the toast facing you from a different angle, and the full reveal after the third shard.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), and the two tuning values you changed and why.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Particles are pink/magenta squares | The particle material could not be found; assign *Default-ParticleSystem* (or any Particles shader material) to the **Burst** child's renderer |
| No sound at all | `MakeChime` still returns silence (TODO 4), the Game view is muted (speaker icon in its toolbar), or `audioSource` is null on that shard |
| Punch ends at the wrong size | Two punches overlapped — `Punch()` must stop the running coroutine and reset to `baseScale` first |
| Toast is mirrored or edge-on | `FaceCamera` used `cam.position - transform.position`; flip the subtraction. If edge-on, `away.y` was not zeroed |
| Guide never appears | `NotifyPickup` is not incrementing (TODO 1), or *Pieces Needed* is higher than the number of shards |
| Reveal light stays bright | The ease-back after the reaction is missing — mirror the `Flash()` decay |

Created by Isac Artzi

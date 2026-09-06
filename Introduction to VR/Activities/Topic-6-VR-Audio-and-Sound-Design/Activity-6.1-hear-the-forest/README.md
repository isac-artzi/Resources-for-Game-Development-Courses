# Activity 6.1 — Hear the Forest: Spatial Audio

Topic 6: VR Audio and Sound Design · Week 11, Tuesday · Stepping stone to Milestone 6 — Sound Design

## Where this fits

By the end of class you will stand in an Enchanted Forest clearing you can navigate with your ears: a stream murmurs to your left, a bird calls from a branch up to your right, a fire crackles ahead, and something hidden behind the bushes swells in volume whenever you turn toward it. Every one of those sounds is a 3D `AudioSource` whose distance settings you can *see*, because you will draw its Min and Max Distance as rings on the ground and print the attenuation the listener should be hearing. You will also write a small audit that confirms the scene has exactly one `AudioListener` and that it rides on the player's head. Milestone 6 asks for background music, ambience, effects and spatial audio in all three environments; this activity is where you learn the three or four numbers on an `AudioSource` that decide whether a sound feels like it is *there* or merely playing.

The scene works with zero downloads: a `ToneFactory` script synthesizes placeholder tones and noise for any source whose Clip field is empty. Swap in real recordings during the last task and hear the difference.

*Elaria hook:* the Mysterious Guide has gone quiet. "Close your eyes," a voice says. "The forest will tell you where the shard fell." You cannot see it from the path. You can hear it.

## Learning goals

- You can configure a 3D `AudioSource` (spatial blend, rolloff mode, min/max distance, volume, doppler) and predict from those numbers how loud it is at a given distance.
- You can explain how humans localize sound (interaural time and level differences, spectral cues) and why Unity's default panning is not the same as a spatializer.
- You can compute Unity's logarithmic attenuation and convert a gain to decibels, and you know that halving amplitude is about −6 dB.
- You can use the dot product of two direction vectors to measure how much the player is facing a target.
- You can find every `AudioListener` in a scene from code and state why there must be exactly one, on the camera.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-6.1-hear-the-forest`). Open it with Unity **6000.5.x** and wait for packages to resolve.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager**, dropdown *In Project*, select **XR Interaction Toolkit**, open **Samples**, and import **Starter Assets** and **XR Interaction Simulator** if they are not already imported.
4. **Edit → Project Settings → XR Plug-in Management**: on the desktop tab check **OpenXR**.
5. **Edit → Project Settings → Audio**: leave *Spatializer Plugin* on *None* for now (we will discuss it) and confirm *Default Speaker Mode* is *Stereo*. Put headphones on — spatial audio is nearly meaningless on laptop speakers.
6. Menu bar → **Activity → Check Setup** — expect two `OK` lines. Then **Activity → Build Starter Scene**. `Activity_6_1` opens.
7. Press **Play**. You hear three placeholder sounds at once: a low hum on the left (the stream), a high tone up-right (the bird), a mid tone ahead (the fire), and a pulsing tone somewhere behind you. The Console lists which sources received a placeholder clip.

Simulator controls that matter today: **Tab** to select the head, **mouse** to turn it (turning is the whole point — spatial audio is about what happens when you rotate), **W A S D** to walk, and the simulator's vertical keys to duck under the bird. Keep the Scene view open next to the Game view so you can watch the rings while you move.

## VR theory

**Spatial audio** is any technique that makes a sound appear to come from a location in 3D space rather than from the speakers. In VR it carries more than mood: it is how players find things outside their narrow field of view, how they judge distance in the dark, and a large part of why a headset feels like a *place*. Braun and Rizzo cover Unity's audio pipeline in Chapter 7 of *XR Development with Unity*; this section fills in the perceptual side.

**How you localize a sound.** Your brain uses three families of cues. The **interaural time difference (ITD)** is the delay between a wavefront reaching the near ear and the far ear — up to about 0.6–0.7 ms for a sound directly to one side, given a head roughly 17 cm wide and sound at 343 m/s. The **interaural level difference (ILD)** is the loudness difference between the ears caused by the head shadowing high frequencies (above ~1.5 kHz). Together they tell you the *azimuth* — left or right — very well, but they are ambiguous front-to-back and say almost nothing about elevation: a sound straight ahead and one straight behind produce the same ITD and ILD. That is the **cone of confusion**. What breaks it is the third cue: **spectral shaping** by your outer ears, head and shoulders, which colors a sound differently depending on where it comes from. A **head-related transfer function (HRTF)** is a measurement of that coloring for every direction, and a **spatializer** is an audio plugin that applies an HRTF to each source so that sounds gain elevation and front/back cues.

**What Unity does by default.** With *Spatial Blend* at 1 and no spatializer plugin, Unity pans a source between the left and right channels based on its angle and applies your rolloff curve for distance. That gives you ILD-like cues and distance, but no ITD and no HRTF. Front and back sound the same; a bird above you sounds level with you. It is still enormously useful, and it is what we use today because it needs no extra package. On Quest 3 you will typically enable the OpenXR or Meta spatializer in *Project Settings → Audio*, which adds HRTF processing per source without changing your code. Design for both: place sounds so the *head turn* resolves ambiguity, which is exactly what the beacon exercise trains.

**Distance attenuation** is the drop in loudness with distance. Real point sources in open air fall off by the **inverse distance law**: amplitude ∝ 1/d, which means −6 dB every time you double the distance. Unity's **Logarithmic** rolloff is precisely that law between *Min Distance* (inside it, full volume) and *Max Distance* (beyond it, the curve flattens — the sound does *not* stop). **Linear** rolloff goes to silence at Max Distance and is easier to design with for small rooms; **Custom** lets you draw the curve. Min Distance is your main creative control: a big source like a stream deserves a large Min Distance (it stays loud over a wide area); a bird gets a small one so it feels like a point.

**Doppler** is the pitch shift of a moving source (or a moving listener). Unity applies it per source scaled by *Doppler Level*; at 1 it is realistic and mostly harmless, but fast teleports can produce a chirp. A stretch goal today has you move the bird to hear it.

**Audio and presence.** Sound arrives at the brain faster than a rendered frame, is not limited to the field of view, and is processed partly below attention. Sound that matches the visual scene — a fire that crackles from the fire, a stream that gets louder as you approach — reinforces the sense of being there; sound that does not match (a fire that is equally loud everywhere) quietly undermines it. The audit script exists because the single most common way to break spatial audio is to have two listeners, or one at the player's feet.

## Math foundation

**Logarithmic (inverse-distance) attenuation.** For a listener at distance $d$ from a source with Min Distance $d_{min}$:

$$g(d) = \frac{d_{min}}{\max(d,\, d_{min})}$$

so $g = 1$ inside the min ring, $g = 0.5$ at $2 d_{min}$, $g = 0.25$ at $4 d_{min}$. Beyond Max Distance Unity holds the last value. The Inspector *Volume* slider multiplies the whole curve.

**Decibels.** Gain is a ratio of amplitudes; decibels express that ratio on a log scale:

$$L = 20 \log_{10}\left(\frac{A_1}{A_0}\right)$$

Worked example: the fire has $d_{min} = 1.5$ m. At $d = 6$ m the gain is $1.5 / 6 = 0.25$, and $20 \log_{10}(0.25) = 20 \times (-0.602) = -12.0$ dB. Halving amplitude ($0.5$) gives $20 \log_{10}(0.5) = -6.02$ dB — remember "−6 dB per doubling of distance". A tenth of the amplitude is −20 dB. Note that a *power* ratio uses 10 log₁₀; we work with amplitudes, hence 20.

**Facing with the dot product.** For unit vectors $\hat{f}$ (where the head looks) and $\hat{u}$ (toward the beacon), $\hat{f}\cdot\hat{u} = \cos\theta$, where $\theta$ is the angle between them. 1 means straight ahead, 0 means directly beside you, −1 means behind. To get the angle back: $\theta = \arccos(\hat{f}\cdot\hat{u})$, in radians — multiply by `Mathf.Rad2Deg`. Example: if the beacon is 45° off your gaze, the dot product is $\cos 45° = 0.707$; `InverseLerp(-1, 1, 0.707)` maps it to 0.854 on the 0..1 scale the volume curve expects.

**Ring points.** A circle of radius $r$ around a center $c$, sampled at $n$ points: $p_i = c + (r\cos\alpha_i,\ 0,\ r\sin\alpha_i)$ with $\alpha_i = 2\pi i / n$.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_6_1.unity` with:

- **Floor** — a 40 m grass plane, and a ring of fourteen primitive trees about 12 m out.
- **XR Origin (XR Rig)** at the origin with the **XR Interaction Simulator**.
- **Audio Tools** — carries `ToneFactory`, which synthesizes placeholder clips for every source with an empty Clip field.
- **Stream** — a 6 m × 1.2 m water-colored slab at (−7, 0, 4). 3D `AudioSource`, Min 2 m, Max 18 m, plus `RolloffVisualizer` set to a noise placeholder and a **Stream Readout** label.
- **Bird** — a small sphere 4.5 m up a trunk at (6, 4.5, 6). Min 1 m, Max 14 m, `RolloffVisualizer` with a 1320 Hz placeholder and a **Bird Readout**.
- **Fire** — an emissive disc with a warm point light at (0, 0.3, 6). Min 1.5 m, Max 12 m, `RolloffVisualizer` with a noise placeholder and a **Fire Readout**.
- **Lost Shard (Beacon)** — an emissive shard on a pedestal at (8, 1.2, −6), behind a screen of bushes. It carries `AudioBeacon` with its `Glow` light and **Beacon Readout** already assigned.
- **Listener Check** — a label 2.5 m ahead carrying `ListenerCheck`, with its `Sources` array pointed at the three visualizers.
- **Welcome Sign**, fog, and a warm low sun.

Every `AudioSource` is set to *Spatial Blend 1*, *Logarithmic* rolloff, *Loop*, *Play On Awake*, and an **empty Clip**. Scripts live in `Assets/Scripts/`.

## Your tasks (about 70 min)

**Task 1 (10 min) — Draw the rings** · file: `Scripts/RolloffVisualizer.cs`, TODO 1–2
Implement `DrawRing` with the circle formula, then call it for both rings in `Update()` using `source.minDistance` and `source.maxDistance`. Press Play, select **Fire** in the Hierarchy, and drag *Min Distance* on the 3D Sound Settings graph while playing.
**Check:** each source has a yellow inner ring and a blue outer ring on the ground (the bird's float at perch height); the inner ring follows the Min Distance slider live.

**Task 2 (15 min) — Predict the loudness** · file: `Scripts/RolloffVisualizer.cs`, TODO 3–4
Implement `LogarithmicGain` and `ToDecibels`, then fill the readout in `Update()` with distance, gain and dB. Walk to the fire's inner ring, then step back to twice that distance.
**Check:** at the yellow ring you read gain 1.00 / 0.0 dB; at about 3 m from the fire you read 0.50 / −6.0 dB; at 6 m, 0.25 / −12.0 dB. Beyond the blue ring the number stops falling. Close your eyes and compare with what you hear.

**Task 3 (15 min) — The beacon calls** · file: `Scripts/AudioBeacon.cs`, TODO 1–4
Compute the dot product between the head's forward and the direction to the shard, map it through `InverseLerp` and the facing curve to a volume, multiply by the pulse, and drive the glow light with the same pulse. Print the off-axis angle.
**Check:** with your eyes closed (or the Game view covered) you can turn until the pulsing tone is loudest, open your eyes, and the shard is centered in view. The readout says 0–5° when you face it and about 180° when you turn away.

**Task 4 (10 min) — Audit the ears** · file: `Scripts/ListenerCheck.cs`, TODO 1–4
Count listeners with `FindObjectsByType<AudioListener>`, confirm the one you found is on `Camera.main`, print the result and warn in the Console if it is wrong. Then rank the three sources by predicted gain. Break it on purpose: create an empty GameObject, add a Camera component, press Play.
**Check:** normally the label reads *Listeners: 1  OK / On head: yes* and *Loudest:* flips between Fire and Stream as you walk between them. With the extra camera it reads *Listeners: 2  PROBLEM* and Unity's own warning appears in the Console. Delete the extra camera.

**Task 5 (10 min) — Make the stream sound like water** · file: `Scripts/ToneFactory.cs`, TODO 2
Implement `Noise()` with the white-noise loop and the one-pole low-pass, and remove the fallback return. Play again.
**Check:** the stream and fire hiss instead of hum, and — with your eyes closed — they are now *easier* to locate than the sine tones. That is the broadband effect from the theory section.

**Task 6 (10 min) — Real sounds and screencast** · no new code
Import the three loops you downloaded (see *Assets you will need*) into `Assets/Audio`, select each and in the Import Settings tick **Force To Mono** (3D sources should be mono; a stereo file is downmixed anyway and wastes memory), then drag each clip into the matching source's *AudioClip* field. Play. Record your screencast now: walk from the stream to the fire while the readouts change, turn to find the beacon, and show the listener audit.

## Stretch goals

- Move the bird: add a script that flies it in a 4 m circle at 6 m/s and listen for the Doppler shift; then set *Doppler Level* to 0 and compare.
- Enable a spatializer: **Edit → Project Settings → Audio → Spatializer Plugin** (pick whichever the OpenXR package offers), tick *Spatialize* on the bird's `AudioSource`, and note in your Padlet bullets whether elevation became audible.
- Add the second harmonic in `ToneFactory.Tone` (TODO 1) and give the beacon a *minor third* instead: play two tones at 660 Hz and 792 Hz through two `AudioSource`s on the shard.

## Assets you will need

See `Docs/FreeAssets.md`. For today, download three short loops and drop them into `Assets/Audio`:

- **Freesound** (filter *License: Creative Commons 0*): search "stream loop" for a 20–30 s water loop, "fire crackle loop" for a 20–30 s fire loop, and "bird chirp" for two or three single chirps (0.5–2 s each; you will use them again in 6.2).
- **Kenney Audio** (kenney.nl → *Audio* packs): the *Nature* or *RPG Audio* packs have clean, CC0 ambience loops if you would rather not sift through Freesound.
- **Pixabay Sound Effects**: search "forest stream" and "campfire" — no attribution required.

Prefer `.wav` or `.ogg`. In each clip's Import Settings set **Load Type** to *Decompress On Load* for clips under 10 s and *Streaming* for anything longer, and tick **Force To Mono** for every clip that will play from a 3D source.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. Audio-specific: in **Project Settings → Audio** on the Android platform set *DSP Buffer Size* to *Good latency* and, if the OpenXR package offers a *Spatializer Plugin*, select it and tick *Spatialize* on the bird and the beacon. On the headset the head-turn cues are physical rather than mouse-driven — the beacon task becomes markedly easier, which is the point.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the rings around the three sources, a readout hitting −6 dB at twice Min Distance, you finding the beacon by ear, and the listener audit (including the "2 listeners" failure).
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), and one sentence on which of the three sources was easiest to locate by ear and why.
- (optional) the three clips you chose, with their Freesound/Kenney/Pixabay source names.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Silence, and the Console shows no `[ToneFactory]` lines | The **Audio Tools** object is missing or `ToneFactory` is disabled; rebuild the scene. Also check the Game view's mute button (speaker icon) is not toggled |
| Every sound is equally loud everywhere | *Spatial Blend* slid back to 0 — set it to 1; or a second `AudioListener` exists (see the Listener Check label) |
| Sounds pan left/right but the bird never sounds "up" | Expected with plain panning; enable a spatializer plugin (stretch goal) or design around it with head-turn cues |
| Rings are ellipses or off-center | `DrawRing` used local space or forgot `transform.position`; the ring must be built in world space around the source |
| Readout shows `-Infinity dB` | `ToDecibels` took log10(0); clamp the gain with `Mathf.Max(gain, 0.00001f)` |
| Beacon readout shows `NaN` | `Mathf.Acos` received a value slightly above 1; clamp the dot product to [−1, 1] |
| Placeholder loop clicks once per second | The stream/fire still use the sine fallback (TODO 2 not done), or `placeholderHz` does not fit whole cycles — `Tone()` rounds it, so pass any value above 20 Hz |

Created by Isac Artzi

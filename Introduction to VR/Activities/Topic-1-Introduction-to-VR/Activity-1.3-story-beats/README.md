# Activity 1.3 — Story Beats in Space

Topic 1: Introduction to Virtual Reality · Week 2, Tuesday · Stepping stone to Milestone 1 — Project Proposal and Concept Development

## Where this fits

Milestone 1 wants a storyline with a beginning, a middle, and an end, a cast of characters, and a concept video that makes someone else want to play *Realm of Legends*. On a page, a storyline is a list. In VR it is a sequence of *places* the player moves through, and the order in which the world draws their eye from one place to the next. Today you stage your storyline as five physical stations around a dusk clearing. A sequencer walks the story forward on a key press: the station for the current beat brightens, the previous one eases down, and the Mysterious Guide turns toward it and speaks its line, typed out at reading speed. When you finish, you will have a walkable storyboard you can narrate over for the concept video, and — more useful still — the habit of asking "where is the player looking, and why?" for every beat you write. The fade and easing code you write today returns in Topic 5 (locomotion comfort) and Topic 7 (polish).

*Elaria hook:* the Guide leads you to a clearing where five stone circles wait in the dusk. "A hero's road is long," she says, "but it is made of moments. Show me yours, one light at a time." As each circle wakes, she turns to it and tells you what will happen there.

## Learning goals

- You can distinguish diegetic from non-diegetic information and explain why the Guide is a diegetic UI.
- You can name three attention-direction cues (light, motion, sound) and point to where each is used in your scene.
- You can write the SmoothStep function by hand, explain why it is preferred to a linear fade, and compute its value at a given time.
- You can implement a data class, drive a sequence of states from an array, and make components talk through public methods.
- You can stage your own five-beat storyline in the scene and narrate it in a screencast.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-1.3-story-beats`). Open it with Unity **6000.5.x**.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup** — two `OK` lines.
6. **Activity → Build Starter Scene**. The scene `Activity_1_3` opens: a dim clearing, five faint stone circles on an arc, the Guide in the middle.
7. Press **Play**, click in the Game view, press **]**. Nothing lights yet — that is the point of today.

Simulator controls that matter today: **Tab** to select the head, **W A S D** / **Q E** to move it, **mouse** to look. The activity's keys, all changeable in the Inspector: **]** next beat, **[** previous beat, **Backspace** restart. Stand still for the first run so you notice the lights; then walk between stations to feel how a lit station reads from 3 m versus 12 m.

## VR theory

**Spatial (environmental) storytelling** is telling the story with the *place*: what is built there, what is broken, where the light falls, what sound leaks from behind a door. In a film the camera chooses what you see; in VR the player owns the camera, so the environment must do the pointing. Your storyline therefore has to be planned as a route through spaces, each beat anchored to a location the player can recognize from a distance and reach. Braun and Rizzo make this point when they contrast screen-based media with XR in the opening chapters of *XR Development with Unity*: the designer trades control of the frame for control of the world.

**Diegetic versus non-diegetic information.** Something is *diegetic* when it exists inside the story world and the characters could perceive it: a torch, a signpost, an NPC's voice, a glowing altar. It is *non-diegetic* when only the player can perceive it: a health bar pasted on the screen, an arrow floating over a quest target, subtitles. Screen-space overlays are painful in VR (they sit at a fixed depth in front of your eyes and fight with everything else) and they break presence. So VR leans diegetic: information is carried by objects and characters in the world. The Guide is your workhorse here — a character who can look, walk, point, and speak is a UI that never feels like one.

**The Guide as diegetic UI.** When she turns toward a station and speaks, three things happen at once: her *motion* draws the eye (peripheral vision is tuned to movement), her *gaze direction* is a pointer humans read effortlessly, and her *words* give meaning to what the player then sees. You will implement all three today with a rotation, a typewriter reveal, and a beacon light. In Topic 4 she gains dialogue choices; in Topic 6, a voice.

**Directing attention: light, motion, sound.** Human attention is drawn to contrast: the brightest thing in a dark scene, the moving thing in a still scene, the sound that just started. A beat change in your scene stacks all three — a beacon fades up, the Guide turns, the speech blips — and the previous station fades *down* so there is only one brightest place. Note the order: dim the old station first, then light the new one, because the eye follows the biggest change and you want that change to be the arrival, not the departure.

**Easing.** A light that snaps on is an alarm; a light that eases on is an invitation. Real light sources warm up, curtains open, torches catch. Eased motion also reads as *intentional* — someone did this on purpose — while linear or instant changes read as machinery. In VR, where the whole world is a display, abrupt changes in brightness are also physically uncomfortable in a dark scene. Use `SmoothStep` for fades and `RotateTowards` for turns.

**Beginning, middle, end.** The classic three-act shape maps onto Elaria cleanly: the *beginning* establishes the broken world and the call (forest edge, first shard); the *middle* raises the stakes and cost (mountain, ruins); the *end* resolves it (restoration). Five stations is enough to sketch that shape; your GDD can have more beats, but the video should show these five.

## Math foundation

**Linear interpolation.** `Mathf.Lerp(a, b, t)` returns $a + (b - a)\,t$ for $t \in [0, 1]$. With $a = 0.15$ (dim), $b = 3.0$ (lit) and $t = 0.5$: $0.15 + 2.85 \times 0.5 = 1.575$.

**Normalized time.** A fade of duration $T$ seconds that has run for $e$ seconds has progress $t = e / T$, clamped to $[0, 1]$. `Mathf.Clamp01(elapsed / fadeSeconds)`.

**SmoothStep.** Linear progress moves at constant speed and stops dead. SmoothStep reshapes it:

$$s(t) = 3t^2 - 2t^3$$

$s(0) = 0$, $s(1) = 1$, and the slope $s'(t) = 6t - 6t^2$ is zero at both ends — the fade starts slowly, speeds up in the middle, and settles. Worked example for a 1.2 s fade from 0.15 to 3.0, at $e = 0.3$ s: $t = 0.25$, $s = 3(0.0625) - 2(0.015625) = 0.1875 - 0.03125 = 0.15625$, intensity $= 0.15 + 2.85 \times 0.156 = 0.60$. Linear would already be at $0.15 + 2.85 \times 0.25 = 0.86$. At $e = 0.6$ s both give $1.575$ (SmoothStep is symmetric about the middle). At $e = 0.9$ s, $t = 0.75$, $s = 0.84375$, intensity $2.55$ versus linear $2.29$. `Mathf.SmoothStep(0f, 1f, t)` computes exactly this.

**Typewriter reveal.** Characters shown after $e$ seconds at rate $r$ characters per second: $n = \lfloor r\,e \rfloor$, capped at the line length $L$. The line is complete after $L / r$ seconds: a 90-character line at 30 cps takes 3.0 s; at 60 cps, 1.5 s. Average adult silent reading is 200–300 words per minute, roughly 20–25 characters per second, so 30 cps stays just ahead of the reader without feeling rushed.

**Constant-speed turning.** `Quaternion.RotateTowards(from, to, maxDegrees)` moves at most `maxDegrees` per call. With 90°/s and `Time.deltaTime`, a 135° swing from Station 1 to Station 5 takes 1.5 s regardless of frame rate. Compare with `Slerp(from, to, 0.1f)` every frame, which is fast at first, slows forever, and depends on frame rate — avoid it for anything you want to time.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_1_3.unity` with:

- **Floor** — 40 m of dark ground, dusk lighting, blue-grey fog, low ambient light so the beacons dominate.
- **XR Origin (XR Rig)** 1 m behind the origin, with the **XR Interaction Simulator**.
- **Stations** — five `Station N` roots on an arc about 8 m away, left to right. Each has a 3.5 m stone **Disc**, a **Beacon** point light 3.5 m up (dim, tinted per environment), a **Title Label** 2.7 m up, and a few primitives dressing it: forest edge with trees and a boulder; a pedestal with a shard among trees; a 15° ramp against a rock face; four pillars and a lintel; an altar with three shards. Each root carries `BeatStation` with *Beacon* and *Title Label* assigned.
- **Mysterious Guide** — an empty at the arc's center carrying `GuideVoice` and an `AudioSource` (no clip yet). Children: **Body** (a 2 m capsule with a small **Nose** so you can see which way she faces), **Guide Glow**, and **Speech Label** (empty until she speaks).
- **Story Sequencer** — an empty carrying `StoryBeatSequencer`, its *Guide* set and its *Beats* filled with a sample five-beat Elaria storyline pointing at Stations 1–5. You will replace the text with your own.
- **Welcome Sign** with the keys.

Scripts live in `Assets/Scripts/`: `StoryBeat.cs` (data), `BeatStation.cs`, `StoryBeatSequencer.cs`, `GuideVoice.cs`.

## Your tasks (about 70 min)

**Task 1 (10 min) — Prime and step** · file: `Scripts/StoryBeatSequencer.cs`, TODO 1–2; `Scripts/StoryBeat.cs`, TODO 2
In `Start()`, loop over `beats`, write each title into its station with `SetTitle` and dim it with `SetActive(false)`. In `Update()` read **]**, **[** and **Backspace** through `Keyboard.current[key].wasPressedThisFrame` and call `Advance(+1)`, `Advance(-1)`, `Restart()`. Implement `StoryBeat.Summary()` so it returns `"[Act] Title"`.
**Check:** on Play the five labels show the sample titles ("The Awakening" …), and pressing **]** prints `[Story] advance requested -> 0` in the Console.

**Task 2 (15 min) — Light one station at a time** · file: `Scripts/StoryBeatSequencer.cs`, TODO 3; `Scripts/BeatStation.cs`, TODO 1–2
In `Advance`, dim the station of the beat you are leaving, update `CurrentIndex`, light the new station, and log `beat.Summary()`. In `BeatStation.SetActive`, capture the *current* intensity and alpha as the fade's start, pick the targets from `active`, and reset `elapsed`. In `Update`, apply `Lerp(from, to, s)` to the beacon intensity and the label alpha — `s` is still the constant 1, so for now the change is instant.
**Check:** **]** makes Station 1's beacon jump to full brightness (no fade yet) and its label to full alpha; **]** again lights Station 2 and Station 1 drops back to a faint glow. Exactly one station is bright at any time. The Console reads `[Beginning] The Awakening`, then `[Beginning] The First Shard`, and so on.

**Task 3 (15 min) — Ease the fades** · file: `Scripts/BeatStation.cs`, TODO 3
Replace the constant `s` with real progress: `t = Clamp01(elapsed / fadeSeconds)`, eased with `Mathf.SmoothStep(0, 1, t)`, computed before the `Lerp`s from Task 2.
**Check:** a beat change now takes about 1.2 s; the light "settles" rather than stopping. Set *Fade Seconds* to 3 on one station and it visibly lags the others. Press **]** twice fast — no flash, the half-finished fade just changes direction.

**Task 4 (15 min) — The Guide speaks and turns** · file: `Scripts/StoryBeatSequencer.cs`, TODO 4; `Scripts/GuideVoice.cs`, TODO 1–4
Call `guide.Say(beat.guideLine, beat.station.transform)` from `Advance`. In `GuideVoice`, reset the typewriter in `Say`, reveal `Substring(0, n)` characters in `Update` with `n = floor(elapsed × charactersPerSecond)`, turn the Guide toward the target with `Quaternion.RotateTowards` on the flattened direction, and in `LateUpdate` keep the speech label above her and facing the head.
**Check:** each **]** types out the beat's line above the Guide at about 30 characters per second while she swings to face the lit station (watch the Nose). Walk around her: the text faces you and stays upright. Drag any short click sound onto the Guide's `AudioSource` *AudioClip* — she blips every four characters.

**Task 5 (15 min) — Your storyline, and the recording** · no new code
Select **Story Sequencer**. In *Beats*, replace the five sample titles and Guide lines with the beginning, middle, and end of *your* team's storyline (two Beginning beats, two Middle, one End is a good split; set *Act* accordingly). Keep each Guide line under about 120 characters so it finishes in four seconds. Optionally rename the station objects and shuffle a few primitives so each station hints at your environment. Record your screencast walking through all five beats and saying one sentence per beat about what the player *does* there.
**Check:** five beats, five lit stations, five lines in your own words, and the Console showing the acts in order Beginning → Middle → End.

## Stretch goals

- Auto-play: add `holdSeconds` to `StoryBeat` (its TODO 1) and implement TODO 5 in the sequencer so the story advances by itself when *Auto Advance* is ticked — the hands-free mode for your concept video.
- Make the title labels billboard toward the head (reuse the pattern from `GuideVoice` TODO 4) so they read from anywhere in the clearing, and scale the *active* station's shard or altar up by 10% with the same eased `t`.
- Add a *sound* cue: give each `BeatStation` an `AudioSource` with a chime and call `Play()` inside `SetActive(true)`. Then, in the Padlet post, say which of light, motion, and sound you noticed first.

## Port to Quest 3

Nothing here needs the headset. Disable the **XR Interaction Simulator**, follow `Docs/PortingToQuest3.md`, and *Build And Run*. Without a keyboard the sequencer needs a controller button: add an `InputActionReference` field to `StoryBeatSequencer`, bind it to a button in the Starter Assets input actions, `Enable()` it in `Start`, and check `action.action.WasPressedThisFrame()` next to the keyboard test. In the headset, stand in the middle of the arc and notice how much stronger the beacon cue is when the lit station is in your periphery than when you are already looking at it.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: all five beats advancing with fades, the Guide turning and speaking each line, and your own storyline text in place.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus one sentence on which attention cue (light, motion, sound) you found most effective and why.
- The five beat titles and Guide lines as plain text — paste them under the video; they seed the storyline section of your GDD.

## Troubleshooting

| Symptom | Fix |
|---|---|
| **]** does nothing and the Console is silent | Click inside the Game view so `Keyboard.current` receives input; check *Next Key* in the Inspector is not one the simulator uses |
| Labels still say "title set by StoryBeatSequencer TODO 1" | `Start()` in the sequencer does not call `SetTitle` for every beat, or a beat's *Station* slot is empty |
| Two stations are bright at once | `Advance` lights the new station before (or instead of) dimming the old one; dim `beats[CurrentIndex]` first, then update `CurrentIndex` |
| Lights snap instead of fading | `s` in `BeatStation.Update` is still the constant 1 (finish TODO 3 and compute it before the `Lerp`s), or `SetActive` never resets `elapsed` to 0 |
| Fade runs but overshoots or flickers | You used `Time.time` instead of accumulating `Time.deltaTime`, or forgot `Clamp01` |
| Guide text appears all at once | `charactersPerSecond` is huge, or you assigned `fullLine` to the label directly in `Say` instead of leaving it empty |
| Guide turns but the text swings away with her | `LateUpdate` in `GuideVoice` must re-aim the label every frame; it is a child of the Guide |

Created by Isac Artzi

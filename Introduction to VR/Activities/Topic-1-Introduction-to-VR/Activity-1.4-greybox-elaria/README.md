# Activity 1.4 — Greybox Elaria

Topic 1: Introduction to Virtual Reality · Week 2, Thursday · Stepping stone to Milestone 1 — Project Proposal and Concept Development

## Where this fits

Milestone 1 closes with a project plan, and a plan for three environments is only credible if someone has walked them. Today you walk all three. **Activity → Build Starter Scene** lays out a *greybox* — a blockout of the Enchanted Forest path, the Mountain Pass switchback, and the Ancient Ruins room in one continuous route, built from grey primitives at true scale: a 2.5 m path, 0.17 m steps, 15° ramps with a 0.9 m handrail, a 6 × 8 × 4 m room with a 1.0 × 2.1 m doorway. You write three scripts that turn the walk into evidence: zone volumes that notice when your head enters each environment, an announcer that names the zone in front of you, and a metrics logger that records seconds and meters per zone and prints a table. Those numbers — "the forest path takes about 20 s, the switchback 35 s" — belong in your project plan and later in your comfort and playtest reports. The last task is not code: you set up your team's Git repository so that from Topic 2 on, every prototype lands in one place.

*Elaria hook:* before the artifact, before the shards, the Guide walks you through the bones of the world — no leaves, no moss, no torchlight. "A hero must know the ground," she says. "How wide is the path when the wolves come? How steep the pass when the storm hits? How low the door when you carry the shard through?" Measure it now, while it is only stone.

## Learning goals

- You can explain what a greybox is for and why it is built at true scale before any art is made.
- You can quote the human metrics for doorways, corridors, steps, ramps and handrails and check a scene against them.
- You can compute a slope angle from rise and run, and a stair's rise/tread relationship, and relate them to comfort in VR.
- You can detect a point inside a rotated box in local space, act on enter/exit edges, and accumulate per-zone time and distance.
- You can initialize a Unity project as a Git repository with the right `.gitignore`, commit the right folders, and work on your own branch.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-1.4-greybox-elaria`). Open it with Unity **6000.5.x**.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup** — two `OK` lines.
6. **Activity → Build Starter Scene**. The scene `Activity_1_4` opens; you are at the start of a grey path.
7. Press **Play** and walk the whole route once without stopping: path, stairs, ramp, ramp, doorway, room. Come back.

Simulator controls that matter today: **Tab** to select the head, **W A S D** to move it, **Q / E** to move it down / up, **mouse** to look. The simulator has no gravity and no collision, so on the stairs and ramps *you* keep the head about 1.6 m above the surface with **E** (hold it lightly while climbing) — awkward, and worth noticing: in Topic 2 the rig's move provider and gravity do this for you. The activity's keys, changeable in the Inspector: **Enter** prints the metrics table, **Backspace** resets it.

## VR theory

**Greyboxing (blockout).** A greybox is the level built from untextured primitives at final scale before any art, lighting or effects. It exists to answer spatial questions cheaply: is the path wide enough, is the climb too long, can you see the door from the forest, does the room feel like a temple or a garage? In flat games a greybox is a layout tool; in VR it is also a *comfort* tool, because the player's own body is inside it and every meter is judged against that body. Braun and Rizzo stress prototyping early with placeholder geometry in the opening chapters of *XR Development with Unity*; this is that habit applied to a whole world. Rule of the day: block it out, walk it, measure it, then change numbers — never decorate a layout you have not walked.

**Human metrics.** Architecture has centuries of measurements for bodies moving through space, and VR inherits them because the player's body is real. Interior doorways are 0.8–1.0 m wide and 2.0–2.1 m tall; a corridor for two people to pass is at least 1.2 m, and in VR 2–3 m feels comfortable because you cannot see your own shoulders. Stair steps rise 0.15–0.19 m with a tread of 0.25–0.30 m (0.17 / 0.28 is a comfortable domestic stair; 0.2 m is the practical maximum). Handrails sit 0.9–1.0 m above the step nosing or ramp surface. A room 6 × 8 m with a 4 m ceiling reads as a hall; the same footprint at 2.4 m reads as a basement. Every one of these numbers is in the starter scene as a labeled primitive so you can stand next to it.

**Slopes and comfort.** Walking up a slope in VR is *visually* a pitch change and *bodily* nothing — your inner ear reports level ground. The mismatch is mild for gentle slopes and grows with steepness and speed. Accessibility codes cap ramps at about 1:12 (4.8°); game levels routinely use 10–15° for "a climb you notice", and beyond about 25–30° players expect stairs or a ladder. The switchback today is 15°: steep enough to read as a mountain, gentle enough that most people are fine with smooth locomotion on it. Landings every 6 m of run give the eye a level reference.

**Sightlines and landmarks.** A greybox also tests what the player can *see* from where. From the forest floor you should glimpse the ruins on the summit — a goal you can point at. From the top landing you should see the whole path you climbed — a reward. If a sightline matters to your story, it must survive the art pass; note it now.

**Distance and time as design data.** Walking speed in VR is 1–1.5 m/s with smooth locomotion, effectively higher with teleport. A 20 m path is 15–20 s of walking — long enough to set a mood, short enough not to bore. Measuring time per zone tells you whether your environments are the right size for what happens in them; measuring distance tells you whether people wandered (exploring) or beelined (bored, or following an obvious path).

## Math foundation

**Slope angle from rise and run.** For a ramp that rises $h$ over a horizontal run $r$:

$$\theta = \arctan\left(\frac{h}{r}\right) \qquad h = r \tan\theta \qquad L = \frac{r}{\cos\theta}$$

where $L$ is the length of the ramp surface. Worked example: the starter ramps have $r = 6$ m and $\theta = 15°$, so $h = 6 \tan 15° = 6 \times 0.268 = 1.61$ m and $L = 6 / \cos 15° = 6 / 0.966 = 6.21$ m. Two ramps plus six steps of 0.17 m put the summit at $1.02 + 2 \times 1.61 = 4.24$ m. Going the other way: a 2 m rise over 8 m of run is $\arctan(0.25) = 14.0°$; the same rise over 4 m is $\arctan(0.5) = 26.6°$ — stairs territory. In code, `Mathf.Atan(h / r) * Mathf.Rad2Deg`.

**Stairs.** Comfortable stairs obey the old rule $2R + T \approx 0.63$ m (rise $R$, tread $T$). The starter uses $R = 0.17$, $T = 0.28$: $0.34 + 0.28 = 0.62$ m. Six steps climb $6 \times 0.17 = 1.02$ m over $6 \times 0.28 = 1.68$ m, an effective slope of $\arctan(1.02/1.68) = 31°$ — which is exactly why it is stairs and not a ramp.

**Point inside a rotated box.** A `BoxCollider` has a `center` and a `size` in its object's *local* space. Transform the head into that space, $p_{local} = M^{-1} p_{world}$ (`transform.InverseTransformPoint`), then the head is inside when

$$|p_x - c_x| \le \tfrac{s_x}{2} \quad\text{and}\quad |p_y - c_y| \le \tfrac{s_y}{2} \quad\text{and}\quad |p_z - c_z| \le \tfrac{s_z}{2}.$$

This works for any rotation and scale of the zone, which `Bounds.Contains` (world-axis-aligned) does not.

**Distance walked and speed.** Each frame, take the head's horizontal displacement and add its length: $d \mathrel{+}= \lVert (\Delta x, 0, \Delta z) \rVert$. Time in zone is the sum of `Time.deltaTime` while inside. Average speed is $\bar v = d / t$. Worked example: 21.3 m of forest path in 16.4 s is 1.30 m/s. Discard any single-frame step longer than 3 m: at 1.4 m/s a person covers 0.02 m per frame at 72 Hz, so a 3 m jump is a reset or a teleport, not a walk.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_1_4.unity` with one continuous route:

- **Floor** — 120 m of grey ground, light haze, bright sun. Everything is grey with a faint tint per environment.
- **XR Origin (XR Rig)** at the origin with the **XR Interaction Simulator**.
- **Enchanted Forest (greybox)** — four 5 m path segments, 2.5 m wide, bending ±15° so the path winds but ends on the z axis about 21 m out; grey trunk-and-crown trees on both sides.
- **Mountain Pass (greybox)** — six steps (0.17 m rise, 0.28 m tread) to **Landing 1**, **Ramp 1 (15 deg)** climbing 6 m of run to the right, **Landing 2**, **Ramp 2 (15 deg)** climbing back, **Landing 3** at 4.24 m. Each ramp has a **Handrail (0.9 m)** on its open edge. A **Rock Spine** separates the ramps and a **Cliff** backs them.
- **Ancient Ruins (greybox)** — a 6 × 8 × 4 m room on a solid plinth at summit height: doorway wall (two pieces and a lintel leaving a **1.0 × 2.1 m** opening), back and side walls, two broken pillars, an altar with a glowing shard, a warm light.
- **Zone volumes** — `Zone - Enchanted Forest`, `Zone - Mountain Pass`, `Zone - Ruins Doorway` and `Zone - Ancient Ruins`: trigger `BoxCollider`s with `ZoneTrigger`, *Announcer* and *Metrics* pre-assigned. Select one in the Scene view to see its tinted gizmo.
- **Zone Announcer** — carries `ZoneAnnouncer` with *Banner* (the **Zone Banner** TextMesh) assigned and an empty `AudioSource` for an optional chime.
- **Player Metrics** — carries `PlayerMetrics` with *Readout* (the **Metrics Readout** sign near the start) assigned.
- Signs at the forest entrance, the stairs, the ramp and the doorway stating the dimensions, and a **Welcome Sign**.

Scripts live in `Assets/Scripts/`: `ZoneTrigger.cs`, `ZoneAnnouncer.cs`, `PlayerMetrics.cs`.

## Your tasks (about 70 min)

**Task 1 (15 min) — Know where the head is** · file: `Scripts/ZoneTrigger.cs`, TODO 1–2
Convert the head position into the zone's local space with `transform.InverseTransformPoint`, compare it against `box.center` ± `box.size / 2` on all three axes, and act only on the frame the answer changes: on entering call `announcer.Announce(zoneName, zoneColor)` and `metrics.EnterZone(zoneName)`; on leaving call `metrics.ExitZone(zoneName)`. Guard both references against null.
**Check:** walking the head onto the path prints `[Metrics] enter Enchanted Forest` once and `[Announcer] Enchanted Forest …` once; stepping off the path sideways prints the exit once. Climb the stairs: `Mountain Pass`. Nothing repeats while you stand still.

**Task 2 (15 min) — Announce it in the world** · file: `Scripts/ZoneAnnouncer.cs`, TODO 1–3
In `Announce`, place the banner 2 m ahead of the eyes on the flattened forward, 0.3 m below eye level, facing the head; set its text and color and restart the timer; play the chime if one is assigned. In `Update`, keep alpha at 1 for `holdSeconds`, then ease it to 0 with `1 - SmoothStep(...)` over `fadeSeconds`.
**Check:** entering the forest drops a green "Enchanted Forest" sign in front of you that you can walk past; it stays 2 s and fades over 1 s. The doorway announces "Ruins Doorway" in gold as you pass through, then "Ancient Ruins" in amber inside.

**Task 3 (20 min) — Measure the walk** · file: `Scripts/PlayerMetrics.cs`, TODO 1–5
Each frame compute the horizontal step from the last head position (discard steps over *Teleport Threshold*), add time and distance to the totals and to the current zone's `ZoneStats`, and update the readout sign. Wire **Enter** to `Debug.Log(BuildReport())` and **Backspace** to `ResetAll()`. In `BuildReport`, write one row per zone — entries, seconds, meters, m/s — and a TOTAL row.
**Check:** walk the full route and press **Enter**. You get four rows and a total; the forest shows roughly 21 m, the mountain roughly 2 × 6 m of run plus landings and stairs (about 18–20 m horizontally), speeds around 1–1.5 m/s. Stopping Play also prints the table.

**Task 4 (10 min) — Change one number, walk again** · no new code
Open `Assets/Editor/ActivitySceneBuilder.cs` and change **one** constant at the top: `RampSlope` to 25, or `DoorHeight` to 1.8, or `PathWidth` to 1.2. Save, run **Activity → Build Starter Scene** again (it overwrites the scene), and walk the route. Press **Enter** and compare the two tables. Put the constant back or keep it — but write down what changed in how the space *felt*.
**Check:** two metrics tables in the Console from two builds, and one sentence of your own about the difference.

**Task 5 (10 min) — Team repository and recording** · no code; see the next section
Follow *Set up your team repository* below with your team, then record your screencast: the whole route with announcements, the metrics table, and your changed constant.

## Set up your team repository

Your semester project needs one shared Unity project under version control from day one of Topic 2. Do this once per team, today, while the project is still small. One person (the *maintainer*) does steps 1–6; everyone does step 7.

1. **Create the project.** In Unity Hub, *New project* → template **Universal 3D** (URP) → name it `RealmOfLegends` → create. Install the XR Interaction Toolkit and OpenXR through the Package Manager exactly as you did for the activities, import the Starter Assets and the XR Interaction Simulator, and enable OpenXR on the desktop tab. Close Unity.
2. **Editor settings that make Git work.** Reopen the project. **Edit → Project Settings → Editor**: *Asset Serialization → Mode* = **Force Text** (scenes and prefabs become mergeable text); *Version Control → Mode* = **Visible Meta Files**. Save (Ctrl/Cmd+S) and close Unity again.
3. **Initialize the repository.** Copy the `.gitignore` file from *this* activity folder (every activity project ships one; it hides `Library/`, `Temp/`, `Logs/`, `UserSettings/`, builds and IDE files) into the `RealmOfLegends` folder. Then in a terminal inside that folder:
   `git init` → `git add .` → `git commit -m "Empty URP + XRI project"`.
   Check with `git status` that `Library/` is *not* listed — if it is, the `.gitignore` is in the wrong place.
4. **What you commit, and what you never commit.** Commit `Assets/`, `Packages/`, `ProjectSettings/`, `.gitignore`, and a `README.md`. Never commit `Library/`, `Temp/`, `Logs/`, `obj/`, `UserSettings/`, `Builds/`, `.vs/`, `.idea/` or `*.csproj` — Unity regenerates all of them. Every asset's `.meta` file **must** be committed; a missing `.meta` breaks every reference to that asset for your teammates.
5. **Push to a remote.** Create an empty private repository on your team's Git host, add every teammate as a collaborator, then
   `git remote add origin <url>` → `git branch -M main` → `git push -u origin main`.
6. **Protect `main`.** In the host's settings, require a pull request to change `main`. `main` must always open in Unity without errors: it is the build you demo.
7. **One branch per person.** Each teammate clones the repository, opens it once in Unity (this regenerates `Library/`, be patient), and creates a branch named after themselves and the feature: `git checkout -b alex/forest-greybox`. Work there, commit small and often (`git add -A && git commit -m "Forest path blockout at 2.5 m"`), push, and open a pull request when a piece works. Pull `main` into your branch before you start each session.
8. **Scenes and conflicts.** Two people editing the same scene produces conflicts that are painful to resolve, even in text mode. Give each environment its own scene (`Forest.unity`, `Mountain.unity`, `Ruins.unity`) with one owner, and share objects through prefabs (a prefab edited by one person merges cleanly into a scene owned by another). If a conflict does happen, keep one side whole rather than hand-merging YAML, then redo the smaller change.
9. **Large files.** Models, textures and audio are binary. Keep individual files under about 50 MB; if your host complains or the repository grows past a few hundred megabytes, enable Git LFS for `*.fbx`, `*.png`, `*.wav` (`git lfs install`, `git lfs track "*.fbx"`, commit the `.gitattributes`).

Do step 7 now, with the maintainer watching each teammate's first push arrive. Your Padlet post for today includes the repository URL.

## Stretch goals

- Add a `Zone - Summit Sightline` volume on Landing 3 and, in the announcer, show a second line "You can see: the forest path" — then rotate that volume 30° in the Inspector and confirm your local-space test still works where `Bounds.Contains` would not.
- Write the metrics table to a CSV file with `System.IO.File.WriteAllText(Path.Combine(Application.persistentDataPath, "walk.csv"), text)` on the report key, and open it in a spreadsheet.
- Build the greybox of *your* team's first environment from the project plan in the new repository: block it out with primitives at true scale, add one `ZoneTrigger`, and push it on your branch before the next class.

## Port to Quest 3

Nothing here needs the headset, but this is the first scene where the headset changes the experience: your real body is the ruler. Disable the **XR Interaction Simulator**, follow `Docs/PortingToQuest3.md`, and *Build And Run*. Without a keyboard the metrics table prints when you stop the app (`OnDisable`) — read it with `adb logcat -s Unity` — or add an `InputActionReference` for a controller button next to the keyboard check. Because the rig has no locomotion yet, you can only walk the first meters of the path physically; stand in the doorway (move the rig there in the Scene view before building) and judge 2.1 m with your own head.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the full route with each zone announced, the metrics table in the Console, and the route again after you changed one constant.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus one sentence about how the changed constant felt.
- The URL of your team repository and the name of your branch.
- (optional) the two metrics tables as text.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Zones never fire, no errors | `Camera.main` was null at `Start` (the rig prefab is missing — import Starter Assets and rebuild), or your inside test compares world coordinates against local `center`/`size` |
| A zone fires every frame | You act on `inside` instead of on the change `inside != HeadInside`; update `HeadInside` after the checks |
| Banner appears at the origin or faces away | `Announce` must position it from `head.position` and rotate with `LookRotation(fwd)`, not `-fwd` |
| Banner never fades | `timer` is not reset to 0 in `Announce`, or `Update` writes alpha only inside the hold branch |
| Metrics show hundreds of meters | The teleport filter is missing: discard steps longer than `teleportThreshold`, and set `lastHeadPos` every frame |
| The table has no rows | `BuildReport` still returns only the header — implement TODO 5; also `EnterZone` is never called because Task 1 is incomplete |
| `git status` lists thousands of files under `Library/` | The `.gitignore` is not in the project root or is named `gitignore` without the leading dot |

Created by Isac Artzi

# Introduction to VR In-Class Activities — Realm of Legends stepping stones

Thirty small Unity projects, one per class meeting. Each folder is a complete Unity 6.5 project skeleton: open it in Unity Hub, import the two XR Interaction Toolkit samples, run **Activity > Build Starter Scene**, read the project's `README.md`, and implement the numbered TODOs in `Assets/Scripts/`. Every activity is a stepping stone toward that topic's course-project milestone.

## How a class meeting runs

1. **Pull** the activity folder from GitHub (download the folder, or `git clone` the repo and open the folder in Unity Hub).
2. **Read** `README.md` (Activity > Open README inside Unity).
3. **Build** the starter scene (Activity > Build Starter Scene) and press Play with the XR Interaction Simulator.
4. **Implement** the TODOs in order. Desktop first; port to Quest 3 when it works (`Docs/PortingToQuest3.md`).
5. **Record** a 2–4 minute screencast and post it to this week's Padlet with 3–5 bullets: what you did, what works, what does not (yet).

Keep each activity in its own Unity project. Every folder defines its own `ActivitySceneBuilder` (and a few script names repeat across activities), so copying two activities' `Assets/` into one project produces duplicate-class compile errors. To carry work forward into your course project, copy only the specific scripts you need.

## Requirements

Unity 6.5 (6000.5.x) with Android Build Support · XR Interaction Toolkit 3.5 · OpenXR 1.17 · Visual Studio Code · a Meta Quest 3 for porting. Packages resolve automatically on first open. Free assets, where an activity needs them, are listed in `Docs/FreeAssets.md`.

## Activity index

| # | Week · Day | Activity | Stepping stone to | Scripts | Assets needed |
|---|---|---|---|---|---|
| 1.1 | W1 · Tue | [Hello, Elaria](Topic-1-Introduction-to-VR/Activity-1.1-hello-elaria/README.md) | Milestone 1 | SpinCollectible, LookAtPlayer, EyeHeightProbe | no |
| 1.2 | W1 · Thu | [Mock View Studio](Topic-1-Introduction-to-VR/Activity-1.2-mock-view-studio/README.md) | Milestone 1 | ViewpointCycler, AnnotationPin, PropPlacer | no |
| 1.3 | W2 · Tue | [Story Beats in Space](Topic-1-Introduction-to-VR/Activity-1.3-story-beats/README.md) | Milestone 1 | StoryBeat, BeatStation, StoryBeatSequencer, GuideVoice | no |
| 1.4 | W2 · Thu | [Greybox Elaria](Topic-1-Introduction-to-VR/Activity-1.4-greybox-elaria/README.md) | Milestone 1 | ZoneTrigger, ZoneAnnouncer, PlayerMetrics | no |
| 2.1 | W3 · Tue | [Grab the Shard](Topic-2-VR-Game-Design-Principles/Activity-2.1-grab-the-shard/README.md) | Milestone 2 | GrabHighlighter, ShardSocketCheck, GrabCounter | no |
| 2.2 | W3 · Thu | [First Steps: Teleport and Move](Topic-2-VR-Game-Design-Principles/Activity-2.2-first-steps/README.md) | Milestone 2 | LocomotionModeSwitch, ComfortVignette, TeleportSpotRandomizer | no |
| 2.3 | W4 · Tue | [Menus in Space](Topic-2-VR-Game-Design-Principles/Activity-2.3-menus-in-space/README.md) | Milestone 2 | PauseMenuController, FollowHeadLazy, MenuButtonFeedback | no |
| 2.4 | W4 · Thu | [Build to Quest and Measure](Topic-2-VR-Game-Design-Principles/Activity-2.4-build-to-quest/README.md) | Milestone 2 | FrameTimeHud, StressSpawner, PerfLogger | no |
| 3.1 | W5 · Tue | [Plant the Forest](Topic-3-3D-Modeling-and-VR-Environments/Activity-3.1-plant-the-forest/README.md) | Milestone 3 | RandomScatter, WindSway, ScaleNormalizer | yes |
| 3.2 | W5 · Thu | [Mountain Pass: Terrain and Weather](Topic-3-3D-Modeling-and-VR-Environments/Activity-3.2-mountain-pass/README.md) | Milestone 3 | WeatherController, AltitudeFog, SnowEmitter | yes |
| 3.3 | W6 · Tue | [Make It Fast: LOD, Occlusion, Lightmaps](Topic-3-3D-Modeling-and-VR-Environments/Activity-3.3-make-it-fast/README.md) | Milestone 3 | LodBuilder, CullingReporter, PerfProbe | yes |
| 3.4 | W6 · Thu | [Ancient Ruins: Modular Kit and Mechanisms](Topic-3-3D-Modeling-and-VR-Environments/Activity-3.4-ancient-ruins/README.md) | Milestone 3 | LeverInteractable, SlidingDoor, HiddenChamberReveal, ArtifactPiece | yes |
| 4.1 | W7 · Tue | [The Hero's Satchel: Inventory](Topic-4-VR-Interaction-and-UI/Activity-4.1-inventory/README.md) | Milestone 4 | InventoryItem, StorableItem, Inventory, InventoryPanel | no |
| 4.2 | W7 · Thu | [Speak with the Sage: NPC Dialogue](Topic-4-VR-Interaction-and-UI/Activity-4.2-speak-with-the-sage/README.md) | Milestone 4 | DialogueNode, DialogueRunner, NpcTalkTrigger | no |
| 4.3 | W8 · Tue | [The Runestone Puzzle](Topic-4-VR-Interaction-and-UI/Activity-4.3-runestone-puzzle/README.md) | Milestone 4 | Runestone, RuneSocket, PuzzleController, PuzzleFeedback | no |
| 4.4 | W8 · Thu | [Usability Lab](Topic-4-VR-Interaction-and-UI/Activity-4.4-usability-lab/README.md) | Milestone 4 | TaskPrompt, SessionRecorder, QuickSurvey | no |
| 5.1 | W9 · Tue | [Teleport, Properly](Topic-5-VR-Locomotion-and-Movement/Activity-5.1-teleport-deep-dive/README.md) | Milestone 5 | TeleportFade, TeleportArcPreview, LandingValidator | no |
| 5.2 | W9 · Thu | [Smooth Move and Turn](Topic-5-VR-Locomotion-and-Movement/Activity-5.2-smooth-and-turn/README.md) | Milestone 5 | MoveTuner, SnapTurnFeedback, SpeedRamp | no |
| 5.3 | W10 · Tue | [Hybrid: Dash and Blink](Topic-5-VR-Locomotion-and-Movement/Activity-5.3-dash-and-blink/README.md) | Milestone 5 | VignetteController, DashLocomotion, BlinkStep | no |
| 5.4 | W10 · Thu | [Comfort Settings and the Rest Point](Topic-5-VR-Locomotion-and-Movement/Activity-5.4-comfort-settings/README.md) | Milestone 5 | ComfortSettings, SettingsMenu, ComfortLog, RestPoint | no |
| 6.1 | W11 · Tue | [Hear the Forest: Spatial Audio](Topic-6-VR-Audio-and-Sound-Design/Activity-6.1-hear-the-forest/README.md) | Milestone 6 | ToneFactory, RolloffVisualizer, AudioBeacon, ListenerCheck | yes |
| 6.2 | W11 · Thu | [Soundscapes and Music Zones](Topic-6-VR-Audio-and-Sound-Design/Activity-6.2-soundscapes/README.md) | Milestone 6 | MusicZoneManager, AmbientLayer, ZoneVolume | yes |
| 6.3 | W12 · Tue | [The Sound of Touch: SFX and Haptics](Topic-6-VR-Audio-and-Sound-Design/Activity-6.3-sound-of-touch/README.md) | Milestone 6 | SfxPool, ImpactSound, HapticPulse | yes |
| 6.4 | W12 · Thu | [Dark Mode: Navigate by Sound](Topic-6-VR-Audio-and-Sound-Design/Activity-6.4-dark-mode/README.md) | Milestone 6 | DarkModeController, SonarPing, OcclusionFilter | yes |
| 7.1 | W13 · Tue | [Portals and Scene Flow](Topic-7-VR-Game-Development-Project/Activity-7.1-portals-and-scenes/README.md) | Milestone 7 | GameManager, SceneLoader, Portal, ArtifactPiece | no |
| 7.2 | W13 · Thu | [Quests and Saves](Topic-7-VR-Game-Development-Project/Activity-7.2-quests-and-saves/README.md) | Milestone 7 | QuestStep, QuestManager, SaveData, SaveSystem | no |
| 7.3 | W14 · Tue | [Playtest Telemetry and Heatmap](Topic-7-VR-Game-Development-Project/Activity-7.3-playtest-telemetry/README.md) | Milestone 7 | PositionSampler, HeatmapGrid, BugReporter | no |
| 7.4 | W14 · Thu | [Juice: Feedback and Polish](Topic-7-VR-Game-Development-Project/Activity-7.4-juice/README.md) | Milestone 7 | PickupBurst, TweenScale, WorldToast, GuideReveal | no |
| 7.5 | W15 · Tue | [Performance Pass for Quest 3](Topic-7-VR-Game-Development-Project/Activity-7.5-performance-pass/README.md) | Milestone 7 | PerfBudgetChecker, BatchingAudit, TextureAudit | no |
| 7.6 | W15 · Thu | [Build, Record, Present](Topic-7-VR-Game-Development-Project/Activity-7.6-demo-kit/README.md) | Milestone 7 | DemoMenu, ScreenshotTool, DemoTimer | no |

## Topics at a glance

### Topic 1: Introduction to Virtual Reality — Milestone 1 — Project Proposal and Concept Development

- **1.1 Hello, Elaria** — Your first running VR scene: an Enchanted Forest clearing at true scale with spinning artifact shards, a placeholder Guide, and a probe that measures eye height, distance, and angular size — the numbers you will quote when you justify your concept.
- **1.2 Mock View Studio** — A diorama stage with preset viewpoints, annotation pins, and a prop placer so you can compose and measure the Unity mock views your 'Re-imagining an Experience in VR' report and concept video need.
- **1.3 Story Beats in Space** — Your GDD storyline (beginning, middle, end) staged as five physical stations that light up in sequence while the Guide speaks each beat — the spatial storyboard for your concept video and character sheet.
- **1.4 Greybox Elaria** — A true-scale greybox of all three Elaria environments in one scene with zone announcements and a walk-time/distance log — the spatial evidence for your project plan — plus your team's Git repository set up and ready for Topic 2.

### Topic 2: VR Game Design Principles on a Variety of Platforms — Milestone 2 — Platform Compatibility Design

- **2.1 Grab the Shard** — Your first real VR interaction loop: grab artifact shards with XRI's grab interactables, seat them in an altar socket that accepts only shards, and read a live progress counter — the pick-up-and-place mechanic your Milestone 2 prototype must show on the headset.
- **2.2 First Steps: Teleport and Move** — Locomotion for the prototype: teleport areas and waystone anchors plus smooth movement from the Starter Assets rig, a runtime switch between teleport-only, smooth-only and hybrid, and a speed-driven comfort vignette — the movement third of the Milestone 2 headset prototype.
- **2.3 Menus in Space** — The UI third of the Milestone 2 prototype: a world-space pause menu with Start/Resume/Quit driven by XRI's ray and UI input module, a lazy follow that keeps it in front of the head without gluing it there, and hover feedback — the foundation for every menu, inventory and dialogue panel in Realm of Legends.
- **2.4 Build to Quest and Measure** — Your first Android build on the Quest 3 plus the tools to measure it: a frame-time HUD, a stress spawner that adds lit cubes on demand, and a CSV logger of frame time versus object count in persistentDataPath — the deployed prototype and the optional performance metrics of Milestone 2.

### Topic 3: 3D Modeling and VR Environments — Milestone 3 — Environment Creation

- **3.1 Plant the Forest** — The Enchanted Forest grows from a greybox clearing into a populated environment: a scatter tool that plants dozens of trees, rocks, and bushes at random while keeping the path clear, wind sway for foliage, a scale gauge that tells you exactly how far off an imported model's size is, and your first PBR ground material from free textures.
- **3.2 Mountain Pass: Terrain and Weather** — The Mountain Pass becomes a real Unity Terrain you can sculpt and paint, with a weather system that blends fog, light, wind, and snow between Clear, Overcast, and Blizzard, thickens the fog as you climb, and keeps a snow emitter over the player's head — the atmosphere layer of your second environment.
- **3.3 Make It Fast: LOD, Occlusion, Lightmaps** — The optimization pass the milestone asks for, practiced on a dense ruins yard: a script that builds three-level LOD Groups, a culling reporter that counts what the camera actually draws, a frame-time probe that writes before/after rows, and the two bakes — occlusion culling and lightmaps — with a filled-in comparison table for three objects.
- **3.4 Ancient Ruins: Modular Kit and Mechanisms** — The third environment gets its bones and its first puzzle: a ruins room assembled from grid-snapped modules, a physical lever (XR grab + hinge) that fires an event when pulled far enough, a stone door that slides open with eased motion, and a hidden chamber that reveals itself when the right artifact is placed on the altar.

### Topic 4: VR Interaction and User Interfaces — Milestone 4 — UI and Interaction Design

- **4.1 The Hero's Satchel: Inventory** — A working body-anchored inventory: grab a collectible, bring it to your hip and it vanishes into a satchel list; a wrist-height world-space panel shows the slots and lets you pull any item back out — the inventory system your Milestone 4 prototype needs.
- **4.2 Speak with the Sage: NPC Dialogue** — A branching NPC dialogue system: walk up to the Forest Sage and look at her, a dialogue box opens above her head, the text types itself out, and two choice buttons branch through Inspector-edited nodes — the dialogue boxes and NPC interaction your Milestone 4 prototype needs.
- **4.3 The Runestone Puzzle** — A complete pattern-matching puzzle: three runestones must be placed into altar sockets in the order shown by glowing glyphs; XRSocketInteractors report placements, a state-machine controller validates the sequence, and light plus sound tell the player whether they got it right — the environment interaction and puzzle your Milestone 4 prototype needs.
- **4.4 Usability Lab** — A reusable user-testing harness: a task prompt panel, a recorder that timestamps every grab, release and socket event and writes time-on-task, errors and success to a CSV, and an in-VR five-question survey — the instrument you will run on classmates for Milestone 4's user testing and iteration.

### Topic 5: VR Locomotion and Movement — Milestone 5 — Movement Mechanics

- **5.1 Teleport, Properly** — The teleport half of the milestone's locomotion set: a blink-to-black fade driven by the TeleportationProvider's events, your own projectile arc preview, and a landing validator that rejects steep or cramped spots — plus anchors that decide which way the player faces on arrival.
- **5.2 Smooth Move and Turn** — The smooth-locomotion half of the milestone: a world-space tuning panel that sets move speed, snap-turn angle, and smooth-turn speed on the rig's providers at runtime, an acceleration ramp that eases speed in, and a click on every snap turn — the knobs you will expose as comfort settings in 5.4.
- **5.3 Hybrid: Dash and Blink** — The hybrid third of the milestone's locomotion set: a 0.2 s dash along the pointed direction with a tunneling vignette, and a 2 m blink step behind a 100 ms fade, both moving the XR Origin directly with collision-clamped distances — plus the notes on when to graduate to a custom XRI LocomotionProvider.
- **5.4 Comfort Settings and the Rest Point** — The comfort half of the milestone: a settings menu (locomotion mode, speed, turn type, vignette strength, height offset) saved with PlayerPrefs and applied to the rig's providers, a rest point the Guide invites you to, and an SSQ-lite comfort log that turns a 5-minute test with a classmate into a CSV you can quote.

### Topic 6: VR Audio and Sound Design — Milestone 6 — Sound Design

- **6.1 Hear the Forest: Spatial Audio** — Your first spatialized soundscape: three 3D AudioSources (stream, bird, fire) with visible rolloff rings, an audio beacon that swells as you turn toward it, and a check that the scene has exactly one AudioListener on the head — the placement and attenuation settings you will reuse on every sound in Realm of Legends.
- **6.2 Soundscapes and Music Zones** — Three walk-through zones (Forest, Mountain, Ruins) that crossfade their music with equal power, swap a layered ambience (constant bed plus randomly timed, randomly placed one-shots), and switch AudioMixer snapshots — the background-music and ambient layers of Milestone 6.
- **6.3 The Sound of Touch: SFX and Haptics** — Every grab, release and impact in the game gets a sound whose loudness follows impact speed and whose pitch varies slightly, played through a pooled one-shot player instead of spawning AudioSources — plus a guarded controller haptic pulse on grab. This is the SFX layer of Milestone 6 and the feedback your Milestone 4 interactions were missing.
- **6.4 Dark Mode: Navigate by Sound** — A toggleable blackout in which the player must reach an audio-beacon objective while avoiding buzzing hazards, helped by a sonar ping that echoes off the nearest surfaces with distance-proportional delay and by low-pass occlusion when a wall stands between listener and source — the 'dark mode' deliverable of Milestone 6 and your first accessibility feature.

### Topic 7: VR Game Development Project — Milestone 7 — Final Integration

- **7.1 Portals and Scene Flow** — The skeleton of the integrated game: a Hub scene with three portals that fade to black, load the Forest, Mountain, and Ruins scenes asynchronously, and a persistent GameManager that remembers which artifact pieces you collected on the way.
- **7.2 Quests and Saves** — The quest spine of the game: a three-step quest whose objectives fire from a grab, a socket, and an NPC approach, a wrist label that shows progress, and JSON save/load to disk so a playtester can stop and continue.
- **7.3 Playtest Telemetry and Heatmap** — The evidence for your playtest report: a sampler that logs where testers walk, a heatmap grid that shows it as colored cells in the Scene view or as cubes in the world, and an in-VR bug reporter that saves a screenshot, position, and timestamp with one button.
- **7.4 Juice: Feedback and Polish** — The polish layer that makes the integrated game feel finished: particles, a light flash, a scale punch and a procedural chime on every pickup, a world-space toast instead of screen text, and the Guide's anticipation-and-reveal moment when the last shard is taken.
- **7.5 Performance Pass for Quest 3** — The last technical pass before the demo: a runtime HUD that measures frame time against the 13.9 ms Quest budget, and two editor audits that list the materials that break batching and the textures that are too large or not ASTC, so the integrated game ships at frame rate.
- **7.6 Build, Record, Present** — The demo kit for the final presentation: a start scene with a world-space menu that jumps to any environment or feature, a screenshot tool, a demo countdown in your view, and a README that carries the presentation checklist and the Quest recording and casting instructions.

## Folder layout of one activity

```
Activity-1.1-hello-elaria/
├── README.md                     the activity: context, VR theory, math, tasks, deliverables
├── Assets/
│   ├── Editor/
│   │   ├── ActivitySceneBuilder.cs   Activity > Build Starter Scene (creates the scene for this activity)
│   │   ├── ActivityMenu.cs           Activity > Open README, Activity > Check Setup
│   │   └── SceneBuilderUtil.cs       shared helpers used by the builder
│   ├── Scenes/                       generated on demand
│   └── Scripts/                      the files you complete — numbered TODOs inside
├── Docs/
│   ├── PortingToQuest3.md
│   └── FreeAssets.md
├── Packages/manifest.json            XR Interaction Toolkit 3.5.1, OpenXR 1.17.0
├── ProjectSettings/ProjectVersion.txt
└── .gitignore
```

Created by Isac Artzi

# Activity 2.4 — Build to Quest and Measure

Topic 2: VR Game Design Principles on a Variety of Platforms · Week 4, Thursday · Stepping stone to Milestone 2 — Platform Compatibility Design

## Where this fits

By the end of class you will have installed your own build on a Meta Quest 3 and stood inside it — the first time Elaria exists somewhere other than a laptop screen. Before you build, you will add three measuring tools to the scene: a frame-time HUD that shows milliseconds and FPS against the 72 Hz budget, a stress spawner that fills the sky with lit cubes fifty at a time, and a CSV logger that records frame time against object count into `Application.persistentDataPath`. Milestone 2 requires a prototype deployed to the headset and offers optional performance metrics; this activity delivers the deployment pipeline and the numbers, and the same tools return in 3.3 for the before/after optimization table and in 7.5 for the final performance pass.

*Elaria hook:* high on the Mountain Pass the Hermit tends a fire and watches the sky. "Wisps," he says, "come in fifties. Count them until the stars begin to stutter — that is how you learn what this mountain can bear." He is talking about your frame budget.

## Learning goals

- You can state the frame budget for 72, 90 and 120 Hz in milliseconds and explain what happens when a frame misses it (reprojection, judder).
- You can distinguish CPU-bound from GPU-bound frames and name the costs behind draw calls, batches, triangles and fill rate.
- You can compute a moving average of `Time.unscaledDeltaTime` with a ring buffer and display it on a head-locked debug HUD, and explain why head-locked UI is otherwise avoided.
- You can show, with numbers, why `Renderer.material` (a unique instance per object) costs more than `Renderer.sharedMaterial`.
- You can write a CSV to `Application.persistentDataPath`, retrieve it from the Quest with `adb pull`, and read a slope (ms per 100 objects) from it.
- You can configure Android, OpenXR and Player settings and produce a working Quest 3 build.

## Before you start (10 min)

1. **Homework check (before class, on your own machine):** Unity Hub → Installs → your Unity 6.5 install → *Add modules* → tick **Android Build Support**, **OpenJDK**, **Android SDK & NDK Tools**. This download is large — do it on home Wi-Fi, not in class. Developer Mode must be on for the headset (Meta Horizon app → Devices → Headset Settings → Developer Mode). Details are in `Docs/PortingToQuest3.md`.
2. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-2.4-build-to-quest`). Open it with Unity **6000.5.x** and wait for packages. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** tab → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR** (the Android tab comes later, in Task 4).
5. Menu bar → **Activity → Check Setup**, then **Activity → Build Starter Scene**. The scene `Activity_2_4` opens.
6. **Edit → Project Settings → Quality**: set *VSync Count* to **Don't Sync** for the current level. Otherwise the Editor pins your frame time to the monitor's refresh rate and the HUD will never move.
7. Press **Play**. You stand on the Hermit's ledge; the HUD placeholder floats below your view.

Simulator controls that matter today: only the keyboard — **B** spawns 50 wisps, **C** clears them, **U** toggles unique materials, **L** saves the CSV. Use **Tab** + mouse on the head to look up at the cloud, and open **Game view → Stats** (top-right of the Game view) to see *Batches*, *Tris* and *SetPass calls* alongside your HUD.

## VR theory

**The frame budget.** A display running at frequency $f$ shows a new image every $1000/f$ milliseconds; that is the total time the CPU and GPU together may spend on a frame. Quest 3 runs at 72 Hz by default (13.9 ms), with 90 Hz (11.1 ms) and 120 Hz (8.3 ms) as options. A desktop game that drops from 60 to 45 FPS looks slightly worse; a VR game that misses its budget makes the *world* stutter while your head keeps moving, and the vestibular–visual conflict from 2.2 arrives instantly. Miss the budget often and players take the headset off. Braun and Rizzo discuss platform constraints and the mobile-VR performance envelope in Chapter 5 of *XR Development with Unity*; `Docs/PortingToQuest3.md` restates the numbers.

**Reprojection.** When a frame is late, the Quest runtime does not show a black frame: it re-displays the previous image, rotated to match your new head orientation (*asynchronous timewarp*), and can synthesize motion between frames (*spacewarp*). Rotation is handled well; translation and moving objects are not, so a late frame shows as judder and smeared edges rather than a freeze. Reprojection is a safety net, not a plan — your HUD tells you when you are leaning on it.

**CPU-bound versus GPU-bound.** Each frame the CPU runs your scripts, physics and animation and then *issues draw calls* — commands telling the GPU what to draw with which material. The GPU then shades every pixel of every triangle. If the CPU is still issuing commands when the deadline comes, you are **CPU-bound**: too many objects, too many scripts, too many unique materials. If the CPU finishes early but the GPU is still shading, you are **GPU-bound**: too many pixels shaded too many times (transparent overlap, post-processing, many lights) or too many triangles. The stress spawner is mostly a CPU test with shared materials and a much harsher one with unique materials; the two real-time point lights on the ledge push the GPU side too, because in the Built-in forward renderer every extra pixel light means another pass over every lit object.

**Draw calls, batches and SetPass calls.** A *draw call* asks the GPU to render one mesh with one material. Unity *batches* objects that share a material into fewer calls (dynamic batching for small meshes, static batching for objects marked Static, GPU instancing for identical meshes). A *SetPass call* switches shader state and is even more expensive than a draw call. One shared material across 1000 cubes means a handful of SetPass calls; 1000 unique materials means 1000 of them. This is why `Renderer.material` — which silently clones the material for that renderer — is the classic student mistake, and why you will flip it on and off today.

**Fill rate and triangles.** The Quest 3 renders two eye images at roughly 2064 × 2208 each, over 9 million pixels per frame before any overdraw. Shading cost scales with pixels × passes; large transparent particles or several overlapping lights multiply it. Triangle counts matter too but are rarely the first wall on Quest 3 — a common working budget is under 1 million triangles per frame and under 200 draw calls, after culling.

**Head-locked HUD — the exception to the rule.** Activity 2.3 told you head-locked UI hurts: it blocks the world, cannot be looked *at* naturally, and moves with your head in a way nothing real does. A debug readout is the one honest exception: it is tiny, semi-peripheral (35 cm below center at 1.5 m ≈ 13° down), shows numbers you must read while turning to look at the wisp cloud, and never ships to players. Even so, keep it small and remove it from the final game.

## Math foundation

**Budget from refresh rate:** $t_{budget} = 1000 / f$ ms. $1000/72 = 13.9$, $1000/90 = 11.1$, $1000/120 = 8.3$. Conversely $\text{FPS} = 1000 / t_{frame}$: a 20 ms frame is 50 FPS.

**Moving average over a window** of $N$ frame times $d_1 \ldots d_N$:

$$\bar d = \frac{1}{N}\sum_{i=1}^{N} d_i, \qquad \text{ms} = 1000\,\bar d, \qquad \text{FPS} = 1/\bar d$$

Averaging FPS values directly is wrong (the mean of 30 and 90 FPS is not 60 FPS in time terms); always average *time*, then convert. The HUD keeps a running sum in a ring buffer: when a new value $d_{new}$ replaces the oldest $d_{old}$, $\text{sum} \leftarrow \text{sum} - d_{old} + d_{new}$ — two operations regardless of $N$. Worked example: with $N = 60$, if 59 frames took 13.0 ms and one took 40 ms (a hitch), $\bar d = (59 \times 13 + 40)/60 = 13.45$ ms — the hitch nudges the average but does not swamp it, and it leaves the window after 60 frames.

**Slope from the CSV.** Frame time grows roughly linearly with object count in the range you will test. From two rows, $(n_1, t_1)$ and $(n_2, t_2)$:

$$\text{slope} = \frac{t_2 - t_1}{n_2 - n_1} \times 100 \quad \text{ms per 100 objects}$$

Example: 200 objects at 9.0 ms and 1200 objects at 14.0 ms give $(14 - 9)/1000 \times 100 = 0.5$ ms per 100 objects. Then the *headroom* is easy: from 9.0 ms you have 4.9 ms until 13.9, so about $4.9 / 0.5 \times 100 = 980$ more objects. Compare the slope with shared and with unique materials; the ratio is the cost of the mistake.

**Angular size of the HUD** (from 1.1): text with a character size of 0.02 at 1.5 m subtends about $2\arctan(0.01/1.5) \approx 0.8°$ per line — right at the legibility floor, which is fine for a readout you glance at and too small for anything a player must read.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_2_4.unity` with:

- **Floor** — a 40 m slate plane, static; **Rocks** — an arc of fourteen static rock cubes forming the ledge.
- **XR Origin (XR Rig)** at the origin with the **XR Interaction Simulator** and an **XR Interaction Manager**.
- **Fire Pit** with a warm **Fire Light**, and a cold **Moon Shard Light** — two real-time point lights that make every spawned cube more expensive.
- **Mountain Hermit** (a capsule) with a label.
- **Frame Time HUD** — a small `TextMesh` parented under the *Main Camera* at local (0, −0.35, 1.5), on the *Ignore Raycast* layer, carrying `FrameTimeHud` with *Label* and *Spawner* wired.
- **Wisp Cloud Center** — an empty 3.5 m up and 6 m ahead marking where cubes appear.
- **Perf Tools** — an empty carrying `StressSpawner` (spawn center, the emissive `Wisp` shared material, radius 3.5 m, batch 50) and `PerfLogger` (HUD and spawner wired, 0.5 s interval).
- A **Welcome Sign** listing the keys.

Scripts live in `Assets/Scripts/`.

## Your tasks (about 70 min)

**Task 1 (15 min) — The frame-time HUD** · file: `Scripts/FrameTimeHud.cs`, TODO 1–3
Each frame push `Time.unscaledDeltaTime` into the ring buffer, maintaining a running sum; divide by the number of filled samples for the average; convert to ms and FPS; print two lines and color the text red when over `budgetMs`.
**Check:** with VSync off the HUD shows a few ms and hundreds of FPS in the Editor; switch VSync back on for a moment and it settles at your monitor's rate (16.7 ms at 60 Hz). The text follows your head everywhere.

**Task 2 (15 min) — Spawn the wisps** · file: `Scripts/StressSpawner.cs`, TODO 1–3
Read **B**, **C** and **U** through `Keyboard.current` (plus the optional action and the auto-ramp timer). In `SpawnBatch`, create cubes at `spawnCenter.position + Random.insideUnitSphere * cloudRadius`, strip their colliders, and assign the material either as `sharedMaterial` or as a per-cube `material` with a random color. In `Clear`, destroy unique material instances, then the cubes.
**Check:** each **B** adds 50 glowing cubes above the ledge and the HUD's object count rises; open **Stats** and watch *Batches* stay nearly flat with shared materials and jump by ~50 per press after **U**. Keep pressing until the HUD turns red; note the count.

**Task 3 (10 min) — Log it** · file: `Scripts/PerfLogger.cs`, TODO 1–3
Append a row every `sampleInterval` seconds with invariant-culture formatting; in `Save`, write `rows` to a timestamped file in `Application.persistentDataPath` and log the path; implement `SlopeMsPerHundred`. Run a quick desktop session: clear, then spawn a batch every few seconds up to 1000 objects, press **L**.
**Check:** the Console prints a path; the CSV opens in a spreadsheet with a `frame_ms` column that climbs with `object_count`.

**Task 4 (25 min) — Build to Quest 3** · no new code; follow `Docs/PortingToQuest3.md` alongside this list
1. Stop Play. Select **StressSpawner** on *Perf Tools* and tick **Auto Ramp** (interval 3 s) so the headset test runs itself, or assign *Spawn Action* to a button action from the *XRI Default Input Actions* asset. Save the scene (Ctrl/Cmd+S).
2. **File → Build Profiles → Android → Switch Platform.** Wait for the re-import.
3. **Edit → Project Settings → XR Plug-in Management → Android tab** → check **OpenXR**. Under **OpenXR → Android**: enable the **Meta Quest Support** feature group; under *Enabled Interaction Profiles* add **Oculus Touch Controller Profile** (and the *Meta Quest Touch Plus Controller Profile* if listed); leave *Render Mode* on *Multi-pass* for this first build.
4. **Project Settings → Player → Android**: *Other Settings* → Minimum API Level Android 10 (API 29) or higher; Scripting Backend **IL2CPP**; Target Architectures **ARM64** only; Graphics APIs **Vulkan** first; Texture compression **ASTC**. *Resolution and Presentation* → Default Orientation **Landscape Left**. Give the product a name and a company name — they form the bundle identifier you will need for `adb pull`.
5. In the Hierarchy, **disable the XR Interaction Simulator** GameObject. Save the scene.
6. Plug in the headset, put it on once to accept *Allow USB debugging*, then in a terminal run `adb devices` and confirm your serial shows `device`.
7. **File → Build Profiles → Android → Build And Run.** Name the APK, then wait — the first IL2CPP build takes several minutes. Do the Stretch reading while you wait.
8. Put the headset on. The app launches; the HUD hangs below your view, and every 3 seconds fifty wisps join the cloud. Watch the ms climb, turn red, and note roughly how many objects that took — then compare with your laptop's number.
9. Back on the laptop: `adb pull /sdcard/Android/data/<bundle id>/files/ .` copies the CSV(s) the logger wrote when you closed the app (or when you pressed the assigned button). Open one and compute the slope for the Padlet post.
**Check:** the app appears under *Library → Unknown Sources* on the headset, runs with the HUD visible, and a CSV with the `Android` platform column lands on your laptop.

**Task 5 (5 min) — Screencast** · no new code
Record on the laptop: the HUD, spawning to red with shared materials, clearing, spawning with unique materials, saving the CSV, and the spreadsheet. If your headset casting works, add 20 seconds of the Quest run; otherwise film the headset screen mirror or simply show the pulled CSV.

## Stretch goals

- Try **GPU instancing**: tick *Enable GPU Instancing* on the Wisp material and watch *Batches* and frame time with 2000 shared-material cubes. Explain the difference in your bullets.
- Log `SystemInfo.graphicsDeviceName` and `Screen.currentResolution` into a header row of the CSV so laptop and headset files are self-describing.
- Read *Unity Manual → XR → Performance* and the `Docs/PortingToQuest3.md` frame-budget section, then set the headset's refresh rate to 90 Hz (Quest Settings → System → Display) and repeat the ramp: how many wisps fit in 11.1 ms?

## Port to Quest 3

This activity *is* the port — Task 4 walks through it, and `Docs/PortingToQuest3.md` remains the reference for every later activity. Two activity-specific notes: keep the HUD tiny and remove it before any playtest with a classmate; and remember that `Keyboard.current` is null on the headset, which is why the spawner has *Auto Ramp* and an optional `InputActionReference` — assign one of the XRI button actions if you prefer pressing a button to waiting.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the HUD updating, the stress ramp to red with shared materials, the same with unique materials (Stats visible), a CSV being saved, and evidence of the Quest build (a captured clip, the headset's *Unknown Sources* entry, or the pulled CSV with `Android` in the platform column).
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus the two slopes (ms per 100 objects, shared vs unique) and the object count at which the headset went red.
- (optional) Attach the CSV your logger produced on the Quest.

## Troubleshooting

| Symptom | Fix |
|---|---|
| HUD is frozen at 16.7 ms / 60 FPS in the Editor | VSync is on: **Project Settings → Quality → VSync Count → Don't Sync**, or toggle *VSync* in the Game view's toolbar |
| HUD shows nothing or the placeholder | TODO 3 in `FrameTimeHud` is not writing `label.text`, or *Label* lost its reference — rebuild the scene |
| B does nothing | `Keyboard.current` is null (Game view not focused — click it), or TODO 1 in `StressSpawner` is empty |
| Frame time barely moves even with 2000 cubes | Your laptop GPU is fast; press **U** for unique materials, or add a third point light — the *slope* is the result, not the absolute |
| `adb: no devices` | Charge-only cable, USB debugging prompt not accepted in the headset, or Developer Mode off — see `Docs/PortingToQuest3.md` |
| Build fails with a Gradle or SDK error | Android modules missing from the Unity install (Before you start, step 1), or a stale build folder — delete it and build again |
| Black screen on the headset | OpenXR not checked on the **Android** tab, or *Meta Quest Support* feature not enabled |
| No CSV after the headset run | `OnDisable` only fires when the app is closed properly — quit the app from the Quest menu (not by just removing the headset) |

Created by Isac Artzi

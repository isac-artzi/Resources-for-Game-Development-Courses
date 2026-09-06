# Activity 3.3 — Make It Fast: LOD, Occlusion, Lightmaps

Topic 3: 3D Modeling and VR Environments · Week 6, Tuesday · Stepping stone to Milestone 3 — Environment Creation

## Where this fits

Milestone 3 asks you to optimize your three environments and to prove it with a before/after performance comparison on three objects. Today you practice the full pass on a scene built to be slow: eighty pillars, each made of several primitives, in a walled yard lit by six point lights. You write a script that turns each pillar into a three-level LOD Group, a reporter that counts how many renderers the camera really draws, and a frame-time probe that logs comparison rows. Then you bake occlusion culling and lightmaps from Unity's own windows and fill in the comparison table for three objects. Bring your Forest and Mountain scenes on a drive: the same steps apply to them tomorrow.

*Elaria hook:* the caretaker of the ruins is old and tired. "Every stone here is a memory," she says, "and I cannot hold them all at once. Teach me to forget what no one is looking at." That is exactly what LODs, culling, and baking do.

## Learning goals

- You can explain frustum culling, occlusion culling, and LOD selection and say which one each of your numbers measures.
- You can build a `LODGroup` from script with `SetLODs` and choose transition heights from object size and distance.
- You can compute an object's screen-relative height from its size, distance, and the camera's vertical field of view.
- You can bake occlusion culling and lightmaps, and explain the difference between real-time, mixed, and baked lights.
- You can produce a before/after table (frame time, visible renderers) for three objects and interpret it.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-3.3-make-it-fast`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → import **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. Menu bar → **Activity → Check Setup**. Both lines should say `OK`.
6. Menu bar → **Activity → Build Starter Scene**. The scene `Activity_3_3` opens.
7. Press **Play**. You face a yard of pillars. Look closely at one: its three detail levels all render at once and shimmer where they overlap. That is the "before" state.
8. Turn on the **Stats** overlay (the *Stats* button at the top right of the Game view). Note *Batches*, *Tris*, and *Verts* now; you will quote them later.

Simulator controls that matter today: **Tab** to select the head, **W A S D** and mouse to walk to the doorway sign and face the wall, **P** to snapshot a perf row once Task 3 is done. **Maximize the Game view** (right-click its tab → Maximize, or Shift+Space) whenever you take a measurement: the Scene view counts as a camera and inflates `isVisible`.

## VR theory

The GPU does not draw the world; it draws whatever survives a chain of filters. **Frustum culling** is the first and it is free: anything outside the camera's viewing pyramid is skipped. Unity does this for every renderer every frame. **Occlusion culling** is the second: objects inside the frustum but hidden behind other objects are skipped too. Unity cannot know that at runtime without help, so you **bake** it — the *Occlusion Culling* window divides the scene into cells and precomputes, for each cell the camera might stand in, which static objects can be seen from there. Only objects marked *Static* (specifically *Occluder Static* and *Occludee Static*) take part. In VR occlusion baking matters more than on a monitor because the headset's wide field of view keeps more of the world inside the frustum.

**Level of detail (LOD)** attacks the objects that *are* visible. A pillar 60 m away covers a few pixels; drawing its capital, orb, and eight-sided shaft is wasted work. A `LODGroup` holds several versions of the object and, each frame, measures how tall the object's bounding box is *on screen* — as a fraction of the screen height, called the **screen-relative height** — and picks the coarsest version that still looks right. Below the last threshold the object is culled entirely. Braun and Rizzo cover LOD, culling, and lighting choices as part of environment optimization in Chapter 8 of *XR Development with Unity*.

**Batching** reduces the CPU cost of *telling* the GPU what to draw. Each draw call carries overhead; the Quest 3 is comfortable with roughly 100–200 batches per frame, not 1000. **Static batching** combines meshes that share a material and never move into one big mesh at load time; **dynamic batching** does the same on the fly for tiny meshes; **GPU instancing** draws many copies of the same mesh with one call. All three want *few materials* — which is why 80 pillars sharing one stone material batch well, and 80 pillars with 80 material variants do not.

**Baked lighting** moves the cost of lights from every frame to a one-time bake. A **real-time** light is evaluated per pixel per frame, and each real-time point light touching an object multiplies that object's shading cost; six real-time torches in a yard are a Quest budget on their own. A **baked** light writes its contribution into **lightmaps** — textures wrapped onto static objects through a second set of UVs (*Generate Lightmap UVs* on imported models) — and then costs nothing at runtime. **Mixed** lights bake the static part and still light dynamic objects like the player's hands in real time. Baking also gives you soft indirect light and ambient occlusion that a mobile GPU could never compute live.

## Math foundation

**Screen-relative height.** An object of height $h$ at distance $d$ viewed with vertical field of view $\phi$ occupies a fraction of the screen height

$$h_s = \frac{h}{2\,d\,\tan(\phi/2)}$$

because the visible height of the world at distance $d$ is $2 d \tan(\phi/2)$. Worked example: a 4.4 m pillar at 20 m in a 60° desktop view: $2 \times 20 \times \tan 30° = 23.1$ m visible, so $h_s = 4.4 / 23.1 = 0.19$ — inside the LOD1 band (0.15–0.40) with the default thresholds. Inverting gives the switch distances: $d = h / (2 h_s \tan(\phi/2))$. For the same pillar the LOD0→LOD1 switch (0.40) happens at 9.5 m, LOD1→LOD2 (0.15) at 25 m, and culling (0.03) at 127 m. On the Quest 3 the vertical FOV is close to 95°, $\tan 47.5° = 1.09$, so every switch happens at roughly half the desktop distance: 5.0 m, 13 m, 67 m. Test your thresholds on the headset, not only on the laptop.

**Choosing thresholds.** A reasonable rule: switch when the *detail you are removing* would be smaller than about one pixel. On a 1080-pixel-tall view, a 0.3 m capital at 20 m is $0.3 / 23.1 \times 1080 = 14$ pixels — still visible; at 60 m it is 4.7 pixels — marginal. So LOD1 should keep the capital and LOD2 may drop it around 40–60 m, which is $h_s \approx 0.05$–$0.07$ for a 4.4 m pillar. Start from the defaults, then tune against what you see.

**Draw-call arithmetic.** Before LODs: 80 pillars × 7 renderers = 560 renderers plus 11 wall pieces, 12 torch parts, 49 statue parts, about 630 in total. After LODs, each pillar shows one level: at most 4, typically 1–2 far away, so roughly 80–160 pillar renderers. With static batching and one shared stone material, those collapse further into a handful of batches. The *Stats* overlay reports *Batches* and *Saved by batching*; the CSV rows from `PerfProbe` report renderers. Quote both.

**Smoothing frame time.** The exponential moving average $m \leftarrow m + (x - m)\,\alpha$ with $\alpha = 0.05$ has an effective window of about $1/\alpha = 20$ frames; at 60 Hz that is a third of a second, at 200 Hz a tenth. If the HUD still flickers, lower $\alpha$.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_3_3.unity` with:

- **Floor** — a 90 m stone plane, static.
- **XR Origin (XR Rig)** at z = −8 facing the yard, with the **XR Interaction Simulator**.
- **Ruins Walls** — three 40 m walls across the yard at z = 5, 20, 35, each split by a 4 m doorway with a mossy lintel, plus two 50 m side walls. All static: these are your occluders.
- **Torches** — six point lights (range 9, real-time by default) on the walls. Static holders.
- **Pillars** — 80 pillars in 8 columns × 10 rows. Each root has children **LOD0** (base, shaft, capital, orb — 4 renderers), **LOD1** (shaft, capital — 2), **LOD2** (one block — 1) and carries `LodBuilder` with thresholds 0.40 / 0.15 / 0.03. Static.
- **Guide Statue** — a plinth and 48 spheres in a spiral: one heavy object with no LODs, for your third table row.
- **Perf** — carries `CullingReporter` (camera assigned) and `PerfProbe` (reporter assigned, label "before", budget 13.9 ms) with a **Perf HUD** `TextMesh` parented to the camera.
- Two signs; warm sun; faint fog.

Scripts live in `Assets/Scripts/`: `LodBuilder.cs`, `CullingReporter.cs`, `PerfProbe.cs`.

## Your tasks (about 70 min)

**Task 1 (15 min) — Build the LOD groups** · file: `Scripts/LodBuilder.cs`, TODO 1–3
Collect each level's renderers with `transform.Find` + `GetComponentsInChildren<Renderer>`, describe them as `LOD` structs, then add a `LODGroup`, `SetLODs`, and `RecalculateBounds`. Because every pillar carries the script, Play builds all 80 groups at once. Try the context menu **Build LODs** on one pillar in Edit mode too — the group persists in the scene.
**Check:** the shimmering overlap is gone. Select a pillar during Play: the LODGroup shows three bars and highlights the active one as you walk away — LOD0 near, LOD1 around 10 m, LOD2 around 25 m, *Culled* far out. The Stats *Tris* count dropped.

**Task 2 (10 min) — The math behind the bars** · file: `Scripts/LodBuilder.cs`, TODO 4–5
Implement `ScreenRelativeHeight` and `DistanceForScreenHeight`. Add a temporary `Debug.Log` in some `Start()` printing the three switch distances for a 4.4 m pillar at 60° and at 95°, then remove it.
**Check:** `ScreenRelativeHeight(4.4f, 20f, 60f)` is about 0.19; the printed switch distances match the Math foundation (9.5 / 25 / 127 m on desktop). Walk to a pillar and confirm the LOD0→LOD1 switch happens near 9–10 m in the simulator.

**Task 3 (10 min) — Count what is drawn** · file: `Scripts/CullingReporter.cs`, TODO 1–2
Gather all renderers in `Start`, then every quarter second build the frustum planes and count renderers whose bounds pass `TestPlanesAABB`, and separately those with `isVisible`.
**Check:** the total is about 630. Facing the yard, in-frustum and visible are close. Turn around to face the empty side: both fall to a handful. Stand at the doorway sign facing Wall A: in-frustum is still high (the pillars behind it are inside the pyramid), visible is a bit lower thanks to LOD culling only — the occlusion bake in Task 5 will change that.

**Task 4 (10 min) — The frame-time HUD and snapshots** · file: `Scripts/PerfProbe.cs`, TODO 1–3
Smooth `unscaledDeltaTime` into milliseconds, write the two-line HUD (red above budget), and call `Snapshot()` on **P**. Maximize the Game view, stand at the doorway sign facing Wall A, wait two seconds, press P. Do the same at the rig start facing the yard, and in front of the statue. Label is "before" for all three.
**Check:** three rows appear in the Console and in `perf_3_3.csv` (the path is printed; on Windows it is under `%USERPROFILE%\AppData\LocalLow\...`, on macOS under `~/Library/Application Support/...`).

**Task 5 (10 min) — Bake occlusion culling** · no new code
Stop Play. Everything is already static, but confirm: select **Pillars** → Inspector → the *Static* dropdown shows *Occluder Static* and *Occludee Static* ticked. **Window → Rendering → Occlusion Culling** → *Bake* tab → set *Smallest Occluder* to 2 (the wall thickness is 0.6 but the doorway is 4 m; 2 is a good compromise) → **Bake**. With the window open, the Scene view's *Occlusion Culling* overlay lets you pick *Visualization* and see culled objects vanish as you move the camera. Then Play, set *Snapshot Label* on **Perf** to `occlusion`, repeat the three snapshots.
**Check:** at the doorway facing Wall A, *visible* drops sharply while *in-frustum* is unchanged — that gap is occlusion culling. In the open yard the two numbers stay close, because nothing hides anything.

**Task 6 (15 min) — Bake lightmaps** · no new code
Select **Directional Light** and each **Torch Light** (or the whole *Torches* parent's lights via the Hierarchy) and set *Mode* to **Baked** for the torches and **Mixed** for the sun. **Window → Rendering → Lighting** → *Scene* tab → *New Lighting Settings* → **Lightmapper: Progressive GPU** (or CPU) → *Lightmap Resolution* 10 texels per unit for a fast first bake → untick *Auto Generate* → **Generate Lighting**. This takes one to three minutes on a laptop. Then Play, label `occlusion+lightmaps`, repeat the three snapshots, and fill in the table below.
**Check:** the torches still light the walls but the Lighting window's *Realtime Lights* count for the torches is 0; soft shadows appear under the capitals and the statue's beads. Frame time falls most in the torch-lit corners. Record your screencast here — show the HUD in all three spots, before and after.

**Before/after table** (copy into your Padlet post; one row per object, read the values from the HUD or CSV while standing at the same spot each time):

| Object | Renderers before → after LOD | Visible renderers before → after bake | Frame ms before → after | Notes |
|---|---|---|---|---|
| Pillar 37 (mid yard) | 7 → | | | LOD thresholds used: 0.40 / 0.15 / 0.03 |
| Long Wall A (from the doorway sign) | 1 → 1 | | | occluder; visible count is what changed |
| Guide Statue | 49 → | | | no LODs yet — stretch goal adds them |

## Stretch goals

- Give the **Guide Statue** LODs: make LOD1 a plinth plus 12 beads and LOD2 a single capsule, then measure again.
- Enable **GPU Instancing** on the stone material (material Inspector → *Enable GPU Instancing*) and compare *Batches* in the Stats overlay with and without it.
- Add `LODGroup.fadeMode = LODFadeMode.CrossFade` with `animateCrossFading = true` in `BuildLods` and judge whether the pops at the switch distance are less noticeable in the simulator.

## Assets you will need

Everything is listed with licenses in `Docs/FreeAssets.md`. For today the primitives are enough, but to practice on a real model bring one of these:

- **Kenney Castle Kit** or **Kenney Dungeon Kit** (CC0): a wall, a pillar, and a stair piece. Kenney's models are already low-poly, so use them to practice *Generate Lightmap UVs* (model Import Settings → Model tab) and *Static* flags rather than LOD reduction.
- **Quaternius Ultimate Fantasy** or **Modular Ruins** pack (CC0): a statue or column with 2–5k triangles — a good LOD0. Build LOD1 and LOD2 from a Kenney low-poly piece of the same silhouette, or from a scaled primitive.
- **Poly Haven** (CC0): one 2K stone texture set for the pillars if you want the lightmap bake to show detail; keep everything on one material so batching still works.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. Occlusion data and lightmaps are part of the scene and build with it. Two headset notes: the vertical field of view is about 95°, so every LOD switch happens at roughly half the desktop distance — walk to a pillar on the headset and check that LOD1 does not appear too close; and the *Stats* overlay is not available in the build, so your `PerfProbe` HUD *is* your instrument there. The P key does nothing on the headset; call `Snapshot()` from a controller button via an `InputActionReference` later, or read the HUD on video.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: a pillar changing LOD as you walk away, the Perf HUD at the doorway sign before and after the occlusion bake, the lightmapped yard with baked torches, and your filled-in table.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus the biggest single win you measured (in ms or renderers) and what caused it.
- (optional) The `perf_3_3.csv` your probe produced.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Pillars still shimmer with all three levels visible | `BuildLods` returned early because `RenderersMissing` was true: TODO 1 still assigns an empty array, or a child is not named exactly `LOD0`/`LOD1`/`LOD2` |
| LODGroup shows three bars but the pillar disappears up close | Thresholds are in the wrong order — they must decrease (0.40, 0.15, 0.03); a larger value later in the array is invalid |
| `isVisible` never drops below in-frustum after the bake | The Scene view is open and counts as a camera — maximize the Game view; or the bake produced no data (check the Occlusion Culling window shows a size in MB) |
| Occlusion bake finishes instantly with 0 bytes | No object is marked *Occluder Static*; select **Ruins Walls** and set the Static dropdown to *Everything* |
| Lighting bake never finishes or the Editor freezes | Lightmap resolution is too high for a first pass; set 5–10 texels per unit and *Lightmap Size* 1024, and make sure only static objects are included |
| Torches still cost frame time after the bake | Their *Mode* is still *Realtime* or *Mixed*; set the six torch lights to *Baked* and Generate Lighting again |
| HUD says a few hundred fps and is unreadable | VSync is off in the Editor; **Edit → Project Settings → Quality → VSync Count → Every V Blank**, or accept the number and compare relative changes |

Created by Isac Artzi

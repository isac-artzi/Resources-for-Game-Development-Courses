# Activity 7.5 — Performance Pass for Quest 3

Topic 7: VR Game Development Project · Week 15, Tuesday · Stepping stone to Milestone 7 — Final Integration

## Where this fits

Thursday you present. Between now and then the one thing that can still sink the demo is frame rate: a build that judders on the Quest feels broken no matter how good the quest logic is. Today you learn to *find* the cost before you *guess* at it. A `PerfBudgetChecker` HUD in front of the camera shows smoothed frame time against the 13.9 ms budget and turns yellow, then red. Two editor audits — `BatchingAudit` and `TextureAudit` under **Activity → Audit** — list the materials that stop batching, the lights that cast shadows, and the textures that are too large or not compressed for Android. The starter scene is deliberately wasteful so the reports have something to say; the last task is to fix it and measure the difference. Then you run the same three tools on your team's integrated Milestone 7 project.

*Elaria hook:* the caretaker of the ruins sweeps the courtyard every dawn. "Thirty kinds of rubble, forty pillars carved one by one, six torches burning all night — no wonder the place groans. Sort it. Then it will stand."

## Learning goals

- You can state the Quest 3 frame budget at 72, 90 and 120 Hz and explain why the usable budget is smaller than 1000/Hz.
- You can measure frame time in code with an exponential moving average and read a HUD without leaving the headset.
- You can explain draw calls, batching (static, dynamic, GPU instancing) and what a single-use material costs.
- You can compute texture memory from width × height × bits per pixel, and explain why ASTC 6×6 is nine times smaller than RGBA32.
- You can write an Editor-only script guarded with `#if UNITY_EDITOR` and expose it as a menu item.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-7.5-performance-pass`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup** — two `OK` lines.
6. **Activity → Build Starter Scene**. This one also generates two PNG textures under `Assets/Textures` (a 2048 px mural and a 512 px tile) — give it a few seconds.
7. Press **Play**. A ruined courtyard: pillars, rubble, torches, a mural on the far wall. Top-left of your view is an empty HUD label. Open **Window → Analysis → Profiler** too, and turn on the **Stats** overlay in the Game view (the *Stats* button) — you will compare your HUD against it.

Simulator controls that matter today: **Tab** to the head, **W A S D** and mouse to look around the courtyard (the visible-renderer count depends on where you look). **F1** hides and shows the HUD. The audits are menu items — **Activity → Audit → Batching Audit** and **Texture Audit** — and print to the Console and to `Audit/*.txt` next to the `Assets` folder.

## VR theory

**The budget.** The Quest 3 refreshes at 72 Hz by default: 13.9 ms per frame for *everything* — your scripts, physics, culling, both eyes' rendering, and the system compositor that warps the image to the lenses (about 1–2 ms you never see). Miss the deadline and the runtime shows the previous frame again, reprojected to the new head pose: the world judders and objects near you smear. That is why the HUD warns at 85 % of the budget, not at 100 %. Braun and Rizzo's Chapter 9 puts optimization at the end of the pipeline, after integration — exactly where you are — because only the integrated scene has the real cost.

**CPU-bound or GPU-bound?** The CPU prepares draw calls (one per material per batch, roughly), runs your `Update`s and physics; the GPU shades pixels. Mobile chips like the Quest's are usually **GPU-bound** on fill rate (two eyes × high resolution × overdraw from transparent particles and overlapping geometry) and **CPU-bound** on draw-call count when a scene has hundreds of separate materials. The Profiler tells you which: if *CPU main thread* is near the budget you have too many calls or too much script work; if *GPU* is, you have too many pixels, lights, or shadow passes. Today's HUD only shows the total — the Profiler is where you go next.

**Batching.** Unity merges draw calls when renderers share a material. *Static batching* pre-combines meshes marked *Static* at build time (costs memory, saves calls). *Dynamic batching* merges small moving meshes on the CPU each frame (limited to ~300 vertices). *GPU instancing* draws many copies of the same mesh with the same material in one call — but only if the material has *Enable GPU Instancing* ticked and the objects are not static. A material used by exactly one renderer can never batch with anything; thirty of them cost thirty calls where one shared material (or a texture atlas, which lets many objects share one material) would cost one or two.

**Textures and memory.** GPU memory on the Quest is shared with the whole system; a handful of uncompressed 2048 px textures can push a scene into stutter. **ASTC** (Adaptive Scalable Texture Compression) packs blocks of pixels into fixed 128-bit chunks: a 6×6 block is 36 pixels in 128 bits, or 3.56 bits per pixel, against 32 for RGBA32. Set it project-wide in *Player Settings → Texture compression format → ASTC* and per texture in the *Android* tab of the importer. The other lever is *Max Size*: a texture on a 0.5 m prop never needs 2048 px in VR — it will rarely fill more than a few hundred pixels of the eye buffer.

**Lights, shadows, MSAA, foveation.** Each real-time light adds cost per pixel it touches; each shadow-casting light re-renders every caster into a shadow map. Budget one shadow-casting light (the sun) and bake the rest. **MSAA 4×** is nearly free on the Quest's tiled GPU and removes the shimmer that ruins VR; keep it on. **Fixed foveated rendering** shades the edges of each eye at lower resolution where the lens is blurry anyway — enable it through the OpenXR *Meta Quest Support* feature settings (the *Foveated Rendering* option; verify the exact name in your OpenXR version) for a free 10–20 % on GPU-heavy scenes.

## Math foundation

**Frame budget:** $T = 1000 / f$ ms. At 72 Hz, $13.9$ ms; 90 Hz, $11.1$ ms; 120 Hz, $8.3$ ms. Usable after ~1.5 ms compositor: about $12.4$, $9.6$, $6.8$ ms.

**Exponential moving average** of frame time with weight $\alpha$:

$$m_{n} = m_{n-1} + \alpha \,(x_n - m_{n-1})$$

which is `Mathf.Lerp(smoothed, ms, alpha)`. With $\alpha = 0.1$ a sudden jump from 5 ms to 15 ms shows as 6.0, 6.9, 7.7, … and reaches 90 % of the way after about 22 frames (0.3 s) — smooth enough to read, fast enough to notice. The *worst* value is kept separately so spikes are never averaged away.

**Texture memory:** $\text{bytes} = w \cdot h \cdot \text{bpp} / 8$, times $4/3$ with a full mip chain.

Worked example, the mural: $2048 \times 2048 \times 32 / 8 = 16{,}777{,}216$ bytes $= 16$ MB, with mips $\approx 21.3$ MB. As ASTC 6×6: bpp $= 128 / 36 = 3.56$; $2048^2 \times 3.56 / 8 = 1.87$ MB, with mips $\approx 2.5$ MB. Set *Max Size* to 1024 as well and it is $0.62$ MB — a 34× reduction with no visible difference at 3 m.

**Draw-call arithmetic.** The starter courtyard, standing at the origin: 3 walls (1 shared material, static → ~1 call), 40 pillars (1 material, instancing off, not static → up to 40 calls), 30 rubble blocks (30 materials → 30 calls), mural + tile (2), floor (1): roughly **75 calls** for a nearly empty scene. After the fix — instancing on the pillar material, rubble on one shared material — it is about **8**. On the Quest a comfortable ceiling is around 100–150 calls per frame, so a scene that starts at 75 with placeholder cubes has already spent half its budget.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_7_5.unity` with:

- A 40 m **Floor**, three static **Walls** sharing one material, warm sun and thin dusty fog; the **XR Origin (XR Rig)** 6 m back with the simulator.
- **Pillars** — 40 identical cylinders, *not* static, sharing the `Pillar` material with **GPU Instancing off** (an instancing candidate).
- **Rubble** — 30 cubes, each with its own material `Unique_00` … `Unique_29` (thirty single-use materials).
- **Torch 0–5** — six point lights with **Soft shadows** on.
- **Mural** — a 6 × 3 m quad with `BigMural.png` (2048 px, default import: max size 2048, Android not overridden) and a **Floor Tile** quad with `SmallTile.png` (512 px), both generated into `Assets/Textures`.
- **Perf HUD** — a `TextMesh` parented to the camera at the top-left of the view carrying `PerfBudgetChecker` (*Target Hz* 72).

Scripts live in `Assets/Scripts/`: `PerfBudgetChecker.cs` (runtime), `BatchingAudit.cs` and `TextureAudit.cs` (editor-only, guarded by `#if UNITY_EDITOR`, reachable under **Activity → Audit**).

## Your tasks (about 70 min)

**Task 1 (15 min) — The HUD** · file: `Scripts/PerfBudgetChecker.cs`, TODO 1–4
Measure `Time.unscaledDeltaTime` in ms, smooth it with `Mathf.Lerp(smoothed, ms, smoothing)`, keep the worst value, count the scene once a second with `CountScene()`, write the four lines, and color them with `ColorFor`.
**Check:** the HUD reads a steady frame time close to the *Stats* overlay's, green. Set *Target Hz* to 500 — red; back to 72 — green. Turn away from the courtyard: *visible renderers* drops.

**Task 2 (15 min) — Batching audit** · file: `Scripts/BatchingAudit.cs`, TODO 1–4
Group `MeshRenderer`s by `sharedMaterials`, list single-use materials, group by `sharedMesh` to find non-static groups of five or more whose material has `enableInstancing == false`, and list shadow-casting lights.
**Check:** **Activity → Audit → Batching Audit** prints ~33 unique materials, thirty `single-use material: Unique_xx` lines, one `instancing candidate: Cylinder x40` line, and six `shadow-casting light: Torch` lines. The same text is in `Audit/batching_report.txt`.

**Task 3 (15 min) — Texture audit** · file: `Scripts/TextureAudit.cs`, TODO 1–4
Find every `Texture2D` under `Assets`, read `maxTextureSize` and the Android platform settings, flag anything over 1024 or not ASTC, estimate memory with `BitsPerPixel`, and print the rows largest first with a total.
**Check:** **Activity → Audit → Texture Audit** shows `!! BigMural 2048x2048 max 2048 Android project default ~21.33 MB` and `SmallTile … ~1.33 MB` (or 0.33 MB if mips are off).

**Task 4 (20 min) — Fix it and measure** · no new code
Write down the HUD's frame time and the Stats *Batches* count. Then: (a) select the `Pillar` material and tick **Enable GPU Instancing**; (b) select all 30 **Rubble** objects and drag `RuinStone` onto them; (c) on **Torch 1–5** set *Shadow Type* to *No Shadows*; (d) select `BigMural.png`, set *Max Size* 1024, and on the **Android** tab tick *Override* and choose **ASTC 6x6**. Re-run both audits and read the HUD again.
**Check:** the batching report has no single-use materials and no instancing candidates, one shadow light remains, the texture report shows the mural at ~0.6 MB, and the Stats *Batches* count fell from roughly 75 to under 15. Record the before/after numbers for your Padlet post.

**Task 5 (5 min) — Point the tools at your project** · no new code
Copy the three scripts into your team's Milestone 7 project (they have no scene dependencies) and run both audits on your integrated scene. Note the three worst offenders.

## Stretch goals

- Add triangle counting to the HUD: sum `mesh.sharedMesh.triangles.Length / 3` for visible `MeshFilter`s once a second (cache it per mesh — `triangles` allocates).
- Extend `TextureAudit` to auto-fix: for flagged textures set `maxTextureSize = 1024`, override Android to `TextureImporterFormat.ASTC_6x6`, and call `importer.SaveAndReimport()` — behind an `EditorUtility.DisplayDialog` confirmation.
- Log the HUD's smoothed and worst ms every 5 s to a CSV (as in Activity 2.4) and graph a 3-minute walk through your project.

## Port to Quest 3

This activity is *about* the Quest, so build it. Disable the **XR Interaction Simulator** GameObject, follow `Docs/PortingToQuest3.md`, and confirm *Texture compression format = ASTC* in Player Settings before the build. On the headset the HUD is your only readout: walk to the mural and to the torches and watch the color. Then connect the Profiler (*Window → Analysis → Profiler → attach to the Android player*) to see whether the red is CPU or GPU. Try the fixed-foveated-rendering option under the OpenXR *Meta Quest Support* feature and compare.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the HUD in the wasteful scene, both audit reports in the Console, the four fixes being applied, and the HUD and *Stats* afterwards.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus the before/after numbers (frame time, batches, mural MB) and the three worst offenders in your team project.
- (optional) The two `Audit/*.txt` reports from your team project.

## Troubleshooting

| Symptom | Fix |
|---|---|
| **Activity → Audit** menu is missing | A compile error somewhere (Console), or the `#if UNITY_EDITOR` / `#endif` pair is broken in one audit file |
| HUD text is empty or the placeholder | TODO 3 never assigns `hud.text`; or `hud` is null — it should be the `TextMesh` on the same object |
| HUD frame time is much lower than the Stats overlay | The Editor's Stats includes editor overhead; both are indicative only — the headset number is the one that counts |
| Texture audit finds nothing | `FindAssets` searched the wrong folder — pass `new[] { "Assets" }`; or every hit had no `TextureImporter` (fonts, render textures are skipped) |
| Batching audit says the pillars are not a candidate | You marked them static, or the `Pillar` material already has instancing on (that is the fix — good) |
| After the ASTC override the mural looks unchanged (or blocky) in the Editor | The Editor previews the format of the *active build target*; switch the target to Android (File → Build Profiles) to preview it, and judge quality on the headset |

Created by Isac Artzi

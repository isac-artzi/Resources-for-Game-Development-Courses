# Activity 3.1 — Plant the Forest

Topic 3: 3D Modeling and VR Environments · Week 5, Tuesday · Stepping stone to Milestone 3 — Environment Creation

## Where this fits

Milestone 3 asks for three environments built from free assets, optimized, and walked through on video. Today you build the tool that makes the first of them — the Enchanted Forest — possible without placing three hundred trees by hand. By the end of class you will have a scatter script that plants prefabs inside a circle with random rotation and size while keeping the hero's path clear, a wind sway that makes foliage breathe, a scale gauge that turns "this tree looks tiny" into an exact import scale factor, and a ground material built from real PBR textures. Everything starts with greybox primitives so you can test the code first; the last two tasks swap in free assets from `Docs/FreeAssets.md`.

*Elaria hook:* the Mysterious Guide leads you to the edge of the Enchanted Forest and stops. "The old woods remember their shape. Speak the words, and they will grow again — but leave the path. Travelers must always find the path."

## Learning goals

- You can explain the difference between a mesh, a material, a shader, and a texture map, and point to each in the Inspector.
- You can import a free 3D model, diagnose a wrong import scale from its renderer bounds, and fix it in the model's Import Settings.
- You can generate uniformly distributed random points in a disk and explain why the naive method clumps.
- You can compute the distance from a point to a line segment and use it to exclude a region.
- You can build a PBR material from albedo, normal, and roughness maps and tile it on the ground.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-3.1-plant-the-forest`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → import **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. Menu bar → **Activity → Check Setup**. Both lines should say `OK`.
6. Menu bar → **Activity → Build Starter Scene**. The scene `Activity_3_1` opens. Notice a new folder `Assets/Prefabs` with three greybox prefabs.
7. Press **Play**. Sixty props appear — all stacked at the center of the clearing. That pile is your "before": `RandomPointInDisk` still returns zero.

Simulator controls that matter today: **Tab** to select the head, **W A S D** to walk along the path, **mouse** to look, **Q/E** (or the vertical keys shown on the simulator panel) to rise above the canopy and look down at the scatter pattern — that top-down view is how you will judge Task 1 and Task 3. Press **R** during Play once Task 5 is done to grow a new forest.

## VR theory

A **mesh** is the geometry: a list of vertices and the triangles that connect them. A **material** is a set of parameters that says how a surface responds to light; it references a **shader**, the GPU program that actually computes the color of each pixel. **Texture maps** are images the material samples: the **albedo** (base color) map says what color the surface is with no lighting applied, the **normal** map fakes small bumps by bending the surface normal per pixel, and the **roughness** (or its inverse, smoothness) map says how sharp reflections are. Braun and Rizzo cover this pipeline in Chapter 8 of *XR Development with Unity*; Unity's Standard and URP Lit shaders implement it.

**Physically based rendering (PBR)** is the convention that those maps describe real-world properties in a consistent way, so a texture set downloaded from Poly Haven looks right under any lighting. That is why free PBR textures are so useful: no tweaking per scene. In VR the normal map matters more than on a monitor because you see surfaces stereoscopically and up close; a flat albedo-only ground reads as painted cardboard.

A **prefab** is a saved GameObject hierarchy you can instantiate many times. Edit the prefab asset and every copy updates. The scatter tool only works because trees are prefabs; a forest of 300 unique GameObjects is not something anyone should maintain by hand.

**Import scale and pivots** are the two things most likely to be wrong with a downloaded model. Modeling tools disagree about units: Blender exports in meters, many older FBX files are in centimeters, and some artists model in inches. Unity's model importer has a *Scale Factor* and a *Convert Units* toggle; fixing scale there is better than scaling the Transform, because colliders, physics, and the scatter tool then see the right size. The **pivot** is the model's local origin. For anything that stands on the ground — trees, rocks, lamp posts — the pivot must be at the base, or your scatter tool will bury half of every tree. If a downloaded model's pivot is at its center, parent it under an empty GameObject and offset the child upward; that empty becomes the prefab root.

**Polygon budget** is a VR-specific constraint. The Quest 3 is comfortable around 500 000 to 1 000 000 triangles per frame. A forest of 300 trees at 2 000 triangles each is 600 000 before you add anything else, which is why this course points you to low-poly kits (Kenney, Quaternius) and why Activity 3.3 adds LODs.

## Math foundation

**Uniform random point in a disk.** If you pick a radius $r$ uniformly in $[0, R]$ and an angle $\theta$ uniformly in $[0, 2\pi)$, you get a clump: the ring between $r$ and $r + dr$ has area proportional to $r$, so a uniform $r$ puts too many points near the center. Half your trees land inside $r < R/2$, which holds only a quarter of the area. The fix is to sample the *area* uniformly, which means the cumulative distribution of $r$ must be $r^2/R^2$. Inverting it:

$$r = R\sqrt{u}, \qquad \theta = 2\pi v, \qquad u, v \sim \text{Uniform}(0,1)$$

Then $x = r\cos\theta$, $z = r\sin\theta$. Worked example with $R = 25$: $u = 0.25$ gives $r = 25 \times 0.5 = 12.5$ m, not $6.25$ m. Exactly a quarter of the trees fall inside that radius, matching a quarter of the area.

**Point-to-segment distance** for the path exclusion. For segment $A \to B$ and point $P$ (all on the ground plane), project $P$ onto the line:

$$t = \frac{(P - A)\cdot(B - A)}{\lVert B - A\rVert^2}, \qquad t \leftarrow \text{clamp}(t, 0, 1), \qquad C = A + t\,(B - A), \qquad d = \lVert P - C\rVert$$

The clamp is what makes it a *segment*: without it, trees beyond the ends of the path would also be excluded, along the infinite line. Worked example: $A = (0, -10)$, $B = (0, 15)$, $P = (3, 4)$ in $(x, z)$. $B - A = (0, 25)$, $P - A = (3, 14)$, so $t = (0 \cdot 3 + 25 \cdot 14) / 625 = 0.56$, $C = (0, 4)$, $d = 3$ m. With `pathHalfWidth = 2.5` that tree is allowed; at $P = (2, 4)$ it is not.

**Scale factor** from the gauge: $k = h_{\text{target}} / h_{\text{measured}}$. A tree measuring 0.13 m that should be 8 m needs $k = 61.5$. If the model was authored in centimeters you will see $k \approx 100$ or $0.01$; inches give $k \approx 39.4$ or $0.0254$. Those round numbers tell you which unit the artist used.

**Random scale** uses `Random.Range(minScale, maxScale)`; with 0.8–1.25 a 7 m tree ranges from 5.6 to 8.75 m. Averaging two draws gives a bell-shaped spread — most trees near 1.0 — which looks more natural than a flat distribution.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_3_1.unity` with:

- **Floor** — a 60 m grass plane, static.
- **Forest Path** — a 3 m × 25 m dirt strip from z = −10 to z = 15 with two empties, **Path Start** and **Path End**, marking the segment to keep clear.
- **XR Origin (XR Rig)** on the path at z = −6, with the **XR Interaction Simulator**.
- **Old Forest Edge** — fourteen static primitive trees in a ring about 27 m out, for enclosure.
- **Scattered Forest** — an empty GameObject carrying `RandomScatter`. Its *Prefabs* array holds Greybox Tree (twice), Greybox Rock, and Greybox Bush; *Path Start/End* and a 2.5 m half-width are set; *Count* is 60 and *Radius* 25.
- **Assets/Prefabs/** — `Greybox Tree` (trunk + two crowns, about 7.7 m, carries `WindSway`), `Greybox Rock`, `Greybox Bush` (carries `WindSway`). All have their pivot at the base.
- **Scale Gauge** — a glowing 2 m post (the height of a tall adult) and **Scale Test Subject**, a copy of the tree prefab at scale 0.02 (13 cm tall) carrying `ScaleNormalizer` with *Target Height* 8 m and its *Readout* label assigned.
- Two signs, a warm sun, and light green fog.

Scripts live in `Assets/Scripts/`: `RandomScatter.cs`, `WindSway.cs`, `ScaleNormalizer.cs`.

## Your tasks (about 70 min)

**Task 1 (10 min) — Spread the forest** · file: `Scripts/RandomScatter.cs`, TODO 1
Implement `RandomPointInDisk` with $r = R\sqrt{u}$, $\theta = 2\pi v$. Press Play and fly up above the clearing. Then, as an experiment, temporarily remove the `Mathf.Sqrt` and look again — you should see the central clump the math section warned about. Put the square root back.
**Check:** props cover the disk evenly out to 25 m; the Console reports "Placed 60 of 60".

**Task 2 (10 min) — Turn and resize them** · file: `Scripts/RandomScatter.cs`, TODO 3–4
Replace the identity rotation with a random yaw and the scale of 1 with a random multiplier between *Min Scale* and *Max Scale*. Keep the `prefab.transform.localScale * scale` line so the multiplier respects whatever size the prefab already has.
**Check:** two copies of the same tree are rotated differently and one is visibly taller than the other; set *Min Scale* and *Max Scale* both to 1 and they match again.

**Task 3 (15 min) — Keep the path clear** · file: `Scripts/RandomScatter.cs`, TODO 2
Implement `DistancePointToSegment` (flatten y, project, clamp $t$, measure to the closest point). `Scatter()` already skips any candidate closer than *Path Half Width*. Walk the path from the rig to Path End.
**Check:** a clean 5 m corridor runs the full length of the dirt strip and nothing spawns on it; beyond Path End (z > 15) trees may sit on the extended line — that is the clamp doing its job. Set *Path Half Width* to 0 and trees return to the path.

**Task 4 (10 min) — Wind** · file: `Scripts/WindSway.cs`, TODO 1–3
Give every tree a position-based phase, then tilt it about an axis perpendicular to the wind by `sin(t) * maxAngle` on top of its rest rotation. Add the gust term. Because `WindSway` lives on the prefab, every scattered tree and bush sways.
**Check:** trees lean with the wind and back once every 2.5 s, neighbours out of step; trunks stay planted. Bushes sway faster and further (their prefab values differ).

**Task 5 (5 min) — Re-scatter on demand** · file: `Scripts/RandomScatter.cs`, TODO 5
In `Update()`, when `Keyboard.current.rKey.wasPressedThisFrame`, bump the seed and call `Scatter()`. Null-check `Keyboard.current`.
**Check:** each press of R grows a different forest; the path is clear every time.

**Task 6 (10 min) — The scale gauge** · file: `Scripts/ScaleNormalizer.cs`, TODO 1–3
Measure the test tree's height from its combined renderer bounds, compute the factor to reach 8 m, and print the report to the label. With *Apply On Start* on, the tree corrects itself when Play begins.
**Check:** the label first reads about "0.13 m tall … multiply scale by 61", and the test tree grows to tower over the 2 m post. In the Inspector its scale is now about 1.23 (0.02 × 61).

**Task 7 (10 min) — Real trees and real ground** · no new code
Import a nature kit (see *Assets you will need*). Drag one tree into the scene next to the Scale Gauge, add `ScaleNormalizer` to it with *Apply On Start* off, press Play, and read the factor. Select the model file in the Project window and set **Import Settings → Model → Scale Factor** to that number (or tick *Convert Units* if the factor is ~100 or ~0.01), then *Apply*. Make a prefab from the corrected tree (drag it into `Assets/Prefabs`), check its pivot is at the base, add `WindSway`, and replace the greybox entries in *Scattered Forest → Prefabs* with your tree, a rock, and a bush. Finally, create a material named `ForestGround`, assign a Poly Haven or ambientCG albedo to *Albedo/Base Map*, its normal map to *Normal Map* (accept the "fix now" prompt), set *Tiling* to about 12 × 12 for a 60 m floor, and drop it on **Floor**.
**Check:** the forest is made of your imported trees at 6–10 m; the ground shows visible detail up close and does not obviously repeat from eye height. Record your screencast here.

## Stretch goals

- Bell-shaped scale: average two `Random.Range` draws (or three) and compare the forest to the flat version.
- Density falloff: multiply the accept probability by `1 - r/R` so the clearing is sparse in the middle and dense at the edge — the classic forest-edge look.
- Add a second, smaller exclusion: a Transform array of "keep clear" spots (the Guide, the altar) with a per-spot radius, reusing `Vector3.Distance` on the flattened positions.

## Assets you will need

Everything is listed with licenses in `Docs/FreeAssets.md`. For today:

- **Kenney Nature Kit** (kenney.nl → Assets → Nature Kit, CC0): download the FBX or glTF folder and import 8–10 pieces — three trees, two rocks, two bushes, a stump, a flower or mushroom. Kenney models are authored in meters at a small "toy" scale, so expect a factor between 2 and 5 rather than 100.
- **Quaternius Ultimate Nature Pack** (quaternius.com, CC0): stylized trees and pines with more shape variety, glTF; pivots are at the base.
- **Poly Haven** or **ambientCG** (CC0): one ground texture set at 2K — search "forest floor", "moss", or "grass" — download the albedo (diffuse), normal (OpenGL/GL variant), and roughness maps as JPG or PNG.

Keep every downloaded file inside `Assets/ThirdParty/<source name>/` and note the source in your GDD credits section.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. Before building, open **Window → Analysis → Rendering Statistics** (the *Stats* button in the Game view) and note the triangle count with your real trees; if it is above about 800 000, lower *Count* on Scattered Forest or pick a lighter tree. The R key does nothing on the headset — that is expected; the forest scatters on start.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the top-down view of an evenly scattered forest with a clear path, trees swaying, the scale gauge correcting the test tree, and your imported trees on the PBR ground.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus the import scale factor you found for your tree and which unit you think the artist used.
- (optional) A screenshot of the clump you got when you removed the square root.

## Troubleshooting

| Symptom | Fix |
|---|---|
| All props still pile at the center after Task 1 | `RandomPointInDisk` still ends with `return Vector3.zero;` — return the computed vector; make sure you did not call `Mathf.Cos` with degrees |
| Trees float or sink | The prefab's pivot is not at its base. Parent the model under an empty GameObject, move the child up so its feet sit at y = 0, and make the empty the prefab root |
| Trees spawn on the path | `DistancePointToSegment` still returns `float.MaxValue`, or you forgot to flatten y — the path empties sit at y = 0 while the prop point may not |
| Whole tree slides sideways when swaying | Same pivot problem as above, or you multiplied `restRotation * AngleAxis` in the wrong order |
| Gauge says 0 m tall | The model has no `Renderer` under it (an empty prefab), or `MeasureHeight` still returns 0 |
| Imported model is pink | The FBX brought materials with a shader Unity does not have; select the model → Materials tab → *Extract Materials*, then set each to Standard/URP Lit and reassign the textures |
| `Keyboard.current` is null | The project is set to the old input backend; **Edit → Project Settings → Player → Active Input Handling → Input System Package (New)** or *Both*, then restart |

Created by Isac Artzi

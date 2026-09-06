# Activity 3.2 — Mountain Pass: Terrain and Weather

Topic 3: 3D Modeling and VR Environments · Week 5, Thursday · Stepping stone to Milestone 3 — Environment Creation

## Where this fits

The second Milestone 3 environment is the Mountain Pass, and mountains are the one place where hand-placed meshes lose to Unity's Terrain system: a heightmap you sculpt with brushes, painted with tiling texture layers, with a collider for free. Today the builder generates a 200 m terrain with a valley climbing toward the pass so you can start immediately, and you write the atmosphere on top of it: a weather controller that blends fog, light, wind, and snow between Clear, Overcast, and Blizzard; an altitude fog that thickens as you climb; and a snow emitter that rides above the player. Then you sculpt a switchback, paint real rock and snow textures, and carry the terrain into your project.

*Elaria hook:* the Mountain Hermit lives above the cloud line and does not come down. The Guide warns you: "The pass changes its mind. Clear when you set out, blind by the third cairn. Learn to read the weather, or the weather will read you."

## Learning goals

- You can explain when to use a Terrain and when to use meshes, and what a heightmap, a terrain layer, and a splatmap are.
- You can sculpt and paint a terrain with the Raise/Lower, Set Height, Smooth, and Paint Texture tools.
- You can state the exponential fog formula, solve for the density that gives a chosen visibility, and check the answer in the scene.
- You can blend a bundle of visual parameters between presets over time with `Lerp` and `SmoothStep`, and explain why easing matters in VR.
- You can configure and drive a `ParticleSystem` from script through its module structs.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-3.2-mountain-pass`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → import **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. Menu bar → **Activity → Check Setup**. Both lines should say `OK`.
6. Menu bar → **Activity → Build Starter Scene**. The scene `Activity_3_2` opens; the terrain data is saved to `Assets/Terrain/MountainPass.asset`.
7. Press **Play**. You stand at the bottom of a gray valley; six cairns lead up toward a lit hut. The sky and fog are clear.

Simulator controls that matter today: **Tab** to select a controller, **mouse** to aim, and the simulator's teleport action (the panel lists the key for *Teleport* — by default the grip/trigger combination shown on screen) to jump up the valley; the whole terrain is a TeleportationArea. Select the head with **Tab** and use **W A S D** plus the vertical keys to fly if you want to check the summit quickly. Keys **1 / 2 / 3** switch the weather once Task 3 is done.

## VR theory

A **Terrain** is a heightfield: a grid of height values (the **heightmap**) that Unity turns into a mesh on the fly, at higher detail near the camera and lower detail far away. Because every cell is one height, a terrain cannot have overhangs or caves — those are meshes. But for hills, valleys, and passes it gives you brush-based sculpting, automatic collision, and a painting system for **terrain layers**, each a PBR texture set tiled across the surface. Which layer shows where is stored in the **splatmap** (Unity calls it the alphamap): one weight per layer per cell, summing to one. The builder paints snow above roughly 55 % of the maximum height and rock below; you will repaint by hand. Braun and Rizzo discuss environment construction, including terrain and lighting choices, in Chapter 8 of *XR Development with Unity*.

**Fog** in Unity is a per-pixel blend toward a fog color based on distance from the camera. Three models exist. *Linear* fog fades between a start and end distance — simple but unphysical. *Exponential* fog multiplies visibility by $e^{-\rho d}$, which is how real haze behaves. *Exponential squared* uses $e^{-(\rho d)^2}$ and keeps the near field clear before falling off sharply. This activity uses exponential fog because its single parameter, the density $\rho$, is easy to reason about and to blend. In VR fog does three jobs: it hides the far clipping plane, it gives distant objects atmospheric depth so scale reads correctly, and — as a design tool — it sets difficulty and mood. A blizzard that cuts visibility to 5 m turns a walk into a search.

**Particles as weather.** Snow, rain, dust, and ash are thousands of tiny billboards (camera-facing quads). Simulating them across a 200 m terrain would be wasteful; nobody sees snow 150 m away in a blizzard. The standard trick is an emitter box that follows the player's head, so a few thousand particles always fall where the player looks. Set the system's *Simulation Space* to World so moving the emitter does not drag falling flakes with it.

**Blending, not switching.** Any instantaneous change to the whole view — sky, fog, light — is jarring on a monitor and can be unpleasant in a headset, where the scene is your entire visual field. Weather should roll in over several seconds. Easing (a `SmoothStep` on the blend parameter) makes the start and end of the change gentle, so it feels like weather rather than a light switch.

**Visibility and comfort.** Dense fog also reduces the number of visible references, and players with weak spatial anchoring can feel less stable. Keep a few high-contrast landmarks (the cairns, the hut's lantern) visible even in the worst weather, and never make fog so dense that the player's own hands vanish.

## Math foundation

**Exponential fog.** The fraction of the scene color that survives at distance $d$ with density $\rho$ is

$$f(d) = e^{-\rho d}$$

so at $f = 0.5$ (half the object, half fog) the visibility distance is $d_{50} = \ln 2 / \rho \approx 0.693 / \rho$. Solving the other way: for "50 % visibility at 40 m", $\rho = 0.693 / 40 = 0.0173$. Worked check with the presets: Clear ($\rho = 0.006$) gives $d_{50} = 115$ m; Blizzard ($\rho = 0.060$) gives 11.6 m; tripled by the altitude factor at the summit, $\rho = 0.18$ and $d_{50} = 3.9$ m.

**Altitude ramp.** `Mathf.InverseLerp(low, high, y)` returns $(y - \text{low}) / (\text{high} - \text{low})$ clamped to $[0, 1]$. With low = 8 m, high = 40 m: at $y = 24$ m the factor is $0.5$; the density becomes $\rho_{\text{base}} \times (1 + 0.5 \times 2) = 2\rho_{\text{base}}$.

**Blend with easing.** The blend parameter advances linearly: $b \leftarrow b + \Delta t / T$ with $T$ = `transitionSeconds`. The eased value is $s = \text{SmoothStep}(0, 1, b) = 3b^2 - 2b^3$. At $b = 0.25$, $s = 0.156$; at $b = 0.5$, $s = 0.5$; at $b = 0.75$, $s = 0.844$. Each parameter is then `Lerp(a, b, s)`. Colors lerp per channel with `Color.Lerp`.

**Heightmap.** The builder samples a function at 129 × 129 points: a `SmoothStep` ramp along $z$ rising to 60 % of the 60 m maximum, plus $0.10\,\sin(3\pi u)\cos(2.5\pi v)$ hills, plus ridges $0.22 \times \text{SmoothStep}(0,1, 2.2\,|u - 0.5|)$ on both sides of the valley. Heights are stored as fractions 0..1 of *Terrain Height* (60 m) — the number you see in the Set Height tool is meters, the number in `SetHeights` is a fraction.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_3_2.unity` with:

- **Mountain Pass Terrain** — 200 × 200 m, 60 m tall, centered on the origin, static, with a `TeleportationArea`. Two placeholder layers, *Rock* and *Snow* (solid-color PNGs in `Assets/Terrain`), painted by altitude.
- **XR Origin (XR Rig)** on the valley floor at about z = −70, with the **XR Interaction Simulator**.
- **Cairns** — six stone markers along the route, and the **Hermit's Hut** with a lantern and the glowing Mountain Hermit at the top of the pass.
- **Snow** — a `ParticleSystem` (world space, box shape 30 × 30 m, slow fall, emission 0) carrying `SnowEmitter` with *Target* pointed at the rig's camera.
- **Weather** — an empty GameObject carrying `WeatherController` (Sun and Snow assigned, three presets pre-filled, Start Weather Clear) and `AltitudeFog` (Weather assigned, 8–40 m ramp, ×3 at the top).
- **Altitude HUD** and **Weather HUD** — two small `TextMesh` labels parented to the camera, assigned to the two scripts' *Readout* fields.
- A welcome sign, a cool sun, and clear-day fog.

Scripts live in `Assets/Scripts/`: `WeatherController.cs`, `AltitudeFog.cs`, `SnowEmitter.cs`.

## Your tasks (about 70 min)

**Task 1 (15 min) — Blend two presets** · file: `Scripts/WeatherController.cs`, TODO 1–2
Implement `Blend` so every field of the preset is interpolated, then in `Update()` advance `blend` by `Time.deltaTime / transitionSeconds`, ease it with `Mathf.SmoothStep`, and `Apply` the blended preset every frame. To test before you have keys, press Play, select **Weather** in the Hierarchy, and use the three-dots menu on the `WeatherController` component → **Next weather** (it cycles Clear → Overcast → Blizzard).
**Check:** each *Next weather* rolls the fog color, density, and sun intensity to the next preset over about 4 seconds with no pop; the Console prints `[Weather] -> Overcast`, then `-> Blizzard`. If it snaps at the halfway point, `Blend` still has its placeholder.

**Task 2 (10 min) — Drive the snow** · file: `Scripts/SnowEmitter.cs`, TODO 3–4; `Scripts/WeatherController.cs`, TODO 4
Implement `SetIntensity` (copy the emission module, set `rateOverTime`) and `SetWind` (velocity-over-lifetime x), then call both from `WeatherController.Apply`. Temporarily set *Start Weather* to Blizzard again.
**Check:** the air fills with sideways-streaking snow at Play; with Clear there is none. Set Start Weather back to Clear.

**Task 3 (10 min) — Keys 1 / 2 / 3** · file: `Scripts/WeatherController.cs`, TODO 3
Read `Keyboard.current` (null-check) and call `SetWeather(0/1/2)` on the digit keys. `SetWeather` already restarts the blend from the current look, so mashing keys mid-transition stays smooth.
**Check:** pressing 3 rolls a blizzard in over 4 s — fog color, sky, sun, and snow all change together — and 1 clears it. The Weather HUD names the preset.

**Task 4 (10 min) — Follow the head** · file: `Scripts/SnowEmitter.cs`, TODO 1–2
Find the camera if no target is set, then in `LateUpdate` place the emitter above the head and a few meters ahead along the flattened view direction.
**Check:** teleport to the third cairn in a blizzard — snow is still falling around you; look straight down and flakes pass below your feet.

**Task 5 (10 min) — Fog that climbs** · file: `Scripts/AltitudeFog.cs`, TODO 1–5
Read the head's altitude, convert it to a 0..1 factor with `InverseLerp`, multiply the weather's base density, write it to `RenderSettings.fogDensity`, and fill the HUD with altitude, multiplier, and the 50 % visibility distance. Implement both static helpers.
**Check:** on a Clear day the HUD at the bottom reads roughly *Altitude 4 m · Fog ×1.00 · 50 % visibility 115 m*; at the hut it reads about ×3 and 38 m, and the far ridges have gone soft. Press 3 at the top and visibility drops to about 4 m — the lantern is your only landmark.

**Task 6 (15 min) — Sculpt and paint** · no new code
Select **Mountain Pass Terrain**. In the Inspector's terrain toolbar choose **Paint Terrain → Raise or Lower Terrain** and carve a switchback: two or three hairpin shelves about 3 m wide zig-zagging up the steepest stretch between cairns 3 and 5 (hold Shift to lower). Use **Set Height** with a fixed value to flatten each shelf and **Smooth Height** on the edges so teleporting onto them works. Then **Paint Texture → Edit Terrain Layers → Add Layer**, create a *Gravel* layer from a downloaded texture set (albedo in *Diffuse*, normal in *Normal Map*, tile size 4–6 m) and paint it along your path; replace the *Rock* and *Snow* placeholders' textures the same way. Finally, drop 4–6 low-poly pines from a nature kit near the bottom cairns for scale.
**Check:** you can teleport up your switchback shelf by shelf; the path reads as a path from 30 m away; textures do not obviously tile at eye height. Record your screencast here — start at the bottom in Clear, switch to Blizzard on the way up.

## Stretch goals

- Add a fourth preset, *Dusk*, with a warm orange fog color and a low sun angle; blend the sun's rotation too (`Quaternion.Slerp` on `sun.transform.rotation`).
- Make the weather change itself: every 30–60 s pick a random preset (`Random.Range`) — but never jump straight from Clear to Blizzard; go through Overcast.
- Sample the terrain under the player with `Terrain.activeTerrain.SampleHeight(position)` and show the *height above ground* on the HUD instead of the world altitude.

## Assets you will need

Everything is listed with licenses in `Docs/FreeAssets.md`. For today:

- **ambientCG** (CC0): three terrain texture sets at 2K — search "Rock" (e.g., a gray cliff), "Snow" (fresh snow), and "Gravel" or "Ground" for the path. Download the JPG bundle: color, normal (GL), roughness.
- **Poly Haven** (CC0): an alternative source for the same three sets, plus an HDRI sky if you want to try a skybox on a Clear day (the fog color must then match the horizon).
- **Kenney Nature Kit** or **Quaternius Ultimate Nature Pack** (CC0): 4–6 pine trees and a few rocks for the tree line near the bottom cairns. Fix the import scale with the gauge from Activity 3.1.

Terrain textures should be *seamless* (tileable); both sites mark theirs as such. Keep downloads under `Assets/ThirdParty/<source name>/`.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. Two mountain-specific notes. Terrain is expensive on mobile GPUs: in the terrain's settings (the gear tab) set *Pixel Error* to 8–15 and *Base Map Distance* to about 200, and keep the layer count at four or fewer. Particles are fill-rate heavy: cap *Max Rate* on the Snow emitter at 300–500 and reduce *Max Particles* to 2000 for the headset build. Keys 1/2/3 do nothing on the headset — add a button later, or leave *Start Weather* on the preset you want to show.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: a teleport climb from the valley to the hut with the Altitude HUD changing, a weather change from Clear to Blizzard mid-climb, snow following you, and your sculpted switchback with painted textures.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus the density you would use for 50 % visibility at 25 m (show your arithmetic).
- (optional) A screenshot of the terrain's Paint Texture layer list with your three real textures.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Weather snaps at the halfway point instead of blending | `Blend` still has the placeholder `return t < 0.5f ? a : b;` |
| Sky stays blue in the blizzard | `Apply` could not find the camera (`Camera.main` was null at Start) — make sure the rig's camera is tagged *MainCamera*, or fetch the camera lazily in `Apply` |
| No snow at all | Emission rate is 0 until `SetIntensity` is implemented and called from `Apply`; check that the *Snow* field on Weather is assigned |
| Snow stays behind when I teleport | `LateUpdate` in `SnowEmitter` is not moving the transform yet, or *Target* is empty and TODO 1 did not fall back to `Camera.main` |
| `ps.emission.rateOverTime = ...` does not compile | Modules are structs returned by value; copy into a local (`var em = ps.emission;`) and assign to the local |
| Fog barely changes when I climb | The head's y is below *Low Altitude* the whole way, or you added instead of multiplied — print `AltitudeFactor` to the HUD to see it |
| Terrain is bright magenta | The Rock/Snow placeholder PNGs failed to import; select them in `Assets/Terrain`, click *Reimport*, or replace them with real textures in the layer assets |

Created by Isac Artzi

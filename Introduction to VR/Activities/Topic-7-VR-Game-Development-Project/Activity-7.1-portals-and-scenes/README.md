# Activity 7.1 — Portals and Scene Flow

Topic 7: VR Game Development Project · Week 13, Tuesday · Stepping stone to Milestone 7 — Final Integration

## Where this fits

Until now every activity lived in one scene. Realm of Legends does not: it has a Forest, a Mountain Pass, and Ancient Ruins, and the player must move between them without the world freezing or snapping. By the end of class you will have a four-scene project — a Hub plus three destination stubs — where walking into a portal fades the view to black, loads the next scene asynchronously, and fades back in; a `GameManager` that survives every load and remembers which artifact pieces you have collected; and a status sign in every scene that reads the same count. That skeleton is exactly what Milestone 7 asks you to fill: replace the stubs with your real Topic 3 environments, keep the portals and the manager, and the integrated game exists.

*Elaria hook:* the Mysterious Guide opens three shimmering doorways in the mist. "Each leads to a piece of the artifact. The doorways remember nothing — but I do. Bring the pieces back to me here." The Hub is the Guide's memory made into a place.

## Learning goals

- You can explain what happens to every GameObject when a scene loads, and why a `DontDestroyOnLoad` singleton is the standard fix for state that must survive.
- You can load a scene asynchronously with `SceneManager.LoadSceneAsync`, hold activation until the view is black, and read progress from `AsyncOperation.progress`.
- You can build a trigger-driven portal that detects only the player's head and hands off to a persistent loader.
- You can keep per-scene objects (labels, collectibles) consistent with persistent state by checking that state when the scene starts.
- You can argue for a per-scene XR rig versus a persistent rig and name the trade-off of each.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-7.1-portals-and-scenes`). Open it with Unity **6000.5.x** and wait for packages to resolve.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** tab → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup** — two `OK` lines in the Console.
6. **Activity → Build Starter Scene**. This builder is bigger than usual: it creates and saves **four** scenes (`Activity_7_1`, `Forest_Stub`, `Mountain_Stub`, `Ruins_Stub`) under `Assets/Scenes`, adds all of them to **File → Build Profiles → Scene List**, and reopens `Activity_7_1` (the Hub). Check the Scene List: the Hub must be first.
7. Press **Play** in the Hub. You stand two meters from three glowing doorways. Walk into one now — nothing happens yet, because the TODOs are empty. That is the starting point.

Simulator controls that matter today: **Tab** to select the head, **W A S D** to walk, **mouse** to look. Everything today is triggered by walking your head into things; you will not need the controllers. If the mouse gets stuck, **Esc** releases it.

## VR theory

**Scene architecture.** A Unity scene is a saved list of GameObjects. `SceneManager.LoadScene` (in Single mode) destroys everything in the current scene and instantiates everything in the next — the camera, the lights, the XR rig, your scripts. Nothing carries over unless you arrange it. The three standard arrangements are: (1) a **persistent object** marked `DontDestroyOnLoad`, which Unity moves into a hidden scene that is never unloaded; (2) **additive loading** (`LoadSceneMode.Additive`), where a "bootstrap" scene holding the rig and managers stays open and environment scenes are loaded on top of it; and (3) **serialized state** written to disk and reloaded (that is Thursday's activity). Today you use (1) because it is the smallest change from a single-scene project and it is what most student projects need. Braun and Rizzo's Chapter 9 covers assembling a full project from parts; this is the plumbing that makes the parts one game.

**The singleton pattern** is the code shape for "exactly one of these, reachable from anywhere": a static `Instance` property plus an `Awake` guard that destroys duplicates. In a multi-scene game the duplicate case is not theoretical — it happens every time you return to the scene that contains the original object. The guard is what keeps the first manager, and its data, alive.

**Async loading and comfort.** On a monitor a half-second freeze during a load is a hiccup. In a headset it is a *tracking* freeze: your head keeps moving, the image does not, and the vestibular system registers the mismatch immediately. Two rules follow. First, never block the main thread with `LoadScene` on anything bigger than a stub — use `LoadSceneAsync`, which streams the scene in over several frames while the head keeps tracking. Second, change what the player sees only when they cannot see it: fade to black, load, activate the scene, fade in. Some games replace the black with a small "loading space" (a dark room with a floor and a slowly filling bar); the principle is the same — the head is always tracked, the world never snaps.

**Per-scene rig or persistent rig?** This project puts an XR Origin in every scene. It is simple, every scene can be pressed Play on its own, and each scene can pick its own spawn point. The cost is that anything attached to the rig — the fade quad, a wrist menu, an inventory — must exist in every scene, and the loader has to re-find it after each load (you will do exactly that in TODO 4 of `SceneLoader.cs`). The alternative is one persistent rig (`DontDestroyOnLoad` on the XR Origin, or a bootstrap scene with additive loads); then the rig's attachments persist for free but you must teleport the rig to a spawn point after each load and make sure no scene contains a second rig or a second `AudioListener`. Either is defensible for Milestone 7; pick one and write the reason into your GDD.

**Triggers and the head.** A `BoxCollider` with *Is Trigger* on fires `OnTriggerEnter` when another collider enters it — but only if one of the two has a `Rigidbody`. The builder therefore parents a small kinematic sphere, the *Head Probe*, to the XR camera. Your portal filters for it with `GetComponentInParent<Camera>()`, so a controller drifting through the doorway does not teleport the player.

## Math foundation

There is little arithmetic today; the numbers that matter are timings and a progress mapping.

**Progress normalization.** `AsyncOperation.progress` runs from 0 to **0.9** while the scene loads and jumps to 1.0 only after activation. To show a 0–100 % bar during the load, divide by 0.9 and clamp:

$$p = \min\left(1, \frac{\text{progress}}{0.9}\right)$$

Worked example: `progress = 0.63` → $p = 0.63 / 0.9 = 0.70$, i.e. 70 %.

**Fade timing.** A fade of duration $T$ seconds advances its parameter by $\Delta t / T$ per frame; the alpha is $a = \text{Lerp}(a_0, a_1, t)$. With $T = 0.4$ s at 72 Hz ($\Delta t \approx 0.0139$ s) the fade takes about 29 frames. Use `Time.unscaledDeltaTime` so a paused game (`Time.timeScale = 0`) cannot leave the player stuck in black. A full portal trip is therefore fade-out (0.4 s) + load (whatever it takes, hidden) + fade-in (0.4 s) — under a second for these stubs, which reads as a blink rather than a loading screen.

**Trigger sizing.** The portal trigger is 1.7 m wide, 2.6 m tall, and 1.2 m deep. At a walking speed of 1.5 m/s the head spends $1.2 / 1.5 = 0.8$ s inside it — plenty for one `OnTriggerEnter`. Make a trigger thinner than a frame's worth of travel ($1.5 \times 0.0139 \approx 0.02$ m) and fast players can tunnel through without ever being "inside".

## The starter scene

**Activity → Build Starter Scene** creates and saves four scenes under `Assets/Scenes`, all in the Scene List:

- **Activity_7_1 (the Hub)** — a 24 m stone floor in violet mist, the **XR Origin (XR Rig)** with the simulator, and three doorways in an arc: **Portal Forest**, **Portal Mountain**, **Portal Ruins**. Each doorway is two pillars, a lintel, a glowing *Surface*, a point light, a label, and a **Trigger** child that carries `Portal` (with *Target Scene* and *Display Name* already set).
- **Game Systems** (Hub only) — carries `GameManager` and `SceneLoader`. Its *Fade Renderer* already points at the Hub's fade quad.
- **Forest_Stub / Mountain_Stub / Ruins_Stub** — each a small world with its own rig, a ring of primitive props, an **Artifact Piece** (emissive cube on a pedestal, with a large trigger collider and `ArtifactPiece` set to `forest_piece`, `mountain_piece`, or `ruins_piece`), a **Return Portal** on your left targeting the Hub, a **World Sign**, and a **Status Sign**.
- In **every** scene the rig's camera has two children: **Head Probe** (kinematic trigger sphere on the *Ignore Raycast* layer) and **Fade Quad** (a 4 × 4 m black quad 0.5 m in front of the eyes, using the transparent `Fade` material, alpha 0).
- A **Status Sign** in every scene that `GameManager` rewrites after each load.

Scripts live in `Assets/Scripts/`: `GameManager.cs`, `SceneLoader.cs`, `Portal.cs`, `ArtifactPiece.cs`.

## Your tasks (about 70 min)

**Task 1 (10 min) — The one that survives** · file: `Scripts/GameManager.cs`, TODO 1
Turn `Awake` into a singleton guard: if a different `instance` already exists, destroy this GameObject and return; otherwise store `this` and call `DontDestroyOnLoad(gameObject)`. Leave the `Instance` getter alone — it already creates a manager when a stub is played by itself.
**Check:** press Play, look at the Hierarchy: `Game Systems` still sits in `Activity_7_1` (nothing has loaded yet). After Task 3 it will appear under a **DontDestroyOnLoad** heading and there will never be two of them.

**Task 2 (15 min) — Fade to black** · file: `Scripts/SceneLoader.cs`, TODO 1
Implement `Fade(from, to)` as a coroutine: accumulate `t` with `Time.unscaledDeltaTime / fadeSeconds`, call `SetFadeAlpha(Mathf.Lerp(from, to, t))`, `yield return null` until `t >= 1`, then snap to `to`. `SetFadeAlpha` is already written and uses `.material` so the shared `Fade` asset is never edited.
**Check:** press Play in the Hub — the world fades in from black over about half a second instead of popping in. Set *Fade Seconds* to 2 and it visibly slows.

**Task 3 (15 min) — Load without freezing** · file: `Scripts/SceneLoader.cs`, TODO 2–4
In `LoadRoutine`: fade out, then `SceneManager.LoadSceneAsync(sceneName)` with `allowSceneActivation = false`; loop while `op.progress < 0.9f`, writing `progress = op.progress / 0.9f`; then allow activation and wait for `isDone`. Finally call `FindFadeRenderer()`, set alpha to 1, and fade in. Remove the placeholder `Debug.Log`.
**Check:** temporarily call `GameManager.Instance.Loader.LoadScene("Forest_Stub")` from the `Start()` of `GameManager` (delete it afterwards) — the Hub fades out and the Forest fades in. The Console shows no *Scene ... couldn't be loaded* error.

**Task 4 (10 min) — The doorway** · file: `Scripts/Portal.cs`, TODO 1–2
In `OnTriggerEnter`, return unless `other.GetComponentInParent<Camera>()` is non-null and the portal is armed; then call `GameManager.Instance.Loader.LoadScene(targetScene)`. Watch the Console line so you know which portal fired.
**Check:** walk into each Hub portal — Forest, Mountain, Ruins each appear with their own floor color and fog. Walk into a stub's Return Portal — you are back in the Hub, and the Hierarchy shows exactly one `Game Systems` under *DontDestroyOnLoad*.

**Task 5 (15 min) — Pieces that stay collected** · files: `Scripts/GameManager.cs`, TODO 2–4 · `Scripts/ArtifactPiece.cs`, TODO 1–3
Implement `Collect` (no duplicates, refresh the label), subscribe to `SceneManager.sceneLoaded` so the label refreshes after every load, and implement `RefreshStatusLabel` with `GameObject.Find(statusLabelName)`. In `ArtifactPiece`, hide the piece in `Start` if the manager already has it, spin it in `Update`, and collect it in `OnTriggerEnter` with the same head filter as the portal.
**Check:** collect the Forest piece (walk through the glow) — it vanishes and the sign reads *1 / 3*. Return to the Hub: the Hub sign also reads *1 / 3*. Go back to the Forest: the piece is still gone. Collect all three and the Hub shows *3 / 3*.

**Task 6 (5 min) — Decide your rig strategy** · no code
Read *Per-scene rig or persistent rig?* above once more. Write one sentence in your Padlet bullets choosing per-scene or persistent for your team's Milestone 7 build, and one reason. If you finish early, try the portal surface pulse in `Portal.cs` TODO 3.

## Stretch goals

- Show load progress: add a `TextMesh` field to `SceneLoader`, and while the fade is black write `Mathf.RoundToInt(progress * 100) + " %"` to a small label parented to the camera (it must survive the load — so create it in code under the `Game Systems` object, not in a scene).
- Give each portal a spawn point: add a `Transform spawnPoint` name to `Portal`, and after the load move the new scene's `XROrigin` (`Object.FindFirstObjectByType<XROrigin>()`, namespace `Unity.XR.CoreUtils`) to a GameObject with that name.
- Convert to a persistent rig: mark the Hub's rig `DontDestroyOnLoad` in `GameManager.Awake`, delete the rigs from the three stubs, and note what breaks (hint: the fade quad now persists, but `Fade Quad` lookups still run).

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject in *every* scene (four of them), then follow `Docs/PortingToQuest3.md`. Check **File → Build Profiles → Scene List**: all four scenes ticked, Hub first — a scene missing from the list is the number one cause of "the portal does nothing" on the headset. Async loading matters more on Quest than on your laptop: the stubs are tiny, but when you swap in your Topic 3 environments a synchronous `LoadScene` would freeze tracking for a second or more.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the fade-in at start, a trip Hub → Forest → Hub with the Hierarchy visible (one `Game Systems` under *DontDestroyOnLoad*), and all three pieces collected with the sign reading *3 / 3* in the Hub.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus your per-scene vs persistent rig decision and one reason.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Portal does nothing, Console says *Scene 'Forest_Stub' couldn't be loaded* | The scene is not in **File → Build Profiles → Scene List**; rerun **Build Starter Scene** or add it by hand |
| Two `Game Systems` after returning to the Hub, count resets to 0 | TODO 1 in `GameManager.cs` is still the placeholder — the duplicate guard must `Destroy(gameObject)` |
| Fade never happens, world pops | `Fade` still snaps to the target alpha; or the `Fade Quad` was not found — check the Console warning and the quad's name |
| Screen stays black after arriving | You fade in before `FindFadeRenderer()`, so the *old* (destroyed) renderer is used; re-find first, then fade |
| Walking into the portal with a controller teleports you | The head filter is missing — `GetComponentInParent<Camera>()` must be non-null |
| Piece reappears after a round trip | `ArtifactPiece.Start` does not check `GameManager.Instance.HasPiece`, or `Collect` never added the id |

Created by Isac Artzi

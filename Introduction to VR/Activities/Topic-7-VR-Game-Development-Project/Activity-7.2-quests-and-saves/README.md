# Activity 7.2 — Quests and Saves

Topic 7: VR Game Development Project · Week 13, Thursday · Stepping stone to Milestone 7 — Final Integration

## Where this fits

On Tuesday you connected the worlds. Today you give the player a reason to travel between them: a quest. By the end of class you will have a `QuestManager` that walks a list of steps — Locked, Active, Done — and completes them from three different signals you already know how to produce: an XRI grab, an XRI socket, and a distance check. A label on your left wrist shows the steps and their markers, and a `SaveSystem` writes the whole state to a JSON file in `Application.persistentDataPath` so a playtester can put the headset down and press **Continue** tomorrow. For Milestone 7 you will attach your real objectives (the runestone puzzle, the Sage dialogue, the artifact pieces) to the same `Complete(id)` call, and the same save file will remember them.

*Elaria hook:* the Forest Sage will not speak until the shard rests on the altar. "Bring it, hero, and I will tell you where the second piece lies." Your wrist band glows with the Guide's handwriting: one line per task, the current one marked with an arrow.

## Learning goals

- You can model a quest as a list of steps with an explicit state machine and explain why exactly one step is Active in a linear quest.
- You can complete objectives from three kinds of signals — a UnityEvent on an interactable, a UnityEvent on an interactor, and a per-frame distance test — and say when each is the right tool.
- You can serialize a `[System.Serializable]` class to JSON with `JsonUtility`, write it to `Application.persistentDataPath`, and read it back.
- You can defend a save-file design choice: ids instead of indices, a version number, and what to do when the file is stale.
- You can build a wrist-anchored label and a world-space save panel that work with the XR ray and the simulator.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-7.2-quests-and-saves`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup** — two `OK` lines.
6. **Activity → Build Starter Scene** → `Activity_7_2` opens.
7. Press **Play**. Look down-left: the wrist label is on your left controller. Ahead: a shard on a pedestal (left), an altar (right), the Sage (far), and a blue save panel to your right.

Simulator controls that matter today: **Tab** cycles head / left controller / right controller. With a controller selected, **W A S D** and the mouse move it; **G** (grip) grabs and **G** again releases; while the controller's ray points at a UI button, **T** (trigger) clicks it. The simulator's on-screen panel lists the exact keys if your version differs. With the head selected, **W A S D** walks. Keyboard shortcuts the script gives you: **F5** save, **F9** continue, **Delete** new game — the same actions as the panel buttons, so you can test saving without aiming.

## VR theory

**Quest structures.** A *linear* quest is a sequence: step 2 unlocks when step 1 completes. A *hub* quest offers several steps at once in any order (collect three pieces, any order) and a final step that waits for all of them. A *branching* quest lets a choice close some steps and open others. Realm of Legends is a hub of three worlds, each with a small linear chain inside — which is why today's manager is linear and Tuesday's GameManager counted pieces in any order. Chapter 9 of Braun and Rizzo discusses assembling gameplay systems into a whole; the quest is the system that gives all the others a purpose.

**State machines.** Each `QuestStep` is in exactly one of three states: **Locked** (not yet available), **Active** (the current objective), **Done**. The transitions are the rules of the game: Locked → Active when the previous step completes, Active → Done when its objective fires. There is no Done → Active. Writing the states as an `enum` and the transitions in one method (`Complete`) means every rule lives in one place, which is what makes the quest debuggable when a tester says "it skipped a step".

**Signals: events versus polling.** XRI raises `selectEntered` on an interactable the frame it is grabbed and on a socket the frame something snaps in — you subscribe once and get called exactly when it matters. "Walked up to the Sage" has no such event, so you *poll*: test the distance every frame. Prefer events when the source already knows the moment; poll when the condition is continuous. In VR polling also has a comfort dimension — a proximity check must not fire when the player merely glances past; that is why the radius is horizontal and generous (1.5 m).

**Serialization** is turning an object graph into bytes and back. `JsonUtility` is Unity's fast, restricted serializer: public fields of `[System.Serializable]` classes, primitives, `Vector3`, and `List<T>` of those. It ignores properties, dictionaries, and polymorphism — limitations that push you toward the simple, flat `SaveData` you will write today. That flatness is a feature: a save file should be boring and inspectable in a text editor.

**Save design.** Three decisions matter more than the format. *Where*: `Application.persistentDataPath` is the only folder guaranteed writable on both your laptop and the Quest. *What key*: store step **ids**, not list positions, so that reordering steps in the Inspector next week does not corrupt every save. *What happens when the file is old*: a `version` field lets `Load` refuse a stale file loudly instead of loading half of it. Autosave on every completed step is the right default for a 10-minute VR session — nobody wants to hunt for a save button with a headset on.

**Body-anchored UI.** The wrist label is parented to the left controller, so it is where your hand is: glance down and read, lift the hand to bring it closer. It is *diegetic-ish* (it belongs to the hero's body, like a bracelet) and it never blocks the view the way a head-locked panel does. The save panel, by contrast, is a world-space canvas — a place you walk to — because saving is a meta action, not part of the fiction.

## Math foundation

**Horizontal distance for the Sage check.** With head position $h$ and Sage position $s$, drop the vertical component and test

$$d = \sqrt{(h_x - s_x)^2 + (h_z - s_z)^2} < r$$

Worked example: head at $(0.4, 1.6, 4.9)$, Sage at $(0, 1.0, 6.0)$: $d = \sqrt{0.16 + 1.21} = \sqrt{1.37} = 1.17$ m, which is inside $r = 1.5$ m — the step completes. Including $y$ would add $0.36$ and give $1.31$ m: still inside here, but a tall player or a crouch would change the answer for no good reason.

**Enum ↔ int.** `QuestStep.State` is `Locked = 0, Active = 1, Done = 2`. Saving `(int)step.state` writes `1`; loading `(QuestStep.State)1` reads `Active`. Never change the numeric values once save files exist.

**Order of states in a linear quest.** After $k$ completions the list reads: $k$ steps Done, one Active, $n - k - 1$ Locked. With $n = 3$ the only legal label patterns are `[>][ ][ ]`, `[x][>][ ]`, `[x][x][>]`, `[x][x][x]` — four states for three steps. If you ever see `[x][ ][>]`, `Complete` activated the wrong step.

**File size sanity.** Three ids of ~10 characters, three ints, a `Vector3`, a bool and some whitespace: under 400 bytes pretty-printed. A save file for the whole game with 20 steps and 10 collectibles is still under 2 KB — there is no reason to compress or to save rarely.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_7_2.unity` with:

- **Floor**, a ring of primitive **Trees**, sun and forest fog, the **XR Origin (XR Rig)** at the origin with the simulator.
- **Shard** — a 12 × 25 cm emissive cube with a `Rigidbody` and `XRGrabInteractable`, resting on **Shard Pedestal** at the left.
- **Altar** — a 1 m block on the right with an **Altar Socket** child: a 25 cm trigger sphere carrying `XRSocketInteractor` (hover meshes on) and an **Attach** point on top.
- **Forest Sage** — a glowing 2 m capsule 6 m ahead.
- **Wrist Label** — a small `TextMesh` parented to the rig's *Left Controller* (if the prefab has none, to the camera).
- **Save Panel** — a world-space canvas (1 px = 1 mm) with a title, a **Status** text, a **Continue Button** and a **New Game Button**; an **EventSystem** with `XRUIInputModule` is added so the XR ray can click.
- **Quest** — carries `SaveSystem` and `QuestManager`. The manager's list already has three steps (`take_shard`, `place_shard`, `meet_sage`) and every reference (shard, socket, Sage, wrist label, status text, buttons, save system) is set.
- Signs above each station saying what it is.

Scripts live in `Assets/Scripts/`: `QuestStep.cs` (data), `QuestManager.cs`, `SaveData.cs` (data), `SaveSystem.cs`.

## Your tasks (about 70 min)

**Task 1 (10 min) — Markers and the first step** · files: `Scripts/QuestStep.cs`, TODO 1 · `Scripts/QuestManager.cs`, TODO 1 and 5
Make `Marker()` return `[ ]`, `[>]`, `[x]` by state. In `QuestManager.Start`, set the first step Active if nothing is Active or Done, then implement `RefreshLabel`: a title line and one line per step (`marker + " " + title`), plus *Complete!* when all are Done.
**Check:** press Play and look at your left hand — three lines, the first marked `[>]`.

**Task 2 (15 min) — Events complete steps** · file: `Scripts/QuestManager.cs`, TODO 2 and 4
Subscribe in `Start`: `shard.selectEntered.AddListener(OnShardGrabbed)`, `altar.selectEntered.AddListener(OnAltarFilled)`, and the two buttons' `onClick`. Then implement `Complete(id)`: ignore it unless that step is Active; set Done; activate the first Locked step; refresh; autosave if enabled.
**Check:** Tab to a controller, walk it to the shard, press **G** — the label flips to `[x][>][ ]` and the Console prints `[Quest] take_shard done`. Carry the shard to the altar and release near the socket — it snaps in and the label reads `[x][x][>]`.

**Task 3 (10 min) — Walking up to the Sage** · file: `Scripts/QuestManager.cs`, TODO 3
In `Update`, if `meet_sage` is Active, compare the horizontal distance from the head to the Sage against `talkRadius` and call `Complete("meet_sage")` when inside.
**Check:** with the head selected, walk to the Sage — at about 1.5 m the last step completes and *Complete!* appears. Restart Play and walk to the Sage first: nothing happens, because the step is Locked.

**Task 4 (15 min) — Save to disk** · files: `Scripts/SaveSystem.cs`, TODO 1 · `Scripts/QuestManager.cs`, TODO 6
Implement `BuildSaveData` (ids, states as ints, shard position, `altar.hasSelection`) and `SaveSystem.Save` with `JsonUtility.ToJson(data, prettyPrint)` and `File.WriteAllText`, inside a try/catch.
**Check:** press **F5**. The status line on the panel says *Saved.* and shows the path. Open that file in a text editor: three ids, three ints, a position.

**Task 5 (15 min) — Continue** · files: `Scripts/SaveSystem.cs`, TODO 2–3 · `Scripts/QuestManager.cs`, TODO 7
Implement `Load` (return null if no file; `JsonUtility.FromJson<SaveData>`; refuse `version < SaveDataVersionRequired`) and `ApplySaveData` (match steps **by id**, cast ints back to the enum, move the shard to its saved position if it is not held or socketed, zero its velocity, refresh).
**Check:** grab the shard, drop it somewhere odd, **F5**, stop Play, Play again, press **F9** or click **Continue** — the label shows the saved markers and the shard lies where you dropped it. Click **New Game** — the file is deleted and the label resets.

**Task 6 (5 min) — Break the file on purpose** · no new code
Edit the JSON by hand: change `"version": 1` to `0` and press **F9** — your version check should refuse it. Then delete a closing brace and try again — the catch block should log instead of crashing. Record your screencast during Tasks 5–6.

## Stretch goals

- Do `SaveData.cs` TODO 1–2: add `savedAt`, bump `version` to 2, and show the timestamp on the status line.
- Make the wrist label face you: add a `LateUpdate` that rotates it toward `Camera.main` around its local up (the `LookAtPlayer` idea from Activity 1.1), so the text is readable however you hold the controller.
- Hub-style quest: add a `bool anyOrder` to `QuestManager`; when true, activate *all* Locked steps at start and add a final "Return to the Guide" step that only activates when the others are Done.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. Two things change on the headset. The save file lands in the app's private `files/` folder on the Quest — the status line shows the path, and you can pull it with `adb pull` or read it through **SideQuest → File Manager**. And there is no keyboard: **Continue** and **New Game** must be clicked with the controller ray, so make sure the panel is at a comfortable height (1.4 m) and that the `EventSystem` has the `XRUIInputModule`, not the standard one.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the wrist label changing as you grab, place, and walk to the Sage; a save (F5 or button), a restart, and a Continue that restores the markers and the shard; and the JSON file open in a text editor.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), and one sentence on which objective in your team's game will use events and which will use polling.
- (optional) Paste the contents of your `realm_save.json`.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Grabbing the shard does nothing to the label | TODO 2 listeners are not added, or `Complete` still has its placeholder — look for the `[Quest]` Console line |
| Shard falls through the altar or never snaps | Release it *inside* the 25 cm socket sphere; check the Altar Socket's collider has *Is Trigger* on and the socket has an *Attach Transform* |
| `[x][ ][>]` or another impossible pattern | `Complete` activated a step by index instead of "the first step that is still Locked" |
| Buttons do not react to the ray | The `EventSystem` needs `XRUIInputModule` (the builder adds one only if none existed — delete any extra EventSystem) |
| Status line says *Saved.* but no file appears | Look at the path printed — it is `persistentDataPath`, not the project folder; on Windows it is under `%USERPROFILE%\AppData\LocalLow\<Company>\<Product>` |
| Continue restores markers but the shard jumps into your hand or the socket | Only move the shard when it is not selected: check `shard.isSelected` before setting its position |

Created by Isac Artzi

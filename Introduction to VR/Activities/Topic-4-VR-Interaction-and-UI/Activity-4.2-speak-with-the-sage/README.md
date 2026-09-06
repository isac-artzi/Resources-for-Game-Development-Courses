# Activity 4.2 — Speak with the Sage: NPC Dialogue

Topic 4: VR Interaction and User Interfaces · Week 7, Thursday · Stepping stone to Milestone 4 — UI and Interaction Design

## Where this fits

By the end of class you will walk up a lantern-lit path to the Forest Sage, look at her, and a dialogue box will open above her head. Her words type themselves out letter by letter; two buttons offer you a choice; each choice leads to another line, and one of them ends the conversation. The whole exchange — six nodes, two branches, a loop back — is data you edit in the Inspector, not code you rewrite. Milestone 4 asks for dialogue boxes and NPC interactions in your Realm of Legends prototype; every NPC in your game (the sage, the mountain hermit, the caretaker of the ruins) can reuse today's three scripts with different words. On Tuesday you built the satchel; today the world starts talking back.

*Elaria hook:* the lanterns lead deeper than you expected. Where the moss begins to glow blue, a hooded figure waits between the trees. She does not call out. She only turns her head when you look at her — and then, as if she had been waiting all along, she speaks.

## Learning goals

- You can describe three NPC interaction models in VR (proximity, gaze, pointing) and justify combining proximity and gaze for a conversation trigger.
- You can test whether the player is looking at something using a dot product against a cosine threshold, without calling `Acos`.
- You can add hysteresis (enter at 3 m, leave at 4 m) to a trigger and explain why a single threshold flickers.
- You can implement a time-based typewriter reveal with `Substring` and explain how reading speed sets the number.
- You can represent a branching conversation as a flat array of nodes with next-indices, edit it in the Inspector, and validate branches so bad data cannot crash the game.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-4.2-speak-with-the-sage`). Open it with Unity **6000.5.x** and wait for the packages to resolve.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** tab → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup**. Both lines should read `OK`.
6. **Activity → Build Starter Scene**. The scene `Activity_4_2` is created and opened.
7. Press **Play**. It is dusk; lanterns flank a path to a hooded figure 5 m ahead. Nothing happens yet — the dialogue box is hidden until your trigger code opens it.

Simulator controls that matter today: **Tab** to select the *head*, then **W A S D** to walk and the **mouse** to look — the trigger uses where the head is and where it points. To click a choice button, Tab to a controller, aim its ray at the button with the mouse, and press the **Trigger** key shown on the simulator's on-screen panel. Until you have done Task 4 you can also press **1** and **2** on the keyboard to choose.

## VR theory

**How do you talk to an NPC in VR?** Flat games press *E*. VR has no *E* and, more importantly, has a body. Three models are common. **Proximity**: the conversation starts when you walk within a few meters — simple, but a player walking past to reach a door gets ambushed by dialogue. **Gaze**: the conversation starts when you look at the NPC — it reads as attention, the way a shopkeeper looks up when you make eye contact, but on its own it misfires when the NPC crosses your view from afar. **Pointing / touching**: the player aims a ray or reaches out — explicit and never accidental, but it needs a tutorial. Most VR adventures combine the first two: *close enough and looking* means "I want to talk". That is what `NpcTalkTrigger` implements, and adding the third as a stretch goal is a few lines.

**Where does the box go?** A dialogue box is spatial UI, so you place it in the world. Three rules from practice: put it *near the speaker* so the eye does not have to travel (above the head or beside the shoulder), keep it *1.2–2 m from the player's eyes* when they stand at talking distance (closer strains the eyes' convergence, farther shrinks the text), and make it *face the player, upright* (yaw only). Height matters: a box at 2.5 m when your eyes are at 1.6 m and you stand 3 m away is about 17° above eye level — a comfortable glance, not a neck crane. Braun and Rizzo discuss UI placement and text legibility in Chapter 8 of *XR Development with Unity*.

**Reading speed and the typewriter.** Adults read silently at roughly 200–250 words per minute — about 15–20 characters per second — and spoken dialogue runs slower, around 150 wpm. A typewriter reveal at 35–45 characters per second is faster than reading but slower than instant, which does two useful things: it paces the exchange so the NPC seems to *speak*, and it stops the player from skimming the last sentence first. It must be *skippable*: a second press finishes the line. Never make a player wait for text they have already read.

**Choices as affordances.** A button is a promise that something happens when you press it. A node with one way forward should show one button, centered, not two where one is blank. Labels should be the player's words ("I lost my way.") rather than verbs ("Ask about the pass"), because in VR the player *is* the hero speaking. Keep them short: at 24 px on a 0.72 m box they are legible at 3 m, but only if they fit on one line.

**Conversations as data.** A branching dialogue is a directed graph. Storing it as a flat array where each node names its successors by index is the simplest representation that a non-programmer can edit in the Inspector, and it is exactly how many shipped games do it under the hood (with a tool on top). The cost is that indices are fragile — insert a node in the middle and every reference shifts. The runner therefore *validates* every jump; a typo ends the conversation politely instead of crashing. In Topic 7 you will attach quest flags to these nodes.

## Math foundation

**Gaze cone test.** Let $\vec l$ be the head's forward direction and $\vec d$ the direction from the head to the NPC, both flattened to the horizontal plane and normalized. For unit vectors,

$$\vec l \cdot \vec d = \cos\theta$$

where $\theta$ is the angle between them. "The NPC is within a cone of half-angle $\alpha$" is therefore

$$\vec l \cdot \vec d \geq \cos\alpha$$

For $\alpha = 30°$, $\cos\alpha = 0.866$. Comparing dot products avoids `Acos` entirely — cheaper, and immune to the `NaN` that `Acos` returns when floating-point error pushes a dot product to 1.0000001.

Worked example: the head is at $(0, 1.6, 2)$ and faces $\vec l_{raw} = (0.34, -0.20, 0.92)$ (looking slightly down and a little right). The gaze target is at $(0, 1.5, 5)$. Flatten and normalize the look: $(0.34, 0, 0.92)$ has length 0.981, so $\vec l = (0.347, 0, 0.938)$. Direction to the target: $(0, -0.1, 3) \to (0, 0, 3) \to \vec d = (0, 0, 1)$. Dot product: $0.347 \cdot 0 + 0.938 \cdot 1 = 0.938$. Since $0.938 \geq 0.866$ the player is looking at the Sage — the angle is $\arccos(0.938) = 20.3°$, inside the 30° cone. Turn the head 40° to the side and $\vec l \approx (0.64, 0, 0.77)$, dot $= 0.77 < 0.866$: not looking.

**Horizontal distance.** $d = \lVert (x_{npc} - x_{head},\ 0,\ z_{npc} - z_{head}) \rVert$. Head at $(0, 1.6, 2)$, NPC at $(0, 0.9, 5)$: $d = 3.0$ m. Crouching to $y = 1.0$ leaves $d$ unchanged, which is why we drop $y$.

**Hysteresis.** Open when $d \leq 3$; close only when $d \geq 4$. Between 3 and 4 m the current state persists. Without the gap, a player standing at $d = 3.00 \pm 0.01$ would see the box open and close every frame as tracking noise crosses the line.

**Typewriter.** After $t$ seconds at rate $r$ characters/second, the number of visible characters is $n = \min(\lfloor r\,t \rfloor,\ L)$ for a line of length $L$. At $r = 40$, a 128-character line completes in $128/40 = 3.2$ s. At 72 Hz that is about one new character every 1.8 frames — smooth. If you revealed one character per *frame* instead, the same line would take 1.8 s at 72 Hz but 0.9 s at 144 Hz; tying it to time keeps it the same everywhere.

**Box height as an angle.** Box center at 2.55 m, eyes at 1.6 m, horizontal distance 3 m: elevation $= \arctan(0.95 / 3) = 17.6°$. Comfortable eye movement without head motion is roughly ±15–20°, so this sits at the edge — if your testers tilt their heads back, lower the box or move it toward the player.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_4_2.unity` with:

- **Floor** — a 30 m moss plane in dusk light with blue-grey fog; **Trees** — fourteen trunk-and-crown primitives around the grove.
- **XR Origin (XR Rig)** at the origin with the **XR Interaction Simulator**, and an **EventSystem** carrying `XRUIInputModule`.
- **Lanterns** — three pairs of iron posts with emissive flames and warm point lights along the path.
- **Glowing Moss** — an emissive disc off to the right, where the Sage says the forest shard sleeps.
- **Forest Sage** — a 1.8 m robed capsule with a hood and a soft violet glow, 5 m ahead. It carries **`NpcTalkTrigger`** with *Runner*, *Head*, *Nameplate* and *Gaze Target* (an empty at chest height) already assigned; *Talk Distance* 3, *Leave Distance* 4, *Gaze Half Angle* 30.
- **Sage Nameplate** — a grey `TextMesh` above her head that your code brightens when you are in range.
- **Sage Dialogue** — a world-space `Canvas` (720 × 400 px at scale 0.001 = 0.72 × 0.40 m) at 2.55 m height, with a *Background* panel containing *Name*, *Body*, *Choice A* and *Choice B* buttons. It carries **`DialogueRunner`** with every UI reference set and a six-node conversation already typed into *Nodes*. The Background starts hidden.
- **Hint Sign** and **Welcome Sign**.

Scripts live in `Assets/Scripts/`: `DialogueNode.cs` (data), `DialogueRunner.cs` (display and branching), `NpcTalkTrigger.cs` (when to open and close).

The flow at runtime: `NpcTalkTrigger.Update` measures distance and gaze every frame → calls `DialogueRunner.Open()` → `ShowNode(0)` copies the node's text into the typewriter and labels the buttons → `Update` reveals characters → a button click or a number key calls `Choose(i)` → `DialogueNode.Next(...)` returns the next index → `ShowNode(next)` or `Close()`. `LateUpdate` turns the box toward the player the whole time.

Open **Sage Dialogue** in the Inspector and expand *Nodes*. Read the six entries and trace the branches with a pencil: 0 → 1 or 2; 1 → 3 or 4; 2 → 1 or end; 3 → 5 or 4; 4 → 5 or 3; 5 → end. Nodes 3 and 4 point at each other — a loop the player can leave through node 5.

## Your tasks (about 70 min)

**Task 1 (10 min) — Distance and range** · file: `Scripts/NpcTalkTrigger.cs`, TODO 1
Compute the horizontal distance from the head to the Sage (subtract positions, zero the y component, take the magnitude) and store it in `currentDistance`. Nothing opens yet, but the Inspector shows the number.
**Check:** select *Forest Sage* while playing; *Current Distance* reads about 5.0 at the start and falls as you walk (head selected, W key).

**Task 2 (15 min) — Are you looking at her?** · file: `Scripts/NpcTalkTrigger.cs`, TODO 2
Flatten and normalize the head's forward and the direction to *Gaze Target*, take `Vector3.Dot`, and store it in `currentGazeDot`. The provided line compares it to `Mathf.Cos(gazeHalfAngle * Mathf.Deg2Rad)`.
**Check:** facing the Sage, *Current Gaze Dot* is 0.95–1.0. Turn the mouse 90° to the side: about 0. Turn your back: negative. At exactly 30° off it crosses 0.866.

**Task 3 (10 min) — Open, stay open, close** · file: `Scripts/NpcTalkTrigger.cs`, TODO 3–4
Open the runner when the box is closed, the distance is at most *Talk Distance*, and the player is looking; close it only when the distance reaches *Leave Distance*. Brighten the nameplate whenever the player is within talk distance.
**Check:** walk toward the Sage while looking at her — the nameplate turns yellow at 3 m and the box appears (showing the whole first line at once for now). Look away: it stays open. Back up: it closes at 4 m. Walk in *without* looking at her: nameplate yellow, no box, until you turn toward her.

**Task 4 (15 min) — Show a node and type it out** · file: `Scripts/DialogueRunner.cs`, TODO 1–2
In `ShowNode`, copy the node text into `fullText`, reset the timer and counter, clear `bodyText`, set the name and both choice labels, and hide *Choice B* when `HasChoiceB` is false (implement `HasChoiceB` in `DialogueNode.cs`, TODO 1). In `Update`, advance the timer and reveal `Mathf.Min(Mathf.FloorToInt(revealTimer * charsPerSecond), fullText.Length)` characters with `Substring`.
**Check:** the Sage's first line appears letter by letter over about 3 s; the buttons read *I seek the artifact shards.* and *I lost my way.* Change *Chars Per Second* to 10 while playing and re-enter range: it crawls.

**Task 5 (20 min) — Branch, validate, and face the player** · files: `Scripts/DialogueNode.cs` TODO 2, `Scripts/DialogueRunner.cs` TODO 3–5
Implement `DialogueNode.Next` (pick `nextA` or `nextB`, warn and return −1 if the index is out of range). In `Choose`, call it and either `ShowNode(next)` or `Close()`. Add the keyboard fallback: keys 1 and 2, where the first press finishes a line still typing and the next press chooses. In `LateUpdate`, rotate the canvas with `Quaternion.LookRotation(transform.position - head.position)` after zeroing y.
**Check:** press 1 mid-sentence — the line completes; press 1 again — node 1 appears. Choose *Tell me of the Mountain Pass.*, then *How do I wake the shard?*: you are looping between nodes 4 and 3. Choose *I understand.* → node 5 shows a single *Farewell.* button; pressing it closes the box. Walk a circle around the Sage with the box open: it turns to face you, text never mirrored. Set a node's *Next A* to 99 in the Inspector: choosing it prints a warning and closes instead of throwing. Record your screencast now: approach, the box opens, two branches, the farewell.

## Stretch goals

- Add the third interaction model: a `XRSimpleInteractable` on the Sage whose `selectEntered` (wired in the Inspector) calls `runner.Open()`, so pointing and clicking her also starts the conversation.
- Give each node an optional `AudioClip voiceLine` and play it through an `AudioSource` on the Sage when the node shows; set the typewriter speed from `clip.length` so text and voice finish together.
- Reuse `LookAtPlayer` from Activity 1.1 on the nameplate, and fade the box in and out over 0.3 s with a `CanvasGroup.alpha` lerp instead of a hard show/hide.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. On the headset, "looking at" is now a real head turn, so try a wider cone (40°) — people rarely center an NPC in their view. Clicking the choice buttons with the controller ray works as-is through `XRUIInputModule`; the keyboard fallback simply does nothing. Ask a classmate to try it without instructions and note whether the yellow nameplate was enough of a hint that the Sage can be spoken to.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: approaching the Sage until the box opens, the typewriter reveal, two different branches (including the 3 ↔ 4 loop), and the farewell closing the box.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus one sentence on what your own game's first NPC line will be and where its box will sit.

## Troubleshooting

| Symptom | Fix |
|---|---|
| **Activity** menu is missing | A compile error; open the Console, fix the red line, wait for the spinner to finish |
| The box never opens | `currentDistance` or `currentGazeDot` still hold placeholders (Tasks 1–2), or TODO 3 is not done; watch both values in the Inspector while you walk |
| The box opens and closes rapidly at the edge | *Leave Distance* is not larger than *Talk Distance*, or you used one threshold for both in TODO 3 |
| The whole line appears at once | TODO 1's placeholder still assigns `node.text` directly, or TODO 2 never runs (`IsOpen` false) |
| Text is mirrored or the box faces away | In TODO 5 you subtracted in the other order: it must be `transform.position - head.position` |
| Two buttons show on the farewell node | `HasChoiceB` still returns `true` (DialogueNode TODO 1) |
| Keys 1 / 2 do nothing | The scene lost focus — click the Game view first; also make sure you read `Keyboard.current` from the Input System, not the legacy `Input` class |
| Clicking a choice with the ray does nothing | The `EventSystem` needs `XRUIInputModule` and the canvas `TrackedDeviceGraphicRaycaster` (both built for you); check the ray actually reaches the button — the box is 2.5 m up |

Created by Isac Artzi

# Activity 4.3 — The Runestone Puzzle

Topic 4: VR Interaction and User Interfaces · Week 8, Tuesday · Stepping stone to Milestone 4 — UI and Interaction Design

## Where this fits

By the end of class you will have a complete VR puzzle: three glowing glyphs on the wall of a ruined chamber show an order — violet, cyan, amber — and three runestones wait on a side table. You grab a stone, carry it to the altar, and drop it near one of three sockets; the socket snaps it into place. When all three are seated, the altar light turns green and a chord plays if the order matches the glyphs, or the light turns red if it does not — and pulling any stone back out lets you try again. Milestone 4 asks for interactions with the environment and for iteration after user testing; this is the environment interaction, built as a proper state machine, with feedback on every channel. Thursday's Usability Lab will use *this* puzzle as the task your classmates perform, so build it clean.

*Elaria hook:* beneath the Ancient Ruins the caretaker has long since gone, but the altar remembers. "The runestones must be set in the order the glyphs show," the Sage told you. Tide, Ember, Dusk — three stones, one right sequence, and a door somewhere that listens.

## Learning goals

- You can use `XRSocketInteractor` as a *reporting* component — subscribing to its `selectEntered` / `selectExited` from a script — and explain how a socket differs from a plain trigger volume.
- You can design a puzzle as a small state machine (Waiting → Checking → Solved | Failed → Waiting) and draw its diagram.
- You can compare two sequences in code and explain why placement *order* is the whole state of this puzzle.
- You can explain the three rules of VR puzzle design — clarity, feedback, recoverability — and point to where each is implemented.
- You can compute how many orderings a sequence puzzle has ($n!$) and use that to reason about difficulty and guessing.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-4.3-runestone-puzzle`). Open it with Unity **6000.5.x** and wait for the packages to resolve.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** tab → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup**. Both lines should read `OK`.
6. **Activity → Build Starter Scene**. The scene `Activity_4_3` is created and opened.
7. Press **Play**. You are in a dim stone chamber. Ahead: an altar with three glowing rings and a blue light; on the wall behind it, three white cubes (they turn colored once your code runs); to your left, a wooden table with three white stones (same).

Simulator controls that matter today: **Tab** to a controller, aim with the **mouse**, move the hand with **W A S D** and the vertical keys, and hold the **Grip** key (see the simulator's on-screen panel) to grab a stone; release Grip with the stone inside a socket ring and the socket takes it. Grip again on a seated stone pulls it out. Move the *head* (Tab) when you need to see the glyphs and the altar at once.

## VR theory

**Sockets vs. triggers.** A trigger volume (Activity 4.1's satchel) only knows that a collider entered. An `XRSocketInteractor` is an *interactor* like a hand: it hovers an interactable that enters its trigger, shows a ghost mesh where the object will sit, and when the player releases the object it *selects* it — snapping the object to the socket's attach point and holding it there. When a hand grabs the object again, the hand's selection wins and the socket raises `selectExited`. Because a socket speaks the same interactor/interactable language as hands, you get snapping, hover preview, and hand-off for free, and your code only has to listen. Braun and Rizzo cover XRI's interactor family in Chapter 8 of *XR Development with Unity*; sockets are the piece you use for "put the thing in the place" everywhere in an adventure game — keys in locks, gems in pedestals, torches in sconces.

**Puzzle design in VR — three rules.** *Clarity*: the player must be able to discover what is being asked. Here the glyphs display the answer in the same colors as the stones, the socket rings glow, and the hover ghost shows where a stone will land. A puzzle whose rule is hidden is not a puzzle, it is a lottery. *Feedback*: every action gets a response — a tick when a stone seats, a light and a sound when the check runs, a color you can read from the doorway. In VR, players are often looking at their hands, so feedback must be redundant across channels (light *and* sound, near *and* far). *Recoverability*: a wrong move must be undoable without a menu. Pulling a stone out is a legal move that the controller hears about and that returns the puzzle to Waiting. Never leave a player stuck in a red-light state with no way out.

**State machines.** A state machine is a finite set of states, a current state, and rules for moving between them on events. It is the standard shape for puzzles, doors, NPCs and quests because it makes the question "what happens if the player does X *now*?" answerable by looking at one diagram instead of reading every `if` in the file. Our puzzle:

```
state      event                         next state
Waiting    stone placed, count < 3   ->  Waiting   (list grows)
Waiting    stone placed, count == 3  ->  Checking
Checking   sequences match           ->  Solved    (terminal)
Checking   sequences differ          ->  Failed
Failed     any stone removed         ->  Waiting   (list shrinks)
Waiting    stone removed             ->  Waiting   (list shrinks)
```

Draw it as circles and arrows before you code it — that sketch is one of your deliverables. Solved is terminal in this activity (placing or removing stones no longer changes anything). The states drive the feedback: the light color *is* the state, made visible.

**Order vs. position.** The plan for this puzzle says "in the order the glyphs show". That is a *sequence* puzzle: which socket a stone lands in does not matter, only the order the stones were seated. A *position* puzzle (left socket must hold Dusk, middle Tide, right Ember) is a different rule with a different data model — each socket would need its own expected id. Both are common; sequence puzzles feel like rituals, position puzzles like locks. You will implement the sequence version and can convert it to a position puzzle as a stretch goal.

**Error handling for players, not programmers.** A wrong order is not an exception — it is expected behavior and most players will hit it at least once. The `Failed` state exists so the game can respond gracefully: a short low sound, a red light, and immediate readiness to try again. Compare that with the alternative some prototypes ship: nothing happens at all, and the player does not know whether the puzzle is broken or they are.

## Math foundation

**Permutations.** The number of ways to order $n$ distinct stones is

$$n! = n \times (n-1) \times \cdots \times 1$$

Three stones: $3! = 6$ orders. Four: $4! = 24$. Five: $120$. A player who ignores the glyphs and guesses has a $1/6$ chance with three stones and $1/24$ with four. With three stones, brute force takes at most six tries of about ten seconds each — a minute — so the *clue* has to carry the puzzle, not the search space. Adding a fourth stone quadruples the search and is the single cheapest way to make guessing unattractive.

Listing the six orders of ids $\{0, 1, 2\}$: 012, 021, 102, 120, 201, 210. The target 201 (Dusk, Tide, Ember) is one of them.

**Sequence comparison.** Two sequences $a$ and $b$ match when they have the same length $L$ and $a_i = b_i$ for every $i \in [0, L)$. In code this is a length check followed by one loop that returns `false` at the first mismatch; the early return matters little for three elements but is the habit to build for the 200-step quest logs of Topic 7. Worked example: placed $[2, 0, 1]$ vs target $[2, 0, 1]$ — $i=0$: 2 = 2; $i=1$: 0 = 0; $i=2$: 1 = 1 → match. Placed $[2, 1, 0]$ — $i=0$ passes, $i=1$: 1 ≠ 0 → mismatch, stop.

**Undo as list removal.** `placedOrder` is a list of ids in arrival order. Removing a stone removes its id (`List.Remove` deletes the first equal element), so after seating Dusk, Ember, then pulling Ember back out, the list is $[2]$ and the next seating appends to it. Because ids are unique, "first equal element" is always the right one.

**The breathing pulse.** The glyph emission intensity while waiting is

$$I(t) = 1.5 + 1.0 \sin(2\pi f t)$$

with $f = 0.8$ Hz, so it swings between 0.5 and 2.5 once every $1/f = 1.25$ s. Emission color is the base color times $I$; HDR values above 1 read as glow with bloom and as a brighter tint without it.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_4_3.unity` with:

- **Chamber** — a 9 × 9 m room with 4 m walls, four pillars, two wall torches with warm point lights; dark fog and a dim sun so the emissive objects read.
- **XR Origin (XR Rig)** at (0, 0, −1.5) with the **XR Interaction Simulator**.
- **Altar** — a 1.4 × 0.9 × 0.6 m block 2.2 m ahead carrying **`PuzzleController`** (*Target Sequence* 2, 0, 1; *Stones*, *Sockets*, *Glyphs*, *Feedback* assigned) and **`PuzzleFeedback`** (*Altar Light*, *Glyphs*, *Audio Source* assigned; clips empty), plus an `AudioSource` and the blue **Altar Light**.
- **Socket 1–3** — children of the altar at x = −0.4, 0, 0.4 on its top: each has a `SphereCollider` (trigger, r = 0.12), an `XRSocketInteractor` with hover meshes on, a **`RuneSocket`** (*Controller* and *Socket Index* set), and a glowing **Socket Ring** visual without a collider.
- **Glyph 1–3** — three 0.32 m emissive cubes on the back wall at 2.1 m, tilted 45°. They start white; `ShowTarget` paints them.
- **Runestones** — *Tide* (id 0, cyan), *Ember* (id 1, amber), *Dusk* (id 2, violet) on the **Side Table** to your left: 0.12 × 0.16 × 0.12 m cubes with `Rigidbody`, `XRGrabInteractable` (kinematic, no throw) and **`Runestone`**. They also start white until `ApplyColor` runs.
- **Glyph Sign**, **Table Sign**, **Welcome Sign**.

Scripts live in `Assets/Scripts/`: `Runestone.cs` (identity), `RuneSocket.cs` (XRI → puzzle events), `PuzzleController.cs` (logic and state), `PuzzleFeedback.cs` (light, glyphs, sound).

The chain at runtime: the socket's `XRSocketInteractor` selects a stone → `RuneSocket.OnStoneEntered` finds the `Runestone` and calls `PuzzleController.OnStonePlaced` → the controller appends the id and, at three, calls `Evaluate` → `SequencesMatch` → `SetState(Solved | Failed)` → `PuzzleFeedback.SetState` colors the light and plays a clip. Removal runs the mirror path through `OnStoneExited` → `OnStoneRemoved`.

## Your tasks (about 70 min)

**Task 1 (10 min) — Stones that know what they are** · file: `Scripts/Runestone.cs`, TODO 1–2
In `ApplyColor`, tint the renderer's material *instance* (`rend.material`, not `sharedMaterial`) with the stone's color and set `_EmissionColor` to `color * 1.5f`. Implement `ToString` as `runeName + " (id " + runeId + ")"`.
**Check:** on Play the three stones on the table are cyan, amber, violet, left to right, and glow softly.

**Task 2 (15 min) — Sockets that report** · file: `Scripts/RuneSocket.cs`, TODO 1–3
Subscribe to the `XRSocketInteractor`'s `selectEntered` and `selectExited` in `OnEnable` (unsubscribe in `OnDisable`). In the handlers, get the `Runestone` from `args.interactableObject.transform.GetComponentInParent<Runestone>()`, ignore anything that is not a stone, update `currentStone`, log, and call the controller's `OnStonePlaced` / `OnStoneRemoved`.
**Check:** grab Tide, move it over the left socket ring — a ghost of the stone appears — release: it snaps in and the Console prints `Placed Tide (id 0) in socket 1`. Grab it again: *Current Stone* on Socket 1 goes empty.

**Task 3 (10 min) — Show the clue** · file: `Scripts/PuzzleController.cs`, TODO 1
In `ShowTarget`, look up `ColorForRune(targetSequence[i])` and paint glyph *i*'s material color and emission with it.
**Check:** the wall glyphs read violet, cyan, amber — Dusk, Tide, Ember. Change *Target Sequence* to 0, 1, 2 in the Inspector and re-enter Play: cyan, amber, violet.

**Task 4 (20 min) — Record, compare, decide** · file: `Scripts/PuzzleController.cs`, TODO 2–5
In `OnStonePlaced`, append the id, tick the feedback, and call `Evaluate` when the list is as long as the target. In `OnStoneRemoved`, remove the id and return to Waiting if the state was Failed. Implement `SequencesMatch` (null and length checks, then a loop). In `Evaluate`, set Solved or Failed based on the comparison and log both sequences with `string.Join`.
**Check:** seat Dusk, Tide, Ember in that order → *State* reads Solved and the Console says `Runestone puzzle SOLVED`. Restart Play and seat Tide, Ember, Dusk → Failed, with the log `Wrong order: 0,1,2 vs 2,0,1`. Pull Dusk out → Waiting; pull the others out, seat them in the right order → Solved.

**Task 5 (15 min) — Make it felt** · file: `Scripts/PuzzleFeedback.cs`, TODO 1–3
In `SetState`, choose the light color for the state and play the solved/failed clip; in `PlayClip`, null-check both source and clip before `PlayOneShot`. In `Update`, compute the pulse intensity (breathing sine while Waiting, 3 when Solved, 0.3 when Failed) and write `glyphBaseColors[i] * intensity` into each glyph's `_EmissionColor`.
**Check:** the altar light is blue and the glyphs breathe; a wrong order turns the light red and dims the glyphs; the right order turns it green and the glyphs lock bright. Optionally drop any two short clips (a click and a chime — see `Docs/FreeAssets.md`, Kenney Audio or Freesound) into *Place Clip* and *Solved Clip*. Record your screencast now: show the clue, one wrong order with recovery, then the solve.

## Stretch goals

- Convert to a *position* puzzle: give `RuneSocket` an `expectedRuneId` and have the controller check every socket's `currentStone.runeId` against it instead of comparing arrival order. Which version did your classmate find clearer?
- Add a fourth stone and socket and watch the glyphs, the target array, and the difficulty ($4! = 24$) all scale without touching the logic.
- Make Solved *do* something: raise a hidden wall section or open a door with a `SmoothStep` motion over 2 s (as in Activity 3.4), and add a reset lever that calls `ResetPuzzle()` and ejects the stones by re-enabling their `Rigidbody` gravity.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. Sockets behave identically on the headset; the thing to test is *reach* — the altar top at 0.9 m and the sockets 0.4 m apart are comfortable for a standing adult, but a seated tester may need the altar lower. If stones sometimes miss the socket, enlarge the `SphereCollider` radius to 0.15. Add a haptic pulse on seat using `HapticImpulsePlayer` from the hand that released the stone (verify on headset).

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the glyphs displaying the order, seating a wrong sequence (red light), recovering by removing a stone, and the solve (green light, glyphs locked bright).
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus your state diagram (a photo of a sketch is fine) and one sentence on which environmental puzzle in your own Realm of Legends will reuse this controller.

## Troubleshooting

| Symptom | Fix |
|---|---|
| **Activity** menu is missing | A compile error; open the Console, fix the red line, wait for the spinner to finish |
| Stones and glyphs stay white | `ApplyColor` (Runestone TODO 1) and `ShowTarget` (Controller TODO 1) are still placeholders |
| A stone hovers a ghost but never snaps in | You released it outside the sphere trigger — release while the ghost is visible; or the stone's `Rigidbody` is missing (the builder adds it) |
| Nothing prints when a stone seats | RuneSocket TODO 1 listeners not added, or TODO 2 returned early because `GetComponentInParent<Runestone>` found nothing — check the stone's `Runestone` component is on the same object as its collider |
| *Placed Order* grows but the state never changes | `Evaluate` is still the placeholder (TODO 5), or `SequencesMatch` still returns `false` (TODO 4), which also makes every order Failed |
| The right order shows Failed | `targetSequence` was edited; compare the Console's `Wrong order: ... vs ...` line — it prints both sequences |
| Light never changes color | `SetState` in `PuzzleFeedback` (TODO 1) is empty, or *Altar Light* is unassigned |
| Glyphs flicker or blow out white | The pulse intensity range is too large; keep it between about 0.5 and 3 |

Created by Isac Artzi

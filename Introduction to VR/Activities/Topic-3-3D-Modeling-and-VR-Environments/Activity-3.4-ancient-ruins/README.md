# Activity 3.4 — Ancient Ruins: Modular Kit and Mechanisms

Topic 3: 3D Modeling and VR Environments · Week 6, Thursday · Stepping stone to Milestone 3 — Environment Creation

## Where this fits

The Ancient Ruins are the third Milestone 3 environment and the one most likely to be built from a **modular kit**: wall, floor, pillar, and stair pieces that snap together on a grid. Today you assemble a ruins room from greybox modules, then make it *do* something — because an environment in a VR quest is not scenery, it is a machine the player operates. You write a lever that reads its own hinge angle and fires an event when pulled far enough, a stone door that slides open with eased motion, and a hidden chamber that reveals itself when the right artifact is placed on an altar. The wiring between them is UnityEvents, so tomorrow's designer can rewire the puzzle in the Inspector without touching code. Exam 1 covers this topic at the end of the week; the theory section is a good review.

*Elaria hook:* the caretaker of the ruins waits by a sealed door. "Every wall here answers to something," she says. "The lever answers to a hand. The altar answers to the sun. Bring it the wrong thing and it will tell you so."

## Learning goals

- You can assemble a room from modular pieces using Unity's grid snapping and explain why pivots and whole-unit sizes matter.
- You can constrain an XR grab interactable with a `HingeJoint` and read its angle as a normalized 0..1 value.
- You can fire a `UnityEvent` exactly once at a threshold using hysteresis, and wire listeners in the Inspector.
- You can animate a transform over a fixed duration with `MoveTowards` and `SmoothStep`, and explain events versus polling.
- You can subscribe to an `XRSocketInteractor`'s select events and validate what was placed with a marker component.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-3.4-ancient-ruins`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** → import **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. Menu bar → **Activity → Check Setup**. Both lines should say `OK`.
6. Menu bar → **Activity → Build Starter Scene**. The scene `Activity_3_4` opens.
7. Press **Play**. A sealed stone room stands ahead; a lever with a red lamp is to your right, an offering table with a gold disc and a gray stone to your left.
8. Select **Lever Handle** in the Hierarchy and look at the Inspector: `HingeJoint`, `XRGrabInteractable`, `LeverInteractable`. Expand *On Pulled ()* — it already lists `Stone Door → SlidingDoor.Open`. That is the wiring you will make work.

Simulator controls that matter today: **Tab** to select the right controller, move it with **W A S D** and the vertical keys until its ray or hand touches the lever handle, then hold **Grab** (the simulator panel shows the key, usually **G** or the mouse button) and move the controller forward and back to swing the handle. Release to let go. Use the same grab to pick up the disc and the stone and carry them through the doorway; hold an object near the altar's socket until the ghost mesh appears, then release. Teleport (the panel lists the key) onto the floor to move quickly.

## VR theory

A **modular kit** is a set of pieces designed to a shared unit — 1 m, 2 m, half-meter steps — with pivots placed so that pieces meet exactly when their positions are multiples of that unit. Kenney's kits, Quaternius's ruins, and most professional environment kits work this way. The payoff is speed and consistency: a level designer places hundreds of pieces by snapping, never by nudging. The discipline it demands is **grid and pivot hygiene**: whole-unit sizes, pivots at a corner or base edge, and Unity's *Grid Snapping* turned on (hold **Ctrl/Cmd** while dragging, or enable the grid-snap toggle in the Scene view toolbar and set *Grid Size* in the Grid and Snap overlay). The starter room uses 1 m and 2 m wall modules on a whole-meter grid; look at any wall's position and you will see integers or exact .25 offsets for thickness. Braun and Rizzo cover environment assembly, prefabs, and interactive props in Chapter 8 of *XR Development with Unity*.

**Interactive elements as environmental puzzles.** In a screen game a door opens because you pressed E. In VR the player has hands, so a lever is a lever: you grab it and pull. That is more satisfying and more legible — the affordance is the shape — but it means the environment must be built from *mechanisms*, small objects with state that react to what the player does. Three patterns cover most of them: a **continuous control** (lever, dial, valve) that produces a value; a **threshold event** (the value crossed a line, fire once); and a **placement check** (the right thing was put in the right spot). Today's scene has one of each.

**Events versus polling.** Polling means asking every frame: "is the lever pulled yet? is the disc on the altar?" It works, but every object needs to know about every other object, and the checks run whether or not anything changed. **Events** invert that: the lever announces "pulled" once, and whoever cares listens. Unity's `UnityEvent` is an event you can wire in the Inspector, which is why the lever does not reference the door at all — a designer can point *On Pulled* at a trapdoor instead. XRI is built the same way: `XRSocketInteractor.selectEntered` fires when something snaps into the socket, and you subscribe in code.

**Physics joints versus animation.** A door can open by animation (an Animator clip, or the scripted `SmoothStep` motion you write today) or by physics (a hinged rigidbody swung by forces). Animation is predictable and cheap and is right for doors, drawbridges, and walls that move on their own. Physics is right for things the player *holds*: the lever is an `XRGrabInteractable` in **velocity tracking** mode — XRI pushes the handle's rigidbody toward your hand and the `HingeJoint` keeps it on its arc within ±60°. You get a convincing lever with no code for the motion at all; your script only *reads* the joint's angle. Mixing the two is the standard recipe: physics where the hand is, animation everywhere else.

**Hysteresis** is the small idea that keeps mechanisms from chattering. If the lever fires *open* at 0.8 and *close* at anything below 0.8, a handle resting near 0.8 flickers between states every frame. Firing *open* at 0.8 and *close* at 0.5 leaves a dead band where nothing changes — a thermostat works the same way.

## Math foundation

**Angle to normalized value.** `Mathf.InverseLerp(a, b, x)` returns $(x - a)/(b - a)$ clamped to $[0, 1]$. With rest at $-60°$ and pulled at $+60°$: an angle of $-60°$ gives $0$, $0°$ gives $0.5$, and $36°$ gives $(36 + 60)/120 = 0.8$ — the threshold. A joint that overshoots to $63°$ still returns $1.0$ because of the clamp.

**Eased motion over a fixed duration.** Progress advances linearly: $p \leftarrow p + \Delta t / T$ with $T$ = 2.5 s, capped at the target by `MoveTowards`. The eased value is $s = 3p^2 - 2p^3$ (`SmoothStep(0,1,p)`), and the position is $\mathbf{x} = \mathbf{x}_{\text{closed}} + s\,\mathbf{o}$ where $\mathbf{o}$ is the open offset. Worked example with $\mathbf{o} = (0, -2.85, 0)$: at $t = 0.5$ s, $p = 0.2$, $s = 0.104$, and the slab has dropped $0.30$ m; at $t = 1.25$ s, $p = 0.5$, $s = 0.5$, $1.43$ m; at $t = 2.0$ s, $p = 0.8$, $s = 0.896$, $2.55$ m. Velocity peaks in the middle — $\dot s = 6p(1-p)$ is $1.5$ at $p = 0.5$ — and is zero at both ends, which is why the slab appears to start and settle gently.

**World-to-grid rounding.** To snap any position to a grid of size $g$: $x' = g \cdot \text{round}(x / g)$, and the same for $z$. With $g = 1$: $x = 2.37 \to 2$, $x = -3.61 \to -4$. For a piece whose pivot is at its corner rather than its center, snap the pivot, and the piece's far edge lands on the grid automatically only if its width is a whole multiple of $g$ — that is the rule the kit must obey.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_3_4.unity` with:

- **Floor** — a 30 m stone plane with a `TeleportationArea`.
- **XR Origin (XR Rig)** at z = −5 facing the room, with the **XR Interaction Simulator**.
- **Ruins Walls (modular, 1 m grid)** — an 8 × 6 m room (x from −4 to 4, z from 0 to 6) and a 8 × 4 m chamber behind it (z from 6.5 to 10), built from 1 m and 2 m wall modules 3 m tall and 0.5 m thick, all static. The front wall has a 2 m doorway; the back wall has a 2 m gap.
- **Stone Door** — a 2 × 2.8 m slab filling the doorway, carrying `SlidingDoor` (*Open Offset* (0, −2.85, 0), *Duration* 2.5).
- **False Wall** — a 2 × 3 m module filling the back-wall gap, carrying `SlidingDoor` (sinks 3.1 m over 3.5 s).
- **Lever Base** (kinematic rigidbody) and **Lever Handle** — a 0.8 m wooden handle with a knob, `Rigidbody` (no gravity), `HingeJoint` (anchor at its base, axis x, limits ±60°), `XRGrabInteractable` (velocity tracking, dynamic attach), and `LeverInteractable` (*Hinge* and *Indicator* assigned; *On Pulled* → Stone Door.Open, *On Released* → Stone Door.Close). A red **Lever Indicator** point light sits on the base.
- **Altar** inside the room with an **Altar Socket** child: a trigger `SphereCollider` and an `XRSocketInteractor` showing hover meshes. An **Altar Message** label floats above it.
- **Hidden Chamber** — carries `HiddenChamberReveal` (*Socket*, *Reveal Light*, *Message* assigned; *Required Piece Id* `sun`; *On Revealed* → False Wall.Open). Inside: a glowing lore scroll on a pedestal and a **Reveal Light** that starts disabled.
- **Offering Table** with the **Sun Disc** (`Rigidbody`, `XRGrabInteractable`, `ArtifactPiece` id `sun`) and the **Loose Stone** (`Rigidbody`, `XRGrabInteractable`, no marker — the decoy).
- Signs, a warm sun, faint fog.

Scripts live in `Assets/Scripts/`: `LeverInteractable.cs`, `SlidingDoor.cs`, `HiddenChamberReveal.cs`, `ArtifactPiece.cs`.

## Your tasks (about 70 min)

**Task 1 (15 min) — Read the lever** · file: `Scripts/LeverInteractable.cs`, TODO 1–2, 4
Read `hinge.angle`, normalize it with `InverseLerp(restAngle, pulledAngle, angle)` into `Value`, and drive the indicator's color from red to green. Turn on the Inspector's *Debug* mode (three dots → Debug) to watch *Value* while you grab the handle in the simulator. If pulling toward you makes the value *fall*, swap *Rest Angle* and *Pulled Angle* in the Inspector.
**Check:** the lamp on the lever base fades from red to green as you pull; *Value* reads 0 at one limit and 1 at the other; the handle never leaves its ±60° arc.

**Task 2 (10 min) — Fire once** · file: `Scripts/LeverInteractable.cs`, TODO 3
Add the edge detection with the two thresholds. `onPulled` is already wired to the door in the Inspector, so nothing else is needed for the door to receive the call — but the door will not move yet.
**Check:** the Console prints `[SlidingDoor] Stone Door opening` exactly once when you pass 80 %, and `closing` once when you drop below 50 %. Wiggling near the top prints nothing more.

**Task 3 (15 min) — The stone door** · file: `Scripts/SlidingDoor.cs`, TODO 1–3
Advance `Progress` with `MoveTowards`, ease it with `SmoothStep`, and set `localPosition` from the closed position and the open offset. Pull the lever halfway back while the slab is still moving to see it reverse smoothly.
**Check:** the slab sinks into the floor over 2.5 s with a gentle start and stop; the doorway is fully clear; releasing the lever raises it again. Walk or teleport into the room.

**Task 4 (15 min) — The altar decides** · file: `Scripts/HiddenChamberReveal.cs`, TODO 1–4; `Scripts/ArtifactPiece.cs`, TODO 1
Subscribe to `socket.selectEntered` in `OnEnable` (and unsubscribe in `OnDisable`), read the placed object's `ArtifactPiece` with `GetComponentInParent`, and either `Reveal` or `Reject`. In `Reveal`, set the message, enable the light, and invoke `onRevealed` — which the scene has wired to the false wall. Make `ArtifactPiece.Matches` case- and space-insensitive.
**Check:** carry the **Loose Stone** to the altar: the label reads "The altar rejects Loose Stone" and nothing moves. Carry the **Sun Disc**: the label changes, the chamber light comes on, the false wall sinks over 3.5 s, and the glowing scroll is visible through the gap. Change *Required Piece Id* to `MOON` and the disc is rejected too.

**Task 5 (15 min) — Build with a real kit** · no new code
Import a modular kit (see *Assets you will need*). In the Scene view enable grid snapping (the magnet/grid toggle in the toolbar; **Edit → Grid and Snap Settings** → *Grid Size* 1 or 0.5, matching the kit's unit) and rebuild one wall of the room from kit pieces: drag a wall piece in, hold **Ctrl/Cmd** while moving it so it lands on the grid, and duplicate with **Ctrl/Cmd+D**. Check the piece's pivot first — if it is centered, note the half-width offset you need. Replace the **Stone Door** mesh with a kit door or gate (keep the `SlidingDoor` component and its offset) and put a kit statue on the **Altar**. Fix scale with the gauge from Activity 3.1 if pieces come in the wrong size.
**Check:** kit pieces meet without gaps or overlaps at the seams; the door still opens and the chamber still reveals. Record your screencast here — lever, door, wrong offering, right offering, reveal.

## Stretch goals

- Add a second lever inside the room wired to the *same* door's `Toggle`, so the player can leave; note how UnityEvents made that a zero-code change.
- Haptics: in `LeverInteractable`, when `onPulled` fires, find the grabbing hand's `HapticImpulsePlayer` (`XRGrabInteractable.selectEntered` gives `args.interactorObject.transform.GetComponentInParent<HapticImpulsePlayer>()`) and send a 0.5 amplitude, 0.1 s impulse — null-check, verify on the headset.
- A `[ContextMenu("Snap To Grid")]` helper on a tiny editor-side script that rounds a selected piece's position to the grid using the formula in the Math foundation.

## Assets you will need

Everything is listed with licenses in `Docs/FreeAssets.md`. For today:

- **Kenney Dungeon Kit** or **Kenney Castle Kit** (kenney.nl, CC0): the FBX/glTF folder — import a wall, a wall with a doorway, a corner, a floor tile, a pillar, a gate or door, and a set of stairs. Kenney kits use a consistent unit (check one wall's bounds with the 3.1 gauge; it is usually 1 unit wide at import, so you may need a Scale Factor of 2–4).
- **Quaternius Modular Ruins** or **Ultimate Fantasy** packs (quaternius.com, CC0): weathered walls, broken columns, a statue for the altar, and rubble props to dress the corners.
- **Sketchfab** (filter *Downloadable*, license CC0 or CC-BY): one hero piece — a carved relief or ancient door — for the false wall or the chamber's back wall; check its triangle count stays under 10 000.

Keep downloads under `Assets/ThirdParty/<source name>/` and record the source and license in your GDD credits.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. On the headset the lever is much more convincing than in the simulator: grab it with the grip button and pull. If the handle jitters in your hand, raise *Rigidbody → Solver Iterations* on the handle to 12 or set **Edit → Project Settings → Physics → Default Solver Iterations** to 12; if it lags behind the hand, that is velocity tracking's smoothing — acceptable for a heavy lever. The socket's hover mesh helps players find the exact drop spot; keep it on. The stretch-goal haptics only work on the headset.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: the lever lamp going red to green, the door sliding open and closing, the Loose Stone rejected and the Sun Disc accepted, the false wall sinking to reveal the scroll, and your kit-built wall with clean seams.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus one sentence on which you found easier to reason about — the physics lever or the scripted door — and why.
- (optional) A screenshot of the *On Pulled* and *On Revealed* event lists in the Inspector.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Handle flies off or spins when grabbed | `XRGrabInteractable → Movement Type` must be *Velocity Tracking* and *Throw On Detach* off; check the `HingeJoint` still has *Connected Body* = Lever Base |
| Handle will not move at all | Lever Base's Rigidbody is not kinematic (it fell) or *Use Limits* min/max are both 0; also make sure the handle's Rigidbody has *Use Gravity* off |
| *Value* stays 0 while pulling | TODO 1 still assigns `angle = 0f`; or the hinge angle is negative in the pull direction — swap Rest and Pulled angles |
| Door opens every frame / Console floods with "opening" | Edge detection is missing (TODO 3) — the `IsPulled` flag must gate the invoke, and `SlidingDoor.Open` returns early if already opening |
| Door moves instantly or not at all | `Progress` is not being advanced (TODO 1 in `SlidingDoor`), or *Duration* is 0 — the script clamps it to 0.01 so check the Inspector |
| Disc will not snap into the altar | The socket's `SphereCollider` must be a trigger, and the disc needs both a `Rigidbody` and an `XRGrabInteractable`; hold it inside the sphere until the hover mesh appears, then release |
| Altar accepts the stone too | `Matches` still compares raw strings and `requiredPieceId` is empty, or `GetComponentInParent<ArtifactPiece>` was called on the wrong transform — use `args.interactableObject.transform` |
| Kit walls overlap or leave slits | Grid Size does not match the kit's unit, or the pivot is centered — snap with the half-width offset, or wrap each piece in an empty parent whose pivot sits at the piece's corner |

Created by Isac Artzi

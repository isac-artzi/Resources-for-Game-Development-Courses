# Activity 2.1 — Grab the Shard

Topic 2: VR Game Design Principles on a Variety of Platforms · Week 3, Tuesday · Stepping stone to Milestone 2 — Platform Compatibility Design

## Where this fits

By the end of class you will have a working pick-up-and-place loop, the most basic unit of VR gameplay: three artifact shards you can grab with either controller, an altar with three sockets that accept a shard and refuse anything else, hover highlights that tell you what is grabbable before you commit, and a floating counter that reads "2/3 placed" and wakes the Guide at "3/3". Milestone 2 asks for a prototype on the headset with movement, interaction, and UI. This is the *interaction* third, built entirely with the XR Interaction Toolkit components you will keep reusing for inventory (Topic 4), the runestone puzzle (4.3), and the lever in the ruins (3.4).

*Elaria hook:* at the edge of the Enchanted Forest stands the Altar of Binding. Three shards of the broken artifact lie on mossy pedestals around it — and so does a river stone that looks almost right. The altar knows the difference. Bind the three true shards and the Mysterious Guide will finally speak.

## Learning goals

- You can explain XRI's interactor/interactable model and name the three interaction states (hover, select, activate) and where each fires in your scene.
- You can make a physics object grabbable with `Rigidbody` + `XRGrabInteractable` and describe what *Movement Type* and *Dynamic Attach* change.
- You can build a socket with `XRSocketInteractor` and a trigger collider, and filter what it accepts by tag inside a `selectEntered` listener.
- You can subscribe C# methods to XRI UnityEvents in code, read the event arguments, and use them to drive visual feedback (material swaps).
- You can define affordance and signifier, and point to the signifier you built.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-2.1-grab-the-shard`). Open it with Unity **6000.5.x** and wait for packages.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** tab → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. Menu bar → **Activity → Check Setup**. Both lines should say `OK`.
6. Menu bar → **Activity → Build Starter Scene**. The Console also reports that it added a tag named `Shard` to the project — the socket filter depends on it.
7. Press **Play**. You stand between the pedestals, facing the altar.

Simulator controls that matter today: **Tab** cycles which device you drive (head → left controller → right controller). With a controller selected, **W A S D** move it and the **mouse** aims it. Look at the simulator's on-screen panel for the button keys — by default **Grip** is on **G** and **Trigger** on **T**, and the mouse buttons mirror them. Grabbing works two ways with the Starter Assets *Near-Far Interactor*: put the controller right on the shard, or aim the ray at it from a distance; in both cases hold **Grip** to take it and release to drop. To seat a shard, carry it into a socket until the translucent "ghost" appears, then let go.

## VR theory

**Interactor and interactable.** XRI splits every interaction into two roles. An **interactor** is something that can act: a hand's ray, a hand's grab sphere, a gaze cursor, or — as you will see today — a socket. An **interactable** is something that can be acted on: a grabbable shard, a teleport anchor, a UI button. A single **XR Interaction Manager** in the scene matches them up every frame and decides who may hover or select what. You never call "grab" yourself; you configure the parts and respond to the events the manager fires. Braun and Rizzo introduce this architecture in Chapter 5 of *XR Development with Unity*, together with the Starter Assets rig you are using.

**The three states.** **Hover** means an interactor is close enough or pointing well enough that a select *could* happen — nothing has been committed yet. **Select** means the interactor has taken the interactable: the shard follows the hand, the socket holds its content. **Activate** is an extra input while selected (the trigger on a held object) and is how you would "use" a torch or "cast" from a wand. Each state has an *entered* and an *exited* UnityEvent, and each event delivers an *args* object naming both parties — `args.interactorObject` and `args.interactableObject`. Reading those arguments is how one script can behave differently when a hand grabs the shard versus when the altar takes it.

**Affordances and signifiers.** Don Norman's terms are the design vocabulary for this whole topic. An **affordance** is a relationship — a shard *affords* grasping because it is small, light, and within reach. A **signifier** is the *cue* that communicates the affordance: the glow, the hover highlight, the ghost that appears when a shard nears its socket. Players cannot feel weight or texture in VR, so signifiers carry more of the load than in the physical world. Rule of thumb: every grabbable object needs a resting signifier (it looks different from scenery) and a hover signifier (it reacts before you commit).

**Near versus far interaction.** Grabbing an object your hand is touching is *near* interaction: intuitive, precise, tiring when everything is far away. Pointing a ray at a distant object and pulling it to you is *far* interaction: fast and comfortable when seated, but it hides physical scale and can feel like a mouse. The Starter Assets *Near-Far Interactor* does both: a small sphere around the controller for near grabs and a ray for far grabs that pulls the object into the hand. Your design decision for the semester project is *which* objects allow far interaction — the puzzle in the ruins probably should not.

**Sockets as design tools.** An `XRSocketInteractor` is an interactor that never moves: it selects any interactable that enters its trigger and snaps it to an attach point. Sockets are the quiet workhorse of VR design — keyholes, weapon holsters, inventory slots, the runestone puzzle in 4.3. Out of the box a socket accepts anything. Today you teach it to refuse the river stone, and you will reuse that pattern whenever "only the right thing fits".

**Platform note.** Nothing you wire today is Quest-specific. The same XRI components read input from the simulator on your laptop, from Touch controllers on Quest 3, and from tracked hands if you enable hand tracking later. That portability is the reason this course builds on XRI plus OpenXR rather than a vendor SDK, and it is the argument you will make in the Milestone 2 compatibility document.

## Math foundation

Two small pieces of geometry decide when things "count as close".

**A ray** is a start point $o$ and a unit direction $d$; every point on it is $p(t) = o + t\,d$ for $t \ge 0$, where $t$ is the distance travelled along the ray in meters. The far interactor casts such a ray from the controller and asks the physics engine for the first collider it crosses. Worked example: the controller sits at $o = (0,\ 1.2,\ 0)$ pointing slightly downward, $d = (0,\ -0.20,\ 0.98)$ (already close to unit length). Where does it cross the altar top plane at $y = 1.0$? Solve $1.2 - 0.20\,t = 1.0 \Rightarrow t = 1.0$ m, so the hit is at $p(1) = (0,\ 1.0,\ 0.98)$. The altar drum is centered at $z = 1.4$ with radius 0.6 m, so its top spans $z \in [0.8,\ 2.0]$ and this hit lands on the altar, 18 cm past its front edge. Raise the aim a little and $t$ grows quickly: with $d_y = -0.10$ the ray reaches $y = 1.0$ only at $t = 2.0$ m, beyond the altar's back edge — which is why a ray pointer feels twitchy on far, flat targets.

**A sphere test** is how the socket decides that a shard is inside it. The socket's trigger is a sphere of radius $r = 0.12$ m; a shard whose collider overlaps that sphere is "in". For the shard's center at offset $(0.05,\ 0.03,\ 0.02)$ from the socket center, the distance is

$$\sqrt{0.05^2 + 0.03^2 + 0.02^2} = \sqrt{0.0025 + 0.0009 + 0.0004} = \sqrt{0.0038} \approx 0.062 \text{ m},$$

well inside 0.12 m. The `clearDistance` in `ShardSocketCheck` (0.30 m) uses the same test in reverse: the socket wakes up again only when `Vector3.Distance` between the rejected stone and the socket exceeds 0.30 m. Sizing rule: a socket trigger should be a little larger than the object it holds (here the shard is 0.30 m tall, the sphere 0.24 m across) so that a slightly sloppy release still snaps, but not so large that two sockets 0.32 m apart fight over one shard.

**Counting** is the only other arithmetic: the counter shows $\text{placed}/\text{total}$ and completion is $\text{placed} = \text{total}$.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_2_1.unity` with:

- **Floor** — a 30 m grass plane, static, plus a loose ring of ten primitive trees for enclosure.
- **XR Origin (XR Rig)** at the origin with the **XR Interaction Simulator**, and an **XR Interaction Manager**.
- **Altar** — a 1 m tall stone drum 1.4 m ahead. On its top sit **Shard Socket 1–3**: each an empty GameObject with a trigger `SphereCollider` (r = 0.12 m), an `XRSocketInteractor` (hover meshes on, 1 s recycle delay), and `ShardSocketCheck`. A flat **Socket Ring** under each shows idle/accepted/rejected colors; its collider is removed so rays pass through it.
- **Shard 1–3** — emissive 0.12 × 0.30 m cubes on 0.8 m pedestals to your left, right, and *behind* you. Each is tagged `Shard` and carries `Rigidbody`, `XRGrabInteractable` (Kinematic movement, dynamic attach) and `GrabHighlighter` with idle/hover/held materials assigned.
- **River Stone** — a grabbable 16 cm sphere on a fourth pedestal to your right-rear, with `GrabHighlighter` but **no** `Shard` tag. It is the decoy.
- **Shard Counter** — a TextMesh above the altar carrying `GrabCounter`; its *Sockets* array holds the three socket scripts and *Reveal On Complete* points at the Guide's glow light, which starts disabled.
- **Mysterious Guide** — a dark 2 m capsule 5.5 m ahead with a label.
- **Welcome Sign** with the two controls you need.

Scripts live in `Assets/Scripts/`. Nothing in the scene depends on the Inspector wiring being redone — every reference is set by the builder.

## Your tasks (about 70 min)

**Task 1 (10 min) — Feel the loop before coding** · no code
Press Play. Tab to the right controller, aim at a shard, hold Grip: the shard flies into your hand (far grab). Move the hand into a socket and watch the ghost mesh appear; release. Try the River Stone: with no code written yet the altar accepts it — that is the bug you will fix. Open the Console and note which XRI warnings, if any, appear.
**Check:** you can grab, carry, drop, and seat a shard using only the simulator. If nothing grabs, revisit *Before you start* step 3.

**Task 2 (15 min) — Hover and hold signifiers** · file: `Scripts/GrabHighlighter.cs`, TODO 1–3
Subscribe the four handler methods to the grab interactable's `hoverEntered`, `hoverExited`, `selectEntered`, `selectExited` events in `OnEnable`. In the hover handlers swap to `hoverMaterial` / back to `idleMaterial`, but only when `IsHeldByHand` is false. In the select handlers test `args.interactorObject is XRSocketInteractor`: a socket seats the shard (idle look), a hand holds it (held look).
**Check:** blue at rest, yellow under the ray, white in the hand, blue again when seated in a socket. Carrying a shard over the altar must not flicker it yellow.

**Task 3 (20 min) — The altar refuses the stone** · file: `Scripts/ShardSocketCheck.cs`, TODO 1–3
Subscribe to the socket's `selectEntered` and `selectExited`. In `OnSelectEntered`, `CompareTag(requiredTag)` on the arriving object: shards set `HasShard = true` and turn the ring green; anything else increments `RejectCount` and starts the `Reject` coroutine. In `Reject`, set `socket.socketActive = false` (XRI releases the object on its next update), color the ring red, wait with `yield return null` until the offender is farther than `clearDistance`, then re-enable the socket and reset the ring.
**Check:** a shard turns its ring green and stays. The River Stone turns the ring red and drops onto the altar; the socket stays asleep until you carry the stone 30 cm away, then a shard is accepted again.

**Task 4 (15 min) — Count and reveal** · file: `Scripts/GrabCounter.cs`, TODO 1–3
Each frame count sockets whose `HasShard` is true, format the text with your `Format` helper, and write it to the label. When the count equals the total for the first time, set `IsComplete`, enable `revealOnComplete`, and log a line.
**Check:** the label goes 0/3 → 1/3 → 2/3 → 3/3 placed; on 3/3 the Guide lights up purple. Pull a shard out: the label drops to 2/3 but the glow stays (completion is a one-way flag — argue in your bullets whether that is the right design).

**Task 5 (10 min) — Tune and record** · no new code
Select a shard and change *Movement Type* on `XRGrabInteractable` to *Velocity Tracking*, then *Instantaneous*; grab and wave it around each time and note what changed in your Padlet bullets. Set one socket's `SphereCollider` radius to 0.05 and try to seat a shard. Set it back to 0.12. Record your screencast during this task: show a hover highlight, a socket accept, the stone being rejected, and the counter reaching 3/3.

## Stretch goals

- Add a proximity signifier that does not need XRI: in `GrabHighlighter`, pulse the emission color when the main camera is within 1.2 m (`Vector3.Distance`, `Mathf.Sin(Time.time * 4f)`, `Material.SetColor("_EmissionColor", …)` on a per-object material instance).
- Give each socket a *specific* shard: add a `public string requiredName` and compare `arrived.name` too, then color-code shards and rings so the player can see which goes where.
- Read about *Interaction Layer Mask* (Unity Manual → XR Interaction Toolkit → Interaction Layers) and move the filter from a tag check to layers, so the socket never even hovers the stone. Which approach gives the player better feedback?

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. Nothing else changes: the Near-Far Interactor reads the Touch controllers' grip through OpenXR exactly as it read the simulator. On the headset, try the near grab (touch the shard, squeeze grip) — it is far more satisfying than the far grab you used on the desktop — and note whether the 0.12 m socket radius still feels forgiving when your real hand is doing the placing. Haptics on grab are Topic 6; leave them for now.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: a hover highlight, grabbing and seating a shard, the River Stone being refused (red ring, stone drops), and the counter reaching 3/3 with the Guide lighting up.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus one sentence on how the three Movement Types felt and one on whether completion should be reversible.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Nothing highlights and the Console shows no hover lines | TODO 1 in `GrabHighlighter` is still empty — the listeners are never added |
| The shard flies into the hand but the socket never shows a ghost | The socket's `SphereCollider` lost *Is Trigger*, or the shard has no `Rigidbody` (trigger events need one moving Rigidbody) |
| Socket accepts the River Stone | TODO 2/3 in `ShardSocketCheck` not done, or the stone was accidentally tagged `Shard`; check its Tag dropdown |
| Console: `Tag: Shard is not defined` | Run **Activity → Build Starter Scene** again (it registers the tag), or add it under **Edit → Project Settings → Tags and Layers** |
| Rejected stone is grabbed again every second | The stone is resting inside the trigger; make sure TODO 3 waits for `clearDistance` before setting `socketActive = true` |
| Shard passes through the altar or jitters | Movement Type is *Instantaneous*; switch back to *Kinematic* and keep *Collision Detection* on Continuous |
| Label never changes from "0/3 placed" | `Format` still returns the placeholder, or `GrabCounter.sockets` is empty — rebuild the scene |

Created by Isac Artzi

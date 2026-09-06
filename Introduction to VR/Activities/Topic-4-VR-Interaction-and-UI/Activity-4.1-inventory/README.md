# Activity 4.1 — The Hero's Satchel: Inventory

Topic 4: VR Interaction and User Interfaces · Week 7, Tuesday · Stepping stone to Milestone 4 — UI and Interaction Design

## Where this fits

By the end of class you will have a working VR inventory: you grab a collectible from a stone table, bring it to your right hip, and it vanishes into a satchel. A small panel floating just above the satchel shows what you carry — six slots with the item's name and color — and when you point the controller ray at a filled slot and click, the item pops back out in front of you, ready to be grabbed again. Milestone 4 asks for menus, an inventory, dialogue boxes, and pick-up interactions in your Realm of Legends prototype. Today is the inventory. The satchel you build here will hold the artifact pieces, lore scrolls, and magical items your quest hands out, and the body-anchoring math you write is the same math you will reuse for any wrist menu or belt tool.

*Elaria hook:* the Mysterious Guide hands you a worn leather satchel. "Everything you gather in Elaria goes in here. It weighs nothing, and it never fills — well, not until you have six things in it." Four objects wait on the stone table at the forest's edge: a blue shard, a lore scroll, a healing potion, and an amber rune. Learn to carry them before the road gets long.

## Learning goals

- You can explain the difference between diegetic, spatial, and meta UI in VR and say which kind a hip satchel is.
- You can anchor an object to the player's *body* (flattened head yaw + fixed offsets) rather than to the head, and explain why head-locked UI is uncomfortable.
- You can subscribe to XR Interaction Toolkit grab events (`selectEntered`, `selectExited`) from a script and use them to track state.
- You can detect a "store" gesture with a trigger volume and `OnTriggerEnter`, and hide/show interactable objects safely.
- You can lay out a grid of UI slots from an index (row = i / cols, col = i % cols) and repaint it from a data list.

## Before you start (10 min)

1. Open **Unity Hub → Add → Add project from disk** and pick this folder (`Activity-4.1-inventory`). Open it with Unity **6000.5.x** and wait for the packages to resolve.
2. If Unity asks to enable the new Input System backends and restart, click **Yes**.
3. **Window → Package Manager** → *In Project* → **XR Interaction Toolkit** → **Samples** tab → *Import* **Starter Assets** and **XR Interaction Simulator**.
4. **Edit → Project Settings → XR Plug-in Management** → desktop tab → check **OpenXR**.
5. **Activity → Check Setup**. Both lines should read `OK`.
6. **Activity → Build Starter Scene**. The scene `Activity_4_1` is created and opened.
7. Press **Play**. You stand in front of a stone table with four glowing objects; a small leather bag with a dark panel floats at your right hip.

Simulator controls that matter today: **Tab** cycles the device you drive (head → left controller → right controller). Drive a controller with **W A S D** (and the vertical keys the panel lists) and aim it with the **mouse**; the simulator's on-screen panel shows which key is **Grip** (grab an object) and which is **Trigger** (click a UI button with the ray). You will use Grip to pick up items and Trigger to click satchel slots. When you move the *head* with WASD the satchel must follow — that is one of the things you are implementing.

## VR theory

**Diegetic, spatial, and meta UI.** Interface elements in a VR game live in one of three places. **Diegetic UI** exists inside the fiction — a satchel on your hip, a lantern that shows your health, a scroll you unroll to read the map. **Spatial UI** is placed in the 3D world but does not pretend to be an object: a floating panel above an NPC, a glowing outline on a grabbable shard. **Meta UI** is drawn on the screen itself — the health bar in the corner of a flat game. Meta UI barely works in VR: there is no "corner of the screen", and anything glued to the camera sits at a fixed focal distance in front of your eyes, blocks the world, and makes many people queasy within a minute. The satchel you build is diegetic (a bag with slots) and the panel above it is spatial UI anchored to that bag. Braun and Rizzo cover VR UI placement in Chapter 8 of *XR Development with Unity*; the short version is that in VR, UI is a *thing in the world*, and you decide where the thing lives.

**Body-anchored UI.** Where should a thing you want to reach at any time live? Not on the head (see above) and not fixed in the world (you walk away from it). The comfortable answer is the *body*: hips, wrists, chest. The catch is that no consumer headset tracks your torso. We fake it from the head: take the head's position, keep only its yaw (the rotation around the vertical axis), and hang the UI off that at a fixed drop and side offset. The result turns when you turn your shoulders, comes with you when you walk, but does **not** bob when you nod or tilt when you look down — which is exactly how a real belt behaves. This is the "flattened forward" math below.

**Inventory paradigms in VR.** Flat games have a grid you open with a key. VR games have tried several replacements: a wrist watch (Half-Life: Alyx), a backpack you reach over your shoulder for (The Walking Dead: Saints & Sinners), a belt with physical slots (many shooters), a radial menu on the thumbstick. All of them share two design rules. First, *storing* must be a bodily gesture — bring the object to the container — because that is what your hands already know how to do. Second, *retrieving* must be fast and visible: you need to see what you have and get it into your hand in one motion. Our satchel stores by proximity (object held + inside the hip volume) and retrieves by a UI click that spawns the object in front of you. It is simple, and it works with the simulator; on the headset you may later replace the click with "reach into the bag".

**Interactor / interactable events.** In the XR Interaction Toolkit, hands are *interactors* and objects are *interactables*. When an interactor selects an interactable, the interactable raises `selectEntered`; when it lets go, `selectExited`. These are `UnityEvent`s, so any script can `AddListener` to them at runtime — no inheritance needed. You did this from the Inspector in Topic 2; today you do it in code so the listener follows the object when it is hidden and shown again.

**Hide, do not destroy.** Storing an object means taking it out of the world for a while. The tempting implementation — `Destroy` it and later `Instantiate` a prefab — needs a prefab per item type and loses any state the object had (a half-read scroll, a dented potion). `SetActive(false)` keeps the whole GameObject, its components, and its data in memory, costs nothing to render, and `SetActive(true)` brings it back exactly as it was. The one thing to know is that the XR Interaction Toolkit *unregisters* an interactable when it is disabled and cancels any grab in progress, and re-registers it when it is enabled. That is why `StorableItem` subscribes to events in `OnEnable` and unsubscribes in `OnDisable` — the object's life has gaps in it.

**Capacity as a design signal.** Six slots is small on purpose. A visible, finite capacity makes players decide what to carry, and a full satchel ("nothing happens when I bring the fifth thing") is a moment that needs feedback — a sound, a shake, a line from the Guide. Notice how the panel already communicates capacity before you hit it: the dim empty slots *are* the message.

**Feedback.** A store gesture with no response feels like a bug. Three cheap confirmations: the object disappears (visual), a short sound (audio, an `AudioSource` slot is ready on the satchel — drop any click into it), and the panel updating. On the headset you would add a haptic pulse on the controller that held the item; the README for Activity 6.3 adds that properly.

## Math foundation

**Flattened forward and the body frame.** The head's forward vector $\vec f_{head}$ has three components. Zero the vertical one and normalize to get the *yaw-only* forward:

$$\vec f = \frac{(f_x,\, 0,\, f_z)}{\lVert (f_x, 0, f_z) \rVert}$$

The body's right vector is perpendicular to both $\vec f$ and the world up:

$$\vec r = \hat{y} \times \vec f$$

(`Vector3.Cross(Vector3.up, forward)` — in Unity's left-handed system this gives +x when the forward is +z, which is "right"). The satchel position is then

$$\vec p_{satchel} = \vec p_{head} + a\,\vec f + s\,\vec r - d\,\hat{y}$$

with $a$ = forward offset, $s$ = side offset, $d$ = hip drop.

Worked example: head at $(0,\ 1.6,\ 0)$, looking 30° down and 45° to the left, so $\vec f_{head} = (-0.61,\ -0.50,\ 0.61)$. Flatten: $(-0.61,\ 0,\ 0.61)$, length 0.866, normalized $\vec f = (-0.707,\ 0,\ 0.707)$. Right: $\hat y \times \vec f = (0.707,\ 0,\ 0.707)$. With $a = 0.15$, $s = 0.25$, $d = 0.7$:

$$\vec p = (0,1.6,0) + 0.15(-0.707,0,0.707) + 0.25(0.707,0,0.707) - (0,0.7,0) = (0.071,\ 0.90,\ 0.283)$$

The satchel sits at 0.90 m — hip height — 0.28 m ahead and slightly to the right, regardless of the 30° downward look. If you had used the raw head forward, the −0.50 vertical component would have pulled the bag 7 cm toward your face and it would jump every time you glanced at it.

**Slot grid from an index.** With $c$ columns, slot $i$ (0-based) lives at

$$\text{row} = \lfloor i / c \rfloor, \qquad \text{col} = i \bmod c$$

and its top-left anchored position, for slot size $w$ and gap $g$, is $x = \text{col}\,(w+g)$, $y = -\text{row}\,(w+g)$ (negative because UI y points up and rows go down). For $c=3$, $w=110$, $g=12$: slot 4 → row 1, col 1 → $(122, -122)$; slot 5 → row 1, col 2 → $(244, -122)$. In C#, `i / cols` on two `int`s is already the floor, and `i % cols` is the remainder.

**Canvas size in meters.** The panel is 400 × 330 canvas pixels at a scale of 0.001, so it is 0.40 m × 0.33 m — about the size of a hardback book, at 0.7 m from your eyes. Text at 18 px is 18 mm tall; at 0.7 m that is $2\arctan(0.009/0.7) \approx 1.5°$, comfortably above the 1° legibility floor from Activity 1.2.

## The starter scene

**Activity → Build Starter Scene** creates and saves `Assets/Scenes/Activity_4_1.unity` with:

- **Floor** — a 24 m grass plane; **Trees** — a half-ring of trunk-and-crown primitives 8 m behind the table.
- **XR Origin (XR Rig)** at the origin with the **XR Interaction Simulator** beside it, and an **EventSystem** carrying `XRUIInputModule` so the controller rays can click UI.
- **Stone Table** — a 1.8 × 0.8 × 0.7 m block 1.2 m ahead.
- **Collectibles** — *Blue Shard*, *Lore Scroll*, *Healing Potion*, *Amber Rune*: emissive primitives, each with a `Rigidbody`, an `XRGrabInteractable` (kinematic movement, dynamic attach), and a **`StorableItem`** whose *Item* foldout already holds the id, display name and color.
- **Satchel** — a child of the rig's *Camera Offset* at local (0.25, 0.9, 0.15): a kinematic `Rigidbody`, a 0.30 × 0.25 × 0.22 m `BoxCollider` set to *Is Trigger*, an `AudioSource` (no clip yet), a small leather **Satchel Marker** cube so you can see it, and the **`Inventory`** script with *Head* pointed at the rig's camera and *Capacity* 6.
- **Satchel Panel** — a world-space `Canvas` (400 × 330 px, scale 0.001, tilted 55° toward your eyes) parented to the Satchel, with a `TrackedDeviceGraphicRaycaster`, a title, and six *Slot* buttons under a *Slots* container. It carries **`InventoryPanel`** with *Inventory* and all six *Slot Buttons* assigned. All six buttons start stacked at the same position — your layout code spreads them out.

Scripts live in `Assets/Scripts/`: `InventoryItem.cs`, `StorableItem.cs`, `Inventory.cs`, `InventoryPanel.cs`.

How the four scripts talk to each other, in the order things happen when you play:

1. A hand grabs the Blue Shard. XRI raises `selectEntered` on its `XRGrabInteractable`; **`StorableItem.OnGrabbed`** hears it and sets `isHeld = true`.
2. You move the hand to your hip. The shard's collider enters the Satchel's trigger; Unity calls **`Inventory.OnTriggerEnter`**, which finds the `StorableItem`, sees `isHeld`, and calls `Store`.
3. `Store` copies the **`InventoryItem`** data into the list, remembers the GameObject, hides it, and calls **`InventoryPanel.Refresh`**, which repaints the slots.
4. You click slot 1. The button's `onClick` calls `InventoryPanel.OnSlotClicked(0)` → `Inventory.Retrieve(0)` → `StorableItem.Reappear(...)`, and the panel refreshes again.

Every arrow in that chain is one TODO. If something does not work, find the first step in the chain that fails and look there.

## Your tasks (about 70 min)

**Task 1 (10 min) — Know when you are held** · file: `Scripts/StorableItem.cs`, TODO 1–2
In `OnEnable`, add `OnGrabbed` and `OnReleased` as listeners on the `XRGrabInteractable`'s `selectEntered` and `selectExited`; remove them in `OnDisable`. In the handlers set `isHeld` and log the item's display name. Subscribing in `OnEnable`/`OnDisable` matters here because storing hides the object and retrieving shows it again — a listener added in `Start` would be added once, a listener added in `OnEnable` without removal would be added twice.
**Check:** grab the Blue Shard (Tab to a controller, aim, Grip). The Console prints `Grabbed Blue Shard` and the *Is Held* box on its `StorableItem` ticks; release and it clears.

**Task 2 (15 min) — Anchor the satchel to your body** · file: `Scripts/Inventory.cs`, TODO 1
In `LateUpdate`, flatten the head's forward (zero y, guard against a zero-length vector, normalize), build the right vector with `Vector3.Cross(Vector3.up, forward)`, and set the satchel's position to `head.position + forward * forwardOffset + right * sideOffset + Vector3.down * hipDrop` with rotation `Quaternion.LookRotation(forward)`. Use `LateUpdate` so the head has already moved this frame.
**Check:** select the head (Tab) and look down — the bag stays at hip height and the panel tilts into view. Turn with the mouse — the bag swings around your right side. Walk with WASD — it comes along. Change *Side Offset* to −0.25 in the Inspector while playing and it jumps to your left hip.

**Task 3 (15 min) — Store on contact** · file: `Scripts/Inventory.cs`, TODO 2–3, and `Scripts/InventoryItem.cs`, TODO 1–2
In `OnTriggerEnter`, find the `StorableItem` on the object (`GetComponentInParent`), ignore it unless `isHeld`, and call `Store`. In `Store`, clone the item data (implement `InventoryItem.Clone` and `ToString`), remember the scene object in `storedObject`, add the entry to `items`, hide the object with `SetActive(false)`, log it, play the feedback sound, and refresh the panel. XRI notices the selected object was disabled and cancels the grab for you.
**Check:** hold a shard, drive that controller down and to your right until it touches the bag — the shard vanishes, the Console prints `Stored: Blue Shard (shard_blue)`, and *Items* in the Inventory's Inspector shows one entry. Throw a shard through the bag without holding it: nothing happens.

**Task 4 (15 min) — Lay out and paint the slots** · file: `Scripts/InventoryPanel.cs`, TODO 1, 3, 4
In `LayoutSlots`, compute `row = i / cols`, `col = i % cols` and set each button's `anchoredPosition` to `(col * (slotSize + gap), -row * (slotSize + gap))`. In `Refresh`, for each slot index below the item count, set the button's `Image.color` to the item color, its child `Text` to the display name, and `interactable = true`; otherwise paint it `emptyColor`, write `-`, and disable it. Update the title to `Satchel n/6`.
**Check:** on Play the six squares form two rows of three. Store the Blue Shard and the Healing Potion: the first slot turns blue and reads *Blue Shard*, the second turns pink and reads *Healing Potion*, the title reads *Satchel 2/6*.

**Task 5 (15 min) — Take it back out** · file: `Scripts/InventoryPanel.cs`, TODO 2, `Scripts/Inventory.cs`, TODO 4, `Scripts/StorableItem.cs`, TODO 3
In `InventoryPanel.Start`, loop over the buttons and add an `onClick` listener that calls `OnSlotClicked(index)` — copy the loop variable into a local first. In `Inventory.Retrieve`, compute a spawn point half a meter ahead of the flattened head forward and a little below eye level, call `Reappear` on the stored object's `StorableItem`, remove the list entry, and refresh. In `Reappear`, zero `linearVelocity` and `angularVelocity` so the item does not inherit the throw it arrived with.
**Check:** point the controller ray at a filled slot and press Trigger — the item appears in front of you and falls to the table or floor; the slot goes dim and the title counts down. Store all four, then retrieve the *first* one: the correct item comes out and the remaining three shift up a slot. Record your screencast now: grab, store two items, show the panel, retrieve one, grab it again.

## Stretch goals

- Play a different sound for store and retrieve (two `AudioClip` fields), and pitch-shift it slightly by the slot index so a full satchel sounds "heavier".
- Add a *left* satchel for consumables only: a second `Inventory` with `sideOffset = -0.25` and a `string acceptsIdPrefix = "potion"` check in `OnTriggerEnter`.
- Retrieve into the *hand* instead of the air: pass the interactor's transform from the clicking controller (find it with `FindFirstObjectByType<XRRayInteractor>()` or `NearFarInteractor`) and spawn 0.1 m in front of it.

## Port to Quest 3

Disable the **XR Interaction Simulator** GameObject and follow `Docs/PortingToQuest3.md`. On the headset the satchel sits at your real hip because the rig reads your real eye height; if it feels high or low, tune *Hip Drop* (a taller player may want 0.75). The panel is readable when you glance down; if it is too dim in a bright scene, raise the *Background* alpha. Storing by touching your hip with a held object feels natural in VR — test it with a classmate and note whether they find the bag without being told.

## Deliverables

- Screencast (2–4 min) posted to this week's Padlet showing: grabbing a collectible, storing two of them at the hip, the panel updating (slots and title), retrieving one, and grabbing it again.
- 3–5 bullets in the Padlet post: what you did, what works, what does not (yet), plus one sentence on where you would put the satchel in your own Realm of Legends prototype and why (hip, wrist, chest?).

## Troubleshooting

| Symptom | Fix |
|---|---|
| **Activity** menu is missing | A compile error; open the Console, fix the red line, wait for the spinner to finish |
| Nothing prints when I grab | Task 1 listeners not added, or you are pressing Trigger instead of Grip — check the simulator's panel for the Grip key |
| The bag is stuck at the origin or floats at eye level | TODO 1 in `Inventory.cs` is not done (the placeholder position under Camera Offset is only a starting guess), or *Head* is empty — it falls back to `Camera.main`, so check the rig's camera is tagged *MainCamera* |
| I touch the bag but nothing is stored | `isHeld` is still false (Task 1), or the collider you touched is on a child and you used `GetComponent` instead of `GetComponentInParent` |
| All six slots sit on top of each other | `LayoutSlots` TODO 1 is still the placeholder; the builder stacks them on purpose |
| Clicking any slot always retrieves the last item | Classic closure bug: copy `i` into a local `int index = i;` before the lambda |
| The ray highlights slots but clicks do nothing | The `EventSystem` needs `XRUIInputModule` (the builder adds it) and the canvas needs `TrackedDeviceGraphicRaycaster`, not `GraphicRaycaster`; also check `interactable` is true on filled slots |
| Retrieved item shoots away | `Reappear` TODO 3 not done — zero `linearVelocity` and `angularVelocity` |

Created by Isac Artzi

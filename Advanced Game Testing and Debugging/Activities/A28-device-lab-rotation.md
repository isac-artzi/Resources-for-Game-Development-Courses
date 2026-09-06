# Activity 28 — Device Lab Rotation

> **Topic 8 · Session 28 · Week 14 (Thu Dec 10)** · In class, ~60 min · Rotating stations in groups, fixed six-minute slots
> **Feeds:** Mobile Game Optimization (due Dec 20)

## The setup
Every phone in this room is a test device, and today the room becomes a device lab. Stations are set
up around the walls; you rotate through four of them on a six-minute clock, running the same route
and capturing the same numbers on each. Nobody optimizes anything today. Today's product is a filled
device matrix and one divergence — a behavior that appears on one tier and not another — because a
divergence is the thing you cannot find by testing harder on the machine you own.

## Why this matters
The device spread on mobile is wider than any console generation gap you have ever worked across. A
four-year-old budget phone and a current flagship differ by roughly an order of magnitude in GPU
throughput, and they differ in kind, not just in degree: different memory ceilings, different texture
compression support, different driver bugs, different screen aspect ratios, different refresh rates,
different OS versions with different lifecycle behavior. The failures that matter are almost never
"it is slower on the cheap phone" — you knew that. They are "the shader compiles to something wrong
on this driver", "the game is killed by the OS on any device with 3 GB", "the layout collides with
the punch-hole camera on this aspect ratio", "the 120 Hz display makes the physics run at half speed
because someone tied a timestep to the frame". Each of those reproduces on exactly one tier and is
invisible everywhere else. And the matrix is what makes this affordable: you cannot test every device
against every quality preset against every OS version, so you have to pick a set, justify it, and
state what risk the set leaves on the table. That is a professional judgment and it belongs in your
test plan, which you wrote on Tuesday and will now revise with evidence.

## What to do
1. **Build the stations (8 min, whole room).** A station is a device plus a card. Set up at least
   four, spread as far apart in capability as the room allows. If the room is short on hardware,
   stations can be **tiers by proxy** and are still worth running — the same device at three quality
   presets, an emulator or simulator profile, and the laptop throttle proxy from Activity 27 with
   different caps. Every station card states: device name, chipset or GPU class, RAM, OS version,
   screen resolution, refresh rate, and the assigned tier (floor, mid, or high). Half of you do not
   know your own phone's chipset; find it now, because "my phone" is not a device matrix row.
2. **Fix the route before anyone measures (4 min).** One route, one duration, one starting state, run
   identically at every station — use the recipe card from Activity 27 if you wrote one. If the route
   differs between stations, the matrix is decoration. Write it on the board where everyone can see
   it.
3. **Rotate: six minutes per station, four stations (25 min).** At each one, capture the same row:
   - **Frame rate** median and 1% low over the route.
   - **Cold load time**, one run.
   - **Peak memory**, and whether the OS issued a low-memory warning.
   - **Battery or temperature delta** across the six minutes, with plugged-in status recorded.
   - **Visual or behavioral notes** — anything that renders, sounds, or behaves differently from the
     station before it. This column catches more real defects than the numbers do.
   Tooling, whatever the station supports:
   - **Android:** `adb shell dumpsys gfxinfo <package> framestats` for frame timing,
     `adb logcat` for warnings and OOM kills, `adb shell dumpsys meminfo <package>` for memory,
     `adb shell dumpsys batterystats` for energy, Android GPU Inspector for a GPU capture, and
     Perfetto for a system trace.
   - **iOS:** Xcode Instruments with the Time Profiler and Allocations, the Metal frame capture and
     GPU counters, and the Energy Log.
   - **Unity:** Profiler attached remotely to a development build over ADB or Wi-Fi, plus the Adaptive
     Performance package if the device supports it. Build a development build with autoconnect, and
     remember that a development build is slower than a release build — compare like with like.
   - **Unreal:** Unreal Insights connected to the device, `stat unit` and `stat rhi` on screen,
     `UE_LOG` markers around the route boundaries.
   - **No tooling at all:** an on-screen frame counter, a stopwatch, and the battery percentage are a
     legitimate row. Say what you measured and how.
4. **Run the interruption pass at two stations (6 min of the rotation).** These take seconds each and
   they are the highest-yield checks on mobile: background and foreground mid-level, incoming call or
   alarm, screen lock and unlock, orientation change, notification shade pulled down mid-action, low
   power mode enabled, and airplane mode toggled during a network call. Record pass or fail per
   device — lifecycle bugs are frequently tier-specific because OS versions differ.
5. **Assemble the matrix and reduce it (10 min).** Build the table: one row per device, columns for
   tier, specs, and each captured metric. Then do the arithmetic that decides your real test budget.
   Count the exhaustive matrix — devices times quality presets times orientation times OS version —
   and write the number. It will be large enough to be obviously unaffordable. Then apply pairwise
   reduction, exactly as in Topic 5: a set of rows that covers every pair of parameter values at
   least once is dramatically smaller than the full cross-product, and it catches the great majority
   of interaction defects. Write both counts and list your reduced set.
6. **Name the divergence (5 min).** Find the one thing in your matrix that is not a smooth gradient —
   a metric that does not scale with device class, a visual artifact on one GPU, a lifecycle failure
   on one OS version, a memory kill on one device. State it as a defect with the device it needs.
   If your matrix shows no divergence at all, say so and state what that means: either your game is
   simple enough not to have one, or your route did not exercise the code that would show it.

## Deliverables — post to Padlet
- [ ] Your filled device matrix: one row per station with tier, chipset, RAM, OS, resolution, refresh rate, and the five captured metrics
- [ ] Your interruption pass results for at least two devices, as pass or fail per check
- [ ] The exhaustive matrix count, your pairwise-reduced count, and the reduced set you would actually run
- [ ] One profiler or tool screenshot from a device, with the metric you were reading circled or labeled
- [ ] A ~150-word write-up naming your divergence, and the minimum viable device set you would commit to in your test plan along with the risk that set accepts

## If you finish early
Take your floor-tier device and find the single quality setting that moves it across a threshold —
resolution scale, shadow resolution, post-processing, texture max size, or a dropped LOD level.
Measure the frame rate before and after on the fixed route. That measurement is the front half of the
optimization you will implement on Tuesday, and having it already in hand means you spend that
session improving the game instead of discovering where to start.

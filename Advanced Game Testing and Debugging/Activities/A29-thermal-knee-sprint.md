# Activity 29 — Find the Knee, Then Move It

> **Topic 8 · Session 29 · Week 15 (Tue Dec 15)** · In class, ~60 min · Solo, with a 15-minute sustained run in the background
> **Feeds:** Mobile Game Optimization (due Dec 20)

## The setup
Start a fifteen-minute sustained play session on your device in the first five minutes of class and
do not stop it. Log the frame rate every two minutes while it runs. Somewhere between minute four and
minute ten your numbers will fall off a cliff you have never seen, because you have never benchmarked
anything for longer than thirty seconds. That cliff is the knee, and it is the real performance
profile of your game. The rest of the session is an optimization sprint aimed at the bound you
identified on Thursday, and a re-measurement done under rules strict enough that the number means
something.

## Why this matters
Phones are thermally constrained in a way that no desktop is. There is no fan and no heatsink worth
the name; sustained load raises the chassis temperature, the governor reduces CPU and GPU clocks to
protect the hardware, and your frame rate settles at whatever the device can dissipate. A game that
runs at 60 fps for the first thirty seconds and 34 fps from minute six onward is a 34 fps game, and
every player who plays for longer than a bus stop experiences it that way. This produces the
counter-intuitive result that is worth carrying into your career: locking to 30 can be *faster on
average* than an unlocked 60, because a lower sustained load keeps the device below the throttle
threshold, so you get a stable 30 forever instead of 60 for four minutes and 24 fps afterward with a
visible frame-time sawtooth. The other half of today is measurement discipline. An optimization
result presented without the controls stated — same device, same route, same starting thermal state,
same build configuration, several runs, median and 1% low reported — is not evidence, and an
experienced reviewer will dismiss it in one question. Your assignment asks for at least two
optimizations with tracked improvements. Today produces one of them, properly measured, and the
protocol you will reuse for the second.

## What to do
1. **Start the sustained run immediately (5 min to set up, then it runs).** Use your own project on a
   phone, a provided sample, any game you can run on a phone you or your partner owns, an emulator
   with the honest caveat that it cannot show you thermal behavior, or the laptop throttle proxy from
   Activity 27 — unplugged, battery saver on, integrated GPU. Before you start, record the controls:
   device, build and configuration, starting battery level, plugged in or not, case on or off, room
   temperature if you can, and the starting device temperature if your tooling exposes it
   (`adb shell dumpsys thermalservice` or the thermal zone files on Android, the Energy and thermal
   state gauges in Xcode). Then play the fixed route on a loop for fifteen minutes without stopping.
2. **Log every two minutes (during the run).** Eight samples: elapsed time, median frame rate over the
   preceding interval, 1% low if you have it, temperature or thermal state if available, battery
   percentage. Write them in a table as you go. Do not reconstruct them afterward from memory.
3. **Find the knee and state it (5 min, after the run).** Plot the eight points — a hand-drawn
   sketch, a spreadsheet chart, or a small table is fine. Then write three numbers in one sentence:
   the frame rate at t=0, the frame rate at t=12 minutes, and the minute at which the decline began.
   Compute the percentage drop. If you see no knee at all, that is a result too — say what it means:
   either you are well under the thermal envelope, or you are bound by something that does not scale
   with clocks, and either conclusion changes what you should optimize next.
4. **Run the 30 versus 60 experiment (8 min).** Lock the frame rate to 30 (`Application.targetFrameRate`
   in Unity, `t.MaxFPS` or the equivalent device profile setting in Unreal, or the in-game frame cap if
   you are testing a shipped game), let the device cool for a few minutes, and run the same route for
   six minutes. Compare the sustained result and the frame-time consistency, not just the average.
   Report what you found honestly even if it contradicts the lecture — some workloads are not thermally
   bound and this experiment shows nothing on them.
5. **Sprint: implement one optimization (20 min).** Aim it at the bound your device lab identified on
   Thursday, not at whatever is easiest to change. On mobile the highest-yield candidates are usually
   bandwidth-side, because tile-based rendering makes bandwidth the scarce resource:
   - **Bandwidth and fill:** cut overdraw (transparent layers, particles, full-screen quads), lower
     the resolution scale or enable dynamic resolution, remove or cheapen a full-screen post effect,
     drop MSAA a level, avoid anything that resolves and re-reads the framebuffer.
   - **CPU:** batching and instancing to cut draw calls, physics tick rate and collision layers, AI
     time-slicing, moving per-frame work off the main thread.
   - **Memory and load:** texture max size per tier, ASTC compression, mesh and audio budgets, asset
     bundle or Addressables grouping to cut cold load time.
   - **Shadows and lighting:** cascade count, shadow resolution, baked versus real-time, distance.
   Change one thing. Two changes measured together give you a result you cannot attribute.
6. **Re-measure under the discipline (12 min).** The rules, and you state them next to the number:
   same device, same build configuration, same route, at least three runs, and the same starting
   thermal state — let the device cool until the starting temperature is within a couple of degrees of
   the earlier run, or wait a fixed cool-down and say how long. Report the **median**, the **1% low**,
   and the **sustained value at minute ten if you can get it, at minute five if you cannot**. Then
   write the honesty line: name at least one variable you were not able to hold constant. Everyone has
   one — ambient temperature, background apps, battery level, a different build configuration. Naming
   it is what makes the rest of the number credible.
7. **Check the cost side (5 min).** Every optimization spends something. State what your change cost
   in visual quality or gameplay feel, in one sentence a designer would accept, and take a
   before-and-after screenshot from the same camera position so the trade is visible rather than
   asserted. If the answer is genuinely "nothing", say that, because a free win is worth flagging.

## Deliverables — post to Padlet
- [ ] Your thermal log: eight rows of elapsed time, frame rate, 1% low, temperature or thermal state, battery, plus the controls you recorded before starting
- [ ] The knee, in one sentence: frame rate at t=0, at t=12, the minute the decline started, and the percentage drop, with your plot or table
- [ ] Your 30 versus 60 result, with the sustained numbers for both
- [ ] The before/after optimization table: three runs each, median, 1% low, sustained value, and the stated controls
- [ ] The same-camera before/after screenshot pair with one line on what the optimization cost, and a ~150-word write-up including the variable you could not hold constant

## If you finish early
Write the second optimization as a prediction before you implement it. State the target, the bound it
attacks, the mechanism, and the number you expect — then implement it after class and see how close
you were. A written prediction that misses by a factor of four teaches you more about your own mental
model of the machine than a successful optimization ever will, and "here is where my model of the
hardware was wrong, and here is what corrected it" is one of the strongest things you can say in an
interview.

# Activity 15 — The Hypothesis Market

> **Topic 5 · Session 15 · Week 8 (Oct 27)** · In class, ~55 min · Teams of three, public betting board, then solo measurement
> **Feeds:** Multi-Layered Game Optimization (due Nov 8)

## The setup
A market opens at the front of the room. Before anyone touches code, every team writes down which
of four candidate optimizations will produce the biggest whole-frame improvement in their project,
and commits to it publicly where the rest of the room can see it. Then everyone measures. The
interesting part of this hour is not who was right. It is the size of the gap between a confident
engineering intuition and what the profiler says, and the fact that the gap is usually enormous in
one specific direction: the thing you were sure about returns a fraction of a percent.

## Why this matters
Amdahl's law is the reason experienced engineers are boring about measuring. If a subsystem is 8%
of your frame and you make it infinitely fast — you delete it entirely — you have bought 8%. That
is the ceiling, and you will not reach it. Every hour you spend making that subsystem twice as
fast buys you 4%, which is 0.66 ms out of a 16.6 ms frame, which is invisible. Meanwhile the
render thread sitting at 47% is where the whole budget lives. Students lose entire weekends to
micro-optimizing a hot-looking function that was never the problem, and then present the result as
an optimization case study with no before-and-after data because the before and after look
identical. Your assignment asks for three layers of optimization with profiling data for each. The
only way to pick three targets worth a week of your life is to rank them by ceiling first, and the
only way to know the ceiling is to measure the share.

## What to do
1. **Establish the baseline share table (12 min).** Use your own project, a provided sample, or any
   game you can profile. Capture a profile of a repeatable scene — reuse the benchmark harness and
   recipe card you built in Activity 13 if you have one. Convert the capture into a share table:
   subsystem, median ms, and percentage of the frame. Six to eight rows, and one row named
   "everything else" so the column sums honestly to the frame time.
   - **Unity:** Profiler hierarchy view, self-ms vs total-ms — know which one you are reading, and
     say so on the table. Timeline view to separate main thread from render thread.
   - **Unreal:** Unreal Insights timing view, or `stat unit` for the Game/Draw/GPU split first and
     `stat game` / `stat scenerendering` to break down the dominant one.
2. **Write four candidates (8 min).** Four specific, implementable optimizations for this project.
   Not "optimize rendering" — "replace the O(n^2) proximity check in EnemyManager with a uniform
   grid", "cut the shadow cascade count from four to two", "pool the projectile prefab instead of
   instantiating", "move the pathfinding update to every fourth frame". Each candidate names the
   row of the share table it attacks.
3. **Place the bet (5 min).** Each team publicly commits, on the board, to one candidate as the
   biggest whole-frame win and states the predicted improvement as a percentage of total frame
   time. Then compute what Amdahl's law actually permits: if the target is share S of the frame and
   you speed it up by factor k, the whole-frame gain is S × (1 − 1/k). Write the ceiling next to
   your bet — the S you get from an infinite k. Several teams will discover at this step that their
   confident bet has a ceiling of 3%, and that is the lesson landing early.
4. **Measure the ceiling by ablation, not by implementation (15 min).** You do not have time to
   implement four optimizations, and you do not need to. For each candidate, find the cheapest way
   to make the subsystem approximately free and measure the frame with it gone. Disable the
   component. Return early from the update. Set the enemy count to zero. Turn shadows off entirely.
   Set the quality preset row to its lowest value. This is an upper bound, not a result — the
   optimized version will always be worse than the deleted version — but it is a bound you can
   measure in three minutes instead of three days. Record all four.
5. **Settle the market (10 min, whole room).** Post the ablation numbers next to the bets. Rank
   candidates by measured ceiling. Find the room's largest prediction error and have that team say
   out loud what misled them: a function that looked expensive because it was long, a subsystem
   that felt slow because it was the newest code, a hot-path assumption inherited from a tutorial.
   Then each team names the one candidate it will actually implement for the assignment, and states
   the realistic gain — the ceiling times a plausible speedup factor, not the ceiling itself.
6. **Note the trap (5 min).** Write one line on why ablation can lie: removing a subsystem can
   also remove work elsewhere (no enemies means no enemy rendering, no enemy audio, no enemy
   physics), which inflates the apparent ceiling. State whether your largest ablation is
   contaminated this way, and how you would isolate it.

## Deliverables — post to Padlet
- [ ] Your baseline share table: subsystem, median ms, percent of frame, summing to the measured frame time
- [ ] Your bet card: the chosen candidate, the predicted whole-frame gain, and the Amdahl ceiling you computed for it
- [ ] The four-row ablation table: candidate, frame time with the subsystem removed, measured ceiling
- [ ] A ~150-word write-up naming your largest prediction error, what misled you, and the target you will implement for the assignment with its realistic expected gain

## If you finish early
Take the candidate with the highest ceiling and ask whether the win is algorithmic or constant-factor.
Count the operations: if the cost grows with n^2 and n is going up next sprint, a 2x constant-factor
win buys you back a single sprint of content, while a grid or a broadphase changes the shape of the
curve permanently. Post the operation count at your current n and at twice your current n. That
comparison is the argument that gets an optimization scheduled instead of deferred.

# Activity 16 — Physics Tournament: Most Agents on the Budget Line

> **Topic 5 · Session 16 · Week 8 (Oct 29)** · In class, ~60 min · Teams of two, running leaderboard on the board, mandatory correctness round
> **Feeds:** Multi-Layered Game Optimization (due Nov 8)

## The setup
One scene, one rule, one number on the board. Spawn interacting agents — enemies, projectiles,
boids, debris, anything that collides and thinks — and find the largest number your build can
simulate while median frame time stays under 16.6 ms. Post that number. Then you have forty minutes
to raise it using physics and game-logic levers only, with no change to rendering and no change to
what the simulation is supposed to do. The leaderboard updates as teams climb. The last round is
the one that decides the tournament: every entry gets checked for whether it still does the job,
and an entry that got fast by quietly doing less is withdrawn by its own authors.

## Why this matters
Physics and gameplay logic are where student projects have the most headroom and the least
discipline, because both are written to work rather than to scale. A naive all-pairs proximity
check at 60 agents is 1,770 comparisons per frame and nobody notices; at 400 agents it is 79,800
and the game is unplayable, and the code did not change. Every studio has the same story about a
gameplay system that was fine in the vertical slice and fell over the week content doubled. The
levers you use today — broadphase structure, collision matrices, sleeping, timestep, time-sliced
AI — are the entire first layer of your assignment, and they are the layer with the largest
measured wins in most student submissions. The correctness round matters just as much: an
optimization that changes gameplay behavior is not an optimization, it is a defect with good
profiling data attached.

## What to do
1. **Build the arena and find your baseline number (12 min).** Your own project, a provided
   sample, or a scene you write here. It needs agents that collide with each other and the world,
   and at least one per-agent decision each frame (pick a target, avoid a neighbor, path toward a
   goal). Ramp the agent count until median frame time crosses 16.6 ms, back off to the last count
   that held, and post that number as your baseline. Fix everything else: resolution, quality
   preset, vsync off, fixed seed, fixed run length. Write the recipe down; you will re-run it a
   dozen times today and every re-run has to be comparable.
2. **Write the behavior contract before you optimize (5 min).** Three to five sentences describing
   what must remain true: every agent-to-agent overlap is detected, no agent passes through a wall,
   agents reach their target in the same number of seconds, the same seed produces the same
   outcome. This is your oracle. Without it, "faster" is unfalsifiable.
3. **Climb the leaderboard (30 min).** Apply levers one at a time and record the ms delta for each.
   One at a time is not a style preference — two changes at once and you cannot attribute the win,
   and your assignment needs attributable before/after data per change.
   - **Broadphase.** Replace all-pairs distance checks with a uniform grid, spatial hash, or the
     engine's own overlap query. Count comparisons per frame before and after and post both counts.
   - **Collision filtering.** Turn off the pairs that can never matter. Unity: the layer collision
     matrix in Physics settings — most projects have every box checked because nobody ever
     unchecked one. Unreal: collision presets and object channels, plus setting collision to query-only
     where you never needed a physical response.
   - **Sleeping.** Bodies that have come to rest should stop costing. Unity: rigidbody sleep
     threshold, and stop nudging transforms of sleeping bodies. Unreal: sleep family settings, and
     checking `stat physics` for how many bodies are actually awake.
   - **Timestep and substeps.** Unity: Fixed Timestep and Maximum Allowed Timestep in Time settings
     — a 0.01 fixed step runs physics 100 times a second whether or not your game needs it.
     Unreal: substepping in the physics settings, with an honest look at whether your max substeps
     is protecting anything.
   - **Collider shape.** Mesh colliders where a capsule, box, or convex hull would do. Look at what
     the collider actually needs to be, not what the mesh happens to be.
   - **Continuous collision.** On by default for everything is a common and expensive mistake. Keep
     it on the fast small things that tunnel and turn it off everywhere else.
   - **Time-sliced logic.** Update expensive decisions on a rotating subset of agents each frame,
     or scale update rate by distance to the camera — behavior LOD. Record the frames-per-decision
     you chose and how you keep the work evenly spread rather than spiking every Nth frame.
4. **Correctness round (10 min, cross-team).** Swap builds with another team. Run their behavior
   contract against their optimized build and try to break it: agents overlapping without a hit,
   a projectile through a wall at high speed, a decision update so slow that the agent visibly
   reacts late, a seeded run producing a different outcome than the baseline. Withdraw or annotate
   anything that fails. Post the check you ran and the verdict.
5. **Post the final number (3 min).** New agent count at 16.6 ms, and the lever table with ms
   deltas so anyone can see which lever earned it.

## Deliverables — post to Padlet
- [ ] Your leaderboard line: baseline agent count at 16.6 ms, final agent count, and the multiplier between them
- [ ] The lever table: lever applied | ms delta | comparisons or awake-body count before and after, where applicable
- [ ] Your behavior contract plus the cross-team correctness verdict on it, signed by the team that checked you
- [ ] A screenshot or short clip of the arena at your final agent count, with a profiler or `stat unit` readout visible

## If you finish early
Take your best lever and check whether it holds on a different machine class — a laptop on battery,
integrated graphics, a phone if you can deploy, or the same machine with the CPU governor set to
power-saving. A broadphase win is usually portable; a win that came from raising the timestep is
usually not, and can flip to a loss where the frame rate is already unstable. Post the cross-machine
comparison. That contrast is exactly the "impact across platforms" analysis the assignment asks for.

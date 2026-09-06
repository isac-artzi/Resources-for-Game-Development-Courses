# Activity 21 — Cut Forty Percent, Keep It Shippable

> **Topic 6 · Session 21 · Week 11 (Nov 17)** · In class, ~60 min · Solo build work with a mid-session fragmentation demo run as a room
> **Feeds:** Memory Optimization (due Nov 22)

## The setup
One constraint, stated once: reduce your project's peak memory by forty percent, with the gameplay
intact and the art direction recognizable. You do not get to cut a feature. You do not get to remove
a level. You get import settings, load types, pooling, allocation discipline, and streaming — and
you get to write down exactly what you gave up, because there is always something. Halfway through,
the room stops and runs a fragmentation experiment together, because the number that decides whether
you fit on a fixed-memory device is not how much you are using. It is how much the allocator has
reserved on your behalf, and those two numbers drift apart in a way that surprises everyone the
first time they watch it happen.

## Why this matters
Three hundred megabytes free and a forty megabyte allocation that fails is not a paradox, it is
Tuesday on a console. Free memory that is scattered in small pieces between long-lived objects
cannot satisfy a large contiguous request, and the classic pattern that creates it is exactly what
gameplay code does naturally: allocate and free objects of varying sizes over a long session. The
project runs fine for twenty minutes in your testing and dies at hour three in someone else's. The
defenses are unglamorous and they work — pool the objects you create and destroy repeatedly, size
your pools once at load, keep long-lived and short-lived allocations apart, and stop allocating in
`Update` at all. Your assignment asks for two documented memory issues with measured before-and-after
data plus prevention strategy; a reserved-versus-used gap that you closed with pooling is a
first-rate second issue, and it demonstrates something more sophisticated than "I made a texture
smaller".

## What to do
1. **Establish peak, and record reserved separately from used (10 min).** Use your own project, a
   provided sample, or any project you can profile. Play through the heaviest thing you have — the
   busiest level, the biggest fight, a scene transition — and record two numbers, not one: total
   used and total reserved. Set your target at sixty percent of the peak and write it down where you
   can see it.
   - **Unity:** the Memory Profiler snapshot summary distinguishes used from reserved, per allocator.
     `Profiler.GetTotalReservedMemoryLong` and `GetTotalAllocatedMemoryLong` give you both at runtime
     so you can log them over a session.
   - **Unreal:** `stat memory` shows allocator totals including virtual versus physical; `memreport -full`
     writes the per-category detail; LLM tags it by subsystem.
2. **Run the fragmentation demo as a room (12 min).** Everyone does the same thing at once. Write or
   enable a loop that repeatedly allocates and frees objects of deliberately mixed sizes — spawn and
   destroy a mix of small projectiles, medium enemies, and occasional large temporary buffers, in a
   random but seeded order, for several thousand iterations. Log used and reserved every hundred
   iterations. Watch used stay roughly flat while reserved climbs and never comes back down. Post
   the two curves. That gap is fragmentation plus allocator retention made visible, and it is the
   single most useful screenshot you will produce in this topic.
3. **Cut toward the target (25 min).** Work down the list by size of win, one change at a time, and
   record the delta for each so you can attribute it later.
   - **Asset-side.** Texture max size and platform compression format; mipmaps on where they belong;
     Read/Write and CPU access off wherever nothing reads the data back; mesh compression and LODs;
     audio load type — streaming for music, compressed in memory for frequent short sounds, and
     decompress-on-load reserved for the very few clips that genuinely need zero latency.
   - **Pooling.** Take the object your game creates and destroys most often and pool it: preallocate
     a fixed count at load, deactivate and recycle rather than destroy, and make the pool size an
     explicit, tuned number rather than an unbounded growth. Re-run the loop from step 2 against the
     pooled version and post the new reserved curve next to the old one.
   - **Allocation discipline.** Remove per-frame allocations from hot paths: string building for UI
     text, LINQ over per-frame collections, closures created in `Update`, boxing through
     non-generic collections, `TArray` growth without a `Reserve`, temporary containers that could
     be fields reused across frames.
   - **Residency.** Load less at once. Level streaming, Addressables or asset bundles, releasing
     handles for content you left behind, texture streaming budgets. This is usually the largest
     single lever and the most disruptive, so make it last and measure carefully.
4. **Write the concession list (8 min).** Three things you gave up, each with the specific cost:
   "props beyond twelve meters now pop one LOD level earlier", "the boss theme streams, so it starts
   about 80 ms after the trigger", "the pool caps at 200 projectiles and the 201st is recycled from
   the oldest live one, which is visible only during the ultimate". A cut you cannot describe is a
   cut you have not tested.
5. **Report honestly (5 min).** Post the before and after for peak used, peak reserved, and the
   percentage cut against your target. If you landed at twenty-two percent instead of forty, say
   twenty-two and name what the next lever would have been. An honest partial result with a stated
   next step reads as competence; a suspiciously exact forty does not.

## Deliverables — post to Padlet
- [ ] The two-curve screenshot from the fragmentation demo: used flat, reserved climbing, before and after pooling
- [ ] Your cut table: change | memory delta | which bucket it came from, sorted by size of win
- [ ] Before/after peak used and peak reserved, with the percentage reduction and your target stated
- [ ] The three-item concession list with a specific, testable description of each cost

## If you finish early
Take your pooled system and make it defensible under stress: what happens when the pool runs dry
mid-fight? Grow it, recycle the oldest, or refuse the spawn — pick one, implement it, and write down
which failure mode a player would notice least. Then run your long session again and check that
reserved memory now has a ceiling. A pool with an undefined exhaustion policy is a crash waiting for
the busiest moment in your game, which is exactly when it will happen.

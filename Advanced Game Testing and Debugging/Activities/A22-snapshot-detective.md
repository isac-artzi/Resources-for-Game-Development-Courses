# Activity 22 — Snapshot Detective and the Allocation Rate Cut

> **Topic 6 · Session 22 · Week 11 (Nov 19)** · In class, ~60 min · Pairs swap evidence, then solo work on the allocation rate
> **Feeds:** Memory Optimization (due Nov 22)

## The setup
Each pair performs a secret sequence of actions between two memory snapshots and hands the resulting
diff — the diff only, not the build, not the description — to another pair. From that evidence
alone you reconstruct what happened: what got loaded, what got spawned and how many, what was
supposed to be released and was not, and whether the session ended where it started. Then you turn
to your own project for the second half and go after the number that drives garbage collection
spikes, which is not heap size and is not object count. It is how fast you allocate. The assignment
is due in three days and this session is designed to produce its two strongest exhibits.

## Why this matters
Snapshot diffing is the most transferable memory skill there is, and the reason is that it turns an
open-ended question into a closed one. "Why is memory high" has no method attached. "What is
different between these two moments, and which of those differences should not be there" has a
method, a tool, and an answer. Reading somebody else's diff without knowing the answer is the
purest version of that skill, because you cannot fall back on knowing your own code. The second
half addresses the other half of the topic: in a garbage-collected engine, allocation rate is what
determines how often the collector runs, and every collection is a frame-time spike that shows up
in your 1% lows while your average frame rate looks perfectly healthy. A game that allocates 2 KB
per frame at 60 fps is producing about 7 MB per minute of garbage that someone has to collect, and
the fix is almost never "tune the collector" — it is "stop allocating in `Update`". Knowing that
ordering, and being able to show the before-and-after allocation rate, is the difference between a
memory report that describes symptoms and one that fixes causes.

## What to do
1. **Stage the crime scene (12 min, pairs).** In your own project, a provided sample, or any project
   you can profile, take a snapshot, perform a deliberate sequence of six to eight actions, then take
   a second snapshot. Build the sequence so the diff has a story in it: load a level, spawn a
   quantity of something specific, open and close a UI, trigger audio, return to the main menu, force
   a collection. Include at least one action that should have released memory and, if you can arrange
   it honestly, one that did not. Write the true sequence on a card and keep it hidden. Export or
   screenshot the diff view: type or category, count delta, size delta. Hand over the diff.
   - **Unity:** Memory Profiler Compare mode between two snapshots, sorted by count delta and by size
     delta — capture both sorts, because they tell different stories.
   - **Unreal:** `memreport -full` before and after with the category tables diffed, plus `obj list`
     counts by class for the object-count view. `gc.CollectGarbage` before the second capture.
2. **Read the evidence (15 min, cross-pair).** With no access to the build, write your reconstruction
   as a numbered timeline, and next to each step cite the specific line of the diff that supports it.
   The reasoning patterns worth practicing out loud: a large jump in one asset category means content
   loaded; a count delta on a gameplay type means spawning, and the ratio of size to count tells you
   how heavy each one is; a category that grew and did not shrink after the return-to-menu step is
   the suspect; render target growth points at a resolution or post-processing change rather than
   gameplay; a big managed heap delta with a small object-count delta suggests large arrays or
   strings rather than many objects. Commit your timeline before the reveal.
3. **Reveal and reconcile (8 min).** Compare against the card. Where you were wrong, name what in the
   diff misled you — it is usually a category whose name does not mean what you assumed, or a
   collection that had not run yet when the second snapshot was taken. Where you were right about
   something the owning pair had not noticed, tell them; teams routinely discover their own unplanned
   retention this way, and that discovery is a legitimate second issue for the assignment.
4. **Cut your allocation rate (20 min, solo, your own project).** Measure first, in bytes per frame.
   - **Unity:** the GC Alloc column in the Profiler hierarchy, sorted descending, with Deep Profile
     only if you need it and with the understanding that deep profiling distorts timings. Watch the
     GC Alloc total per frame and the sawtooth of the managed heap in the Memory module.
   - **Unreal:** garbage collection time in `stat game`, object counts via `obj list`, and LLM for
     where allocations are tagged. C++ allocation in hot paths shows up in Insights as time inside
     the allocator.
   Find your top three per-frame allocators and kill them. The usual convicts: string concatenation
   for on-screen text, LINQ in a per-frame path, closures and lambdas created inside `Update`,
   boxing a struct into an `object` or a non-generic collection, `GetComponent` or `Find` in a hot
   path returning arrays, a new temporary list or array every frame, `TArray` growth without
   `Reserve`. Re-measure bytes per frame and record both numbers.
5. **Then, and only then, look at the collector (5 min).** With the rate reduced, note what tuning is
   even available and be honest about what it does. Unity's incremental garbage collector spreads
   collection over several frames — it converts one large spike into several small ones, which helps
   your 1% lows but does not reduce total work and adds a little overhead. Unreal's clustering and
   the timing knobs around purging pending-kill objects move when the cost lands, not whether it
   exists. Write one sentence on what you changed, or one sentence on why you correctly changed
   nothing. Both are respectable; tuning a collector to hide an allocation problem is not.

## Deliverables — post to Padlet
- [ ] The diff you were handed plus your reconstructed timeline, each step citing the diff line that supports it, and one line on what misled you
- [ ] Your allocation rate before and after, in bytes per frame, from the same scene and route
- [ ] The three allocators you killed, as short before/after snippets
- [ ] A ~150-word write-up on the 1% low or GC spike change you measured, and the one collector setting you tuned or deliberately left alone, with your reason

## If you finish early
Assemble the assignment exhibit while everything is still open. Two issues, each with: the snapshot
or profiler evidence that found it, the reference chain or allocation site that caused it, the fix,
the after measurement from an identical scene and route, and one prevention rule stated so a
teammate could follow it on a different platform. Post the outline. Writing the outline while the
tools are open takes fifteen minutes; reconstructing it on Saturday from memory takes three hours
and the numbers will not match.

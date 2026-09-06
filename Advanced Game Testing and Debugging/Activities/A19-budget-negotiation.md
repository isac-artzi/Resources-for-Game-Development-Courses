# Activity 19 — The Budget Negotiation

> **Topic 6 · Session 19 · Week 10 (Nov 10)** · In class, ~55 min · Room splits into designers and engineers, negotiate in fours, signed agreement at the end
> **Feeds:** Memory Optimization (due Nov 22)

## The setup
Your producer walks in with a ceiling: the title has to run on a device where you get roughly 3 GB
for everything the game owns, and that is not negotiable because it is the hardware. Everything
below the ceiling is negotiable, and that is what this session is. Half the room plays design and
technical art, defending the texture budget, the audio, the number of distinct enemy types loaded at
once. Half plays engineering, defending headroom, the streaming pool, and the fact that memory does
not degrade gracefully. You have forty minutes to produce a one-page allocation table that both
sides sign, and then you measure your actual project against it and find out who was lying.

## Why this matters
Frame time is elastic. If you go over, the game gets slower and players complain. Memory is a wall.
If you go over on a fixed-memory device you do not get a slower game, you get a termination — the
platform kills you, cert rejects you, and the crash report says nothing useful because the
allocation that failed was the innocent 2 MB one that happened to be last. That asymmetry is why
memory is budgeted up front and per-system, and why "we will optimize it later" is a sentence that
ends projects. This is also the argument you are structurally set up to lose in industry, because
the person asking for a larger texture budget has something beautiful to show and you have a
spreadsheet. You win it by arriving with the ceiling, the buckets, the reserve, and a measurement,
which is precisely what your assignment's cross-platform prevention strategy needs to contain.

## What to do
1. **Take a side and take a snapshot (12 min).** Draw your role. Whatever side you are on, first get
   real numbers from your own project, a provided sample, or any project you can open, because both
   sides argue better with data.
   - **Unity:** Memory Profiler package — take a snapshot in a development build if you can, since
     editor snapshots include the editor and will mislead you. Read the summary breakdown: managed
     heap used vs reserved, graphics/native, and the largest object categories. `Profiler.GetTotalReservedMemoryLong`
     for a runtime readout you can print on screen.
   - **Unreal:** `stat memory` for the quick view, `memreport -full` for the real one — it writes a
     file with per-category totals — and the Low Level Memory Tracker (`stat llm`, with LLM enabled)
     for allocation tagged by subsystem. `obj list` sorted by class tells you what is resident.
2. **Draft the buckets (10 min, in role).** The agreement covers a fixed set of buckets, each with a
   number in megabytes: textures, meshes and geometry, animation, audio, code and managed heap,
   render targets and GPU-side buffers, level and streaming pool, and reserve. Do not skip reserve.
   Ten to fifteen percent of the ceiling stays unallocated to absorb fragmentation, platform
   overhead, and the spike when a player opens the map during a streaming load. Every side wants to
   spend the reserve; the reserve is what stops the crash.
3. **Negotiate (18 min, groups of four, two per side).** Argue with specifics. Design's strongest
   moves: name what the player actually looks at, propose a tier instead of a cut, offer to trade
   variety for resolution or the reverse, and demand to know the measured cost rather than the
   feared one. Engineering's strongest moves: show the snapshot, quantify what a bucket overrun does
   to the lowest target device, offer streaming or on-demand loading instead of an outright cut, and
   convert every request into "which bucket does that come out of" — the question that ends
   unbounded asks. Both sides must concede at least one thing in writing. An agreement where one
   side won completely is a fantasy that will be renegotiated in a hallway next week.
4. **Sign it (5 min).** The one-page table: bucket, agreed cap in MB, who owns it, and one line per
   bucket saying what happens when it goes over — what gets cut first, and who decides. Both names
   on it. This "over-budget policy" column is the part that makes the agreement useful under pressure
   and the part every student version omits.
5. **Reality check (10 min).** Measure your actual project against your signed table. Fill in an
   actuals column. Almost certainly one or two buckets are over, and one is dramatically under
   because you guessed. For each overrun, write the specific first cut you would make — not "reduce
   textures" but "drop the four hero-prop albedos from 2048 to 1024, recovering 18 MB". For the
   bucket you badly misjudged, write one line on why your intuition was wrong; that sentence is
   usually the most valuable thing produced in this hour.

## Deliverables — post to Padlet
- [ ] The signed budget table: bucket | agreed cap | owner | over-budget policy | measured actual, with the reserve line explicit and both negotiators named
- [ ] Your snapshot screenshot showing the totals you argued from, with the tool and build type stated
- [ ] The concession you made and the concession you extracted, one line each
- [ ] A ~150-word write-up on the bucket your project misjudged most and the specific first cut you would make there

## If you finish early
Re-run the negotiation against a second ceiling — the same game on a device with half the memory.
Halving the ceiling does not halve every bucket; code and managed heap barely move, render targets
scale with resolution, and textures absorb most of the difference. Post the two-column comparison
and name the bucket that refused to shrink. That is your cross-platform prevention argument, and it
is the section of the assignment where most submissions have nothing concrete to say.

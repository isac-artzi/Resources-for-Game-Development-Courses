# Activity 18 — Code Archaeology, Then Defend Your Matrix

> **Topic 5 · Session 18 · Week 9 (Nov 5)** · In class, ~60 min · Solo reading in the first half, teams of three for the matrix defense
> **Feeds:** Multi-Layered Game Optimization (due Nov 8)

## The setup
Two halves, both uncomfortable. First you read somebody else's shipped engine code — real code from
a real open-source game or engine — and work out why a professional wrote a hot loop the strange
way they wrote it, because the strange way is almost always the fast way and the reason is almost
never in a comment. Then you turn to your own project's quality settings, compute how many
configurations you would have to test to be exhaustive, look at the number, and build a pairwise
set instead. You will then defend that reduction to a team playing the role of the person who has
to sign off on shipping with it. The assignment is due in three days; both halves go straight into it.

## Why this matters
The third optimization layer is code patterns, and it is the layer students most often fake,
because "I refactored for cache locality" is easy to say and hard to show. Reading code that was
optimized by people who had to hit a frame budget on fixed hardware is the fastest way to build the
pattern vocabulary that makes your own third layer real: pooling, freelists, preallocated arrays,
structure-of-arrays layouts, branch elimination, virtual calls hoisted out of inner loops.
The second half exists because optimizations do not stand alone — they ship as flags, and flags
multiply. Four quality parameters with three values each is 81 configurations; add two hardware
tiers and a rendering backend toggle and you are past 300. Nobody tests 300 configurations. The
combinatorial testing literature is clear about why you do not have to: the great majority of
interaction defects are triggered by a pair of parameters, so a set that covers every pair, which
is often under fifteen rows, catches most of what exhaustive testing would catch for one-twentieth
of the effort. Being able to explain that arithmetic calmly to a producer is a genuine
career-differentiating skill, and it is the argument that gets your reduced test plan approved.

## What to do
1. **Dig a hot loop out of real code (22 min, solo).** Open a real, public game or engine codebase.
   Good digs: Godot Engine's server and physics code, id Software's released Quake and Doom sources,
   Box2D's broadphase, an ECS library like EnTT, Unity's public C# reference source, or Unreal's
   source if you have access. Find one function or data structure that was clearly written for
   speed rather than clarity, and answer four questions in writing: what does it do; what would the
   obvious naive implementation look like; what specifically did the author trade away — readability,
   memory, generality, precision; and what hardware fact makes it pay off — cache line size, branch
   prediction, allocation cost, virtual dispatch, SIMD width. Post the snippet with your reasoning,
   not the author's. If a comment already explains it, keep digging until you find one that does not.
2. **Find the same pattern, or its absence, in your own code (8 min).** Locate one place in your
   project where the pattern you just read about applies: a per-frame allocation inside `Update`, a
   `GetComponent` or `FindObjectOfType` call in a hot path, string concatenation building UI text
   every frame, a LINQ chain over a per-frame collection, boxing through a non-generic container, a
   `TArray` that grows without a `Reserve`, an interface call inside a loop over ten thousand
   elements. Write the two-line fix, or the reason it does not apply here. This is your third-layer
   candidate and you now have a shipped-code precedent to cite for it.
3. **Compute the exhaustive matrix (8 min, teams of three).** List your project's real quality and
   optimization parameters with their values — for example: shadow quality (off, low, high), texture
   resolution tier (half, full), anti-aliasing (off, FXAA, TAA), dynamic resolution (on, off), agent
   count preset (low, high), and hardware tier (integrated, mid GPU, mobile). Multiply the value
   counts. Write the total on the board and next to it write your honest minutes-per-configuration
   for a benchmark run, then the total hours. That number is the whole argument.
4. **Build the pairwise set (12 min).** Construct a set of configurations where every pair of values
   from every pair of parameters appears in at least one row. Greedy construction by hand works
   fine at this size: start with a row, then repeatedly add the row that covers the most
   not-yet-covered pairs. Track your coverage as you go. Stop when every pair is covered, and count
   your rows. Then state, explicitly, what the reduction does not cover: three-way interactions.
   Name one plausible three-way failure in your project — say, dynamic resolution on, plus TAA, plus
   the integrated-GPU tier, producing a shimmer that neither pair alone reveals — and add it back as
   a named seed row. A pairwise set with two hand-picked seed rows is the professional answer.
5. **Defend it (10 min, cross-team).** Swap with another team, who plays the producer asking the
   only two questions that matter: what could ship broken that this plan would miss, and why should
   they accept that risk. Your defense needs the arithmetic, the seed rows, and one more thing — the
   oracle. For each row, what tells you it passed? Frame time under budget is one check; the other
   is a metamorphic relation, a property that must hold no matter what the settings do. "Lowering
   shadow resolution must not change where the player can stand", "changing anti-aliasing must not
   change the damage number", "the same seed must produce the same enemy path at every quality
   preset". Post the relations you chose. A quality setting that changes gameplay is the defect
   pairwise testing is really hunting.

## Deliverables — post to Padlet
- [ ] Your archaeology snippet plus a ~150-word explanation of what the author traded and what hardware fact makes the trade pay
- [ ] The matching pattern in your own project: the offending lines and your fix, as a short before/after snippet
- [ ] The arithmetic line: parameters and values, exhaustive configuration count, minutes per run, total hours
- [ ] Your pairwise table with the row count, the added seed row and the three-way interaction it guards, and the metamorphic relation you will check on every row
- [ ] One sentence from your defense that the opposing team accepted, and one objection you could not answer

## If you finish early
Run three rows of your pairwise set for real and record frame time plus your metamorphic relation
verdict for each. Three measured rows in your assignment beats a beautifully constructed table that
was never executed, and if one of the three shows an interaction — a setting combination that is
slower than either setting alone would predict — you have found the exact result this topic is
about and it belongs on your first slide.

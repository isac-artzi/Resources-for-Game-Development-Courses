# Activity 12 — Profiler Teach-Back

> **Topic 4 · Session 12 · Week 6 (Oct 15)** · In class, ~60 min · Pairs, one panel each, three minutes at the front
> **Feeds:** Performance Profiling Techniques (due Oct 25)

## The setup
Nobody learns a profiler by being shown a profiler. Each pair gets exactly one panel — one Unity
Profiler module or one Unreal Insights track or `stat` group — and fifteen minutes to become the
room's expert on it. Then three minutes at the front: what it measures, where the number physically
comes from, what a bad reading looks like, and the one question it cannot answer. Twelve pairs,
twelve panels, and everyone leaves with an index of which tool answers which question. The teaching
is the point; explaining a panel out loud is how you find out you did not understand the units.

## Why this matters
Your assignment asks you to justify a tool for each of three niche areas. "I used the Unity
Profiler" is not a justification. "I used the Rendering module because the symptom was a draw-call
count that scaled with enemy count, and the Frame Debugger because I needed to see which of those
calls broke batching" is a justification, and the difference is knowing what each panel is
instrumented to see. There is a second reason to be picky about tools: every profiler perturbs what
it measures. A sampling profiler interrupts at intervals and infers; an instrumented profiler
inserts timing code and charges you for it. Unity's Deep Profile mode will happily make your frame
five times slower and then show you a beautifully detailed picture of a game that no longer exists.
Knowing which mode you are in, and what it costs, is the difference between data and fiction.

## What to do
1. **Draw a panel (3 min).** One per pair, no duplicates. Unity side: CPU Usage Timeline view;
   CPU Hierarchy view including the Calls and GC Alloc columns; Memory module and a snapshot;
   Rendering module; Frame Debugger; Audio module; Physics module; Deep Profile mode versus normal
   sampling. Unreal side: Insights Timing view with the Game, Render, RHI and GPU tracks; Insights
   Counters track; Memory Insights or the Low Level Memory Tracker; `stat unit` and `stat unitgraph`;
   `stat scenerendering`; `stat gpu`; `stat game`; a RenderDoc capture's event browser. If your pair
   cannot run one engine, take a panel on the other, or take RenderDoc against any game you can
   launch.
2. **Learn it under pressure (15 min).** Open your panel on a real running project — yours, a
   provided sample, or anything you can launch — and answer four questions in writing. What exactly
   does this panel measure, in what units? Where does the number come from — sampled, instrumented,
   or queried from the driver? What does a healthy reading look like here, with a number? What does
   a bad reading look like, and what would you open next? Capture a screenshot of both a normal and
   an abnormal reading; if you cannot produce a bad reading naturally, cause one — spawn a thousand
   objects, force a texture reload, allocate in a loop.
3. **Build the cheat card (7 min).** One screenshot with three or four annotations drawn on it,
   labeling the column or track that matters and what number should alarm you. This card is the
   artifact; it should be usable by a classmate who has never opened your panel, six weeks from now,
   at midnight.
4. **Teach the room (24 min total, three minutes per pair, timed hard).** Show the panel, say what
   it is for, show the bad reading, say what you would open next, and name one thing this panel
   cannot tell you. That last item is the one people forget: the Rendering module will not tell you
   why a shader is expensive, `stat unit` will not tell you which system inside the game thread is
   at fault, and a memory snapshot will not tell you when the allocation happened. Take one question
   from the room; if you cannot answer it, say so and write it down.
5. **Assemble the room index (8 min).** As the presentations run, everyone keeps a running two-column
   list: symptom on the left, panel to open on the right. At the end, write your own top three — for
   the three niche areas you intend to pick for the assignment, which panel do you open first, and
   which one do you open second when the first is inconclusive.

## Deliverables — post to Padlet
- [ ] Your annotated cheat card: one screenshot of your panel with three or four labels and the number that should alarm a reader
- [ ] Your bad reading, captured, with one sentence on what caused it and what you would open next
- [ ] The one question your panel cannot answer, in a single sentence
- [ ] Your personal index: three symptoms you expect in your assignment, and the first and second panel you would open for each

## If you finish early
Measure the observer effect on your own panel. Run the same route with the profiler off (using an
external frame-time readout), then on, then in the heaviest mode your panel offers — Deep Profile,
or Insights with every track enabled — and post the three frame-time averages. Being able to say
"instrumentation cost me 4.2 ms per frame, so I profiled the shipping-style build for the final
numbers" is exactly the discipline that separates a credible performance report from a plausible one.

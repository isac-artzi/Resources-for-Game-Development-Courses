# Activity 14 — The Symptom Clinic

> **Topic 4 · Session 14 · Week 7 (Oct 22)** · In class, ~60 min · Solo spike, then a rotating hot seat
> **Feeds:** Performance Profiling Techniques (due Oct 25) and the midterm exam (90 min, this window)

## The setup
For the next twenty minutes you are the room's consultant on one niche performance area, and you
have to earn that by taking an actual measurement rather than reading about the tool. Then the
clinic opens. Symptom cards get read aloud, you get ninety seconds in the hot seat, and you have to
answer four things without opening anything: what is the likely bound, what tool do you open first,
what number do you read first, and what would prove you wrong. That last question is the one the
midterm will keep asking in different costumes, and it is the one that separates diagnosis from
guessing.

## Why this matters
Everything in the first half of this course converges on this hour. A symptom arrives — a
complaint, a crash, a stutter, a red pipeline — and the professional response is a fast, defensible
first move plus a statement of what would falsify it. That is the same move whether the symptom is
a defect to be triaged by severity and priority, a repro that fails on someone else's machine, a
test that goes red one run in eight, or a frame-time graph with a spike every 1.4 seconds. Your
assignment needs three niche areas with a justified tool each, and the justification has to name
what the tool can see that the alternative cannot. The midterm needs the same reasoning under a
clock, on material you cannot look up in the moment. Practicing the reasoning out loud with people
who will argue back is a far better preparation than rereading anything.

## What to do
1. **Claim an area (3 min).** From the eight: cache usage, network latency and bandwidth, disk I/O
   and streaming, shader performance, physics bottlenecks, threading overhead, audio load, garbage
   collection and fragmentation. No more than three people per area — if yours fills, take the one
   you know least, which is likely the one you should be picking for the assignment anyway.
2. **Spike it (20 min).** Two outputs. First, a tool choice with a rejected alternative, in one
   sentence: what your tool can see that the other cannot. Second, one real measurement from your
   own project, a provided sample, or any game you can run, captured as a screenshot with the key
   number circled. Starting points, not a required list:
   - **Cache** — Intel VTune or `valgrind --tool=cachegrind` on a standalone loop; measure miss rate
     on an array-of-structs layout versus a struct-of-arrays one.
   - **Network** — Wireshark for what is actually on the wire, versus in-engine net stats for what
     the engine believes it sent; measure packet rate and payload size, not just ping.
   - **I/O and streaming** — Unreal Insights file activity, Windows Performance Analyzer, or
     Perfetto; measure time to first interactive frame and where the load thread blocks.
   - **Shader** — RenderDoc or PIX for a frame capture, Unity Frame Debugger for draw-call order;
     read cost per pass and overdraw rather than total GPU time.
   - **Physics** — Unity Physics profiler module and Physics Debugger, or `stat physics`; measure
     active rigid bodies, contacts, and solver iteration cost.
   - **Threading** — Insights thread tracks or Unity's Timeline view with the Jobs system; look for
     a worker thread that is idle while the main thread is saturated, which is the real symptom.
   - **Audio** — Unity Audio profiler module or `stat audio`, plus the Wwise or FMOD profiler if the
     project uses one; measure active voices and decode cost.
   - **GC and fragmentation** — Unity Memory Profiler snapshots plus the GC Alloc column, or the Low
     Level Memory Tracker and `memreport -full` in Unreal; measure allocation rate per frame, since
     that is what drives collection frequency.
3. **Open the clinic (22 min, rotating hot seat, 90 seconds each).** Symptom cards are drawn at
   random and read aloud. The person in the hot seat answers four things, out loud, with no tools:
   likely bound or cause, first tool opened, first number read, and what result would falsify the
   hypothesis. The room may challenge once. Sample cards, and write two more of your own before you
   start:
   - Average FPS is 61 but players say it stutters constantly.
   - The build runs at 60 in the editor and 22 in the packaged build.
   - A test passes locally and fails in the pipeline about one run in eight.
   - Frame time is fine for ten minutes, then degrades and never recovers.
   - Memory climbs 4 MB per level load and never comes back down.
   - The bug reproduces for you five times out of five and the assigned engineer marked it Cannot
     Reproduce.
   - Two producers disagree about whether an S2 that happens to one player in a thousand outranks
     an S3 that happens to everyone.
   - GPU time is flat at 15.9 ms regardless of what you remove from the scene.
   - A shop purchase works in play but the currency is wrong after a relaunch.
   - CPU frame time doubled after a change that only touched a data file.
4. **Write your field card (12 min, solo).** One page, in your own words, built from what the room
   said rather than from the slides. Five blocks: how to identify the bound in sixty seconds and
   which two numbers you compare; the six defect triggers with a one-line example each; the four
   common repro repairs and what Cannot Reproduce actually indicates; severity versus priority as
   separate axes with an example where they diverge; and percentile literacy — average versus 99th
   percentile and why the average hides hitches. Keep it to a page. If it needs two pages you are
   copying, not compressing, and compression is what makes it useful under a ninety-minute clock.
5. **Trade cards and stress-test them (3 min).** Swap field cards with a neighbor and each mark the
   one line that would not help you at speed — too vague, too long, or restating a term instead of
   giving the move. Fix that line before you post.

## Deliverables — post to Padlet
- [ ] Your three chosen niche areas with the tool for each and the rejected alternative, one sentence apiece
- [ ] One measurement screenshot from your spike with the key number circled and the units stated
- [ ] Your hot-seat answers for the two cards you drew: bound, first tool, first number, what would falsify it
- [ ] A photo or snippet of your one-page field card, with the line your neighbor made you rewrite

## If you finish early
Take the symptom card the room argued about most and turn it into a written diagnostic plan: three
hypotheses ranked by likelihood, the measurement that separates them, and the order you would run
them in. A ranked hypothesis list with a discriminating test is what a senior engineer produces when
handed a vague complaint, and it is a better interview answer than any tool name.

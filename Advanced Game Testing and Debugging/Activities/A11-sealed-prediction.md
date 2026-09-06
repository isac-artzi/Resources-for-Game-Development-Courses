# Activity 11 — The Sealed Prediction

> **Topic 4 · Session 11 · Week 6 (Oct 13)** · In class, ~55 min · Solo prediction, pairs to measure, room reveal
> **Feeds:** Performance Profiling Techniques (due Oct 25)

## The setup
Before anyone opens a profiler today, everyone writes down where they think the frame time goes —
in milliseconds, category by category, summing to a real budget. Fold it, put your name on it, hand
it in. Then you measure the same scene properly and we compare. The gap between what you believed
and what the capture says is the entire deliverable. Every experienced performance engineer has a
story about the week they spent optimizing the wrong subsystem; today costs you fifty minutes
instead.

## Why this matters
Frame budget is the currency of this topic: 16.6 ms at 60 fps, 33.3 at 30, 8.3 at 120. Everything
you add is spending against a fixed amount, and average FPS — the number every student quotes — is
the one metric that hides the problem. A game that renders 59 frames in 16 ms and one in 300 ms
reports 62 fps average and feels broken, because what players perceive is the stutter, not the mean.
That is why the industry reads frame *time*, in milliseconds, with percentiles: the 1% and 0.1%
lows are where hitches live. Meanwhile your intuition about which subsystem is expensive is almost
certainly wrong in a specific, predictable way — students overestimate rendering and underestimate
garbage collection, script overhead, and the cost of things that run every frame for no reason.
Your assignment requires you to profile before you optimize. This activity is the demonstration of
why that ordering is not bureaucracy.

## What to do
1. **Choose a scene and a repeatable route (5 min).** Your own project, a provided sample, or any
   game you can currently run and observe with an external tool. Write down a route you can repeat
   exactly: spawn point, path, duration of 60 seconds, same graphics settings, same resolution.
   Write down your target frame rate and convert it to a millisecond budget now.
2. **Predict, sealed (10 min, solo, no tools open).** Split your budget across: gameplay scripts,
   animation, physics, culling and scene traversal, render thread, GPU, audio, garbage collection
   and other. The numbers must sum to your budget. Then predict three more things: average frame
   time, the 1% low frame time, and whether you are CPU-bound or GPU-bound, with one sentence of
   reasoning. Seal it. No edits after this point — a prediction you can revise teaches nothing.
3. **Measure (16 min, pairs, one machine at a time).** Profile a development build if you can rather
   than the editor, and say which you used, because editor overhead will distort every number you
   report.
   - **Unity:** Profiler window, CPU Usage module in Timeline view for where the frame goes, then
     Hierarchy view for the per-call costs; watch the GC Alloc column; check the Rendering module
     for draw calls and batches. Connect to a player build rather than profiling the editor if the
     project allows it.
   - **Unreal:** `stat unit` for the four headline numbers — Frame, Game, Draw, GPU — then
     `stat game`, `stat scenerendering` and `stat gpu` to break them down. If Frame is close to
     GPU you are GPU-bound; if Frame tracks Game you are on the game thread. Unreal Insights gives
     you the same picture with real tracks.
4. **Read the frame time series, not the average (10 min).** Capture the frame-time graph over your
   60-second route and get the numbers out: Unity's Profiler frame chart, or log
   `Time.unscaledDeltaTime` per frame to a CSV; in Unreal use `stat startfile` / `stat stopfile` or
   the CSV profiler. Compute average frame time, the 99th percentile (your 1% low) and your worst
   frame. Then compute what your average FPS says. Put the two side by side.
5. **Open the envelope (9 min, whole room).** Fill in an error column: predicted ms, actual ms,
   absolute error, per category. Name your largest miss and its direction. Around the room, look for
   the pattern — the categories the class systematically over- and under-estimates are the
   categories you should always measure rather than reason about.
6. **Commit to a bound (5 min).** One sentence: "This scene is [CPU/GPU/bandwidth/IO]-bound, and the
   evidence is [the specific two numbers]." That sentence, with those numbers, is the opening of
   your assignment write-up, and being able to produce it in sixty seconds from a fresh capture is
   the thing an interviewer is checking for.

## Deliverables — post to Padlet
- [ ] A photo or screenshot of your sealed prediction, timestamped before your capture
- [ ] The comparison table: category | predicted ms | actual ms | error, plus your route description and whether you profiled editor or build
- [ ] A frame-time graph screenshot with three numbers written on it: average frame time, 99th percentile, worst frame — and the average FPS the same data would report
- [ ] A ~150-word write-up of your biggest miss: what you believed, what was true, and what you would now measure before assuming

## If you finish early
Inject a pathology and watch what survives the average. Add a deliberate per-frame allocation or a
heavy loop somewhere, re-run the same route, and post both frame-time graphs. In most cases average
FPS barely moves while the 1% low collapses — capture the two numbers that prove it, since that
contrast is the most persuasive single slide in a performance report.

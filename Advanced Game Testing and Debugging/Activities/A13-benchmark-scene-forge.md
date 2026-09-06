# Activity 13 — Benchmark Scene Forge

> **Topic 4 · Session 13 · Week 7 (Oct 20)** · In class, ~60 min · Solo build, pairs for the repeatability run-off
> **Feeds:** Performance Profiling Techniques (due Oct 25)

## The setup
"I walked around for a bit and it felt smoother after the fix" is the sentence that gets a
performance claim thrown out. Your assignment requires you to implement one optimization and
measure it, which means you need a harness that can tell a real 12% improvement from run-to-run
noise. Today you build the smallest scene that reproduces your performance problem on demand, wrap
your suspect subsystem in custom instrumentation so it shows up by name in the profiler, and then
run a repeatability run-off: five runs, and the pair with the tightest spread wins. Tight spread is
not a cosmetic virtue. It is the resolution limit of your entire experiment.

## Why this matters
An optimization result is a claim about cause and effect, and a claim is only as good as the
controls behind it. Studios keep dedicated benchmark maps for this reason — a fixed flythrough that
runs identically on every build, so a 0.8 ms regression introduced on Tuesday is visible on
Wednesday instead of at cert. The skill transfers directly: the ability to say "same scene, same
route, same device, five runs, median 14.2 ms before and 11.9 ms after, run-to-run spread 3%" is
what turns your assignment from a story into evidence. It is also the answer to the interview
question about how you know an optimization worked.

## What to do
1. **Name the symptom and the hypothesis (5 min).** One sentence each: "frame time spikes to 40 ms
   when more than about sixty enemies are active" and "I think it is the per-enemy pathfinding
   update, not rendering". Your benchmark scene exists to make that hypothesis falsifiable, so it
   should contain the suspect and as little else as possible.
2. **Strip to the minimum, and fix everything else (14 min).** Build the scene from your own
   project, a provided sample, or from scratch. Eight conditions must be nailed down and written on
   a recipe card, because a benchmark you cannot describe is a benchmark nobody can repeat: fixed
   resolution and quality preset; vsync off, so you measure work rather than waiting; a scripted
   camera path or a fixed camera rather than you holding the stick; a fixed seed for anything
   random; a fixed timestep and a fixed number of simulated steps; a warm-up period that you discard
   (shader compilation and first-touch allocations belong in nobody's measurement); a fixed run
   duration or frame count; and a stated build type — development build, not editor, if you possibly
   can.
3. **Instrument by name (13 min).** Wrap your suspect subsystem so the profiler shows your label
   rather than a stack of engine frames.
   - **Unity:** a static `ProfilerMarker` created once and used with `using (marker.Auto())` around
     the block — allocate the marker at field level, not per frame, or you are measuring your own
     instrumentation. `Profiler.BeginSample`/`EndSample` works too. Add a second marker on the
     subsystem you believe is innocent, so your capture can exonerate it.
   - **Unreal:** `TRACE_CPUPROFILER_EVENT_SCOPE(MySubsystemTick)` for Insights, or
     `DECLARE_CYCLE_STAT` with `SCOPE_CYCLE_COUNTER` in a stat group you can call up with
     `stat mygroup`. Same rule: instrument the suspect and one control.
   Verify the label actually appears in the capture before you go further. Half of instrumentation
   time is discovering the scope did not compile into the build you profiled.
4. **The repeatability run-off (14 min, pairs, stopwatch).** Run your benchmark five times, cold each
   time if load is part of the measurement. Record median frame time and the median cost of your
   named marker. Compute spread as (max - min) / median, as a percentage. Under 5% is a usable
   harness. If your spread is 20%, you cannot detect a 15% optimization and there is no point
   optimizing until you fix that — hunt the noise: background processes, a random spawn you forgot
   to seed, thermal drift, an unwarmed shader cache, editor overhead, a variable-length route.
   Fix and re-run. Post both spreads, before and after.
5. **Prove your harness can see (10 min).** Inject a regression you control — add 20% more agents,
   raise a loop count, add a redundant pass — and re-run. Your instrumentation should report the
   change in the named marker, and the change should be clearly larger than your spread. If it is
   not, your harness is blind at that magnitude and you should say so out loud. Write down the
   smallest change your harness can reliably detect. That number is the honest error bar on every
   claim you make next week.
6. **Write the recipe card (4 min).** The eight fixed conditions, the run count, the metric you
   report (median and 99th percentile, not average), and the machine. Anyone in the room should be
   able to reproduce your number from that card alone.

## Deliverables — post to Padlet
- [ ] Your recipe card: the eight fixed conditions, run count, metric reported, and machine
- [ ] The instrumentation snippet showing your named marker, plus a profiler screenshot where the label is visible
- [ ] The five-run table with median, max, min and spread percentage — before and after you tightened determinism
- [ ] The injected-regression result and one line stating the smallest change your harness can reliably detect

## If you finish early
Automate the run. Wrap it in a command-line launch with a fixed duration that writes the frame times
to a CSV on exit — Unity `-batchmode` with a timed quit, or an Unreal `-ExecCmds` line with
`csvprofile start` and an automatic exit. A benchmark that runs from a command line can run on a
schedule, and a benchmark that runs on a schedule catches the performance regression the week it
lands rather than the month you notice.

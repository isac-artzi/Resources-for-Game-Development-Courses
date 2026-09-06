# Activity 04 — Blind Repro Relay

> **Topic 2 · Session 4 · Week 2 (Sep 17)** · In class, ~50 min · Pairs, stopwatch, silence enforced
> **Feeds:** Benchmark – Bug Reporting and Workflow (due Sep 27)

## The setup
Your steps go to a contractor in another time zone who has never opened this project, cannot ask
you anything, and has one attempt before the ticket comes back marked Cannot Reproduce. Today
your classmate plays that contractor. You will hand over written repro steps and then you will
sit on your hands. No hints, no pointing at the screen, no "oh you have to be sprinting." If they
cannot reproduce it from what you wrote, the bug does not exist yet.

## Why this matters
Cannot Reproduce is not a verdict on the bug; it is a process failure, and most of the time the
failure is in the steps. An unreproducible ticket burns an engineer's afternoon, comes back to
QA, gets re-tested, and often closes with the defect still in the build — which is how bugs reach
players through a team that technically found them. Reproducibility is also the difference
between a bug report and a complaint, and it is the skill your benchmark is actually assessing.
The steps you write today are the ones you will submit; the difference is that these have been
tested on a hostile stranger.

## What to do
1. **Choose a bug you can reproduce on demand (5 min).** Use your own project, a provided sample,
   or any game you can currently run. It must fire at least three times in five attempts for you
   personally — verify that now, with a tally, before you write anything. If it does not, pick
   another bug or narrow the conditions until it does.
2. **Write the steps cold (12 min).** Second-person imperative, one action per step, no
   compound steps, no assumed knowledge. Include the preconditions a stranger cannot infer: build
   or version, platform, settings changed from default, save file or starting state, controller or
   keyboard, window mode. Add Expected result and Actual result. Then add a stop condition — how
   the reader knows they have seen the bug rather than something adjacent. Hand the sheet over.
   From this moment you may not speak to your partner about it.
3. **Run the relay (10 min, timed).** Your partner works from the sheet only, on their own or
   your machine, and calls out one of three outcomes: reproduced, reproduced but not what the
   steps described, or failed. Record the wall-clock seconds to first reproduction and the exact
   step number where they hesitated, went the wrong way, or asked a question they were not allowed
   to ask. Hesitation is data. Write the step number down even if they recover.
4. **Swap and repeat (10 min).** Same rules, roles reversed. Be a hostile reader: do exactly what
   is written, not what you assume was meant. If a step says "open the menu" and there are three
   menus, open the wrong one.
5. **Revise against the failure (8 min).** Rewrite only the steps that caused hesitation or
   failure. Most repairs are one of four kinds: a missing precondition, a compound step split in
   two, an ambiguous noun made specific, or a timing detail nobody wrote down ("during the
   autosave icon, not after"). Label each repair with its kind.
6. **Re-run the repaired sheet (5 min).** New reader if one is free, same reader if not. Record
   the new time to first reproduction. The delta between the two runs is the value of the rewrite,
   and it is the most persuasive thing you can put in your benchmark video.

## Deliverables — post to Padlet
- [ ] Your final repro steps, second-person imperative, with preconditions, Expected, Actual, and stop condition
- [ ] The relay log: outcome, seconds to first reproduction, and the step number where your reader stalled
- [ ] A short table of your repairs: step number | what went wrong | repair kind (precondition / compound step / ambiguity / timing)
- [ ] Before-and-after times, with one sentence on what the difference cost or saved

## If you finish early
Deliberately break your own steps: remove the single precondition you think matters least and
hand it to a third reader. Post whether they still reproduced it. Preconditions that turn out not
to matter are noise in a ticket, and knowing which of yours are load-bearing does more for a reader than
adding five more lines.

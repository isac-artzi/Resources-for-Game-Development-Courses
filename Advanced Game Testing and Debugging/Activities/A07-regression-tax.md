# Activity 07 — The Regression Tax

> **Topic 3 · Session 7 · Week 4 (Sep 29)** · In class, ~55 min · Solo run against a stopwatch, then whole-room sort
> **Feeds:** Benchmark – Unit Testing and Defect Trigger Coverage (due Oct 11)

## The setup
A build lands. Before anyone plays anything new, somebody has to confirm the game still launches,
still saves, still loads that save, and still lets you quit without a hang. That list is a smoke
checklist, and on a real team it gets run every single time code changes. Today you build one for
a game you can actually run, execute it by hand with a stopwatch running, and then do the
arithmetic that every automation argument in your career will rest on. Nobody automates because
automation is modern. They automate because they priced the alternative.

## Why this matters
"Should we automate this?" is a budget question, and you will lose that argument if your answer is
a preference instead of a number. The number has two halves: the cost of running the check by hand
times how often it must run, and the *detection latency* — how many hours a regression sits in the
build before a human happens to notice. A four-minute manual smoke pass sounds free until it runs
against eight pushes a day for a twelve-week project, at which point you have spent about
forty-eight hours of a person's life re-launching a main menu. The second half of today is the
counterweight: the classic test pyramid assumes most of your value is in cheap unit tests, and
games distort that assumption badly, because frames, physics, input timing and "does this feel
right" refuse to fit in a unit test. Knowing which of your checks genuinely belongs at which layer
is what keeps your benchmark suite from being either useless or unfinishable.

## What to do
1. **Pick a target and write the checklist (10 min).** Use your own project, a provided sample, or
   any game you can currently launch. Write ten to fourteen checks that must pass on every build.
   Each one needs a binary oracle — a stranger reading it must be able to say pass or fail without
   asking you. "Settings feel responsive" is not a check. "Set resolution to 1280x720, quit,
   relaunch: the game opens at 1280x720" is a check. Cover at minimum: cold launch, new game, save,
   load that save, a settings change that must persist, pause and resume, audio mute, and clean
   quit.
2. **Run it by hand, on the clock (12 min).** Time each individual check in seconds, not the batch.
   Mark pass, fail, or unsure. Star every check where you had to stop and decide what "correct"
   even meant — those stars are specification gaps, and they will bite you again when you write the
   assertion.
3. **Price the tax (7 min).** Total your seconds. Then multiply: total minutes x pushes per day
   (use 8 if you have no real number) x working days in a twelve-week project (60). Convert to
   hours. Write a second number beside it: if this checklist only gets run when someone remembers,
   how many hours can a regression live in the build before discovery? Those two numbers are your
   automation argument, and they are the first slide of any pitch you ever make for test time.
4. **Sort onto the pyramid (13 min, whole room).** Four columns on the board: unit, integration,
   smoke/soak, human playtest. Put every check from your list into exactly one column, and be ready
   to defend it. Use these questions to decide: does the check need a rendered frame? does it need
   more than one system talking? does it need real elapsed time? does it need a human judgment that
   cannot be written as an assertion? A check that only needs data in and data out is a unit test
   and should be, because it will run in under a millisecond and can run on every save.
5. **Argue the middle (8 min).** Look at the room's columns. In most game projects the unit layer
   is thinner than the textbook pyramid predicts and the integration and smoke layers are fatter,
   because so much game logic is entangled with engine lifecycle. Pick the two most contested
   checks and settle them out loud. The useful outcome is not agreement — it is hearing the reason
   a classmate would draw the line somewhere else.
6. **Name your first automation (5 min).** Choose the single check with the best ratio of manual
   cost to automation difficulty. Write the assertion you would make, in one line of pseudocode:
   what you call, with what input, and the exact expected value. That line is the seed of the first
   test you write on Thursday and it belongs in your benchmark.

## Deliverables — post to Padlet
- [ ] Your timed smoke checklist as a table: check | seconds | pass/fail/unsure | starred if the correct behavior was ambiguous
- [ ] The regression tax arithmetic — total minutes, pushes per day, project days, resulting hours — plus your detection-latency estimate in hours
- [ ] A photo or screenshot of your checks sorted into the four pyramid layers, with the one placement you would defend against challenge
- [ ] The one-line assertion for the first check you intend to automate, naming the input and the exact expected value

## If you finish early
Run your checklist a second time and compare the per-check times. You will be faster, and some
checks will drift in what you actually did. Post the two time columns and the drift you noticed —
a human running the same procedure twice does not run the same procedure twice, which is the
quieter half of the automation argument.

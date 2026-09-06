# Activity 09 — The Trigger Gauntlet

> **Topic 3 · Session 9 · Week 5 (Oct 6)** · In class, ~60 min · Teams of three rotating through six stations, seven minutes each
> **Feeds:** Benchmark – Unit Testing and Defect Trigger Coverage (due Oct 11)

## The setup
Six stations around the room, one per defect trigger: Configuration, Startup, Exception, Stress,
Normal, Restart. Your team brings one feature and must break it a different way at every station.
The rule that makes this hard is that the station dictates the *kind* of attack — you cannot bring
your favorite bug six times. At each station you leave behind a named test scenario and the
assertion it would make. Six stations, six scenarios, and by the end of the hour you have the
skeleton of the exact suite your benchmark asks for, built while five other teams were watching
you fail to think of a Restart case.

## Why this matters
Most student test suites cover Normal six times. That is the trigger where the code was written, so
it is the trigger where the code works. The six-trigger model exists because shipped defects
cluster everywhere else: a game that is flawless in play but loses your keybinds on relaunch
(Configuration), crashes when the save file from last week's build is loaded (Startup), throws on
a null quest reference nobody could produce by playing normally (Exception), degrades after forty
minutes of soak (Stress), or forgets that a coroutine was mid-flight when the scene reloaded
(Restart). Each trigger is a different question about your feature, and a suite that only asks one
question gives you the false confidence that is worse than no tests at all. Your benchmark requires
all six for exactly this reason, and the trigger everyone finds hardest today is the trigger their
project is most likely to be broken by.

## What to do
1. **Bring one feature and post its contract (5 min).** Your own project, a provided sample, or a
   ten-line feature you can write here: inventory with a capacity, save/load of a settings object,
   a quest state machine, a currency wallet, a cooldown, an audio bus. Write its contract in under
   six lines — inputs, valid ranges, states, what it persists. Every station attacks this contract.
2. **Run the gauntlet (42 min, seven minutes per station).** At each station, produce a written
   scenario and the assertion, then write the test or at minimum the named, failing stub. Move when
   the timer goes, finished or not; an unfinished station is information about your feature.
   - **Configuration** — change the environment, not the input. Non-default quality preset, a
     different locale or decimal separator, ultrawide or 4:3, keyboard vs gamepad, a settings file
     from an older build, missing optional config. Assertion example: parsing a saved float still
     works under a locale that uses a comma.
   - **Startup** — first run and cold init. No save file at all, a corrupt or truncated one, a save
     written by version 0.3 loaded by 0.4, initialization order where a manager is asked for data
     before it is ready. Assertion example: a fresh profile loads with defaults and no exception.
   - **Exception** — feed it what should never happen. Null reference, empty collection, negative
     quantity, an ID that does not exist, a division by zero, a timed-out request. Assert the
     specific exception type or the specific graceful fallback — not just "it does not crash".
   - **Stress** — quantity and duration. Ten thousand items, two hundred simultaneous purchases,
     inputs spammed faster than a human can, the same operation repeated for a simulated hour.
     Assert a bound: no unbounded growth, result still correct at the extreme, completes under a
     stated time.
   - **Normal** — the happy path, written honestly. The typical value a real player produces, the
     documented behavior, the one everyone assumes works. Assert exact values, not existence.
     If this one takes you seven minutes you did not have a specification.
   - **Restart** — stop and resume. Quit and relaunch mid-transaction, reload the scene, enter and
     exit play mode, suspend and resume, reset the state machine while a timer is running. Assert
     that state survives what should survive and resets what should reset.
3. **Fill the board (8 min, whole room).** Post all six scenarios. Look across teams: which trigger
   produced the thinnest scenarios room-wide, and which produced a real defect somebody did not
   know about. Name out loud the one trigger your feature is least defended against.
4. **Wire the harness (5 min).** Give your six tests a shared setup and teardown so they can run in
   any order, and add one log line per test that prints the trigger name and the input used. When
   a CI run goes red at 2 a.m. and you read the log two days later, that line is the difference
   between a diagnosis and a shrug. You will need both the harness and the logging for the
   benchmark, so build them here where you can ask for help.

## Deliverables — post to Padlet
- [ ] The six-row gauntlet table: trigger | scenario in one sentence | exact assertion | status (passing, failing, stub)
- [ ] A screenshot of your test runner showing six tests named so the trigger is readable in the list
- [ ] Your setup/teardown snippet plus one example log line as it actually prints
- [ ] A ~150-word write-up on the trigger you found hardest to design for, and what that tells you about your feature

## If you finish early
Take the trigger your feature failed at and turn it into a defect report in the tracker you chose
in Activity 05 — PAL title, imperative steps, expected and actual, the failing test named as
evidence. A failing test attached to a ticket is the strongest bug report format that exists,
because the fix has a built-in verification step and nobody can close it on a hunch.

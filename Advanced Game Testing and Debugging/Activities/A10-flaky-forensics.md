# Activity 10 — Flaky Test Forensics and the First Red Build

> **Topic 3 · Session 10 · Week 5 (Oct 8)** · In class, ~60 min · Pairs, then a solo pipeline push
> **Feeds:** Benchmark – Unit Testing and Defect Trigger Coverage (due Oct 11)

## The setup
A test fails one run in eight. It passed on your machine four times, so you re-ran the pipeline and
it went green, and everybody moved on. That is the moment a team starts training itself to ignore
red. Today you manufacture a flake on purpose, hand it to a classmate as a crime scene, and then
put a real pipeline behind a real repository so that a push you make in this room turns a build red
and tells you about it. Nobody is building an engine in CI today. A free GitHub account and a tiny
repository with your pure test project in it is enough, and it is what your benchmark needs.

## Why this matters
Flakiness is not a nuisance category, it is a defect class, and it usually indicates something real:
a hidden dependency on wall-clock time, on test execution order, on an unseeded RNG, on frame
timing, on shared static state, or on a resource that is not there every time. The compounding
arithmetic is what should scare you. Give each test a 2% chance of failing spuriously and put 200
tests in a suite: the chance that a completely clean run comes back red is 1 - 0.98^200, which is
about 98%. A suite that is red almost every time is a suite nobody reads, which means the one
genuine regression in there ships. The professional response is not deletion — deleting a flaky
test deletes the coverage and hides the nondeterminism. It is quarantine with an owner and a date.

## What to do
1. **Manufacture a flake (10 min, pairs).** Take a card with an assigned nondeterminism source —
   wall clock, test order, unseeded RNG, frame timing, shared static state, external file or network
   resource — and write one test that fails somewhere between one run in four and one run in ten.
   Then run it twenty times and record the observed failure count. Unity: use the test runner's
   repeat, or a `[Repeat(20)]`-style loop or a short script. Unreal: run the Automation test twenty
   times from the console or a batch command. Plain C#/C++: a loop in the harness is fine. Write
   down the true source on a slip of paper and keep it face down.
2. **Swap crime scenes and hunt (12 min, stopwatch).** Trade tests with another pair. You get the
   code and the failure rate, not the answer. Find the nondeterminism, name the source, and fix it
   properly — inject and freeze the clock, seed the RNG explicitly and log the seed, isolate the
   shared state in teardown, remove the order dependency, stop touching the file system. Record the
   time to diagnosis. Then re-run twenty times and show zero failures. Compare against the slip.
3. **Stand up a pipeline (18 min, solo).** Free GitHub account, new repository, push the smallest
   thing that can hold your tests: your plain C# library plus its test project, or a small C++ or
   dotnet project. Do not put the engine in CI today. Add a workflow under
   `.github/workflows/tests.yml` that runs on push and on pull request, restores, builds, and runs
   the test command, and uploads the test results file as an artifact. Confirm it goes green.
4. **Break it on purpose (7 min).** Push a commit that makes one assertion fail — change an
   expected value — and let the pipeline go red. Screenshot the failed run, the failing test name in
   the log, and the notification you received. Check your GitHub notification settings so a failed
   workflow on your own repository actually reaches you by email; that is the "automated failure
   alert" your benchmark asks for, and it costs one checkbox. Then push the fix and screenshot green.
   The red-to-green pair of screenshots is worth more in your video than any amount of narration.
5. **Write the quarantine policy (8 min).** Three lines, in your repository's README: how a flaky
   test gets tagged or moved to a quarantined suite, who owns it, and by what date it is either
   fixed or the underlying nondeterminism is filed as a defect. Add your own crying-wolf number —
   your suite size and your honest per-test flake rate, run through 1 - (1 - p)^n. If the number
   embarrasses you, that is the point of computing it.
6. **Check the feedback latency (5 min).** Note how long your pipeline took from push to result. If
   it exceeds about ten minutes, developers stop waiting for it and start pushing over it. Write one
   sentence on what you would move to a later stage to keep the fast feedback fast.

## Deliverables — post to Padlet
- [ ] Your flake table: assigned source | failures out of 20 before | the fix you applied | failures out of 20 after
- [ ] Time to diagnosis on the test you inherited, and the source you named versus the source on the slip
- [ ] Your workflow YAML snippet plus two screenshots — the red run with the failing test name, and the green run after the fix
- [ ] Your three-line quarantine policy and your crying-wolf arithmetic, with your real suite size and flake rate

## If you finish early
Add a scheduled run of the same suite — nightly on a cron trigger — and explain in one sentence why
a suite that only runs on push will still miss a class of failures. Time-dependent and
environment-dependent defects surface when nobody is pushing, which is exactly when nobody is
looking.

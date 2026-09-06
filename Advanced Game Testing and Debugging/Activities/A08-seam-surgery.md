# Activity 08 — Seam Surgery, Then Break My Test

> **Topic 3 · Session 8 · Week 4 (Oct 1)** · In class, ~60 min · Pair programming, driver and navigator swap every eight minutes
> **Feeds:** Benchmark – Unit Testing and Defect Trigger Coverage (due Oct 11)

## The setup
You have a feature that cannot be unit tested. Almost everybody does. It reads the clock directly,
it rolls its own random numbers, it asks a singleton for the player, it polls input inside an
update loop, and it lives in a class the engine constructs for you. None of that is bad code; it is
ordinary game code. It is also untestable code, and by the end of this hour it will not be. Then
the pair next to you is going to try to write a deliberately wrong implementation that still passes
the test you just wrote. If they succeed, your assertion was decoration.

## Why this matters
Testability is not a property of your discipline, it is a property of your dependencies. Every
place where your code reaches out and grabs something global — the wall clock, the RNG, the input
device, a scene object, a static manager — is a place a test cannot control, and anything a test
cannot control it cannot assert about. The fix has a name older than either engine: inject the
dependency. Pass in an `IClock`, an `IRandom`, an `IInputSource`, and now your damage calculation
runs in a millisecond with no engine at all, on a seeded RNG, at exactly the timestamp you chose.
The second half matters just as much: a test that passes is not evidence of anything until you know
it can fail. `Assert.IsNotNull(result)` passes for a function that returns garbage. Weak assertions
are how teams end up with four hundred green tests and a broken build.

## What to do
1. **Pick the victim and write the assertion first (6 min).** Use your own project, a provided
   sample, or any small feature you can write from scratch in ten lines: damage with a crit roll,
   stamina drain over time, a cooldown, loot rarity selection, a shop purchase, a save-version
   check. Before you touch the code, write the sentence you want to be able to assert — "a crit on
   a 10-damage hit against 2 armor deals 18" — with real numbers in it. If you cannot write that
   sentence, you do not understand the feature well enough to test it.
2. **Diagnose the seams (8 min).** Read the code together and list every hidden dependency. The
   usual four: the clock (`Time.deltaTime`, `FApp::GetDeltaTime`, `DateTime.Now`), randomness
   (`Random.Range`, `FMath::RandRange`), input (`Input.GetKey`, direct controller polling), and
   global or scene state (singletons, `GetComponent`, `GetWorld()`, static caches). Mark each with
   what a test would need to control it. That list is your surgical plan.
3. **Operate (18 min, swap driver every 8 min).** Extract the decision from the engine. The target
   shape is a plain class or function that takes its inputs and returns its output — no engine
   types in the signature if you can avoid it. Where you genuinely need time or randomness, take an
   interface in the constructor and pass a fake in the test. Keep the engine class as a thin caller
   that gathers real inputs and hands them to the pure logic.
   - **Unity:** put the logic in a plain C# class under a runtime assembly definition, write an
     EditMode test with `[Test]` and Arrange-Act-Assert, name it
     `Method_Condition_ExpectedResult`, and inject a `FakeClock : IClock` and a seeded `IRandom`.
     Reach for `[UnityTest]` with `yield return null` only if you genuinely need a frame to pass —
     most of you will not.
   - **Unreal:** put the logic in a plain C++ class or a static `UFUNCTION`, then write an
     Automation Spec with `DEFINE_SPEC` / `Describe` / `It`, or a simple test with
     `IMPLEMENT_SIMPLE_AUTOMATION_TEST` and `TestEqual`. Inject a fake clock and a seeded stream
     (`FRandomStream`) rather than calling the global RNG.
4. **Get to green, then hand it over (6 min).** The test must pass, and you must have seen it fail
   at least once — change the expected value, watch it go red, change it back. A test you have never
   seen fail is an untested test. Then pass your test file to the neighboring pair. Keep your
   implementation.
5. **Break my test (12 min).** You now hold someone else's test and none of their implementation.
   Write the most obviously wrong implementation you can that still makes their test pass: return
   the constant they asserted, ignore an argument entirely, hard-code the crit branch, ignore the
   clock. Time-box it and be ruthless. Hand back both the cheat and a one-line diagnosis of the
   weakness — no boundary case, only one input tested, an assertion on the wrong property, a
   null-check standing in for a value check.
6. **Strengthen and re-run (8 min).** Repair your assertions so the cheat fails. The usual repairs
   are: assert the exact value not its existence, add a second case that forces the branch, add a
   boundary at zero or the maximum, and assert that a dependency was actually used (advance the
   fake clock and show the output changes). Re-run. Both tests green, cheat red.

## Deliverables — post to Padlet
- [ ] A before-and-after snippet of the seam: the original line that reached for a global, and the injected version
- [ ] Your passing test, with its name visible, plus a screenshot of the test runner showing it green
- [ ] The cheat implementation that passed a classmate's test, and the one-line diagnosis of the weak assertion
- [ ] The strengthened assertion that kills the cheat, with a sentence naming which repair kind you used

## If you finish early
Add setup and teardown to your fixture and prove they matter: introduce a static field that leaks
state between tests, run the suite in a different order, and capture the failure. Then fix it with
teardown and post both runs. Test isolation is invisible until it is a Tuesday-afternoon mystery,
and the six-trigger suite you build on Monday will have far more shared state than this one does.

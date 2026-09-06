# Activity 20 — Thirty-Minute Leak Hunt

> **Topic 6 · Session 20 · Week 10 (Nov 12)** · In class, ~60 min · Pairs plant, pairs hunt, thirty minutes on the clock
> **Feeds:** Memory Optimization (due Nov 22)

## The setup
You have twenty minutes to plant a retention bug in a partner pair's project, and they have twenty
minutes to plant one in yours. Then the clock starts and you get thirty minutes to find theirs. The
planter writes the truth on a folded card and does not speak. The only way to win this reliably is
the technique the topic is built around: snapshot, act, snapshot, diff — and then follow the
reference chain until you find the thing that is still holding on to memory that should be gone.
Guessing does not work. Reading the code does not work either, because the planter chose a
plausible-looking line.

## Why this matters
In a garbage-collected engine, a "memory leak" is almost never a leak in the C sense. Nothing was
forgotten by the allocator; something is still referenced, and the collector is doing exactly its
job by keeping it alive. That means the diagnostic question is not "where did the memory go" but
"who still points at it", and the answer is nearly always one of five suspects: a static collection
that only ever grows, an event or delegate subscribed and never unsubscribed, a closure that
captured a reference to the whole object, an object marked to survive scene loads that was never
cleaned up, or a cached reference in a manager that outlives the thing it caches. In C++ and Unreal
you add real ones: `new` without `delete`, a `UPROPERTY` still pointing at an actor you meant to
destroy, a `TSharedPtr` cycle where two objects keep each other alive forever. The reason to
practice under a stopwatch is that in production you find these at the worst possible time — a soak
test failing at hour four, three days before cert — and the person who can drive a snapshot diff
calmly is the person who fixes it. Your assignment requires two documented memory issues with
before-and-after measurement; the one you find today is the strongest candidate for the first.

## What to do
1. **Learn the tools cold, before anything is hidden from you (10 min).** Take two snapshots of a
   clean run with an obvious action between them so you can see what a diff looks like when you
   already know the answer.
   - **Unity:** Memory Profiler package. Capture a snapshot, perform the action, capture a second,
     switch to Compare mode, and sort by size delta. Then use the references panel on a grown object
     to walk the chain back to whatever roots it. Profile a development build over the editor
     whenever you can.
   - **Unreal:** `memreport -full` before and after, and diff the category tables. `obj list class=YourClass`
     to count instances, `obj refs name=ObjectName` to see who holds a reference. Force a collection
     with `gc.CollectGarbage` before the second capture so you are not looking at objects that are
     merely pending. Enable the Low Level Memory Tracker for per-subsystem attribution.
2. **Plant one (20 min, pairs, silent).** Into your partner pair's project — or the provided sample,
   or a small scene either of you can run. It has to be plausible, it has to grow with a repeatable
   player action, and it must not crash immediately. Choose from: a static or singleton list that
   accumulates spawned objects; an event subscription in `OnEnable` with no matching unsubscribe;
   a lambda registered as a callback that captures `this`; an object kept alive across scene loads
   and re-created every time the scene reloads; a manager caching every enemy it has ever seen; in
   C++/Unreal, a `UPROPERTY` array that is appended to and never emptied, or two objects holding
   shared pointers to each other. Write on the folded card: the exact file and line, the action that
   grows it, and the reference chain that keeps it alive. Hand the card to the instructor, not the
   hunters.
3. **Hunt (30 min, on the clock).** Work the method, not your instincts.
   - Find a repeatable action that you suspect grows memory — enter and exit a room, spawn and kill
     ten enemies, open and close the menu twenty times. Repeat it enough times that a small leak
     becomes a large one; ten iterations turns a 200 KB leak into something impossible to miss.
   - Snapshot, run the action loop, force a collection, snapshot again, diff.
   - Read the diff by object count first and size second. Fifty extra instances of one class is a
     louder signal than a few megabytes spread across everything.
   - Take the grown type and walk its references back to a root. Name the chain out loud:
     "the enemy is held by a list, the list is a field on a static instance, so nothing can ever
     collect it".
   - Stop when you can state the chain. The line of code is trivially findable once you have it, and
     stating the chain is what separates a diagnosis from a lucky grep.
4. **Reveal and fix (10 min).** Open the card. Compare the planted truth against your reconstruction
   — you may have found a real, unplanted leak instead, which happens more often than teams expect
   and counts as a better result. Write the fix and prove it with a third snapshot after the same
   action loop: object count returns to baseline, or memory flattens. A fix with no post-fix snapshot
   is a hope.
5. **Write the prevention rule (5 min).** One rule, phrased so a teammate could follow it without
   understanding the bug: "every `+=` on an event in `OnEnable` has a matching `-=` in `OnDisable`,
   and the pull request template asks about it". Prevention rules are the part of the assignment
   that most submissions handwave; you now have one you earned.

## Deliverables — post to Padlet
- [ ] Your snapshot diff screenshot with the grown type and its delta visible
- [ ] The reference chain written as a chain, in one line, from the leaked object to its root
- [ ] The fix as a short before/after snippet, plus the post-fix snapshot showing counts returning to baseline
- [ ] A ~150-word write-up: time to find, what led you wrong first, and the one prevention rule you would put in your team's code review checklist

## If you finish early
Turn the hunt into a test. Write an automated check that runs your action loop N times, forces a
collection, and asserts that the instance count of the suspect type returned to its starting value —
a PlayMode test in Unity, an Automation or Functional Test in Unreal. A retention bug that has a
failing test can never quietly come back, and a soak test that watches a counter is how studios
catch the leaks that only show up at hour four.

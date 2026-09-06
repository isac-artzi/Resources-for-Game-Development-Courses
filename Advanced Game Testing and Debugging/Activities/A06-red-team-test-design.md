# Activity 06 — Red Team, Blue Team: Break the Parameter

> **Topic 2 · Session 6 · Week 3 (Sep 24)** · In class, ~60 min · Two teams, then individual work time
> **Feeds:** Benchmark – Bug Reporting and Workflow (due Sep 27)

## The setup
Blue team owns a feature and claims it is tested. Red team gets the same specification and
twenty-five minutes to produce a case that breaks it — using technique, not luck. The rule that
makes this hard: red team may not fire randomly. Every attempted break must come from a named
technique, written down before it is run. Random button-mashing finds bugs; it does not find them
again next sprint, and it cannot be handed to somebody else. After the duel, you spend the rest
of the session turning today's best cases into the third report your benchmark needs.

## Why this matters
Coppola et al. make the case that line coverage tells you almost nothing about a game, because
the interesting failures live in state, space, and interaction rather than in unexecuted lines.
Test design techniques are how you get coverage of the things that actually matter without
testing infinity: equivalence partitions collapse thousands of inputs into a handful of classes,
boundary values target the off-by-one that lives at every edge, decision tables catch the
combination nobody considered, state-transition diagrams find the illegal move between screens,
and pairwise reduction turns an unrunnable matrix into a morning of work. These are the
techniques that let you say "we tested the stamina system" and mean something specific.

## What to do
1. **Pick a feature with numbers in it (5 min).** From your own project, a provided sample, or
   any game you can run: stamina, ammo, a shop with currency, an inventory with a capacity limit,
   a save-slot list, a difficulty selector, a matchmaking queue. It needs at least one numeric
   range and at least two states. Blue team writes the specification as they believe it works —
   ranges, limits, allowed transitions — in under ten lines. That written specification is the
   contract for the duel.
2. **Red team designs, does not play (15 min).** Produce, on paper first: equivalence partitions
   for each numeric input (valid low, valid typical, valid high, invalid below, invalid above);
   boundary cases at each edge (minimum minus one, minimum, minimum plus one, and the same at the
   maximum); a decision table for any behavior that depends on two or more conditions at once
   (has currency by has inventory space, for instance) with every combination enumerated; and a
   state-transition table listing the moves the specification does not mention — pausing during a
   transaction, backing out mid-save, opening the menu while stamina is draining. Choose the eight
   cases most likely to break the contract.
3. **Fire (10 min).** Run the eight, in order, with the specification visible. Record for each:
   technique used, input, expected by specification, actual. A case that passes is still evidence;
   it is a documented claim about behavior, which is more than blue team had at the start.
4. **Blue team responds (8 min).** For each break, blue team gives exactly one of three answers:
   it is a defect, the specification was wrong, or the specification was ambiguous. The third
   answer is the interesting one — spec ambiguity is a root-cause category, and a bug filed
   against an ambiguous specification needs a designer, not an engineer. Log which answer each
   break received.
5. **Swap and rerun a short round (7 min).** Teams trade features. Red team's design work is now
   blue team's inheritance: can they run someone else's test table without explanation? If not,
   the table was underspecified — fix it, since the same is true of everything you hand a
   contractor.
6. **Benchmark work time (15 min).** Take the most damaging confirmed break from either side and
   write it up completely: PAL title, second-person imperative steps, Expected, Actual, evidence
   attached, entered in the tracker you chose in Activity 05, with severity and priority as
   separate fields and the ticket moved past New into a real state. That is one of your three
   benchmark reports, done properly, in class, while people who can argue with you are still in
   the room. Ask two of them to argue with it.

## Deliverables — post to Padlet
- [ ] Your one-page test design: partitions, boundary table, decision table, and the state moves the specification never mentioned
- [ ] The results table for the eight cases: technique | input | expected | actual | blue team's answer (defect / spec wrong / spec ambiguous)
- [ ] A screenshot of the finished ticket in your tracker, showing title, steps, attachment, and current state
- [ ] A ~150-word write-up of the case you are proudest of, naming the technique that produced it and why random play would probably have missed it

## If you finish early
Count the full combination matrix for your feature — every value class of every parameter
multiplied out — then build a pairwise-covering set that hits every pair of values at least once,
and compare the two counts. Post both numbers and the covering table. That arithmetic is the
argument you will use in Topic 5 when someone tells you the test matrix is too large to run.

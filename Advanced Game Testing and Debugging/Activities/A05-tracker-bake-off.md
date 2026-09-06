# Activity 05 — Tracker Bake-Off and the Life Cycle Drive

> **Topic 2 · Session 5 · Week 3 (Sep 22)** · In class, ~55 min · Trios, one tool each
> **Feeds:** Benchmark – Bug Reporting and Workflow (due Sep 27)

## The setup
Your benchmark requires a real ticket in a real tracker with real state transitions, and you have
to choose the tool this week. Jira, Trello, and Bugzilla each make one thing easy and one thing
painful, and the sales pages will not tell you which. So run a bake-off: three people, three
tools, the same bug, the same clock. Then drive that ticket all the way around the life cycle
including the ugly parts — the reopen, the duplicate, the cannot-reproduce — because those are
the transitions that teach you what a workflow is actually for.

## Why this matters
A tracker is a shared state machine for a team that is never all awake at once. Every transition
obligates a specific person to do a specific thing: moving a ticket to Fixed obligates the
engineer to name a commit, moving it to In Verification obligates QA to re-test on a build that
contains that commit, and closing it obligates somebody to say which build the fix shipped in.
Teams that treat status as decoration lose track of what is actually in the build, which is the
condition Neville-Neil describes as an ailing project. You will be handed an existing board on
your first job. Knowing what a well-configured one feels like is how you avoid inheriting a mess
and calling it normal.

## What to do
1. **Split the tools (2 min).** One person on Jira, one on Trello, one on Bugzilla — free tiers
   or a provided instance. Nobody takes the tool they already know.
2. **Same bug, three tools, twelve minutes on the clock.** All three of you file the *same* bug
   from Activity 04, with the same PAL title, the same imperative steps, Expected, Actual, and at
   least one real attachment (clip, log, or save). Start the timer when you open the new-issue
   form and stop it when the ticket is complete enough that a stranger could act on it. Record
   the elapsed time and, honestly, the moment you got stuck.
3. **Compare the fit (8 min).** Answer four questions as a trio, with evidence from what you just
   did, not from reputation: Which tool made severity and priority separate fields without
   configuration? Which one let you attach evidence fastest? Which one would let a triage lead
   find your ticket among two thousand? Which one made you type something the team does not need?
   Name a winner per question. Different tools should win different questions; if one tool sweeps,
   you are ranking familiarity, not fit.
4. **Drive the life cycle (20 min).** In the winning tool, configure the states you need and walk
   one ticket through the full path: New, Triaged, Assigned, In Progress, Fixed, In Verification,
   Closed. At every transition write a one-line comment saying who owns the ticket now and what
   they must produce before the next move. Then force three unhappy paths on separate tickets:
   one Reopened after a failed verification, one closed as Duplicate with a link to the original,
   and one closed Cannot Reproduce. For the Cannot Reproduce, write the comment you would want to
   receive — the one that names exactly what was tried, on what build, how many attempts, so the
   original reporter can respond with something better than "works for me."
5. **Name the failure at each edge (8 min).** Pick the three transitions your trio thinks go
   wrong most often on student teams and write the specific failure at that edge in one line each
   (for example: Fixed with no commit reference, so verification cannot know what to test).
6. **Lock your benchmark tool (5 min).** Decide which tool you will each use for the benchmark and
   write one sentence of justification tied to something you measured today.

## Deliverables — post to Padlet
- [ ] Bake-off table: Tool | minutes to a complete ticket | where you got stuck | which of the four questions it won
- [ ] Screenshots of one ticket in at least three different states, with the transition comments visible
- [ ] Your Cannot Reproduce comment, quoted in full
- [ ] Three transition-failure lines, and one sentence naming your benchmark tool and why

## If you finish early
File a near-duplicate of a teammate's ticket on purpose, with a title you think is different
enough. Then search the board as a triage lead would and see whether you can find both. Post the
search term that surfaces one but not the other — that gap is exactly what a bad title costs.

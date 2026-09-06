# Activity 03 — Patch Notes Autopsy

> **Topic 2 · Session 3 · Week 2 (Sep 15)** · In class, ~50 min · Pairs, then cross-pair review
> **Feeds:** Benchmark – Bug Reporting and Workflow (due Sep 27)

## The setup
"Fixed an issue where the player could become permanently stuck after fast-travelling while
mounted." That line is the public tombstone of a bug report somebody wrote months earlier. Behind
it sat a title, a set of repro steps, an expected and an actual result, a build number, and a clip.
Today you exhume it. You will take real patch notes from a shipped game, reverse-engineer the bug
report that must have existed, and write it in PAL format — then hand it to another pair to see
whether they can find it in a tracker of two thousand tickets.

## Why this matters
A bug report title is not a sentence; it is a search key. PAL — Problem, Action, Location — puts
the searchable words first because the person reading it is scanning a list of hundreds and the
person before them may have already filed the same thing. Zhang et al. found duplicate detection
is still an unsolved problem in practice, and every duplicate costs a triage pass and an
engineer's context switch. The habit you build here is the one that makes you findable, and it is
the single most visible signal of professionalism in a tracker. Studios read your old tickets when
they hire you.

## What to do
1. **Choose a corpse (5 min).** Find recent patch notes for any shipped game — Steam news, a
   console patch page, a live-service update blog, or the release notes of an engine or plugin you
   use. Pick three lines that describe bug fixes, not features. Prefer lines that are vague; vague
   lines have more to reconstruct.
2. **Reconstruct the defect (12 min).** For each line, write what the player was most likely
   doing when it fired, what they saw, and what should have happened instead. State your
   confidence honestly: some lines will support one clear reconstruction, some three competing
   ones. Where you have competing reconstructions, keep the one that would be cheapest to
   disprove with a single test.
3. **Write the PAL title (8 min).** Problem, Action, Location, in that order, under about twelve
   words, no articles wasted, no punchlines. "Save corruption when quitting during autosave in
   Hollow Keep" beats "Game ate my save!!!" Write three titles, one per fix. Read them aloud —
   if the first three words could describe forty other bugs, rewrite.
4. **Fill the body (10 min).** For each: Steps to reproduce in second-person imperative, one
   action per numbered step ("Launch the game", "Load the Hollow Keep save", "Press Escape during
   the autosave icon"). Then Expected result and Actual result as separate fields, each one
   sentence. Then the evidence list you would have attached: clip, log excerpt, save file, build
   number, platform, commit SHA. You do not have the real artifacts — name what you would attach
   and why each one answers a question the engineer will otherwise ask you in a comment thread.
5. **Cross-pair search test (10 min).** Swap all three titles with another pair, titles only, no
   bodies. Each pair guesses, from the title alone, the category, the likely severity, and the
   system involved. Then reveal the bodies. Every wrong guess is a defect in your title, not in
   their reading. Rewrite the titles that failed.
6. **Root-cause guess (5 min).** For your strongest reconstruction, name the root-cause category
   you would bet on and the one experiment that would confirm or kill your bet.

## Deliverables — post to Padlet
- [ ] Three PAL titles, with the original patch-note line quoted underneath each
- [ ] One complete report body: steps in second-person imperative, Expected, Actual, attachment list
- [ ] The cross-pair result: which title was misread, what the other pair guessed, and your rewritten title
- [ ] One sentence naming your root-cause bet and the experiment that would settle it

## If you finish early
Search the same game's patch notes for a fix that reappears in a later update. A bug that comes
back is either a bad fix, a missing regression test, or two different bugs wearing the same
symptom. Post which you think it was and what evidence in the notes supports you.

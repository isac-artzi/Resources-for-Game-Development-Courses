# Activity 01 — Bug Taxonomy Safari

> **Topic 1 · Session 1 · Week 1 (Sep 8)** · In class, ~50 min · Solo capture, then whole room
> **Feeds:** Bug Investigation and Prioritization (due Sep 13)

## The setup
You have shipped student projects. You have never been asked to prove one works. Today the room
starts a habit that runs for fifteen weeks: nothing counts as a defect until you can show it. A
studio does not run on "it felt janky in the tutorial level" — it runs on a capture, a build
number, and a name for the failure that everyone in the room uses the same way. You are going on
a safari. Bring back specimens.

## Why this matters
In your first QA standup someone will say "the shop is broken." That sentence starts an argument,
not a fix. An engineer will ask what "broken" means, a designer will say it is intended, and the
producer will move on to the next agenda item while the defect stays in the build. The tester who
can say "progression-blocking, S1, reproduces 5 of 5 times on build 0.4.2, twelve-second clip
attached" gets the fix. The tester with a feeling gets ignored. Naming a defect precisely is the
entry skill of the entire discipline, and it is the thing every QA interview probes in the first
ten minutes.

## What to do
1. **Pick a hunting ground (3 min).** Use your own project, a provided sample project, or any
   game you can currently launch on the machine in front of you — Steam, itch.io, mobile, a
   browser game, an old console build. There is no wrong choice. Note the exact build or version
   string now; you will need it later and you will not remember it.
2. **Hunt for 20 minutes, on the clock.** Play adversarially, not well. Walk into geometry.
   Spam inputs during transitions. Alt-tab mid-cutscene. Buy something with an empty wallet.
   Resize the window. Disconnect the controller mid-jump. Every time something behaves in a way
   a player would not expect, capture it: screenshot, or a clip of ten seconds or less. Aim for
   six candidates. You will keep four.
3. **Name each specimen (10 min).** For each capture write one line: what you did, what happened,
   what you expected instead. Do not editorialize. "Character clips through the market stall and
   falls out of the level" is a specimen. "Collision is bad" is an opinion.
4. **Classify (7 min).** Assign every specimen exactly one primary category from this list, and
   be prepared to defend the choice: functional, graphical/rendering, physics, AI, audio, network,
   progression-blocking, economy, localization, platform-certification, performance. If two
   categories genuinely fit, write both and note which one a player would feel first. That tension
   is the interesting part — bring it to the discussion.
5. **Build the wall (10 min, whole room).** Post your specimens. As a room, cluster them by
   category and look at what you find: which categories are crowded, which are empty, and what
   the empty ones tell you about how this room plays. Nobody finds localization bugs by accident.
   Argue one contested classification out loud before you sit down.

## Deliverables — post to Padlet
- [ ] Four captures (screenshot or clip under ten seconds each), each labeled with the game and its build/version string
- [ ] A four-row table: Specimen | Did / Happened / Expected | Primary category | Second-guess category (or "none")
- [ ] One sentence naming the category the room found least often, and your theory for why
- [ ] The one classification you would still defend if challenged, in a single sentence of evidence

## If you finish early
Take your strongest specimen and try to find its boundary: does it happen on the first stall or
every stall, at any speed or only above a certain one, on one save file or all of them? Post the
narrowest condition you can prove it needs. That sentence is the seed of a root cause.

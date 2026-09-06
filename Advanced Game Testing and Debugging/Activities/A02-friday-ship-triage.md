# Activity 02 — The Friday Ship Meeting

> **Topic 1 · Session 2 · Week 1 (Sep 10)** · In class, ~55 min · Groups of four, assigned roles
> **Feeds:** Bug Investigation and Prioritization (due Sep 13)

## The setup
It is Wednesday. The build goes to certification Friday morning. Your group has a bug list and
ninety minutes of engineering time left in the week — which means roughly three fixes, and only
if they are small. The producer has already told the publisher the date. Everyone in the room
wants the same thing (a game that ships and works), and everyone is about to disagree loudly
about how to get it. You will run that meeting for real, with roles, and you will not be allowed
to argue from taste.

## Why this matters
Severity and priority are two different axes, and conflating them is how triage meetings turn
into shouting. Severity is about the damage to the player when the bug fires: an S0 corrupts
saves, an S3 misaligns a tooltip. Priority is about what we fix next given the date, the risk,
and who is free — a cosmetic bug on the store page screenshot can outrank a crash in a mode
nobody has unlocked yet. Repro rate is the third axis and the one students forget: an S1 that
fires once in two hundred sessions and an S1 that fires every time are not the same problem.
Professionals who cannot separate these axes get overruled by whoever talks loudest, which in
practice is always the person with the ship date.

## What to do
1. **Assign roles (2 min).** Producer, QA lead, engineer, designer. The producer owns the date
   and may ask "what breaks if we ship it?" at any time. The engineer owns effort estimates and
   may refuse a fix as too risky this close to cert. The designer owns intent and is the only
   person who can rule something "working as designed." The QA lead owns evidence and runs the
   meeting.
2. **Assemble the docket (8 min).** Pool your specimens from Activity 01 and add anything from
   your own past projects. Pick eight bugs, and make sure the docket is uneven on purpose:
   at least one that is obviously catastrophic, one cosmetic bug in a place players will
   screenshot, one that reproduces rarely, and one where it is unclear whether it is a bug at all.
3. **Rate the axes independently (10 min).** Every bug gets three ratings before any discussion
   of what to fix: severity on the S0–S3 ladder, repro rate as a fraction of attempts you have
   actually run (5 of 5, 1 of 12 — no guessing, run it if you do not know), and player visibility
   in one word. Do not decide priority yet. Deciding priority while you are still arguing severity
   is the failure mode this step exists to prevent.
4. **Hold the meeting (20 min, on the clock).** Go bug by bug. Each rating must be defended with
   evidence: a capture, a repro count, a specific consequence to a specific player. The producer
   should challenge any severity supported only by "it feels bad" and the QA lead should refuse
   any challenge that is not itself evidence. Produce a ranked fix order and draw the line after
   the third item — everything below the line ships broken. Write down, for each of the top three,
   the root-cause category you suspect (logic, state, timing/race, precision, resource,
   integration, spec ambiguity, hardware/driver variance) and what evidence would confirm it.
5. **Sign the ship list (5 min).** The producer reads out the bugs going out the door with the
   build. Everyone must be able to state the worst thing a player experiences on Friday. If nobody
   can say it out loud, your severity ratings were wrong.
6. **Find the disagreement you did not resolve (5 min).** Every group has one. Write it down as
   two competing one-sentence positions. It goes in your video.

## Deliverables — post to Padlet
- [ ] Your eight-row triage table: Bug | Severity | Repro rate (x of y) | Visibility | Priority rank | Root-cause suspicion
- [ ] The ship line drawn clearly, with one sentence naming the worst thing a Friday player experiences
- [ ] A ~150-word write-up of one severity you argued and won or lost, quoting the evidence that moved the room
- [ ] The unresolved disagreement, stated as two positions of one sentence each

## If you finish early
Re-run the top three with the date moved: you now ship in six weeks, not two days. Note which
priority ranks change and which do not. The ones that do not change are your true severity
ordering — that distinction is the cleanest way to explain severity versus priority in an
interview, and it belongs in your video.

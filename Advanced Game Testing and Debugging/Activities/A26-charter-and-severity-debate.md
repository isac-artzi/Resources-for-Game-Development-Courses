# Activity 26 — Charters, Then the Severity Debate

> **Topic 7 · Session 26 · Week 13 (Thu Dec 3)** · In class, ~60 min · Solo chartered session, then assigned-position debate in fours
> **Feeds:** Ethical and Compliance Testing with a Christian Worldview (due Dec 6)

## The setup
First half: a real session-based exploratory test. You write a charter, you time-box it, you take
notes as you go, and you debrief it in a fixed structure — this is exploratory testing done as a
discipline, not as wandering around with a controller. Second half: you take one finding out of
your own session and the room argues about its severity, with positions assigned by draw. Some of
you will be told to argue that the finding should be downgraded so the build can ship on time. You
will argue it honestly and well, because the point of the exercise is that you find out how easy
that argument is to make, and how good it sounds coming out of your own mouth.

## Why this matters
Two skills collide in this session and both of them show up in every QA career. The first is that
scripted tests only find the bugs you already thought of; the interesting defects — the ethical and
compliance ones especially — are found by a person exploring with a purpose, and the only way that
scales in a studio is if the exploration is chartered, time-boxed, and debriefed so its results are
comparable to anyone else's. The second is that severity is where integrity actually gets tested in
this job. Nobody will ever ask you to lie. Someone will ask, warmly and reasonably, whether this is
"really an S1", eleven days before a date that a hundred people have planned around, and every
argument in the room will be plausible. There is also a bias problem underneath it: whose report of
"unplayable" gets believed. A finding from a senior engineer with a capture gets an owner in ten
minutes; the same finding from an external tester, or about a player population nobody in the room
belongs to, waits three sprints. Your assignment asks for exploratory testing with charters and a
reflection on integrity, respect for others, and stewardship. Both halves of today produce material
you will use directly.

## What to do
1. **Write a charter (7 min).** Use the standard form and keep it to one sentence:
   **Explore** *(area)* **with** *(resources)* **to discover** *(information)*. Then add a time box,
   an oracle — how you will know something is wrong — and what is explicitly out of scope. Pick one
   of these missions, because they aim at the assignment:
   - Explore the first five minutes of the game with a network capture running, to discover what
     data leaves the machine before the player has consented to anything.
   - Explore the settings and onboarding flow with the accessibility constraints from Activity 23,
     to discover which options exist, which are discoverable, and which do nothing.
   - Explore the monetization or reward loop with a fresh account, to discover whether randomized
     outcomes, currency conversions, and odds are disclosed clearly enough for a player to know what
     they are buying.
   - Explore the game's content against its declared age rating, to discover the moment that would
     surprise a rating board.
   - Explore subtitles against audio across three scenes, to discover mismatches, missing speaker
     identification, and gameplay-critical sound with no visual counterpart.
   Use your own project, a provided sample, or any game you can currently run. A charter that could
   be satisfied by "I played for a while" is too vague — rewrite it until it names the information
   you are hunting.
2. **Run the session, 25 minutes, uninterrupted.** Keep notes in three separate streams as you go,
   not afterward: **test notes** (what you did and saw), **issues** (defects, each with enough to
   reproduce), and **obstacles** (things that stopped you testing — a missing build, a login you do
   not have, a tool you could not get running). Track roughly how your time split between setting
   up, testing on charter, and investigating a specific bug. If you go off charter because you found
   something better, write down when and why; that is a legitimate result, not a failure.
3. **Debrief in the fixed structure (8 min).** Write five short paragraphs, one line each is fine:
   **Past** (what you did), **Results** (what you found), **Obstacles** (what got in the way),
   **Outlook** (what is still unexplored and what charter should come next), **Feelings** (how you
   felt about the area — and yes, this one counts, because a tester's unease about an area is a
   signal that has been right often enough to keep). Then propose the next charter. A session that
   does not produce the next charter has wasted the most valuable thing it generated.
4. **Set the table for the debate (5 min, groups of four).** Each group picks one real finding from
   one member's session — ideally an accessibility, privacy, or ratings finding, because those are
   the ones where the argument gets genuinely hard. State it as a defect: what happens, who it
   affects, how often, and the proposed severity. Then draw positions:
   - **Position A:** argue that the severity should be downgraded and the build should ship on the
     date. Use the real arguments: the affected population is small, there is a workaround, we can
     patch in week two, the data does not show players hitting it, the date has commitments behind
     it, we have shipped worse.
   - **Position B:** argue that the severity stands and the build holds.
   - **Position C:** the producer. You have to decide, out loud, with a reason, and you have to say
     what you would tell the affected players if asked.
   - **Position D:** the recorder. You say nothing. You write down which specific arguments actually
     moved the room, and whether they moved it because they were true or because they were
     comfortable.
   You may be assigned a position you do not hold. Argue it honestly and as well as you can — a
   position you cannot state at its strongest is one you do not understand.
5. **Debate, then debrief the debate (15 min).** Two rounds of four minutes, then the producer
   decides, then the recorder reads their notes. Spend the last five minutes on the part that
   matters: what made the weaker argument persuasive? Name the specific moves when you see them —
   severity quietly redefined as priority so the conversation is about scheduling instead of harm;
   "no one has complained" used as evidence when nobody affected is in your player base yet;
   sample-size arguments that assume your playtest panel represents your players; the passive voice
   arriving right when accountability does ("it was decided to defer"); and the oldest one, the
   person with the most seniority in the room going first. Also ask directly: if the same finding
   had been reported by a different person, would it have moved faster?
6. **Write the position paragraph (5 min).** One paragraph, in your own voice, on the ethical claim
   the debate exposed. This is the seed of the reflection due Dec 6, so make it specific — not
   "testers should be honest" but "I will not restate a severity to protect a date, and here is what
   I will do instead when I am asked to." Ground it. The course frames three grounds and asks you to
   engage them seriously: integrity in reporting, where a false measure corrupts the instrument
   other people use to decide whether a build is safe; respect for persons as the reason
   accessibility is owed rather than donated, because a player with one hand is not a market
   segment but a person; and stewardship of player data as something entrusted to you rather than
   owned by you. Name the ground you are standing on — a duty of respect owed to persons as ends in
   themselves, a professional code such as the ACM/IEEE software engineering ethics principles, a
   religious commitment, or your own considered commitments. The requirement is a real argument
   with a real ground, not agreement.

## Deliverables — post to Padlet
- [ ] Your charter, with time box, oracle, and out-of-scope, plus your session sheet showing the three note streams and your rough time split
- [ ] Your five-line debrief (Past, Results, Obstacles, Outlook, Feelings) and the next charter you propose
- [ ] The finding your group debated, stated as a defect, with the severity before the debate and after it
- [ ] The recorder's list: the two arguments that actually moved the room, and one line on whether each was true or merely comfortable
- [ ] Your position paragraph, ~150 words, with the ground you argue from stated explicitly

## If you finish early
Write the counter-argument to your own position paragraph — the strongest honest case against what
you just committed to, made by someone who is not a villain. Then write the one sentence that
answers it. A reflection that has survived its own best objection reads completely differently from
one that has never met one, and this is the paragraph that will carry your video.

# Activity 30 — Demo Day and the Portfolio Build

> **Topic 8 · Session 30 · Week 15 (Thu Dec 17)** · In class, ~60 min · Solo assembly, then three-minute showcase with questions from the room
> **Feeds:** Mobile Game Optimization (due Dec 20) · Final exam (90 min)

## The setup
Last session. The room turns into a science fair. Each of you stands up for three minutes with the
single strongest before-and-after result you produced this semester — any topic, any week — presents
it with the number and the method, and then takes two hard questions from the room. Before that, you
spend fifteen minutes assembling the semester into one artifact you can actually send to an employer.
The questions are not a formality; the question bank runs across all eight topics, and answering two
of them under mild pressure in front of people is a better preparation for the final exam than
rereading anything. This is not a summary session. Fifteen weeks produced roughly two dozen artifacts
sitting in a Padlet and a folder on your machine, and in three months they will be unfindable and
unexplainable. Today converts them into two things that survive: a portfolio artifact with a link you
can paste into an application, and the spoken version of your best result, rehearsed once in front of
an audience that will interrupt.

## Why this matters
Two facts about the industry make this hour worth more than another lecture. First, QA and technical
hiring is evidence-based in a way that most software hiring is not — the interviewer is going to ask
you to describe a bug you found and how you found it, and the difference between a candidate who says
"I used the profiler" and one who says "average frame rate looked fine at 58, so I looked at the 1%
lows, found a 40 ms spike every 2.3 seconds, matched it to a GC collection, traced it to a string
concatenation in the HUD update, pooled the builder, and the 1% low went from 19 to 46 fps" is the
entire interview. Second, your own memory of this semester decays fast. The number you measured on
October 20 is already fuzzy and by February it will be gone, along with the ability to explain what
made it interesting. Writing it down while you can still reconstruct the method is not
administration; it is the only way any of this stays yours.

## What to do
1. **Assemble the exhibit index (15 min).** Go back through the semester's work — your own Padlet
   posts, your repositories, your assignment submissions — and pick six to eight exhibits that span
   the topics. Aim for coverage: a bug report in PAL format with real repro steps, a unit test suite
   covering the six defect triggers, a profiling capture with a diagnosed bound, a frame-time or share
   table, a memory snapshot diff with a reference chain, an accessibility barrier log, a compliance
   checklist, a security attack log, a mobile test plan, a thermal curve, an optimization before and
   after. For each one, write four fields:
   - **Problem** — one sentence, stated as what was wrong, not what you did.
   - **Evidence** — the artifact itself: capture, table, snippet, log, or link.
   - **Action** — what you changed or concluded.
   - **Result** — a number. Every exhibit that can carry a number must carry one. Exhibits that
     cannot (a checklist, a charter) carry a finding instead.
2. **Publish it somewhere with a URL (part of that 15 min).** A GitHub repository with a README that
   is the index and the artifacts in folders is the strongest option and costs ten minutes. A single
   PDF, an itch.io page, or a slide deck exported and linked also works. The requirement is that a
   stranger can open one link and see the index without asking you for anything. Name the repository
   something a recruiter can read.
3. **Prepare the three-minute talk (5 min).** One exhibit only — your strongest before-and-after.
   Structure it: what was wrong and how you knew, what you measured and with what, what you changed,
   what the number did, and one sentence on **what you would not claim** from this result. That last
   sentence is the one that makes experienced people trust you, and almost nobody thinks to say it.
4. **Demo day (25 min, whole room).** Three minutes each, standing, in rotation. After each talk the
   room asks exactly two questions, drawn from the presenter's own work or from this bank — and the
   room is expected to actually ask hard ones, because a soft question wastes the presenter's turn:
   - How do you know that improvement was not thermal noise, or a different build configuration?
   - Your average frame rate barely moved. What did the 1% low do, and why is that the number?
   - What is the difference between the severity and the priority of the defect you just described?
   - Your test passes. Name a real defect it would fail to catch.
   - What does a memory leak mean in a garbage-collected engine, and what did yours actually turn out
     to be?
   - Which of your tests would be flaky on CI, and what would you do about it rather than deleting it?
   - You captured this in the editor. What changes in a development build, and in a shipping build?
   - What is your lawful basis for collecting that field, and how would you delete it on request?
   - Your game runs at 60 in the editor and 22 on the phone. Walk me through the first three things
     you look at.
   - Who does this defect affect that nobody in this room belongs to, and how would you have found it?
   Write down the two you were asked and how you answered. If an answer came out badly, write the
   better version now while it stings — that is the one you will remember.
5. **Write the interview answer (8 min).** Ninety seconds, written out, to: *"Tell me about a bug you
   found and how you found it."* Use one exhibit. Situation, what made it hard, the method you
   applied, the evidence, the outcome with a number, and what you changed about how you work as a
   result. Read it out loud once and cut every sentence that does not contain a fact.
6. **Map what you can prove, and what you cannot (7 min).** One list, eight rows, one per topic:
   *"I can prove I can do this, and here is the exhibit"* or *"I cannot yet, and here is the gap."*
   Be honest — the honest version is the useful one. The two topics where you have no exhibit are
   exactly where your remaining preparation time goes before the exam, and you now know that from
   evidence instead of from anxiety. Name the specific thing you will revisit for each: not "review
   memory" but "I never explained fragmentation to anyone, so I will explain it to someone tonight."

## Deliverables — post to Padlet
- [ ] The link to your published portfolio artifact, openable by a stranger
- [ ] Your exhibit index: six to eight rows of Problem / Evidence / Action / Result, with a topic label on each
- [ ] The two questions the room asked you and your answers, including the better version of any answer you fumbled
- [ ] Your ninety-second interview answer, written out
- [ ] Your eight-row can-prove / cannot-yet list, with the two gaps you are closing before the exam and the specific action for each

## If you finish early
Take the exhibit you are least proud of and improve it in fifteen minutes — recapture a screenshot at
a readable resolution, add the missing build number and device to a table, or rewrite a result
sentence so it names the control conditions. Portfolios are judged on their weakest visible item far
more than on their best one, and a reviewer who finds one table with no units on it starts doubting
every number on the page.

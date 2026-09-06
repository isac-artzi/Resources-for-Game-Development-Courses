# Activity 24 — Write the Checklist, Then Fail Your Own Build

> **Topic 7 · Session 24 · Week 12/13 (meets Tue Dec 1 - Thanksgiving week shift)** · In class or async, ~50 min · Solo audit, then paired cross-check
> **Feeds:** Ethical and Compliance Testing with a Christian Worldview (due Dec 6)

*Note: this session may be rescheduled because of the Thanksgiving break; everything below works alone at your own desk, and the cross-check step can be done entirely on Padlet.*

## The setup
A publisher is not going to hand you a friendly summary of what your game must satisfy before it
goes on a store. You will be handed a certification requirement document with a few hundred
numbered items written in flat legal prose, and you will be expected to turn it into something a
tester can execute in an afternoon. Today you write that document yourself, from scratch, for your
own game — and then you run it against your own build and fail yourself in public. Nobody has ever
authored a certification checklist and then passed it on the first pass. That is not a failure of
your game. That is the checklist doing its job.

## Why this matters
Certification is the one gate in this industry that does not care about your opinion. A submission
that fails cert comes back with an item number and a repro, your date slips by the length of the
resubmission window, and every marketing commitment attached to that date slips with it. The
failures are almost never gameplay: they are a controller unplugged during a save, the wrong
capitalization of a platform's own terminology, a game that does not survive being suspended and
resumed, an age gate missing where the law requires one, an unlicensed font in the credits, or a
patch that added a gambling-shaped mechanic and quietly invalidated the rating the store is
displaying. Ratings work the same way: a rating is issued against a declaration you signed, and if
a later build contradicts the declaration, the rating is wrong and that is your problem, not the
rating board's. Your assignment asks for a compliance assessment covering ratings, copyright, and
legal exposure. A generic paragraph about "following industry standards" is worth nothing. A
twenty-line checklist you wrote, executed, with three honest failures on it, is a professional
artifact.

## What to do
1. **Pick a target and declare it (3 min).** Choose a plausible destination for your own project, a
   provided sample, or any game you can currently run: a console store, a mobile store, or a PC
   storefront. Write one line: platform, storefront, intended age rating, and the regions you claim
   to be shipping in. Every item you write below has to be true *for that target*. A checklist with
   no declared target is a wish list.
2. **Author the checklist (20 min).** Twenty lines minimum, drawn from five sources. Each line must
   be phrased so that a tester who has never seen your game can execute it and produce a verdict.
   "Handles interruptions gracefully" is not executable. "With the game in an active level, receive
   an incoming call; on returning, the game is paused, audio has resumed at prior volume, and no
   progress since the last checkpoint is lost" is executable.
   - **Ratings content.** Walk the categories a rating questionnaire actually asks about: violence
     and its realism, blood, language, sexual content, controlled substances, gambling or
     gambling-simulating mechanics, user-to-user interaction, unrestricted internet access,
     location sharing, and in-game purchases including randomized ones. Write a line per category
     that applies to you, stating what your game contains and what descriptor that implies.
   - **Platform technical requirements.** Lifecycle and interruption (suspend, resume, background,
     foreground, low memory, incoming call or alarm), controller connect and disconnect mid-action,
     save data integrity including a write interrupted by power loss, install and uninstall leaving
     nothing behind, correct use of the platform's own terminology and button glyphs, offline
     behavior, and account sign-out mid-session.
   - **Legal and copyright.** Every third-party asset in your build: font, music track, sound
     effect, model, plugin, shader, code snippet copied from an answer site. For each one, the
     license, whether attribution is required, and whether the license permits commercial use. This
     line is where student projects fail hardest and it is the one that follows you personally.
   - **Consumer protection.** If you have any randomized purchase, disclosure of odds; if you have
     currency, whether the real-money value is clear; if children can plausibly play, whether an
     age gate exists before data collection or purchase.
   - **Accessibility and localization surface.** Carry two lines forward from Activity 23 — the
     measured contrast failure and the single-channel information — as compliance items, because on
     a growing number of platforms and in a growing number of jurisdictions they now are.
3. **Run the audit against a real build (15 min).** Every line gets exactly one verdict:
   **Pass**, **Fail**, **Not applicable** with a one-clause reason, or **Cannot determine**.
   "Cannot determine" is a legitimate and professional answer — it means the check needs hardware,
   a document, or an authority you do not have — and each one generates a named question for a
   named person. Record evidence for every Fail: a screenshot, a clip, or a file path. An audit
   line with a Fail and no evidence is an accusation.
4. **Classify the failures (7 min).** For each Fail, decide: is this a **submission blocker** (the
   platform or the law will stop you), a **rating risk** (it contradicts what you declared), or an
   **ordinary defect** (it should be fixed but it will not stop a submission)? Write the reason in
   one sentence. Then answer this question in writing: which single change to your game, made three
   days before submission, would silently invalidate your declared rating? Almost every game has
   one, and it is usually a monetization or user-generated-content feature.
5. **Cross-check with one other person (5 min, in class or on Padlet).** Take somebody else's
   checklist and run three of its lines against your own build. You are looking for two things:
   lines that are not actually executable by a stranger, and lines you did not think to write. Post
   your three verdicts as a comment on their post. If the session does not meet, do this
   asynchronously — pick any two posted checklists and comment on both.

## Deliverables — post to Padlet
- [ ] Your target declaration: platform, storefront, intended rating, regions
- [ ] Your checklist, at least twenty executable lines, grouped by the five sources
- [ ] The audit results table with verdicts and evidence links, including at least three Fails or Cannot-determines
- [ ] Your third-party asset inventory: asset, source, license, attribution required yes/no, commercial use permitted yes/no
- [ ] A ~150-word write-up naming the one failure you would escalate as a submission blocker, the change that would silently invalidate your rating, and the questions your Cannot-determines generated

## If you finish early
Turn three of your checklist lines into automated checks that run in CI: assert that the build has
no debug or developer flag enabled in a shipping configuration, assert that the version string and
build number are present and correctly formatted, assert that no asset in the licensed-restricted
folder is referenced by the shipped build. Certification items that can be automated should never
be verified by a human twice, and being the person who noticed that is how you get put in charge of
the submission pipeline.

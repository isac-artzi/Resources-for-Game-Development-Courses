# Activity 23 — The Constrained Play-Through

> **Topic 7 · Session 23 · Week 12 (Tue Nov 24)** · In class, ~55 min · Solo under a drawn constraint, then whole room
> **Feeds:** Ethical and Compliance Testing with a Christian Worldview (due Dec 6)

## The setup
You are going to play a game today under a constraint you did not choose. You will draw it, and
you will keep it for twenty-five minutes of real play — no peeking around it, no "I'll just do
this one menu normally". Some of you will lose a hand. Some of you will lose color. Some of you
will lose sound, or you will lose the three hundred milliseconds between deciding and acting that
your entire skill at games is built on. Then you will write down every single place the game
stopped working for you, and you will discover that most of them were cheap to fix and nobody
tested for them.

## Why this matters
Accessibility gets taught as a checklist and it dies as a checklist, because a checklist item you
have never felt is a chore. The list has an item that says "do not use color alone to convey
information" and you nod and you keep shipping a red-versus-green ammo indicator, because on your
monitor it is obvious. Roughly one man in twelve has some form of color vision deficiency; a
deuteranope looking at your indicator sees two nearly identical muddy tones and dies in your
tutorial. The professional consequence is concrete: accessibility failures are found by players in
public, on launch day, and the fix ships in a patch six weeks later with an apology attached, when
the same defect cost about forty minutes to fix during development. Your assignment requires
accessibility testing across color blindness, mobility, and cognitive dimensions with evidence.
Evidence means a barrier you can name, a screen where it happens, and a guideline it violates —
not a paragraph saying you care about inclusion.

## What to do
1. **Draw a constraint (2 min).** One per person, and you keep it. If the room is small, take two.
   - **One hand.** Your non-dominant hand goes in your pocket or behind your back. Keyboard and
     mouse with one hand, or a controller played with one thumb. No resting the pad on your knee.
   - **No color.** Turn on a color vision filter for the whole display: Windows Settings >
     Accessibility > Color filters (deuteranopia / protanopia / tritanopia), macOS System Settings >
     Accessibility > Display > Color Filters, or a full-screen shader overlay if you have one. Do
     not use the game's own colorblind mode — you are testing whether it needs one.
   - **No audio.** Sound off completely. Subtitles on if the game has them, and if it does not, that
     is your first finding.
   - **Delayed input.** Sit on your hands. A partner holds the controls and presses exactly and only
     what you say out loud. This costs you one to three seconds per action and it is the closest
     thing in this room to playing with a motor impairment or a switch device.
   - **Low vision.** Push your chair back to roughly two meters from the screen, or set the display
     to a resolution that makes UI text render at half its intended size.
2. **Play for 25 minutes, on the clock.** Use your own project, a provided sample, or any game you
   can currently run. Play the parts that matter: the first-time launch, the settings menu, the
   tutorial, one real combat or challenge encounter, an inventory or shop screen, a save/load, and
   a death-and-retry loop. Do not play the parts you are good at. Play the parts a new player has
   to survive.
3. **Log every barrier as it happens, not afterward.** You will forget them. Keep a running table
   with these columns and fill a row the moment you get stuck: **Moment** (screen or action) /
   **What I could not do** / **Why, in design terms** / **Guideline it touches** / **Cheapest fix
   that would have worked**. Aim for eight rows in twenty-five minutes. Most people get more.
4. **Make two of them measurable (10 min).** A barrier written as a feeling is arguable; a barrier
   written as a number is a defect.
   - Take a screenshot of a UI element that failed you and sample the two colors involved —
     foreground text and its background, or the two states of an indicator. Compute the WCAG
     contrast ratio and compare against 4.5:1 for body text and 3:1 for large text and non-text UI
     components. Write the actual ratio.
   - Take one place where information was carried by color alone and state the redundant channel
     that should be there: shape, icon, label, pattern, position, or motion. "Low health is red" is
     not a design; "low health is red, pulsing, and the number turns to a fraction" is.
   - If your constraint was audio, find one line of speech or one gameplay-critical sound with no
     subtitle, no caption, and no visual counterpart, and name it. If subtitles exist, check them
     against the standard tests: readable size, a background or outline so they survive a bright
     scene, speaker identification when more than one character talks, and no more than two lines
     at a time.
   - If your constraint was input, count the actions that require two simultaneous inputs or a held
     input, and check whether the game lets you remap or toggle them.
5. **Trade constraints for five minutes.** Hand your setup to someone who drew a different one and
   take theirs. You will find barriers your own constraint hid from you, and you will discover that
   two constraints on the same screen do not add — they multiply.
6. **Room debrief (10 min).** Post the wall. Sort every barrier the room found into three buckets:
   fixable in under an hour, fixable this sprint, needs a design change. The under-an-hour column
   will be the longest one, and that fact is the point of the session. Argue one barrier that
   somebody wants to call "not a bug, that's the design" — you will meet that sentence again in
   Activity 26, and it is worth hearing it now while the stakes are low.

## Deliverables — post to Padlet
- [ ] Your barrier log, at least six rows: Moment / What I could not do / Why in design terms / Guideline it touches / Cheapest fix
- [ ] One screenshot taken under your constraint, annotated with the element that failed and the WCAG contrast ratio you measured for it
- [ ] One line naming a piece of information the game carried by a single channel, and the redundant channel you would add
- [ ] A ~150-word write-up: the barrier that surprised you most, whether you would have caught it in a normal play session, and the one you will fix first in your own project

## If you finish early
Take your cheapest fix and actually implement it — a contrast bump, a shape added to a colored
indicator, a hold-to-press converted to a toggle, a font size scalar. Capture before and after
under the same constraint and post both images side by side. A before/after pair with a measured
contrast ratio underneath it is the single most convincing exhibit you can put in the accessibility
section of the assignment, and it takes about twenty minutes.

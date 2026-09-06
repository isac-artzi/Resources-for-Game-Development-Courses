# Activity 27 — First Contact With the Phone, Then Swap Plans

> **Topic 8 · Session 27 · Week 14 (Tue Dec 8)** · In class, ~60 min · Solo deploy and baseline, then adversarial plan review in pairs
> **Feeds:** Mobile Game Optimization (due Dec 20)

## The setup
Get something running on a phone in the first fifteen minutes. Not optimized, not pretty — running,
with a stopwatch on it and four numbers written down. Then spend the rest of the session writing the
test plan your assignment requires, and hand it to a partner whose only job is to find the criteria
you wrote that nobody could actually execute. A test plan that survives a hostile read is a document;
one that has never been read by anyone else is a wish.

## Why this matters
The phone is not a small PC. It is a different machine with a different renderer, a different memory
model, and a thermal ceiling that your desktop does not have. Tile-based deferred rendering means
overdraw and full-screen post effects cost you bandwidth in a way that is nearly free on a discrete
GPU. Memory is unified, so a texture budget and a system memory budget are the same budget. And the
number that matters is not the frame rate at second thirty, it is the frame rate at minute ten, after
the chassis has heated and the governor has clawed back clocks. Students routinely present a mobile
optimization result measured in the first twenty seconds of a session, on a plugged-in device, and
the number is meaningless. The test plan is the fix for all of that: it is where you commit, in
advance and in writing, to the device, the route, the duration, and the threshold — so that the
result you present later is a measurement rather than an anecdote. Your assignment names a detailed
test plan with manual and automated strategies as its first deliverable. You are writing it today
while the tools are open, not on the nineteenth.

## What to do
1. **Get a target running (15 min).** Any of these counts, and nobody is blocked:
   - Your own project built to Android or iOS and installed on your phone or a partner's.
   - A provided sample project built to a device.
   - Any game already on your phone that you can play and observe — you can still measure load time,
     sustained frame rate by observation, battery drain, and thermal behavior.
   - A device emulator or simulator, with the honest note that it tells you nothing about thermal or
     GPU behavior and is only good for lifecycle, layout, and input testing.
   - The **throttle proxy**: run on your laptop with the discriminating handicaps applied — cap the
     resolution to 720p, force the integrated GPU, run unplugged with the OS battery saver on and
     the power plan set to its lowest, and cap the frame rate to 30. It is not a phone, but it is a
     machine that cannot brute-force, and it will surface the same class of problem.
2. **Capture the t=0 baseline (part of that 15 min).** Four numbers, written down with the device
   name and the exact build:
   - **Cold load time**, from tapping the icon to the first frame you can interact with. Stopwatch is
     fine; do it three times and record the median.
   - **Frame rate** on a fixed thirty-second route, with the median and, if your tooling gives it to
     you, the 1% low.
   - **Peak memory** during that route.
   - **Battery percentage** at start and at the end of a ten-minute session, or the energy readout
     if your tooling has one. Note whether the device was plugged in — if it was, the number is
     invalid, and knowing that is half the lesson.
3. **Log first contact failures (10 min).** These are the defects that exist only because the machine
   is different, and they never appear on your development desktop. Walk the game and find at least
   five:
   - Touch targets smaller than roughly a fingertip — about 44 points or 9 millimeters — or placed
     under the thumb that holds the phone.
   - Text sized for a 27-inch monitor and unreadable at arm's length.
   - Safe areas: content under a notch, a punch-hole camera, a rounded corner, or the home indicator.
   - Interactions that assume hover, a right click, a scroll wheel, or a cursor that can rest.
   - Orientation change mid-play, and whether the layout survives it.
   - Backgrounding: press the home button mid-level. Does it pause, does audio stop, does it come
     back, does it lose progress? Then receive a call or an alarm. Then let the OS kill it for memory
     and relaunch. These are the top certification failures on mobile and they cost minutes to test.
   - Network loss mid-session, and airplane mode toggled on and off.
4. **Write the test plan (20 min).** One page. Sections, each of which has to say something specific
   about *your* game:
   - **Scope and non-goals.** What is being tested, and what you are explicitly not testing, so
     nobody assumes coverage you do not have.
   - **Device matrix.** Your target tiers with a stated reason for each — a low tier that represents
     the floor you promise to support, a mid tier that represents most of your players, a high tier
     that catches the opposite failures. Name real devices or real chipset classes, not "low-end
     phone".
   - **Entry and exit criteria.** What must be true before testing starts (build number, symbols
     available, test accounts, a clean device state) and what must be true to call it done.
   - **Pass thresholds, as numbers.** Median frame rate on the named route, 1% low, cold load time
     ceiling, peak memory against a stated budget, battery drain per ten minutes, and — the one
     everybody forgets — sustained frame rate at minute ten, not at second thirty.
   - **Manual charters.** Three to five, in the charter form you practiced in Activity 26, aimed at
     interruption, input, layout, and network conditions.
   - **Automated suites.** What runs without a human: unit and PlayMode or Functional tests in CI, a
     scripted route driven from a test harness, an automated load-time measurement, a monkey or
     stress pass (`adb shell monkey` is a legitimate starting point), a device farm run if you have
     access to one. Say what each suite would catch that a human would miss, and be honest about
     what automation cannot see on a phone: thermal behavior, visual quality, and touch ergonomics.
   - **Telemetry plan.** What you would instrument to learn what players actually do, and how you
     would weight your test effort by that distribution rather than by what is easy to test. Testing
     a route nobody plays is coverage you paid for and did not receive.
   - **Risks and gaps.** The tiers you cannot obtain, the OS versions you cannot install, the thing
     you will therefore not know before shipping.
5. **Swap and attack the plan (10 min, pairs).** Read your partner's plan with one goal: find what
   cannot be executed. Specifically hunt for a pass threshold with no baseline behind it, a criterion
   with no measurable verdict ("performance is acceptable"), a device tier with no rationale, an
   automated suite that requires infrastructure neither of you has, a route that is not defined
   precisely enough for two people to run it the same way, and any place the plan measures at t=0.
   Deliver three specific objections. Then fix two of them in your own plan before you post it.

## Deliverables — post to Padlet
- [ ] Your t=0 baseline table: device or proxy, build, cold load median of three, route frame rate median and 1% low, peak memory, battery start and end, plugged-in yes or no
- [ ] Your first-contact failure list, at least five, each with a screenshot or one-line repro
- [ ] Your one-page test plan (snippet, screenshot, or link)
- [ ] The three objections your partner raised, and a one-line note on the two you fixed and the one you did not, with the reason

## If you finish early
Define your benchmark route so precisely that a stranger could reproduce it: the starting save, the
exact path, the actions taken at each step, the duration, the device state before starting, and how
many runs you will average. Post it as a numbered recipe card. Every measurement you present for the
rest of this course depends on that card existing, and the version you write today with the game in
front of you is worth ten times the one you reconstruct next week.

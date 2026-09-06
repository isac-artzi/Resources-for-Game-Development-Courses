# Activity 25 — Adversarial Security Pairing

> **Topic 7 · Session 25 · Week 13 (Tue Dec 1)** · In class, ~60 min · Pairs, builder and attacker, then swap
> **Feeds:** Ethical and Compliance Testing with a Christian Worldview (due Dec 6)

## The setup
One of you writes a score submission and save routine. The other one breaks it. You get fifteen
minutes to build and twenty minutes to attack, then you trade chairs and do it again. The builder
is not allowed to defend during the attack window and the attacker is not allowed to read the code
before the clock starts — you attack the interface, the way a real attacker does, which means you
attack the save file on disk and the request on the wire. Both of you document. At the end of the
hour each of you has a working exploit and a hardened routine with a before-and-after diff, which
is exactly the artifact the security half of your assignment asks for.

## Why this matters
Game security failures do not usually look like a movie. They look like a JSON save file with
`"coins": 500` in plain text, a leaderboard that accepts whatever number the client sends, a name
field concatenated straight into a SQL statement, and a crash log helpfully containing the player's
email address and device identifier because someone added it to make support easier. Every one of
those is a student-project pattern that survives into shipped games. The consequences are not
abstract: a client-trusted leaderboard means your top ten is fictional within a week of launch and
your community stops caring; an unencrypted, unvalidated save means an economy exploit that ruins a
live game's balance; PII in logs is the most common way a small studio ends up on the wrong side of
GDPR or CCPA, because logs get shipped to third-party crash services, retained forever, and nobody
ever wrote down that they contain personal data. The single most common mistake this class makes is
putting a player identifier into a log line "temporarily". Practicing the attack once is what makes
you see the defense as obvious rather than as paranoia.

## What to do
1. **Builder: write the target (15 min).** Use your own project, a provided sample, or a fresh
   twenty-line script — this does not need a game attached. Build two things, honestly, the way you
   would if nobody were watching:
   - A **save routine** that writes at least four fields to disk: player name, a currency or score,
     a progress marker, and a timestamp. Use whatever you would normally use — JSON to persistent
     data path, `PlayerPrefs`, a `USaveGame`, a binary blob.
   - A **score submission** that sends name and score somewhere: a local HTTP endpoint, a local
     SQLite or text file standing in for a server, or a shared file on the machine. If you have no
     server, a function called `SubmitScore(name, score)` that appends a row to a table file is a
     perfectly good target.
   Do not harden anything yet. Write it the way it gets written at 1 a.m.
2. **Attacker: work the four classes, twenty minutes on the clock.** Log every attempt whether it
   worked or not — the failures are evidence too.
   - **Tamper.** Find the save on disk and edit it. Change the currency to a large number, then to a
     negative one, then to a non-numeric string. Does it load? Does it crash? Does it silently clamp?
     Copy the file into another profile slot and see whether it is accepted.
   - **Replay and forge.** Submit the same score twice. Submit a score for a session that never
     happened. Submit a completion time shorter than the level's minimum possible time. If there is
     a request, capture it and resend it unchanged.
   - **Inject.** Put hostile content in the one field the player controls, the name: an SQL fragment
     such as `Bobby'); DROP TABLE scores;--`, a markup or script fragment if the name is ever shown
     in a web view or a launcher, a format specifier such as `%s%s%s`, a path traversal such as
     `../../save`, a string of ten thousand characters, an emoji sequence and a right-to-left
     override character. Note which one crashed, which one corrupted the display, and which one
     changed the meaning of a statement.
   - **Trust the client.** Ask what the server or the loader believes without checking. Score sent
     by the client with no server-side recomputation, level completion asserted by the client,
     currency spent client-side and the result reported, an object identifier in a request that you
     can increment to reach somebody else's record.
3. **Sweep for PII, both of you (8 min).** Run the game normally for two minutes, then open every
   log, crash dump, analytics payload, and save file it produced. Search for: name, email, IP
   address, device or advertising identifier, account token, precise location, machine name, file
   paths containing a real user account name. For each hit write the regulation surface it touches
   — personal data under GDPR, personal information under CCPA, anything about a child under COPPA
   — and what a tester is and is not allowed to do with a build that contains real player data.
   This is a five-minute check that most professional teams do not perform and it will find
   something.
4. **Harden one thing properly (12 min).** Pick your worst finding and fix it for real, then write
   down honestly what your fix does and does not buy:
   - Server-side validation and recomputation: the only actual fix for client trust. Everything else
     is a speed bump.
   - Parameterized queries or prepared statements: the actual fix for injection. Escaping by hand is
     not.
   - Input validation at the boundary: length cap, allowed character set, numeric range, applied
     when data enters, not when it is displayed.
   - Integrity: an HMAC or signature over the save contents with a key the client does not simply
     contain. Say the honest thing out loud — anything encrypted on a machine the attacker owns is
     obfuscation, and its value is raising cost, not preventing access.
   - Transport: TLS, and a note on what plaintext HTTP telemetry exposes on a shared network.
   - Log redaction and data minimization: stop collecting the field, or hash it, or truncate it. The
     cheapest way to protect data is not to have it.
5. **Write the data inventory (8 min).** Five to eight rows, for your own game: **What you collect /
   Why you need it / Where it lives / How long you keep it / How a player would get it deleted**.
   Then answer the deletion question concretely: if a player emailed you today and asked you to
   delete everything you hold about them, could you actually do it, and what would you miss? Almost
   every honest answer is "the analytics backups and the crash logs", and knowing that about your
   own system is the whole point of the exercise.

## Deliverables — post to Padlet
- [ ] Your attack log: attempt / payload used / result / the control that would have stopped it, at least six rows across at least three of the four classes
- [ ] One screenshot or short clip of a successful tamper, forge, or injection, with the payload visible
- [ ] The hardening as a before/after snippet, plus one line on what the fix genuinely prevents and what it only makes expensive
- [ ] Your PII sweep result: the lines you found, or a stated "none found and here is where I looked"
- [ ] Your data inventory table and a ~150-word write-up answering whether you could honestly fulfil a deletion request today

## If you finish early
Turn your best exploit into a regression test. A unit or integration test that submits a negative
score, a ten-thousand-character name, and a replayed submission, and asserts that each one is
rejected with the right error, means that class of bug cannot silently return when somebody
refactors the endpoint next month. Post the test and its output. Security findings that end in a
test are the ones that stay fixed.

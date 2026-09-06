# Activity 17 — Asset Autopsy and the Blind A/B

> **Topic 5 · Session 17 · Week 9 (Nov 3)** · In class, ~60 min · Solo autopsy, then a whole-room blind viewing
> **Feeds:** Multi-Layered Game Optimization (due Nov 8)

## The setup
Every project has five assets that are quietly eating a quarter of the build. Nobody chose them on
purpose. Someone imported a 4096-pixel texture for a crate the player sees at ten meters, someone
left Read/Write enabled on a mesh so it lives in system memory and video memory at the same time,
and someone dropped a stereo 48 kHz WAV in for a UI click. Today you put the five heaviest assets in
your project on the table, open them up, and write down cause of death for each. Then you fix one —
and at the end of the hour the room looks at two screenshots and votes on which one is the
optimized build. If the room cannot tell, you did the job right.

## Why this matters
Asset optimization is the layer where the wins are largest and the argument is hardest. A texture
import setting can take 40 MB to 5 MB in thirty seconds of work, which no amount of clever C# will
match — but the moment an artist believes you have degraded their work, you will spend three
meetings defending a change that saved a week of load time. The professional skill is not knowing
that ASTC exists. It is being able to walk into a review with a before-and-after pair, a memory
number, and a blind comparison that nobody can pass, so the conversation is about evidence rather
than taste. Your assignment's second layer is exactly this, and the blind A/B is the strongest
possible piece of evidence you can put in a slide deck.

## What to do
1. **Pull the heavy list (12 min).** Your own project, a provided sample, or any project you can
   open. Get a ranked list of the biggest contributors — by build size, by runtime memory, or both,
   and say which you used.
   - **Unity:** the build report in `Editor.log` after a build lists assets by size, largest first.
     For runtime, take a Memory Profiler snapshot and sort by size in the All Of Memory view.
     Check the Texture and Mesh importers for the guilty five.
   - **Unreal:** the Size Map on a level or asset, the Asset Audit window with resource size
     columns, `obj list class=Texture2D` or `class=StaticMesh` sorted by size, and the Reference
     Viewer to see who is dragging what in.
2. **Autopsy the top five (18 min).** For each, write cause of death — the specific reason it is
   heavy, not a general one. Candidates worth checking, because these five account for most student
   bloat: resolution far beyond the screen space the asset occupies; wrong or absent compression
   format for the target platform (BCn on desktop, ASTC on mobile, ETC2 as a fallback); mipmaps
   disabled, which costs bandwidth every frame as well as memory; Read/Write or CPU access left on,
   duplicating the asset in system memory; no LOD chain on a mesh with a six-figure triangle count;
   audio imported uncompressed and set to decompress on load when it should stream or stay
   compressed in memory; an animation clip with no compression and full float curves; a shader
   compiling hundreds of variants because a keyword got multiplied by a quality setting. Record the
   current size and the size you believe is achievable.
3. **Fix exactly one, and instrument the fix (12 min).** Pick the asset with the best ratio of bytes
   saved to risk of visible change. Apply the fix, rebuild or reimport, and record the new number
   from the same tool you used in step 1. Capture a screenshot of the asset in-game before and after
   from an identical camera position — same position, same rotation, same lighting, same resolution,
   same time of day. A pair shot from two different angles proves nothing and will be challenged.
4. **The blind viewing (12 min, whole room).** Post your pair with the labels stripped, in a
   randomized order that only you know. Everyone walks the wall and votes on which image in each
   pair is the optimized one. Then the tallies go up. A room voting at roughly a coin flip means
   the saving was free. A room picking your optimized shot correctly nine times out of ten means
   you took a fidelity hit you need to either justify or undo — and it is much better to learn that
   here than in the assignment review.
5. **Write the trade line (6 min).** One sentence in the form a technical artist would accept:
   "Reducing the hero prop's albedo from 4096 to 1024 with ASTC 6x6 saved 21 MB of VRAM and 0.4 ms
   of texture bandwidth per frame; twelve of fourteen viewers could not identify the optimized
   image." Numbers on both sides of the trade, always.

## Deliverables — post to Padlet
- [ ] The five-row autopsy table: asset | current size | cause of death in one specific clause | achievable size
- [ ] Your before/after pair from an identical camera, with the tool readout showing the size change
- [ ] The blind A/B tally for your pair, as raw counts, and one line interpreting it
- [ ] Your trade line: the saving and the fidelity cost in the same sentence, both quantified

## If you finish early
Check what your fix does on a different target. A texture format that is ideal on desktop may not
be supported on mobile and will silently fall back to something larger and slower, and an audio
setting that streams well from an SSD can stutter from slower storage. Switch the build target,
reimport, and record whether your saving survived. Post the two-platform comparison — the
assignment asks you to analyze the impact of optimizations across platforms, and one honest
counterexample where a win did not transfer is worth more than five wins that did.

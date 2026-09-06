// PuzzleFeedback.cs — Activity 4.3: The Runestone Puzzle
// Turns puzzle states into things the player can see and hear: the altar light changes color, the glyph cubes
// pulse while the puzzle waits and burn steady when it is solved, and short sounds mark each placement,
// success, and failure. Keeping feedback in its own script means the controller stays pure logic.
// Attached to: "Altar" in the starter scene, next to PuzzleController. Altar Light, glyphs and an AudioSource are assigned.
// Created by Isac Artzi

using UnityEngine;

public class PuzzleFeedback : MonoBehaviour
{
    [Header("Light")]
    [Tooltip("The point light above the altar. Its color tells the state from across the room.")]
    public Light altarLight;

    public Color waitingColor = new Color(0.35f, 0.65f, 1.0f);
    public Color checkingColor = Color.white;
    public Color solvedColor = new Color(0.3f, 1.0f, 0.4f);
    public Color failedColor = new Color(1.0f, 0.25f, 0.2f);

    [Header("Glyphs")]
    [Tooltip("The glyph cubes on the wall. They pulse while Waiting and glow steadily when Solved.")]
    public Renderer[] glyphs;

    [Tooltip("Pulses per second while waiting.")]
    public float pulseFrequency = 0.8f;

    [Header("Sound (optional — drop any short clips in)")]
    public AudioSource audioSource;
    public AudioClip placeClip;
    public AudioClip solvedClip;
    public AudioClip failedClip;

    PuzzleController.PuzzleState current = PuzzleController.PuzzleState.Waiting;
    // Each glyph's color as painted by PuzzleController.ShowTarget(). Read lazily on the first Update, because
    // Start() order between scripts is not guaranteed and we need the painted colors, not the white default.
    Color[] glyphBaseColors;

    /// <summary>Called by PuzzleController whenever the state changes.</summary>
    public void SetState(PuzzleController.PuzzleState state)
    {
        current = state;

        // TODO 1: Color the light for the state and play the matching sound.
        //             Color c = waitingColor;
        //             switch (state)
        //             {
        //                 case PuzzleController.PuzzleState.Checking: c = checkingColor; break;
        //                 case PuzzleController.PuzzleState.Solved:   c = solvedColor;  PlayClip(solvedClip); break;
        //                 case PuzzleController.PuzzleState.Failed:   c = failedColor;  PlayClip(failedClip); break;
        //             }
        //             if (altarLight != null) altarLight.color = c;
        //         Why color AND sound: a player looking at their hands misses the light; a player in a noisy room misses
        //         the sound. Redundant channels are the rule for puzzle feedback.
        //         Look at: Light.color, the switch statement, AudioSource.PlayOneShot.
        //         Check: the altar light is blue at start, turns green on the right order, red on a wrong one.
    }

    /// <summary>Short confirmation when any stone seats, regardless of correctness.</summary>
    public void PlayPlaceTick()
    {
        PlayClip(placeClip);
    }

    void Update()
    {
        if (glyphs == null || glyphs.Length == 0) return;
        if (glyphBaseColors == null) CacheGlyphColors();

        // TODO 2: Pulse the glyphs while Waiting; hold them bright when Solved; dim them when Failed.
        //             float intensity;
        //             if (current == PuzzleController.PuzzleState.Solved) intensity = 3f;
        //             else if (current == PuzzleController.PuzzleState.Failed) intensity = 0.3f;
        //             else intensity = 1.5f + 1.0f * Mathf.Sin(2f * Mathf.PI * pulseFrequency * Time.time);   // 0.5 .. 2.5
        //             for (int i = 0; i < glyphs.Length; i++)
        //                 if (glyphs[i] != null && glyphs[i].material.HasProperty("_EmissionColor"))
        //                     glyphs[i].material.SetColor("_EmissionColor", glyphBaseColors[i] * intensity);
        //         Why a sine: a slow breathing pulse says "this is waiting for you" without being an alarm.
        //         Look at: Mathf.Sin, Material.SetColor("_EmissionColor", ...).
        //         Check: the wall glyphs breathe about once per 1.25 s; on success they lock bright; on failure they dim.
    }

    void CacheGlyphColors()
    {
        glyphBaseColors = new Color[glyphs.Length];
        for (int i = 0; i < glyphs.Length; i++)
            glyphBaseColors[i] = glyphs[i] != null ? glyphs[i].material.color : Color.white;
    }

    void PlayClip(AudioClip clip)
    {
        // TODO 3: Play the clip if both the source and the clip exist: audioSource.PlayOneShot(clip, 1f).
        //         Why null-check: the starter scene ships without audio files; the puzzle must work silently too.
        //         Check: assign any short clip from Docs/FreeAssets.md sources to Place Clip and seat a stone.
    }
}

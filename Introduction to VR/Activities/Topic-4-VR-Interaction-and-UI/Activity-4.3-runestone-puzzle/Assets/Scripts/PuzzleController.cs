// PuzzleController.cs — Activity 4.3: The Runestone Puzzle
// The brain of the puzzle: knows the target order, shows it on the glyph cubes, records the order in which
// stones are seated, compares the two when every socket is filled, and moves through a small state machine
// (Waiting → Checking → Solved | Failed → Waiting). It never touches XRI directly — RuneSocket does that.
// Attached to: "Altar" in the starter scene, with sockets, stones, glyphs and feedback already assigned.
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    /// <summary>The puzzle's states. Feedback (light, sound) is driven from these.</summary>
    public enum PuzzleState { Waiting, Checking, Solved, Failed }

    [Header("Target")]
    [Tooltip("Rune ids in the order the player must place them. Length should equal the number of sockets.")]
    public int[] targetSequence = { 2, 0, 1 };

    [Header("Scene wiring")]
    [Tooltip("All runestones in the scene, used to look up a color for each id when painting the glyphs.")]
    public Runestone[] stones;

    [Tooltip("The sockets on the altar, left to right.")]
    public RuneSocket[] sockets;

    [Tooltip("The glyph cubes on the wall, left to right. Glyph i shows the color of targetSequence[i].")]
    public Renderer[] glyphs;

    [Tooltip("Where state changes are turned into light and sound.")]
    public PuzzleFeedback feedback;

    [Header("State (read-only at runtime)")]
    [Tooltip("Current state of the puzzle.")]
    public PuzzleState state = PuzzleState.Waiting;

    [Tooltip("Rune ids in the order they were seated. Cleared when the puzzle resets.")]
    public List<int> placedOrder = new List<int>();

    void Start()
    {
        ShowTarget();
        SetState(PuzzleState.Waiting);
    }

    /// <summary>Paints each glyph cube with the color of the rune it stands for.</summary>
    public void ShowTarget()
    {
        if (glyphs == null || targetSequence == null) return;
        for (int i = 0; i < glyphs.Length && i < targetSequence.Length; i++)
        {
            if (glyphs[i] == null) continue;

            // TODO 1: Find the color for targetSequence[i] and paint glyph i with it.
            //             Color c = ColorForRune(targetSequence[i]);
            //             glyphs[i].material.color = c;
            //             if (glyphs[i].material.HasProperty("_EmissionColor")) glyphs[i].material.SetColor("_EmissionColor", c * 2f);
            //         Why show it at all: a puzzle is only fair if the answer is discoverable. The glyphs ARE the clue.
            //         Look at: ColorForRune (below), Renderer.material, Material.SetColor.
            //         Check: the three wall glyphs turn violet, cyan, amber (the order Dusk, Tide, Ember) at Play.
        }
    }

    /// <summary>The color of the stone whose id matches, or white if no stone has that id.</summary>
    public Color ColorForRune(int runeId)
    {
        if (stones != null)
        {
            foreach (var s in stones)
                if (s != null && s.runeId == runeId) return s.color;
        }
        return Color.white;
    }

    /// <summary>Called by a RuneSocket when a stone seats. Records the id and checks when the altar is full.</summary>
    public void OnStonePlaced(RuneSocket socket, Runestone stone)
    {
        if (stone == null) return;
        if (state == PuzzleState.Solved) return;           // nothing more to do once solved

        // TODO 2: Record the placement and evaluate when complete.
        //             placedOrder.Add(stone.runeId);
        //             if (feedback != null) feedback.PlayPlaceTick();
        //             if (placedOrder.Count >= targetSequence.Length) Evaluate();
        //         Why record ORDER and not socket position: the puzzle is "in which sequence", so the list of ids in
        //         the order they arrived is the whole answer. Which socket each went into does not matter here.
        //         Look at: List<int>.Add, List<int>.Count.
        //         Check: the Placed Order list in the Inspector grows as you seat stones; on the third stone the
        //         state changes to Solved or Failed.
    }

    /// <summary>Called by a RuneSocket when a stone is pulled out. Lets the player undo and recover from Failed.</summary>
    public void OnStoneRemoved(RuneSocket socket, Runestone stone)
    {
        if (stone == null) return;
        if (state == PuzzleState.Solved) return;

        // TODO 3: Undo.
        //             placedOrder.Remove(stone.runeId);           // removes the first matching id
        //             if (state == PuzzleState.Failed) SetState(PuzzleState.Waiting);
        //         Why go back to Waiting on ANY removal after a failure: the red light said "wrong"; the moment the
        //         player changes something, the puzzle is live again. This is the "recoverability" rule.
        //         Look at: List<int>.Remove(T).
        //         Check: after a wrong order (red light), pull out one stone — the light returns to blue and the
        //         list has two entries; seat the stone again and the check runs again.
    }

    /// <summary>Compares two sequences element by element.</summary>
    public static bool SequencesMatch(IList<int> a, IList<int> b)
    {
        // TODO 4: Return false if either is null or the lengths differ; otherwise loop and return false at the
        //         first position where a[i] != b[i]; return true after the loop.
        //         Look at: IList<int>.Count, a plain for loop. (List<int> and int[] both implement IList<int>.)
        //         Check with the Console or a quick test: SequencesMatch(new[]{2,0,1}, new List<int>{2,0,1}) is true,
        //         {2,1,0} is false, {2,0} is false.
        return false;
    }

    /// <summary>Compares placedOrder to targetSequence and moves to Solved or Failed.</summary>
    void Evaluate()
    {
        SetState(PuzzleState.Checking);

        // TODO 5: Decide.
        //             bool ok = SequencesMatch(placedOrder, targetSequence);
        //             SetState(ok ? PuzzleState.Solved : PuzzleState.Failed);
        //             Debug.Log(ok ? "Runestone puzzle SOLVED" : "Wrong order: " + string.Join(",", placedOrder) + " vs " + string.Join(",", targetSequence));
        //         Look at: string.Join(string, IEnumerable<T>) for readable logs.
        //         Check: Dusk, Tide, Ember (violet, cyan, amber) → green light and the success sound; any other order → red.
        SetState(PuzzleState.Waiting);   // placeholder until TODO 5 is done
    }

    /// <summary>Changes state and forwards it to the feedback script.</summary>
    public void SetState(PuzzleState newState)
    {
        state = newState;
        if (feedback != null) feedback.SetState(newState);
    }

    /// <summary>Clears the recorded order (stones stay where they are). Used by the reset button in the stretch goals.</summary>
    public void ResetPuzzle()
    {
        placedOrder.Clear();
        SetState(PuzzleState.Waiting);
    }
}

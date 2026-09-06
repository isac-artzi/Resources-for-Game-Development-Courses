// ArtifactPiece.cs — Activity 3.4: Ancient Ruins: Modular Kit and Mechanisms
// A tiny marker component: "this grabbable is a piece of the legendary artifact, and this is which piece".
// HiddenChamberReveal looks for it on whatever lands in the altar socket, so a loose stone does not open the chamber.
// Attached to: "Sun Disc" in the starter scene (the decoy "Loose Stone" deliberately has none).
// Created by Isac Artzi

using UnityEngine;

public class ArtifactPiece : MonoBehaviour
{
    [Header("Identity")]
    [Tooltip("Which piece this is. HiddenChamberReveal compares this to its Required Piece Id (case-insensitive).")]
    public string pieceId = "sun";

    [Tooltip("Name shown in messages and, later, in the inventory (Topic 4).")]
    public string displayName = "Sun Disc";

    /// <summary>True when this piece matches the given id, ignoring case and surrounding spaces.</summary>
    public bool Matches(string requiredId)
    {
        // TODO 1: Compare robustly. Return false if either string is null or empty; otherwise
        //             return string.Equals(pieceId.Trim(), requiredId.Trim(), System.StringComparison.OrdinalIgnoreCase);
        //         Why: designers type "Sun", "sun " and "SUN" — a puzzle should not fail on a stray space.
        //         Check: Matches("SUN") and Matches(" sun") both return true; Matches("moon") returns false.
        return pieceId == requiredId;
    }

    // TODO 2 (optional, 2 min): add a [ContextMenu("Log identity")] method that Debug.Logs displayName + " (" + pieceId + ")".
    //         It is a cheap way to confirm which object you are looking at in a crowded Hierarchy.
}

// Runestone.cs — Activity 4.3: The Runestone Puzzle
// Identifies one runestone: which rune it carries (an integer id the puzzle compares) and the color that
// stands in for its glyph. Sockets read the id; the glyph cubes on the wall borrow the color.
// Attached to: Runestone Tide / Ember / Dusk in the starter scene, next to their XRGrabInteractable + Rigidbody.
// Created by Isac Artzi

using UnityEngine;

public class Runestone : MonoBehaviour
{
    [Header("Identity")]
    [Tooltip("Which rune this is. The puzzle's target sequence is a list of these ids. Keep them unique.")]
    public int runeId;

    [Tooltip("Name for logs and labels, e.g. 'Tide'.")]
    public string runeName = "Rune";

    [Tooltip("Color of the rune. The glyph cubes show the target order using these colors.")]
    public Color color = Color.cyan;

    void Start()
    {
        ApplyColor();
    }

    /// <summary>Paints this stone's renderer with its color (base + emission) so id and look always agree.</summary>
    public void ApplyColor()
    {
        var rend = GetComponent<Renderer>();
        if (rend == null) return;

        // TODO 1: Tint the stone. Use rend.material (an instance for THIS renderer, so the three stones can differ
        //         while sharing one material asset) and set both the base color and the emission:
        //             rend.material.color = color;
        //             if (rend.material.HasProperty("_EmissionColor")) rend.material.SetColor("_EmissionColor", color * 1.5f);
        //         Why an instance: rend.sharedMaterial would recolor every object using that asset — all three stones.
        //         Look at: Renderer.material vs Renderer.sharedMaterial, Material.SetColor.
        //         Check: in Play the three stones are cyan, amber and violet; changing Color in the Inspector and
        //         re-entering Play changes the stone.
    }

    /// <summary>Readable form for logs, e.g. "Tide (id 0)".</summary>
    public override string ToString()
    {
        // TODO 2: Return runeName + " (id " + runeId + ")".
        //         Check: the Console prints "Placed Tide (id 0) in socket 1" when you seat a stone (RuneSocket TODO 2).
        return base.ToString();
    }
}

// ArtifactPiece.cs — Activity 7.1: Portals and Scene Flow
// One of the three pieces of the legendary artifact. Walk into it and it reports itself to the persistent
// GameManager, then disappears — and stays gone, even after you leave and come back to this world.
// Attached to: "Artifact Piece" in each stub scene (Forest_Stub, Mountain_Stub, Ruins_Stub).
// Created by Isac Artzi

using UnityEngine;

public class ArtifactPiece : MonoBehaviour
{
    [Header("Identity")]
    [Tooltip("Unique id stored in the GameManager, e.g. forest_piece. Two pieces must never share an id.")]
    public string pieceId = "forest_piece";

    [Header("Presentation")]
    [Tooltip("Spin speed in degrees per second so the piece reads as collectible.")]
    public float degreesPerSecond = 60f;

    [Tooltip("Optional light to switch off when collected.")]
    public Light glow;

    void Start()
    {
        // TODO 1: If this piece was already collected in an earlier visit, hide it right away:
        //             if (GameManager.Instance.HasPiece(pieceId)) Hide();
        //         Why: the scene is rebuilt from disk every time it loads, so the piece is back — only the
        //         GameManager remembers. Persistence = state outside the scene + a check when the scene starts.
        //         Check: collect the Forest piece, go to the Hub, return to the Forest — the piece is gone.
    }

    void Update()
    {
        // TODO 2: Spin: transform.Rotate(Vector3.up, degreesPerSecond * Time.deltaTime, Space.World);
        //         Check: the piece turns steadily on its pedestal.
    }

    void OnTriggerEnter(Collider other)
    {
        // TODO 3: Same head filter as the Portal: if (other.GetComponentInParent<Camera>() == null) return;
        //         Then: if (GameManager.Instance.Collect(pieceId)) Hide();
        //         Look at: GameManager.Collect returns true only the first time — Hide() is then safe.
        //         Check: walking into the piece makes it vanish and the Status Sign count goes up by one.
    }

    /// <summary>Hides the piece without destroying it (keeps the Inspector readable during class).</summary>
    public void Hide()
    {
        var r = GetComponent<Renderer>();
        if (r != null) r.enabled = false;
        var c = GetComponent<Collider>();
        if (c != null) c.enabled = false;
        if (glow != null) glow.enabled = false;
    }
}

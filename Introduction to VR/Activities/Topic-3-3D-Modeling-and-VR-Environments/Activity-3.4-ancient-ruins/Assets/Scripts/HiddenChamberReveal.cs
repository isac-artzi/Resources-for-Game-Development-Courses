// HiddenChamberReveal.cs — Activity 3.4: Ancient Ruins: Modular Kit and Mechanisms
// Watches an XRSocketInteractor on the altar. When an object carrying ArtifactPiece with the right id is placed,
// it fires On Revealed (wired in the starter scene to the false wall's SlidingDoor.Open) and lights the chamber.
// Anything else — a loose stone, the wrong piece — is politely rejected with a message.
// This is "events, not polling": the socket tells us when something happens; we do not check every frame.
// Attached to: "Hidden Chamber" in the starter scene (Socket, Reveal Light and Message label pre-set).
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HiddenChamberReveal : MonoBehaviour
{
    [Header("Puzzle")]
    [Tooltip("The altar socket to watch. Set in the starter scene.")]
    public XRSocketInteractor socket;

    [Tooltip("The ArtifactPiece.pieceId that opens the chamber.")]
    public string requiredPieceId = "sun";

    [Tooltip("Once revealed, stay revealed even if the piece is removed.")]
    public bool stayRevealed = true;

    [Header("Feedback")]
    [Tooltip("Optional light inside the chamber, off until revealed.")]
    public Light revealLight;

    [Tooltip("Optional 3D label above the altar for hints and rejections.")]
    public TextMesh message;

    [Header("Events")]
    [Tooltip("Fires once when the correct piece is placed. The starter scene wires this to the false wall's SlidingDoor.Open.")]
    public UnityEvent onRevealed = new UnityEvent();

    [Tooltip("Fires when a wrong object is placed. Wire a sound or a light flicker here.")]
    public UnityEvent onRejected = new UnityEvent();

    /// <summary>True after the chamber has opened.</summary>
    public bool Revealed { get; private set; }

    void Start()
    {
        if (revealLight != null) revealLight.enabled = false;
        if (message != null) message.text = "Place the Sun Disc on the altar";
    }

    void OnEnable()
    {
        // TODO 1: Subscribe to the socket's select event so OnSocketFilled runs whenever something snaps in:
        //             if (socket != null) socket.selectEntered.AddListener(OnSocketFilled);
        //         Look at: XRSocketInteractor.selectEntered (a UnityEvent<SelectEnterEventArgs>).
        //         Why in OnEnable and not Start: paired with the RemoveListener in OnDisable, it never leaks a subscription
        //         when the chamber object is disabled and re-enabled.
    }

    void OnDisable()
    {
        // TODO 1 (continued): if (socket != null) socket.selectEntered.RemoveListener(OnSocketFilled);
    }

    /// <summary>Called by the socket each time an interactable is selected (snapped) into it.</summary>
    public void OnSocketFilled(SelectEnterEventArgs args)
    {
        // TODO 2: Find out what was placed.
        //             Transform placed = args.interactableObject.transform;
        //             ArtifactPiece piece = placed.GetComponentInParent<ArtifactPiece>();
        //         Look at: SelectEnterEventArgs.interactableObject (an IXRSelectInteractable; .transform gives its Transform).
        //         GetComponentInParent also finds the marker if the collider that got grabbed is a child of the piece.

        // TODO 3: Decide.
        //             if (piece != null && piece.Matches(requiredPieceId)) Reveal(piece.displayName);
        //             else Reject(placed.name);
        //         Check: the Sun Disc opens the false wall; the Loose Stone shows "The altar rejects ..." and nothing moves.
    }

    void Reveal(string what)
    {
        if (Revealed && stayRevealed) return;
        Revealed = true;

        // TODO 4: Announce and fire.
        //             if (message != null) message.text = what + " accepted. The wall remembers...";
        //             if (revealLight != null) revealLight.enabled = true;
        //             onRevealed.Invoke();
        //         Look at: UnityEvent.Invoke. Open the Inspector on Hidden Chamber to see who is listening (False Wall -> Open).
        //         Check: the light comes on inside the chamber and the false wall sinks, revealing the lore scroll.
        Debug.Log("[HiddenChamberReveal] revealed by " + what);
    }

    void Reject(string what)
    {
        if (message != null) message.text = "The altar rejects " + what;
        onRejected.Invoke();
        Debug.Log("[HiddenChamberReveal] rejected " + what);
    }
}

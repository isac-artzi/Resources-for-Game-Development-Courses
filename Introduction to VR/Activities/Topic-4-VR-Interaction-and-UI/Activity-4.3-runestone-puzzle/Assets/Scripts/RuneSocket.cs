// RuneSocket.cs — Activity 4.3: The Runestone Puzzle
// Sits next to an XRSocketInteractor and translates its select events into puzzle language:
// "a Runestone with id X was placed in / removed from socket N". The XRI socket does the snapping and
// holding; this script only listens and reports to the PuzzleController.
// Attached to: Socket 1, Socket 2, Socket 3 on the altar in the starter scene.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class RuneSocket : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("The puzzle this socket reports to.")]
    public PuzzleController controller;

    [Tooltip("Position of this socket on the altar, 0-based, left to right. Used only for logs and labels.")]
    public int socketIndex;

    [Header("State (read-only at runtime)")]
    [Tooltip("The stone currently seated here, or null.")]
    public Runestone currentStone;

    XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        if (socket == null)
            Debug.LogWarning("[RuneSocket] " + name + " has no XRSocketInteractor; nothing can be placed here.");
    }

    void OnEnable()
    {
        if (socket == null) return;
        // TODO 1: Listen to the socket. XRSocketInteractor is an INTERACTOR, so it raises selectEntered when it
        //         takes hold of a stone and selectExited when a hand pulls the stone back out:
        //             socket.selectEntered.AddListener(OnStoneEntered);
        //             socket.selectExited.AddListener(OnStoneExited);
        //         Remove both in OnDisable (below) so a disabled/re-enabled socket never double-reports.
        //         Look at: XRSocketInteractor.selectEntered / selectExited (UnityEvents), UnityEvent<T>.AddListener.
        //         Check: drop a stone into a socket; the Console prints the "Placed ..." line from TODO 2.
    }

    void OnDisable()
    {
        if (socket == null) return;
        // TODO 1 (continued): socket.selectEntered.RemoveListener(OnStoneEntered); socket.selectExited.RemoveListener(OnStoneExited);
    }

    /// <summary>Called by XRI when the socket snaps an interactable into place.</summary>
    public void OnStoneEntered(SelectEnterEventArgs args)
    {
        // TODO 2: Find out WHAT was placed and tell the controller.
        //             var stone = args.interactableObject.transform.GetComponentInParent<Runestone>();
        //             if (stone == null) return;                    // something else fell in; ignore it
        //             currentStone = stone;
        //             Debug.Log("Placed " + stone + " in socket " + (socketIndex + 1));
        //             if (controller != null) controller.OnStonePlaced(this, stone);
        //         Look at: SelectEnterEventArgs.interactableObject (an IXRSelectInteractable) → .transform,
        //                  Component.GetComponentInParent<T>.
        //         Check: the Current Stone field in the Inspector fills in when a stone seats.
    }

    /// <summary>Called by XRI when the interactable leaves the socket (usually because a hand grabbed it).</summary>
    public void OnStoneExited(SelectExitEventArgs args)
    {
        // TODO 3: Report the removal and clear the slot.
        //             var stone = args.interactableObject.transform.GetComponentInParent<Runestone>();
        //             if (stone == null) return;
        //             currentStone = null;
        //             if (controller != null) controller.OnStoneRemoved(this, stone);
        //         Why this matters: a puzzle you cannot UNDO is a trap. Removing a stone must be a legal move that
        //         the controller hears about, so the player can recover from a wrong order.
        //         Check: pull a seated stone out; Current Stone empties and the controller's Placed Order shrinks.
    }

    /// <summary>True when a runestone is seated here.</summary>
    public bool IsFilled { get { return currentStone != null; } }
}

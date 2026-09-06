// GrabHighlighter.cs — Activity 2.1: Grab the Shard
// Swaps an object's material as it moves through XRI's interaction states: idle -> hovered -> held -> seated.
// This is the object's "signifier": the world telling you "this can be picked up" before you commit to a grab.
// Attached to: Shard 1-3 and the River Stone (each already has a Rigidbody + XRGrabInteractable).
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GrabHighlighter : MonoBehaviour
{
    [Header("Materials")]
    [Tooltip("Shown when nothing hovers or holds this object. Leave empty to reuse whatever the Renderer starts with.")]
    public Material idleMaterial;

    [Tooltip("Shown while an interactor (a hand's ray, or a socket) hovers this object.")]
    public Material hoverMaterial;

    [Tooltip("Shown while a hand holds this object. When a SOCKET holds it, we show idle instead (it is 'seated').")]
    public Material heldMaterial;

    [Header("Debug")]
    [Tooltip("Print every hover/select event to the Console. Turn off once it works.")]
    public bool logEvents = true;

    Renderer rend;
    XRGrabInteractable grab;

    /// <summary>True while a hand (not a socket) is holding this object.</summary>
    public bool IsHeldByHand { get; private set; }

    void Awake()
    {
        rend = GetComponent<Renderer>();
        grab = GetComponent<XRGrabInteractable>();
        if (idleMaterial == null && rend != null) idleMaterial = rend.sharedMaterial;
    }

    void OnEnable()
    {
        if (grab == null)
        {
            Debug.LogWarning(name + ": GrabHighlighter needs an XRGrabInteractable on the same GameObject.");
            return;
        }

        // TODO 1: Subscribe to the four XRI UnityEvents on the grab interactable so our methods run when the
        //         state changes. One line each:
        //             grab.hoverEntered.AddListener(OnHoverEntered);
        //             grab.hoverExited.AddListener(OnHoverExited);
        //             grab.selectEntered.AddListener(OnSelectEntered);
        //             grab.selectExited.AddListener(OnSelectExited);
        //         Look at: XRBaseInteractable.hoverEntered / hoverExited / selectEntered / selectExited (UnityEvents),
        //         UnityEvent<T>.AddListener. OnDisable below already removes them, so subscribing in OnEnable
        //         keeps the pair balanced when the object is toggled off and on.
        //         Why code and not the Inspector: the starter scene is generated, and a listener you add in code is
        //         visible to anyone reading the script — that is easier to debug than a hidden Inspector wire.
        //         Check: with Log Events on, point a controller ray at a shard and the Console prints "hover enter".
    }

    void OnDisable()
    {
        if (grab == null) return;
        grab.hoverEntered.RemoveListener(OnHoverEntered);
        grab.hoverExited.RemoveListener(OnHoverExited);
        grab.selectEntered.RemoveListener(OnSelectEntered);
        grab.selectExited.RemoveListener(OnSelectExited);
    }

    /// <summary>Called by XRI when any interactor starts hovering this object.</summary>
    public void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (logEvents) Debug.Log(name + ": hover enter by " + args.interactorObject.transform.name);

        // TODO 2: If a hand is NOT already holding us, show hoverMaterial (call Apply(hoverMaterial)).
        //         Why the guard: while you carry a shard over the altar, the socket hovers it too — the shard must
        //         stay in its "held" look, not flicker to yellow.
        //         Check: the shard turns yellow when the ray touches it and back to blue when the ray leaves.
    }

    /// <summary>Called by XRI when an interactor stops hovering this object.</summary>
    public void OnHoverExited(HoverExitEventArgs args)
    {
        if (logEvents) Debug.Log(name + ": hover exit by " + args.interactorObject.transform.name);

        // TODO 2 (continued): If no hand holds us, go back to idleMaterial.
    }

    /// <summary>Called by XRI when an interactor selects (grabs) this object.</summary>
    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (logEvents) Debug.Log(name + ": selected by " + args.interactorObject.transform.name);

        // TODO 3: Decide who grabbed us. args.interactorObject is the interactor; test its type:
        //             bool bySocket = args.interactorObject is XRSocketInteractor;
        //         - If a socket took us: IsHeldByHand = false and Apply(idleMaterial) — the shard is "seated".
        //         - Otherwise a hand took us: IsHeldByHand = true and Apply(heldMaterial).
        //         Look at: SelectEnterEventArgs.interactorObject (IXRSelectInteractor), the C# 'is' operator,
        //         XRSocketInteractor in UnityEngine.XR.Interaction.Toolkit.Interactors.
        //         Check: grab a shard -> it turns white; drop it into a socket -> it turns blue again while seated.
    }

    /// <summary>Called by XRI when the interactor releases this object.</summary>
    public void OnSelectExited(SelectExitEventArgs args)
    {
        if (logEvents) Debug.Log(name + ": released by " + args.interactorObject.transform.name);

        // TODO 3 (continued): IsHeldByHand = false, then Apply(idleMaterial).
        //         Note: if a socket grabs the shard the same frame you release it, XRI fires your release first and
        //         the socket's select second, so the order of materials still ends correct.
    }

    /// <summary>Swaps the shared material (no per-object material instance is created, so batching still works).</summary>
    void Apply(Material m)
    {
        if (rend != null && m != null) rend.sharedMaterial = m;
    }
}

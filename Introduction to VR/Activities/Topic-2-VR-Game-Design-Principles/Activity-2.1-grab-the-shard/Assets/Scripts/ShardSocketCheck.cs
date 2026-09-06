// ShardSocketCheck.cs — Activity 2.1: Grab the Shard
// Guards one socket on the Altar of Binding. XRI's XRSocketInteractor will happily swallow ANY grab interactable that
// enters its trigger; this script checks the tag of what arrived and spits out anything that is not a Shard.
// Attached to: Shard Socket 1-3 (each has a trigger SphereCollider + XRSocketInteractor; the ring below shows state).
// Created by Isac Artzi

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ShardSocketCheck : MonoBehaviour
{
    [Header("Filter")]
    [Tooltip("Only objects with this tag are allowed to stay in the socket.")]
    public string requiredTag = "Shard";

    [Tooltip("A rejected object must be carried at least this far (meters) from the socket before it wakes up again.")]
    public float clearDistance = 0.3f;

    [Header("Feedback")]
    [Tooltip("The flat ring under the socket. Its material shows idle / accepted / rejected.")]
    public Renderer ringRenderer;
    public Material ringIdle;
    public Material ringAccept;
    public Material ringReject;

    [Header("Debug")]
    public bool logEvents = true;

    /// <summary>True while a correctly tagged shard is seated in this socket.</summary>
    public bool HasShard { get; private set; }

    /// <summary>How many wrong objects this socket has refused (for your Padlet bullets).</summary>
    public int RejectCount { get; private set; }

    XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    void OnEnable()
    {
        if (socket == null)
        {
            Debug.LogWarning(name + ": ShardSocketCheck needs an XRSocketInteractor on the same GameObject.");
            return;
        }

        // TODO 1: Subscribe to the socket's select events:
        //             socket.selectEntered.AddListener(OnSelectEntered);
        //             socket.selectExited.AddListener(OnSelectExited);
        //         Look at: XRBaseInteractor.selectEntered / selectExited. Interactors fire the same event pair as
        //         interactables — the socket is an INTERACTOR (it grabs things), the shard is an INTERACTABLE.
        //         Why selectEntered and not hoverEntered: hover only means "something is nearby"; select means the
        //         socket has actually taken the object, so this is the moment to accept or refuse it.
        //         Check: release a shard over a socket -> Console prints "socket took Shard 1".
        SetRing(ringIdle);
    }

    void OnDisable()
    {
        if (socket == null) return;
        socket.selectEntered.RemoveListener(OnSelectEntered);
        socket.selectExited.RemoveListener(OnSelectExited);
    }

    /// <summary>The socket has grabbed something. Decide whether it may stay.</summary>
    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        var arrived = args.interactableObject.transform.gameObject;
        if (logEvents) Debug.Log(name + ": socket took " + arrived.name);

        // TODO 2: Tag check.
        //             if (arrived.CompareTag(requiredTag)) { HasShard = true; SetRing(ringAccept); }
        //             else { RejectCount++; StartCoroutine(Reject(args.interactableObject)); }
        //         Look at: GameObject.CompareTag (faster and typo-safe compared with == on .tag),
        //         MonoBehaviour.StartCoroutine.
        //         Check: a shard turns the ring green; the River Stone turns it red and is pushed back out (TODO 3).
    }

    /// <summary>The socket let go of its content (someone pulled the shard out, or Reject() kicked it out).</summary>
    public void OnSelectExited(SelectExitEventArgs args)
    {
        if (logEvents) Debug.Log(name + ": socket released " + args.interactableObject.transform.name);
        HasShard = false;
        if (socket != null && socket.socketActive) SetRing(ringIdle);
    }

    /// <summary>
    /// Kicks a wrong object out and keeps the socket asleep until that object has been carried away.
    /// Runs as a coroutine so it can wait across frames.
    /// </summary>
    IEnumerator Reject(IXRSelectInteractable offender)
    {
        // TODO 3: Refuse the object.
        //         1. socket.socketActive = false;   -> XRSocketInteractor stops selecting, so the interaction manager
        //                                             releases the object on its next update (it falls back onto the altar).
        //            SetRing(ringReject);
        //         2. Wait until the offender is far enough away:
        //             while (offender != null && Vector3.Distance(offender.transform.position, transform.position) < clearDistance)
        //                 yield return null;
        //         3. socket.socketActive = true;  SetRing(ringIdle);
        //         Look at: XRSocketInteractor.socketActive, IXRInteractable.transform, 'yield return null' (wait one frame).
        //         Why not simply destroy or teleport the stone: in VR, objects that vanish from your hand break presence.
        //         Refusing and letting physics drop it is what a real altar would do.
        //         Alternative to step 1 you may see in the docs: socket.interactionManager.SelectExit(socket, offender).
        //         Check: drop the River Stone in a socket -> ring flashes red, the stone drops, the socket ignores it
        //         until you carry it 30 cm away; then a real shard is accepted again.
        yield break;
    }

    void SetRing(Material m)
    {
        if (ringRenderer != null && m != null) ringRenderer.sharedMaterial = m;
    }
}

// StorableItem.cs — Activity 4.1: The Hero's Satchel: Inventory
// Marks a grabbable object as "can go in the satchel" and tracks whether a hand is holding it right now.
// The Inventory only stores items that are HELD when they touch the satchel, so a shard rolling past
// your hip does not get swallowed.
// Attached to: each Shard / Scroll / Potion in the starter scene, next to its XRGrabInteractable + Rigidbody.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StorableItem : MonoBehaviour
{
    [Header("Item data")]
    [Tooltip("What this object is, as the satchel will describe it. Edit id, display name and color here.")]
    public InventoryItem item = new InventoryItem();

    [Header("State (read-only at runtime)")]
    [Tooltip("True while an interactor (a hand) is selecting this object. Set by the grab events.")]
    public bool isHeld;

    XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        if (grab == null)
            Debug.LogWarning("[StorableItem] " + name + " has no XRGrabInteractable; it cannot be picked up or stored.");
    }

    void OnEnable()
    {
        if (grab == null) return;
        // TODO 1: Subscribe to the grab events so this script hears when a hand takes or drops the object.
        //             grab.selectEntered.AddListener(OnGrabbed);
        //             grab.selectExited.AddListener(OnReleased);
        //         Look at: XRGrabInteractable.selectEntered / selectExited (UnityEvents in XRI 3.5),
        //                  UnityEvent<T>.AddListener. Subscribe in OnEnable and remove in OnDisable so an object
        //                  that is hidden and shown again (that is exactly what storing does) never ends up
        //                  with two listeners.
        //         Check: grab a shard in Play mode; the Console prints "Grabbed Blue Shard" (see TODO 2).
    }

    void OnDisable()
    {
        if (grab == null) return;
        // TODO 1 (continued): grab.selectEntered.RemoveListener(OnGrabbed); grab.selectExited.RemoveListener(OnReleased);
    }

    /// <summary>Called by XRI when an interactor starts selecting (grabbing) this object.</summary>
    public void OnGrabbed(SelectEnterEventArgs args)
    {
        // TODO 2: Set isHeld = true and Debug.Log("Grabbed " + item.displayName).
        //         Look at: SelectEnterEventArgs.interactorObject (which hand) if you want to print it.
        //         Check: the isHeld checkbox in the Inspector ticks while you hold the object.
    }

    /// <summary>Called by XRI when the interactor lets go.</summary>
    public void OnReleased(SelectExitEventArgs args)
    {
        // TODO 2 (continued): Set isHeld = false.
        //         Check: the checkbox clears when you release. Drop a shard straight through the satchel
        //         volume: nothing happens, because it was not held when it entered.
    }

    /// <summary>Puts the object back into the world at a position, with its physics reset.</summary>
    public void Reappear(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
        isHeld = false;
        gameObject.SetActive(true);
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // TODO 3: Zero the rigidbody's motion so the item does not keep the velocity it had when stored.
            //             rb.linearVelocity = Vector3.zero;  rb.angularVelocity = Vector3.zero;
            //         Look at: Rigidbody.linearVelocity (Unity 6 name; 'velocity' is the obsolete alias).
            //         Check: pull an item out of the satchel right after throwing it in — it appears calmly in
            //         front of you instead of flying off.
        }
    }
}

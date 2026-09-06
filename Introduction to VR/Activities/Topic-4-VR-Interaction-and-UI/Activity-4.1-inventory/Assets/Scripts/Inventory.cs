// Inventory.cs — Activity 4.1: The Hero's Satchel: Inventory
// The satchel itself: an invisible trigger volume that rides at the player's hip, a list of the items
// inside it, and the two operations every inventory needs — Store and Retrieve.
// Attached to: "Satchel" in the starter scene (child of the rig's Camera Offset), together with a
// kinematic Rigidbody and a BoxCollider set to Is Trigger. The Satchel Panel canvas is its child.
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Body anchor")]
    [Tooltip("The player's head (the XR camera). The starter scene assigns it; if empty, Camera.main is used.")]
    public Transform head;

    [Tooltip("How far below eye level the satchel sits, in meters. Eye 1.6 m minus hip 0.9 m is about 0.7.")]
    public float hipDrop = 0.7f;

    [Tooltip("Offset to the player's right, in meters. Positive = right hip.")]
    public float sideOffset = 0.25f;

    [Tooltip("Offset in front of the body, in meters. A little forward keeps it reachable and visible when you look down.")]
    public float forwardOffset = 0.15f;

    [Header("Contents")]
    [Tooltip("Maximum number of items. Matches the number of slots on the panel.")]
    public int capacity = 6;

    [Tooltip("What is currently inside. Read-only at runtime; watch it fill up in the Inspector.")]
    public List<InventoryItem> items = new List<InventoryItem>();

    [Header("Feedback and display")]
    [Tooltip("The world-space panel that shows the slots. Refresh() is called whenever the list changes.")]
    public InventoryPanel panel;

    [Tooltip("Optional. Plays one shot on store and retrieve if a clip is assigned.")]
    public AudioSource audioSource;

    [Tooltip("Where retrieved items appear: this far in front of the head, in meters.")]
    public float retrieveDistance = 0.5f;

    void Start()
    {
        if (head == null && Camera.main != null) head = Camera.main.transform;
        if (panel != null) panel.Refresh();
    }

    void LateUpdate()
    {
        if (head == null) return;

        // TODO 1: Anchor the satchel to the BODY, not the head. Build a flattened forward from the head:
        //             Vector3 forward = head.forward;  forward.y = 0f;
        //             if (forward.sqrMagnitude < 0.0001f) return;   // looking straight up or down: keep last pose
        //             forward.Normalize();
        //             Vector3 right = Vector3.Cross(Vector3.up, forward);
        //         Then place this transform at
        //             head.position + forward * forwardOffset + right * sideOffset + Vector3.down * hipDrop
        //         and rotate it with Quaternion.LookRotation(forward) so the panel yaws with the body but never pitches.
        //         Why flatten? A hip does not tilt when you nod. If you used head.forward directly the satchel
        //         would swing up to your face every time you looked down to find it.
        //         Look at: Vector3.Cross, Quaternion.LookRotation, Transform.SetPositionAndRotation.
        //         Check: in Play, select the head (Tab) and look down — the panel stays at hip height; turn
        //         left/right — it swings around to stay at your right hip; walk — it comes with you.
    }

    void OnTriggerEnter(Collider other)
    {
        // TODO 2: Decide whether the thing that just entered the satchel volume should be stored.
        //             var storable = other.GetComponentInParent<StorableItem>();
        //             if (storable == null || !storable.isHeld) return;
        //             Store(storable);
        //         Why GetComponentInParent: the collider that touched us may sit on a child of the item.
        //         Why require isHeld: only a deliberate "put it away" should store; a shard flying past should not.
        //         Look at: Collider.GetComponentInParent<T>(), MonoBehaviour.OnTriggerEnter (needs a Rigidbody
        //                  on at least one side — the Satchel has a kinematic one, the items have dynamic ones).
        //         Check: hold a shard, move the hand (Tab to a controller, WASD) down to your right hip — the
        //         shard vanishes and the Console prints "Stored: ...".
    }

    /// <summary>Takes a held object out of the world and adds its data to the list.</summary>
    public void Store(StorableItem storable)
    {
        if (storable == null) return;
        if (items.Count >= capacity)
        {
            Debug.Log("[Inventory] Satchel is full (" + capacity + ").");
            return;
        }

        // TODO 3: Store it.
        //         1. InventoryItem entry = storable.item.Clone();
        //         2. entry.storedObject = storable.gameObject;
        //         3. items.Add(entry);
        //         4. storable.gameObject.SetActive(false);   // XRI cancels the grab when a selected object is disabled
        //         5. Debug.Log("Stored: " + entry);  then PlayFeedback();  then if (panel != null) panel.Refresh();
        //         Why hide instead of Destroy: we want the same object back later with its data intact, and
        //         hiding is free — no prefab bookkeeping needed for this activity.
        //         Check: the Items list in the Inspector grows by one and the slot lights up on the panel.
    }

    /// <summary>Puts the item in the given slot back into the world in front of the player.</summary>
    public void Retrieve(int index)
    {
        if (index < 0 || index >= items.Count) return;
        if (head == null) return;

        // TODO 4: Bring the item back.
        //         1. InventoryItem entry = items[index];
        //         2. Compute a spawn point: head.position + flattened head.forward * retrieveDistance + Vector3.down * 0.15f
        //            (flatten the forward exactly as in TODO 1 so the item never spawns at your feet or in the sky).
        //         3. var storable = entry.storedObject != null ? entry.storedObject.GetComponent<StorableItem>() : null;
        //            if (storable != null) storable.Reappear(spawnPoint, Quaternion.identity);
        //         4. items.RemoveAt(index);  PlayFeedback();  if (panel != null) panel.Refresh();
        //         Look at: List<T>.RemoveAt, StorableItem.Reappear.
        //         Check: click a filled slot with the controller ray — the item pops out half a meter in front of
        //         you and can be grabbed again; the slot goes back to empty.
    }

    /// <summary>Plays the store/retrieve sound if an AudioSource with a clip is assigned.</summary>
    void PlayFeedback()
    {
        if (audioSource != null && audioSource.clip != null) audioSource.PlayOneShot(audioSource.clip, 1f);
    }

    /// <summary>How many items are inside right now.</summary>
    public int Count { get { return items.Count; } }
}

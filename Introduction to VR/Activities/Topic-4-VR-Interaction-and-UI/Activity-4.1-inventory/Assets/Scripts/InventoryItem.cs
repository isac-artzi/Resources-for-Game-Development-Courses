// InventoryItem.cs — Activity 4.1: The Hero's Satchel: Inventory
// A plain data class that describes one collectible: its id, the name shown in the satchel panel,
// and the color used for its slot icon. It is NOT a MonoBehaviour — it is a field on StorableItem
// (edited in the Inspector) and an entry in Inventory's list at runtime.
// Created by Isac Artzi

using UnityEngine;

/// <summary>
/// Data for one collectible. [System.Serializable] lets Unity show it in the Inspector
/// as a foldout on any MonoBehaviour that has a public InventoryItem field.
/// </summary>
[System.Serializable]
public class InventoryItem
{
    [Tooltip("Short unique id, e.g. 'shard_blue'. Used to tell items apart in code and in logs.")]
    public string id = "item";

    [Tooltip("What the satchel panel shows on the slot, e.g. 'Blue Shard'.")]
    public string displayName = "Item";

    [Tooltip("Color of the slot icon. Colored squares stand in for icons until you have art.")]
    public Color color = Color.white;

    [Tooltip("Runtime only: the scene object that was stored, hidden with SetActive(false). Inventory fills this in.")]
    [HideInInspector] public GameObject storedObject;

    /// <summary>A copy of the static data (id, name, color) without the runtime object reference.</summary>
    public InventoryItem Clone()
    {
        // TODO 1: Return a new InventoryItem whose id, displayName and color equal this one's.
        //         Why: Inventory keeps its own list entry per stored object; copying the data means
        //         editing the list later cannot corrupt the StorableItem sitting on the object.
        //         Look at: object initializers  new InventoryItem { id = id, displayName = displayName, color = color }.
        //         Check: after storing a shard, the panel slot shows the shard's name and color (Task 4).
        return new InventoryItem();
    }

    /// <summary>Readable one-line form for Debug.Log, e.g. "Blue Shard (shard_blue)".</summary>
    public override string ToString()
    {
        // TODO 2: Return displayName + " (" + id + ")".
        //         Check: the Console prints "Stored: Blue Shard (shard_blue)" when you store a shard.
        return base.ToString();
    }
}

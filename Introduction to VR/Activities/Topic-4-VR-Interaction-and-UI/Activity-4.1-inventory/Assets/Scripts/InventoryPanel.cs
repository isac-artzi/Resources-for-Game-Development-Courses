// InventoryPanel.cs — Activity 4.1: The Hero's Satchel: Inventory
// The visible side of the satchel: a world-space Canvas at hip height with a grid of slot buttons.
// Each slot shows the stored item's name and color; clicking a filled slot pulls the item back out.
// Attached to: "Satchel Panel" (a Canvas, child of the Satchel) in the starter scene. The builder creates the
// slot buttons and assigns them; you write the layout and the refresh logic.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [Header("Data source")]
    [Tooltip("The Inventory this panel displays and sends Retrieve() calls to.")]
    public Inventory inventory;

    [Header("Slots")]
    [Tooltip("One Button per slot, in index order. Each has an Image (the square) and a child Text (the name).")]
    public Button[] slotButtons;

    [Tooltip("Optional title text, e.g. 'Satchel 2/6'.")]
    public Text titleText;

    [Header("Grid layout (canvas pixels; the canvas is scaled so 1 px = 1 mm)")]
    [Tooltip("Slots per row.")]
    public int columns = 3;

    [Tooltip("Width and height of one slot in pixels.")]
    public float slotSize = 110f;

    [Tooltip("Gap between slots in pixels.")]
    public float gap = 12f;

    [Header("Empty-slot look")]
    [Tooltip("Color of an empty slot square.")]
    public Color emptyColor = new Color(0.2f, 0.2f, 0.25f, 0.6f);

    void Start()
    {
        LayoutSlots();

        // TODO 2: Wire every slot button to Retrieve. Inside a for loop over slotButtons:
        //             int index = i;                                   // copy! see why below
        //             slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
        //         Why the copy: a lambda captures the VARIABLE i, not its value; without the copy every button
        //         would retrieve the last index. This is the most common closure bug in Unity UI code.
        //         Look at: Button.onClick (a UnityEvent), lambda expressions.
        //         Check: click the first filled slot and the FIRST stored item comes out, not the last.

        Refresh();
    }

    /// <summary>Places the slot buttons in a grid: index → (row, column) → anchored position.</summary>
    public void LayoutSlots()
    {
        if (slotButtons == null) return;
        int cols = Mathf.Max(1, columns);
        for (int i = 0; i < slotButtons.Length; i++)
        {
            if (slotButtons[i] == null) continue;
            var rt = slotButtons[i].GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(slotSize, slotSize);

            // TODO 1: Compute the slot's grid cell and position.
            //             int row = i / cols;          // integer division
            //             int col = i % cols;          // remainder
            //             float x = col * (slotSize + gap);
            //             float y = -row * (slotSize + gap);   // UI y grows UP, so rows go DOWN with a minus sign
            //             rt.anchoredPosition = new Vector2(x, y);
            //         The buttons' anchors and pivot are already set to top-left by the builder, so (0,0) is the
            //         top-left corner of the panel body and the whole grid hangs from there.
            //         Look at: RectTransform.anchoredPosition, integer division vs. modulo.
            //         Check: with 6 slots and 3 columns you see two rows of three squares; change Columns to 2 in
            //         the Inspector and re-enter Play — three rows of two.
        }
    }

    /// <summary>Repaints every slot from the inventory's current list.</summary>
    public void Refresh()
    {
        if (slotButtons == null) return;
        int count = inventory != null ? inventory.Count : 0;

        for (int i = 0; i < slotButtons.Length; i++)
        {
            var button = slotButtons[i];
            if (button == null) continue;
            var image = button.GetComponent<Image>();
            var label = button.GetComponentInChildren<Text>();

            // TODO 3: Paint this slot.
            //         If i < count:  var item = inventory.items[i];
            //                        image.color = item.color;  label.text = item.displayName;  button.interactable = true;
            //         Else:          image.color = emptyColor;  label.text = "-";               button.interactable = false;
            //         Why interactable=false on empty slots: the ray still highlights them, but a click does nothing
            //         and the dimmed look tells the player "nothing here" before they try.
            //         Look at: Image.color, Text.text, Selectable.interactable.
            //         Check: store the blue shard — slot 1 turns blue and reads "Blue Shard"; the rest stay dim.
            if (image != null) image.color = emptyColor;
            if (label != null) label.text = "-";
        }

        // TODO 4: Update the title: titleText.text = "Satchel " + count + "/" + inventory.capacity  (null-check both).
        //         Check: the title reads "Satchel 0/6", then "Satchel 1/6" after the first store.
    }

    /// <summary>Called by a slot button. Asks the inventory to put that item back into the world.</summary>
    public void OnSlotClicked(int index)
    {
        if (inventory == null) return;
        inventory.Retrieve(index);
    }
}

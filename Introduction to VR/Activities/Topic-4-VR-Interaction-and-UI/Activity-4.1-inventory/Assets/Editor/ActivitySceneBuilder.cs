// ActivitySceneBuilder.cs — Activity 4.1: The Hero's Satchel: Inventory
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a forest-edge camp with four grabbable collectibles on a stone
// table, a hip-anchored satchel trigger volume on the XR rig, and a world-space slot panel wired to it.
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.UI;
using ActivityTools;

public static class ActivitySceneBuilder
{
    const int SlotCount = 6;

    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1f, 0.93f, 0.80f), 1.0f, new Vector3(40f, -25f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.50f, 0.65f, 0.55f), 0.015f);

        // Materials -------------------------------------------------------
        var grass  = SceneBuilderUtil.MakeMaterial("Grass",  new Color(0.25f, 0.50f, 0.20f));
        var stone  = SceneBuilderUtil.MakeMaterial("Stone",  new Color(0.50f, 0.50f, 0.55f));
        var bark   = SceneBuilderUtil.MakeMaterial("Bark",   new Color(0.35f, 0.25f, 0.15f));
        var leaves = SceneBuilderUtil.MakeMaterial("Leaves", new Color(0.15f, 0.40f, 0.18f));
        var leather = SceneBuilderUtil.MakeMaterial("Leather", new Color(0.45f, 0.30f, 0.18f));

        // Floor and player ------------------------------------------------
        SceneBuilderUtil.AddFloor(24f, grass);
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);

        // A half-ring of trees behind the table for enclosure -------------
        var trees = SceneBuilderUtil.AddEmpty("Trees", Vector3.zero);
        for (int i = 0; i < 7; i++)
        {
            float angle = Mathf.Lerp(-60f, 60f, i / 6f) * Mathf.Deg2Rad;
            var basePos = new Vector3(Mathf.Sin(angle) * 8f, 0f, Mathf.Cos(angle) * 8f);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + (i + 1), basePos + Vector3.up * 2f,
                                          new Vector3(0.5f, 2f, 0.5f), bark, trees.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + (i + 1), basePos + Vector3.up * 5f,
                                          Vector3.one * 3f, leaves, trees.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(trees);

        // Stone table with four collectibles -------------------------------
        var table = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Stone Table", new Vector3(0f, 0.4f, 1.2f),
                                                  new Vector3(1.8f, 0.8f, 0.7f), stone);
        table.isStatic = true;
        SceneBuilderUtil.AddLabel("Table Sign", "Collectibles - grab one and bring it to your right hip",
                                  new Vector3(0f, 1.5f, 1.6f), 0.035f, new Color(1f, 0.95f, 0.7f));

        var items = SceneBuilderUtil.AddEmpty("Collectibles", Vector3.zero);
        MakeCollectible(items.transform, PrimitiveType.Cube,    "Blue Shard",     "shard_blue",   new Color(0.35f, 0.75f, 1.00f), new Vector3(-0.6f, 0.95f, 1.2f), new Vector3(0.10f, 0.22f, 0.10f));
        MakeCollectible(items.transform, PrimitiveType.Cylinder,"Lore Scroll",    "scroll_lore",  new Color(0.95f, 0.85f, 0.60f), new Vector3(-0.2f, 0.88f, 1.2f), new Vector3(0.06f, 0.12f, 0.06f));
        MakeCollectible(items.transform, PrimitiveType.Capsule, "Healing Potion", "potion_heal",  new Color(0.95f, 0.25f, 0.45f), new Vector3( 0.2f, 0.92f, 1.2f), new Vector3(0.09f, 0.10f, 0.09f));
        MakeCollectible(items.transform, PrimitiveType.Sphere,  "Amber Rune",     "rune_amber",   new Color(1.00f, 0.65f, 0.20f), new Vector3( 0.6f, 0.90f, 1.2f), new Vector3(0.14f, 0.14f, 0.14f));

        // The satchel: a trigger volume at hip height on the rig ----------
        Transform cameraOffset = rig.transform.Find("Camera Offset");
        if (cameraOffset == null) cameraOffset = rig.transform;
        var satchel = SceneBuilderUtil.AddEmpty("Satchel", Vector3.zero, cameraOffset);
        satchel.transform.localPosition = new Vector3(0.25f, 0.9f, 0.15f);   // Inventory.LateUpdate re-anchors it every frame
        var satchelBody = satchel.AddComponent<Rigidbody>();
        satchelBody.isKinematic = true;
        satchelBody.useGravity = false;
        var satchelTrigger = satchel.AddComponent<BoxCollider>();
        satchelTrigger.isTrigger = true;
        satchelTrigger.size = new Vector3(0.30f, 0.25f, 0.22f);
        var satchelAudio = satchel.AddComponent<AudioSource>();
        satchelAudio.playOnAwake = false;
        satchelAudio.spatialBlend = 1f;

        // A small visible bag so you can see where the trigger is (no collider, so it never blocks a grab)
        var bag = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Satchel Marker", Vector3.zero,
                                                new Vector3(0.18f, 0.14f, 0.10f), leather, satchel.transform);
        bag.transform.localPosition = Vector3.zero;
        Object.DestroyImmediate(bag.GetComponent<Collider>());

        var inventory = satchel.AddComponent<Inventory>();
        inventory.capacity = SlotCount;
        inventory.audioSource = satchelAudio;
        var headCam = rig.GetComponentInChildren<Camera>(true);
        if (headCam != null) inventory.head = headCam.transform;

        // The satchel panel: a world-space canvas above the bag -----------
        var panel = MakeSatchelPanel(satchel.transform, inventory);
        inventory.panel = panel;

        if (Object.FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(XRUIInputModule));

        // Welcome sign ----------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 4.1 - The Hero's Satchel\nGrab, store at your hip, click a slot to take it back\nOpen README.md (Activity > Open README)",
                                  new Vector3(0f, 2.4f, 4f), 0.05f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_4_1");
        Selection.activeGameObject = rig;
    }

    // ---------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------

    static GameObject MakeCollectible(Transform parent, PrimitiveType shape, string displayName, string id, Color color,
                                      Vector3 position, Vector3 scale)
    {
        var mat = SceneBuilderUtil.MakeEmissiveMaterial("Item_" + id, color, 1.2f);
        var go = SceneBuilderUtil.AddPrimitive(shape, displayName, position, scale, mat, parent);

        var rb = go.AddComponent<Rigidbody>();
        rb.mass = 0.5f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var grab = go.AddComponent<XRGrabInteractable>();
        grab.movementType = XRBaseInteractable.MovementType.Kinematic;
        grab.useDynamicAttach = true;
        grab.throwOnDetach = true;

        // InventoryItem is plain data, not a MonoBehaviour: there is no AddComponent<InventoryItem>() —
        // it lives as the 'item' field of StorableItem and is edited in that component's Inspector foldout.
        var storable = go.AddComponent<StorableItem>();
        storable.item = new InventoryItem();
        storable.item.id = id;
        storable.item.displayName = displayName;
        storable.item.color = color;
        return go;
    }

    static InventoryPanel MakeSatchelPanel(Transform parent, Inventory inventory)
    {
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var canvasGo = new GameObject("Satchel Panel", typeof(Canvas), typeof(CanvasScaler), typeof(TrackedDeviceGraphicRaycaster));
        canvasGo.transform.SetParent(parent, false);
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        var rt = canvasGo.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(400f, 330f);
        rt.localScale = Vector3.one * 0.001f;                       // 1 px = 1 mm → 0.40 m x 0.33 m panel
        rt.localPosition = new Vector3(0f, 0.28f, 0.05f);           // just above the bag
        rt.localRotation = Quaternion.Euler(55f, 0f, 0f);           // tilted so it faces your eyes when you look down

        // Dark background
        var bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(canvasGo.transform, false);
        Stretch(bg.GetComponent<RectTransform>());
        bg.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.85f);

        // Title
        var title = MakeText(canvasGo.transform, "Title", "Satchel 0/" + SlotCount, font, 26, TextAnchor.MiddleCenter);
        var titleRt = title.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0f, 1f); titleRt.anchorMax = new Vector2(1f, 1f); titleRt.pivot = new Vector2(0.5f, 1f);
        titleRt.anchoredPosition = new Vector2(0f, -8f); titleRt.sizeDelta = new Vector2(0f, 40f);

        // Slot container: its (0,0) is the top-left corner where the grid hangs from
        var slotsGo = new GameObject("Slots", typeof(RectTransform));
        slotsGo.transform.SetParent(canvasGo.transform, false);
        var slotsRt = slotsGo.GetComponent<RectTransform>();
        slotsRt.anchorMin = new Vector2(0f, 1f); slotsRt.anchorMax = new Vector2(0f, 1f); slotsRt.pivot = new Vector2(0f, 1f);
        slotsRt.anchoredPosition = new Vector2(28f, -60f); slotsRt.sizeDelta = new Vector2(344f, 232f);

        var buttons = new Button[SlotCount];
        for (int i = 0; i < SlotCount; i++)
        {
            var slot = new GameObject("Slot " + (i + 1), typeof(RectTransform), typeof(Image), typeof(Button));
            slot.transform.SetParent(slotsGo.transform, false);
            var slotRt = slot.GetComponent<RectTransform>();
            slotRt.anchorMin = new Vector2(0f, 1f); slotRt.anchorMax = new Vector2(0f, 1f); slotRt.pivot = new Vector2(0f, 1f);
            slotRt.anchoredPosition = Vector2.zero;                 // InventoryPanel.LayoutSlots() spreads them out (TODO 1)
            slotRt.sizeDelta = new Vector2(110f, 110f);
            var img = slot.GetComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.25f, 0.6f);
            var btn = slot.GetComponent<Button>();
            btn.targetGraphic = img;

            var label = MakeText(slot.transform, "Text", "-", font, 18, TextAnchor.MiddleCenter);
            Stretch(label.GetComponent<RectTransform>());
            buttons[i] = btn;
        }

        var panel = canvasGo.AddComponent<InventoryPanel>();
        panel.inventory = inventory;
        panel.slotButtons = buttons;
        panel.titleText = title.GetComponent<Text>();
        panel.columns = 3;
        panel.slotSize = 110f;
        panel.gap = 12f;
        return panel;
    }

    static GameObject MakeText(Transform parent, string name, string content, Font font, int size, TextAnchor anchor)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var text = go.GetComponent<Text>();
        text.text = content;
        text.font = font;
        text.fontSize = size;
        text.alignment = anchor;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return go;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}

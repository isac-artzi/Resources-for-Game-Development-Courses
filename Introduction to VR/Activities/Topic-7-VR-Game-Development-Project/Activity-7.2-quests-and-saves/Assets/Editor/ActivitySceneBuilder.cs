// ActivitySceneBuilder.cs — Activity 7.2: Quests and Saves
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a forest clearing with a grabbable shard, an altar socket,
// a Sage NPC, a wrist label on the left controller, a save panel (Continue / New Game), and a "Quest" object
// carrying QuestManager + SaveSystem with every reference pre-wired.
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;
using ActivityTools;

public static class ActivitySceneBuilder
{
    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1f, 0.95f, 0.85f), 1.1f, new Vector3(45f, -30f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.55f, 0.70f, 0.60f), 0.02f);

        // Materials -------------------------------------------------------
        var grass    = SceneBuilderUtil.MakeMaterial("Grass", new Color(0.25f, 0.50f, 0.20f));
        var stone    = SceneBuilderUtil.MakeMaterial("Stone", new Color(0.55f, 0.55f, 0.58f));
        var bark     = SceneBuilderUtil.MakeMaterial("Bark", new Color(0.35f, 0.25f, 0.15f));
        var leaves   = SceneBuilderUtil.MakeMaterial("Leaves", new Color(0.15f, 0.40f, 0.18f));
        var shardMat = SceneBuilderUtil.MakeEmissiveMaterial("Shard", new Color(0.40f, 0.80f, 1.00f), 2f);
        var sageMat  = SceneBuilderUtil.MakeEmissiveMaterial("Sage", new Color(0.75f, 1.00f, 0.75f), 0.8f);
        var altarMat = SceneBuilderUtil.MakeEmissiveMaterial("Altar", new Color(0.60f, 0.50f, 0.80f), 0.5f);

        SceneBuilderUtil.AddFloor(30f, grass);
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);

        // Trees for enclosure ---------------------------------------------
        var trees = SceneBuilderUtil.AddEmpty("Trees", Vector3.zero);
        for (int i = 0; i < 10; i++)
        {
            float a = i * Mathf.PI * 2f / 10f;
            var p = new Vector3(Mathf.Cos(a) * 10f, 0f, Mathf.Sin(a) * 10f);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + i, p + Vector3.up * 2f, new Vector3(0.5f, 2f, 0.5f), bark, trees.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + i, p + Vector3.up * 5f, Vector3.one * 3f, leaves, trees.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(trees);

        // Station 1: the shard on a pedestal (grabbable) -------------------
        var ped = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Shard Pedestal", new Vector3(-1.5f, 0.5f, 2.0f), new Vector3(0.4f, 0.5f, 0.4f), stone);
        ped.isStatic = true;
        var shard = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Shard", new Vector3(-1.5f, 1.1f, 2.0f), new Vector3(0.12f, 0.25f, 0.12f), shardMat);
        var shardRb = shard.AddComponent<Rigidbody>();
        shardRb.mass = 0.5f;
        var grab = shard.AddComponent<XRGrabInteractable>();
        grab.throwOnDetach = true;
        SceneBuilderUtil.AddPointLight("Shard Light", new Vector3(-1.5f, 1.4f, 2.0f), new Color(0.4f, 0.8f, 1f), 3f, 1.2f, shard.transform);
        SceneBuilderUtil.AddLabel("Shard Sign", "1. Take the shard", new Vector3(-1.5f, 1.7f, 2.3f), 0.04f, Color.white);

        // Station 2: the altar with a socket ------------------------------
        var altar = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Altar", new Vector3(1.5f, 0.5f, 2.5f), new Vector3(0.8f, 1.0f, 0.8f), altarMat);
        altar.isStatic = true;
        var socketGo = SceneBuilderUtil.AddEmpty("Altar Socket", new Vector3(1.5f, 1.15f, 2.5f), altar.transform);
        var socketCol = socketGo.AddComponent<SphereCollider>();
        socketCol.isTrigger = true;
        socketCol.radius = 0.25f;
        var socket = socketGo.AddComponent<XRSocketInteractor>();
        var attach = SceneBuilderUtil.AddEmpty("Attach", new Vector3(1.5f, 1.15f, 2.5f), socketGo.transform);
        socket.attachTransform = attach.transform;
        socket.showInteractableHoverMeshes = true;
        SceneBuilderUtil.AddLabel("Altar Sign", "2. Place it on the altar", new Vector3(1.5f, 1.7f, 2.8f), 0.04f, Color.white);

        // Station 3: the Sage ---------------------------------------------
        var sage = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Forest Sage", new Vector3(0f, 1f, 6f), new Vector3(0.6f, 1f, 0.6f), sageMat);
        SceneBuilderUtil.AddPointLight("Sage Glow", new Vector3(0f, 1.6f, 5.6f), new Color(0.7f, 1f, 0.7f), 4f, 1.2f, sage.transform);
        SceneBuilderUtil.AddLabel("Sage Sign", "3. Walk up to the Sage", new Vector3(0f, 2.4f, 6f), 0.045f, new Color(0.8f, 1f, 0.8f));

        // Wrist label on the left controller (falls back to a camera-anchored label) ----
        var wrist = AddWristLabel(rig);

        // Save panel (world-space canvas) ----------------------------------
        Text statusText;
        Button continueBtn, newGameBtn;
        // Yaw 60 deg turns the canvas' readable side toward the player standing at the origin.
        AddSavePanel(new Vector3(2.5f, 1.4f, 1.5f), 60f, out statusText, out continueBtn, out newGameBtn);

        // Quest object with everything wired ---------------------------------
        var questGo = SceneBuilderUtil.AddEmpty("Quest", Vector3.zero);
        var save = questGo.AddComponent<SaveSystem>();
        var qm = questGo.AddComponent<QuestManager>();
        // QuestStep and SaveData are plain data classes, not components: the steps are created here, and
        // SaveSystem produces SaveData at runtime via JsonUtility.FromJson<SaveData>.
        qm.steps = new List<QuestStep>();
        qm.steps.Add(new QuestStep("take_shard", "Take the shard from its pedestal"));
        qm.steps.Add(new QuestStep("place_shard", "Place the shard on the altar"));
        qm.steps.Add(new QuestStep("meet_sage", "Walk up to the Forest Sage"));
        qm.shard = grab;
        qm.altar = socket;
        qm.sage = sage.transform;
        qm.talkRadius = 1.5f;
        qm.wristLabel = wrist.GetComponent<TextMesh>();
        qm.statusText = statusText;
        qm.continueButton = continueBtn;
        qm.newGameButton = newGameBtn;
        qm.saveSystem = save;

        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 7.2 - Quests and Saves\nLook at your left hand for the quest.\nF5 save - F9 continue - Delete new game",
                                  new Vector3(0f, 2.2f, 3.5f), 0.04f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_7_2");
        Selection.activeGameObject = rig;
    }

    /// <summary>A small TextMesh parented to the left controller; if the rig has none, to the camera.</summary>
    static GameObject AddWristLabel(GameObject rig)
    {
        Transform anchor = FindDeep(rig.transform, "Left Controller");
        var label = SceneBuilderUtil.AddLabel("Wrist Label", "QUEST\n(implement QuestManager TODO 5)", Vector3.zero, 0.008f, new Color(1f, 0.95f, 0.6f));
        var tm = label.GetComponent<TextMesh>();
        tm.anchor = TextAnchor.LowerLeft;
        tm.alignment = TextAlignment.Left;
        tm.fontSize = 48;
        if (anchor != null)
        {
            label.transform.SetParent(anchor, false);
            label.transform.localPosition = new Vector3(0.02f, 0.06f, -0.04f);
            label.transform.localRotation = Quaternion.Euler(50f, 0f, 0f);
        }
        else
        {
            var cam = rig.GetComponentInChildren<Camera>(true);
            if (cam != null) label.transform.SetParent(cam.transform, false);
            label.transform.localPosition = new Vector3(-0.35f, -0.25f, 0.8f);
            label.transform.localRotation = Quaternion.identity;
            tm.characterSize = 0.012f;
        }
        return label;
    }

    static Transform FindDeep(Transform root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    /// <summary>World-space canvas (1 px = 1 mm) with a title, a status line and two buttons.</summary>
    static void AddSavePanel(Vector3 position, float yaw, out Text statusText, out Button continueBtn, out Button newGameBtn)
    {
        var canvasGo = new GameObject("Save Panel", typeof(Canvas), typeof(CanvasScaler), typeof(TrackedDeviceGraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        var rt = canvasGo.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(500f, 360f);
        rt.localScale = Vector3.one * 0.001f;
        rt.position = position;
        rt.rotation = Quaternion.Euler(0f, yaw, 0f);

        var bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(canvasGo.transform, false);
        var bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one; bgRt.offsetMin = Vector2.zero; bgRt.offsetMax = Vector2.zero;
        bg.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.1f, 0.85f);

        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        AddText(canvasGo.transform, "Title", "Realm of Legends — Save", font, 30, new Vector2(0f, 140f), new Vector2(460f, 50f));
        statusText = AddText(canvasGo.transform, "Status", "Status", font, 18, new Vector2(0f, 80f), new Vector2(460f, 70f));
        continueBtn = AddButton(canvasGo.transform, "Continue Button", "Continue", font, new Vector2(0f, 0f));
        newGameBtn  = AddButton(canvasGo.transform, "New Game Button", "New Game", font, new Vector2(0f, -90f));

        if (Object.FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(XRUIInputModule));
    }

    static Text AddText(Transform parent, string name, string content, Font font, int size, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
        var text = go.GetComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.text = content;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        return text;
    }

    static Button AddButton(Transform parent, string name, string label, Font font, Vector2 anchoredPos)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(320f, 70f);
        go.GetComponent<Image>().color = new Color(0.25f, 0.45f, 0.85f, 1f);
        var text = AddText(go.transform, "Text", label, font, 28, Vector2.zero, new Vector2(320f, 70f));
        text.color = Color.white;
        return go.GetComponent<Button>();
    }
}

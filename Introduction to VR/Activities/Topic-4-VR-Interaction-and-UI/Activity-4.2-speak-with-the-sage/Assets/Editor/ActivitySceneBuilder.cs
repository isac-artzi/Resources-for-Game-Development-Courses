// ActivitySceneBuilder.cs — Activity 4.2: Speak with the Sage: NPC Dialogue
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a lantern-lit forest grove with the Forest Sage, a nameplate,
// and a hidden world-space dialogue box above her head wired to a six-node branching conversation.
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;
using ActivityTools;

public static class ActivitySceneBuilder
{
    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(0.75f, 0.80f, 1.0f), 0.45f, new Vector3(20f, -40f, 0f));   // dusk
        SceneBuilderUtil.SetFog(new Color(0.30f, 0.40f, 0.42f), 0.03f);

        // Materials -------------------------------------------------------
        var moss   = SceneBuilderUtil.MakeMaterial("Moss",   new Color(0.18f, 0.38f, 0.20f));
        var bark   = SceneBuilderUtil.MakeMaterial("Bark",   new Color(0.30f, 0.22f, 0.14f));
        var leaves = SceneBuilderUtil.MakeMaterial("Leaves", new Color(0.12f, 0.32f, 0.16f));
        var robe   = SceneBuilderUtil.MakeMaterial("Robe",   new Color(0.35f, 0.25f, 0.50f));
        var iron   = SceneBuilderUtil.MakeMaterial("Iron",   new Color(0.20f, 0.20f, 0.22f));
        var flame  = SceneBuilderUtil.MakeEmissiveMaterial("Flame", new Color(1.0f, 0.75f, 0.35f), 2.5f);
        var glowMoss = SceneBuilderUtil.MakeEmissiveMaterial("GlowMoss", new Color(0.35f, 0.70f, 1.0f), 1.5f);

        // Floor and player ------------------------------------------------
        SceneBuilderUtil.AddFloor(30f, moss);
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);
        var headCam = rig.GetComponentInChildren<Camera>(true);
        Transform head = headCam != null ? headCam.transform : null;

        // Trees around the grove ------------------------------------------
        var trees = SceneBuilderUtil.AddEmpty("Trees", Vector3.zero);
        for (int i = 0; i < 14; i++)
        {
            float angle = i * Mathf.PI * 2f / 14f;
            float radius = 10f + Mathf.Cos(i * 2.3f) * 2f;
            var basePos = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius + 2f);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + (i + 1), basePos + Vector3.up * 2.5f,
                                          new Vector3(0.6f, 2.5f, 0.6f), bark, trees.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + (i + 1), basePos + Vector3.up * 6f,
                                          Vector3.one * 3.5f, leaves, trees.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(trees);

        // Lantern posts along the path to the Sage ------------------------
        var lanterns = SceneBuilderUtil.AddEmpty("Lanterns", Vector3.zero);
        for (int i = 0; i < 3; i++)
        {
            float z = 1.5f + i * 1.5f;
            foreach (float x in new[] { -1.2f, 1.2f })
            {
                var post = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Lantern Post", new Vector3(x, 0.6f, z),
                                                         new Vector3(0.08f, 0.6f, 0.08f), iron, lanterns.transform);
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Flame", new Vector3(x, 1.3f, z), Vector3.one * 0.18f,
                                              flame, post.transform);
                SceneBuilderUtil.AddPointLight("Lantern Light", new Vector3(x, 1.4f, z), new Color(1f, 0.75f, 0.4f), 4f, 1.2f,
                                               post.transform);
            }
        }
        SceneBuilderUtil.MarkStaticRecursive(lanterns);

        // Glowing moss patch where the forest shard "sleeps" (set dressing for the dialogue) ------------
        var mossPatch = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Glowing Moss", new Vector3(2.5f, 0.02f, 6.5f),
                                                      new Vector3(1.2f, 0.02f, 1.2f), glowMoss);
        mossPatch.isStatic = true;

        // The Forest Sage --------------------------------------------------
        var sage = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Forest Sage", new Vector3(0f, 0.9f, 5f),
                                                 new Vector3(0.5f, 0.9f, 0.5f), robe);          // 1.8 m tall
        var hood = SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Hood", new Vector3(0f, 1.75f, 5f),
                                                 Vector3.one * 0.42f, robe, sage.transform);
        Object.DestroyImmediate(hood.GetComponent<Collider>());
        SceneBuilderUtil.AddPointLight("Sage Glow", new Vector3(0f, 1.6f, 4.6f), new Color(0.7f, 0.6f, 1f), 3f, 1.0f, sage.transform);

        var nameplate = SceneBuilderUtil.AddLabel("Sage Nameplate", "Forest Sage", new Vector3(0f, 2.05f, 5f), 0.04f, Color.gray);
        var gazeTarget = SceneBuilderUtil.AddEmpty("Gaze Target", new Vector3(0f, 1.5f, 5f), sage.transform);

        // Dialogue box above the Sage --------------------------------------
        var runner = MakeDialogueBox(new Vector3(0f, 2.55f, 4.7f), head);
        runner.nodes = SageNodes();

        var talk = sage.AddComponent<NpcTalkTrigger>();
        talk.runner = runner;
        talk.head = head;
        talk.nameplate = nameplate.GetComponent<TextMesh>();
        talk.gazeTarget = gazeTarget.transform;
        talk.talkDistance = 3.0f;
        talk.leaveDistance = 4.0f;
        talk.gazeHalfAngle = 30f;

        if (Object.FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(XRUIInputModule));

        // Signs -------------------------------------------------------------
        SceneBuilderUtil.AddLabel("Hint Sign", "Walk toward the Sage and look at her to speak\nKeys 1 / 2 pick a choice on desktop",
                                  new Vector3(0f, 1.9f, 2.2f), 0.035f, new Color(1f, 0.95f, 0.7f));
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 4.2 - Speak with the Sage\nOpen README.md (Activity > Open README)",
                                  new Vector3(-3.5f, 2.2f, 3.5f), 0.05f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_4_2");
        Selection.activeGameObject = rig;
    }

    // ---------------------------------------------------------------------
    // Conversation data (edit freely in the Inspector after building)
    // DialogueNode is plain serializable data, not a MonoBehaviour: there is no AddComponent<DialogueNode>();
    // the nodes live in DialogueRunner.nodes and show up as a foldout list in its Inspector.
    // ---------------------------------------------------------------------

    static DialogueNode[] SageNodes()
    {
        return new[]
        {
            Node("You carry the satchel of the chosen one. Few come this deep into the forest without turning back. What brings you to my grove?",
                 "I seek the artifact shards.", 1, "I lost my way.", 2),
            Node("Three shards fell when the balance broke: one in these woods, one on the Mountain Pass, one beneath the Ancient Ruins. The forest shard sleeps where the moss glows blue.",
                 "How do I wake it?", 3, "Tell me of the Mountain Pass.", 4),
            Node("Everyone who enters Elaria is lost at first. Follow the lanterns; they lead back to the clearing. Return when you know what you seek.",
                 "I seek the artifact shards.", 1, "Farewell.", -1),
            Node("Place your hand on the stone and speak your name. The forest listens. But take care: the runestones must be set in the order the glyphs show.",
                 "I understand.", 5, "Tell me of the Mountain Pass.", 4),
            Node("The pass is cold and the hermit colder. Bring him a healing potion and he may open the gate for you.",
                 "Thank you, Sage.", 5, "How do I wake the shard?", 3),
            Node("Go now. The lanterns will light your way, and I will be here when the wind changes.",
                 "Farewell.", -1, "", -1),
        };
    }

    static DialogueNode Node(string text, string a, int nextA, string b, int nextB)
    {
        var n = new DialogueNode();
        n.text = text;
        n.choiceA = a; n.nextA = nextA;
        n.choiceB = b; n.nextB = nextB;
        return n;
    }

    // ---------------------------------------------------------------------
    // UI
    // ---------------------------------------------------------------------

    static DialogueRunner MakeDialogueBox(Vector3 position, Transform head)
    {
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var canvasGo = new GameObject("Sage Dialogue", typeof(Canvas), typeof(CanvasScaler), typeof(TrackedDeviceGraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        var rt = canvasGo.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(720f, 400f);
        rt.localScale = Vector3.one * 0.001f;           // 0.72 m x 0.40 m
        rt.position = position;

        var bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(canvasGo.transform, false);
        Stretch(bg.GetComponent<RectTransform>());
        bg.GetComponent<Image>().color = new Color(0.06f, 0.07f, 0.10f, 0.88f);

        var nameGo = MakeText(bg.transform, "Name", "Forest Sage", font, 30, TextAnchor.MiddleLeft, new Color(1f, 0.9f, 0.6f));
        var nameRt = nameGo.GetComponent<RectTransform>();
        nameRt.anchorMin = new Vector2(0f, 1f); nameRt.anchorMax = new Vector2(1f, 1f); nameRt.pivot = new Vector2(0.5f, 1f);
        nameRt.anchoredPosition = new Vector2(0f, -14f); nameRt.sizeDelta = new Vector2(-48f, 44f);

        var bodyGo = MakeText(bg.transform, "Body", "", font, 28, TextAnchor.UpperLeft, Color.white);
        var bodyRt = bodyGo.GetComponent<RectTransform>();
        bodyRt.anchorMin = new Vector2(0f, 0f); bodyRt.anchorMax = new Vector2(1f, 1f);
        bodyRt.offsetMin = new Vector2(24f, 110f); bodyRt.offsetMax = new Vector2(-24f, -64f);

        var btnA = MakeButton(bg.transform, "Choice A", "Continue", font, new Vector2(0.03f, 0.05f), new Vector2(0.48f, 0.24f));
        var btnB = MakeButton(bg.transform, "Choice B", "", font, new Vector2(0.52f, 0.05f), new Vector2(0.97f, 0.24f));

        var runner = canvasGo.AddComponent<DialogueRunner>();
        runner.panelRoot = bg;
        runner.nameText = nameGo.GetComponent<Text>();
        runner.bodyText = bodyGo.GetComponent<Text>();
        runner.choiceAButton = btnA.GetComponent<Button>();
        runner.choiceAText = btnA.GetComponentInChildren<Text>();
        runner.choiceBButton = btnB.GetComponent<Button>();
        runner.choiceBText = btnB.GetComponentInChildren<Text>();
        runner.head = head;
        runner.charsPerSecond = 40f;
        return runner;
    }

    static GameObject MakeButton(Transform parent, string name, string label, Font font, Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        var img = go.GetComponent<Image>();
        img.color = new Color(0.25f, 0.30f, 0.45f, 1f);
        go.GetComponent<Button>().targetGraphic = img;
        var text = MakeText(go.transform, "Text", label, font, 24, TextAnchor.MiddleCenter, Color.white);
        Stretch(text.GetComponent<RectTransform>());
        return go;
    }

    static GameObject MakeText(Transform parent, string name, string content, Font font, int size, TextAnchor anchor, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var text = go.GetComponent<Text>();
        text.text = content;
        text.font = font;
        text.fontSize = size;
        text.alignment = anchor;
        text.color = color;
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

// ActivitySceneBuilder.cs — Activity 5.4: Comfort Settings and the Rest Point
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a dusk glade with a ring of marker posts to walk laps around, a
// bench with a lantern (the rest point) and the Guide beside it, a vignette quad under the camera, and two
// world-space panels — the comfort Settings menu and the SSQ-lite Comfort Log — fully wired to their scripts.
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Unity.XR.CoreUtils;
using ActivityTools;

public static class ActivitySceneBuilder
{
    static readonly Color PanelColor  = new Color(0.08f, 0.09f, 0.12f, 0.88f);
    static readonly Color TrackColor  = new Color(0.25f, 0.28f, 0.32f, 1f);
    static readonly Color FillColor   = new Color(1.00f, 0.75f, 0.45f, 1f);
    static readonly Color HandleColor = new Color(0.95f, 0.95f, 1.00f, 1f);
    static readonly Color ButtonColor = new Color(0.30f, 0.34f, 0.40f, 1f);
    static readonly Color TextColor   = new Color(0.95f, 0.95f, 1.00f, 1f);

    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1f, 0.70f, 0.50f), 0.6f, new Vector3(12f, -60f, 0f));   // low dusk sun
        SceneBuilderUtil.SetFog(new Color(0.35f, 0.38f, 0.50f), 0.02f);

        // Materials -------------------------------------------------------
        var grass   = SceneBuilderUtil.MakeMaterial("DuskGrass", new Color(0.18f, 0.32f, 0.18f));
        var bark    = SceneBuilderUtil.MakeMaterial("Bark",      new Color(0.30f, 0.22f, 0.14f));
        var leaves  = SceneBuilderUtil.MakeMaterial("Leaves",    new Color(0.12f, 0.30f, 0.16f));
        var wood    = SceneBuilderUtil.MakeMaterial("BenchWood", new Color(0.45f, 0.32f, 0.20f));
        var postMat = SceneBuilderUtil.MakeEmissiveMaterial("MarkerPost", new Color(0.5f, 0.8f, 1f), 0.7f);
        var lanternMat = SceneBuilderUtil.MakeEmissiveMaterial("LanternGlass", new Color(1f, 0.8f, 0.5f), 2f);
        var guideMat = SceneBuilderUtil.MakeEmissiveMaterial("Guide", new Color(0.90f, 0.85f, 1.00f), 1.2f);
        var vignetteMat = MakeOverlayMaterial("VignetteBlack", new Color(0f, 0f, 0f, 0f));
        vignetteMat.mainTexture = MakeRingTextureAsset("VignetteRing", 128, 0.3f, 0.8f);
        EditorUtility.SetDirty(vignetteMat);

        // Floor (teleportable) and rig -----------------------------------------
        var floor = SceneBuilderUtil.AddFloor(50f, grass);
        var area = floor.AddComponent<TeleportationArea>();
        area.matchOrientation = MatchOrientation.WorldSpaceUp;
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);
        Camera cam = rig.GetComponentInChildren<Camera>(true);

        // Ring of eight marker posts, radius 8 m, centered 10 m ahead: the comfort-test lap ---------------------
        var ring = SceneBuilderUtil.AddEmpty("Test Ring", new Vector3(0f, 0f, 10f));
        for (int i = 0; i < 8; i++)
        {
            float a = i * Mathf.PI * 2f / 8f;
            var pos = new Vector3(Mathf.Cos(a) * 8f, 0f, 10f + Mathf.Sin(a) * 8f);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Post " + (i + 1), pos + Vector3.up * 0.6f, new Vector3(0.15f, 0.6f, 0.15f), postMat, ring.transform);
            SceneBuilderUtil.AddLabel("Post Label " + (i + 1), (i + 1).ToString(), pos + Vector3.up * 1.5f, 0.06f, new Color(0.6f, 0.9f, 1f), ring.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(ring);

        // Trees around the glade -------------------------------------------------------------------------------
        var trees = SceneBuilderUtil.AddEmpty("Trees", Vector3.zero);
        const int treeCount = 20;
        for (int i = 0; i < treeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / treeCount;
            float radius = 15f + Mathf.Sin(i * 2.3f) * 2.5f;
            var basePos = new Vector3(Mathf.Cos(angle) * radius, 0f, 10f + Mathf.Sin(angle) * radius);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + (i + 1), basePos + Vector3.up * 2f, new Vector3(0.5f, 2f, 0.5f), bark, trees.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + (i + 1), basePos + Vector3.up * 5f, Vector3.one * 3f, leaves, trees.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(trees);

        // The rest point: bench + lantern + Guide, off to the right, outside the ring --------------------------
        var restPos = new Vector3(11f, 0f, 4f);
        var restGo = SceneBuilderUtil.AddEmpty("Rest Point", restPos);
        var bench = SceneBuilderUtil.AddEmpty("Bench", restPos, restGo.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Seat", restPos + new Vector3(0f, 0.45f, 0f), new Vector3(1.6f, 0.08f, 0.45f), wood, bench.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Leg L", restPos + new Vector3(-0.65f, 0.2f, 0f), new Vector3(0.1f, 0.4f, 0.4f), wood, bench.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Leg R", restPos + new Vector3(0.65f, 0.2f, 0f), new Vector3(0.1f, 0.4f, 0.4f), wood, bench.transform);
        SceneBuilderUtil.MarkStaticRecursive(bench);

        var lanternPost = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Lantern Post", restPos + new Vector3(1.2f, 0.9f, 0.3f), new Vector3(0.08f, 0.9f, 0.08f), bark, restGo.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Lantern Glass", restPos + new Vector3(1.2f, 1.9f, 0.3f), Vector3.one * 0.25f, lanternMat, restGo.transform);
        var lanternLightGo = SceneBuilderUtil.AddPointLight("Lantern Light", restPos + new Vector3(1.2f, 1.9f, 0.3f), new Color(1f, 0.85f, 0.6f), 8f, 0.8f, restGo.transform);
        lanternPost.isStatic = true;

        var guide = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Mysterious Guide", restPos + new Vector3(-1.4f, 1f, 0.6f), new Vector3(0.6f, 1f, 0.6f), guideMat, restGo.transform);
        var invite = SceneBuilderUtil.AddLabel("Invite Label", "", restPos + new Vector3(-1.4f, 2.4f, 0.6f), 0.05f, new Color(0.95f, 0.9f, 1f), restGo.transform);

        var ambienceGo = SceneBuilderUtil.AddEmpty("Wind Ambience", restPos + Vector3.up * 1.6f, restGo.transform);
        var ambience = ambienceGo.AddComponent<AudioSource>();
        ambience.spatialBlend = 0f;   // 2D bed; the rest point lowers its volume
        ambience.loop = true;
        ambience.playOnAwake = false;

        var rest = restGo.AddComponent<RestPoint>();
        if (cam != null) rest.head = cam.transform;
        rest.lantern = lanternLightGo.GetComponent<Light>();
        rest.ambience = ambience;
        rest.inviteLabel = invite.GetComponent<TextMesh>();
        rest.restRadius = 2.0f;

        // Vignette quad under the camera ----------------------------------------------------------------------------
        Renderer vignetteRenderer = null;
        if (cam != null)
        {
            cam.nearClipPlane = Mathf.Min(cam.nearClipPlane, 0.05f);
            var vq = SceneBuilderUtil.AddPrimitive(PrimitiveType.Quad, "Vignette Quad", Vector3.zero, Vector3.one * 0.7f, vignetteMat, cam.transform);
            vq.transform.localPosition = new Vector3(0f, 0f, 0.2f);
            vq.transform.localRotation = Quaternion.identity;
            var col = vq.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
            vignetteRenderer = vq.GetComponent<Renderer>();
            vignetteRenderer.shadowCastingMode = ShadowCastingMode.Off;
            vignetteRenderer.receiveShadows = false;
        }

        // ComfortSettings on the rig ----------------------------------------------------------------------------------
        var settings = rig.AddComponent<ComfortSettings>();
        settings.origin = rig.GetComponent<XROrigin>();
        settings.teleportProvider = rig.GetComponentInChildren<TeleportationProvider>(true);
        settings.moveProvider = rig.GetComponentInChildren<ContinuousMoveProvider>(true);
        settings.snapTurnProvider = rig.GetComponentInChildren<SnapTurnProvider>(true);
        settings.smoothTurnProvider = rig.GetComponentInChildren<ContinuousTurnProvider>(true);
        settings.vignetteQuad = vignetteRenderer;

        if (Object.FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(XRUIInputModule));

        // Settings panel (left) --------------------------------------------------------------------------------------
        var sp = MakeCanvas("Settings Panel", new Vector3(-0.55f, 1.35f, 1.9f), -18f, new Vector2(720f, 620f));
        MakeText(sp, "Title", "Comfort Settings", new Vector2(0f, 275f), new Vector2(680f, 40f), 30, TextAnchor.MiddleCenter);

        MakeText(sp, "Mode Label", "Locomotion", new Vector2(-300f, 215f), new Vector2(160f, 34f), 24, TextAnchor.MiddleLeft);
        var modeGroup = new GameObject("Mode Group", typeof(RectTransform), typeof(ToggleGroup));
        modeGroup.transform.SetParent(sp, false);
        var mg = modeGroup.GetComponent<ToggleGroup>();
        var tTele = MakeToggle(sp, "Teleport Toggle", "Teleport", new Vector2(-140f, 215f), 200f, true);
        var tSmooth = MakeToggle(sp, "Smooth Toggle", "Smooth", new Vector2(40f, 215f), 180f, false);
        var tBoth = MakeToggle(sp, "Both Toggle", "Both", new Vector2(210f, 215f), 160f, false);
        tTele.group = mg; tSmooth.group = mg; tBoth.group = mg;

        MakeText(sp, "Turn Label", "Turning", new Vector2(-300f, 160f), new Vector2(160f, 34f), 24, TextAnchor.MiddleLeft);
        var turnGroup = new GameObject("Turn Group", typeof(RectTransform), typeof(ToggleGroup));
        turnGroup.transform.SetParent(sp, false);
        var tg = turnGroup.GetComponent<ToggleGroup>();
        var tSnap = MakeToggle(sp, "Snap Toggle", "Snap", new Vector2(-140f, 160f), 200f, true);
        var tSmoothTurn = MakeToggle(sp, "Smooth Turn Toggle", "Smooth turn", new Vector2(60f, 160f), 220f, false);
        tSnap.group = tg; tSmoothTurn.group = tg;

        var speedSlider = MakeSlider(sp, "Speed Slider", new Vector2(-40f, 90f), 360f, 1f, 4f, false, 1.5f);
        MakeText(sp, "Speed Label", "Move speed", new Vector2(-300f, 90f), new Vector2(170f, 34f), 24, TextAnchor.MiddleLeft);
        var speedValue = MakeText(sp, "Speed Value", "--", new Vector2(240f, 90f), new Vector2(150f, 34f), 24, TextAnchor.MiddleLeft);

        var vigSlider = MakeSlider(sp, "Vignette Slider", new Vector2(-40f, 25f), 360f, 0f, 1f, false, 0.6f);
        MakeText(sp, "Vignette Label", "Vignette", new Vector2(-300f, 25f), new Vector2(170f, 34f), 24, TextAnchor.MiddleLeft);
        var vigValue = MakeText(sp, "Vignette Value", "--", new Vector2(240f, 25f), new Vector2(150f, 34f), 24, TextAnchor.MiddleLeft);

        var heightSlider = MakeSlider(sp, "Height Slider", new Vector2(-40f, -40f), 360f, -0.3f, 0.3f, false, 0f);
        MakeText(sp, "Height Label", "Height offset", new Vector2(-300f, -40f), new Vector2(170f, 34f), 24, TextAnchor.MiddleLeft);
        var heightValue = MakeText(sp, "Height Value", "--", new Vector2(240f, -40f), new Vector2(150f, 34f), 24, TextAnchor.MiddleLeft);

        var saveBtn = MakeButton(sp, "Save Button", "Save", new Vector2(-110f, -130f), new Vector2(200f, 50f));
        var resetBtn = MakeButton(sp, "Reset Button", "Reset defaults", new Vector2(120f, -130f), new Vector2(240f, 50f));
        var settingsStatus = MakeText(sp, "Status", "Unsaved", new Vector2(0f, -200f), new Vector2(680f, 60f), 20, TextAnchor.MiddleCenter);
        MakeText(sp, "Hint", "Changes apply immediately; Save writes them to PlayerPrefs", new Vector2(0f, -260f), new Vector2(680f, 30f), 18, TextAnchor.MiddleCenter);

        var menu = sp.gameObject.AddComponent<SettingsMenu>();
        menu.settings = settings;
        menu.teleportToggle = tTele; menu.smoothToggle = tSmooth; menu.bothToggle = tBoth;
        menu.snapToggle = tSnap; menu.smoothTurnToggle = tSmoothTurn;
        menu.speedSlider = speedSlider; menu.vignetteSlider = vigSlider; menu.heightSlider = heightSlider;
        menu.speedText = speedValue; menu.vignetteText = vigValue; menu.heightText = heightValue;
        menu.saveButton = saveBtn; menu.resetButton = resetBtn; menu.statusText = settingsStatus;

        // Comfort Log panel (right) -----------------------------------------------------------------------------------
        var lp = MakeCanvas("Comfort Log Panel", new Vector3(0.55f, 1.35f, 1.9f), 18f, new Vector2(720f, 620f));
        MakeText(lp, "Title", "Comfort Log (SSQ-lite)", new Vector2(0f, 275f), new Vector2(680f, 40f), 30, TextAnchor.MiddleCenter);
        var startBtn = MakeButton(lp, "Start Button", "Start 5-min test", new Vector2(-150f, 215f), new Vector2(280f, 50f));
        var timerText = MakeText(lp, "Timer", "00:00", new Vector2(150f, 215f), new Vector2(200f, 50f), 34, TextAnchor.MiddleCenter);
        MakeText(lp, "Scale", "0 none   1 slight   2 moderate   3 severe", new Vector2(0f, 165f), new Vector2(680f, 30f), 18, TextAnchor.MiddleCenter);

        string[] questions = { "General discomfort", "Nausea", "Dizziness", "Eye strain" };
        var answerButtons = new Button[16];
        var answerTexts = new Text[4];
        for (int q = 0; q < 4; q++)
        {
            float y = 110f - q * 65f;
            MakeText(lp, "Q" + q + " Label", questions[q], new Vector2(-230f, y), new Vector2(230f, 40f), 22, TextAnchor.MiddleLeft);
            for (int v = 0; v < 4; v++)
            {
                answerButtons[q * 4 + v] = MakeButton(lp, "Q" + q + " Rating " + v, v.ToString(), new Vector2(-60f + v * 70f, y), new Vector2(56f, 46f));
            }
            answerTexts[q] = MakeText(lp, "Q" + q + " Answer", "-", new Vector2(270f, y), new Vector2(60f, 40f), 26, TextAnchor.MiddleCenter);
        }
        var submitBtn = MakeButton(lp, "Submit Button", "Submit to CSV", new Vector2(0f, -170f), new Vector2(300f, 50f));
        var logStatus = MakeText(lp, "Status", "", new Vector2(0f, -235f), new Vector2(680f, 60f), 20, TextAnchor.MiddleCenter);
        MakeText(lp, "Hint", "Keys 0-3 answer the current question on desktop", new Vector2(0f, -280f), new Vector2(680f, 30f), 18, TextAnchor.MiddleCenter);

        var log = lp.gameObject.AddComponent<ComfortLog>();
        log.settings = settings;
        log.restPoint = rest;
        log.startButton = startBtn;
        log.timerText = timerText;
        log.answerButtons = answerButtons;
        log.answerTexts = answerTexts;
        log.submitButton = submitBtn;
        log.statusText = logStatus;

        // Signs ----------------------------------------------------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "The Glade at Dusk\nActivity 5.4 - Comfort Settings and the Rest Point\nOpen README.md (Activity > Open README)",
                                  new Vector3(0f, 2.5f, 3.5f), 0.05f, Color.white);
        SceneBuilderUtil.AddLabel("Ring Sign", "Walk the ring: 1 -> 8, then back", new Vector3(0f, 2.2f, 10f), 0.05f, new Color(0.6f, 0.9f, 1f));
        SceneBuilderUtil.AddLabel("Rest Sign", "Rest point", restPos + new Vector3(0f, 2.9f, 0f), 0.05f, new Color(1f, 0.85f, 0.6f));

        SceneBuilderUtil.SaveScene(scene, "Activity_5_4");
        Selection.activeGameObject = rig;
    }

    // ------------------------------------------------------------------------------------------------------------
    // Local helpers
    // ------------------------------------------------------------------------------------------------------------

    static Font UiFont()
    {
        return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    /// <summary>A world-space canvas (1 px = 1 mm) with a dark background, turned by yawDegrees toward the player.</summary>
    static Transform MakeCanvas(string name, Vector3 position, float yawDegrees, Vector2 sizePx)
    {
        var canvasGo = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(TrackedDeviceGraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        var rt = canvasGo.GetComponent<RectTransform>();
        rt.sizeDelta = sizePx;
        rt.localScale = Vector3.one * 0.001f;
        rt.position = position;
        rt.rotation = Quaternion.Euler(0f, yawDegrees, 0f);

        var panel = new GameObject("Background", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvasGo.transform, false);
        Stretch(panel.GetComponent<RectTransform>(), 0f, 0f, 1f, 1f, Vector2.zero, Vector2.zero);
        panel.GetComponent<Image>().color = PanelColor;
        return canvasGo.transform;
    }

    static void Stretch(RectTransform rt, float minX, float minY, float maxX, float maxY, Vector2 offsetMin, Vector2 offsetMax)
    {
        rt.anchorMin = new Vector2(minX, minY);
        rt.anchorMax = new Vector2(maxX, maxY);
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
    }

    static Text MakeText(Transform parent, string name, string content, Vector2 anchoredPos, Vector2 size, int fontSize, TextAnchor align)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;
        var text = go.GetComponent<Text>();
        text.font = UiFont();
        text.fontSize = fontSize;
        text.color = TextColor;
        text.alignment = align;
        text.text = content;
        text.raycastTarget = false;
        return text;
    }

    static Button MakeButton(Transform parent, string name, string label, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;
        var img = go.GetComponent<Image>();
        img.color = ButtonColor;
        var btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        var text = MakeText(go.transform, "Text", label, Vector2.zero, size, 22, TextAnchor.MiddleCenter);
        Stretch(text.GetComponent<RectTransform>(), 0f, 0f, 1f, 1f, Vector2.zero, Vector2.zero);
        return btn;
    }

    static Slider MakeSlider(Transform parent, string name, Vector2 anchoredPos, float width, float min, float max, bool wholeNumbers, float value)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Slider));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, 40f);
        rt.anchoredPosition = anchoredPos;

        var bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(go.transform, false);
        Stretch(bg.GetComponent<RectTransform>(), 0f, 0.3f, 1f, 0.7f, Vector2.zero, Vector2.zero);
        bg.GetComponent<Image>().color = TrackColor;

        var fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(go.transform, false);
        Stretch(fillArea.GetComponent<RectTransform>(), 0f, 0.3f, 1f, 0.7f, new Vector2(10f, 0f), new Vector2(-10f, 0f));

        var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(fillArea.transform, false);
        var fillRt = fill.GetComponent<RectTransform>();
        Stretch(fillRt, 0f, 0f, 0f, 1f, Vector2.zero, Vector2.zero);
        fillRt.sizeDelta = new Vector2(10f, 0f);
        fill.GetComponent<Image>().color = FillColor;

        var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(go.transform, false);
        Stretch(handleArea.GetComponent<RectTransform>(), 0f, 0f, 1f, 1f, new Vector2(15f, 0f), new Vector2(-15f, 0f));

        var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
        handle.transform.SetParent(handleArea.transform, false);
        var handleRt = handle.GetComponent<RectTransform>();
        Stretch(handleRt, 0f, 0f, 0f, 1f, Vector2.zero, Vector2.zero);
        handleRt.sizeDelta = new Vector2(30f, 0f);
        handle.GetComponent<Image>().color = HandleColor;

        var slider = go.GetComponent<Slider>();
        slider.fillRect = fillRt;
        slider.handleRect = handleRt;
        slider.targetGraphic = handle.GetComponent<Image>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = wholeNumbers;
        slider.value = value;
        return slider;
    }

    static Toggle MakeToggle(Transform parent, string name, string label, Vector2 anchoredPos, float width, bool isOn)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Toggle));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, 40f);
        rt.anchoredPosition = anchoredPos;

        var box = new GameObject("Background", typeof(RectTransform), typeof(Image));
        box.transform.SetParent(go.transform, false);
        var boxRt = box.GetComponent<RectTransform>();
        boxRt.anchorMin = new Vector2(0f, 0.5f);
        boxRt.anchorMax = new Vector2(0f, 0.5f);
        boxRt.sizeDelta = new Vector2(34f, 34f);
        boxRt.anchoredPosition = new Vector2(17f, 0f);
        box.GetComponent<Image>().color = TrackColor;

        var check = new GameObject("Checkmark", typeof(RectTransform), typeof(Image));
        check.transform.SetParent(box.transform, false);
        Stretch(check.GetComponent<RectTransform>(), 0f, 0f, 1f, 1f, new Vector2(6f, 6f), new Vector2(-6f, -6f));
        check.GetComponent<Image>().color = FillColor;

        var text = MakeText(go.transform, "Label", label, Vector2.zero, new Vector2(width - 44f, 40f), 22, TextAnchor.MiddleLeft);
        var textRt = text.GetComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0f, 0.5f);
        textRt.anchorMax = new Vector2(0f, 0.5f);
        textRt.pivot = new Vector2(0f, 0.5f);
        textRt.anchoredPosition = new Vector2(44f, 0f);

        var toggle = go.GetComponent<Toggle>();
        toggle.targetGraphic = box.GetComponent<Image>();
        toggle.graphic = check.GetComponent<Image>();
        toggle.isOn = isOn;
        return toggle;
    }

    /// <summary>Transparent unlit material: URP/Unlit set to Transparent when a pipeline is active, else Sprites/Default.</summary>
    static Material MakeOverlayMaterial(string name, Color color)
    {
        SceneBuilderUtil.EnsureFolder(SceneBuilderUtil.MaterialsFolder);
        string path = SceneBuilderUtil.MaterialsFolder + "/" + name + ".mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null)
        {
            existing.color = color;
            EditorUtility.SetDirty(existing);
            return existing;
        }

        Shader shader = null;
        if (GraphicsSettings.currentRenderPipeline != null) shader = Shader.Find("Universal Render Pipeline/Unlit");
        Material mat;
        if (shader != null)
        {
            mat = new Material(shader);
            mat.SetFloat("_Surface", 1f);
            mat.SetFloat("_Blend", 0f);
            mat.SetFloat("_ZWrite", 0f);
            mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.SetOverrideTag("RenderType", "Transparent");
        }
        else
        {
            mat = new Material(Shader.Find("Sprites/Default"));
        }
        mat.renderQueue = (int)RenderQueue.Transparent + 50;
        mat.color = color;
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    /// <summary>
    /// Generates the radial vignette mask (clear center, black edge) and saves it as a Texture2D asset so the
    /// material can reference it. Radius 1 = middle of an edge; alpha ramps from 'inner' to 'outer'.
    /// </summary>
    static Texture2D MakeRingTextureAsset(string name, int size, float inner, float outer)
    {
        SceneBuilderUtil.EnsureFolder(SceneBuilderUtil.MaterialsFolder);
        string path = SceneBuilderUtil.MaterialsFolder + "/" + name + ".asset";
        var existing = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (existing != null) return existing;

        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        var pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float u = (x + 0.5f) / size, v = (y + 0.5f) / size;
                float r = new Vector2(u - 0.5f, v - 0.5f).magnitude * 2f;
                float a = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(inner, outer, r));
                pixels[y * size + x] = new Color(0f, 0f, 0f, a);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();
        AssetDatabase.CreateAsset(tex, path);
        return tex;
    }
}

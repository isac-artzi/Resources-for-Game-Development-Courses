// ActivitySceneBuilder.cs — Activity 5.2: Smooth Move and Turn
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: an Enchanted Forest clearing with an S-curve path through
// three arches, a world-space tuning panel (three sliders + a toggle) wired to MoveTuner, and SpeedRamp +
// SnapTurnFeedback on the rig with floating readouts.
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.UI;
using ActivityTools;

public static class ActivitySceneBuilder
{
    static readonly Color PanelColor  = new Color(0.08f, 0.10f, 0.12f, 0.85f);
    static readonly Color TrackColor  = new Color(0.25f, 0.28f, 0.32f, 1f);
    static readonly Color FillColor   = new Color(0.35f, 0.75f, 1.00f, 1f);
    static readonly Color HandleColor = new Color(0.95f, 0.95f, 1.00f, 1f);
    static readonly Color TextColor   = new Color(0.95f, 0.95f, 1.00f, 1f);

    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1f, 0.95f, 0.85f), 1.1f, new Vector3(45f, -30f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.55f, 0.70f, 0.60f), 0.015f);

        // Materials -------------------------------------------------------
        var grass   = SceneBuilderUtil.MakeMaterial("Grass",  new Color(0.25f, 0.50f, 0.20f));
        var path    = SceneBuilderUtil.MakeMaterial("Path",   new Color(0.55f, 0.48f, 0.35f));
        var bark    = SceneBuilderUtil.MakeMaterial("Bark",   new Color(0.35f, 0.25f, 0.15f));
        var leaves  = SceneBuilderUtil.MakeMaterial("Leaves", new Color(0.15f, 0.40f, 0.18f));
        var archMat = SceneBuilderUtil.MakeEmissiveMaterial("ArchStone", new Color(0.6f, 0.8f, 1f), 0.6f);
        var guideMat = SceneBuilderUtil.MakeEmissiveMaterial("Guide", new Color(0.90f, 0.85f, 1.00f), 1.2f);

        // Floor and rig ----------------------------------------------------
        SceneBuilderUtil.AddFloor(50f, grass);
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);
        Camera cam = rig.GetComponentInChildren<Camera>(true);
        Transform leftHand = FindChildByName(rig.transform, "Left Controller");

        // S-curve path: flat slabs following a sine in x as z increases, with three arches to steer through ------
        var pathRoot = SceneBuilderUtil.AddEmpty("Path", Vector3.zero);
        for (int i = 0; i < 16; i++)
        {
            float z = 2f + i * 1.5f;
            float x = Mathf.Sin(z * 0.35f) * 4f;
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Slab " + (i + 1), new Vector3(x, 0.02f, z),
                                          new Vector3(2f, 0.04f, 1.4f), path, pathRoot.transform);
        }
        int[] archIndices = { 3, 8, 13 };
        for (int a = 0; a < archIndices.Length; a++)
        {
            float z = 2f + archIndices[a] * 1.5f;
            float x = Mathf.Sin(z * 0.35f) * 4f;
            var arch = SceneBuilderUtil.AddEmpty("Arch " + (a + 1), new Vector3(x, 0f, z), pathRoot.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Post L", new Vector3(x - 1.3f, 1.25f, z), new Vector3(0.3f, 2.5f, 0.3f), archMat, arch.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Post R", new Vector3(x + 1.3f, 1.25f, z), new Vector3(0.3f, 2.5f, 0.3f), archMat, arch.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Lintel", new Vector3(x, 2.6f, z), new Vector3(2.9f, 0.3f, 0.3f), archMat, arch.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(pathRoot);

        // Ring of trees for optic flow — smooth locomotion is only felt when something passes by ----------------
        var trees = SceneBuilderUtil.AddEmpty("Trees", Vector3.zero);
        const int treeCount = 24;
        for (int i = 0; i < treeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / treeCount;
            float radius = 14f + Mathf.Sin(i * 1.7f) * 3f;
            var basePos = new Vector3(Mathf.Cos(angle) * radius, 0f, 12f + Mathf.Sin(angle) * radius);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + (i + 1), basePos + Vector3.up * 2f,
                                          new Vector3(0.5f, 2f, 0.5f), bark, trees.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + (i + 1), basePos + Vector3.up * 5f,
                                          Vector3.one * 3f, leaves, trees.transform);
        }
        // A few trees close to the path so they sweep past at walking speed.
        Vector3[] nearTrees = { new Vector3(-4f, 0f, 5f), new Vector3(6f, 0f, 9f), new Vector3(-6f, 0f, 15f), new Vector3(5f, 0f, 20f) };
        for (int i = 0; i < nearTrees.Length; i++)
        {
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Near Trunk " + (i + 1), nearTrees[i] + Vector3.up * 2f,
                                          new Vector3(0.5f, 2f, 0.5f), bark, trees.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Near Crown " + (i + 1), nearTrees[i] + Vector3.up * 5f,
                                          Vector3.one * 3f, leaves, trees.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(trees);

        // The Guide at the end of the path -------------------------------------------------------------------
        float endZ = 2f + 15 * 1.5f + 2f;
        var guide = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Mysterious Guide",
                                                  new Vector3(Mathf.Sin(endZ * 0.35f) * 4f, 1f, endZ), new Vector3(0.6f, 1f, 0.6f), guideMat);
        SceneBuilderUtil.AddPointLight("Guide Glow", guide.transform.position + Vector3.up * 0.6f, new Color(0.8f, 0.7f, 1f), 5f, 1.5f, guide.transform);
        SceneBuilderUtil.AddLabel("Guide Label", "Walk, do not lurch", guide.transform.position + Vector3.up * 1.5f, 0.06f, new Color(0.95f, 0.9f, 1f));

        // Rig scripts -------------------------------------------------------------------------------------------
        var ramp = rig.AddComponent<SpeedRamp>();
        ramp.moveProvider = rig.GetComponentInChildren<ContinuousMoveProvider>(true);
        ramp.rampSeconds = 0.3f;
        var rampLabel = SceneBuilderUtil.AddLabel("Speed Readout", "SpeedRamp: implement TODO 1-5", new Vector3(-1.6f, 1.2f, 1.6f), 0.035f, new Color(1f, 0.95f, 0.5f));
        ramp.readout = rampLabel.GetComponent<TextMesh>();

        var audio = rig.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        var snapFx = rig.AddComponent<SnapTurnFeedback>();
        snapFx.snapTurnProvider = rig.GetComponentInChildren<SnapTurnProvider>(true);
        if (cam != null) snapFx.head = cam.transform;
        var snapLabel = SceneBuilderUtil.AddLabel("Turn Readout", "SnapTurnFeedback: implement TODO 1-4", new Vector3(1.6f, 1.2f, 1.6f), 0.035f, new Color(1f, 0.95f, 0.5f));
        snapFx.readout = snapLabel.GetComponent<TextMesh>();

        // Tuning panel (world-space canvas, 1 px = 1 mm) --------------------------------------------------------
        var canvasGo = new GameObject("Tuner Panel", typeof(Canvas), typeof(CanvasScaler), typeof(TrackedDeviceGraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        var crt = canvasGo.GetComponent<RectTransform>();
        crt.sizeDelta = new Vector2(700f, 420f);
        crt.localScale = Vector3.one * 0.001f;
        crt.position = new Vector3(0f, 1.35f, 1.6f);
        if (Object.FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(XRUIInputModule));

        var panel = new GameObject("Background", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvasGo.transform, false);
        Stretch(panel.GetComponent<RectTransform>(), 0f, 0f, 1f, 1f, Vector2.zero, Vector2.zero);
        panel.GetComponent<Image>().color = PanelColor;

        MakeText(canvasGo.transform, "Title", "Locomotion Tuner", new Vector2(0f, 175f), new Vector2(660f, 40f), 30, TextAnchor.MiddleCenter);

        var speedSlider = MakeSlider(canvasGo.transform, "Speed Slider", new Vector2(-60f, 100f), 380f, 1f, 4f, false, 2f);
        MakeText(canvasGo.transform, "Speed Label", "Move speed", new Vector2(-300f, 100f), new Vector2(180f, 34f), 24, TextAnchor.MiddleLeft);
        var speedValue = MakeText(canvasGo.transform, "Speed Value", "-- m/s", new Vector2(240f, 100f), new Vector2(170f, 34f), 24, TextAnchor.MiddleLeft);

        var snapSlider = MakeSlider(canvasGo.transform, "Snap Slider", new Vector2(-60f, 30f), 380f, 0f, 2f, true, 1f);
        MakeText(canvasGo.transform, "Snap Label", "Snap turn", new Vector2(-300f, 30f), new Vector2(180f, 34f), 24, TextAnchor.MiddleLeft);
        var snapValue = MakeText(canvasGo.transform, "Snap Value", "-- deg", new Vector2(240f, 30f), new Vector2(170f, 34f), 24, TextAnchor.MiddleLeft);

        var turnSlider = MakeSlider(canvasGo.transform, "Turn Speed Slider", new Vector2(-60f, -40f), 380f, 30f, 180f, false, 60f);
        MakeText(canvasGo.transform, "Turn Label", "Smooth turn", new Vector2(-300f, -40f), new Vector2(180f, 34f), 24, TextAnchor.MiddleLeft);
        var turnValue = MakeText(canvasGo.transform, "Turn Value", "-- deg/s", new Vector2(240f, -40f), new Vector2(170f, 34f), 24, TextAnchor.MiddleLeft);

        var handToggle = MakeToggle(canvasGo.transform, "Hand Relative Toggle", "Hand-relative movement (off = head-relative)", new Vector2(-40f, -115f), false);

        MakeText(canvasGo.transform, "Hint", "Keys: - / = speed   [ snap angle   ] hand-relative",
                 new Vector2(0f, -175f), new Vector2(660f, 30f), 18, TextAnchor.MiddleCenter);

        var tuner = canvasGo.AddComponent<MoveTuner>();
        tuner.moveProvider = ramp.moveProvider;
        tuner.snapTurnProvider = snapFx.snapTurnProvider;
        tuner.smoothTurnProvider = rig.GetComponentInChildren<ContinuousTurnProvider>(true);
        tuner.speedRamp = ramp;
        tuner.speedSlider = speedSlider;
        tuner.snapSlider = snapSlider;
        tuner.turnSpeedSlider = turnSlider;
        tuner.handRelativeToggle = handToggle;
        tuner.speedText = speedValue;
        tuner.snapText = snapValue;
        tuner.turnSpeedText = turnValue;
        if (cam != null) tuner.headForward = cam.transform;
        tuner.handForward = leftHand != null ? leftHand : (cam != null ? cam.transform : null);

        // Signs ----------------------------------------------------------------------------------------------------
        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Enchanted Forest\nActivity 5.2 - Smooth Move and Turn\nOpen README.md (Activity > Open README)",
                                  new Vector3(0f, 2.4f, 3.5f), 0.05f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_5_2");
        Selection.activeGameObject = rig;
    }

    // ------------------------------------------------------------------------------------------------------------
    // Local helpers
    // ------------------------------------------------------------------------------------------------------------

    static Transform FindChildByName(Transform root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    static Font UiFont()
    {
        return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    /// <summary>Sets anchors and offsets so the rect fills (a portion of) its parent.</summary>
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

    /// <summary>A horizontal UnityEngine.UI.Slider built from RectTransforms + Images (no prefab needed).</summary>
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

    /// <summary>A UnityEngine.UI.Toggle with a square box, a checkmark fill, and a label to its right.</summary>
    static Toggle MakeToggle(Transform parent, string name, string label, Vector2 anchoredPos, bool isOn)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Toggle));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(620f, 40f);
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

        var text = MakeText(go.transform, "Label", label, new Vector2(40f, 0f), new Vector2(560f, 40f), 22, TextAnchor.MiddleLeft);
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
}

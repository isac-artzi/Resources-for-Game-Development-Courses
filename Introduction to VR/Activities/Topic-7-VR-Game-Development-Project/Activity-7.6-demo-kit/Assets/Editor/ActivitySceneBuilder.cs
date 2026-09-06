// ActivitySceneBuilder.cs — Activity 7.6: Build, Record, Present
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a demo start scene (Activity_7_6) plus two small target scenes
// (Demo_Forest, Demo_Ruins). Every scene gets its own XR rig, a world-space Demo Menu listing all three scenes,
// a Demo Tools object (ScreenshotTool) and a Timer Label on the camera (DemoTimer). All scenes go into the Scene List.
// Created by Isac Artzi

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using ActivityTools;

public static class ActivitySceneBuilder
{
    static readonly string[] SceneNames = { "Activity_7_6", "Demo_Forest", "Demo_Ruins" };
    static readonly string[] SceneLabels = { "Demo Start", "Enchanted Forest", "Ancient Ruins" };

    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        string startPath = BuildScene(0, new Color(0.30f, 0.30f, 0.38f), new Color(0.35f, 0.35f, 0.45f));
        BuildScene(1, new Color(0.22f, 0.45f, 0.20f), new Color(0.55f, 0.70f, 0.60f));
        BuildScene(2, new Color(0.62f, 0.55f, 0.42f), new Color(0.75f, 0.65f, 0.50f));

        EditorSceneManager.OpenScene(startPath);
        Selection.activeGameObject = GameObject.Find("Demo Menu");
        Debug.Log("[Activity] Built the demo start scene + 2 target scenes. Press Play and use the menu (or keys 1-3).");
    }

    static string BuildScene(int index, Color floorColor, Color fogColor)
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(Color.white, 1.0f, new Vector3(45f, -30f, 0f));
        SceneBuilderUtil.SetFog(fogColor, 0.02f);

        var floorMat = SceneBuilderUtil.MakeMaterial(SceneNames[index] + "_Floor", floorColor);
        var propMat = SceneBuilderUtil.MakeMaterial(SceneNames[index] + "_Prop", floorColor * 0.7f);
        SceneBuilderUtil.AddFloor(24f, floorMat);
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);

        // A few props so each scene is recognizable on a recording.
        var props = SceneBuilderUtil.AddEmpty("Props", Vector3.zero);
        for (int i = 0; i < 8; i++)
        {
            float a = i * Mathf.PI * 2f / 8f;
            var p = new Vector3(Mathf.Cos(a) * 8f, 0f, Mathf.Sin(a) * 8f);
            if (index == 1)
            {
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + i, p + Vector3.up * 2f, new Vector3(0.5f, 2f, 0.5f), propMat, props.transform);
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + i, p + Vector3.up * 5f, Vector3.one * 3f, floorMat, props.transform);
            }
            else if (index == 2)
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Column " + i, p + Vector3.up * 1.5f, new Vector3(0.6f, 1.5f, 0.6f), propMat, props.transform);
            else
                SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Stage Block " + i, p + Vector3.up * 0.25f, new Vector3(1f, 0.5f, 1f), propMat, props.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(props);

        // Demo Menu canvas in front of the player ------------------------------------------
        var menuGo = AddMenuCanvas(new Vector3(0f, 1.4f, 2.0f));
        var menu = menuGo.GetComponent<DemoMenu>();
        menu.sceneNames = (string[])SceneNames.Clone();
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var buttons = new Button[SceneNames.Length];
        for (int i = 0; i < SceneNames.Length; i++)
            buttons[i] = AddButton(menuGo.transform, "Button " + SceneNames[i], (i + 1) + ". " + SceneLabels[i], font, new Vector2(0f, 60f - i * 80f));
        menu.buttons = buttons;

        // Timer label + screenshot tool ------------------------------------------------------
        var cam = rig.GetComponentInChildren<Camera>(true);
        var timerGo = SceneBuilderUtil.AddLabel("Timer Label", "", Vector3.zero, 0.012f, new Color(0.8f, 0.9f, 1f));
        var confirmGo = SceneBuilderUtil.AddLabel("Capture Label", "", Vector3.zero, 0.01f, new Color(1f, 0.95f, 0.6f));
        if (cam != null)
        {
            timerGo.transform.SetParent(cam.transform, false);
            timerGo.transform.localPosition = new Vector3(0.35f, 0.32f, 1.2f);
            timerGo.transform.localRotation = Quaternion.identity;
            confirmGo.transform.SetParent(cam.transform, false);
            confirmGo.transform.localPosition = new Vector3(0f, -0.3f, 1.2f);
            confirmGo.transform.localRotation = Quaternion.identity;
        }
        var timer = timerGo.AddComponent<DemoTimer>();
        timer.label = timerGo.GetComponent<TextMesh>();
        timer.totalMinutes = 5f;

        var tools = SceneBuilderUtil.AddEmpty("Demo Tools", Vector3.zero);
        var shot = tools.AddComponent<ScreenshotTool>();
        shot.confirmLabel = confirmGo.GetComponent<TextMesh>();
        shot.hideDuringCapture = new GameObject[] { timerGo, menuGo, confirmGo };

        SceneBuilderUtil.AddLabel("Scene Sign", SceneLabels[index] + "\nActivity 7.6 - Build, Record, Present\n1-3 jump - P screenshot - T timer",
                                  new Vector3(0f, 2.6f, 5f), 0.045f, Color.white);

        return SceneBuilderUtil.SaveScene(scene, SceneNames[index]);
    }

    /// <summary>World-space canvas (1 px = 1 mm) with a title, carrying DemoMenu. Adds an XR EventSystem if needed.</summary>
    static GameObject AddMenuCanvas(Vector3 position)
    {
        var canvasGo = new GameObject("Demo Menu", typeof(Canvas), typeof(CanvasScaler), typeof(TrackedDeviceGraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        var rt = canvasGo.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(420f, 360f);
        rt.localScale = Vector3.one * 0.001f;
        rt.position = position;

        var bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(canvasGo.transform, false);
        var bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one; bgRt.offsetMin = Vector2.zero; bgRt.offsetMax = Vector2.zero;
        bg.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.1f, 0.85f);

        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        AddText(canvasGo.transform, "Title", "Realm of Legends — Demo", font, 28, new Vector2(0f, 145f), new Vector2(400f, 50f));

        canvasGo.AddComponent<DemoMenu>();

        if (Object.FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(XRUIInputModule));
        return canvasGo;
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
        return text;
    }

    static Button AddButton(Transform parent, string name, string label, Font font, Vector2 anchoredPos)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(340f, 64f);
        go.GetComponent<Image>().color = new Color(0.25f, 0.45f, 0.85f, 1f);
        AddText(go.transform, "Text", label, font, 26, Vector2.zero, new Vector2(340f, 64f));
        return go.GetComponent<Button>();
    }
}

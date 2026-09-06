// ActivitySceneBuilder.cs — Activity 2.3: Menus in Space
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a dusk clearing with drifting fireflies (something visibly moving,
// so pausing is obvious), a world-space Canvas with Start / Resume / Quit buttons that XRI's ray can press,
// an EventSystem with the XR UI input module, and the pause controller, lazy follow and button feedback scripts.
// Created by Isac Artzi

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using ActivityTools;

public static class ActivitySceneBuilder
{
    // 1 canvas pixel = 1 mm in the world. A 600 x 400 px canvas is therefore 0.6 x 0.4 m.
    const float CanvasScale = 0.001f;

    [MenuItem("Activity/Build Starter Scene", priority = 10)]
    public static void Build()
    {
        var scene = SceneBuilderUtil.NewScene();
        SceneBuilderUtil.SetSun(new Color(1.0f, 0.75f, 0.55f), 0.7f, new Vector3(12f, -40f, 0f));   // dusk
        SceneBuilderUtil.SetFog(new Color(0.30f, 0.34f, 0.45f), 0.02f);

        // Materials -------------------------------------------------------
        var grass    = SceneBuilderUtil.MakeMaterial("Grass",  new Color(0.18f, 0.35f, 0.16f));
        var bark     = SceneBuilderUtil.MakeMaterial("Bark",   new Color(0.30f, 0.22f, 0.14f));
        var leaves   = SceneBuilderUtil.MakeMaterial("Leaves", new Color(0.12f, 0.30f, 0.15f));
        var guideMat = SceneBuilderUtil.MakeEmissiveMaterial("Guide",   new Color(0.90f, 0.85f, 1.00f), 1.2f);
        var firefly  = SceneBuilderUtil.MakeEmissiveMaterial("Firefly", new Color(1.0f, 0.9f, 0.4f), 3f);

        // Floor, player, interaction manager ---------------------------------
        SceneBuilderUtil.AddFloor(30f, grass);
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);
        if (Object.FindFirstObjectByType<XRInteractionManager>() == null)
            new GameObject("XR Interaction Manager", typeof(XRInteractionManager));

        // Trees --------------------------------------------------------------
        var trees = SceneBuilderUtil.AddEmpty("Trees", Vector3.zero);
        const int treeCount = 10;
        for (int i = 0; i < treeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / treeCount + 0.15f;
            float radius = 8.5f + Mathf.Sin(i * 1.9f) * 1.5f;
            var basePos = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Trunk " + (i + 1), basePos + Vector3.up * 2f,
                                          new Vector3(0.5f, 2f, 0.5f), bark, trees.transform);
            SceneBuilderUtil.AddPrimitive(PrimitiveType.Sphere, "Crown " + (i + 1), basePos + Vector3.up * 5f,
                                          Vector3.one * 3f, leaves, trees.transform);
        }
        SceneBuilderUtil.MarkStaticRecursive(trees);

        // The Guide and a cloud of fireflies (visible motion: it freezes when Time.timeScale = 0) ----------
        var guide = SceneBuilderUtil.AddPrimitive(PrimitiveType.Capsule, "Mysterious Guide",
                                                  new Vector3(0f, 1f, 4.5f), new Vector3(0.6f, 1f, 0.6f), guideMat);
        SceneBuilderUtil.AddPointLight("Guide Glow", new Vector3(0f, 1.6f, 4.0f), new Color(0.8f, 0.7f, 1f), 6f, 1.8f,
                                       guide.transform);
        SceneBuilderUtil.AddLabel("Guide Label", "Open the spellbook when you need to breathe.",
                                  new Vector3(0f, 2.4f, 4.5f), 0.05f, new Color(0.95f, 0.9f, 1f));

        var fireflies = new GameObject("Fireflies", typeof(ParticleSystem));
        fireflies.transform.position = new Vector3(0f, 1.5f, 3f);
        var ps = fireflies.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startSize = 0.05f;
        main.startLifetime = 6f;
        main.startSpeed = 0.35f;
        main.maxParticles = 300;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        var emission = ps.emission;
        emission.rateOverTime = 40f;
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(8f, 2f, 8f);
        fireflies.GetComponent<ParticleSystemRenderer>().sharedMaterial = firefly;

        // World-space UI: Canvas + TrackedDeviceGraphicRaycaster, EventSystem + XRUIInputModule ----------
        var canvasGo = new GameObject("Menu Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(TrackedDeviceGraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        var canvasRt = canvasGo.GetComponent<RectTransform>();
        canvasRt.sizeDelta = new Vector2(600f, 400f);
        canvasRt.localScale = Vector3.one * CanvasScale;
        canvasRt.position = new Vector3(0f, 1.4f, 1.5f);   // 1.5 m ahead, a little below eye level

        var panel = MakeImage("Panel", canvasRt, new Color(0.06f, 0.08f, 0.14f, 0.88f));
        Stretch(panel.rectTransform);

        var title = MakeText("Title", canvasRt, "Realm of Legends", 48, new Color(1f, 0.92f, 0.6f));
        Place(title.rectTransform, new Vector2(0f, 140f), new Vector2(560f, 70f));
        var subtitle = MakeText("Subtitle", canvasRt, "The Guide's spellbook  -  aim and press Trigger", 22, new Color(0.8f, 0.85f, 0.95f));
        Place(subtitle.rectTransform, new Vector2(0f, 90f), new Vector2(560f, 40f));

        var startButton  = MakeButton("Start Button",  canvasRt, "Start",  new Vector2(0f,  25f));
        var resumeButton = MakeButton("Resume Button", canvasRt, "Resume", new Vector2(0f, -65f));
        var quitButton   = MakeButton("Quit Button",   canvasRt, "Quit",   new Vector2(0f, -155f));

        if (Object.FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(XRUIInputModule));

        // The canvas follows the head lazily -------------------------------------------------------------
        var follow = canvasGo.AddComponent<FollowHeadLazy>();
        follow.distance = 1.5f;
        follow.heightOffset = -0.15f;

        // Pause controller -------------------------------------------------------------------------------
        var statusLabel = SceneBuilderUtil.AddLabel("Status Label", "State: (implement PauseMenuController)  [P]",
                                                    new Vector3(0f, 2.3f, 3.0f), 0.045f, new Color(1f, 0.95f, 0.5f));
        var controller = new GameObject("Menu Controller").AddComponent<PauseMenuController>();
        controller.menuRoot = canvasGo;
        controller.startButton = startButton;
        controller.resumeButton = resumeButton;
        controller.quitButton = quitButton;
        controller.titleText = subtitle;
        controller.statusLabel = statusLabel.GetComponent<TextMesh>();

        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 2.3 - Menus in Space\nTab to a controller, aim the ray at a button, press Trigger.\nP toggles the menu.",
                                  new Vector3(0f, 2.8f, 3.0f), 0.045f, Color.white);

        SceneBuilderUtil.SaveScene(scene, "Activity_2_3");
        Selection.activeGameObject = rig;
    }

    // ------------------------------------------------------------------ UI helpers

    static Image MakeImage(string name, RectTransform parent, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = color;
        return img;
    }

    static Text MakeText(string name, RectTransform parent, string content, int fontSize, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var text = go.GetComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.raycastTarget = false;   // let the ray hit the button behind the label, not the label
        return text;
    }

    static Button MakeButton(string name, RectTransform parent, string caption, Vector2 anchoredPos)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = new Color(0.20f, 0.30f, 0.50f, 1f);
        var button = go.GetComponent<Button>();
        button.targetGraphic = img;
        button.transition = Selectable.Transition.None;   // MenuButtonFeedback owns the visuals
        Place(go.GetComponent<RectTransform>(), anchoredPos, new Vector2(360f, 80f));

        var label = MakeText("Label", go.GetComponent<RectTransform>(), caption, 36, Color.white);
        Stretch(label.rectTransform);

        var audio = go.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.ignoreListenerPause = true;   // the menu must still beep while AudioListener.pause is true
        audio.spatialBlend = 1f;            // the click comes from the button's position in space
        audio.minDistance = 0.5f;
        audio.maxDistance = 6f;

        var feedback = go.AddComponent<MenuButtonFeedback>();
        feedback.normalColor = img.color;
        feedback.hoverColor = new Color(0.35f, 0.55f, 0.85f, 1f);
        feedback.audioSource = audio;
        return button;
    }

    static void Place(RectTransform rt, Vector2 anchoredPos, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}

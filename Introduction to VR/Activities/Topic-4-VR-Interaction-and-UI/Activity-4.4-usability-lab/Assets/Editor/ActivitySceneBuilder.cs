// ActivitySceneBuilder.cs — Activity 4.4: Usability Lab
// Menu: Activity > Build Starter Scene
// Creates the starter scene for this activity: a neutral test room with three grabbable shards on a table, an altar
// with one XR socket, a task-prompt panel, a hidden five-question survey panel, and a SessionRecorder whose
// handlers are wired (as persistent listeners) to every grab and socket event in the scene.
// Created by Isac Artzi

using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
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
        SceneBuilderUtil.SetSun(new Color(1f, 0.98f, 0.95f), 1.0f, new Vector3(50f, -30f, 0f));
        SceneBuilderUtil.SetFog(new Color(0.75f, 0.78f, 0.82f), 0.01f);   // a clean, evenly lit lab: nothing to distract the participant

        // Materials -------------------------------------------------------
        var floorMat = SceneBuilderUtil.MakeMaterial("LabFloor", new Color(0.62f, 0.62f, 0.65f));
        var wallMat  = SceneBuilderUtil.MakeMaterial("LabWall",  new Color(0.85f, 0.86f, 0.88f));
        var woodMat  = SceneBuilderUtil.MakeMaterial("LabWood",  new Color(0.55f, 0.42f, 0.28f));
        var altarMat = SceneBuilderUtil.MakeMaterial("LabAltar", new Color(0.35f, 0.35f, 0.40f));
        var ringMat  = SceneBuilderUtil.MakeEmissiveMaterial("LabSocketRing", new Color(0.6f, 0.8f, 1f), 1.0f);

        // Room and player --------------------------------------------------
        SceneBuilderUtil.AddFloor(10f, floorMat);
        var rig = SceneBuilderUtil.AddXrRig(Vector3.zero);
        var playerEye = new Vector3(0f, 1.6f, 0f);

        var room = SceneBuilderUtil.AddEmpty("Room", Vector3.zero);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall Back",  new Vector3(0f, 1.5f, 4.5f),  new Vector3(9f, 3f, 0.2f), wallMat, room.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall Left",  new Vector3(-4.5f, 1.5f, 0f), new Vector3(0.2f, 3f, 9f), wallMat, room.transform);
        SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Wall Right", new Vector3(4.5f, 1.5f, 0f),  new Vector3(0.2f, 3f, 9f), wallMat, room.transform);
        SceneBuilderUtil.MarkStaticRecursive(room);

        // Recorder ----------------------------------------------------------
        var recorderGo = SceneBuilderUtil.AddEmpty("Session Recorder", Vector3.zero);
        var recorder = recorderGo.AddComponent<SessionRecorder>();
        recorder.participantId = "P1";
        recorder.designVariant = "A";

        // Table with three shards -------------------------------------------
        var table = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Shard Table", new Vector3(-0.9f, 0.4f, 1.4f), new Vector3(1.2f, 0.8f, 0.6f), woodMat);
        table.isStatic = true;
        var shards = SceneBuilderUtil.AddEmpty("Shards", Vector3.zero);
        MakeShard(shards.transform, recorder, "Blue Shard",  new Color(0.3f, 0.6f, 1.0f), new Vector3(-1.25f, 0.92f, 1.4f));
        MakeShard(shards.transform, recorder, "Red Shard",   new Color(1.0f, 0.3f, 0.3f), new Vector3(-0.90f, 0.92f, 1.4f));
        MakeShard(shards.transform, recorder, "Green Shard", new Color(0.3f, 0.9f, 0.4f), new Vector3(-0.55f, 0.92f, 1.4f));
        SceneBuilderUtil.AddLabel("Table Sign", "Shards", new Vector3(-0.9f, 1.3f, 1.7f), 0.04f, new Color(0.2f, 0.2f, 0.25f));

        // Altar with one socket ---------------------------------------------
        var altar = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, "Altar", new Vector3(1.1f, 0.45f, 1.8f), new Vector3(0.6f, 0.9f, 0.6f), altarMat);
        altar.isStatic = true;
        var socketGo = SceneBuilderUtil.AddEmpty("Altar Socket", new Vector3(1.1f, 1.0f, 1.8f), altar.transform);
        var trigger = socketGo.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.14f;
        var socket = socketGo.AddComponent<XRSocketInteractor>();
        socket.showInteractableHoverMeshes = true;
        var ring = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cylinder, "Socket Ring", new Vector3(1.1f, 0.905f, 1.8f), new Vector3(0.26f, 0.01f, 0.26f), ringMat, socketGo.transform);
        Object.DestroyImmediate(ring.GetComponent<Collider>());
        SceneBuilderUtil.AddLabel("Altar Sign", "Altar", new Vector3(1.1f, 1.4f, 2.1f), 0.04f, new Color(0.2f, 0.2f, 0.25f));

        // Wire the socket to the recorder. Persistent listeners are saved with the scene (a plain AddListener in an
        // Editor script would be lost the moment the scene is saved and reloaded).
        UnityEventTools.AddPersistentListener<SelectEnterEventArgs>(socket.selectEntered, recorder.OnSocketEnter);
        UnityEventTools.AddPersistentListener<SelectExitEventArgs>(socket.selectExited, recorder.OnSocketExit);

        // Panels -----------------------------------------------------------------
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var prompt = MakeTaskPanel(new Vector3(0f, 1.55f, 2.6f), playerEye, font);
        prompt.recorder = recorder;
        prompt.tasks = new[]
        {
            MakeTask("Pick up the BLUE shard and place it on the altar.", "Blue Shard"),
            MakeTask("Take the blue shard off the altar and put the RED shard there instead.", "Red Shard"),
            MakeTask("Now swap again: the GREEN shard goes on the altar.", "Green Shard"),
        };
        recorder.taskPrompt = prompt;

        var survey = MakeSurveyPanel(new Vector3(2.3f, 1.55f, 1.0f), playerEye, font);
        survey.recorder = recorder;
        prompt.survey = survey;

        if (Object.FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(XRUIInputModule));

        SceneBuilderUtil.AddLabel("Welcome Sign",
                                  "Activity 4.4 - Usability Lab\nModerator: set Participant Id on Session Recorder, press Play, then Start\nOpen README.md (Activity > Open README)",
                                  new Vector3(-2.6f, 2.0f, 2.6f), 0.04f, new Color(0.2f, 0.2f, 0.25f));

        SceneBuilderUtil.SaveScene(scene, "Activity_4_4");
        Selection.activeGameObject = recorderGo;
    }

    // ---------------------------------------------------------------------
    // Objects
    // ---------------------------------------------------------------------

    static GameObject MakeShard(Transform parent, SessionRecorder recorder, string name, Color color, Vector3 position)
    {
        var mat = SceneBuilderUtil.MakeEmissiveMaterial("Lab_" + name.Replace(" ", ""), color, 1.0f);
        var go = SceneBuilderUtil.AddPrimitive(PrimitiveType.Cube, name, position, new Vector3(0.10f, 0.22f, 0.10f), mat, parent);
        go.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        var rb = go.AddComponent<Rigidbody>();
        rb.mass = 0.5f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var grab = go.AddComponent<XRGrabInteractable>();
        grab.movementType = XRBaseInteractable.MovementType.Kinematic;
        grab.useDynamicAttach = true;
        grab.throwOnDetach = false;

        UnityEventTools.AddPersistentListener<SelectEnterEventArgs>(grab.selectEntered, recorder.OnGrab);
        UnityEventTools.AddPersistentListener<SelectExitEventArgs>(grab.selectExited, recorder.OnRelease);
        return go;
    }

    static TaskPrompt.Task MakeTask(string instruction, string target)
    {
        var t = new TaskPrompt.Task();
        t.instruction = instruction;
        t.targetObjectName = target;
        return t;
    }

    // ---------------------------------------------------------------------
    // UI
    // ---------------------------------------------------------------------

    static GameObject MakeCanvas(string name, Vector2 sizePx, Vector3 position, Vector3 playerEye)
    {
        var canvasGo = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(TrackedDeviceGraphicRaycaster));
        canvasGo.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        var rt = canvasGo.GetComponent<RectTransform>();
        rt.sizeDelta = sizePx;
        rt.localScale = Vector3.one * 0.001f;                  // 1 px = 1 mm
        rt.position = position;
        Vector3 away = position - playerEye; away.y = 0f;      // readable side toward the player at the origin
        if (away.sqrMagnitude > 0.001f) rt.rotation = Quaternion.LookRotation(away);
        return canvasGo;
    }

    static TaskPrompt MakeTaskPanel(Vector3 position, Vector3 playerEye, Font font)
    {
        var canvasGo = MakeCanvas("Task Panel", new Vector2(680f, 380f), position, playerEye);

        var bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(canvasGo.transform, false);
        Stretch(bg.GetComponent<RectTransform>());
        bg.GetComponent<Image>().color = new Color(0.10f, 0.11f, 0.15f, 0.92f);

        var instruction = MakeText(bg.transform, "Instruction", "", font, 30, TextAnchor.UpperLeft, Color.white);
        var instRt = instruction.GetComponent<RectTransform>();
        instRt.anchorMin = new Vector2(0f, 0f); instRt.anchorMax = new Vector2(1f, 1f);
        instRt.offsetMin = new Vector2(28f, 130f); instRt.offsetMax = new Vector2(-28f, -24f);

        var status = MakeText(bg.transform, "Status", "", font, 26, TextAnchor.MiddleLeft, new Color(1f, 0.85f, 0.5f));
        var statRt = status.GetComponent<RectTransform>();
        statRt.anchorMin = new Vector2(0f, 0f); statRt.anchorMax = new Vector2(0.6f, 0f); statRt.pivot = new Vector2(0f, 0f);
        statRt.anchoredPosition = new Vector2(28f, 30f); statRt.sizeDelta = new Vector2(0f, 60f);

        var start = MakeButton(bg.transform, "Start Button", "Start", font, new Vector2(0.66f, 0.07f), new Vector2(0.96f, 0.26f));

        var prompt = canvasGo.AddComponent<TaskPrompt>();
        prompt.instructionText = instruction.GetComponent<Text>();
        prompt.statusText = status.GetComponent<Text>();
        prompt.startButton = start.GetComponent<Button>();
        prompt.startButtonText = start.GetComponentInChildren<Text>();
        return prompt;
    }

    static QuickSurvey MakeSurveyPanel(Vector3 position, Vector3 playerEye, Font font)
    {
        var canvasGo = MakeCanvas("Survey Panel", new Vector2(960f, 640f), position, playerEye);

        var bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(canvasGo.transform, false);
        Stretch(bg.GetComponent<RectTransform>());
        bg.GetComponent<Image>().color = new Color(0.10f, 0.11f, 0.15f, 0.92f);

        var title = MakeText(bg.transform, "Title", "Quick survey    1 = strongly disagree ... 5 = strongly agree", font, 26, TextAnchor.MiddleLeft, new Color(1f, 0.9f, 0.6f));
        var titleRt = title.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0f, 1f); titleRt.anchorMax = new Vector2(1f, 1f); titleRt.pivot = new Vector2(0.5f, 1f);
        titleRt.anchoredPosition = new Vector2(0f, -16f); titleRt.sizeDelta = new Vector2(-48f, 44f);

        const int rows = 5;
        var questionTexts = new Text[rows];
        var buttons = new Button[rows * 5];
        float rowHeight = 84f;
        float firstRowY = -90f;
        for (int r = 0; r < rows; r++)
        {
            float y = firstRowY - r * rowHeight;

            var q = MakeText(bg.transform, "Question " + (r + 1), "", font, 24, TextAnchor.MiddleLeft, Color.white);
            var qRt = q.GetComponent<RectTransform>();
            qRt.anchorMin = new Vector2(0f, 1f); qRt.anchorMax = new Vector2(0f, 1f); qRt.pivot = new Vector2(0f, 0.5f);
            qRt.anchoredPosition = new Vector2(28f, y); qRt.sizeDelta = new Vector2(540f, rowHeight - 12f);
            questionTexts[r] = q.GetComponent<Text>();

            for (int v = 1; v <= 5; v++)
            {
                var b = new GameObject("Answer " + (r + 1) + "-" + v, typeof(RectTransform), typeof(Image), typeof(Button));
                b.transform.SetParent(bg.transform, false);
                var bRt = b.GetComponent<RectTransform>();
                bRt.anchorMin = new Vector2(0f, 1f); bRt.anchorMax = new Vector2(0f, 1f); bRt.pivot = new Vector2(0f, 0.5f);
                bRt.anchoredPosition = new Vector2(590f + (v - 1) * 70f, y); bRt.sizeDelta = new Vector2(60f, 60f);
                var img = b.GetComponent<Image>();
                img.color = new Color(0.25f, 0.30f, 0.45f, 1f);
                b.GetComponent<Button>().targetGraphic = img;
                var label = MakeText(b.transform, "Text", v.ToString(), font, 26, TextAnchor.MiddleCenter, Color.white);
                Stretch(label.GetComponent<RectTransform>());
                buttons[r * 5 + (v - 1)] = b.GetComponent<Button>();
            }
        }

        var submit = MakeButton(bg.transform, "Submit Button", "Submit", font, new Vector2(0.72f, 0.04f), new Vector2(0.96f, 0.15f));

        var footer = MakeText(bg.transform, "Footer", "", font, 22, TextAnchor.MiddleLeft, new Color(1f, 0.85f, 0.5f));
        var fRt = footer.GetComponent<RectTransform>();
        fRt.anchorMin = new Vector2(0f, 0f); fRt.anchorMax = new Vector2(0.7f, 0f); fRt.pivot = new Vector2(0f, 0f);
        fRt.anchoredPosition = new Vector2(28f, 20f); fRt.sizeDelta = new Vector2(0f, 60f);

        var survey = canvasGo.AddComponent<QuickSurvey>();
        survey.panelRoot = bg;
        survey.questionTexts = questionTexts;
        survey.answerButtons = buttons;
        survey.submitButton = submit.GetComponent<Button>();
        survey.footerText = footer.GetComponent<Text>();
        return survey;
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
        var text = MakeText(go.transform, "Text", label, font, 26, TextAnchor.MiddleCenter, Color.white);
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

// DialogueRunner.cs — Activity 4.2: Speak with the Sage: NPC Dialogue
// Drives one conversation: shows a node's text with a typewriter reveal, labels the two choice buttons,
// follows the player's choice to the next node, and keeps the dialogue box turned toward the player.
// Attached to: "Sage Dialogue" (a world-space Canvas above the Sage) in the starter scene. The nodes array
// is filled in by the builder; edit the words there in the Inspector.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueRunner : MonoBehaviour
{
    [Header("Conversation")]
    [Tooltip("The nodes of this conversation. Index 0 is the opening line. Edit text and branches here.")]
    public DialogueNode[] nodes;

    [Tooltip("Name shown in the title bar of the box.")]
    public string speakerName = "Forest Sage";

    [Header("UI references (set by the builder)")]
    [Tooltip("The panel root to show/hide. Usually the Background object under this canvas.")]
    public GameObject panelRoot;

    [Tooltip("Speaker name text.")]
    public Text nameText;

    [Tooltip("Where the node's text is revealed.")]
    public Text bodyText;

    [Tooltip("Left choice button and its label.")]
    public Button choiceAButton;
    public Text choiceAText;

    [Tooltip("Right choice button and its label.")]
    public Button choiceBButton;
    public Text choiceBText;

    [Header("Typewriter")]
    [Tooltip("Characters revealed per second. 35-45 reads as 'speaking'; comfortable silent reading is about 15-20.")]
    public float charsPerSecond = 40f;

    [Header("Facing")]
    [Tooltip("The player's head (XR camera). If empty, Camera.main is used.")]
    public Transform head;

    /// <summary>True while a conversation is on screen.</summary>
    public bool IsOpen { get; private set; }

    /// <summary>Index of the node being shown, or -1 when closed.</summary>
    public int CurrentIndex { get; private set; }

    string fullText = "";      // the complete text of the current node
    float revealTimer;         // seconds since the current node started
    int shownChars;            // how many characters are visible right now

    void Start()
    {
        if (head == null && Camera.main != null) head = Camera.main.transform;
        if (choiceAButton != null) choiceAButton.onClick.AddListener(() => Choose(0));
        if (choiceBButton != null) choiceBButton.onClick.AddListener(() => Choose(1));
        CurrentIndex = -1;
        SetVisible(false);
    }

    /// <summary>Starts the conversation from node 0. Called by NpcTalkTrigger.</summary>
    public void Open()
    {
        if (nodes == null || nodes.Length == 0)
        {
            Debug.LogWarning("[DialogueRunner] No nodes to show.");
            return;
        }
        IsOpen = true;
        SetVisible(true);
        ShowNode(0);
    }

    /// <summary>Ends the conversation and hides the box. Called by NpcTalkTrigger or by a -1 branch.</summary>
    public void Close()
    {
        IsOpen = false;
        CurrentIndex = -1;
        SetVisible(false);
    }

    /// <summary>Displays the given node: resets the typewriter and labels the buttons.</summary>
    public void ShowNode(int index)
    {
        if (nodes == null || index < 0 || index >= nodes.Length) { Close(); return; }
        CurrentIndex = index;
        DialogueNode node = nodes[index];

        // TODO 1: Prepare the text for the typewriter and label the choices.
        //         1. fullText = node.text;  revealTimer = 0f;  shownChars = 0;  bodyText.text = "";
        //         2. nameText.text = speakerName;
        //         3. choiceAText.text = node.choiceA;
        //         4. choiceBText.text = node.choiceB;  choiceBButton.gameObject.SetActive(node.HasChoiceB);
        //         Why clear bodyText: the reveal in Update() rebuilds it from fullText each frame.
        //         Look at: GameObject.SetActive, Text.text.
        //         Check: when the box opens, the buttons read "I seek the artifact shards." and "I lost my way.",
        //         and the body is empty for a moment before the letters start appearing (TODO 2).
        if (bodyText != null) bodyText.text = node.text;   // placeholder: shows everything at once until TODO 1-2 are done
    }

    void Update()
    {
        if (!IsOpen) return;

        // TODO 2: Typewriter reveal. Advance revealTimer by Time.deltaTime, then
        //             int target = Mathf.FloorToInt(revealTimer * charsPerSecond);
        //             shownChars = Mathf.Min(target, fullText.Length);
        //             bodyText.text = fullText.Substring(0, shownChars);
        //         Why time-based and not "one char per frame": 72 Hz vs 144 Hz would otherwise double the speed.
        //         Look at: string.Substring(int, int), Mathf.FloorToInt, Mathf.Min.
        //         Check: a 120-character line finishes in about 3 s at 40 chars/s. Set Chars Per Second to 10 and
        //         it crawls; to 1000 and it appears at once.

        // TODO 3: Desktop fallback so you can test without aiming the ray: keys 1 and 2 choose A and B.
        //             var kb = Keyboard.current;
        //             if (kb == null) return;
        //             bool pressed1 = kb.digit1Key.wasPressedThisFrame;
        //             bool pressed2 = kb.digit2Key.wasPressedThisFrame;
        //             if (!pressed1 && !pressed2) return;
        //             if (shownChars < fullText.Length) { revealTimer = 1000f; return; }   // first press: finish the line
        //             if (pressed1) Choose(0);
        //             else if (nodes[CurrentIndex].HasChoiceB) Choose(1);
        //         Look at: UnityEngine.InputSystem.Keyboard.current (the new Input System; never the legacy Input class).
        //         Check: press 1 while the Sage is talking — the line completes; press 1 again — the branch follows.
    }

    /// <summary>Follows choice 0 (A) or 1 (B) from the current node.</summary>
    public void Choose(int choiceIndex)
    {
        if (!IsOpen || nodes == null || CurrentIndex < 0 || CurrentIndex >= nodes.Length) return;

        // TODO 4: Branch.
        //             int next = nodes[CurrentIndex].Next(choiceIndex, nodes.Length);
        //             if (next < 0) Close(); else ShowNode(next);
        //         Look at: DialogueNode.Next (you wrote its validation in DialogueNode.cs TODO 2).
        //         Check: "I lost my way." leads to the lantern line; "Farewell." closes the box. Walk out of range
        //         and back in and the conversation restarts at node 0.
    }

    void LateUpdate()
    {
        if (!IsOpen || head == null) return;

        // TODO 5: Keep the box facing the player, upright. A world-space Canvas is readable from the side its
        //         forward (+z) points AWAY from, exactly like a TextMesh:
        //             Vector3 away = transform.position - head.position;  away.y = 0f;
        //             if (away.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(away);
        //         Why yaw only: a box that tilts toward your eyes as you approach reads as "falling on you"; a
        //         box that stays upright reads as a sign.
        //         Look at: Quaternion.LookRotation.
        //         Check: circle the Sage with the head selected — the box turns to face you and the text is never mirrored.
    }

    void SetVisible(bool visible)
    {
        if (panelRoot != null) panelRoot.SetActive(visible);
    }
}

// DialogueNode.cs — Activity 4.2: Speak with the Sage: NPC Dialogue
// One line of NPC dialogue plus up to two player choices, each pointing at the index of the next node.
// Not a MonoBehaviour: DialogueRunner holds an array of these and you edit them in its Inspector.
// An index of -1 means "end the conversation".
// Created by Isac Artzi

using UnityEngine;

/// <summary>
/// A node in a branching conversation. Nodes live in DialogueRunner.nodes; choices refer to other
/// nodes by array index, which keeps the data flat and Inspector-friendly.
/// </summary>
[System.Serializable]
public class DialogueNode
{
    [Tooltip("What the NPC says. Keep it to 2-3 short sentences: VR readers are slow and the box is small.")]
    [TextArea(2, 5)]
    public string text = "...";

    [Header("Choice A (left button)")]
    [Tooltip("Label on the first button, e.g. 'I seek the shards.'")]
    public string choiceA = "Continue";

    [Tooltip("Index of the node to show after choice A. -1 ends the conversation.")]
    public int nextA = -1;

    [Header("Choice B (right button) — leave the label empty for a single-choice node")]
    [Tooltip("Label on the second button. Empty = the second button is hidden.")]
    public string choiceB = "";

    [Tooltip("Index of the node to show after choice B. -1 ends the conversation.")]
    public int nextB = -1;

    /// <summary>True when this node offers a second choice (its label is not empty).</summary>
    public bool HasChoiceB
    {
        get
        {
            // TODO 1: Return true only if choiceB has text: !string.IsNullOrWhiteSpace(choiceB).
            //         Why: a node with one way forward should show ONE button; an empty second button is a
            //         broken affordance (it invites a click that does nothing).
            //         Check: the Sage's farewell node shows a single centered "Farewell." button.
            return true;
        }
    }

    /// <summary>
    /// The index of the node that follows the given choice (0 = A, 1 = B), or -1 to end.
    /// Guards against out-of-range indices so a typo in the Inspector cannot crash the runner.
    /// </summary>
    public int Next(int choiceIndex, int nodeCount)
    {
        // TODO 2: Pick nextA for choiceIndex 0 and nextB for 1; anything else returns -1.
        //         Then validate: if the picked index is >= nodeCount, Debug.LogWarning about it and return -1.
        //         Why validate here: every branch passes through this one method, so one check protects them all.
        //         Look at: Debug.LogWarning, comparison operators.
        //         Check: set a node's Next A to 99 in the Inspector; choosing it ends the dialogue with a warning
        //         in the Console instead of an IndexOutOfRangeException.
        return -1;
    }
}

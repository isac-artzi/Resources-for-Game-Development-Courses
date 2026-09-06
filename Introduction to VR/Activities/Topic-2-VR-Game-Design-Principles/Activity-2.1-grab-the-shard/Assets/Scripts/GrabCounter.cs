// GrabCounter.cs — Activity 2.1: Grab the Shard
// Reads every socket on the altar, prints "2/3 placed" on a floating label, and wakes the Guide when all three
// shards are seated. This is the first "quest step complete" logic of Realm of Legends.
// Attached to: Shard Counter (the TextMesh above the altar). Sockets and the Guide's glow are pre-wired.
// Created by Isac Artzi

using UnityEngine;

public class GrabCounter : MonoBehaviour
{
    [Header("What to count")]
    [Tooltip("Every socket on the altar. The starter scene fills this with the three Shard Sockets.")]
    public ShardSocketCheck[] sockets;

    [Header("Where to show it")]
    [Tooltip("The TextMesh that shows 'n/3 placed'. Pre-set to the TextMesh on this GameObject.")]
    public TextMesh label;

    [Tooltip("Word after the fraction, e.g. 'placed' or 'bound'.")]
    public string verb = "placed";

    [Header("On completion")]
    [Tooltip("Turned on once every socket holds a shard. The starter scene points this at the Guide's glow light.")]
    public GameObject revealOnComplete;

    /// <summary>How many sockets currently hold a correctly tagged shard.</summary>
    public int PlacedCount { get; private set; }

    /// <summary>True once every socket has been filled at least once this session.</summary>
    public bool IsComplete { get; private set; }

    void Start()
    {
        if (label == null) label = GetComponent<TextMesh>();
        if (revealOnComplete != null) revealOnComplete.SetActive(false);
    }

    void Update()
    {
        if (sockets == null || sockets.Length == 0) return;

        // TODO 1: Count. Loop over sockets; for each non-null socket whose HasShard is true, add one.
        //         Store the result in PlacedCount (the setter is private but this class may write it).
        //         Why poll every frame instead of subscribing to events: three booleans are cheap to read, and polling
        //         cannot get out of sync if a shard is pulled out again. Events are the right tool when work is expensive.
        //         Check: watch PlacedCount in the Inspector (Debug mode) go 0 -> 1 -> 2 as you seat shards.

        // TODO 2: Show it. Build the text with Format(PlacedCount, sockets.Length, verb) — implement Format below —
        //         and assign it to label.text. Only assign when the text actually changed if you want to be tidy.
        //         Check: the label above the altar reads "0/3 placed", then "1/3 placed", ...

        // TODO 3: Complete once. When PlacedCount == sockets.Length and IsComplete is still false:
        //             IsComplete = true; if (revealOnComplete != null) revealOnComplete.SetActive(true);
        //             Debug.Log("The Guide awakens.");
        //         Why the flag: without it the code above would run every frame for the rest of the session.
        //         Check: seat the third shard -> the Guide's purple glow switches on and stays on.
    }

    /// <summary>Formats the progress line, e.g. Format(2, 3, "placed") returns "2/3 placed".</summary>
    public static string Format(int placed, int total, string verb)
    {
        // TODO 2 (continued): return placed + "/" + total + " " + verb;
        //         Check: Format(3, 3, "bound") == "3/3 bound".
        return "?/? " + verb;
    }
}

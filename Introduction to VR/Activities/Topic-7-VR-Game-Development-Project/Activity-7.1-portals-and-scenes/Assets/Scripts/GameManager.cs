// GameManager.cs — Activity 7.1: Portals and Scene Flow
// The one object that survives every scene change. It remembers which artifact pieces the hero has
// collected and keeps the "Status Sign" label in whatever scene is currently loaded up to date.
// Attached to: "Game Systems" in the Hub scene (Activity_7_1), together with SceneLoader.
// If a stub scene is played directly, GameManager.Instance creates itself so Portals still work.
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SceneLoader))]
public class GameManager : MonoBehaviour
{
    [Header("Progress")]
    [Tooltip("Ids of the artifact pieces collected so far. Filled at runtime; shown here so you can watch it in the Inspector.")]
    public List<string> collectedPieces = new List<string>();

    [Tooltip("How many pieces exist in the whole game. Used for the 'x / y' readout.")]
    public int totalPieces = 3;

    [Header("Status label")]
    [Tooltip("Name of the TextMesh GameObject the manager looks for in every scene after it loads.")]
    public string statusLabelName = "Status Sign";

    static GameManager instance;

    /// <summary>
    /// The single GameManager. Finds an existing one, or creates a bare "Game Systems (auto)" object so that
    /// a stub scene played on its own still has a manager and a SceneLoader.
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Object.FindFirstObjectByType<GameManager>();
                if (instance == null)
                {
                    var go = new GameObject("Game Systems (auto)");
                    instance = go.AddComponent<GameManager>();   // RequireComponent also adds a SceneLoader
                }
            }
            return instance;
        }
    }

    /// <summary>The SceneLoader that lives on the same GameObject.</summary>
    public SceneLoader Loader
    {
        get { return GetComponent<SceneLoader>(); }
    }

    void Awake()
    {
        // TODO 1: Make this a persistent singleton.
        //         - If 'instance' is not null and is not this, another GameManager already exists (this happens
        //           the moment you come BACK to the Hub, because the Hub scene contains its own "Game Systems").
        //           Destroy(gameObject) and return, so the older one keeps its collected pieces.
        //         - Otherwise set instance = this and call DontDestroyOnLoad(gameObject).
        //         Look at: Object.DontDestroyOnLoad — moves the object to a special scene that survives LoadScene.
        //         Why: scene loading destroys every object in the old scene. Anything that must remember state
        //         (progress, settings, the loader itself) has to live outside the scenes.
        //         Check: in Play mode, walk through a portal. In the Hierarchy a "DontDestroyOnLoad" scene appears
        //         holding Game Systems. Walk back to the Hub: there is still exactly ONE Game Systems object.
        instance = this;   // placeholder so the scene works before TODO 1 (but duplicates will appear)
    }

    void OnEnable()
    {
        // TODO 3: Subscribe to SceneManager.sceneLoaded so the label in the new scene gets refreshed:
        //             SceneManager.sceneLoaded += OnSceneLoaded;
        //         and unsubscribe in OnDisable (SceneManager.sceneLoaded -= OnSceneLoaded).
        //         Why: the label is a per-scene object; the manager is not. Each new scene brings a new,
        //         blank label, and only the manager knows the real count.
        //         Check: collect the Forest piece, return to the Hub — the Hub's sign says "1 / 3".
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshStatusLabel();
    }

    /// <summary>Records a collected piece. Returns true if it was new.</summary>
    public bool Collect(string pieceId)
    {
        // TODO 2: If collectedPieces already contains pieceId, return false (do not count it twice).
        //         Otherwise add it, log "[GameManager] collected <id>", call RefreshStatusLabel() and return true.
        //         Look at: List<T>.Contains, List<T>.Add.
        //         Check: walk into the Forest piece twice (leave and re-enter its trigger) — the count stays at 1.
        Debug.Log("[GameManager] Collect() called for " + pieceId + " — implement TODO 2");
        return false;
    }

    /// <summary>True if the piece with this id has been collected.</summary>
    public bool HasPiece(string pieceId)
    {
        return collectedPieces.Contains(pieceId);
    }

    /// <summary>One-line summary such as "Artifact pieces: 1 / 3".</summary>
    public string Summary()
    {
        return "Artifact pieces: " + collectedPieces.Count + " / " + totalPieces;
    }

    /// <summary>Finds the status label in the current scene (by name) and writes the summary into it.</summary>
    public void RefreshStatusLabel()
    {
        // TODO 4: Find the label. GameObject.Find(statusLabelName) returns null if this scene has no sign —
        //         that is fine, just return. Otherwise GetComponent<TextMesh>() and set .text = Summary()
        //         plus a second line naming the active scene: SceneManager.GetActiveScene().name.
        //         Look at: GameObject.Find (slow — fine once per scene load, never every frame), TextMesh.text.
        //         Check: the sign in every scene shows the same count and the correct scene name.
    }
}

// SceneLoader.cs — Activity 7.1: Portals and Scene Flow
// Loads a scene the comfortable way for VR: fade the view to black, load asynchronously (never freezing
// the head tracking), then fade back in once the new scene is active.
// Attached to: "Game Systems" next to GameManager (it travels with it between scenes).
// The fade is a black quad parented to the XR camera, named "Fade Quad"; every scene the builder makes has one.
// Created by Isac Artzi

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Fade")]
    [Tooltip("Seconds for the fade to black and again for the fade back in. 0.3–0.5 s feels like a blink, not a wait.")]
    public float fadeSeconds = 0.4f;

    [Tooltip("Name of the quad in front of the camera that this script darkens. Each scene has its own.")]
    public string fadeQuadName = "Fade Quad";

    [Tooltip("The renderer of the fade quad in the CURRENT scene. Re-found after every load.")]
    public Renderer fadeRenderer;

    [Header("Read-only status")]
    [Tooltip("True while a load is in progress. Portals ignore triggers while this is true.")]
    public bool isLoading;

    [Tooltip("0..1 progress of the current load, for a loading label or bar.")]
    [Range(0f, 1f)] public float progress;

    void Start()
    {
        // The first scene also deserves a fade-in: start black and clear. Works once TODO 1 is done.
        FindFadeRenderer();
        SetFadeAlpha(1f);
        StartCoroutine(Fade(1f, 0f));
    }

    /// <summary>Begins a fade + async load of the named scene. Ignored if a load is already running.</summary>
    public void LoadScene(string sceneName)
    {
        if (isLoading) return;
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SceneLoader] No scene name given.");
            return;
        }
        StartCoroutine(LoadRoutine(sceneName));
    }

    IEnumerator LoadRoutine(string sceneName)
    {
        isLoading = true;
        progress = 0f;

        // TODO 2: Fade to black before anything else: yield return Fade(0f, 1f);
        //         Why: the player never sees the world snap or freeze — they see a blink.

        // TODO 3: Start the load without letting Unity switch scenes yet:
        //             AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        //             op.allowSceneActivation = false;
        //             while (op.progress < 0.9f) { progress = op.progress / 0.9f; yield return null; }
        //         Look at: AsyncOperation.progress (it stops at 0.9 until activation), allowSceneActivation.
        //         Why hold activation: so you decide the exact moment the world changes — while the view is black.
        //         Then: op.allowSceneActivation = true; while (!op.isDone) yield return null;
        //         Check: the Console shows no "scene not in build settings" error; the Hierarchy switches scenes.
        Debug.Log("[SceneLoader] Would load " + sceneName + " here — implement TODO 3 (LoadSceneAsync).");
        yield return null;

        // TODO 4: The old Fade Quad died with the old scene (the rig is per-scene). Re-find the new one:
        //             FindFadeRenderer(); SetFadeAlpha(1f);
        //         then fade back in: yield return Fade(1f, 0f);
        //         Check: arriving in the Forest you see black first, then the forest fades in over ~0.4 s.

        progress = 1f;
        isLoading = false;
    }

    /// <summary>Lerps the fade quad's alpha from 'from' to 'to' over fadeSeconds.</summary>
    public IEnumerator Fade(float from, float to)
    {
        // TODO 1: Animate the alpha. Keep a float t = 0; each frame add Time.unscaledDeltaTime / fadeSeconds,
        //         set SetFadeAlpha(Mathf.Lerp(from, to, t)) and yield return null until t >= 1, then set exactly 'to'.
        //         Look at: Mathf.Lerp, Time.unscaledDeltaTime (so a paused Time.timeScale cannot trap you in black).
        //         Check: press Play in the Hub — the world fades in from black instead of popping.
        SetFadeAlpha(to);
        yield return null;
    }

    /// <summary>Writes the alpha into the fade material's color (uses .material, i.e. an instance, not the asset).</summary>
    public void SetFadeAlpha(float alpha)
    {
        if (fadeRenderer == null) return;
        var c = fadeRenderer.material.color;   // .material = per-renderer instance; .sharedMaterial would edit the asset
        c.a = Mathf.Clamp01(alpha);
        fadeRenderer.material.color = c;
    }

    /// <summary>Looks for the fade quad in the current scene by name.</summary>
    public void FindFadeRenderer()
    {
        var go = GameObject.Find(fadeQuadName);
        fadeRenderer = go != null ? go.GetComponent<Renderer>() : null;
        if (fadeRenderer == null)
            Debug.LogWarning("[SceneLoader] No '" + fadeQuadName + "' in scene " + SceneManager.GetActiveScene().name +
                             " — portals will still work, but without a fade.");
    }
}

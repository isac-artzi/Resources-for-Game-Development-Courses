// TeleportFade.cs — Activity 5.1: Teleport, Properly
// A "blink": the moment the TeleportationProvider moves the rig, the view cuts to black, holds for a
// few frames, and fades back in at the destination. The player's eyes read it as their own blink,
// which is far more comfortable than an instant cut from one place to another.
// Attached to: Teleport Fade (a black quad parented under the Main Camera of the XR Origin).
// Created by Isac Artzi

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class TeleportFade : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("The TeleportationProvider on the rig's Locomotion object. Leave empty to find it at runtime.")]
    public TeleportationProvider provider;

    [Tooltip("Renderer of the black quad in front of the camera. The starter scene assigns the one on this GameObject.")]
    public Renderer fadePanel;

    [Header("Timing (seconds)")]
    [Tooltip("How long the view stays fully black after the teleport before fading back in. 0.05–0.10 feels like a blink.")]
    public float holdSeconds = 0.08f;

    [Tooltip("How long the fade from black back to clear takes. 0.10–0.20 is the comfortable range.")]
    public float fadeInSeconds = 0.15f;

    [Header("Debug")]
    [Tooltip("How many teleports this component has faded since Play started (read-only).")]
    public int teleportCount;

    Material fadeMaterial;
    Coroutine running;

    void Start()
    {
        if (fadePanel == null) fadePanel = GetComponent<Renderer>();
        if (fadePanel != null)
        {
            // .material (not .sharedMaterial) gives this renderer its own copy, so changing alpha here
            // does not tint every other object that uses the same asset.
            fadeMaterial = fadePanel.material;
            SetAlpha(0f);
        }

        // TODO 1: Find the provider if none was assigned in the Inspector.
        //         Look at: Object.FindFirstObjectByType<TeleportationProvider>() (it lives on the rig's
        //         "Locomotion" child). If it is still null, Debug.LogWarning and return — nothing else to do.
        //         Why at runtime: the rig is a prefab instance; finding it here is more robust than a hand-set reference.
        //         Check: no warning in the Console when you press Play.

        // TODO 2: Subscribe to the provider's C# events (these are plain events, not UnityEvents):
        //             provider.locomotionStarted += OnLocomotionStarted;
        //             provider.locomotionEnded   += OnLocomotionEnded;
        //         Both have the signature void Handler(LocomotionProvider p).
        //         Look at: LocomotionProvider.locomotionStarted / locomotionEnded (Unity Manual → XR Interaction Toolkit → Locomotion).
        //         Check: Debug.Log inside each handler prints once per teleport.
    }

    void OnDestroy()
    {
        // TODO 3: Unsubscribe (provider.locomotionStarted -= ...; provider.locomotionEnded -= ...) if provider != null.
        //         Why: a destroyed MonoBehaviour that is still subscribed throws MissingReferenceException the next
        //         time the event fires — you will see this when you reload the scene in Play mode.
    }

    /// <summary>Called by the provider the frame it moves the rig. For a teleport this is instantaneous.</summary>
    void OnLocomotionStarted(LocomotionProvider p)
    {
        // TODO 4: Cut to black immediately: SetAlpha(1f). If a fade-in coroutine is still running from a previous
        //         teleport, stop it first (StopCoroutine(running)) so two fades do not fight over the alpha.
        //         Increment teleportCount.
        //         Check: the frame you land, the view is fully black (pause Play right after a teleport to confirm).
    }

    /// <summary>Called by the provider when the move is complete. Starts the hold + fade-in.</summary>
    void OnLocomotionEnded(LocomotionProvider p)
    {
        // TODO 5: running = StartCoroutine(FadeIn());
        //         Check: after each teleport you see black, then the new spot fades in over about 0.15 s.
    }

    IEnumerator FadeIn()
    {
        // TODO 6: Wait holdSeconds (yield return new WaitForSeconds(holdSeconds)), then loop for fadeInSeconds:
        //             float t = 0f;
        //             while (t < fadeInSeconds) { t += Time.deltaTime; SetAlpha(1f - Mathf.Clamp01(t / fadeInSeconds)); yield return null; }
        //             SetAlpha(0f);
        //         Look at: WaitForSeconds, Time.deltaTime, Mathf.Clamp01.
        //         Why a coroutine: it spreads the fade over many frames without blocking Update.
        //         Check: set fadeInSeconds to 2 temporarily and watch the fade clearly; then put it back to 0.15.
        yield return null;
    }

    /// <summary>Sets the alpha of the fade quad's color (0 = clear, 1 = fully black).</summary>
    public void SetAlpha(float alpha)
    {
        if (fadeMaterial == null) return;
        var c = fadeMaterial.color;
        c.a = Mathf.Clamp01(alpha);
        fadeMaterial.color = c;
    }
}

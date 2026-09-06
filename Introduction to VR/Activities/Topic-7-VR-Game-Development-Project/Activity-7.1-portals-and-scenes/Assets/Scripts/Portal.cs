// Portal.cs — Activity 7.1: Portals and Scene Flow
// A doorway between worlds. When the player's head enters the trigger volume, the portal asks the
// persistent SceneLoader to fade out and load the target scene.
// Attached to: each "Portal ..." trigger volume in the Hub and each "Return Portal" in the stub scenes.
// The head is detected through the "Head Probe" (a small kinematic trigger sphere the builder parents to the camera).
// Created by Isac Artzi

using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Destination")]
    [Tooltip("Exact scene name to load. It must be in File > Build Profiles > Scene List (the builder adds it).")]
    public string targetScene = "Forest_Stub";

    [Tooltip("Shown in the Console and usable for a label, e.g. 'Enchanted Forest'.")]
    public string displayName = "Enchanted Forest";

    [Header("Behavior")]
    [Tooltip("Seconds after the scene starts before this portal may fire. Stops an instant bounce-back on arrival.")]
    public float armDelay = 1.0f;

    [Tooltip("Optional: a renderer to pulse while the portal is armed (the shimmering surface).")]
    public Renderer surface;

    float armedAt;

    void Start()
    {
        armedAt = Time.time + armDelay;
    }

    void Update()
    {
        // TODO 3 (optional polish): pulse the surface so the portal reads as "alive":
        //         if (surface != null) surface.material.SetColor("_EmissionColor", baseColor * (1f + 0.3f * Mathf.Sin(Time.time * 3f)));
        //         Store baseColor once in Start() from surface.sharedMaterial.GetColor("_EmissionColor").
        //         Look at: Material.SetColor, Mathf.Sin. Check: the portal surface breathes slowly.
    }

    void OnTriggerEnter(Collider other)
    {
        // TODO 1: Only react to the player's head. The Head Probe is a child of the XR camera, so:
        //             if (other.GetComponentInParent<Camera>() == null) return;
        //         Why: the controllers also carry colliders; you do not want a stray hand to teleport the player.
        //         Also return if Time.time < armedAt (not armed yet).
        //         Look at: Component.GetComponentInParent<T>() — searches this object and its parents.

        // TODO 2: Ask the persistent loader to travel:
        //             Debug.Log("[Portal] entering " + displayName);
        //             GameManager.Instance.Loader.LoadScene(targetScene);
        //         Why go through GameManager.Instance instead of a scene reference: the loader lives in the
        //         DontDestroyOnLoad scene, so no object in THIS scene can hold a serialized reference to it.
        //         Check: walking into a Hub portal fades to black and the Forest/Mountain/Ruins stub appears.
        //         Walking into the stub's Return Portal brings you back to the Hub with the same GameManager.
    }
}

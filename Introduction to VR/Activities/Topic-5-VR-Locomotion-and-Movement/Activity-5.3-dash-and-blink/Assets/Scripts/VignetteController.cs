// VignetteController.cs — Activity 5.3: Hybrid: Dash and Blink
// Owns the two overlay quads in front of the camera: a radial "tunnel" vignette whose strength eases toward a
// target value every frame, and a solid black quad for blinks. Both are plain quads with transparent materials;
// the vignette's ring shape comes from a small alpha texture this script generates at startup.
// Attached to: Comfort Overlays (an empty under the Main Camera) in the starter scene; both quads are pre-assigned.
// Created by Isac Artzi

using System.Collections;
using UnityEngine;

public class VignetteController : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("Renderer of the vignette quad (radial gradient, darkens the edges of the view).")]
    public Renderer vignetteQuad;

    [Tooltip("Renderer of the blink quad (solid black, covers the whole view).")]
    public Renderer blinkQuad;

    [Header("Vignette shape (0 = center, 1 = quad edge)")]
    [Tooltip("Inside this radius the view is fully clear.")]
    [Range(0f, 1f)] public float innerRadius = 0.30f;

    [Tooltip("At this radius and beyond the vignette reaches full darkness.")]
    [Range(0f, 1.5f)] public float outerRadius = 0.80f;

    [Tooltip("Resolution of the generated gradient texture. 128 is plenty; it is only a soft ring.")]
    public int textureSize = 128;

    [Header("Vignette motion")]
    [Tooltip("Strength the vignette eases toward every frame. Dash sets this to its strength, then back to 0.")]
    [Range(0f, 1f)] public float targetVignette = 0f;

    [Tooltip("How fast the current strength moves toward the target, in strength units per second. 6 ≈ 0.17 s for a full swing.")]
    public float vignetteSpeed = 6f;

    [Header("Debug (read-only)")]
    [Range(0f, 1f)] public float currentVignette;

    Material vignetteMaterial;
    Material blinkMaterial;

    void Awake()
    {
        if (vignetteQuad != null)
        {
            vignetteMaterial = vignetteQuad.material;      // instance, so we can change alpha freely
            vignetteMaterial.mainTexture = MakeRadialTexture(textureSize, innerRadius, outerRadius);
        }
        if (blinkQuad != null) blinkMaterial = blinkQuad.material;
        ApplyVignette(0f);
        SetBlackout(0f);
    }

    /// <summary>
    /// A square texture that is transparent in the middle and opaque black toward the edges: the tunnel mask.
    /// Radius is measured from the center with 1.0 at the middle of an edge (so corners go past 1).
    /// </summary>
    public static Texture2D MakeRadialTexture(int size, float inner, float outer)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        var pixels = new Color[size * size];

        // TODO 1: For every pixel (x, y): float u = (x + 0.5f) / size, v = (y + 0.5f) / size;
        //             float r = new Vector2(u - 0.5f, v - 0.5f).magnitude * 2f;          // 0 at center, 1 at edge midpoint
        //             float a = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(inner, outer, r));  // 0 inside inner, 1 beyond outer
        //             pixels[y * size + x] = new Color(0f, 0f, 0f, a);
        //         Then tex.SetPixels(pixels); tex.Apply();
        //         Look at: Mathf.InverseLerp(a, b, value) → 0..1 position of value between a and b (clamped); Mathf.SmoothStep.
        //         Why generate it: no texture file to import, and you can tune inner/outer in the Inspector and rebuild.
        //         Check: select the Vignette Quad in Play mode, open its material — the preview shows a soft black ring
        //         with a clear center. Until then the ring is invisible (all pixels transparent).
        for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color(0f, 0f, 0f, 0f);   // placeholder: fully clear
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    void Update()
    {
        // TODO 2: Ease currentVignette toward targetVignette and apply it:
        //             currentVignette = Mathf.MoveTowards(currentVignette, targetVignette, vignetteSpeed * Time.deltaTime);
        //             ApplyVignette(currentVignette);
        //         Look at: Mathf.MoveTowards(current, target, maxDelta) — a linear ease with a speed limit, no overshoot.
        //         Why ease at all: a vignette that pops in on one frame is itself a visual jolt; 100–200 ms of ease is invisible.
        //         Check: set Target Vignette to 1 in the Inspector during Play — the edges darken over about 0.17 s; set 0 — they clear.
    }

    /// <summary>Writes a vignette strength (0–1) straight to the material alpha. Prefer setting targetVignette for smooth changes.</summary>
    public void ApplyVignette(float strength)
    {
        if (vignetteMaterial == null) return;
        var c = vignetteMaterial.color;
        c.a = Mathf.Clamp01(strength);
        vignetteMaterial.color = c;
    }

    /// <summary>Sets the blink quad's alpha: 0 = clear, 1 = fully black.</summary>
    public void SetBlackout(float alpha)
    {
        // TODO 3: Same pattern as ApplyVignette but on blinkMaterial (null-check).
        //         Check: call SetBlackout(1f) from BlinkStep — the whole view goes black, not just the edges.
    }

    /// <summary>Fades the blink quad from one alpha to another over the given seconds. Use with StartCoroutine.</summary>
    public IEnumerator FadeBlackout(float from, float to, float seconds)
    {
        // TODO 4: float t = 0f; while (t < seconds) { t += Time.deltaTime; SetBlackout(Mathf.Lerp(from, to, Mathf.Clamp01(t / seconds))); yield return null; }
        //         SetBlackout(to);
        //         Why unscaled time is NOT used here: if the game pauses (timeScale 0) the fade should pause too.
        //         Check: BlinkStep's fade-in reveals the new spot over 0.1 s instead of popping.
        SetBlackout(to);
        yield return null;
    }
}

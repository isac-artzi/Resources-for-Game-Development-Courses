// ComfortVignette.cs — Activity 2.2: First Steps: Teleport and Move
// Darkens the edges of the view while the rig moves smoothly. The darker the periphery, the less optic flow reaches
// the eye, and the smaller the conflict with the inner ear that reports "you are not moving". Tracking motion (your
// real steps) is ignored on purpose: only artificial motion of the XR Origin counts.
// Attached to: Comfort Vignette (a Quad parented to the Main Camera, 0.35 m ahead, using the 'Vignette' material).
// Created by Isac Artzi

using UnityEngine;

public class ComfortVignette : MonoBehaviour
{
    [Header("What moves")]
    [Tooltip("The XR Origin root. Its position changes only when a locomotion provider moves you, not when you lean.")]
    public Transform rig;

    [Header("Speed to darkness")]
    [Tooltip("Below this speed (m/s) the vignette is fully transparent.")]
    public float minSpeed = 0.2f;

    [Tooltip("At or above this speed (m/s) the vignette reaches Max Alpha.")]
    public float maxSpeed = 1.5f;

    [Range(0f, 1f)]
    [Tooltip("Darkest the edges get. 0.85 is strong; comfort-sensitive players often want 1.")]
    public float maxAlpha = 0.85f;

    [Tooltip("How quickly alpha chases its target (per second). Higher = snappier. 6 reaches ~90% in 0.4 s.")]
    public float smoothing = 6f;

    [Tooltip("A jump larger than this in one frame is a teleport, which needs no vignette.")]
    public float teleportJumpMeters = 1.0f;

    [Header("Hole shape (texture)")]
    [Range(0f, 1.4f)]
    [Tooltip("Normalized radius where darkening starts (0 = center, 1 = edge midpoint, 1.41 = corner).")]
    public float innerRadius = 0.45f;

    [Range(0f, 1.5f)]
    [Tooltip("Normalized radius where darkening is complete.")]
    public float outerRadius = 0.85f;

    [Tooltip("Pixels per side of the generated gradient texture. 128 is plenty for a soft ring.")]
    public int textureSize = 128;

    Material mat;          // an instance, so we never edit the shared asset in Play mode
    Vector3 lastRigPosition;
    float alpha;

    /// <summary>Artificial speed of the rig in m/s, measured last frame.</summary>
    public float CurrentSpeed { get; private set; }

    /// <summary>Current opacity of the vignette edges, 0..1.</summary>
    public float CurrentAlpha { get { return alpha; } }

    void Start()
    {
        var rend = GetComponent<Renderer>();
        if (rend != null) mat = rend.material;
        if (rig == null && Camera.main != null) rig = Camera.main.transform.root;
        if (rig != null) lastRigPosition = rig.position;

        // TODO 1: Give the material a radial gradient so the middle stays clear and the edges darken:
        //             var tex = BuildGradient();
        //             if (mat != null && tex != null) mat.mainTexture = tex;
        //         Implement BuildGradient() below.
        //         Check: pause Play mode, select Comfort Vignette, and look at the material preview — a soft dark ring
        //         with a transparent center. Without TODO 1 the whole quad darkens uniformly (still a valid fade!).
    }

    /// <summary>
    /// Builds a square texture whose alpha is 0 inside innerRadius and 1 beyond outerRadius, measured from the center
    /// in normalized units (1 = the distance from center to an edge midpoint). RGB is white so the material color tints it.
    /// </summary>
    public Texture2D BuildGradient()
    {
        // TODO 1 (continued):
        //   var tex = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        //   tex.wrapMode = TextureWrapMode.Clamp;
        //   for each y in 0..textureSize-1, for each x in 0..textureSize-1:
        //       float u = (x + 0.5f) / textureSize;  float v = (y + 0.5f) / textureSize;       // 0..1 across the quad
        //       float r = Vector2.Distance(new Vector2(u, v), new Vector2(0.5f, 0.5f)) * 2f; // 0 center, 1 edge, 1.41 corner
        //       float a = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(innerRadius, outerRadius, r));
        //       tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
        //   tex.Apply();  return tex;
        // Look at: Texture2D.SetPixel / Apply, Mathf.InverseLerp (maps [a,b] to [0,1], clamped), Mathf.SmoothStep.
        // Why SmoothStep: a linear ramp shows a visible ring edge; the S-curve hides it.
        // Check: r = 0.65 with inner 0.45 / outer 0.85 gives InverseLerp = 0.5 and alpha = 0.5.
        return null;
    }

    void LateUpdate()
    {
        if (rig == null || mat == null) return;
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        // TODO 2: Speed of the rig. Distance moved since last frame divided by dt:
        //             float moved = (rig.position - lastRigPosition).magnitude;
        //             lastRigPosition = rig.position;
        //             CurrentSpeed = (moved > teleportJumpMeters) ? 0f : moved / dt;
        //         Why the teleport guard: a 6 m blink in one frame would read as 430 m/s and slam the vignette shut
        //         for a moment after every teleport, which is exactly when the eye needs a clear view of where it landed.
        //         Check: walk with the left stick and watch Current Speed in the Inspector sit near the provider's
        //         Move Speed (default 1 m/s); teleporting leaves it at 0.

        // TODO 3: Target alpha from speed with a linear ramp between minSpeed and maxSpeed:
        //             float target = maxAlpha * Mathf.InverseLerp(minSpeed, maxSpeed, CurrentSpeed);
        //         Look at: Mathf.InverseLerp — clamps automatically, so speeds above maxSpeed give exactly maxAlpha.

        // TODO 4: Ease alpha toward the target and push it into the material's color:
        //             alpha = Mathf.Lerp(alpha, target, 1f - Mathf.Exp(-smoothing * dt));
        //             var c = mat.color; c.a = alpha; mat.color = c;
        //         Why the exponential form: it moves the same fraction of the remaining gap per second regardless
        //         of frame rate, so the fade feels identical at 72 Hz and 144 Hz.
        //         Check: start walking -> edges darken within half a second; stop -> they clear. Turn Max Alpha to 0
        //         and nothing happens (a quick way to compare comfort with and without the vignette).
    }
}

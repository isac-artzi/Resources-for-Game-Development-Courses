// WeatherController.cs — Activity 3.2: Mountain Pass: Terrain and Weather
// Blends the whole atmosphere — fog color and density, sun intensity, snow amount, wind — between three presets
// (Clear, Overcast, Blizzard) over a few seconds instead of snapping. Keys 1/2/3 switch presets on desktop.
// Other scripts read CurrentBaseFogDensity so they can layer their own effects on top (see AltitudeFog).
// Attached to: "Weather" in the starter scene (Sun and Snow references are pre-set, presets pre-filled).
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.InputSystem;

public class WeatherController : MonoBehaviour
{
    /// <summary>The three weather states the pass can be in. Index matches the presets array.</summary>
    public enum Kind { Clear = 0, Overcast = 1, Blizzard = 2 }

    /// <summary>Everything that changes with the weather, in one bundle so it can be lerped as a unit.</summary>
    [System.Serializable]
    public class Preset
    {
        [Tooltip("Name shown in the Console and the HUD.")]
        public string name = "Clear";

        [Tooltip("Fog color. Also used as the camera background so sky and fog agree.")]
        public Color fogColor = new Color(0.72f, 0.80f, 0.92f);

        [Tooltip("Exponential fog density. 0.005 = far horizon; 0.06 = ten meters of visibility.")]
        public float fogDensity = 0.006f;

        [Tooltip("Directional light intensity. Blizzards are dim.")]
        public float sunIntensity = 1.2f;

        [Range(0f, 1f)]
        [Tooltip("Snow amount 0..1, passed to SnowEmitter.SetIntensity.")]
        public float snow = 0f;

        [Tooltip("Sideways wind speed applied to snow, in m/s.")]
        public float windX = 0f;
    }

    [Header("Presets (index 0 = Clear, 1 = Overcast, 2 = Blizzard)")]
    [Tooltip("The three atmospheres. The starter scene fills these in; tune them freely.")]
    public Preset[] presets = new Preset[]
    {
        new Preset { name = "Clear",    fogColor = new Color(0.72f, 0.80f, 0.92f), fogDensity = 0.006f, sunIntensity = 1.2f, snow = 0.0f, windX = 0f },
        new Preset { name = "Overcast", fogColor = new Color(0.62f, 0.65f, 0.70f), fogDensity = 0.018f, sunIntensity = 0.7f, snow = 0.25f, windX = 1f },
        new Preset { name = "Blizzard", fogColor = new Color(0.78f, 0.80f, 0.84f), fogDensity = 0.060f, sunIntensity = 0.35f, snow = 1.0f, windX = 6f },
    };

    [Header("Transition")]
    [Tooltip("Seconds a full change from one preset to another takes.")]
    public float transitionSeconds = 4f;

    [Tooltip("Which preset is active when Play starts.")]
    public Kind startWeather = Kind.Clear;

    [Header("Scene references")]
    [Tooltip("The directional light. Set in the starter scene.")]
    public Light sun;

    [Tooltip("The snow particle emitter. Set in the starter scene; may be null.")]
    public SnowEmitter snow;

    [Tooltip("Optional 3D label that shows the current weather name.")]
    public TextMesh readout;

    /// <summary>Fog density of the blended weather BEFORE altitude effects. AltitudeFog multiplies this.</summary>
    public float CurrentBaseFogDensity { get; private set; }

    /// <summary>Index of the preset we are heading toward.</summary>
    public int TargetIndex { get; private set; }

    Preset from;      // where the blend started
    Preset to;        // where it is going
    float blend = 1f; // 0 = fully 'from', 1 = fully 'to'
    Camera cam;

    void Start()
    {
        cam = Camera.main;
        TargetIndex = Mathf.Clamp((int)startWeather, 0, presets.Length - 1);
        from = presets[TargetIndex];
        to = presets[TargetIndex];
        blend = 1f;
        Apply(to);
    }

    void Update()
    {
        // TODO 3: Desktop controls. Read the Input System keyboard (null-check Keyboard.current) and call
        //         SetWeather(0) on digit1Key.wasPressedThisFrame, SetWeather(1) on digit2Key, SetWeather(2) on digit3Key.
        //         Look at: UnityEngine.InputSystem.Keyboard.current.digit1Key. Never use the legacy Input class.
        //         Check: pressing 3 turns the pass into a blizzard over 4 seconds; 1 clears it again.

        // TODO 2: Advance the blend and apply the in-between preset every frame.
        //         1. if (blend < 1f) blend = Mathf.Min(1f, blend + Time.deltaTime / Mathf.Max(0.01f, transitionSeconds));
        //         2. Preset now = Blend(from, to, Mathf.SmoothStep(0f, 1f, blend));
        //         3. Apply(now);
        //         Why SmoothStep: an eased blend starts and ends gently, which reads as weather rolling in rather
        //         than a dimmer switch being turned. Why re-apply every frame even when blend == 1: AltitudeFog and
        //         other scripts may write RenderSettings too; re-applying the base each frame keeps the order predictable.
        //         Check: the change from Clear to Blizzard takes about transitionSeconds and never pops.
    }

    /// <summary>Starts a timed transition toward the preset at index (0 Clear, 1 Overcast, 2 Blizzard).</summary>
    public void SetWeather(int index)
    {
        if (presets == null || presets.Length == 0) return;
        index = Mathf.Clamp(index, 0, presets.Length - 1);
        if (index == TargetIndex && blend >= 1f) return;

        // Start the new blend from whatever the sky looks like right now, so a change mid-transition is seamless.
        from = Blend(from, to, Mathf.SmoothStep(0f, 1f, blend));
        to = presets[index];
        TargetIndex = index;
        blend = 0f;
        Debug.Log("[Weather] -> " + to.name);
    }

    /// <summary>Convenience for UnityEvents, buttons and the Inspector's context menu: cycle Clear -> Overcast -> Blizzard -> Clear.</summary>
    [ContextMenu("Next weather")]
    public void NextWeather()
    {
        SetWeather((TargetIndex + 1) % presets.Length);
    }

    /// <summary>A preset that is t of the way from a to b (t = 0 gives a, t = 1 gives b).</summary>
    public static Preset Blend(Preset a, Preset b, float t)
    {
        // TODO 1: Interpolate every field.
        //         var p = new Preset();
        //         p.name         = t < 0.5f ? a.name : b.name;
        //         p.fogColor     = Color.Lerp(a.fogColor, b.fogColor, t);
        //         p.fogDensity   = Mathf.Lerp(a.fogDensity, b.fogDensity, t);
        //         p.sunIntensity = Mathf.Lerp(a.sunIntensity, b.sunIntensity, t);
        //         p.snow         = Mathf.Lerp(a.snow, b.snow, t);
        //         p.windX        = Mathf.Lerp(a.windX, b.windX, t);
        //         return p;
        //         Look at: Color.Lerp, Mathf.Lerp. Both clamp t to 0..1 for you.
        //         Check: Blend(Clear, Blizzard, 0.5f).fogDensity is 0.033 (the midpoint of 0.006 and 0.060).
        return t < 0.5f ? a : b;   // placeholder: snaps at the halfway point instead of blending
    }

    /// <summary>Writes one preset to the scene: fog, camera background, sun, snow, label.</summary>
    void Apply(Preset p)
    {
        if (p == null) return;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = p.fogColor;
        RenderSettings.fogDensity = p.fogDensity;
        CurrentBaseFogDensity = p.fogDensity;

        if (cam != null)
        {
            // A skybox stays bright blue in a blizzard. A solid background in the fog color makes fog and sky one thing.
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = p.fogColor;
        }

        if (sun != null) sun.intensity = p.sunIntensity;

        // TODO 4: Drive the snow. If snow != null: snow.SetIntensity(p.snow); snow.SetWind(p.windX);
        //         (Implement those two methods in SnowEmitter.cs — TODO 3 and 4 there.)
        //         Check: Blizzard fills the air with fast sideways snow; Clear has none.

        if (readout != null) readout.text = "Weather: " + p.name;
    }
}

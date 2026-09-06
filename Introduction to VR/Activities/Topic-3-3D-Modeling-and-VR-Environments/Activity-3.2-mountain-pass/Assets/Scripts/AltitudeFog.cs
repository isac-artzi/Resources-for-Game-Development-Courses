// AltitudeFog.cs — Activity 3.2: Mountain Pass: Terrain and Weather
// Thickens the fog as the player climbs: the valley floor is clear, the high pass is lost in cloud.
// Reads the base density from WeatherController and multiplies it by an altitude factor every frame,
// then prints altitude and visibility to a small HUD label so you can reason about the numbers.
// Attached to: "Weather" in the starter scene (Weather reference and HUD label pre-set).
// Created by Isac Artzi

using UnityEngine;

public class AltitudeFog : MonoBehaviour
{
    [Header("Source")]
    [Tooltip("The controller that owns the base fog. Set in the starter scene.")]
    public WeatherController weather;

    [Header("Altitude ramp (meters, world y of the player's head)")]
    [Tooltip("At or below this altitude the fog is exactly the weather's base density.")]
    public float lowAltitude = 8f;

    [Tooltip("At or above this altitude the fog reaches base * (1 + maxExtraMultiplier).")]
    public float highAltitude = 45f;

    [Tooltip("How much thicker the fog gets at the top. 2 means three times the base density.")]
    public float maxExtraMultiplier = 2f;

    [Header("Report")]
    [Tooltip("Optional HUD label (a TextMesh parented to the camera in the starter scene).")]
    public TextMesh readout;

    /// <summary>0 at lowAltitude, 1 at highAltitude, computed each frame.</summary>
    public float AltitudeFactor { get; private set; }

    Transform head;

    void Start()
    {
        if (Camera.main != null) head = Camera.main.transform;
        if (weather == null) weather = FindFirstObjectByType<WeatherController>();
    }

    // LateUpdate so this runs AFTER WeatherController.Update has written the base density this frame.
    void LateUpdate()
    {
        if (head == null || weather == null) return;

        // TODO 1: Read the altitude: float altitude = head.position.y;
        //         (The XR Origin sits on the terrain, so the camera's world y is terrain height + eye height.)

        // TODO 2: Turn altitude into a 0..1 factor with Mathf.InverseLerp(lowAltitude, highAltitude, altitude)
        //         and store it in AltitudeFactor. InverseLerp clamps, so below lowAltitude you get 0 and above
        //         highAltitude you get 1 — no extra ifs needed.
        //         Look at: Mathf.InverseLerp(a, b, value).

        // TODO 3: Thicken the fog: float density = weather.CurrentBaseFogDensity * (1f + AltitudeFactor * maxExtraMultiplier);
        //         RenderSettings.fogDensity = density;
        //         Why multiply instead of add: a Blizzard is already dense; tripling it at the summit reads right,
        //         whereas adding a fixed 0.02 would be invisible in a blizzard and overwhelming on a clear day.
        //         Check: teleport up the ramp on a Clear day — the far ridges fade as you climb; teleport back down and they return.

        // TODO 4: Report. Visibility is where the fog reaches 50 %: d50 = ln(2) / density (see Math foundation).
        //         if (readout != null) readout.text = "Altitude " + altitude.ToString("F1") + " m\n" +
        //                                             "Fog x" + (1f + AltitudeFactor * maxExtraMultiplier).ToString("F2") + "\n" +
        //                                             "50% visibility " + HalfVisibilityDistance(density).ToString("F0") + " m";
        //         Check: on a Clear day at the bottom the HUD reads about 115 m of visibility (0.693 / 0.006);
        //         in a Blizzard at the top it drops to about 4 m.
        if (readout != null) readout.text = "AltitudeFog: implement TODO 1-4";
    }

    /// <summary>Distance at which exponential fog of the given density hides half of what is behind it.</summary>
    public static float HalfVisibilityDistance(float density)
    {
        // TODO 4 (continued): return density > 0f ? Mathf.Log(2f) / density : float.PositiveInfinity;
        //         Look at: Mathf.Log(float) is the natural log (base e). ln 2 = 0.693.
        //         Check: HalfVisibilityDistance(0.0173f) is about 40 m — the number solved in the Math foundation.
        return 0f;
    }

    /// <summary>The density that gives 50 % visibility at the given distance — the inverse of the method above.</summary>
    public static float DensityForHalfVisibility(float meters)
    {
        // TODO 5: return meters > 0f ? Mathf.Log(2f) / meters : 0f;
        //         Use this when tuning presets: "I want 40 m of visibility in Overcast" -> DensityForHalfVisibility(40f).
        //         Check: DensityForHalfVisibility(40f) is about 0.0173.
        return 0f;
    }
}

// SnowEmitter.cs — Activity 3.2: Mountain Pass: Terrain and Weather
// Keeps a snow ParticleSystem hovering above the player's head wherever they teleport, and exposes two knobs
// — SetIntensity (emission rate) and SetWind (sideways drift) — that WeatherController turns as the weather blends.
// Snow that covers a whole 200 m terrain would need hundreds of thousands of particles; snow that follows the
// player needs a few thousand and looks identical from inside.
// Attached to: "Snow" in the starter scene (the ParticleSystem is configured by the builder).
// Created by Isac Artzi

using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SnowEmitter : MonoBehaviour
{
    [Header("Follow")]
    [Tooltip("What to hover above. Leave empty to use the main camera (the player's head).")]
    public Transform target;

    [Tooltip("Meters above the head the emitter box sits. Snow spawns here and falls past the eyes.")]
    public float heightAboveHead = 6f;

    [Tooltip("Meters ahead of the view direction to center the box, so you see snow where you look.")]
    public float leadDistance = 4f;

    [Header("Amount")]
    [Tooltip("Particles per second at intensity 1 (Blizzard). 300 is dense but cheap; 1500 is a whiteout.")]
    public float maxRate = 800f;

    ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        // TODO 1: If target is null and Camera.main is not, use Camera.main.transform.
        //         Check: with Target empty in the Inspector, the snow still follows you.
    }

    // LateUpdate: the head has already moved this frame, so the box lands in the right place.
    void LateUpdate()
    {
        if (target == null) return;

        // TODO 2: Follow the head. Build a horizontal forward vector (target.forward with y set to 0, normalized —
        //         guard against a zero vector when the player looks straight up) and set
        //             transform.position = target.position + Vector3.up * heightAboveHead + forwardFlat * leadDistance;
        //         Look at: Vector3.normalized, Vector3.sqrMagnitude. Keep the rotation unchanged: snow falls straight down
        //         in world space (the ParticleSystem's Simulation Space is World, so moving the emitter does not drag
        //         existing flakes along).
        //         Check: teleport across the pass — the snow is always overhead; look down and there is snow below you too.
    }

    /// <summary>0 = no snow, 1 = full maxRate. Called by WeatherController every frame during a transition.</summary>
    public void SetIntensity(float intensity01)
    {
        // TODO 3: ParticleSystem modules are structs you copy, edit, and let write back through their properties:
        //             var emission = ps.emission;
        //             emission.rateOverTime = Mathf.Clamp01(intensity01) * maxRate;
        //         Look at: ParticleSystem.EmissionModule.rateOverTime (a MinMaxCurve; assigning a float works).
        //         Common mistake: writing ps.emission.rateOverTime = ... directly does not compile because
        //         ps.emission returns a copy — take the copy into a local first.
        //         Check: press 1 (Clear) and the flakes stop appearing; press 3 and they pour.
    }

    /// <summary>Sideways drift in m/s along world x. Positive blows toward +x.</summary>
    public void SetWind(float metersPerSecond)
    {
        // TODO 4: var vel = ps.velocityOverLifetime;
        //         vel.enabled = true;
        //         vel.space = ParticleSystemSimulationSpace.World;
        //         vel.x = metersPerSecond;               // MinMaxCurve accepts a float
        //         Look at: ParticleSystem.VelocityOverLifetimeModule. The builder already gives flakes a slow fall (y);
        //         you only change x here.
        //         Check: in a Blizzard the snow streaks sideways instead of drifting straight down.
    }
}

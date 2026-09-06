// LodBuilder.cs — Activity 3.3: Make It Fast: LOD, Occlusion, Lightmaps
// Builds a three-level LODGroup from three child GameObjects named LOD0 / LOD1 / LOD2 (detailed, medium, coarse).
// Runs when Play starts (Build On Awake) or from the component's context menu in the Editor, so you can set up a
// LOD Group on a hundred pillars with one script instead of a hundred Inspector sessions.
// Attached to: every "Pillar" prop in the starter scene; use it on your imported props too — just name the
// children LOD0, LOD1, LOD2 (or change Level Names).
// Created by Isac Artzi

using System.Collections.Generic;
using UnityEngine;

public class LodBuilder : MonoBehaviour
{
    [Header("Levels (nearest first)")]
    [Tooltip("Names of the child GameObjects that hold each level's renderers. Order: most detailed first.")]
    public string[] levelNames = new string[] { "LOD0", "LOD1", "LOD2" };

    [Tooltip("Screen-relative height (0..1) BELOW which each level hands over to the next. A pillar taking up 40 % of " +
             "the screen height or more shows LOD0; between 15 % and 40 % LOD1; between 3 % and 15 % LOD2; under 3 % it is culled.")]
    public float[] transitionHeights = new float[] { 0.40f, 0.15f, 0.03f };

    [Header("When")]
    [Tooltip("Build the LODGroup automatically when Play starts.")]
    public bool buildOnAwake = true;

    /// <summary>True after BuildLods() succeeded on this object.</summary>
    public bool Built { get; private set; }

    void Awake()
    {
        if (buildOnAwake) BuildLods();
    }

    /// <summary>Creates (or refreshes) the LODGroup on this GameObject from the named children.</summary>
    [ContextMenu("Build LODs")]
    public void BuildLods()
    {
        if (levelNames == null || levelNames.Length == 0) return;
        int levels = Mathf.Min(levelNames.Length, transitionHeights.Length);
        var lods = new LOD[levels];

        for (int i = 0; i < levels; i++)
        {
            // TODO 1: Collect this level's renderers.
            //         Transform level = transform.Find(levelNames[i]);   (null if the child does not exist — log a warning and return)
            //         Renderer[] renderers = level.GetComponentsInChildren<Renderer>(true);
            //         Look at: Transform.Find(string) searches direct children by name; GetComponentsInChildren<Renderer>
            //         gathers every MeshRenderer under it, so a level may be several primitives.
            Renderer[] renderers = new Renderer[0];

            // TODO 2: Describe the level. A LOD is a struct: new LOD(screenRelativeTransitionHeight, renderers).
            //         lods[i] = new LOD(transitionHeights[i], renderers);
            //         Look at: UnityEngine.LOD. The height is the fraction of the screen the object's bounding box
            //         must fill for this level (or a better one) to be used. See Math foundation for how to choose it.
            lods[i] = new LOD(transitionHeights[i], renderers);
        }

        if (RenderersMissing(lods))
        {
            // Before TODO 1 there is nothing to build: leave the object as it is (all three levels stay visible).
            Built = false;
            return;
        }

        // TODO 3: Attach and configure the group.
        //         LODGroup group = GetComponent<LODGroup>();  if (group == null) group = gameObject.AddComponent<LODGroup>();
        //         group.SetLODs(lods);
        //         group.RecalculateBounds();   // the group needs the combined bounds of all levels to measure screen height
        //         Built = true;
        //         Look at: LODGroup.SetLODs, LODGroup.RecalculateBounds. Also read LODGroup.fadeMode if you want cross-fades.
        //         Check: press Play, select any Pillar — a LODGroup component appears with three colored bars. Walk away from it:
        //         the Inspector highlights LOD0, then LOD1, then LOD2, then "Culled". Only one level's renderers are visible
        //         at a time (before this TODO all three overlap and z-fight).
    }

    /// <summary>
    /// Fraction of the screen height an object of the given height fills at the given distance, for a camera with the
    /// given VERTICAL field of view. This is the number the LODGroup compares to transitionHeights.
    /// </summary>
    public static float ScreenRelativeHeight(float objectHeightMeters, float distanceMeters, float verticalFovDegrees)
    {
        // TODO 4: Implement  h_s = h / (2 d tan(fov / 2)).
        //         if (distanceMeters <= 0f) return 1f;
        //         float halfFovRad = verticalFovDegrees * 0.5f * Mathf.Deg2Rad;
        //         return objectHeightMeters / (2f * distanceMeters * Mathf.Tan(halfFovRad));
        //         Look at: Mathf.Tan (radians), Mathf.Deg2Rad.
        //         Check: ScreenRelativeHeight(4f, 20f, 60f) is about 0.173 — a 4 m pillar 20 m away fills 17 % of a 60-degree view,
        //         which lands in the LOD1 band (0.15-0.40) with the default thresholds.
        return 0f;
    }

    /// <summary>Distance at which an object of the given height crosses the given screen-relative height.</summary>
    public static float DistanceForScreenHeight(float objectHeightMeters, float screenRelativeHeight, float verticalFovDegrees)
    {
        // TODO 5: The inverse:  d = h / (2 h_s tan(fov / 2)).  Guard screenRelativeHeight <= 0 by returning float.PositiveInfinity.
        //         Use it to sanity-check thresholds: DistanceForScreenHeight(4f, 0.03f, 60f) is about 115 m — so with the
        //         default thresholds a 4 m pillar is culled beyond 115 m on desktop. On the Quest (vertical FOV ~ 95 degrees) it
        //         culls closer, at about 61 m. Print both to the Console in a Start() somewhere if you are curious.
        //         Check: DistanceForScreenHeight(4f, 0.173f, 60f) returns 20 — the round trip of TODO 4.
        return 0f;
    }

    static bool RenderersMissing(LOD[] lods)
    {
        for (int i = 0; i < lods.Length; i++)
            if (lods[i].renderers == null || lods[i].renderers.Length == 0) return true;
        return false;
    }
}

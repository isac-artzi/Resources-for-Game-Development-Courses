// ScaleNormalizer.cs — Activity 3.1: Plant the Forest
// Measures how tall a model really is (from its renderer bounds) and reports the scale factor that would make
// it the height you want. Imported models arrive in centimeters, inches, or "whatever the artist used";
// this gauge turns "it looks small" into a number you can type into the model's Import Settings.
// Attached to: "Scale Test Subject" under the Scale Gauge station (a deliberately mis-scaled greybox tree).
// Created by Isac Artzi

using UnityEngine;

public class ScaleNormalizer : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("How tall this object should be in meters. A forest tree: 6-10 m. A bush: 0.8-1.5 m. A rock: 0.3-2 m.")]
    public float targetHeightMeters = 8f;

    [Tooltip("Multiply this object's scale by the computed factor as soon as Play starts.")]
    public bool applyOnStart = true;

    [Header("Report")]
    [Tooltip("Optional 3D label that shows the measurement. The starter scene assigns the gauge's label.")]
    public TextMesh readout;

    /// <summary>Height in meters measured by the last call to MeasureHeight().</summary>
    public float MeasuredHeight { get; private set; }

    /// <summary>Scale factor computed by the last call to Report().</summary>
    public float Factor { get; private set; }

    void Start()
    {
        Report();
        if (applyOnStart && Factor > 0f && !Mathf.Approximately(Factor, 1f))
        {
            transform.localScale *= Factor;
            Report();   // measure again so the label shows the corrected height
        }
    }

    /// <summary>
    /// World-space height (y extent) of everything rendered under this object, in meters.
    /// Includes the current Transform scale, so the factor returned by FactorFor() multiplies the CURRENT scale.
    /// </summary>
    public float MeasureHeight()
    {
        // TODO 1: Measure with renderer bounds.
        //         1. Renderer[] renderers = GetComponentsInChildren<Renderer>();  if none, return 0f.
        //         2. Bounds total = renderers[0].bounds;  then for each other renderer: total.Encapsulate(r.bounds);
        //         3. return total.size.y;
        //         Look at: Renderer.bounds (world-space axis-aligned box), Bounds.Encapsulate, Bounds.size.
        //         Why bounds and not transform.localScale: scale tells you nothing about the mesh — a 0.01 m mesh at
        //         scale 1 and a 100 m mesh at scale 0.0001 are both "scale 1" in some sense. Bounds measure meters.
        //         Check: the gauge reports the greybox test tree as about 0.13 m tall (it was placed at scale 0.02).
        return 0f;
    }

    /// <summary>The multiplier that turns a model of the measured height into targetHeightMeters.</summary>
    public float FactorFor(float measuredHeight)
    {
        // TODO 2: Return targetHeightMeters / measuredHeight, but if measuredHeight is below 0.0001f return 1f
        //         (nothing to measure — do not divide by zero and do not blow the object up to infinity).
        //         Check: for the test tree the factor is about 61 (8 m / 0.13 m).
        return 1f;
    }

    /// <summary>Measures, computes the factor, prints to the Console and the optional label.</summary>
    [ContextMenu("Measure and report")]
    public void Report()
    {
        MeasuredHeight = MeasureHeight();
        Factor = FactorFor(MeasuredHeight);

        // TODO 3: Build a readable report and show it in both places.
        //         string msg = name + ": " + MeasuredHeight.ToString("F2") + " m tall. Target " + targetHeightMeters +
        //                      " m -> multiply scale by " + Factor.ToString("F2");
        //         Debug.Log("[ScaleNormalizer] " + msg);   and   if (readout != null) readout.text = msg;
        //         Check: the label beside the 2 m post reads e.g. "Scale Test Subject: 0.13 m tall. Target 8 m ->
        //         multiply scale by 61.4" and, because Apply On Start is on, the tree then towers over the post.
        string placeholder = name + ": implement TODO 1-3 in ScaleNormalizer.cs";
        if (readout != null) readout.text = placeholder;
    }
}

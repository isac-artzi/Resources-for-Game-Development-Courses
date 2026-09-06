// TweenScale.cs — Activity 7.4: Juice: Feedback and Polish
// A "scale punch": the object swells past its normal size and settles back, driven by an AnimationCurve
// you can shape in the Inspector. Reusable on anything — shards, the Guide, a UI panel.
// Attached to: each Shard and the Mysterious Guide in the starter scene.
// Created by Isac Artzi

using System.Collections;
using UnityEngine;

public class TweenScale : MonoBehaviour
{
    [Header("Curve")]
    [Tooltip("Scale multiplier over normalized time 0..1. Starts at 1, overshoots to ~1.35, returns to 1.")]
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.15f, 1.35f), new Keyframe(0.5f, 1f));

    [Tooltip("Seconds the whole punch takes.")]
    public float duration = 0.5f;

    [Header("Read-only")]
    [Tooltip("Scale the object returns to. Captured in Awake.")]
    public Vector3 baseScale = Vector3.one;

    Coroutine running;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    /// <summary>Plays the punch from the start (restarts if one is already running).</summary>
    [ContextMenu("Punch")]
    public void Punch()
    {
        // TODO 1: If 'running' is not null, StopCoroutine(running) and reset transform.localScale = baseScale.
        //         Then running = StartCoroutine(PunchRoutine()).
        //         Why restart instead of stacking: two overlapping punches would fight over localScale and
        //         the second could end at the wrong size. One owner at a time.
        //         Check: right-click the component header > Punch in Play mode — the shard swells and settles.
    }

    /// <summary>Grows from zero to baseScale with an overshoot ("ease-out back"). Used by the Guide's reveal.</summary>
    public Coroutine GrowFromZero(float seconds)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(GrowRoutine(seconds));
        return running;
    }

    IEnumerator PunchRoutine()
    {
        // TODO 2: Drive the curve. float t = 0; while (t < 1f) { t += Time.deltaTime / duration;
        //             transform.localScale = baseScale * curve.Evaluate(Mathf.Clamp01(t)); yield return null; }
        //         then transform.localScale = baseScale; running = null.
        //         Look at: AnimationCurve.Evaluate(float time) — returns the curve's value; here a multiplier.
        //         Why a curve and not Lerp: the shape (fast overshoot, slow settle) IS the feel. Open the curve in the
        //         Inspector and drag the middle key higher — the punch gets bigger without touching code.
        //         Check: the shard punches to ~1.35x in 0.15 s and is back to normal at 0.5 s.
        yield return null;
        running = null;
    }

    IEnumerator GrowRoutine(float seconds)
    {
        // TODO 3: Ease-out back: s(t) = 1 + 2.70158 * (t-1)^3 + 1.70158 * (t-1)^2  (overshoots to ~1.1 near t = 0.7).
        //         float t = 0; transform.localScale = Vector3.zero;
        //         while (t < 1f) { t += Time.deltaTime / seconds; float u = Mathf.Clamp01(t) - 1f;
        //             float s = 1f + 2.70158f * u * u * u + 1.70158f * u * u;
        //             transform.localScale = baseScale * s; yield return null; }
        //         then transform.localScale = baseScale; running = null.
        //         Look at: the README's Math foundation for where these constants come from.
        //         Check: the Guide pops in from nothing, slightly too big, then settles — like a stage entrance.
        transform.localScale = baseScale;
        yield return null;
        running = null;
    }
}

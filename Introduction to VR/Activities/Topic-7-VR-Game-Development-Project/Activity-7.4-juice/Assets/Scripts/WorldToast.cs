// WorldToast.cs — Activity 7.4: Juice: Feedback and Polish
// A short message that appears IN THE WORLD at the place something happened, rises a little, fades, and
// faces the player. The VR replacement for the screen-space "+1" text that would be glued to your eyes.
// Attached to: "Toast" in the starter scene (a TextMesh). One toast is shared by every shard and the Guide.
// Created by Isac Artzi

using System.Collections;
using UnityEngine;

public class WorldToast : MonoBehaviour
{
    [Header("Motion")]
    [Tooltip("How far the text rises over its lifetime, in meters.")]
    public float riseMeters = 0.3f;

    [Tooltip("Lifetime in seconds.")]
    public float seconds = 1.2f;

    [Tooltip("Alpha over normalized time: fast in, hold, fade out.")]
    public AnimationCurve alphaCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.1f, 1f), new Keyframe(0.6f, 1f), new Keyframe(1f, 0f));

    [Header("Parts")]
    [Tooltip("The TextMesh to show. Empty = the one on this GameObject.")]
    public TextMesh label;

    Coroutine running;
    Color baseColor;

    void Awake()
    {
        if (label == null) label = GetComponent<TextMesh>();
        if (label != null)
        {
            baseColor = label.color;
            SetAlpha(0f);
        }
    }

    /// <summary>Shows 'text' starting at 'worldPos' (plus a small offset above), rising and fading.</summary>
    public void Show(string text, Vector3 worldPos)
    {
        if (label == null) return;
        // TODO 1: Restart cleanly: if (running != null) StopCoroutine(running);
        //         label.text = text; running = StartCoroutine(Routine(worldPos + Vector3.up * 0.25f));
        //         Check: grabbing a shard prints "Shard taken!" in the air just above it.
        label.text = text;
        transform.position = worldPos + Vector3.up * 0.25f;
        SetAlpha(1f);
    }

    IEnumerator Routine(Vector3 start)
    {
        // TODO 2: Rise with an ease-out cubic and fade with the curve:
        //             float t = 0; while (t < 1f) { t += Time.deltaTime / seconds; float k = Mathf.Clamp01(t);
        //                 float eased = 1f - Mathf.Pow(1f - k, 3f);                  // ease-out cubic: fast start, gentle stop
        //                 transform.position = start + Vector3.up * riseMeters * eased;
        //                 SetAlpha(alphaCurve.Evaluate(k)); FaceCamera(); yield return null; }
        //         then SetAlpha(0f); running = null.
        //         Look at: Mathf.Pow, AnimationCurve.Evaluate. Why ease-out: the text "arrives" quickly and drifts to a stop,
        //         which reads as light and pleasant; a linear rise looks mechanical.
        //         Check: the text lifts about 30 cm and is gone after 1.2 s; it never snaps.
        yield return null;
        running = null;
    }

    /// <summary>Turns the label so its readable side faces the main camera, upright.</summary>
    public void FaceCamera()
    {
        // TODO 3: var cam = Camera.main; if (cam == null) return;
        //         Vector3 away = transform.position - cam.transform.position; away.y = 0f;
        //         if (away.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(away);
        //         (A TextMesh is read from its -z side, so its +z must point AWAY from the viewer — same trick as Activity 1.1.)
        //         Check: walk around a shard and grab it from the far side — the toast still reads correctly.
    }

    void SetAlpha(float a)
    {
        var c = baseColor;
        c.a = Mathf.Clamp01(a);
        label.color = c;
    }
}

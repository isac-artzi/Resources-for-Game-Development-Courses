// AnnotationPin.cs — Activity 1.2: Mock View Studio
// A 3D "sticky note": a label floating above a point of interest, connected to it by a thin leader line,
// that faces the player and keeps a constant angular size so it is readable from any distance.
// You will use pins to annotate your mock views ("doorway 1.0 x 2.1 m", "text must read at 0.75 m").
// Attached to: each Pin object in the starter scene, and to every prop spawned by PropPlacer.
// Created by Isac Artzi

using UnityEngine;

public class AnnotationPin : MonoBehaviour
{
    [Header("What to annotate")]
    [Tooltip("The point the leader line starts from. Leave empty to use this object's own position.")]
    public Transform anchor;

    [Tooltip("The note. Keep it to one or two short lines; long text is unreadable in VR.")]
    [TextArea(1, 3)]
    public string text = "Annotation";

    [Header("Layout")]
    [Tooltip("How far above the anchor the label floats, in meters.")]
    public float labelHeight = 0.4f;

    [Header("Legibility")]
    [Tooltip("When true, the label scales with distance so its angular size stays constant (it reads the same at 1 m and 8 m).")]
    public bool keepAngularSize = true;

    [Tooltip("Distance in meters at which the label has scale 1. Farther than this it grows, nearer it shrinks.")]
    public float referenceDistance = 2f;

    [Tooltip("Lower and upper clamp for the scale so a pin at 30 m does not become a billboard.")]
    public float minScale = 0.5f;
    public float maxScale = 6f;

    [Header("Parts (auto-created if empty)")]
    [Tooltip("The TextMesh that shows the note.")]
    public TextMesh label;

    [Tooltip("The LineRenderer that draws the leader from anchor to label.")]
    public LineRenderer leader;

    Transform head;

    void Start()
    {
        if (anchor == null) anchor = transform;
        head = Camera.main != null ? Camera.main.transform : null;
        EnsureParts();

        // TODO 1: Show the note. Assign text to label.text.
        //         Look at: TextMesh.text. (The builder already put a placeholder string in the TextMesh;
        //         yours must replace it, otherwise every pin says "Pin: implement TODO 1-4".)
        //         Check: each pin in the scene shows its own note from the Inspector's Text field.
    }

    void LateUpdate()
    {
        if (label == null || anchor == null) return;

        // TODO 2: Float the label above the anchor: label.transform.position = anchor.position + Vector3.up * labelHeight.
        //         Why LateUpdate: the anchor may itself be moved by another script this frame (PropPlacer,
        //         SpinCollectible ...); LateUpdate runs after all Updates so the label never lags a frame behind.
        //         Check: move a pin's anchor object in the Scene view during Play; the label follows.

        // TODO 3: Face the player (review from 1.1). A TextMesh reads from its -z side, so its forward must point AWAY
        //         from the viewer: Vector3 away = label.transform.position - head.position; away.y = 0f;
        //         if (away.sqrMagnitude > 0.0001f) label.transform.rotation = Quaternion.LookRotation(away);
        //         Guard head == null (no camera yet).
        //         Check: walk around a pin; its text always faces you and never tilts.

        // TODO 4: Keep the angular size constant. Angular size is theta = 2 atan(s / 2d): if the label's physical size s
        //         grows in proportion to the distance d, theta stays the same. So:
        //             float d = Vector3.Distance(head.position, label.transform.position);
        //             float scale = keepAngularSize ? Mathf.Clamp(d / referenceDistance, minScale, maxScale) : 1f;
        //             label.transform.localScale = Vector3.one * scale;
        //         Look at: Vector3.Distance, Mathf.Clamp.
        //         Check: read the Mountain pin from Station 5 (12 m) and from Station 1: same apparent size.
        //         Toggle Keep Angular Size off and it shrinks with distance like a real sign.

        DrawLeader();
    }

    /// <summary>Draws the leader line from the anchor to the bottom of the label.</summary>
    void DrawLeader()
    {
        if (leader == null || label == null || anchor == null) return;

        // TODO 5: Feed the LineRenderer two world-space points: the anchor and a point just under the label.
        //             leader.positionCount = 2;
        //             leader.SetPosition(0, anchor.position);
        //             leader.SetPosition(1, label.transform.position + Vector3.down * 0.05f * label.transform.localScale.y);
        //         Look at: LineRenderer.SetPosition, LineRenderer.useWorldSpace (the builder sets it true).
        //         Why a leader at all: in VR a label floating in space is ambiguous — is it about the table or the
        //         wall behind it? The line removes the doubt.
        //         Check: a thin line connects each note to its object; select the pin and drag the anchor to confirm.
    }

    /// <summary>
    /// Creates a label and a leader line as children if none were assigned, so a pin added at runtime
    /// (by PropPlacer) works without any Inspector setup. Plain helper — nothing to implement here.
    /// </summary>
    public void EnsureParts()
    {
        if (label == null)
        {
            var go = new GameObject("Pin Label");
            go.transform.SetParent(transform, false);
            label = go.AddComponent<TextMesh>();
            label.fontSize = 64;
            label.characterSize = 0.02f;
            label.anchor = TextAnchor.LowerCenter;
            label.alignment = TextAlignment.Center;
            label.color = new Color(1f, 0.95f, 0.6f);
            label.text = "Pin: implement TODO 1-5 in AnnotationPin.cs";
        }
        if (leader == null)
        {
            var go = new GameObject("Pin Leader");
            go.transform.SetParent(transform, false);
            leader = go.AddComponent<LineRenderer>();
            leader.useWorldSpace = true;
            leader.positionCount = 2;
            leader.startWidth = 0.004f;
            leader.endWidth = 0.004f;
            leader.startColor = label.color;
            leader.endColor = label.color;
            var shader = Shader.Find("Sprites/Default");
            if (shader != null) leader.material = new Material(shader);
        }
    }
}

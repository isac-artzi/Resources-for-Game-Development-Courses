// GuideVoice.cs — Activity 1.3: Story Beats in Space
// The Mysterious Guide's "voice": a speech label that types out her line character by character, a slow turn
// toward the station she is talking about, and an optional soft blip per few characters. She is a diegetic
// narrator — information delivered by a character in the world, not by a screen overlay.
// Attached to: "Mysterious Guide" in the starter scene (Speech Label is pre-set; Blip is optional).
// Created by Isac Artzi

using UnityEngine;

public class GuideVoice : MonoBehaviour
{
    [Header("Speech")]
    [Tooltip("The TextMesh above the Guide that shows her current line.")]
    public TextMesh speechLabel;

    [Tooltip("Typewriter speed. 30 characters per second is a comfortable reading pace; 60 feels hurried.")]
    public float charactersPerSecond = 30f;

    [Tooltip("Height of the speech label above the Guide's feet, in meters. The Guide is 2 m tall.")]
    public float labelHeight = 2.4f;

    [Header("Attention")]
    [Tooltip("How fast the Guide turns toward the active station, in degrees per second. Slow is deliberate; fast is startled.")]
    public float turnDegreesPerSecond = 90f;

    [Header("Sound (optional)")]
    [Tooltip("An AudioSource with a short click or chime. Leave empty for a silent Guide.")]
    public AudioSource blip;

    [Tooltip("Play one blip every N characters revealed.")]
    public int charactersPerBlip = 4;

    string fullLine = "";
    float revealTimer;
    int revealedCount;
    Transform faceTarget;
    Transform head;

    void Start()
    {
        head = Camera.main != null ? Camera.main.transform : null;
        if (speechLabel != null) speechLabel.text = "";
    }

    /// <summary>Starts speaking a new line and turning toward a target (pass null to just fall silent).</summary>
    public void Say(string line, Transform target)
    {
        fullLine = line ?? "";
        faceTarget = target;

        // TODO 1: Reset the typewriter so the new line starts from its first character.
        //             revealTimer = 0f; revealedCount = 0;
        //             if (speechLabel != null) speechLabel.text = "";
        //         Check: pressing ] mid-sentence cuts the old line and starts the new one from the beginning.
    }

    void Update()
    {
        // TODO 2: Typewriter reveal. Count time and show the first N characters of fullLine:
        //             revealTimer += Time.deltaTime;
        //             int n = Mathf.Min(fullLine.Length, Mathf.FloorToInt(revealTimer * charactersPerSecond));
        //             if (n != revealedCount) {
        //                 revealedCount = n;
        //                 if (speechLabel != null) speechLabel.text = fullLine.Substring(0, n);
        //                 if (blip != null && charactersPerBlip > 0 && n % charactersPerBlip == 0) blip.Play();
        //             }
        //         Look at: String.Substring(int start, int length), Mathf.FloorToInt, Mathf.Min.
        //         Why reveal over time: it paces the player's reading, it signals that speech is happening, and it
        //         is a motion cue that draws the eye to the Guide at the moment she has something to say.
        //         Check: a 90-character line takes about 3 s at 30 cps; setting 200 cps makes it appear almost at once.

        // TODO 3: Turn toward the target, around the vertical axis only, at a limited speed.
        //             if (faceTarget != null) {
        //                 Vector3 to = faceTarget.position - transform.position; to.y = 0f;
        //                 if (to.sqrMagnitude > 0.0001f) {
        //                     Quaternion want = Quaternion.LookRotation(to);
        //                     transform.rotation = Quaternion.RotateTowards(transform.rotation, want, turnDegreesPerSecond * Time.deltaTime);
        //                 }
        //             }
        //         Look at: Quaternion.RotateTowards(from, to, maxDegreesDelta) — a constant angular speed, unlike Slerp with a fixed t.
        //         Check: on each beat the Guide swings to face the lit station over about a second; the capsule's
        //         "nose" (its +z) ends up pointing at the station (use the Scene view gizmo to confirm).
    }

    void LateUpdate()
    {
        // TODO 4: Keep the speech label readable: put it at transform.position + Vector3.up * labelHeight and billboard it
        //         toward the head (away = label.position - head.position; away.y = 0; LookRotation). Guard speechLabel and
        //         head against null. The label is a child of the Guide, so without this it would swing away when she turns in TODO 3.
        //         Check: walk around the Guide while she speaks; the text always faces you and never rotates with her body.
    }
}

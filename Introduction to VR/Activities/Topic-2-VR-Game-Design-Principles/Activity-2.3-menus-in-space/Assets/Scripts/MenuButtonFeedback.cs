// MenuButtonFeedback.cs — Activity 2.3: Menus in Space
// Hover and click feedback for a world-space UI button: a gentle scale punch and color tint while the ray hovers, and
// a short generated beep on click. In VR, buttons have no mouse cursor to confirm "you are on it", so the button
// itself must answer. Works with any pointer the EventSystem knows about — XRI rays, mouse, or touch.
// Attached to: Start Button, Resume Button, Quit Button (each also has an Image, a Button and an AudioSource).
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Scale")]
    [Tooltip("Scale multiplier while hovered. 1.08-1.15 reads as 'alive' without shouting.")]
    public float hoverScale = 1.1f;

    [Tooltip("How fast the scale chases its target (per second).")]
    public float scaleSpeed = 14f;

    [Header("Color")]
    public Color normalColor = new Color(0.20f, 0.30f, 0.50f, 1f);
    public Color hoverColor = new Color(0.35f, 0.55f, 0.85f, 1f);

    [Header("Sound")]
    [Tooltip("Plays the click beep. Pre-wired to the AudioSource on this button.")]
    public AudioSource audioSource;

    [Tooltip("Beep frequency in Hz. 880 = the A above concert A.")]
    public float beepFrequency = 880f;

    [Tooltip("Beep length in seconds.")]
    public float beepDuration = 0.08f;

    Image image;
    Vector3 baseScale;
    float targetScale = 1f;
    AudioClip beep;

    /// <summary>True while a pointer (ray) is over this button.</summary>
    public bool IsHovered { get; private set; }

    void Awake()
    {
        image = GetComponent<Image>();
        baseScale = transform.localScale;
        if (image != null) image.color = normalColor;

        // TODO 4: Build the click sound once: beep = MakeBeep(beepFrequency, beepDuration);  (implement MakeBeep below)
    }

    void Update()
    {
        // TODO 2: Ease the scale toward targetScale using UNSCALED time (the menu is used while paused):
        //             float s = transform.localScale.x / baseScale.x;                 // current multiplier
        //             s = Mathf.Lerp(s, targetScale, 1f - Mathf.Exp(-scaleSpeed * Time.unscaledDeltaTime));
        //             transform.localScale = baseScale * s;
        //         Look at: Time.unscaledDeltaTime, Mathf.Exp, Vector3 * float.
        //         Check: the button grows ~10% within a tenth of a second when the ray enters and shrinks back when it leaves.
    }

    /// <summary>EventSystem callback: the ray (or mouse) moved onto this button.</summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHovered = true;

        // TODO 1: Hover on: targetScale = hoverScale; if (image != null) image.color = hoverColor;
        //         Look at: IPointerEnterHandler (UnityEngine.EventSystems). XRI's XRUIInputModule turns ray hovers into the
        //         same pointer events a mouse would send, so one script serves desktop and headset.
        //         Check: the button tints light blue the moment the ray touches it.
    }

    /// <summary>EventSystem callback: the pointer left this button.</summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovered = false;

        // TODO 1 (continued): Hover off: targetScale = 1f; image.color = normalColor.
    }

    /// <summary>EventSystem callback: the button was clicked (UI Press on the ray, or a mouse click).</summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // TODO 3: Click feedback: a quick punch and the beep.
        //             transform.localScale = baseScale * (hoverScale * 0.92f);   // dip, then Update eases back up
        //             if (audioSource != null && beep != null) audioSource.PlayOneShot(beep, 0.6f);
        //         Look at: AudioSource.PlayOneShot(AudioClip, float volumeScale).
        //         Why the dip: motion that starts the instant you press is what makes a virtual button feel physical.
        //         Check: pressing Trigger on a button gives a visible bounce and a short beep from the button's position.
    }

    /// <summary>
    /// Generates a short sine-wave beep with a linear fade-out so it does not click at the end.
    /// </summary>
    public static AudioClip MakeBeep(float frequency, float duration)
    {
        // TODO 4 (continued):
        //   int rate = AudioSettings.outputSampleRate;             // usually 48000
        //   int n = Mathf.Max(1, Mathf.RoundToInt(rate * duration));
        //   var data = new float[n];
        //   for (int i = 0; i < n; i++) {
        //       float t = (float)i / rate;                          // seconds
        //       float fade = 1f - (float)i / n;                     // 1 -> 0
        //       data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * fade * 0.5f;
        //   }
        //   var clip = AudioClip.Create("Beep", n, 1, rate, false);
        //   clip.SetData(data, 0);
        //   return clip;
        // Look at: AudioClip.Create(name, lengthSamples, channels, frequency, stream), AudioClip.SetData, Mathf.Sin.
        // Math: a tone is y(t) = A sin(2 pi f t); at 880 Hz and 48 kHz one cycle spans 48000/880 = 54.5 samples.
        // Check: the beep is a clean short tone; set frequency 440 and it drops an octave.
        return null;
    }
}

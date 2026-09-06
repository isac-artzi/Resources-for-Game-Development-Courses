// AmbientLayer.cs — Activity 6.2: Soundscapes and Music Zones
// One environment's ambience: a constant "bed" loop (wind, water, room tone) that fades in and out, plus
// "detail" one-shots (a bird, a rock fall, a drip) fired at random times, at random positions around the
// player, with a little random pitch — so the same three clips never sound like a loop.
// Attached to: Forest Ambience, Mountain Ambience, Ruins Ambience (children of "Ambience" in the starter scene).
// Created by Isac Artzi

using UnityEngine;

public class AmbientLayer : MonoBehaviour
{
    [Header("Bed (constant layer)")]
    [Tooltip("Looping AudioSource for the bed. Set by the builder (no clip; a placeholder hum is used until you assign one).")]
    public AudioSource bed;

    [Tooltip("Bed volume when this layer is active.")]
    [Range(0f, 1f)]
    public float bedVolume = 0.5f;

    [Tooltip("Seconds for the bed to fade fully in or out.")]
    public float fadeSeconds = 2f;

    [Tooltip("Placeholder frequency for the bed while its clip is empty.")]
    public float placeholderBedHz = 110f;

    [Header("Detail one-shots (random layer)")]
    [Tooltip("Short clips: bird chirps, distant rockfall, water drips. Leave empty to hear placeholder blips.")]
    public AudioClip[] oneShots;

    [Tooltip("Average seconds between one-shots. Actual gaps are random (exponential) around this mean.")]
    public float meanSecondsBetween = 4f;

    [Tooltip("Never fire two one-shots closer together than this.")]
    public float minSecondsBetween = 0.5f;

    [Tooltip("Random pitch multiplier range for each one-shot.")]
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    [Tooltip("One-shots are placed this far (min..max meters) from the player's head, at a random compass direction.")]
    public Vector2 distanceRange = new Vector2(3f, 8f);

    [Tooltip("Height range (meters above the floor) for one-shot positions. Birds high, drips low.")]
    public Vector2 heightRange = new Vector2(1f, 4f);

    [Tooltip("How many one-shots may overlap. Each voice is a pooled 3D AudioSource created at Start.")]
    [Range(1, 8)]
    public int voices = 3;

    [Tooltip("Placeholder frequency for one-shots while the array is empty.")]
    public float placeholderOneShotHz = 880f;

    [Header("Debug")]
    [Tooltip("Draw a short debug ray where each one-shot is placed (Scene view, Play mode).")]
    public bool drawOneShotPositions = true;

    /// <summary>True while this layer is the current zone's ambience.</summary>
    public bool IsActive { get; private set; }

    /// <summary>Where the bed volume is heading (bedVolume when active, 0 when not).</summary>
    public float TargetBedVolume { get { return targetBedVolume; } }

    float targetBedVolume;
    float nextOneShotTime;
    AudioSource[] pool;
    int nextVoice;
    Transform head;
    AudioClip placeholderBlip;

    void Start()
    {
        if (Camera.main != null) head = Camera.main.transform;

        if (bed != null)
        {
            bed.loop = true;
            bed.playOnAwake = false;
            if (bed.clip == null) bed.clip = MusicZoneManager.PlaceholderChord(placeholderBedHz, 2f);
            bed.volume = 0f;
        }

        // Pool of 3D voices for the one-shots. A one-shot must not live on the bed source: PlayOneShot on a
        // looping source shares its position, pitch and volume — you want each detail sound to have its own.
        pool = new AudioSource[Mathf.Max(1, voices)];
        for (int i = 0; i < pool.Length; i++)
        {
            var go = new GameObject("Detail Voice " + (i + 1));
            go.transform.SetParent(transform, false);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 1f;
            src.rolloffMode = AudioRolloffMode.Logarithmic;
            src.minDistance = 2f;
            src.maxDistance = 30f;
            pool[i] = src;
        }
    }

    /// <summary>Turns this layer on (bed fades in, one-shots start) or off (bed fades out, one-shots stop).</summary>
    public void SetActive(bool on)
    {
        IsActive = on;
        targetBedVolume = on ? bedVolume : 0f;
        if (on && bed != null && !bed.isPlaying) bed.Play();
        if (on) ScheduleNext();
    }

    void Update()
    {
        // TODO 1: Fade the bed toward its target and stop it when it has faded out.
        //             if (bed != null)
        //             {
        //                 float step = Time.deltaTime * Mathf.Max(0.01f, bedVolume) / Mathf.Max(0.05f, fadeSeconds);
        //                 bed.volume = Mathf.MoveTowards(bed.volume, targetBedVolume, step);
        //                 if (!IsActive && bed.volume <= 0f && bed.isPlaying) bed.Stop();
        //             }
        //         Look at: Mathf.MoveTowards. Why stop when silent: a stopped source costs nothing; a silent playing one
        //         still takes a mixer voice (Unity's default limit is 32 real voices).
        //         Check: entering a zone, its bed rises over fadeSeconds; leaving, it falls and "playing" disappears from
        //         the Audio Director readout (add a Debug.Log if you want to see the Stop).

        // TODO 3: Fire one-shots on schedule.
        //             if (IsActive && Time.time >= nextOneShotTime) { PlayOneShot(); ScheduleNext(); }
        //         Check: in the Forest you hear a blip every few seconds — sometimes two close together, sometimes a long
        //         gap — never a metronome.
    }

    /// <summary>Picks the time of the next one-shot using an exponential inter-arrival time.</summary>
    void ScheduleNext()
    {
        // TODO 2: Exponential inter-arrival — the natural model for "random, independent events".
        //             float u = Random.value;                                   // 0..1
        //             float wait = -Mathf.Log(1f - u) * meanSecondsBetween;     // inverse CDF of the exponential distribution
        //             wait = Mathf.Max(minSecondsBetween, wait);
        //             nextOneShotTime = Time.time + wait;
        //         Look at: Random.value, Mathf.Log (natural log). Why: the mean of the waits is meanSecondsBetween, but
        //         short gaps are common and long gaps happen too, which is how real birds behave. Guard: Mathf.Log(0) is
        //         -infinity; Random.value can return exactly 1, so use Mathf.Clamp(u, 0f, 0.999f) if you are careful.
        //         Check: log the waits for a minute; their average is near meanSecondsBetween and they are all different.
        nextOneShotTime = Time.time + meanSecondsBetween;   // placeholder: a boring, regular beat — replace it
    }

    /// <summary>Plays one detail sound from a random position around the head with a random pitch.</summary>
    void PlayOneShot()
    {
        if (pool == null || pool.Length == 0) return;
        var voice = pool[nextVoice];
        nextVoice = (nextVoice + 1) % pool.Length;

        AudioClip clip = null;
        if (oneShots != null && oneShots.Length > 0) clip = oneShots[Random.Range(0, oneShots.Length)];
        if (clip == null)
        {
            if (placeholderBlip == null) placeholderBlip = MusicZoneManager.PlaceholderChord(placeholderOneShotHz, 0.5f);
            clip = placeholderBlip;
        }

        // TODO 4: Position and pitch.
        //             Vector3 center = head != null ? head.position : transform.position;
        //             float angle = Random.Range(0f, Mathf.PI * 2f);
        //             float dist  = Random.Range(distanceRange.x, distanceRange.y);
        //             Vector3 pos = new Vector3(center.x + Mathf.Cos(angle) * dist,
        //                                       Random.Range(heightRange.x, heightRange.y),
        //                                       center.z + Mathf.Sin(angle) * dist);
        //             voice.transform.position = pos;
        //             voice.pitch = Random.Range(pitchRange.x, pitchRange.y);
        //             if (drawOneShotPositions) Debug.DrawRay(pos, Vector3.up, Color.yellow, 1f);
        //         Look at: Random.Range(float, float), Debug.DrawRay. Why around the HEAD and not the zone center: the
        //         player should feel surrounded wherever they stand in the zone.
        //         Check: with the Scene view open, yellow ticks appear 3–8 m around you, each blip comes from a different
        //         direction, and pitches vary slightly. Set pitchRange to (1,1) to hear how mechanical it becomes.
        voice.transform.position = transform.position;   // placeholder: everything from one spot — replace it
        voice.pitch = 1f;

        voice.PlayOneShot(clip, 1f);
    }
}

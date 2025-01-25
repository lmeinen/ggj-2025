using System.Collections;
using System.Linq;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Tooltip("GameObject name of the happy clip")]
    public string HAPPY_CLIP_NAME = "Happy";

    [Tooltip("GameObject name of the hell clip")]
    public string HELL_CLIP_NAME = "Hell";

    [Tooltip("GameObject name of the glitch clip")]
    public string GLITCH_CLIP_NAME = "Glitch";

    [Tooltip("audio clip fade transition duration in seconds")]
    [SerializeField] private float HAPPY_FADE_DURATION = 1f;

    [Tooltip("audio clip fade transition duration in seconds")]
    [SerializeField] private float GLITCH_FADE_DURATION = 5f;

    private AudioSource happy;
    private AudioSource hell;
    private AudioSource glitch;
    private bool _isFading;

    void Awake()
    {
        AudioSource[] audioClips = GetComponentsInChildren<AudioSource>();
        happy = audioClips.First(clip => clip.name == HAPPY_CLIP_NAME);
        hell = audioClips.First(clip => clip.name == HELL_CLIP_NAME);
        glitch = audioClips.First(clip => clip.name == GLITCH_CLIP_NAME);
        happy.Play();
        hell.Stop();
        _isFading = false;
        DontDestroyOnLoad(gameObject); // Keep this GameObject alive across scenes
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.I.Finished && happy.isPlaying)
        {
            // start glitch
            StartCoroutine(FadeToGlitch(happy, glitch, HAPPY_FADE_DURATION));
        }

        if (glitch.isPlaying && glitch.time >= glitch.clip.length - GLITCH_FADE_DURATION && !_isFading)
        {
            // start fading out glitch for hell sound
            StartCoroutine(Crossfade(glitch, hell, GLITCH_FADE_DURATION));
            _isFading = true;
        }
    }

    private IEnumerator FadeToGlitch(AudioSource from, AudioSource to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            from.volume = Mathf.Lerp(1f, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        from.Stop();
        from.volume = 1f;
        to.volume = 1f;
        to.Play();
        Game.I.StartGlitch();
    }

    private IEnumerator Crossfade(AudioSource from, AudioSource to, float duration)
    {
        to.volume = 0f;
        to.Play();

        float elapsed = 0f;

        while (elapsed < duration)
        {
            from.volume = Mathf.Lerp(1f, 0f, elapsed / duration);
            to.volume = Mathf.Lerp(0f, 1f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        from.Stop();
        from.volume = 1f;
        to.volume = 1f;
    }
}

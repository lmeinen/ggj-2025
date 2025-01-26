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

    [Tooltip("music audio volume")]
    [SerializeField] private float MUSIC_VOLUME = 0.7f;

    public static MusicManager I { get; private set; }

    private AudioSource happy;
    private AudioSource hell;
    private AudioSource glitch;

    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (I != null && I != this)
        {
            Destroy(this);
            return;
        }

        I = this;
        AudioSource[] audioClips = GetComponentsInChildren<AudioSource>();
        happy = audioClips.First(clip => clip.name == HAPPY_CLIP_NAME);
        hell = audioClips.First(clip => clip.name == HELL_CLIP_NAME);
        glitch = audioClips.First(clip => clip.name == GLITCH_CLIP_NAME);
        happy.volume = MUSIC_VOLUME;
        hell.volume = MUSIC_VOLUME;
        glitch.volume = MUSIC_VOLUME;
        happy.Play();
        hell.Stop();
        DontDestroyOnLoad(gameObject); // Keep this GameObject alive across scenes
    }

    public void StartHappy()
    {
        glitch.Stop();
        hell.Stop();
        happy.Play();
    }

    public void StartHell()
    {
        happy.Stop();
        glitch.Stop();
        hell.Play();
    }

    public void StartGlitch() {
        happy.Stop();
        hell.Stop();
        glitch.Play();
    }
}

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public float minPitch = 0.9f;    // Minimum pitch variation
    public float maxPitch = 1.1f;    // Maximum pitch variation

    private AudioSource audioSource;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip, bool randomizePitch)
    {
        if (randomizePitch)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);

        }

        Debug.Log("Playing sound with pitch " + audioSource.pitch);
        audioSource.PlayOneShot(clip);

        // reset pitch
        audioSource.pitch = 1f;
    }
}

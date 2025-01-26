using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public enum State
{
    TUTORIAL,
    WAVE_TEXT,
    IN_WAVE,
    GLITCHING
}

//The game Singleton. Here, you can store references to objects that need to be globally accessible
public class Game : MonoBehaviour
{
    // we separate objects' rendering from their logic, and group their rendering styles into layers
    public const string BLUE_PILL_LAYER = "BluePill";
    public const string RED_PILL_LAYER = "RedPill";

    public static Game I { get; private set; }
    public bool Finished { get => _finished; private set => _finished = value; }


    public ParticleSystem bubblePopParticles;
    public ParticleSystem bulletPopParticles;
    public ParticleSystem splatterParticles;
    public float popParticleSpeed = 10;
    private EnemySpawner _enemySpawner;

    public InputActionReference _startGameAction;
    public GameObject _tutorialObject;

    private float _textDuration = 3f;
    public float TextDuration { get => _textDuration; private set => _textDuration = value; }


    public Volume globalVolume;
    public VolumeProfile bluePillProfile;
    public VolumeProfile redPillProfile;


    Camera _mainCamera;
    public Camera MainCamera => _mainCamera;
    bool _finished = false;

    private State _gameState;
    public State GameState { get => _gameState; private set => _gameState = value; }

    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (I != null && I != this)
        {
            Destroy(this);
        }
        else
        {
            I = this;
            AddRenderLayer(BLUE_PILL_LAYER);
            RemoveRenderLayer(RED_PILL_LAYER);
        }
    }


    private void Start()
    {
        _mainCamera = Camera.main;
        _enemySpawner = FindAnyObjectByType<EnemySpawner>();
        GameState = State.TUTORIAL;
    }


    void Update()
    {
        switch (GameState)
        {
            case State.TUTORIAL:
                if (_startGameAction.action.ReadValue<Vector2>().sqrMagnitude != 0)
                {
                    _tutorialObject.SetActive(false);
                    GameState = State.WAVE_TEXT;
                    _enemySpawner.ShowWaveText();
                    TextDuration = 3f;
                }
                break;
            case State.WAVE_TEXT:
                TextDuration -= Time.deltaTime;
                if (TextDuration <= 0)
                {
                    _enemySpawner.SpawnWave();
                    GameState = State.IN_WAVE;
                }
                break;
            case State.IN_WAVE:
                if (Input.GetKeyDown(KeyCode.C))
                {
                    // Kill all enemies (for testing purposes, of course)
                    _enemySpawner.KillEveryone();
                }

                if (IsWaveFinished())
                {
                    StartCoroutine(StartGlitch(_enemySpawner.GlitchDuration(), _enemySpawner.PlayGlitchSound()));
                    GameState = State.GLITCHING;
                }
                break;
            case State.GLITCHING:
                // do nothing - handled by coroutine
                break;
        }


        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            StartCoroutine(StartGlitch(5f, true));
        }
    }

    public IEnumerator StartGlitch(float durationS, bool glitchSound)
    {
        Debug.Log($"Playing glitch for {durationS} seconds with sound = {glitchSound}");

        if (glitchSound)
        {
            StartCoroutine(MusicManager.I.FadeToGlitch());
        }
        else
        {
            MusicManager.I.StartHell();
        }

        // switch rendering style
        DestroyAllBubbles();
        _enemySpawner.CleanupWave();
        RemoveRenderLayer(BLUE_PILL_LAYER);
        AddRenderLayer(RED_PILL_LAYER);
        globalVolume.profile = redPillProfile;

        // Wait for the specified duration
        yield return new WaitForSeconds(durationS);

        // switch back to happy land
        RemoveRenderLayer(RED_PILL_LAYER);
        AddRenderLayer(BLUE_PILL_LAYER);
        MusicManager.I.StartHappy();
        globalVolume.profile = bluePillProfile;

        // After glitch, start next wave
        _enemySpawner.ShowWaveText();
        GameState = State.WAVE_TEXT;
        TextDuration = 3f;
    }

    public void DestroyAllBubbles()
    {
        // Find all objects of type Bubble
        Bubble[] bubbles = FindObjectsByType<Bubble>(FindObjectsSortMode.None);

        // Loop through and destroy each one
        foreach (Bubble bubble in bubbles)
        {
            Destroy(bubble.gameObject);
        }
    }

    public static GameObject CreateShot(GameObject g, Vector3 pos, Quaternion rot, Vector3 speed, float spread, Vector3 parent_vel)
    {
        GameObject shot = Instantiate(g, pos, rot, I.transform);
        shot.GetComponent<Shot>().Initialize(speed, spread, parent_vel);
        return shot;
    }

    // Method to add a specific layer
    void AddRenderLayer(string layerName)
    {
        Camera mainCamera = Camera.main;
        int layerIndex = LayerMask.NameToLayer(layerName);
        mainCamera.cullingMask |= 1 << layerIndex;
    }

    // Method to remove a specific layer
    void RemoveRenderLayer(string layerName)
    {
        Camera mainCamera = Camera.main;
        int layerIndex = LayerMask.NameToLayer(layerName);
        mainCamera.cullingMask &= ~(1 << layerIndex);
    }

    private bool IsWaveFinished()
    {
        int noOfLiveEnemies = _enemySpawner.NoOfLiveEnemies();
        Debug.Log("No of live enemies " + noOfLiveEnemies);
        return noOfLiveEnemies <= 0;
    }
}

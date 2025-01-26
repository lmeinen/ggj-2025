using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;
using UnityEngine.InputSystem;

[Serializable]
public class Spawn
{
    public int enemy_count;
    public Transform spawn;
}

[Serializable]
public class EnemyWave
{
    public List<Spawn> spawn;
}


enum SpawnerState
{
    TUTORIAL,
    WAVE_TEXT,
    IN_WAVE
}


public class EnemySpawner : MonoBehaviour
{
    public List<EnemyWave> enemyWaves;
    public GameObject enemyPrefab;

    readonly List<Enemy> _currentEnemies = new();


    public TextMeshProUGUI _waveText;
    float _textDuration = 3f;


    public InputActionReference _startGameAction;
    public GameObject _tutorialObject;

    SpawnerState state = SpawnerState.TUTORIAL;


    int _waveIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        

        if (state == SpawnerState.TUTORIAL)
        {
            if (_startGameAction.action.ReadValue<Vector2>().sqrMagnitude != 0)
            {
                _tutorialObject.SetActive(false);
                ToShowWave();
            }
            return;
        }

        if (state == SpawnerState.WAVE_TEXT)
        {
            _textDuration -= Time.deltaTime;
            if (_textDuration < 0f)
            {
                ToInWave();
            }
        }

        if (state == SpawnerState.IN_WAVE)
        {
            if (_currentEnemies.Count == 0 || _currentEnemies.All(x => x.Dead))
            {
                ToShowWave();
            }
        }
    }

    private void ToShowWave()
    {
        state = SpawnerState.WAVE_TEXT;
        _textDuration = 3f;
        _waveText.gameObject.SetActive(true);
        _waveText.text = $"Wave {_waveIndex + 1}";
    }

    private void ToInWave()
    {
        state = SpawnerState.IN_WAVE;
        _waveText.gameObject.SetActive(false);
        _waveText.gameObject.SetActive(false);

        _currentEnemies.Clear();

        foreach (var spawn in enemyWaves[_waveIndex].spawn)
        {
            for (int i = 0; i < spawn.enemy_count; i++)
            {
                var enemy = Instantiate(enemyPrefab, spawn.spawn.position, spawn.spawn.rotation).GetComponent<Enemy>();
                _currentEnemies.Add(enemy);
            }
        }

        _waveIndex++;
    }
}

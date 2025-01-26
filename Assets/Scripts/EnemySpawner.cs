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
    public float glitchDuration;
    public bool playGlitchSound;
}


public class EnemySpawner : MonoBehaviour
{
    readonly List<Enemy> _currentEnemies = new();

    public List<EnemyWave> enemyWaves;
    public GameObject enemyPrefab;
    public TextMeshProUGUI _waveText;
    public InputActionReference _startGameAction;
    public GameObject _tutorialObject;
    public Bubble bubblePrefab;

    int _waveIndex = 0;

    public void ShowWaveText()
    {
        _waveText.gameObject.SetActive(true);
        _waveText.text = $"Wave {_waveIndex + 1}";
    }

    public void KillEveryone()
    {
        _currentEnemies.ForEach(enemy => enemy.HitByBubble(Instantiate(bubblePrefab, enemy.transform.position, Quaternion.identity, Game.I.transform)));
    }

    public int NoOfLiveEnemies()
    {
        return _currentEnemies.Count(enemy => !enemy.Dead);
    }

    public float GlitchDuration()
    {
        return enemyWaves[_waveIndex - 1].glitchDuration;
    }

    public bool PlayGlitchSound()
    {
        return enemyWaves[_waveIndex - 1].playGlitchSound;
    }

    public void SpawnWave()
    {
        _waveText.gameObject.SetActive(false);
        _waveText.gameObject.SetActive(false);

        _currentEnemies.ForEach(enemy => Destroy(enemy));
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

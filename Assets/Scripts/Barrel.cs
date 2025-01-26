using Unity.Cinemachine;
using UnityEngine;

public class Barrel : MonoBehaviour
{

    public float bubbleAmount = 100f;
    public float bubbleSpeed = 3f;
    public GameObject bubblePrefab;
    public GameObject enemyShotPrefab;

    public float respawnDuration = 5f;
    float _respawnTimer = 0f;

    public CinemachineImpulseSource _explosionScreenShake;

    public AudioClip explosionSound;
    public float explosionVolume;


    private void Start()
    {
    }

    private void Update()
    {

        _respawnTimer -= Time.deltaTime;
        if (_respawnTimer < 0f)
        {
            transform.localScale = Vector3.one * Mathf.Min(1, transform.localScale.x + Time.deltaTime / respawnDuration / 0.2f);
        }

    }


    private void OnTriggerEnter(Collider collision)
    {
        float size = transform.localScale.x;
        if (size < 0.5f) return;

        bool is_bubble = collision.CompareTag("Bubble");
        bool is_enemy_shot = collision.CompareTag("EnemyShot");

        if (is_bubble || is_enemy_shot)
        {
            if (explosionSound != null)
            {
                SoundManager.Instance.PlaySound(explosionSound, explosionVolume, true);
            }
            _explosionScreenShake.GenerateImpulse();
            transform.localScale = Vector3.one * 0.1f;
            _respawnTimer = respawnDuration * 0.8f;
            int spawn_amount = (int)(bubbleAmount * size * size * size);
            for (int i = 0; i < spawn_amount; i++)
            {
                Vector3 speed = bubbleSpeed * Random.insideUnitSphere;
                if (speed.y < 0) speed.y = -speed.y;

                Quaternion rot = is_bubble ? Quaternion.identity : Quaternion.LookRotation(speed, Random.insideUnitSphere);

                Game.CreateShot(is_bubble ? bubblePrefab : enemyShotPrefab, transform.position, rot, speed, 0f, Vector3.zero);
            }
        }
    }


}

using Unity.Cinemachine;
using UnityEngine;

public class Barrel : MonoBehaviour
{

    public float bubbleAmount = 100f;
    public float bubbleSpeed = 3f;
    public GameObject bubblePrefab;
    public GameObject enemyShotPrefab;


    public CinemachineImpulseSource _explosionScreenShake;

    private void OnTriggerEnter(Collider collision)
    {
        bool is_bubble = collision.CompareTag("Bubble");
        bool is_enemy_shot = collision.CompareTag("EnemyShot");

        if (is_bubble || is_enemy_shot)
        {
            _explosionScreenShake.GenerateImpulse();
            Destroy(gameObject);
            for (int i = 0; i < bubbleAmount; i++)
            {
                Vector3 speed = bubbleSpeed * Random.insideUnitSphere;
                if (speed.y < 0) speed.y = -speed.y;

                Quaternion rot = is_bubble ? Quaternion.identity : Quaternion.LookRotation(speed, Random.insideUnitSphere);

                Game.CreateShot(is_bubble ? bubblePrefab : enemyShotPrefab, transform.position, rot, speed, 0f, Vector3.zero);
            }
        }
    }
}

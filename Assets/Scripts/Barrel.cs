using UnityEngine;

public class Barrel : MonoBehaviour
{

    public float bubbleAmount = 100f;
    public float bubbleSpeed = 3f;
    public GameObject bubblePrefab;
    public GameObject enemyShotPrefab;


    private void OnTriggerEnter(Collider collision)
    {
        bool is_bubble = collision.CompareTag("Bubble");
        bool is_enemy_shot = collision.CompareTag("EnemyShot");

        if (is_bubble || is_enemy_shot)
        {
            Destroy(gameObject);
            for (int i = 0; i < bubbleAmount; i++)
            {
                Vector3 speed = bubbleSpeed * Random.insideUnitSphere;

                Quaternion rot = is_bubble ? Quaternion.identity : Quaternion.LookRotation(speed, Random.insideUnitSphere);

                LevelState.CreateShot(is_bubble ? bubblePrefab : enemyShotPrefab, transform.position, rot, speed, 0f, Vector3.zero);
            }
        }
    }
}

using UnityEngine;

public class Barrel : MonoBehaviour
{

    public float bubbleAmount = 100f;
    public float bubbleSpeed = 3f;
    public GameObject bubblePrefab;


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Bubble") || collision.CompareTag("EnemyShot"))
        {
            Destroy(gameObject);
            for (int i = 0; i < bubbleAmount; i++)
            {
                Vector3 speed = bubbleSpeed * Random.insideUnitSphere;
                LevelState.CreateShot(bubblePrefab, transform.position, Quaternion.LookRotation(speed, Vector3.up), speed, 0f);
            }
        }
    }
}

using UnityEngine;

public class Barrel : MonoBehaviour
{

    public float bubbleAmount = 100f;
    public float bubbleSpeed = 3f;
    public GameObject bubblePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

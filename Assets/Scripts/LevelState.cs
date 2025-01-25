using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;



//The game Singleton. Here, you can store references to objects that need to be globally accessible
public class LevelState : MonoBehaviour
{
    public static LevelState I { get; private set; }
    public bool Finished { get => _finished; private set => _finished = value; }


    public ParticleSystem bubblePopParticles;
    public float popParticleSpeed = 10;



    Camera _mainCamera;
    bool _finished = false;
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
        }
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }




    void Update()
    {
        // find all instances of Enemy prefabs in the current scene
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        // log the number of enemies found
        // Debug.Log($"Scanned for enemies [no={enemies.Length};alive={enemies.Count(enemy => !enemy.Dead)}]");

        // If you want to do something with each enemy
        foreach (Enemy enemy in enemies)
        {
            // Example: You could access specific properties or call methods on each enemy
            // Debug.Log($"=== Enemy found [position={enemy.transform.position};alive={enemy.Dead}]");
        }

        if (enemies.Count(enemy => !enemy.Dead) == 0)
        {
            Finished = true;
        }
    }

    public static GameObject CreateShot(GameObject g, Vector3 pos, Quaternion rot, Vector3 speed, float spread)
    {
        GameObject shot = Instantiate(g, pos, rot, I.transform);
        shot.GetComponent<Shot>().Initialize(speed, spread);
        return shot;
    }

    public static void BubblePop(Vector3 pos, Color color)
    {
        Vector3 r = I._mainCamera.transform.right * I.popParticleSpeed;
        Vector3 u = I._mainCamera.transform.up * I.popParticleSpeed;

        for (int i = 0; i < 40; i++)
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            ParticleSystem.EmitParams ps = new()
            {
                position = pos,
                velocity = Mathf.Sin(angle) * r + Mathf.Cos(angle) * u,
                startColor = color
            };
            I.bubblePopParticles.Emit(ps, 1);
        } 
    }
}

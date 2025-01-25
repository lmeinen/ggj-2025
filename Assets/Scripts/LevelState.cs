using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;



//The game Singleton. Here, you can store references to objects that need to be globally accessible
public class LevelState : MonoBehaviour
{
    public static LevelState I { get; private set; }
    public bool Finished { get => _finished; private set => _finished = value; }

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
}

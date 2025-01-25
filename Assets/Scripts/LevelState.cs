using System.Linq;
using UnityEngine;



//The game Singleton. Here, you can store references to objects that need to be globally accessible
public class LevelState : MonoBehaviour
{
    // we separate objects' rendering from their logic, and group their rendering styles into layers
    public const string BLUE_PILL_LAYER = "BluePill";
    public const string RED_PILL_LAYER = "RedPill";

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
            AddRenderLayer(BLUE_PILL_LAYER);
            RemoveRenderLayer(RED_PILL_LAYER);
        }
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }




    void Update()
    {
        if (IsLevelFinished())
        {
            Finished = true;
            
            // switch rendering style
            RemoveRenderLayer(BLUE_PILL_LAYER);
            AddRenderLayer(RED_PILL_LAYER);
        }
    }

    public static GameObject CreateShot(GameObject g, Vector3 pos, Quaternion rot, Vector3 speed, float spread, Vector3 parent_vel)
    {
        GameObject shot = Instantiate(g, pos, rot, I.transform);
        shot.GetComponent<Shot>().Initialize(speed, spread, parent_vel);
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

    private bool IsLevelFinished() {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        return enemies.Count(enemy => !enemy.Dead) == 0;
    }
}

using UnityEngine;

public class Bubble : Shot
{
    //whether this bubble has hit an enemy
    bool hit_enemy = false;
    //the position and size of the hit enemy
    Vector3 hit_enemy_pos;
    float hit_enemy_size;

    [Tooltip("How quickly to converge to the hit enemy"), Range(0.01f, 1f)]
    public float afterHitConvergeRate = 0.5f;

    [Tooltip("Audioclip to use (as base - pitch variations are randomized) when bubble pops")]
    public AudioClip popSound;

    [Tooltip("Array with colors the bubbles should have")]
    [SerializeField] private Color[] colors = new Color[] {  };
    protected Material _mat;

    protected override bool CanBeDestroyed => !hit_enemy;
    protected override Color Color => _mat.color;

    //Initialize the bubble - give it the initial velocity and spread
    public override void Initialize(Vector3 speed, float spread, Vector3 parent_vel)
    {
        base.Initialize(speed, spread, parent_vel);
        _mat = GetComponent<MeshRenderer>().material;
        //assign a random, bright, color
        //_mat.color = Random.ColorHSV(0, 1, 1, 1, 0.5f, 0.5f, 0.9f, 0.9f);
        int rand = Random.Range(0, colors.Length);
        _mat.color = colors[rand];
    }

    protected override AudioClip HitSound() => popSound;

    // Update is called once per frame
    protected override void MyUpdate()
    {
        base.MyUpdate();
        if (hit_enemy)
        {
            //converge to the hit enemy at a given rate (afterHitConvergeRate percent each second)
            float K = Mathf.Exp(Mathf.Log(afterHitConvergeRate) * Time.deltaTime);

            transform.position = Vector3.Lerp(hit_enemy_pos, transform.position, K);
            transform.localScale = Vector3.Lerp(Vector3.one * hit_enemy_size, transform.localScale, K);
        }
        //send the velocity to the shader (fast bubbles are stretched a bit)
        _mat.SetVector("_Velocity", transform.worldToLocalMatrix * _rb.linearVelocity);
    }


    //When the bubble hits an enemy, start playing the surround animation and disable rigidbody simulation
    public void HitEnemy(Enemy enemy)
    {
        _rb.isKinematic = true;
        hit_enemy = true;
        hit_enemy_pos = enemy.Center;
        hit_enemy_size = enemy.transform.localScale.x * 4f;
        GetComponent<Collider>().enabled = false;
    }
}

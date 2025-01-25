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

    protected Material _mat;

    protected override bool CanBeDestroyed => !hit_enemy;


    //Initialize the bubble - give it the initial velocity and spread
    public override void Initialize(Vector3 speed, float spread, Vector3 parent_vel)
    {
        base.Initialize(speed, spread, parent_vel);
        _mat = GetComponent<MeshRenderer>().material;
        //assign a random, bright, color
        _mat.color = Random.ColorHSV(0, 1, 1, 1, 0.5f, 0.5f, 0.6f, 0.6f);
    }

    // Update is called once per frame
    protected override void MyUpdate()
    {
        base.MyUpdate();
        //if the bubble didn't hit an enemy, make it drift up slightly
        if (!hit_enemy)
        {
            _rb.AddForce(-0.03f * Time.deltaTime * Physics.gravity, ForceMode.VelocityChange);
        }
        else
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
        hit_enemy_pos = enemy.transform.position;
        hit_enemy_size = enemy.transform.localScale.x * 4f;
    }

    protected override void WhenDestroyed()
    {
        base.WhenDestroyed();
        SpawnParticles(LevelState.I.bubblePopParticles, _mat.color);
    }
}

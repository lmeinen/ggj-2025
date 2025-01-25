using UnityEngine;

public class Shot : MonoBehaviour
{
    protected Rigidbody _rb;

    float _lifetime;


    public float lifetimeFrom = 3f;
    public float lifetimeTo = 7f;

    protected virtual bool CanBeDestroyed => true;
    protected virtual Color Color => Color.white;


    public virtual void Initialize(Vector3 speed, float spread, Vector3 parent_vel)
    {
        _lifetime = Random.Range(lifetimeFrom, lifetimeTo);

        _rb = GetComponent<Rigidbody>();
        
        Vector3 right = new (-speed.z, 0, speed.x);
        float angle = Random.Range(-spread/2, spread/2);
        _rb.linearVelocity = Mathf.Sin(angle) * right + Mathf.Cos(angle) * speed + parent_vel;

        transform.localScale *= Random.Range(0.5f, 1.0f);        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_rb == null) Initialize(Vector3.zero, 0, Vector3.zero);
    }

    void Update() => MyUpdate();

    protected virtual void MyUpdate()
    {
        _lifetime -= Time.deltaTime;
        if (_lifetime < 0 && CanBeDestroyed)
        {
            WhenDestroyed();
            Destroy(gameObject);
        }
    }

    public virtual void HitSomething(Collider hit) { }

    protected virtual void WhenDestroyed() {
        Destroy(gameObject);
        SpawnParticles(Game.I.bubblePopParticles, Color);
    }


    protected void SpawnParticles(ParticleSystem s, Color c)
    {
        Vector3 pos = transform.position;

        Vector3 r = Game.I.MainCamera.transform.right * Game.I.popParticleSpeed;
        Vector3 u = Game.I.MainCamera.transform.up * Game.I.popParticleSpeed;

        for (int i = 0; i < 40; i++)
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            ParticleSystem.EmitParams ps = new()
            {
                position = pos,
                velocity = Mathf.Sin(angle) * r + Mathf.Cos(angle) * u,
                startColor = c
            };
            s.Emit(ps, 1);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            WhenDestroyed();
            Destroy(gameObject);

            ParticleSystem.EmitParams p = new()
            {
                position = collision.GetContact(0).point + collision.GetContact(0).normal * 0.02f,
                rotation3D = -Quaternion.LookRotation(collision.GetContact(0).normal).eulerAngles,
                startColor = Color
            };
            Game.I.splatterParticles.Emit(p, 1);
        }
        else
        {
            HitSomething(collision.collider);
        }
    }
}

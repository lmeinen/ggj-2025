using Unity.VisualScripting;
using UnityEngine;

public class Shot : MonoBehaviour
{
    protected Rigidbody _rb;

    float _lifetime;


    public float lifetimeFrom = 3f;
    public float lifetimeTo = 7f;

    public float sizeFrom = .5f;
    public float sizeTo = 1f;

    public float gravityFrom = 0f;
    public float gravityTo = 0f;

    protected virtual bool CanBeDestroyed => true;
    protected virtual Color Color => Color.white;


    float _gravity;
    public virtual void Initialize(Vector3 speed, float spread, Vector3 parent_vel)
    {
        _lifetime = Random.Range(lifetimeFrom, lifetimeTo);
        _gravity = Random.Range(gravityFrom, gravityTo);

        _rb = GetComponent<Rigidbody>();

        Vector3 right = new(-speed.z, 0, speed.x);
        float angle = Random.Range(-spread / 2, spread / 2);
        _rb.linearVelocity = Mathf.Sin(angle) * right + Mathf.Cos(angle) * speed + parent_vel;

        transform.localScale *= Random.Range(sizeFrom, sizeTo);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() => MyStart();

    protected virtual void MyStart()
    {
        if (_rb == null) Initialize(Vector3.zero, 0, Vector3.zero);
    }

    void Update() => MyUpdate();

    protected virtual void MyUpdate()
    {
        _lifetime -= Time.deltaTime;
        if (_lifetime < 0 && CanBeDestroyed)
        {
            Destroy(gameObject);
        }
        _rb.AddForce(new Vector3(0, _gravity * Time.deltaTime, 0), ForceMode.VelocityChange);
    }

    protected virtual AudioClip HitSound() { return null; }

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
            ParticleSystem.EmitParams p = new()
            {
                position = collision.GetContact(0).point + collision.GetContact(0).normal * 0.02f,
                rotation3D = -Quaternion.LookRotation(collision.GetContact(0).normal).eulerAngles,
                startColor = Color
            };
            Game.I.splatterParticles.Emit(p, 1);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (HitSound() != null)
        {
            if (!SoundManager.Instance.IsDestroyed())
                SoundManager.Instance.PlaySound(HitSound(), 1f, true);
        }
        SpawnParticles(Game.I.bubblePopParticles, Color);
    }
}

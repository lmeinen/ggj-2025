using UnityEngine;

public class Shot : MonoBehaviour
{
    protected Rigidbody _rb;
    public virtual void Initialize(Vector3 speed, float spread, Vector3 parent_vel)
    {
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

    public virtual void HitSomething(Collider hit) { }
}

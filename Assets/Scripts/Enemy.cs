using UnityEngine;

public class Enemy : MonoBehaviour
{
    public VisualEffectManager VisualEffectManager;

    Player _player;

    //true if the enemy is dead (= captured by a bubble)
    bool _dead = false;
    //the bubble the enemy is in or null if alive
    Bubble _inBubble;

    //the gun of the enemy
    public Gun gun;

    public bool Dead { get => _dead; private set => _dead = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VisualEffectManager = GetComponent<VisualEffectManager>();
        VisualEffectManager.EnableBluepill();
        // _material = GetComponent<MeshRenderer>().material;
        _player = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Dead)
        {
            //try aiming at the player
            int layer_mask = LayerMask.GetMask("Player") | LayerMask.GetMask("Walls");
            Vector3 dif = _player.transform.position - transform.position;
            bool hit_anything = Physics.Raycast(transform.position, dif.normalized, out RaycastHit hit, float.PositiveInfinity, layer_mask);

            //if we can see the player, aim at him and attempt to fire
            if (hit_anything && hit.collider.CompareTag("Player"))
            {
                gun.AimAt(_player.transform.position);
                gun.ShouldFire = true;
            }
            else
            {
                gun.ShouldFire = false;
            }
        }
        else
        {
            gun.ShouldFire = false;
        }
    }

    //Called when the enemy is hit by a bubble
    void HitByBubble(Bubble b)
    {
        //is this the first bubble to hit this enemy
        if (_inBubble == null)
        {
            //change the enemy color, save the bubble, and mark enemy as dead
            VisualEffectManager.EnableRedpill();
            // _material.color = Color.green;
            _inBubble = b;
            Dead = true;
        }
        else
        {
            // enemy already in a bubble -> destroy the newly arrived bubble
            if (_inBubble == b)
                return;
            //enemy already in a bubble, just destroy it
            Destroy(b.gameObject);
        }
    }

    //Called when a something enters the enemy trigger collider
    public void OnTriggerEnter(Collider other)
    {
        //if the collider was entered by a bubble, the enemy dies and the bubble should surround it
        if (other.CompareTag("Bubble"))
        {
            Bubble b = other.GetComponent<Bubble>();
            b.HitEnemy(this);
            HitByBubble(b);
        }
    }
}

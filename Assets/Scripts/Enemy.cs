using System.Globalization;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // public VisualEffectManager VisualEffectManager;

    Player _player;

    //true if the enemy is dead (= captured by a bubble)
    bool _dead = false;
    //the bubble the enemy is in or null if alive
    Bubble _inBubble;

    //the gun of the enemy
    public Gun gun;


    public float turningRate = 0f;
    public float viewCone = Mathf.PI * 0.8f;
    public float viewDistance = 10f;

    public bool Dead { get => _dead; private set => _dead = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // VisualEffectManager = GetComponent<VisualEffectManager>();
        // VisualEffectManager.EnableBluepill();
        // _material = GetComponent<MeshRenderer>().material;
        _player = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Dead)
        {
            Vector3 dif = _player.transform.position - transform.position;
            float distance = dif.magnitude;
            dif /= distance;
            float angle = Mathf.Acos(Vector3.Dot(dif, transform.right.normalized));

            bool player_detected = (_player.gun.FiredRecently || angle < viewCone/2) && distance < viewDistance;

            //player in view cone and close enough
            if (player_detected)
            {
                //try aiming at the player
                int layer_mask = LayerMask.GetMask("Player") | LayerMask.GetMask("Walls");
                bool hit_anything = Physics.Raycast(transform.position, dif.normalized, out RaycastHit hit, float.PositiveInfinity, layer_mask);

                //if we can see the player, aim at him and attempt to fire
                if (hit_anything && hit.collider.CompareTag("Player"))
                {
                    //gun.AimAt(_player.transform.position);
                    //turn towards the player
                    float target_rot = -Mathf.Atan2(dif.z, dif.x) * Mathf.Rad2Deg;
                    float current_rot = transform.rotation.eulerAngles.y;

                    //assume both in range <-PI, PI>
                    float dist_pos = (target_rot - current_rot + 360) % 360;
                    float dist_neg = (-target_rot+ current_rot + 360) % 360;

                    float next_rot;
                    float rot_step = turningRate * Time.deltaTime;
                    if (Mathf.Min(dist_pos, dist_neg) < rot_step)
                        next_rot = target_rot;
                    else
                    {
                        if (dist_pos < dist_neg)
                        {
                            next_rot = current_rot + rot_step;
                        }
                        else
                        {
                            next_rot = current_rot - rot_step;
                        }
                    }
                    transform.rotation = Quaternion.Euler(0, next_rot, 0);

                    gun.AttemptToFire(Vector3.zero);
                }
            }
        }
    }

    //Called when the enemy is hit by a bubble
    void HitByBubble(Bubble b)
    {
        //is this the first bubble to hit this enemy
        if (_inBubble == null)
        {
            //change the enemy color, save the bubble, and mark enemy as dead
            // VisualEffectManager.EnableRedpill();
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

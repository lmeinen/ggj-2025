using System.Globalization;
using UnityEngine;
using UnityEngine.AI;

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

    NavMeshAgent _agent;

    float _lastSeenPlayer = 0f;
    float _navigationTimer = 0f;
    Vector3 _lastPlayerPosition = Vector3.zero;

    public Vector3 Center => transform.position + new Vector3(0, 1, 0);


    public float turningRate = 1f;
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
        _agent = GetComponent<NavMeshAgent>();
        _lastSeenPlayer = Time.realtimeSinceStartup;

    }

    // Update is called once per frame
    void Update()
    {
        if (!Dead)
        {
            Vector3 pos = Center;

            Vector3 dif = _player.Center - pos;
            float distance = dif.magnitude;
            dif /= distance;
            //float angle = Mathf.Acos(Vector3.Dot(dif, transform.right.normalized));

            bool player_detected = distance < viewDistance;
            bool seen_player_recently = Time.realtimeSinceStartup - _lastSeenPlayer < 1f;

            //player in view cone and close enough
            if (player_detected)
            {
                //try aiming at the player
                int layer_mask = LayerMask.GetMask("Player") | LayerMask.GetMask("Walls");
                bool hit_anything = Physics.Raycast(pos, dif.normalized, out RaycastHit hit, float.PositiveInfinity, layer_mask);
                bool can_see_player = hit_anything && hit.collider.CompareTag("Player");

                //if we can see the player, aim at him and attempt to fire
                if (can_see_player)
                {
                    _lastPlayerPosition = _player.Center;

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

                    if (_navigationTimer < 0f)
                    {
                        _agent.speed = 3f;
                        _navigationTimer = 1f;
                        _agent.SetDestination(_lastPlayerPosition);
                    }
                    _lastSeenPlayer = Time.realtimeSinceStartup;
                }
                else
                {
                    if (seen_player_recently)
                    {
                        gun.AimAt(_lastPlayerPosition);
                    }
                    else
                    {
                        gun.AimAt(transform.position + _agent.velocity);
                    }
                }
            }
            //navigation time is 0 and we haven't seen the player for a while -> go to a random position nearby
            if (_navigationTimer < 0f && !seen_player_recently)
            {
                _navigationTimer = 1f;

                _agent.speed = 1.5f;
                float move_angle = Random.Range(0, 2 * Mathf.PI);
                //move to map center
                Vector3 dir = new Vector3(Mathf.Cos(move_angle), 0, Mathf.Sin(move_angle)) * 10f;
                _agent.SetDestination(dir);
            }
            _navigationTimer -= Time.deltaTime;
        }
    }

    public void Kill() {
        Dead = true;
    }

    // Called when the enemy is hit by a bubble
    public void HitByBubble(Bubble b)
    {
        _agent.isStopped = true;
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

    private void OnDestroy() {
        // clean up bubble
        Destroy(_inBubble.gameObject);
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

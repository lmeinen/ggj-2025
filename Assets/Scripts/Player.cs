using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Tooltip("The action that moves the player (Arrow keys on PC)")]
    public InputActionReference moveAction;

    [Tooltip("The action that fires the gun (Left mouse on PC)")]
    public InputActionReference fireAction;

    [Tooltip("The dash action (Right mouse on PC)")]
    public InputActionReference dashAction;

    [Tooltip("How many units per second the player travels")]
    public float speed = 1f;

    [Tooltip("The player gun")]
    public Gun gun;

    [Tooltip("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerVisuals playerVis;


    public ParticleSystem dashTrail;

    Camera _mainCamera;
    Rigidbody _rigidbody;


    public PlayerDash dash;

    Vector3 _dashDirection;

    public Vector3 Center => transform.position + new Vector3(0, 1f, 0);

    bool _dead = false;

    void Start()
    {
        _mainCamera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        //move the player according to keyboard input
        Vector2 move_action = moveAction.action.ReadValue<Vector2>();
        Vector3 move = speed * new Vector3(move_action.x, 0, move_action.y);
        if (move.x != 0 || move.z != 0) _dashDirection = move;

        if (dashAction.action.triggered && dash.CanDash())
            dash.Dash(_dashDirection.normalized);
        


        _rigidbody.linearVelocity = move + dash.Velocity;

        if (dash.Velocity != Vector3.zero)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector4 local = new (Random.Range(-0.5f, 0.5f), Random.Range(0f, 1.7f), -0.2f, 1f);

                ParticleSystem.EmitParams ps = new()
                {
                    position = transform.localToWorldMatrix * local,
                    //velocity = -_rigidbody.linearVelocity / 5f
                };
                dashTrail.Emit(ps, 1);
            }
        }


        if (move_action.x != 0 || move_action.y != 0)
        {
            _dashDirection = move;
            if (!animator.GetBool("isMoving")) animator.SetBool("isMoving", true);
        }
        else
        {
            if (animator.GetBool("isMoving")) animator.SetBool("isMoving", false);
        }

        //obtain the point where the mouse is pointing at the ground (assumed to be an infinite plane at Y = 0)
        Ray r = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        float dist = -r.origin.y / r.direction.y;

        //if the ray from the mouse hits the ground, aim the gun at that point
        if (dist > 0)
        {
            gun.AimAt(r.GetPoint(dist));
            playerVis.AimAt(r.GetPoint(dist));
            playerVis.transform.Rotate(0,90,0);
        }
        else
        {
            Debug.LogError("Mouse is not pointing at the ground");
        }
        //gun should fire = mouse is pressed
        if (fireAction.action.ReadValue<float>() != 0f)
            gun.AttemptToFire(move);


    }


    //The player was hit by something
    public void OnTriggerEnter(Collider other)
    {
        //if he was hit by an enemy shot
        if (!_dead && other.CompareTag("EnemyShot"))
        {
            _dead = true;
            //TODO somehow kill the player & reload the level & maybe show the glitch
            //Debug.Log("Player hit by enemy shot. Skill issue.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

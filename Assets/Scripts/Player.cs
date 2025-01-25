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


    Camera _mainCamera;
    Rigidbody _rigidbody;

    public float dashCooldown = 0f;
    public float maxDashes = 2f;
    public float dashDistance = 1f;
    float _dashAmount = 0f;

    Vector2 _dashDirection;

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
        _rigidbody.linearVelocity = move;

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

        if (dashAction.action.triggered)
        {
            if (_dashAmount >= dashCooldown)
            {
                _dashAmount -= dashCooldown;
                _dashDirection = _dashDirection.normalized;
                transform.position += new Vector3(_dashDirection.x, 0, _dashDirection.y) * dashDistance;
            }
        }
        _dashAmount = Mathf.Min(maxDashes, _dashAmount + Time.deltaTime);
    }


    //The player was hit by something
    public void OnTriggerEnter(Collider other)
    {
        //if he was hit by an enemy shot
        if (other.CompareTag("EnemyShot"))
        {
            //TODO somehow kill the player & reload the level & maybe show the glitch
            Debug.Log("Player hit by enemy shot. Skill issue.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

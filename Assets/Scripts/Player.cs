using UnityEngine;
using UnityEngine.InputSystem;

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


    Camera _mainCamera;

    public float dashCooldown = 0f;
    public float maxDashes = 2f;
    public float dashDistance = 1f;
    float _dashAmount = 0f;

    Vector2 _dashDirection;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //move the player according to keyboard input
        Vector2 move = moveAction.action.ReadValue<Vector2>();
        transform.position += speed * Time.deltaTime * new Vector3(move.x, 0, move.y);

        if (move.y != 0 || move.x != 0)
            _dashDirection = move;

        //obtain the point where the mouse is pointing at the ground (assumed to be an infinite plane at Y = 0)
        Ray r = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        float dist = -r.origin.y / r.direction.y;

        //if the ray from the mouse hits the ground, aim the gun at that point
        if (dist > 0)
        {
            gun.AimAt(r.GetPoint(dist));
        }
        else
        {
            Debug.LogError("Mouse is not pointing at the ground");
        }
        //gun should fire = mouse is pressed
        gun.ShouldFire = fireAction.action.ReadValue<float>() != 0f;

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
        }
    }
}

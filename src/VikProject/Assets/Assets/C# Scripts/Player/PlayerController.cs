using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    InputHandler input;

    PlayerMovemet movement;
    PlayerRotation rotation;

    IMovementMode walkMode;
    IMovementMode runMode;
    IMovementInterrupt interrupt;
    [SerializeField] float turnSpeed = 10f;

    Transform cam;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<InputHandler>();

        cam = Camera.main.transform;

        walkMode = new Walk();
        runMode = new Run();
        IMovementInterrupt[] allInterrupts =
        GetComponents<IMovementInterrupt>();

        interrupt = new MovementInterruptAggregator(allInterrupts);
        movement = new PlayerMovemet(rb, walkMode,interrupt);
        rotation = new PlayerRotation(transform, turnSpeed,interrupt);
    }

    void Update()
    {
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 direction =
            camForward * input.moveInput.y +
            camRight * input.moveInput.x;

        movement.SetDirection(direction);
        rotation.Rotate(direction);

        HandleMovementMode();
        Debug.Log("interup status :"+interrupt.IsMovementInterrupt);
    }

    void FixedUpdate()
    {
        movement.Move();
    }

    void HandleMovementMode()
    {
        movement.SetMode(input.isRun ? runMode : walkMode);
    }
}
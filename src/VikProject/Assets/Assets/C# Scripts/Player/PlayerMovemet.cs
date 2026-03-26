using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

public class PlayerMovemet : IMovement
{
    private readonly Rigidbody rb;
    private readonly IMovementInterrupt interrupt;
    private IMovementMode currentMode;
    private Vector3 moveDirection;

    public PlayerMovemet(
        Rigidbody rb,
        IMovementMode defaultMode,
        IMovementInterrupt interrupt)
    {
        this.rb = rb;
        this.currentMode = defaultMode;
        this.interrupt = interrupt;
    }

    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction;
    }
    public void SetMode(IMovementMode mode)
    {
        currentMode = mode;
    }
    public void Move()
    {
        if (interrupt.IsMovementInterrupt)
        {
            
            return;
        }

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        Vector3 targetVelocity = moveDirection * currentMode.MaxSpeed;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            horizontalVelocity = Vector3.Lerp(
                horizontalVelocity,
                targetVelocity,
                currentMode.Acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            horizontalVelocity = Vector3.Lerp(
                horizontalVelocity,
                Vector3.zero,
                currentMode.Deceleration * Time.fixedDeltaTime
            );
        }

        // ⭐ ใส่กลับเข้า Rigidbody (อย่าลืมแกน Y)
        rb.linearVelocity = new Vector3(
            horizontalVelocity.x,
            rb.linearVelocity.y,
            horizontalVelocity.z
        );
    }
}
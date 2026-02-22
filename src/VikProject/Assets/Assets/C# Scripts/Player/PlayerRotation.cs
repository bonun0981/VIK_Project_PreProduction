using UnityEngine;

public class PlayerRotation
{
    private Transform player;
    private float turnSpeed;
    private readonly IMovementInterrupt interrupt;
    public PlayerRotation(Transform player, float turnSpeed, IMovementInterrupt interrupt)
    {
        this.player = player;
        this.turnSpeed = turnSpeed;
        this.interrupt = interrupt;
    }

    public void Rotate(Vector3 direction)
    {
        if (interrupt.IsMovementInterrupt)
        {

            return;
        }
        if (direction.sqrMagnitude < 0.01f)
            return;

        // สร้าง rotation เป้าหมายจาก direction
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // หมุนเฉพาะแกน Y (ป้องกันก้ม/เงย)
        Vector3 euler = targetRotation.eulerAngles;
        Quaternion yRotation = Quaternion.Euler(0f, euler.y, 0f);

        // Smooth rotate
        player.rotation = Quaternion.Slerp(
            player.rotation,
            yRotation,
            turnSpeed * Time.deltaTime
        );
    }


}
